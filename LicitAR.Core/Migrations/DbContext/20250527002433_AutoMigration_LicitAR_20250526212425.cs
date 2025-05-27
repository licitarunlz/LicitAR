using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicitAR.Core.Migrations.DbContext
{
    /// <inheritdoc />
    public partial class AutoMigration_LicitAR_20250526212425 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdEstadoEvaluacion",
                table: "Evaluaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EvaluacionOfertasDetalle",
                columns: table => new
                {
                    IdEvaluacionOfertaDetalle = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEvaluacion = table.Column<int>(type: "int", nullable: false),
                    IdOferta = table.Column<int>(type: "int", nullable: false),
                    IdOfertaDetalle = table.Column<int>(type: "int", nullable: false),
                    OfertaDetalleGanadora = table.Column<bool>(type: "bit", nullable: false),
                    EvaluacionIdEvaluacion = table.Column<int>(type: "int", nullable: true),
                    OfertaIdOferta = table.Column<int>(type: "int", nullable: true),
                    Audit_IdUsuarioAlta = table.Column<int>(type: "int", nullable: false),
                    Audit_FechaAlta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Audit_IdUsuarioModificacion = table.Column<int>(type: "int", nullable: true),
                    Audit_FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Audit_IdUsuarioBaja = table.Column<int>(type: "int", nullable: true),
                    Audit_FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluacionOfertasDetalle", x => x.IdEvaluacionOfertaDetalle);
                    table.ForeignKey(
                        name: "FK_EvaluacionOfertasDetalle_Evaluaciones_EvaluacionIdEvaluacion",
                        column: x => x.EvaluacionIdEvaluacion,
                        principalTable: "Evaluaciones",
                        principalColumn: "IdEvaluacion");
                    table.ForeignKey(
                        name: "FK_EvaluacionOfertasDetalle_OfertasDetalle_IdOfertaDetalle",
                        column: x => x.IdOfertaDetalle,
                        principalTable: "OfertasDetalle",
                        principalColumn: "IdOfertaDetalle");
                    table.ForeignKey(
                        name: "FK_EvaluacionOfertasDetalle_Ofertas_OfertaIdOferta",
                        column: x => x.OfertaIdOferta,
                        principalTable: "Ofertas",
                        principalColumn: "IdOferta");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EvaluacionOfertasDetalle_EvaluacionIdEvaluacion",
                table: "EvaluacionOfertasDetalle",
                column: "EvaluacionIdEvaluacion");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluacionOfertasDetalle_IdOfertaDetalle",
                table: "EvaluacionOfertasDetalle",
                column: "IdOfertaDetalle");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluacionOfertasDetalle_OfertaIdOferta",
                table: "EvaluacionOfertasDetalle",
                column: "OfertaIdOferta");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EvaluacionOfertasDetalle");

            migrationBuilder.DropColumn(
                name: "IdEstadoEvaluacion",
                table: "Evaluaciones");
        }
    }
}
