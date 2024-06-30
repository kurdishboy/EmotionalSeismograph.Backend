using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyRegistrations;

internal static class MicrosoftEntityFramework
{
    public static IServiceCollection AddMicrosoftEntityFramework(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(
            options => options.UseInMemoryDatabase("AppDb"));

        return services;
    }
}