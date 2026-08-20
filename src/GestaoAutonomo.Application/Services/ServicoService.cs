using GestaoAutonomo.Application.DTOs.Servico;
using GestaoAutonomo.Application.Exceptions;
using GestaoAutonomo.Application.Interfaces;
using GestaoAutonomo.Domain.Entities;

namespace GestaoAutonomo.Application.Services;

public class ServicoService : IServicoService
{
    private readonly IServicoRepository _servicoRepository;

    public ServicoService(IServicoRepository servicoRepository)
    {
        _servicoRepository = servicoRepository;
    }

    public async Task<ServicoDto> CriarAsync(Guid usuarioId, CriarServicoDto dto, CancellationToken ct)
    {
        var servico = new Servico
        {
            UsuarioId = usuarioId,
            Nome = dto.Nome,
            Descricao = dto.Descricao,
            Duracao = dto.Duracao,
            ValorPadrao = dto.ValorPadrao
        };

        await _servicoRepository.AdicionarAsync(servico, ct);
        await _servicoRepository.SalvarAlteracoesAsync(ct);

        return ParaDto(servico);
    }

    public async Task<ServicoDto?> ObterPorIdAsync(Guid usuarioId, Guid id, CancellationToken ct)
    {
        var servico = await _servicoRepository.ObterPorIdAsync(usuarioId, id, ct);
        return servico is null ? null : ParaDto(servico);
    }

    public async Task<IReadOnlyList<ServicoDto>> ListarAsync(Guid usuarioId, CancellationToken ct)
    {
        var servicos = await _servicoRepository.ListarAsync(usuarioId, ct);
        return servicos.Select(ParaDto).ToList();
    }

    public async Task<ServicoDto> AtualizarAsync(Guid usuarioId, Guid id, AtualizarServicoDto dto, CancellationToken ct)
    {
        var servico = await _servicoRepository.ObterPorIdAsync(usuarioId, id, ct)
            ?? throw new RecursoNaoEncontradoException("Serviço não encontrado.");

        servico.Nome = dto.Nome;
        servico.Descricao = dto.Descricao;
        servico.Duracao = dto.Duracao;
        servico.ValorPadrao = dto.ValorPadrao;

        await _servicoRepository.SalvarAlteracoesAsync(ct);

        return ParaDto(servico);
    }

    public async Task RemoverAsync(Guid usuarioId, Guid id, CancellationToken ct)
    {
        var servico = await _servicoRepository.ObterPorIdAsync(usuarioId, id, ct)
            ?? throw new RecursoNaoEncontradoException("Serviço não encontrado.");

        _servicoRepository.Remover(servico);
        await _servicoRepository.SalvarAlteracoesAsync(ct);
    }

    private static ServicoDto ParaDto(Servico servico) => new(
        servico.Id,
        servico.Nome,
        servico.Descricao,
        servico.Duracao,
        servico.ValorPadrao,
        servico.CreatedAt);
}
