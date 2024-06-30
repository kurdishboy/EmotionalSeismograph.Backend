using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Api.DependencyRegistrations;

public static class MicrosoftIdentity
{
    public static IServiceCollection AddMicrosoftIdentity(this IServiceCollection services)
    {
        services.AddAuthorization();

        services.AddIdentityApiEndpoints<User>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        return services;
    }

    public static WebApplication? UseMicrosoftIdentity(this WebApplication? app)
    {
        if (app is null) return app;

        app.UseAuthorization();
        app.MapIdentityApi<User>();

        return app;
    }
}