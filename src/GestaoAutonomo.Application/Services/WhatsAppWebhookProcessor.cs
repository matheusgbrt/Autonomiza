using System.Text.RegularExpressions;
using GestaoAutonomo.Application.Common;
using GestaoAutonomo.Application.Interfaces;
using GestaoAutonomo.Domain.Enums;

namespace GestaoAutonomo.Application.Services;

public class WhatsAppWebhookProcessor : IWhatsAppWebhookProcessor
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly IWhatsAppSender _whatsAppSender;

    public WhatsAppWebhookProcessor(
        IUsuarioRepository usuarioRepository,
        IClienteRepository clienteRepository,
        IAgendamentoRepository agendamentoRepository,
        IWhatsAppSender whatsAppSender)
    {
        _usuarioRepository = usuarioRepository;
        _clienteRepository = clienteRepository;
        _agendamentoRepository = agendamentoRepository;
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

        var agendamento = await _agendamentoRepository.ObterProximoPendenteAsync(usuario.Id, cliente.Id, DateTime.UtcNow, ct);
        if (agendamento is null) return;

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

    private async Task<Domain.Entities.Cliente?> EncontrarClientePorTelefoneAsync(Guid usuarioId, string telefoneRemetente, CancellationToken ct)
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
