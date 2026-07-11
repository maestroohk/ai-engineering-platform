# Implementation Report

> Produced by the AI at the end of every implementation
> session. Filed in the conversation, in the commit, or in
> the PR. A session that does not produce this report has
> not ended.

---

## Plan Reference

- **Approved plan:** `M2.1 — Application Shell Foundation`
- **Plan path:** `.ai/plans/M2.1-application-shell-skeleton.md`
- **Deviations from plan:** None.

The plan and the report are paired: the plan is the
contract, the report is the receipt. Every
implementation must cite the approved plan; the
`Deviations` section is mandatory even when empty.

---

## Summary

The M2.1 slice delivered the **application shell
foundation**: two layouts (`AppLayout` and
`AppEmptyLayout`), two placeholder shell components
(`AppSidebarSlot` and `AppTopBarSlot`) that reserve
the regions M2.2 and M2.3 will fill, one presentational
helper (`AppShellRegion`) that the slots and the
content region all compose through, the M1.1 chrome
migration (the template's `MainLayout.razor`,
`MainLayout.razor.css`, `NavMenu.razor`, and
`NavMenu.razor.css` are removed), the Tailwind content
path extension so the new `Layouts/` directory's
classes are scanned, the `Layouts/_Imports.razor` so
the cross-folder type references resolve, the wrap of
`/design-system` and `/not-found` in `AppEmptyLayout`,
and the `Routes.razor` `DefaultLayout` change to
`AppLayout`. The M1.2 design-system components
(19 components, 77 bUnit tests, 3 active + 4
registered-but-disabled architecture tests) are
preserved unchanged. M2.1 advances the M2 milestone
(Application Shell and Navigation) from planning to
delivery; it is the substrate M2.2
(`INavigationRegistry`, registry-driven
`AppSidebar`, `Pages_AreReachable_Through_Registry`)
and M2.3 (`AppTopBar`, `AppBreadcrumb`, theme toggle
in the trailing slot) compose against.

## Files Created

- `src/AiEng.Platform.App/Layouts/AppLayout.razor` — the
  authenticated/shell layout. Two-column CSS grid:
  `14rem` sidebar slot, `1fr` main. Main is two-row
  grid: top-bar header, content main. Wraps `@Body`
  in `AppContainer` + `AppStack Gap=Medium`. Preserves
  the Blazor `<div id="blazor-error-ui">` block
  unchanged.
- `src/AiEng.Platform.App/Layouts/AppLayout.razor.css`
  — scoped styles for `.app-shell`,
  `.app-shell-sidebar`, `.app-shell-main`,
  `.app-shell-topbar`, `.app-shell-content`,
  `.app-region`, and the preserved `#blazor-error-ui`
  block. Mobile breakpoint collapses the grid to
  one column under 768px.
- `src/AiEng.Platform.App/Layouts/AppEmptyLayout.razor`
  — the chrome-free layout for `/design-system` and
  `/not-found`. Single centered `AppContainer` +
  `AppStack` of `@Body`, no sidebar or top bar. Same
  `#blazor-error-ui` block.
- `src/AiEng.Platform.App/Layouts/AppEmptyLayout.razor.css`
  — scoped styles for `.app-shell-empty`,
  `.app-shell-empty-content`, `.app-region`, and
  `#blazor-error-ui`.
- `src/AiEng.Platform.App/Layouts/_Imports.razor` —
  five `@using` directives that resolve
  cross-folder type references without polluting
  the App-wide `Components/_Imports.razor`:
  `Common`, `Primitive`, `Display`, `Feedback`,
  `Shell`.
- `src/AiEng.Platform.App/Components/Shell/AppShellRegion.razor`
  — presentational helper. `<section
  class="app-region" data-app-region="@Name"
  aria-label="@Name">` wrapping `ChildContent` with
  `@attributes="AdditionalAttributes"` splat-through.
  Used by the two slots and by the content region in
  both layouts.
