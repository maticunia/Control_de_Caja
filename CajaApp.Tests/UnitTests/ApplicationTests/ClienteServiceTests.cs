using CajaApp.Application.DTOs.Cliente;
using CajaApp.Application.Interfaces;
using CajaApp.Tests.Fixtures;
using Xunit;

namespace CajaApp.Tests.UnitTests.ApplicationTests;

[Collection("CajaAppTests")]
public class ClienteServiceTests
{
    private readonly CollectionFixture _fixture;
    private readonly IClienteService _clienteService;

    public ClienteServiceTests(CollectionFixture fixture)
    {
        _fixture = fixture;
        _clienteService = fixture.Fixture.ServiceProvider.GetRequiredService<IClienteService>();
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsCliente()
    {
        var cliente = await _fixture.Fixture.CreateTestClienteAsync("Juan", "Perez");
        
        var result = await _clienteService.GetByIdAsync(cliente.Id);
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Juan", result.Value.Nombre);
        Assert.Equal("Perez", result.Value.Apellidos);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNotFound()
    {
        var result = await _clienteService.GetByIdAsync(999);
        
        Assert.False(result.IsSuccess);
        Assert.Equal("CLIENTE.NOT_FOUND", result.Error?.Code);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllClientes()
    {
        await _fixture.Fixture.CreateTestClienteAsync("Juan", "Perez");
        await _fixture.Fixture.CreateTestClienteAsync("Maria", "Gomez");
        await _fixture.Fixture.CreateTestClienteAsync("Carlos", "Rodriguez");
        
        var result = await _clienteService.GetAllAsync();
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(3, result.Value.Count);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_CreatesCliente()
    {
        var dto = new CreateClienteDto("Pedro", "Sanchez", "pedro@example.com", "123456", "Calle 123");
        
        var result = await _clienteService.CreateAsync(dto);
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Pedro", result.Value.Nombre);
        Assert.Equal("Sanchez", result.Value.Apellidos);
        Assert.Equal("pedro@example.com", result.Value.Email);
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_UpdatesCliente()
    {
        var cliente = await _fixture.Fixture.CreateTestClienteAsync("Juan", "Perez");
        var dto = new UpdateClienteDto(cliente.Id, "Juan", "Gomez", "juan.gomez@example.com", "987654", "Av. Principal");
        
        var result = await _clienteService.UpdateAsync(dto);
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Juan", result.Value.Nombre);
        Assert.Equal("Gomez", result.Value.Apellidos);
        Assert.Equal("juan.gomez@example.com", result.Value.Email);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesCliente()
    {
        var cliente = await _fixture.Fixture.CreateTestClienteAsync("To Delete", "Test");
        
        var result = await _clienteService.DeleteAsync(cliente.Id);
        
        Assert.True(result.IsSuccess);

        var checkResult = await _clienteService.GetByIdAsync(cliente.Id);
        Assert.False(checkResult.IsSuccess);
    }

    [Fact]
    public async Task SearchAsync_WithNombre_FiltersClientes()
    {
        await _fixture.Fixture.CreateTestClienteAsync("Juan", "Perez");
        await _fixture.Fixture.CreateTestClienteAsync("Maria", "Gomez");
        await _fixture.Fixture.CreateTestClienteAsync("Juan", "Rodriguez");
        
        var result = await _clienteService.SearchAsync("Juan", null);
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);
        Assert.All(result.Value, c => Assert.Contains("Juan", c.Nombre));
    }

    [Fact]
    public async Task SearchAsync_WithEmail_FiltersClientes()
    {
        var cliente1 = await _fixture.Fixture.CreateTestClienteAsync("Juan", "Perez");
        var cliente2 = await _fixture.Fixture.CreateTestClienteAsync("Maria", "Gomez");
        
        // Update one cliente with email
        var updateDto = new UpdateClienteDto(cliente1.Id, "Juan", "Perez", "juan@test.com", null, null);
        await _clienteService.UpdateAsync(updateDto);
        
        var result = await _clienteService.SearchAsync(null, "juan@test.com");
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value);
        Assert.Equal("juan@test.com", result.Value[0].Email);
    }
}
