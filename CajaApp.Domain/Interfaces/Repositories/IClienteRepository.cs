using CajaApp.Domain.Entities;
using CajaApp.Domain.Shared;

namespace CajaApp.Domain.Interfaces.Repositories;

public interface IClienteRepository : IRepository<Cliente>
{
    Task<Result<IReadOnlyList<Cliente>>> SearchAsync(string? nombre, string? email);
    Task<Result<Cliente?>> GetByEmailAsync(string email);
}
