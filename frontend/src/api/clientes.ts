import { apiClient } from './client';
import type { AgendamentoDto, AtualizarClienteDto, ClienteDto, CriarClienteDto } from './types';

export async function listarClientes(): Promise<ClienteDto[]> {
  const { data } = await apiClient.get<ClienteDto[]>('/api/clientes');
  return data;
}

export async function obterCliente(id: string): Promise<ClienteDto> {
  const { data } = await apiClient.get<ClienteDto>(`/api/clientes/${id}`);
  return data;
}

export async function criarCliente(dto: CriarClienteDto): Promise<ClienteDto> {
  const { data } = await apiClient.post<ClienteDto>('/api/clientes', dto);
  return data;
}

export async function atualizarCliente(id: string, dto: AtualizarClienteDto): Promise<ClienteDto> {
  const { data } = await apiClient.put<ClienteDto>(`/api/clientes/${id}`, dto);
  return data;
}

export async function removerCliente(id: string): Promise<void> {
  await apiClient.delete(`/api/clientes/${id}`);
}

export async function listarAgendamentosDoCliente(id: string): Promise<AgendamentoDto[]> {
  const { data } = await apiClient.get<AgendamentoDto[]>(`/api/clientes/${id}/agendamentos`);
  return data;
}
