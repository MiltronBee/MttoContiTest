using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tiempo_libre.Migrations
{
    /// <inheritdoc />
    public partial class AddSolicitudIntercambiosOReprogramacionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReprogramacionesDeVacaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEmpleadoSindicalizadoSolicitante = table.Column<int>(type: "int", nullable: false),
                    IdUsuarioComiteSindicalSolicitante = table.Column<int>(type: "int", nullable: true),
                    NominaEmpleadoSindical = table.Column<int>(type: "int", nullable: false),
                    IdPeriodoDeProgramacionAnual = table.Column<int>(type: "int", nullable: false),
                    Detalles = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    FechaDiasDeVacacionOriginal = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaDiasDeVacacionReprogramada = table.Column<DateOnly>(type: "date", nullable: false),
                    IdCalPorEmpCambiar = table.Column<int>(type: "int", nullable: false),
                    IdCalPorEmpNuevo = table.Column<int>(type: "int", nullable: false),
                    Estatus = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReprogramacionesDeVacaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReprogramacionesDeVacaciones_CalendarioPorEmpleados_IdCalPorEmpCambiar",
                        column: x => x.IdCalPorEmpCambiar,
                        principalTable: "CalendarioPorEmpleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReprogramacionesDeVacaciones_CalendarioPorEmpleados_IdCalPorEmpNuevo",
                        column: x => x.IdCalPorEmpNuevo,
                        principalTable: "CalendarioPorEmpleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReprogramacionesDeVacaciones_ProgramacionesAnuales_IdPeriodoDeProgramacionAnual",
                        column: x => x.IdPeriodoDeProgramacionAnual,
                        principalTable: "ProgramacionesAnuales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReprogramacionesDeVacaciones_Users_IdEmpleadoSindicalizadoSolicitante",
                        column: x => x.IdEmpleadoSindicalizadoSolicitante,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReprogramacionesDeVacaciones_Users_IdUsuarioComiteSindicalSolicitante",
                        column: x => x.IdUsuarioComiteSindicalSolicitante,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudIntercambiosOReprogramacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdJefeArea = table.Column<int>(type: "int", nullable: false),
                    IdLiderGrupo = table.Column<int>(type: "int", nullable: false),
                    IdUsuarioComiteSindicalSolicitante = table.Column<int>(type: "int", nullable: true),
                    IdEmpleadoSindicalizadoSolicitante = table.Column<int>(type: "int", nullable: false),
                    IdJefeLiderAutoriza = table.Column<int>(type: "int", nullable: true),
                    IdArea = table.Column<int>(type: "int", nullable: false),
                    IdGrupo = table.Column<int>(type: "int", nullable: false),
                    IdIntercambiosDiaFestivoPorDescanso = table.Column<int>(type: "int", nullable: true),
                    IdReprogramacionesDeVacaciones = table.Column<int>(type: "int", nullable: true),
                    TipoDeSolicitud = table.Column<int>(type: "int", nullable: false),
                    NominaEmpleadoSindical = table.Column<int>(type: "int", nullable: false),
                    Justificacion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Estatus = table.Column<int>(type: "int", nullable: false),
                    FechaRespuesta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdPeriodoDeProgramacionAnual = table.Column<int>(type: "int", nullable: false),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudIntercambiosOReprogramacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudIntercambiosOReprogramacion_Areas_IdArea",
                        column: x => x.IdArea,
                        principalTable: "Areas",
                        principalColumn: "AreaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudIntercambiosOReprogramacion_Grupos_IdGrupo",
                        column: x => x.IdGrupo,
                        principalTable: "Grupos",
                        principalColumn: "GrupoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudIntercambiosOReprogramacion_IntercambiosDiaFestivoPorDescanso_IdIntercambiosDiaFestivoPorDescanso",
                        column: x => x.IdIntercambiosDiaFestivoPorDescanso,
                        principalTable: "IntercambiosDiaFestivoPorDescanso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudIntercambiosOReprogramacion_ProgramacionesAnuales_IdPeriodoDeProgramacionAnual",
                        column: x => x.IdPeriodoDeProgramacionAnual,
                        principalTable: "ProgramacionesAnuales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudIntercambiosOReprogramacion_ReprogramacionesDeVacaciones_IdReprogramacionesDeVacaciones",
                        column: x => x.IdReprogramacionesDeVacaciones,
                        principalTable: "ReprogramacionesDeVacaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudIntercambiosOReprogramacion_Users_IdEmpleadoSindicalizadoSolicitante",
                        column: x => x.IdEmpleadoSindicalizadoSolicitante,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudIntercambiosOReprogramacion_Users_IdJefeArea",
                        column: x => x.IdJefeArea,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudIntercambiosOReprogramacion_Users_IdJefeLiderAutoriza",
                        column: x => x.IdJefeLiderAutoriza,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudIntercambiosOReprogramacion_Users_IdLiderGrupo",
                        column: x => x.IdLiderGrupo,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudIntercambiosOReprogramacion_Users_IdUsuarioComiteSindicalSolicitante",
                        column: x => x.IdUsuarioComiteSindicalSolicitante,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReprogramacionesDeVacaciones_IdCalPorEmpCambiar",
                table: "ReprogramacionesDeVacaciones",
                column: "IdCalPorEmpCambiar");

            migrationBuilder.CreateIndex(
                name: "IX_ReprogramacionesDeVacaciones_IdCalPorEmpNuevo",
                table: "ReprogramacionesDeVacaciones",
                column: "IdCalPorEmpNuevo");

            migrationBuilder.CreateIndex(
                name: "IX_ReprogramacionesDeVacaciones_IdEmpleadoSindicalizadoSolicitante",
                table: "ReprogramacionesDeVacaciones",
                column: "IdEmpleadoSindicalizadoSolicitante");

            migrationBuilder.CreateIndex(
                name: "IX_ReprogramacionesDeVacaciones_IdPeriodoDeProgramacionAnual",
                table: "ReprogramacionesDeVacaciones",
                column: "IdPeriodoDeProgramacionAnual");

            migrationBuilder.CreateIndex(
                name: "IX_ReprogramacionesDeVacaciones_IdUsuarioComiteSindicalSolicitante",
                table: "ReprogramacionesDeVacaciones",
                column: "IdUsuarioComiteSindicalSolicitante");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudIntercambiosOReprogramacion_IdArea",
                table: "SolicitudIntercambiosOReprogramacion",
                column: "IdArea");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudIntercambiosOReprogramacion_IdEmpleadoSindicalizadoSolicitante",
                table: "SolicitudIntercambiosOReprogramacion",
                column: "IdEmpleadoSindicalizadoSolicitante");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudIntercambiosOReprogramacion_IdGrupo",
                table: "SolicitudIntercambiosOReprogramacion",
                column: "IdGrupo");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudIntercambiosOReprogramacion_IdIntercambiosDiaFestivoPorDescanso",
                table: "SolicitudIntercambiosOReprogramacion",
                column: "IdIntercambiosDiaFestivoPorDescanso");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudIntercambiosOReprogramacion_IdJefeArea",
                table: "SolicitudIntercambiosOReprogramacion",
                column: "IdJefeArea");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudIntercambiosOReprogramacion_IdJefeLiderAutoriza",
                table: "SolicitudIntercambiosOReprogramacion",
                column: "IdJefeLiderAutoriza");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudIntercambiosOReprogramacion_IdLiderGrupo",
                table: "SolicitudIntercambiosOReprogramacion",
                column: "IdLiderGrupo");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudIntercambiosOReprogramacion_IdPeriodoDeProgramacionAnual",
                table: "SolicitudIntercambiosOReprogramacion",
                column: "IdPeriodoDeProgramacionAnual");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudIntercambiosOReprogramacion_IdReprogramacionesDeVacaciones",
                table: "SolicitudIntercambiosOReprogramacion",
                column: "IdReprogramacionesDeVacaciones");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudIntercambiosOReprogramacion_IdUsuarioComiteSindicalSolicitante",
                table: "SolicitudIntercambiosOReprogramacion",
                column: "IdUsuarioComiteSindicalSolicitante");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolicitudIntercambiosOReprogramacion");

            migrationBuilder.DropTable(
                name: "ReprogramacionesDeVacaciones");
        }
    }
}
