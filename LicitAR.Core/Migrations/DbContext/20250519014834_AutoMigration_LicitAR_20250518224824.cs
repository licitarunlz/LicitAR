using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicitAR.Core.Migrations.DbContext
{
    /// <inheritdoc />
    public partial class AutoMigration_LicitAR_20250518224824 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OfertasDetalle_LicitacionesDetalle_IdLicitacionDetalle",
                table: "OfertasDetalle");

            migrationBuilder.AddForeignKey(
                name: "FK_OfertasDetalle_LicitacionesDetalle_IdLicitacionDetalle",
                table: "OfertasDetalle",
                column: "IdLicitacionDetalle",
                principalTable: "LicitacionesDetalle",
                principalColumn: "IdLicitacionDetalle",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OfertasDetalle_LicitacionesDetalle_IdLicitacionDetalle",
                table: "OfertasDetalle");

            migrationBuilder.AddForeignKey(
                name: "FK_OfertasDetalle_LicitacionesDetalle_IdLicitacionDetalle",
                table: "OfertasDetalle",
                column: "IdLicitacionDetalle",
                principalTable: "LicitacionesDetalle",
                principalColumn: "IdLicitacionDetalle",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
