using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tiempo_libre.Migrations
{
    /// <inheritdoc />
    public partial class SyncBloquesReservacionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Migración vacía - las tablas BloquesReservacion, AsignacionesBloque y CambiosBloque ya existen
            // Esta migración solo sincroniza el modelo con el estado actual de la base de datos
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AsignacionesBloque");

            migrationBuilder.DropTable(
                name: "CambiosBloque");

            migrationBuilder.DropTable(
                name: "BloquesReservacion");
        }
    }
}
