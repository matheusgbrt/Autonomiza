import { apiClient } from './client';
import type {
  AtualizarLancamentoFinanceiroDto,
  CriarLancamentoFinanceiroDto,
  LancamentoFinanceiroDto,
  SaldoMensalDto,
} from './types';

export async function listarLancamentos(): Promise<LancamentoFinanceiroDto[]> {
  const { data } = await apiClient.get<LancamentoFinanceiroDto[]>('/api/lancamentos-financeiros');
  return data;
}

export async function criarLancamento(dto: CriarLancamentoFinanceiroDto): Promise<LancamentoFinanceiroDto> {
  const { data } = await apiClient.post<LancamentoFinanceiroDto>('/api/lancamentos-financeiros', dto);
  return data;
}

export async function atualizarLancamento(
  id: string,
  dto: AtualizarLancamentoFinanceiroDto,
): Promise<LancamentoFinanceiroDto> {
  const { data } = await apiClient.put<LancamentoFinanceiroDto>(`/api/lancamentos-financeiros/${id}`, dto);
  return data;
}

export async function removerLancamento(id: string): Promise<void> {
  await apiClient.delete(`/api/lancamentos-financeiros/${id}`);
}

export async function obterSaldoMensal(ano?: number, mes?: number): Promise<SaldoMensalDto> {
  const { data } = await apiClient.get<SaldoMensalDto>('/api/lancamentos-financeiros/saldo', {
    params: { ano, mes },
  });
  return data;
}
