using CajaApp.Application.DTOs.Movimiento;
using CajaApp.Domain.Enums;
using CajaApp.Domain.Shared;

namespace CajaApp.Application.Interfaces;

public interface IMovimientoService
{
    Task<Result<MovimientoCajaDto>> GetByIdAsync(int id);
    Task<Result<IReadOnlyList<MovimientoCajaDto>>> GetAllAsync();
    Task<Result<MovimientoCajaDto>> CreateAsync(CreateMovimientoDto dto);
    Task<Result<MovimientoCajaDto>> UpdateAsync(UpdateMovimientoDto dto);
    Task<Result> DeleteAsync(int id);
    Task<Result<IReadOnlyList<MovimientoCajaDto>>> GetByCajaAsync(int cajaId);
    Task<Result<IReadOnlyList<MovimientoCajaDto>>> GetByFechaAsync(DateTime fechaInicio, DateTime fechaFin);
    Task<Result<IReadOnlyList<MovimientoCajaDto>>> GetByTipoAsync(TipoMovimiento tipo);
    Task<Result<decimal>> GetSaldoActualAsync(int cajaId);
    Task<Result<IReadOnlyList<MovimientoCajaDto>>> GetByClienteAsync(int clienteId);
}
