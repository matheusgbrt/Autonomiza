using GestaoAutonomo.Application.DTOs.Tarefa;
using GestaoAutonomo.Application.Exceptions;
using GestaoAutonomo.Application.Interfaces;
using GestaoAutonomo.Domain.Entities;

namespace GestaoAutonomo.Application.Services;

public class TarefaService : ITarefaService
{
    private readonly ITarefaRepository _tarefaRepository;

    public TarefaService(ITarefaRepository tarefaRepository)
    {
        _tarefaRepository = tarefaRepository;
    }

    public async Task<TarefaDto> CriarAsync(Guid usuarioId, CriarTarefaDto dto, CancellationToken ct)
    {
        var tarefa = new Tarefa
        {
            UsuarioId = usuarioId,
            Titulo = dto.Titulo,
            Descricao = dto.Descricao,
            DataVencimento = dto.DataVencimento
        };

        await _tarefaRepository.AdicionarAsync(tarefa, ct);
        await _tarefaRepository.SalvarAlteracoesAsync(ct);

        return ParaDto(tarefa);
    }

    public async Task<TarefaDto?> ObterPorIdAsync(Guid usuarioId, Guid id, CancellationToken ct)
    {
        var tarefa = await _tarefaRepository.ObterPorIdAsync(usuarioId, id, ct);
        return tarefa is null ? null : ParaDto(tarefa);
    }

    public async Task<IReadOnlyList<TarefaDto>> ListarAsync(Guid usuarioId, CancellationToken ct)
    {
        var tarefas = await _tarefaRepository.ListarAsync(usuarioId, ct);
        return tarefas.Select(ParaDto).ToList();
    }

    public async Task<TarefaDto> AtualizarAsync(Guid usuarioId, Guid id, AtualizarTarefaDto dto, CancellationToken ct)
    {
        var tarefa = await _tarefaRepository.ObterPorIdAsync(usuarioId, id, ct)
            ?? throw new RecursoNaoEncontradoException("Tarefa não encontrada.");

        tarefa.Titulo = dto.Titulo;
        tarefa.Descricao = dto.Descricao;
        tarefa.Concluida = dto.Concluida;
        tarefa.DataVencimento = dto.DataVencimento;

        await _tarefaRepository.SalvarAlteracoesAsync(ct);

        return ParaDto(tarefa);
    }

    public async Task RemoverAsync(Guid usuarioId, Guid id, CancellationToken ct)
    {
        var tarefa = await _tarefaRepository.ObterPorIdAsync(usuarioId, id, ct)
            ?? throw new RecursoNaoEncontradoException("Tarefa não encontrada.");

        _tarefaRepository.Remover(tarefa);
        await _tarefaRepository.SalvarAlteracoesAsync(ct);
    }

    private static TarefaDto ParaDto(Tarefa tarefa) => new(
        tarefa.Id,
        tarefa.Titulo,
        tarefa.Descricao,
        tarefa.Concluida,
        tarefa.DataVencimento,
        tarefa.CreatedAt);
}
