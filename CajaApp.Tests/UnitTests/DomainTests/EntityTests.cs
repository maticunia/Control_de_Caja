using CajaApp.Domain.Entities;
using CajaApp.Domain.Enums;
using CajaApp.Domain.ValueObjects;
using Xunit;

namespace CajaApp.Tests.UnitTests.DomainTests;

public class EntityTests
{
    [Fact]
    public void Cliente_Creation_WithValidData_Succeeds()
    {
        var cliente = new Cliente("Juan", "Perez", "juan@example.com", "123456789", "Calle 123");
        
        Assert.Equal("Juan", cliente.Nombre);
        Assert.Equal("Perez", cliente.Apellidos);
        Assert.Equal("juan@example.com", cliente.Email);
        Assert.Equal("123456789", cliente.Telefono);
        Assert.Equal("Calle 123", cliente.Direccion);
        Assert.Equal("Juan Perez", cliente.NombreCompleto);
        Assert.NotEqual(DateTime.MinValue, cliente.FechaCreacion);
    }

    [Fact]
    public void Cliente_Creation_WithEmptyNombre_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => 
            new Cliente(string.Empty, "Perez", null, null, null));
    }

    [Fact]
    public void Cliente_Creation_WithEmptyApellidos_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => 
            new Cliente("Juan", string.Empty, null, null, null));
    }

    [Fact]
    public void Cliente_Actualizar_WithValidData_Succeeds()
    {
        var cliente = new Cliente("Juan", "Perez", "juan@example.com", "123", "Dir1");
        cliente.Actualizar("Juan", "Gomez", "juan.gomez@example.com", "456", "Dir2");
        
        Assert.Equal("Juan", cliente.Nombre);
        Assert.Equal("Gomez", cliente.Apellidos);
        Assert.Equal("juan.gomez@example.com", cliente.Email);
        Assert.Equal("456", cliente.Telefono);
        Assert.Equal("Dir2", cliente.Direccion);
    }

    [Fact]
    public void Caja_Creation_WithValidData_Succeeds()
    {
        var caja = new Caja("Caja Principal", "Descripción", Monto.Create(1000));
        
        Assert.Equal("Caja Principal", caja.Nombre);
        Assert.Equal("Descripción", caja.Descripcion);
        Assert.Equal(1000m, (decimal)caja.SaldoActual);
        Assert.Equal(EstadoCaja.Cerrada, caja.Estado);
        Assert.True(caja.EstaCerrada);
        Assert.False(caja.EstaAbierta);
    }

    [Fact]
    public void Caja_Abrir_WithValidSaldo_Succeeds()
    {
        var caja = new Caja("Caja 1", "Desc", Monto.Create(0));
        var result = caja.Abrir(Monto.Create(500));
        
        Assert.True(result.IsSuccess);
        Assert.Equal(EstadoCaja.Abierta, caja.Estado);
        Assert.Equal(500m, (decimal)caja.SaldoActual);
        Assert.True(caja.EstaAbierta);
        Assert.NotNull(caja.FechaApertura);
    }

    [Fact]
    public void Caja_Abrir_WhenAlreadyOpen_Fails()
    {
        var caja = new Caja("Caja 1", "Desc", Monto.Create(0));
        caja.Abrir(Monto.Create(500));
        
        var result = caja.Abrir(Monto.Create(100));
        
        Assert.False(result.IsSuccess);
        Assert.Equal("CAJA.YA_ABIERTA", result.Error?.Code);
    }

    [Fact]
    public void Caja_Cerrar_WhenOpen_Succeeds()
    {
        var caja = new Caja("Caja 1", "Desc", Monto.Create(0));
        caja.Abrir(Monto.Create(500));
        
        var result = caja.Cerrar();
        
        Assert.True(result.IsSuccess);
        Assert.Equal(EstadoCaja.Cerrada, caja.Estado);
        Assert.True(caja.EstaCerrada);
        Assert.NotNull(caja.FechaCierre);
    }

    [Fact]
    public void Caja_Cerrar_WhenClosed_Fails()
    {
        var caja = new Caja("Caja 1", "Desc", Monto.Create(0));
        
        var result = caja.Cerrar();
        
        Assert.False(result.IsSuccess);
        Assert.Equal("CAJA.YA_CERRADA", result.Error?.Code);
    }

    [Fact]
    public void Caja_RegistrarMovimiento_Ingreso_WhenOpen_Succeeds()
    {
        var caja = new Caja("Caja 1", "Desc", Monto.Create(100));
        caja.Abrir(Monto.Create(100));
        
        var result = caja.RegistrarMovimiento(TipoMovimiento.Ingreso, Monto.Create(50));
        
        Assert.True(result.IsSuccess);
        Assert.Equal(150m, (decimal)caja.SaldoActual);
    }

    [Fact]
    public void Caja_RegistrarMovimiento_WhenClosed_Fails()
    {
        var caja = new Caja("Caja 1", "Desc", Monto.Create(100));
        
        var result = caja.RegistrarMovimiento(TipoMovimiento.Ingreso, Monto.Create(50));
        
        Assert.False(result.IsSuccess);
        Assert.Equal("CAJA.CERRADA", result.Error?.Code);
    }

    [Fact]
    public void Caja_RegistrarMovimiento_Egreso_WhenOpen_WithSufficientSaldo_Succeeds()
    {
        var caja = new Caja("Caja 1", "Desc", Monto.Create(100));
        caja.Abrir(Monto.Create(100));
        
        var result = caja.RegistrarMovimiento(TipoMovimiento.Egreso, Monto.Create(50));
        
        Assert.True(result.IsSuccess);
        Assert.Equal(50m, (decimal)caja.SaldoActual);
    }

    [Fact]
    public void Caja_RegistrarMovimiento_Egreso_WhenOpen_WithInsufficientSaldo_Fails()
    {
        var caja = new Caja("Caja 1", "Desc", Monto.Create(50));
        caja.Abrir(Monto.Create(50));
        
        var result = caja.RegistrarMovimiento(TipoMovimiento.Egreso, Monto.Create(100));
        
        Assert.False(result.IsSuccess);
        Assert.Equal("CAJA.SALDO_INSUFICIENTE", result.Error?.Code);
    }

    [Fact]
    public void MovimientoCaja_Creation_WithValidData_Succeeds()
    {
        var movimiento = new MovimientoCaja(
            1, 
            TipoMovimiento.Ingreso, 
            Monto.Create(100), 
            "Venta", 
            "Ventas",
            1,
            "REF-001");
        
        Assert.Equal(1, movimiento.CajaId);
        Assert.Equal(TipoMovimiento.Ingreso, movimiento.Tipo);
        Assert.Equal(100m, (decimal)movimiento.Monto);
        Assert.Equal("Venta", movimiento.Descripcion);
        Assert.Equal("Ventas", movimiento.Categoria);
        Assert.Equal(1, movimiento.ClienteId);
        Assert.Equal("REF-001", movimiento.Referencia);
        Assert.True(movimiento.EsIngreso);
        Assert.False(movimiento.EsEgreso);
        Assert.NotEqual(DateTime.MinValue, movimiento.Fecha);
    }

    [Fact]
    public void MovimientoCaja_Creation_WithInvalidCajaId_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => 
            new MovimientoCaja(0, TipoMovimiento.Ingreso, Monto.Create(100), "Test"));
    }

    [Fact]
    public void MovimientoCaja_Creation_WithZeroMonto_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => 
            new MovimientoCaja(1, TipoMovimiento.Ingreso, Monto.Create(0), "Test"));
    }

    [Fact]
    public void MovimientoCaja_Creation_WithEmptyDescripcion_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => 
            new MovimientoCaja(1, TipoMovimiento.Ingreso, Monto.Create(100), string.Empty));
    }

    [Fact]
    public void Monto_Creation_WithValidValue_Succeeds()
    {
        var monto = Monto.Create(100);
        
        Assert.Equal(100m, monto.Valor);
    }

    [Fact]
    public void Monto_Creation_WithNegativeValue_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => Monto.Create(-100));
    }

    [Fact]
    public void Monto_ImplicitConversion_ToDecimal()
    {
        Monto monto = 100m;
        decimal value = monto;
        
        Assert.Equal(100m, value);
    }

    [Fact]
    public void Monto_ImplicitConversion_FromDecimal()
    {
        decimal value = 100m;
        Monto monto = value;
        
        Assert.Equal(100m, monto.Valor);
    }
}
