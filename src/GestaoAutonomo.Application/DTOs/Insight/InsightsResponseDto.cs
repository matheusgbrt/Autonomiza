namespace GestaoAutonomo.Application.DTOs.Insight;

public record InsightsResponseDto(
    IReadOnlyList<InsightDto> Insights,
    DateTime GeradoEm,
    DateTime ExpiraEm,
    bool DoCache);