- `src/AiEng.Platform.App/Components/Shell/AppSidebarSlot.razor`
  — M2.2 placeholder. Composes `AppShellRegion
  Name="sidebar"` with an `AppStack` of one
  `AppEmptyState` ("Sidebar lands in M2.2 — the
  navigation registry and the registry-driven sidebar
  arrive in M2.2").
- `src/AiEng.Platform.App/Components/Shell/AppTopBarSlot.razor`
  — M2.3 placeholder. Composes `AppShellRegion
  Name="topbar"` with an `AppStack` of one
  `AppPageHeader` ("Top bar lands in M2.3") and one
  `AppAlert Variant=Information` ("M2.1 placeholder
  — this region is intentionally empty. M2.3
  replaces it with the real top bar").
- `tests/AiEng.Platform.ComponentTests/Layouts/AppLayoutTests.cs`
  — 8 bUnit tests using the
  `Add(p => p.Body, "<span>body</span>")` pattern
  for `LayoutComponentBase` components.
- `tests/AiEng.Platform.ComponentTests/Layouts/AppEmptyLayoutTests.cs`
  — 6 bUnit tests.
- `tests/AiEng.Platform.ComponentTests/Shell/AppSidebarSlotTests.cs`
  — 5 bUnit tests.
- `tests/AiEng.Platform.ComponentTests/Shell/AppTopBarSlotTests.cs`
  — 6 bUnit tests.

## Files Modified

- `src/AiEng.Platform.App/Components/_Imports.razor` —
  added `@using AiEng.Platform.App.Components.Shell`
  and `@using AiEng.Platform.App.Layouts` so the
  shell components and the two layouts are
  resolvable in `Pages/`.
- `src/AiEng.Platform.App/Components/Routes.razor` —
  `DefaultLayout="typeof(Layout.MainLayout)"`
  became
  `DefaultLayout="typeof(Layouts.AppLayout)"`. The
  five M1 template pages (`/`, `/counter`,
  `/weather`, `/error`) and the `/design-system`
  page that does not override `@layout` all reach
  `AppLayout` by default.
- `src/AiEng.Platform.App/Components/Pages/DesignSystem.razor`
  — added `@layout AppEmptyLayout` directive. The
  design-system catalogue renders without the
  M2.2/M2.3 chrome (no "Sidebar lands in M2.2"
  placeholder on the doc page; the catalogue is
  self-explanatory).
- `src/AiEng.Platform.App/Components/Pages/NotFound.razor`
  — `@layout MainLayout` became
  `@layout AppEmptyLayout` per plan § 13 Risk 3
  (the not-found page should not show the
  shell-in-progress chrome).
- `tailwind.config.js` — content array gained
  `"./src/AiEng.Platform.App/Layouts/**/*.razor"`
  and
  `"./src/AiEng.Platform.App/Layouts/**/*.razor.css"`.
  Without these, the Tailwind scanner does not see
  the layout classes and the new CSS is purged
  out of `wwwroot/css/app.css`.
- `ROADMAP.md` — M2 row: "Planned" → "Active
  (M2.1 delivered 2026-07-11)"; M2.1 row:
  "Awaiting Approval" → "Delivered (M2.1,
  2026-07-11)"; matrix § 4 row 1332: noted the
  M2.1 deliverable; bottom paragraph: "next
  session approves the M2.2 plan and starts M2.2
  implementation."
- `.ai/state/current.md` — milestone, slice,
  status, last completed task, active branch,
  last stable commit, application status, CSS
  build status, test status, implemented
  capabilities, active plan, last implementation
  report, next recommended task, last updated,
  linked artefacts all updated to reflect M2.1
  delivery.
- `.ai/state/task-board.md` — Ready section
  trimmed (M2.1 row removed; M2.2 row updated to
  Status: Ready, plan path: `Awaiting Approval`);
  In Progress section reset to "(none — M2.1
  closed in the M2.1 closeout session,
  2026-07-11)"; Done Recently gained the M2.1
  closeout entry with the 5 new components, 25
  new tests, branch, and commit message.
