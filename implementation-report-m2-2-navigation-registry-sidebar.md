# Implementation Report — M2.2 — Navigation Registry and Sidebar

> **Closing receipt for M2.2 — Navigation
> Registry and Sidebar.** The M2.2 slice
> delivers the navigation registry and the
> registry-driven sidebar. The slice ends
> when the sidebar reads from
> `INavigationRegistry`, every page in
> `Components/Pages/` has a `[RouteMetadata]`
> attribute whose `Href` matches a real route,
> and the
> `Pages_AreReachable_Through_Registry`
> architecture test is active and green.
> **All three end-of-slice conditions are
> satisfied.**

---

## Plan Reference

- **Approved plan:** `M2.2 — Navigation
  Registry and Sidebar`
- **Plan path:** `.ai/plans/M2.2-navigation-registry-sidebar.md`
- **Deviations from plan:** None.
  The implementation followed the
  plan's 24-step order exactly.

The plan and the report are paired: the
plan is the contract, the report is the
receipt. Every implementation cites the
approved plan; the `Deviations` section
is mandatory even when empty.

---

## Summary

M2.2 is the second slice of milestone M2
(Application Shell and Navigation). It
introduces the **navigation registry** — a
typed data model that describes every
navigable route in the application — and
the **registry-driven sidebar** that
replaces M2.1's `AppSidebarSlot`
placeholder. The registry is the data
model the sidebar (M2.2), the breadcrumb
(M2.3), and the M3+ pages compose
against.

The slice ships:

- The `INavigationRegistry` interface
  and the `RouteMetadata` record in
  `src/AiEng.Platform.Application/Navigation/`.
- The `[RouteMetadata]` attribute
  applied to every page in
  `src/AiEng.Platform.App/Components/Pages/`.
- The `RouteRegistry` implementation
  that scans the App assembly at
  startup, handles
  `ReflectionTypeLoadException`,
  produces a sorted, parent-aware
  list, and provides `FindByHref` /
  `ChildrenOf` lookups.
- The composition root
  (`AddPlatformServices` +
  `AddNavigation`) in
  `src/AiEng.Platform.App/Composition/`,
  wired into `Program.cs`.
- `AppSidebar`, `AppSidebarItem`, and
  `AppNavItem` in
  `src/AiEng.Platform.App/Components/Navigation/`,
  composing only the M1.2 design
  system primitives.
- The
  `Pages_AreReachable_Through_Registry`
  architecture test (1 new, active
  and green).
- 28 new bUnit / integration tests
  across 5 new test files; the M1.2
  (77 tests) and M2.1 (25 tests)
  component tests are preserved.

The M2.1 `AppSidebarSlot` placeholder
and its 5 bUnit tests are deleted; the
M2.1 `AppTopBarSlot` placeholder and
its 6 bUnit tests remain (M2.3 ships
the real `AppTopBar`).

The M2.1 shell foundation
(`AppLayout`, `AppEmptyLayout`,
`AppShellRegion`) is the substrate M2.2
composes against. The M3+ sessions
consume the registry to enumerate
navigable routes without re-scanning
the assembly.

---

## Files Created

- `src/AiEng.Platform.Application/Navigation/INavigationRegistry.cs`
  — the registry contract (Routes,
  FindByHref, ChildrenOf).
- `src/AiEng.Platform.Application/Navigation/RouteMetadata.cs`
  — the data record (Href, Title,
  Order, Description, Icon, Parent,
  BadgeText, ShowInSidebar,
  MatchPrefix).
- `src/AiEng.Platform.Application/Navigation/RouteMetadataAttribute.cs`
  — the page-level annotation
  (`AttributeUsage = Class`,
  `AllowMultiple = false`,
  `Inherited = false`); provides a
  `ToMetadata()` projection.
