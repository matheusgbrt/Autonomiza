namespace GestaoAutonomo.Application.DTOs.Dashboard;

public record DashboardAvancadoDto(
    IReadOnlyList<RentabilidadeServicoDto> RentabilidadePorServico,
    decimal TaxaFidelizacaoPercentual,
    decimal ProjecaoFaturamentoProximos30Dias,
    int HealthScoreNegocio,
    decimal? SatisfacaoMedia,
    IReadOnlyList<PontoTrimestralDto> CrescimentoReceitaTrimestral);
