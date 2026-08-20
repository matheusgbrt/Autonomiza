namespace GestaoAutonomo.Application.DTOs.Tarefa;

public record AtualizarTarefaDto(
    string Titulo,
    string? Descricao,
    bool Concluida,
    DateTime? DataVencimento);
