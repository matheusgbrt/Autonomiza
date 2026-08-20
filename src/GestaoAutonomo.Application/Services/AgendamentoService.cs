using GestaoAutonomo.Application.DTOs.Agendamento;
using GestaoAutonomo.Application.Exceptions;
using GestaoAutonomo.Application.Interfaces;
using GestaoAutonomo.Domain.Entities;

namespace GestaoAutonomo.Application.Services;

public class AgendamentoService : IAgendamentoService
{
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IServicoRepository _servicoRepository;

    public AgendamentoService(
        IAgendamentoRepository agendamentoRepository,
        IClienteRepository clienteRepository,
        IServicoRepository servicoRepository)
    {
        _agendamentoRepository = agendamentoRepository;
        _clienteRepository = clienteRepository;
        _servicoRepository = servicoRepository;
    }

    public async Task<AgendamentoDto> CriarAsync(Guid usuarioId, CriarAgendamentoDto dto, CancellationToken ct)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(usuarioId, dto.ClienteId, ct)
            ?? throw new RecursoNaoEncontradoException("Cliente não encontrado.");
        var servico = await _servicoRepository.ObterPorIdAsync(usuarioId, dto.ServicoId, ct)
            ?? throw new RecursoNaoEncontradoException("Serviço não encontrado.");

        var dataHoraFim = dto.DataHoraInicio.Add(servico.Duracao);

        if (await _agendamentoRepository.ExisteConflitoAsync(usuarioId, dto.DataHoraInicio, dataHoraFim, null, ct))
        {
            throw new AgendamentoConflitanteException();
        }

        var agendamento = new Agendamento
        {
            UsuarioId = usuarioId,
            ClienteId = cliente.Id,
            ServicoId = servico.Id,
            DataHoraInicio = dto.DataHoraInicio,
            DataHoraFim = dataHoraFim,
            Observacoes = dto.Observacoes
        };

        await _agendamentoRepository.AdicionarAsync(agendamento, ct);
        await _agendamentoRepository.SalvarAlteracoesAsync(ct);

        return ParaDto(agendamento, cliente.Nome, servico.Nome);
    }

    public async Task<AgendamentoDto?> ObterPorIdAsync(Guid usuarioId, Guid id, CancellationToken ct)
    {
        var agendamento = await _agendamentoRepository.ObterPorIdAsync(usuarioId, id, ct);
        if (agendamento is null) return null;

        var cliente = await _clienteRepository.ObterPorIdAsync(usuarioId, agendamento.ClienteId, ct);
        var servico = await _servicoRepository.ObterPorIdAsync(usuarioId, agendamento.ServicoId, ct);

        return ParaDto(agendamento, cliente?.Nome ?? "—", servico?.Nome ?? "—");
    }

    public async Task<IReadOnlyList<AgendamentoDto>> ListarAsync(Guid usuarioId, DateTime? inicio, DateTime? fim, CancellationToken ct)
    {
        var agendamentos = inicio is not null && fim is not null
            ? await _agendamentoRepository.ListarPorPeriodoAsync(usuarioId, inicio.Value, fim.Value, ct)
            : await _agendamentoRepository.ListarAsync(usuarioId, ct);

        if (agendamentos.Count == 0) return Array.Empty<AgendamentoDto>();

        var clientes = (await _clienteRepository.ListarAsync(usuarioId, ct)).ToDictionary(c => c.Id, c => c.Nome);
        var servicos = (await _servicoRepository.ListarAsync(usuarioId, ct)).ToDictionary(s => s.Id, s => s.Nome);

        return agendamentos
            .Select(a => ParaDto(a, clientes.GetValueOrDefault(a.ClienteId, "—"), servicos.GetValueOrDefault(a.ServicoId, "—")))
            .ToList();
    }

    public async Task<IReadOnlyList<AgendamentoDto>> ListarPorClienteAsync(Guid usuarioId, Guid clienteId, CancellationToken ct)
    {
        var agendamentos = await _agendamentoRepository.ListarPorClienteAsync(usuarioId, clienteId, ct);
        if (agendamentos.Count == 0) return Array.Empty<AgendamentoDto>();

        var cliente = await _clienteRepository.ObterPorIdAsync(usuarioId, clienteId, ct);
        var servicos = (await _servicoRepository.ListarAsync(usuarioId, ct)).ToDictionary(s => s.Id, s => s.Nome);

        return agendamentos
            .Select(a => ParaDto(a, cliente?.Nome ?? "—", servicos.GetValueOrDefault(a.ServicoId, "—")))
            .ToList();
    }

    public async Task<AgendamentoDto> AtualizarAsync(Guid usuarioId, Guid id, AtualizarAgendamentoDto dto, CancellationToken ct)
    {
        var agendamento = await _agendamentoRepository.ObterPorIdAsync(usuarioId, id, ct)
            ?? throw new RecursoNaoEncontradoException("Agendamento não encontrado.");

        var cliente = await _clienteRepository.ObterPorIdAsync(usuarioId, dto.ClienteId, ct)
            ?? throw new RecursoNaoEncontradoException("Cliente não encontrado.");
        var servico = await _servicoRepository.ObterPorIdAsync(usuarioId, dto.ServicoId, ct)
            ?? throw new RecursoNaoEncontradoException("Serviço não encontrado.");

        var dataHoraFim = dto.DataHoraInicio.Add(servico.Duracao);

        if (await _agendamentoRepository.ExisteConflitoAsync(usuarioId, dto.DataHoraInicio, dataHoraFim, agendamento.Id, ct))
        {
            throw new AgendamentoConflitanteException();
        }

        agendamento.ClienteId = cliente.Id;
        agendamento.ServicoId = servico.Id;
        agendamento.DataHoraInicio = dto.DataHoraInicio;
        agendamento.DataHoraFim = dataHoraFim;
        agendamento.Status = dto.Status;
        agendamento.Observacoes = dto.Observacoes;
        agendamento.NotaAtendimento = dto.NotaAtendimento;

        await _agendamentoRepository.SalvarAlteracoesAsync(ct);

        return ParaDto(agendamento, cliente.Nome, servico.Nome);
    }

    public async Task RemoverAsync(Guid usuarioId, Guid id, CancellationToken ct)
    {
        var agendamento = await _agendamentoRepository.ObterPorIdAsync(usuarioId, id, ct)
            ?? throw new RecursoNaoEncontradoException("Agendamento não encontrado.");

        _agendamentoRepository.Remover(agendamento);
        await _agendamentoRepository.SalvarAlteracoesAsync(ct);
    }

    private static AgendamentoDto ParaDto(Agendamento agendamento, string clienteNome, string servicoNome) => new(
        agendamento.Id,
        agendamento.ClienteId,
        clienteNome,
        agendamento.ServicoId,
        servicoNome,
        agendamento.DataHoraInicio,
        agendamento.DataHoraFim,
        agendamento.Status,
        agendamento.Observacoes,
        agendamento.NotaAtendimento,
        agendamento.CreatedAt);
}
