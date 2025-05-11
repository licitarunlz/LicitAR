using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicitAR.Core.Migrations.DbContext
{
    /// <inheritdoc />
    public partial class AutoMigration_LicitAR_20250511003811 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PersonaUsuario",
                columns: table => new
                {
                    IdPersona = table.Column<int>(type: "int", nullable: false),
                    IdUsuario = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Audit_IdUsuarioAlta = table.Column<int>(type: "int", nullable: false),
                    Audit_FechaAlta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Audit_IdUsuarioModificacion = table.Column<int>(type: "int", nullable: true),
                    Audit_FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Audit_IdUsuarioBaja = table.Column<int>(type: "int", nullable: true),
                    Audit_FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonaUsuario", x => new { x.IdPersona, x.IdUsuario });
                    table.ForeignKey(
                        name: "FK_PersonaUsuario_AspNetUsers_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonaUsuario_Personas_IdPersona",
                        column: x => x.IdPersona,
                        principalTable: "Personas",
                        principalColumn: "IdPersona",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rubro",
                columns: table => new
                {
                    IdRubro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Audit_IdUsuarioAlta = table.Column<int>(type: "int", nullable: false),
                    Audit_FechaAlta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Audit_IdUsuarioModificacion = table.Column<int>(type: "int", nullable: true),
                    Audit_FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Audit_IdUsuarioBaja = table.Column<int>(type: "int", nullable: true),
                    Audit_FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rubro", x => x.IdRubro);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersonaUsuario_IdUsuario",
                table: "PersonaUsuario",
                column: "IdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonaUsuario");

            migrationBuilder.DropTable(
                name: "Rubro");
        }
    }
}
