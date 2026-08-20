using GestaoAutonomo.API.Common;
using GestaoAutonomo.Application.DTOs.Insight;
using GestaoAutonomo.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoAutonomo.API.Controllers;

[ApiController]
[Authorize(Policy = GestaoAutonomo.Infrastructure.DependencyInjection.PremiumOnlyPolicy)]
[Route("api/pro/insights")]
public class InsightController : ControllerBase
{
    private readonly IAiConsultorService _aiConsultorService;

    public InsightController(IAiConsultorService aiConsultorService)
    {
        _aiConsultorService = aiConsultorService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(InsightsResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<InsightsResponseDto>> Obter(CancellationToken ct)
    {
        var insights = await _aiConsultorService.ObterInsightsAsync(User.GetUsuarioId(), ct);
        return Ok(insights);
    }
}
