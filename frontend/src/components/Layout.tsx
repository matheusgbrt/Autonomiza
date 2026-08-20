import type { ComponentType, ReactNode } from 'react';
import { NavLink } from 'react-router-dom';
import {
  Activity,
  Calendar,
  CheckSquare,
  FileText,
  Gift,
  LayoutGrid,
  LogOut,
  MessageCircle,
  Sparkles,
  Target,
  Users,
  Wallet,
} from 'lucide-react';
import { useAuth } from '../auth/AuthContext';
import { Badge } from './ui/Badge';
import { Logo } from './Logo';

type NavItem = { to: string; label: string; icon: ComponentType<{ size?: number; className?: string }>; end?: boolean };

const FREE_LINKS: NavItem[] = [
  { to: '/', label: 'Dashboard', icon: LayoutGrid, end: true },
  { to: '/clientes', label: 'Clientes', icon: Users },
  { to: '/servicos', label: 'Serviços', icon: FileText },
  { to: '/agenda', label: 'Agenda', icon: Calendar },
  { to: '/financeiro', label: 'Financeiro', icon: Wallet },
  { to: '/tarefas', label: 'Tarefas', icon: CheckSquare },
  { to: '/metas', label: 'Metas', icon: Target },
];

const PRO_LINKS: NavItem[] = [
  { to: '/pro/dashboard', label: 'Dashboard Avançado', icon: Activity },
  { to: '/pro/insights', label: 'IA Consultora', icon: Sparkles },
  { to: '/pro/recomendacoes', label: 'Recomendações', icon: Gift },
  { to: '/pro/whatsapp', label: 'Integração WhatsApp', icon: MessageCircle },
];

function navLinkClass(isActive: boolean) {
  return `flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-colors ${
    isActive ? 'bg-indigo/15 text-ink' : 'text-muted hover:bg-elevated hover:text-ink'
  }`;
}

function NavList({ links }: { links: NavItem[] }) {
  return (
    <>
      {links.map((link) => (
        <NavLink key={link.to} to={link.to} end={link.end} className={({ isActive }) => navLinkClass(isActive)}>
          <link.icon size={17} className="shrink-0" />
          {link.label}
        </NavLink>
      ))}
    </>
  );
}

export function Layout({ children }: { children: ReactNode }) {
  const { user, isPro, logout } = useAuth();

  return (
    <div className="flex min-h-screen bg-base">
      <aside className="flex w-64 flex-shrink-0 flex-col border-r border-stroke bg-surface">
        <div className="border-b border-stroke px-5 py-5">
          <Logo size={28} />
          <p className="mt-1 text-xs text-faint">Gestão para o profissional autônomo</p>
        </div>

        <nav className="flex-1 space-y-1 overflow-y-auto px-3 py-4">
          <NavList links={FREE_LINKS} />

          <div className="mt-6 mb-2 flex items-center gap-2 px-3">
            <span className="text-xs font-semibold uppercase tracking-wide text-faint">Pro IA</span>
            {!isPro && <Badge tone="amber">bloqueado</Badge>}
          </div>
          <NavList links={PRO_LINKS} />
        </nav>

        <div className="border-t border-stroke px-5 py-4">
          <div className="mb-2 flex items-center justify-between gap-2">
            <div className="min-w-0">
              <p className="truncate text-sm font-medium text-ink">{user?.nome}</p>
              <p className="truncate text-xs text-faint" title={user?.email}>{user?.email}</p>
            </div>
            <Badge tone={isPro ? 'violet' : 'slate'}>{isPro ? 'Pro' : 'Free'}</Badge>
          </div>
          <button onClick={logout} className="flex cursor-pointer items-center gap-1.5 text-xs font-medium text-faint hover:text-muted">
            <LogOut size={14} />
            Sair
          </button>
        </div>
      </aside>

      <main className="flex-1 overflow-y-auto p-8">{children}</main>
    </div>
  );
}