- `src/AiEng.Platform.Application/Navigation/RouteRegistry.cs`
  — the assembly-scan implementation
  that produces a sorted, parent-aware
  list. Enumerates types with
  `[RouteMetadata]`, calls
  `ToMetadata()`, handles
  `ReflectionTypeLoadException` via
  `SafeGetTypes`, builds a
  case-insensitive dictionary for
  `FindByHref` / `ChildrenOf`.
- `src/AiEng.Platform.App/Composition/ServiceCollectionExtensions.cs`
  — the composition root
  (`AddPlatformServices(params
  Assembly[])`) that calls
  `AddNavigation`.
- `src/AiEng.Platform.App/Composition/NavigationServiceCollectionExtensions.cs`
  — the `AddNavigation(params
  Assembly[])` extension that
  registers `INavigationRegistry` as
  a singleton.
- `src/AiEng.Platform.App/Components/Navigation/_Imports.razor`
  — resolves the design-system
  primitive types and the
  Application.Navigation namespace
  in the new folder.
- `src/AiEng.Platform.App/Components/Navigation/AppSidebar.razor`
  (+ `.razor.css`) — the
  registry-driven sidebar; injects
  `INavigationRegistry`, filters by
  `ShowInSidebar`, renders one
  `AppNavItem` per visible route
  inside `AppShellRegion
  Name="sidebar"`.
- `src/AiEng.Platform.App/Components/Navigation/AppSidebarItem.razor`
  (+ `.razor.css`) — the sidebar
  section group with title heading
  and `AppStack Gap=Small`.
- `src/AiEng.Platform.App/Components/Navigation/AppNavItem.razor`
  (+ `.razor.css`) — renders a
  `NavLink` with the design-system
  ghost-button styling, supports
  `Match=All` / `Prefix` via
  `Route.MatchPrefix`, sets
  `aria-current="page"` on the active
  link, renders an `AppBadge` if
  `Route.BadgeText` is non-null.
- `tests/AiEng.Platform.ComponentTests/Navigation/RouteRegistryTests.cs`
  — 6 tests (assembly scan,
  ordering, FindByHref case
  insensitivity, ChildrenOf
  filtering, reflection-load
  resilience).
- `tests/AiEng.Platform.ComponentTests/Navigation/AppNavItemTests.cs`
  — 8 tests (NavLink rendering,
  Match=All/Prefix, aria-current,
  AriaLabel default + override,
  badge rendering).
- `tests/AiEng.Platform.ComponentTests/Navigation/AppSidebarItemTests.cs`
  — 4 tests (title heading,
  ChildContent wrapping, aria-label,
  AppStack usage).
- `tests/AiEng.Platform.ComponentTests/Navigation/AppSidebarTests.cs`
  — 6 tests (one `AppNavItem` per
  non-hidden route, aria-current on
  active route, region attributes,
  empty registry, registry-driven
  not hardcoded, region name).
- `tests/AiEng.Platform.ComponentTests/Navigation/NavigationServiceCollectionExtensionsTests.cs`
  — 3 tests (DI registration,
  singleton lifetime, assembly
  scan via DI container).
- `tests/AiEng.Platform.ArchitectureTests/PagesAreReachableThroughRegistryTests.cs`
  — 1 test (every
  `Components/Pages/*.razor` has a
  `[RouteMetadata]` attribute whose
  `Href` is in the registry).

---

## Files Modified

- `src/AiEng.Platform.App/Components/_Imports.razor`
  — added
  `@using AiEng.Platform.Application.Navigation`
  and
  `@using AiEng.Platform.App.Components.Navigation`
  so the 6 pages can apply
  `[RouteMetadata]` and so the
  Layouts can render
  `<AppNavItem />` / `<AppSidebar />`.
- `src/AiEng.Platform.App/Layouts/_Imports.razor`
  — added
  `@using AiEng.Platform.App.Components.Navigation`
  and
  `@using AiEng.Platform.Application.Navigation`
  so `AppLayout.razor` can render
  `<AppSidebar />` and resolve
  `INavigationRegistry` injection.
