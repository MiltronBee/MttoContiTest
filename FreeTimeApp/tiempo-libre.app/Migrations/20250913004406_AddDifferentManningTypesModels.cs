using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tiempo_libre.Migrations
{
    /// <inheritdoc />
    public partial class AddDifferentManningTypesModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ManningPorDia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuarioModifica = table.Column<int>(type: "int", nullable: false),
                    IdJefeArea = table.Column<int>(type: "int", nullable: false),
                    IdArea = table.Column<int>(type: "int", nullable: false),
                    Anio = table.Column<int>(type: "int", nullable: false),
                    Mes = table.Column<int>(type: "int", nullable: false),
                    Dia = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    PorcentajeManning = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    BorradoLogico = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManningPorDia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManningPorDia_Areas_IdArea",
                        column: x => x.IdArea,
                        principalTable: "Areas",
                        principalColumn: "AreaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ManningPorDia_Users_IdJefeArea",
                        column: x => x.IdJefeArea,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ManningPorDia_Users_IdUsuarioModifica",
                        column: x => x.IdUsuarioModifica,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ManningPorMes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuarioModifica = table.Column<int>(type: "int", nullable: false),
                    IdJefeArea = table.Column<int>(type: "int", nullable: false),
                    IdArea = table.Column<int>(type: "int", nullable: false),
                    Anio = table.Column<int>(type: "int", nullable: false),
                    Mes = table.Column<int>(type: "int", nullable: false),
                    PorcentajeManning = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    BorradoLogico = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManningPorMes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManningPorMes_Areas_IdArea",
                        column: x => x.IdArea,
                        principalTable: "Areas",
                        principalColumn: "AreaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ManningPorMes_Users_IdJefeArea",
                        column: x => x.IdJefeArea,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ManningPorMes_Users_IdUsuarioModifica",
                        column: x => x.IdUsuarioModifica,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ManningPorDia_IdArea",
                table: "ManningPorDia",
                column: "IdArea");

            migrationBuilder.CreateIndex(
                name: "IX_ManningPorDia_IdJefeArea",
                table: "ManningPorDia",
                column: "IdJefeArea");

            migrationBuilder.CreateIndex(
                name: "IX_ManningPorDia_IdUsuarioModifica",
                table: "ManningPorDia",
                column: "IdUsuarioModifica");

            migrationBuilder.CreateIndex(
                name: "IX_ManningPorMes_IdArea",
                table: "ManningPorMes",
                column: "IdArea");

            migrationBuilder.CreateIndex(
                name: "IX_ManningPorMes_IdJefeArea",
                table: "ManningPorMes",
                column: "IdJefeArea");

            migrationBuilder.CreateIndex(
                name: "IX_ManningPorMes_IdUsuarioModifica",
                table: "ManningPorMes",
                column: "IdUsuarioModifica");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ManningPorDia");

            migrationBuilder.DropTable(
                name: "ManningPorMes");
        }
    }
}
