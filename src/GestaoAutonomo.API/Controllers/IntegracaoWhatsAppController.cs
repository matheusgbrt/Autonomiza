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
    private readonly IValidator<AtualizarConfiguracaoWhatsAppDto> _configuracaoValidator;

    public IntegracaoWhatsAppController(
        IIntegracaoWhatsAppService integracaoService,
        IValidator<ConfigurarWhatsAppDto> configurarValidator,
        IValidator<AtualizarConfiguracaoWhatsAppDto> configuracaoValidator)
    {
        _integracaoService = integracaoService;
        _configurarValidator = configurarValidator;
        _configuracaoValidator = configuracaoValidator;
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

    [HttpGet("estatisticas")]
    [ProducesResponseType(typeof(EstatisticasWhatsAppDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<EstatisticasWhatsAppDto>> ObterEstatisticas(CancellationToken ct)
    {
        var estatisticas = await _integracaoService.ObterEstatisticasAsync(User.GetUsuarioId(), ct);
        return Ok(estatisticas);
    }

    [HttpGet("mensagens")]
    [ProducesResponseType(typeof(IReadOnlyList<MensagemWhatsAppDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<MensagemWhatsAppDto>>> ObterUltimaConversa(CancellationToken ct)
    {
        var mensagens = await _integracaoService.ObterUltimaConversaAsync(User.GetUsuarioId(), ct);
        return Ok(mensagens);
    }

    [HttpGet("configuracoes")]
    [ProducesResponseType(typeof(ConfiguracaoWhatsAppDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ConfiguracaoWhatsAppDto>> ObterConfiguracao(CancellationToken ct)
    {
        var configuracao = await _integracaoService.ObterConfiguracaoAsync(User.GetUsuarioId(), ct);
        return Ok(configuracao);
    }

    [HttpPut("configuracoes")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AtualizarConfiguracao(AtualizarConfiguracaoWhatsAppDto dto, CancellationToken ct)
    {
        var validacao = await _configuracaoValidator.ValidateAsync(dto, ct);
        if (!validacao.IsValid)
        {
            return ValidationProblem(validacao.ToModelState());
        }

        await _integracaoService.AtualizarConfiguracaoAsync(User.GetUsuarioId(), dto, ct);
        return NoContent();
    }
}
