using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicitAR.Core.Migrations.DbContext
{
    /// <inheritdoc />
    public partial class AutoMigration_LicitAR_20250511215721 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "TiposPersona",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "TiposContacto",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "Rubro",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "Provincias",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "Personas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Localidades",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "Localidades",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "Licitaciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "EstadosLicitacion",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "EntidadesLicitantes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "CategoriasLicitacion",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_EntidadesLicitantes_IdLocalidad",
                table: "EntidadesLicitantes",
                column: "IdLocalidad");

            migrationBuilder.CreateIndex(
                name: "IX_EntidadesLicitantes_IdProvincia",
                table: "EntidadesLicitantes",
                column: "IdProvincia");

            migrationBuilder.AddForeignKey(
                name: "FK_EntidadesLicitantes_Localidades_IdLocalidad",
                table: "EntidadesLicitantes",
                column: "IdLocalidad",
                principalTable: "Localidades",
                principalColumn: "IdLocalidad",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EntidadesLicitantes_Provincias_IdProvincia",
                table: "EntidadesLicitantes",
                column: "IdProvincia",
                principalTable: "Provincias",
                principalColumn: "IdProvincia",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntidadesLicitantes_Localidades_IdLocalidad",
                table: "EntidadesLicitantes");

            migrationBuilder.DropForeignKey(
                name: "FK_EntidadesLicitantes_Provincias_IdProvincia",
                table: "EntidadesLicitantes");

            migrationBuilder.DropIndex(
                name: "IX_EntidadesLicitantes_IdLocalidad",
                table: "EntidadesLicitantes");

            migrationBuilder.DropIndex(
                name: "IX_EntidadesLicitantes_IdProvincia",
                table: "EntidadesLicitantes");

            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "TiposPersona");

            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "TiposContacto");

            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "Rubro");

            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "Provincias");

            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "Localidades");

            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "Licitaciones");

            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "EstadosLicitacion");

            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "EntidadesLicitantes");

            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "CategoriasLicitacion");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Localidades",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);
        }
    }
}
