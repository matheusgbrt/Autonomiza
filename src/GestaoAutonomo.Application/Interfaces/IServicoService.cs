using GestaoAutonomo.Application.DTOs.Servico;

namespace GestaoAutonomo.Application.Interfaces;

public interface IServicoService
{
    Task<ServicoDto> CriarAsync(Guid usuarioId, CriarServicoDto dto, CancellationToken ct);
    Task<ServicoDto?> ObterPorIdAsync(Guid usuarioId, Guid id, CancellationToken ct);
    Task<IReadOnlyList<ServicoDto>> ListarAsync(Guid usuarioId, CancellationToken ct);
    Task<ServicoDto> AtualizarAsync(Guid usuarioId, Guid id, AtualizarServicoDto dto, CancellationToken ct);
    Task RemoverAsync(Guid usuarioId, Guid id, CancellationToken ct);
}
