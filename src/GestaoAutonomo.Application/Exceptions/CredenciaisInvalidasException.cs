namespace GestaoAutonomo.Application.Exceptions;

public sealed class CredenciaisInvalidasException : AppException
{
    public override int StatusCode => 401;

    public CredenciaisInvalidasException() : base("E-mail ou senha inválidos.")
    {
    }
}
