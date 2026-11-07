using CajaApp.Domain.Enums;
using CajaApp.Domain.ValueObjects;

namespace CajaApp.Domain.Entities;

public class Caja
{
    public int Id { get; private set; }
    public string Nombre { get; private set; }
    public string Descripcion { get; private set; }
    public Monto SaldoActual { get; private set; }
    public DateTime? FechaApertura { get; private set; }
    public DateTime? FechaCierre { get; private set; }
    public EstadoCaja Estado { get; private set; }
    public DateTime FechaCreacion { get; private set; }
    public byte[] Version { get; private set; } = Array.Empty<byte>();

    private Caja() { }

    public Caja(string nombre, string descripcion, Monto saldoInicial)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre es obligatorio", nameof(nombre));

        Nombre = nombre.Trim();
        Descripcion = descripcion?.Trim() ?? string.Empty;
        SaldoActual = saldoInicial;
        Estado = EstadoCaja.Cerrada;
        FechaCreacion = DateTime.UtcNow;
    }

    public Result Abrir(Monto saldoInicial)
    {
        if (Estado == EstadoCaja.Abierta)
            return Result.Fail("CAJA.YA_ABIERTA", "La caja ya está abierta");

        if (saldoInicial.Valor < 0)
            return Result.Fail("CAJA.SALDO_NEGATIVO", "El saldo inicial no puede ser negativo");

        SaldoActual = saldoInicial;
        FechaApertura = DateTime.UtcNow;
        FechaCierre = null;
        Estado = EstadoCaja.Abierta;

        return Result.Ok();
    }

    public Result Cerrar()
    {
        if (Estado == EstadoCaja.Cerrada)
            return Result.Fail("CAJA.YA_CERRADA", "La caja ya está cerrada");

        FechaCierre = DateTime.UtcNow;
        Estado = EstadoCaja.Cerrada;

        return Result.Ok();
    }

    public Result RegistrarMovimiento(TipoMovimiento tipo, Monto monto)
    {
        if (Estado == EstadoCaja.Cerrada)
            return Result.Fail("CAJA.CERRADA", "No se puede registrar movimientos en una caja cerrada");

        if (tipo == TipoMovimiento.Egreso)
        {
            if (SaldoActual.Valor < monto.Valor)
                return Result.Fail("CAJA.SALDO_INSUFICIENTE", "El saldo de la caja no puede quedar en negativo");

            SaldoActual = Monto.Create(SaldoActual.Valor - monto.Valor);
        }
        else
        {
            SaldoActual = Monto.Create(SaldoActual.Valor + monto.Valor);
        }

        return Result.Ok();
    }

    public void Actualizar(string nombre, string descripcion)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre es obligatorio", nameof(nombre));

        Nombre = nombre.Trim();
        Descripcion = descripcion?.Trim() ?? string.Empty;
    }

    public bool EstaAbierta => Estado == EstadoCaja.Abierta;
    public bool EstaCerrada => Estado == EstadoCaja.Cerrada;
}
