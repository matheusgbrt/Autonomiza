namespace GestaoAutonomo.Application.DTOs.Agendamento;

public record CriarAgendamentoDto(
    Guid ClienteId,
    Guid ServicoId,
    DateTime DataHoraInicio,
    string? Observacoes);
