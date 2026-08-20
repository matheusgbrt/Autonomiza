using FluentValidation;
using GestaoAutonomo.API.Common;
using GestaoAutonomo.Application.DTOs.Auth;
using GestaoAutonomo.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestaoAutonomo.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IValidator<RegistrarUsuarioDto> _registrarValidator;
    private readonly IValidator<LoginDto> _loginValidator;

    public AuthController(
        IAuthService authService,
        IValidator<RegistrarUsuarioDto> registrarValidator,
        IValidator<LoginDto> loginValidator)
    {
        _authService = authService;
        _registrarValidator = registrarValidator;
        _loginValidator = loginValidator;
    }

    [HttpPost("registrar")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponseDto>> Registrar(RegistrarUsuarioDto dto, CancellationToken ct)
    {
        var validacao = await _registrarValidator.ValidateAsync(dto, ct);
        if (!validacao.IsValid)
        {
            return ValidationProblem(validacao.ToModelState());
        }

        var resultado = await _authService.RegistrarAsync(dto, ct);
        return StatusCode(StatusCodes.Status201Created, resultado);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto, CancellationToken ct)
    {
        var validacao = await _loginValidator.ValidateAsync(dto, ct);
        if (!validacao.IsValid)
        {
            return ValidationProblem(validacao.ToModelState());
        }

        var resultado = await _authService.LoginAsync(dto, ct);
        return Ok(resultado);
    }
}
