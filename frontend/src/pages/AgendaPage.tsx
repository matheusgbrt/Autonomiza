import { useMemo, useState, type FormEvent } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Star } from 'lucide-react';
import * as agendamentosApi from '../api/agendamentos';
import * as clientesApi from '../api/clientes';
import * as servicosApi from '../api/servicos';
import { StatusAgendamento, type AgendamentoDto } from '../api/types';
import { extractErrorMessage } from '../api/client';
import { Button } from '../components/ui/Button';
import { Input, Select, Textarea } from '../components/ui/Input';
import { Modal } from '../components/ui/Modal';
import { Table, Thead, Tbody, Tr, Th, Td } from '../components/ui/Table';
import { PageHeader } from '../components/PageHeader';
import { Badge } from '../components/ui/Badge';

type Periodo = 'dia' | 'semana' | 'mes' | 'todos';

const STATUS_LABEL: Record<StatusAgendamento, { label: string; tone: 'slate' | 'green' | 'amber' | 'red' | 'indigo' }> = {
  [StatusAgendamento.Agendado]: { label: 'Agendado', tone: 'slate' },
  [StatusAgendamento.Confirmado]: { label: 'Confirmado', tone: 'green' },
  [StatusAgendamento.Concluido]: { label: 'Concluído', tone: 'indigo' },
  [StatusAgendamento.Cancelado]: { label: 'Cancelado', tone: 'red' },
};

function calcularPeriodo(periodo: Periodo): { inicio: string; fim: string } | undefined {
  if (periodo === 'todos') return undefined;

  const agora = new Date();
  const inicio = new Date(agora.getFullYear(), agora.getMonth(), agora.getDate());
  const fim = new Date(inicio);

  if (periodo === 'dia') {
    fim.setDate(fim.getDate() + 1);
  } else if (periodo === 'semana') {
    fim.setDate(fim.getDate() + 7);
  } else {
    fim.setMonth(fim.getMonth() + 1);
  }

  return { inicio: inicio.toISOString(), fim: fim.toISOString() };
}

interface FormState {
  clienteId: string;
  servicoId: string;
  dataHoraInicio: string;
  observacoes: string;
}

const FORM_VAZIO: FormState = { clienteId: '', servicoId: '', dataHoraInicio: '', observacoes: '' };