- `src/AiEng.Platform.App/Layouts/AppLayout.razor`
  — replaced `<AppSidebarSlot />`
  with `<AppSidebar />` in the
  `<aside class="app-shell-sidebar">`
  region.
- `src/AiEng.Platform.App/Program.cs`
  — added
  `builder.Services.AddPlatformServices(typeof(Program).Assembly);`
  after `AddRazorComponents()` to
  register `INavigationRegistry` as
  a singleton and trigger the
  assembly scan.
- `src/AiEng.Platform.App/Components/Pages/Home.razor`
  — added
  `@attribute [RouteMetadata("/", "Home", Order = 0)]`.
- `src/AiEng.Platform.App/Components/Pages/Counter.razor`
  — added
  `@attribute [RouteMetadata("/counter", "Counter", Order = 1)]`.
- `src/AiEng.Platform.App/Components/Pages/Weather.razor`
  — added
  `@attribute [RouteMetadata("/weather", "Weather", Order = 2)]`.
- `src/AiEng.Platform.App/Components/Pages/Error.razor`
  — added
  `@attribute [RouteMetadata("/error", "Error", Order = 3, ShowInSidebar = false)]`.
- `src/AiEng.Platform.App/Components/Pages/NotFound.razor`
  — added
  `@attribute [RouteMetadata("/not-found", "Not found", Order = 99, ShowInSidebar = false)]`.
- `src/AiEng.Platform.App/Components/Pages/DesignSystem.razor`
  — added
  `@attribute [RouteMetadata("/design-system", "Design system", Order = 100)]`.
- `tests/AiEng.Platform.ComponentTests/Layouts/AppLayoutTests.cs`
  — registered a stub
  `INavigationRegistry` in the
  `BunitContext` ctor so the 8
  `AppLayoutTests` render
  `<AppSidebar />` without DI
  errors.
- `ROADMAP.md` — updated the M2.2
  row in the slice-breakdown table
  from `Plan Awaiting Approval` to
  `Delivered (M2.2, 2026-07-11)`;
  added the M2.2 closeout note to
  the M2 DoD.

---

## Files Deleted

- `src/AiEng.Platform.App/Components/Shell/AppSidebarSlot.razor`
  — the M2.1 placeholder is
  replaced by the registry-driven
  `AppSidebar` (M2.2).
- `tests/AiEng.Platform.ComponentTests/Shell/AppSidebarSlotTests.cs`
  — the 5 bUnit tests for the
  M2.1 placeholder are replaced by
  the 6 `AppSidebarTests` (M2.2).

---

## Reusable Components Introduced

- `AppSidebar` — purpose:
  registry-driven sidebar; folder:
  `src/AiEng.Platform.App/Components/Navigation/`;
  variants: none; state slots:
  injects `INavigationRegistry` via
  `[Inject]`; renders inside
  `AppShellRegion Name="sidebar"`.
- `AppSidebarItem` — purpose:
  sidebar section group with title
  heading; folder:
  `src/AiEng.Platform.App/Components/Navigation/`;
  variants: none; state slots:
  `[Parameter] string Title`,
  `[Parameter] RenderFragment? ChildContent`.
- `AppNavItem` — purpose: a
  `NavLink` rendered with the
  design-system ghost-button
  styling; folder:
  `src/AiEng.Platform.App/Components/Navigation/`;
  variants: none; state slots:
  `[Parameter] RouteMetadata Route`
  (required),
  `[Parameter] string? AriaLabel`
  (optional override).

---

## Services Introduced

- `INavigationRegistry` — methods:
  `IReadOnlyList<RouteMetadata>
  Routes { get; }`, `RouteMetadata?
  FindByHref(string href)`,
  `IReadOnlyList<RouteMetadata>
  ChildrenOf(string? parentHref)`;
  lifetime: singleton; dependencies:
  none (a pure data registry).
- `RouteRegistry` (concrete
  implementation) — methods:
  constructor `RouteRegistry(params
  Assembly[] scanAssemblies)`;
  same three members as the
  interface. Lifetime: singleton
  (created by `AddNavigation`).
  Dependencies: the assemblies to
  scan.

