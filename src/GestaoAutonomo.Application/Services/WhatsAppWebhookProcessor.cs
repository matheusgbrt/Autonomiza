using System.Globalization;
using System.Text.RegularExpressions;
using GestaoAutonomo.Application.Common;
using GestaoAutonomo.Application.DTOs.Agendamento;
using GestaoAutonomo.Application.Exceptions;
using GestaoAutonomo.Application.Interfaces;
using GestaoAutonomo.Domain.Entities;
using GestaoAutonomo.Domain.Enums;

namespace GestaoAutonomo.Application.Services;

public class WhatsAppWebhookProcessor : IWhatsAppWebhookProcessor
{
    private const int JanelaComercialInicioHora = 9;
    private const int JanelaComercialFimHora = 18;
    private const int DiasParaOfertar = 5;
    private const int MaximoSlotsOfertados = 6;

    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IServicoRepository _servicoRepository;
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly IAgendamentoService _agendamentoService;
    private readonly IWhatsAppSender _whatsAppSender;

    public WhatsAppWebhookProcessor(
        IUsuarioRepository usuarioRepository,
        IClienteRepository clienteRepository,
        IServicoRepository servicoRepository,
        IAgendamentoRepository agendamentoRepository,
        IAgendamentoService agendamentoService,
        IWhatsAppSender whatsAppSender)
    {
        _usuarioRepository = usuarioRepository;
        _clienteRepository = clienteRepository;
        _servicoRepository = servicoRepository;
        _agendamentoRepository = agendamentoRepository;
        _agendamentoService = agendamentoService;
        _whatsAppSender = whatsAppSender;
    }

    public async Task ProcessarMensagemRecebidaAsync(
        string instanceId,
        string telefoneRemetente,
        string? mensagemTexto,
        string? botaoSelecionadoId,
        bool fromMe,
        CancellationToken ct)
    {
        if (fromMe) return;

        var usuario = await _usuarioRepository.ObterPorZApiInstanceIdAsync(instanceId, ct);
        if (usuario is null || string.IsNullOrWhiteSpace(usuario.ZApiToken)) return;

        var cliente = await EncontrarClientePorTelefoneAsync(usuario.Id, telefoneRemetente, ct);
        if (cliente is null) return;

        var agendamentoPendente = await _agendamentoRepository.ObterProximoPendenteAsync(usuario.Id, cliente.Id, DateTime.UtcNow, ct);

        if (agendamentoPendente is not null)
        {
            await ProcessarConfirmacaoOuCancelamentoAsync(usuario, agendamentoPendente, mensagemTexto, botaoSelecionadoId, telefoneRemetente, ct);
            return;
        }

        await ProcessarFluxoDeAgendamentoAsync(usuario, cliente, botaoSelecionadoId, telefoneRemetente, ct);
    }

    private async Task ProcessarConfirmacaoOuCancelamentoAsync(
        Usuario usuario, Agendamento agendamento, string? mensagemTexto, string? botaoSelecionadoId, string telefoneRemetente, CancellationToken ct)
    {
        var comando = InterpretarComando(botaoSelecionadoId, mensagemTexto);

        if (comando == ComandoAgendamento.Confirmar && agendamento.Status == StatusAgendamento.Agendado)
        {
            agendamento.Status = StatusAgendamento.Confirmado;
            await _agendamentoRepository.SalvarAlteracoesAsync(ct);

            await _whatsAppSender.EnviarTextoAsync(
                usuario.ZApiInstanceId!, usuario.ZApiToken!, usuario.ZApiClientToken, telefoneRemetente,
                $"Agendamento confirmado para {agendamento.DataHoraInicio:dd/MM 'às' HH:mm}. Até breve!", ct);
            return;
        }

        if (comando == ComandoAgendamento.Cancelar &&
            (agendamento.Status == StatusAgendamento.Agendado || agendamento.Status == StatusAgendamento.Confirmado))
        {
            agendamento.Status = StatusAgendamento.Cancelado;
            await _agendamentoRepository.SalvarAlteracoesAsync(ct);

            await _whatsAppSender.EnviarTextoAsync(
                usuario.ZApiInstanceId!, usuario.ZApiToken!, usuario.ZApiClientToken, telefoneRemetente,
                $"Seu agendamento de {agendamento.DataHoraInicio:dd/MM 'às' HH:mm} foi cancelado.", ct);
            return;
        }

        await _whatsAppSender.EnviarBotoesAsync(
            usuario.ZApiInstanceId!, usuario.ZApiToken!, usuario.ZApiClientToken, telefoneRemetente,
            $"Você tem um agendamento em {agendamento.DataHoraInicio:dd/MM 'às' HH:mm}. Confirmar ou cancelar?",
            BotoesAgendamento.ConfirmarCancelar, ct);
    }

