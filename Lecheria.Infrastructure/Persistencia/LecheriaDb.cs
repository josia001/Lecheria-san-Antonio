using Lecheria.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lecheria.Infrastructure;

public class LecheriaDb(DbContextOptions<LecheriaDb> options)
    : DbContext(options)
{
    public DbSet<Alimentacion> Alimentaciones => Set<Alimentacion>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        base.OnModelCreating(model);

        // Configuración de Reses (HU-03 y HU-04)
        model.Entity<Res>()
            .HasIndex(r => r.Codigo)
            .IsUnique();

        model.Entity<Res>()
            .Property(r => r.Codigo)
            .HasMaxLength(Res.LongitudMaximaCodigo)
            .UseCollation("Latin1_General_100_CI_AS");

        model.Entity<Res>()
            .Property(r => r.Peso)
            .HasPrecision(18, 4);

        // Configuración de Alimentación (HU-06)
        model.Entity<Alimentacion>(entity =>
        {
            entity.ToTable("Alimentaciones");

            entity.HasKey(a => a.Id);

            entity.Property(a => a.TipoAlimento)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(a => a.Cantidad)
                .HasPrecision(18, 4)
                .IsRequired();

            entity.Property(a => a.UnidadMedida)
                .IsRequired()
                .HasMaxLength(30);

            entity.Property(a => a.FechaSuministro)
                .IsRequired();

            entity.HasOne(a => a.Res)
                .WithMany()
                .HasForeignKey(a => a.ResId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}