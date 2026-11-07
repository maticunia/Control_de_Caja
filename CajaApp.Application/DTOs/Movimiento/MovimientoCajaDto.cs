using CajaApp.Domain.Enums;

namespace CajaApp.Application.DTOs.Movimiento;

public record MovimientoCajaDto(
    int Id,
    int CajaId,
    string CajaNombre,
    int? ClienteId,
    string? ClienteNombre,
    TipoMovimiento Tipo,
    decimal Monto,
    string Descripcion,
    string? Categoria,
    DateTime Fecha,
    string? Referencia);
