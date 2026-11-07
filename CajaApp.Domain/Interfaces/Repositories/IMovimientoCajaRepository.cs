using CajaApp.Domain.Entities;
using CajaApp.Domain.Enums;
using CajaApp.Domain.Shared;

namespace CajaApp.Domain.Interfaces.Repositories;

public interface IMovimientoCajaRepository : IRepository<MovimientoCaja>
{
    Task<Result<IReadOnlyList<MovimientoCaja>>> GetByCajaAsync(int cajaId);
    Task<Result<IReadOnlyList<MovimientoCaja>>> GetByFechaAsync(DateTime fechaInicio, DateTime fechaFin);
    Task<Result<IReadOnlyList<MovimientoCaja>>> GetByTipoAsync(TipoMovimiento tipo);
    Task<Result<decimal>> GetSaldoActualAsync(int cajaId);
    Task<Result<IReadOnlyList<MovimientoCaja>>> GetByClienteAsync(int clienteId);
}
