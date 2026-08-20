import { apiClient } from './client';
import type { AtualizarMetaDto, CriarMetaDto, MetaDto } from './types';

export async function listarMetas(): Promise<MetaDto[]> {
  const { data } = await apiClient.get<MetaDto[]>('/api/metas');
  return data;
}

export async function criarMeta(dto: CriarMetaDto): Promise<MetaDto> {
  const { data } = await apiClient.post<MetaDto>('/api/metas', dto);
  return data;
}

export async function atualizarMeta(id: string, dto: AtualizarMetaDto): Promise<MetaDto> {
  const { data } = await apiClient.put<MetaDto>(`/api/metas/${id}`, dto);
  return data;
}

export async function removerMeta(id: string): Promise<void> {
  await apiClient.delete(`/api/metas/${id}`);
}
