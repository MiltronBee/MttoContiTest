using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tiempo_libre.Migrations
{
    /// <inheritdoc />
    public partial class NewFieldForReglaModelForAppId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReglaEnumId",
                table: "Reglas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Reglas_ReglaEnumId",
                table: "Reglas",
                column: "ReglaEnumId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reglas_ReglaEnumId",
                table: "Reglas");

            migrationBuilder.DropColumn(
                name: "ReglaEnumId",
                table: "Reglas");
        }
    }
}
