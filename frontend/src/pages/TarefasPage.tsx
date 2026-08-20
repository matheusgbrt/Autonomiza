import { useState, type FormEvent } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import * as tarefasApi from '../api/tarefas';
import type { ItemChecklistDto, TarefaDto } from '../api/types';
import { extractErrorMessage } from '../api/client';
import { dateOnlyInputToIso } from '../utils/date';
import { Button } from '../components/ui/Button';
import { Card } from '../components/ui/Card';
import { Input, Textarea } from '../components/ui/Input';
import { Modal } from '../components/ui/Modal';
import { PageHeader } from '../components/PageHeader';

interface FormState {
  titulo: string;
  descricao: string;
  dataVencimento: string;
  itensIniciais: string[];
}

const FORM_VAZIO: FormState = { titulo: '', descricao: '', dataVencimento: '', itensIniciais: [] };

export function TarefasPage() {
  const queryClient = useQueryClient();
  const { data: tarefas, isLoading } = useQuery({ queryKey: ['tarefas'], queryFn: tarefasApi.listarTarefas });

  const [modalAberto, setModalAberto] = useState(false);
  const [form, setForm] = useState<FormState>(FORM_VAZIO);
  const [novoItemTexto, setNovoItemTexto] = useState('');
  const [erro, setErro] = useState<string | null>(null);
  const [novoItemPorTarefa, setNovoItemPorTarefa] = useState<Record<string, string>>({});

  const invalidar = () => queryClient.invalidateQueries({ queryKey: ['tarefas'] });

  const criar = useMutation({
    mutationFn: () =>
      tarefasApi.criarTarefa({
        titulo: form.titulo,
        descricao: form.descricao || null,
        dataVencimento: form.dataVencimento ? dateOnlyInputToIso(form.dataVencimento) : null,
        itensIniciais: form.itensIniciais.length > 0 ? form.itensIniciais : null,
      }),
    onSuccess: () => {
      invalidar();
      setModalAberto(false);
    },
    onError: (error) => setErro(extractErrorMessage(error)),
  });

  const alternarConcluida = useMutation({
    mutationFn: (tarefa: TarefaDto) =>
      tarefasApi.atualizarTarefa(tarefa.id, {
        titulo: tarefa.titulo,
        descricao: tarefa.descricao,
        concluida: !tarefa.concluida,
        dataVencimento: tarefa.dataVencimento,
      }),
    onSuccess: invalidar,
  });

  const removerTarefa = useMutation({
    mutationFn: (id: string) => tarefasApi.removerTarefa(id),
    onSuccess: invalidar,
  });

  const alternarItem = useMutation({
    mutationFn: ({ tarefaId, item }: { tarefaId: string; item: ItemChecklistDto }) =>
      tarefasApi.atualizarItem(tarefaId, item.id, {
        descricao: item.descricao,
        concluido: !item.concluido,
        ordem: item.ordem,
      }),
    onSuccess: invalidar,
  });

  const removerItem = useMutation({
    mutationFn: ({ tarefaId, itemId }: { tarefaId: string; itemId: string }) => tarefasApi.removerItem(tarefaId, itemId),
    onSuccess: invalidar,
  });

  const adicionarItem = useMutation({
    mutationFn: ({ tarefaId, descricao }: { tarefaId: string; descricao: string }) =>
      tarefasApi.adicionarItem(tarefaId, { descricao }),
    onSuccess: (_data, variaveis) => {
      invalidar();
      setNovoItemPorTarefa((atual) => ({ ...atual, [variaveis.tarefaId]: '' }));
    },
  });

  function abrirNovo() {
    setForm(FORM_VAZIO);
    setNovoItemTexto('');
    setErro(null);
    setModalAberto(true);
  }

  function adicionarItemInicial() {
    if (!novoItemTexto.trim()) return;
    setForm({ ...form, itensIniciais: [...form.itensIniciais, novoItemTexto.trim()] });
    setNovoItemTexto('');
  }

  function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setErro(null);
    criar.mutate();
  }

  return (
    <div>
      <PageHeader
        title="Tarefas"
        subtitle="Lista de pendências e checklists operacionais"
        action={<Button onClick={abrirNovo}>Nova tarefa</Button>}
      />

      {isLoading ? (
        <p className="text-sm text-faint">Carregando…</p>
      ) : tarefas?.length === 0 ? (
        <p className="text-sm text-faint">Nenhuma tarefa cadastrada ainda.</p>
      ) : (
        <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
          {tarefas?.map((tarefa) => (
            <Card key={tarefa.id}>
              <div className="mb-3 flex items-start justify-between gap-2">
                <label className="flex cursor-pointer items-start gap-2">
                  <input
                    type="checkbox"
                    checked={tarefa.concluida}
                    onChange={() => alternarConcluida.mutate(tarefa)}
                    className="mt-1 h-4 w-4 cursor-pointer rounded border-stroke text-indigo"
                  />
                  <span>
                    <span className={`block font-semibold ${tarefa.concluida ? 'text-faint line-through' : 'text-ink'}`}>
                      {tarefa.titulo}
                    </span>
                    {tarefa.descricao && <span className="mt-0.5 block text-sm text-faint">{tarefa.descricao}</span>}
                    {tarefa.dataVencimento && (
                      <span className="mt-0.5 block text-xs text-faint">
                        Vence em {new Date(tarefa.dataVencimento).toLocaleDateString('pt-BR')}
                      </span>
                    )}
                  </span>
                </label>
                <Button variant="ghost" onClick={() => removerTarefa.mutate(tarefa.id)}>
                  ✕
                </Button>
              </div>

              {tarefa.itens.length > 0 && (
                <ul className="mb-3 space-y-1.5 border-t border-stroke pt-3">
                  {tarefa.itens.map((item) => (
                    <li key={item.id} className="flex items-center justify-between gap-2 text-sm">
                      <label className="flex flex-1 cursor-pointer items-center gap-2">
                        <input
                          type="checkbox"
                          checked={item.concluido}
                          onChange={() => alternarItem.mutate({ tarefaId: tarefa.id, item })}
                          className="h-3.5 w-3.5 cursor-pointer rounded border-stroke text-indigo"
                        />
                        <span className={item.concluido ? 'text-faint line-through' : 'text-muted'}>
                          {item.descricao}
                        </span>
                      </label>
                      <button
                        onClick={() => removerItem.mutate({ tarefaId: tarefa.id, itemId: item.id })}
                        className="cursor-pointer text-xs text-faint/60 hover:text-rose"
                      >
                        ✕
                      </button>
                    </li>
                  ))}
                </ul>
              )}

              <form
                onSubmit={(e) => {
                  e.preventDefault();
                  const texto = novoItemPorTarefa[tarefa.id]?.trim();
                  if (texto) adicionarItem.mutate({ tarefaId: tarefa.id, descricao: texto });
                }}
                className="flex gap-2"
              >
                <input
                  value={novoItemPorTarefa[tarefa.id] ?? ''}
                  onChange={(e) => setNovoItemPorTarefa((atual) => ({ ...atual, [tarefa.id]: e.target.value }))}
                  placeholder="Adicionar item ao checklist…"
                  className="flex-1 rounded-lg border border-stroke px-2 py-1 text-sm focus:border-indigo focus:outline-none"
                />
                <Button type="submit" variant="secondary" className="px-3 py-1 text-xs">
                  Adicionar
                </Button>
              </form>
            </Card>
          ))}
        </div>
      )}

      {modalAberto && (
        <Modal title="Nova tarefa" onClose={() => setModalAberto(false)}>
          <form onSubmit={handleSubmit} className="space-y-4">
            <Input label="Título" value={form.titulo} onChange={(e) => setForm({ ...form, titulo: e.target.value })} required />
            <Textarea
              label="Descrição"
              value={form.descricao}
              onChange={(e) => setForm({ ...form, descricao: e.target.value })}
              rows={2}
            />
            <Input
              label="Vencimento (opcional)"
              type="date"
              value={form.dataVencimento}
              onChange={(e) => setForm({ ...form, dataVencimento: e.target.value })}
            />

            <div>
              <span className="mb-1 block text-sm font-medium text-muted">Itens do checklist (opcional)</span>
              {form.itensIniciais.length > 0 && (
                <ul className="mb-2 space-y-1">
                  {form.itensIniciais.map((item, i) => (
                    <li key={i} className="flex items-center justify-between rounded bg-elevated px-2 py-1 text-sm">
                      <span>{item}</span>
                      <button
                        type="button"
                        onClick={() => setForm({ ...form, itensIniciais: form.itensIniciais.filter((_, idx) => idx !== i) })}
                        className="cursor-pointer text-faint hover:text-rose"
                      >
                        ✕
                      </button>
                    </li>
                  ))}
                </ul>
              )}
              <div className="flex gap-2">
                <input
                  value={novoItemTexto}
                  onChange={(e) => setNovoItemTexto(e.target.value)}
                  onKeyDown={(e) => {
                    if (e.key === 'Enter') {
                      e.preventDefault();
                      adicionarItemInicial();
                    }
                  }}
                  placeholder="Ex: Ligar ar-condicionado"
                  className="flex-1 rounded-lg border border-stroke px-3 py-2 text-sm focus:border-indigo focus:outline-none"
                />
                <Button type="button" variant="secondary" onClick={adicionarItemInicial}>
                  Adicionar
                </Button>
              </div>
            </div>

            {erro && <p className="text-sm text-rose">{erro}</p>}
            <div className="flex justify-end gap-2 pt-2">
              <Button type="button" variant="secondary" onClick={() => setModalAberto(false)}>
                Cancelar
              </Button>
              <Button type="submit" disabled={criar.isPending}>
                {criar.isPending ? 'Salvando…' : 'Salvar'}
              </Button>
            </div>
          </form>
        </Modal>
      )}
    </div>
  );
}
