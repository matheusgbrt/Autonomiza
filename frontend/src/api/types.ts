// Espelha os DTOs do backend (GestaoAutonomo.Application/DTOs) 1:1.
// Enums são serializados como número pelo System.Text.Json (sem conversor de string),
// então a ordem dos membros aqui precisa bater exatamente com o enum em C#.

export const StatusAgendamento = {
  Agendado: 0,
  Confirmado: 1,
  Concluido: 2,
  Cancelado: 3,
} as const;
export type StatusAgendamento = (typeof StatusAgendamento)[keyof typeof StatusAgendamento];

export const TipoLancamento = {
  Entrada: 0,
  Saida: 1,
} as const;
export type TipoLancamento = (typeof TipoLancamento)[keyof typeof TipoLancamento];

export const TipoMeta = {
  Faturamento: 0,
  Atendimentos: 1,
} as const;
export type TipoMeta = (typeof TipoMeta)[keyof typeof TipoMeta];

export const CategoriaInsight = {
  Cancelamento: 0,
  HorarioOcioso: 1,
  TendenciaReceita: 2,
} as const;
export type CategoriaInsight = (typeof CategoriaInsight)[keyof typeof CategoriaInsight];

export const CategoriaRecomendacao = {
  PacoteSugerido: 0,
  OtimizacaoHorarioPico: 1,
} as const;
export type CategoriaRecomendacao = (typeof CategoriaRecomendacao)[keyof typeof CategoriaRecomendacao];

export type Plano = 'Free' | 'Pro';

// ---------- Auth ----------
export interface RegistrarUsuarioDto {
  nome: string;
  email: string;
  senha: string;
}

export interface LoginDto {
  email: string;
  senha: string;
}

export interface AuthResponseDto {
  token: string;
  expiraEm: string;
  usuarioId: string;
  nome: string;
  email: string;
  plano: Plano;
}

export interface SimularPlanoDto {
  plano: 0 | 1; // Plano.Free = 0, Plano.Pro = 1
}

// ---------- Cliente ----------
export interface ClienteDto {
  id: string;
  nome: string;
  email: string | null;
  telefone: string | null;
  observacoes: string | null;
  createdAt: string;
}

export interface CriarClienteDto {
  nome: string;
  email?: string | null;
  telefone?: string | null;
  observacoes?: string | null;
}

export type AtualizarClienteDto = CriarClienteDto;

// ---------- Servico ----------
export interface ServicoDto {
  id: string;
  nome: string;
  descricao: string | null;
  duracao: string; // "HH:mm:ss"
  valorPadrao: number;
  createdAt: string;
}

export interface CriarServicoDto {
  nome: string;
  descricao?: string | null;
  duracao: string; // "HH:mm:ss"
  valorPadrao: number;
}

export type AtualizarServicoDto = CriarServicoDto;

// ---------- Agendamento ----------
export interface AgendamentoDto {
  id: string;
  clienteId: string;
  clienteNome: string;
  servicoId: string;
  servicoNome: string;
  dataHoraInicio: string;
  dataHoraFim: string;
  status: StatusAgendamento;
  observacoes: string | null;
  notaAtendimento: number | null;
  createdAt: string;
}

export interface CriarAgendamentoDto {
  clienteId: string;
  servicoId: string;
  dataHoraInicio: string;
  observacoes?: string | null;
}

export interface AtualizarAgendamentoDto {
  clienteId: string;
  servicoId: string;
  dataHoraInicio: string;
  status: StatusAgendamento;
  observacoes?: string | null;
  notaAtendimento?: number | null;
}

// ---------- Financeiro ----------
export interface LancamentoFinanceiroDto {
  id: string;
  tipo: TipoLancamento;
  categoria: string;
  valor: number;
  data: string;
  descricao: string | null;
  clienteId: string | null;
  agendamentoId: string | null;
  createdAt: string;
}

export interface CriarLancamentoFinanceiroDto {
  tipo: TipoLancamento;
  categoria: string;
  valor: number;
  data: string;
  descricao?: string | null;
  clienteId?: string | null;
  agendamentoId?: string | null;
}

export type AtualizarLancamentoFinanceiroDto = CriarLancamentoFinanceiroDto;

export interface SaldoCategoriaDto {
  categoria: string;
  totalEntradas: number;
  totalSaidas: number;
}

