using Lecheria.Domain.Entities;
using Lecheria.Domain.Interfaces;

namespace Lecheria.Application.Interfaces;

public interface IResRepository : IRepository<Res>
{
    Task<Res?> GetByCodigoAsync(string codigo);
}
