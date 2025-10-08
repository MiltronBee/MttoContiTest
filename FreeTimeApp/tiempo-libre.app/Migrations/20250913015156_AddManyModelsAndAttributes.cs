using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tiempo_libre.Migrations
{
    /// <inheritdoc />
    public partial class AddManyModelsAndAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UltimoInicioSesion",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Manning",
                table: "Areas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AreaIngenieros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    IngenieroId = table.Column<int>(type: "int", nullable: false),
                    SuplenteId = table.Column<int>(type: "int", nullable: true),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaDesasignacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AreaIngenieros", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AreaIngeniero_Suplente",
                        column: x => x.SuplenteId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AreaIngenieros_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "AreaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AreaIngenieros_Users_IngenieroId",
                        column: x => x.IngenieroId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AreaIngeniero_Unique",
                table: "AreaIngenieros",
                columns: new[] { "AreaId", "IngenieroId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AreaIngenieros_IngenieroId",
                table: "AreaIngenieros",
                column: "IngenieroId");

            migrationBuilder.CreateIndex(
                name: "IX_AreaIngenieros_SuplenteId",
                table: "AreaIngenieros",
                column: "SuplenteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AreaIngenieros");

            migrationBuilder.DropColumn(
                name: "UltimoInicioSesion",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Manning",
                table: "Areas");
        }
    }
}
