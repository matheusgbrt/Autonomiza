using System.Globalization;
using GestaoAutonomo.Application.DTOs.Recomendacao;
using GestaoAutonomo.Application.Interfaces;
using GestaoAutonomo.Domain.Entities;
using GestaoAutonomo.Domain.Enums;

namespace GestaoAutonomo.Application.Services;

public class RecomendacaoService : IRecomendacaoService
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(24);

    private readonly IRecomendacaoRepository _recomendacaoRepository;
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly IServicoRepository _servicoRepository;

    public RecomendacaoService(
        IRecomendacaoRepository recomendacaoRepository,
        IAgendamentoRepository agendamentoRepository,
        IServicoRepository servicoRepository)
    {
        _recomendacaoRepository = recomendacaoRepository;
        _agendamentoRepository = agendamentoRepository;
        _servicoRepository = servicoRepository;
    }

    public async Task<RecomendacoesResponseDto> ObterRecomendacoesAsync(Guid usuarioId, CancellationToken ct)
    {
        var agora = DateTime.UtcNow;
        var vigentes = await _recomendacaoRepository.ObterVigentesAsync(usuarioId, agora, ct);

        if (vigentes.Count > 0)
        {
            return ParaDto(vigentes, doCache: true);
        }

        var agendamentos = await _agendamentoRepository.ListarAsync(usuarioId, ct);
        var servicos = await _servicoRepository.ListarAsync(usuarioId, ct);

        var expiraEm = agora.Add(CacheTtl);
        var novas = new List<RecomendacaoIA>
        {
            GerarRecomendacaoPacote(usuarioId, agendamentos, servicos, expiraEm),
            GerarRecomendacaoHorarioPico(usuarioId, agendamentos, expiraEm)
        };

        await _recomendacaoRepository.SubstituirAsync(usuarioId, novas, ct);

        return ParaDto(novas, doCache: false);
    }

    private static RecomendacaoIA GerarRecomendacaoPacote(
        Guid usuarioId, IReadOnlyList<Agendamento> agendamentos, IReadOnlyList<Servico> servicos, DateTime expiraEm)
    {
        var validos = agendamentos.Where(a => a.Status != StatusAgendamento.Cancelado).ToList();

        if (validos.Count < 5 || servicos.Count < 2)
        {
            return NovaRecomendacao(usuarioId, CategoriaRecomendacao.PacoteSugerido,
                "Ainda não há dados suficientes (histórico de agendamentos ou variedade de serviços) para sugerir um pacote.", expiraEm);
        }

        var nomePorServico = servicos.ToDictionary(s => s.Id, s => s.Nome);

        var maisRecorrente = validos
            .GroupBy(a => a.ServicoId)
            .Select(g => new { ServicoId = g.Key, Quantidade = g.Count() })
            .OrderByDescending(x => x.Quantidade)
            .First();

        var candidatoParaPacote = servicos
            .Where(s => s.Id != maisRecorrente.ServicoId)
            .OrderByDescending(s => s.ValorPadrao)
            .FirstOrDefault();

        if (candidatoParaPacote is null)
        {
            return NovaRecomendacao(usuarioId, CategoriaRecomendacao.PacoteSugerido,
                "Ainda não há outro serviço cadastrado para compor um pacote.", expiraEm);
        }

        var nomeRecorrente = nomePorServico.GetValueOrDefault(maisRecorrente.ServicoId, "—");
        var mensagem = $"Seu serviço mais recorrente é \"{nomeRecorrente}\" ({maisRecorrente.Quantidade} atendimentos). " +
                        $"Considere criar um pacote combinando com \"{candidatoParaPacote.Nome}\" para aumentar o ticket médio " +
                        "dos clientes que já confiam nesse serviço.";

        return NovaRecomendacao(usuarioId, CategoriaRecomendacao.PacoteSugerido, mensagem, expiraEm);
    }

    private static RecomendacaoIA GerarRecomendacaoHorarioPico(Guid usuarioId, IReadOnlyList<Agendamento> agendamentos, DateTime expiraEm)
    {
        var validos = agendamentos.Where(a => a.Status != StatusAgendamento.Cancelado).ToList();

        if (validos.Count < 5)
        {
            return NovaRecomendacao(usuarioId, CategoriaRecomendacao.OtimizacaoHorarioPico,
                "Ainda não há histórico suficiente para identificar seu horário de pico.", expiraEm);
        }

        var pico = validos
            .GroupBy(a => a.DataHoraInicio.Hour)
            .Select(g => new { Hora = g.Key, Quantidade = g.Count() })
            .OrderByDescending(x => x.Quantidade)
            .First();

        var mensagem = $"Seu horário de pico é entre {pico.Hora.ToString(CultureInfo.InvariantCulture)}h e " +
                        $"{(pico.Hora + 1).ToString(CultureInfo.InvariantCulture)}h ({pico.Quantidade} atendimentos). " +
                        "Considere ajustar preços nesse horário ou promover os horários menos movimentados para equilibrar sua agenda.";

        return NovaRecomendacao(usuarioId, CategoriaRecomendacao.OtimizacaoHorarioPico, mensagem, expiraEm);
    }

    private static RecomendacaoIA NovaRecomendacao(Guid usuarioId, CategoriaRecomendacao categoria, string mensagem, DateTime expiraEm) => new()
    {
        UsuarioId = usuarioId,
        Categoria = categoria,
        Mensagem = mensagem,
        ExpiraEm = expiraEm
    };

    private static RecomendacoesResponseDto ParaDto(IReadOnlyList<RecomendacaoIA> recomendacoes, bool doCache)
    {
        var geradoEm = recomendacoes.Min(r => r.CreatedAt);
        var expiraEm = recomendacoes.Min(r => r.ExpiraEm);

        return new RecomendacoesResponseDto(
            recomendacoes.Select(r => new RecomendacaoDto(r.Categoria, r.Mensagem)).ToList(),
            geradoEm,
            expiraEm,
            doCache);
    }
}
