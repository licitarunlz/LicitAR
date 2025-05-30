using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicitAR.Core.Migrations.DbContext
{
    /// <inheritdoc />
    public partial class AutoMigration_LicitAR_20250522231232 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OfertasDetalle_LicitacionesDetalle_LicitacionDetalleIdLicitacionDetalle",
                table: "OfertasDetalle");

            migrationBuilder.DropIndex(
                name: "IX_OfertasDetalle_LicitacionDetalleIdLicitacionDetalle",
                table: "OfertasDetalle");

            migrationBuilder.DropColumn(
                name: "LicitacionDetalleIdLicitacionDetalle",
                table: "OfertasDetalle");

            migrationBuilder.AddColumn<string>(
                name: "CodigoOfertaLicitacion",
                table: "Ofertas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OfertasDetalle_IdLicitacionDetalle",
                table: "OfertasDetalle",
                column: "IdLicitacionDetalle");

            migrationBuilder.AddForeignKey(
                name: "FK_OfertasDetalle_LicitacionesDetalle_IdLicitacionDetalle",
                table: "OfertasDetalle",
                column: "IdLicitacionDetalle",
                principalTable: "LicitacionesDetalle",
                principalColumn: "IdLicitacionDetalle");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OfertasDetalle_LicitacionesDetalle_IdLicitacionDetalle",
                table: "OfertasDetalle");

            migrationBuilder.DropIndex(
                name: "IX_OfertasDetalle_IdLicitacionDetalle",
                table: "OfertasDetalle");

            migrationBuilder.DropColumn(
                name: "CodigoOfertaLicitacion",
                table: "Ofertas");

            migrationBuilder.AddColumn<int>(
                name: "LicitacionDetalleIdLicitacionDetalle",
                table: "OfertasDetalle",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OfertasDetalle_LicitacionDetalleIdLicitacionDetalle",
                table: "OfertasDetalle",
                column: "LicitacionDetalleIdLicitacionDetalle");

            migrationBuilder.AddForeignKey(
                name: "FK_OfertasDetalle_LicitacionesDetalle_LicitacionDetalleIdLicitacionDetalle",
                table: "OfertasDetalle",
                column: "LicitacionDetalleIdLicitacionDetalle",
                principalTable: "LicitacionesDetalle",
                principalColumn: "IdLicitacionDetalle");
        }
    }
}
