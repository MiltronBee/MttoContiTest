using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tiempo_libre.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingFilesToStoreDataForCalendarGenerationProcess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActividadDelDia",
                table: "Vacaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AntiguedadEnAnios",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AntiguedadEnDias",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DiasDeVacacionesAsignados",
                table: "Users",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActividadDelDia",
                table: "Vacaciones");

            migrationBuilder.DropColumn(
                name: "AntiguedadEnAnios",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AntiguedadEnDias",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DiasDeVacacionesAsignados",
                table: "Users");
        }
    }
}
