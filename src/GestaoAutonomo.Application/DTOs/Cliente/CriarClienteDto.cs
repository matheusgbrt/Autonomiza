namespace GestaoAutonomo.Application.DTOs.Cliente;

public record CriarClienteDto(
    string Nome,
    string? Email,
    string? Telefone,
    string? Observacoes);
