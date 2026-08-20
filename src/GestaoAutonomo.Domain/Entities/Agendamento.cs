using GestaoAutonomo.Domain.Common;
using GestaoAutonomo.Domain.Enums;

namespace GestaoAutonomo.Domain.Entities;

public class Agendamento : BaseEntity
{
    public Guid UsuarioId { get; set; }
    public Guid ClienteId { get; set; }
    public Guid ServicoId { get; set; }
    public DateTime DataHoraInicio { get; set; }
    public DateTime DataHoraFim { get; set; }
    public StatusAgendamento Status { get; set; } = StatusAgendamento.Agendado;
    public string? Observacoes { get; set; }
    public bool LembreteEnviado { get; set; }
}
