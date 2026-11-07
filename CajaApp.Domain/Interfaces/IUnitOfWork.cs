using CajaApp.Domain.Shared;

namespace CajaApp.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ICajaRepository Cajas { get; }
    IClienteRepository Clientes { get; }
    IMovimientoCajaRepository Movimientos { get; }
    
    Task<Result> SaveChangesAsync();
}
