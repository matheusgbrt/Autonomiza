/** Remove tudo que não for dígito. */
export function apenasDigitos(valor: string): string {
  return valor.replace(/\D/g, '');
}

/**
 * Formata um telefone brasileiro progressivamente a partir dos dígitos já digitados.
 * Aceita números com ou sem código do país (55) e com 8 ou 9 dígitos locais,
 * então funciona tanto para exibir um valor já salvo quanto para mascarar um input em digitação.
 */
export function formatarTelefone(valor: string): string {
  let digitos = apenasDigitos(valor).slice(0, 13);
  let prefixo = '';

  if (digitos.length > 11 && digitos.startsWith('55')) {
    prefixo = '+55 ';
    digitos = digitos.slice(2);
  }

  if (digitos.length === 0) return '';
  if (digitos.length <= 2) return `${prefixo}(${digitos}`;
  if (digitos.length <= 6) return `${prefixo}(${digitos.slice(0, 2)}) ${digitos.slice(2)}`;
  if (digitos.length <= 10) return `${prefixo}(${digitos.slice(0, 2)}) ${digitos.slice(2, 6)}-${digitos.slice(6)}`;
  return `${prefixo}(${digitos.slice(0, 2)}) ${digitos.slice(2, 7)}-${digitos.slice(7, 11)}`;
}
