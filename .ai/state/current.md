# Current Project State

> **One-page snapshot. Read this first in any new
> session.** The most recent update wins; older
> snapshots live in `.ai/handoffs/`. The state files
> reflect the actual state of the repository; the
> repository wins when the two disagree (see
> `.ai/session-start.md` step 6 — state reconciliation).
>
> **State architecture (M0.5).** This file is the
> human-readable projection. The canonical machine-readable
> state is in `.ai/state/*.json` (the JSON files are the
> source of truth; this file is regenerated from them).
> The capability model is in
> [`.ai/state/capabilities.json`](./capabilities.json);
> the human-readable projection is in
> [`.ai/state/capability-mapping.md`](./capability-mapping.md).
> Small in-flight decisions are in
> [`.ai/state/decision-log.md`](./decision-log.md).
> The self-awareness state is in
> [`.ai/state/session.json`](./session.json).

## Product

- **Name:** AI Engineering Platform
  ([`PRODUCT.md`](./../../PRODUCT.md)).
- **Purpose:** Windows-first control centre for
  AI-assisted software development. The platform
  orchestrates coding agents; it does not replace
  them.
- **Repository:**
  `C:\Users\hkasozi\source\repos\ai-engineering-platform`.
- **Solution file:** `AiEng.Platform.slnx` (.NET 10
  SLN-X format).
- **Primary deployment target:** Windows 11
  (desktop first; secondary: Windows 10).
- **Stack:** .NET 10 / C# 14 / Blazor Web App /
  Tailwind v3 / bUnit 2.7.2 / xUnit 2.9.3.

## Current Milestone

- **Active milestone:** **M2 — Application Shell and
  Navigation** (active; M2.1 delivered 2026-07-11;
  M2.2 delivered 2026-07-11).
- **M2.1 — Application Shell Foundation:**
  **Delivered (2026-07-11).** The shell
  foundation lands: two layouts
  (`AppLayout`, `AppEmptyLayout`), two
  placeholder shell components
  (`AppSidebarSlot`, `AppTopBarSlot`), one
  presentational helper (`AppShellRegion`),
  and the migration of the M1.1 chrome. The
  five M1 template pages and `/design-system`
  reach the new layout root in place. See
  [`implementation-report-m2-1-application-shell-foundation.md`](./../../implementation-report-m2-1-application-shell-foundation.md).
- **M2.2 — Navigation Registry and Sidebar:**
  **Delivered (2026-07-11).** The
  navigation registry lands:
  `INavigationRegistry`, `RouteMetadata`,
  `RouteMetadataAttribute`,
  `RouteRegistry` in
  `src/AiEng.Platform.Application/Navigation/`;
  the composition root
  (`AddPlatformServices` +
  `AddNavigation`) in
  `src/AiEng.Platform.App/Composition/`;
  `AppSidebar`, `AppSidebarItem`,
  `AppNavItem` in
  `src/AiEng.Platform.App/Components/Navigation/`;
  `[RouteMetadata]` on all 6 pages;
  `AppSidebarSlot` and
  `AppSidebarSlotTests` deleted; the
  `Pages_AreReachable_Through_Registry`
  architecture test is active and green.
  The top bar, breadcrumb, page header
  integration, and self-awareness
  dashboard remain in M2.3, M2.4, and
  M2.5. See
  [`implementation-report-m2-2-navigation-registry-sidebar.md`](./../../implementation-report-m2-2-navigation-registry-sidebar.md).
- **M2 next slice:** **M2.3 — Top Bar,
  Breadcrumbs, and Page Headers.** The
  plan at
  [`.ai/plans/M2.3-topbar-breadcrumbs.md`](./../../.ai/plans/M2.3-topbar-breadcrumbs.md)
  is now a full plan in `Awaiting
  Approval` state (promoted from the
  Draft stub in the M2.2 closeout
  session).
- **M2.4 plan stub:** remains a concise
  summary
  (`.ai/plans/M2.4-project-intelligence-dashboard.md`);
  promoted to a full plan when M2.3
  closes.
- **M2.5 / M2.6:** summary entries in
  the task board; planned in detail
  after M2.4 / M2.5 close.
