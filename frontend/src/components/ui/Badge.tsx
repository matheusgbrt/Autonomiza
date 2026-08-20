type Tone = 'slate' | 'green' | 'amber' | 'red' | 'indigo' | 'violet' | 'cyan';

const TONE_CLASSES: Record<Tone, string> = {
  slate: 'bg-elevated text-muted',
  green: 'bg-mint/15 text-mint',
  amber: 'bg-amber/15 text-amber',
  red: 'bg-rose/15 text-rose',
  indigo: 'bg-indigo/15 text-indigo',
  violet: 'bg-violet/15 text-violet',
  cyan: 'bg-cyan/15 text-cyan',
};

export function Badge({ tone = 'slate', children }: { tone?: Tone; children: React.ReactNode }) {
  return (
    <span className={`inline-block rounded-full px-2.5 py-0.5 text-xs font-medium ${TONE_CLASSES[tone]}`}>
      {children}
    </span>
  );
}
