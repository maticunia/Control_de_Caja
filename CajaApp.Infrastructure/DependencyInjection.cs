using CajaApp.Domain.Interfaces;
using CajaApp.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace CajaApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        // Database
        services.AddDatabase(connectionString);

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
