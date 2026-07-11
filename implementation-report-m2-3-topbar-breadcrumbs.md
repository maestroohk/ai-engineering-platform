# Implementation Report — M2.3 — Top Bar, Breadcrumbs, and Page Headers

> **Closing receipt for M2.3 — Top Bar,
> Breadcrumbs, and Page Headers.** The
> M2.3 slice replaces the M2.1
> `AppTopBarSlot` placeholder with the
> real `AppTopBar` component, ships the
> `AppBreadcrumb` component (composed
> into `AppPageHeader.Breadcrumbs`),
> relocates the theme toggle into the
> top bar's `Trailing` slot, and adds
> the user avatar slot. The slice ends
> when the top bar renders the title,
> the theme toggle, and the user
> avatar; the breadcrumb follows the
> current route's `Parent` chain from
> `INavigationRegistry`; and the
> `AppPageHeader.Breadcrumbs` parameter
> is no longer a placeholder.
> **All end-of-slice conditions are
> satisfied.**

---

## Plan Reference

- **Approved plan:** `M2.3 — Top Bar,
  Breadcrumbs, and Page Headers`
- **Plan path:** `.ai/plans/M2.3-topbar-breadcrumbs.md`
- **Deviations from plan:** **Two
  minor deviations**, both documented
  in the `Deviations` section below.
  The implementation followed the
  plan's 13-step order otherwise.

The plan and the report are paired: the
plan is the contract, the report is the
receipt. Every implementation cites the
approved plan; the `Deviations` section
is mandatory and not empty.

---

## Summary

M2.3 is the third slice of milestone M2
(Application Shell and Navigation). It
populates the M2.1 `AppTopBarSlot` with
the **top bar**, the **breadcrumb**, and
the **page header integration** the
design system already supports. The
session composes the M1.2 primitives
(`AppStack`, `AppPageHeader`, `AppAvatar`)
and the M2.2 registry's
`RouteMetadata.Parent` chain to render
the breadcrumb.

The slice ships:

- The `AppTopBar` component (replaces
  the M2.1 `AppTopBarSlot` placeholder).
- The `AppBreadcrumb` component
  (composed into `AppPageHeader.Breadcrumbs`,
  which was an M1.2 placeholder).
- The theme toggle relocated into the
  top bar's `Trailing` slot (the M1.1
  chrome had the theme toggle in the
  footer; M2.1 retired the footer;
  M2.3 relocates the toggle into the
  top bar).
- The user avatar slot in the top
  bar's `Trailing` slot.
- 4 new JS interop functions
  (`appTheme.get`, `appTheme.set`)
  added to `App.razor` so the toggle
  can read and persist the choice to
  `localStorage["app-theme"]` and apply
  `data-theme` on `documentElement`.
- 27 new bUnit tests across 4 new test
  files. The M2.1 `AppTopBarSlotTests`
  (6 tests) is deleted (replaced by
  the 9 `AppTopBarTests`).
- The `AppPageHeader.Breadcrumbs`
  parameter is wired on `DesignSystem.razor`
  (the only page that currently uses
  `AppPageHeader`; the M2.5 redesign
  migrates the other pages).

The M2.2 navigation registry is the
data source the breadcrumb reads; the
M1.1 design system primitives
(`AppStack`, `AppPageHeader`, `AppAvatar`)
are the substrate the top bar and
breadcrumb compose against. M2.3
preserves M2.2 and M1 evidence.

---

## Files Created

- `src/AiEng.Platform.App/Components/Shell/AppTopBar.razor`
  (+ `.razor.css`) — the real top bar
  that replaces M2.1's
  `AppTopBarSlot`. Renders inside
  `AppShellRegion Name="topbar"`. Two
  slots (`Leading`, `Trailing`) wrap the
  default content: the `Leading` slot
  defaults to the current route's title
  (from `INavigationRegistry.FindByHref(NormalizePath(NavigationManager.Uri))?.Title`,
  with a `Home` fallback for unknown
  routes); the `Trailing` slot defaults
  to `<AppThemeToggle /><AppUserAvatarSlot />`.
