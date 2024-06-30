using Infrastructure.DependencyRegistrations;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class LayerDependencyRegistration
{
    public static IServiceCollection UseInfrastructure(this IServiceCollection services)
    {
        services.AddMicrosoftEntityFramework();

        return services;
    }
}