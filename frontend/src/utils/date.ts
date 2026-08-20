/**
 * Converte o valor de um <input type="date"> ("YYYY-MM-DD") para um ISO datetime,
 * interpretando os componentes como horário LOCAL em vez de UTC. `new Date("YYYY-MM-DD")`
 * (parse direto da string) trata o valor como meia-noite UTC, o que, em fusos atrás de
 * UTC, faz o dia "voltar" quando exibido depois via toLocaleDateString — por isso não
 * usamos o parse direto aqui.
 */
export function dateOnlyInputToIso(value: string): string {
  const [year, month, day] = value.split('-').map(Number);
  return new Date(year, month - 1, day).toISOString();
}

/** Formata um Date como "YYYY-MM-DD" usando os componentes LOCAIS (não UTC), para uso em <input type="date">. */
export function toDateInputValue(date: Date): string {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  return `${year}-${month}-${day}`;
}
