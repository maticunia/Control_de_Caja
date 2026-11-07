using CajaApp.Domain.Enums;
using CajaApp.Domain.ValueObjects;

namespace CajaApp.Domain.Entities;

public class MovimientoCaja
{
    public int Id { get; private set; }
    public int CajaId { get; private set; }
    public int? ClienteId { get; private set; }
    public TipoMovimiento Tipo { get; private set; }
    public Monto Monto { get; private set; }
    public string Descripcion { get; private set; }
    public string? Categoria { get; private set; }
    public DateTime Fecha { get; private set; }
    public string? Referencia { get; private set; }

    public virtual Caja? Caja { get; private set; }
    public virtual Cliente? Cliente { get; private set; }

    private MovimientoCaja() { }

    public MovimientoCaja(
        int cajaId,
        TipoMovimiento tipo,
        Monto monto,
        string descripcion,
        string? categoria = null,
        int? clienteId = null,
        string? referencia = null)
    {
        if (cajaId <= 0)
            throw new ArgumentException("El ID de caja es obligatorio", nameof(cajaId));

        if (monto.Valor <= 0)
            throw new ArgumentException("El monto debe ser mayor a cero", nameof(monto));

        if (string.IsNullOrWhiteSpace(descripcion))
            throw new ArgumentException("La descripción es obligatoria", nameof(descripcion));

        CajaId = cajaId;
        Tipo = tipo;
        Monto = monto;
        Descripcion = descripcion.Trim();
        Categoria = categoria?.Trim();
        ClienteId = clienteId;
        Referencia = referencia?.Trim();
        Fecha = DateTime.UtcNow;
    }

    public void AsignarCliente(int clienteId) => ClienteId = clienteId;

    public void Actualizar(string descripcion, string? categoria, string? referencia)
    {
        if (string.IsNullOrWhiteSpace(descripcion))
            throw new ArgumentException("La descripción es obligatoria", nameof(descripcion));

        Descripcion = descripcion.Trim();
        Categoria = categoria?.Trim();
        Referencia = referencia?.Trim();
    }

    public bool EsIngreso => Tipo == TipoMovimiento.Ingreso;
    public bool EsEgreso => Tipo == TipoMovimiento.Egreso;
}