- **M0.5 — Architecture Refinement and Project
  Intelligence:** **Done (closed 2026-07-10;
  coherent commit `1d98acd`).** M0.5 is the
  refinement that lands between M1 and M2. It
  is not a product milestone. The ten M0.5
  improvements are landed; the architecture
  score is 23 → 42 (+19) on five dimensions.
  See
  [`implementation-report-m0.5-architecture-refinement.md`](./../../implementation-report-m0.5-architecture-refinement.md)
  and
  [`.ai/handoffs/2026-07-10-m0.5-architecture-refinement.md`](./../../.ai/handoffs/2026-07-10-m0.5-architecture-refinement.md).

## Current Slice

- **Active slice:** **M2.3 — Top Bar,
  Breadcrumbs, and Page Headers** (plan
  promoted 2026-07-11 from the Draft
  stub to a full plan in
  `Awaiting Approval`). Implementation
  has not started; per the Progressive
  Coding Rule, M2.3 waits for explicit
  user authorisation in the next
  session.
- **Last completed slice:** **M2.2 —
  Navigation Registry and Sidebar**
  (delivered 2026-07-11). The branch
  `feature/m2-2-navigation-registry-sidebar`
  carries the implementation; the
  closeout commit is produced on this
  branch.
- **M2.1 — Application Shell
  Foundation** (delivered 2026-07-11).
  The branch
  `feature/m2-1-application-shell`
  carries the implementation; the M2.1
  closeout commit
  `feat(m2.1): add application shell foundation`
  is on this branch.
- **M0.5 closeout:** the M0.5 refinement is
  closed in the M0.5 commit `1d98acd` (per
  Rule 17 in `AGENTS.md`); the state files
  are reconciled with the repository; the
  `.ai/handoffs/2026-07-10-m0.5-architecture-refinement.md`
  handoff is written; the
  `implementation-report-m0.5-architecture-refinement.md`
  report is written. M0.5 does not modify
  application code or completed milestones.

## Status

- **M0 — Documentation Foundation:** **Done.**
- **M1 — Design System Core:** **Done (closed
  2026-07-10).**
- **M0.5 — Architecture Refinement and Project
  Intelligence:** **Done (closed 2026-07-10;
  commit `1d98acd`).**
  Documentation-only refinement; the ten
  improvements are landed. The architecture
  score is 23 → 42 (+19).
- **M2 — Application Shell and Navigation:**
  **Active (M2.1 Delivered 2026-07-11;
  M2.2 Delivered 2026-07-11;
  M2.3 plan Awaiting Approval;
  M2.4 plan stub in Draft).**
- **M3 through M8:** Planned; no evidence yet.

## Last Completed Milestone

- **M0.5 — Architecture Refinement and Project
  Intelligence**, closed **2026-07-10**. See
  [`implementation-report-m0.5-architecture-refinement.md`](./../../implementation-report-m0.5-architecture-refinement.md)
  and
  [`.ai/handoffs/2026-07-10-m0.5-architecture-refinement.md`](./../../.ai/handoffs/2026-07-10-m0.5-architecture-refinement.md).
- **M1 — Design System Core**, closed
  **2026-07-10** (preceding milestone). The M1
  evidence is unchanged by M0.5; see
  [`implementation-report-m1-closeout.md`](./../../implementation-report-m1-closeout.md)
  and
  [`.ai/handoffs/2026-07-10-m1-closeout.md`](./../../.ai/handoffs/2026-07-10-m1-closeout.md).

## Last Completed Task

