import type { HTMLAttributes, TdHTMLAttributes, ThHTMLAttributes } from 'react';

export function Table({ className = '', ...props }: HTMLAttributes<HTMLTableElement>) {
  return (
    <div className="overflow-x-auto rounded-2xl border border-stroke bg-surface">
      <table className={`w-full min-w-max text-left text-sm ${className}`} {...props} />
    </div>
  );
}

export function Thead({ className = '', ...props }: HTMLAttributes<HTMLTableSectionElement>) {
  return <thead className={`border-b border-stroke bg-elevated text-xs uppercase tracking-wide text-faint ${className}`} {...props} />;
}

export function Th({ className = '', ...props }: ThHTMLAttributes<HTMLTableCellElement>) {
  return <th className={`px-4 py-3 font-semibold ${className}`} {...props} />;
}

export function Tbody({ className = '', ...props }: HTMLAttributes<HTMLTableSectionElement>) {
  return <tbody className={`divide-y divide-stroke ${className}`} {...props} />;
}

export function Tr({ className = '', ...props }: HTMLAttributes<HTMLTableRowElement>) {
  return <tr className={`hover:bg-elevated ${className}`} {...props} />;
}

export function Td({ className = '', ...props }: TdHTMLAttributes<HTMLTableCellElement>) {
  return <td className={`px-4 py-3 align-middle text-muted ${className}`} {...props} />;
}
