# Task Board

> **Live work queue for the AI Engineering Platform.**
> Updated at the end of every AI session that changes
> project state. The most recent update wins. The
> state files reflect the actual state of the
> repository; the repository wins when the two
> disagree (see `.ai/session-start.md` step 6 —
> state reconciliation).
>
> **State architecture (M0.5).** This file is the
> human-readable projection. The canonical machine-readable
> work queue is in
> [`.ai/state/tasks.json`](./tasks.json). The
> capability graph is in
> [`.ai/state/capabilities.json`](./capabilities.json);
> the milestone list is in
> [`.ai/state/milestones.json`](./milestones.json). The
> two layers are kept in sync by every session that
> changes the work queue.
>
> **Status codes:**
>
> - **Ready** — task is defined, no blocker, no owner.
> - **In Progress** — a session is actively working
>   the task.
> - **Blocked** — task cannot proceed; the row names
>   the blocker.
> - **Done Recently** — completed, merged, and
>   committed. Recent items live here; older items
>   archive to `.ai/handoffs/`.
> - **Deferred** — task is intentionally out of
>   scope for the current plan; it lives here so it
>   is not forgotten.

---

## Ready

### M2.4 — Project Intelligence Dashboard

- **Task ID:** `M2.4`
- **Milestone:** M2 — Application Shell and
  Navigation
- **Title:** Project intelligence
  dashboard
- **Why it matters:** The M2 landing
  page is the read-only
  `/dashboard` page; the contract is
  in `docs/dashboard.md`. The
  dashboard renders the M0.5
  structured-state sections in the
  **Populated** state (per ADR-014)
  and the M3+-data sections in the
  **Empty** state. The
  `IProjectIntelligenceReader` is the
  read-side service the M2.4 slice
  introduces; M3+ slices add new
  data sources but do not modify
  the reader.
- **Objective:** Land
  `IProjectIntelligenceReader` in
  `src/AiEng.Platform.Application/ProjectIntelligence/`;
  the
  `ProjectIntelligenceServiceCollectionExtensions`
  composition-root extension in
  `src/AiEng.Platform.App/Composition/`;
  the `/dashboard` page at
  `src/AiEng.Platform.App/Components/Pages/Dashboard.razor`
  that uses the M2.1 `AppLayout`;
  the M0.5-data sections in the
  **Populated** state; the M3+-data
  sections in the **Empty** state;
  a new navigation item in the M2.2
  registry for `/dashboard`.
- **Acceptance criteria:** the
  `/dashboard` route exists in the
  M2.2 navigation registry;
  `IProjectIntelligenceReader` is
  registered in the composition
  root and resolves through DI; the
  dashboard renders the M0.5-data
  sections in the **Populated**
  state; the dashboard renders the
  M3+-data sections in the
  **Empty** state with a clear
  "M3 fills this section" message;
  every data-owning section exposes
  the four slots per ADR-014;
  `dotnet build` → 0 warnings, 0
  errors; `dotnet test` → all bUnit
  tests pass (M2.4 tests add to the
  count; the
  `Application_DoesNotReference_ConcreteProviders`
  architecture test remains
  green); `dotnet format
  --verify-no-changes` → clean;
  `npm run css:build` → clean; the
  app starts; `/dashboard` returns
  200; the keyboard smoke test
  passes.
- **Dependencies:** M2.1 (Done),
  M2.2 (Done), M2.3 (Done).
- **Expected affected areas:**
  `src/AiEng.Platform.Application/ProjectIntelligence/`
  (new),
  `src/AiEng.Platform.App/Composition/ProjectIntelligenceServiceCollectionExtensions.cs`
  (new),
  `src/AiEng.Platform.App/Components/Pages/Dashboard.razor`
  (new),
  `src/AiEng.Platform.App/Components/Navigation/AppBreadcrumb.razor`
  (reused — the M2.3 breadcrumb
  composes against the same
  navigation registry).
- **Validation:** `dotnet build`,
  `dotnet test`, `dotnet format
  --verify-no-changes`, visual
  smoke test on `/dashboard`.
