using GestaoAutonomo.Application.Common;
using GestaoAutonomo.Application.Interfaces;
using GestaoAutonomo.Domain.Enums;

namespace GestaoAutonomo.Application.Services;

public class LembreteAgendamentoService : ILembreteAgendamentoService
{
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IWhatsAppSender _whatsAppSender;

    public LembreteAgendamentoService(
        IAgendamentoRepository agendamentoRepository,
        IUsuarioRepository usuarioRepository,
        IClienteRepository clienteRepository,
        IWhatsAppSender whatsAppSender)
    {
        _agendamentoRepository = agendamentoRepository;
        _usuarioRepository = usuarioRepository;
        _clienteRepository = clienteRepository;
        _whatsAppSender = whatsAppSender;
    }

    public async Task EnviarLembretesPendentesAsync(CancellationToken ct)
    {
        var agora = DateTime.UtcNow;
        var janelaInicio = agora.AddHours(23);
        var janelaFim = agora.AddHours(25);

        var pendentes = await _agendamentoRepository.ListarPendentesDeLembreteAsync(janelaInicio, janelaFim, ct);
        if (pendentes.Count == 0) return;

        var enviouAlgum = false;

        foreach (var agendamento in pendentes)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(agendamento.UsuarioId, ct);
            if (usuario is null || usuario.Plano != Plano.Pro ||
                string.IsNullOrWhiteSpace(usuario.ZApiInstanceId) || string.IsNullOrWhiteSpace(usuario.ZApiToken))
            {
                continue;
            }

            var cliente = await _clienteRepository.ObterPorIdAsync(usuario.Id, agendamento.ClienteId, ct);
            if (cliente is null || string.IsNullOrWhiteSpace(cliente.Telefone))
            {
                continue;
            }

            var mensagem = $"Lembrete: você tem um agendamento amanhã, {agendamento.DataHoraInicio:dd/MM 'às' HH:mm}.";

            await _whatsAppSender.EnviarBotoesAsync(
                usuario.ZApiInstanceId, usuario.ZApiToken, usuario.ZApiClientToken, cliente.Telefone,
                mensagem, BotoesAgendamento.ConfirmarCancelar, ct);

            agendamento.LembreteEnviado = true;
            enviouAlgum = true;
        }

        if (enviouAlgum)
        {
            await _agendamentoRepository.SalvarAlteracoesAsync(ct);
        }
    }
}
