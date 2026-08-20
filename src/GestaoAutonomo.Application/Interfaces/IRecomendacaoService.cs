using GestaoAutonomo.Application.DTOs.Recomendacao;

namespace GestaoAutonomo.Application.Interfaces;

public interface IRecomendacaoService
{
    Task<RecomendacoesResponseDto> ObterRecomendacoesAsync(Guid usuarioId, CancellationToken ct);
}
