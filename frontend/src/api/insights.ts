import { apiClient } from './client';
import type { InsightsResponseDto } from './types';

export async function obterInsights(): Promise<InsightsResponseDto> {
  const { data } = await apiClient.get<InsightsResponseDto>('/api/pro/insights');
  return data;
}
