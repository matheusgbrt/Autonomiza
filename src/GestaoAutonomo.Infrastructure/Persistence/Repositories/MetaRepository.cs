using GestaoAutonomo.Application.Interfaces;
using GestaoAutonomo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestaoAutonomo.Infrastructure.Persistence.Repositories;

public class MetaRepository : IMetaRepository
{
    private readonly AppDbContext _context;

    public MetaRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Meta?> ObterPorIdAsync(Guid usuarioId, Guid id, CancellationToken ct) =>
        _context.Metas.FirstOrDefaultAsync(m => m.UsuarioId == usuarioId && m.Id == id, ct);

    public async Task<IReadOnlyList<Meta>> ListarAsync(Guid usuarioId, CancellationToken ct) =>
        await _context.Metas
            .Where(m => m.UsuarioId == usuarioId)
            .OrderByDescending(m => m.PeriodoInicio)
            .ToListAsync(ct);

    public async Task AdicionarAsync(Meta meta, CancellationToken ct) =>
        await _context.Metas.AddAsync(meta, ct);

    public void Remover(Meta meta) => _context.Metas.Remove(meta);

    public Task SalvarAlteracoesAsync(CancellationToken ct) => _context.SaveChangesAsync(ct);
}
