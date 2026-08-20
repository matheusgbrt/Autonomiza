using GestaoAutonomo.Application.Interfaces;
using GestaoAutonomo.Domain.Entities;
using GestaoAutonomo.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GestaoAutonomo.Infrastructure.Persistence.Repositories;

public class AgendamentoRepository : IAgendamentoRepository
{
    private readonly AppDbContext _context;

    public AgendamentoRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Agendamento?> ObterPorIdAsync(Guid usuarioId, Guid id, CancellationToken ct) =>
        _context.Agendamentos.FirstOrDefaultAsync(a => a.UsuarioId == usuarioId && a.Id == id, ct);

    public async Task<IReadOnlyList<Agendamento>> ListarAsync(Guid usuarioId, CancellationToken ct) =>
        await _context.Agendamentos
            .Where(a => a.UsuarioId == usuarioId)
            .OrderBy(a => a.DataHoraInicio)
            .ToListAsync(ct);

    public Task<bool> ExisteConflitoAsync(Guid usuarioId, DateTime inicio, DateTime fim, Guid? ignorarId, CancellationToken ct) =>
        _context.Agendamentos.AnyAsync(a =>
            a.UsuarioId == usuarioId &&
            a.Status != StatusAgendamento.Cancelado &&
            (ignorarId == null || a.Id != ignorarId) &&
            a.DataHoraInicio < fim &&
            a.DataHoraFim > inicio,
            ct);

    public Task<Agendamento?> ObterProximoPendenteAsync(Guid usuarioId, Guid clienteId, DateTime agora, CancellationToken ct) =>
        _context.Agendamentos
            .Where(a =>
                a.UsuarioId == usuarioId &&
                a.ClienteId == clienteId &&
                a.DataHoraInicio >= agora &&
                (a.Status == StatusAgendamento.Agendado || a.Status == StatusAgendamento.Confirmado))
            .OrderBy(a => a.DataHoraInicio)
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<Agendamento>> ListarPendentesDeLembreteAsync(DateTime janelaInicio, DateTime janelaFim, CancellationToken ct) =>
        await _context.Agendamentos
            .Where(a =>
                !a.LembreteEnviado &&
                a.DataHoraInicio >= janelaInicio &&
                a.DataHoraInicio <= janelaFim &&
                (a.Status == StatusAgendamento.Agendado || a.Status == StatusAgendamento.Confirmado))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Agendamento>> ListarPorPeriodoAsync(Guid usuarioId, DateTime inicio, DateTime fimExclusivo, CancellationToken ct) =>
        await _context.Agendamentos
            .Where(a => a.UsuarioId == usuarioId && a.DataHoraInicio >= inicio && a.DataHoraInicio < fimExclusivo)
            .OrderBy(a => a.DataHoraInicio)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Agendamento>> ListarPorClienteAsync(Guid usuarioId, Guid clienteId, CancellationToken ct) =>
        await _context.Agendamentos
            .Where(a => a.UsuarioId == usuarioId && a.ClienteId == clienteId)
            .OrderByDescending(a => a.DataHoraInicio)
            .ToListAsync(ct);

    public async Task AdicionarAsync(Agendamento agendamento, CancellationToken ct) =>
        await _context.Agendamentos.AddAsync(agendamento, ct);

    public void Remover(Agendamento agendamento) => _context.Agendamentos.Remove(agendamento);

    public Task SalvarAlteracoesAsync(CancellationToken ct) => _context.SaveChangesAsync(ct);
}
