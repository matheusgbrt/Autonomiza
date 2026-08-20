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
        return services;
    }
}
