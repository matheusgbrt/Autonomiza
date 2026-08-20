namespace GestaoAutonomo.API.Contracts;

public record ZApiWebhookRequest(string? Phone, bool FromMe, ZApiWebhookText? Text, ZApiButtonsResponse? ButtonsResponseMessage);

public record ZApiWebhookText(string? Message);

public record ZApiButtonsResponse(string? ButtonId, string? Message);
