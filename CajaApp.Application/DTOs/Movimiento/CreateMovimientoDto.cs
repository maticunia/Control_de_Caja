using CajaApp.Domain.Enums;

namespace CajaApp.Application.DTOs.Movimiento;

public record CreateMovimientoDto(
    int CajaId,
    TipoMovimiento Tipo,
    decimal Monto,
    string Descripcion,
    string? Categoria = null,
    int? ClienteId = null,
    string? Referencia = null);
