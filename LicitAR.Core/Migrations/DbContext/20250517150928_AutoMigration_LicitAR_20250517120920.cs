using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicitAR.Core.Migrations.DbContext
{
    /// <inheritdoc />
    public partial class AutoMigration_LicitAR_20250517120920 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemsLicitacion_Licitaciones_IdLicitacion",
                table: "ItemsLicitacion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemsLicitacion",
                table: "ItemsLicitacion");

            migrationBuilder.RenameTable(
                name: "ItemsLicitacion",
                newName: "LicitacionesDetalle");

            migrationBuilder.RenameIndex(
                name: "IX_ItemsLicitacion_IdLicitacion",
                table: "LicitacionesDetalle",
                newName: "IX_LicitacionesDetalle_IdLicitacion");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LicitacionesDetalle",
                table: "LicitacionesDetalle",
                column: "IdLicitacionDetalle");

            migrationBuilder.AddForeignKey(
                name: "FK_LicitacionesDetalle_Licitaciones_IdLicitacion",
                table: "LicitacionesDetalle",
                column: "IdLicitacion",
                principalTable: "Licitaciones",
                principalColumn: "IdLicitacion",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LicitacionesDetalle_Licitaciones_IdLicitacion",
                table: "LicitacionesDetalle");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LicitacionesDetalle",
                table: "LicitacionesDetalle");

            migrationBuilder.RenameTable(
                name: "LicitacionesDetalle",
                newName: "ItemsLicitacion");

            migrationBuilder.RenameIndex(
                name: "IX_LicitacionesDetalle_IdLicitacion",
                table: "ItemsLicitacion",
                newName: "IX_ItemsLicitacion_IdLicitacion");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemsLicitacion",
                table: "ItemsLicitacion",
                column: "IdLicitacionDetalle");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemsLicitacion_Licitaciones_IdLicitacion",
                table: "ItemsLicitacion",
                column: "IdLicitacion",
                principalTable: "Licitaciones",
                principalColumn: "IdLicitacion",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
