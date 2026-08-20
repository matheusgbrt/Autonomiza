using GestaoAutonomo.Domain.Enums;

namespace GestaoAutonomo.Application.DTOs.Meta;

public record MetaDto(
    Guid Id,
    TipoMeta Tipo,
    string Titulo,
    decimal ValorAlvo,
    decimal ValorAtual,
    decimal ProgressoPercentual,
    DateTime PeriodoInicio,
    DateTime PeriodoFim,
    DateTime CreatedAt);
