using Application.EmotionsModule;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class LayerDependencyRegistration
{
    public static void AddApplication(this IServiceCollection services)
    {
        // Register services
        services.AddScoped<EmotionServices>();
    }
}