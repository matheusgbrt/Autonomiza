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
    private readonly IMensagemWhatsAppRepository _mensagemRepository;

    public WhatsAppWebhookProcessor(
        IUsuarioRepository usuarioRepository,
        IClienteRepository clienteRepository,
        IServicoRepository servicoRepository,
        IAgendamentoRepository agendamentoRepository,
        IAgendamentoService agendamentoService,
        IWhatsAppSender whatsAppSender,
        IMensagemWhatsAppRepository mensagemRepository)
    {
        _usuarioRepository = usuarioRepository;
        _clienteRepository = clienteRepository;
        _servicoRepository = servicoRepository;
        _agendamentoRepository = agendamentoRepository;
        _agendamentoService = agendamentoService;
        _whatsAppSender = whatsAppSender;
        _mensagemRepository = mensagemRepository;
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
        if (!usuario.WhatsAppRespostasAutomaticasAtivas) return;

        var cliente = await EncontrarClientePorTelefoneAsync(usuario.Id, telefoneRemetente, ct);
        if (cliente is null) return;

        var primeiraConversa = !await _mensagemRepository.ExisteConversaAnteriorAsync(usuario.Id, telefoneRemetente, ct);

        await LogarMensagemAsync(usuario.Id, cliente.Id, telefoneRemetente, DirecaoMensagemWhatsApp.Recebida,
            botaoSelecionadoId ?? mensagemTexto ?? string.Empty, ct);

        if (primeiraConversa && !string.IsNullOrWhiteSpace(usuario.WhatsAppMensagemBoasVindas))
        {
            await EnviarELogarTextoAsync(usuario, cliente.Id, telefoneRemetente, usuario.WhatsAppMensagemBoasVindas!, ct);
        }

        var agendamentoPendente = await _agendamentoRepository.ObterProximoPendenteAsync(usuario.Id, cliente.Id, DateTime.UtcNow, ct);

        if (agendamentoPendente is not null)
        {
            await ProcessarConfirmacaoOuCancelamentoAsync(usuario, cliente, agendamentoPendente, mensagemTexto, botaoSelecionadoId, telefoneRemetente, ct);
            return;
        }

        await ProcessarFluxoDeAgendamentoAsync(usuario, cliente, botaoSelecionadoId, telefoneRemetente, ct);
    }

    private async Task ProcessarConfirmacaoOuCancelamentoAsync(
        Usuario usuario, Cliente cliente, Agendamento agendamento, string? mensagemTexto, string? botaoSelecionadoId, string telefoneRemetente, CancellationToken ct)
    {
        var comando = InterpretarComando(botaoSelecionadoId, mensagemTexto);

        if (comando == ComandoAgendamento.Confirmar && agendamento.Status == StatusAgendamento.Agendado)
        {
            agendamento.Status = StatusAgendamento.Confirmado;
            await _agendamentoRepository.SalvarAlteracoesAsync(ct);

            await EnviarELogarTextoAsync(usuario, cliente.Id, telefoneRemetente,
                $"Agendamento confirmado para {agendamento.DataHoraInicio:dd/MM 'às' HH:mm}. Até breve!", ct);
            return;
        }

        if (comando == ComandoAgendamento.Cancelar &&
            (agendamento.Status == StatusAgendamento.Agendado || agendamento.Status == StatusAgendamento.Confirmado))
        {
            agendamento.Status = StatusAgendamento.Cancelado;
            await _agendamentoRepository.SalvarAlteracoesAsync(ct);

            await EnviarELogarTextoAsync(usuario, cliente.Id, telefoneRemetente,
                $"Seu agendamento de {agendamento.DataHoraInicio:dd/MM 'às' HH:mm} foi cancelado.", ct);
            return;
        }

        await EnviarELogarBotoesAsync(usuario, cliente.Id, telefoneRemetente,
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
                    if (!usuario.WhatsAppHorariosDisponiveisAtivo)
                    {
                        await EnviarELogarTextoAsync(usuario, cliente.Id, telefoneRemetente,
                            $"Para agendar \"{servico.Nome}\", entre em contato diretamente com a gente. Agendamento automático está temporariamente indisponível.", ct);
                        return;
                    }

                    await OfertarHorariosAsync(usuario, cliente, servico, telefoneRemetente, ct);
                    return;
                }
            }
        }

        // Passo 1: nada reconhecido -> oferece a lista de serviços
        await OfertarServicosAsync(usuario, cliente, telefoneRemetente, ct);
    }

    private async Task OfertarServicosAsync(Usuario usuario, Cliente cliente, string telefoneRemetente, CancellationToken ct)
    {
        var servicos = await _servicoRepository.ListarAsync(usuario.Id, ct);
        if (servicos.Count == 0) return;

        var opcoes = servicos
            .Select(s => new OpcaoWhatsApp(s.Id.ToString(), s.Nome, $"{s.ValorPadrao:C2} · {s.Duracao:hh\\:mm}"))
            .ToList();

        await EnviarELogarListaAsync(usuario, cliente.Id, telefoneRemetente,
            "Qual serviço você gostaria de agendar?", "Serviços disponíveis", "Escolher", opcoes, ct);
    }

    private async Task OfertarHorariosAsync(Usuario usuario, Cliente cliente, Servico servico, string telefoneRemetente, CancellationToken ct)
    {
        var slots = await GerarSlotsDisponiveisAsync(usuario.Id, servico, ct);

        if (slots.Count == 0)
        {
            await EnviarELogarTextoAsync(usuario, cliente.Id, telefoneRemetente,
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

        await EnviarELogarListaAsync(usuario, cliente.Id, telefoneRemetente,
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
            var dto = new CriarAgendamentoDto(cliente.Id, servicoId, dataHoraInicio, IntegracaoWhatsAppService.ObservacaoAgendamentoViaWhatsApp);
            var agendamento = await _agendamentoService.CriarAsync(usuario.Id, dto, ct);

            if (!usuario.WhatsAppConfirmarAgendamentosAtivo)
            {
                var entidade = await _agendamentoRepository.ObterPorIdAsync(usuario.Id, agendamento.Id, ct);
                if (entidade is not null)
                {
                    entidade.Status = StatusAgendamento.Confirmado;
                    await _agendamentoRepository.SalvarAlteracoesAsync(ct);
                }
            }

            await EnviarELogarTextoAsync(usuario, cliente.Id, telefoneRemetente,
                $"Agendado! {agendamento.ServicoNome} em {agendamento.DataHoraInicio:dd/MM 'às' HH:mm}. Até breve!", ct);
        }
        catch (AgendamentoConflitanteException)
        {
            await EnviarELogarTextoAsync(usuario, cliente.Id, telefoneRemetente,
                "Esse horário acabou de ser preenchido. Vamos tentar de novo? Responda qualquer coisa para ver os serviços.", ct);
        }
        catch (RecursoNaoEncontradoException)
        {
            await EnviarELogarTextoAsync(usuario, cliente.Id, telefoneRemetente,
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

    private async Task LogarMensagemAsync(Guid usuarioId, Guid? clienteId, string telefone, DirecaoMensagemWhatsApp direcao, string conteudo, CancellationToken ct)
    {
        await _mensagemRepository.AdicionarAsync(new MensagemWhatsApp
        {
            UsuarioId = usuarioId,
            ClienteId = clienteId,
            Telefone = telefone,
            Direcao = direcao,
            Conteudo = conteudo,
        }, ct);
        await _mensagemRepository.SalvarAlteracoesAsync(ct);
    }

    private async Task EnviarELogarTextoAsync(Usuario usuario, Guid clienteId, string telefoneDestino, string mensagem, CancellationToken ct)
    {
        await _whatsAppSender.EnviarTextoAsync(usuario.ZApiInstanceId!, usuario.ZApiToken!, usuario.ZApiClientToken, telefoneDestino, mensagem, ct);
        await LogarMensagemAsync(usuario.Id, clienteId, telefoneDestino, DirecaoMensagemWhatsApp.Enviada, mensagem, ct);
    }

    private async Task EnviarELogarBotoesAsync(
        Usuario usuario, Guid clienteId, string telefoneDestino, string mensagem, IReadOnlyList<BotaoWhatsApp> botoes, CancellationToken ct)
    {
        await _whatsAppSender.EnviarBotoesAsync(usuario.ZApiInstanceId!, usuario.ZApiToken!, usuario.ZApiClientToken, telefoneDestino, mensagem, botoes, ct);
        await LogarMensagemAsync(usuario.Id, clienteId, telefoneDestino, DirecaoMensagemWhatsApp.Enviada, mensagem, ct);
    }

    private async Task EnviarELogarListaAsync(
        Usuario usuario, Guid clienteId, string telefoneDestino, string mensagem, string titulo, string botaoLabel,
        IReadOnlyList<OpcaoWhatsApp> opcoes, CancellationToken ct)
    {
        await _whatsAppSender.EnviarListaOpcoesAsync(usuario.ZApiInstanceId!, usuario.ZApiToken!, usuario.ZApiClientToken, telefoneDestino, mensagem, titulo, botaoLabel, opcoes, ct);
        await LogarMensagemAsync(usuario.Id, clienteId, telefoneDestino, DirecaoMensagemWhatsApp.Enviada, mensagem, ct);
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
