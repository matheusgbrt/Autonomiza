using GestaoAutonomo.Domain.Common;
using GestaoAutonomo.Domain.Enums;

namespace GestaoAutonomo.Domain.Entities;

public class LancamentoFinanceiro : BaseEntity
{
    public Guid UsuarioId { get; set; }
    public TipoLancamento Tipo { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime Data { get; set; }
    public string? Descricao { get; set; }
    public Guid? ClienteId { get; set; }
    public Guid? AgendamentoId { get; set; }
}
