namespace GestaoAutonomo.Application.DTOs.Integracao;

public record EstatisticasWhatsAppDto(
    int ConversasHoje,
    int AgendamentosHoje,
    int AgendamentosMes,
    decimal TaxaConversaoPercentual);
