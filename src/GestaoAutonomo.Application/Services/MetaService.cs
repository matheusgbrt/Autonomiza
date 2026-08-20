using GestaoAutonomo.Application.DTOs.Meta;
using GestaoAutonomo.Application.Exceptions;
using GestaoAutonomo.Application.Interfaces;
using GestaoAutonomo.Domain.Entities;

namespace GestaoAutonomo.Application.Services;

public class MetaService : IMetaService
{
    private readonly IMetaRepository _metaRepository;

    public MetaService(IMetaRepository metaRepository)
    {
        _metaRepository = metaRepository;
    }

    public async Task<MetaDto> CriarAsync(Guid usuarioId, CriarMetaDto dto, CancellationToken ct)
    {
        var meta = new Meta
        {
            UsuarioId = usuarioId,
            Tipo = dto.Tipo,
            Titulo = dto.Titulo,
            ValorAlvo = dto.ValorAlvo,
            PeriodoInicio = dto.PeriodoInicio,
            PeriodoFim = dto.PeriodoFim
        };

        await _metaRepository.AdicionarAsync(meta, ct);
        await _metaRepository.SalvarAlteracoesAsync(ct);

        return ParaDto(meta);
    }

    public async Task<MetaDto?> ObterPorIdAsync(Guid usuarioId, Guid id, CancellationToken ct)
    {
        var meta = await _metaRepository.ObterPorIdAsync(usuarioId, id, ct);
        return meta is null ? null : ParaDto(meta);
    }

    public async Task<IReadOnlyList<MetaDto>> ListarAsync(Guid usuarioId, CancellationToken ct)
    {
        var metas = await _metaRepository.ListarAsync(usuarioId, ct);
        return metas.Select(ParaDto).ToList();
    }

    public async Task<MetaDto> AtualizarAsync(Guid usuarioId, Guid id, AtualizarMetaDto dto, CancellationToken ct)
    {
        var meta = await _metaRepository.ObterPorIdAsync(usuarioId, id, ct)
            ?? throw new RecursoNaoEncontradoException("Meta não encontrada.");

        meta.Tipo = dto.Tipo;
        meta.Titulo = dto.Titulo;
        meta.ValorAlvo = dto.ValorAlvo;
        meta.PeriodoInicio = dto.PeriodoInicio;
        meta.PeriodoFim = dto.PeriodoFim;

        await _metaRepository.SalvarAlteracoesAsync(ct);

        return ParaDto(meta);
    }

    public async Task RemoverAsync(Guid usuarioId, Guid id, CancellationToken ct)
    {
        var meta = await _metaRepository.ObterPorIdAsync(usuarioId, id, ct)
            ?? throw new RecursoNaoEncontradoException("Meta não encontrada.");

        _metaRepository.Remover(meta);
        await _metaRepository.SalvarAlteracoesAsync(ct);
    }

    private static MetaDto ParaDto(Meta meta) => new(
        meta.Id,
        meta.Tipo,
        meta.Titulo,
        meta.ValorAlvo,
        meta.PeriodoInicio,
        meta.PeriodoFim,
        meta.CreatedAt);
}
