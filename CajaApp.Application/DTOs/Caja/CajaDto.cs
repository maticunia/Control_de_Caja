using CajaApp.Domain.Enums;

namespace CajaApp.Application.DTOs.Caja;

public record CajaDto(
    int Id,
    string Nombre,
    string Descripcion,
    decimal SaldoActual,
    DateTime? FechaApertura,
    DateTime? FechaCierre,
    EstadoCaja Estado,
    DateTime FechaCreacion);
