using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicitAR.Core.Migrations.DbContext
{
    /// <inheritdoc />
    public partial class AutoMigration_LicitAR_20250517234818 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EstadosOferta",
                columns: table => new
                {
                    IdEstadoOferta = table.Column<int>(type: "int", nullable: false)
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
                    table.PrimaryKey("PK_EstadosOferta", x => x.IdEstadoOferta);
                });

            migrationBuilder.CreateTable(
                name: "Ofertas",
                columns: table => new
                {
                    IdOferta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdLicitacion = table.Column<int>(type: "int", nullable: false),
                    IdPersona = table.Column<int>(type: "int", nullable: false),
                    FechaOferta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdEstadoOferta = table.Column<int>(type: "int", nullable: false),
                    Audit_IdUsuarioAlta = table.Column<int>(type: "int", nullable: false),
                    Audit_FechaAlta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Audit_IdUsuarioModificacion = table.Column<int>(type: "int", nullable: true),
                    Audit_FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Audit_IdUsuarioBaja = table.Column<int>(type: "int", nullable: true),
                    Audit_FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ofertas", x => x.IdOferta);
                    table.ForeignKey(
                        name: "FK_Ofertas_EstadosOferta_IdEstadoOferta",
                        column: x => x.IdEstadoOferta,
                        principalTable: "EstadosOferta",
                        principalColumn: "IdEstadoOferta",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ofertas_Licitaciones_IdLicitacion",
                        column: x => x.IdLicitacion,
                        principalTable: "Licitaciones",
                        principalColumn: "IdLicitacion",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ofertas_Personas_IdPersona",
                        column: x => x.IdPersona,
                        principalTable: "Personas",
                        principalColumn: "IdPersona",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OfertasDetalle",
                columns: table => new
                {
                    IdOfertaDetalle = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdOferta = table.Column<int>(type: "int", nullable: false),
                    IdLicitacionDetalle = table.Column<int>(type: "int", nullable: false),
                    ImporteUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImporteSubtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LicitacionDetalleIdLicitacionDetalle = table.Column<int>(type: "int", nullable: true),
                    Audit_IdUsuarioAlta = table.Column<int>(type: "int", nullable: false),
                    Audit_FechaAlta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Audit_IdUsuarioModificacion = table.Column<int>(type: "int", nullable: true),
                    Audit_FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Audit_IdUsuarioBaja = table.Column<int>(type: "int", nullable: true),
                    Audit_FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfertasDetalle", x => x.IdOfertaDetalle);
                    table.ForeignKey(
                        name: "FK_OfertasDetalle_LicitacionesDetalle_LicitacionDetalleIdLicitacionDetalle",
                        column: x => x.LicitacionDetalleIdLicitacionDetalle,
                        principalTable: "LicitacionesDetalle",
                        principalColumn: "IdLicitacionDetalle");
                    table.ForeignKey(
                        name: "FK_OfertasDetalle_Ofertas_IdOferta",
                        column: x => x.IdOferta,
                        principalTable: "Ofertas",
                        principalColumn: "IdOferta",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ofertas_IdEstadoOferta",
                table: "Ofertas",
                column: "IdEstadoOferta");

            migrationBuilder.CreateIndex(
                name: "IX_Ofertas_IdLicitacion",
                table: "Ofertas",
                column: "IdLicitacion");

            migrationBuilder.CreateIndex(
                name: "IX_Ofertas_IdPersona",
                table: "Ofertas",
                column: "IdPersona");

            migrationBuilder.CreateIndex(
                name: "IX_OfertasDetalle_IdOferta",
                table: "OfertasDetalle",
                column: "IdOferta");

            migrationBuilder.CreateIndex(
                name: "IX_OfertasDetalle_LicitacionDetalleIdLicitacionDetalle",
                table: "OfertasDetalle",
                column: "LicitacionDetalleIdLicitacionDetalle");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OfertasDetalle");

            migrationBuilder.DropTable(
                name: "Ofertas");

            migrationBuilder.DropTable(
                name: "EstadosOferta");
        }
    }
}
