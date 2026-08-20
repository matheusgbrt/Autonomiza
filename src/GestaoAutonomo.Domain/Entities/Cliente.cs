using GestaoAutonomo.Domain.Common;

namespace GestaoAutonomo.Domain.Entities;

public class Cliente : BaseEntity
{
    public Guid UsuarioId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public string? Observacoes { get; set; }
}
