namespace GestaoAutonomo.Application.DTOs.Servico;

public record AtualizarServicoDto(
    string Nome,
    string? Descricao,
    TimeSpan Duracao,
    decimal ValorPadrao);
