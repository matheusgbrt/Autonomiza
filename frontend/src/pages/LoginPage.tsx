import { useState, type FormEvent } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';
import { extractErrorMessage } from '../api/client';
import { Button } from '../components/ui/Button';
import { Card } from '../components/ui/Card';
import { Input } from '../components/ui/Input';
import { Logo } from '../components/Logo';

export function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');
  const [erro, setErro] = useState<string | null>(null);
  const [carregando, setCarregando] = useState(false);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setErro(null);
    setCarregando(true);
    try {
      await login({ email, senha });
      const destino = (location.state as { from?: Location })?.from?.pathname ?? '/';
      navigate(destino, { replace: true });
    } catch (error) {
      setErro(extractErrorMessage(error));
    } finally {
      setCarregando(false);
    }
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-base px-4">
      <Card className="w-full max-w-sm">
        <Logo size={36} className="mb-4" />
        <p className="mb-6 text-sm text-faint">Entre na sua conta</p>

        <form onSubmit={handleSubmit} className="space-y-4">
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
            required
          />

          {erro && <p className="text-sm text-rose">{erro}</p>}

          <Button type="submit" disabled={carregando} className="w-full">
            {carregando ? 'Entrando…' : 'Entrar'}
          </Button>
        </form>

        <p className="mt-6 text-center text-sm text-faint">
          Ainda não tem conta?{' '}
          <Link to="/registrar" className="font-medium text-indigo hover:text-indigo/80">
            Criar conta
          </Link>
        </p>
      </Card>
    </div>
  );
}
