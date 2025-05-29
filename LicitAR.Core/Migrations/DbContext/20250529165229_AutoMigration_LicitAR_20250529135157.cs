using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicitAR.Core.Migrations.DbContext
{
    /// <inheritdoc />
    public partial class AutoMigration_LicitAR_20250529135157 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LicitacionEstadoHistorial",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdLicitacion = table.Column<int>(type: "int", nullable: false),
                    IdEstadoAnterior = table.Column<int>(type: "int", nullable: true),
                    IdEstadoNuevo = table.Column<int>(type: "int", nullable: false),
                    FechaCambio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdUsuarioCambio = table.Column<int>(type: "int", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicitacionEstadoHistorial", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LicitacionEstadoHistorial_EstadosLicitacion_IdEstadoAnterior",
                        column: x => x.IdEstadoAnterior,
                        principalTable: "EstadosLicitacion",
                        principalColumn: "IdEstadoLicitacion",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LicitacionEstadoHistorial_EstadosLicitacion_IdEstadoNuevo",
                        column: x => x.IdEstadoNuevo,
                        principalTable: "EstadosLicitacion",
                        principalColumn: "IdEstadoLicitacion",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LicitacionEstadoHistorial_Licitaciones_IdLicitacion",
                        column: x => x.IdLicitacion,
                        principalTable: "Licitaciones",
                        principalColumn: "IdLicitacion",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LicitacionEstadoHistorial_IdEstadoAnterior",
                table: "LicitacionEstadoHistorial",
                column: "IdEstadoAnterior");

            migrationBuilder.CreateIndex(
                name: "IX_LicitacionEstadoHistorial_IdEstadoNuevo",
                table: "LicitacionEstadoHistorial",
                column: "IdEstadoNuevo");

            migrationBuilder.CreateIndex(
                name: "IX_LicitacionEstadoHistorial_IdLicitacion",
                table: "LicitacionEstadoHistorial",
                column: "IdLicitacion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LicitacionEstadoHistorial");
        }
    }
}
