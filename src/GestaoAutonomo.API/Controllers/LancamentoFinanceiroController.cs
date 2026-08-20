using FluentValidation;
using GestaoAutonomo.API.Common;
using GestaoAutonomo.Application.DTOs.LancamentoFinanceiro;
using GestaoAutonomo.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoAutonomo.API.Controllers;

[ApiController]
[Authorize]
[Route("api/lancamentos-financeiros")]
public class LancamentoFinanceiroController : ControllerBase
{
    private readonly ILancamentoFinanceiroService _lancamentoService;
    private readonly IValidator<CriarLancamentoFinanceiroDto> _criarValidator;
    private readonly IValidator<AtualizarLancamentoFinanceiroDto> _atualizarValidator;

    public LancamentoFinanceiroController(
        ILancamentoFinanceiroService lancamentoService,
        IValidator<CriarLancamentoFinanceiroDto> criarValidator,
        IValidator<AtualizarLancamentoFinanceiroDto> atualizarValidator)
    {
        _lancamentoService = lancamentoService;
        _criarValidator = criarValidator;
        _atualizarValidator = atualizarValidator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<LancamentoFinanceiroDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<LancamentoFinanceiroDto>>> Listar(CancellationToken ct)
    {
        var lancamentos = await _lancamentoService.ListarAsync(User.GetUsuarioId(), ct);
        return Ok(lancamentos);
    }

    [HttpGet("saldo")]
    [ProducesResponseType(typeof(SaldoMensalDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<SaldoMensalDto>> ObterSaldoMensal([FromQuery] int? ano, [FromQuery] int? mes, CancellationToken ct)
    {
        var agora = DateTime.UtcNow;
        var saldo = await _lancamentoService.ObterSaldoMensalAsync(User.GetUsuarioId(), ano ?? agora.Year, mes ?? agora.Month, ct);
        return Ok(saldo);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(LancamentoFinanceiroDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LancamentoFinanceiroDto>> ObterPorId(Guid id, CancellationToken ct)
    {
        var lancamento = await _lancamentoService.ObterPorIdAsync(User.GetUsuarioId(), id, ct);
        return lancamento is null ? NotFound() : Ok(lancamento);
    }

    [HttpPost]
    [ProducesResponseType(typeof(LancamentoFinanceiroDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LancamentoFinanceiroDto>> Criar(CriarLancamentoFinanceiroDto dto, CancellationToken ct)
    {
        var validacao = await _criarValidator.ValidateAsync(dto, ct);
        if (!validacao.IsValid)
        {
            return ValidationProblem(validacao.ToModelState());
        }

        var lancamento = await _lancamentoService.CriarAsync(User.GetUsuarioId(), dto, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = lancamento.Id }, lancamento);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(LancamentoFinanceiroDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LancamentoFinanceiroDto>> Atualizar(Guid id, AtualizarLancamentoFinanceiroDto dto, CancellationToken ct)
    {
        var validacao = await _atualizarValidator.ValidateAsync(dto, ct);
        if (!validacao.IsValid)
        {
            return ValidationProblem(validacao.ToModelState());
        }

        var lancamento = await _lancamentoService.AtualizarAsync(User.GetUsuarioId(), id, dto, ct);
        return Ok(lancamento);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(Guid id, CancellationToken ct)
    {
        await _lancamentoService.RemoverAsync(User.GetUsuarioId(), id, ct);
        return NoContent();
    }
}
