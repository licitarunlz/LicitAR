using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicitAR.Core.Migrations.DbContext
{
    /// <inheritdoc />
    public partial class AutoMigration_LicitAR_20250511193639 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Licitaciones_IdEstadoLicitacion",
                table: "Licitaciones",
                column: "IdEstadoLicitacion");

            migrationBuilder.AddForeignKey(
                name: "FK_Licitaciones_EstadosLicitacion_IdEstadoLicitacion",
                table: "Licitaciones",
                column: "IdEstadoLicitacion",
                principalTable: "EstadosLicitacion",
                principalColumn: "IdEstadoLicitacion",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Licitaciones_EstadosLicitacion_IdEstadoLicitacion",
                table: "Licitaciones");

            migrationBuilder.DropIndex(
                name: "IX_Licitaciones_IdEstadoLicitacion",
                table: "Licitaciones");
        }
    }
}
