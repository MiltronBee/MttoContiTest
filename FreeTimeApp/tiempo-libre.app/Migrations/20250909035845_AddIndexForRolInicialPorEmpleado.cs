using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tiempo_libre.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexForRolInicialPorEmpleado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RolesInicialesPorEmpleado_Nomina",
                table: "RolesInicialesPorEmpleado",
                column: "Nomina");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RolesInicialesPorEmpleado_Nomina",
                table: "RolesInicialesPorEmpleado");
        }
    }
}
