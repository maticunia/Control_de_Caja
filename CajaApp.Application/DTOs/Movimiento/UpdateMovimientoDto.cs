namespace CajaApp.Application.DTOs.Movimiento;

public record UpdateMovimientoDto(
    int Id,
    string Descripcion,
    string? Categoria,
    string? Referencia);