- The M2.2 implementation session
  (2026-07-11), which:
  - Delivered M2.2 — Navigation Registry
    and Sidebar per the approved plan
    (`.ai/plans/M2.2-navigation-registry-sidebar.md`).
    The plan was approved 2026-07-11 and
    implementation followed § 16 (24-step
    order) of the plan.
  - Added the navigation registry
    (`INavigationRegistry`,
    `RouteMetadata`,
    `RouteMetadataAttribute`,
    `RouteRegistry`) in
    `src/AiEng.Platform.Application/Navigation/`.
  - Added the composition root
    (`AddPlatformServices` +
    `AddNavigation`) in
    `src/AiEng.Platform.App/Composition/`.
  - Added `AppSidebar`, `AppSidebarItem`,
    `AppNavItem` in
    `src/AiEng.Platform.App/Components/Navigation/`.
  - Replaced `AppSidebarSlot` with
    `AppSidebar` in `AppLayout.razor`;
    deleted `AppSidebarSlot.razor` and
    `AppSidebarSlotTests.cs`.
  - Applied `[RouteMetadata]` to all
    six pages (Home, Counter, Weather,
    Error, NotFound, DesignSystem);
    Error and NotFound use
    `ShowInSidebar = false`.
  - Wired `Program.cs` to call
    `AddPlatformServices(typeof(Program).Assembly)`.
  - Activated the
    `Pages_AreReachable_Through_Registry`
    architecture test (1 new test,
    active and green).
  - Added 28 new bUnit / integration
    tests + 1 new architecture test;
    125 total component tests pass.
  - Updated the project-continuity
    state per Rule 15 (`current.md`,
    `task-board.md`, `session.json`,
    `tasks.json`, `milestones.json`,
    `capabilities.json`, `ROADMAP.md`,
    `handoffs/latest.md`), and
    produced
    `implementation-report-m2-2-navigation-registry-sidebar.md`.
  - Promoted the M2.3 plan stub to a
    full plan in `Awaiting Approval`
    state. T-003 is `Ready`.
  - The session does **not** implement
    M2.3; per the Progressive Coding
    Rule, M2.3 is the next session's
    responsibility.
  - The commit
    `feat(m2.2): add navigation registry and sidebar`
    is the closing receipt; no push
    (no remote configured).
- The M2.1 implementation session
  (2026-07-11) is the prior task; the
  M2 delivery-reconciliation session
  (2026-07-10) is the prior-prior task;
  the M0.5 architecture refinement
  session (2026-07-10) is the
  prior-prior-prior task.

## Active Branch

- **`feature/m2-2-navigation-registry-sidebar`**.
  The M2.2 implementation branch
  carries the M2.2 closeout commit. The
  M0.5 closeout commit `1d98acd` is the
  base; the M2.1 closeout commit
  `feat(m2.1): add application shell foundation`
  is the M2.1 chain tip on
  `feature/m2-1-application-shell`; the
  M2.2 commit is the
  `feature/m2-2-navigation-registry-sidebar`
  tip. No remote is configured, so push
  is skipped (the session records this
  explicitly per the brief).

## Last Stable Commit

- The M2.2 closeout commit
  `feat(m2.2): add navigation registry and sidebar`
  on `feature/m2-2-navigation-registry-sidebar`
  (created 2026-07-11). The parent
  commit is the project-intelligence
  refinement commit
  `docs(state): refine project-intelligence for command-driven reporting`
  on `feature/m2-1-application-shell`
  (the closeout chain is
  `de082fd` → `ef1063c` →
  `32ab73d` on
  `feature/m2-1-application-shell`,
  then the M2.2 commit on
  `feature/m2-2-navigation-registry-sidebar`).
  Working tree is clean at the close of
  the M2.2 session.

## Application Status

- **Runnable.** The Blazor Server app builds and
  serves on `http://localhost:5286`. The five M1
  routes (`/`, `/counter`, `/weather`,
  `/design-system`, `/not-found`) all return 200.
  The M2 application shell is in place: `AppLayout`
  composes the registry-driven `AppSidebar` (M2.2
  the real sidebar), the `AppTopBarSlot` (M2.3
  placeholder), and the content area. The sidebar
  renders four nav items (Home, Counter, Weather,
  Design system) for the four sidebar-visible
  routes; Error and NotFound are correctly hidden
  (`ShowInSidebar = false`). The
  `data-app-region` attributes are present on
  every region. `/design-system` and `/not-found`
  use `AppEmptyLayout`. Light and dark themes
  render correctly (the design-token contract is
  unchanged from M1.1). The keyboard smoke test
  passes: Tab through the shell; every
  interactive element is reachable; focus is
  visible.

## CSS Build Status

- `npm run css:build` → exit 0. Output:
  approximately 11,500 bytes minified at the
  close of the M2.2 session. The M2.1
  Tailwind content-path update
  (`Layouts/**/*.{razor,razor.css}`) plus the
  M2.2 content-path coverage of
  `Components/Navigation/**/*.{razor,razor.css}`
  (already covered by the M2.1 wildcard
  `./src/AiEng.Platform.App/Components/**/*.razor`)
  keep the new navigation utility classes
  in the compiled CSS. All M1 design
  tokens are present in the compiled CSS;
  no token has been added or removed.

