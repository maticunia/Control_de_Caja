using CajaApp.Application.DTOs.Movimiento;
using CajaApp.Application.Interfaces;
using CajaApp.Domain.Enums;
using CajaApp.Tests.Fixtures;
using Xunit;

namespace CajaApp.Tests.UnitTests.ApplicationTests;

[Collection("CajaAppTests")]
public class MovimientoServiceTests
{
    private readonly CollectionFixture _fixture;
    private readonly IMovimientoService _movimientoService;
    private readonly ICajaService _cajaService;
    private readonly IClienteService _clienteService;

    public MovimientoServiceTests(CollectionFixture fixture)
    {
        _fixture = fixture;
        _movimientoService = fixture.Fixture.ServiceProvider.GetRequiredService<IMovimientoService>();
        _cajaService = fixture.Fixture.ServiceProvider.GetRequiredService<ICajaService>();
        _clienteService = fixture.Fixture.ServiceProvider.GetRequiredService<IClienteService>();
    }

    [Fact]
    public async Task CreateAsync_WithValidData_CreatesMovimiento()
    {
        var caja = await _fixture.Fixture.CreateTestCajaAsync("Test Caja", 1000);
        await _cajaService.AbrirCajaAsync(caja.Id, 1000);
        
        var dto = new CreateMovimientoDto(
            caja.Id,
            TipoMovimiento.Ingreso,
            500,
            "Venta de producto",
            "Ventas",
            null,
            "REF-001");
        
        var result = await _movimientoService.CreateAsync(dto);
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(caja.Id, result.Value.CajaId);
        Assert.Equal(TipoMovimiento.Ingreso, result.Value.Tipo);
        Assert.Equal(500m, result.Value.Monto);
        Assert.Equal("Venta de producto", result.Value.Descripcion);
    }

    [Fact]
    public async Task CreateAsync_WithClosedCaja_Fails()
    {
        var caja = await _fixture.Fixture.CreateTestCajaAsync("Closed Caja", 1000);
        
        var dto = new CreateMovimientoDto(
            caja.Id,
            TipoMovimiento.Ingreso,
            500,
            "Test");
        
        var result = await _movimientoService.CreateAsync(dto);
        
        Assert.False(result.IsSuccess);
        Assert.Equal("CAJA.CERRADA", result.Error?.Code);
    }

