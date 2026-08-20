using GestaoAutonomo.Application.DTOs.Dashboard;
using GestaoAutonomo.Application.Interfaces;
using GestaoAutonomo.Domain.Entities;
using GestaoAutonomo.Domain.Enums;

namespace GestaoAutonomo.Application.Services;

public class DashboardService : IDashboardService
{
    private const int MaximoTrimestresExibidos = 8;

    private readonly ILancamentoFinanceiroRepository _lancamentoRepository;
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly IServicoRepository _servicoRepository;

    public DashboardService(
        ILancamentoFinanceiroRepository lancamentoRepository,
        IAgendamentoRepository agendamentoRepository,
        IServicoRepository servicoRepository)
    {
        _lancamentoRepository = lancamentoRepository;
        _agendamentoRepository = agendamentoRepository;
        _servicoRepository = servicoRepository;
    }

    public async Task<ResumoDashboardDto> ObterResumoAsync(Guid usuarioId, CancellationToken ct)
    {
        var fimExclusivo = DateTime.UtcNow.Date.AddDays(1);
        var inicio = fimExclusivo.AddDays(-30);

        var lancamentos = await _lancamentoRepository.ListarEntrePeriodoAsync(usuarioId, inicio, fimExclusivo, ct);
        var entradas = lancamentos.Where(l => l.Tipo == TipoLancamento.Entrada).ToList();

        var totalVendas = entradas.Sum(l => l.Valor);
        var quantidadeVendas = entradas.Count;
        var ticketMedio = quantidadeVendas > 0 ? totalVendas / quantidadeVendas : 0m;

        var serieDiaria = Enumerable.Range(0, 30)
            .Select(offset => inicio.AddDays(offset))
            .Select(dia => new PontoSerieDto(
                DateOnly.FromDateTime(dia),
                entradas.Where(l => l.Data.Date == dia.Date).Sum(l => l.Valor)))
            .ToList();

        return new ResumoDashboardDto(totalVendas, ticketMedio, quantidadeVendas, serieDiaria);
    }

    public async Task<DashboardAvancadoDto> ObterAvancadoAsync(Guid usuarioId, DateOnly? inicio, DateOnly? fim, CancellationToken ct)
    {
        var fimExclusivo = DateTime.UtcNow.Date.AddDays(1);
        var inicioUltimos30Dias = fimExclusivo.AddDays(-30);

        var lancamentos = await _lancamentoRepository.ListarAsync(usuarioId, ct);
        var agendamentos = await _agendamentoRepository.ListarAsync(usuarioId, ct);
        var servicos = await _servicoRepository.ListarAsync(usuarioId, ct);

        var janelaInicio = inicio?.ToDateTime(TimeOnly.MinValue);
        var janelaFimExclusivo = fim?.ToDateTime(TimeOnly.MinValue).AddDays(1);

        var lancamentosNaJanela = janelaInicio is not null && janelaFimExclusivo is not null
            ? lancamentos.Where(l => l.Data >= janelaInicio && l.Data < janelaFimExclusivo).ToList()
            : lancamentos;

        var agendamentosNaJanela = janelaInicio is not null && janelaFimExclusivo is not null
            ? agendamentos.Where(a => a.DataHoraInicio >= janelaInicio && a.DataHoraInicio < janelaFimExclusivo).ToList()
            : agendamentos;

        var servicoPorAgendamento = agendamentos.ToDictionary(a => a.Id, a => a.ServicoId);
        var nomeServico = servicos.ToDictionary(s => s.Id, s => s.Nome);

        var entradasVinculadas = lancamentosNaJanela
            .Where(l => l.Tipo == TipoLancamento.Entrada && l.AgendamentoId is not null)
            .Where(l => servicoPorAgendamento.ContainsKey(l.AgendamentoId!.Value))
            .ToList();

        var rentabilidadePorServico = entradasVinculadas
            .GroupBy(l => servicoPorAgendamento[l.AgendamentoId!.Value])
            .Select(g => new RentabilidadeServicoDto(
                g.Key,
                nomeServico.GetValueOrDefault(g.Key, "—"),
                g.Sum(l => l.Valor),
                g.Count(),
                g.Sum(l => l.Valor) / g.Count()))
            .OrderByDescending(r => r.ReceitaTotal)
            .ToList();

        var clientesComAgendamento = agendamentosNaJanela
            .Where(a => a.Status != StatusAgendamento.Cancelado)
            .GroupBy(a => a.ClienteId)
            .ToList();

        var totalClientesAtendidos = clientesComAgendamento.Count;
        var clientesRecorrentes = clientesComAgendamento.Count(g => g.Count() > 1);
        var taxaFidelizacao = totalClientesAtendidos > 0
            ? Math.Round(100m * clientesRecorrentes / totalClientesAtendidos, 2)
            : 0m;

        var entradasUltimos30Dias = lancamentos
            .Where(l => l.Tipo == TipoLancamento.Entrada && l.Data >= inicioUltimos30Dias && l.Data < fimExclusivo)
            .Sum(l => l.Valor);

        var satisfacaoMedia = CalcularSatisfacaoMedia(agendamentosNaJanela);
        var crescimentoTrimestral = CalcularCrescimentoTrimestral(lancamentos);
        var taxaConclusao = CalcularTaxaConclusao(agendamentosNaJanela);
        var scoreCrescimento = CalcularScoreCrescimento(crescimentoTrimestral);

        var healthScore = (int)Math.Round(
            0.4m * taxaFidelizacao +
            0.3m * taxaConclusao +
            0.3m * scoreCrescimento);

        return new DashboardAvancadoDto(
            rentabilidadePorServico,
            taxaFidelizacao,
            entradasUltimos30Dias,
            Math.Clamp(healthScore, 0, 100),
            satisfacaoMedia,
            crescimentoTrimestral);
    }

