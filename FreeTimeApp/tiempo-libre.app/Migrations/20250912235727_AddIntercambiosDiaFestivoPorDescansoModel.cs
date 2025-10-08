using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tiempo_libre.Migrations
{
    /// <inheritdoc />
    public partial class AddIntercambiosDiaFestivoPorDescansoModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IntercambiosDiaFestivoPorDescanso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdJefeArea = table.Column<int>(type: "int", nullable: false),
                    IdLiderGrupo = table.Column<int>(type: "int", nullable: false),
                    IdJefeAutoriza = table.Column<int>(type: "int", nullable: true),
                    IdUsuarioComiteSindicalSolicitante = table.Column<int>(type: "int", nullable: true),
                    IdArea = table.Column<int>(type: "int", nullable: false),
                    IdGrupo = table.Column<int>(type: "int", nullable: false),
                    IdEmpleadoSindicalizadoSolicitante = table.Column<int>(type: "int", nullable: false),
                    NominaEmpleadoSindical = table.Column<int>(type: "int", nullable: false),
                    IdDiaFestivoTrabajado = table.Column<int>(type: "int", nullable: false),
                    FechaDiaFestivoTrabajado = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaDiaDescansoQueTomara = table.Column<DateOnly>(type: "date", nullable: false),
                    Justificacion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Estatus = table.Column<int>(type: "int", nullable: false),
                    IdCalPorEmpDiaDescansoQueTomara = table.Column<int>(type: "int", nullable: true),
                    TiposDeCambiosEnum = table.Column<int>(type: "int", nullable: false),
                    CambiosAplicados = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IdPeriodoDeProgramacionAnual = table.Column<int>(type: "int", nullable: false),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntercambiosDiaFestivoPorDescanso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IntercambiosDiaFestivoPorDescanso_Areas_IdArea",
                        column: x => x.IdArea,
                        principalTable: "Areas",
                        principalColumn: "AreaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IntercambiosDiaFestivoPorDescanso_CalendarioPorEmpleados_IdCalPorEmpDiaDescansoQueTomara",
                        column: x => x.IdCalPorEmpDiaDescansoQueTomara,
                        principalTable: "CalendarioPorEmpleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IntercambiosDiaFestivoPorDescanso_DiasFestivosTrabajados_IdDiaFestivoTrabajado",
                        column: x => x.IdDiaFestivoTrabajado,
                        principalTable: "DiasFestivosTrabajados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IntercambiosDiaFestivoPorDescanso_Grupos_IdGrupo",
                        column: x => x.IdGrupo,
                        principalTable: "Grupos",
                        principalColumn: "GrupoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IntercambiosDiaFestivoPorDescanso_ProgramacionesAnuales_IdPeriodoDeProgramacionAnual",
                        column: x => x.IdPeriodoDeProgramacionAnual,
                        principalTable: "ProgramacionesAnuales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IntercambiosDiaFestivoPorDescanso_Users_IdEmpleadoSindicalizadoSolicitante",
                        column: x => x.IdEmpleadoSindicalizadoSolicitante,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IntercambiosDiaFestivoPorDescanso_Users_IdJefeArea",
                        column: x => x.IdJefeArea,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IntercambiosDiaFestivoPorDescanso_Users_IdJefeAutoriza",
                        column: x => x.IdJefeAutoriza,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IntercambiosDiaFestivoPorDescanso_Users_IdLiderGrupo",
                        column: x => x.IdLiderGrupo,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IntercambiosDiaFestivoPorDescanso_Users_IdUsuarioComiteSindicalSolicitante",
                        column: x => x.IdUsuarioComiteSindicalSolicitante,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IntercambiosDiaFestivoPorDescanso_IdArea",
                table: "IntercambiosDiaFestivoPorDescanso",
                column: "IdArea");

            migrationBuilder.CreateIndex(
                name: "IX_IntercambiosDiaFestivoPorDescanso_IdCalPorEmpDiaDescansoQueTomara",
                table: "IntercambiosDiaFestivoPorDescanso",
                column: "IdCalPorEmpDiaDescansoQueTomara");

            migrationBuilder.CreateIndex(
                name: "IX_IntercambiosDiaFestivoPorDescanso_IdDiaFestivoTrabajado",
                table: "IntercambiosDiaFestivoPorDescanso",
                column: "IdDiaFestivoTrabajado");

            migrationBuilder.CreateIndex(
                name: "IX_IntercambiosDiaFestivoPorDescanso_IdEmpleadoSindicalizadoSolicitante",
                table: "IntercambiosDiaFestivoPorDescanso",
                column: "IdEmpleadoSindicalizadoSolicitante");

            migrationBuilder.CreateIndex(
                name: "IX_IntercambiosDiaFestivoPorDescanso_IdGrupo",
                table: "IntercambiosDiaFestivoPorDescanso",
                column: "IdGrupo");

            migrationBuilder.CreateIndex(
                name: "IX_IntercambiosDiaFestivoPorDescanso_IdJefeArea",
                table: "IntercambiosDiaFestivoPorDescanso",
                column: "IdJefeArea");

            migrationBuilder.CreateIndex(
                name: "IX_IntercambiosDiaFestivoPorDescanso_IdJefeAutoriza",
                table: "IntercambiosDiaFestivoPorDescanso",
                column: "IdJefeAutoriza");

            migrationBuilder.CreateIndex(
                name: "IX_IntercambiosDiaFestivoPorDescanso_IdLiderGrupo",
                table: "IntercambiosDiaFestivoPorDescanso",
                column: "IdLiderGrupo");

            migrationBuilder.CreateIndex(
                name: "IX_IntercambiosDiaFestivoPorDescanso_IdPeriodoDeProgramacionAnual",
                table: "IntercambiosDiaFestivoPorDescanso",
                column: "IdPeriodoDeProgramacionAnual");

            migrationBuilder.CreateIndex(
                name: "IX_IntercambiosDiaFestivoPorDescanso_IdUsuarioComiteSindicalSolicitante",
                table: "IntercambiosDiaFestivoPorDescanso",
                column: "IdUsuarioComiteSindicalSolicitante");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IntercambiosDiaFestivoPorDescanso");
        }
    }
}
