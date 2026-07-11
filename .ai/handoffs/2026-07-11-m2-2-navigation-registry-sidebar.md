# Session Handoff — M2.2 — Navigation Registry and Sidebar

> **Per-session handoff for the M2.2
> closeout session, 2026-07-11.**
> M2.2 — Navigation Registry and Sidebar
> is **Delivered**. The coherent commit
> `feat(m2.2): add navigation registry and sidebar`
> is on
> `feature/m2-2-navigation-registry-sidebar`.
> The next session opens the M2.3 plan
> (`.ai/plans/M2.3-topbar-breadcrumbs.md`,
> `Awaiting Approval`).

---

## Task

Deliver the M2.2 — Navigation Registry
and Sidebar slice per the approved plan
(`.ai/plans/M2.2-navigation-registry-sidebar.md`):
land the `INavigationRegistry` data
model in `Application.Navigation`, the
registry-driven sidebar in
`Components.Navigation`, the
`Pages_AreReachable_Through_Registry`
architecture test, and apply
`[RouteMetadata]` to every page in
`Components/Pages/`. Reconcile the stale
projections (`.ai/state/current.md`,
`.ai/state/task-board.md`,
`.ai/state/session.json`) per the user
instruction, update the capability
graph, the tasks JSON, the milestones
JSON, and `ROADMAP.md`, write the
implementation report and handoff,
commit, and stop.

## Branch

`feature/m2-2-navigation-registry-sidebar`.

The M2.2 closeout commit is on this
branch. The parent commit is the
project-intelligence refinement commit
on `feature/m2-1-application-shell`
(the M2.1 closeout chain:
`de082fd` → `ef1063c` → `32ab73d` on
`feature/m2-1-application-shell`, then
the M2.2 commit on
`feature/m2-2-navigation-registry-sidebar`).

No remote is configured; push is
skipped per the brief.

## Current Status

**Awaiting review.** M2.2 is closed.
The slice satisfies the plan's
end-of-slice conditions:

1. The sidebar reads from
   `INavigationRegistry` and renders
   one `AppNavItem` per
   sidebar-visible route.
2. Every page in
   `Components/Pages/` has a
   `[RouteMetadata]` attribute whose
   `Href` matches a real route.
3. The
   `Pages_AreReachable_Through_Registry`
   architecture test is active and
   green.

The next session approves the M2.3
plan and starts M2.3 implementation.

## Work Completed

- 4 new types in
  `src/AiEng.Platform.Application/Navigation/`:
  `INavigationRegistry`,
  `RouteMetadata`,
  `RouteMetadataAttribute`,
  `RouteRegistry`.
- 2 new extension methods in
  `src/AiEng.Platform.App/Composition/`:
  `ServiceCollectionExtensions.AddPlatformServices`
  and
  `NavigationServiceCollectionExtensions.AddNavigation`.
- 3 new components in
  `src/AiEng.Platform.App/Components/Navigation/`:
  `AppSidebar`, `AppSidebarItem`,
  `AppNavItem` (each with its
  `.razor.css`).
- `[RouteMetadata]` applied to all
  6 pages (Home, Counter, Weather,
  Error, NotFound, DesignSystem);
  Error and NotFound use
  `ShowInSidebar = false`.
- `AppLayout.razor` updated to
  render `<AppSidebar />` (the M2.1
  `<AppSidebarSlot />` placeholder is
  deleted).
- `Program.cs` wired to call
  `AddPlatformServices(typeof(Program).Assembly)`.
- `_Imports.razor` files updated
  for the new namespaces and
  folders.
- 28 new bUnit / integration tests
  across 5 new test files
  (`RouteRegistryTests`,
  `AppNavItemTests`,
  `AppSidebarItemTests`,
  `AppSidebarTests`,
  `NavigationServiceCollectionExtensionsTests`).
- 1 new architecture test
  (`PagesAreReachableThroughRegistryTests`)
  — active and green.
- M2.1 `AppSidebarSlot.razor` and
  `AppSidebarSlotTests.cs` deleted
  (the registry-driven `AppSidebar`
  replaces the placeholder).
- 8 pre-existing `AppLayoutTests`
  updated to register a stub
  `INavigationRegistry` in the
  `BunitContext` ctor.
- Project-continuity state
  reconciled (`.ai/state/current.md`,
  `.ai/state/task-board.md`,
  `.ai/state/session.json`,
  `.ai/state/tasks.json`,
  `.ai/state/milestones.json`,
  `.ai/state/capabilities.json`).
- `ROADMAP.md` M2.2 row updated to
  `Delivered (M2.2, 2026-07-11)`;
  the M2 DoD section records the
  M2.2 closeout.
- M2.3 plan stub promoted to a
  full plan in `Awaiting Approval`
  (`.ai/plans/M2.3-topbar-breadcrumbs.md`
  with a new § 8 Implementation
  Order).
- Implementation report:
  `implementation-report-m2-2-navigation-registry-sidebar.md`.
