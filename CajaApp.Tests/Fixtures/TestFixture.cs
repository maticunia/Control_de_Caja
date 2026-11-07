using CajaApp.Application.Interfaces;
using CajaApp.Application.UseCases.Caja;
using CajaApp.Application.UseCases.Cliente;
using CajaApp.Application.UseCases.Movimiento;
using CajaApp.Domain.Entities;
using CajaApp.Domain.Interfaces;
using CajaApp.Domain.Interfaces.Repositories;
using CajaApp.Domain.Shared;
using CajaApp.Infrastructure.Data;
using CajaApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Data.Common;

namespace CajaApp.Tests.Fixtures;

public class TestFixture : IDisposable
{
    public IServiceProvider ServiceProvider { get; }
    public AppDbContext DbContext { get; }

    public TestFixture()
    {
        // In-memory database
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        DbContext = new AppDbContext(options);
        DbContext.Database.EnsureCreated();

        // Setup DI
        var services = new ServiceCollection();
        
        // Infrastructure
        services.AddScoped<ICajaRepository>(_ => new CajaRepository(DbContext));
        services.AddScoped<IClienteRepository>(_ => new ClienteRepository(DbContext));
        services.AddScoped<IMovimientoCajaRepository>(_ => new MovimientoCajaRepository(DbContext));
        services.AddScoped<IUnitOfWork>(_ => new UnitOfWork(DbContext));

        // Application
        services.AddScoped<ICajaService, CajaService>();
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IMovimientoService, MovimientoService>();

        ServiceProvider = services.BuildServiceProvider();
    }

    public void Dispose()
    {
        DbContext.Database.EnsureDeleted();
        DbContext.Dispose();
    }

    // Helper methods for seeding test data
    public async Task<Caja> CreateTestCajaAsync(string nombre, decimal saldoInicial = 0)
    {
        var caja = new Caja(nombre, "Test description", new Monto(saldoInicial));
        await DbContext.Cajas.AddAsync(caja);
        await DbContext.SaveChangesAsync();
        return caja;
    }

    public async Task<Cliente> CreateTestClienteAsync(string nombre, string apellidos)
    {
        var cliente = new Cliente(nombre, apellidos, null, null, null);
        await DbContext.Clientes.AddAsync(cliente);
        await DbContext.SaveChangesAsync();
        return cliente;
    }

    public async Task<MovimientoCaja> CreateTestMovimientoAsync(
        int cajaId, 
        Domain.Enums.TipoMovimiento tipo, 
        decimal monto,
        string descripcion)
    {
        var movimiento = new MovimientoCaja(
            cajaId, 
            tipo, 
            new Monto(monto), 
            descripcion);
        await DbContext.Movimientos.AddAsync(movimiento);
        await DbContext.SaveChangesAsync();
        return movimiento;
    }
}

public class CollectionFixture : IDisposable
{
    public TestFixture Fixture { get; }

    public CollectionFixture()
    {
        Fixture = new TestFixture();
    }

    public void Dispose()
    {
        Fixture.Dispose();
    }
}

[CollectionDefinition("CajaAppTests")]
public class CajaAppTestsCollection : ICollectionFixture<CollectionFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
