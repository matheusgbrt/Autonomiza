import type { HTMLAttributes } from 'react';

export function Card({ className = '', ...props }: HTMLAttributes<HTMLDivElement>) {
  return (
    <div
      className={`rounded-2xl border border-stroke bg-surface p-6 ${className}`}
      {...props}
    />
  );
}
