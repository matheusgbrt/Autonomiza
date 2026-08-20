using GestaoAutonomo.Application.Interfaces;

namespace GestaoAutonomo.Application.Services;

public class InsightsRefreshService : IInsightsRefreshService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAiConsultorService _aiConsultorService;

    public InsightsRefreshService(IUsuarioRepository usuarioRepository, IAiConsultorService aiConsultorService)
    {
        _usuarioRepository = usuarioRepository;
        _aiConsultorService = aiConsultorService;
    }

    public async Task GerarInsightsParaTodosUsuariosProAsync(CancellationToken ct)
    {
        var usuariosPro = await _usuarioRepository.ListarProAsync(ct);

        foreach (var usuario in usuariosPro)
        {
            await _aiConsultorService.ObterInsightsAsync(usuario.Id, ct);
        }
    }
}
