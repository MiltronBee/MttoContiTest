using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tiempo_libre.Migrations
{
    /// <inheritdoc />
    public partial class SistemaBloquesReservacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Crear tabla BloquesReservacion
            migrationBuilder.CreateTable(
                name: "BloquesReservacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GrupoId = table.Column<int>(type: "int", nullable: false),
                    AnioGeneracion = table.Column<int>(type: "int", nullable: false),
                    NumeroBloque = table.Column<int>(type: "int", nullable: false),
                    FechaHoraInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaHoraFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PersonasPorBloque = table.Column<int>(type: "int", nullable: false),
                    DuracionHoras = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Activo"),
                    EsBloqueCola = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FechaGeneracion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    GeneradoPor = table.Column<int>(type: "int", nullable: false),
                    FechaAprobacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AprobadoPor = table.Column<int>(type: "int", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BloquesReservacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BloquesReservacion_Grupos_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "Grupos",
                        principalColumn: "GrupoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BloquesReservacion_Users_GeneradoPor",
                        column: x => x.GeneradoPor,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_BloquesReservacion_Users_AprobadoPor",
                        column: x => x.AprobadoPor,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            // Crear tabla AsignacionesBloque
            migrationBuilder.CreateTable(
                name: "AsignacionesBloque",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BloqueId = table.Column<int>(type: "int", nullable: false),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    PosicionEnBloque = table.Column<int>(type: "int", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    AsignedoPor = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Asignado"),
                    FechaCompletado = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsignacionesBloque", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AsignacionesBloque_BloquesReservacion_BloqueId",
                        column: x => x.BloqueId,
                        principalTable: "BloquesReservacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AsignacionesBloque_Users_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_AsignacionesBloque_Users_AsignedoPor",
                        column: x => x.AsignedoPor,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            // Crear tabla CambiosBloque
            migrationBuilder.CreateTable(
                name: "CambiosBloque",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    BloqueOrigenId = table.Column<int>(type: "int", nullable: false),
                    BloqueDestinoId = table.Column<int>(type: "int", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FechaCambio = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    AutorizadoPor = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Aplicado"),
                    ObservacionesAdicionales = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CambiosBloque", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CambiosBloque_Users_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_CambiosBloque_BloquesReservacion_BloqueOrigenId",
                        column: x => x.BloqueOrigenId,
                        principalTable: "BloquesReservacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_CambiosBloque_BloquesReservacion_BloqueDestinoId",
                        column: x => x.BloqueDestinoId,
                        principalTable: "BloquesReservacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_CambiosBloque_Users_AutorizadoPor",
                        column: x => x.AutorizadoPor,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            // Crear índices
            migrationBuilder.CreateIndex(
                name: "IX_BloquesReservacion_GrupoId",
                table: "BloquesReservacion",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_BloquesReservacion_AnioGeneracion_GrupoId",
                table: "BloquesReservacion",
                columns: new[] { "AnioGeneracion", "GrupoId" });

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionesBloque_BloqueId",
                table: "AsignacionesBloque",
                column: "BloqueId");

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionesBloque_EmpleadoId",
                table: "AsignacionesBloque",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_CambiosBloque_EmpleadoId",
                table: "CambiosBloque",
                column: "EmpleadoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "CambiosBloque");
            migrationBuilder.DropTable(name: "AsignacionesBloque");
            migrationBuilder.DropTable(name: "BloquesReservacion");
        }
    }
}
