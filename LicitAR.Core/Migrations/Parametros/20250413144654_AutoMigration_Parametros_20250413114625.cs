using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicitAR.Core.Migrations.Parametros
{
    /// <inheritdoc />
    public partial class AutoMigration_Parametros_20250413114625 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AuditTable_IdUsuarioModificacion",
                table: "TiposPersona",
                newName: "Audit_IdUsuarioModificacion");

            migrationBuilder.RenameColumn(
                name: "AuditTable_IdUsuarioBaja",
                table: "TiposPersona",
                newName: "Audit_IdUsuarioBaja");

            migrationBuilder.RenameColumn(
                name: "AuditTable_IdUsuarioAlta",
                table: "TiposPersona",
                newName: "Audit_IdUsuarioAlta");

            migrationBuilder.RenameColumn(
                name: "AuditTable_FechaModificacion",
                table: "TiposPersona",
                newName: "Audit_FechaModificacion");

            migrationBuilder.RenameColumn(
                name: "AuditTable_FechaBaja",
                table: "TiposPersona",
                newName: "Audit_FechaBaja");

            migrationBuilder.RenameColumn(
                name: "AuditTable_FechaAlta",
                table: "TiposPersona",
                newName: "Audit_FechaAlta");

            migrationBuilder.RenameColumn(
                name: "AuditTable_IdUsuarioModificacion",
                table: "TiposContacto",
                newName: "Audit_IdUsuarioModificacion");

            migrationBuilder.RenameColumn(
                name: "AuditTable_IdUsuarioBaja",
                table: "TiposContacto",
                newName: "Audit_IdUsuarioBaja");

            migrationBuilder.RenameColumn(
                name: "AuditTable_IdUsuarioAlta",
                table: "TiposContacto",
                newName: "Audit_IdUsuarioAlta");

            migrationBuilder.RenameColumn(
                name: "AuditTable_FechaModificacion",
                table: "TiposContacto",
                newName: "Audit_FechaModificacion");

            migrationBuilder.RenameColumn(
                name: "AuditTable_FechaBaja",
                table: "TiposContacto",
                newName: "Audit_FechaBaja");

            migrationBuilder.RenameColumn(
                name: "AuditTable_FechaAlta",
                table: "TiposContacto",
                newName: "Audit_FechaAlta");

            migrationBuilder.RenameColumn(
                name: "AuditTable_IdUsuarioModificacion",
                table: "Provincias",
                newName: "Audit_IdUsuarioModificacion");

            migrationBuilder.RenameColumn(
                name: "AuditTable_IdUsuarioBaja",
                table: "Provincias",
                newName: "Audit_IdUsuarioBaja");

            migrationBuilder.RenameColumn(
                name: "AuditTable_IdUsuarioAlta",
                table: "Provincias",
                newName: "Audit_IdUsuarioAlta");

            migrationBuilder.RenameColumn(
                name: "AuditTable_FechaModificacion",
                table: "Provincias",
                newName: "Audit_FechaModificacion");

            migrationBuilder.RenameColumn(
                name: "AuditTable_FechaBaja",
                table: "Provincias",
                newName: "Audit_FechaBaja");

            migrationBuilder.RenameColumn(
                name: "AuditTable_FechaAlta",
                table: "Provincias",
                newName: "Audit_FechaAlta");

            migrationBuilder.RenameColumn(
                name: "AuditTable_IdUsuarioModificacion",
                table: "Localidades",
                newName: "Audit_IdUsuarioModificacion");

            migrationBuilder.RenameColumn(
                name: "AuditTable_IdUsuarioBaja",
                table: "Localidades",
                newName: "Audit_IdUsuarioBaja");

            migrationBuilder.RenameColumn(
                name: "AuditTable_IdUsuarioAlta",
                table: "Localidades",
                newName: "Audit_IdUsuarioAlta");

            migrationBuilder.RenameColumn(
                name: "AuditTable_FechaModificacion",
                table: "Localidades",
                newName: "Audit_FechaModificacion");

            migrationBuilder.RenameColumn(
                name: "AuditTable_FechaBaja",
                table: "Localidades",
                newName: "Audit_FechaBaja");

            migrationBuilder.RenameColumn(
                name: "AuditTable_FechaAlta",
                table: "Localidades",
                newName: "Audit_FechaAlta");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Audit_IdUsuarioModificacion",
                table: "TiposPersona",
                newName: "AuditTable_IdUsuarioModificacion");

            migrationBuilder.RenameColumn(
                name: "Audit_IdUsuarioBaja",
                table: "TiposPersona",
                newName: "AuditTable_IdUsuarioBaja");

            migrationBuilder.RenameColumn(
                name: "Audit_IdUsuarioAlta",
                table: "TiposPersona",
                newName: "AuditTable_IdUsuarioAlta");

            migrationBuilder.RenameColumn(
                name: "Audit_FechaModificacion",
                table: "TiposPersona",
                newName: "AuditTable_FechaModificacion");

            migrationBuilder.RenameColumn(
                name: "Audit_FechaBaja",
                table: "TiposPersona",
                newName: "AuditTable_FechaBaja");

            migrationBuilder.RenameColumn(
                name: "Audit_FechaAlta",
                table: "TiposPersona",
                newName: "AuditTable_FechaAlta");

            migrationBuilder.RenameColumn(
                name: "Audit_IdUsuarioModificacion",
                table: "TiposContacto",
                newName: "AuditTable_IdUsuarioModificacion");

            migrationBuilder.RenameColumn(
                name: "Audit_IdUsuarioBaja",
                table: "TiposContacto",
                newName: "AuditTable_IdUsuarioBaja");

            migrationBuilder.RenameColumn(
                name: "Audit_IdUsuarioAlta",
                table: "TiposContacto",
                newName: "AuditTable_IdUsuarioAlta");

            migrationBuilder.RenameColumn(
                name: "Audit_FechaModificacion",
                table: "TiposContacto",
                newName: "AuditTable_FechaModificacion");

            migrationBuilder.RenameColumn(
                name: "Audit_FechaBaja",
                table: "TiposContacto",
                newName: "AuditTable_FechaBaja");

            migrationBuilder.RenameColumn(
                name: "Audit_FechaAlta",
                table: "TiposContacto",
                newName: "AuditTable_FechaAlta");

            migrationBuilder.RenameColumn(
                name: "Audit_IdUsuarioModificacion",
                table: "Provincias",
                newName: "AuditTable_IdUsuarioModificacion");

            migrationBuilder.RenameColumn(
                name: "Audit_IdUsuarioBaja",
                table: "Provincias",
                newName: "AuditTable_IdUsuarioBaja");

            migrationBuilder.RenameColumn(
                name: "Audit_IdUsuarioAlta",
                table: "Provincias",
                newName: "AuditTable_IdUsuarioAlta");

            migrationBuilder.RenameColumn(
                name: "Audit_FechaModificacion",
                table: "Provincias",
                newName: "AuditTable_FechaModificacion");

            migrationBuilder.RenameColumn(
                name: "Audit_FechaBaja",
                table: "Provincias",
                newName: "AuditTable_FechaBaja");

            migrationBuilder.RenameColumn(
                name: "Audit_FechaAlta",
                table: "Provincias",
                newName: "AuditTable_FechaAlta");

            migrationBuilder.RenameColumn(
                name: "Audit_IdUsuarioModificacion",
                table: "Localidades",
                newName: "AuditTable_IdUsuarioModificacion");

            migrationBuilder.RenameColumn(
                name: "Audit_IdUsuarioBaja",
                table: "Localidades",
                newName: "AuditTable_IdUsuarioBaja");

            migrationBuilder.RenameColumn(
                name: "Audit_IdUsuarioAlta",
                table: "Localidades",
                newName: "AuditTable_IdUsuarioAlta");

            migrationBuilder.RenameColumn(
                name: "Audit_FechaModificacion",
                table: "Localidades",
                newName: "AuditTable_FechaModificacion");

            migrationBuilder.RenameColumn(
                name: "Audit_FechaBaja",
                table: "Localidades",
                newName: "AuditTable_FechaBaja");

            migrationBuilder.RenameColumn(
                name: "Audit_FechaAlta",
                table: "Localidades",
                newName: "AuditTable_FechaAlta");
        }
    }
}
