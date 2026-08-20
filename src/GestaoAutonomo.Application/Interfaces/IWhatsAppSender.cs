namespace GestaoAutonomo.Application.Interfaces;

public record BotaoWhatsApp(string Id, string Label);

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
}
