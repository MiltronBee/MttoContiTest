using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tiempo_libre.Migrations
{
    /// <inheritdoc />
    public partial class ChangesForCalendarioPorEmpleadoModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EsDiaDeDescanso",
                table: "CalendarioPorEmpleados",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EsDiaDePermiso",
                table: "CalendarioPorEmpleados",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EsDiaDeVacaciones",
                table: "CalendarioPorEmpleados",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EsDiaFestivo",
                table: "CalendarioPorEmpleados",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EsDiaInhabil",
                table: "CalendarioPorEmpleados",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EsDiaIntercambiado",
                table: "CalendarioPorEmpleados",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EsDiaLaboral",
                table: "CalendarioPorEmpleados",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "EsDiaReprogramado",
                table: "CalendarioPorEmpleados",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "IdArea",
                table: "CalendarioPorEmpleados",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdIntercambioDiaFestivoPorDescanso",
                table: "CalendarioPorEmpleados",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdProgramacionAnual",
                table: "CalendarioPorEmpleados",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdReprogramacionDeVacaciones",
                table: "CalendarioPorEmpleados",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdRolSemanal",
                table: "CalendarioPorEmpleados",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CalendarioPorEmpleados_IdIntercambioDiaFestivoPorDescanso",
                table: "CalendarioPorEmpleados",
                column: "IdIntercambioDiaFestivoPorDescanso");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarioPorEmpleados_IdProgramacionAnual",
                table: "CalendarioPorEmpleados",
                column: "IdProgramacionAnual");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarioPorEmpleados_IdReprogramacionDeVacaciones",
                table: "CalendarioPorEmpleados",
                column: "IdReprogramacionDeVacaciones");

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarioPorEmpleados_IntercambiosDiaFestivoPorDescanso_IdIntercambioDiaFestivoPorDescanso",
                table: "CalendarioPorEmpleados",
                column: "IdIntercambioDiaFestivoPorDescanso",
                principalTable: "IntercambiosDiaFestivoPorDescanso",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarioPorEmpleados_ProgramacionesAnuales_IdProgramacionAnual",
                table: "CalendarioPorEmpleados",
                column: "IdProgramacionAnual",
                principalTable: "ProgramacionesAnuales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarioPorEmpleados_ReprogramacionesDeVacaciones_IdReprogramacionDeVacaciones",
                table: "CalendarioPorEmpleados",
                column: "IdReprogramacionDeVacaciones",
                principalTable: "ReprogramacionesDeVacaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalendarioPorEmpleados_IntercambiosDiaFestivoPorDescanso_IdIntercambioDiaFestivoPorDescanso",
                table: "CalendarioPorEmpleados");

            migrationBuilder.DropForeignKey(
                name: "FK_CalendarioPorEmpleados_ProgramacionesAnuales_IdProgramacionAnual",
                table: "CalendarioPorEmpleados");

            migrationBuilder.DropForeignKey(
                name: "FK_CalendarioPorEmpleados_ReprogramacionesDeVacaciones_IdReprogramacionDeVacaciones",
                table: "CalendarioPorEmpleados");

            migrationBuilder.DropIndex(
                name: "IX_CalendarioPorEmpleados_IdIntercambioDiaFestivoPorDescanso",
                table: "CalendarioPorEmpleados");

            migrationBuilder.DropIndex(
                name: "IX_CalendarioPorEmpleados_IdProgramacionAnual",
                table: "CalendarioPorEmpleados");

            migrationBuilder.DropIndex(
                name: "IX_CalendarioPorEmpleados_IdReprogramacionDeVacaciones",
                table: "CalendarioPorEmpleados");

            migrationBuilder.DropColumn(
                name: "EsDiaDeDescanso",
                table: "CalendarioPorEmpleados");

            migrationBuilder.DropColumn(
                name: "EsDiaDePermiso",
                table: "CalendarioPorEmpleados");

            migrationBuilder.DropColumn(
                name: "EsDiaDeVacaciones",
                table: "CalendarioPorEmpleados");

            migrationBuilder.DropColumn(
                name: "EsDiaFestivo",
                table: "CalendarioPorEmpleados");

            migrationBuilder.DropColumn(
                name: "EsDiaInhabil",
                table: "CalendarioPorEmpleados");

            migrationBuilder.DropColumn(
                name: "EsDiaIntercambiado",
                table: "CalendarioPorEmpleados");

            migrationBuilder.DropColumn(
                name: "EsDiaLaboral",
                table: "CalendarioPorEmpleados");

            migrationBuilder.DropColumn(
                name: "EsDiaReprogramado",
                table: "CalendarioPorEmpleados");

            migrationBuilder.DropColumn(
                name: "IdArea",
                table: "CalendarioPorEmpleados");

            migrationBuilder.DropColumn(
                name: "IdIntercambioDiaFestivoPorDescanso",
                table: "CalendarioPorEmpleados");

            migrationBuilder.DropColumn(
                name: "IdProgramacionAnual",
                table: "CalendarioPorEmpleados");

            migrationBuilder.DropColumn(
                name: "IdReprogramacionDeVacaciones",
                table: "CalendarioPorEmpleados");

            migrationBuilder.DropColumn(
                name: "IdRolSemanal",
                table: "CalendarioPorEmpleados");
        }
    }
}
