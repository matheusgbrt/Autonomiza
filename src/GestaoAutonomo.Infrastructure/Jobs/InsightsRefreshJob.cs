using GestaoAutonomo.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GestaoAutonomo.Infrastructure.Jobs;

public class InsightsRefreshJob : BackgroundService
{
    private static readonly TimeSpan Intervalo = TimeSpan.FromHours(24);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<InsightsRefreshJob> _logger;

    public InsightsRefreshJob(IServiceScopeFactory scopeFactory, ILogger<InsightsRefreshJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var insightsRefreshService = scope.ServiceProvider.GetRequiredService<IInsightsRefreshService>();
                await insightsRefreshService.GerarInsightsParaTodosUsuariosProAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Erro ao gerar insights automáticos para usuários Pro.");
            }

            try
            {
                await Task.Delay(Intervalo, stoppingToken);
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}
