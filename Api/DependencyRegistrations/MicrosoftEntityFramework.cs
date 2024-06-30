using Microsoft.EntityFrameworkCore;

namespace Api.DependencyRegistrations;

public static class MicrosoftEntityFramework
{
    public static IServiceCollection AddMicrosoftEntityFramework(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(
            options => options.UseInMemoryDatabase("AppDb"));

        return services;
    }
}