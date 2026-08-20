export function ProgressBar({ percentual }: { percentual: number }) {
  const largura = Math.max(0, Math.min(100, percentual));
  const cor = largura >= 100 ? 'bg-emerald-500' : largura >= 50 ? 'bg-indigo-500' : 'bg-amber-500';

  return (
    <div className="h-2 w-full overflow-hidden rounded-full bg-slate-100">
      <div className={`h-full rounded-full ${cor} transition-all`} style={{ width: `${largura}%` }} />
    </div>
  );
}
