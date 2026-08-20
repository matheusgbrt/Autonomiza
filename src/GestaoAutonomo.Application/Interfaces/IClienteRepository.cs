using GestaoAutonomo.Domain.Entities;

namespace GestaoAutonomo.Application.Interfaces;

public interface IClienteRepository
{
    Task<Cliente?> ObterPorIdAsync(Guid usuarioId, Guid id, CancellationToken ct);
    Task<IReadOnlyList<Cliente>> ListarAsync(Guid usuarioId, CancellationToken ct);
    Task AdicionarAsync(Cliente cliente, CancellationToken ct);
    void Remover(Cliente cliente);
    Task SalvarAlteracoesAsync(CancellationToken ct);
}
