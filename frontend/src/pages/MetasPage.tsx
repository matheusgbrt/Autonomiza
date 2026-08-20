import { useState, type FormEvent } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import * as metasApi from '../api/metas';
import { TipoMeta } from '../api/types';
import { extractErrorMessage } from '../api/client';
import { Button } from '../components/ui/Button';
import { Card } from '../components/ui/Card';
import { Input, Select } from '../components/ui/Input';
import { Modal } from '../components/ui/Modal';
import { PageHeader } from '../components/PageHeader';
import { ProgressBar } from '../components/ui/ProgressBar';
import { Badge } from '../components/ui/Badge';
import { dateOnlyInputToIso, toDateInputValue } from '../utils/date';

function formatarValor(tipo: TipoMeta, valor: number) {
  return tipo === TipoMeta.Faturamento
    ? valor.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })
    : `${valor} atendimento${valor === 1 ? '' : 's'}`;
}

interface FormState {
  tipo: TipoMeta;
  titulo: string;
  valorAlvo: string;
  periodoInicio: string;
  periodoFim: string;
}

const hoje = new Date();
const primeiroDiaMes = toDateInputValue(new Date(hoje.getFullYear(), hoje.getMonth(), 1));
const ultimoDiaMes = toDateInputValue(new Date(hoje.getFullYear(), hoje.getMonth() + 1, 0));

const FORM_VAZIO: FormState = {
  tipo: TipoMeta.Faturamento,
  titulo: '',
  valorAlvo: '',
  periodoInicio: primeiroDiaMes,
  periodoFim: ultimoDiaMes,
};

export function MetasPage() {
  const queryClient = useQueryClient();
  const { data: metas, isLoading } = useQuery({ queryKey: ['metas'], queryFn: metasApi.listarMetas });

  const [modalAberto, setModalAberto] = useState(false);
  const [form, setForm] = useState<FormState>(FORM_VAZIO);
  const [erro, setErro] = useState<string | null>(null);

  const criar = useMutation({
    mutationFn: () =>
      metasApi.criarMeta({
        tipo: form.tipo,
        titulo: form.titulo,
        valorAlvo: Number(form.valorAlvo),
        periodoInicio: dateOnlyInputToIso(form.periodoInicio),
        periodoFim: dateOnlyInputToIso(form.periodoFim),
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['metas'] });
      setModalAberto(false);
    },
    onError: (error) => setErro(extractErrorMessage(error)),
  });

  const remover = useMutation({
    mutationFn: (id: string) => metasApi.removerMeta(id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['metas'] }),
  });

  function abrirNovo() {
    setForm(FORM_VAZIO);
    setErro(null);
    setModalAberto(true);
  }

  function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setErro(null);
    criar.mutate();
  }

  return (
    <div>
      <PageHeader
        title="Metas"
        subtitle="Definição e acompanhamento de metas de faturamento e atendimentos"
        action={<Button onClick={abrirNovo}>Nova meta</Button>}
      />

      {isLoading ? (
        <p className="text-sm text-slate-500">Carregando…</p>
      ) : metas?.length === 0 ? (
        <p className="text-sm text-slate-400">Nenhuma meta cadastrada ainda.</p>
      ) : (
        <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
          {metas?.map((meta) => (
            <Card key={meta.id}>
              <div className="mb-3 flex items-start justify-between">
                <div>
                  <h3 className="font-semibold text-slate-900">{meta.titulo}</h3>
                  <Badge tone={meta.tipo === TipoMeta.Faturamento ? 'indigo' : 'slate'}>
                    {meta.tipo === TipoMeta.Faturamento ? 'Faturamento' : 'Atendimentos'}
                  </Badge>
                </div>
                <Button variant="ghost" onClick={() => remover.mutate(meta.id)}>
                  ✕
                </Button>
              </div>

              <div className="mb-2 flex items-baseline justify-between text-sm">
                <span className="text-slate-500">
                  {formatarValor(meta.tipo, meta.valorAtual)} de {formatarValor(meta.tipo, meta.valorAlvo)}
                </span>
                <span className="font-semibold text-slate-900">{meta.progressoPercentual.toFixed(0)}%</span>
              </div>
              <ProgressBar percentual={meta.progressoPercentual} />

              <p className="mt-3 text-xs text-slate-400">
                {new Date(meta.periodoInicio).toLocaleDateString('pt-BR')} até{' '}
                {new Date(meta.periodoFim).toLocaleDateString('pt-BR')}
              </p>
            </Card>
          ))}
        </div>
      )}

      {modalAberto && (
        <Modal title="Nova meta" onClose={() => setModalAberto(false)}>
          <form onSubmit={handleSubmit} className="space-y-4">
            <Select label="Tipo" value={form.tipo} onChange={(e) => setForm({ ...form, tipo: Number(e.target.value) as TipoMeta })}>
              <option value={TipoMeta.Faturamento}>Faturamento</option>
              <option value={TipoMeta.Atendimentos}>Atendimentos</option>
            </Select>
            <Input label="Título" value={form.titulo} onChange={(e) => setForm({ ...form, titulo: e.target.value })} required />
            <Input
              label={form.tipo === TipoMeta.Faturamento ? 'Valor alvo (R$)' : 'Quantidade alvo'}
              type="number"
              step={form.tipo === TipoMeta.Faturamento ? '0.01' : '1'}
              min="0.01"
              value={form.valorAlvo}
              onChange={(e) => setForm({ ...form, valorAlvo: e.target.value })}
              required
            />
            <div className="grid grid-cols-2 gap-4">
              <Input
                label="Início do período"
                type="date"
                value={form.periodoInicio}
                onChange={(e) => setForm({ ...form, periodoInicio: e.target.value })}
                required
              />
              <Input
                label="Fim do período"
                type="date"
                value={form.periodoFim}
                onChange={(e) => setForm({ ...form, periodoFim: e.target.value })}
                required
              />
            </div>
            {erro && <p className="text-sm text-red-600">{erro}</p>}
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
