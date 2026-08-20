import { useQuery } from '@tanstack/react-query';
import * as dashboardApi from '../../api/dashboard';
import { Card } from '../../components/ui/Card';
import { PageHeader } from '../../components/PageHeader';
import { Table, Thead, Tbody, Tr, Th, Td } from '../../components/ui/Table';

function formatarMoeda(valor: number) {
  return valor.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
}

export function DashboardAvancadoPage() {
  const { data, isLoading, isError } = useQuery({
    queryKey: ['dashboard-avancado'],
    queryFn: dashboardApi.obterAvancado,
  });

  if (isLoading) return <p className="text-sm text-slate-500">Carregando…</p>;
  if (isError || !data) return <p className="text-sm text-red-600">Não foi possível carregar o dashboard avançado.</p>;

  return (
    <div>
      <PageHeader
        title="Dashboard Avançado"
        subtitle="Indicadores de rentabilidade, fidelização e projeções — identifique os serviços mais lucrativos"
      />

      <div className="mb-8 grid grid-cols-1 gap-4 sm:grid-cols-2">
        <Card>
          <p className="text-xs font-medium uppercase text-slate-500">Taxa de fidelização</p>
          <p className="mt-2 text-2xl font-bold text-slate-900">{data.taxaFidelizacaoPercentual.toFixed(0)}%</p>
          <p className="mt-1 text-xs text-slate-400">Clientes com mais de um atendimento</p>
        </Card>
        <Card>
          <p className="text-xs font-medium uppercase text-slate-500">Projeção próximos 30 dias</p>
          <p className="mt-2 text-2xl font-bold text-slate-900">
            {formatarMoeda(data.projecaoFaturamentoProximos30Dias)}
          </p>
          <p className="mt-1 text-xs text-slate-400">Baseada no faturamento dos últimos 30 dias</p>
        </Card>
      </div>

      <p className="mb-3 text-sm font-semibold text-slate-700">Rentabilidade por serviço</p>
      {data.rentabilidadePorServico.length === 0 ? (
        <p className="text-sm text-slate-400">
          Ainda não há vendas vinculadas a agendamentos para calcular rentabilidade por serviço.
        </p>
      ) : (
        <Table>
          <Thead>
            <Tr>
              <Th>Serviço</Th>
              <Th>Receita total</Th>
              <Th>Atendimentos</Th>
              <Th>Ticket médio</Th>
            </Tr>
          </Thead>
          <Tbody>
            {data.rentabilidadePorServico.map((s) => (
              <Tr key={s.servicoId}>
                <Td className="font-medium text-slate-900">{s.servicoNome}</Td>
                <Td>{formatarMoeda(s.receitaTotal)}</Td>
                <Td>{s.quantidadeAtendimentos}</Td>
                <Td>{formatarMoeda(s.ticketMedio)}</Td>
              </Tr>
            ))}
          </Tbody>
        </Table>
      )}
    </div>
  );
}
