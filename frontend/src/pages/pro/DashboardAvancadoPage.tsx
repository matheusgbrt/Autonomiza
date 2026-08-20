import { Fragment, useMemo } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Activity, Printer, Star, TrendingUp, Users, Wallet } from 'lucide-react';
import * as dashboardApi from '../../api/dashboard';
import * as agendamentosApi from '../../api/agendamentos';
import { Card } from '../../components/ui/Card';
import { Button } from '../../components/ui/Button';
import { PageHeader } from '../../components/PageHeader';
import { InsightsCard } from '../../components/pro/InsightsList';
import { RecomendacoesCard } from '../../components/pro/RecomendacoesList';

function formatarMoeda(valor: number) {
  return valor.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
}

const DONUT_CORES = ['#22d3ee', '#8b5cf6', '#6366f1', '#34d399', '#fbbf24', '#fb7185'];

type KpiTone = 'indigo' | 'cyan' | 'mint' | 'amber';

const KPI_TONE_CLASSES: Record<KpiTone, { badge: string; track: string }> = {
  indigo: { badge: 'bg-indigo/15 text-indigo', track: 'bg-indigo' },
  cyan: { badge: 'bg-cyan/15 text-cyan', track: 'bg-cyan' },
  mint: { badge: 'bg-mint/15 text-mint', track: 'bg-mint' },
  amber: { badge: 'bg-amber/15 text-amber', track: 'bg-amber' },
};

function KpiCard({
  icon: Icon,
  label,
  value,
  sublabel,
  percent,
  tone = 'indigo',
}: {
  icon: typeof Wallet;
  label: string;
  value: string;
  sublabel: string;
  percent?: number;
  tone?: KpiTone;
}) {
  const classes = KPI_TONE_CLASSES[tone];
  return (
    <Card>
      <div className="mb-3 flex items-center justify-between">
        <p className="text-xs font-medium uppercase tracking-wide text-faint">{label}</p>
        <div className={`flex h-7 w-7 items-center justify-center rounded-lg ${classes.badge}`}>
          <Icon size={14} />
        </div>
      </div>
      <p className="text-2xl font-bold text-ink">{value}</p>
      <p className="mt-1 text-xs text-faint">{sublabel}</p>
      {percent !== undefined && (
        <div className="mt-3 h-1.5 w-full overflow-hidden rounded-full bg-elevated">
          <div
            className={`h-full rounded-full transition-all ${classes.track}`}
            style={{ width: `${Math.max(0, Math.min(100, percent))}%` }}
          />
        </div>
      )}
    </Card>
  );
}

function Donut({ segments }: { segments: { label: string; value: number }[] }) {
  const total = segments.reduce((acc, s) => acc + s.value, 0);
  const raio = 60;
  const circunferencia = 2 * Math.PI * raio;
  let acumulado = 0;

  return (
    <div className="flex flex-col items-center gap-6">
      <svg viewBox="0 0 160 160" width={160} height={160} className="shrink-0 -rotate-90">
        <circle cx={80} cy={80} r={raio} fill="none" stroke="#16223c" strokeWidth={20} />
        {total > 0 &&
          segments.map((s, i) => {
            const fracao = s.value / total;
            const comprimento = fracao * circunferencia;
            const offset = -((acumulado / total) * circunferencia);
            acumulado += s.value;
            return (
              <circle
                key={s.label}
                cx={80}
                cy={80}
                r={raio}
                fill="none"
                stroke={DONUT_CORES[i % DONUT_CORES.length]}
                strokeWidth={20}
                strokeDasharray={`${comprimento} ${circunferencia - comprimento}`}
                strokeDashoffset={offset}
              />
            );
          })}
      </svg>
      <div className="w-full min-w-0 flex-1 space-y-2">
        {segments.length === 0 && <p className="text-sm text-faint">Sem dados suficientes ainda.</p>}
        {segments.map((s, i) => (
          <div key={s.label} className="flex items-center justify-between gap-3 text-sm">
            <span className="flex min-w-0 items-center gap-2 text-muted">
              <span
                className="h-2.5 w-2.5 shrink-0 rounded-full"
                style={{ backgroundColor: DONUT_CORES[i % DONUT_CORES.length] }}
              />
              <span className="truncate">{s.label}</span>
            </span>
            <span className="shrink-0 font-medium text-ink">{total > 0 ? `${((s.value / total) * 100).toFixed(0)}%` : '—'}</span>
          </div>
        ))}
      </div>
    </div>
  );
}

const DIAS_SEMANA = ['Segunda', 'Terça', 'Quarta', 'Quinta', 'Sexta', 'Sábado'];
const HORAS_GRADE = Array.from({ length: 12 }, (_, i) => i + 8); // 8h..19h

