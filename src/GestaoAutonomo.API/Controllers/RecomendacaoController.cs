using GestaoAutonomo.API.Common;
using GestaoAutonomo.Application.DTOs.Recomendacao;
using GestaoAutonomo.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoAutonomo.API.Controllers;

[ApiController]
[Authorize(Policy = GestaoAutonomo.Infrastructure.DependencyInjection.PremiumOnlyPolicy)]
[Route("api/pro/recomendacoes")]
public class RecomendacaoController : ControllerBase
{
    private readonly IRecomendacaoService _recomendacaoService;

    public RecomendacaoController(IRecomendacaoService recomendacaoService)
    {
        _recomendacaoService = recomendacaoService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(RecomendacoesResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<RecomendacoesResponseDto>> Obter(CancellationToken ct)
    {
        var recomendacoes = await _recomendacaoService.ObterRecomendacoesAsync(User.GetUsuarioId(), ct);
        return Ok(recomendacoes);
    }
}