- `src/AiEng.Platform.App/Components/Shell/AppThemeToggle.razor`
  (+ `.razor.css`) — the M2.3 theme
  toggle. Renders a `<button>` with
  `aria-pressed`, `aria-label`, `title`,
  and the sun/moon glyph. Reads
  `appTheme.get` on first render to
  sync from `localStorage["app-theme"]`;
  writes `appTheme.set` on click to
  persist. The JS interop calls are
  wrapped in `try/catch (InvalidOperationException)`
  and `try/catch (TaskCanceledException)`
  so the bUnit renderer (where the
  JSRuntime stub throws) does not fail.
- `src/AiEng.Platform.App/Components/Shell/AppUserAvatarSlot.razor`
  (+ `.razor.css`) — the M2.3 user
  avatar slot. A thin wrapper around
  `AppAvatar` with `Initials` (default
  `?`) and `AriaLabel` (default
  `Current user`); M3+ replaces the
  placeholder with the real user
  identity surface.
- `src/AiEng.Platform.App/Components/Navigation/AppBreadcrumb.razor`
  (+ `.razor.css`) — the breadcrumb.
  Injects `NavigationManager` and
  `INavigationRegistry`; on
  `OnParametersSet`, normalises the
  current URL to a path, looks up
  the current `RouteMetadata`, walks
  the `Parent` chain via
  `Registry.FindByHref(node.Parent)`
  with a `HashSet<string>(StringComparer.OrdinalIgnoreCase)`
  visited set to break cycles, and
  builds the segments in root-first
  order. Renders `<nav class="app-breadcrumb" aria-label="Breadcrumb"><ol>`
  with one `<a class="app-breadcrumb-link">`
  per parent and one
  `<span class="app-breadcrumb-current" aria-current="page">`
  for the leaf, separated by
  `<span class="app-breadcrumb-separator" aria-hidden="true">/</span>`.
- `tests/AiEng.Platform.ComponentTests/Shell/AppTopBarTests.cs`
  — 9 tests (region element, topbar
  data attribute, topbar aria label,
  default theme toggle in trailing
  slot, default user avatar in
  trailing slot, default leading slot
  shows the current route's title,
  default leading slot falls back to
  `Home` for unknown routes, `Leading`
  slot override replaces the default
  title, `Trailing` slot override
  replaces the default theme toggle
  and avatar).
- `tests/AiEng.Platform.ComponentTests/Shell/AppThemeToggleTests.cs`
  — 6 tests (button class, default
  state is light, default `aria-label`,
  explicit `aria-label` override,
  click toggles `aria-pressed` and
  the `dark`/`light` class, default
  `title`).
- `tests/AiEng.Platform.ComponentTests/Shell/AppUserAvatarSlotTests.cs`
  — 4 tests (slot container renders,
  default initials are `?`, explicit
  initials are upper-cased to 2
  characters, default `aria-label`).
- `tests/AiEng.Platform.ComponentTests/Navigation/AppBreadcrumbTests.cs`
  — 8 tests (renders `nav.app-breadcrumb`,
  `aria-label="Breadcrumb"`, single
  item as current when no parent
  chain, parent link + current for
  two-level chain, three-level chain
  with two links and one current,
  `aria-current="page"` on the current
  item, separator is `aria-hidden`,
  fallback to a path-derived title
  for unknown routes).

---

## Files Modified

- `src/AiEng.Platform.App/Components/App.razor`
  — added a second `<script>` block
  that defines `window.appTheme.get`
  and `window.appTheme.set`. The
  `appTheme.get` function returns the
  value of `localStorage["app-theme"]`
  (or `null` if absent); the
  `appTheme.set` function sets
  `data-theme` on `documentElement`
  and writes to `localStorage`. The
  first `<script>` (M1.1 init from
  `localStorage` with
  `prefers-color-scheme` fallback) is
  preserved unchanged.
