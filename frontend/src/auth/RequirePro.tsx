import type { ReactNode } from 'react';
import { useState } from 'react';
import { useAuth } from './AuthContext';
import { Button } from '../components/ui/Button';
import { Card } from '../components/ui/Card';

export function RequirePro({ children }: { children: ReactNode }) {
  const { isPro, simularUpgrade } = useAuth();
  const [carregando, setCarregando] = useState(false);
  const [erro, setErro] = useState<string | null>(null);

  if (isPro) {
    return <>{children}</>;
  }

  async function handleSimularUpgrade() {
    setCarregando(true);
    setErro(null);
    try {
      await simularUpgrade('Pro');
    } catch {
      setErro('Não foi possível simular o upgrade agora.');
    } finally {
      setCarregando(false);
    }
  }

  return (
    <Card className="mx-auto mt-12 max-w-lg text-center">
      <div className="mb-3 inline-block rounded-full bg-amber-100 px-3 py-1 text-xs font-semibold uppercase tracking-wide text-amber-700">
        Recurso Pro
      </div>
      <h2 className="mb-2 text-xl font-semibold text-slate-900">
        Esse recurso faz parte do Autonomiza Pro IA
      </h2>
      <p className="mb-6 text-sm text-slate-600">
        Sua conta está no plano gratuito (Autonomiza). Este projeto ainda não tem
        cobrança real — use o botão abaixo só para fins de demonstração.
      </p>
      {erro && <p className="mb-4 text-sm text-red-600">{erro}</p>}
      <Button onClick={handleSimularUpgrade} disabled={carregando}>
        {carregando ? 'Simulando…' : 'Simular upgrade para Pro'}
      </Button>
    </Card>
  );
}
