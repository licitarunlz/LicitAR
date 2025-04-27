using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicitAR.Core.Migrations.Identidad
{
    /// <inheritdoc />
    public partial class AutoMigration_LicitARIdentity_20250415003826 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdUsuario",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdUsuario",
                table: "AspNetUsers");
        }
    }
}
