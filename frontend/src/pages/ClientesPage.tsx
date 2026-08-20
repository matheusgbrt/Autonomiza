import { useState, type FormEvent } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import * as clientesApi from '../api/clientes';
import type { ClienteDto } from '../api/types';
import { extractErrorMessage } from '../api/client';
import { Button } from '../components/ui/Button';
import { Input, Textarea } from '../components/ui/Input';
import { Modal } from '../components/ui/Modal';
import { Table, Thead, Tbody, Tr, Th, Td } from '../components/ui/Table';
import { PageHeader } from '../components/PageHeader';

interface FormState {
  nome: string;
  email: string;
  telefone: string;
  observacoes: string;
}

const FORM_VAZIO: FormState = { nome: '', email: '', telefone: '', observacoes: '' };

export function ClientesPage() {
  const queryClient = useQueryClient();
  const { data: clientes, isLoading } = useQuery({ queryKey: ['clientes'], queryFn: clientesApi.listarClientes });

  const [modalAberto, setModalAberto] = useState(false);
  const [editando, setEditando] = useState<ClienteDto | null>(null);
  const [form, setForm] = useState<FormState>(FORM_VAZIO);
  const [erro, setErro] = useState<string | null>(null);

  const salvar = useMutation({
    mutationFn: async () => {
      const dto = {
        nome: form.nome,
        email: form.email || null,
        telefone: form.telefone || null,
        observacoes: form.observacoes || null,
      };
      return editando ? clientesApi.atualizarCliente(editando.id, dto) : clientesApi.criarCliente(dto);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['clientes'] });
      fecharModal();
    },
    onError: (error) => setErro(extractErrorMessage(error)),
  });

  const remover = useMutation({
    mutationFn: (id: string) => clientesApi.removerCliente(id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['clientes'] }),
  });

  function abrirNovo() {
    setEditando(null);
    setForm(FORM_VAZIO);
    setErro(null);
    setModalAberto(true);
  }

  function abrirEdicao(cliente: ClienteDto) {
    setEditando(cliente);
    setForm({
      nome: cliente.nome,
      email: cliente.email ?? '',
      telefone: cliente.telefone ?? '',
      observacoes: cliente.observacoes ?? '',
    });
    setErro(null);
    setModalAberto(true);
  }

  function fecharModal() {
    setModalAberto(false);
  }

  function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setErro(null);
    salvar.mutate();
  }

  return (
    <div>
      <PageHeader
        title="Clientes"
        subtitle="Registro básico de contatos e histórico de atendimentos"
        action={<Button onClick={abrirNovo}>Novo cliente</Button>}
      />

      {isLoading ? (
        <p className="text-sm text-slate-500">Carregando…</p>
      ) : (
        <Table>
          <Thead>
            <Tr>
              <Th>Nome</Th>
              <Th>E-mail</Th>
              <Th>Telefone</Th>
              <Th />
            </Tr>
          </Thead>
          <Tbody>
            {clientes?.length === 0 && (
              <Tr>
                <Td colSpan={4} className="text-center text-slate-400">
                  Nenhum cliente cadastrado ainda.
                </Td>
              </Tr>
            )}
            {clientes?.map((cliente) => (
              <Tr key={cliente.id}>
                <Td className="font-medium text-slate-900">
                  <Link to={`/clientes/${cliente.id}`} className="hover:text-indigo-600">
                    {cliente.nome}
                  </Link>
                </Td>
                <Td>{cliente.email ?? '—'}</Td>
                <Td>{cliente.telefone ?? '—'}</Td>
                <Td className="text-right">
                  <div className="flex justify-end gap-2">
                    <Button variant="ghost" onClick={() => abrirEdicao(cliente)}>
                      Editar
                    </Button>
                    <Button variant="ghost" onClick={() => remover.mutate(cliente.id)}>
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
        <Modal title={editando ? 'Editar cliente' : 'Novo cliente'} onClose={fecharModal}>
          <form onSubmit={handleSubmit} className="space-y-4">
            <Input label="Nome" value={form.nome} onChange={(e) => setForm({ ...form, nome: e.target.value })} required />
            <Input
              label="E-mail"
              type="email"
              value={form.email}
              onChange={(e) => setForm({ ...form, email: e.target.value })}
            />
            <Input
              label="Telefone"
              value={form.telefone}
              onChange={(e) => setForm({ ...form, telefone: e.target.value })}
              placeholder="Ex: 5511999999999"
            />
            <Textarea
              label="Observações"
              value={form.observacoes}
              onChange={(e) => setForm({ ...form, observacoes: e.target.value })}
              rows={3}
            />
            {erro && <p className="text-sm text-red-600">{erro}</p>}
            <div className="flex justify-end gap-2 pt-2">
              <Button type="button" variant="secondary" onClick={fecharModal}>
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
