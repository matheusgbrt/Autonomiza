namespace GestaoAutonomo.Application.DTOs.Recomendacao;

public record RecomendacoesResponseDto(
    IReadOnlyList<RecomendacaoDto> Recomendacoes,
    DateTime GeradoEm,
    DateTime ExpiraEm,
    bool DoCache);
