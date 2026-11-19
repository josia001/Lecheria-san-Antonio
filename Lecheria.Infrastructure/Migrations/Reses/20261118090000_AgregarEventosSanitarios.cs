using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lecheria.Infrastructure.Migrations.Reses
{
    public partial class AgregarEventosSanitarios : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "EventoSanitario", schema: "dbo");
        }
    }
}
