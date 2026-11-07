using CajaApp.Domain.Shared;

namespace CajaApp.Domain.Interfaces.Repositories;

public interface IRepository<T> where T : class
{
    Task<Result<T>> GetByIdAsync(int id);
    Task<Result<IReadOnlyList<T>>> GetAllAsync();
    Task<Result<T>> AddAsync(T entity);
    Task<Result<T>> UpdateAsync(T entity);
    Task<Result> DeleteAsync(int id);
}
