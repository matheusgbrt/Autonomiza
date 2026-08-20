namespace GestaoAutonomo.Application.Exceptions;

public sealed class IntegracaoWhatsAppJaVinculadaException : AppException
{
    public override int StatusCode => 409;

    public IntegracaoWhatsAppJaVinculadaException() : base("Este Instance ID do Z-API já está vinculado a outra conta.")
    {
    }
}
