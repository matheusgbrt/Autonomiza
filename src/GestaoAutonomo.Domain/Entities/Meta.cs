using GestaoAutonomo.Domain.Common;
using GestaoAutonomo.Domain.Enums;

namespace GestaoAutonomo.Domain.Entities;

public class Meta : BaseEntity
{
    public Guid UsuarioId { get; set; }
    public TipoMeta Tipo { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public decimal ValorAlvo { get; set; }
    public DateTime PeriodoInicio { get; set; }
    public DateTime PeriodoFim { get; set; }
}