- Per-session handoff: this file
  (mirrored to
  `.ai/handoffs/latest.md`).
- Coherent commit
  `feat(m2.2): add navigation registry and sidebar`
  on
  `feature/m2-2-navigation-registry-sidebar`.

## Files Changed

Created:

- `src/AiEng.Platform.Application/Navigation/INavigationRegistry.cs`
- `src/AiEng.Platform.Application/Navigation/RouteMetadata.cs`
- `src/AiEng.Platform.Application/Navigation/RouteMetadataAttribute.cs`
- `src/AiEng.Platform.Application/Navigation/RouteRegistry.cs`
- `src/AiEng.Platform.App/Composition/ServiceCollectionExtensions.cs`
- `src/AiEng.Platform.App/Composition/NavigationServiceCollectionExtensions.cs`
- `src/AiEng.Platform.App/Components/Navigation/_Imports.razor`
- `src/AiEng.Platform.App/Components/Navigation/AppSidebar.razor`
- `src/AiEng.Platform.App/Components/Navigation/AppSidebar.razor.css`
- `src/AiEng.Platform.App/Components/Navigation/AppSidebarItem.razor`
- `src/AiEng.Platform.App/Components/Navigation/AppSidebarItem.razor.css`
- `src/AiEng.Platform.App/Components/Navigation/AppNavItem.razor`
- `src/AiEng.Platform.App/Components/Navigation/AppNavItem.razor.css`
- `tests/AiEng.Platform.ComponentTests/Navigation/RouteRegistryTests.cs`
- `tests/AiEng.Platform.ComponentTests/Navigation/AppNavItemTests.cs`
- `tests/AiEng.Platform.ComponentTests/Navigation/AppSidebarItemTests.cs`
- `tests/AiEng.Platform.ComponentTests/Navigation/AppSidebarTests.cs`
- `tests/AiEng.Platform.ComponentTests/Navigation/NavigationServiceCollectionExtensionsTests.cs`
- `tests/AiEng.Platform.ArchitectureTests/PagesAreReachableThroughRegistryTests.cs`
- `implementation-report-m2-2-navigation-registry-sidebar.md`
- `.ai/handoffs/2026-07-11-m2-2-navigation-registry-sidebar.md`
- `.ai/handoffs/latest.md` (mirror)

Modified:

- `src/AiEng.Platform.App/Components/_Imports.razor`
- `src/AiEng.Platform.App/Layouts/_Imports.razor`
- `src/AiEng.Platform.App/Layouts/AppLayout.razor`
- `src/AiEng.Platform.App/Program.cs`
- `src/AiEng.Platform.App/Components/Pages/Home.razor`
- `src/AiEng.Platform.App/Components/Pages/Counter.razor`
- `src/AiEng.Platform.App/Components/Pages/Weather.razor`
- `src/AiEng.Platform.App/Components/Pages/Error.razor`
- `src/AiEng.Platform.App/Components/Pages/NotFound.razor`
- `src/AiEng.Platform.App/Components/Pages/DesignSystem.razor`
- `tests/AiEng.Platform.ComponentTests/Layouts/AppLayoutTests.cs`
- `ROADMAP.md`
- `.ai/state/current.md`
- `.ai/state/task-board.md`
- `.ai/state/session.json`
- `.ai/state/tasks.json`
- `.ai/state/milestones.json`
- `.ai/state/capabilities.json`
- `.ai/plans/M2.3-topbar-breadcrumbs.md`
  (Draft stub → full Awaiting
  Approval plan)

Deleted:

- `src/AiEng.Platform.App/Components/Shell/AppSidebarSlot.razor`
- `tests/AiEng.Platform.ComponentTests/Shell/AppSidebarSlotTests.cs`

## Commands Run

- `dotnet build AiEng.Platform.slnx`
  — 0 warnings, 0 errors.
- `dotnet test AiEng.Platform.slnx --no-build`
  — 125 passed, 0 failed
  (component); 4 passed, 4
  skipped, 0 failed
  (architecture).
- `npm run css:build` — exit 0.
- `dotnet format --verify-no-changes`
  — clean (after
  `unix2dos` to convert the
  newly created files to
  CRLF).
- `dotnet run --project src/AiEng.Platform.App --launch-profile http`
  + 5 `curl` — all 5 routes
  return 200; the `/` HTML
  contains the 4 sidebar-visible
  `href`s.

## Test Status

- **Last build:** pass (0
  warnings, 0 errors with
  `TreatWarningsAsErrors=true`).
- **Last test run:** 125
  passed, 0 failed (component);
  4 passed, 4 skipped, 0 failed
  (architecture).
- **Last format check:** clean
  (`dotnet format
  --verify-no-changes`).
- **Last visual smoke test:**
  5/5 routes 200; the sidebar
  renders the 4 expected nav
  items.
- **Open regressions:** none.

## Unresolved Issues

