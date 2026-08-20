import { PageHeader } from '../../components/PageHeader';
import { RecomendacoesCard } from '../../components/pro/RecomendacoesList';

export function RecomendacoesPage() {
  return (
    <div>
      <PageHeader
        title="Recomendações Inteligentes"
        subtitle="Sugestões contextuais para aumentar o ticket médio e otimizar horários de pico"
      />
      <RecomendacoesCard />
    </div>
  );
}
