using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tiempo_libre.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTurnosXGrupoXReglaModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TurnosXGruposXReglas_Grupos_IdGrupo",
                table: "TurnosXGruposXReglas");

            migrationBuilder.DropForeignKey(
                name: "FK_TurnosXGruposXReglas_Reglas_IdRegla",
                table: "TurnosXGruposXReglas");

            migrationBuilder.DropIndex(
                name: "IX_TurnosXGruposXReglas_IdGrupo",
                table: "TurnosXGruposXReglas");

            migrationBuilder.DropIndex(
                name: "IX_TurnosXGruposXReglas_IdRegla",
                table: "TurnosXGruposXReglas");

            migrationBuilder.AddColumn<int>(
                name: "GrupoId",
                table: "TurnosXGruposXReglas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReglaId",
                table: "TurnosXGruposXReglas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TurnosXGruposXReglas_GrupoId",
                table: "TurnosXGruposXReglas",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_TurnosXGruposXReglas_ReglaId",
                table: "TurnosXGruposXReglas",
                column: "ReglaId");

            migrationBuilder.AddForeignKey(
                name: "FK_TurnosXGruposXReglas_Grupos_GrupoId",
                table: "TurnosXGruposXReglas",
                column: "GrupoId",
                principalTable: "Grupos",
                principalColumn: "GrupoId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TurnosXGruposXReglas_Reglas_ReglaId",
                table: "TurnosXGruposXReglas",
                column: "ReglaId",
                principalTable: "Reglas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TurnosXGruposXReglas_Grupos_GrupoId",
                table: "TurnosXGruposXReglas");

            migrationBuilder.DropForeignKey(
                name: "FK_TurnosXGruposXReglas_Reglas_ReglaId",
                table: "TurnosXGruposXReglas");

            migrationBuilder.DropIndex(
                name: "IX_TurnosXGruposXReglas_GrupoId",
                table: "TurnosXGruposXReglas");

            migrationBuilder.DropIndex(
                name: "IX_TurnosXGruposXReglas_ReglaId",
                table: "TurnosXGruposXReglas");

            migrationBuilder.DropColumn(
                name: "GrupoId",
                table: "TurnosXGruposXReglas");

            migrationBuilder.DropColumn(
                name: "ReglaId",
                table: "TurnosXGruposXReglas");

            migrationBuilder.CreateIndex(
                name: "IX_TurnosXGruposXReglas_IdGrupo",
                table: "TurnosXGruposXReglas",
                column: "IdGrupo");

            migrationBuilder.CreateIndex(
                name: "IX_TurnosXGruposXReglas_IdRegla",
                table: "TurnosXGruposXReglas",
                column: "IdRegla");

            migrationBuilder.AddForeignKey(
                name: "FK_TurnosXGruposXReglas_Grupos_IdGrupo",
                table: "TurnosXGruposXReglas",
                column: "IdGrupo",
                principalTable: "Grupos",
                principalColumn: "GrupoId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TurnosXGruposXReglas_Reglas_IdRegla",
                table: "TurnosXGruposXReglas",
                column: "IdRegla",
                principalTable: "Reglas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