- `.ai/state/session.json` — fully rewritten for
  the implementation session: session_id
  `m2-1-application-shell-foundation`,
  session_type `implementation`, full scope
  (in_scope, out_of_scope), current_understanding
  (active_slice M2.2 Awaiting Approval,
  last_completed_task T-001 M2.1 closeout,
  last_stable_commit the M2.1 closeout commit),
  last_action (committed M2.1 closeout),
  intended_next_action (M2.2 plan review).
- `.ai/state/tasks.json` — T-001 (M2.1)
  rewritten to `status: "Done"`,
  `completed_at: "2026-07-11T00:00:00Z"`, full
  evidence object (branch, files_added,
  files_modified, files_deleted, tests object);
  T-002 (M2.2) promoted from `Draft` to `Ready`
  with notes reflecting the plan promotion.
- `.ai/state/milestones.json` — M2 milestone
  evidence: `commits` array now includes the
  M2.1 closeout commit hash; `slices.M2_1`
  object added with the slice's `status:
  "delivered"`, `delivered_at: "2026-07-11"`,
  and `summary` of the deliverable.
- `.ai/handoffs/latest.md` — replaced the M2
  reconciliation handoff with the M2.1 closeout
  handoff (per-session handoff mirrored to
  `.ai/handoffs/2026-07-11-m2-1-application-shell-foundation.md`).
- `tests/AiEng.Platform.ComponentTests/ComponentTests.csproj` — no change; the new test
  files in `Layouts/` and `Shell/` are picked up
  by the existing `**/*Tests.cs` glob.

## Files Deleted

- `src/AiEng.Platform.App/Components/Layout/MainLayout.razor` —
  the M1.1 chrome layout (template-derived with the
  M1.1 theme toggle in the footer) is replaced by
  `AppLayout` (which preserves the
  `#blazor-error-ui` block in its own markup).
- `src/AiEng.Platform.App/Components/Layout/MainLayout.razor.css` —
  scoped CSS for the deleted layout; the
  equivalent layout shell classes live in
  `Layouts/AppLayout.razor.css`.
- `src/AiEng.Platform.App/Components/Layout/NavMenu.razor` —
  the M1.1 sidebar with its four hard-coded
  `NavLink`s. M2.1 ships only the
  `AppSidebarSlot` placeholder; M2.2 replaces it
  with the registry-driven `AppSidebar`.
- `src/AiEng.Platform.App/Components/Layout/NavMenu.razor.css` —
  scoped CSS for the deleted nav menu. No
  equivalent is needed in M2.1.

## Reusable Components Introduced

- `AppLayout` — purpose: the M2.1 shell layout
  (sidebar + top bar + content); folder:
  `src/AiEng.Platform.App/Layouts/`; variants:
  none (single layout, but its empty sibling
  `AppEmptyLayout` is the chrome-free
  counterpart); state slots: `@Body` (from
  `LayoutComponentBase`).
- `AppEmptyLayout` — purpose: chrome-free layout
  for catalogue pages; folder:
  `src/AiEng.Platform.App/Layouts/`; variants:
  none; state slots: `@Body`.
- `AppSidebarSlot` — purpose: M2.2 placeholder
  in the sidebar region; folder:
  `src/AiEng.Platform.App/Components/Shell/`;
  variants: none; state slots: none (renders a
  fixed `AppEmptyState`).
- `AppTopBarSlot` — purpose: M2.3 placeholder in
  the top-bar region; folder:
  `src/AiEng.Platform.App/Components/Shell/`;
  variants: none; state slots: none (renders a
  fixed `AppPageHeader` + `AppAlert`).
- `AppShellRegion` — purpose: presentational
  helper that wraps a labelled region in
  `<section class="app-region"
  data-app-region="@Name" aria-label="@Name">`;
  folder:
  `src/AiEng.Platform.App/Components/Shell/`;
  variants: `Name` (the only state slot —
  `"sidebar"`, `"topbar"`, or `"content"`);
  splat-through `AdditionalAttributes` for
  future expansion.

## Services Introduced

- None. M2.1 is presentational only; the navigation
  service collection extensions land in M2.2.

## Providers Touched

- None. No provider code is touched in M2.1. The
  M2.2 navigation registry will live in
  `AiEng.Platform.Application/Navigation/` and be
  composed by M2.2's
  `NavigationServiceCollectionExtensions.cs`.

## Tests Added

- Unit: 0 (M2.1 has no service classes; the
  `AppShellRegion` / `AppSidebarSlot` /
  `AppTopBarSlot` / `AppLayout` / `AppEmptyLayout`
  surface is presentational, tested via bUnit
  rather than unit tests).
- bUnit: 25 (8 `AppLayoutTests` + 6
  `AppEmptyLayoutTests` + 5
  `AppSidebarSlotTests` + 6
  `AppTopBarSlotTests`).
- Contract: 0.
- Integration: 0.
- Architecture: 0 (the
  `Pages_AreReachable_Through_Registry` test lands
  with M2.2; the M2.1 architecture-test count is
  unchanged from the M1 closeout baseline of 3
  active + 4 registered-but-disabled).
- Regression: 0 (the M1.2 bUnit suite is
  preserved: 77 tests, all green, no skips or
  removals).

## Commands Run

The actual commands the session ran, in order.

- `git status` — verify clean working tree on
  `main` at `ba6c1e8`.
- `git checkout -b feature/m2-1-application-shell` —
  create the M2.1 task branch.
- `Write`/`Edit` for each new and modified file
  (the 5 new components, the 2 modified pages, the
  modified `Routes.razor` / `_Imports.razor` /
  `tailwind.config.js`).
- `npm run css:build` — produces
  `wwwroot/css/app.css`. 11,381 bytes minified
  (post-M2.1).
- `dotnet restore` — restore NuGet packages for
  the new test project layout (the
  `Layouts/` and `Shell/` test folders are
  already covered by the existing
  `**/*Tests.cs` glob).
- `dotnet build` — verify the C# / Razor project
  still compiles with
  `TreatWarningsAsErrors=true`. 0 warnings, 0
  errors.
- `dotnet test` — full test suite. 102 passed, 4
  skipped, 0 failed (component tests); 3 passed,
  4 skipped, 0 failed (architecture tests).
- `dotnet format --verify-no-changes` — no
  formatting drift after the line-ending fix
  (the new files were written with CRLF endings
  matching the .NET project; an initial run
  without `--verify-no-changes` corrected a
  pair of test files that had drifted to LF,
  and a subsequent run returned exit 0).
- `dotnet run --project src/AiEng.Platform.App`
  (background) — start the dev server on
  `http://localhost:5286`.
