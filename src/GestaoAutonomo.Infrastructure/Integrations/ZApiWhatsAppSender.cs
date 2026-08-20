using System.Net.Http.Json;
using GestaoAutonomo.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace GestaoAutonomo.Infrastructure.Integrations;

public class ZApiWhatsAppSender : IWhatsAppSender
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ZApiWhatsAppSender> _logger;

    public ZApiWhatsAppSender(HttpClient httpClient, ILogger<ZApiWhatsAppSender> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public Task EnviarTextoAsync(string instanceId, string token, string? clientToken, string telefoneDestino, string mensagem, CancellationToken ct) =>
        EnviarAsync(
            instanceId, token, clientToken, "send-text",
            new { phone = telefoneDestino, message = mensagem },
            telefoneDestino, ct);

    public Task EnviarBotoesAsync(
        string instanceId,
        string token,
        string? clientToken,
        string telefoneDestino,
        string mensagem,
        IReadOnlyList<BotaoWhatsApp> botoes,
        CancellationToken ct) =>
        EnviarAsync(
            instanceId, token, clientToken, "send-button-list",
            new
            {
                phone = telefoneDestino,
                message = mensagem,
                buttonList = new
                {
                    buttons = botoes.Select(b => new { id = b.Id, label = b.Label })
                }
            },
            telefoneDestino, ct);

    private async Task EnviarAsync(
        string instanceId,
        string token,
        string? clientToken,
        string endpoint,
        object corpo,
        string telefoneDestino,
        CancellationToken ct)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, $"instances/{instanceId}/token/{token}/{endpoint}");

            if (!string.IsNullOrWhiteSpace(clientToken))
            {
                request.Headers.Add("Client-Token", clientToken);
            }

            request.Content = JsonContent.Create(corpo);

            var response = await _httpClient.SendAsync(request, ct);
            if (!response.IsSuccessStatusCode)
            {
                var corpoResposta = await response.Content.ReadAsStringAsync(ct);
                _logger.LogWarning("Falha ao enviar mensagem via Z-API ({Status}): {Body}", response.StatusCode, corpoResposta);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao enviar mensagem via Z-API para {Telefone}.", telefoneDestino);
        }
    }
}
