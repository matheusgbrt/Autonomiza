using GestaoAutonomo.Application.DTOs.Meta;
using GestaoAutonomo.Application.Exceptions;
using GestaoAutonomo.Application.Interfaces;
using GestaoAutonomo.Domain.Entities;
using GestaoAutonomo.Domain.Enums;

namespace GestaoAutonomo.Application.Services;

public class MetaService : IMetaService
{
    private readonly IMetaRepository _metaRepository;
    private readonly ILancamentoFinanceiroRepository _lancamentoRepository;
    private readonly IAgendamentoRepository _agendamentoRepository;

    public MetaService(
        IMetaRepository metaRepository,
        ILancamentoFinanceiroRepository lancamentoRepository,
        IAgendamentoRepository agendamentoRepository)
    {
        _metaRepository = metaRepository;
        _lancamentoRepository = lancamentoRepository;
        _agendamentoRepository = agendamentoRepository;
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

        return await ParaDtoAsync(usuarioId, meta, ct);
    }

    public async Task<MetaDto?> ObterPorIdAsync(Guid usuarioId, Guid id, CancellationToken ct)
    {
        var meta = await _metaRepository.ObterPorIdAsync(usuarioId, id, ct);
        return meta is null ? null : await ParaDtoAsync(usuarioId, meta, ct);
    }

    public async Task<IReadOnlyList<MetaDto>> ListarAsync(Guid usuarioId, CancellationToken ct)
    {
        var metas = await _metaRepository.ListarAsync(usuarioId, ct);
        var resultado = new List<MetaDto>(metas.Count);
        foreach (var meta in metas)
        {
            resultado.Add(await ParaDtoAsync(usuarioId, meta, ct));
        }
        return resultado;
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

        return await ParaDtoAsync(usuarioId, meta, ct);
    }

    public async Task RemoverAsync(Guid usuarioId, Guid id, CancellationToken ct)
    {
        var meta = await _metaRepository.ObterPorIdAsync(usuarioId, id, ct)
            ?? throw new RecursoNaoEncontradoException("Meta não encontrada.");

        _metaRepository.Remover(meta);
        await _metaRepository.SalvarAlteracoesAsync(ct);
    }

    private async Task<MetaDto> ParaDtoAsync(Guid usuarioId, Meta meta, CancellationToken ct)
    {
        var valorAtual = meta.Tipo == TipoMeta.Faturamento
            ? (await _lancamentoRepository.ListarEntrePeriodoAsync(usuarioId, meta.PeriodoInicio, meta.PeriodoFim, ct))
                .Where(l => l.Tipo == TipoLancamento.Entrada)
                .Sum(l => l.Valor)
            : (await _agendamentoRepository.ListarPorPeriodoAsync(usuarioId, meta.PeriodoInicio, meta.PeriodoFim, ct))
                .Count(a => a.Status != StatusAgendamento.Cancelado);

        var progresso = meta.ValorAlvo > 0
            ? Math.Round(100m * valorAtual / meta.ValorAlvo, 2)
            : 0m;

        return new MetaDto(
            meta.Id,
            meta.Tipo,
            meta.Titulo,
            meta.ValorAlvo,
            valorAtual,
            progresso,
            meta.PeriodoInicio,
            meta.PeriodoFim,
            meta.CreatedAt);
    }
}
