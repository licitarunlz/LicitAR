using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicitAR.Core.Migrations.DbContext
{
    /// <inheritdoc />
    public partial class AutoMigration_LicitAR_20250615011702 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OfertaChecklistItems_Ofertas_OfertaIdOferta",
                table: "OfertaChecklistItems");

            migrationBuilder.DropIndex(
                name: "IX_OfertaChecklistItems_OfertaIdOferta",
                table: "OfertaChecklistItems");

            migrationBuilder.DropColumn(
                name: "OfertaIdOferta",
                table: "OfertaChecklistItems");

            migrationBuilder.CreateIndex(
                name: "IX_OfertaChecklistItems_IdOferta",
                table: "OfertaChecklistItems",
                column: "IdOferta");

            migrationBuilder.AddForeignKey(
                name: "FK_OfertaChecklistItems_Ofertas_IdOferta",
                table: "OfertaChecklistItems",
                column: "IdOferta",
                principalTable: "Ofertas",
                principalColumn: "IdOferta");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OfertaChecklistItems_Ofertas_IdOferta",
                table: "OfertaChecklistItems");

            migrationBuilder.DropIndex(
                name: "IX_OfertaChecklistItems_IdOferta",
                table: "OfertaChecklistItems");

            migrationBuilder.AddColumn<int>(
                name: "OfertaIdOferta",
                table: "OfertaChecklistItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OfertaChecklistItems_OfertaIdOferta",
                table: "OfertaChecklistItems",
                column: "OfertaIdOferta");

            migrationBuilder.AddForeignKey(
                name: "FK_OfertaChecklistItems_Ofertas_OfertaIdOferta",
                table: "OfertaChecklistItems",
                column: "OfertaIdOferta",
                principalTable: "Ofertas",
                principalColumn: "IdOferta");
        }
    }
}