    private async Task ProcessarFluxoDeAgendamentoAsync(Usuario usuario, Cliente cliente, string? botaoSelecionadoId, string telefoneRemetente, CancellationToken ct)
    {
        // Passo 3: seleção de horário -> id no formato "{servicoId}_{dataHoraIsoUtc}"
        if (!string.IsNullOrWhiteSpace(botaoSelecionadoId))
        {
            var partes = botaoSelecionadoId.Split('_', 2);
            if (partes.Length == 2 &&
                Guid.TryParse(partes[0], out var servicoIdSlot) &&
                DateTime.TryParse(partes[1], CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var dataHoraSlot))
            {
                await CriarAgendamentoViaBookingAsync(usuario, cliente, servicoIdSlot, dataHoraSlot, telefoneRemetente, ct);
                return;
            }

            // Passo 2: seleção de serviço -> id é o Guid do Servico
            if (Guid.TryParse(botaoSelecionadoId, out var servicoIdEscolhido))
            {
                var servico = await _servicoRepository.ObterPorIdAsync(usuario.Id, servicoIdEscolhido, ct);
                if (servico is not null)
                {
                    await OfertarHorariosAsync(usuario, servico, telefoneRemetente, ct);
                    return;
                }
            }
        }

        // Passo 1: nada reconhecido -> oferece a lista de serviços
        await OfertarServicosAsync(usuario, telefoneRemetente, ct);
    }

    private async Task OfertarServicosAsync(Usuario usuario, string telefoneRemetente, CancellationToken ct)
    {
        var servicos = await _servicoRepository.ListarAsync(usuario.Id, ct);
        if (servicos.Count == 0) return;

        var opcoes = servicos
            .Select(s => new OpcaoWhatsApp(s.Id.ToString(), s.Nome, $"{s.ValorPadrao:C2} · {s.Duracao:hh\\:mm}"))
            .ToList();

        await _whatsAppSender.EnviarListaOpcoesAsync(
            usuario.ZApiInstanceId!, usuario.ZApiToken!, usuario.ZApiClientToken, telefoneRemetente,
            "Qual serviço você gostaria de agendar?", "Serviços disponíveis", "Escolher", opcoes, ct);
    }

    private async Task OfertarHorariosAsync(Usuario usuario, Servico servico, string telefoneRemetente, CancellationToken ct)
    {
        var slots = await GerarSlotsDisponiveisAsync(usuario.Id, servico, ct);

        if (slots.Count == 0)
        {
            await _whatsAppSender.EnviarTextoAsync(
                usuario.ZApiInstanceId!, usuario.ZApiToken!, usuario.ZApiClientToken, telefoneRemetente,
                "Não encontramos horários livres nos próximos dias. Entre em contato diretamente para agendar.", ct);
            return;
        }

        var cultura = CultureInfo.GetCultureInfo("pt-BR");
        var opcoes = slots
            .Select(s => new OpcaoWhatsApp(
                $"{servico.Id}_{s:yyyy-MM-ddTHH:mm:ss}Z",
                s.ToString("dd/MM 'às' HH:mm", cultura),
                null))
            .ToList();

        await _whatsAppSender.EnviarListaOpcoesAsync(
            usuario.ZApiInstanceId!, usuario.ZApiToken!, usuario.ZApiClientToken, telefoneRemetente,
            $"Horários disponíveis para \"{servico.Nome}\":", "Horários", "Escolher", opcoes, ct);
    }

