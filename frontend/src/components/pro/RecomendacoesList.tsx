import { useQuery } from '@tanstack/react-query';
import { Gift, TrendingUp } from 'lucide-react';
import * as recomendacoesApi from '../../api/recomendacoes';
import { CategoriaRecomendacao } from '../../api/types';
import { Card } from '../ui/Card';

const CATEGORIA_INFO: Record<CategoriaRecomendacao, { label: string; icon: typeof Gift; tone: string }> = {
  [CategoriaRecomendacao.PacoteSugerido]: { label: 'Pacote sugerido', icon: Gift, tone: 'text-mint bg-mint/15' },
  [CategoriaRecomendacao.OtimizacaoHorarioPico]: { label: 'Horário de pico', icon: TrendingUp, tone: 'text-violet bg-violet/15' },
};

export function RecomendacoesList({ compact = false }: { compact?: boolean }) {
  const { data, isLoading, isError } = useQuery({
    queryKey: ['recomendacoes'],
    queryFn: recomendacoesApi.obterRecomendacoes,
  });

  if (isLoading) return <p className="text-sm text-faint">Carregando…</p>;
  if (isError || !data) return <p className="text-sm text-rose">Não foi possível carregar as recomendações.</p>;

  return (
    <div>
      <div className="space-y-3">
        {data.recomendacoes.length === 0 && <p className="text-sm text-faint">Nenhuma recomendação no momento.</p>}
        {data.recomendacoes.map((recomendacao, i) => {
          const info = CATEGORIA_INFO[recomendacao.categoria];
          const Icon = info.icon;
          return (
            <div key={i} className="flex gap-3">
              <div className={`flex h-8 w-8 shrink-0 items-center justify-center rounded-lg ${info.tone}`}>
                <Icon size={16} />
              </div>
              <div>
                <p className="text-sm font-semibold text-ink">{info.label}</p>
                <p className="mt-0.5 text-sm text-muted">{recomendacao.mensagem}</p>
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

export function RecomendacoesCard({ compact = false }: { compact?: boolean }) {
  return (
    <Card>
      <div className="mb-4 flex items-center justify-between">
        <p className="text-sm font-semibold text-ink">Recomendações Personalizadas</p>
        <span className="rounded-full bg-elevated px-2.5 py-1 text-[10px] font-bold tracking-wide text-faint uppercase">
          Ações sugeridas
        </span>
      </div>
      <RecomendacoesList compact={compact} />
    </Card>
  );
}
