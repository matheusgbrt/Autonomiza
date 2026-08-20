namespace GestaoAutonomo.Application.DTOs.Dashboard;

public record ResumoDashboardDto(
    decimal TotalVendas,
    decimal TicketMedio,
    int QuantidadeVendas,
    IReadOnlyList<PontoSerieDto> SerieDiaria);
