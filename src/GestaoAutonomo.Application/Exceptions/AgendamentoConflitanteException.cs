namespace GestaoAutonomo.Application.Exceptions;

public sealed class AgendamentoConflitanteException : AppException
{
    public override int StatusCode => 409;

    public AgendamentoConflitanteException() : base("Já existe um agendamento nesse intervalo de horário.")
    {
    }
}