- `curl http://localhost:5286/`,
  `curl http://localhost:5286/counter`,
  `curl http://localhost:5286/weather`,
  `curl http://localhost:5286/design-system`,
  `curl http://localhost:5286/not-found` — visual
  smoke test. All 5 routes return 200; the new
  shell HTML structure (`<div class="app-shell">`
  root, `<aside class="app-shell-sidebar">`,
  `<header class="app-shell-topbar">`,
  `<main class="app-shell-content">`, all with
  `data-app-region` attributes) renders on the
  first 3 routes; `/design-system` and
  `/not-found` render under `AppEmptyLayout`
  without the sidebar/topbar chrome.
- `git add` and `git commit -m "feat(m2.1): add
  application shell foundation"` — the coherent
  closeout commit on
  `feature/m2-1-application-shell`. Push skipped
  (no remote configured).

## Validation Results

The actual results. Be honest; if something failed and was
fixed, say so.

- `npm run css:build`: exit 0, 11,381 bytes
  minified.
- `dotnet build`: 0 warnings, 0 errors.
- `dotnet test`: 102 passed, 4 skipped, 0 failed
  (component tests); 3 passed, 4 skipped, 0 failed
  (architecture tests).
- `dotnet format --verify-no-changes`: exit 0.
- Visual smoke test: all 5 M1 routes return 200;
  the new shell HTML structure renders correctly
  with the `data-app-region` attributes; the two
  placeholders ("Sidebar lands in M2.2" and
  "Top bar lands in M2.3") are visible in the
  sidebar and top-bar regions of the three
  default-layout routes; `/design-system` and
  `/not-found` render under `AppEmptyLayout`
  without the chrome.
