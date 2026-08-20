using GestaoAutonomo.Domain.Entities;

namespace GestaoAutonomo.Application.Interfaces;

public interface ITarefaRepository
{
    Task<Tarefa?> ObterPorIdAsync(Guid usuarioId, Guid id, CancellationToken ct);
    Task<IReadOnlyList<Tarefa>> ListarAsync(Guid usuarioId, CancellationToken ct);
    Task AdicionarAsync(Tarefa tarefa, CancellationToken ct);
    void Remover(Tarefa tarefa);
    Task SalvarAlteracoesAsync(CancellationToken ct);
}
