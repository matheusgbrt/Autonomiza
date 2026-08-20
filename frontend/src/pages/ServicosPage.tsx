import { useState, type FormEvent } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import * as servicosApi from '../api/servicos';
import type { ServicoDto } from '../api/types';
import { extractErrorMessage } from '../api/client';
import { Button } from '../components/ui/Button';
import { Input, Textarea } from '../components/ui/Input';
import { Modal } from '../components/ui/Modal';
import { Table, Thead, Tbody, Tr, Th, Td } from '../components/ui/Table';
import { PageHeader } from '../components/PageHeader';

interface FormState {
  nome: string;
  descricao: string;
  duracao: string; // "HH:mm" do <input type="time">
  valorPadrao: string;
}

const FORM_VAZIO: FormState = { nome: '', descricao: '', duracao: '00:30', valorPadrao: '' };

function formatarDuracao(duracaoIso: string) {
  return duracaoIso.slice(0, 5);
}

export function ServicosPage() {
  const queryClient = useQueryClient();
  const { data: servicos, isLoading } = useQuery({ queryKey: ['servicos'], queryFn: servicosApi.listarServicos });

  const [modalAberto, setModalAberto] = useState(false);
  const [editando, setEditando] = useState<ServicoDto | null>(null);
  const [form, setForm] = useState<FormState>(FORM_VAZIO);
  const [erro, setErro] = useState<string | null>(null);

  const salvar = useMutation({
    mutationFn: async () => {
      const dto = {
        nome: form.nome,
        descricao: form.descricao || null,
        duracao: `${form.duracao}:00`,
        valorPadrao: Number(form.valorPadrao),
      };
      return editando ? servicosApi.atualizarServico(editando.id, dto) : servicosApi.criarServico(dto);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['servicos'] });
      setModalAberto(false);
    },
    onError: (error) => setErro(extractErrorMessage(error)),
  });

  const remover = useMutation({
    mutationFn: (id: string) => servicosApi.removerServico(id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['servicos'] }),
  });

  function abrirNovo() {
    setEditando(null);
    setForm(FORM_VAZIO);
    setErro(null);
    setModalAberto(true);
  }

  function abrirEdicao(servico: ServicoDto) {
    setEditando(servico);
    setForm({
      nome: servico.nome,
      descricao: servico.descricao ?? '',
      duracao: formatarDuracao(servico.duracao),
      valorPadrao: String(servico.valorPadrao),
    });
    setErro(null);
    setModalAberto(true);
  }

  function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setErro(null);
    salvar.mutate();
  }

  return (
    <div>
      <PageHeader
        title="Serviços"
        subtitle="Catálogo de serviços com duração e valor padrão"
        action={<Button onClick={abrirNovo}>Novo serviço</Button>}
      />

      {isLoading ? (
        <p className="text-sm text-slate-500">Carregando…</p>
      ) : (
        <Table>
          <Thead>
            <Tr>
              <Th>Nome</Th>
              <Th>Duração</Th>
              <Th>Valor padrão</Th>
              <Th />
            </Tr>
          </Thead>
          <Tbody>
            {servicos?.length === 0 && (
              <Tr>
                <Td colSpan={4} className="text-center text-slate-400">
                  Nenhum serviço cadastrado ainda.
                </Td>
              </Tr>
            )}
            {servicos?.map((servico) => (
              <Tr key={servico.id}>
                <Td className="font-medium text-slate-900">{servico.nome}</Td>
                <Td>{formatarDuracao(servico.duracao)}</Td>
                <Td>{servico.valorPadrao.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}</Td>
                <Td className="text-right">
                  <div className="flex justify-end gap-2">
                    <Button variant="ghost" onClick={() => abrirEdicao(servico)}>
                      Editar
                    </Button>
                    <Button variant="ghost" onClick={() => remover.mutate(servico.id)}>
                      Remover
                    </Button>
                  </div>
                </Td>
              </Tr>
            ))}
          </Tbody>
        </Table>
      )}

      {modalAberto && (
        <Modal title={editando ? 'Editar serviço' : 'Novo serviço'} onClose={() => setModalAberto(false)}>
          <form onSubmit={handleSubmit} className="space-y-4">
            <Input label="Nome" value={form.nome} onChange={(e) => setForm({ ...form, nome: e.target.value })} required />
            <Textarea
              label="Descrição"
              value={form.descricao}
              onChange={(e) => setForm({ ...form, descricao: e.target.value })}
              rows={2}
            />
            <div className="grid grid-cols-2 gap-4">
              <Input
                label="Duração"
                type="time"
                value={form.duracao}
                onChange={(e) => setForm({ ...form, duracao: e.target.value })}
                required
              />
              <Input
                label="Valor padrão (R$)"
                type="number"
                step="0.01"
                min="0"
                value={form.valorPadrao}
                onChange={(e) => setForm({ ...form, valorPadrao: e.target.value })}
                required
              />
            </div>
            {erro && <p className="text-sm text-red-600">{erro}</p>}
            <div className="flex justify-end gap-2 pt-2">
              <Button type="button" variant="secondary" onClick={() => setModalAberto(false)}>
                Cancelar
              </Button>
              <Button type="submit" disabled={salvar.isPending}>
                {salvar.isPending ? 'Salvando…' : 'Salvar'}
              </Button>
            </div>
          </form>
        </Modal>
      )}
    </div>
  );
}
