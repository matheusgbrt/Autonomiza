using GestaoAutonomo.Application.Interfaces;
using GestaoAutonomo.Domain.Entities;
using GestaoAutonomo.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GestaoAutonomo.Infrastructure.Persistence.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken ct) =>
        _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email, ct);

    public Task<Usuario?> ObterPorIdAsync(Guid id, CancellationToken ct) =>
        _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<Usuario?> ObterPorZApiInstanceIdAsync(string instanceId, CancellationToken ct) =>
        _context.Usuarios.FirstOrDefaultAsync(u => u.ZApiInstanceId == instanceId, ct);

    public async Task<IReadOnlyList<Usuario>> ListarProAsync(CancellationToken ct) =>
        await _context.Usuarios.Where(u => u.Plano == Plano.Pro).ToListAsync(ct);

    public async Task AdicionarAsync(Usuario usuario, CancellationToken ct) =>
        await _context.Usuarios.AddAsync(usuario, ct);

    public Task SalvarAlteracoesAsync(CancellationToken ct) => _context.SaveChangesAsync(ct);
}
