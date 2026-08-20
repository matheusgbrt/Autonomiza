using GestaoAutonomo.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GestaoAutonomo.Infrastructure.Jobs;

public class LembreteAgendamentoJob : BackgroundService
{
    private static readonly TimeSpan Intervalo = TimeSpan.FromMinutes(15);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<LembreteAgendamentoJob> _logger;

    public LembreteAgendamentoJob(IServiceScopeFactory scopeFactory, ILogger<LembreteAgendamentoJob> logger)
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
                var lembreteService = scope.ServiceProvider.GetRequiredService<ILembreteAgendamentoService>();
                await lembreteService.EnviarLembretesPendentesAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Erro ao processar lembretes de agendamento.");
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
