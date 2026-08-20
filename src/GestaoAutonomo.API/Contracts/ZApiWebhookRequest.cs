namespace GestaoAutonomo.API.Contracts;

public record ZApiWebhookRequest(
    string? Phone,
    bool FromMe,
    ZApiWebhookText? Text,
    ZApiButtonsResponse? ButtonsResponseMessage,
    ZApiListResponse? ListResponseMessage);

public record ZApiWebhookText(string? Message);

public record ZApiButtonsResponse(string? ButtonId, string? Message);

public record ZApiListResponse(string? SelectedRowId, string? Message);
