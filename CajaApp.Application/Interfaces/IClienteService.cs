using CajaApp.Application.DTOs.Cliente;
using CajaApp.Domain.Shared;

namespace CajaApp.Application.Interfaces;

public interface IClienteService
{
    Task<Result<ClienteDto>> GetByIdAsync(int id);
    Task<Result<IReadOnlyList<ClienteDto>>> GetAllAsync();
    Task<Result<ClienteDto>> CreateAsync(CreateClienteDto dto);
    Task<Result<ClienteDto>> UpdateAsync(UpdateClienteDto dto);
    Task<Result> DeleteAsync(int id);
    Task<Result<IReadOnlyList<ClienteDto>>> SearchAsync(string? nombre, string? email);
}
