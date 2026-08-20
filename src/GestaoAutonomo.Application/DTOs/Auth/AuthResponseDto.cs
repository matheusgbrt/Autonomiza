namespace GestaoAutonomo.Application.DTOs.Auth;

public record AuthResponseDto(
    string Token,
    DateTime ExpiraEm,
    Guid UsuarioId,
    string Nome,
    string Email,
    string Plano);
