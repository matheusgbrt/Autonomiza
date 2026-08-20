using FluentValidation;
using GestaoAutonomo.API.Common;
using GestaoAutonomo.Application.DTOs.Agendamento;
using GestaoAutonomo.Application.DTOs.Cliente;
using GestaoAutonomo.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoAutonomo.API.Controllers;

[ApiController]
[Authorize]
[Route("api/clientes")]
public class ClienteController : ControllerBase
{
    private readonly IClienteService _clienteService;
    private readonly IAgendamentoService _agendamentoService;
    private readonly IValidator<CriarClienteDto> _criarValidator;
    private readonly IValidator<AtualizarClienteDto> _atualizarValidator;

    public ClienteController(
        IClienteService clienteService,
        IAgendamentoService agendamentoService,
        IValidator<CriarClienteDto> criarValidator,
        IValidator<AtualizarClienteDto> atualizarValidator)
    {
        _clienteService = clienteService;
        _agendamentoService = agendamentoService;
        _criarValidator = criarValidator;
        _atualizarValidator = atualizarValidator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ClienteDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ClienteDto>>> Listar(CancellationToken ct)
    {
        var clientes = await _clienteService.ListarAsync(User.GetUsuarioId(), ct);
        return Ok(clientes);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteDto>> ObterPorId(Guid id, CancellationToken ct)
    {
        var cliente = await _clienteService.ObterPorIdAsync(User.GetUsuarioId(), id, ct);
        return cliente is null ? NotFound() : Ok(cliente);
    }

    [HttpGet("{id:guid}/agendamentos")]
    [ProducesResponseType(typeof(IReadOnlyList<AgendamentoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<AgendamentoDto>>> ListarAgendamentos(Guid id, CancellationToken ct)
    {
        var usuarioId = User.GetUsuarioId();
        var cliente = await _clienteService.ObterPorIdAsync(usuarioId, id, ct);
        if (cliente is null) return NotFound();

        var agendamentos = await _agendamentoService.ListarPorClienteAsync(usuarioId, id, ct);
        return Ok(agendamentos);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClienteDto>> Criar(CriarClienteDto dto, CancellationToken ct)
    {
        var validacao = await _criarValidator.ValidateAsync(dto, ct);
        if (!validacao.IsValid)
        {
            return ValidationProblem(validacao.ToModelState());
        }

        var cliente = await _clienteService.CriarAsync(User.GetUsuarioId(), dto, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = cliente.Id }, cliente);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteDto>> Atualizar(Guid id, AtualizarClienteDto dto, CancellationToken ct)
    {
        var validacao = await _atualizarValidator.ValidateAsync(dto, ct);
        if (!validacao.IsValid)
        {
            return ValidationProblem(validacao.ToModelState());
        }

        var cliente = await _clienteService.AtualizarAsync(User.GetUsuarioId(), id, dto, ct);
        return Ok(cliente);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(Guid id, CancellationToken ct)
    {
        await _clienteService.RemoverAsync(User.GetUsuarioId(), id, ct);
        return NoContent();
    }
}
