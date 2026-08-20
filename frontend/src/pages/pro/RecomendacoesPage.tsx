import { useQuery } from '@tanstack/react-query';
import * as recomendacoesApi from '../../api/recomendacoes';
import { CategoriaRecomendacao } from '../../api/types';
import { Card } from '../../components/ui/Card';
import { Badge } from '../../components/ui/Badge';
import { PageHeader } from '../../components/PageHeader';

const CATEGORIA_INFO: Record<CategoriaRecomendacao, { label: string; tone: 'green' | 'indigo' }> = {
  [CategoriaRecomendacao.PacoteSugerido]: { label: 'Pacote sugerido', tone: 'green' },
  [CategoriaRecomendacao.OtimizacaoHorarioPico]: { label: 'Horário de pico', tone: 'indigo' },
};

export function RecomendacoesPage() {
  const { data, isLoading, isError } = useQuery({
    queryKey: ['recomendacoes'],
    queryFn: recomendacoesApi.obterRecomendacoes,
  });

  if (isLoading) return <p className="text-sm text-slate-500">Carregando…</p>;
  if (isError || !data) return <p className="text-sm text-red-600">Não foi possível carregar as recomendações.</p>;

  return (
    <div>
      <PageHeader
        title="Recomendações Inteligentes"
        subtitle="Sugestões contextuais para aumentar o ticket médio e otimizar horários de pico"
      />

      <div className="space-y-4">
        {data.recomendacoes.map((recomendacao, i) => {
          const info = CATEGORIA_INFO[recomendacao.categoria];
          return (
            <Card key={i}>
              <Badge tone={info.tone}>{info.label}</Badge>
              <p className="mt-2 text-sm text-slate-700">{recomendacao.mensagem}</p>
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