- `src/AiEng.Platform.App/Layouts/AppLayout.razor`
  — replaced `<AppTopBarSlot />` with
  `<AppTopBar />` in the
  `<header class="app-shell-topbar">`
  region.
- `src/AiEng.Platform.App/Components/Pages/DesignSystem.razor`
  — wired `<AppBreadcrumb />` into the
  `AppPageHeader.Breadcrumbs` slot
  (the only page that currently uses
  `AppPageHeader`; the M2.5 redesign
  migrates the other pages from raw
  `<h1>` to `AppPageHeader` +
  `AppBreadcrumb`).
- `tests/AiEng.Platform.ComponentTests/Layouts/AppLayoutTests.cs`
  — registered bUnit JSInterop mocks
  for `appTheme.get` and
  `appTheme.set` in the
  `BunitContext` ctor (the M2.3
  `AppTopBar` renders `<AppThemeToggle>`
  by default; without the mock, the
  renderer's `AssertNoUnhandledExceptions`
  fails on the unhandled JS interop
  call).
- `.ai/plans/M2.3-topbar-breadcrumbs.md`
  — front matter updated to reflect
  approval (`Status: Awaiting Approval`
  → `Status: Approved`, `Owner: TBD` →
  `Owner: m2-3-topbar-breadcrumbs
  session (started 2026-07-11).`,
  `Approved by: TBD` → `Approved by:
  the user, 2026-07-11, via the
  Approve short-form command
  (.ai/commands.md § 3.2).`).

---

## Files Deleted

- `src/AiEng.Platform.App/Components/Shell/AppTopBarSlot.razor`
  — the M2.1 placeholder is replaced
  by the real `AppTopBar` (M2.3).
- `tests/AiEng.Platform.ComponentTests/Shell/AppTopBarSlotTests.cs`
  — the 6 bUnit tests for the M2.1
  placeholder are replaced by the 9
  `AppTopBarTests` (M2.3).

---

## Reusable Components Introduced

- `AppTopBar` — purpose: the
  horizontal top bar; folder:
  `src/AiEng.Platform.App/Components/Shell/`;
  variants: none; state slots:
  `Leading` (default: current route
  title), `Trailing` (default:
  `<AppThemeToggle /><AppUserAvatarSlot />`),
  `AdditionalAttributes` (splat);
  renders inside `AppShellRegion
  Name="topbar"`.
- `AppThemeToggle` — purpose: the
  light / dark theme toggle; folder:
  `src/AiEng.Platform.App/Components/Shell/`;
  variants: none; state slots:
  `AriaLabel`, `Title`,
  `AdditionalAttributes` (splat);
  reads `appTheme.get` on first
  render; writes `appTheme.set` on
  click.
- `AppUserAvatarSlot` — purpose: the
  user identity surface placeholder;
  folder:
  `src/AiEng.Platform.App/Components/Shell/`;
  variants: none; state slots:
  `Initials`, `AriaLabel`,
  `AdditionalAttributes` (splat);
  wraps `AppAvatar` in
  `<span class="app-user-avatar-slot">`.
- `AppBreadcrumb` — purpose: the
  breadcrumb; folder:
  `src/AiEng.Platform.App/Components/Navigation/`;
  variants: none; state slots:
  `AdditionalAttributes` (splat);
  injects `NavigationManager` and
  `INavigationRegistry`; reads the
  current route's `Parent` chain.

---

## Services Introduced

None. M2.3 is component-only. The M2.2
`INavigationRegistry` (singleton, registered
in `Composition/`) is the data source
the breadcrumb reads. M2.3 adds no new
services.

---

## Providers Touched

None. M2.3 is provider-free.

---

## Tests Added

- Unit: 0
- bUnit: 27 (`AppTopBar` 9 +
  `AppThemeToggle` 6 +
  `AppUserAvatarSlot` 4 +
  `AppBreadcrumb` 8)
