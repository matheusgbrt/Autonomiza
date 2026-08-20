namespace GestaoAutonomo.Application.Exceptions;

public sealed class EmailJaCadastradoException : AppException
{
    public override int StatusCode => 409;

    public EmailJaCadastradoException() : base("Já existe um usuário cadastrado com este e-mail.")
    {
    }
}
