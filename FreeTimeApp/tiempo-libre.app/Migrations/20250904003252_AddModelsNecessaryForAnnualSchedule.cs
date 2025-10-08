using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tiempo_libre.Migrations
{
    /// <inheritdoc />
    public partial class AddModelsNecessaryForAnnualSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "FechaIngreso",
                table: "Users",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CentroCoste",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Nomina",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Posicion",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DiasInhabiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaInicial = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaFinal = table.Column<DateOnly>(type: "date", nullable: false),
                    AnioFechaInicial = table.Column<int>(type: "int", nullable: false),
                    MesFechaInicial = table.Column<int>(type: "int", nullable: false),
                    DiaFechaInicial = table.Column<int>(type: "int", nullable: false),
                    AnioFechaFinal = table.Column<int>(type: "int", nullable: false),
                    MesFechaFinal = table.Column<int>(type: "int", nullable: false),
                    DiaFechaFinal = table.Column<int>(type: "int", nullable: false),
                    Detalles = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    TipoActividadDelDia = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiasInhabiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reglas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    NumDeGrupos = table.Column<int>(type: "int", nullable: false),
                    Prioridad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reglas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GruposPorRegla",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdRegla = table.Column<int>(type: "int", nullable: false),
                    IdGrupo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GruposPorRegla", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GruposPorRegla_Grupos_IdGrupo",
                        column: x => x.IdGrupo,
                        principalTable: "Grupos",
                        principalColumn: "GrupoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GruposPorRegla_Reglas_IdRegla",
                        column: x => x.IdRegla,
                        principalTable: "Reglas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IncidenciaOPermisos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaInicial = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaFinal = table.Column<DateOnly>(type: "date", nullable: false),
                    AnioFechaInicial = table.Column<int>(type: "int", nullable: false),
                    MesFechaInicial = table.Column<int>(type: "int", nullable: false),
                    DiaFechaInicial = table.Column<int>(type: "int", nullable: false),
                    AnioFechaFinal = table.Column<int>(type: "int", nullable: false),
                    MesFechaFinal = table.Column<int>(type: "int", nullable: false),
                    DiaFechaFinal = table.Column<int>(type: "int", nullable: false),
                    DiaDeLaSemana = table.Column<int>(type: "int", nullable: false),
                    IdUsuarioAutoiza = table.Column<int>(type: "int", nullable: false),
                    IdUsuarioSindicato = table.Column<int>(type: "int", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdUsuarioEmpleado = table.Column<int>(type: "int", nullable: false),
                    NominaEmpleado = table.Column<int>(type: "int", nullable: false),
                    IdGrupo = table.Column<int>(type: "int", nullable: false),
                    IdRegla = table.Column<int>(type: "int", nullable: false),
                    Detalles = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    TiposDeIncedencia = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidenciaOPermisos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidenciaOPermisos_Grupos_IdGrupo",
                        column: x => x.IdGrupo,
                        principalTable: "Grupos",
                        principalColumn: "GrupoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncidenciaOPermisos_Reglas_IdRegla",
                        column: x => x.IdRegla,
                        principalTable: "Reglas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncidenciaOPermisos_Users_IdUsuarioAutoiza",
                        column: x => x.IdUsuarioAutoiza,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncidenciaOPermisos_Users_IdUsuarioEmpleado",
                        column: x => x.IdUsuarioEmpleado,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncidenciaOPermisos_Users_IdUsuarioSindicato",
                        column: x => x.IdUsuarioSindicato,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TurnosXGruposXReglas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IndicePorRegla = table.Column<int>(type: "int", nullable: false),
                    IdRegla = table.Column<int>(type: "int", nullable: false),
                    IdGrupo = table.Column<int>(type: "int", nullable: false),
                    DiaDeLaSemana = table.Column<int>(type: "int", nullable: false),
                    OpcionTipoActividadDelDia = table.Column<int>(type: "int", nullable: false),
                    Turno = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TurnosXGruposXReglas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TurnosXGruposXReglas_Grupos_IdGrupo",
                        column: x => x.IdGrupo,
                        principalTable: "Grupos",
                        principalColumn: "GrupoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TurnosXGruposXReglas_Reglas_IdRegla",
                        column: x => x.IdRegla,
                        principalTable: "Reglas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vacaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NominaEmpleado = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    AnioFecha = table.Column<int>(type: "int", nullable: false),
                    MesFecha = table.Column<int>(type: "int", nullable: false),
                    DiaFecha = table.Column<int>(type: "int", nullable: false),
                    TurnoCubria = table.Column<int>(type: "int", nullable: false),
                    IdGrupo = table.Column<int>(type: "int", nullable: false),
                    IdTurnoXGrupoXRegla = table.Column<int>(type: "int", nullable: false),
                    AsignadaPorJefe = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateOnly>(type: "date", nullable: false),
                    IdUsuarioEmpleadoSindicalizado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vacaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vacaciones_Grupos_IdGrupo",
                        column: x => x.IdGrupo,
                        principalTable: "Grupos",
                        principalColumn: "GrupoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vacaciones_TurnosXGruposXReglas_IdTurnoXGrupoXRegla",
                        column: x => x.IdTurnoXGrupoXRegla,
                        principalTable: "TurnosXGruposXReglas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vacaciones_UsuarioEmpleadoSindicalizado",
                        column: x => x.IdUsuarioEmpleadoSindicalizado,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CalendarioPorEmpleados",
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
                    IdTurnoXGrupoXRegla = table.Column<int>(type: "int", nullable: false),
                    TipoActividadDelDia = table.Column<int>(type: "int", nullable: false),
                    Turno = table.Column<int>(type: "int", nullable: false),
                    IdRegla = table.Column<int>(type: "int", nullable: false),
                    IdGrupo = table.Column<int>(type: "int", nullable: false),
                    IdUsuarioEmpleadoSindicalizado = table.Column<int>(type: "int", nullable: false),
                    NominaEmpleado = table.Column<int>(type: "int", nullable: false),
                    IdVacaciones = table.Column<int>(type: "int", nullable: true),
                    IdIncidenciaOPermiso = table.Column<int>(type: "int", nullable: true),
                    TipoDeIncedencia = table.Column<int>(type: "int", nullable: true),
                    IdDiaInhabil = table.Column<int>(type: "int", nullable: true),
                    DetallesDiaInhabil = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                        name: "FK_CalendarioPorEmpleados_IncidenciaOPermisos_IdIncidenciaOPermiso",
                        column: x => x.IdIncidenciaOPermiso,
                        principalTable: "IncidenciaOPermisos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CalendarioPorEmpleados_Reglas_IdRegla",
                        column: x => x.IdRegla,
                        principalTable: "Reglas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CalendarioPorEmpleados_TurnosXGruposXReglas_IdTurnoXGrupoXRegla",
                        column: x => x.IdTurnoXGrupoXRegla,
                        principalTable: "TurnosXGruposXReglas",
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
                name: "IX_CalendarioPorEmpleados_IdRegla",
                table: "CalendarioPorEmpleados",
                column: "IdRegla");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarioPorEmpleados_IdTurnoXGrupoXRegla",
                table: "CalendarioPorEmpleados",
                column: "IdTurnoXGrupoXRegla");

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

            migrationBuilder.CreateIndex(
                name: "IX_DiasInhabiles_AnioFechaFinal",
                table: "DiasInhabiles",
                column: "AnioFechaFinal");

            migrationBuilder.CreateIndex(
                name: "IX_DiasInhabiles_AnioFechaInicial",
                table: "DiasInhabiles",
                column: "AnioFechaInicial");

            migrationBuilder.CreateIndex(
                name: "IX_DiasInhabiles_DiaFechaFinal",
                table: "DiasInhabiles",
                column: "DiaFechaFinal");

            migrationBuilder.CreateIndex(
                name: "IX_DiasInhabiles_DiaFechaInicial",
                table: "DiasInhabiles",
                column: "DiaFechaInicial");

            migrationBuilder.CreateIndex(
                name: "IX_DiasInhabiles_MesFechaFinal",
                table: "DiasInhabiles",
                column: "MesFechaFinal");

            migrationBuilder.CreateIndex(
                name: "IX_DiasInhabiles_MesFechaInicial",
                table: "DiasInhabiles",
                column: "MesFechaInicial");

            migrationBuilder.CreateIndex(
                name: "IX_GruposPorRegla_IdGrupo",
                table: "GruposPorRegla",
                column: "IdGrupo");

            migrationBuilder.CreateIndex(
                name: "IX_GruposPorRegla_IdRegla",
                table: "GruposPorRegla",
                column: "IdRegla");

            migrationBuilder.CreateIndex(
                name: "IX_IncidenciaOPermisos_AnioFechaInicial",
                table: "IncidenciaOPermisos",
                column: "AnioFechaInicial");

            migrationBuilder.CreateIndex(
                name: "IX_IncidenciaOPermisos_DiaFechaInicial",
                table: "IncidenciaOPermisos",
                column: "DiaFechaInicial");

            migrationBuilder.CreateIndex(
                name: "IX_IncidenciaOPermisos_Fecha",
                table: "IncidenciaOPermisos",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_IncidenciaOPermisos_IdGrupo",
                table: "IncidenciaOPermisos",
                column: "IdGrupo");

            migrationBuilder.CreateIndex(
                name: "IX_IncidenciaOPermisos_IdRegla",
                table: "IncidenciaOPermisos",
                column: "IdRegla");

            migrationBuilder.CreateIndex(
                name: "IX_IncidenciaOPermisos_IdUsuarioAutoiza",
                table: "IncidenciaOPermisos",
                column: "IdUsuarioAutoiza");

            migrationBuilder.CreateIndex(
                name: "IX_IncidenciaOPermisos_IdUsuarioEmpleado",
                table: "IncidenciaOPermisos",
                column: "IdUsuarioEmpleado");

            migrationBuilder.CreateIndex(
                name: "IX_IncidenciaOPermisos_IdUsuarioSindicato",
                table: "IncidenciaOPermisos",
                column: "IdUsuarioSindicato");

            migrationBuilder.CreateIndex(
                name: "IX_IncidenciaOPermisos_MesFechaInicial",
                table: "IncidenciaOPermisos",
                column: "MesFechaInicial");

            migrationBuilder.CreateIndex(
                name: "IX_IncidenciaOPermisos_NominaEmpleado",
                table: "IncidenciaOPermisos",
                column: "NominaEmpleado");

            migrationBuilder.CreateIndex(
                name: "IX_TurnosXGruposXReglas_IdGrupo",
                table: "TurnosXGruposXReglas",
                column: "IdGrupo");

            migrationBuilder.CreateIndex(
                name: "IX_TurnosXGruposXReglas_IdRegla",
                table: "TurnosXGruposXReglas",
                column: "IdRegla");

            migrationBuilder.CreateIndex(
                name: "IX_Vacaciones_AnioFecha",
                table: "Vacaciones",
                column: "AnioFecha");

            migrationBuilder.CreateIndex(
                name: "IX_Vacaciones_DiaFecha",
                table: "Vacaciones",
                column: "DiaFecha");

            migrationBuilder.CreateIndex(
                name: "IX_Vacaciones_Fecha",
                table: "Vacaciones",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_Vacaciones_IdGrupo",
                table: "Vacaciones",
                column: "IdGrupo");

            migrationBuilder.CreateIndex(
                name: "IX_Vacaciones_IdTurnoXGrupoXRegla",
                table: "Vacaciones",
                column: "IdTurnoXGrupoXRegla");

            migrationBuilder.CreateIndex(
                name: "IX_Vacaciones_IdUsuarioEmpleadoSindicalizado",
                table: "Vacaciones",
                column: "IdUsuarioEmpleadoSindicalizado");

            migrationBuilder.CreateIndex(
                name: "IX_Vacaciones_MesFecha",
                table: "Vacaciones",
                column: "MesFecha");

            migrationBuilder.CreateIndex(
                name: "IX_Vacaciones_NominaEmpleado",
                table: "Vacaciones",
                column: "NominaEmpleado");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalendarioPorEmpleados");

            migrationBuilder.DropTable(
                name: "GruposPorRegla");

            migrationBuilder.DropTable(
                name: "DiasInhabiles");

            migrationBuilder.DropTable(
                name: "IncidenciaOPermisos");

            migrationBuilder.DropTable(
                name: "Vacaciones");

            migrationBuilder.DropTable(
                name: "TurnosXGruposXReglas");

            migrationBuilder.DropTable(
                name: "Reglas");

            migrationBuilder.DropColumn(
                name: "CentroCoste",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Nomina",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Posicion",
                table: "Users");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaIngreso",
                table: "Users",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);
        }
    }
}
