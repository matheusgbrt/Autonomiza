using GestaoAutonomo.Domain.Common;

namespace GestaoAutonomo.Domain.Entities;

public class Tarefa : BaseEntity
{
    public Guid UsuarioId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public bool Concluida { get; set; }
    public DateTime? DataVencimento { get; set; }
}
