using GestaoAutonomo.Application.DTOs.Integracao;

namespace GestaoAutonomo.Application.Interfaces;

public interface IIntegracaoWhatsAppService
{
    Task ConfigurarAsync(Guid usuarioId, ConfigurarWhatsAppDto dto, CancellationToken ct);
    Task<StatusIntegracaoWhatsAppDto> ObterStatusAsync(Guid usuarioId, CancellationToken ct);
}
