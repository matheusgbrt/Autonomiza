using GestaoAutonomo.Domain.Enums;

namespace GestaoAutonomo.Application.DTOs.Agendamento;

public record AtualizarAgendamentoDto(
    Guid ClienteId,
    Guid ServicoId,
    DateTime DataHoraInicio,
    StatusAgendamento Status,
    string? Observacoes);
