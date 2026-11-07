using CajaApp.Domain.Entities;
using CajaApp.Domain.Interfaces.Repositories;
using CajaApp.Domain.Shared;
using CajaApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CajaApp.Infrastructure.Repositories;

public class ClienteRepository : RepositoryBase<Cliente>, IClienteRepository
{
    public ClienteRepository(AppDbContext context) : base(context) { }

    public async Task<Result<IReadOnlyList<Cliente>>> SearchAsync(string? nombre, string? email)
    {
        try
        {
            var query = _dbSet.AsQueryable();

            if (!string.IsNullOrWhiteSpace(nombre))
            {
                query = query.Where(c => 
                    c.Nombre.Contains(nombre) || 
                    c.Apellidos.Contains(nombre) ||
                    (c.Nombre + " " + c.Apellidos).Contains(nombre));
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                query = query.Where(c => c.Email != null && c.Email.Contains(email));
            }

            var clientes = await query.ToListAsync();
            return Result<IReadOnlyList<Cliente>>.Ok(clientes);
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyList<Cliente>>.Fail("ERROR", ex.Message);
        }
    }

    public async Task<Result<Cliente?>> GetByEmailAsync(string email)
    {
        try
        {
            var normalized = email.Trim().ToLower();
            var cliente = await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Email != null && c.Email.ToLower() == normalized);
            return Result<Cliente?>.Ok(cliente);
        }
        catch (Exception ex)
        {
            return Result<Cliente?>.Fail("ERROR", ex.Message);
        }
    }
}
