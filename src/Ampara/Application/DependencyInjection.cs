using Ampara.Application.Adoptions;
using Ampara.Application.Animals;
using Ampara.Application.Auth;
using Ampara.Application.Dashboard;

namespace Ampara.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<AuthService>();
        services.AddScoped<AnimalService>();
        services.AddScoped<AdoptionService>();
        services.AddScoped<DashboardService>();
        return services;
    }
}
