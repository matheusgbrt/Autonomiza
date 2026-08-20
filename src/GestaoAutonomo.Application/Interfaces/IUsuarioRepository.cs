using GestaoAutonomo.Domain.Entities;

namespace GestaoAutonomo.Application.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken ct);
    Task<Usuario?> ObterPorIdAsync(Guid id, CancellationToken ct);
    Task<Usuario?> ObterPorZApiInstanceIdAsync(string instanceId, CancellationToken ct);
    Task<IReadOnlyList<Usuario>> ListarProAsync(CancellationToken ct);
    Task AdicionarAsync(Usuario usuario, CancellationToken ct);
    Task SalvarAlteracoesAsync(CancellationToken ct);
}
