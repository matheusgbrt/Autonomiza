using GestaoAutonomo.Application.Interfaces;
using GestaoAutonomo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestaoAutonomo.Infrastructure.Persistence.Repositories;

public class RecomendacaoRepository : IRecomendacaoRepository
{
    private readonly AppDbContext _context;

    public RecomendacaoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<RecomendacaoIA>> ObterVigentesAsync(Guid usuarioId, DateTime agora, CancellationToken ct) =>
        await _context.RecomendacoesIA
            .Where(r => r.UsuarioId == usuarioId && r.ExpiraEm > agora)
            .ToListAsync(ct);

    public async Task SubstituirAsync(Guid usuarioId, IReadOnlyList<RecomendacaoIA> novas, CancellationToken ct)
    {
        var antigas = await _context.RecomendacoesIA
            .Where(r => r.UsuarioId == usuarioId)
            .ToListAsync(ct);

        _context.RecomendacoesIA.RemoveRange(antigas);
        await _context.RecomendacoesIA.AddRangeAsync(novas, ct);
        await _context.SaveChangesAsync(ct);
    }
}
