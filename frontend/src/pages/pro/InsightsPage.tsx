import { useQuery } from '@tanstack/react-query';
import * as insightsApi from '../../api/insights';
import { CategoriaInsight } from '../../api/types';
import { Card } from '../../components/ui/Card';
import { Badge } from '../../components/ui/Badge';
import { PageHeader } from '../../components/PageHeader';

const CATEGORIA_INFO: Record<CategoriaInsight, { label: string; tone: 'red' | 'amber' | 'indigo' }> = {
  [CategoriaInsight.Cancelamento]: { label: 'Cancelamento', tone: 'red' },
  [CategoriaInsight.HorarioOcioso]: { label: 'Horário ocioso', tone: 'amber' },
  [CategoriaInsight.TendenciaReceita]: { label: 'Tendência de receita', tone: 'indigo' },
};

export function InsightsPage() {
  const { data, isLoading, isError } = useQuery({ queryKey: ['insights'], queryFn: insightsApi.obterInsights });

  if (isLoading) return <p className="text-sm text-slate-500">Carregando…</p>;
  if (isError || !data) return <p className="text-sm text-red-600">Não foi possível carregar os insights.</p>;

  return (
    <div>
      <PageHeader
        title="IA Consultora"
        subtitle="Análise preditiva sobre padrões de cancelamento, horários ociosos e tendências de receita"
      />

      <div className="space-y-4">
        {data.insights.map((insight, i) => {
          const info = CATEGORIA_INFO[insight.categoria];
          return (
            <Card key={i}>
              <Badge tone={info.tone}>{info.label}</Badge>
              <p className="mt-2 text-sm text-slate-700">{insight.mensagem}</p>
            </Card>
          );
        })}
      </div>

      <p className="mt-6 text-xs text-slate-400">
        Gerado em {new Date(data.geradoEm).toLocaleString('pt-BR')} · atualiza automaticamente após{' '}
        {new Date(data.expiraEm).toLocaleString('pt-BR')}
      </p>
    </div>
  );
}
