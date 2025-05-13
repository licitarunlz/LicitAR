using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicitAR.Core.Migrations.DbContext
{
    /// <inheritdoc />
    public partial class AutoMigration_LicitAR_20250511210425 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "CategoriasLicitacion",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.CreateIndex(
                name: "IX_Licitaciones_IdCategoriaLicitacion",
                table: "Licitaciones",
                column: "IdCategoriaLicitacion");

            migrationBuilder.AddForeignKey(
                name: "FK_Licitaciones_CategoriasLicitacion_IdCategoriaLicitacion",
                table: "Licitaciones",
                column: "IdCategoriaLicitacion",
                principalTable: "CategoriasLicitacion",
                principalColumn: "IdCategoriaLicitacion",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Licitaciones_CategoriasLicitacion_IdCategoriaLicitacion",
                table: "Licitaciones");

            migrationBuilder.DropIndex(
                name: "IX_Licitaciones_IdCategoriaLicitacion",
                table: "Licitaciones");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "CategoriasLicitacion",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);
        }
    }
}
