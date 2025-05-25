using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicitAR.Core.Migrations.DbContext
{
    /// <inheritdoc />
    public partial class AutoMigration_LicitAR_20250525181721 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Evaluaciones",
                columns: table => new
                {
                    IdEvaluacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdLicitacion = table.Column<int>(type: "int", nullable: false),
                    IdUsuarioEvaluador = table.Column<int>(type: "int", nullable: false),
                    FechaInicioEvaluacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFinEvaluacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Audit_IdUsuarioAlta = table.Column<int>(type: "int", nullable: false),
                    Audit_FechaAlta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Audit_IdUsuarioModificacion = table.Column<int>(type: "int", nullable: true),
                    Audit_FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Audit_IdUsuarioBaja = table.Column<int>(type: "int", nullable: true),
                    Audit_FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evaluaciones", x => x.IdEvaluacion);
                    table.ForeignKey(
                        name: "FK_Evaluaciones_Licitaciones_IdLicitacion",
                        column: x => x.IdLicitacion,
                        principalTable: "Licitaciones",
                        principalColumn: "IdLicitacion",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EvaluacionOfertas",
                columns: table => new
                {
                    IdEvaluacionOferta = table.Column<int>(type: "int", nullable: false),
                    IdEvaluacion = table.Column<int>(type: "int", nullable: false),
                    IdOferta = table.Column<int>(type: "int", nullable: false),
                    OfertaGanadora = table.Column<bool>(type: "bit", nullable: false),
                    Audit_IdUsuarioAlta = table.Column<int>(type: "int", nullable: false),
                    Audit_FechaAlta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Audit_IdUsuarioModificacion = table.Column<int>(type: "int", nullable: true),
                    Audit_FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Audit_IdUsuarioBaja = table.Column<int>(type: "int", nullable: true),
                    Audit_FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluacionOfertas", x => x.IdEvaluacionOferta);
                    table.ForeignKey(
                        name: "FK_EvaluacionOfertas_Evaluaciones_IdEvaluacionOferta",
                        column: x => x.IdEvaluacionOferta,
                        principalTable: "Evaluaciones",
                        principalColumn: "IdEvaluacion");
                    table.ForeignKey(
                        name: "FK_EvaluacionOfertas_Ofertas_IdOferta",
                        column: x => x.IdOferta,
                        principalTable: "Ofertas",
                        principalColumn: "IdOferta");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Evaluaciones_IdLicitacion",
                table: "Evaluaciones",
                column: "IdLicitacion");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluacionOfertas_IdOferta",
                table: "EvaluacionOfertas",
                column: "IdOferta");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EvaluacionOfertas");

            migrationBuilder.DropTable(
                name: "Evaluaciones");
        }
    }
}
