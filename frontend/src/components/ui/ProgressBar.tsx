export function ProgressBar({ percentual }: { percentual: number }) {
  const largura = Math.max(0, Math.min(100, percentual));
  const cor = largura >= 100 ? 'bg-mint' : largura >= 50 ? 'bg-indigo' : 'bg-amber';

  return (
    <div className="h-2 w-full overflow-hidden rounded-full bg-elevated">
      <div className={`h-full rounded-full ${cor} transition-all`} style={{ width: `${largura}%` }} />
    </div>
  );
}
