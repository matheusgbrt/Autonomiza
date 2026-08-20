using GestaoAutonomo.Application.Interfaces;
using GestaoAutonomo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestaoAutonomo.Infrastructure.Persistence.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly AppDbContext _context;

    public ClienteRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Cliente?> ObterPorIdAsync(Guid usuarioId, Guid id, CancellationToken ct) =>
        _context.Clientes.FirstOrDefaultAsync(c => c.UsuarioId == usuarioId && c.Id == id, ct);

    public async Task<IReadOnlyList<Cliente>> ListarAsync(Guid usuarioId, CancellationToken ct) =>
        await _context.Clientes
            .Where(c => c.UsuarioId == usuarioId)
            .OrderBy(c => c.Nome)
            .ToListAsync(ct);

    public async Task AdicionarAsync(Cliente cliente, CancellationToken ct) =>
        await _context.Clientes.AddAsync(cliente, ct);

    public void Remover(Cliente cliente) => _context.Clientes.Remove(cliente);

    public Task SalvarAlteracoesAsync(CancellationToken ct) => _context.SaveChangesAsync(ct);
}
