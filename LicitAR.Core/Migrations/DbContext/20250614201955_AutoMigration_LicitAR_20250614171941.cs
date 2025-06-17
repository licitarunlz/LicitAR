using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicitAR.Core.Migrations.DbContext
{
    /// <inheritdoc />
    public partial class AutoMigration_LicitAR_20250614171941 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NombreArchivoOriginal",
                table: "LicitacionDocumentacion",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(244)",
                oldMaxLength: 244);

            migrationBuilder.AddColumn<string>(
                name: "BlobUri",
                table: "LicitacionDocumentacion",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlobUri",
                table: "LicitacionDocumentacion");

            migrationBuilder.AlterColumn<string>(
                name: "NombreArchivoOriginal",
                table: "LicitacionDocumentacion",
                type: "nvarchar(244)",
                maxLength: 244,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);
        }
    }
}
