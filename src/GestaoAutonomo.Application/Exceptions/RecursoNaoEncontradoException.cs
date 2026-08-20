namespace GestaoAutonomo.Application.Exceptions;

public sealed class RecursoNaoEncontradoException : AppException
{
    public override int StatusCode => 404;

    public RecursoNaoEncontradoException(string mensagem) : base(mensagem)
    {
    }
}
