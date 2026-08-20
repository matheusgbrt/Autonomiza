import { useQuery } from '@tanstack/react-query';
import { AlertTriangle, Clock3, TrendingDown } from 'lucide-react';
import * as insightsApi from '../../api/insights';
import { CategoriaInsight } from '../../api/types';
import { Card } from '../ui/Card';

const CATEGORIA_INFO: Record<CategoriaInsight, { label: string; icon: typeof AlertTriangle; tone: string }> = {
  [CategoriaInsight.Cancelamento]: { label: 'Cancelamento', icon: AlertTriangle, tone: 'text-rose bg-rose/15' },
  [CategoriaInsight.HorarioOcioso]: { label: 'Horário ocioso', icon: Clock3, tone: 'text-amber bg-amber/15' },
  [CategoriaInsight.TendenciaReceita]: { label: 'Tendência de receita', icon: TrendingDown, tone: 'text-indigo bg-indigo/15' },
};

export function InsightsList({ compact = false }: { compact?: boolean }) {
  const { data, isLoading, isError } = useQuery({ queryKey: ['insights'], queryFn: insightsApi.obterInsights });

  if (isLoading) return <p className="text-sm text-faint">Carregando…</p>;
  if (isError || !data) return <p className="text-sm text-rose">Não foi possível carregar os insights.</p>;

  return (
    <div>
      <div className="space-y-3">
        {data.insights.length === 0 && <p className="text-sm text-faint">Nenhum achado no momento.</p>}
        {data.insights.map((insight, i) => {
          const info = CATEGORIA_INFO[insight.categoria];
          const Icon = info.icon;
          return (
            <div key={i} className="flex gap-3">
              <div className={`flex h-8 w-8 shrink-0 items-center justify-center rounded-lg ${info.tone}`}>
                <Icon size={16} />
              </div>
              <div>
                <p className="text-sm font-semibold text-ink">{info.label}</p>
                <p className="mt-0.5 text-sm text-muted">{insight.mensagem}</p>
              </div>
            </div>
          );
        })}
      </div>
      {!compact && (
        <p className="mt-6 text-xs text-faint">
          Gerado em {new Date(data.geradoEm).toLocaleString('pt-BR')} · atualiza automaticamente após{' '}
          {new Date(data.expiraEm).toLocaleString('pt-BR')}
        </p>
      )}
    </div>
  );
}

export function InsightsCard({ compact = false }: { compact?: boolean }) {
  return (
    <Card>
      <div className="mb-4 flex items-center justify-between">
        <p className="text-sm font-semibold text-ink">Principais Achados</p>
        <span className="rounded-full bg-elevated px-2.5 py-1 text-[10px] font-bold tracking-wide text-faint uppercase">
          IA · Diagnóstico
        </span>
      </div>
      <InsightsList compact={compact} />
    </Card>
  );
}