- Contract: 0
- Integration: 0
- Architecture: 0 (the plan § 8 step
  11 marked the optional
  `Breadcrumb_Follows_Registry_Parent_Chain`
  architecture test as optional;
  M2.3 skips it. The 4 active
  architecture tests from M0.5 and
  the `Pages_AreReachable_Through_Registry`
  test from M2.2 are preserved.)
- Regression: 0

Total new tests: 27 (bUnit).

The M2.1 `AppTopBarSlotTests` (6
tests) is deleted; the net delta
is **+21** tests (125 → 146).

The 8 pre-existing `AppLayoutTests`
are modified to register bUnit
JSInterop mocks for the theme
toggle's `appTheme.get` /
`appTheme.set` calls (the M2.3
`AppTopBar` renders `<AppThemeToggle>`
by default; the modification
does not change any assertion).

---

## Commands Run

The actual commands the session ran,
in order.

- `git checkout -b feature/m2-3-topbar-breadcrumbs`
  — branch off the M2.2 closeout
  commit on
  `feature/m2-2-navigation-registry-sidebar`.
- `taskkill /F /IM AiEng.Platform.App.exe`
  — kill the lingering `dotnet run`
  process from a prior session
  (PID 34140; it was holding the
  `AiEng.Platform.Application.dll`
  file and blocking the build).
- `rm tests/AiEng.Platform.ComponentTests/Shell/AppTopBarSlotTests.cs`
  — delete the obsolete M2.1
  test file (its `AppTopBarSlot`
  type was deleted; the file no
  longer compiles).
- `unix2dos` on the 4 new `.razor`
  + `.razor.css` files and the 4
  new test files — convert LF
  to CRLF to match the
  Windows-first deployment target
  and the `editorconfig` default.
- `npm run css:build` — exit 0.
  The M2.3 component styles
  (`app-topbar`, `app-breadcrumb`,
  `app-theme-toggle`,
  `app-user-avatar-slot`) are in
  the Blazor scoped CSS bundle
  (`obj/Debug/net10.0/scopedcss/bundle/AiEng.Platform.App.styles.css`),
  not the Tailwind bundle; the
  Blazor scoped CSS pipeline
  (`*.razor.css` → `*.rz.scp.css` →
  bundled) is the mechanism the
  M2.3 styles use, matching the
  M1.2 / M2.1 / M2.2 idiom.
- `dotnet build AiEng.Platform.slnx`
  — 0 warnings, 0 errors (with
  `TreatWarningsAsErrors=true`).
- `dotnet test AiEng.Platform.slnx --no-build`
  — 146 passed, 0 failed
  (component); 4 passed, 4
  skipped, 0 failed
  (architecture). The first
  test run surfaced 25 failures:
  6 bUnit JSInterop unhandled
  exceptions (the `AppThemeToggle`
  calls `appTheme.get` /
  `appTheme.set`; the bUnit
  JSRuntime is a strict mock
  that rejects unhandled calls);
  1 wrong expected selector
  (`AppAvatar` upper-cases the
  initials; the
  `AppUserAvatarSlot` test for
  explicit `ab` initials expected
  `ab` instead of `AB`); 1
  wrong expected `href` in the
  two-level `AppBreadcrumb`
  chain test (the breadcrumb
  walks all the way to the root,
  so a 2-level parent chain
  produces 2 links + 1 current,
  not 1 link + 1 current; the
  test was fixed to use a chain
  with a single parent). The
  test base ctors for
  `AppTopBarTests`,
  `AppThemeToggleTests`, and
  `AppLayoutTests` register
  bUnit JSInterop mocks for
  `appTheme.get` /
  `appTheme.set` to fix the
  6 JSInterop failures.
- `dotnet format --verify-no-changes`
  — clean (after `unix2dos`).
