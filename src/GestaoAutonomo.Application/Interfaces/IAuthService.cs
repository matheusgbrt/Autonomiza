using GestaoAutonomo.Application.DTOs.Auth;

namespace GestaoAutonomo.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegistrarAsync(RegistrarUsuarioDto dto, CancellationToken ct);
    Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken ct);
}
