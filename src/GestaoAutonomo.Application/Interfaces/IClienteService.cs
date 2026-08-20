using GestaoAutonomo.Application.DTOs.Cliente;

namespace GestaoAutonomo.Application.Interfaces;

public interface IClienteService
{
    Task<ClienteDto> CriarAsync(Guid usuarioId, CriarClienteDto dto, CancellationToken ct);
    Task<ClienteDto?> ObterPorIdAsync(Guid usuarioId, Guid id, CancellationToken ct);
    Task<IReadOnlyList<ClienteDto>> ListarAsync(Guid usuarioId, CancellationToken ct);
    Task<ClienteDto> AtualizarAsync(Guid usuarioId, Guid id, AtualizarClienteDto dto, CancellationToken ct);
    Task RemoverAsync(Guid usuarioId, Guid id, CancellationToken ct);
}
