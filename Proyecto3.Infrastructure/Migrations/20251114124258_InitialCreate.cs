using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyecto3.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Colaboradores",
                columns: table => new
                {
                    ColaboradorID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCompleto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RolActual = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CuentaProyecto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisponibilidadMovilidad = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colaboradores", x => x.ColaboradorID);
                });

            migrationBuilder.CreateTable(
                name: "PerfilesVacante",
                columns: table => new
                {
                    VacanteID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombrePerfil = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NivelDeseado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaUrgencia = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activa = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilesVacante", x => x.VacanteID);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    SkillID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreSkill = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoSkill = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.SkillID);
                });

            migrationBuilder.CreateTable(
                name: "Certificaciones",
                columns: table => new
                {
                    CertificacionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ColaboradorID = table.Column<int>(type: "int", nullable: false),
                    NombreCertificacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaObtencion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Institucion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificaciones", x => x.CertificacionID);
                    table.ForeignKey(
                        name: "FK_Certificaciones_Colaboradores_ColaboradorID",
                        column: x => x.ColaboradorID,
                        principalTable: "Colaboradores",
                        principalColumn: "ColaboradorID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HabilidadesColaboradores",
                columns: table => new
                {
                    ColaboradorID = table.Column<int>(type: "int", nullable: false),
                    SkillID = table.Column<int>(type: "int", nullable: false),
                    NivelDominio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaUltimaEvaluacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HabilidadesColaboradores", x => new { x.ColaboradorID, x.SkillID });
                    table.ForeignKey(
                        name: "FK_HabilidadesColaboradores_Colaboradores_ColaboradorID",
                        column: x => x.ColaboradorID,
                        principalTable: "Colaboradores",
                        principalColumn: "ColaboradorID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HabilidadesColaboradores_Skills_SkillID",
                        column: x => x.SkillID,
                        principalTable: "Skills",
                        principalColumn: "SkillID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequisitosVacante",
                columns: table => new
                {
                    RequisitoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VacanteID = table.Column<int>(type: "int", nullable: false),
                    SkillID = table.Column<int>(type: "int", nullable: false),
                    NivelMinimoRequerido = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequisitosVacante", x => x.RequisitoID);
                    table.ForeignKey(
                        name: "FK_RequisitosVacante_PerfilesVacante_VacanteID",
                        column: x => x.VacanteID,
                        principalTable: "PerfilesVacante",
                        principalColumn: "VacanteID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequisitosVacante_Skills_SkillID",
                        column: x => x.SkillID,
                        principalTable: "Skills",
                        principalColumn: "SkillID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Certificaciones_ColaboradorID",
                table: "Certificaciones",
                column: "ColaboradorID");

            migrationBuilder.CreateIndex(
                name: "IX_HabilidadesColaboradores_SkillID",
                table: "HabilidadesColaboradores",
                column: "SkillID");

            migrationBuilder.CreateIndex(
                name: "IX_RequisitosVacante_SkillID",
                table: "RequisitosVacante",
                column: "SkillID");

            migrationBuilder.CreateIndex(
                name: "IX_RequisitosVacante_VacanteID",
                table: "RequisitosVacante",
                column: "VacanteID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Certificaciones");

            migrationBuilder.DropTable(
                name: "HabilidadesColaboradores");

            migrationBuilder.DropTable(
                name: "RequisitosVacante");

            migrationBuilder.DropTable(
                name: "Colaboradores");

            migrationBuilder.DropTable(
                name: "PerfilesVacante");

            migrationBuilder.DropTable(
                name: "Skills");
        }
    }
}
