import { apiClient } from './client';
import type { DashboardAvancadoDto, ResumoDashboardDto } from './types';

export async function obterResumo(): Promise<ResumoDashboardDto> {
  const { data } = await apiClient.get<ResumoDashboardDto>('/api/dashboard/resumo');
  return data;
}

export async function obterAvancado(): Promise<DashboardAvancadoDto> {
  const { data } = await apiClient.get<DashboardAvancadoDto>('/api/pro/dashboard/avancado');
  return data;
}
