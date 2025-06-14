using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicitAR.Core.Migrations.DbContext
{
    /// <inheritdoc />
    public partial class AutoMigration_LicitAR_20250607121042 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Rubro",
                table: "Rubro");

            migrationBuilder.RenameTable(
                name: "Rubro",
                newName: "Rubros");

            migrationBuilder.AddColumn<int>(
                name: "IdRubro",
                table: "Personas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Observacion",
                table: "OfertasDetalle",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdRubro",
                table: "Licitaciones",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rubros",
                table: "Rubros",
                column: "IdRubro");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_IdRubro",
                table: "Personas",
                column: "IdRubro");

            migrationBuilder.CreateIndex(
                name: "IX_Licitaciones_IdRubro",
                table: "Licitaciones",
                column: "IdRubro");

            migrationBuilder.AddForeignKey(
                name: "FK_Licitaciones_Rubros_IdRubro",
                table: "Licitaciones",
                column: "IdRubro",
                principalTable: "Rubros",
                principalColumn: "IdRubro");

            migrationBuilder.AddForeignKey(
                name: "FK_Personas_Rubros_IdRubro",
                table: "Personas",
                column: "IdRubro",
                principalTable: "Rubros",
                principalColumn: "IdRubro");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Licitaciones_Rubros_IdRubro",
                table: "Licitaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Personas_Rubros_IdRubro",
                table: "Personas");

            migrationBuilder.DropIndex(
                name: "IX_Personas_IdRubro",
                table: "Personas");

            migrationBuilder.DropIndex(
                name: "IX_Licitaciones_IdRubro",
                table: "Licitaciones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rubros",
                table: "Rubros");

            migrationBuilder.DropColumn(
                name: "IdRubro",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "Observacion",
                table: "OfertasDetalle");

            migrationBuilder.DropColumn(
                name: "IdRubro",
                table: "Licitaciones");

            migrationBuilder.RenameTable(
                name: "Rubros",
                newName: "Rubro");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rubro",
                table: "Rubro",
                column: "IdRubro");
        }
    }
}
