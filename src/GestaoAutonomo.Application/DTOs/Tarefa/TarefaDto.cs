namespace GestaoAutonomo.Application.DTOs.Tarefa;

public record TarefaDto(
    Guid Id,
    string Titulo,
    string? Descricao,
    bool Concluida,
    DateTime? DataVencimento,
    IReadOnlyList<ItemChecklistDto> Itens,
    DateTime CreatedAt);
