using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tiempo_libre.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAuditUserFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConfiguracionVacaciones_Users_UpdatedBy",
                table: "ConfiguracionVacaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_ExcepcionesPorcentaje_Users_CreatedBy",
                table: "ExcepcionesPorcentaje");

            migrationBuilder.DropForeignKey(
                name: "FK_ExcepcionesPorcentaje_Users_UpdatedBy",
                table: "ExcepcionesPorcentaje");

            migrationBuilder.DropIndex(
                name: "IX_ExcepcionesPorcentaje_CreatedBy",
                table: "ExcepcionesPorcentaje");

            migrationBuilder.DropIndex(
                name: "IX_ExcepcionesPorcentaje_UpdatedBy",
                table: "ExcepcionesPorcentaje");

            migrationBuilder.DropIndex(
                name: "IX_ConfiguracionVacaciones_UpdatedBy",
                table: "ConfiguracionVacaciones");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ExcepcionesPorcentaje");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ExcepcionesPorcentaje");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ConfiguracionVacaciones");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "ExcepcionesPorcentaje",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "ExcepcionesPorcentaje",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "ConfiguracionVacaciones",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExcepcionesPorcentaje_CreatedBy",
                table: "ExcepcionesPorcentaje",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ExcepcionesPorcentaje_UpdatedBy",
                table: "ExcepcionesPorcentaje",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionVacaciones_UpdatedBy",
                table: "ConfiguracionVacaciones",
                column: "UpdatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_ConfiguracionVacaciones_Users_UpdatedBy",
                table: "ConfiguracionVacaciones",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExcepcionesPorcentaje_Users_CreatedBy",
                table: "ExcepcionesPorcentaje",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExcepcionesPorcentaje_Users_UpdatedBy",
                table: "ExcepcionesPorcentaje",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
