namespace GestaoAutonomo.Application.DTOs.Cliente;

public record AtualizarClienteDto(
    string Nome,
    string? Email,
    string? Telefone,
    string? Observacoes);
