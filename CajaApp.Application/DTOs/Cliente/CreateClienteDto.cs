namespace CajaApp.Application.DTOs.Cliente;

public record CreateClienteDto(
    string Nombre,
    string Apellidos,
    string? Email,
    string? Telefono,
    string? Direccion);
