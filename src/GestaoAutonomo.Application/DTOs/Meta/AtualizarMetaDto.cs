using GestaoAutonomo.Domain.Enums;

namespace GestaoAutonomo.Application.DTOs.Meta;

public record AtualizarMetaDto(
    TipoMeta Tipo,
    string Titulo,
    decimal ValorAlvo,
    DateTime PeriodoInicio,
    DateTime PeriodoFim);
