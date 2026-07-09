using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SIGERI.Application.Interfaces;
using SIGERI.Infrastructure.Persistence;

[assembly: System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]

namespace SIGERI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SigeriDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.MigrationsAssembly("SIGERI.Infrastructure")));

        services.AddScoped<ISigeriDbContext>(provider => provider.GetRequiredService<SigeriDbContext>());

        return services;
    }
}
