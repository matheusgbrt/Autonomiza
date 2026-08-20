import { Navigate, Route, Routes } from 'react-router-dom';
import { AuthProvider, useAuth } from './auth/AuthContext';
import { RequireAuth } from './auth/RequireAuth';
import { RequirePro } from './auth/RequirePro';
import { Layout } from './components/Layout';
import { LoginPage } from './pages/LoginPage';
import { RegisterPage } from './pages/RegisterPage';
import { DashboardPage } from './pages/DashboardPage';
import { ClientesPage } from './pages/ClientesPage';
import { ClienteDetalhePage } from './pages/ClienteDetalhePage';
import { ServicosPage } from './pages/ServicosPage';
import { AgendaPage } from './pages/AgendaPage';
import { FinanceiroPage } from './pages/FinanceiroPage';
import { TarefasPage } from './pages/TarefasPage';
import { MetasPage } from './pages/MetasPage';
import { DashboardAvancadoPage } from './pages/pro/DashboardAvancadoPage';
import { InsightsPage } from './pages/pro/InsightsPage';
import { RecomendacoesPage } from './pages/pro/RecomendacoesPage';
import { IntegracaoWhatsAppPage } from './pages/pro/IntegracaoWhatsAppPage';

function PublicOnly({ children }: { children: React.ReactNode }) {
  const { isAuthenticated } = useAuth();
  if (isAuthenticated) return <Navigate to="/" replace />;
  return <>{children}</>;
}

function AppRoutes() {
  return (
    <Routes>
      <Route
        path="/login"
        element={
          <PublicOnly>
            <LoginPage />
          </PublicOnly>
        }
      />
      <Route
        path="/registrar"
        element={
          <PublicOnly>
            <RegisterPage />
          </PublicOnly>
        }
      />

      <Route
        path="/*"
        element={
          <RequireAuth>
            <Layout>
              <Routes>
                <Route path="/" element={<DashboardPage />} />
                <Route path="/clientes" element={<ClientesPage />} />
                <Route path="/clientes/:id" element={<ClienteDetalhePage />} />
                <Route path="/servicos" element={<ServicosPage />} />
                <Route path="/agenda" element={<AgendaPage />} />
                <Route path="/financeiro" element={<FinanceiroPage />} />
                <Route path="/tarefas" element={<TarefasPage />} />
                <Route path="/metas" element={<MetasPage />} />

                <Route
                  path="/pro/dashboard"
                  element={
                    <RequirePro>
                      <DashboardAvancadoPage />
                    </RequirePro>
                  }
                />
                <Route
                  path="/pro/insights"
                  element={
                    <RequirePro>
                      <InsightsPage />
                    </RequirePro>
                  }
                />
                <Route
                  path="/pro/recomendacoes"
                  element={
                    <RequirePro>
                      <RecomendacoesPage />
                    </RequirePro>
                  }
                />
                <Route
                  path="/pro/whatsapp"
                  element={
                    <RequirePro>
                      <IntegracaoWhatsAppPage />
                    </RequirePro>
                  }
                />

                <Route path="*" element={<Navigate to="/" replace />} />
              </Routes>
            </Layout>
          </RequireAuth>
        }
      />
    </Routes>
  );
}

function App() {
  return (
    <AuthProvider>
      <AppRoutes />
    </AuthProvider>
  );
}

export default App;
