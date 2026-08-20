namespace GestaoAutonomo.Application.DTOs.Tarefa;

public record CriarTarefaDto(
    string Titulo,
    string? Descricao,
    DateTime? DataVencimento);
