using GestaoAutonomo.Application.Interfaces;
using GestaoAutonomo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestaoAutonomo.Infrastructure.Persistence.Repositories;

public class InsightRepository : IInsightRepository
{
    private readonly AppDbContext _context;

    public InsightRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<InsightIA>> ObterVigentesAsync(Guid usuarioId, DateTime agora, CancellationToken ct) =>
        await _context.InsightsIA
            .Where(i => i.UsuarioId == usuarioId && i.ExpiraEm > agora)
            .ToListAsync(ct);

    public async Task SubstituirAsync(Guid usuarioId, IReadOnlyList<InsightIA> novos, CancellationToken ct)
    {
        var antigos = await _context.InsightsIA
            .Where(i => i.UsuarioId == usuarioId)
            .ToListAsync(ct);

        _context.InsightsIA.RemoveRange(antigos);
        await _context.InsightsIA.AddRangeAsync(novos, ct);
        await _context.SaveChangesAsync(ct);
    }
}
