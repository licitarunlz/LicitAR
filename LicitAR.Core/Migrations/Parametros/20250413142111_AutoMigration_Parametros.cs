using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicitAR.Core.Migrations.Parametros
{
    /// <inheritdoc />
    public partial class AutoMigration_Parametros : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Localidades",
                columns: table => new
                {
                    IdLocalidad = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdProvincia = table.Column<int>(type: "int", nullable: false),
                    AuditTable_IdUsuarioAlta = table.Column<int>(type: "int", nullable: false),
                    AuditTable_FechaAlta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AuditTable_IdUsuarioModificacion = table.Column<int>(type: "int", nullable: true),
                    AuditTable_FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AuditTable_IdUsuarioBaja = table.Column<int>(type: "int", nullable: true),
                    AuditTable_FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localidades", x => x.IdLocalidad);
                });

            migrationBuilder.CreateTable(
                name: "Provincias",
                columns: table => new
                {
                    IdProvincia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuditTable_IdUsuarioAlta = table.Column<int>(type: "int", nullable: false),
                    AuditTable_FechaAlta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AuditTable_IdUsuarioModificacion = table.Column<int>(type: "int", nullable: true),
                    AuditTable_FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AuditTable_IdUsuarioBaja = table.Column<int>(type: "int", nullable: true),
                    AuditTable_FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provincias", x => x.IdProvincia);
                });

            migrationBuilder.CreateTable(
                name: "TiposContacto",
                columns: table => new
                {
                    IdTipoContacto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuditTable_IdUsuarioAlta = table.Column<int>(type: "int", nullable: false),
                    AuditTable_FechaAlta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AuditTable_IdUsuarioModificacion = table.Column<int>(type: "int", nullable: true),
                    AuditTable_FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AuditTable_IdUsuarioBaja = table.Column<int>(type: "int", nullable: true),
                    AuditTable_FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposContacto", x => x.IdTipoContacto);
                });

            migrationBuilder.CreateTable(
                name: "TiposPersona",
                columns: table => new
                {
                    IdTipoPersona = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuditTable_IdUsuarioAlta = table.Column<int>(type: "int", nullable: false),
                    AuditTable_FechaAlta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AuditTable_IdUsuarioModificacion = table.Column<int>(type: "int", nullable: true),
                    AuditTable_FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AuditTable_IdUsuarioBaja = table.Column<int>(type: "int", nullable: true),
                    AuditTable_FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposPersona", x => x.IdTipoPersona);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Localidades");

            migrationBuilder.DropTable(
                name: "Provincias");

            migrationBuilder.DropTable(
                name: "TiposContacto");

            migrationBuilder.DropTable(
                name: "TiposPersona");
        }
    }
}
