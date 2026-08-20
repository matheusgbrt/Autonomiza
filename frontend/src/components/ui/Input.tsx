import type { ChangeEvent, InputHTMLAttributes, LabelHTMLAttributes, SelectHTMLAttributes, TextareaHTMLAttributes } from 'react';
import { apenasDigitos, formatarTelefone } from '../../utils/mask';

interface FieldWrapperProps {
  label: string;
  error?: string;
  children: React.ReactNode;
}

function FieldWrapper({ label, error, children }: FieldWrapperProps) {
  return (
    <label className="block text-sm">
      <span className="mb-1 block font-medium text-muted">{label}</span>
      {children}
      {error && <span className="mt-1 block text-xs text-rose">{error}</span>}
    </label>
  );
}

interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
  label: string;
  error?: string;
}

export function Input({ label, error, className = '', ...props }: InputProps) {
  return (
    <FieldWrapper label={label} error={error}>
      <input
        className={`w-full rounded-lg border border-stroke bg-elevated px-3 py-2 text-sm text-ink placeholder:text-faint focus:border-indigo focus:outline-none focus:ring-1 focus:ring-indigo ${className}`}
        {...props}
      />
    </FieldWrapper>
  );
}

interface TextareaProps extends TextareaHTMLAttributes<HTMLTextAreaElement> {
  label: string;
  error?: string;
}

export function Textarea({ label, error, className = '', ...props }: TextareaProps) {
  return (
    <FieldWrapper label={label} error={error}>
      <textarea
        className={`w-full rounded-lg border border-stroke bg-elevated px-3 py-2 text-sm text-ink placeholder:text-faint focus:border-indigo focus:outline-none focus:ring-1 focus:ring-indigo ${className}`}
        {...props}
      />
    </FieldWrapper>
  );
}

interface SelectProps extends SelectHTMLAttributes<HTMLSelectElement> {
  label: string;
  error?: string;
}

export function Select({ label, error, className = '', children, ...props }: SelectProps) {
  return (
    <FieldWrapper label={label} error={error}>
      <select
        className={`w-full rounded-lg border border-stroke bg-elevated px-3 py-2 text-sm text-ink focus:border-indigo focus:outline-none focus:ring-1 focus:ring-indigo ${className}`}
        {...props}
      >
        {children}
      </select>
    </FieldWrapper>
  );
}

export function FieldLabel(props: LabelHTMLAttributes<HTMLLabelElement>) {
  return <label className="mb-1 block text-sm font-medium text-muted" {...props} />;
}

interface PhoneInputProps {
  label: string;
  value: string;
  onChange: (digitos: string) => void;
  error?: string;
  required?: boolean;
}

/** Input de telefone com máscara brasileira progressiva — o valor mantido no estado é só dígitos. */
export function PhoneInput({ label, value, onChange, error, required }: PhoneInputProps) {
  return (
    <FieldWrapper label={label} error={error}>
      <input
        type="tel"
        inputMode="tel"
        value={formatarTelefone(value)}
        onChange={(e) => onChange(apenasDigitos(e.target.value).slice(0, 13))}
        required={required}
        placeholder="(11) 91234-5678"
        className="w-full rounded-lg border border-stroke bg-elevated px-3 py-2 text-sm text-ink placeholder:text-faint focus:border-indigo focus:outline-none focus:ring-1 focus:ring-indigo"
      />
    </FieldWrapper>
  );
}

interface CurrencyInputProps {
  label: string;
  value: string;
  onChange: (valorDecimal: string) => void;
  error?: string;
  required?: boolean;
}

/** Input monetário (R$) com máscara — dígitos preenchem da direita para a esquerda, como em apps bancários. */
export function CurrencyInput({ label, value, onChange, error, required }: CurrencyInputProps) {
  const temValor = value !== '' && value !== null && value !== undefined && !Number.isNaN(Number(value));
  const exibicao = temValor
    ? (Math.round(Number(value) * 100) / 100).toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
    : '';

  function handleChange(e: ChangeEvent<HTMLInputElement>) {
    const digitos = apenasDigitos(e.target.value);
    if (digitos === '') {
      onChange('');
      return;
    }
    onChange((parseInt(digitos, 10) / 100).toFixed(2));
  }

  return (
    <FieldWrapper label={label} error={error}>
      <div className="relative">
        <span className="pointer-events-none absolute top-1/2 left-3 -translate-y-1/2 text-sm text-faint">R$</span>
        <input
          type="text"
          inputMode="decimal"
          value={exibicao}
          onChange={handleChange}
          required={required}
          placeholder="0,00"
          className="w-full rounded-lg border border-stroke bg-elevated py-2 pr-3 pl-10 text-sm text-ink placeholder:text-faint focus:border-indigo focus:outline-none focus:ring-1 focus:ring-indigo"
        />
      </div>
    </FieldWrapper>
  );
}