function HorariosDePicoHeatmap({ dataHoraInicio }: { dataHoraInicio: string[] }) {
  const contagem = useMemo(() => {
    const mapa = new Map<string, number>();
    for (const iso of dataHoraInicio) {
      const data = new Date(iso);
      const diaSemana = data.getDay(); // 0=domingo
      const hora = data.getHours();
      if (diaSemana === 0 || hora < 8 || hora > 19) continue;
      const chave = `${diaSemana}-${hora}`;
      mapa.set(chave, (mapa.get(chave) ?? 0) + 1);
    }
    return mapa;
  }, [dataHoraInicio]);

  const maximo = Math.max(1, ...contagem.values());

  return (
    <div className="overflow-x-auto">
      <div className="grid min-w-[560px] grid-cols-[60px_repeat(12,1fr)] gap-1 text-[10px] text-faint">
        <div />
        {HORAS_GRADE.map((h) => (
          <div key={h} className="text-center">
            {h}h
          </div>
        ))}
        {DIAS_SEMANA.map((dia, diaIndex) => (
          <Fragment key={dia}>
            <div className="flex items-center text-xs text-muted">{dia}</div>
            {HORAS_GRADE.map((hora) => {
              const valor = contagem.get(`${diaIndex + 1}-${hora}`) ?? 0;
              const intensidade = valor / maximo;
              return (
                <div
                  key={`${dia}-${hora}`}
                  className="aspect-square rounded"
                  style={{ backgroundColor: `rgba(99, 102, 241, ${0.08 + intensidade * 0.82})` }}
                  title={`${dia} ${hora}h — ${valor} agendamento${valor === 1 ? '' : 's'}`}
                />
              );
            })}
          </Fragment>
        ))}
      </div>
      <div className="mt-3 flex items-center gap-2 text-xs text-faint">
        <span>menos</span>
        <div className="flex gap-0.5">
          {[0.1, 0.3, 0.5, 0.7, 0.9].map((op) => (
            <div key={op} className="h-3 w-3 rounded" style={{ backgroundColor: `rgba(99, 102, 241, ${op})` }} />
          ))}
        </div>
        <span>mais</span>
      </div>
    </div>
  );
}

