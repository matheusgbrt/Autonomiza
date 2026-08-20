using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoAutonomo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddInsightIAAndWhatsAppIntegration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ZApiClientToken",
                table: "Usuarios",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZApiInstanceId",
                table: "Usuarios",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZApiToken",
                table: "Usuarios",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InsightsIA",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    Categoria = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Mensagem = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    ExpiraEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsightsIA", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_ZApiInstanceId",
                table: "Usuarios",
                column: "ZApiInstanceId",
                unique: true,
                filter: "\"ZApiInstanceId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InsightsIA_UsuarioId",
                table: "InsightsIA",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InsightsIA");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_ZApiInstanceId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "ZApiClientToken",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "ZApiInstanceId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "ZApiToken",
                table: "Usuarios");
        }
    }
}
