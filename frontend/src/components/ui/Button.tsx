import type { ButtonHTMLAttributes } from 'react';

type Variant = 'primary' | 'secondary' | 'danger' | 'ghost';

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: Variant;
}

const VARIANT_CLASSES: Record<Variant, string> = {
  primary: 'bg-indigo text-white hover:bg-indigo/90 disabled:bg-indigo/40',
  secondary: 'bg-elevated text-muted border border-stroke hover:bg-glass disabled:text-faint',
  danger: 'bg-rose text-white hover:bg-rose/90 disabled:bg-rose/40',
  ghost: 'text-muted hover:bg-elevated disabled:text-faint',
};

export function Button({ variant = 'primary', className = '', ...props }: ButtonProps) {
  return (
    <button
      className={`inline-flex cursor-pointer items-center justify-center gap-2 rounded-lg px-4 py-2 text-sm font-medium transition-colors disabled:cursor-not-allowed ${VARIANT_CLASSES[variant]} ${className}`}
      {...props}
    />
  );
}