export function DashboardAvancadoPage() {
  const { data, isLoading, isError } = useQuery({
    queryKey: ['dashboard-avancado'],
    queryFn: () => dashboardApi.obterAvancado(),
  });
  const { data: resumo } = useQuery({ queryKey: ['dashboard-resumo'], queryFn: dashboardApi.obterResumo });
  const { data: agendamentos } = useQuery({
    queryKey: ['agendamentos', 'todos'],
    queryFn: () => agendamentosApi.listarAgendamentos(),
  });

  if (isLoading) return <p className="text-sm text-faint">Carregando…</p>;
  if (isError || !data) return <p className="text-sm text-rose">Não foi possível carregar o dashboard avançado.</p>;

  const servicosOrdenados = [...data.rentabilidadePorServico].sort((a, b) => b.receitaTotal - a.receitaTotal);
  const maiorReceita = Math.max(1, ...servicosOrdenados.map((s) => s.receitaTotal));
  const maiorValorDia = Math.max(1, ...(resumo?.serieDiaria.map((p) => p.total) ?? [0]));
  const maiorValorTrimestre = Math.max(1, ...data.crescimentoReceitaTrimestral.map((p) => p.total));

  return (
    <div>
      <PageHeader
        title="Dashboard Avançado"
        subtitle="Indicadores de rentabilidade, fidelização e projeções — identifique os serviços mais lucrativos"
        action={
          <Button variant="secondary" onClick={() => window.print()}>
            <Printer size={15} />
            Exportar relatório
          </Button>
        }
      />

      <div className="mb-6 grid grid-cols-1 gap-4 sm:grid-cols-2 xl:grid-cols-4">
        <KpiCard
          icon={Activity}
          label="Health do Negócio"
          value={`${data.healthScoreNegocio}`}
          sublabel="Retenção, conclusão e crescimento combinados"
          percent={data.healthScoreNegocio}
        />
        <KpiCard
          icon={Wallet}
          label="Faturamento do mês"
          value={resumo ? formatarMoeda(resumo.totalVendas) : '—'}
          sublabel="Total em vendas nos últimos 30 dias"
          tone="cyan"
        />
        <KpiCard
          icon={Users}
          label="Retenção de clientes"
          value={`${data.taxaFidelizacaoPercentual.toFixed(0)}%`}
          sublabel="Clientes com mais de um atendimento"
          percent={data.taxaFidelizacaoPercentual}
          tone="mint"
        />
        <KpiCard
          icon={Star}
          label="Satisfação"
          value={data.satisfacaoMedia !== null ? `${data.satisfacaoMedia.toFixed(1)}/5` : 'Sem dados'}
          sublabel={data.satisfacaoMedia !== null ? 'Nota média dos atendimentos concluídos' : 'Registre notas ao concluir atendimentos na Agenda'}
          tone="amber"
        />
      </div>

      <div className="mb-6 grid grid-cols-1 gap-4 sm:grid-cols-2">
        <KpiCard
          icon={TrendingUp}
          label="Projeção próximos 30 dias"
          value={formatarMoeda(data.projecaoFaturamentoProximos30Dias)}
          sublabel="Baseada no faturamento recente"
        />
        <KpiCard
          icon={Wallet}
          label="Ticket médio"
          value={resumo ? formatarMoeda(resumo.ticketMedio) : '—'}
          sublabel={`${resumo?.quantidadeVendas ?? 0} vendas no período`}
        />
      </div>

      <div className="mb-6 grid grid-cols-1 gap-4 xl:grid-cols-3">
        <Card>
          <p className="mb-4 text-sm font-semibold text-ink">Faturamento diário</p>
          <div className="flex h-40 items-end gap-1">
            {(resumo?.serieDiaria ?? []).map((ponto) => (
              <div key={ponto.data} className="group relative h-full flex-1">
                <div
                  className="absolute bottom-0 w-full rounded-t bg-indigo/70 transition-colors group-hover:bg-indigo"
                  style={{ height: `${Math.max(4, (ponto.total / maiorValorDia) * 100)}%` }}
                />
                <div className="pointer-events-none absolute -top-8 left-1/2 -translate-x-1/2 whitespace-nowrap rounded border border-stroke bg-glass px-2 py-1 text-xs text-ink opacity-0 group-hover:opacity-100">
                  {new Date(ponto.data).toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit' })}:{' '}
                  {formatarMoeda(ponto.total)}
                </div>
              </div>
            ))}
          </div>
        </Card>

        <Card>
          <p className="mb-1 text-sm font-semibold text-ink">Crescimento de Receita</p>
          <p className="mb-4 text-xs text-faint">por trimestre</p>
          {data.crescimentoReceitaTrimestral.length === 0 ? (
            <p className="text-sm text-faint">Ainda não há entradas financeiras suficientes.</p>
          ) : (
            <div className="flex h-40 items-end gap-2">
              {data.crescimentoReceitaTrimestral.map((ponto) => (
                <div key={`${ponto.ano}-${ponto.trimestre}`} className="group relative h-full flex-1">
                  <div
                    className="absolute bottom-0 w-full rounded-t bg-violet/70 transition-colors group-hover:bg-violet"
                    style={{ height: `${Math.max(4, (ponto.total / maiorValorTrimestre) * 100)}%` }}
                  />
                  <div className="pointer-events-none absolute -top-8 left-1/2 -translate-x-1/2 whitespace-nowrap rounded border border-stroke bg-glass px-2 py-1 text-xs text-ink opacity-0 group-hover:opacity-100">
                    {formatarMoeda(ponto.total)}
                  </div>
                  <p className="absolute -bottom-5 left-1/2 -translate-x-1/2 whitespace-nowrap text-[10px] text-faint">
                    {ponto.ano}·T{ponto.trimestre}
                  </p>
                </div>
              ))}
            </div>
          )}
        </Card>

        <Card>
          <p className="mb-4 text-sm font-semibold text-ink">Categorias de Serviço</p>
          <Donut segments={servicosOrdenados.slice(0, 6).map((s) => ({ label: s.servicoNome, value: s.receitaTotal }))} />
        </Card>
      </div>

      <div className="mb-6 grid grid-cols-1 gap-4 xl:grid-cols-2">
        <Card>
          <p className="mb-4 text-sm font-semibold text-ink">Serviços Mais Lucrativos</p>
          {servicosOrdenados.length === 0 ? (
            <p className="text-sm text-faint">
              Ainda não há vendas vinculadas a agendamentos para calcular rentabilidade por serviço.
            </p>
          ) : (
            <div className="space-y-4">
              {servicosOrdenados.map((s, i) => (
                <div key={s.servicoId} className="flex items-center gap-3">
                  <div className="flex h-6 w-6 shrink-0 items-center justify-center rounded-full bg-elevated text-xs font-bold text-faint">
                    {i + 1}
                  </div>
                  <div className="flex-1">
                    <div className="flex items-baseline justify-between text-sm">
                      <span className="font-medium text-ink">{s.servicoNome}</span>
                      <span className="text-muted">{formatarMoeda(s.receitaTotal)}</span>
                    </div>
                    <p className="mb-1 text-xs text-faint">{s.quantidadeAtendimentos} atendimentos</p>
                    <div className="h-1.5 w-full overflow-hidden rounded-full bg-elevated">
                      <div
                        className="h-full rounded-full bg-cyan"
                        style={{ width: `${(s.receitaTotal / maiorReceita) * 100}%` }}
                      />
                    </div>
                  </div>
                </div>
              ))}
            </div>
          )}
        </Card>

        <Card>
          <p className="mb-1 text-sm font-semibold text-ink">Horários de Pico</p>
          <p className="mb-4 text-xs text-faint">densidade de agendamentos (todos os períodos)</p>
          <HorariosDePicoHeatmap dataHoraInicio={agendamentos?.map((a) => a.dataHoraInicio) ?? []} />
        </Card>
      </div>

      <div className="grid grid-cols-1 gap-4 xl:grid-cols-2">
        <InsightsCard compact />
        <RecomendacoesCard compact />
      </div>
    </div>
  );
}