None. The M2.2 slice is closed.
The only forward-looking items
are the next session's M2.3
implementation (top bar,
breadcrumb, page header
integration), the M2.4 plan
promotion (project intelligence
dashboard, still in `Draft`),
and the cosmetic M1 follow-up
(`AppToolbar` example on
`/design-system`).

## Exact Next Step

> **Approve the M2.3 plan and
> start M2.3 implementation per
> the plan's own order.** Read
> `.ai/plans/M2.3-topbar-breadcrumbs.md`
> end-to-end. Sign off § 17.
> Branch off the M2.2 closeout
> commit:
> `git checkout -b feature/m2-3-topbar-breadcrumbs feature/m2-2-navigation-registry-sidebar`.
> Follow the 13-step lifecycle in
> `.ai/workflows/progressive-coding.md`:
> 1. Land `AppTopBar`
>    (replaces `AppTopBarSlot`).
> 2. Land `AppBreadcrumb`
>    (consumes the M2.2
>    `INavigationRegistry`).
> 3. Move the theme toggle into
>    the top bar's `Trailing`
>    slot.
> 4. Add the user avatar slot.
> 5. Wire `AppBreadcrumb` into
>    `AppPageHeader.Breadcrumbs`
>    on every page that uses
>    `AppPageHeader`.
> 6. Replace `<AppTopBarSlot />`
>    with `<AppTopBar />` in
>    `AppLayout.razor`.
> 7. Add 18–24 bUnit tests.
> 8. Validate (css:build,
>    build, test, format, visual
>    smoke + theme toggle).
> 9. Update the
>    project-continuity state.
> 10. Promote the M2.4 plan
>     stub to a full plan in
>     `Awaiting Approval`.
> 11. Write
>     `implementation-report-m2-3-topbar-breadcrumbs.md`
>     and the per-session
>     handoff.
> 12. Single coherent commit
>     `feat(m2.3): add top bar, breadcrumb, and page header integration`
>     on
>     `feature/m2-3-topbar-breadcrumbs`.
> 13. Stop. Do NOT begin M2.4.

## Documents the Next Session Must Read

In the order they must be read.

1. `AGENTS.md`
2. `.ai/session-start.md`
3. `PRODUCT.md` — the product
   definition.
4. `.ai/state/current.md` —
   the one-page snapshot.
5. `.ai/state/task-board.md` —
   the live work queue.
6. `.ai/handoffs/latest.md` —
   the most recent handoff
   (this file, mirrored).
7. `.ai/plans/M2.3-topbar-breadcrumbs.md`
   — the M2.3 plan to approve
   and implement.
8. `implementation-report-m2-2-navigation-registry-sidebar.md`
   — the M2.2 implementation
   report (the receipt for the
   preceding slice).
9. `implementation-report-m2-1-application-shell-foundation.md`
   — the M2.1 implementation
   report (the substrate M2.3
   composes against).
10. `ROADMAP.md` — the M2
    section, the matrix in § 4,
    and the M2.3 DoD entry.
11. `docs/design-system.md` —
    the design-system catalogue
    (M3 page headers use
    `AppPageHeader`; the M2.3
    plan wires `Breadcrumbs`
    into it).
12. The M2.3 plan's linked
    artefact list
    (`.ai/plans/M2.3-topbar-breadcrumbs.md`
    § 7 Approval).
13. `docs/dashboard.md` —
    § 3.1 names the top bar's
    slots (title, current time,
    theme toggle) and informs
    the M2.3 composition.
14. `docs/ui-principles.md`
    § 2.1 — the layout root
    rule (M2.3 follows the M2.1
    composition; no new layout
    root).

## Linked Artefacts

- `.ai/plans/M2.2-navigation-registry-sidebar.md`
  — the approved plan this
  handoff closes.
- `.ai/plans/M2.3-topbar-breadcrumbs.md`
  — the next plan (Awaiting
  Approval).
- `task-brief.md` — none
  produced for M2.2.
- `implementation-plan.md` —
  the M2.2 plan serves as the
  plan; no separate
  implementation-plan.md was
  produced.
- `PRODUCT.md` — the product
  definition; the M2.2 plan's
  "Why it matters" cell cites
  § 1 (vision) and § 2
  (capabilities).
- Current commit hash:
  `feat(m2.2): add navigation registry and sidebar`
  on
  `feature/m2-2-navigation-registry-sidebar`.
- Current branch:
  `feature/m2-2-navigation-registry-sidebar`.
- In-flight worktree or stash:
  none.
- `.ai/state/current.md` and
  `.ai/state/task-board.md` —
  the project-continuity state
  (Rule 15 in `AGENTS.md`).
- The implementation report
  that pairs with this
  handoff:
  `implementation-report-m2-2-navigation-registry-sidebar.md`.
- Prior handoffs:
  `.ai/handoffs/2026-07-11-m2-1-application-shell-foundation.md`,
  `.ai/handoffs/2026-07-10-m1-closeout.md`,
  `.ai/handoffs/2026-07-10-m0.5-architecture-refinement.md`.
