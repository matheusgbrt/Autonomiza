import { useState, type FormEvent } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';
import { extractErrorMessage } from '../api/client';
import { Button } from '../components/ui/Button';
import { Card } from '../components/ui/Card';
import { Input } from '../components/ui/Input';
import { Logo } from '../components/Logo';

export function RegisterPage() {
  const { registrar } = useAuth();
  const navigate = useNavigate();

  const [nome, setNome] = useState('');
  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');
  const [erro, setErro] = useState<string | null>(null);
  const [carregando, setCarregando] = useState(false);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setErro(null);
    setCarregando(true);
    try {
      await registrar({ nome, email, senha });
      navigate('/', { replace: true });
    } catch (error) {
      setErro(extractErrorMessage(error));
    } finally {
      setCarregando(false);
    }
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-slate-50 px-4">
      <Card className="w-full max-w-sm">
        <Logo size={32} className="mb-4" />
        <h1 className="mb-1 text-xl font-bold text-slate-900">Criar conta</h1>
        <p className="mb-6 text-sm text-slate-500">Comece grátis no Autonomiza</p>

        <form onSubmit={handleSubmit} className="space-y-4">
          <Input label="Nome" value={nome} onChange={(e) => setNome(e.target.value)} required />
          <Input
            label="E-mail"
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />
          <Input
            label="Senha"
            type="password"
            value={senha}
            onChange={(e) => setSenha(e.target.value)}
            minLength={8}
            required
          />

          {erro && <p className="text-sm text-red-600">{erro}</p>}

          <Button type="submit" disabled={carregando} className="w-full">
            {carregando ? 'Criando…' : 'Criar conta'}
          </Button>
        </form>

        <p className="mt-6 text-center text-sm text-slate-500">
          Já tem conta?{' '}
          <Link to="/login" className="font-medium text-indigo-600 hover:text-indigo-500">
            Entrar
          </Link>
        </p>
      </Card>
    </div>
  );
}
