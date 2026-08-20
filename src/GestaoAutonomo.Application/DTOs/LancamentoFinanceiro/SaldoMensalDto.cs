namespace GestaoAutonomo.Application.DTOs.LancamentoFinanceiro;

public record SaldoMensalDto(
    int Ano,
    int Mes,
    decimal TotalEntradas,
    decimal TotalSaidas,
    decimal Saldo,
    IReadOnlyList<SaldoCategoriaDto> PorCategoria);

public record SaldoCategoriaDto(string Categoria, decimal TotalEntradas, decimal TotalSaidas);
