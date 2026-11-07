using CajaApp.Domain.Entities;
using CajaApp.Domain.Shared;

namespace CajaApp.Domain.Interfaces.Repositories;

public interface ICajaRepository : IRepository<Caja>
{
    Task<Result<Caja?>> GetCajaAbiertaAsync();
    Task<Result<IReadOnlyList<Caja>>> GetCajasConSaldoAsync();
}
