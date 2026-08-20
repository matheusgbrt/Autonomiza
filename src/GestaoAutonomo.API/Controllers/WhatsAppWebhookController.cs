using GestaoAutonomo.API.Contracts;
using GestaoAutonomo.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestaoAutonomo.API.Controllers;

[ApiController]
[Route("api/webhooks/zapi")]
public class WhatsAppWebhookController : ControllerBase
{
    private readonly IWhatsAppWebhookProcessor _webhookProcessor;

    public WhatsAppWebhookController(IWhatsAppWebhookProcessor webhookProcessor)
    {
        _webhookProcessor = webhookProcessor;
    }

    [HttpPost("{instanceId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Receber(string instanceId, ZApiWebhookRequest payload, CancellationToken ct)
    {
        var temTexto = !string.IsNullOrWhiteSpace(payload.Text?.Message);
        var temBotao = !string.IsNullOrWhiteSpace(payload.ButtonsResponseMessage?.ButtonId);

        if (!string.IsNullOrWhiteSpace(payload.Phone) && (temTexto || temBotao))
        {
            await _webhookProcessor.ProcessarMensagemRecebidaAsync(
                instanceId, payload.Phone, payload.Text?.Message, payload.ButtonsResponseMessage?.ButtonId, payload.FromMe, ct);
        }

        return Ok();
    }
}