## Build Status

- `dotnet build AiEng.Platform.slnx` → exit 0.
  **0 warnings, 0 errors** (with
  `TreatWarningsAsErrors=true` from
  `Directory.Build.props`).

## Test Status

- `dotnet test AiEng.Platform.slnx --no-build` →
  **129 passed, 4 skipped, 0 failed.**
  - `AiEng.Platform.UnitTests`: 0 tests (no
    Domain logic in M1, M2.1, or M2.2).
  - `AiEng.Platform.ComponentTests`: 125
    bUnit / integration tests, all passing
    (77 M1.2 + 25 M2.1 + 23 M2.2 bUnit).
  - `AiEng.Platform.ArchitectureTests`: 8
    tests in total — 4 active (passing)
    and 4 registered-but-disabled
    (skipped) per ADR-016 and the M4-D
    activation milestone. The
    `Pages_AreReachable_Through_Registry`
    test is the new M2.2 active test.

## Implemented Capabilities

The 19 M1.2 components, in five categories:

- **Primitives (7):** `AppButton`, `AppIconButton`,
  `AppBadge`, `AppStatusDot`, `AppContainer`,
  `AppStack`, `AppInputLabel` (the last is in the
  `Inputs/` group; counted under Primitives by the
  M1.2 plan).
- **Layout (4):** `AppCard`, `AppSection`,
  `AppPanel`, `AppToolbar` (the M1.2 plan groups
  `AppToolbar` under Layout).
- **Display (2):** `AppAvatar`, `AppPageHeader`.
- **Feedback (5):** `AppAlert`, `AppEmptyState`,
  `AppErrorState`, `AppLoading`, `AppSkeleton`.
- **Inputs (1):** `AppInputLabel`.

Plus the M2.1 application shell foundation
(2 layouts + 2 placeholder shell components + 1
presentational helper):

- **Layouts (2):** `AppLayout` (the M2 layout
  root; three-region composition:
  `AppSidebar` (M2.2 the real
  registry-driven sidebar) +
  `AppTopBarSlot` (M2.3 placeholder) +
  content area) and `AppEmptyLayout` (the
  M2 alternate layout; no sidebar, no top
  bar).
- **Shell placeholder components (1):**
  `AppTopBarSlot` (renders an
  `AppPageHeader` and an `AppAlert` saying
  "Top bar lands in M2.3"; M2.2 removed
  `AppSidebarSlot`, replaced by the real
  `AppSidebar`).
- **Presentational helper (1):**
  `AppShellRegion` (renders
  `data-app-region="<name>"` and
  `aria-label="<name>"`; the M2.5
  accessibility audit and the M2.4
  dashboard tests query the data
  attribute).

Plus the M2.2 navigation registry and
registry-driven sidebar (replaces the M2.1
`AppSidebarSlot` placeholder):

- **Application.Navigation (4 types):**
  `INavigationRegistry` (the registry
  contract), `RouteMetadata` (the data
  record), `RouteMetadataAttribute` (the
  page-level annotation), and
  `RouteRegistry` (the assembly-scan
  implementation; produces a sorted,
  parent-aware list of routes).
- **Composition (2 extensions):**
  `NavigationServiceCollectionExtensions.AddNavigation`
  (registers `INavigationRegistry` as a
  singleton), and
  `ServiceCollectionExtensions.AddPlatformServices`
  (the composition root that calls
  `AddNavigation`; called once from
  `Program.cs`).
- **Navigation components (3):**
  `AppSidebar` (data-driven sidebar;
  injects `INavigationRegistry`,
  filters by `ShowInSidebar`, renders
  one `AppNavItem` per visible route
  inside `AppShellRegion
  Name="sidebar"`), `AppSidebarItem`
  (sidebar section group with title
  heading and `AppStack Gap=Small`),
  `AppNavItem` (renders a `NavLink`
  with the design-system ghost-button
  styling, supports `Match=All` /
  `Prefix` via `Route.MatchPrefix`,
  sets `aria-current="page"` on the
  active link, renders an `AppBadge`
  if `Route.BadgeText` is non-null).
