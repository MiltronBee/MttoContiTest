using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tiempo_libre.Migrations
{
    /// <inheritdoc />
    public partial class AddRolSemanalModelAndRemoveSindicalizadosModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RolesSemanales",
                columns: table => new
                {
                    RolSemanalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rol = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    IndiceSemana = table.Column<int>(type: "int", nullable: false),
                    IdRegla = table.Column<int>(type: "int", nullable: false),
                    ReglaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesSemanales", x => x.RolSemanalId);
                    table.ForeignKey(
                        name: "FK_RolSemanal_Regla",
                        column: x => x.IdRegla,
                        principalTable: "Reglas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolesSemanales_Reglas_ReglaId",
                        column: x => x.ReglaId,
                        principalTable: "Reglas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RolesSemanales_IdRegla",
                table: "RolesSemanales",
                column: "IdRegla");

            migrationBuilder.CreateIndex(
                name: "IX_RolesSemanales_ReglaId",
                table: "RolesSemanales",
                column: "ReglaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropTable(
                name: "RolesSemanales");

        }
    }
}
