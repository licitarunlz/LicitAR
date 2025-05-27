using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicitAR.Core.Migrations.DbContext
{
    /// <inheritdoc />
    public partial class AutoMigration_LicitAR_20250526232111 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EvaluacionOfertasDetalle_Evaluaciones_EvaluacionIdEvaluacion",
                table: "EvaluacionOfertasDetalle");

            migrationBuilder.RenameColumn(
                name: "EvaluacionIdEvaluacion",
                table: "EvaluacionOfertasDetalle",
                newName: "EstadoEvaluacionIdEstadoEvaluacion");

            migrationBuilder.RenameIndex(
                name: "IX_EvaluacionOfertasDetalle_EvaluacionIdEvaluacion",
                table: "EvaluacionOfertasDetalle",
                newName: "IX_EvaluacionOfertasDetalle_EstadoEvaluacionIdEstadoEvaluacion");

            migrationBuilder.CreateTable(
                name: "EstadoEvaluacion",
                columns: table => new
                {
                    IdEstadoEvaluacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    Audit_IdUsuarioAlta = table.Column<int>(type: "int", nullable: false),
                    Audit_FechaAlta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Audit_IdUsuarioModificacion = table.Column<int>(type: "int", nullable: true),
                    Audit_FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Audit_IdUsuarioBaja = table.Column<int>(type: "int", nullable: true),
                    Audit_FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadoEvaluacion", x => x.IdEstadoEvaluacion);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EvaluacionOfertasDetalle_IdEvaluacion",
                table: "EvaluacionOfertasDetalle",
                column: "IdEvaluacion");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluaciones_IdEstadoEvaluacion",
                table: "Evaluaciones",
                column: "IdEstadoEvaluacion");

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluaciones_EstadoEvaluacion_IdEstadoEvaluacion",
                table: "Evaluaciones",
                column: "IdEstadoEvaluacion",
                principalTable: "EstadoEvaluacion",
                principalColumn: "IdEstadoEvaluacion",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EvaluacionOfertasDetalle_EstadoEvaluacion_EstadoEvaluacionIdEstadoEvaluacion",
                table: "EvaluacionOfertasDetalle",
                column: "EstadoEvaluacionIdEstadoEvaluacion",
                principalTable: "EstadoEvaluacion",
                principalColumn: "IdEstadoEvaluacion");

            migrationBuilder.AddForeignKey(
                name: "FK_EvaluacionOfertasDetalle_Evaluaciones_IdEvaluacion",
                table: "EvaluacionOfertasDetalle",
                column: "IdEvaluacion",
                principalTable: "Evaluaciones",
                principalColumn: "IdEvaluacion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evaluaciones_EstadoEvaluacion_IdEstadoEvaluacion",
                table: "Evaluaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_EvaluacionOfertasDetalle_EstadoEvaluacion_EstadoEvaluacionIdEstadoEvaluacion",
                table: "EvaluacionOfertasDetalle");

            migrationBuilder.DropForeignKey(
                name: "FK_EvaluacionOfertasDetalle_Evaluaciones_IdEvaluacion",
                table: "EvaluacionOfertasDetalle");

            migrationBuilder.DropTable(
                name: "EstadoEvaluacion");

            migrationBuilder.DropIndex(
                name: "IX_EvaluacionOfertasDetalle_IdEvaluacion",
                table: "EvaluacionOfertasDetalle");

            migrationBuilder.DropIndex(
                name: "IX_Evaluaciones_IdEstadoEvaluacion",
                table: "Evaluaciones");

            migrationBuilder.RenameColumn(
                name: "EstadoEvaluacionIdEstadoEvaluacion",
                table: "EvaluacionOfertasDetalle",
                newName: "EvaluacionIdEvaluacion");

            migrationBuilder.RenameIndex(
                name: "IX_EvaluacionOfertasDetalle_EstadoEvaluacionIdEstadoEvaluacion",
                table: "EvaluacionOfertasDetalle",
                newName: "IX_EvaluacionOfertasDetalle_EvaluacionIdEvaluacion");

            migrationBuilder.AddForeignKey(
                name: "FK_EvaluacionOfertasDetalle_Evaluaciones_EvaluacionIdEvaluacion",
                table: "EvaluacionOfertasDetalle",
                column: "EvaluacionIdEvaluacion",
                principalTable: "Evaluaciones",
                principalColumn: "IdEvaluacion");
        }
    }
}
