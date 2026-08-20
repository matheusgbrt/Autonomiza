using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoAutonomo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWhatsAppMensagensEConfiguracoesEHealthScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "WhatsAppConfirmarAgendamentosAtivo",
                table: "Usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "WhatsAppHorariosDisponiveisAtivo",
                table: "Usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "WhatsAppLembretesAutomaticosAtivos",
                table: "Usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "WhatsAppMensagemBoasVindas",
                table: "Usuarios",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "WhatsAppRespostasAutomaticasAtivas",
                table: "Usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "NotaAtendimento",
                table: "Agendamentos",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MensagensWhatsApp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uuid", nullable: true),
                    Telefone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Direcao = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Conteudo = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MensagensWhatsApp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MensagensWhatsApp_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MensagensWhatsApp_ClienteId",
                table: "MensagensWhatsApp",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_MensagensWhatsApp_UsuarioId_CreatedAt",
                table: "MensagensWhatsApp",
                columns: new[] { "UsuarioId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_MensagensWhatsApp_UsuarioId_Telefone",
                table: "MensagensWhatsApp",
                columns: new[] { "UsuarioId", "Telefone" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MensagensWhatsApp");

            migrationBuilder.DropColumn(
                name: "WhatsAppConfirmarAgendamentosAtivo",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "WhatsAppHorariosDisponiveisAtivo",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "WhatsAppLembretesAutomaticosAtivos",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "WhatsAppMensagemBoasVindas",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "WhatsAppRespostasAutomaticasAtivas",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "NotaAtendimento",
                table: "Agendamentos");
        }
    }
}
