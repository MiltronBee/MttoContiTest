using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tiempo_libre.Migrations
{
    /// <inheritdoc />
    public partial class AddExtraDataForGrupo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DuracionDeturno",
                table: "Grupos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "IdentificadorSAP",
                table: "Grupos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PersonasPorTurno",
                table: "Grupos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DuracionDeturno",
                table: "Grupos");

            migrationBuilder.DropColumn(
                name: "IdentificadorSAP",
                table: "Grupos");

            migrationBuilder.DropColumn(
                name: "PersonasPorTurno",
                table: "Grupos");
        }
    }
}
