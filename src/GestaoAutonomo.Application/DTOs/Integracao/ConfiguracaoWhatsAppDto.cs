namespace GestaoAutonomo.Application.DTOs.Integracao;

public record ConfiguracaoWhatsAppDto(
    bool RespostasAutomaticas,
    bool HorariosDisponiveis,
    bool ConfirmarAgendamentos,
    bool LembretesAutomaticos,
    string? MensagemBoasVindas);

public record AtualizarConfiguracaoWhatsAppDto(
    bool RespostasAutomaticas,
    bool HorariosDisponiveis,
    bool ConfirmarAgendamentos,
    bool LembretesAutomaticos,
    string? MensagemBoasVindas);