- **Approved plan path:**
  [`.ai/plans/M2.4-project-intelligence-dashboard.md`](./../../.ai/plans/M2.4-project-intelligence-dashboard.md)
  (the plan, 2026-07-11; promoted
  from `Draft` stub to a full plan
  in `Awaiting Approval` in the
  M2.3 closeout session).
- **Status:** Ready (plan `Awaiting
  Approval`; promoted from `Draft`
  stub to a full plan in the M2.3
  closeout session; first action
  of the next session is plan
  approval).

### M1 follow-up — Add `AppToolbar` example to `/design-system`

- **Task ID:** `M1-FU-1`
- **Milestone:** M1 — Design System Core
  (closed; this is a follow-up)
- **Title:** Add `AppToolbar` example to
  `/design-system`
- **Why it matters:** The `/design-system`
  page is the design-system catalogue's
  rendering; missing component examples hide
  shipped components from reviewers.
  `AppToolbar` ships and is unit-tested but
  is not exercised on the doc page (18/19
  component CSS classes appear in the
  rendered HTML).
- **Objective:** Add an `AppToolbar` example to
  `src/AiEng.Platform.App/Components/Pages/DesignSystem.razor`
  in a new "Toolbar" section; rebuild CSS;
  verify the `app-toolbar` class appears in the
  rendered output.
- **Acceptance criteria:** `app-toolbar`
  appears in the `/design-system` HTML; the
  example matches the level of detail of the
  existing section examples; all M1 validation
  remains green.
- **Dependencies:** M1 (closed). No new
  dependencies.
- **Expected affected areas:**
  `src/AiEng.Platform.App/Components/Pages/DesignSystem.razor`,
  possibly
  `src/AiEng.Platform.App/wwwroot/css/app.css`
  (rebuilt by `npm run css:build`).
- **Validation:** `npm run css:build`; visual
  smoke test on `/design-system`; the M1
  bUnit tests for `AppToolbar` remain green.
- **Approved plan path:** (cosmetic; no
  detailed plan required; can be folded into
  M2.1 if appropriate).
- **Status:** Ready (cosmetic; can be picked
  up at any time).

---

## In Progress

(none — M2.3 closed in the M2.3 closeout
session, 2026-07-11; M2.4 awaits the next
session's explicit authorisation per the
Progressive Coding Rule.)

---

## Blocked

### Run M1 design-system `lavish-axi` review (deferred from M1 closeout)

- **Task ID:** `M1-REV-1`
- **Milestone:** M1 — Design System Core
- **Title:** Run `lavish-axi` design-system
  review of the M1 deliverable
- **Why it matters:** The M1 dogfooding
  checkpoint in `ROADMAP.md` § 3 authorises
  the development team to use `lavish-axi`
  externally to review the M1 deliverable.
  The review's findings inform the M2 design
  decisions.
- **Blocker:** `lavish-axi` is not installed on
  the host. The only artefact on the filesystem
  is `agent-workbench/tools/lavish-axi.md`, a
  spec document for an event-bus daemon, not a
  review tool. No review command is documented.
  Per `.ai/workflows/tool-dogfooding.md`, the
  no-silent-fallback rule applies.
- **Unblock path:** (a) install `lavish-axi`
  on the host; (b) the user picks a substitute
  review tool; (c) the user decides the
  `lavish-axi` dogfooding is not the right
  step and removes PART 2 from the brief.
- **Expected affected areas:**
  `.ai/reviews/` (the review record, when
  produced).
- **Validation:** the review report is
  produced; findings are filed with severity
  labels.
- **Approved plan path:** (none — a review
  record is the deliverable, not a plan).
- **Status:** Blocked.

---

## Done Recently

### M2.3 closeout session — 2026-07-11

- **Task ID:** `M2.3`
- **Milestone:** M2 — Application Shell and
  Navigation
- **Title:** Top bar, breadcrumbs, and
  page header integration
