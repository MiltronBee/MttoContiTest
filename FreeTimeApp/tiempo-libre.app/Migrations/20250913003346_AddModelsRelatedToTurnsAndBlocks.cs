using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tiempo_libre.Migrations
{
    /// <inheritdoc />
    public partial class AddModelsRelatedToTurnsAndBlocks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmpleadosXBloquesDeTurnos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdBloqueDeTurnos = table.Column<int>(type: "int", nullable: false),
                    IdEmpleadoSindicalAgendara = table.Column<int>(type: "int", nullable: false),
                    NominaEmpleadoSindicalAgendara = table.Column<int>(type: "int", nullable: false),
                    AntiguedadEnAniosAlMomentoDeAgendar = table.Column<int>(type: "int", nullable: false),
                    FechaYHoraAgendacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AgendooVacaciones = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AgendoTodo = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FechaYHoraReservoVacaciones = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpleadosXBloquesDeTurnos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpleadosXBloquesDeTurnos_BloqueDeTurnosAgendarVacaciones_IdBloqueDeTurnos",
                        column: x => x.IdBloqueDeTurnos,
                        principalTable: "BloqueDeTurnosAgendarVacaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmpleadosXBloquesDeTurnos_Users_IdEmpleadoSindicalAgendara",
                        column: x => x.IdEmpleadoSindicalAgendara,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReservacionesDeVacacionesPorEmpleado",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdBloqueDeTurnos = table.Column<int>(type: "int", nullable: false),
                    IdEmpleadoSindicalizado = table.Column<int>(type: "int", nullable: false),
                    NominaEmpleadoSindical = table.Column<int>(type: "int", nullable: false),
                    IdCalPorEmpDeVacaciones = table.Column<int>(type: "int", nullable: false),
                    FechaDiaDeVacacion = table.Column<DateOnly>(type: "date", nullable: false),
                    Cretated_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservacionesDeVacacionesPorEmpleado", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservacionesDeVacacionesPorEmpleado_BloqueDeTurnosAgendarVacaciones_IdBloqueDeTurnos",
                        column: x => x.IdBloqueDeTurnos,
                        principalTable: "BloqueDeTurnosAgendarVacaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservacionesDeVacacionesPorEmpleado_CalendarioPorEmpleados_IdCalPorEmpDeVacaciones",
                        column: x => x.IdCalPorEmpDeVacaciones,
                        principalTable: "CalendarioPorEmpleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservacionesDeVacacionesPorEmpleado_Users_IdEmpleadoSindicalizado",
                        column: x => x.IdEmpleadoSindicalizado,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadosXBloquesDeTurnos_IdBloqueDeTurnos",
                table: "EmpleadosXBloquesDeTurnos",
                column: "IdBloqueDeTurnos");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadosXBloquesDeTurnos_IdEmpleadoSindicalAgendara",
                table: "EmpleadosXBloquesDeTurnos",
                column: "IdEmpleadoSindicalAgendara");

            migrationBuilder.CreateIndex(
                name: "IX_ReservacionesDeVacacionesPorEmpleado_IdBloqueDeTurnos",
                table: "ReservacionesDeVacacionesPorEmpleado",
                column: "IdBloqueDeTurnos");

            migrationBuilder.CreateIndex(
                name: "IX_ReservacionesDeVacacionesPorEmpleado_IdCalPorEmpDeVacaciones",
                table: "ReservacionesDeVacacionesPorEmpleado",
                column: "IdCalPorEmpDeVacaciones");

            migrationBuilder.CreateIndex(
                name: "IX_ReservacionesDeVacacionesPorEmpleado_IdEmpleadoSindicalizado",
                table: "ReservacionesDeVacacionesPorEmpleado",
                column: "IdEmpleadoSindicalizado");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmpleadosXBloquesDeTurnos");

            migrationBuilder.DropTable(
                name: "ReservacionesDeVacacionesPorEmpleado");
        }
    }
}
