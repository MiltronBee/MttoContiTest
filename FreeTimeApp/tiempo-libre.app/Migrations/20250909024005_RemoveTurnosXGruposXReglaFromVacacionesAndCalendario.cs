using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tiempo_libre.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTurnosXGruposXReglaFromVacacionesAndCalendario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalendarioPorEmpleados_TurnosXGruposXReglas_IdTurnoXGrupoXRegla",
                table: "CalendarioPorEmpleados");

            migrationBuilder.DropForeignKey(
                name: "FK_Vacaciones_TurnosXGruposXReglas_IdTurnoXGrupoXRegla",
                table: "Vacaciones");

            migrationBuilder.DropTable(
                name: "TurnosXGruposXReglas");

            migrationBuilder.RenameColumn(
                name: "IdTurnoXGrupoXRegla",
                table: "Vacaciones",
                newName: "IdTurnoXRolSemanalXRegla");

            migrationBuilder.RenameIndex(
                name: "IX_Vacaciones_IdTurnoXGrupoXRegla",
                table: "Vacaciones",
                newName: "IX_Vacaciones_IdTurnoXRolSemanalXRegla");

            migrationBuilder.RenameColumn(
                name: "IdTurnoXGrupoXRegla",
                table: "CalendarioPorEmpleados",
                newName: "IdTurnoXRolSemanalXRegla");

            migrationBuilder.RenameIndex(
                name: "IX_CalendarioPorEmpleados_IdTurnoXGrupoXRegla",
                table: "CalendarioPorEmpleados",
                newName: "IX_CalendarioPorEmpleados_IdTurnoXRolSemanalXRegla");

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarioPorEmpleados_TurnosXRolSemanalXRegla_IdTurnoXRolSemanalXRegla",
                table: "CalendarioPorEmpleados",
                column: "IdTurnoXRolSemanalXRegla",
                principalTable: "TurnosXRolSemanalXRegla",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vacaciones_TurnosXRolSemanalXRegla_IdTurnoXRolSemanalXRegla",
                table: "Vacaciones",
                column: "IdTurnoXRolSemanalXRegla",
                principalTable: "TurnosXRolSemanalXRegla",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalendarioPorEmpleados_TurnosXRolSemanalXRegla_IdTurnoXRolSemanalXRegla",
                table: "CalendarioPorEmpleados");

            migrationBuilder.DropForeignKey(
                name: "FK_Vacaciones_TurnosXRolSemanalXRegla_IdTurnoXRolSemanalXRegla",
                table: "Vacaciones");

            migrationBuilder.RenameColumn(
                name: "IdTurnoXRolSemanalXRegla",
                table: "Vacaciones",
                newName: "IdTurnoXGrupoXRegla");

            migrationBuilder.RenameIndex(
                name: "IX_Vacaciones_IdTurnoXRolSemanalXRegla",
                table: "Vacaciones",
                newName: "IX_Vacaciones_IdTurnoXGrupoXRegla");

            migrationBuilder.RenameColumn(
                name: "IdTurnoXRolSemanalXRegla",
                table: "CalendarioPorEmpleados",
                newName: "IdTurnoXGrupoXRegla");

            migrationBuilder.RenameIndex(
                name: "IX_CalendarioPorEmpleados_IdTurnoXRolSemanalXRegla",
                table: "CalendarioPorEmpleados",
                newName: "IX_CalendarioPorEmpleados_IdTurnoXGrupoXRegla");

            migrationBuilder.CreateTable(
                name: "TurnosXGruposXReglas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GrupoId = table.Column<int>(type: "int", nullable: false),
                    ReglaId = table.Column<int>(type: "int", nullable: false),
                    ActividadDelDia = table.Column<int>(type: "int", nullable: false),
                    DiaDeLaSemana = table.Column<int>(type: "int", nullable: false),
                    IdGrupo = table.Column<int>(type: "int", nullable: false),
                    IdRegla = table.Column<int>(type: "int", nullable: false),
                    IndicePorRegla = table.Column<int>(type: "int", nullable: false),
                    Turno = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TurnosXGruposXReglas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TurnosXGruposXReglas_Grupos_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "Grupos",
                        principalColumn: "GrupoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TurnosXGruposXReglas_Reglas_ReglaId",
                        column: x => x.ReglaId,
                        principalTable: "Reglas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TurnosXGruposXReglas_GrupoId",
                table: "TurnosXGruposXReglas",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_TurnosXGruposXReglas_ReglaId",
                table: "TurnosXGruposXReglas",
                column: "ReglaId");

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarioPorEmpleados_TurnosXGruposXReglas_IdTurnoXGrupoXRegla",
                table: "CalendarioPorEmpleados",
                column: "IdTurnoXGrupoXRegla",
                principalTable: "TurnosXGruposXReglas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vacaciones_TurnosXGruposXReglas_IdTurnoXGrupoXRegla",
                table: "Vacaciones",
                column: "IdTurnoXGrupoXRegla",
                principalTable: "TurnosXGruposXReglas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
