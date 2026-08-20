import { apiClient } from './client';
import type { RecomendacoesResponseDto } from './types';

export async function obterRecomendacoes(): Promise<RecomendacoesResponseDto> {
  const { data } = await apiClient.get<RecomendacoesResponseDto>('/api/pro/recomendacoes');
  return data;
}
