using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tiempo_libre.Migrations
{
    /// <inheritdoc />
    public partial class AddTheVacacionesPorAntiguedadModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VacacionesPorAntiguedadId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VacacionesPorAntiguedad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AntiguedadEnAniosRangoInicial = table.Column<int>(type: "int", nullable: false),
                    AntiguedadEnAniosRangoFinal = table.Column<int>(type: "int", nullable: true),
                    TotalDiasDeVacaciones = table.Column<int>(type: "int", nullable: false),
                    DiasAsignadosPorContinental = table.Column<int>(type: "int", nullable: false),
                    DiasParaAsignarAutomaticamente = table.Column<int>(type: "int", nullable: false),
                    DiasPorEscogerPorEmpleado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacacionesPorAntiguedad", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_VacacionesPorAntiguedadId",
                table: "Users",
                column: "VacacionesPorAntiguedadId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_VacacionesPorAntiguedad",
                table: "Users",
                column: "VacacionesPorAntiguedadId",
                principalTable: "VacacionesPorAntiguedad",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_VacacionesPorAntiguedad",
                table: "Users");

            migrationBuilder.DropTable(
                name: "VacacionesPorAntiguedad");

            migrationBuilder.DropIndex(
                name: "IX_Users_VacacionesPorAntiguedadId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "VacacionesPorAntiguedadId",
                table: "Users");
        }
    }
}
