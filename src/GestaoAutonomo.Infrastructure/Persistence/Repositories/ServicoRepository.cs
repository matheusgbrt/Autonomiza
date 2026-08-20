using GestaoAutonomo.Application.Interfaces;
using GestaoAutonomo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestaoAutonomo.Infrastructure.Persistence.Repositories;

public class ServicoRepository : IServicoRepository
{
    private readonly AppDbContext _context;

    public ServicoRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Servico?> ObterPorIdAsync(Guid usuarioId, Guid id, CancellationToken ct) =>
        _context.Servicos.FirstOrDefaultAsync(s => s.UsuarioId == usuarioId && s.Id == id, ct);

    public async Task<IReadOnlyList<Servico>> ListarAsync(Guid usuarioId, CancellationToken ct) =>
        await _context.Servicos
            .Where(s => s.UsuarioId == usuarioId)
            .OrderBy(s => s.Nome)
            .ToListAsync(ct);

    public async Task AdicionarAsync(Servico servico, CancellationToken ct) =>
        await _context.Servicos.AddAsync(servico, ct);

    public void Remover(Servico servico) => _context.Servicos.Remove(servico);

    public Task SalvarAlteracoesAsync(CancellationToken ct) => _context.SaveChangesAsync(ct);
}
