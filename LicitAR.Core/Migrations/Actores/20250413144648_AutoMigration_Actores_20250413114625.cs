using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicitAR.Core.Migrations.Actores
{
    /// <inheritdoc />
    public partial class AutoMigration_Actores_20250413114625 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AuditTable_IdUsuarioModificacion",
                table: "Personas",
                newName: "Audit_IdUsuarioModificacion");

            migrationBuilder.RenameColumn(
                name: "AuditTable_IdUsuarioBaja",
                table: "Personas",
                newName: "Audit_IdUsuarioBaja");

            migrationBuilder.RenameColumn(
                name: "AuditTable_IdUsuarioAlta",
                table: "Personas",
                newName: "Audit_IdUsuarioAlta");

            migrationBuilder.RenameColumn(
                name: "AuditTable_FechaModificacion",
                table: "Personas",
                newName: "Audit_FechaModificacion");

            migrationBuilder.RenameColumn(
                name: "AuditTable_FechaBaja",
                table: "Personas",
                newName: "Audit_FechaBaja");

            migrationBuilder.RenameColumn(
                name: "AuditTable_FechaAlta",
                table: "Personas",
                newName: "Audit_FechaAlta");

            migrationBuilder.RenameColumn(
                name: "AuditTable_IdUsuarioModificacion",
                table: "EntidadesLicitantes",
                newName: "Audit_IdUsuarioModificacion");

            migrationBuilder.RenameColumn(
                name: "AuditTable_IdUsuarioBaja",
                table: "EntidadesLicitantes",
                newName: "Audit_IdUsuarioBaja");

            migrationBuilder.RenameColumn(
                name: "AuditTable_IdUsuarioAlta",
                table: "EntidadesLicitantes",
                newName: "Audit_IdUsuarioAlta");

            migrationBuilder.RenameColumn(
                name: "AuditTable_FechaModificacion",
                table: "EntidadesLicitantes",
                newName: "Audit_FechaModificacion");

            migrationBuilder.RenameColumn(
                name: "AuditTable_FechaBaja",
                table: "EntidadesLicitantes",
                newName: "Audit_FechaBaja");

            migrationBuilder.RenameColumn(
                name: "AuditTable_FechaAlta",
                table: "EntidadesLicitantes",
                newName: "Audit_FechaAlta");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Audit_IdUsuarioModificacion",
                table: "Personas",
                newName: "AuditTable_IdUsuarioModificacion");

            migrationBuilder.RenameColumn(
                name: "Audit_IdUsuarioBaja",
                table: "Personas",
                newName: "AuditTable_IdUsuarioBaja");

            migrationBuilder.RenameColumn(
                name: "Audit_IdUsuarioAlta",
                table: "Personas",
                newName: "AuditTable_IdUsuarioAlta");

            migrationBuilder.RenameColumn(
                name: "Audit_FechaModificacion",
                table: "Personas",
                newName: "AuditTable_FechaModificacion");

            migrationBuilder.RenameColumn(
                name: "Audit_FechaBaja",
                table: "Personas",
                newName: "AuditTable_FechaBaja");

            migrationBuilder.RenameColumn(
                name: "Audit_FechaAlta",
                table: "Personas",
                newName: "AuditTable_FechaAlta");

            migrationBuilder.RenameColumn(
                name: "Audit_IdUsuarioModificacion",
                table: "EntidadesLicitantes",
                newName: "AuditTable_IdUsuarioModificacion");

            migrationBuilder.RenameColumn(
                name: "Audit_IdUsuarioBaja",
                table: "EntidadesLicitantes",
                newName: "AuditTable_IdUsuarioBaja");

            migrationBuilder.RenameColumn(
                name: "Audit_IdUsuarioAlta",
                table: "EntidadesLicitantes",
                newName: "AuditTable_IdUsuarioAlta");

            migrationBuilder.RenameColumn(
                name: "Audit_FechaModificacion",
                table: "EntidadesLicitantes",
                newName: "AuditTable_FechaModificacion");

            migrationBuilder.RenameColumn(
                name: "Audit_FechaBaja",
                table: "EntidadesLicitantes",
                newName: "AuditTable_FechaBaja");

            migrationBuilder.RenameColumn(
                name: "Audit_FechaAlta",
                table: "EntidadesLicitantes",
                newName: "AuditTable_FechaAlta");
        }
    }
}
