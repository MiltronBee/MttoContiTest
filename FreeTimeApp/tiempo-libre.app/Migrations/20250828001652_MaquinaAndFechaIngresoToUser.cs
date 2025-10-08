using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tiempo_libre.Migrations
{
    /// <inheritdoc />
    public partial class MaquinaAndFechaIngresoToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaIngreso",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Maquina",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaIngreso",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Maquina",
                table: "Users");
        }
    }
}
