using GestaoAutonomo.Application.Interfaces;
using GestaoAutonomo.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace GestaoAutonomo.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<Usuario> _hasher = new();

    public string Hash(string senha) => _hasher.HashPassword(default!, senha);

    public bool Verificar(string senhaHash, string senhaFornecida) =>
        _hasher.VerifyHashedPassword(default!, senhaHash, senhaFornecida) != PasswordVerificationResult.Failed;
}