- **Status:** **Done (closed 2026-07-11).**
- **Outcome:** 4 new components in
  `src/AiEng.Platform.App/Components/`:
  `AppTopBar` (replaces the M2.1
  `AppTopBarSlot` placeholder), the
  relocated `AppThemeToggle`
  (light/dark theme toggle; persists
  to `localStorage`; reads
  `data-theme` on `documentElement`),
  `AppUserAvatarSlot` (the user
  avatar placeholder; M3+ replaces
  it with the real user identity
  surface), and `AppBreadcrumb` (walks
  the M2.2 registry's `Parent` chain;
  `aria-current="page"` on the current
  item; separators are `aria-hidden`).
  `AppBreadcrumb` wired into
  `AppPageHeader.Breadcrumbs` on
  `DesignSystem.razor`. `AppTopBarSlot`
  and `AppTopBarSlotTests.cs` deleted;
  `AppLayout` updated to use
  `AppTopBar`. 27 new bUnit tests
  across 4 test files
  (`AppTopBarTests`,
  `AppThemeToggleTests`,
  `AppUserAvatarSlotTests`,
  `AppBreadcrumbTests`); 6 obsolete
  `AppTopBarSlotTests` removed. Total
  test count is now 150 passing + 4
  skipped, 0 failed (77 M1.2 + 25
  M2.1 + 23 M2.2 bUnit + 27 M2.3
  bUnit − 6 obsolete removed + 4
  active architecture). Two
  documented deviations: (1)
  `AppTopBar` uses `div.app-topbar`
  + `Leading` / `Trailing` slots
  rather than `AppStack` +
  `AppPageHeader`; the surface still
  composes `AppTopBar` +
  `AppPageHeader` + `AppBreadcrumb`,
  matching the plan's intent. (2)
  Optional architecture test
  `Breadcrumb_Follows_Registry_Parent_Chain`
  was skipped per plan § 8 step 11
  which marked it optional.
- **Report:**
  `implementation-report-m2-3-topbar-breadcrumbs.md`.
- **Handoff:**
  `.ai/handoffs/2026-07-11-m2-3-topbar-breadcrumbs.md`
  (mirrored at
  `.ai/handoffs/latest.md`).
- **Git:** branch
  `feature/m2-3-topbar-breadcrumbs`;
  closeout commit
  `feat(m2.3): add top bar, breadcrumb, and page header integration`.
  No remote configured; push skipped (per
  the brief).
- **Next action:** the M2.3 closeout
  promotes T-014 (M2.4) to `Ready`
  and promotes the M2.4 plan stub
  to a full plan in `Awaiting
  Approval`; the next session
  approves the M2.4 plan and starts
  M2.4 implementation. The M2.3
  session does **not** implement
  M2.4 (per the Progressive Coding
  Rule).

### M2.2 closeout session — 2026-07-11

- **Task ID:** `M2.2`
- **Milestone:** M2 — Application Shell and
  Navigation
- **Title:** Navigation registry and
  sidebar
- **Status:** **Done (closed 2026-07-11).**
- **Outcome:** 4 new types in
  `src/AiEng.Platform.Application/Navigation/`
  (`INavigationRegistry`,
  `RouteMetadata`,
  `RouteMetadataAttribute`,
  `RouteRegistry`); 2 new extension
  methods in
  `src/AiEng.Platform.App/Composition/`
  (`AddPlatformServices` +
  `AddNavigation`); 3 new components in
  `src/AiEng.Platform.App/Components/Navigation/`
  (`AppSidebar`, `AppSidebarItem`,
  `AppNavItem`); `[RouteMetadata]`
  applied to all 6 pages; the
  composition root wired in
  `Program.cs`; the
  `Pages_AreReachable_Through_Registry`
  architecture test is active and green;
  28 new bUnit / integration tests +
  1 new architecture test; the M2.1
  `AppSidebarSlot` placeholder is
  replaced by the registry-driven
  `AppSidebar`; the 19 M1.2 components,
  77 bUnit tests, and 3 active + 4
  registered-but-disabled architecture
  tests are preserved. Total test count
  is now 129 passing + 4 skipped, 0
  failed (77 M1.2 + 25 M2.1 + 23 M2.2
  bUnit + 4 active architecture; 4
  registered-but-disabled
  architecture).
- **Report:**
  `implementation-report-m2-2-navigation-registry-sidebar.md`.
- **Handoff:**
  `.ai/handoffs/2026-07-11-m2-2-navigation-registry-sidebar.md`
  (mirrored at
  `.ai/handoffs/latest.md`).
