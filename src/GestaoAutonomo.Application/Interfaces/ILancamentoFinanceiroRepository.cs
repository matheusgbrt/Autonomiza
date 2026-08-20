using GestaoAutonomo.Domain.Entities;

namespace GestaoAutonomo.Application.Interfaces;

public interface ILancamentoFinanceiroRepository
{
    Task<LancamentoFinanceiro?> ObterPorIdAsync(Guid usuarioId, Guid id, CancellationToken ct);
    Task<IReadOnlyList<LancamentoFinanceiro>> ListarAsync(Guid usuarioId, CancellationToken ct);
    Task<IReadOnlyList<LancamentoFinanceiro>> ListarEntrePeriodoAsync(Guid usuarioId, DateTime inicio, DateTime fimExclusivo, CancellationToken ct);
    Task AdicionarAsync(LancamentoFinanceiro lancamento, CancellationToken ct);
    void Remover(LancamentoFinanceiro lancamento);
    Task SalvarAlteracoesAsync(CancellationToken ct);
}
