using GestaoAutonomo.Domain.Entities;

namespace GestaoAutonomo.Application.Interfaces;

public interface IMetaRepository
{
    Task<Meta?> ObterPorIdAsync(Guid usuarioId, Guid id, CancellationToken ct);
    Task<IReadOnlyList<Meta>> ListarAsync(Guid usuarioId, CancellationToken ct);
    Task AdicionarAsync(Meta meta, CancellationToken ct);
    void Remover(Meta meta);
    Task SalvarAlteracoesAsync(CancellationToken ct);
}
