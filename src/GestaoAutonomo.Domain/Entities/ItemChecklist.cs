using GestaoAutonomo.Domain.Common;

namespace GestaoAutonomo.Domain.Entities;

public class ItemChecklist : BaseEntity
{
    public Guid TarefaId { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public bool Concluido { get; set; }
    public int Ordem { get; set; }
}
