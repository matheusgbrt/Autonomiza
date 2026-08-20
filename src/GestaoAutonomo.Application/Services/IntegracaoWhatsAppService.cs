using GestaoAutonomo.Application.DTOs.Integracao;
using GestaoAutonomo.Application.Exceptions;
using GestaoAutonomo.Application.Interfaces;

namespace GestaoAutonomo.Application.Services;

public class IntegracaoWhatsAppService : IIntegracaoWhatsAppService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public IntegracaoWhatsAppService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task ConfigurarAsync(Guid usuarioId, ConfigurarWhatsAppDto dto, CancellationToken ct)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId, ct)
            ?? throw new RecursoNaoEncontradoException("Usuário não encontrado.");

        var conflito = await _usuarioRepository.ObterPorZApiInstanceIdAsync(dto.InstanceId, ct);
        if (conflito is not null && conflito.Id != usuarioId)
        {
            throw new IntegracaoWhatsAppJaVinculadaException();
        }

        usuario.ZApiInstanceId = dto.InstanceId;
        usuario.ZApiToken = dto.Token;
        usuario.ZApiClientToken = dto.ClientToken;

        await _usuarioRepository.SalvarAlteracoesAsync(ct);
    }

    public async Task<StatusIntegracaoWhatsAppDto> ObterStatusAsync(Guid usuarioId, CancellationToken ct)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId, ct)
            ?? throw new RecursoNaoEncontradoException("Usuário não encontrado.");

        var conectado = !string.IsNullOrWhiteSpace(usuario.ZApiInstanceId) && !string.IsNullOrWhiteSpace(usuario.ZApiToken);
        return new StatusIntegracaoWhatsAppDto(conectado, usuario.ZApiInstanceId);
    }
}
