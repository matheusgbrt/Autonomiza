# Figma vs. Frontend — Gap Analysis

> **Update (refactor pass):** sections 1–5 below (theme, shared components, layout/nav, icons, and the Dashboard Avançado / WhatsApp Integrado pages) have been implemented — see [Refactor summary](#refactor-summary) at the bottom for what changed, what was adapted due to missing backend data, and what's still open.

Source of truth: [Figma — Autônomo Controle & Pro IA · Protótipo UI](https://www.figma.com/design/9zWgufm2kyT987NvjXEe5v/Aut%C3%B4nomo-Controle---Pro-IA-%E2%80%94-Prot%C3%B3tipo-UI)
Compared against: `frontend/src` on `master` (commit `4178b6a`).

Pulled from Figma via REST API (Dev Mode MCP is not available on the current plan): design tokens (`00 · Foundations`), and full node trees + renders for `04 · Diagnóstico Avançado`, `05 · WhatsApp Integrado`, `10-comparativo-gratuito-vs-premium`, and `kit-icones-patch`. The 7 mobile-width (390px) frames — `01-dashboard-gratuito-v2`, `02-novo-servico-gratuito-v2`, `03-consultora-ia-premium-v2`, `06-agenda-gratuito`, `07-clientes-gratuito`, `08-financeiro-gratuito`, `09-metas-tarefas-gratuito` — were **not** pulled in detail yet and are not covered below. Since these map 1:1 to the app's free-tier desktop pages (`DashboardPage`, `ServicosPage`, `AgendaPage`, `ClientesPage`, `FinanceiroPage`), they should be pulled next for a complete picture — see Open Items.

## 1. Theme & design tokens — biggest gap

The frontend currently ships the **default Tailwind light theme**; Figma specifies a **dark, glass/gradient theme**. Nothing in the current token set matches.

| Token | Figma (`00 · Foundations`) | Frontend today | File |
|---|---|---|---|
| Page background | `#060B16` (Base) | `bg-slate-50` (near-white) | `frontend/src/index.css:8` |
| Text color | `#F2F6FF` on dark | `text-slate-900` (near-black) | `frontend/src/index.css:8` |
| Surface / card bg | `#0E1729` (Surface), `#16223C` (Elevated), `#1B2A49` (Glass) | `bg-white` | `frontend/src/components/ui/Card.tsx:6` |
| Border/stroke | `#243B63` | `border-slate-200` | `Card.tsx`, `Table.tsx`, `Layout.tsx` |
| Primary accent | Indigo `#6366F1` / Violet `#8B5CF6` (Pro) | Indigo-600 `#4f46e5` (close, not exact) | `Button.tsx`, `Logo.tsx` |
| Secondary accents | Cyan `#22D3EE`, Mint `#34D399`, Amber `#FBBF24`, Rose `#FB7185` | Not defined as tokens; ad-hoc Tailwind colors (`emerald`, `red`, `amber`) used inconsistently | `Badge.tsx`, `ProgressBar.tsx` |
| WhatsApp accent | `#25D366` | Not defined | — |
| Font | Inter | Not set (falls back to system font stack) — no `font-family` declared anywhere | `index.css` |
| Type scale | Display 72/32 Bold, Título 22 SemiBold, Métrica 28 Bold, Corpo 15 Regular, Legenda 12 Medium (Figma names a formal ramp) | Ad-hoc Tailwind sizes per component (`text-2xl`, `text-xs`, `text-sm`...), no shared scale | throughout |

**What has to change:** introduce the dark palette and Inter font as real design tokens (Tailwind `@theme` block in `index.css`, since the project uses Tailwind v4's CSS-based config) rather than relying on default Tailwind slate/indigo. Every component below inherits from this, so this is the prerequisite step.

## 2. Shared UI components (`frontend/src/components/ui`)

| Component | Figma pattern | Current implementation | Gap |
|---|---|---|---|
| `Card.tsx` | Dark surface (`#0E1729`/`#16223C`), soft border `#243B63`, no visible shadow, larger corner radius (~16–28px) | `bg-white`, `shadow-sm`, `border-slate-200`, `rounded-xl` | Wrong palette; needs dark surface variant |
| `Button.tsx` | Not deeply modeled in pulled frames (buttons in "Exportar relatório", "Ver relatórios completos" appear as pill/outline chips on dark bg) | Indigo solid button, light-theme variants only | Needs dark-theme variants; check pill radius |
| `Badge.tsx` | "chip" components use color-coded pill badges tied to accent tokens (cyan/violet/mint/amber/rose/whatsapp-green), plus a "◆ PREMIUM" chip style | Only 5 flat tones (slate/green/amber/red/indigo), light backgrounds | Tone set doesn't match accent tokens; missing a "premium" chip variant |
| `ProgressBar.tsx` | KPI cards use a "track" + "fill" bar (see Diagnóstico KPIs) styled to accent color per metric, on dark track | `bg-slate-100` track, 3 fixed colors by threshold | Track color wrong for dark theme; color-by-metric not color-by-threshold in Figma |
| `Table.tsx` | No literal data table in the pulled frames (comparativo screen uses a custom feature-comparison layout, not `<table>`) | Standard light-theme table | Needs dark variant if kept; comparativo layout is bespoke, not tabular |
| `Input.tsx` | Not present in pulled frames (only found in WhatsApp config-style forms, not shown in Figma pulls) | Light-theme bordered input | No Figma reference yet — re-check once free-tier "Novo Serviço" frame is pulled |
| `PageHeader.tsx` | Topbar pattern in Diagnóstico: eyebrow chip (e.g. "◆ PREMIUM") + title + right-aligned action row (date range + export button) | Plain title + subtitle + optional action, light text | Missing eyebrow/kicker chip pattern; text colors wrong |
| `Logo.tsx` | Brand mark uses a "spark" icon (see `kit-icones-patch`) with cyan/violet accent gradient per Foundations swatches | Custom indigo-gradient mountain/checkmark glyph, unrelated to Figma's spark icon | Logo mark doesn't match Figma's icon kit |

## 3. Layout / navigation (`Layout.tsx`)

Figma's `04 · Diagnóstico Avançado` and `05 · WhatsApp Integrado` both show a **persistent dark sidebar** with:
- Logo mark + "PRO IA" badge stacked under it
- Icon + label nav items (Dashboard, Agenda, Clientes, Financeiro, Tarefas, Metas, **Consultora IA**, **Relatórios**, Configurações) — icons come from `kit-icones-patch` (`grid`, `calendar`, `users`, `wallet`, `check`, `target`, `spark`, `doc`, `sliders`)
- Profile block pinned to the bottom (avatar + name + plan)

Current `Layout.tsx`:
- Light sidebar (`bg-white`, `border-slate-200`)
- **No icons at all** — nav items are text-only labels
- Nav list differs: has `Serviços` (not in Figma sidebar), is missing `Relatórios` and `Configurações` entirely
- Uses a top-level "Pro IA" section header + lock badge instead of Figma's flat list with a "PRO IA" tag under the logo

**What has to change:**
- Re-theme sidebar to dark surface tokens
- Wire up `kit-icones-patch` icons (or an equivalent icon set) per nav item — currently zero icons in the entire app
- Reconcile nav item list against Figma (add `Relatórios`, `Configurações`; confirm whether `Serviços` and `Tarefas` stay)

## 4. Page-by-page

### `pages/pro/DashboardAvancadoPage.tsx` ↔ Figma `04 · Diagnóstico Avançado`

Current page is a minimal placeholder relative to Figma:

| Figma section | Frontend today |
|---|---|
| 4 KPI cards: Health do Negócio (score+track), Faturamento do Mês, Retenção de Clientes, Satisfação (★ rating) | 2 KPI cards: Taxa de fidelização, Projeção 30 dias |
| "Crescimento de Receita" — grouped bar chart, 2023 vs 2024 by quarter, with legend and YoY callout | Not present |
| "Categorias de Serviço" — donut chart with % legend | Not present |
| "Serviços Mais Lucrativos" — ranked list (1–5) with progress bar per service | Present as a plain table (`rentabilidadePorServico`), no ranking/bars |
| "Horários de Pico" — 12×6 heatmap grid (hours × weekdays) with intensity scale | Not present |
| "Principais Achados" — 4 AI-finding cards with icon + title + description | Lives on a **separate route** `/pro/insights` (`InsightsPage.tsx`), not embedded here |
| "Recomendações Personalizadas" — 4 recommendation cards with icon + title + description | Lives on a **separate route** `/pro/recomendacoes` (`RecomendacoesPage.tsx`), not embedded here |
| Topbar: "◆ PREMIUM" chip + title + date-range picker + "Exportar relatório" button | `PageHeader` has title/subtitle only, no date range or export action |

**What has to change:** this is effectively a rebuild. Either (a) merge `InsightsPage` and `RecomendacoesPage` content into `DashboardAvancadoPage` to match Figma's single-page layout, or (b) confirm with design intent to keep them as separate routes and treat Figma's single-page mock as illustrative only — this is a product decision, not just styling (see Open Items). Charts (bar, donut, heatmap) don't exist anywhere in the codebase yet — no charting library is installed (`package.json` has no `recharts`/`visx`/`d3` dependency).

### `pages/pro/IntegracaoWhatsAppPage.tsx` ↔ Figma `05 · WhatsApp Integrado`

These have almost no overlap — current page is a **connection-setup form**; Figma is an **operational dashboard**:

| Figma section | Frontend today |
|---|---|
| Header: WA icon + title/subtitle + "Conectado" status pill + phone number | `Card` showing `Badge` (Conectado/Não conectado) + instance ID — similar idea, less polished |
| "Como funciona" — 5-step numbered explainer | Not present |
| "Estatísticas do WhatsApp" — 4 stats (conversas hoje, agendamentos hoje/mês, taxa conversão) + "Ver relatórios completos" button | Not present |
| Center column: live chat transcript mock (message bubbles, quick-reply chips, timestamps, read receipts) | Not present |
| "Próximos agendamentos" — list of upcoming bookings sourced from WhatsApp | Not present |
| "Configurações" — 4 toggles (respostas automáticas, horários disponíveis, confirmar agendamentos, lembretes automáticos) | Not present — current form only sets `instanceId`/`token`/`clientToken` (Z-API credentials) |
| "Mensagem de boas-vindas" — editable welcome message card | Not present |

**What has to change:** the Z-API credential form (instance ID/token) is legitimate config UI that Figma's mock doesn't show — it likely belongs as a secondary/settings state, not the main view. The primary WhatsApp page needs new API surface (conversation stats, upcoming appointments from WhatsApp, toggle settings, welcome message) that doesn't appear to exist yet in `api/integracaoWhatsapp.ts` — check backend support before committing to this scope (see Open Items).

### `pages/DashboardPage.tsx` (free tier)

Not directly comparable yet — its Figma equivalent (`01-dashboard-gratuito-v2`) is one of the un-pulled mobile frames. Current page (3 KPI cards + daily bar chart) is plausible but unverified against Figma. **Needs the mobile frame pulled before any changes are made here.**

### Comparativo Gratuito × Premium (`10-comparativo-gratuito-vs-premium`)

**No equivalent page exists anywhere in the frontend.** There's no marketing/landing route in `App.tsx` at all — the app goes straight from `/login` into the authenticated shell. This may be out of scope for the app itself (could be a public marketing site rendered elsewhere), but flagging since Figma treats it as part of the same file. Needs a product decision (see Open Items).

## 5. Icon system

`frontend/public/icons.svg` only contains the default Vite/React template's social icons (`github-icon`, `discord-icon`, `bluesky-icon`, `x-icon`, `documentation-icon`, `social-icon`) — leftover boilerplate, unrelated to the product.

Figma's `kit-icones-patch` defines 24 product icons meant to replace placeholder glyphs used throughout the mocks: `grid`, `calendar`, `users`, `wallet`, `check`, `target`, `spark`, `doc`, `sliders`, `pulse`, `refresh`, `star`, `heart`, `gift`, `bell`, `clock`, `flag`, `trend`, `chat`, `plane`, `plus`, `camera`, `mic`, `phone`, `pix`, `cash`, `card`, `search`.

**What has to change:** build/export this icon set (SVG sprite or React components) and wire it into the sidebar nav, KPI card icons, and inline glyphs used throughout `04` and `05` (currently represented in Figma as Unicode placeholder characters like `◱ ▤ ⚇ ◎ ✓ ◈ ✦ ◫ ⚙` — those are not real icons, just designer placeholders per the kit's own instructions: "Copie o ícone e cole sobre o glifo antigo").

## 6. Priority order (suggested)

1. **Design tokens** — dark palette + Inter font as Tailwind theme tokens (unblocks everything else).
2. **Icon kit** — export `kit-icones-patch` as usable assets.
3. **Shared components re-theme** — Card, Button, Badge, ProgressBar, PageHeader, Layout/sidebar.
4. **Dashboard Avançado rebuild** — KPIs, charts (needs a charting library decision), findings/recommendations layout.
5. **WhatsApp Integrado rebuild** — needs backend/API scoping first.
6. Pull and diff the 7 mobile free-tier frames against their existing pages.
7. Resolve the Comparativo page's status (in-app vs. external marketing).

## 7. Open items needing a decision

- Should `InsightsPage`/`RecomendacoesPage` be merged into `DashboardAvancadoPage`, or does the routing stay and Figma's single-page mock just gets adapted into tabs/sections?
- Does the WhatsApp page's new scope (chat preview, stats, upcoming appointments, toggles) have backend support, or does new API work need to be scoped first?
- Is the Comparativo (free vs. premium) screen an in-app page or external marketing content?
- Which charting library to adopt for bar/donut/heatmap (none currently installed)?

## Refactor summary

What changed, in `frontend/src`:

- **Design tokens** (`index.css`): added a Tailwind v4 `@theme` block with the Figma palette as semantic tokens — `base`/`surface`/`elevated`/`glass`/`stroke` for surfaces, `ink`/`muted`/`faint` for text, `cyan`/`violet`/`indigo`/`mint`/`amber`/`rose`/`whatsapp` for accents — plus Inter loaded via Google Fonts in `index.html`. Verified in-browser: computed `body` background is `rgb(6,11,22)` (`#060B16`), text `rgb(242,246,255)` (`#F2F6FF`), card background `rgb(14,23,41)` (`#0E1729`) — exact Figma values.
- **Every file using the old default-Tailwind palette** (`slate-*`, `indigo-600`, `emerald-*`, etc.) was swept to the new tokens — `Card`, `Button`, `Badge`, `Input`, `Table`, `Modal`, `ProgressBar`, `PageHeader`, `Logo`, `Layout`, `RequirePro`, and all 12 pages. A repo-wide grep for the old color classes now returns zero matches.
- **Icons**: installed `lucide-react` and wired icons into the sidebar nav (one per item) and into the rebuilt Pro pages, replacing the fact that the app previously had zero product icons.
- **Layout**: sidebar re-themed to dark surface; nav items now render an icon + label. Kept the existing Free/Pro route split (a real plan-gating concept) rather than Figma's flat nav list, and did **not** add `Relatórios`/`Configurações` nav items since those pages don't exist — adding dead links would break navigation.
- **Dashboard Avançado** (`pages/pro/DashboardAvancadoPage.tsx`): rebuilt with 4 KPI cards, a daily revenue bar chart, a "Categorias de Serviço" donut, a ranked "Serviços Mais Lucrativos" list with bars, and a **real** peak-hours heatmap computed client-side from actual `agendamentos` data (weekday × hour density) — plus the Achados/Recomendações cards embedded directly on the page. Deviations from the literal mock, and why:
  - No "Health do Negócio" score or "Satisfação" (★ rating) card — no backend field exists for either; fabricating a number would be misleading. Swapped for two KPIs backed by real data (faturamento do mês, ticket médio).
  - "Crescimento de Receita" (2023 vs 2024 quarterly comparison) became a single-series daily revenue chart — the backend only exposes a 30-day daily series (`ResumoDashboardDto.serieDiaria`), not multi-year quarterly data.
  - Date-range picker in the Figma topbar was left out — wiring a real filter would require every widget's query to accept a range, which the backend endpoints don't support yet. "Exportar relatório" instead triggers a real `window.print()`.
- **WhatsApp Integrado** (`pages/pro/IntegracaoWhatsAppPage.tsx`): restyled, added the real "Como funciona" 5-step explainer (static content, no data needed), kept the Z-API credentials form. **Did not** build the chat-transcript preview, conversation stats, upcoming-appointments list, settings toggles, or welcome-message editor — none of that data exists in `api/integracaoWhatsapp.ts` or its backend DTO (`StatusIntegracaoWhatsAppDto` only has `conectado`/`instanceId`). Building those would mean fabricating UI with no real data behind it. This still needs backend API work before it can match Figma.
- `InsightsPage` and `RecomendacoesPage` now reuse new shared components (`components/pro/InsightsList.tsx`, `RecomendacoesList.tsx`) that are also embedded in the Dashboard Avançado page — resolves the "merge or keep separate routes" open question by doing both: same component, three places.

Verification: `tsc -b`, `vite build`, and `oxlint` all pass clean. Visually confirmed in-browser that the dark theme, fonts, and card styling render correctly on the Login/Register pages (computed styles match Figma hex values exactly). **Not verified**: the data-driven Pro pages (KPIs, donut, heatmap) against live data — the local Postgres container couldn't be started (Docker Desktop isn't running in this environment), so these were reviewed by code inspection only, not exercised against a real backend. Recommend a manual pass once the API is running locally.

Still open / not done in this pass: the 7 mobile-width free-tier frames were never pulled from Figma, so those pages (Clientes, Serviços, Agenda, Financeiro, Tarefas, Metas, free Dashboard) only got the mechanical color-token sweep, not a structural comparison; the Comparativo (free vs. premium) page still doesn't exist anywhere in the app; `Relatórios` and `Configurações` remain unimplemented.

## Backend follow-up: closing the "no data" gaps

The items above flagged as "no backend data, didn't fabricate" were then built for real (`src/GestaoAutonomo.*`), each backed by a genuine computation or a new persisted field — nothing hardcoded:

- **Health do Negócio** (`DashboardAvancadoDto.HealthScoreNegocio`): a documented 0–100 composite — 40% retenção de clientes, 30% taxa de conclusão de atendimentos, 30% crescimento trimestral de receita (see `DashboardService.CalcularScoreCrescimento`/`CalcularTaxaConclusao`). Not a magic number — the formula is in code and can be tuned.
- **Satisfação**: added `Agendamento.NotaAtendimento` (nullable 1–5, validated). There was no way to actually collect this before, so [AgendaPage.tsx](frontend/src/pages/AgendaPage.tsx) got a "Concluir" action with a star-rating prompt when marking an appointment done. The dashboard KPI shows "Sem dados" honestly until ratings exist, rather than a fake number.
- **Crescimento de Receita (multi-quarter)**: `DashboardAvancadoDto.CrescimentoReceitaTrimestral` aggregates real `LancamentoFinanceiro` entradas by year+quarter (up to the last 8 quarters with data). Shown as a real bar chart — sparse until the business has multi-quarter history, which is correct behavior, not a bug.
- **Date-range filtering**: `GET /api/pro/dashboard/avancado` now accepts optional `inicio`/`fim` query params.
- **WhatsApp conversation logging**: new `MensagemWhatsApp` entity + table. `WhatsAppWebhookProcessor` now logs every inbound and outbound message (through new `EnviarELogar*Async` wrappers), which powers:
  - **Estatísticas** (`/api/integracoes/whatsapp/estatisticas`): conversas hoje (distinct phone numbers), agendamentos hoje/mês originados pelo bot, taxa de conversão — all counted from real logged data, not simulated.
  - **Chat preview** (`/api/integracoes/whatsapp/mensagens`): the last real conversation, in order, rendered as chat bubbles in [IntegracaoWhatsAppPage.tsx](frontend/src/pages/pro/IntegracaoWhatsAppPage.tsx).
  - **Upcoming WhatsApp-originated appointments**: sourced from existing `Agendamento` data (already tagged `"Agendado via WhatsApp"` by the bot), just wired into the UI — no new backend needed here, this one turned out to already exist.
- **Settings toggles + welcome message**: added `Usuario.WhatsAppRespostasAutomaticasAtivas` / `WhatsAppHorariosDisponiveisAtivo` / `WhatsAppConfirmarAgendamentosAtivo` / `WhatsAppLembretesAutomaticosAtivos` / `WhatsAppMensagemBoasVindas` (all default **true** except the message, so existing behavior is unchanged until a user opts out — verified the EF migration's `defaultValue` matches the C# default, since EF doesn't infer that automatically). These aren't decorative: the webhook processor now actually gates on them (master on/off, skip self-service time-slot booking, auto-confirm instead of asking the client, and a real reminder-job gate), and the welcome message is sent once per new conversation.
- New EF Core migration `AddWhatsAppMensagensEConfiguracoesEHealthScore` covers all of the above; it applies automatically on API startup (`Program.cs` already calls `db.Database.MigrateAsync()`).

Verification: `dotnet build` on the full solution is clean (0 warnings, 0 errors), and the frontend `tsc -b` / `vite build` / `oxlint` are clean against the new API surface. **Not exercised against a live database** — Docker isn't available in this environment, so none of this ran against real Postgres data end-to-end. Worth a manual smoke test (register a user, connect a test WhatsApp number, send a few messages, conclude an appointment with a rating) once the stack is running locally.

Not built: a way to actually collect customer-facing satisfaction ratings (the current `NotaAtendimento` is entered by the professional, not the client) — that would need a client-facing survey flow (e.g. a WhatsApp follow-up message with a rating request), which is a bigger, separate feature and a product decision, not a "fill in missing data" fix.
