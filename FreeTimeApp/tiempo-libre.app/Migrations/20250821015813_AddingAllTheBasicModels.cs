using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tiempo_libre.Migrations
{
    /// <inheritdoc />
    public partial class AddingAllTheBasicModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    AreaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnidadOrganizativaSap = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    NombreGeneral = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Areas__70B82048C119927A", x => x.AreaId);
                });

            migrationBuilder.CreateTable(
                name: "Empleados",
                columns: table => new
                {
                    Nomina = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FechaAlta = table.Column<DateOnly>(type: "date", nullable: true),
                    CentroCoste = table.Column<int>(type: "int", nullable: true),
                    Posicion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UnidadOrganizativa = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    EncargadoRegistro = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Rol = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Empleado__765BE2D8758F406A", x => x.Nomina);
                });

            migrationBuilder.CreateTable(
                name: "PermisosEIncapacidades",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nomina = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Posicion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Desde = table.Column<DateOnly>(type: "date", nullable: true),
                    Hasta = table.Column<DateOnly>(type: "date", nullable: true),
                    ClAbPre = table.Column<int>(type: "int", nullable: true),
                    ClaseAbsentismo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Dias = table.Column<double>(type: "float", nullable: true),
                    DiaNat = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Permisos__3214EC27F814167F", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Abreviation = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolesEmpleados",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nomina = table.Column<int>(type: "int", nullable: true),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Fecha = table.Column<DateOnly>(type: "date", nullable: true),
                    Dia = table.Column<string>(type: "char(2)", unicode: false, fixedLength: true, maxLength: 2, nullable: true),
                    PHTD = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    TextoPlanHrTrDia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CentroDeCoste = table.Column<int>(type: "int", nullable: true),
                    EncTiempos = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__RolesEmp__3214EC2740992AEF", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Sindicalizados",
                columns: table => new
                {
                    SindicalizadoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nomina = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FechaAlta = table.Column<DateOnly>(type: "date", nullable: true),
                    CentroCoste = table.Column<int>(type: "int", nullable: true),
                    Posicion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EncargadoRegistro = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Sindical__958BE91075C969C4", x => x.SindicalizadoId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordSalt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    GrupoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Grupos",
                columns: table => new
                {
                    GrupoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Grupos__556BF040AD6A4924", x => x.GrupoId);
                    table.ForeignKey(
                        name: "FK_Grupos_Areas",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "AreaId");
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    RolesId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.RolesId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RolesId",
                        column: x => x.RolesId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SindicalizadosPorGrupo",
                columns: table => new
                {
                    SindicalizadoId = table.Column<int>(type: "int", nullable: false),
                    GrupoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Empleado__80DD5614D5A43EBC", x => new { x.SindicalizadoId, x.GrupoId });
                    table.ForeignKey(
                        name: "FK_EmpleadosPorGrupo_Empleados",
                        column: x => x.SindicalizadoId,
                        principalTable: "Sindicalizados",
                        principalColumn: "SindicalizadoId");
                    table.ForeignKey(
                        name: "FK_EmpleadosPorGrupo_Grupos",
                        column: x => x.GrupoId,
                        principalTable: "Grupos",
                        principalColumn: "GrupoId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Grupos_AreaId",
                table: "Grupos",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "UQ__Sindical__765BE2D9A52A48AC",
                table: "Sindicalizados",
                column: "Nomina",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SindicalizadosPorGrupo_GrupoId",
                table: "SindicalizadosPorGrupo",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Empleados");

            migrationBuilder.DropTable(
                name: "PermisosEIncapacidades");

            migrationBuilder.DropTable(
                name: "RolesEmpleados");

            migrationBuilder.DropTable(
                name: "SindicalizadosPorGrupo");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Sindicalizados");

            migrationBuilder.DropTable(
                name: "Grupos");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Areas");
        }
    }
}