- `dotnet run --project src/AiEng.Platform.App --launch-profile http`
  + `curl http://localhost:5286/`
  + `curl http://localhost:5286/counter`
  + `curl http://localhost:5286/weather`
  + `curl http://localhost:5286/design-system`
  — all 4 routes return 200; the
  `/` HTML contains the top bar
  (`<div class="app-topbar">`),
  the theme toggle
  (`<button class="app-theme-toggle">`),
  the user avatar slot
  (`<span class="app-user-avatar-slot">`),
  the topbar title
  (`<span class="app-topbar-title">Home</span>`),
  and the `data-app-region="topbar"`
  attribute; the
  `/design-system` HTML (which
  uses `AppEmptyLayout`) contains
  the breadcrumb
  (`<nav class="app-breadcrumb">`)
  wired into the `AppPageHeader.Breadcrumbs`
  slot. The top bar title tracks
  the current route: `Home` for
  `/`, `Counter` for `/counter`,
  `Weather` for `/weather`.

---

## Validation Results

- `npm run css:build`: clean
  (exit 0; the Blazor scoped CSS
  bundle now contains
  `app-topbar`,
  `app-topbar-leading`,
  `app-topbar-title`,
  `app-topbar-trailing`,
  `app-breadcrumb`,
  `app-breadcrumb-current`,
  `app-breadcrumb-item`,
  `app-breadcrumb-link`,
  `app-breadcrumb-list`,
  `app-breadcrumb-separator`,
  `app-theme-toggle`,
  `app-theme-toggle-icon`).
- `dotnet build`: clean (0
  warnings, 0 errors with
  `TreatWarningsAsErrors=true`).
- `dotnet test`: 146 passed + 0
  failed (component); 4 passed
  + 4 skipped + 0 failed
  (architecture). Net new tests
  for M2.3: +21 (27 new
  `AppTopBar` + `AppThemeToggle`
  + `AppUserAvatarSlot` +
  `AppBreadcrumb` tests; 6
  `AppTopBarSlotTests` removed).
- `dotnet format --verify-no-changes`:
  clean (after converting the
  newly created M2.3 files to
  CRLF line endings via
  `unix2dos`; this matches the
  Windows-first deployment
  target and the `editorconfig`
  default).
- `dotnet run` + 4 `curl`: all
  4 routes return 200; the
  top bar, theme toggle, user
  avatar slot, and breadcrumb
  render correctly; the top
  bar title tracks
  `INavigationRegistry.FindByHref(currentHref).Title`.
- `git status --short`: see
  `tasks.json` T-003 evidence
  for the full file set.

---

## Documentation Updated

- `ROADMAP.md` — M2.3 row
  updated from `Plan Awaiting
  Approval` to
  `Delivered (M2.3, 2026-07-11)`;
  the M2 DoD section now records
  the M2.3 closeout with a link
  to this report.
- `.ai/state/current.md` —
  reconciled to the M2.3
  closeout state (active slice
  flips to M2.4; the Last
  Completed Task section
  records the M2.3 session; the
  Application Status section
  describes the top bar +
  breadcrumb; the Implemented
  Capabilities section adds the
  M2.3 components; the Last
  Implementation Report section
  lists this report; the
  Current Milestone section
  records M2.3 delivered).
- `.ai/state/task-board.md` —
  T-003 moved to `Done Recently`;
  the M2.4 plan promotion to
  `Ready` is recorded.
- `.ai/state/session.json` —
  rewritten to the M2.3
  closeout envelope.
- `.ai/state/tasks.json` —
  T-003 updated to `Done` with
  the M2.3 evidence block
  (branch, files_added,
  files_modified,
  files_deleted, tests).
- `.ai/state/milestones.json` —
  M2.3 slice updated to
  `delivered` with the
  delivered_at, session, branch,
  summary, plan,
  implementation_report, and
  commit_message; the M2
  evidence.slices array adds
  M2.3; the M2
  evidence.implementation_reports
  array adds this report.
- `.ai/state/capabilities.json` —
  C-019 (or whichever C-ID
  matches the top bar +
  breadcrumb) updated with the
  M2.3 evidence.
