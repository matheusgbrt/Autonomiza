using GestaoAutonomo.Application.Interfaces;
using GestaoAutonomo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestaoAutonomo.Infrastructure.Persistence.Repositories;

public class ItemChecklistRepository : IItemChecklistRepository
{
    private readonly AppDbContext _context;

    public ItemChecklistRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<ItemChecklist?> ObterPorIdAsync(Guid tarefaId, Guid id, CancellationToken ct) =>
        _context.ItensChecklist.FirstOrDefaultAsync(i => i.TarefaId == tarefaId && i.Id == id, ct);

    public async Task<IReadOnlyList<ItemChecklist>> ListarPorTarefaAsync(Guid tarefaId, CancellationToken ct) =>
        await _context.ItensChecklist
            .Where(i => i.TarefaId == tarefaId)
            .OrderBy(i => i.Ordem)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<ItemChecklist>> ListarPorTarefasAsync(IReadOnlyList<Guid> tarefaIds, CancellationToken ct) =>
        await _context.ItensChecklist
            .Where(i => tarefaIds.Contains(i.TarefaId))
            .OrderBy(i => i.Ordem)
            .ToListAsync(ct);

    public Task<int> ContarPorTarefaAsync(Guid tarefaId, CancellationToken ct) =>
        _context.ItensChecklist.CountAsync(i => i.TarefaId == tarefaId, ct);

    public async Task AdicionarAsync(ItemChecklist item, CancellationToken ct) =>
        await _context.ItensChecklist.AddAsync(item, ct);

    public void Remover(ItemChecklist item) => _context.ItensChecklist.Remove(item);

    public Task SalvarAlteracoesAsync(CancellationToken ct) => _context.SaveChangesAsync(ct);
}
