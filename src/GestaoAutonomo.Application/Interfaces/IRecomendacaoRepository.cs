using GestaoAutonomo.Domain.Entities;

namespace GestaoAutonomo.Application.Interfaces;

public interface IRecomendacaoRepository
{
    Task<IReadOnlyList<RecomendacaoIA>> ObterVigentesAsync(Guid usuarioId, DateTime agora, CancellationToken ct);
    Task SubstituirAsync(Guid usuarioId, IReadOnlyList<RecomendacaoIA> novas, CancellationToken ct);
}
