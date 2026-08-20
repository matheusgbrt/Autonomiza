import { useState, type FormEvent } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { CalendarCheck, MessageSquareText, PhoneCall, Send } from 'lucide-react';
import * as integracaoApi from '../../api/integracaoWhatsapp';
import * as agendamentosApi from '../../api/agendamentos';
import { extractErrorMessage } from '../../api/client';
import type { ConfiguracaoWhatsAppDto } from '../../api/types';
import { Badge } from '../../components/ui/Badge';
import { Button } from '../../components/ui/Button';
import { Card } from '../../components/ui/Card';
import { Input, Textarea } from '../../components/ui/Input';
import { PageHeader } from '../../components/PageHeader';

const COMO_FUNCIONA = [
  { titulo: 'Cliente envia mensagem', descricao: 'Ele chama no WhatsApp e pede um horário.', icon: MessageSquareText },
  { titulo: 'Escolha do serviço', descricao: 'O bot mostra seus serviços e valores.', icon: PhoneCall },
  { titulo: 'Data e horário', descricao: 'Só aparecem as janelas realmente livres.', icon: CalendarCheck },
  { titulo: 'Confirmação automática', descricao: 'Cliente recebe o comprovante na hora.', icon: Send },
];

function StatBox({ label, value }: { label: string; value: string }) {
  return (
    <div>
      <p className="text-xs text-faint">{label}</p>
      <p className="mt-1 text-xl font-bold text-ink">{value}</p>
    </div>
  );
}

function ChatPreview() {
  const { data: mensagens, isLoading } = useQuery({
    queryKey: ['whatsapp-ultima-conversa'],
    queryFn: integracaoApi.obterUltimaConversa,
  });

  if (isLoading) return <p className="text-sm text-faint">Carregando…</p>;
  if (!mensagens || mensagens.length === 0) {
    return <p className="text-sm text-faint">Nenhuma conversa registrada ainda — assim que um cliente escrever no WhatsApp, ela aparece aqui.</p>;
  }

  return (
    <div className="max-h-80 space-y-2 overflow-y-auto">
      {mensagens.map((m, i) => (
        <div key={i} className={`flex ${m.direcao === 'Enviada' ? 'justify-end' : 'justify-start'}`}>
          <div
            className={`max-w-[80%] rounded-xl px-3 py-2 text-sm ${
              m.direcao === 'Enviada' ? 'bg-whatsapp/20 text-ink' : 'bg-elevated text-ink'
            }`}
          >
            <p className="whitespace-pre-wrap">{m.conteudo}</p>
            <p className="mt-1 text-[10px] text-faint">{new Date(m.criadoEm).toLocaleString('pt-BR')}</p>
          </div>
        </div>
      ))}
    </div>
  );
}

function ProximosAgendamentosWhatsApp() {
  const { data: agendamentos, isLoading } = useQuery({
    queryKey: ['agendamentos', 'proximos-whatsapp'],
    queryFn: () =>
      agendamentosApi.listarAgendamentos({
        inicio: new Date().toISOString(),
        fim: new Date(Date.now() + 14 * 24 * 60 * 60 * 1000).toISOString(),
      }),
  });

  const viaWhatsApp = (agendamentos ?? []).filter((a) => a.observacoes === 'Agendado via WhatsApp').slice(0, 6);

  if (isLoading) return <p className="text-sm text-faint">Carregando…</p>;
  if (viaWhatsApp.length === 0) {
    return <p className="text-sm text-faint">Nenhum agendamento originado pelo WhatsApp nos próximos 14 dias.</p>;
  }

  return (
    <div className="space-y-3">
      {viaWhatsApp.map((a) => (
        <div key={a.id} className="flex items-center justify-between text-sm">
          <div>
            <p className="font-medium text-ink">{a.clienteNome}</p>
            <p className="text-xs text-faint">{a.servicoNome}</p>
          </div>
          <p className="text-xs text-muted">
            {new Date(a.dataHoraInicio).toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit' })} ·{' '}
            {new Date(a.dataHoraInicio).toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' })}
          </p>
        </div>
      ))}
    </div>
  );
}

function ConfiguracoesWhatsApp() {
  const queryClient = useQueryClient();
  const { data: configuracao, isLoading } = useQuery({
    queryKey: ['whatsapp-configuracao'],
    queryFn: integracaoApi.obterConfiguracao,
  });

  const [form, setForm] = useState<ConfiguracaoWhatsAppDto | null>(null);
  const [configuracaoSincronizada, setConfiguracaoSincronizada] = useState<ConfiguracaoWhatsAppDto | null>(null);

  if (configuracao && configuracao !== configuracaoSincronizada) {
    setConfiguracaoSincronizada(configuracao);
    setForm(configuracao);
  }

  const salvar = useMutation({
    mutationFn: (dto: ConfiguracaoWhatsAppDto) => integracaoApi.atualizarConfiguracao(dto),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['whatsapp-configuracao'] }),
  });

  if (isLoading || !form) return <p className="text-sm text-faint">Carregando…</p>;

  function alternar(campo: keyof Omit<ConfiguracaoWhatsAppDto, 'mensagemBoasVindas'>) {
    if (!form) return;
    const atualizado = { ...form, [campo]: !form[campo] };
    setForm(atualizado);
    salvar.mutate(atualizado);
  }

  const toggles: { campo: keyof Omit<ConfiguracaoWhatsAppDto, 'mensagemBoasVindas'>; label: string }[] = [
    { campo: 'respostasAutomaticas', label: 'Respostas automáticas' },
    { campo: 'horariosDisponiveis', label: 'Agendamento por horários disponíveis' },
    { campo: 'confirmarAgendamentos', label: 'Exigir confirmação do cliente' },
    { campo: 'lembretesAutomaticos', label: 'Lembretes automáticos' },
  ];

  return (
    <div className="space-y-3">
      {toggles.map((t) => (
        <label key={t.campo} className="flex cursor-pointer items-center justify-between text-sm">
          <span className="text-muted">{t.label}</span>
          <input
            type="checkbox"
            checked={form[t.campo]}
            onChange={() => alternar(t.campo)}
            className="h-4 w-4 cursor-pointer accent-indigo"
          />
        </label>
      ))}
    </div>
  );
}

