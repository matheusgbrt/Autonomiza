using GestaoAutonomo.Application.Interfaces;
using GestaoAutonomo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestaoAutonomo.Infrastructure.Persistence.Repositories;

public class LancamentoFinanceiroRepository : ILancamentoFinanceiroRepository
{
    private readonly AppDbContext _context;

    public LancamentoFinanceiroRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<LancamentoFinanceiro?> ObterPorIdAsync(Guid usuarioId, Guid id, CancellationToken ct) =>
        _context.LancamentosFinanceiros.FirstOrDefaultAsync(l => l.UsuarioId == usuarioId && l.Id == id, ct);

    public async Task<IReadOnlyList<LancamentoFinanceiro>> ListarAsync(Guid usuarioId, CancellationToken ct) =>
        await _context.LancamentosFinanceiros
            .Where(l => l.UsuarioId == usuarioId)
            .OrderByDescending(l => l.Data)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<LancamentoFinanceiro>> ListarEntrePeriodoAsync(Guid usuarioId, DateTime inicio, DateTime fimExclusivo, CancellationToken ct) =>
        await _context.LancamentosFinanceiros
            .Where(l => l.UsuarioId == usuarioId && l.Data >= inicio && l.Data < fimExclusivo)
            .OrderBy(l => l.Data)
            .ToListAsync(ct);

    public async Task AdicionarAsync(LancamentoFinanceiro lancamento, CancellationToken ct) =>
        await _context.LancamentosFinanceiros.AddAsync(lancamento, ct);

    public void Remover(LancamentoFinanceiro lancamento) => _context.LancamentosFinanceiros.Remove(lancamento);

    public Task SalvarAlteracoesAsync(CancellationToken ct) => _context.SaveChangesAsync(ct);
}
