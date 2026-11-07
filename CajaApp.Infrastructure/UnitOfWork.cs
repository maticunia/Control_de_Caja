using CajaApp.Domain.Entities;
using CajaApp.Domain.Interfaces;
using CajaApp.Domain.Interfaces.Repositories;
using CajaApp.Domain.Shared;
using CajaApp.Infrastructure.Data;
using CajaApp.Infrastructure.Repositories;

namespace CajaApp.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private CajaRepository? _cajaRepository;
    private ClienteRepository? _clienteRepository;
    private MovimientoCajaRepository? _movimientoRepository;

    public UnitOfWork(AppDbContext context) => _context = context;

    public ICajaRepository Cajas => _cajaRepository ??= new CajaRepository(_context);
    public IClienteRepository Clientes => _clienteRepository ??= new ClienteRepository(_context);
    public IMovimientoCajaRepository Movimientos => _movimientoRepository ??= new MovimientoCajaRepository(_context);

    public async Task<Result> SaveChangesAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail("DB.SAVE_ERROR", ex.Message);
        }
    }

    public void Dispose() => _context.Dispose();
}
