using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicitAR.Core.Migrations.Identidad
{
    /// <inheritdoc />
    public partial class AutoMigration_LicitARIdentity_20250413130054 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Cuit",
                table: "AspNetUsers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cuit",
                table: "AspNetUsers");
        }
    }
}
