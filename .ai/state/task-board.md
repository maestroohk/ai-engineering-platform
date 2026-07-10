# Task Board

> **Live work queue for the AI Engineering Platform.**
> Updated at the end of every AI session that changes
> project state. The most recent update wins. The
> state files reflect the actual state of the
> repository; the repository wins when the two
> disagree (see `.ai/session-start.md` step 6 —
> state reconciliation).
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

### M2.1 — Application Shell Skeleton

- **Task ID:** `M2.1`
- **Milestone:** M2 — Application Shell and
  Navigation
- **Title:** Application shell skeleton
- **Why it matters:** The application shell is the
  prerequisite for every later milestone. Every
  project-registration, worktree, launch, review,
  and disposition surface is reached through the
  shell. Without the shell, the M1.2 design system
  has no honest consumer.
- **Objective:** Replace the M1.1 chrome
  (`MainLayout.razor`, `NavMenu.razor`) with the
  M2 application shell — `AppSidebar`,
  `AppTopBar`, `AppBreadcrumb`, `AppNavItem`,
  `AppLayout`, `AppEmptyLayout` — composed from
  the M1.2 design-system primitives. Introduce
  `INavigationRegistry` and the route-metadata
  data model. Redesign the five M1 template pages
  to compose the new shell.
- **Acceptance criteria:**
  - The six M2.1 components ship with bUnit
    tests.
  - The five template pages reach a meaningful
    empty state that uses the design system.
  - The architecture test
    `Pages_AreReachable_Through_Registry` is
    in place and green.
  - `dotnet build` → 0 warnings, 0 errors.
  - `dotnet test` → all tests pass; no new
    failures.
  - `dotnet format --verify-no-changes` → clean.
  - `npm run css:build` → clean; new component
    classes present in the compiled CSS.
  - The app starts on `http://localhost:5286`
    and every route returns 200.
  - Light and dark themes render correctly; the
    theme toggle in the top bar flips the theme.
  - The keyboard smoke test passes: Tab through
    the shell; every interactive element is
    reachable; focus is visible; the breadcrumb
    is reachable.
- **Dependencies:** M1 — Design System Core
  (closed 2026-07-10). M1.2 supplies the 19
  design-system components the M2.1 shell
  composes.
- **Expected affected areas:**
  - `src/AiEng.Platform.App/Components/`
    (new `Components/Navigation/` folder; new
    `Layouts/` folder; redesigned
    `Pages/`).
  - `src/AiEng.Platform.Application/Navigation/`
    (new `INavigationRegistry`,
    `RouteMetadata`,
    `RouteMetadataAttribute`,
    `RouteRegistry`).
  - `src/AiEng.Platform.App/Composition/`
    (new
    `NavigationServiceCollectionExtensions`).
  - `src/AiEng.Platform.App/Program.cs`
    (DI registration of the navigation
    registry).
  - `docs/design-system.md` (catalogue updates,
    version bump to 0.3.0 per ADR-015).
  - `docs/component-guidelines.md` (new
    component documentation; the
    `INavigationRegistry` contract).
  - `ROADMAP.md` (M2 row updates).
- **Validation:** `dotnet build` clean;
  `dotnet test` all green; `dotnet format
  --verify-no-changes` clean; `npm run
  css:build` clean; visual smoke test on
  `http://localhost:5286`; keyboard smoke test.
- **Approved plan path:**
  [`.ai/plans/M2.1-application-shell-skeleton.md`](./../../.ai/plans/M2.1-application-shell-skeleton.md)
- **Status:** Ready (plan `Awaiting Approval`).

### M2.2 — Sidebar and navigation registry

- **Task ID:** `M2.2`
- **Milestone:** M2 — Application Shell and
  Navigation
- **Title:** Sidebar and navigation registry
- **Why it matters:** The sidebar is the primary
  navigation surface; the navigation registry is
  the data model M3+ consume. A registry-driven
  sidebar ensures the M3 project-registration
  page and every later page are reachable through
  a single source of truth.
- **Objective:** Land the `INavigationRegistry`
  and the route-metadata data model. Wire the
  registry into `AppSidebar`. Validate that every
  page in `Components/Pages/` has a
  `[RouteMetadata]` attribute and is reachable
  through the registry.
- **Acceptance criteria:** the registry is the
  data source for the sidebar; every page is
  routable through the registry; the
  `Pages_AreReachable_Through_Registry`
  architecture test is active and green.
- **Dependencies:** M2.1.
- **Expected affected areas:**
  `src/AiEng.Platform.Application/Navigation/`,
  `src/AiEng.Platform.App/Components/Navigation/AppSidebar*`,
  `src/AiEng.Platform.App/Components/Pages/*`,
  `tests/AiEng.Platform.ArchitectureTests/`.
- **Validation:** `dotnet build`, `dotnet test`,
  `dotnet format --verify-no-changes`, visual
  smoke test.
- **Approved plan path:** (not yet planned in
  detail; will be drafted after M2.1 closes).
- **Status:** Ready (summary; details to be
  planned after M2.1).

### M2.3 — Top bar and page header integration

- **Task ID:** `M2.3`
- **Milestone:** M2 — Application Shell and
  Navigation
- **Title:** Top bar and page header
  integration
- **Why it matters:** The top bar hosts the
  theme toggle, the user avatar, and the
  application-level actions; the page header
  is the consistent first line of every page.
  Both are reached on every route; both must
  compose the M1.2 design system.
- **Objective:** Land `AppTopBar` with its
  `Leading` and `Trailing` slots. Move the
  theme toggle (M1.1) into the `Trailing`
  slot. Land the `AppBreadcrumb` component and
  wire it into `AppPageHeader`'s `Breadcrumbs`
  parameter (M1.2 placeholder).
- **Acceptance criteria:** the top bar composes
  `AppToolbar`; the breadcrumb follows the
  current route's `Parent` chain; the theme
  toggle flips light/dark; the `AppPageHeader`
  `Breadcrumbs` placeholder is no longer a
  placeholder.
- **Dependencies:** M2.1, M2.2.
- **Expected affected areas:**
  `src/AiEng.Platform.App/Components/Navigation/AppTopBar*`,
  `src/AiEng.Platform.App/Components/Navigation/AppBreadcrumb*`,
  `src/AiEng.Platform.App/Components/Pages/*`.
- **Validation:** `dotnet build`, `dotnet test`,
  `dotnet format --verify-no-changes`, visual
  smoke test, theme-toggle smoke test.
- **Approved plan path:** (not yet planned in
  detail; will be drafted after M2.2 closes).
- **Status:** Ready (summary).

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

(none)

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

### M2.4 — Empty application routes (summary)

- **Milestone:** M2.
- **Why deferred:** every M2.1 / M2.2 / M2.3
  page reaches an empty state; the
  full-coverage pass over every route is
  natural after the shell is wired.
- **First action (later):** enumerate every
  route in `Components/Pages/`; verify each
  renders an `AppEmptyState` that names the
  page and links back to the design system.

### M2.5 — Responsive and keyboard validation (summary)

- **Milestone:** M2.
- **Why deferred:** the validation pass over
  the shell at the 1280x720 minimum and the
  full keyboard smoke test is the last M2
  task before the M2 closeout.
- **First action (later):** define the
  responsive matrix in
  `docs/ui-principles.md` § 10.1; run the
  keyboard smoke test across every route.

### M2.6 — M2 closeout and external Treehouse dogfooding checkpoint (summary)

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
