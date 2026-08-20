namespace GestaoAutonomo.Application.DTOs.Tarefa;

public record ItemChecklistDto(Guid Id, string Descricao, bool Concluido, int Ordem);
