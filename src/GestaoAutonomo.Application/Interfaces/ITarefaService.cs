using GestaoAutonomo.Application.DTOs.Tarefa;

namespace GestaoAutonomo.Application.Interfaces;

public interface ITarefaService
{
    Task<TarefaDto> CriarAsync(Guid usuarioId, CriarTarefaDto dto, CancellationToken ct);
    Task<TarefaDto?> ObterPorIdAsync(Guid usuarioId, Guid id, CancellationToken ct);
    Task<IReadOnlyList<TarefaDto>> ListarAsync(Guid usuarioId, CancellationToken ct);
    Task<TarefaDto> AtualizarAsync(Guid usuarioId, Guid id, AtualizarTarefaDto dto, CancellationToken ct);
    Task RemoverAsync(Guid usuarioId, Guid id, CancellationToken ct);
}