    [Fact]
    public async Task GetByCajaAsync_WithValidCajaId_ReturnsMovimientos()
    {
        var caja = await _fixture.Fixture.CreateTestCajaAsync("Test Caja", 1000);
        await _cajaService.AbrirCajaAsync(caja.Id, 1000);
        
        await _movimientoService.CreateAsync(new CreateMovimientoDto(
            caja.Id, TipoMovimiento.Ingreso, 100, "Mov 1"));
        await _movimientoService.CreateAsync(new CreateMovimientoDto(
            caja.Id, TipoMovimiento.Egreso, 50, "Mov 2"));
        
        var result = await _movimientoService.GetByCajaAsync(caja.Id);
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);
    }

    [Fact]
    public async Task GetByTipoAsync_WithValidTipo_ReturnsMovimientos()
    {
        var caja = await _fixture.Fixture.CreateTestCajaAsync("Test Caja", 1000);
        await _cajaService.AbrirCajaAsync(caja.Id, 1000);
        
        await _movimientoService.CreateAsync(new CreateMovimientoDto(
            caja.Id, TipoMovimiento.Ingreso, 100, "Ingreso 1"));
        await _movimientoService.CreateAsync(new CreateMovimientoDto(
            caja.Id, TipoMovimiento.Ingreso, 200, "Ingreso 2"));
        await _movimientoService.CreateAsync(new CreateMovimientoDto(
            caja.Id, TipoMovimiento.Egreso, 50, "Egreso 1"));
        
        var result = await _movimientoService.GetByTipoAsync(TipoMovimiento.Ingreso);
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);
        Assert.All(result.Value, m => Assert.Equal(TipoMovimiento.Ingreso, m.Tipo));
    }

    [Fact]
    public async Task GetSaldoActualAsync_WithMovimientos_ReturnsCorrectSaldo()
    {
        var caja = await _fixture.Fixture.CreateTestCajaAsync("Test Caja", 1000);
        await _cajaService.AbrirCajaAsync(caja.Id, 1000);
        
        await _movimientoService.CreateAsync(new CreateMovimientoDto(
            caja.Id, TipoMovimiento.Ingreso, 500, "Ingreso"));
        await _movimientoService.CreateAsync(new CreateMovimientoDto(
            caja.Id, TipoMovimiento.Egreso, 200, "Egreso"));
        
        var result = await _movimientoService.GetSaldoActualAsync(caja.Id);
        
        Assert.True(result.IsSuccess);
        Assert.Equal(1300m, result.Value);
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_UpdatesMovimiento()
    {
        var caja = await _fixture.Fixture.CreateTestCajaAsync("Test Caja", 1000);
        await _cajaService.AbrirCajaAsync(caja.Id, 1000);
        
        var createResult = await _movimientoService.CreateAsync(new CreateMovimientoDto(
            caja.Id, TipoMovimiento.Ingreso, 100, "Original"));
        
        var updateDto = new UpdateMovimientoDto(
            createResult.Value.Id,
            "Updated",
            "New Category",
            "New Ref");
        
        var result = await _movimientoService.UpdateAsync(updateDto);
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Updated", result.Value.Descripcion);
        Assert.Equal("New Category", result.Value.Categoria);
        Assert.Equal("New Ref", result.Value.Referencia);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesMovimiento()
    {
        var caja = await _fixture.Fixture.CreateTestCajaAsync("Test Caja", 1000);
        await _cajaService.AbrirCajaAsync(caja.Id, 1000);
        
        var createResult = await _movimientoService.CreateAsync(new CreateMovimientoDto(
            caja.Id, TipoMovimiento.Ingreso, 100, "To Delete"));
        
        var result = await _movimientoService.DeleteAsync(createResult.Value.Id);
        
        Assert.True(result.IsSuccess);

        var checkResult = await _movimientoService.GetByIdAsync(createResult.Value.Id);
        Assert.False(checkResult.IsSuccess);
    }

    [Fact]
    public async Task CreateAsync_WithCliente_AssociatesMovimiento()
    {
        var caja = await _fixture.Fixture.CreateTestCajaAsync("Test Caja", 1000);
        await _cajaService.AbrirCajaAsync(caja.Id, 1000);
        
        var cliente = await _fixture.Fixture.CreateTestClienteAsync("Juan", "Perez");
        
        var dto = new CreateMovimientoDto(
            caja.Id,
            TipoMovimiento.Ingreso,
            500,
            "Venta",
            null,
            cliente.Id,
            null);
        
        var result = await _movimientoService.CreateAsync(dto);
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(cliente.Id, result.Value.ClienteId);
    }

    [Fact]
    public async Task GetByClienteAsync_WithValidClienteId_ReturnsMovimientos()
    {
        var caja = await _fixture.Fixture.CreateTestCajaAsync("Test Caja", 1000);
        await _cajaService.AbrirCajaAsync(caja.Id, 1000);
        
        var cliente = await _fixture.Fixture.CreateTestClienteAsync("Juan", "Perez");
        
        await _movimientoService.CreateAsync(new CreateMovimientoDto(
            caja.Id, TipoMovimiento.Ingreso, 100, "Mov 1", null, cliente.Id));
        await _movimientoService.CreateAsync(new CreateMovimientoDto(
            caja.Id, TipoMovimiento.Ingreso, 200, "Mov 2", null, cliente.Id));
        
        var result = await _movimientoService.GetByClienteAsync(cliente.Id);
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);
        Assert.All(result.Value, m => Assert.Equal(cliente.Id, m.ClienteId));
    }
}
