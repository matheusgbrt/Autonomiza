namespace GestaoAutonomo.Application.DTOs.Servico;

public record ServicoDto(
    Guid Id,
    string Nome,
    string? Descricao,
    TimeSpan Duracao,
    decimal ValorPadrao,
    DateTime CreatedAt);
