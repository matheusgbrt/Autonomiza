import { apiClient } from './client';
import type { ConfigurarWhatsAppDto, StatusIntegracaoWhatsAppDto } from './types';

export async function obterStatus(): Promise<StatusIntegracaoWhatsAppDto> {
  const { data } = await apiClient.get<StatusIntegracaoWhatsAppDto>('/api/integracoes/whatsapp');
  return data;
}

export async function configurar(dto: ConfigurarWhatsAppDto): Promise<void> {
  await apiClient.put('/api/integracoes/whatsapp', dto);
}
