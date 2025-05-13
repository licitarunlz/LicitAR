using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicitAR.Core.Migrations.DbContext
{
    /// <inheritdoc />
    public partial class AutoMigration_LicitAR_20250512194357 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PersonaUsuario_AspNetUsers_IdUsuario",
                table: "PersonaUsuario");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonaUsuario_Personas_IdPersona",
                table: "PersonaUsuario");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PersonaUsuario",
                table: "PersonaUsuario");

            migrationBuilder.RenameTable(
                name: "PersonaUsuario",
                newName: "PersonaUsuarios");

            migrationBuilder.RenameIndex(
                name: "IX_PersonaUsuario_IdUsuario",
                table: "PersonaUsuarios",
                newName: "IX_PersonaUsuarios_IdUsuario");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PersonaUsuarios",
                table: "PersonaUsuarios",
                columns: new[] { "IdPersona", "IdUsuario" });

            migrationBuilder.AddForeignKey(
                name: "FK_PersonaUsuarios_AspNetUsers_IdUsuario",
                table: "PersonaUsuarios",
                column: "IdUsuario",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonaUsuarios_Personas_IdPersona",
                table: "PersonaUsuarios",
                column: "IdPersona",
                principalTable: "Personas",
                principalColumn: "IdPersona",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PersonaUsuarios_AspNetUsers_IdUsuario",
                table: "PersonaUsuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonaUsuarios_Personas_IdPersona",
                table: "PersonaUsuarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PersonaUsuarios",
                table: "PersonaUsuarios");

            migrationBuilder.RenameTable(
                name: "PersonaUsuarios",
                newName: "PersonaUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_PersonaUsuarios_IdUsuario",
                table: "PersonaUsuario",
                newName: "IX_PersonaUsuario_IdUsuario");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PersonaUsuario",
                table: "PersonaUsuario",
                columns: new[] { "IdPersona", "IdUsuario" });

            migrationBuilder.AddForeignKey(
                name: "FK_PersonaUsuario_AspNetUsers_IdUsuario",
                table: "PersonaUsuario",
                column: "IdUsuario",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonaUsuario_Personas_IdPersona",
                table: "PersonaUsuario",
                column: "IdPersona",
                principalTable: "Personas",
                principalColumn: "IdPersona",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
