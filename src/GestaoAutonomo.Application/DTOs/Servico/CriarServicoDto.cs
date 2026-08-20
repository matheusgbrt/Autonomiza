namespace GestaoAutonomo.Application.DTOs.Servico;

public record CriarServicoDto(
    string Nome,
    string? Descricao,
    TimeSpan Duracao,
    decimal ValorPadrao);
