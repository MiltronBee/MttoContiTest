using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tiempo_libre.Migrations
{
    /// <inheritdoc />
    public partial class AddPrecisionForManningFieldOnAreaModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "FechaInicioGeneracionDeCalendario",
                table: "CalendarioEmpleados",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<int>(
                name: "IdRolSemanalIniciaGeneracionDeCalendario",
                table: "CalendarioEmpleados",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "Manning",
                table: "Areas",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarioEmpleados_IdRolSemanalIniciaGeneracionDeCalendario",
                table: "CalendarioEmpleados",
                column: "IdRolSemanalIniciaGeneracionDeCalendario");

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarioEmpleados_RolesSemanales_IdRolSemanalIniciaGeneracionDeCalendario",
                table: "CalendarioEmpleados",
                column: "IdRolSemanalIniciaGeneracionDeCalendario",
                principalTable: "RolesSemanales",
                principalColumn: "RolSemanalId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalendarioEmpleados_RolesSemanales_IdRolSemanalIniciaGeneracionDeCalendario",
                table: "CalendarioEmpleados");

            migrationBuilder.DropIndex(
                name: "IX_CalendarioEmpleados_IdRolSemanalIniciaGeneracionDeCalendario",
                table: "CalendarioEmpleados");

            migrationBuilder.DropColumn(
                name: "FechaInicioGeneracionDeCalendario",
                table: "CalendarioEmpleados");

            migrationBuilder.DropColumn(
                name: "IdRolSemanalIniciaGeneracionDeCalendario",
                table: "CalendarioEmpleados");

            migrationBuilder.AlterColumn<decimal>(
                name: "Manning",
                table: "Areas",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");
        }
    }
}
