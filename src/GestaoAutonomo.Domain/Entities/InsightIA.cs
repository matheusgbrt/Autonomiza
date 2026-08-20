using GestaoAutonomo.Domain.Common;
using GestaoAutonomo.Domain.Enums;

namespace GestaoAutonomo.Domain.Entities;

public class InsightIA : BaseEntity
{
    public Guid UsuarioId { get; set; }
    public CategoriaInsight Categoria { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public DateTime ExpiraEm { get; set; }
}
