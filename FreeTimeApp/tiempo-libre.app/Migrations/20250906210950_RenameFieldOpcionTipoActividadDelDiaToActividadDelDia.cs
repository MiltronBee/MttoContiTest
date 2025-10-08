using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tiempo_libre.Migrations
{
    /// <inheritdoc />
    public partial class RenameFieldOpcionTipoActividadDelDiaToActividadDelDia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OpcionTipoActividadDelDia",
                table: "TurnosXGruposXReglas",
                newName: "ActividadDelDia");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ActividadDelDia",
                table: "TurnosXGruposXReglas",
                newName: "OpcionTipoActividadDelDia");
        }
    }
}