---

## Providers Touched

- None. M2.2 is provider-free.
  The composition root
  (`Composition/`) exists for the
  future provider wiring; the
  `AddNavigation` extension is the
  only one called today. M4-D
  introduces the first concrete
  providers and the
  `AddProviderComposition()` call.

---

## Tests Added

- Unit: 0
- bUnit: 24 (`AppNavItem` 8 +
  `AppSidebarItem` 4 + `AppSidebar`
  6 + `RouteRegistry` 6)
- Contract: 0
- Integration: 3
  (`NavigationServiceCollectionExtensions`)
- Architecture: 1
  (`Pages_AreReachable_Through_Registry`)
- Regression: 0

Total new tests: 28
(bUnit / integration) + 1
(architecture) = 29.

The M2.1 `AppSidebarSlot` and its 5
bUnit tests are deleted (counted as
-5); the M2.1 `AppTopBarSlot` and
its 6 bUnit tests remain (M2.3 ships
the real `AppTopBar`).

The 8 pre-existing
`AppLayoutTests` are modified to
register a stub
`INavigationRegistry` in the
`BunitContext` ctor; the
modification does not change any
assertion.

---

## Commands Run

The actual commands the session ran,
in order.

- `git checkout -b feature/m2-2-navigation-registry-sidebar`
  — branch off the M2.1 closeout
  chain (the M2.1 closeout commit
  on `feature/m2-1-application-shell`).
- `dotnet build AiEng.Platform.slnx`
  — 0 warnings, 0 errors.
- `dotnet test AiEng.Platform.slnx --no-build`
  — 125 passed, 0 failed
  (component); 4 passed, 4
  skipped, 0 failed (architecture).
- `npm run css:build` — exit 0.
  Minified CSS bundle ~11,500
  bytes; new navigation utility
  classes are in the compiled
  CSS.
- `dotnet format --verify-no-changes`
  — clean (after `unix2dos` to
  convert newly created files to
  CRLF line endings).
- `dotnet run --project src/AiEng.Platform.App --launch-profile http`
  + `curl http://localhost:5286/`
  + `curl http://localhost:5286/counter`
  + `curl http://localhost:5286/weather`
  + `curl http://localhost:5286/design-system`
  + `curl http://localhost:5286/not-found`
  — all 5 routes return 200; the
  `/` HTML contains the 4
  sidebar-visible `href`s
  (`/`, `/counter`, `/weather`,
  `/design-system`); the
  `Error` and `NotFound` routes
  are correctly hidden.
- `git status --short` — see the
  pre-commit evidence in
  `tasks.json` T-002.

---

## Validation Results

- `npm run css:build`: clean
  (exit 0; new navigation
  utility classes are in the
  compiled CSS).
- `dotnet build`: clean
  (0 warnings, 0 errors with
  `TreatWarningsAsErrors=true`).
- `dotnet test`: 125 passed + 0
  failed (component); 4 passed
  + 4 skipped + 0 failed
  (architecture). The 8
  `AppLayoutTests` failures
  encountered during
  implementation (no
  `INavigationRegistry`
  registered in the bUnit
  context) were fixed by
  registering a stub registry
  in the ctor; this is a test
  fix, not a production fix.
- `dotnet format --verify-no-changes`:
  clean (after converting the
  newly created M2.2 files to
  CRLF line endings via
  `unix2dos`; this matches the
  Windows-first deployment
  target and the `editorconfig`
  default).
- `dotnet run` + 5 curl: all
  5 routes return 200; the
  sidebar renders the 4
  expected nav items.
- `git status --short`: see
  `tasks.json` T-002 evidence
  for the full file set.

---

## Documentation Updated

- `ROADMAP.md` — M2.2 row
  updated to
  `Delivered (M2.2, 2026-07-11)`;
  the M2 DoD section now records
  the M2.2 closeout with a link
  to this report.