- **Page attributes (6):** `[RouteMetadata]`
  applied to Home (`/`, `Home`, 0),
  Counter (`/counter`, 1), Weather
  (`/weather`, 2), Error (`/error`, 3,
  `ShowInSidebar=false`), NotFound
  (`/not-found`, 99,
  `ShowInSidebar=false`), DesignSystem
  (`/design-system`, 100).
- **Architecture test (1 new,
  active):** `Pages_AreReachable_Through_Registry`
  walks `Components/Pages/`, asserts
  every `.razor` has a
  `[RouteMetadata]` attribute whose
  `Href` is in the registry.

Plus the supporting infrastructure:

- The Tailwind v3 + PostCSS pipeline
  (`npm run css:build`, `npm run css:watch`).
- The design-token catalogue (`themes.css`,
  `tokens.css`).
- The light and dark themes via the data-attribute
  theme switcher.
- The `/design-system` documentation page.
- The 3 active + 4 registered-but-disabled
  architecture tests in
  `tests/AiEng.Platform.ArchitectureTests/Boundaries/`.

## External Tools Dogfooded

- **None in this session.** The M1 dogfooding
  checkpoint (`lavish-axi`) was deferred during the
  M1 closeout session because `lavish-axi` is not
  installed on the host. The deferral is recorded in
  [`.ai/reviews/M1-design-system-lavish-axi-review.md`](./../../.ai/reviews/M1-design-system-lavish-axi-review.md).
- The M2 dogfooding checkpoint (Treehouse, for
  isolated development worktrees) is documented in
  `ROADMAP.md` but has not been exercised; the M2
  milestone is not started.

## Known Issues

- **`AppToolbar` example missing on
  `/design-system`.** The `AppToolbar` component
  ships and is unit-tested, but the
  `DesignSystem.razor` page does not include a
  Toolbar section. 18/19 component CSS classes
  appear in the rendered HTML. This is a cosmetic
  gap, not a DoD failure. A `Ready` item in
  [`.ai/state/task-board.md`](./task-board.md) adds
  a Toolbar example to the doc page.
- **No git remote.** The repository has no
  configured remote. The first three commits are
  local-only. Adding a remote is a separate
  decision.

## Deferred Findings

- **`lavish-axi` M1 design-system review is
  deferred.** The tool is not installed on the
  host; the only artefact on the filesystem is
  `agent-workbench/tools/lavish-axi.md`, a spec for
  an event-bus daemon, not a review tool. See
  [`.ai/reviews/M1-design-system-lavish-axi-review.md`](./../../.ai/reviews/M1-design-system-lavish-axi-review.md)
  and the `Blocked` section of
  [`.ai/state/task-board.md`](./task-board.md).
- **M2 plan stubs are `Draft`.** The M2.4
  plan stub is concise summary; the
  full plan lands when the previous
  slice (M2.3) closes. M2.2 and M2.3
  plans are no longer stubs. The
  Progressive Coding Rule
  (`.ai/workflows/progressive-coding.md`)
  is the operating rule that governs
  task selection; each task in the
  group moves through the 13-step
  task lifecycle, no step is optional.

## Active Plan

- [`.ai/plans/M2.1-application-shell-skeleton.md`](./../../.ai/plans/M2.1-application-shell-skeleton.md)
  — M2.1, **Delivered (2026-07-11)**. The
  closeout commit
  `feat(m2.1): add application shell foundation`
  is on
  `feature/m2-1-application-shell`. The
  plan is preserved in
  `.ai/plans/M2.1-application-shell-skeleton.md`
  for traceability; the status field
  reflects the closeout.
- [`.ai/plans/M2.2-navigation-registry-sidebar.md`](./../../.ai/plans/M2.2-navigation-registry-sidebar.md)
  — M2.2 plan, **Delivered (2026-07-11)**
  (promoted from `Draft` to `Awaiting
  Approval` in the M2.1 closeout
  session; approved and implemented in
  the M2.2 closeout session).
