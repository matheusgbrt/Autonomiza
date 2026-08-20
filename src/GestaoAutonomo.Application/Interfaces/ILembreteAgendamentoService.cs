namespace GestaoAutonomo.Application.Interfaces;

public interface ILembreteAgendamentoService
{
    Task EnviarLembretesPendentesAsync(CancellationToken ct);
}
