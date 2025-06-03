using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicitAR.Core.Migrations.DbContext
{
    /// <inheritdoc />
    public partial class AutoMigration_LicitAR_20250601210007 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Licitaciones_IdEntidadLicitante",
                table: "Licitaciones",
                column: "IdEntidadLicitante");

            migrationBuilder.AddForeignKey(
                name: "FK_Licitaciones_EntidadesLicitantes_IdEntidadLicitante",
                table: "Licitaciones",
                column: "IdEntidadLicitante",
                principalTable: "EntidadesLicitantes",
                principalColumn: "IdEntidadLicitante");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Licitaciones_EntidadesLicitantes_IdEntidadLicitante",
                table: "Licitaciones");

            migrationBuilder.DropIndex(
                name: "IX_Licitaciones_IdEntidadLicitante",
                table: "Licitaciones");
        }
    }
}
