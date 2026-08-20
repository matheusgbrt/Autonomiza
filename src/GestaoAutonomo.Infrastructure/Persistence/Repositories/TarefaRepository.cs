using GestaoAutonomo.Application.Interfaces;
using GestaoAutonomo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestaoAutonomo.Infrastructure.Persistence.Repositories;

public class TarefaRepository : ITarefaRepository
{
    private readonly AppDbContext _context;

    public TarefaRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Tarefa?> ObterPorIdAsync(Guid usuarioId, Guid id, CancellationToken ct) =>
        _context.Tarefas.FirstOrDefaultAsync(t => t.UsuarioId == usuarioId && t.Id == id, ct);

    public async Task<IReadOnlyList<Tarefa>> ListarAsync(Guid usuarioId, CancellationToken ct) =>
        await _context.Tarefas
            .Where(t => t.UsuarioId == usuarioId)
            .OrderBy(t => t.Concluida)
            .ThenBy(t => t.DataVencimento)
            .ToListAsync(ct);

    public async Task AdicionarAsync(Tarefa tarefa, CancellationToken ct) =>
        await _context.Tarefas.AddAsync(tarefa, ct);

    public void Remover(Tarefa tarefa) => _context.Tarefas.Remove(tarefa);

    public Task SalvarAlteracoesAsync(CancellationToken ct) => _context.SaveChangesAsync(ct);
}
