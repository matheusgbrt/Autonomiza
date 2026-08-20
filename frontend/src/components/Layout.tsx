import type { ReactNode } from 'react';
import { NavLink } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';
import { Badge } from './ui/Badge';

const FREE_LINKS = [
  { to: '/', label: 'Dashboard', end: true },
  { to: '/clientes', label: 'Clientes' },
  { to: '/servicos', label: 'Serviços' },
  { to: '/agenda', label: 'Agenda' },
  { to: '/financeiro', label: 'Financeiro' },
  { to: '/tarefas', label: 'Tarefas' },
  { to: '/metas', label: 'Metas' },
];

const PRO_LINKS = [
  { to: '/pro/dashboard', label: 'Dashboard Avançado' },
  { to: '/pro/insights', label: 'IA Consultora' },
  { to: '/pro/recomendacoes', label: 'Recomendações' },
  { to: '/pro/whatsapp', label: 'Integração WhatsApp' },
];

function navLinkClass(isActive: boolean) {
  return `block rounded-lg px-3 py-2 text-sm font-medium transition-colors ${
    isActive ? 'bg-indigo-50 text-indigo-700' : 'text-slate-600 hover:bg-slate-100'
  }`;
}

export function Layout({ children }: { children: ReactNode }) {
  const { user, isPro, logout } = useAuth();

  return (
    <div className="flex min-h-screen bg-slate-50">
      <aside className="flex w-64 flex-shrink-0 flex-col border-r border-slate-200 bg-white">
        <div className="border-b border-slate-200 px-5 py-5">
          <h1 className="text-lg font-bold text-slate-900">Autônomo Controle</h1>
          <p className="text-xs text-slate-500">Gestão para o profissional autônomo</p>
        </div>

        <nav className="flex-1 space-y-1 px-3 py-4">
          {FREE_LINKS.map((link) => (
            <NavLink key={link.to} to={link.to} end={link.end} className={({ isActive }) => navLinkClass(isActive)}>
              {link.label}
            </NavLink>
          ))}

          <div className="mt-6 mb-2 flex items-center gap-2 px-3">
            <span className="text-xs font-semibold uppercase tracking-wide text-slate-400">Pro IA</span>
            {!isPro && <Badge tone="amber">bloqueado</Badge>}
          </div>
          {PRO_LINKS.map((link) => (
            <NavLink key={link.to} to={link.to} className={({ isActive }) => navLinkClass(isActive)}>
              {link.label}
            </NavLink>
          ))}
        </nav>

        <div className="border-t border-slate-200 px-5 py-4">
          <div className="mb-2 flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-slate-900">{user?.nome}</p>
              <p className="text-xs text-slate-500">{user?.email}</p>
            </div>
            <Badge tone={isPro ? 'indigo' : 'slate'}>{isPro ? 'Pro' : 'Free'}</Badge>
          </div>
          <button onClick={logout} className="text-xs font-medium text-slate-500 hover:text-slate-700">
            Sair
          </button>
        </div>
      </aside>

      <main className="flex-1 overflow-y-auto p-8">{children}</main>
    </div>
  );
}
