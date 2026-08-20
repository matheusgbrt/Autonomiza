import { PageHeader } from '../../components/PageHeader';
import { InsightsCard } from '../../components/pro/InsightsList';

export function InsightsPage() {
  return (
    <div>
      <PageHeader
        title="IA Consultora"
        subtitle="Análise preditiva sobre padrões de cancelamento, horários ociosos e tendências de receita"
      />
      <InsightsCard />
    </div>
  );
}