function MensagemBoasVindas() {
  const queryClient = useQueryClient();
  const { data: configuracao, isLoading } = useQuery({
    queryKey: ['whatsapp-configuracao'],
    queryFn: integracaoApi.obterConfiguracao,
  });

  const [mensagem, setMensagem] = useState('');
  const [editando, setEditando] = useState(false);
  const [configuracaoSincronizada, setConfiguracaoSincronizada] = useState<ConfiguracaoWhatsAppDto | null>(null);

  if (configuracao && configuracao !== configuracaoSincronizada) {
    setConfiguracaoSincronizada(configuracao);
    setMensagem(configuracao.mensagemBoasVindas ?? '');
  }

  const salvar = useMutation({
    mutationFn: () =>
      integracaoApi.atualizarConfiguracao({
        respostasAutomaticas: configuracao?.respostasAutomaticas ?? true,
        horariosDisponiveis: configuracao?.horariosDisponiveis ?? true,
        confirmarAgendamentos: configuracao?.confirmarAgendamentos ?? true,
        lembretesAutomaticos: configuracao?.lembretesAutomaticos ?? true,
        mensagemBoasVindas: mensagem || null,
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['whatsapp-configuracao'] });
      setEditando(false);
    },
  });

  if (isLoading) return <p className="text-sm text-faint">Carregando…</p>;

  if (!editando) {
    return (
      <div>
        <p className="text-sm text-muted">
          {mensagem || 'Nenhuma mensagem de boas-vindas configurada — o bot vai direto para a lista de serviços.'}
        </p>
        <Button variant="secondary" className="mt-3" onClick={() => setEditando(true)}>
          Editar mensagem
        </Button>
      </div>
    );
  }

  return (
    <div className="space-y-3">
      <Textarea
        label="Mensagem de boas-vindas"
        value={mensagem}
        onChange={(e) => setMensagem(e.target.value)}
        rows={3}
        placeholder="Ex: Olá! Sou a assistente virtual. Como posso te ajudar hoje?"
      />
      <div className="flex justify-end gap-2">
        <Button variant="secondary" onClick={() => setEditando(false)}>
          Cancelar
        </Button>
        <Button onClick={() => salvar.mutate()} disabled={salvar.isPending}>
          {salvar.isPending ? 'Salvando…' : 'Salvar'}
        </Button>
      </div>
    </div>
  );
}

