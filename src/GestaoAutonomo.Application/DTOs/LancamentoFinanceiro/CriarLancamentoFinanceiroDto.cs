using GestaoAutonomo.Domain.Enums;

namespace GestaoAutonomo.Application.DTOs.LancamentoFinanceiro;

public record CriarLancamentoFinanceiroDto(
    TipoLancamento Tipo,
    string Categoria,
    decimal Valor,
    DateTime Data,
    string? Descricao,
    Guid? ClienteId,
    Guid? AgendamentoId);
