using GestaoAutonomo.Domain.Common;
using GestaoAutonomo.Domain.Enums;

namespace GestaoAutonomo.Domain.Entities;

public class MensagemWhatsApp : BaseEntity
{
    public Guid UsuarioId { get; set; }
    public Guid? ClienteId { get; set; }
    public string Telefone { get; set; } = string.Empty;
    public DirecaoMensagemWhatsApp Direcao { get; set; }
    public string Conteudo { get; set; } = string.Empty;
}
