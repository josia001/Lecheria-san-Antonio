using Lecheria.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lecheria.Infrastructure;

public class LecheriaDb(DbContextOptions<LecheriaDb> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<Res>().HasIndex(r => r.Codigo).IsUnique();
        model.Entity<Res>().Property(r => r.Codigo).HasMaxLength(Res.LongitudMaximaCodigo).UseCollation("Latin1_General_100_CI_AS");
        model.Entity<Res>().Property(r => r.Peso).HasPrecision(18, 4);
    }
}
