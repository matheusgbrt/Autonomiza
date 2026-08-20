namespace GestaoAutonomo.Application.DTOs.Cliente;

public record ClienteDto(
    Guid Id,
    string Nome,
    string? Email,
    string? Telefone,
    string? Observacoes,
    DateTime CreatedAt);
