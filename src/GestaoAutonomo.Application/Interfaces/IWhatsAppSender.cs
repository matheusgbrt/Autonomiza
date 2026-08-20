namespace GestaoAutonomo.Application.Interfaces;

public record BotaoWhatsApp(string Id, string Label);

public record OpcaoWhatsApp(string Id, string Titulo, string? Descricao);

public interface IWhatsAppSender
{
    Task EnviarTextoAsync(string instanceId, string token, string? clientToken, string telefoneDestino, string mensagem, CancellationToken ct);

    Task EnviarBotoesAsync(
        string instanceId,
        string token,
        string? clientToken,
        string telefoneDestino,
        string mensagem,
        IReadOnlyList<BotaoWhatsApp> botoes,
        CancellationToken ct);

    Task EnviarListaOpcoesAsync(
        string instanceId,
        string token,
        string? clientToken,
        string telefoneDestino,
        string mensagem,
        string titulo,
        string botaoLabel,
        IReadOnlyList<OpcaoWhatsApp> opcoes,
        CancellationToken ct);
}
