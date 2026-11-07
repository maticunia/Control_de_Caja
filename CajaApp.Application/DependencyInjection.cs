using CajaApp.Application.Interfaces;
using CajaApp.Application.UseCases.Caja;
using CajaApp.Application.UseCases.Cliente;
using CajaApp.Application.UseCases.Movimiento;
using Microsoft.Extensions.DependencyInjection;

namespace CajaApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Services
        services.AddScoped<ICajaService, CajaService>();
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IMovimientoService, MovimientoService>();

        return services;
    }
}
