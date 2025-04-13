using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicitAR.Core.Migrations.Actores
{
    /// <inheritdoc />
    public partial class AutoMigration_Actores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EntidadesLicitantes",
                columns: table => new
                {
                    IdEntidadLicitante = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cuit = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    RazonSocial = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AuditTable_IdUsuarioAlta = table.Column<int>(type: "int", nullable: false),
                    AuditTable_FechaAlta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AuditTable_IdUsuarioModificacion = table.Column<int>(type: "int", nullable: true),
                    AuditTable_FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AuditTable_IdUsuarioBaja = table.Column<int>(type: "int", nullable: true),
                    AuditTable_FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdProvincia = table.Column<int>(type: "int", nullable: false),
                    IdLocalidad = table.Column<int>(type: "int", nullable: false),
                    DireccionBarrio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DireccionCalle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DireccionNumero = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DireccionPiso = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DireccionDepto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DireccionCodigoPostal = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntidadesLicitantes", x => x.IdEntidadLicitante);
                });

            migrationBuilder.CreateTable(
                name: "Personas",
                columns: table => new
                {
                    IdPersona = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RazonSocial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdTipoPersona = table.Column<int>(type: "int", nullable: false),
                    Cuit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuditTable_IdUsuarioAlta = table.Column<int>(type: "int", nullable: false),
                    AuditTable_FechaAlta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AuditTable_IdUsuarioModificacion = table.Column<int>(type: "int", nullable: true),
                    AuditTable_FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AuditTable_IdUsuarioBaja = table.Column<int>(type: "int", nullable: true),
                    AuditTable_FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdProvincia = table.Column<int>(type: "int", nullable: false),
                    IdLocalidad = table.Column<int>(type: "int", nullable: false),
                    DireccionBarrio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DireccionCalle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DireccionNumero = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DireccionPiso = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DireccionDepto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DireccionCodigoPostal = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personas", x => x.IdPersona);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntidadesLicitantes");

            migrationBuilder.DropTable(
                name: "Personas");
        }
    }
}
