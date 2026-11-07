namespace CajaApp.Application.DTOs.Cliente;

public record ClienteDto(
    int Id,
    string Nombre,
    string Apellidos,
    string? Email,
    string? Telefono,
    string? Direccion,
    DateTime FechaCreacion);
