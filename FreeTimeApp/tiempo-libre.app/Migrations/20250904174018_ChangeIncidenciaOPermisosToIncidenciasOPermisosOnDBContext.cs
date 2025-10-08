using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tiempo_libre.Migrations
{
    /// <inheritdoc />
    public partial class ChangeIncidenciaOPermisosToIncidenciasOPermisosOnDBContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalendarioPorEmpleados_IncidenciaOPermisos_IdIncidenciaOPermiso",
                table: "CalendarioPorEmpleados");

            migrationBuilder.DropForeignKey(
                name: "FK_IncidenciaOPermisos_Grupos_IdGrupo",
                table: "IncidenciaOPermisos");

            migrationBuilder.DropForeignKey(
                name: "FK_IncidenciaOPermisos_Reglas_IdRegla",
                table: "IncidenciaOPermisos");

            migrationBuilder.DropForeignKey(
                name: "FK_IncidenciaOPermisos_Users_IdUsuarioAutoiza",
                table: "IncidenciaOPermisos");

            migrationBuilder.DropForeignKey(
                name: "FK_IncidenciaOPermisos_Users_IdUsuarioEmpleado",
                table: "IncidenciaOPermisos");

            migrationBuilder.DropForeignKey(
                name: "FK_IncidenciaOPermisos_Users_IdUsuarioSindicato",
                table: "IncidenciaOPermisos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IncidenciaOPermisos",
                table: "IncidenciaOPermisos");

            migrationBuilder.RenameTable(
                name: "IncidenciaOPermisos",
                newName: "IncidenciasOPermisos");

            migrationBuilder.RenameIndex(
                name: "IX_IncidenciaOPermisos_NominaEmpleado",
                table: "IncidenciasOPermisos",
                newName: "IX_IncidenciasOPermisos_NominaEmpleado");

            migrationBuilder.RenameIndex(
                name: "IX_IncidenciaOPermisos_MesFechaInicial",
                table: "IncidenciasOPermisos",
                newName: "IX_IncidenciasOPermisos_MesFechaInicial");

            migrationBuilder.RenameIndex(
                name: "IX_IncidenciaOPermisos_IdUsuarioSindicato",
                table: "IncidenciasOPermisos",
                newName: "IX_IncidenciasOPermisos_IdUsuarioSindicato");

            migrationBuilder.RenameIndex(
                name: "IX_IncidenciaOPermisos_IdUsuarioEmpleado",
                table: "IncidenciasOPermisos",
                newName: "IX_IncidenciasOPermisos_IdUsuarioEmpleado");

            migrationBuilder.RenameIndex(
                name: "IX_IncidenciaOPermisos_IdUsuarioAutoiza",
                table: "IncidenciasOPermisos",
                newName: "IX_IncidenciasOPermisos_IdUsuarioAutoiza");

            migrationBuilder.RenameIndex(
                name: "IX_IncidenciaOPermisos_IdRegla",
                table: "IncidenciasOPermisos",
                newName: "IX_IncidenciasOPermisos_IdRegla");

            migrationBuilder.RenameIndex(
                name: "IX_IncidenciaOPermisos_IdGrupo",
                table: "IncidenciasOPermisos",
                newName: "IX_IncidenciasOPermisos_IdGrupo");

            migrationBuilder.RenameIndex(
                name: "IX_IncidenciaOPermisos_Fecha",
                table: "IncidenciasOPermisos",
                newName: "IX_IncidenciasOPermisos_Fecha");

            migrationBuilder.RenameIndex(
                name: "IX_IncidenciaOPermisos_DiaFechaInicial",
                table: "IncidenciasOPermisos",
                newName: "IX_IncidenciasOPermisos_DiaFechaInicial");

            migrationBuilder.RenameIndex(
                name: "IX_IncidenciaOPermisos_AnioFechaInicial",
                table: "IncidenciasOPermisos",
                newName: "IX_IncidenciasOPermisos_AnioFechaInicial");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IncidenciasOPermisos",
                table: "IncidenciasOPermisos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarioPorEmpleados_IncidenciasOPermisos_IdIncidenciaOPermiso",
                table: "CalendarioPorEmpleados",
                column: "IdIncidenciaOPermiso",
                principalTable: "IncidenciasOPermisos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IncidenciasOPermisos_Grupos_IdGrupo",
                table: "IncidenciasOPermisos",
                column: "IdGrupo",
                principalTable: "Grupos",
                principalColumn: "GrupoId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IncidenciasOPermisos_Reglas_IdRegla",
                table: "IncidenciasOPermisos",
                column: "IdRegla",
                principalTable: "Reglas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IncidenciasOPermisos_Users_IdUsuarioAutoiza",
                table: "IncidenciasOPermisos",
                column: "IdUsuarioAutoiza",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IncidenciasOPermisos_Users_IdUsuarioEmpleado",
                table: "IncidenciasOPermisos",
                column: "IdUsuarioEmpleado",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IncidenciasOPermisos_Users_IdUsuarioSindicato",
                table: "IncidenciasOPermisos",
                column: "IdUsuarioSindicato",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalendarioPorEmpleados_IncidenciasOPermisos_IdIncidenciaOPermiso",
                table: "CalendarioPorEmpleados");

            migrationBuilder.DropForeignKey(
                name: "FK_IncidenciasOPermisos_Grupos_IdGrupo",
                table: "IncidenciasOPermisos");

            migrationBuilder.DropForeignKey(
                name: "FK_IncidenciasOPermisos_Reglas_IdRegla",
                table: "IncidenciasOPermisos");

            migrationBuilder.DropForeignKey(
                name: "FK_IncidenciasOPermisos_Users_IdUsuarioAutoiza",
                table: "IncidenciasOPermisos");

            migrationBuilder.DropForeignKey(
                name: "FK_IncidenciasOPermisos_Users_IdUsuarioEmpleado",
                table: "IncidenciasOPermisos");

            migrationBuilder.DropForeignKey(
                name: "FK_IncidenciasOPermisos_Users_IdUsuarioSindicato",
                table: "IncidenciasOPermisos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IncidenciasOPermisos",
                table: "IncidenciasOPermisos");

            migrationBuilder.RenameTable(
                name: "IncidenciasOPermisos",
                newName: "IncidenciaOPermisos");

            migrationBuilder.RenameIndex(
                name: "IX_IncidenciasOPermisos_NominaEmpleado",
                table: "IncidenciaOPermisos",
                newName: "IX_IncidenciaOPermisos_NominaEmpleado");

            migrationBuilder.RenameIndex(
                name: "IX_IncidenciasOPermisos_MesFechaInicial",
                table: "IncidenciaOPermisos",
                newName: "IX_IncidenciaOPermisos_MesFechaInicial");

            migrationBuilder.RenameIndex(
                name: "IX_IncidenciasOPermisos_IdUsuarioSindicato",
                table: "IncidenciaOPermisos",
                newName: "IX_IncidenciaOPermisos_IdUsuarioSindicato");

            migrationBuilder.RenameIndex(
                name: "IX_IncidenciasOPermisos_IdUsuarioEmpleado",
                table: "IncidenciaOPermisos",
                newName: "IX_IncidenciaOPermisos_IdUsuarioEmpleado");

            migrationBuilder.RenameIndex(
                name: "IX_IncidenciasOPermisos_IdUsuarioAutoiza",
                table: "IncidenciaOPermisos",
                newName: "IX_IncidenciaOPermisos_IdUsuarioAutoiza");

            migrationBuilder.RenameIndex(
                name: "IX_IncidenciasOPermisos_IdRegla",
                table: "IncidenciaOPermisos",
                newName: "IX_IncidenciaOPermisos_IdRegla");

            migrationBuilder.RenameIndex(
                name: "IX_IncidenciasOPermisos_IdGrupo",
                table: "IncidenciaOPermisos",
                newName: "IX_IncidenciaOPermisos_IdGrupo");

            migrationBuilder.RenameIndex(
                name: "IX_IncidenciasOPermisos_Fecha",
                table: "IncidenciaOPermisos",
                newName: "IX_IncidenciaOPermisos_Fecha");

            migrationBuilder.RenameIndex(
                name: "IX_IncidenciasOPermisos_DiaFechaInicial",
                table: "IncidenciaOPermisos",
                newName: "IX_IncidenciaOPermisos_DiaFechaInicial");

            migrationBuilder.RenameIndex(
                name: "IX_IncidenciasOPermisos_AnioFechaInicial",
                table: "IncidenciaOPermisos",
                newName: "IX_IncidenciaOPermisos_AnioFechaInicial");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IncidenciaOPermisos",
                table: "IncidenciaOPermisos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarioPorEmpleados_IncidenciaOPermisos_IdIncidenciaOPermiso",
                table: "CalendarioPorEmpleados",
                column: "IdIncidenciaOPermiso",
                principalTable: "IncidenciaOPermisos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IncidenciaOPermisos_Grupos_IdGrupo",
                table: "IncidenciaOPermisos",
                column: "IdGrupo",
                principalTable: "Grupos",
                principalColumn: "GrupoId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IncidenciaOPermisos_Reglas_IdRegla",
                table: "IncidenciaOPermisos",
                column: "IdRegla",
                principalTable: "Reglas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IncidenciaOPermisos_Users_IdUsuarioAutoiza",
                table: "IncidenciaOPermisos",
                column: "IdUsuarioAutoiza",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IncidenciaOPermisos_Users_IdUsuarioEmpleado",
                table: "IncidenciaOPermisos",
                column: "IdUsuarioEmpleado",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IncidenciaOPermisos_Users_IdUsuarioSindicato",
                table: "IncidenciaOPermisos",
                column: "IdUsuarioSindicato",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