- `.ai/state/current.md` —
  reconciled to the M2.2
  closeout state (active slice
  flips to M2.3; the Last
  Completed Task section
  records the M2.2 session; the
  Application Status section
  describes the registry-driven
  sidebar; the Implemented
  Capabilities section adds the
  M2.2 components; the Last
  Implementation Report section
  lists this report).
- `.ai/state/task-board.md` —
  T-002 moved to `Done Recently`;
  T-003 promoted to `Ready`
  (M2.3 plan promoted to
  `Awaiting Approval`); the
  In Progress section records
  the empty state.
- `.ai/state/session.json` —
  rewritten to the M2.2
  closeout envelope.
- `.ai/state/tasks.json` —
  T-002 updated to `Done` with
  the M2.2 evidence block
  (branch, files_added,
  files_modified,
  files_deleted, tests); T-003
  updated to `Ready`.
- `.ai/state/milestones.json` —
  M2.2 slice updated to
  `delivered` with the
  delivered_at, session, branch,
  summary, plan,
  implementation_report, and
  commit_message; the M2
  evidence.slices array adds
  M2.2; the M2
  evidence.implementation_reports
  array adds this report; the
  M2.3 slice updated to `plan
  awaiting approval`.
- `.ai/state/capabilities.json` —
  C-019 updated to
  `status: Done`,
  `completion_status: Verified`,
  `next_task: null`,
  `delivered_by_tasks: [T-001,
  T-002]`, `evidence.reports`
  appends this report,
  `evidence.tests` appends the
  5 new test files +
  `Pages_AreReachable_Through_Registry`,
  `evidence.source_paths`
  appends the 4 new
  Application.Navigation files +
  the 2 new Composition files +
  the 3 new
  Components.Navigation files,
  `completed_criteria` lists
  every M2.2 acceptance bullet
  with its evidence.
- `.ai/plans/M2.3-topbar-breadcrumbs.md`
  — promoted from the `Draft`
  stub to a full M2.3 plan in
  `Awaiting Approval` (front
  matter, status, § 7 Approval,
  and a new § 8 Implementation
  Order with the 13-step
  lifecycle).
- `.ai/handoffs/2026-07-11-m2-2-navigation-registry-sidebar.md`
  + mirror to
  `.ai/handoffs/latest.md` —
  the per-session handoff.

No new docs in `docs/`; the M2.2
slice composes the M1.2 design
system and the M2.1 shell
foundation without introducing
new design-system primitives
or new architecture patterns.
AGENTS.md, ARCHITECTURE.md, and
DECISIONS.md are unchanged.

---

## Deviations

None. The implementation followed
the M2.2 plan's 24-step order
exactly. Two minor notes that are
not deviations:

- The plan implied the
  `Composition/` folder already
  existed; it did not. M2.2
  created both
  `ServiceCollectionExtensions.cs`
  (the composition root) and
  `NavigationServiceCollectionExtensions.cs`
  in the new `Composition/`
  folder. This matches the
  plan's intent.
- The plan implied the App
  project does not yet reference
  `AiEng.Platform.Application`;
  it does. M2.2 required no
  project-graph change.
- The plan implied the
  Tailwind content path needed
  to be updated to include
  `Components/Navigation/`. The
  existing wildcard
  `./src/AiEng.Platform.App/Components/**/*.razor`
  already covers the new
  folder; no
  `tailwind.config.js` change
  was needed (a no-op).

---

## Known Limitations

- The
  `Pages_AreReachable_Through_Registry`
  architecture test locates
  `AiEng.Platform.App.dll` by
  enumerating the test project's
  base directory; it is
  resilient to the build output
  layout but depends on the
  `AiEng.Platform.App` project
  shipping the `[RouteMetadata]`
  attributes on the page types
  themselves (not on an
  intermediate base class).
- The breadcrumb follows the
  `RouteMetadata.Parent` chain,
  but no M2.2 page sets a
  `Parent` (the M2.2 pages are
  all top-level). The breadcrumb
  is M2.3's concern; the
  registry's `Parent` field is
  the data source M2.3 reads.
