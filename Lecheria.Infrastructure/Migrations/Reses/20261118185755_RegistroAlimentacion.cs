using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lecheria.Infrastructure.Migrations.Reses
{
    /// <inheritdoc />
    public partial class RegistroAlimentacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alimentaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResId = table.Column<int>(type: "int", nullable: false),
                    TipoAlimento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UnidadMedida = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FechaSuministro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alimentaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alimentaciones_Res_ResId",
                        column: x => x.ResId,
                        principalTable: "Res",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alimentaciones_ResId",
                table: "Alimentaciones",
                column: "ResId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alimentaciones");
        }
    }
}
