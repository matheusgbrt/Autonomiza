using GestaoAutonomo.Domain.Enums;

namespace GestaoAutonomo.Application.DTOs.Meta;

public record MetaDto(
    Guid Id,
    TipoMeta Tipo,
    string Titulo,
    decimal ValorAlvo,
    DateTime PeriodoInicio,
    DateTime PeriodoFim,
    DateTime CreatedAt);
