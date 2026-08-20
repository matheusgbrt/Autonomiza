import { useState, type FormEvent } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import * as financeiroApi from '../api/financeiro';
import { TipoLancamento } from '../api/types';
import { extractErrorMessage } from '../api/client';
import { Button } from '../components/ui/Button';
import { Card } from '../components/ui/Card';
import { CurrencyInput, Input, Select } from '../components/ui/Input';
import { Modal } from '../components/ui/Modal';
import { Table, Thead, Tbody, Tr, Th, Td } from '../components/ui/Table';
import { PageHeader } from '../components/PageHeader';
import { Badge } from '../components/ui/Badge';
import { dateOnlyInputToIso, toDateInputValue } from '../utils/date';

function formatarMoeda(valor: number) {
  return valor.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
}

interface FormState {
  tipo: TipoLancamento;
  categoria: string;
  valor: string;
  data: string;
  descricao: string;
}

const FORM_VAZIO: FormState = {
  tipo: TipoLancamento.Entrada,
  categoria: '',
  valor: '',
  data: toDateInputValue(new Date()),
  descricao: '',
};

export function FinanceiroPage() {
  const queryClient = useQueryClient();
  const { data: lancamentos, isLoading } = useQuery({
    queryKey: ['lancamentos'],
    queryFn: financeiroApi.listarLancamentos,
  });
  const { data: saldo } = useQuery({ queryKey: ['saldo-mensal'], queryFn: () => financeiroApi.obterSaldoMensal() });

  const [modalAberto, setModalAberto] = useState(false);
  const [form, setForm] = useState<FormState>(FORM_VAZIO);
  const [erro, setErro] = useState<string | null>(null);

  const criar = useMutation({
    mutationFn: () =>
      financeiroApi.criarLancamento({
        tipo: form.tipo,
        categoria: form.categoria,
        valor: Number(form.valor),
        data: dateOnlyInputToIso(form.data),
        descricao: form.descricao || null,
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['lancamentos'] });
      queryClient.invalidateQueries({ queryKey: ['saldo-mensal'] });
      setModalAberto(false);
    },
    onError: (error) => setErro(extractErrorMessage(error)),
  });

  const remover = useMutation({
    mutationFn: (id: string) => financeiroApi.removerLancamento(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['lancamentos'] });
      queryClient.invalidateQueries({ queryKey: ['saldo-mensal'] });
    },
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
        title="Financeiro"
        subtitle="Entradas, saídas, saldo mensal e categorias"
        action={<Button onClick={abrirNovo}>Novo lançamento</Button>}
      />

      {saldo && (
        <div className="mb-8 grid grid-cols-1 gap-4 sm:grid-cols-3">
          <Card>
            <p className="text-xs font-medium uppercase text-faint">Entradas do mês</p>
            <p className="mt-2 text-2xl font-bold text-mint">{formatarMoeda(saldo.totalEntradas)}</p>
          </Card>
          <Card>
            <p className="text-xs font-medium uppercase text-faint">Saídas do mês</p>
            <p className="mt-2 text-2xl font-bold text-rose">{formatarMoeda(saldo.totalSaidas)}</p>
          </Card>
          <Card>
            <p className="text-xs font-medium uppercase text-faint">Saldo</p>
            <p className={`mt-2 text-2xl font-bold ${saldo.saldo >= 0 ? 'text-ink' : 'text-rose'}`}>
              {formatarMoeda(saldo.saldo)}
            </p>
          </Card>
        </div>
      )}

      {saldo && saldo.porCategoria.length > 0 && (
        <Card className="mb-8">
          <p className="mb-3 text-sm font-semibold text-muted">Por categoria (mês atual)</p>
          <div className="space-y-2">
            {saldo.porCategoria.map((c) => (
              <div key={c.categoria} className="flex items-center justify-between text-sm">
                <span className="text-muted">{c.categoria}</span>
                <span className="text-ink">
                  <span className="text-mint">{formatarMoeda(c.totalEntradas)}</span>
                  {c.totalSaidas > 0 && <span className="ml-2 text-rose">-{formatarMoeda(c.totalSaidas)}</span>}
                </span>
              </div>
            ))}
          </div>
        </Card>
      )}

      {isLoading ? (
        <p className="text-sm text-faint">Carregando…</p>
      ) : (
        <Table>
          <Thead>
            <Tr>
              <Th>Tipo</Th>
              <Th>Categoria</Th>
              <Th>Valor</Th>
              <Th>Data</Th>
              <Th />
            </Tr>
          </Thead>
          <Tbody>
            {lancamentos?.length === 0 && (
              <Tr>
                <Td colSpan={5} className="text-center text-faint">
                  Nenhum lançamento ainda.
                </Td>
              </Tr>
            )}
            {lancamentos?.map((lancamento) => (
              <Tr key={lancamento.id}>
                <Td>
                  <Badge tone={lancamento.tipo === TipoLancamento.Entrada ? 'green' : 'red'}>
                    {lancamento.tipo === TipoLancamento.Entrada ? 'Entrada' : 'Saída'}
                  </Badge>
                </Td>
                <Td>{lancamento.categoria}</Td>
                <Td className="font-medium text-ink">{formatarMoeda(lancamento.valor)}</Td>
                <Td>{new Date(lancamento.data).toLocaleDateString('pt-BR')}</Td>
                <Td className="text-right">
                  <Button variant="ghost" onClick={() => remover.mutate(lancamento.id)}>
                    Remover
                  </Button>
                </Td>
              </Tr>
            ))}
          </Tbody>
        </Table>
      )}

      {modalAberto && (
        <Modal title="Novo lançamento" onClose={() => setModalAberto(false)}>
          <form onSubmit={handleSubmit} className="space-y-4">
            <Select
              label="Tipo"
              value={form.tipo}
              onChange={(e) => setForm({ ...form, tipo: Number(e.target.value) as TipoLancamento })}
            >
              <option value={TipoLancamento.Entrada}>Entrada</option>
              <option value={TipoLancamento.Saida}>Saída</option>
            </Select>
            <Input
              label="Categoria"
              value={form.categoria}
              onChange={(e) => setForm({ ...form, categoria: e.target.value })}
              placeholder="Ex: Serviços, Material, Aluguel…"
              required
            />
            <CurrencyInput label="Valor (R$)" value={form.valor} onChange={(valor) => setForm({ ...form, valor })} required />
            <Input
              label="Data"
              type="date"
              value={form.data}
              onChange={(e) => setForm({ ...form, data: e.target.value })}
              required
            />
            <Input
              label="Descrição"
              value={form.descricao}
              onChange={(e) => setForm({ ...form, descricao: e.target.value })}
            />
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
