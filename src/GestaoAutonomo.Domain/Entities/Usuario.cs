using GestaoAutonomo.Domain.Common;
using GestaoAutonomo.Domain.Enums;

namespace GestaoAutonomo.Domain.Entities;

public class Usuario : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public Plano Plano { get; set; } = Plano.Free;
}
