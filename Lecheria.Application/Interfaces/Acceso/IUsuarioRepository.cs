using Lecheria.Domain.Entities;
using Lecheria.Domain.Interfaces;

namespace Lecheria.Application.Interfaces;

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> GetByNombreUsuarioAsync(string nombreUsuario);
}