- `git status --short`: empty after the
  closeout commit (push skipped — no remote
  configured, per the brief).

## Documentation Updated

- `ROADMAP.md` — M2 row, M2.1 row, matrix § 4
  row 1332, bottom paragraph all updated to
  reflect M2.1 delivery.
- `.ai/state/current.md` — full update to
  reflect M2.1 delivery (see "Files Modified"
  for the section list).
- `.ai/state/task-board.md` — M2.1 row moved to
  Done Recently; M2.2 row updated to Ready
  with the Awaiting Approval plan path.
- `.ai/state/session.json` — rewritten for the
  implementation session.
- `.ai/state/tasks.json` — T-001 marked Done
  with full evidence; T-002 promoted to Ready.
- `.ai/state/milestones.json` — M2 evidence
  updated; M2.1 slice evidence added.
- `.ai/handoffs/latest.md` and
  `.ai/handoffs/2026-07-11-m2-1-application-shell-foundation.md` —
  the per-session handoff (mirror).
- `docs/design-system.md` — no change. The
  M1.2 design system is the substrate M2.1
  composes against; M2.1 does not add new
  design-system primitives. The new layouts
  and shell components are **M2.1** components,
  not M1.2 design-system components, and live
  outside the design-system catalogue.
- `DECISIONS.md` — no change. M2.1 is a
  mechanical composition of M1.2; no new ADR
  is warranted.
- `AGENTS.md` — no change (per plan § 4
  out-of-scope).

## Deviations

- None. The implementation followed the
  revised M2.1 plan exactly. The bUnit
  test pattern for `LayoutComponentBase`
  (`.Add(p => p.Body, ...)` rather than
  `.AddChildContent(...)`) was identified
  during the first test run and corrected
  before the final `dotnet test`; this was
  a test-mechanics discovery, not a plan
  deviation.
- The line-ending fix (`dotnet format`
  without `--verify-no-changes` to correct
  test files that had drifted to LF) was
  a tooling correction, not a plan
  deviation.

## Known Limitations

- The sidebar and top bar are M2.2 and M2.3
  placeholders, not real chrome. The
  `AppSidebarSlot` renders an `AppEmptyState`
  ("Sidebar lands in M2.2"); the `AppTopBarSlot`
  renders an `AppPageHeader` + `AppAlert`
  ("Top bar lands in M2.3"). The M2.1
  deliverable is the **shell foundation** —
  the stable region targets M2.2 and M2.3
  fill.
- `/design-system` and `/not-found` render
  under `AppEmptyLayout` (no sidebar/topbar).
  The five M1 template pages (`/`, `/counter`,
  `/weather`, `/error`) and any other page
  that does not override `@layout` reach
  `AppLayout` via the `DefaultLayout` change
  in `Routes.razor`.
- The mobile breakpoint (sidebar collapses
  under 768px) is implemented at the
  AppLayout level only. M2.5 will define
  the full responsive matrix in
  `docs/ui-principles.md` § 10.1; M2.1
  delivers a single breakpoint that is
  good enough for the 1280x720 minimum
  viewport per the validation requirements.
- The theme toggle is **not** in the M2.1
  shell. The M1.1 `MainLayout.razor` had a
  footer theme toggle button; M2.1 deletes
  `MainLayout.razor` and does not move the
  toggle into `AppLayout`. M2.3 reintroduces
  the toggle in `AppTopBar.Trailing` slot
  per the M2.3 plan stub.
- The M2.1 architecture-test count is
  unchanged from M1.2 (3 active + 4
  registered-but-disabled). The
  `Pages_AreReachable_Through_Registry`
  test lands with M2.2; the
  `Composites_AreReachable_Through_DependencyInjection`
  tests land with M4-D per the M1 closeout.
