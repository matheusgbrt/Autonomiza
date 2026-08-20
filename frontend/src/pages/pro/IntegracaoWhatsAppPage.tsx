import { useState, type FormEvent } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import * as integracaoApi from '../../api/integracaoWhatsapp';
import { extractErrorMessage } from '../../api/client';
import { Badge } from '../../components/ui/Badge';
import { Button } from '../../components/ui/Button';
import { Card } from '../../components/ui/Card';
import { Input } from '../../components/ui/Input';
import { PageHeader } from '../../components/PageHeader';

export function IntegracaoWhatsAppPage() {
  const queryClient = useQueryClient();
  const { data: status, isLoading } = useQuery({ queryKey: ['whatsapp-status'], queryFn: integracaoApi.obterStatus });

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
        title="Integração WhatsApp"
        subtitle="Automatização do fluxo de agendamentos — confirmações automáticas que reduzem faltas"
      />

      <Card className="mb-6">
        <p className="mb-1 text-xs font-medium uppercase text-slate-500">Status da conexão</p>
        {isLoading ? (
          <p className="text-sm text-slate-500">Carregando…</p>
        ) : (
          <div className="flex items-center gap-3">
            <Badge tone={status?.conectado ? 'green' : 'slate'}>
              {status?.conectado ? 'Conectado' : 'Não conectado'}
            </Badge>
            {status?.instanceId && <span className="text-sm text-slate-500">Instance ID: {status.instanceId}</span>}
          </div>
        )}
      </Card>

      <Card>
        <p className="mb-4 text-sm font-semibold text-slate-700">Configurar instância Z-API</p>
        <form onSubmit={handleSubmit} className="space-y-4">
          <Input label="Instance ID" value={instanceId} onChange={(e) => setInstanceId(e.target.value)} required />
          <Input label="Token" type="password" value={token} onChange={(e) => setToken(e.target.value)} required />
          <Input
            label="Client Token (opcional)"
            type="password"
            value={clientToken}
            onChange={(e) => setClientToken(e.target.value)}
          />
          {erro && <p className="text-sm text-red-600">{erro}</p>}
          {sucesso && <p className="text-sm text-emerald-600">Integração configurada com sucesso.</p>}
          <Button type="submit" disabled={configurar.isPending}>
            {configurar.isPending ? 'Salvando…' : 'Salvar configuração'}
          </Button>
        </form>
      </Card>
    </div>
  );
}
