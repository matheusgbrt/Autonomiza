using GestaoAutonomo.Application.DTOs.LancamentoFinanceiro;

namespace GestaoAutonomo.Application.Interfaces;

public interface ILancamentoFinanceiroService
{
    Task<LancamentoFinanceiroDto> CriarAsync(Guid usuarioId, CriarLancamentoFinanceiroDto dto, CancellationToken ct);
    Task<LancamentoFinanceiroDto?> ObterPorIdAsync(Guid usuarioId, Guid id, CancellationToken ct);
    Task<IReadOnlyList<LancamentoFinanceiroDto>> ListarAsync(Guid usuarioId, CancellationToken ct);
    Task<LancamentoFinanceiroDto> AtualizarAsync(Guid usuarioId, Guid id, AtualizarLancamentoFinanceiroDto dto, CancellationToken ct);
    Task RemoverAsync(Guid usuarioId, Guid id, CancellationToken ct);
}
