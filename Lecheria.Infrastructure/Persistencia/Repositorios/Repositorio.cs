using Lecheria.Application.Exception;
using Lecheria.Domain.Entities;
using Lecheria.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Lecheria.Infrastructure;

public class Repositorio<T>(LecheriaDb db) : IRepository<T> where T : EntityBase
{
    protected DbSet<T> Datos => db.Set<T>();
    protected virtual IQueryable<T> Consulta => Datos.AsNoTracking();

    public Task<T?> GetByIdAsync(int id) => Consulta.SingleOrDefaultAsync(x => x.Id == id);
    public async Task AddAsync(T entity)
    {
        Datos.Add(entity);
        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException e) when (e.InnerException is SqlException { Number: 2601 or 2627 or 547 or 2628 or 8152 or 8115 })
        {
            throw new AppException("El cambio entra en conflicto con un registro existente.");
        }
    }
}
