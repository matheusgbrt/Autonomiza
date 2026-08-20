namespace GestaoAutonomo.Application.Interfaces;

public interface IInsightsRefreshService
{
    Task GerarInsightsParaTodosUsuariosProAsync(CancellationToken ct);
}
