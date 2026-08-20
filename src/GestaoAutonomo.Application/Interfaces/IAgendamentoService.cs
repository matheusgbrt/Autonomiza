using GestaoAutonomo.Application.DTOs.Agendamento;

namespace GestaoAutonomo.Application.Interfaces;

public interface IAgendamentoService
{
    Task<AgendamentoDto> CriarAsync(Guid usuarioId, CriarAgendamentoDto dto, CancellationToken ct);
    Task<AgendamentoDto?> ObterPorIdAsync(Guid usuarioId, Guid id, CancellationToken ct);
    Task<IReadOnlyList<AgendamentoDto>> ListarAsync(Guid usuarioId, CancellationToken ct);
    Task<AgendamentoDto> AtualizarAsync(Guid usuarioId, Guid id, AtualizarAgendamentoDto dto, CancellationToken ct);
    Task RemoverAsync(Guid usuarioId, Guid id, CancellationToken ct);
}