    private static decimal? CalcularSatisfacaoMedia(IReadOnlyList<Agendamento> agendamentos)
    {
        var notas = agendamentos
            .Where(a => a.Status == StatusAgendamento.Concluido && a.NotaAtendimento is not null)
            .Select(a => a.NotaAtendimento!.Value)
            .ToList();

        return notas.Count > 0 ? Math.Round((decimal)notas.Average(), 1) : null;
    }

    private static decimal CalcularTaxaConclusao(IReadOnlyList<Agendamento> agendamentos)
    {
        var finalizados = agendamentos.Count(a => a.Status is StatusAgendamento.Concluido or StatusAgendamento.Cancelado);
        if (finalizados == 0) return 100m;

        var concluidos = agendamentos.Count(a => a.Status == StatusAgendamento.Concluido);
        return Math.Round(100m * concluidos / finalizados, 2);
    }

    private static IReadOnlyList<PontoTrimestralDto> CalcularCrescimentoTrimestral(IReadOnlyList<LancamentoFinanceiro> lancamentos)
    {
        return lancamentos
            .Where(l => l.Tipo == TipoLancamento.Entrada)
            .GroupBy(l => (Ano: l.Data.Year, Trimestre: (l.Data.Month - 1) / 3 + 1))
            .OrderBy(g => g.Key.Ano).ThenBy(g => g.Key.Trimestre)
            .Select(g => new PontoTrimestralDto(g.Key.Ano, g.Key.Trimestre, g.Sum(l => l.Valor)))
            .TakeLast(MaximoTrimestresExibidos)
            .ToList();
    }

    private static decimal CalcularScoreCrescimento(IReadOnlyList<PontoTrimestralDto> trimestres)
    {
        if (trimestres.Count < 2) return 50m;

        var atual = trimestres[^1].Total;
        var anterior = trimestres[^2].Total;

        if (anterior == 0) return atual > 0 ? 100m : 50m;

        var variacaoPercentual = (atual - anterior) / anterior * 100m;
        var variacaoLimitada = Math.Clamp(variacaoPercentual, -50m, 50m);
        return 50m + variacaoLimitada;
    }
}
