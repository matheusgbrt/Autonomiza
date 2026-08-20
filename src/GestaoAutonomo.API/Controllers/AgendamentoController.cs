using FluentValidation;
using GestaoAutonomo.API.Common;
using GestaoAutonomo.Application.DTOs.Agendamento;
using GestaoAutonomo.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoAutonomo.API.Controllers;

[ApiController]
[Authorize]
[Route("api/agendamentos")]
public class AgendamentoController : ControllerBase
{
    private readonly IAgendamentoService _agendamentoService;
    private readonly IValidator<CriarAgendamentoDto> _criarValidator;
    private readonly IValidator<AtualizarAgendamentoDto> _atualizarValidator;

    public AgendamentoController(
        IAgendamentoService agendamentoService,
        IValidator<CriarAgendamentoDto> criarValidator,
        IValidator<AtualizarAgendamentoDto> atualizarValidator)
    {
        _agendamentoService = agendamentoService;
        _criarValidator = criarValidator;
        _atualizarValidator = atualizarValidator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AgendamentoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AgendamentoDto>>> Listar(
        [FromQuery] DateTime? inicio, [FromQuery] DateTime? fim, CancellationToken ct)
    {
        var agendamentos = await _agendamentoService.ListarAsync(User.GetUsuarioId(), inicio, fim, ct);
        return Ok(agendamentos);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AgendamentoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AgendamentoDto>> ObterPorId(Guid id, CancellationToken ct)
    {
        var agendamento = await _agendamentoService.ObterPorIdAsync(User.GetUsuarioId(), id, ct);
        return agendamento is null ? NotFound() : Ok(agendamento);
    }

    [HttpPost]
    [ProducesResponseType(typeof(AgendamentoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AgendamentoDto>> Criar(CriarAgendamentoDto dto, CancellationToken ct)
    {
        var validacao = await _criarValidator.ValidateAsync(dto, ct);
        if (!validacao.IsValid)
        {
            return ValidationProblem(validacao.ToModelState());
        }

        var agendamento = await _agendamentoService.CriarAsync(User.GetUsuarioId(), dto, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = agendamento.Id }, agendamento);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AgendamentoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AgendamentoDto>> Atualizar(Guid id, AtualizarAgendamentoDto dto, CancellationToken ct)
    {
        var validacao = await _atualizarValidator.ValidateAsync(dto, ct);
        if (!validacao.IsValid)
        {
            return ValidationProblem(validacao.ToModelState());
        }

        var agendamento = await _agendamentoService.AtualizarAsync(User.GetUsuarioId(), id, dto, ct);
        return Ok(agendamento);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(Guid id, CancellationToken ct)
    {
        await _agendamentoService.RemoverAsync(User.GetUsuarioId(), id, ct);
        return NoContent();
    }
}
