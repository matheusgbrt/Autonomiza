using FluentValidation;
using GestaoAutonomo.API.Common;
using GestaoAutonomo.Application.DTOs.Meta;
using GestaoAutonomo.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoAutonomo.API.Controllers;

[ApiController]
[Authorize]
[Route("api/metas")]
public class MetaController : ControllerBase
{
    private readonly IMetaService _metaService;
    private readonly IValidator<CriarMetaDto> _criarValidator;
    private readonly IValidator<AtualizarMetaDto> _atualizarValidator;

    public MetaController(
        IMetaService metaService,
        IValidator<CriarMetaDto> criarValidator,
        IValidator<AtualizarMetaDto> atualizarValidator)
    {
        _metaService = metaService;
        _criarValidator = criarValidator;
        _atualizarValidator = atualizarValidator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<MetaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<MetaDto>>> Listar(CancellationToken ct)
    {
        var metas = await _metaService.ListarAsync(User.GetUsuarioId(), ct);
        return Ok(metas);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MetaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MetaDto>> ObterPorId(Guid id, CancellationToken ct)
    {
        var meta = await _metaService.ObterPorIdAsync(User.GetUsuarioId(), id, ct);
        return meta is null ? NotFound() : Ok(meta);
    }

    [HttpPost]
    [ProducesResponseType(typeof(MetaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MetaDto>> Criar(CriarMetaDto dto, CancellationToken ct)
    {
        var validacao = await _criarValidator.ValidateAsync(dto, ct);
        if (!validacao.IsValid)
        {
            return ValidationProblem(validacao.ToModelState());
        }

        var meta = await _metaService.CriarAsync(User.GetUsuarioId(), dto, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = meta.Id }, meta);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(MetaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MetaDto>> Atualizar(Guid id, AtualizarMetaDto dto, CancellationToken ct)
    {
        var validacao = await _atualizarValidator.ValidateAsync(dto, ct);
        if (!validacao.IsValid)
        {
            return ValidationProblem(validacao.ToModelState());
        }

        var meta = await _metaService.AtualizarAsync(User.GetUsuarioId(), id, dto, ct);
        return Ok(meta);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(Guid id, CancellationToken ct)
    {
        await _metaService.RemoverAsync(User.GetUsuarioId(), id, ct);
        return NoContent();
    }
}
