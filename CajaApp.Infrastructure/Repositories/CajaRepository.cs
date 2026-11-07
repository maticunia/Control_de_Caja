using CajaApp.Domain.Entities;
using CajaApp.Domain.Enums;
using CajaApp.Domain.Interfaces.Repositories;
using CajaApp.Domain.Shared;
using CajaApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CajaApp.Infrastructure.Repositories;

public class CajaRepository : RepositoryBase<Caja>, ICajaRepository
{
    public CajaRepository(AppDbContext context) : base(context) { }

    public async Task<Result<Caja?>> GetCajaAbiertaAsync()
    {
        try
        {
            var caja = await _dbSet
                .FirstOrDefaultAsync(c => c.Estado == EstadoCaja.Abierta);
            return Result<Caja?>.Ok(caja);
        }
        catch (Exception ex)
        {
            return Result<Caja?>.Fail("ERROR", ex.Message);
        }
    }

    public async Task<Result<IReadOnlyList<Caja>>> GetCajasConSaldoAsync()
    {
        try
        {
            var cajas = await _dbSet
                .Where(c => c.SaldoActual.Valor > 0)
                .ToListAsync();
            return Result<IReadOnlyList<Caja>>.Ok(cajas);
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyList<Caja>>.Fail("ERROR", ex.Message);
        }
    }
}
