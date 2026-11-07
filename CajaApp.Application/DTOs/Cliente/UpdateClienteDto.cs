namespace CajaApp.Application.DTOs.Cliente;

public record UpdateClienteDto(
    int Id,
    string Nombre,
    string Apellidos,
    string? Email,
    string? Telefono,
    string? Direccion);
