using GestaoAutonomo.Domain.Enums;

namespace GestaoAutonomo.Application.DTOs.LancamentoFinanceiro;

public record LancamentoFinanceiroDto(
    Guid Id,
    TipoLancamento Tipo,
    string Categoria,
    decimal Valor,
    DateTime Data,
    string? Descricao,
    Guid? ClienteId,
    Guid? AgendamentoId,
    DateTime CreatedAt);
