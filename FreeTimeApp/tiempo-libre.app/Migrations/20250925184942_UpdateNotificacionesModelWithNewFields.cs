using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tiempo_libre.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNotificacionesModelWithNewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TipoDeNotificacion",
                table: "Notificaciones",
                type: "int",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "IdUsuarioEmisor",
                table: "Notificaciones",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "AreaId",
                table: "Notificaciones",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Estatus",
                table: "Notificaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAccion",
                table: "Notificaciones",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "GrupoId",
                table: "Notificaciones",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetadatosJson",
                table: "Notificaciones",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NombreEmisor",
                table: "Notificaciones",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TipoMovimiento",
                table: "Notificaciones",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_AreaId",
                table: "Notificaciones",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_GrupoId",
                table: "Notificaciones",
                column: "GrupoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notificaciones_Areas_AreaId",
                table: "Notificaciones",
                column: "AreaId",
                principalTable: "Areas",
                principalColumn: "AreaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notificaciones_Grupos_GrupoId",
                table: "Notificaciones",
                column: "GrupoId",
                principalTable: "Grupos",
                principalColumn: "GrupoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notificaciones_Areas_AreaId",
                table: "Notificaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Notificaciones_Grupos_GrupoId",
                table: "Notificaciones");

            migrationBuilder.DropIndex(
                name: "IX_Notificaciones_AreaId",
                table: "Notificaciones");

            migrationBuilder.DropIndex(
                name: "IX_Notificaciones_GrupoId",
                table: "Notificaciones");

            migrationBuilder.DropColumn(
                name: "AreaId",
                table: "Notificaciones");

            migrationBuilder.DropColumn(
                name: "Estatus",
                table: "Notificaciones");

            migrationBuilder.DropColumn(
                name: "FechaAccion",
                table: "Notificaciones");

            migrationBuilder.DropColumn(
                name: "GrupoId",
                table: "Notificaciones");

            migrationBuilder.DropColumn(
                name: "MetadatosJson",
                table: "Notificaciones");

            migrationBuilder.DropColumn(
                name: "NombreEmisor",
                table: "Notificaciones");

            migrationBuilder.DropColumn(
                name: "TipoMovimiento",
                table: "Notificaciones");

            migrationBuilder.AlterColumn<string>(
                name: "TipoDeNotificacion",
                table: "Notificaciones",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "IdUsuarioEmisor",
                table: "Notificaciones",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
