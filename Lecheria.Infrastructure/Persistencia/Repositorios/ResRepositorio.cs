using Lecheria.Application.Interfaces;
using Lecheria.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lecheria.Infrastructure;

public class ResRepositorio(LecheriaDb db) : Repositorio<Res>(db), IResRepository
{
    public Task<Res?> GetByCodigoAsync(string codigo) => Consulta.SingleOrDefaultAsync(r => r.Codigo == codigo);
}
