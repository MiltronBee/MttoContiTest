using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tiempo_libre.Migrations
{
    /// <inheritdoc />
    public partial class AddDiasFestivosTrabajadosModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiasFestivosTrabajados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuarioEmpleadoSindicalizado = table.Column<int>(type: "int", nullable: false),
                    NominaEmpleadoSindical = table.Column<int>(type: "int", nullable: false),
                    IdArea = table.Column<int>(type: "int", nullable: false),
                    IdGrupo = table.Column<int>(type: "int", nullable: false),
                    FechaDiaFestivoTrabajado = table.Column<DateOnly>(type: "date", nullable: false),
                    Compensado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiasFestivosTrabajados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiasFestivosTrabajados_Areas_IdArea",
                        column: x => x.IdArea,
                        principalTable: "Areas",
                        principalColumn: "AreaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiasFestivosTrabajados_Grupos_IdGrupo",
                        column: x => x.IdGrupo,
                        principalTable: "Grupos",
                        principalColumn: "GrupoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiasFestivosTrabajados_Users_IdUsuarioEmpleadoSindicalizado",
                        column: x => x.IdUsuarioEmpleadoSindicalizado,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiasFestivosTrabajados_IdArea",
                table: "DiasFestivosTrabajados",
                column: "IdArea");

            migrationBuilder.CreateIndex(
                name: "IX_DiasFestivosTrabajados_IdGrupo",
                table: "DiasFestivosTrabajados",
                column: "IdGrupo");

            migrationBuilder.CreateIndex(
                name: "IX_DiasFestivosTrabajados_IdUsuarioEmpleadoSindicalizado",
                table: "DiasFestivosTrabajados",
                column: "IdUsuarioEmpleadoSindicalizado");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiasFestivosTrabajados");
        }
    }
}
