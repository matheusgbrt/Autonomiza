import { apiClient } from './client';
import type { AtualizarServicoDto, CriarServicoDto, ServicoDto } from './types';

export async function listarServicos(): Promise<ServicoDto[]> {
  const { data } = await apiClient.get<ServicoDto[]>('/api/servicos');
  return data;
}

export async function criarServico(dto: CriarServicoDto): Promise<ServicoDto> {
  const { data } = await apiClient.post<ServicoDto>('/api/servicos', dto);
  return data;
}

export async function atualizarServico(id: string, dto: AtualizarServicoDto): Promise<ServicoDto> {
  const { data } = await apiClient.put<ServicoDto>(`/api/servicos/${id}`, dto);
  return data;
}

export async function removerServico(id: string): Promise<void> {
  await apiClient.delete(`/api/servicos/${id}`);
}
