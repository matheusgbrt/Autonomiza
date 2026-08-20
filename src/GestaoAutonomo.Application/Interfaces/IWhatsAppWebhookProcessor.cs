namespace GestaoAutonomo.Application.Interfaces;

public interface IWhatsAppWebhookProcessor
{
    Task ProcessarMensagemRecebidaAsync(
        string instanceId,
        string telefoneRemetente,
        string? mensagemTexto,
        string? botaoSelecionadoId,
        bool fromMe,
        CancellationToken ct);
}
