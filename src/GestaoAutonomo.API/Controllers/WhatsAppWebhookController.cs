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
        var selecaoId = payload.ButtonsResponseMessage?.ButtonId ?? payload.ListResponseMessage?.SelectedRowId;
        var temTexto = !string.IsNullOrWhiteSpace(payload.Text?.Message);
        var temSelecao = !string.IsNullOrWhiteSpace(selecaoId);

        if (!string.IsNullOrWhiteSpace(payload.Phone) && (temTexto || temSelecao))
        {
            await _webhookProcessor.ProcessarMensagemRecebidaAsync(
                instanceId, payload.Phone, payload.Text?.Message, selecaoId, payload.FromMe, ct);
        }

        return Ok();
    }
}
