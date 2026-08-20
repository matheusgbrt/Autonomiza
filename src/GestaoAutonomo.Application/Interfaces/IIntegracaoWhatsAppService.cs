using GestaoAutonomo.Application.DTOs.Integracao;

namespace GestaoAutonomo.Application.Interfaces;

public interface IIntegracaoWhatsAppService
{
    Task ConfigurarAsync(Guid usuarioId, ConfigurarWhatsAppDto dto, CancellationToken ct);
    Task<StatusIntegracaoWhatsAppDto> ObterStatusAsync(Guid usuarioId, CancellationToken ct);
    Task<EstatisticasWhatsAppDto> ObterEstatisticasAsync(Guid usuarioId, CancellationToken ct);
    Task<IReadOnlyList<MensagemWhatsAppDto>> ObterUltimaConversaAsync(Guid usuarioId, CancellationToken ct);
    Task<ConfiguracaoWhatsAppDto> ObterConfiguracaoAsync(Guid usuarioId, CancellationToken ct);
    Task AtualizarConfiguracaoAsync(Guid usuarioId, AtualizarConfiguracaoWhatsAppDto dto, CancellationToken ct);
}
