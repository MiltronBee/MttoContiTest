using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tiempo_libre.Migrations
{
    /// <inheritdoc />
    public partial class ReestructuringCalendarForEmployees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IntercambiosDiaFestivoPorDescanso_CalendarioPorEmpleados_IdCalPorEmpDiaDescansoQueTomara",
                table: "IntercambiosDiaFestivoPorDescanso");

            migrationBuilder.DropForeignKey(
                name: "FK_ReprogramacionesDeVacaciones_CalendarioPorEmpleados_IdCalPorEmpCambiar",
                table: "ReprogramacionesDeVacaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_ReprogramacionesDeVacaciones_CalendarioPorEmpleados_IdCalPorEmpNuevo",
                table: "ReprogramacionesDeVacaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservacionesDeVacacionesPorEmpleado_CalendarioPorEmpleados_IdCalPorEmpDeVacaciones",
                table: "ReservacionesDeVacacionesPorEmpleado");

            migrationBuilder.DropTable(
                name: "CalendarioPorEmpleados");

            migrationBuilder.RenameColumn(
                name: "IdCalPorEmpDeVacaciones",
                table: "ReservacionesDeVacacionesPorEmpleado",
                newName: "IdProgramacionAnual");

            migrationBuilder.RenameColumn(
                name: "Cretated_At",
                table: "ReservacionesDeVacacionesPorEmpleado",
                newName: "Created_At");

            migrationBuilder.RenameIndex(
                name: "IX_ReservacionesDeVacacionesPorEmpleado_IdCalPorEmpDeVacaciones",
                table: "ReservacionesDeVacacionesPorEmpleado",
                newName: "IX_ReservacionesDeVacacionesPorEmpleado_IdProgramacionAnual");

            migrationBuilder.RenameColumn(
                name: "IdCalPorEmpNuevo",
                table: "ReprogramacionesDeVacaciones",
                newName: "IdDiaCalEmpNuevo");

            migrationBuilder.RenameColumn(
                name: "IdCalPorEmpCambiar",
                table: "ReprogramacionesDeVacaciones",
                newName: "IdDiaCalEmpCambiar");

            migrationBuilder.RenameIndex(
                name: "IX_ReprogramacionesDeVacaciones_IdCalPorEmpNuevo",
                table: "ReprogramacionesDeVacaciones",
                newName: "IX_ReprogramacionesDeVacaciones_IdDiaCalEmpNuevo");

            migrationBuilder.RenameIndex(
                name: "IX_ReprogramacionesDeVacaciones_IdCalPorEmpCambiar",
                table: "ReprogramacionesDeVacaciones",
                newName: "IX_ReprogramacionesDeVacaciones_IdDiaCalEmpCambiar");

            migrationBuilder.RenameColumn(
                name: "IdCalPorEmpDiaDescansoQueTomara",
                table: "IntercambiosDiaFestivoPorDescanso",
                newName: "IdDiaCalEmpDescansoQueTomara");

            migrationBuilder.RenameIndex(
                name: "IX_IntercambiosDiaFestivoPorDescanso_IdCalPorEmpDiaDescansoQueTomara",
                table: "IntercambiosDiaFestivoPorDescanso",
                newName: "IX_IntercambiosDiaFestivoPorDescanso_IdDiaCalEmpDescansoQueTomara");

            migrationBuilder.AddColumn<int>(
                name: "IdCalendarioEmpleado",
                table: "ReservacionesDeVacacionesPorEmpleado",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdDiaCalendarioEmpleado",
                table: "ReservacionesDeVacacionesPorEmpleado",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CalendarioEmpleados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdRegla = table.Column<int>(type: "int", nullable: false),
                    IdArea = table.Column<int>(type: "int", nullable: false),
                    IdGrupo = table.Column<int>(type: "int", nullable: false),
                    IdProgramacionAnual = table.Column<int>(type: "int", nullable: true),
                    IdUsuarioEmpleadoSindicalizado = table.Column<int>(type: "int", nullable: false),
                    NominaEmpleado = table.Column<int>(type: "int", nullable: false),
                    IdVacacionesPorAntiguedad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarioEmpleados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalendarioEmpleados_Areas_IdArea",
                        column: x => x.IdArea,
                        principalTable: "Areas",
                        principalColumn: "AreaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CalendarioEmpleados_Grupos_IdGrupo",
                        column: x => x.IdGrupo,
                        principalTable: "Grupos",
                        principalColumn: "GrupoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CalendarioEmpleados_ProgramacionesAnuales_IdProgramacionAnual",
                        column: x => x.IdProgramacionAnual,
                        principalTable: "ProgramacionesAnuales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CalendarioEmpleados_Reglas_IdRegla",
                        column: x => x.IdRegla,
                        principalTable: "Reglas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CalendarioEmpleados_Users_IdUsuarioEmpleadoSindicalizado",
                        column: x => x.IdUsuarioEmpleadoSindicalizado,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CalendarioEmpleados_VacacionesPorAntiguedad_IdVacacionesPorAntiguedad",
                        column: x => x.IdVacacionesPorAntiguedad,
                        principalTable: "VacacionesPorAntiguedad",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DiasCalendarioEmpleado",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaDelDia = table.Column<DateOnly>(type: "date", nullable: false),
                    AnioFecha = table.Column<int>(type: "int", nullable: false),
                    MesFecha = table.Column<int>(type: "int", nullable: false),
                    DiaFecha = table.Column<int>(type: "int", nullable: false),
                    DiaDeLaSemana = table.Column<int>(type: "int", nullable: false),
                    TipoActividadDelDia = table.Column<int>(type: "int", nullable: false),
                    Turno = table.Column<int>(type: "int", nullable: false),
                    TipoDeIncedencia = table.Column<int>(type: "int", nullable: true),
                    IdProgramacionAnual = table.Column<int>(type: "int", nullable: false),
                    IdCalendarioEmpleado = table.Column<int>(type: "int", nullable: false),
                    IdArea = table.Column<int>(type: "int", nullable: false),
                    IdGrupo = table.Column<int>(type: "int", nullable: false),
                    IdRegla = table.Column<int>(type: "int", nullable: false),
                    IdRolSemanal = table.Column<int>(type: "int", nullable: false),
                    IdTurnoXRolSemanalXRegla = table.Column<int>(type: "int", nullable: false),
                    IdUsuarioEmpleadoSindicalizado = table.Column<int>(type: "int", nullable: false),
                    NominaEmpleado = table.Column<int>(type: "int", nullable: false),
                    IdVacaciones = table.Column<int>(type: "int", nullable: true),
                    IdIncidenciaOPermiso = table.Column<int>(type: "int", nullable: true),
                    IdDiaInhabil = table.Column<int>(type: "int", nullable: true),
                    DetallesDiaInhabil = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdIntercambioDiaFestivoPorDescanso = table.Column<int>(type: "int", nullable: true),
                    IdReprogramacionDeVacaciones = table.Column<int>(type: "int", nullable: true),
                    EsDiaFestivo = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EsDiaDeDescanso = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EsDiaLaboral = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    EsDiaInhabil = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EsDiaDeVacaciones = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EsDiaDePermiso = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EsDiaReprogramado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EsDiaIntercambiado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiasCalendarioEmpleado", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiasCalendarioEmpleado_Areas_IdArea",
                        column: x => x.IdArea,
                        principalTable: "Areas",
                        principalColumn: "AreaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiasCalendarioEmpleado_CalendarioEmpleados_IdCalendarioEmpleado",
                        column: x => x.IdCalendarioEmpleado,
                        principalTable: "CalendarioEmpleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiasCalendarioEmpleado_DiasInhabiles_IdDiaInhabil",
                        column: x => x.IdDiaInhabil,
                        principalTable: "DiasInhabiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiasCalendarioEmpleado_Grupos_IdGrupo",
                        column: x => x.IdGrupo,
                        principalTable: "Grupos",
                        principalColumn: "GrupoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiasCalendarioEmpleado_IncidenciasOPermisos_IdIncidenciaOPermiso",
                        column: x => x.IdIncidenciaOPermiso,
                        principalTable: "IncidenciasOPermisos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiasCalendarioEmpleado_IntercambiosDiaFestivoPorDescanso_IdIntercambioDiaFestivoPorDescanso",
                        column: x => x.IdIntercambioDiaFestivoPorDescanso,
                        principalTable: "IntercambiosDiaFestivoPorDescanso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiasCalendarioEmpleado_ProgramacionesAnuales_IdProgramacionAnual",
                        column: x => x.IdProgramacionAnual,
                        principalTable: "ProgramacionesAnuales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiasCalendarioEmpleado_Reglas_IdRegla",
                        column: x => x.IdRegla,
                        principalTable: "Reglas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiasCalendarioEmpleado_ReprogramacionesDeVacaciones_IdReprogramacionDeVacaciones",
                        column: x => x.IdReprogramacionDeVacaciones,
                        principalTable: "ReprogramacionesDeVacaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiasCalendarioEmpleado_RolesSemanales_IdRolSemanal",
                        column: x => x.IdRolSemanal,
                        principalTable: "RolesSemanales",
                        principalColumn: "RolSemanalId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiasCalendarioEmpleado_TurnosXRolSemanalXRegla_IdTurnoXRolSemanalXRegla",
                        column: x => x.IdTurnoXRolSemanalXRegla,
                        principalTable: "TurnosXRolSemanalXRegla",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiasCalendarioEmpleado_Users_IdUsuarioEmpleadoSindicalizado",
                        column: x => x.IdUsuarioEmpleadoSindicalizado,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiasCalendarioEmpleado_Vacaciones_IdVacaciones",
                        column: x => x.IdVacaciones,
                        principalTable: "Vacaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReservacionesDeVacacionesPorEmpleado_IdCalendarioEmpleado",
                table: "ReservacionesDeVacacionesPorEmpleado",
                column: "IdCalendarioEmpleado");

            migrationBuilder.CreateIndex(
                name: "IX_ReservacionesDeVacacionesPorEmpleado_IdDiaCalendarioEmpleado",
                table: "ReservacionesDeVacacionesPorEmpleado",
                column: "IdDiaCalendarioEmpleado");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarioEmpleados_IdArea",
                table: "CalendarioEmpleados",
                column: "IdArea");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarioEmpleados_IdGrupo",
                table: "CalendarioEmpleados",
                column: "IdGrupo");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarioEmpleados_IdProgramacionAnual",
                table: "CalendarioEmpleados",
                column: "IdProgramacionAnual");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarioEmpleados_IdRegla",
                table: "CalendarioEmpleados",
                column: "IdRegla");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarioEmpleados_IdUsuarioEmpleadoSindicalizado",
                table: "CalendarioEmpleados",
                column: "IdUsuarioEmpleadoSindicalizado");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarioEmpleados_IdVacacionesPorAntiguedad",
                table: "CalendarioEmpleados",
                column: "IdVacacionesPorAntiguedad");

            migrationBuilder.CreateIndex(
                name: "IX_DiasCalendarioEmpleado_IdArea",
                table: "DiasCalendarioEmpleado",
                column: "IdArea");

            migrationBuilder.CreateIndex(
                name: "IX_DiasCalendarioEmpleado_IdCalendarioEmpleado",
                table: "DiasCalendarioEmpleado",
                column: "IdCalendarioEmpleado");

            migrationBuilder.CreateIndex(
                name: "IX_DiasCalendarioEmpleado_IdDiaInhabil",
                table: "DiasCalendarioEmpleado",
                column: "IdDiaInhabil");

            migrationBuilder.CreateIndex(
                name: "IX_DiasCalendarioEmpleado_IdGrupo",
                table: "DiasCalendarioEmpleado",
                column: "IdGrupo");

            migrationBuilder.CreateIndex(
                name: "IX_DiasCalendarioEmpleado_IdIncidenciaOPermiso",
                table: "DiasCalendarioEmpleado",
                column: "IdIncidenciaOPermiso");

            migrationBuilder.CreateIndex(
                name: "IX_DiasCalendarioEmpleado_IdIntercambioDiaFestivoPorDescanso",
                table: "DiasCalendarioEmpleado",
                column: "IdIntercambioDiaFestivoPorDescanso");

            migrationBuilder.CreateIndex(
                name: "IX_DiasCalendarioEmpleado_IdProgramacionAnual",
                table: "DiasCalendarioEmpleado",
                column: "IdProgramacionAnual");

            migrationBuilder.CreateIndex(
                name: "IX_DiasCalendarioEmpleado_IdRegla",
                table: "DiasCalendarioEmpleado",
                column: "IdRegla");

            migrationBuilder.CreateIndex(
                name: "IX_DiasCalendarioEmpleado_IdReprogramacionDeVacaciones",
                table: "DiasCalendarioEmpleado",
                column: "IdReprogramacionDeVacaciones");

            migrationBuilder.CreateIndex(
                name: "IX_DiasCalendarioEmpleado_IdRolSemanal",
                table: "DiasCalendarioEmpleado",
                column: "IdRolSemanal");

            migrationBuilder.CreateIndex(
                name: "IX_DiasCalendarioEmpleado_IdTurnoXRolSemanalXRegla",
                table: "DiasCalendarioEmpleado",
                column: "IdTurnoXRolSemanalXRegla");

            migrationBuilder.CreateIndex(
                name: "IX_DiasCalendarioEmpleado_IdUsuarioEmpleadoSindicalizado",
                table: "DiasCalendarioEmpleado",
                column: "IdUsuarioEmpleadoSindicalizado");

            migrationBuilder.CreateIndex(
                name: "IX_DiasCalendarioEmpleado_IdVacaciones",
                table: "DiasCalendarioEmpleado",
                column: "IdVacaciones");

            migrationBuilder.AddForeignKey(
                name: "FK_IntercambiosDiaFestivoPorDescanso_DiasCalendarioEmpleado_IdDiaCalEmpDescansoQueTomara",
                table: "IntercambiosDiaFestivoPorDescanso",
                column: "IdDiaCalEmpDescansoQueTomara",
                principalTable: "DiasCalendarioEmpleado",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReprogramacionesDeVacaciones_DiasCalendarioEmpleado_IdDiaCalEmpCambiar",
                table: "ReprogramacionesDeVacaciones",
                column: "IdDiaCalEmpCambiar",
                principalTable: "DiasCalendarioEmpleado",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReprogramacionesDeVacaciones_DiasCalendarioEmpleado_IdDiaCalEmpNuevo",
                table: "ReprogramacionesDeVacaciones",
                column: "IdDiaCalEmpNuevo",
                principalTable: "DiasCalendarioEmpleado",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservacionesDeVacacionesPorEmpleado_CalendarioEmpleados_IdCalendarioEmpleado",
                table: "ReservacionesDeVacacionesPorEmpleado",
                column: "IdCalendarioEmpleado",
                principalTable: "CalendarioEmpleados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservacionesDeVacacionesPorEmpleado_DiasCalendarioEmpleado_IdDiaCalendarioEmpleado",
                table: "ReservacionesDeVacacionesPorEmpleado",
                column: "IdDiaCalendarioEmpleado",
                principalTable: "DiasCalendarioEmpleado",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservacionesDeVacacionesPorEmpleado_ProgramacionesAnuales_IdProgramacionAnual",
                table: "ReservacionesDeVacacionesPorEmpleado",
                column: "IdProgramacionAnual",
                principalTable: "ProgramacionesAnuales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IntercambiosDiaFestivoPorDescanso_DiasCalendarioEmpleado_IdDiaCalEmpDescansoQueTomara",
                table: "IntercambiosDiaFestivoPorDescanso");

            migrationBuilder.DropForeignKey(
                name: "FK_ReprogramacionesDeVacaciones_DiasCalendarioEmpleado_IdDiaCalEmpCambiar",
                table: "ReprogramacionesDeVacaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_ReprogramacionesDeVacaciones_DiasCalendarioEmpleado_IdDiaCalEmpNuevo",
                table: "ReprogramacionesDeVacaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservacionesDeVacacionesPorEmpleado_CalendarioEmpleados_IdCalendarioEmpleado",
                table: "ReservacionesDeVacacionesPorEmpleado");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservacionesDeVacacionesPorEmpleado_DiasCalendarioEmpleado_IdDiaCalendarioEmpleado",
                table: "ReservacionesDeVacacionesPorEmpleado");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservacionesDeVacacionesPorEmpleado_ProgramacionesAnuales_IdProgramacionAnual",
                table: "ReservacionesDeVacacionesPorEmpleado");

            migrationBuilder.DropTable(
                name: "DiasCalendarioEmpleado");

            migrationBuilder.DropTable(
                name: "CalendarioEmpleados");

            migrationBuilder.DropIndex(
                name: "IX_ReservacionesDeVacacionesPorEmpleado_IdCalendarioEmpleado",
                table: "ReservacionesDeVacacionesPorEmpleado");

            migrationBuilder.DropIndex(
                name: "IX_ReservacionesDeVacacionesPorEmpleado_IdDiaCalendarioEmpleado",
                table: "ReservacionesDeVacacionesPorEmpleado");

            migrationBuilder.DropColumn(
                name: "IdCalendarioEmpleado",
                table: "ReservacionesDeVacacionesPorEmpleado");

            migrationBuilder.DropColumn(
                name: "IdDiaCalendarioEmpleado",
                table: "ReservacionesDeVacacionesPorEmpleado");

            migrationBuilder.RenameColumn(
                name: "IdProgramacionAnual",
                table: "ReservacionesDeVacacionesPorEmpleado",
                newName: "IdCalPorEmpDeVacaciones");

            migrationBuilder.RenameColumn(
                name: "Created_At",
                table: "ReservacionesDeVacacionesPorEmpleado",
                newName: "Cretated_At");

            migrationBuilder.RenameIndex(
                name: "IX_ReservacionesDeVacacionesPorEmpleado_IdProgramacionAnual",
                table: "ReservacionesDeVacacionesPorEmpleado",
                newName: "IX_ReservacionesDeVacacionesPorEmpleado_IdCalPorEmpDeVacaciones");

            migrationBuilder.RenameColumn(
                name: "IdDiaCalEmpNuevo",
                table: "ReprogramacionesDeVacaciones",
                newName: "IdCalPorEmpNuevo");

            migrationBuilder.RenameColumn(
                name: "IdDiaCalEmpCambiar",
                table: "ReprogramacionesDeVacaciones",
                newName: "IdCalPorEmpCambiar");

            migrationBuilder.RenameIndex(
                name: "IX_ReprogramacionesDeVacaciones_IdDiaCalEmpNuevo",
                table: "ReprogramacionesDeVacaciones",
                newName: "IX_ReprogramacionesDeVacaciones_IdCalPorEmpNuevo");

            migrationBuilder.RenameIndex(
                name: "IX_ReprogramacionesDeVacaciones_IdDiaCalEmpCambiar",
                table: "ReprogramacionesDeVacaciones",
                newName: "IX_ReprogramacionesDeVacaciones_IdCalPorEmpCambiar");

            migrationBuilder.RenameColumn(
                name: "IdDiaCalEmpDescansoQueTomara",
                table: "IntercambiosDiaFestivoPorDescanso",
                newName: "IdCalPorEmpDiaDescansoQueTomara");

            migrationBuilder.RenameIndex(
                name: "IX_IntercambiosDiaFestivoPorDescanso_IdDiaCalEmpDescansoQueTomara",
                table: "IntercambiosDiaFestivoPorDescanso",
                newName: "IX_IntercambiosDiaFestivoPorDescanso_IdCalPorEmpDiaDescansoQueTomara");

            migrationBuilder.CreateTable(
                name: "CalendarioPorEmpleados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdDiaInhabil = table.Column<int>(type: "int", nullable: true),
                    IdGrupo = table.Column<int>(type: "int", nullable: false),
                    IdIncidenciaOPermiso = table.Column<int>(type: "int", nullable: true),
                    IdIntercambioDiaFestivoPorDescanso = table.Column<int>(type: "int", nullable: true),
                    IdProgramacionAnual = table.Column<int>(type: "int", nullable: true),
                    IdRegla = table.Column<int>(type: "int", nullable: false),
                    IdReprogramacionDeVacaciones = table.Column<int>(type: "int", nullable: true),
                    IdTurnoXRolSemanalXRegla = table.Column<int>(type: "int", nullable: false),
                    IdUsuarioEmpleadoSindicalizado = table.Column<int>(type: "int", nullable: false),
                    IdVacaciones = table.Column<int>(type: "int", nullable: true),
                    AnioFecha = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DetallesDiaInhabil = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiaDeLaSemana = table.Column<int>(type: "int", nullable: false),
                    DiaFecha = table.Column<int>(type: "int", nullable: false),
                    EsDiaDeDescanso = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EsDiaDePermiso = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EsDiaDeVacaciones = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EsDiaFestivo = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EsDiaInhabil = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EsDiaIntercambiado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EsDiaLaboral = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    EsDiaReprogramado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FechaDelDia = table.Column<DateOnly>(type: "date", nullable: false),
                    IdArea = table.Column<int>(type: "int", nullable: true),
                    IdRolSemanal = table.Column<int>(type: "int", nullable: true),
                    MesFecha = table.Column<int>(type: "int", nullable: false),
                    NominaEmpleado = table.Column<int>(type: "int", nullable: false),
                    TipoActividadDelDia = table.Column<int>(type: "int", nullable: false),
                    TipoDeIncedencia = table.Column<int>(type: "int", nullable: true),
                    Turno = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarioPorEmpleados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalendarioPorEmpleados_DiasInhabiles_IdDiaInhabil",
                        column: x => x.IdDiaInhabil,
                        principalTable: "DiasInhabiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CalendarioPorEmpleados_Grupos_IdGrupo",
                        column: x => x.IdGrupo,
                        principalTable: "Grupos",
                        principalColumn: "GrupoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CalendarioPorEmpleados_IncidenciasOPermisos_IdIncidenciaOPermiso",
                        column: x => x.IdIncidenciaOPermiso,
                        principalTable: "IncidenciasOPermisos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CalendarioPorEmpleados_IntercambiosDiaFestivoPorDescanso_IdIntercambioDiaFestivoPorDescanso",
                        column: x => x.IdIntercambioDiaFestivoPorDescanso,
                        principalTable: "IntercambiosDiaFestivoPorDescanso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CalendarioPorEmpleados_ProgramacionesAnuales_IdProgramacionAnual",
                        column: x => x.IdProgramacionAnual,
                        principalTable: "ProgramacionesAnuales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CalendarioPorEmpleados_Reglas_IdRegla",
                        column: x => x.IdRegla,
                        principalTable: "Reglas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CalendarioPorEmpleados_ReprogramacionesDeVacaciones_IdReprogramacionDeVacaciones",
                        column: x => x.IdReprogramacionDeVacaciones,
                        principalTable: "ReprogramacionesDeVacaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CalendarioPorEmpleados_TurnosXRolSemanalXRegla_IdTurnoXRolSemanalXRegla",
                        column: x => x.IdTurnoXRolSemanalXRegla,
                        principalTable: "TurnosXRolSemanalXRegla",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CalendarioPorEmpleados_Users_IdUsuarioEmpleadoSindicalizado",
                        column: x => x.IdUsuarioEmpleadoSindicalizado,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CalendarioPorEmpleados_Vacaciones_IdVacaciones",
                        column: x => x.IdVacaciones,
                        principalTable: "Vacaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarioPorEmpleados_AnioFecha",
                table: "CalendarioPorEmpleados",
                column: "AnioFecha");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarioPorEmpleados_DiaFecha",
                table: "CalendarioPorEmpleados",
                column: "DiaFecha");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarioPorEmpleados_FechaDelDia",
                table: "CalendarioPorEmpleados",
                column: "FechaDelDia");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarioPorEmpleados_IdDiaInhabil",
                table: "CalendarioPorEmpleados",
                column: "IdDiaInhabil");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarioPorEmpleados_IdGrupo",
                table: "CalendarioPorEmpleados",
                column: "IdGrupo");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarioPorEmpleados_IdIncidenciaOPermiso",
                table: "CalendarioPorEmpleados",
                column: "IdIncidenciaOPermiso");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarioPorEmpleados_IdIntercambioDiaFestivoPorDescanso",
                table: "CalendarioPorEmpleados",
                column: "IdIntercambioDiaFestivoPorDescanso");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarioPorEmpleados_IdProgramacionAnual",
                table: "CalendarioPorEmpleados",
                column: "IdProgramacionAnual");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarioPorEmpleados_IdRegla",
                table: "CalendarioPorEmpleados",
                column: "IdRegla");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarioPorEmpleados_IdReprogramacionDeVacaciones",
                table: "CalendarioPorEmpleados",
                column: "IdReprogramacionDeVacaciones");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarioPorEmpleados_IdTurnoXRolSemanalXRegla",
                table: "CalendarioPorEmpleados",
                column: "IdTurnoXRolSemanalXRegla");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarioPorEmpleados_IdUsuarioEmpleadoSindicalizado",
                table: "CalendarioPorEmpleados",
                column: "IdUsuarioEmpleadoSindicalizado");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarioPorEmpleados_IdVacaciones",
                table: "CalendarioPorEmpleados",
                column: "IdVacaciones");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarioPorEmpleados_MesFecha",
                table: "CalendarioPorEmpleados",
                column: "MesFecha");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarioPorEmpleados_NominaEmpleado",
                table: "CalendarioPorEmpleados",
                column: "NominaEmpleado");

            migrationBuilder.AddForeignKey(
                name: "FK_IntercambiosDiaFestivoPorDescanso_CalendarioPorEmpleados_IdCalPorEmpDiaDescansoQueTomara",
                table: "IntercambiosDiaFestivoPorDescanso",
                column: "IdCalPorEmpDiaDescansoQueTomara",
                principalTable: "CalendarioPorEmpleados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReprogramacionesDeVacaciones_CalendarioPorEmpleados_IdCalPorEmpCambiar",
                table: "ReprogramacionesDeVacaciones",
                column: "IdCalPorEmpCambiar",
                principalTable: "CalendarioPorEmpleados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReprogramacionesDeVacaciones_CalendarioPorEmpleados_IdCalPorEmpNuevo",
                table: "ReprogramacionesDeVacaciones",
                column: "IdCalPorEmpNuevo",
                principalTable: "CalendarioPorEmpleados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservacionesDeVacacionesPorEmpleado_CalendarioPorEmpleados_IdCalPorEmpDeVacaciones",
                table: "ReservacionesDeVacacionesPorEmpleado",
                column: "IdCalPorEmpDeVacaciones",
                principalTable: "CalendarioPorEmpleados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
