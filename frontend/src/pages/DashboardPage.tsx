import { useQuery } from '@tanstack/react-query';
import * as dashboardApi from '../api/dashboard';
import { Card } from '../components/ui/Card';
import { PageHeader } from '../components/PageHeader';

function formatarMoeda(valor: number) {
  return valor.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
}

export function DashboardPage() {
  const { data, isLoading, isError } = useQuery({
    queryKey: ['dashboard-resumo'],
    queryFn: dashboardApi.obterResumo,
  });

  if (isLoading) return <p className="text-sm text-slate-500">Carregando…</p>;
  if (isError || !data) return <p className="text-sm text-red-600">Não foi possível carregar o resumo.</p>;

  const maiorValorDia = Math.max(1, ...data.serieDiaria.map((p) => p.total));

  return (
    <div>
      <PageHeader title="Dashboard" subtitle="Resumo dos últimos 30 dias (Autonomiza)" />

      <div className="mb-8 grid grid-cols-1 gap-4 sm:grid-cols-3">
        <Card>
          <p className="text-xs font-medium uppercase text-slate-500">Total em vendas</p>
          <p className="mt-2 text-2xl font-bold text-slate-900">{formatarMoeda(data.totalVendas)}</p>
        </Card>
        <Card>
          <p className="text-xs font-medium uppercase text-slate-500">Ticket médio</p>
          <p className="mt-2 text-2xl font-bold text-slate-900">{formatarMoeda(data.ticketMedio)}</p>
        </Card>
        <Card>
          <p className="text-xs font-medium uppercase text-slate-500">Quantidade de vendas</p>
          <p className="mt-2 text-2xl font-bold text-slate-900">{data.quantidadeVendas}</p>
        </Card>
      </div>

      <Card>
        <p className="mb-4 text-sm font-semibold text-slate-700">Série diária</p>
        <div className="flex h-40 items-end gap-1">
          {data.serieDiaria.map((ponto) => (
            <div key={ponto.data} className="group relative h-full flex-1">
              <div
                className="absolute bottom-0 w-full rounded-t bg-indigo-400 transition-colors group-hover:bg-indigo-500"
                style={{ height: `${Math.max(4, (ponto.total / maiorValorDia) * 100)}%` }}
              />
              <div className="pointer-events-none absolute -top-8 left-1/2 -translate-x-1/2 whitespace-nowrap rounded bg-slate-900 px-2 py-1 text-xs text-white opacity-0 group-hover:opacity-100">
                {new Date(ponto.data).toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit' })}:{' '}
                {formatarMoeda(ponto.total)}
              </div>
            </div>
          ))}
        </div>
      </Card>
    </div>
  );
}
