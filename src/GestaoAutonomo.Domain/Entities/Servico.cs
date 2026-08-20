using GestaoAutonomo.Domain.Common;

namespace GestaoAutonomo.Domain.Entities;

public class Servico : BaseEntity
{
    public Guid UsuarioId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public TimeSpan Duracao { get; set; }
    public decimal ValorPadrao { get; set; }
}
