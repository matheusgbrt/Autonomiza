using GestaoAutonomo.Domain.Entities;

namespace GestaoAutonomo.Application.Interfaces;

public interface IAgendamentoRepository
{
    Task<Agendamento?> ObterPorIdAsync(Guid usuarioId, Guid id, CancellationToken ct);
    Task<IReadOnlyList<Agendamento>> ListarAsync(Guid usuarioId, CancellationToken ct);
    Task<bool> ExisteConflitoAsync(Guid usuarioId, DateTime inicio, DateTime fim, Guid? ignorarId, CancellationToken ct);
    Task<Agendamento?> ObterProximoPendenteAsync(Guid usuarioId, Guid clienteId, DateTime agora, CancellationToken ct);
    Task<IReadOnlyList<Agendamento>> ListarPendentesDeLembreteAsync(DateTime janelaInicio, DateTime janelaFim, CancellationToken ct);
    Task AdicionarAsync(Agendamento agendamento, CancellationToken ct);
    void Remover(Agendamento agendamento);
    Task SalvarAlteracoesAsync(CancellationToken ct);
}
