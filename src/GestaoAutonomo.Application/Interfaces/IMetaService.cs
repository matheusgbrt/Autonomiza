using GestaoAutonomo.Application.DTOs.Meta;

namespace GestaoAutonomo.Application.Interfaces;

public interface IMetaService
{
    Task<MetaDto> CriarAsync(Guid usuarioId, CriarMetaDto dto, CancellationToken ct);
    Task<MetaDto?> ObterPorIdAsync(Guid usuarioId, Guid id, CancellationToken ct);
    Task<IReadOnlyList<MetaDto>> ListarAsync(Guid usuarioId, CancellationToken ct);
    Task<MetaDto> AtualizarAsync(Guid usuarioId, Guid id, AtualizarMetaDto dto, CancellationToken ct);
    Task RemoverAsync(Guid usuarioId, Guid id, CancellationToken ct);
}