export interface SaldoMensalDto {
  ano: number;
  mes: number;
  totalEntradas: number;
  totalSaidas: number;
  saldo: number;
  porCategoria: SaldoCategoriaDto[];
}

// ---------- Tarefa ----------
export interface ItemChecklistDto {
  id: string;
  descricao: string;
  concluido: boolean;
  ordem: number;
}

export interface CriarItemChecklistDto {
  descricao: string;
}

export interface AtualizarItemChecklistDto {
  descricao: string;
  concluido: boolean;
  ordem: number;
}

export interface TarefaDto {
  id: string;
  titulo: string;
  descricao: string | null;
  concluida: boolean;
  dataVencimento: string | null;
  itens: ItemChecklistDto[];
  createdAt: string;
}

export interface CriarTarefaDto {
  titulo: string;
  descricao?: string | null;
  dataVencimento?: string | null;
  itensIniciais?: string[] | null;
}

export interface AtualizarTarefaDto {
  titulo: string;
  descricao?: string | null;
  concluida: boolean;
  dataVencimento?: string | null;
}

// ---------- Meta ----------
export interface MetaDto {
  id: string;
  tipo: TipoMeta;
  titulo: string;
  valorAlvo: number;
  valorAtual: number;
  progressoPercentual: number;
  periodoInicio: string;
  periodoFim: string;
  createdAt: string;
}

export interface CriarMetaDto {
  tipo: TipoMeta;
  titulo: string;
  valorAlvo: number;
  periodoInicio: string;
  periodoFim: string;
}

export type AtualizarMetaDto = CriarMetaDto;

// ---------- Dashboard ----------
export interface PontoSerieDto {
  data: string;
  total: number;
}

export interface ResumoDashboardDto {
  totalVendas: number;
  ticketMedio: number;
  quantidadeVendas: number;
  serieDiaria: PontoSerieDto[];
}

export interface RentabilidadeServicoDto {
  servicoId: string;
  servicoNome: string;
  receitaTotal: number;
  quantidadeAtendimentos: number;
  ticketMedio: number;
}

export interface PontoTrimestralDto {
  ano: number;
  trimestre: number;
  total: number;
}

export interface DashboardAvancadoDto {
  rentabilidadePorServico: RentabilidadeServicoDto[];
  taxaFidelizacaoPercentual: number;
  projecaoFaturamentoProximos30Dias: number;
  healthScoreNegocio: number;
  satisfacaoMedia: number | null;
  crescimentoReceitaTrimestral: PontoTrimestralDto[];
}

// ---------- IA Consultora ----------
export interface InsightDto {
  categoria: CategoriaInsight;
  mensagem: string;
}

export interface InsightsResponseDto {
  insights: InsightDto[];
  geradoEm: string;
  expiraEm: string;
  doCache: boolean;
}

// ---------- Recomendações ----------
export interface RecomendacaoDto {
  categoria: CategoriaRecomendacao;
  mensagem: string;
}

export interface RecomendacoesResponseDto {
  recomendacoes: RecomendacaoDto[];
  geradoEm: string;
  expiraEm: string;
  doCache: boolean;
}

// ---------- Integração WhatsApp ----------
export interface ConfigurarWhatsAppDto {
  instanceId: string;
  token: string;
  clientToken?: string | null;
}

export interface StatusIntegracaoWhatsAppDto {
  conectado: boolean;
  instanceId: string | null;
}

export interface EstatisticasWhatsAppDto {
  conversasHoje: number;
  agendamentosHoje: number;
  agendamentosMes: number;
  taxaConversaoPercentual: number;
}

export interface MensagemWhatsAppDto {
  clienteNome: string | null;
  telefone: string;
  direcao: 'Recebida' | 'Enviada';
  conteudo: string;
  criadoEm: string;
}

export interface ConfiguracaoWhatsAppDto {
  respostasAutomaticas: boolean;
  horariosDisponiveis: boolean;
  confirmarAgendamentos: boolean;
  lembretesAutomaticos: boolean;
  mensagemBoasVindas: string | null;
}

export type AtualizarConfiguracaoWhatsAppDto = ConfiguracaoWhatsAppDto;

// ---------- Erros ----------
export interface ProblemDetailsDto {
  title?: string;
  status?: number;
  errors?: Record<string, string[]>;
}
