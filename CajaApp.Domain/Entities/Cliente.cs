namespace CajaApp.Domain.Entities;

public class Cliente
{
    public int Id { get; private set; }
    public string Nombre { get; private set; }
    public string Apellidos { get; private set; }
    public string? Email { get; private set; }
    public string? Telefono { get; private set; }
    public string? Direccion { get; private set; }
    public DateTime FechaCreacion { get; private set; }

    private Cliente() { }

    public Cliente(string nombre, string apellidos, string? email, string? telefono, string? direccion)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre es obligatorio", nameof(nombre));
        
        if (string.IsNullOrWhiteSpace(apellidos))
            throw new ArgumentException("Los apellidos son obligatorios", nameof(apellidos));

        Nombre = nombre.Trim();
        Apellidos = apellidos.Trim();
        Email = email?.Trim();
        Telefono = telefono?.Trim();
        Direccion = direccion?.Trim();
        FechaCreacion = DateTime.UtcNow;
    }

    public void Actualizar(string nombre, string apellidos, string? email, string? telefono, string? direccion)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre es obligatorio", nameof(nombre));
        
        if (string.IsNullOrWhiteSpace(apellidos))
            throw new ArgumentException("Los apellidos son obligatorios", nameof(apellidos));

        Nombre = nombre.Trim();
        Apellidos = apellidos.Trim();
        Email = email?.Trim();
        Telefono = telefono?.Trim();
        Direccion = direccion?.Trim();
    }

    public string NombreCompleto => $"{Nombre} {Apellidos}";
}
