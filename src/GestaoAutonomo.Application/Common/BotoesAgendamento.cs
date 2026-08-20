using GestaoAutonomo.Application.Interfaces;

namespace GestaoAutonomo.Application.Common;

public static class BotoesAgendamento
{
    public static readonly IReadOnlyList<BotaoWhatsApp> ConfirmarCancelar = new[]
    {
        new BotaoWhatsApp("1", "Confirmar"),
        new BotaoWhatsApp("2", "Cancelar")
    };
}