export function AgendaPage() {
  const queryClient = useQueryClient();
  const [periodo, setPeriodo] = useState<Periodo>('semana');
  const intervalo = useMemo(() => calcularPeriodo(periodo), [periodo]);

  const { data: agendamentos, isLoading } = useQuery({
    queryKey: ['agendamentos', periodo],
    queryFn: () => agendamentosApi.listarAgendamentos(intervalo),
  });

  const { data: clientes } = useQuery({ queryKey: ['clientes'], queryFn: clientesApi.listarClientes });
  const { data: servicos } = useQuery({ queryKey: ['servicos'], queryFn: servicosApi.listarServicos });

  const [modalAberto, setModalAberto] = useState(false);
  const [form, setForm] = useState<FormState>(FORM_VAZIO);
  const [erro, setErro] = useState<string | null>(null);

  const criar = useMutation({
    mutationFn: () =>
      agendamentosApi.criarAgendamento({
        clienteId: form.clienteId,
        servicoId: form.servicoId,
        dataHoraInicio: new Date(form.dataHoraInicio).toISOString(),
        observacoes: form.observacoes || null,
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['agendamentos'] });
      setModalAberto(false);
    },
    onError: (error) => setErro(extractErrorMessage(error)),
  });

  const mudarStatus = useMutation({
    mutationFn: ({ agendamento, status, notaAtendimento }: { agendamento: AgendamentoDto; status: StatusAgendamento; notaAtendimento?: number | null }) =>
      agendamentosApi.atualizarAgendamento(agendamento.id, {
        clienteId: agendamento.clienteId,
        servicoId: agendamento.servicoId,
        dataHoraInicio: agendamento.dataHoraInicio,
        status,
        observacoes: agendamento.observacoes,
        notaAtendimento: notaAtendimento ?? agendamento.notaAtendimento,
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['agendamentos'] });
      setAvaliando(null);
    },
  });

  const [avaliando, setAvaliando] = useState<AgendamentoDto | null>(null);
  const [notaSelecionada, setNotaSelecionada] = useState(5);

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
        title="Agenda"
        subtitle="Visualização por dia, semana ou mês — evita conflitos de horário"
        action={<Button onClick={abrirNovo}>Novo agendamento</Button>}
      />

      <div className="mb-4 flex gap-2">
        {(['dia', 'semana', 'mes', 'todos'] as Periodo[]).map((opcao) => (
          <button
            key={opcao}
            onClick={() => setPeriodo(opcao)}
            className={`cursor-pointer rounded-lg px-3 py-1.5 text-sm font-medium capitalize ${
              periodo === opcao ? 'bg-indigo text-white' : 'bg-elevated text-muted border border-stroke'
            }`}
          >
            {opcao === 'mes' ? 'mês' : opcao}
          </button>
        ))}
      </div>

      {isLoading ? (
        <p className="text-sm text-faint">Carregando…</p>
      ) : (
        <Table>
          <Thead>
            <Tr>
              <Th>Cliente</Th>
              <Th>Serviço</Th>
              <Th>Data/hora</Th>
              <Th>Status</Th>
              <Th />
            </Tr>
          </Thead>
          <Tbody>
            {agendamentos?.length === 0 && (
              <Tr>
                <Td colSpan={5} className="text-center text-faint">
                  Nenhum agendamento nesse período.
                </Td>
              </Tr>
            )}
            {agendamentos?.map((agendamento) => {
              const status = STATUS_LABEL[agendamento.status];
              return (
                <Tr key={agendamento.id}>
                  <Td className="font-medium text-ink">{agendamento.clienteNome}</Td>
                  <Td>{agendamento.servicoNome}</Td>
                  <Td>{new Date(agendamento.dataHoraInicio).toLocaleString('pt-BR')}</Td>
                  <Td>
                    <Badge tone={status.tone}>{status.label}</Badge>
                  </Td>
                  <Td className="text-right">
                    <div className="flex justify-end gap-2">
                      {agendamento.status === StatusAgendamento.Agendado && (
                        <Button
                          variant="ghost"
                          onClick={() => mudarStatus.mutate({ agendamento, status: StatusAgendamento.Confirmado })}
                        >
                          Confirmar
                        </Button>
                      )}
                      {agendamento.status === StatusAgendamento.Confirmado && (
                        <Button
                          variant="ghost"
                          onClick={() => {
                            setNotaSelecionada(agendamento.notaAtendimento ?? 5);
                            setAvaliando(agendamento);
                          }}
                        >
                          Concluir
                        </Button>
                      )}
                      {agendamento.status !== StatusAgendamento.Cancelado &&
                        agendamento.status !== StatusAgendamento.Concluido && (
                        <Button
                          variant="ghost"
                          onClick={() => mudarStatus.mutate({ agendamento, status: StatusAgendamento.Cancelado })}
                        >
                          Cancelar
                        </Button>
                      )}
                    </div>
                  </Td>
                </Tr>
              );
            })}
          </Tbody>
        </Table>
      )}

      {modalAberto && (
        <Modal title="Novo agendamento" onClose={() => setModalAberto(false)}>
          <form onSubmit={handleSubmit} className="space-y-4">
            <Select
              label="Cliente"
              value={form.clienteId}
              onChange={(e) => setForm({ ...form, clienteId: e.target.value })}
              required
            >
              <option value="">Selecione…</option>
              {clientes?.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.nome}
                </option>
              ))}
            </Select>
            <Select
              label="Serviço"
              value={form.servicoId}
              onChange={(e) => setForm({ ...form, servicoId: e.target.value })}
              required
            >
              <option value="">Selecione…</option>
              {servicos?.map((s) => (
                <option key={s.id} value={s.id}>
                  {s.nome}
                </option>
              ))}
            </Select>
            <Input
              label="Data e hora"
              type="datetime-local"
              value={form.dataHoraInicio}
              onChange={(e) => setForm({ ...form, dataHoraInicio: e.target.value })}
              required
            />
            <Textarea
              label="Observações"
              value={form.observacoes}
              onChange={(e) => setForm({ ...form, observacoes: e.target.value })}
              rows={2}
            />
            {erro && <p className="text-sm text-rose">{erro}</p>}
            <div className="flex justify-end gap-2 pt-2">
              <Button type="button" variant="secondary" onClick={() => setModalAberto(false)}>
                Cancelar
              </Button>
              <Button type="submit" disabled={criar.isPending}>
                {criar.isPending ? 'Salvando…' : 'Agendar'}
              </Button>
            </div>
          </form>
        </Modal>
      )}

      {avaliando && (
        <Modal title="Concluir atendimento" onClose={() => setAvaliando(null)}>
          <p className="mb-4 text-sm text-muted">
            {avaliando.clienteNome} · {avaliando.servicoNome}
          </p>
          <p className="mb-2 text-sm font-medium text-muted">Avaliação do atendimento (opcional)</p>
          <div className="mb-6 flex gap-1">
            {[1, 2, 3, 4, 5].map((n) => (
              <button key={n} type="button" onClick={() => setNotaSelecionada(n)} aria-label={`${n} estrelas`} className="cursor-pointer">
                <Star size={24} className={n <= notaSelecionada ? 'fill-amber text-amber' : 'text-stroke'} />
              </button>
            ))}
          </div>
          <div className="flex justify-end gap-2">
            <Button type="button" variant="secondary" onClick={() => setAvaliando(null)}>
              Cancelar
            </Button>
            <Button
              onClick={() =>
                mudarStatus.mutate({ agendamento: avaliando, status: StatusAgendamento.Concluido, notaAtendimento: notaSelecionada })
              }
              disabled={mudarStatus.isPending}
            >
              {mudarStatus.isPending ? 'Salvando…' : 'Concluir'}
            </Button>
          </div>
        </Modal>
      )}
    </div>
  );
}