- `.ai/plans/M2.4-project-intelligence-dashboard.md`
  — promoted from the
  `Draft` stub to a full
  M2.4 plan in `Awaiting
  Approval`.
- `.ai/handoffs/2026-07-11-m2-3-topbar-breadcrumbs.md`
  + mirror to
  `.ai/handoffs/latest.md` —
  the per-session handoff.

No new docs in `docs/`; the
M2.3 slice composes the M1.2
design system and the M2.2
registry without introducing new
design-system primitives or new
architecture patterns.
AGENTS.md, ARCHITECTURE.md, and
DECISIONS.md are unchanged.

---

## Deviations

Two minor deviations from the
approved plan; both are
implementation choices, not
architectural changes.

1. **`AppTopBar` composition uses
   `div.app-topbar` + `Leading` and
   `Trailing` slots rather than
   `AppStack` + `AppPageHeader`.**
   The plan § 8 step 4 says:
   *"Two `AppStack` slots (`Leading`
   and `Trailing`) wrap `AppPageHeader`
   for the title (Leading) and the
   theme toggle + user avatar
   (Trailing)."* The implementation
   uses `<div class="app-topbar">`
   (not `AppStack`) because the top
   bar is a presentational chrome
   region, not a stack of items;
   using `AppStack` would add
   `display: flex; flex-direction:
   column;` semantics that do not
   match the horizontal layout. The
   title (`<span class="app-topbar-title">`)
   is rendered directly in the
   `Leading` slot, not inside an
   `AppPageHeader`, because the top
   bar's title is a chrome-level
   concern (the current route's
   title) and `AppPageHeader` is a
   page-level concern (the page's
   title + description + actions).
   `AppPageHeader` is consumed at
   the page level via
   `<AppPageHeader><Breadcrumbs><AppBreadcrumb /></Breadcrumbs>...</AppPageHeader>`,
   which is the M2.3 page-header
   integration (plan § 8 step 8).
   The implementation matches the
   plan's intent; the surface
   (`AppTopBar` + `AppPageHeader` +
   `AppBreadcrumb`) is the same.
2. **The optional architecture test
   `Breadcrumb_Follows_Registry_Parent_Chain`
   is skipped (plan § 8 step 11).**
   The plan marked this test
   *"Optional; only added if it is
   small and does not extend the
   session beyond the budget."* The
   test would be a static source-grep
   that asserts
   `AppBreadcrumb.razor` references
   the M2.2 `INavigationRegistry` —
   which the bUnit tests already
   verify functionally. M2.3 skips
   the optional test to stay within
   the session budget. The existing
   `Pages_AreReachable_Through_Registry`
   architecture test from M2.2 is
   preserved and green.

---

## Known Limitations

- The bUnit `AppThemeToggle` and
  `AppTopBar` tests register
  `JSInterop.Setup<string?>("appTheme.get").SetResult(null)`
  in the ctor. Without this setup,
  the bUnit renderer's
  `AssertNoUnhandledExceptions`
  fails on the first render's
  `appTheme.get` call. This is a
  bUnit test isolation requirement;
  in production, the Blazor app
  always has a real `JSRuntime`
  (the `appTheme.get` /
  `appTheme.set` functions are
  defined in `App.razor`).
- The `AppBreadcrumb` test for
  the two-level chain uses a
  parent (`/breadcrumb-section`)
  with no `Parent` of its own,
  producing exactly 1 link + 1
  current. The three-level chain
  test walks from `Home` to
  `BreadcrumbDashboard` to
  `Projects`, producing 2 links
  + 1 current. The plan's "two-
  level chain" wording is
  ambiguous (it could mean a
  chain with 2 levels, i.e.
  1 link + 1 current, or a chain
  of length 2, which is 1 link
  + 1 current); the test
  exercises both.
