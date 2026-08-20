import { apiClient } from './client';
import type {
  AtualizarConfiguracaoWhatsAppDto,
  ConfigurarWhatsAppDto,
  ConfiguracaoWhatsAppDto,
  EstatisticasWhatsAppDto,
  MensagemWhatsAppDto,
  StatusIntegracaoWhatsAppDto,
} from './types';

export async function obterStatus(): Promise<StatusIntegracaoWhatsAppDto> {
  const { data } = await apiClient.get<StatusIntegracaoWhatsAppDto>('/api/integracoes/whatsapp');
  return data;
}

export async function configurar(dto: ConfigurarWhatsAppDto): Promise<void> {
  await apiClient.put('/api/integracoes/whatsapp', dto);
}

export async function obterEstatisticas(): Promise<EstatisticasWhatsAppDto> {
  const { data } = await apiClient.get<EstatisticasWhatsAppDto>('/api/integracoes/whatsapp/estatisticas');
  return data;
}

export async function obterUltimaConversa(): Promise<MensagemWhatsAppDto[]> {
  const { data } = await apiClient.get<MensagemWhatsAppDto[]>('/api/integracoes/whatsapp/mensagens');
  return data;
}

export async function obterConfiguracao(): Promise<ConfiguracaoWhatsAppDto> {
  const { data } = await apiClient.get<ConfiguracaoWhatsAppDto>('/api/integracoes/whatsapp/configuracoes');
  return data;
}

export async function atualizarConfiguracao(dto: AtualizarConfiguracaoWhatsAppDto): Promise<void> {
  await apiClient.put('/api/integracoes/whatsapp/configuracoes', dto);
}