- The
  `NavigationServiceCollectionExtensionsTests`
  (3 tests) test the DI
  integration; they do not test
  the assembly-scan edge cases
  (the 6 `RouteRegistryTests`
  cover the edge cases directly).
- The `AppNavItem` tests use
  bUnit's
  `Services.GetRequiredService<NavigationManager>().NavigateTo(...)`
  to set the active route; the
  tests do not exercise a
  real-world click. The
  `AppNavItem` is a thin
  wrapper around `NavLink`;
  the `NavLink` active-state
  behaviour is well-tested in
  the framework.

---

## Next Recommended Step

> **M2.3 — Top Bar, Breadcrumbs, and
> Page Headers.** Read
> `.ai/plans/M2.3-topbar-breadcrumbs.md`.
> The plan is now `Awaiting
> Approval` (the M2.2 closeout
> session promoted the M2.3
> plan stub to a full plan with
> the 13-step lifecycle). The
> first action of the next
> session is to either approve
> the M2.3 plan (and start M2.3
> implementation per the plan's
> own order) or amend the plan
> and re-submit it. The next
> session follows the
> **Progressive Coding Rule** in
> `.ai/workflows/progressive-coding.md`:
> one task per session;
> 13-step lifecycle; stop after
> the coherent commit.

---

## Project Continuity (Rule 15) and Evidence (Rule 17)

A session that ends without
updating the project-continuity
state and leaving evidence has
not ended. Confirm that the
following were done at session
end:

- [x] `.ai/state/current.md` —
      updated to reflect the
      state of the repository
      right now (milestone,
      branch, last commit, last
      validation result, exact
      next step).
- [x] `.ai/state/task-board.md` —
      the task the session
      worked (T-002) moved from
      `In Progress` to `Done`;
      T-003 promoted to `Ready`.
- [x] `.ai/handoffs/2026-07-11-m2-2-navigation-registry-sidebar.md`
      — the per-session
      handoff, written
      following the
      `session-handoff.md`
      template.
- [x] `.ai/handoffs/latest.md` —
      mirror of the
      per-session handoff.
- [x] `implementation-report-m2-2-navigation-registry-sidebar.md`
      — the receipt (this
      file).
- [x] **Coherent commit**
      (Rule 17 in `AGENTS.md`):
      `feat(m2.2): add navigation registry and sidebar`
      on
      `feature/m2-2-navigation-registry-sidebar`.
      The commit is local;
      pushing requires explicit
      authorisation (the
      session has no remote
      configured; push is
      skipped per the brief).

---

## Linked Artefacts

- `.ai/plans/M2.2-navigation-registry-sidebar.md`
  — the approved plan this
  report implements against
  (mandatory).
- `.ai/plans/M2.1-application-shell-skeleton.md`
  — the M2.1 plan; the shell
  foundation M2.2 composes
  against.
- `.ai/plans/M2.3-topbar-breadcrumbs.md`
  — the M2.3 plan, promoted
  from the `Draft` stub to a
  full `Awaiting Approval`
  plan in the M2.2 closeout
  session.
- `implementation-report-m2-1-application-shell-foundation.md`
  — the M2.1 implementation
  report; M2.2 preserves M2.1
  evidence.
- `.ai/handoffs/2026-07-11-m2-2-navigation-registry-sidebar.md`
  — the M2.2 per-session
  handoff.
- `.ai/handoffs/2026-07-11-m2-1-application-shell-foundation.md`
  — the M2.1 per-session
  handoff.
- `task-board.md` —
  `T-002` in `Done Recently`;
  `T-003` in `Ready`.
- `current.md` — the live
  state, updated at session
  end (Rule 15 in `AGENTS.md`).
- The commit hash of the
  session's work (Rule 17 in
  `AGENTS.md`):
  `feat(m2.2): add navigation registry and sidebar`
  on
  `feature/m2-2-navigation-registry-sidebar`.
