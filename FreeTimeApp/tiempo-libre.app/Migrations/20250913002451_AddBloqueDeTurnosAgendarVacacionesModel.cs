using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tiempo_libre.Migrations
{
    /// <inheritdoc />
    public partial class AddBloqueDeTurnosAgendarVacacionesModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BloqueDeTurnosAgendarVacaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdJefeArea = table.Column<int>(type: "int", nullable: false),
                    IdArea = table.Column<int>(type: "int", nullable: false),
                    IdGrupo = table.Column<int>(type: "int", nullable: false),
                    IdPeriodoDeProgramacionAnual = table.Column<int>(type: "int", nullable: false),
                    IndiceBloqueDeTurnos = table.Column<int>(type: "int", nullable: false),
                    NombreDelBloque = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaYHoraInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaYHoraFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DuracionEnHoras = table.Column<int>(type: "int", nullable: false),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BloqueDeTurnosAgendarVacaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BloqueDeTurnosAgendarVacaciones_Areas_IdArea",
                        column: x => x.IdArea,
                        principalTable: "Areas",
                        principalColumn: "AreaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BloqueDeTurnosAgendarVacaciones_Grupos_IdGrupo",
                        column: x => x.IdGrupo,
                        principalTable: "Grupos",
                        principalColumn: "GrupoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BloqueDeTurnosAgendarVacaciones_ProgramacionesAnuales_IdPeriodoDeProgramacionAnual",
                        column: x => x.IdPeriodoDeProgramacionAnual,
                        principalTable: "ProgramacionesAnuales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BloqueDeTurnosAgendarVacaciones_Users_IdJefeArea",
                        column: x => x.IdJefeArea,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BloqueDeTurnosAgendarVacaciones_IdArea",
                table: "BloqueDeTurnosAgendarVacaciones",
                column: "IdArea");

            migrationBuilder.CreateIndex(
                name: "IX_BloqueDeTurnosAgendarVacaciones_IdGrupo",
                table: "BloqueDeTurnosAgendarVacaciones",
                column: "IdGrupo");

            migrationBuilder.CreateIndex(
                name: "IX_BloqueDeTurnosAgendarVacaciones_IdJefeArea",
                table: "BloqueDeTurnosAgendarVacaciones",
                column: "IdJefeArea");

            migrationBuilder.CreateIndex(
                name: "IX_BloqueDeTurnosAgendarVacaciones_IdPeriodoDeProgramacionAnual",
                table: "BloqueDeTurnosAgendarVacaciones",
                column: "IdPeriodoDeProgramacionAnual");

            migrationBuilder.CreateIndex(
                name: "IX_BloqueDeTurnosAgendarVacaciones_IndiceBloqueDeTurnos_IdPeriodoDeProgramacionAnual",
                table: "BloqueDeTurnosAgendarVacaciones",
                columns: new[] { "IndiceBloqueDeTurnos", "IdPeriodoDeProgramacionAnual" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BloqueDeTurnosAgendarVacaciones");
        }
    }
}
