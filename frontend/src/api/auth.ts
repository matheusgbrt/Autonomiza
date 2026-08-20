import { apiClient } from './client';
import type { AuthResponseDto, LoginDto, RegistrarUsuarioDto, SimularPlanoDto } from './types';

export async function registrar(dto: RegistrarUsuarioDto): Promise<AuthResponseDto> {
  const { data } = await apiClient.post<AuthResponseDto>('/api/auth/registrar', dto);
  return data;
}

export async function login(dto: LoginDto): Promise<AuthResponseDto> {
  const { data } = await apiClient.post<AuthResponseDto>('/api/auth/login', dto);
  return data;
}

export async function simularPlano(dto: SimularPlanoDto): Promise<AuthResponseDto> {
  const { data } = await apiClient.post<AuthResponseDto>('/api/auth/simular-plano', dto);
  return data;
}
