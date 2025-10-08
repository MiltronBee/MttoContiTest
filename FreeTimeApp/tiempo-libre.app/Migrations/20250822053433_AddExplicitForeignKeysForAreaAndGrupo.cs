using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tiempo_libre.Migrations
{
    /// <inheritdoc />
    public partial class AddExplicitForeignKeysForAreaAndGrupo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LiderId",
                table: "Grupos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LiderSuplenteId",
                table: "Grupos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JefeId",
                table: "Areas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JefeSuplenteId",
                table: "Areas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Grupos_LiderId",
                table: "Grupos",
                column: "LiderId");

            migrationBuilder.CreateIndex(
                name: "IX_Grupos_LiderSuplenteId",
                table: "Grupos",
                column: "LiderSuplenteId");

            migrationBuilder.CreateIndex(
                name: "IX_Areas_JefeId",
                table: "Areas",
                column: "JefeId");

            migrationBuilder.CreateIndex(
                name: "IX_Areas_JefeSuplenteId",
                table: "Areas",
                column: "JefeSuplenteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Area_Jefe",
                table: "Areas",
                column: "JefeId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Area_JefeSuplente",
                table: "Areas",
                column: "JefeSuplenteId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Grupo_Lider",
                table: "Grupos",
                column: "LiderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Grupo_LiderSuplente",
                table: "Grupos",
                column: "LiderSuplenteId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Area_Jefe",
                table: "Areas");

            migrationBuilder.DropForeignKey(
                name: "FK_Area_JefeSuplente",
                table: "Areas");

            migrationBuilder.DropForeignKey(
                name: "FK_Grupo_Lider",
                table: "Grupos");

            migrationBuilder.DropForeignKey(
                name: "FK_Grupo_LiderSuplente",
                table: "Grupos");

            migrationBuilder.DropIndex(
                name: "IX_Grupos_LiderId",
                table: "Grupos");

            migrationBuilder.DropIndex(
                name: "IX_Grupos_LiderSuplenteId",
                table: "Grupos");

            migrationBuilder.DropIndex(
                name: "IX_Areas_JefeId",
                table: "Areas");

            migrationBuilder.DropIndex(
                name: "IX_Areas_JefeSuplenteId",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "LiderId",
                table: "Grupos");

            migrationBuilder.DropColumn(
                name: "LiderSuplenteId",
                table: "Grupos");

            migrationBuilder.DropColumn(
                name: "JefeId",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "JefeSuplenteId",
                table: "Areas");
        }
    }
}
