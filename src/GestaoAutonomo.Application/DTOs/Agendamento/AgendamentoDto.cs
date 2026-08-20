using GestaoAutonomo.Domain.Enums;

namespace GestaoAutonomo.Application.DTOs.Agendamento;

public record AgendamentoDto(
    Guid Id,
    Guid ClienteId,
    string ClienteNome,
    Guid ServicoId,
    string ServicoNome,
    DateTime DataHoraInicio,
    DateTime DataHoraFim,
    StatusAgendamento Status,
    string? Observacoes,
    int? NotaAtendimento,
    DateTime CreatedAt);
