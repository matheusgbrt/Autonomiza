# Autonomiza

Plataforma de gestão para profissionais autônomos (agenda, clientes, financeiro,
tarefas e metas), com um tier Pro que acrescenta IA consultora, recomendações
inteligentes, dashboard avançado e agendamento via WhatsApp.

- **Autonomiza** (gratuito) — Agenda de Atendimentos, Cadastro de Clientes,
  Financeiro Básico, Tarefas (com checklist) e Metas.
- **Autonomiza Pro IA** (premium) — Dashboard Avançado, IA Consultora
  (insights preditivos), Recomendações Inteligentes e Integração WhatsApp
  (agendamento automatizado via Z-API).

## Stack

**Backend** — .NET 10, Clean Architecture (`Domain` / `Application` /
`Infrastructure` / `API`), Entity Framework Core + Npgsql (PostgreSQL),
autenticação JWT, FluentValidation, Swagger.

**Frontend** — React 19 + TypeScript + Vite, Tailwind CSS v4, React Router,
TanStack Query, Axios.

## Estrutura do repositório

```
src/
  GestaoAutonomo.Domain          # entidades e regras de domínio
  GestaoAutonomo.Application     # DTOs, interfaces, services, validators
  GestaoAutonomo.Infrastructure  # EF Core, repositórios, DI, integrações externas
  GestaoAutonomo.API             # controllers, Program.cs, appsettings
frontend/                        # SPA React (cobre Free + Pro)
deploy/                          # stack de produção (Portainer + Caddy)
.github/workflows/               # CI: build e publish das imagens no Docker Hub
```

## Rodando localmente

### Opção 1 — Docker Compose (recomendado, sobe tudo)

```bash
cp .env.example .env
# preencher JWT_KEY em .env (openssl rand -base64 48)
docker compose up --build
```

- API: http://localhost:8080 (Swagger em `/swagger`)
- Frontend: http://localhost:8081
- Postgres: `localhost:5432`

Esse `docker-compose.yml` é só para dev local — CORS e a URL da API já
apontam para as portas publicadas por ele mesmo (8080/8081), sem depender de
domínio nenhum.

### Opção 2 — API e frontend separados

```bash
# API (precisa de um Postgres rodando e das connection strings/JWT configurados
# via appsettings.Development.json ou dotnet user-secrets)
dotnet run --project src/GestaoAutonomo.API

# Frontend
cd frontend
npm install
npm run dev
```

O frontend usa `VITE_API_URL` (ver `frontend/.env.example` se existir, ou
configure `http://localhost:5152`/porta da API local) para saber onde chamar
a API — em dev via Vite isso é lido em runtime pelo `import.meta.env`.

## Testes de plano Pro em dev

O endpoint `POST /api/auth/simular-plano` (dev-only) troca o plano do usuário
autenticado sem precisar de cobrança real — é o que o botão "Simular
upgrade" da tela de recurso bloqueado usa no frontend.

## CI/CD

Todo push em `master` roda [`.github/workflows/docker-publish.yml`](.github/workflows/docker-publish.yml),
que builda e publica duas imagens no Docker Hub:

- `matheusgbrt/gestao-autonomo-api`
- `matheusgbrt/gestao-autonomo-frontend`

(tags `latest` e `<sha do commit>`). A URL da API é embutida no bundle do
frontend em build-time (`VITE_API_URL`, via a repo variable `VITE_API_URL`
no GitHub Actions), já que o Vite não lê variáveis de ambiente em runtime
numa build de produção.

Secrets necessários no repositório: `DOCKERHUB_USERNAME`, `DOCKERHUB_TOKEN`.

## Deploy em produção

Ver [`deploy/`](deploy) — stack pronta para subir via Portainer, puxando as
imagens já publicadas no Docker Hub (sem build local), com Postgres próprio e
um [`Caddyfile`](deploy/Caddyfile) de exemplo para o reverse proxy.
