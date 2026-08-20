import { useQuery } from '@tanstack/react-query';
import { Link, useParams } from 'react-router-dom';
import * as clientesApi from '../api/clientes';
import { StatusAgendamento } from '../api/types';
import { Badge } from '../components/ui/Badge';
import { Card } from '../components/ui/Card';
import { PageHeader } from '../components/PageHeader';
import { Table, Thead, Tbody, Tr, Th, Td } from '../components/ui/Table';
import { formatarTelefone } from '../utils/mask';

const STATUS_LABEL: Record<StatusAgendamento, { label: string; tone: 'slate' | 'green' | 'amber' | 'red' | 'indigo' }> = {
  [StatusAgendamento.Agendado]: { label: 'Agendado', tone: 'slate' },
  [StatusAgendamento.Confirmado]: { label: 'Confirmado', tone: 'green' },
  [StatusAgendamento.Concluido]: { label: 'Concluído', tone: 'indigo' },
  [StatusAgendamento.Cancelado]: { label: 'Cancelado', tone: 'red' },
};

export function ClienteDetalhePage() {
  const { id } = useParams<{ id: string }>();

  const { data: cliente } = useQuery({
    queryKey: ['clientes', id],
    queryFn: () => clientesApi.obterCliente(id!),
    enabled: !!id,
  });

  const { data: historico, isLoading } = useQuery({
    queryKey: ['clientes', id, 'agendamentos'],
    queryFn: () => clientesApi.listarAgendamentosDoCliente(id!),
    enabled: !!id,
  });

  return (
    <div>
      <Link to="/clientes" className="mb-4 inline-block text-sm text-indigo hover:text-indigo/80">
        ← Voltar para clientes
      </Link>

      <PageHeader title={cliente?.nome ?? 'Cliente'} subtitle="Histórico de atendimentos" />

      <Card className="mb-6">
        <dl className="grid grid-cols-1 gap-4 sm:grid-cols-3">
          <div>
            <dt className="text-xs font-medium uppercase text-faint">E-mail</dt>
            <dd className="mt-1 text-sm text-ink">{cliente?.email ?? '—'}</dd>
          </div>
          <div>
            <dt className="text-xs font-medium uppercase text-faint">Telefone</dt>
            <dd className="mt-1 text-sm text-ink">{cliente?.telefone ? formatarTelefone(cliente.telefone) : '—'}</dd>
          </div>
          <div>
            <dt className="text-xs font-medium uppercase text-faint">Observações</dt>
            <dd className="mt-1 text-sm text-ink">{cliente?.observacoes ?? '—'}</dd>
          </div>
        </dl>
      </Card>

      {isLoading ? (
        <p className="text-sm text-faint">Carregando…</p>
      ) : (
        <Table>
          <Thead>
            <Tr>
              <Th>Serviço</Th>
              <Th>Data</Th>
              <Th>Status</Th>
            </Tr>
          </Thead>
          <Tbody>
            {historico?.length === 0 && (
              <Tr>
                <Td colSpan={3} className="text-center text-faint">
                  Nenhum atendimento registrado ainda.
                </Td>
              </Tr>
            )}
            {historico?.map((agendamento) => {
              const status = STATUS_LABEL[agendamento.status];
              return (
                <Tr key={agendamento.id}>
                  <Td className="font-medium text-ink">{agendamento.servicoNome}</Td>
                  <Td>{new Date(agendamento.dataHoraInicio).toLocaleString('pt-BR')}</Td>
                  <Td>
                    <Badge tone={status.tone}>{status.label}</Badge>
                  </Td>
                </Tr>
              );
            })}
          </Tbody>
        </Table>
      )}
    </div>
  );
}
