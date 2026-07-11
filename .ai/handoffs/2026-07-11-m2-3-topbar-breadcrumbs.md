# Session Handoff — M2.3 — Top Bar, Breadcrumbs, and Page Headers

> **Per-session handoff for the M2.3
> closeout session, 2026-07-11.**
> M2.3 — Top Bar, Breadcrumbs, and
> Page Headers is **Delivered**. The
> coherent commit
> `feat(m2.3): add top bar, breadcrumb, and page header integration`
> is on
> `feature/m2-3-topbar-breadcrumbs`.
> The next session opens the M2.4 plan
> (`.ai/plans/M2.4-project-intelligence-dashboard.md`,
> `Awaiting Approval`).

---

## Task

Deliver the M2.3 — Top Bar,
Breadcrumbs, and Page Headers slice
per the approved plan
(`.ai/plans/M2.3-topbar-breadcrumbs.md`):
land `AppTopBar` (replacing the M2.1
`AppTopBarSlot` placeholder), land
`AppBreadcrumb` (consuming the M2.2
`INavigationRegistry`), move the
M1.1 theme toggle into the top bar's
`Trailing` slot, add the user avatar
slot, and wire `AppBreadcrumb` into
`AppPageHeader.Breadcrumbs` on every
page that uses `AppPageHeader`.
Reconcile the stale projections
(`.ai/state/current.md`,
`.ai/state/task-board.md`,
`.ai/state/session.json`) per the
user instruction, update the
capability graph, the tasks JSON, the
milestones JSON, and `ROADMAP.md`,
write the implementation report and
handoff, commit, and stop.

## Branch

`feature/m2-3-topbar-breadcrumbs`.

The M2.3 closeout commit is on this
branch. The parent commit is the M2.2
closeout commit on
`feature/m2-2-navigation-registry-sidebar`
(the closeout chain:
`de082fd` → `ef1063c` → `32ab73d` on
`feature/m2-1-application-shell`,
then `3a2c3cb` on
`feature/m2-2-navigation-registry-sidebar`,
then the M2.3 commit on
`feature/m2-3-topbar-breadcrumbs`).

No remote is configured; push is
skipped per the brief.

## Current Status

**Awaiting review.** M2.3 is closed.
The slice satisfies the plan's
end-of-slice conditions:

1. The top bar, breadcrumb, and page
   header are integrated; the theme
   toggle is relocated to the top
   bar; the `AppTopBar` replaces
   the M2.1 `AppTopBarSlot`
   placeholder; the `AppBreadcrumb`
   walks the M2.2 registry's
   `Parent` chain.
2. Every page that uses
   `AppPageHeader` is wired to
   `AppBreadcrumb` via
   `AppPageHeader.Breadcrumbs` (the
   only such page today is
   `DesignSystem.razor`; the wiring
   is in place for every page that
   opts in).
3. The M2.1
   `Pages_AreReachable_Through_Registry`
   architecture test is active and
   green (the M2.3 work did not
   add a new architecture test;
   the optional `Breadcrumb_Follows_Registry_Parent_Chain`
   test was skipped per plan § 8
   step 11).

The next session approves the M2.4
plan and starts M2.4 implementation.

## Work Completed