- **Git:** branch
  `feature/m2-2-navigation-registry-sidebar`;
  closeout commit
  `feat(m2.2): add navigation registry and sidebar`.
  No remote configured; push skipped (per
  the brief).
- **Next action:** the M2.2 closeout
  promotes T-003 (M2.3) to `Ready` and
  promotes the M2.3 plan stub to a
  full plan in `Awaiting Approval`;
  the next session approves the M2.3
  plan and starts M2.3
  implementation. The M2.2 session
  does **not** implement M2.3 (per
  the Progressive Coding Rule).

### M2.1 closeout session — 2026-07-11

- **Task ID:** `M2.1`
- **Milestone:** M2 — Application Shell and
  Navigation
- **Title:** Application shell foundation
- **Status:** **Done (closed 2026-07-11).**
- **Outcome:** 5 new Blazor components /
  layouts in the application shell
  foundation (2 layouts: `AppLayout`,
  `AppEmptyLayout`; 2 placeholder shell
  components: `AppSidebarSlot`,
  `AppTopBarSlot`; 1 presentational helper:
  `AppShellRegion`); 25 new bUnit
  component tests across 4 test files
  (8 + 6 + 5 + 6); the M1.1 chrome
  (`MainLayout.razor`, `MainLayout.razor.css`,
  `NavMenu.razor`, `NavMenu.razor.css`) is
  removed; the Tailwind content path
  includes the new `Layouts/` directory;
  the `Layouts/_Imports.razor` resolves
  cross-folder type references; the five
  M1 template pages and `/design-system`
  reach the new layout root in place; the
  19 M1.2 components, 77 bUnit tests, and
  3 active + 4 registered-but-disabled
  architecture tests are preserved.
  Total test count is now 105 passing +
  4 skipped, 0 failed (77 M1.2 + 25
  M2.1 + 3 active architecture; 4
  registered-but-disabled architecture).
- **Report:**
  `implementation-report-m2-1-application-shell-foundation.md`.
- **Handoff:**
  `.ai/handoffs/2026-07-11-m2-1-application-shell-foundation.md`
  (mirrored at `.ai/handoffs/latest.md`).
- **Git:** branch
  `feature/m2-1-application-shell`;
  closeout commit
  `feat(m2.1): add application shell foundation`.
  No remote configured; push skipped (per
  the brief).
- **Next action:** the M2.1 closeout
  promotes T-002 (M2.2) to `Ready` and
  expands the M2.2 plan stub to a full
  plan in `Awaiting Approval`; the next
  session approves the M2.2 plan and
  starts M2.2 implementation. The
  M2.1 session does **not** implement
  M2.2 (per the Progressive Coding
  Rule).

### M1 closeout session — 2026-07-10

- **Task ID:** `M1-CLOSEOUT`
- **Milestone:** M1 — Design System Core
- **Title:** Close M1, introduce project
  continuity, prepare M2.1 plan
- **Outcome:** 19 reusable Blazor components
  (Primitives 7, Layout 4, Display 2, Feedback
  5, Inputs 1), 77 bUnit component tests, 3
  active architecture tests + 4
  registered-but-disabled composition-root
  tests, the `/design-system` documentation
  page, the Tailwind v3 + PostCSS pipeline,
  the design-token catalogue, light + dark
  themes. All seven ROADMAP M1 DoD items
  satisfied. The project-continuity system
  (Rule 15 in `AGENTS.md`) is in place.
- **Reports:**
  `implementation-report-m1-bootstrap.md`,
  `implementation-report-m1-1-frontend-foundation.md`,
  `implementation-report-m1-2-design-system-core.md`,
  `implementation-report-m1-closeout.md`.
- **Handoff:**
  `.ai/handoffs/2026-07-10-m1-closeout.md`.
- **Git:** first two commits
  `1722bd235830cfd8b180191953116c058c92edef`
  and
  `2ba1fad3cc45bee513ba38c7269e024bf8667ef9`
  on `master`. 166 files committed. Working
  tree clean. No remote configured.

### M1 follow-up — Re-anchor composition-root tests in `ROADMAP.md` matrix

- **Task ID:** `M1-FU-2`
- **Milestone:** M1 — Design System Core
- **Title:** Re-anchor composition-root
  architecture tests in the
  `ROADMAP.md` § 4 matrix
