using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicitAR.Core.Migrations.Identidad
{
    /// <inheritdoc />
    public partial class AutoMigration_LicitARIdentity_20250415002849 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Audit_FechaAlta",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Audit_FechaBaja",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Audit_FechaModificacion",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Audit_IdUsuarioAlta",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Audit_IdUsuarioBaja",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Audit_IdUsuarioModificacion",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Audit_FechaAlta",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Audit_FechaBaja",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Audit_FechaModificacion",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Audit_IdUsuarioAlta",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Audit_IdUsuarioBaja",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Audit_IdUsuarioModificacion",
                table: "AspNetUsers");
        }
    }
}
