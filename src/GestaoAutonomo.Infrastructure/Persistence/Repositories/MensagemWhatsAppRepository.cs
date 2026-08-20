using GestaoAutonomo.Application.Interfaces;
using GestaoAutonomo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestaoAutonomo.Infrastructure.Persistence.Repositories;

public class MensagemWhatsAppRepository : IMensagemWhatsAppRepository
{
    private readonly AppDbContext _context;

    public MensagemWhatsAppRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AdicionarAsync(MensagemWhatsApp mensagem, CancellationToken ct) =>
        await _context.MensagensWhatsApp.AddAsync(mensagem, ct);

    public Task<bool> ExisteConversaAnteriorAsync(Guid usuarioId, string telefone, CancellationToken ct) =>
        _context.MensagensWhatsApp.AnyAsync(m => m.UsuarioId == usuarioId && m.Telefone == telefone, ct);

    public Task<int> ContarConversasUnicasAsync(Guid usuarioId, DateTime inicio, DateTime fimExclusivo, CancellationToken ct) =>
        _context.MensagensWhatsApp
            .Where(m => m.UsuarioId == usuarioId && m.CreatedAt >= inicio && m.CreatedAt < fimExclusivo)
            .Select(m => m.Telefone)
            .Distinct()
            .CountAsync(ct);

    public async Task<IReadOnlyList<MensagemWhatsApp>> ListarUltimaConversaAsync(Guid usuarioId, int quantidade, CancellationToken ct)
    {
        var ultimoTelefone = await _context.MensagensWhatsApp
            .Where(m => m.UsuarioId == usuarioId)
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => m.Telefone)
            .FirstOrDefaultAsync(ct);

        if (ultimoTelefone is null) return Array.Empty<MensagemWhatsApp>();

        var mensagens = await _context.MensagensWhatsApp
            .Where(m => m.UsuarioId == usuarioId && m.Telefone == ultimoTelefone)
            .OrderByDescending(m => m.CreatedAt)
            .Take(quantidade)
            .ToListAsync(ct);

        mensagens.Reverse();
        return mensagens;
    }

    public Task SalvarAlteracoesAsync(CancellationToken ct) => _context.SaveChangesAsync(ct);
}
