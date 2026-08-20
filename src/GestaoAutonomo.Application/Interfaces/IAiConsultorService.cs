using GestaoAutonomo.Application.DTOs.Insight;

namespace GestaoAutonomo.Application.Interfaces;

public interface IAiConsultorService
{
    Task<InsightsResponseDto> ObterInsightsAsync(Guid usuarioId, CancellationToken ct);
}
