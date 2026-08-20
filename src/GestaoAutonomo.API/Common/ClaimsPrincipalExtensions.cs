using System.Security.Claims;

namespace GestaoAutonomo.API.Common;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUsuarioId(this ClaimsPrincipal principal)
    {
        var uid = principal.FindFirstValue("uid")
            ?? throw new InvalidOperationException("Claim 'uid' não encontrada no token.");
        return Guid.Parse(uid);
    }
}