- [`.ai/plans/M2.3-topbar-breadcrumbs.md`](./../../.ai/plans/M2.3-topbar-breadcrumbs.md)
  — M2.3 plan, **Awaiting Approval**
  (promoted from `Draft` stub to a
  full plan in the M2.2 closeout
  session, 2026-07-11).
- [`.ai/plans/M2.4-project-intelligence-dashboard.md`](./../../.ai/plans/M2.4-project-intelligence-dashboard.md)
  — M2.4 plan stub, status `Draft`.

## Last Implementation Report

- [`implementation-report-m2-2-navigation-registry-sidebar.md`](./../../implementation-report-m2-2-navigation-registry-sidebar.md)
  — the M2.2 implementation report
  (the closing receipt for M2.2, 2026-07-11).
- [`implementation-report-m2-1-application-shell-foundation.md`](./../../implementation-report-m2-1-application-shell-foundation.md)
  — the M2.1 implementation report
  (the closing receipt for M2.1, 2026-07-11).
- [`implementation-report-m0.5-architecture-refinement.md`](./../../implementation-report-m0.5-architecture-refinement.md)
  — the M0.5 architecture refinement report
  (the closing receipt for M0.5).
- [`reconciliation-report-m2-task-breakdown.md`](./../../reconciliation-report-m2-task-breakdown.md)
  — the M2 delivery-reconciliation report
  (the closing receipt for the M2
  reconciliation session, 2026-07-10).
- [`implementation-report-m1-closeout.md`](./../../implementation-report-m1-closeout.md)
  — the M1 closeout report (preceding
  milestone; M0.5, the M2 reconciliation,
  and the M2.1 closeout preserve M1
  evidence).

## Next Recommended Task

> **M2.3 — Top Bar, Breadcrumbs, and Page
> Headers.** Read
> [`.ai/plans/M2.3-topbar-breadcrumbs.md`](./../../.ai/plans/M2.3-topbar-breadcrumbs.md).
> The plan was promoted from the
> `Draft` stub to a full plan in
> `Awaiting Approval` (the M2.2
> closeout session produced the
> promotion). The first action in the
> next session is to either approve
> the M2.3 plan (and start M2.3
> implementation per the plan's own
> order) or amend the plan and
> re-submit it.
> The next session follows the **Progressive
> Coding Rule** in
> [`.ai/workflows/progressive-coding.md`](./../../.ai/workflows/progressive-coding.md):
> one task per session; 13-step lifecycle;
> stop after the coherent commit.

The detailed breakdown of the M2 slices is
in
[`.ai/state/task-board.md`](./task-board.md)
and the M2 plan files in
[`.ai/plans/`](./../../.ai/plans/). The
next three actionable items are:

