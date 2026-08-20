using GestaoAutonomo.Domain.Entities;

namespace GestaoAutonomo.Application.Interfaces;

public interface IItemChecklistRepository
{
    Task<ItemChecklist?> ObterPorIdAsync(Guid tarefaId, Guid id, CancellationToken ct);
    Task<IReadOnlyList<ItemChecklist>> ListarPorTarefaAsync(Guid tarefaId, CancellationToken ct);
    Task<IReadOnlyList<ItemChecklist>> ListarPorTarefasAsync(IReadOnlyList<Guid> tarefaIds, CancellationToken ct);
    Task<int> ContarPorTarefaAsync(Guid tarefaId, CancellationToken ct);
    Task AdicionarAsync(ItemChecklist item, CancellationToken ct);
    void Remover(ItemChecklist item);
    Task SalvarAlteracoesAsync(CancellationToken ct);
}