export function IntegracaoWhatsAppPage() {
  const queryClient = useQueryClient();
  const { data: status, isLoading } = useQuery({ queryKey: ['whatsapp-status'], queryFn: integracaoApi.obterStatus });
  const { data: estatisticas } = useQuery({ queryKey: ['whatsapp-estatisticas'], queryFn: integracaoApi.obterEstatisticas });

  const [instanceId, setInstanceId] = useState('');
  const [token, setToken] = useState('');
  const [clientToken, setClientToken] = useState('');
  const [erro, setErro] = useState<string | null>(null);
  const [sucesso, setSucesso] = useState(false);

  const configurar = useMutation({
    mutationFn: () => integracaoApi.configurar({ instanceId, token, clientToken: clientToken || null }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['whatsapp-status'] });
      setSucesso(true);
      setToken('');
      setClientToken('');
    },
    onError: (error) => setErro(extractErrorMessage(error)),
  });

  function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setErro(null);
    setSucesso(false);
    configurar.mutate();
  }

  return (
    <div>
      <PageHeader
        title="WhatsApp Integrado"
        subtitle="Agendamentos automáticos, 24 horas por dia, sem você digitar nada"
      />

      <Card className="mb-6">
        <div className="flex flex-wrap items-center justify-between gap-3">
          <div>
            <p className="mb-1 text-xs font-medium uppercase text-faint">Status da conexão</p>
            {isLoading ? (
              <p className="text-sm text-faint">Carregando…</p>
            ) : (
              <div className="flex items-center gap-3">
                <Badge tone={status?.conectado ? 'green' : 'slate'}>
                  {status?.conectado ? 'Conectado' : 'Não conectado'}
                </Badge>
                {status?.instanceId && <span className="text-sm text-faint">Instance ID: {status.instanceId}</span>}
              </div>
            )}
          </div>
        </div>
      </Card>

      <div className="mb-6 grid grid-cols-1 gap-4 xl:grid-cols-2">
        <Card>
          <p className="mb-4 text-sm font-semibold text-ink">Como funciona</p>
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            {COMO_FUNCIONA.map((passo, i) => (
              <div key={passo.titulo} className="flex gap-3">
                <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-whatsapp/15 text-sm font-bold text-whatsapp">
                  {i + 1}
                </div>
                <div>
                  <p className="text-sm font-semibold text-ink">{passo.titulo}</p>
                  <p className="mt-0.5 text-sm text-muted">{passo.descricao}</p>
                </div>
              </div>
            ))}
          </div>
        </Card>

        <Card>
          <p className="mb-4 text-sm font-semibold text-ink">Estatísticas do WhatsApp</p>
          <div className="grid grid-cols-2 gap-4">
            <StatBox label="Conversas hoje" value={`${estatisticas?.conversasHoje ?? 0}`} />
            <StatBox label="Agendamentos hoje" value={`${estatisticas?.agendamentosHoje ?? 0}`} />
            <StatBox label="Agendamentos no mês" value={`${estatisticas?.agendamentosMes ?? 0}`} />
            <StatBox label="Taxa de conversão" value={`${(estatisticas?.taxaConversaoPercentual ?? 0).toFixed(1)}%`} />
          </div>
        </Card>
      </div>

      <div className="mb-6 grid grid-cols-1 gap-4 xl:grid-cols-3">
        <Card className="xl:col-span-1">
          <p className="mb-4 text-sm font-semibold text-ink">Última conversa</p>
          <ChatPreview />
        </Card>

        <Card>
          <p className="mb-4 text-sm font-semibold text-ink">Próximos agendamentos via WhatsApp</p>
          <ProximosAgendamentosWhatsApp />
        </Card>

        <Card>
          <p className="mb-4 text-sm font-semibold text-ink">Configurações</p>
          <ConfiguracoesWhatsApp />
        </Card>
      </div>

      <Card className="mb-6">
        <p className="mb-4 text-sm font-semibold text-ink">Mensagem de boas-vindas</p>
        <MensagemBoasVindas />
      </Card>

      <Card>
        <p className="mb-4 text-sm font-semibold text-ink">Configurar instância Z-API</p>
        <form onSubmit={handleSubmit} className="space-y-4">
          <Input label="Instance ID" value={instanceId} onChange={(e) => setInstanceId(e.target.value)} required />
          <Input label="Token" type="password" value={token} onChange={(e) => setToken(e.target.value)} required />
          <Input
            label="Client Token (opcional)"
            type="password"
            value={clientToken}
            onChange={(e) => setClientToken(e.target.value)}
          />
          {erro && <p className="text-sm text-rose">{erro}</p>}
          {sucesso && <p className="text-sm text-mint">Integração configurada com sucesso.</p>}
          <Button type="submit" disabled={configurar.isPending}>
            {configurar.isPending ? 'Salvando…' : 'Salvar configuração'}
          </Button>
        </form>
      </Card>
    </div>
  );
}
