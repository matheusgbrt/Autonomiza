using GestaoAutonomo.Domain.Entities;

namespace GestaoAutonomo.Application.Interfaces;

public interface IServicoRepository
{
    Task<Servico?> ObterPorIdAsync(Guid usuarioId, Guid id, CancellationToken ct);
    Task<IReadOnlyList<Servico>> ListarAsync(Guid usuarioId, CancellationToken ct);
    Task AdicionarAsync(Servico servico, CancellationToken ct);
    void Remover(Servico servico);
    Task SalvarAlteracoesAsync(CancellationToken ct);
}
