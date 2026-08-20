import { apiClient } from './client';
import type {
  AtualizarItemChecklistDto,
  AtualizarTarefaDto,
  CriarItemChecklistDto,
  CriarTarefaDto,
  ItemChecklistDto,
  TarefaDto,
} from './types';

export async function listarTarefas(): Promise<TarefaDto[]> {
  const { data } = await apiClient.get<TarefaDto[]>('/api/tarefas');
  return data;
}

export async function criarTarefa(dto: CriarTarefaDto): Promise<TarefaDto> {
  const { data } = await apiClient.post<TarefaDto>('/api/tarefas', dto);
  return data;
}

export async function atualizarTarefa(id: string, dto: AtualizarTarefaDto): Promise<TarefaDto> {
  const { data } = await apiClient.put<TarefaDto>(`/api/tarefas/${id}`, dto);
  return data;
}

export async function removerTarefa(id: string): Promise<void> {
  await apiClient.delete(`/api/tarefas/${id}`);
}

export async function adicionarItem(tarefaId: string, dto: CriarItemChecklistDto): Promise<ItemChecklistDto> {
  const { data } = await apiClient.post<ItemChecklistDto>(`/api/tarefas/${tarefaId}/itens`, dto);
  return data;
}

export async function atualizarItem(
  tarefaId: string,
  itemId: string,
  dto: AtualizarItemChecklistDto,
): Promise<ItemChecklistDto> {
  const { data } = await apiClient.put<ItemChecklistDto>(`/api/tarefas/${tarefaId}/itens/${itemId}`, dto);
  return data;
}

export async function removerItem(tarefaId: string, itemId: string): Promise<void> {
  await apiClient.delete(`/api/tarefas/${tarefaId}/itens/${itemId}`);
}
