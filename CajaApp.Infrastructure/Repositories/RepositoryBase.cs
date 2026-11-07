using CajaApp.Domain.Interfaces.Repositories;
using CajaApp.Domain.Shared;
using CajaApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CajaApp.Infrastructure.Repositories;

public class RepositoryBase<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public RepositoryBase(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<Result<T>> GetByIdAsync(int id)
    {
        try
        {
            var entity = await _dbSet.FindAsync(id);
            return entity is null 
                ? Result<T>.Fail("NOT_FOUND", $"{typeof(T).Name} no encontrado")
                : Result<T>.Ok(entity);
        }
        catch (Exception ex)
        {
            return Result<T>.Fail("ERROR", ex.Message);
        }
    }

    public virtual async Task<Result<IReadOnlyList<T>>> GetAllAsync()
    {
        try
        {
            var entities = await _dbSet.ToListAsync();
            return Result<IReadOnlyList<T>>.Ok(entities);
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyList<T>>.Fail("ERROR", ex.Message);
        }
    }

    public virtual async Task<Result<T>> AddAsync(T entity)
    {
        try
        {
            await _dbSet.AddAsync(entity);
            return Result<T>.Ok(entity);
        }
        catch (Exception ex)
        {
            return Result<T>.Fail("ERROR", ex.Message);
        }
    }

    public virtual async Task<Result<T>> UpdateAsync(T entity)
    {
        try
        {
            _dbSet.Update(entity);
            return Result<T>.Ok(entity);
        }
        catch (Exception ex)
        {
            return Result<T>.Fail("ERROR", ex.Message);
        }
    }

    public virtual async Task<Result> DeleteAsync(int id)
    {
        try
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity is null)
                return Result.Fail("NOT_FOUND", $"{typeof(T).Name} no encontrado");

            _dbSet.Remove(entity);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail("ERROR", ex.Message);
        }
    }
}
