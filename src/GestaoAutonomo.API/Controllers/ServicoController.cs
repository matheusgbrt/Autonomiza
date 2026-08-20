using FluentValidation;
using GestaoAutonomo.API.Common;
using GestaoAutonomo.Application.DTOs.Servico;
using GestaoAutonomo.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoAutonomo.API.Controllers;

[ApiController]
[Authorize]
[Route("api/servicos")]
public class ServicoController : ControllerBase
{
    private readonly IServicoService _servicoService;
    private readonly IValidator<CriarServicoDto> _criarValidator;
    private readonly IValidator<AtualizarServicoDto> _atualizarValidator;

    public ServicoController(
        IServicoService servicoService,
        IValidator<CriarServicoDto> criarValidator,
        IValidator<AtualizarServicoDto> atualizarValidator)
    {
        _servicoService = servicoService;
        _criarValidator = criarValidator;
        _atualizarValidator = atualizarValidator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ServicoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ServicoDto>>> Listar(CancellationToken ct)
    {
        var servicos = await _servicoService.ListarAsync(User.GetUsuarioId(), ct);
        return Ok(servicos);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ServicoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServicoDto>> ObterPorId(Guid id, CancellationToken ct)
    {
        var servico = await _servicoService.ObterPorIdAsync(User.GetUsuarioId(), id, ct);
        return servico is null ? NotFound() : Ok(servico);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ServicoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ServicoDto>> Criar(CriarServicoDto dto, CancellationToken ct)
    {
        var validacao = await _criarValidator.ValidateAsync(dto, ct);
        if (!validacao.IsValid)
        {
            return ValidationProblem(validacao.ToModelState());
        }

        var servico = await _servicoService.CriarAsync(User.GetUsuarioId(), dto, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = servico.Id }, servico);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ServicoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServicoDto>> Atualizar(Guid id, AtualizarServicoDto dto, CancellationToken ct)
    {
        var validacao = await _atualizarValidator.ValidateAsync(dto, ct);
        if (!validacao.IsValid)
        {
            return ValidationProblem(validacao.ToModelState());
        }

        var servico = await _servicoService.AtualizarAsync(User.GetUsuarioId(), id, dto, ct);
        return Ok(servico);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(Guid id, CancellationToken ct)
    {
        await _servicoService.RemoverAsync(User.GetUsuarioId(), id, ct);
        return NoContent();
    }
}
