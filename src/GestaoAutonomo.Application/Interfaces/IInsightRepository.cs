using GestaoAutonomo.Domain.Entities;

namespace GestaoAutonomo.Application.Interfaces;

public interface IInsightRepository
{
    Task<IReadOnlyList<InsightIA>> ObterVigentesAsync(Guid usuarioId, DateTime agora, CancellationToken ct);
    Task SubstituirAsync(Guid usuarioId, IReadOnlyList<InsightIA> novos, CancellationToken ct);
}
