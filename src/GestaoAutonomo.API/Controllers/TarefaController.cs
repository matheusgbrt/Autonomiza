using FluentValidation;
using GestaoAutonomo.API.Common;
using GestaoAutonomo.Application.DTOs.Tarefa;
using GestaoAutonomo.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoAutonomo.API.Controllers;

[ApiController]
[Authorize]
[Route("api/tarefas")]
public class TarefaController : ControllerBase
{
    private readonly ITarefaService _tarefaService;
    private readonly IValidator<CriarTarefaDto> _criarValidator;
    private readonly IValidator<AtualizarTarefaDto> _atualizarValidator;

    public TarefaController(
        ITarefaService tarefaService,
        IValidator<CriarTarefaDto> criarValidator,
        IValidator<AtualizarTarefaDto> atualizarValidator)
    {
        _tarefaService = tarefaService;
        _criarValidator = criarValidator;
        _atualizarValidator = atualizarValidator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TarefaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TarefaDto>>> Listar(CancellationToken ct)
    {
        var tarefas = await _tarefaService.ListarAsync(User.GetUsuarioId(), ct);
        return Ok(tarefas);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TarefaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TarefaDto>> ObterPorId(Guid id, CancellationToken ct)
    {
        var tarefa = await _tarefaService.ObterPorIdAsync(User.GetUsuarioId(), id, ct);
        return tarefa is null ? NotFound() : Ok(tarefa);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TarefaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TarefaDto>> Criar(CriarTarefaDto dto, CancellationToken ct)
    {
        var validacao = await _criarValidator.ValidateAsync(dto, ct);
        if (!validacao.IsValid)
        {
            return ValidationProblem(validacao.ToModelState());
        }

        var tarefa = await _tarefaService.CriarAsync(User.GetUsuarioId(), dto, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = tarefa.Id }, tarefa);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TarefaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TarefaDto>> Atualizar(Guid id, AtualizarTarefaDto dto, CancellationToken ct)
    {
        var validacao = await _atualizarValidator.ValidateAsync(dto, ct);
        if (!validacao.IsValid)
        {
            return ValidationProblem(validacao.ToModelState());
        }

        var tarefa = await _tarefaService.AtualizarAsync(User.GetUsuarioId(), id, dto, ct);
        return Ok(tarefa);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(Guid id, CancellationToken ct)
    {
        await _tarefaService.RemoverAsync(User.GetUsuarioId(), id, ct);
        return NoContent();
    }
}
