using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RealEstateApp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDomainToEnglish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrosPunchCard");

            migrationBuilder.DropTable(
                name: "SesionesPunchCard");

            migrationBuilder.CreateTable(
                name: "PunchCardSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    TotalRecords = table.Column<int>(type: "integer", nullable: false),
                    ProcessedRecords = table.Column<int>(type: "integer", nullable: false),
                    ErrorRecords = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    UserName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    UploadDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PunchCardSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PunchCardRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PunchCardSessionId = table.Column<int>(type: "integer", nullable: false),
                    EmployeeName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Department = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ClockInTime = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    ClockOutTime = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    HoursWorked = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    NameEdited = table.Column<bool>(type: "boolean", nullable: false),
                    OriginalName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PunchCardRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PunchCardRecords_PunchCardSessions_PunchCardSessionId",
                        column: x => x.PunchCardSessionId,
                        principalTable: "PunchCardSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PunchCardRecords_PunchCardSessionId",
                table: "PunchCardRecords",
                column: "PunchCardSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_PunchCardRecords_PunchCardSessionId_EmployeeName_Date",
                table: "PunchCardRecords",
                columns: new[] { "PunchCardSessionId", "EmployeeName", "Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PunchCardRecords");

            migrationBuilder.DropTable(
                name: "PunchCardSessions");

            migrationBuilder.CreateTable(
                name: "SesionesPunchCard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    FechaSubida = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    NombreArchivo = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    RegistrosConError = table.Column<int>(type: "integer", nullable: false),
                    RegistrosProcesados = table.Column<int>(type: "integer", nullable: false),
                    TotalRegistros = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    UsuarioNombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SesionesPunchCard", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosPunchCard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SesionPunchCardId = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    Departamento = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HoraEntrada = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    HoraSalida = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    HorasTrabajadas = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    NombreEditado = table.Column<bool>(type: "boolean", nullable: false),
                    NombreEmpleado = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NombreOriginal = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Observacion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosPunchCard", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosPunchCard_SesionesPunchCard_SesionPunchCardId",
                        column: x => x.SesionPunchCardId,
                        principalTable: "SesionesPunchCard",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosPunchCard_SesionPunchCardId",
                table: "RegistrosPunchCard",
                column: "SesionPunchCardId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosPunchCard_SesionPunchCardId_NombreEmpleado_Fecha",
                table: "RegistrosPunchCard",
                columns: new[] { "SesionPunchCardId", "NombreEmpleado", "Fecha" });
        }
    }
}
