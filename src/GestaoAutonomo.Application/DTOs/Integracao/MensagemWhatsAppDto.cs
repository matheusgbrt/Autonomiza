namespace GestaoAutonomo.Application.DTOs.Integracao;

public record MensagemWhatsAppDto(
    string? ClienteNome,
    string Telefone,
    string Direcao,
    string Conteudo,
    DateTime CriadoEm);
