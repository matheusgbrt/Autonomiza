using GestaoAutonomo.Domain.Entities;

namespace GestaoAutonomo.Application.Interfaces;

public interface IMensagemWhatsAppRepository
{
    Task AdicionarAsync(MensagemWhatsApp mensagem, CancellationToken ct);
    Task<bool> ExisteConversaAnteriorAsync(Guid usuarioId, string telefone, CancellationToken ct);
    Task<int> ContarConversasUnicasAsync(Guid usuarioId, DateTime inicio, DateTime fimExclusivo, CancellationToken ct);
    Task<IReadOnlyList<MensagemWhatsApp>> ListarUltimaConversaAsync(Guid usuarioId, int quantidade, CancellationToken ct);
    Task SalvarAlteracoesAsync(CancellationToken ct);
}
