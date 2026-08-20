using GestaoAutonomo.Application.DTOs.Dashboard;

namespace GestaoAutonomo.Application.Interfaces;

public interface IDashboardService
{
    Task<ResumoDashboardDto> ObterResumoAsync(Guid usuarioId, CancellationToken ct);
    Task<DashboardAvancadoDto> ObterAvancadoAsync(Guid usuarioId, DateOnly? inicio, DateOnly? fim, CancellationToken ct);
}
