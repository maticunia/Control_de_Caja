using CajaApp.Domain.Entities;
using CajaApp.Domain.Enums;
using CajaApp.Domain.Interfaces.Repositories;
using CajaApp.Domain.Shared;
using CajaApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CajaApp.Infrastructure.Repositories;

public class MovimientoCajaRepository : RepositoryBase<MovimientoCaja>, IMovimientoCajaRepository
{
    public MovimientoCajaRepository(AppDbContext context) : base(context) { }

    public async Task<Result<IReadOnlyList<MovimientoCaja>>> GetByCajaAsync(int cajaId)
    {
        try
        {
            var movimientos = await _dbSet
                .Include(m => m.Caja)
                .Include(m => m.Cliente)
                .AsNoTracking()
                .Where(m => m.CajaId == cajaId)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();
            return Result<IReadOnlyList<MovimientoCaja>>.Ok(movimientos);
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyList<MovimientoCaja>>.Fail("ERROR", ex.Message);
        }
    }

    public async Task<Result<IReadOnlyList<MovimientoCaja>>> GetByFechaAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        try
        {
            var movimientos = await _dbSet
                .Include(m => m.Caja)
                .Include(m => m.Cliente)
                .AsNoTracking()
                .Where(m => m.Fecha >= fechaInicio && m.Fecha <= fechaFin)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();
            return Result<IReadOnlyList<MovimientoCaja>>.Ok(movimientos);
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyList<MovimientoCaja>>.Fail("ERROR", ex.Message);
        }
    }

    public async Task<Result<IReadOnlyList<MovimientoCaja>>> GetByTipoAsync(TipoMovimiento tipo)
    {
        try
        {
            var movimientos = await _dbSet
                .Include(m => m.Caja)
                .Include(m => m.Cliente)
                .AsNoTracking()
                .Where(m => m.Tipo == tipo)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();
            return Result<IReadOnlyList<MovimientoCaja>>.Ok(movimientos);
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyList<MovimientoCaja>>.Fail("ERROR", ex.Message);
        }
    }

    public async Task<Result<decimal>> GetSaldoActualAsync(int cajaId)
    {
        try
        {
            var ingresos = await _dbSet
                .Where(m => m.CajaId == cajaId && m.Tipo == TipoMovimiento.Ingreso)
                .SumAsync(m => m.Monto.Valor);

            var egresos = await _dbSet
                .Where(m => m.CajaId == cajaId && m.Tipo == TipoMovimiento.Egreso)
                .SumAsync(m => m.Monto.Valor);

            var saldo = ingresos - egresos;
            return Result<decimal>.Ok(saldo);
        }
        catch (Exception ex)
        {
            return Result<decimal>.Fail("ERROR", ex.Message);
        }
    }

    public async Task<Result<IReadOnlyList<MovimientoCaja>>> GetByClienteAsync(int clienteId)
    {
        try
        {
            var movimientos = await _dbSet
                .Include(m => m.Caja)
                .Include(m => m.Cliente)
                .AsNoTracking()
                .Where(m => m.ClienteId == clienteId)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();
            return Result<IReadOnlyList<MovimientoCaja>>.Ok(movimientos);
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyList<MovimientoCaja>>.Fail("ERROR", ex.Message);
        }
    }
}
