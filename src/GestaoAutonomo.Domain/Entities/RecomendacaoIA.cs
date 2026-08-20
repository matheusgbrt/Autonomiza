using GestaoAutonomo.Domain.Common;
using GestaoAutonomo.Domain.Enums;

namespace GestaoAutonomo.Domain.Entities;

public class RecomendacaoIA : BaseEntity
{
    public Guid UsuarioId { get; set; }
    public CategoriaRecomendacao Categoria { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public DateTime ExpiraEm { get; set; }
}
