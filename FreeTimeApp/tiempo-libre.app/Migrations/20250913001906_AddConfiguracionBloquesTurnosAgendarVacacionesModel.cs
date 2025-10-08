using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tiempo_libre.Migrations
{
    /// <inheritdoc />
    public partial class AddConfiguracionBloquesTurnosAgendarVacacionesModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfiguracionBloquesTurnosAgendarVacaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdArea = table.Column<int>(type: "int", nullable: false),
                    IdGrupo = table.Column<int>(type: "int", nullable: false),
                    MaximoDeEmpleadosQuePuedenAgendar = table.Column<int>(type: "int", nullable: false),
                    TiempoEnHorasDelTurno = table.Column<int>(type: "int", nullable: false),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionBloquesTurnosAgendarVacaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfiguracionBloquesTurnosAgendarVacaciones_Areas_IdArea",
                        column: x => x.IdArea,
                        principalTable: "Areas",
                        principalColumn: "AreaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConfiguracionBloquesTurnosAgendarVacaciones_Grupos_IdGrupo",
                        column: x => x.IdGrupo,
                        principalTable: "Grupos",
                        principalColumn: "GrupoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionBloquesTurnosAgendarVacaciones_IdArea",
                table: "ConfiguracionBloquesTurnosAgendarVacaciones",
                column: "IdArea");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionBloquesTurnosAgendarVacaciones_IdGrupo",
                table: "ConfiguracionBloquesTurnosAgendarVacaciones",
                column: "IdGrupo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracionBloquesTurnosAgendarVacaciones");
        }
    }
}
