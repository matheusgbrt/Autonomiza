import { apiClient } from './client';
import type { AgendamentoDto, AtualizarAgendamentoDto, CriarAgendamentoDto } from './types';

export async function listarAgendamentos(periodo?: { inicio: string; fim: string }): Promise<AgendamentoDto[]> {
  const { data } = await apiClient.get<AgendamentoDto[]>('/api/agendamentos', {
    params: periodo,
  });
  return data;
}

export async function criarAgendamento(dto: CriarAgendamentoDto): Promise<AgendamentoDto> {
  const { data } = await apiClient.post<AgendamentoDto>('/api/agendamentos', dto);
  return data;
}

export async function atualizarAgendamento(id: string, dto: AtualizarAgendamentoDto): Promise<AgendamentoDto> {
  const { data } = await apiClient.put<AgendamentoDto>(`/api/agendamentos/${id}`, dto);
  return data;
}

export async function removerAgendamento(id: string): Promise<void> {
  await apiClient.delete(`/api/agendamentos/${id}`);
}
