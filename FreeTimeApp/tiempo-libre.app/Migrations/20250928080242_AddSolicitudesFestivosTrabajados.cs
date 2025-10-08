using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tiempo_libre.Migrations
{
    /// <inheritdoc />
    public partial class AddSolicitudesFestivosTrabajados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SolicitudesFestivosTrabajados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    FestivoTrabajadoOriginalId = table.Column<int>(type: "int", nullable: false),
                    FechaNuevaSolicitada = table.Column<DateOnly>(type: "date", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EstadoSolicitud = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Pendiente"),
                    PorcentajeCalculado = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    SolicitadoPorId = table.Column<int>(type: "int", nullable: false),
                    JefeAreaId = table.Column<int>(type: "int", nullable: true),
                    FechaRespuesta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AprobadoPorId = table.Column<int>(type: "int", nullable: true),
                    MotivoRechazo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FestivoOriginal = table.Column<DateOnly>(type: "date", nullable: false),
                    Nomina = table.Column<int>(type: "int", nullable: false),
                    VacacionCreadaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesFestivosTrabajados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudesFestivosTrabajados_DiasFestivosTrabajadosOriginalTable_FestivoTrabajadoOriginalId",
                        column: x => x.FestivoTrabajadoOriginalId,
                        principalTable: "DiasFestivosTrabajadosOriginalTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudesFestivosTrabajados_Users_AprobadoPorId",
                        column: x => x.AprobadoPorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudesFestivosTrabajados_Users_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudesFestivosTrabajados_Users_JefeAreaId",
                        column: x => x.JefeAreaId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudesFestivosTrabajados_Users_SolicitadoPorId",
                        column: x => x.SolicitadoPorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudesFestivosTrabajados_VacacionesProgramadas_VacacionCreadaId",
                        column: x => x.VacacionCreadaId,
                        principalTable: "VacacionesProgramadas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesFestivosTrabajados_AprobadoPorId",
                table: "SolicitudesFestivosTrabajados",
                column: "AprobadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesFestivosTrabajados_EmpleadoId",
                table: "SolicitudesFestivosTrabajados",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesFestivosTrabajados_FestivoTrabajadoOriginalId",
                table: "SolicitudesFestivosTrabajados",
                column: "FestivoTrabajadoOriginalId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesFestivosTrabajados_JefeAreaId",
                table: "SolicitudesFestivosTrabajados",
                column: "JefeAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesFestivosTrabajados_SolicitadoPorId",
                table: "SolicitudesFestivosTrabajados",
                column: "SolicitadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesFestivosTrabajados_VacacionCreadaId",
                table: "SolicitudesFestivosTrabajados",
                column: "VacacionCreadaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolicitudesFestivosTrabajados");
        }
    }
}
