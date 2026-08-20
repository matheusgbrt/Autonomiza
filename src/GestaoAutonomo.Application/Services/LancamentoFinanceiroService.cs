using GestaoAutonomo.Application.DTOs.LancamentoFinanceiro;
using GestaoAutonomo.Application.Exceptions;
using GestaoAutonomo.Application.Interfaces;
using GestaoAutonomo.Domain.Entities;
using GestaoAutonomo.Domain.Enums;

namespace GestaoAutonomo.Application.Services;

public class LancamentoFinanceiroService : ILancamentoFinanceiroService
{
    private readonly ILancamentoFinanceiroRepository _lancamentoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IAgendamentoRepository _agendamentoRepository;

    public LancamentoFinanceiroService(
        ILancamentoFinanceiroRepository lancamentoRepository,
        IClienteRepository clienteRepository,
        IAgendamentoRepository agendamentoRepository)
    {
        _lancamentoRepository = lancamentoRepository;
        _clienteRepository = clienteRepository;
        _agendamentoRepository = agendamentoRepository;
    }

    public async Task<LancamentoFinanceiroDto> CriarAsync(Guid usuarioId, CriarLancamentoFinanceiroDto dto, CancellationToken ct)
    {
        await ValidarReferenciasAsync(usuarioId, dto.ClienteId, dto.AgendamentoId, ct);

        var lancamento = new LancamentoFinanceiro
        {
            UsuarioId = usuarioId,
            Tipo = dto.Tipo,
            Categoria = dto.Categoria,
            Valor = dto.Valor,
            Data = dto.Data,
            Descricao = dto.Descricao,
            ClienteId = dto.ClienteId,
            AgendamentoId = dto.AgendamentoId
        };

        await _lancamentoRepository.AdicionarAsync(lancamento, ct);
        await _lancamentoRepository.SalvarAlteracoesAsync(ct);

        return ParaDto(lancamento);
    }

    public async Task<LancamentoFinanceiroDto?> ObterPorIdAsync(Guid usuarioId, Guid id, CancellationToken ct)
    {
        var lancamento = await _lancamentoRepository.ObterPorIdAsync(usuarioId, id, ct);
        return lancamento is null ? null : ParaDto(lancamento);
    }

    public async Task<IReadOnlyList<LancamentoFinanceiroDto>> ListarAsync(Guid usuarioId, CancellationToken ct)
    {
        var lancamentos = await _lancamentoRepository.ListarAsync(usuarioId, ct);
        return lancamentos.Select(ParaDto).ToList();
    }

    public async Task<SaldoMensalDto> ObterSaldoMensalAsync(Guid usuarioId, int ano, int mes, CancellationToken ct)
    {
        var inicio = new DateTime(ano, mes, 1, 0, 0, 0, DateTimeKind.Utc);
        var fimExclusivo = inicio.AddMonths(1);

        var lancamentos = await _lancamentoRepository.ListarEntrePeriodoAsync(usuarioId, inicio, fimExclusivo, ct);

        var totalEntradas = lancamentos.Where(l => l.Tipo == TipoLancamento.Entrada).Sum(l => l.Valor);
        var totalSaidas = lancamentos.Where(l => l.Tipo == TipoLancamento.Saida).Sum(l => l.Valor);

        var porCategoria = lancamentos
            .GroupBy(l => l.Categoria)
            .Select(g => new SaldoCategoriaDto(
                g.Key,
                g.Where(l => l.Tipo == TipoLancamento.Entrada).Sum(l => l.Valor),
                g.Where(l => l.Tipo == TipoLancamento.Saida).Sum(l => l.Valor)))
            .OrderByDescending(c => c.TotalEntradas)
            .ToList();

        return new SaldoMensalDto(ano, mes, totalEntradas, totalSaidas, totalEntradas - totalSaidas, porCategoria);
    }

    public async Task<LancamentoFinanceiroDto> AtualizarAsync(Guid usuarioId, Guid id, AtualizarLancamentoFinanceiroDto dto, CancellationToken ct)
    {
        var lancamento = await _lancamentoRepository.ObterPorIdAsync(usuarioId, id, ct)
            ?? throw new RecursoNaoEncontradoException("Lançamento financeiro não encontrado.");

        await ValidarReferenciasAsync(usuarioId, dto.ClienteId, dto.AgendamentoId, ct);

        lancamento.Tipo = dto.Tipo;
        lancamento.Categoria = dto.Categoria;
        lancamento.Valor = dto.Valor;
        lancamento.Data = dto.Data;
        lancamento.Descricao = dto.Descricao;
        lancamento.ClienteId = dto.ClienteId;
        lancamento.AgendamentoId = dto.AgendamentoId;

        await _lancamentoRepository.SalvarAlteracoesAsync(ct);

        return ParaDto(lancamento);
    }

    public async Task RemoverAsync(Guid usuarioId, Guid id, CancellationToken ct)
    {
        var lancamento = await _lancamentoRepository.ObterPorIdAsync(usuarioId, id, ct)
            ?? throw new RecursoNaoEncontradoException("Lançamento financeiro não encontrado.");

        _lancamentoRepository.Remover(lancamento);
        await _lancamentoRepository.SalvarAlteracoesAsync(ct);
    }

    private async Task ValidarReferenciasAsync(Guid usuarioId, Guid? clienteId, Guid? agendamentoId, CancellationToken ct)
    {
        if (clienteId is not null && await _clienteRepository.ObterPorIdAsync(usuarioId, clienteId.Value, ct) is null)
        {
            throw new RecursoNaoEncontradoException("Cliente não encontrado.");
        }

        if (agendamentoId is not null && await _agendamentoRepository.ObterPorIdAsync(usuarioId, agendamentoId.Value, ct) is null)
        {
            throw new RecursoNaoEncontradoException("Agendamento não encontrado.");
        }
    }

    private static LancamentoFinanceiroDto ParaDto(LancamentoFinanceiro lancamento) => new(
        lancamento.Id,
        lancamento.Tipo,
        lancamento.Categoria,
        lancamento.Valor,
        lancamento.Data,
        lancamento.Descricao,
        lancamento.ClienteId,
        lancamento.AgendamentoId,
        lancamento.CreatedAt);
}
