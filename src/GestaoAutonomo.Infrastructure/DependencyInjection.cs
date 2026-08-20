using System.Text;
using GestaoAutonomo.Application.Interfaces;
using GestaoAutonomo.Domain.Enums;
using GestaoAutonomo.Infrastructure.Integrations;
using GestaoAutonomo.Infrastructure.Jobs;
using GestaoAutonomo.Infrastructure.Persistence;
using GestaoAutonomo.Infrastructure.Persistence.Repositories;
using GestaoAutonomo.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace GestaoAutonomo.Infrastructure;

public static class DependencyInjection
{
    public const string PremiumOnlyPolicy = "PremiumOnly";
    public const string FrontendCorsPolicy = "Frontend";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? new[] { "http://localhost:5173" };

        services.AddCors(options =>
        {
            options.AddPolicy(FrontendCorsPolicy, policy =>
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyHeader()
                      .AllowAnyMethod());
        });

        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IServicoRepository, ServicoRepository>();
        services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();
        services.AddScoped<ILancamentoFinanceiroRepository, LancamentoFinanceiroRepository>();
        services.AddScoped<ITarefaRepository, TarefaRepository>();
        services.AddScoped<IItemChecklistRepository, ItemChecklistRepository>();
        services.AddScoped<IMetaRepository, MetaRepository>();
        services.AddScoped<IInsightRepository, InsightRepository>();
        services.AddScoped<IRecomendacaoRepository, RecomendacaoRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddHttpClient<IWhatsAppSender, ZApiWhatsAppSender>(client =>
        {
            client.BaseAddress = new Uri("https://api.z-api.io/");
        });

        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ?? new JwtSettings();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
                };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy(PremiumOnlyPolicy, policy => policy.RequireClaim("plano", nameof(Plano.Pro)));

        services.AddHostedService<LembreteAgendamentoJob>();
        services.AddHostedService<InsightsRefreshJob>();

        return services;
    }
}
