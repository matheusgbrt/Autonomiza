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

    /// <summary>Nota de 1 a 5 registrada pelo profissional após o atendimento; alimenta o KPI de Satisfação.</summary>
    public int? NotaAtendimento { get; set; }
}
