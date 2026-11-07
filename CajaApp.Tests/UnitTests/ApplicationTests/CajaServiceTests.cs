using CajaApp.Application.DTOs.Caja;
using CajaApp.Application.Interfaces;
using CajaApp.Domain.Entities;
using CajaApp.Domain.Enums;
using CajaApp.Domain.Shared;
using CajaApp.Tests.Fixtures;
using Moq;
using Xunit;

namespace CajaApp.Tests.UnitTests.ApplicationTests;

[Collection("CajaAppTests")]
public class CajaServiceTests
{
    private readonly CollectionFixture _fixture;
    private readonly ICajaService _cajaService;

    public CajaServiceTests(CollectionFixture fixture)
    {
        _fixture = fixture;
        _cajaService = fixture.Fixture.ServiceProvider.GetRequiredService<ICajaService>();
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsCaja()
    {
        var caja = await _fixture.Fixture.CreateTestCajaAsync("Test Caja", 100);
        
        var result = await _cajaService.GetByIdAsync(caja.Id);
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Test Caja", result.Value.Nombre);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNotFound()
    {
        var result = await _cajaService.GetByIdAsync(999);
        
        Assert.False(result.IsSuccess);
        Assert.Equal("CAJA.NOT_FOUND", result.Error?.Code);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCajas()
    {
        await _fixture.Fixture.CreateTestCajaAsync("Caja 1", 100);
        await _fixture.Fixture.CreateTestCajaAsync("Caja 2", 200);
        await _fixture.Fixture.CreateTestCajaAsync("Caja 3", 300);
        
        var result = await _cajaService.GetAllAsync();
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(3, result.Value.Count);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_CreatesCaja()
    {
        var dto = new CreateCajaDto("New Caja", "Description", 500);
        
        var result = await _cajaService.CreateAsync(dto);
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("New Caja", result.Value.Nombre);
        Assert.Equal("Description", result.Value.Descripcion);
        Assert.Equal(500m, result.Value.SaldoActual);
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_UpdatesCaja()
    {
        var caja = await _fixture.Fixture.CreateTestCajaAsync("Original", 100);
        var dto = new UpdateCajaDto(caja.Id, "Updated", "New Description");
        
        var result = await _cajaService.UpdateAsync(dto);
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Updated", result.Value.Nombre);
        Assert.Equal("New Description", result.Value.Descripcion);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesCaja()
    {
        var caja = await _fixture.Fixture.CreateTestCajaAsync("To Delete", 100);
        
        var result = await _cajaService.DeleteAsync(caja.Id);
        
        Assert.True(result.IsSuccess);

        var checkResult = await _cajaService.GetByIdAsync(caja.Id);
        Assert.False(checkResult.IsSuccess);
    }

    [Fact]
    public async Task AbrirCajaAsync_WithValidData_OpensCaja()
    {
        var caja = await _fixture.Fixture.CreateTestCajaAsync("To Open", 0);
        
        var result = await _cajaService.AbrirCajaAsync(caja.Id, 1000);
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(EstadoCaja.Abierta, result.Value.Estado);
        Assert.Equal(1000m, result.Value.SaldoActual);
    }

    [Fact]
    public async Task AbrirCajaAsync_WithAlreadyOpenCaja_Fails()
    {
        var caja = await _fixture.Fixture.CreateTestCajaAsync("Already Open", 0);
        await _cajaService.AbrirCajaAsync(caja.Id, 1000);
        
        var result = await _cajaService.AbrirCajaAsync(caja.Id, 500);
        
        Assert.False(result.IsSuccess);
        Assert.Equal("CAJA.YA_ABIERTA", result.Error?.Code);
    }

    [Fact]
    public async Task CerrarCajaAsync_WithOpenCaja_ClosesCaja()
    {
        var caja = await _fixture.Fixture.CreateTestCajaAsync("To Close", 0);
        await _cajaService.AbrirCajaAsync(caja.Id, 1000);
        
        var result = await _cajaService.CerrarCajaAsync(caja.Id);
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(EstadoCaja.Cerrada, result.Value.Estado);
    }

    [Fact]
    public async Task GetCajaAbiertaAsync_WithOpenCaja_ReturnsCaja()
    {
        var caja = await _fixture.Fixture.CreateTestCajaAsync("Open Caja", 0);
        await _cajaService.AbrirCajaAsync(caja.Id, 1000);
        
        var result = await _cajaService.GetCajaAbiertaAsync();
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(caja.Id, result.Value.Id);
    }

    [Fact]
    public async Task GetCajaAbiertaAsync_WithNoOpenCaja_ReturnsNull()
    {
        var result = await _cajaService.GetCajaAbiertaAsync();
        
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value);
    }
}
