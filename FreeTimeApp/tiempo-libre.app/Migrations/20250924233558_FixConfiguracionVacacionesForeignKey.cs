using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tiempo_libre.Migrations
{
    /// <inheritdoc />
    public partial class FixConfiguracionVacacionesForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConfiguracionVacaciones_Users_UpdatedBy",
                table: "ConfiguracionVacaciones");

            migrationBuilder.AddForeignKey(
                name: "FK_ConfiguracionVacaciones_Users_UpdatedBy",
                table: "ConfiguracionVacaciones",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConfiguracionVacaciones_Users_UpdatedBy",
                table: "ConfiguracionVacaciones");

            migrationBuilder.AddForeignKey(
                name: "FK_ConfiguracionVacaciones_Users_UpdatedBy",
                table: "ConfiguracionVacaciones",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