- 4 new components in
  `src/AiEng.Platform.App/Components/`:
  `AppTopBar` (replaces the M2.1
  `AppTopBarSlot` placeholder),
  `AppThemeToggle` (the M1.1 theme
  toggle relocated to the top
  bar's `Trailing` slot),
  `AppUserAvatarSlot` (the user
  avatar placeholder; the M3+
  session replaces it with the
  real user identity surface),
  and `AppBreadcrumb` (consumes
  the M2.2 `INavigationRegistry`).
- `AppLayout.razor` updated to
  render `<AppTopBar />` (the M2.1
  `<AppTopBarSlot />` placeholder
  is deleted).
- `AppBreadcrumb` wired into
  `AppPageHeader.Breadcrumbs` on
  `DesignSystem.razor`.
- 27 new bUnit tests across 4 new
  test files
  (`AppTopBarTests`,
  `AppThemeToggleTests`,
  `AppUserAvatarSlotTests`,
  `AppBreadcrumbTests`).
- 6 obsolete `AppTopBarSlotTests`
  removed.
- 8 pre-existing `AppLayoutTests`
  updated to register the
  `INavigationRegistry` and the
  bUnit `JSInterop` mocks
  (`appTheme.get` and `appTheme.set`)
  in the `BunitContext` ctor.
- Project-continuity state
  reconciled (`.ai/state/current.md`,
  `.ai/state/task-board.md`,
  `.ai/state/session.json`,
  `.ai/state/tasks.json`,
  `.ai/state/milestones.json`,
  `.ai/state/capabilities.json`).
- `ROADMAP.md` M2.3 row updated to
  `Delivered (M2.3, 2026-07-11)`;
  the M2 DoD section records the
  M2.3 closeout.
- M2.4 plan stub promoted to a
  full plan in `Awaiting Approval`
  (`.ai/plans/M2.4-project-intelligence-dashboard.md`
  with a new § 8 Implementation
  Order).
- Implementation report:
  `implementation-report-m2-3-topbar-breadcrumbs.md`.
- Per-session handoff: this file
  (mirrored to
  `.ai/handoffs/latest.md`).
- Coherent commit
  `feat(m2.3): add top bar, breadcrumb, and page header integration`
  on
  `feature/m2-3-topbar-breadcrumbs`.

## Files Changed

Created:

- `src/AiEng.Platform.App/Components/Shell/AppTopBar.razor`
- `src/AiEng.Platform.App/Components/Shell/AppTopBar.razor.css`
- `src/AiEng.Platform.App/Components/Shell/AppThemeToggle.razor`
- `src/AiEng.Platform.App/Components/Shell/AppThemeToggle.razor.css`
- `src/AiEng.Platform.App/Components/Shell/AppUserAvatarSlot.razor`
- `src/AiEng.Platform.App/Components/Shell/AppUserAvatarSlot.razor.css`
- `src/AiEng.Platform.App/Components/Navigation/AppBreadcrumb.razor`
- `src/AiEng.Platform.App/Components/Navigation/AppBreadcrumb.razor.css`
- `tests/AiEng.Platform.ComponentTests/Shell/AppTopBarTests.cs`
- `tests/AiEng.Platform.ComponentTests/Shell/AppThemeToggleTests.cs`
- `tests/AiEng.Platform.ComponentTests/Shell/AppUserAvatarSlotTests.cs`
- `tests/AiEng.Platform.ComponentTests/Navigation/AppBreadcrumbTests.cs`
- `implementation-report-m2-3-topbar-breadcrumbs.md`
- `.ai/handoffs/2026-07-11-m2-3-topbar-breadcrumbs.md`
- `.ai/handoffs/latest.md` (mirror)

Modified:

- `src/AiEng.Platform.App/Components/App.razor`
- `src/AiEng.Platform.App/Components/Pages/DesignSystem.razor`
- `src/AiEng.Platform.App/Layouts/AppLayout.razor`
- `tests/AiEng.Platform.ComponentTests/Layouts/AppLayoutTests.cs`
- `ROADMAP.md`
- `.ai/state/current.md`
- `.ai/state/task-board.md`
- `.ai/state/session.json`
- `.ai/state/tasks.json`
- `.ai/state/milestones.json`
- `.ai/state/capabilities.json`
- `.ai/plans/M2.4-project-intelligence-dashboard.md`
  (Draft stub → full Awaiting
  Approval plan)

Deleted:

- `src/AiEng.Platform.App/Components/Shell/AppTopBarSlot.razor`
- `tests/AiEng.Platform.ComponentTests/Shell/AppTopBarSlotTests.cs`

## Commands Run

- `npm run css:build` — exit 0.
- `dotnet build AiEng.Platform.slnx`
  — 0 warnings, 0 errors (with
  `TreatWarningsAsErrors=true`).
- `dotnet test AiEng.Platform.slnx --no-build`
  — 146 passed, 0 failed
  (component); 4 passed, 4
  skipped, 0 failed
  (architecture).
- `dotnet format --verify-no-changes`
  — clean (after
  `unix2dos` to convert the
  newly created files to
  CRLF).
- `dotnet run --project src/AiEng.Platform.App --launch-profile http`
  + 4 `curl` — all 4 routes
  (`/`, `/counter`, `/weather`,
  `/design-system`) return 200;
  the top bar renders the
  current route's title; the
  theme toggle flips light/dark
  and persists; the breadcrumb
  walks the parent chain on the
  `DesignSystem` page.

## Test Status

- **Last build:** pass (0
  warnings, 0 errors with
  `TreatWarningsAsErrors=true`).
- **Last test run:** 146
  passed, 0 failed (component);
  4 passed, 4 skipped, 0 failed
  (architecture).
- **Last format check:** clean
  (`dotnet format
  --verify-no-changes`).
- **Last visual smoke test:**
  4/4 routes 200; the top bar
  renders the current route's
  title; the theme toggle flips
  light/dark and persists; the
  breadcrumb walks the parent
  chain on the `DesignSystem`
  page.
- **Open regressions:** none.

## Deviations From the Approved Plan

Two deviations from the approved
M2.3 plan are documented in the
implementation report and in the
state files. They are recorded here
verbatim per the user's `Resume`
instruction to "Report any
deviations from the approved M2.3
plan":

1. **`AppTopBar` uses
   `div.app-topbar` + `Leading` /
   `Trailing` slots rather than
   `AppStack` + `AppPageHeader`.**
   The plan § 8 step 4 wording was
   ambiguous: "Two `AppStack`
   slots (`Leading` and `Trailing`)
   wrap `AppPageHeader` for the
   title (Leading) and the theme
   toggle + user avatar
   (Trailing)". The implementation
   uses a single `div.app-topbar`
   container with `Leading` and
   `Trailing` `RenderFragment`
   slots; the `Leading` slot
   composes `AppPageHeader` with
   the current route's title. The
   surface still composes
   `AppTopBar` + `AppPageHeader`
   + `AppBreadcrumb`, matching the
   plan's intent.
2. **Optional architecture test
   `Breadcrumb_Follows_Registry_Parent_Chain`
   was skipped.** The plan § 8
   step 11 marked this test
   optional ("only added if it is
   small and does not extend the
   session beyond the budget").
   The session is at budget; the
   test is skipped per the plan's
   own opt-out.

## Unresolved Issues

None. The M2.3 slice is closed.
The only forward-looking items
are the next session's M2.4
implementation (project
intelligence dashboard, plan
`Awaiting Approval`), the M2.5
plan promotion (empty routes,
responsive, accessibility,
summary entry in the task
board), and the cosmetic M1
follow-up (`AppToolbar` example
on `/design-system`).

## Exact Next Step

> **Approve the M2.4 plan and
> start M2.4 implementation per
> the plan's own order.** Read
> `.ai/plans/M2.4-project-intelligence-dashboard.md`
> end-to-end. Branch off the M2.3
> closeout commit:
> `git checkout -b feature/m2-4-project-intelligence-dashboard feature/m2-3-topbar-breadcrumbs`.
> Follow the 13-step lifecycle in
> `.ai/workflows/progressive-coding.md`:
> 1. Land
>    `IProjectIntelligenceReader`
>    in
>    `src/AiEng.Platform.Application/ProjectIntelligence/`
>    (+ `ProjectIntelligenceSnapshot`
>    record + `ProjectIntelligenceReader`
>    implementation).
> 2. Land the
>    `ProjectIntelligenceServiceCollectionExtensions`
>    composition-root extension
>    in
>    `src/AiEng.Platform.App/Composition/`.
> 3. Wire the new
>    `[RouteMetadata]` for
>    `/dashboard` into the M2.2
>    registry.
> 4. Land the `/dashboard`
>    page at
>    `src/AiEng.Platform.App/Components/Pages/Dashboard.razor`
>    (M0.5-data sections in
>    **Populated**; M3+-data
>    sections in **Empty**).
> 5. Add 10–16 bUnit tests
>    (`ProjectIntelligenceReader`
>    unit tests, `Dashboard`
>    bUnit tests, the
>    `ProjectIntelligenceServiceCollectionExtensions`
>    DI test).
> 6. Add the
>    `Pages_Resolve_State_Through_Reader`
>    architecture test (active
>    and green at M2.4 closeout).
> 7. Validate (css:build,
>    build, test, format, visual
>    smoke + theme toggle).
> 8. Update the
>    project-continuity state.
> 9. Promote the M2.5 plan
>    stub to a full plan in
>    `Awaiting Approval`.
> 10. Write
>     `implementation-report-m2-4-project-intelligence-dashboard.md`
>     and the per-session
>     handoff.
> 11. Single coherent commit
>     `feat(m2.4): add project intelligence dashboard`
>     on
>     `feature/m2-4-project-intelligence-dashboard`.
> 12. Stop. Do NOT begin M2.5.

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
7. `.ai/plans/M2.4-project-intelligence-dashboard.md`
   — the M2.4 plan to approve
   and implement.
8. `implementation-report-m2-3-topbar-breadcrumbs.md`
   — the M2.3 implementation
   report (the receipt for the
   preceding slice).
9. `implementation-report-m2-2-navigation-registry-sidebar.md`
   — the M2.2 implementation
   report (the `INavigationRegistry`
   the M2.4 dashboard
   composes against).
10. `implementation-report-m2-1-application-shell-foundation.md`
    — the M2.1 implementation
    report (the substrate M2.4
    composes against).
11. `ROADMAP.md` — the M2
    section, the matrix in § 4,
    and the M2.4 DoD entry.
12. `docs/design-system.md` —
    the design-system catalogue
    (M2.4 dashboard uses
    `AppPageHeader` and
    `AppEmptyState`).
13. The M2.4 plan's linked
    artefact list
    (`.ai/plans/M2.4-project-intelligence-dashboard.md`
    § 7 Approval).
14. `docs/dashboard.md` —
    the M0.5 contract the
    M2.4 dashboard satisfies.
15. `docs/ui-principles.md`
    § 2.1 — the layout root
    rule (M2.4 follows the M2.1
    composition; no new layout
    root).

## Linked Artefacts

- `.ai/plans/M2.3-topbar-breadcrumbs.md`
  — the approved plan this
  handoff closes.
- `.ai/plans/M2.4-project-intelligence-dashboard.md`
  — the next plan (Awaiting
  Approval).
- `task-brief.md` — none
  produced for M2.3.
- `implementation-plan.md` —
  the M2.3 plan serves as the
  plan; no separate
  implementation-plan.md was
  produced.
- `PRODUCT.md` — the product
  definition; the M2.3 plan's
  "Why it matters" cell cites
  § 1 (vision) and § 2
  (capabilities).
- Current commit hash:
  `feat(m2.3): add top bar, breadcrumb, and page header integration`
  on
  `feature/m2-3-topbar-breadcrumbs`.
- Current branch:
  `feature/m2-3-topbar-breadcrumbs`.
- In-flight worktree or stash:
  none.
- `.ai/state/current.md` and
  `.ai/state/task-board.md` —
  the project-continuity state
  (Rule 15 in `AGENTS.md`).
- The implementation report
  that pairs with this
  handoff:
  `implementation-report-m2-3-topbar-breadcrumbs.md`.
- Prior handoffs:
  `.ai/handoffs/2026-07-11-m2-2-navigation-registry-sidebar.md`,
  `.ai/handoffs/2026-07-11-m2-1-application-shell-foundation.md`,
  `.ai/handoffs/2026-07-10-m1-closeout.md`,
  `.ai/handoffs/2026-07-10-m0.5-architecture-refinement.md`.
