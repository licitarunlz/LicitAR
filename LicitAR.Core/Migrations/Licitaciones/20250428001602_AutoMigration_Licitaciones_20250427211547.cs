using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicitAR.Core.Migrations.Licitaciones
{
    /// <inheritdoc />
    public partial class AutoMigration_Licitaciones_20250427211547 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Licitaciones",
                columns: table => new
                {
                    IdEntidadLicitante = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdLicitacion = table.Column<int>(type: "int", nullable: false),
                    CodigoLicitacion = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    FechaPublicacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdEstadoLicitacion = table.Column<int>(type: "int", nullable: false),
                    IdCategoriaLicitacion = table.Column<int>(type: "int", nullable: false),
                    Audit_IdUsuarioAlta = table.Column<int>(type: "int", nullable: false),
                    Audit_FechaAlta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Audit_IdUsuarioModificacion = table.Column<int>(type: "int", nullable: true),
                    Audit_FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Audit_IdUsuarioBaja = table.Column<int>(type: "int", nullable: true),
                    Audit_FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Licitaciones", x => x.IdEntidadLicitante);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Licitaciones");
        }
    }
}
