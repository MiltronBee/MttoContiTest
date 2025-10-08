using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tiempo_libre.Migrations
{
    /// <inheritdoc />
    public partial class TurnoXRolSemanalXReglaModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolesSemanales_Reglas_ReglaId",
                table: "RolesSemanales");

            migrationBuilder.DropIndex(
                name: "IX_RolesSemanales_ReglaId",
                table: "RolesSemanales");

            migrationBuilder.DropColumn(
                name: "ReglaId",
                table: "RolesSemanales");

            migrationBuilder.CreateTable(
                name: "TurnosXRolSemanalXRegla",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IndicePorRegla = table.Column<int>(type: "int", nullable: false),
                    IdRegla = table.Column<int>(type: "int", nullable: false),
                    IdRolSemanal = table.Column<int>(type: "int", nullable: false),
                    DiaDeLaSemana = table.Column<int>(type: "int", nullable: false),
                    ActividadDelDia = table.Column<int>(type: "int", nullable: false),
                    Turno = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TurnosXRolSemanalXRegla", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TurnosXRolSemanalXRegla_Reglas_IdRegla",
                        column: x => x.IdRegla,
                        principalTable: "Reglas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TurnosXRolSemanalXRegla_RolesSemanales_IdRolSemanal",
                        column: x => x.IdRolSemanal,
                        principalTable: "RolesSemanales",
                        principalColumn: "RolSemanalId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TurnosXRolSemanalXRegla_IdRegla",
                table: "TurnosXRolSemanalXRegla",
                column: "IdRegla");

            migrationBuilder.CreateIndex(
                name: "IX_TurnosXRolSemanalXRegla_IdRolSemanal",
                table: "TurnosXRolSemanalXRegla",
                column: "IdRolSemanal");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TurnosXRolSemanalXRegla");

            migrationBuilder.AddColumn<int>(
                name: "ReglaId",
                table: "RolesSemanales",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolesSemanales_ReglaId",
                table: "RolesSemanales",
                column: "ReglaId");

            migrationBuilder.AddForeignKey(
                name: "FK_RolesSemanales_Reglas_ReglaId",
                table: "RolesSemanales",
                column: "ReglaId",
                principalTable: "Reglas",
                principalColumn: "Id");
        }
    }
}
