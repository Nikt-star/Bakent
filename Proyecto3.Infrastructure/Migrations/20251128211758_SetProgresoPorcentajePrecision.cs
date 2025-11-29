using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyecto3.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SetProgresoPorcentajePrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TipoSkill",
                table: "Skills",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Beneficios",
                columns: table => new
                {
                    BeneficioID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beneficios", x => x.BeneficioID);
                });

            migrationBuilder.CreateTable(
                name: "Cursos",
                columns: table => new
                {
                    CursoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DuracionHoras = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cursos", x => x.CursoID);
                });

            migrationBuilder.CreateTable(
                name: "Notificaciones",
                columns: table => new
                {
                    NotificacionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ColaboradorID = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mensaje = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Leida = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaLectura = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificaciones", x => x.NotificacionID);
                    table.ForeignKey(
                        name: "FK_Notificaciones_Colaboradores_ColaboradorID",
                        column: x => x.ColaboradorID,
                        principalTable: "Colaboradores",
                        principalColumn: "ColaboradorID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Perfiles",
                columns: table => new
                {
                    PerfilID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ColaboradorID = table.Column<int>(type: "int", nullable: false),
                    Resumen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LinkedIn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PerfilCompleto = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Perfiles", x => x.PerfilID);
                    table.ForeignKey(
                        name: "FK_Perfiles_Colaboradores_ColaboradorID",
                        column: x => x.ColaboradorID,
                        principalTable: "Colaboradores",
                        principalColumn: "ColaboradorID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Puntos",
                columns: table => new
                {
                    PuntoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ColaboradorID = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    Razon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaOtorgamiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAcumulado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Puntos", x => x.PuntoID);
                    table.ForeignKey(
                        name: "FK_Puntos_Colaboradores_ColaboradorID",
                        column: x => x.ColaboradorID,
                        principalTable: "Colaboradores",
                        principalColumn: "ColaboradorID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VacanteAplicaciones",
                columns: table => new
                {
                    AplicacionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VacanteID = table.Column<int>(type: "int", nullable: false),
                    ColaboradorID = table.Column<int>(type: "int", nullable: false),
                    FechaAplicacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Interes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notas = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacanteAplicaciones", x => x.AplicacionID);
                    table.ForeignKey(
                        name: "FK_VacanteAplicaciones_Colaboradores_ColaboradorID",
                        column: x => x.ColaboradorID,
                        principalTable: "Colaboradores",
                        principalColumn: "ColaboradorID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VacanteAplicaciones_PerfilesVacante_VacanteID",
                        column: x => x.VacanteID,
                        principalTable: "PerfilesVacante",
                        principalColumn: "VacanteID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgresoCursos",
                columns: table => new
                {
                    ProgresoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ColaboradorID = table.Column<int>(type: "int", nullable: false),
                    CursoID = table.Column<int>(type: "int", nullable: false),
                    PorcentajeCompletacion = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Completado = table.Column<bool>(type: "bit", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaCompletacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgresoCursos", x => x.ProgresoID);
                    table.ForeignKey(
                        name: "FK_ProgresoCursos_Colaboradores_ColaboradorID",
                        column: x => x.ColaboradorID,
                        principalTable: "Colaboradores",
                        principalColumn: "ColaboradorID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProgresoCursos_Cursos_CursoID",
                        column: x => x.CursoID,
                        principalTable: "Cursos",
                        principalColumn: "CursoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_ColaboradorID",
                table: "Notificaciones",
                column: "ColaboradorID");

            migrationBuilder.CreateIndex(
                name: "IX_Perfiles_ColaboradorID",
                table: "Perfiles",
                column: "ColaboradorID");

            migrationBuilder.CreateIndex(
                name: "IX_ProgresoCursos_ColaboradorID",
                table: "ProgresoCursos",
                column: "ColaboradorID");

            migrationBuilder.CreateIndex(
                name: "IX_ProgresoCursos_CursoID",
                table: "ProgresoCursos",
                column: "CursoID");

            migrationBuilder.CreateIndex(
                name: "IX_Puntos_ColaboradorID",
                table: "Puntos",
                column: "ColaboradorID");

            migrationBuilder.CreateIndex(
                name: "IX_VacanteAplicaciones_ColaboradorID",
                table: "VacanteAplicaciones",
                column: "ColaboradorID");

            migrationBuilder.CreateIndex(
                name: "IX_VacanteAplicaciones_VacanteID",
                table: "VacanteAplicaciones",
                column: "VacanteID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Beneficios");

            migrationBuilder.DropTable(
                name: "Notificaciones");

            migrationBuilder.DropTable(
                name: "Perfiles");

            migrationBuilder.DropTable(
                name: "ProgresoCursos");

            migrationBuilder.DropTable(
                name: "Puntos");

            migrationBuilder.DropTable(
                name: "VacanteAplicaciones");

            migrationBuilder.DropTable(
                name: "Cursos");

            migrationBuilder.AlterColumn<string>(
                name: "TipoSkill",
                table: "Skills",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
