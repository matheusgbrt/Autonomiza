using GestaoAutonomo.API.Common;
using GestaoAutonomo.Application.DTOs.Dashboard;
using GestaoAutonomo.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoAutonomo.API.Controllers;

[ApiController]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("api/dashboard/resumo")]
    [ProducesResponseType(typeof(ResumoDashboardDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResumoDashboardDto>> Resumo(CancellationToken ct)
    {
        var resumo = await _dashboardService.ObterResumoAsync(User.GetUsuarioId(), ct);
        return Ok(resumo);
    }

    [HttpGet("api/pro/dashboard/avancado")]
    [Authorize(Policy = GestaoAutonomo.Infrastructure.DependencyInjection.PremiumOnlyPolicy)]
    [ProducesResponseType(typeof(DashboardAvancadoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<DashboardAvancadoDto>> Avancado([FromQuery] DateOnly? inicio, [FromQuery] DateOnly? fim, CancellationToken ct)
    {
        var avancado = await _dashboardService.ObterAvancadoAsync(User.GetUsuarioId(), inicio, fim, ct);
        return Ok(avancado);
    }
}
