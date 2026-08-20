import { createContext, useCallback, useContext, useMemo, useState, type ReactNode } from 'react';
import * as authApi from '../api/auth';
import { clearStoredToken, setStoredToken } from '../api/client';
import type { LoginDto, Plano, RegistrarUsuarioDto } from '../api/types';

const SESSION_STORAGE_KEY = 'gestao-autonomo:session';

interface AuthUser {
  usuarioId: string;
  nome: string;
  email: string;
  plano: Plano;
}

interface AuthContextValue {
  user: AuthUser | null;
  isAuthenticated: boolean;
  isPro: boolean;
  login: (dto: LoginDto) => Promise<void>;
  registrar: (dto: RegistrarUsuarioDto) => Promise<void>;
  logout: () => void;
  simularUpgrade: (plano: Plano) => Promise<void>;
}

const AuthContext = createContext<AuthContextValue | null>(null);

function lerSessaoArmazenada(): AuthUser | null {
  const bruto = localStorage.getItem(SESSION_STORAGE_KEY);
  if (!bruto) return null;
  try {
    return JSON.parse(bruto) as AuthUser;
  } catch {
    return null;
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<AuthUser | null>(() => lerSessaoArmazenada());

  const salvarSessao = useCallback((response: { token: string; usuarioId: string; nome: string; email: string; plano: Plano }) => {
    setStoredToken(response.token);
    const perfil: AuthUser = {
      usuarioId: response.usuarioId,
      nome: response.nome,
      email: response.email,
      plano: response.plano,
    };
    localStorage.setItem(SESSION_STORAGE_KEY, JSON.stringify(perfil));
    setUser(perfil);
  }, []);

  const login = useCallback(
    async (dto: LoginDto) => {
      const resposta = await authApi.login(dto);
      salvarSessao(resposta);
    },
    [salvarSessao],
  );

  const registrar = useCallback(
    async (dto: RegistrarUsuarioDto) => {
      const resposta = await authApi.registrar(dto);
      salvarSessao(resposta);
    },
    [salvarSessao],
  );

  const simularUpgrade = useCallback(
    async (plano: Plano) => {
      const resposta = await authApi.simularPlano({ plano: plano === 'Pro' ? 1 : 0 });
      salvarSessao(resposta);
    },
    [salvarSessao],
  );

  const logout = useCallback(() => {
    clearStoredToken();
    localStorage.removeItem(SESSION_STORAGE_KEY);
    setUser(null);
  }, []);

  const value = useMemo<AuthContextValue>(
    () => ({
      user,
      isAuthenticated: user !== null,
      isPro: user?.plano === 'Pro',
      login,
      registrar,
      logout,
      simularUpgrade,
    }),
    [user, login, registrar, logout, simularUpgrade],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth precisa estar dentro de um AuthProvider');
  return ctx;
}
