using FluentValidation;
using GestaoAutonomo.API.Common;
using GestaoAutonomo.Application.DTOs.Integracao;
using GestaoAutonomo.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoAutonomo.API.Controllers;

[ApiController]
[Authorize]
[Route("api/integracoes/whatsapp")]
public class IntegracaoWhatsAppController : ControllerBase
{
    private readonly IIntegracaoWhatsAppService _integracaoService;
    private readonly IValidator<ConfigurarWhatsAppDto> _configurarValidator;

    public IntegracaoWhatsAppController(
        IIntegracaoWhatsAppService integracaoService,
        IValidator<ConfigurarWhatsAppDto> configurarValidator)
    {
        _integracaoService = integracaoService;
        _configurarValidator = configurarValidator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(StatusIntegracaoWhatsAppDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<StatusIntegracaoWhatsAppDto>> ObterStatus(CancellationToken ct)
    {
        var status = await _integracaoService.ObterStatusAsync(User.GetUsuarioId(), ct);
        return Ok(status);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Configurar(ConfigurarWhatsAppDto dto, CancellationToken ct)
    {
        var validacao = await _configurarValidator.ValidateAsync(dto, ct);
        if (!validacao.IsValid)
        {
            return ValidationProblem(validacao.ToModelState());
        }

        await _integracaoService.ConfigurarAsync(User.GetUsuarioId(), dto, ct);
        return NoContent();
    }
}
