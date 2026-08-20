using GestaoAutonomo.Application.DTOs.Integracao;
using GestaoAutonomo.Application.Exceptions;
using GestaoAutonomo.Application.Interfaces;
using GestaoAutonomo.Domain.Enums;

namespace GestaoAutonomo.Application.Services;

public class IntegracaoWhatsAppService : IIntegracaoWhatsAppService
{
    public const string ObservacaoAgendamentoViaWhatsApp = "Agendado via WhatsApp";

    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMensagemWhatsAppRepository _mensagemRepository;
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly IClienteRepository _clienteRepository;

    public IntegracaoWhatsAppService(
        IUsuarioRepository usuarioRepository,
        IMensagemWhatsAppRepository mensagemRepository,
        IAgendamentoRepository agendamentoRepository,
        IClienteRepository clienteRepository)
    {
        _usuarioRepository = usuarioRepository;
        _mensagemRepository = mensagemRepository;
        _agendamentoRepository = agendamentoRepository;
        _clienteRepository = clienteRepository;
    }

    public async Task ConfigurarAsync(Guid usuarioId, ConfigurarWhatsAppDto dto, CancellationToken ct)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId, ct)
            ?? throw new RecursoNaoEncontradoException("Usuário não encontrado.");

        var conflito = await _usuarioRepository.ObterPorZApiInstanceIdAsync(dto.InstanceId, ct);
        if (conflito is not null && conflito.Id != usuarioId)
        {
            throw new IntegracaoWhatsAppJaVinculadaException();
        }

        usuario.ZApiInstanceId = dto.InstanceId;
        usuario.ZApiToken = dto.Token;
        usuario.ZApiClientToken = dto.ClientToken;

        await _usuarioRepository.SalvarAlteracoesAsync(ct);
    }

    public async Task<StatusIntegracaoWhatsAppDto> ObterStatusAsync(Guid usuarioId, CancellationToken ct)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId, ct)
            ?? throw new RecursoNaoEncontradoException("Usuário não encontrado.");

        var conectado = !string.IsNullOrWhiteSpace(usuario.ZApiInstanceId) && !string.IsNullOrWhiteSpace(usuario.ZApiToken);
        return new StatusIntegracaoWhatsAppDto(conectado, usuario.ZApiInstanceId);
    }

    public async Task<EstatisticasWhatsAppDto> ObterEstatisticasAsync(Guid usuarioId, CancellationToken ct)
    {
        var hoje = DateTime.UtcNow.Date;
        var amanha = hoje.AddDays(1);
        var inicioDoMes = new DateTime(hoje.Year, hoje.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var proximoMes = inicioDoMes.AddMonths(1);

        var conversasHoje = await _mensagemRepository.ContarConversasUnicasAsync(usuarioId, hoje, amanha, ct);

        var agendamentos = await _agendamentoRepository.ListarAsync(usuarioId, ct);
        var agendamentosViaWhatsApp = agendamentos
            .Where(a => a.Observacoes == ObservacaoAgendamentoViaWhatsApp)
            .ToList();

        var agendamentosHoje = agendamentosViaWhatsApp.Count(a => a.CreatedAt >= hoje && a.CreatedAt < amanha);
        var agendamentosMes = agendamentosViaWhatsApp.Count(a => a.CreatedAt >= inicioDoMes && a.CreatedAt < proximoMes);

        var taxaConversao = conversasHoje > 0
            ? Math.Round(100m * agendamentosHoje / conversasHoje, 1)
            : 0m;

        return new EstatisticasWhatsAppDto(conversasHoje, agendamentosHoje, agendamentosMes, taxaConversao);
    }

    public async Task<IReadOnlyList<MensagemWhatsAppDto>> ObterUltimaConversaAsync(Guid usuarioId, CancellationToken ct)
    {
        var mensagens = await _mensagemRepository.ListarUltimaConversaAsync(usuarioId, 20, ct);
        if (mensagens.Count == 0) return Array.Empty<MensagemWhatsAppDto>();

        var clientes = (await _clienteRepository.ListarAsync(usuarioId, ct)).ToDictionary(c => c.Id, c => c.Nome);

        return mensagens
            .Select(m => new MensagemWhatsAppDto(
                m.ClienteId is not null ? clientes.GetValueOrDefault(m.ClienteId.Value) : null,
                m.Telefone,
                m.Direcao.ToString(),
                m.Conteudo,
                m.CreatedAt))
            .ToList();
    }

    public async Task<ConfiguracaoWhatsAppDto> ObterConfiguracaoAsync(Guid usuarioId, CancellationToken ct)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId, ct)
            ?? throw new RecursoNaoEncontradoException("Usuário não encontrado.");

        return new ConfiguracaoWhatsAppDto(
            usuario.WhatsAppRespostasAutomaticasAtivas,
            usuario.WhatsAppHorariosDisponiveisAtivo,
            usuario.WhatsAppConfirmarAgendamentosAtivo,
            usuario.WhatsAppLembretesAutomaticosAtivos,
            usuario.WhatsAppMensagemBoasVindas);
    }

    public async Task AtualizarConfiguracaoAsync(Guid usuarioId, AtualizarConfiguracaoWhatsAppDto dto, CancellationToken ct)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId, ct)
            ?? throw new RecursoNaoEncontradoException("Usuário não encontrado.");

        usuario.WhatsAppRespostasAutomaticasAtivas = dto.RespostasAutomaticas;
        usuario.WhatsAppHorariosDisponiveisAtivo = dto.HorariosDisponiveis;
        usuario.WhatsAppConfirmarAgendamentosAtivo = dto.ConfirmarAgendamentos;
        usuario.WhatsAppLembretesAutomaticosAtivos = dto.LembretesAutomaticos;
        usuario.WhatsAppMensagemBoasVindas = dto.MensagemBoasVindas;

        await _usuarioRepository.SalvarAlteracoesAsync(ct);
    }
}