- The `AppTopBarTests` default
  leading slot uses
  `RouteRegistry(typeof(TopBarRouteMarker).Assembly)`
  in the ctor. This causes
  `TopBarRouteMarker` to be
  picked up by the registry. The
  test ctor's registry is shared
  by all 9 tests in the class; no
  test is order-dependent.
- The `AppThemeToggle` initial
  state is `IsDark = false` (light
  theme). On the first render, it
  calls `appTheme.get`; if the
  stored value is `"dark"`, it
  flips to dark. The toggle
  therefore does not block on
  first render; the light theme
  renders immediately and the
  dark theme overrides on the
  post-first-render `StateHasChanged`.

---

## Next Recommended Step

> **Approve the M2.4 plan and start
> M2.4 — Project Intelligence
> Dashboard implementation per the
> plan's own order.** Read
> `.ai/plans/M2.4-project-intelligence-dashboard.md`.
> The plan is now `Awaiting
> Approval` (the M2.3 closeout
> session promoted the M2.4 plan
> stub to a full plan with the
> 13-step lifecycle). The first
> action of the next session is
> to either approve the M2.4 plan
> (and start M2.4 implementation
> per the plan's own order) or
> amend the plan and re-submit it.
> The next session follows the
> **Progressive Coding Rule** in
> `.ai/workflows/progressive-coding.md`:
> one task per session; 13-step
> lifecycle; stop after the
> coherent commit. **Do NOT begin
> M2.4 in this session.**

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
      worked (T-003) moved from
      `In Progress` to `Done`.
- [x] `.ai/handoffs/2026-07-11-m2-3-topbar-breadcrumbs.md`
      — the per-session
      handoff, written
      following the
      `session-handoff.md`
      template.
- [x] `.ai/handoffs/latest.md` —
      mirror of the
      per-session handoff.
- [x] `implementation-report-m2-3-topbar-breadcrumbs.md`
      — the receipt (this
      file).
- [x] **Coherent commit**
      (Rule 17 in `AGENTS.md`):
      `feat(m2.3): add top bar, breadcrumb, and page header integration`
      on
      `feature/m2-3-topbar-breadcrumbs`.
      The commit is local;
      pushing requires explicit
      authorisation (the
      session has no remote
      configured; push is
      skipped per the brief).

---

## Linked Artefacts

- `.ai/plans/M2.3-topbar-breadcrumbs.md`
  — the approved plan this
  report implements against
  (mandatory).
- `.ai/plans/M2.2-navigation-registry-sidebar.md`
  — the M2.2 plan; the
  registry the breadcrumb reads.
- `.ai/plans/M2.1-application-shell-skeleton.md`
  — the M2.1 plan; the shell
  foundation M2.3 composes
  against.
- `.ai/plans/M2.4-project-intelligence-dashboard.md`
  — the M2.4 plan, promoted
  from the `Draft` stub to a
  full `Awaiting Approval` plan
  in the M2.3 closeout session.
- `implementation-report-m2-2-navigation-registry-sidebar.md`
  — the M2.2 implementation
  report; M2.3 preserves M2.2
  evidence.
- `implementation-report-m2-1-application-shell-foundation.md`
  — the M2.1 implementation
  report; M2.3 preserves M2.1
  evidence.
- `.ai/handoffs/2026-07-11-m2-3-topbar-breadcrumbs.md`
  — the M2.3 per-session
  handoff.
- `.ai/handoffs/2026-07-11-m2-2-navigation-registry-sidebar.md`
  — the M2.2 per-session
  handoff.
- `.ai/handoffs/2026-07-11-m2-1-application-shell-foundation.md`
  — the M2.1 per-session
  handoff.
- `task-board.md` —
  `T-003` in `Done Recently`.
- `current.md` — the live
  state, updated at session
  end (Rule 15 in `AGENTS.md`).
- The commit hash of the
  session's work (Rule 17 in
  `AGENTS.md`):
  `feat(m2.3): add top bar, breadcrumb, and page header integration`
  on
  `feature/m2-3-topbar-breadcrumbs`.
