
using Lecheria.Domain.Entities;

namespace Lecheria.Domain.Interfaces;

public interface IRepository<T> where T : EntityBase
{
    Task<T?> GetByIdAsync(int id);

    Task AddAsync(T entity);
}
