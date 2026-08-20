using FluentValidation;
using GestaoAutonomo.Application.Interfaces;
using GestaoAutonomo.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GestaoAutonomo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjection));
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IServicoService, ServicoService>();
        services.AddScoped<IAgendamentoService, AgendamentoService>();
        services.AddScoped<ILancamentoFinanceiroService, LancamentoFinanceiroService>();
        services.AddScoped<ITarefaService, TarefaService>();
        services.AddScoped<IMetaService, MetaService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IAiConsultorService, AiConsultorService>();
        services.AddScoped<IRecomendacaoService, RecomendacaoService>();
        services.AddScoped<IIntegracaoWhatsAppService, IntegracaoWhatsAppService>();
        services.AddScoped<IWhatsAppWebhookProcessor, WhatsAppWebhookProcessor>();
        services.AddScoped<ILembreteAgendamentoService, LembreteAgendamentoService>();
        services.AddScoped<IInsightsRefreshService, InsightsRefreshService>();
        return services;
    }
}
