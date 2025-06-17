using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicitAR.Core.Migrations.DbContext
{
    /// <inheritdoc />
    public partial class AutoMigration_LicitAR_20250614215411 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LicitacionChecklistItems",
                columns: table => new
                {
                    IdLicitacionChecklistItem = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdLicitacion = table.Column<int>(type: "int", nullable: false),
                    DescripcionItem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentoObligatorio = table.Column<bool>(type: "bit", nullable: false),
                    Audit_IdUsuarioAlta = table.Column<int>(type: "int", nullable: false),
                    Audit_FechaAlta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Audit_IdUsuarioModificacion = table.Column<int>(type: "int", nullable: true),
                    Audit_FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Audit_IdUsuarioBaja = table.Column<int>(type: "int", nullable: true),
                    Audit_FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicitacionChecklistItems", x => x.IdLicitacionChecklistItem);
                    table.ForeignKey(
                        name: "FK_LicitacionChecklistItems_Licitaciones_IdLicitacion",
                        column: x => x.IdLicitacion,
                        principalTable: "Licitaciones",
                        principalColumn: "IdLicitacion",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OfertaChecklistItems",
                columns: table => new
                {
                    IdOfertaChecklistItem = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdOferta = table.Column<int>(type: "int", nullable: false),
                    IdLicitacionChecklistItem = table.Column<int>(type: "int", nullable: false),
                    BlobUri = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
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
                    table.PrimaryKey("PK_OfertaChecklistItems", x => x.IdOfertaChecklistItem);
                    table.ForeignKey(
                        name: "FK_OfertaChecklistItems_LicitacionChecklistItems_IdLicitacionChecklistItem",
                        column: x => x.IdLicitacionChecklistItem,
                        principalTable: "LicitacionChecklistItems",
                        principalColumn: "IdLicitacionChecklistItem");
                    table.ForeignKey(
                        name: "FK_OfertaChecklistItems_Ofertas_OfertaIdOferta",
                        column: x => x.OfertaIdOferta,
                        principalTable: "Ofertas",
                        principalColumn: "IdOferta");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LicitacionChecklistItems_IdLicitacion",
                table: "LicitacionChecklistItems",
                column: "IdLicitacion");

            migrationBuilder.CreateIndex(
                name: "IX_OfertaChecklistItems_IdLicitacionChecklistItem",
                table: "OfertaChecklistItems",
                column: "IdLicitacionChecklistItem");

            migrationBuilder.CreateIndex(
                name: "IX_OfertaChecklistItems_OfertaIdOferta",
                table: "OfertaChecklistItems",
                column: "OfertaIdOferta");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OfertaChecklistItems");

            migrationBuilder.DropTable(
                name: "LicitacionChecklistItems");
        }
    }
}
