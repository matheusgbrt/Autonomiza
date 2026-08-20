using System.Globalization;
using GestaoAutonomo.Application.DTOs.Insight;
using GestaoAutonomo.Application.Interfaces;
using GestaoAutonomo.Domain.Entities;
using GestaoAutonomo.Domain.Enums;

namespace GestaoAutonomo.Application.Services;

public class AiConsultorService : IAiConsultorService
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(24);

    private readonly IInsightRepository _insightRepository;
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly ILancamentoFinanceiroRepository _lancamentoRepository;

    public AiConsultorService(
        IInsightRepository insightRepository,
        IAgendamentoRepository agendamentoRepository,
        ILancamentoFinanceiroRepository lancamentoRepository)
    {
        _insightRepository = insightRepository;
        _agendamentoRepository = agendamentoRepository;
        _lancamentoRepository = lancamentoRepository;
    }

    public async Task<InsightsResponseDto> ObterInsightsAsync(Guid usuarioId, CancellationToken ct)
    {
        var agora = DateTime.UtcNow;
        var vigentes = await _insightRepository.ObterVigentesAsync(usuarioId, agora, ct);

        if (vigentes.Count > 0)
        {
            return ParaDto(vigentes, doCache: true);
        }

        var agendamentos = await _agendamentoRepository.ListarAsync(usuarioId, ct);
        var lancamentos = await _lancamentoRepository.ListarAsync(usuarioId, ct);

        var expiraEm = agora.Add(CacheTtl);
        var novos = new List<InsightIA>
        {
            GerarInsightCancelamento(usuarioId, agendamentos, expiraEm),
            GerarInsightHorarioOcioso(usuarioId, agendamentos, expiraEm),
            GerarInsightTendenciaReceita(usuarioId, lancamentos, agora, expiraEm)
        };

        await _insightRepository.SubstituirAsync(usuarioId, novos, ct);

        return ParaDto(novos, doCache: false);
    }

    private static InsightIA GerarInsightCancelamento(Guid usuarioId, IReadOnlyList<Agendamento> agendamentos, DateTime expiraEm)
    {
        if (agendamentos.Count < 5)
        {
            return NovoInsight(usuarioId, CategoriaInsight.Cancelamento,
                "Ainda não há agendamentos suficientes para identificar padrões de cancelamento.", expiraEm);
        }

        var cancelados = agendamentos.Count(a => a.Status == StatusAgendamento.Cancelado);
        var taxa = 100m * cancelados / agendamentos.Count;

        var mensagem = taxa >= 20m
            ? $"{taxa.ToString("F1", CultureInfo.InvariantCulture)}% dos seus agendamentos foram cancelados. Considere enviar lembretes de confirmação com antecedência para reduzir faltas."
            : $"Sua taxa de cancelamento está saudável ({taxa.ToString("F1", CultureInfo.InvariantCulture)}%). Continue confirmando os agendamentos com os clientes.";

        return NovoInsight(usuarioId, CategoriaInsight.Cancelamento, mensagem, expiraEm);
    }

    private static InsightIA GerarInsightHorarioOcioso(Guid usuarioId, IReadOnlyList<Agendamento> agendamentos, DateTime expiraEm)
    {
        var validos = agendamentos.Where(a => a.Status != StatusAgendamento.Cancelado).ToList();

        if (validos.Count < 5)
        {
            return NovoInsight(usuarioId, CategoriaInsight.HorarioOcioso,
                "Ainda não há histórico suficiente para identificar horários ociosos.", expiraEm);
        }

        var porDiaDaSemana = validos
            .GroupBy(a => a.DataHoraInicio.DayOfWeek)
            .ToDictionary(g => g.Key, g => g.Count());

        var todosOsDias = Enum.GetValues<DayOfWeek>();
        var diaComMenosMovimento = todosOsDias
            .OrderBy(dia => porDiaDaSemana.GetValueOrDefault(dia, 0))
            .First();

        var nomeDia = NomeDiaSemanaPtBr(diaComMenosMovimento);
        var mensagem = $"{nomeDia} costuma ter menos agendamentos. Considere uma promoção ou lembrete extra para ocupar esse dia.";

        return NovoInsight(usuarioId, CategoriaInsight.HorarioOcioso, mensagem, expiraEm);
    }

    private static InsightIA GerarInsightTendenciaReceita(
        Guid usuarioId,
        IReadOnlyList<LancamentoFinanceiro> lancamentos,
        DateTime agora,
        DateTime expiraEm)
    {
        var fimPeriodoAtual = agora.Date.AddDays(1);
        var inicioPeriodoAtual = fimPeriodoAtual.AddDays(-30);
        var inicioPeriodoAnterior = inicioPeriodoAtual.AddDays(-30);

        var receitaAtual = lancamentos
            .Where(l => l.Tipo == TipoLancamento.Entrada && l.Data >= inicioPeriodoAtual && l.Data < fimPeriodoAtual)
            .Sum(l => l.Valor);

        var receitaAnterior = lancamentos
            .Where(l => l.Tipo == TipoLancamento.Entrada && l.Data >= inicioPeriodoAnterior && l.Data < inicioPeriodoAtual)
            .Sum(l => l.Valor);

        if (receitaAtual == 0 && receitaAnterior == 0)
        {
            return NovoInsight(usuarioId, CategoriaInsight.TendenciaReceita,
                "Ainda não há lançamentos financeiros suficientes para calcular sua tendência de receita.", expiraEm);
        }

        if (receitaAnterior == 0)
        {
            return NovoInsight(usuarioId, CategoriaInsight.TendenciaReceita,
                $"Você faturou {receitaAtual.ToString("C2", CultureInfo.GetCultureInfo("pt-BR"))} nos últimos 30 dias, sem histórico anterior para comparação.", expiraEm);
        }

        var variacao = 100m * (receitaAtual - receitaAnterior) / receitaAnterior;
        var direcao = variacao switch
        {
            > 1m => "subiu",
            < -1m => "caiu",
            _ => "ficou estável"
        };

        var mensagem = direcao == "ficou estável"
            ? "Sua receita ficou estável em relação aos 30 dias anteriores."
            : $"Sua receita {direcao} {Math.Abs(variacao).ToString("F1", CultureInfo.InvariantCulture)}% em relação aos 30 dias anteriores.";

        return NovoInsight(usuarioId, CategoriaInsight.TendenciaReceita, mensagem, expiraEm);
    }

    private static string NomeDiaSemanaPtBr(DayOfWeek dia) => dia switch
    {
        DayOfWeek.Sunday => "Domingo",
        DayOfWeek.Monday => "Segunda-feira",
        DayOfWeek.Tuesday => "Terça-feira",
        DayOfWeek.Wednesday => "Quarta-feira",
        DayOfWeek.Thursday => "Quinta-feira",
        DayOfWeek.Friday => "Sexta-feira",
        DayOfWeek.Saturday => "Sábado",
        _ => dia.ToString()
    };

    private static InsightIA NovoInsight(Guid usuarioId, CategoriaInsight categoria, string mensagem, DateTime expiraEm) => new()
    {
        UsuarioId = usuarioId,
        Categoria = categoria,
        Mensagem = mensagem,
        ExpiraEm = expiraEm
    };

    private static InsightsResponseDto ParaDto(IReadOnlyList<InsightIA> insights, bool doCache)
    {
        var geradoEm = insights.Min(i => i.CreatedAt);
        var expiraEm = insights.Min(i => i.ExpiraEm);

        return new InsightsResponseDto(
            insights.Select(i => new InsightDto(i.Categoria, i.Mensagem)).ToList(),
            geradoEm,
            expiraEm,
            doCache);
    }
}
