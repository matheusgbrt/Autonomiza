namespace GestaoAutonomo.Application.DTOs.Dashboard;

public record RentabilidadeServicoDto(
    Guid ServicoId,
    string ServicoNome,
    decimal ReceitaTotal,
    int QuantidadeAtendimentos,
    decimal TicketMedio);
