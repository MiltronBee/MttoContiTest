using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tiempo_libre.Migrations
{
    /// <inheritdoc />
    public partial class ConfiguracionSistemaVacacionesUnificado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfiguracionVacaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PorcentajeAusenciaMaximo = table.Column<decimal>(type: "decimal(5,2)", nullable: false, defaultValue: 4.5m),
                    PeriodoActual = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Cerrado"),
                    AnioVigente = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionVacaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfiguracionVacaciones_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ExcepcionesPorcentaje",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GrupoId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    PorcentajeMaximoPermitido = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExcepcionesPorcentaje", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExcepcionesPorcentaje_Grupos_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "Grupos",
                        principalColumn: "GrupoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExcepcionesPorcentaje_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExcepcionesPorcentaje_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VacacionesProgramadas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    FechaVacacion = table.Column<DateOnly>(type: "date", nullable: false),
                    TipoVacacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrigenAsignacion = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Manual"),
                    EstadoVacacion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Activa"),
                    PeriodoProgramacion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaProgramacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    PuedeSerIntercambiada = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacacionesProgramadas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VacacionesProgramadas_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VacacionesProgramadas_Users_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VacacionesProgramadas_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesReprogramacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    VacacionOriginalId = table.Column<int>(type: "int", nullable: false),
                    FechaNuevaSolicitada = table.Column<DateOnly>(type: "date", nullable: false),
                    EstadoSolicitud = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Pendiente"),
                    MotivoRechazo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    JefeAreaId = table.Column<int>(type: "int", nullable: true),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    FechaRespuesta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PorcentajeCalculado = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ObservacionesEmpleado = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ObservacionesJefe = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesReprogramacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudesReprogramacion_Users_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudesReprogramacion_Users_JefeAreaId",
                        column: x => x.JefeAreaId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudesReprogramacion_VacacionesProgramadas_VacacionOriginalId",
                        column: x => x.VacacionOriginalId,
                        principalTable: "VacacionesProgramadas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionVacaciones_UpdatedBy",
                table: "ConfiguracionVacaciones",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ExcepcionesPorcentaje_CreatedBy",
                table: "ExcepcionesPorcentaje",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ExcepcionesPorcentaje_GrupoId",
                table: "ExcepcionesPorcentaje",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_ExcepcionesPorcentaje_UpdatedBy",
                table: "ExcepcionesPorcentaje",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesReprogramacion_EmpleadoId",
                table: "SolicitudesReprogramacion",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesReprogramacion_JefeAreaId",
                table: "SolicitudesReprogramacion",
                column: "JefeAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesReprogramacion_VacacionOriginalId",
                table: "SolicitudesReprogramacion",
                column: "VacacionOriginalId");

            migrationBuilder.CreateIndex(
                name: "IX_VacacionesProgramadas_CreatedBy",
                table: "VacacionesProgramadas",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_VacacionesProgramadas_EmpleadoId",
                table: "VacacionesProgramadas",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_VacacionesProgramadas_UpdatedBy",
                table: "VacacionesProgramadas",
                column: "UpdatedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracionVacaciones");

            migrationBuilder.DropTable(
                name: "ExcepcionesPorcentaje");

            migrationBuilder.DropTable(
                name: "SolicitudesReprogramacion");

            migrationBuilder.DropTable(
                name: "VacacionesProgramadas");
        }
    }
}