    private async Task<List<DateTime>> GerarSlotsDisponiveisAsync(Guid usuarioId, Servico servico, CancellationToken ct)
    {
        var slots = new List<DateTime>();
        var hoje = DateTime.UtcNow.Date;

        for (var dia = 1; dia <= DiasParaOfertar && slots.Count < MaximoSlotsOfertados; dia++)
        {
            var data = hoje.AddDays(dia);
            var horarioAtual = data.AddHours(JanelaComercialInicioHora);
            var horarioLimite = data.AddHours(JanelaComercialFimHora);

            while (horarioAtual.Add(servico.Duracao) <= horarioLimite && slots.Count < MaximoSlotsOfertados)
            {
                var fim = horarioAtual.Add(servico.Duracao);
                if (!await _agendamentoRepository.ExisteConflitoAsync(usuarioId, horarioAtual, fim, null, ct))
                {
                    slots.Add(horarioAtual);
                }
                horarioAtual = fim;
            }
        }

        return slots;
    }

    private async Task CriarAgendamentoViaBookingAsync(
        Usuario usuario, Cliente cliente, Guid servicoId, DateTime dataHoraInicio, string telefoneRemetente, CancellationToken ct)
    {
        try
        {
            var dto = new CriarAgendamentoDto(cliente.Id, servicoId, dataHoraInicio, "Agendado via WhatsApp");
            var agendamento = await _agendamentoService.CriarAsync(usuario.Id, dto, ct);

            await _whatsAppSender.EnviarTextoAsync(
                usuario.ZApiInstanceId!, usuario.ZApiToken!, usuario.ZApiClientToken, telefoneRemetente,
                $"Agendado! {agendamento.ServicoNome} em {agendamento.DataHoraInicio:dd/MM 'às' HH:mm}. Até breve!", ct);
        }
        catch (AgendamentoConflitanteException)
        {
            await _whatsAppSender.EnviarTextoAsync(
                usuario.ZApiInstanceId!, usuario.ZApiToken!, usuario.ZApiClientToken, telefoneRemetente,
                "Esse horário acabou de ser preenchido. Vamos tentar de novo? Responda qualquer coisa para ver os serviços.", ct);
        }
        catch (RecursoNaoEncontradoException)
        {
            await _whatsAppSender.EnviarTextoAsync(
                usuario.ZApiInstanceId!, usuario.ZApiToken!, usuario.ZApiClientToken, telefoneRemetente,
                "Não encontramos esse serviço. Responda qualquer coisa para ver os serviços disponíveis.", ct);
        }
    }

    private async Task<Cliente?> EncontrarClientePorTelefoneAsync(Guid usuarioId, string telefoneRemetente, CancellationToken ct)
    {
        var digitosRemetente = SomenteDigitos(telefoneRemetente);
        var clientes = await _clienteRepository.ListarAsync(usuarioId, ct);

        return clientes.FirstOrDefault(c =>
        {
            if (string.IsNullOrWhiteSpace(c.Telefone)) return false;
            var digitosCliente = SomenteDigitos(c.Telefone);
            if (digitosCliente.Length < 8 || digitosRemetente.Length < 8) return false;

            var sufixoCliente = digitosCliente[^Math.Min(8, digitosCliente.Length)..];
            var sufixoRemetente = digitosRemetente[^Math.Min(8, digitosRemetente.Length)..];
            return sufixoCliente == sufixoRemetente;
        });
    }

    private static string SomenteDigitos(string valor) => Regex.Replace(valor, "[^0-9]", "");

    private static ComandoAgendamento InterpretarComando(string? botaoSelecionadoId, string? mensagemTexto)
    {
        if (!string.IsNullOrWhiteSpace(botaoSelecionadoId))
        {
            return botaoSelecionadoId.Trim() switch
            {
                "1" => ComandoAgendamento.Confirmar,
                "2" => ComandoAgendamento.Cancelar,
                _ => ComandoAgendamento.Nenhum
            };
        }

        if (string.IsNullOrWhiteSpace(mensagemTexto)) return ComandoAgendamento.Nenhum;

        var texto = mensagemTexto.Trim().ToLowerInvariant();

        return texto switch
        {
            "1" or "confirmar" or "confirmo" or "sim" => ComandoAgendamento.Confirmar,
            "2" or "cancelar" or "cancelo" or "não" or "nao" => ComandoAgendamento.Cancelar,
            _ => ComandoAgendamento.Nenhum
        };
    }

    private enum ComandoAgendamento
    {
        Nenhum,
        Confirmar,
        Cancelar
    }
}