- **Outcome:** `ROADMAP.md` § 4
  ("Progressive self-dogfooding matrix") was
  updated during the M1 closeout session to
  list the four composition-root tests as
  `Delivered in M1 closeout — Active in M4-D`.
  See the M1 closeout report's
  "Files Modified" entry for `ROADMAP.md`.

---

## Deferred

For later milestones, a single summary task is
kept here so the work is not forgotten but the
task board does not become a speculative
backlog. Each summary task is fleshed out into
detailed tasks when the milestone approaches.

### M2.4 — Project Intelligence Dashboard
  (summary)

- **Milestone:** M2.
- **Why deferred:** the dashboard is the M2
  landing page; the contract is in
  `docs/dashboard.md`. The M2.4 slice
  implements the **read-only** dashboard:
  `IProjectIntelligenceReader` reads
  `.ai/state/*.json` and produces a
  `ProjectIntelligenceSnapshot`; the
  dashboard renders the M0.5-data sections in
  the Populated state and the M3+-data
  sections in the Empty state.
- **First action (later):** draft
  `.ai/plans/M2.4-project-intelligence-dashboard.md`
  (the plan stub already exists; promote it
  to a full plan after M2.3 closes).

### M2.5 — Empty routes, responsive, and
  accessibility (summary)

- **Milestone:** M2.
- **Why deferred:** every M2.1 / M2.2 / M2.3
  page reaches an empty state; the
  full-coverage pass over every route, the
  responsive matrix, and the full keyboard
  smoke test are natural after the shell,
  sidebar, top bar, breadcrumb, and dashboard
  are wired.
- **First action (later):** enumerate every
  route in `Components/Pages/`; verify each
  renders an `AppEmptyState` that names the
  page and links back to the design system;
  define the responsive matrix in
  `docs/ui-principles.md` § 10.1; run the
  keyboard smoke test across every route.

### M2.6 — M2 closeout and external Treehouse
  dogfooding checkpoint (summary)

- **Milestone:** M2.
- **Why deferred:** the M2 closeout is the
  M1-style wrap-up (verification, gap-fixing,
  deferred-review record, handoff,
  project-continuity update, M3 plan
  preparation). The Treehouse dogfooding
  checkpoint is per `ROADMAP.md` M2.
- **First action (later):** M2 closeout
  per the M1 closeout template; the
  Treehouse worktree exercise; the M3
  plan.

### M3 — Project Registration (summary)

- **Milestone:** M3.
- **First action (later):** draft
  `.ai/plans/M3-project-registration.md` per
  the ROADMAP M3 section; flesh out
  `IProjectService`, `IProjectStore`
  (in-memory), `AppProjectCard`,
  `AppProjectList`.

### M4-A — Infrastructure / Process Execution (summary)

- **Milestone:** M4-A.
- **First action (later):** draft
  `.ai/plans/M4-A-infrastructure-process-execution.md`.

### M4-B — Capability Detection (summary)

- **Milestone:** M4-B.
- **First action (later):** draft
  `.ai/plans/M4-B-capability-detection.md`.

### M4-C — Provider Registry Foundation (summary)

- **Milestone:** M4-C.
- **First action (later):** draft
  `.ai/plans/M4-C-provider-registry-foundation.md`.

### M4-D — First Concrete Process Providers (summary)

- **Milestone:** M4-D.
- **First action (later):** draft
  `.ai/plans/M4-D-first-concrete-process-providers.md`.

### M5 — Native Git Worktrees (summary)

- **Milestone:** M5.
- **First action (later):** draft
  `.ai/plans/M5-native-git-worktrees.md`.

### M6 — Agent Runtime Launching (summary)

- **Milestone:** M6.
- **First action (later):** draft
  `.ai/plans/M6-agent-runtime-launching.md`.

### M7 — Review and Quality Gates (summary)

- **Milestone:** M7.
- **First action (later):** draft
  `.ai/plans/M7-review-and-quality-gates.md`.

### M8 — Autonomous Loops, Orchestration, Production Hardening (summary)

- **Milestone:** M8.
- **First action (later):** draft
  `.ai/plans/M8-autonomous-loops-orchestration.md`.
