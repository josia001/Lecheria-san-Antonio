using Lecheria.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lecheria.Infrastructure;

public class LecheriaDb(DbContextOptions<LecheriaDb> options)
    : DbContext(options)
{
    public DbSet<Alimentacion> Alimentaciones => Set<Alimentacion>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<Res>().HasIndex(r => r.Codigo).IsUnique();
        model.Entity<Res>().Property(r => r.Codigo).HasMaxLength(Res.LongitudMaximaCodigo).UseCollation("Latin1_General_100_CI_AS");
        model.Entity<Res>().Property(r => r.Peso).HasPrecision(18, 4);

        model.Entity<EventoSanitario>().ToTable("EventoSanitario", "dbo");
        model.Entity<EventoSanitario>()
            .HasOne<Res>()
            .WithMany()
            .HasForeignKey(evento => evento.ResId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public async Task InicializarAsync()
    {
        await Database.EnsureCreatedAsync();

        // EnsureCreated no agrega tablas a las bases de ganado que ya existen.
        await using var transaccion = await Database.BeginTransactionAsync();
        await Database.ExecuteSqlRawAsync("""
            IF OBJECT_ID(N'dbo.EventoSanitario', N'U') IS NULL
            BEGIN
                DECLARE @bloqueo int;
                EXEC @bloqueo = sp_getapplock
                    @Resource = N'Lecheria.EsquemaSanitario',
                    @LockMode = 'Exclusive',
                    @LockOwner = 'Transaction',
                    @LockTimeout = 15000;

                IF @bloqueo < 0
                    THROW 51000, 'No se pudo bloquear la actualizacion del esquema sanitario.', 1;

                IF OBJECT_ID(N'dbo.EventoSanitario', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.EventoSanitario (
                        Id int IDENTITY NOT NULL CONSTRAINT PK_EventoSanitario PRIMARY KEY,
                        ResId int NOT NULL,
                        Tipo int NOT NULL,
                        Fecha datetime2 NOT NULL,
                        Descripcion nvarchar(max) NOT NULL,
                        CONSTRAINT FK_EventoSanitario_Res_ResId
                            FOREIGN KEY (ResId) REFERENCES dbo.Res (Id)
                    );

                    CREATE INDEX IX_EventoSanitario_ResId ON dbo.EventoSanitario (ResId);
                END;
            END;
            """);
        await transaccion.CommitAsync();
    }
}