1. **M2.3 — Top Bar, Breadcrumbs, and
   Page Headers** (plan `Awaiting
   Approval`; implementation follows
   the plan's own order).
2. **M2.4 — Project Intelligence
   Dashboard** (plan stub `Draft`;
   promoted to a full plan when M2.3
   closes).
3. **M1 follow-up — Add `AppToolbar`
   example to `/design-system`**
   (cosmetic; the work is small and can
   be folded into M2.3 if appropriate).

## Last Updated

- **2026-07-11** (M2.2 closeout
  session). This version supersedes
  the M2.1 closeout version
  (2026-07-11) and the M2
  delivery-reconciliation version
  (2026-07-10). The M2.2 closeout
  session delivers the M2.2
  navigation registry and
  registry-driven sidebar, runs the
  validation suite (npm css:build,
  dotnet build, dotnet test, dotnet
  format, visual smoke test),
  updates the project-continuity
  state per Rule 15, produces
  `implementation-report-m2-2-navigation-registry-sidebar.md`,
  promotes the M2.3 plan stub to a
  full plan in `Awaiting Approval`,
  and creates the M2.2 closeout
  commit. The M2.2 commit is the
  most recent commit on
  `feature/m2-2-navigation-registry-sidebar`;
  no push (no remote configured).

## Linked Artefacts

- [`VISION.md`](./../../VISION.md) — the
  permanent vision document (M0.5;
  tier 1 of the document hierarchy).
- [`.ai/state/task-board.md`](./task-board.md) —
  the live work queue (M2.1, M2.2
  Done; M2.3 in `Ready`, plan
  `Awaiting Approval`; M2.4 in
  `Draft`).
- [`.ai/handoffs/latest.md`](./../../.ai/handoffs/latest.md) —
  the most recent handoff.
- [`.ai/handoffs/2026-07-10-m0.5-architecture-refinement.md`](./../../.ai/handoffs/2026-07-10-m0.5-architecture-refinement.md)
  — the M0.5 closeout session handoff.
- [`.ai/handoffs/2026-07-10-m1-closeout.md`](./../../.ai/handoffs/2026-07-10-m1-closeout.md) —
  the M1 closeout session handoff.
- [`.ai/plans/M2.1-application-shell-skeleton.md`](./../../.ai/plans/M2.1-application-shell-skeleton.md) —
  the revised M2.1 plan (Delivered,
  2026-07-11).
- [`.ai/plans/M2.2-navigation-registry-sidebar.md`](./../../.ai/plans/M2.2-navigation-registry-sidebar.md) —
  the M2.2 plan (Delivered,
  2026-07-11).
- [`.ai/plans/M2.3-topbar-breadcrumbs.md`](./../../.ai/plans/M2.3-topbar-breadcrumbs.md) —
  the M2.3 plan (Awaiting Approval,
  2026-07-11; promoted from `Draft`
  stub in the M2.2 closeout session).
- [`.ai/plans/M2.4-project-intelligence-dashboard.md`](./../../.ai/plans/M2.4-project-intelligence-dashboard.md) —
  the M2.4 plan stub (Draft, 2026-07-10).
- [`.ai/plans/master-delivery-plan.md`](./../../.ai/plans/master-delivery-plan.md) —
  the master delivery plan.
- [`.ai/reviews/M1-design-system-lavish-axi-review.md`](./../../.ai/reviews/M1-design-system-lavish-axi-review.md) —
  the deferred `lavish-axi` review record.
- [`PRODUCT.md`](./../../PRODUCT.md) — the product
  definition.
- [`ROADMAP.md`](./../../ROADMAP.md) — the
  milestone plan (source of truth for milestone
  ordering and scope).
- [`DECISIONS.md`](./../../DECISIONS.md) — ADR
  index (ADR-011 through ADR-016 are the
  relevant ones for M1 / M2).
- [`AGENTS.md`](./../../AGENTS.md) — the
  constitution (17 rules; Document Map in
  § 6 includes the M0.5 tiered hierarchy).
- [`.ai/session-start.md`](./../../.ai/session-start.md) —
  the operational sequence.
- [`.ai/workflows/progressive-coding.md`](./../../.ai/workflows/progressive-coding.md) —
  the Progressive Coding Rule
  (M2 delivery-reconciliation, 2026-07-10).
- [`implementation-report-m0.5-architecture-refinement.md`](./../../implementation-report-m0.5-architecture-refinement.md)
  — the M0.5 architecture review (the
  closing receipt for M0.5).
- [`reconciliation-report-m2-task-breakdown.md`](./../../reconciliation-report-m2-task-breakdown.md)
  — the M2 delivery-reconciliation
  report (the closing receipt for the
  M2 reconciliation session, 2026-07-10).
- [`implementation-report-m1-closeout.md`](./../../implementation-report-m1-closeout.md)
  — the M1 closeout report (preceding
  milestone; M0.5, the M2
  reconciliation, and the M2.1 closeout
  preserve M1 evidence).
- [`implementation-report-m2-1-application-shell-foundation.md`](./../../implementation-report-m2-1-application-shell-foundation.md)
  — the M2.1 implementation report
  (the closing receipt for M2.1,
  2026-07-11).
- [`implementation-report-m2-2-navigation-registry-sidebar.md`](./../../implementation-report-m2-2-navigation-registry-sidebar.md)
  — the M2.2 implementation report
  (the closing receipt for M2.2,
  2026-07-11).
- [`docs/dashboard.md`](./../../docs/dashboard.md)
  — the product dashboard definition (M0.5).
- [`.ai/backlog/`](./../../.ai/backlog/) —
  the engineering backlog (M0.5).
- [`.ai/state/decision-log.md`](./decision-log.md)
  — the decision log (M0.5).
- [`.ai/state/capability-mapping.md`](./capability-mapping.md)
  — the capability mapping (M0.5).
