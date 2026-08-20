using GestaoAutonomo.Application.DTOs.Dashboard;
using GestaoAutonomo.Application.Interfaces;
using GestaoAutonomo.Domain.Enums;

namespace GestaoAutonomo.Application.Services;

public class DashboardService : IDashboardService
{
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

    public async Task<DashboardAvancadoDto> ObterAvancadoAsync(Guid usuarioId, CancellationToken ct)
    {
        var fimExclusivo = DateTime.UtcNow.Date.AddDays(1);
        var inicioUltimos30Dias = fimExclusivo.AddDays(-30);

        var lancamentos = await _lancamentoRepository.ListarAsync(usuarioId, ct);
        var agendamentos = await _agendamentoRepository.ListarAsync(usuarioId, ct);
        var servicos = await _servicoRepository.ListarAsync(usuarioId, ct);

        var servicoPorAgendamento = agendamentos.ToDictionary(a => a.Id, a => a.ServicoId);
        var nomeServico = servicos.ToDictionary(s => s.Id, s => s.Nome);

        var entradasVinculadas = lancamentos
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

        var clientesComAgendamento = agendamentos
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

        return new DashboardAvancadoDto(rentabilidadePorServico, taxaFidelizacao, entradasUltimos30Dias);
    }
}
