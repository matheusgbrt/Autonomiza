using GestaoAutonomo.Application.DTOs.Auth;
using GestaoAutonomo.Application.Exceptions;
using GestaoAutonomo.Application.Interfaces;
using GestaoAutonomo.Domain.Entities;

namespace GestaoAutonomo.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponseDto> RegistrarAsync(RegistrarUsuarioDto dto, CancellationToken ct)
    {
        var existente = await _usuarioRepository.ObterPorEmailAsync(dto.Email, ct);
        if (existente is not null)
        {
            throw new EmailJaCadastradoException();
        }

        var usuario = new Usuario
        {
            Nome = dto.Nome,
            Email = dto.Email,
            SenhaHash = _passwordHasher.Hash(dto.Senha)
        };

        await _usuarioRepository.AdicionarAsync(usuario, ct);
        await _usuarioRepository.SalvarAlteracoesAsync(ct);

        return GerarResposta(usuario);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken ct)
    {
        var usuario = await _usuarioRepository.ObterPorEmailAsync(dto.Email, ct)
            ?? throw new CredenciaisInvalidasException();

        if (!_passwordHasher.Verificar(usuario.SenhaHash, dto.Senha))
        {
            throw new CredenciaisInvalidasException();
        }

        return GerarResposta(usuario);
    }

    private AuthResponseDto GerarResposta(Usuario usuario)
    {
        var (token, expiraEm) = _jwtTokenGenerator.GerarToken(usuario);
        return new AuthResponseDto(token, expiraEm, usuario.Id, usuario.Nome, usuario.Email, usuario.Plano.ToString());
    }
}