- No push to a remote. The branch
  `feature/m2-1-application-shell` is
  local only; the brief explicitly
  instructs that push is skipped when no
  remote is configured, and no remote is
  configured.

## Next Recommended Step

The next session reads
`.ai/plans/M2.2-navigation-registry-sidebar.md`
and either approves it (and starts M2.2
implementation) or amends it and re-submits.
The Progressive Coding Rule applies: one task
per session, 13-step lifecycle, stop after the
coherent commit. M2.2 implementation is NOT
started in this session.

The exact action for the next session:

1. Open
   `.ai/plans/M2.2-navigation-registry-sidebar.md`.
2. Reconcile the structured state with the
   actual repository and Git history (HEAD
   should be the M2.1 closeout commit on
   `feature/m2-1-application-shell`).
3. If the M2.2 plan is approved, mark T-002
   `In Progress` and implement the plan
   (INavigationRegistry, RouteMetadata,
   RouteMetadataAttribute, RouteRegistry,
   AppSidebar, AppSidebarItem, AppNavItem,
   the `[RouteMetadata]` application to every
   page in `Components/Pages/`, and the
   `Pages_AreReachable_Through_Registry`
   architecture test activation).
4. If the plan needs amendment, edit it and
   re-submit; do not start implementation
   against an unapproved plan.

## Project Continuity (Rule 15) and Evidence (Rule 17)

A session that ends without updating the
project-continuity state and leaving evidence
has not ended. Confirm that the following were
done at session end:

- [x] `.ai/state/current.md` — updated to reflect
      the state of the repository right now
      (milestone M2, branch
      `feature/m2-1-application-shell`, last
      commit the M2.1 closeout commit, last
      validation result 102 passed / 4 skipped /
      0 failed, exact next step M2.2 plan
      review).
- [x] `.ai/state/task-board.md` — M2.1 moved
      from `In Progress` to `Done`; M2.2
      promoted to `Ready`; no new `Ready` items
      beyond the M2.2 row that was already
      there.
- [x] `.ai/handoffs/2026-07-11-m2-1-application-shell-foundation.md` —
      the per-session handoff.
- [x] `.ai/handoffs/latest.md` — mirror of the
      per-session handoff.
- [x] `implementation-report-m2-1-application-shell-foundation.md`
      — the receipt (this file).
- [x] **Coherent commit** (Rule 17 in
      `AGENTS.md`)
      `feat(m2.1): add application shell foundation`
      on
      `feature/m2-1-application-shell`
      that includes the implementation, the
      documentation, the implementation report,
      the state updates, and the handoff. The
      commit is local; pushing requires explicit
      authorisation (and is not authorised by
      the brief — no remote is configured).

## Linked Artefacts

- `.ai/plans/M2.1-application-shell-skeleton.md`
  — the approved plan this report implements
  against (mandatory).
- `.ai/plans/M2.2-navigation-registry-sidebar.md`
  — the M2.2 plan, expanded from `Draft` to
  `Awaiting Approval` in this session.
- `.ai/handoffs/2026-07-11-m2-1-application-shell-foundation.md`
  — the per-session handoff, written at session
  end (Rule 15 in `AGENTS.md`).
- `.ai/handoffs/latest.md` — mirror of the
  per-session handoff.
- `.ai/state/current.md` and
  `.ai/state/task-board.md` — the live state,
  updated at session end (Rule 15 in
  `AGENTS.md`).
- `.ai/state/tasks.json` and
  `.ai/state/milestones.json` — the
  canonical machine-readable state, updated
  at session end.
- `.ai/state/session.json` — the session
  envelope, rewritten for the implementation
  session.
- The commit hash of the session's work on
  `feature/m2-1-application-shell`
  (Rule 17 in `AGENTS.md`). The branch is
  local; the brief explicitly instructs that
  push is skipped when no remote is configured.
