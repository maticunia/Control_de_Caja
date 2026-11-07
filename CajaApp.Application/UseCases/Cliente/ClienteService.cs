using CajaApp.Application.DTOs.Cliente;
using CajaApp.Application.Interfaces;
using CajaApp.Domain.Entities;
using CajaApp.Domain.Interfaces;
using CajaApp.Domain.Shared;

namespace CajaApp.Application.UseCases.Cliente;

public class ClienteService : IClienteService
{
    private readonly IUnitOfWork _unitOfWork;

    public ClienteService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<ClienteDto>> GetByIdAsync(int id)
    {
        var result = await _unitOfWork.Clientes.GetByIdAsync(id);
        if (!result.IsSuccess) return Result<ClienteDto>.Fail(result.Error!);
        
        var cliente = result.Value;
        if (cliente is null) return Result<ClienteDto>.Fail("CLIENTE.NOT_FOUND", "Cliente no encontrado");
        
        return MapToDto(cliente);
    }

    public async Task<Result<IReadOnlyList<ClienteDto>>> GetAllAsync()
    {
        var result = await _unitOfWork.Clientes.GetAllAsync();
        if (!result.IsSuccess) return Result<IReadOnlyList<ClienteDto>>.Fail(result.Error!);
        
        return Result<IReadOnlyList<ClienteDto>>.Ok(result.Value.Select(MapToDto).ToList());
    }

    public async Task<Result<ClienteDto>> CreateAsync(CreateClienteDto dto)
    {
        try
        {
            var cliente = new Cliente(dto.Nombre, dto.Apellidos, dto.Email, dto.Telefono, dto.Direccion);
            var result = await _unitOfWork.Clientes.AddAsync(cliente);
            
            if (!result.IsSuccess) return Result<ClienteDto>.Fail(result.Error!);
            
            await _unitOfWork.SaveChangesAsync();
            return MapToDto(result.Value);
        }
        catch (Exception ex)
        {
            return Result<ClienteDto>.Fail("CLIENTE.CREATE_ERROR", ex.Message);
        }
    }

    public async Task<Result<ClienteDto>> UpdateAsync(UpdateClienteDto dto)
    {
        var result = await _unitOfWork.Clientes.GetByIdAsync(dto.Id);
        if (!result.IsSuccess) return Result<ClienteDto>.Fail(result.Error!);
        
        var cliente = result.Value;
        if (cliente is null) return Result<ClienteDto>.Fail("CLIENTE.NOT_FOUND", "Cliente no encontrado");
        
        try
        {
            cliente.Actualizar(dto.Nombre, dto.Apellidos, dto.Email, dto.Telefono, dto.Direccion);
            var updateResult = await _unitOfWork.Clientes.UpdateAsync(cliente);
            
            if (!updateResult.IsSuccess) return Result<ClienteDto>.Fail(updateResult.Error!);
            
            await _unitOfWork.SaveChangesAsync();
            return MapToDto(updateResult.Value);
        }
        catch (Exception ex)
        {
            return Result<ClienteDto>.Fail("CLIENTE.UPDATE_ERROR", ex.Message);
        }
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var result = await _unitOfWork.Clientes.GetByIdAsync(id);
        if (!result.IsSuccess) return Result.Fail(result.Error!);
        
        var cliente = result.Value;
        if (cliente is null) return Result.Fail("CLIENTE.NOT_FOUND", "Cliente no encontrado");
        
        var deleteResult = await _unitOfWork.Clientes.DeleteAsync(id);
        if (!deleteResult.IsSuccess) return Result.Fail(deleteResult.Error!);
        
        await _unitOfWork.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<IReadOnlyList<ClienteDto>>> SearchAsync(string? nombre, string? email)
    {
        var result = await _unitOfWork.Clientes.SearchAsync(nombre, email);
        if (!result.IsSuccess) return Result<IReadOnlyList<ClienteDto>>.Fail(result.Error!);
        
        return Result<IReadOnlyList<ClienteDto>>.Ok(result.Value.Select(MapToDto).ToList());
    }

    private static ClienteDto MapToDto(Cliente cliente) => new ClienteDto(
        cliente.Id,
        cliente.Nombre,
        cliente.Apellidos,
        cliente.Email,
        cliente.Telefono,
        cliente.Direccion,
        cliente.FechaCreacion);
}
