using GestaoAutonomo.Application.DTOs.Tarefa;
using GestaoAutonomo.Application.Exceptions;
using GestaoAutonomo.Application.Interfaces;
using GestaoAutonomo.Domain.Entities;

namespace GestaoAutonomo.Application.Services;

public class TarefaService : ITarefaService
{
    private readonly ITarefaRepository _tarefaRepository;
    private readonly IItemChecklistRepository _itemChecklistRepository;

    public TarefaService(ITarefaRepository tarefaRepository, IItemChecklistRepository itemChecklistRepository)
    {
        _tarefaRepository = tarefaRepository;
        _itemChecklistRepository = itemChecklistRepository;
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

        var itens = new List<ItemChecklist>();
        if (dto.ItensIniciais is { Count: > 0 })
        {
            for (var i = 0; i < dto.ItensIniciais.Count; i++)
            {
                var item = new ItemChecklist { TarefaId = tarefa.Id, Descricao = dto.ItensIniciais[i], Ordem = i };
                itens.Add(item);
                await _itemChecklistRepository.AdicionarAsync(item, ct);
            }
            await _itemChecklistRepository.SalvarAlteracoesAsync(ct);
        }

        return ParaDto(tarefa, itens);
    }

    public async Task<TarefaDto?> ObterPorIdAsync(Guid usuarioId, Guid id, CancellationToken ct)
    {
        var tarefa = await _tarefaRepository.ObterPorIdAsync(usuarioId, id, ct);
        if (tarefa is null) return null;

        var itens = await _itemChecklistRepository.ListarPorTarefaAsync(tarefa.Id, ct);
        return ParaDto(tarefa, itens);
    }

    public async Task<IReadOnlyList<TarefaDto>> ListarAsync(Guid usuarioId, CancellationToken ct)
    {
        var tarefas = await _tarefaRepository.ListarAsync(usuarioId, ct);
        if (tarefas.Count == 0) return Array.Empty<TarefaDto>();

        var itens = await _itemChecklistRepository.ListarPorTarefasAsync(tarefas.Select(t => t.Id).ToList(), ct);
        var itensPorTarefa = itens.GroupBy(i => i.TarefaId).ToDictionary(g => g.Key, g => g.ToList());

        return tarefas
            .Select(t => ParaDto(t, itensPorTarefa.GetValueOrDefault(t.Id, new List<ItemChecklist>())))
            .ToList();
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

        var itens = await _itemChecklistRepository.ListarPorTarefaAsync(tarefa.Id, ct);
        return ParaDto(tarefa, itens);
    }

    public async Task RemoverAsync(Guid usuarioId, Guid id, CancellationToken ct)
    {
        var tarefa = await _tarefaRepository.ObterPorIdAsync(usuarioId, id, ct)
            ?? throw new RecursoNaoEncontradoException("Tarefa não encontrada.");

        _tarefaRepository.Remover(tarefa);
        await _tarefaRepository.SalvarAlteracoesAsync(ct);
    }

    public async Task<ItemChecklistDto> AdicionarItemAsync(Guid usuarioId, Guid tarefaId, CriarItemChecklistDto dto, CancellationToken ct)
    {
        await ObterTarefaDoUsuarioAsync(usuarioId, tarefaId, ct);

        var proximaOrdem = await _itemChecklistRepository.ContarPorTarefaAsync(tarefaId, ct);
        var item = new ItemChecklist { TarefaId = tarefaId, Descricao = dto.Descricao, Ordem = proximaOrdem };

        await _itemChecklistRepository.AdicionarAsync(item, ct);
        await _itemChecklistRepository.SalvarAlteracoesAsync(ct);

        return ParaItemDto(item);
    }

    public async Task<ItemChecklistDto> AtualizarItemAsync(Guid usuarioId, Guid tarefaId, Guid itemId, AtualizarItemChecklistDto dto, CancellationToken ct)
    {
        await ObterTarefaDoUsuarioAsync(usuarioId, tarefaId, ct);

        var item = await _itemChecklistRepository.ObterPorIdAsync(tarefaId, itemId, ct)
            ?? throw new RecursoNaoEncontradoException("Item do checklist não encontrado.");

        item.Descricao = dto.Descricao;
        item.Concluido = dto.Concluido;
        item.Ordem = dto.Ordem;

        await _itemChecklistRepository.SalvarAlteracoesAsync(ct);

        return ParaItemDto(item);
    }

    public async Task RemoverItemAsync(Guid usuarioId, Guid tarefaId, Guid itemId, CancellationToken ct)
    {
        await ObterTarefaDoUsuarioAsync(usuarioId, tarefaId, ct);

        var item = await _itemChecklistRepository.ObterPorIdAsync(tarefaId, itemId, ct)
            ?? throw new RecursoNaoEncontradoException("Item do checklist não encontrado.");

        _itemChecklistRepository.Remover(item);
        await _itemChecklistRepository.SalvarAlteracoesAsync(ct);
    }

    private async Task<Tarefa> ObterTarefaDoUsuarioAsync(Guid usuarioId, Guid tarefaId, CancellationToken ct) =>
        await _tarefaRepository.ObterPorIdAsync(usuarioId, tarefaId, ct)
            ?? throw new RecursoNaoEncontradoException("Tarefa não encontrada.");

    private static TarefaDto ParaDto(Tarefa tarefa, IReadOnlyList<ItemChecklist> itens) => new(
        tarefa.Id,
        tarefa.Titulo,
        tarefa.Descricao,
        tarefa.Concluida,
        tarefa.DataVencimento,
        itens.OrderBy(i => i.Ordem).Select(ParaItemDto).ToList(),
        tarefa.CreatedAt);

    private static ItemChecklistDto ParaItemDto(ItemChecklist item) => new(
        item.Id,
        item.Descricao,
        item.Concluido,
        item.Ordem);
}
