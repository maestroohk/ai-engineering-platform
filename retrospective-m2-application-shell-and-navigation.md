# Retrospective — M2 — Application Shell and Navigation

> **The M2 milestone retrospective** (per the
> Milestone Closeout Standard at
> `.ai/workflows/milestone-closeout.md`, introduced in
> the M2.6 closeout slice on 2026-07-11). This is the
> **first milestone retrospective** in this repository.
> The retrospective follows the standard's 13 sections
> in order. Each section cites the evidence that backs
> the claim.

---

## 1. Delivered Capabilities

M2 delivered the following capabilities. The
verification evidence is the M2.5 closeout commit
(`1614f93`) and the M2.6 closeout commit
(`m2` tag, M2.6 commit on `main`).

| C-ID    | Title                       | Status     | Evidence                                                                                       |
| ------- | --------------------------- | ---------- | ---------------------------------------------------------------------------------------------- |
| C-019   | INavigationService          | Done       | `INavigationRegistry` in `src/AiEng.Platform.Application/Navigation/`; `RouteMetadata`, `RouteMetadataAttribute`, `RouteRegistry`; `[RouteMetadata]` on all 6 pages; the `Pages_AreReachable_Through_Registry` architecture test is active and green (M2.2). |
| C-022   | IProjectIntelligenceReader  | Done       | `IProjectIntelligenceReader`, `ProjectIntelligenceSnapshot`, `ProjectIntelligenceReader` in `src/AiEng.Platform.Application/ProjectIntelligence/`; `AddProjectIntelligence` extension in `src/AiEng.Platform.App/Composition/`; `Dashboard.razor` at `/dashboard`; the `Pages_Resolve_State_Through_Reader` architecture test is active and green (M2.4). |

**Other delivered surface** (components and
infrastructure, not formal C-IDs):

- The full Blazor application shell
  (`AppLayout`, `AppEmptyLayout`, `AppShellRegion`,
  `AppSidebar`, `AppSidebarItem`, `AppNavItem`,
  `AppTopBar`, `AppThemeToggle`, `AppUserAvatarSlot`,
  `AppBreadcrumb`).
- The T-017 theme toggle fix (the click handler is
  wired; the M2.3 + M2.4 + M2.5 chain of fixes is
  documented in
  `implementation-report-m2-5-empty-routes-responsive-accessibility.md`).
- The empty-route pattern (`Home.razor` and
  `NotFound.razor` reach `AppEmptyState`).
- The responsive matrix
  (`AppLayout.razor.css` `lg` / `md` / `sm` media
  queries; documented in `docs/ui-principles.md` §
  10.1).
- The accessibility audit harness
  (`KeyboardSmokeTests`, `AriaCurrentInvariantTests`,
  `AxeCoreAuditTests` — the last is
  registered-but-disabled per ADR-016 / M4-D).

## 2. Deferred Capabilities

M2 deferred the following capabilities. The
deferral rationale is recorded for each.

| C-ID / surface                | Status                       | Deferral rationale                                                                                                                                                |
| ----------------------------- | ---------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| C-023, C-024, C-025 (planned) | Not in the capability graph  | The M2.5 plan named three C-IDs (C-023 = empty-route pattern, C-024 = responsive matrix, C-025 = a11y audit) as the M2.5 deliverables. The C-IDs were never registered in `.ai/state/capabilities.json`; the deliverables landed as cross-cutting hardening of the M2.1–M2.4 surface rather than as new formal capabilities. The cross-cutting hardening is real (Home.razor, NotFound.razor, AppLayout.razor.css, the four new test files), but the capability graph treats it as part of the M2.1–M2.4 capability delivery. **No C-IDs are deferred; the three C-IDs were never registered.** |
| Axe-core audit activation     | Registered but disabled      | `tests/AiEng.Platform.ArchitectureTests/A11y/AxeCoreAuditTests.cs` has three tests, each `[Fact(Skip = "...")]` with the skip reason citing ADR-016 / M4-D. The activation milestone is M4-D, which introduces the first concrete process providers and the first composition-root architecture test activation. |
| Provider-boundary tests       | Registered but disabled      | `tests/AiEng.Platform.ArchitectureTests/Boundaries/CompositionRootBoundaryTests.cs` has four tests, each skipped per the M1 closeout. Activation is M4-D. |
| `AppDialog`, `AppTabs`, `AppTab` (M2.1 plan) | Removed (not implemented)    | The M2.1 plan's "Reusable components introduced" list included `AppDialog` and `AppTabs` "deferred to a later M2 slice or removed if unused." M2.6 confirms removal: the components were never needed; M2 closes without them. |
| Icon-rail collapse at <1024px | Future enhancement           | The M2.5 plan named "sidebar collapses to an icon rail below 1280px." The implementation narrows the sidebar progressively (8rem at 1024–1279px) because most sidebar routes do not carry an `Icon` (only `Dashboard` does). The icon-rail collapse lands in M8 closeout when every sidebar route carries an `Icon`. |
| Mobile viewport support below 1024px | Out of M2 scope       | Per ADR-005, the M2 primary viewport is 1280x720 minimum. The full responsive matrix (mobile + tablet + ultrawide) lands in M8 closeout. |
| `lavish-axi` M1 design-system review (M1 follow-up) | Blocked, not M2's debt  | The M1 closeout records `lavish-axi` as `Blocked` (the tool is not installed on the host). The M2 retrospective does not assume the debt; the block is inherited from M1. |

## 3. Technical Debt

M2's known technical debt. Each item names the
file, the debt, and the milestone that resolves it.

| Debt                                                                                                       | File / area                                                                                                                                            | Resolved in       |
| ---------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ | ----------------- |
| The `master` branch in the task board / `current.md` / `milestones.json` references the legacy branch name. | Various Markdown + JSON files (the re-keying commit `92d0c17` already fixed the Git-branch references; the M2 evidence section is up to date).         | **Resolved in the M2.6 infrastructure commit (2026-07-11; `097c016`).** |
| The `current.md` "Test Status" line still says "197 passed, 0 failed, 7 skipped" — needs the M2.6 closeout to add the M2.6 closeout validation. | `.ai/state/current.md` lines 463–475                                                                                                                  | **Resolved in the M2.6 closeout session (this retrospective's companion state updates).** |
| The `ProjectIntelligenceReader`'s `repoRoot` default walks up from `Directory.GetCurrentDirectory()` looking for `AiEng.Platform.slnx`. The default is correct for the current dev loop but brittle for production-style hosting where the working directory may be different. | `src/AiEng.Platform.Application/ProjectIntelligence/ProjectIntelligenceReader.cs`                                                                       | M3 (when the production hosting story is fleshed out; M3 may swap the default for an environment-driven path). |
| The dashboard's `CapabilityProgress` card shows the M0.5 + M2 capability counts but does not yet show the per-capability acceptance criteria. The criteria live in the new `capabilities.json` `acceptance_criteria` field but the dashboard does not render them. | `src/AiEng.Platform.App/Components/Pages/Dashboard.razor`                                                                                              | M3 or M8 (depends on the user's call on the per-capability drill-down; the data is already in the snapshot). |
| The design-system catalogue does not yet include an `AppToolbar` example (the M1 follow-up `M1-FU-1`). | `src/AiEng.Platform.App/Components/Pages/DesignSystem.razor`                                                                                            | M1-FU-1 (already on the task board as `Ready`; can be folded into M2.6 or a later M3 task). |
| The `app.css` rebuilt by `npm run css:build` is committed to the repository; future versions may want to gitignore it. | `src/AiEng.Platform.App/wwwroot/css/app.css` (currently committed; the build pipeline re-emits it)                                                    | A future ADR (no immediate action; the current setup is intentional for the no-tooling-required build). |

## 4. Known Issues

Open bugs, follow-up items, and
registered-but-disabled tests.

| Issue                                                                              | Severity       | Source                                                                                                       |
| ---------------------------------------------------------------------------------- | -------------- | ------------------------------------------------------------------------------------------------------------ |
| The T-017 theme toggle bug is **fixed** in M2.5 (commit `1614f93`). The fix is the `@rendermode InteractiveServer` declaration on `AppThemeToggle.razor` (not on `AppLayout.razor`, because the layout's `@Body` is a `RenderFragment` delegate that Blazor refuses to serialize across the SSR → interactive boundary). The M2.5 closeout's `AppLayout_ThemeToggleWiringTests` bUnit suite asserts the click handler is wired when the layout is rendered. | Resolved       | M2.5 closeout, T-017 entry in `tasks.json`. |
| The axe-core accessibility audit is registered-but-disabled. The 3 tests in `tests/AiEng.Platform.ArchitectureTests/A11y/AxeCoreAuditTests.cs` will activate in M4-D. | Low (deferred) | M2.5 closeout, `milestones.json` M2.5 evidence block. |
| The 4 provider-boundary tests in `tests/AiEng.Platform.ArchitectureTests/Boundaries/CompositionRootBoundaryTests.cs` are registered-but-disabled. They activate in M4-D. | Low (deferred) | M1 closeout, `milestones.json` M1 evidence block. |
| No `lavish-axi` review. The M1 design-system review tool is not installed on the host. | Blocked        | M1 closeout, `.ai/state/task-board.md` `Blocked` section. |
| The Tailwind content path includes the `Layouts/` directory but the post-build `app.css` does not show whether the Tailwind JIT engine's content discovery has scanned every Razor file. A regression test for the Tailwind content path is a future M3 / M4 task. | Low            | M2.1 closeout, `ROADMAP.md` § 3 M2.1 DoD. |
| The `master-delivery-plan.md` file's name retains "master" because the file is a delivery artefact, not a Git branch. The phrase "master delivery plan" is preserved in dozens of references. This is **intentional**, not a defect. | Not a defect   | M2.6 infrastructure commit (`097c016`), `.ai/workflows/branching-strategy.md` § 5. |

## 5. Lessons Learned

What M2's sessions taught the team. The lessons
are inputs to M3's plan.

### 5.1 Process lessons

- **One task per session is the right rhythm.** Each
  M2 slice was a single session with a single coherent
  commit; the per-slice handoff + implementation
  report + state update + coherent commit pattern is
  sustainable. M3 will follow the same pattern.
- **The Project-Continuity State (Rule 15) earns its
  place.** The structured state in `.ai/state/`
  made it trivial to resume after a context-window
  summary; the M2.5 session resumed from `.ai/state/`
  alone and produced a coherent commit without
  re-deriving any project context. The pattern
  generalises.
- **The command protocol (`.ai/commands.md`) is
  under-used.** M2 sessions all started with the
  full `feature.md` prompt; the `Continue`,
  `Approve`, `Resume`, `Finish` commands were used
  only for the plan-approval steps. M3 may benefit
  from using the commands more aggressively for
  short user instructions.
- **The M1 closeout template is reusable.** M2.6
  mirrors the M1 closeout's shape (verification +
  gap-fixing + deferred-review record + state
  update + handoff + implementation report +
  next-milestone plan). The M2.6 closeout
  **introduces** the Milestone Closeout Standard so
  the template is documented once, not re-stated
  per closeout.

### 5.2 Technical lessons

- **Blazor's `@rendermode InteractiveServer` cannot
  be declared on a `LayoutComponentBase`.** The
  layout's `@Body` is a `RenderFragment` delegate
  that Blazor refuses to serialize across the
  SSR → interactive boundary. The minimum-blast-
  radius fix is to declare the directive on the
  interactive child component, not on the layout.
  M2.5's T-017 fix is the canonical example. **The
  lesson is recorded in the M2.5 implementation
  report and in the T-017 entry of `tasks.json`.**
- **bUnit's `Render<T>()` does not honour
  `@layout` directives.** Tests that assert the
  page's layout must use reflection to read the
  `[Layout]` attribute, or they must render the
  layout directly. The M2.5 `EmptyRoutesTests` is
  the canonical example.
- **The Tailwind v3 JIT engine's content discovery
  is sensitive to file paths.** The M2.1 work added
  the `Layouts/` directory to the `content` path
  in `tailwind.config.js`; the lesson is that
  adding a new top-level Razor directory requires
  a corresponding update to the Tailwind content
  path. The lesson is recorded in
  `docs/architecture-principles.md` § "Frontend
  build pipeline."
- **The `Pages_Resolve_State_Through_Reader`
  architecture test is the M2.4 single-seam
  enforcement mechanism.** The dashboard consumes
  state only through `IProjectIntelligenceReader`;
  the architecture test fails the build if a page
  reads `.ai/state/*.json` directly. The pattern
  generalises: every read-side service should ship
  with an architecture test that enforces the
  single seam.
- **The `AppLayout` + `AppEmptyLayout` split is the
  right boundary.** The M1.1 chrome (single layout
  for everything) was a smell; the M2.1 split
  (one layout for the application shell, one for
  the design-system catalogue) is a clean boundary
  that the M1 template pages and the M2 pages
  both respect.

## 6. Architecture Changes

The architectural decisions M2 made or accepted.
Each change cites the ADR or the workflow that
approved it.

- **ADR-005 — the M2 primary viewport (1280x720
  minimum).** The M2.5 plan accepts the 1280x720
  minimum; the M2.5 implementation adds the `lg` /
  `md` / `sm` breakpoints but does not add mobile
  (<1024px) support. The M8 closeout adds the full
  responsive matrix.
- **ADR-013 — progressive self-dogfooding.** The
  M2 shell composes the M1.2 design system; the
  architecture test `Pages_Use_DesignSystem_Components_Not_DOM`
  fails the build if a page file contains a literal
  `<button>`, `<input>`, or inline-style attribute.
  The M2 plan satisfies the ADR; the M2.4 dashboard
  is the only data-owning page in M2 and consumes
  `AppCard` + `AppPageHeader` + `AppEmptyState` from
  the design system.
- **ADR-014 — read-side vs write-side state.** The
  M2.4 dashboard is read-only; the project
  registration page (M3) is the first write-side
  surface. The split is enforced by the
  `IProjectIntelligenceReader` interface
  (read-side) and the future `IProjectService` /
  `IProjectStore` (write-side, M3).
- **ADR-016 — registered-but-disabled tests.** The
  M2.5 axe-core tests and the M1
  provider-boundary tests are registered-but-disabled.
  The ADR says they activate in M4-D (the first
  concrete process providers milestone). M2 closes
  with the tests disabled; the activation is M4-D's
  responsibility.

## 7. Documentation Changes

The documents M2 added, modified, or deprecated.
The list is exhaustive of the M2 working tree.

### Added (M2)

- `.ai/plans/M2.1-application-shell-skeleton.md`
- `.ai/plans/M2.2-navigation-registry-sidebar.md`
- `.ai/plans/M2.3-topbar-breadcrumbs.md`
- `.ai/plans/M2.4-project-intelligence-dashboard.md`
- `.ai/plans/M2.5-empty-routes-responsive-accessibility.md`
- `.ai/plans/M2.6-m2-closeout-and-treehouse-dogfooding.md` (M2.6)
- `.ai/workflows/milestone-closeout.md` (M2.6; the
  Milestone Closeout Standard)
- `implementation-report-m2-1-application-shell-foundation.md`
- `implementation-report-m2-2-navigation-registry-sidebar.md`
- `implementation-report-m2-3-topbar-breadcrumbs.md`
- `implementation-report-m2-4-project-intelligence-dashboard.md`
- `implementation-report-m2-5-empty-routes-responsive-accessibility.md`
- `implementation-report-m2-6-m2-closeout.md` (M2.6)
- `retrospective-m2-application-shell-and-navigation.md`
  (M2.6; this file)
- `.ai/handoffs/2026-07-11-m2-1-application-shell-foundation.md`
- `.ai/handoffs/2026-07-11-m2-2-navigation-registry-sidebar.md`
- `.ai/handoffs/2026-07-11-m2-3-topbar-breadcrumbs.md`
- `.ai/handoffs/2026-07-11-m2-4-project-intelligence-dashboard.md`
- `.ai/handoffs/2026-07-11-m2-5-empty-routes-responsive-accessibility.md`
- `.ai/handoffs/2026-07-11-m2-6-m2-closeout.md` (M2.6)
- `reconciliation-report-m2-task-breakdown.md`

### Modified (M2 + M2.6)

- `ROADMAP.md` (M2 block in § 2 + § 3; M2 row
  status; M2.5 row; M2.6 row; M2 DoD bullets)
- `.ai/plans/master-delivery-plan.md` (M2 block in
  § 1 + § 3)
- `.ai/state/current.md` (active milestone, last
  completed task, application status, build / test
  status, active plan, last implementation report,
  next recommended task, last updated, linked
  artefacts)
- `.ai/state/task-board.md` (M2.1 → M2.5 in Done
  Recently; M2.6 in In Progress; M3 in Deferred;
  M1-FU-1 in Ready)
- `.ai/state/milestones.json` (M2 slices M2.1 →
  M2.6; M2 evidence block; M2 status: Active → Done
  in M2.6)
- `.ai/state/tasks.json` (T-001 → T-017 records)
- `.ai/state/session.json` (M2.6 closeout envelope)
- `.ai/README.md` (workflows directory listing;
  task-routing table)
- `CONTRIBUTING.md` (§ 3 Branching Model; replaced
  by reference to the new strategy in M2.6)
- `docs/ui-principles.md` (§ 10.1 responsive matrix
  in M2.5)
- `docs/dashboard.md` (the `branch` example
  re-keyed from `master` to `main` in M2.6
  infrastructure)

### Deprecated

- `AGENTS.md` does not deprecate any M2-era rule.
- The M1-era `feature-lifecycle.md` is the
  per-feature workflow; M2.6 introduces the
  Milestone Closeout Standard for the
  per-milifecycle concern. The two workflows
  complement each other; neither is deprecated.
- The M0.5-era `AGENTS.md` rule count was 17 (the
  M2 era keeps the 17-rule count; no rules are
  added or removed in M2).

## 8. Testing Summary

The canonical M2 test status. The numbers are the
evidence the M2.6 closeout re-validated.

| Test category   | Passed | Failed | Skipped | Total | Source                                                         |
| --------------- | ------ | ------ | ------- | ----- | -------------------------------------------------------------- |
| Unit            | 6      | 0      | 0       | 6     | `tests/AiEng.Platform.UnitTests/`                              |
| Component (bUnit)| 185   | 0      | 0       | 185   | `tests/AiEng.Platform.ComponentTests/`                         |
| Architecture    | 6      | 0      | 7       | 13    | `tests/AiEng.Platform.ArchitectureTests/`                      |
| **Total active** | **197** | **0** | **0** | **197** | The canonical active test count.                             |
| **Total skipped** | —     | —      | **7**   | —     | 3 axe-core (`AxeCoreAuditTests`) + 4 provider-boundary (`CompositionRootBoundaryTests`); both registered-but-disabled per ADR-016 / M4-D. |
| **Grand total**   | **197** | **0** | **7** | **204** | (active + skipped)                                            |

### New tests added by M2 slice

| Slice | New tests                                  | Notes                                                       |
| ----- | ------------------------------------------ | ----------------------------------------------------------- |
| M2.1  | 25 bUnit                                   | AppLayout, AppEmptyLayout, AppSidebarSlot, AppTopBarSlot, AppShellRegion |
| M2.2  | 28 bUnit + 1 architecture                  | RouteRegistry, AppSidebar, AppSidebarItem, AppNavItem; the `Pages_AreReachable_Through_Registry` architecture test |
| M2.3  | 27 bUnit (−6 obsolete)                     | AppTopBar, AppThemeToggle, AppUserAvatarSlot, AppBreadcrumb; the obsolete `AppTopBarSlotTests` removed |
| M2.4  | 6 unit + 13 bUnit + 2 architecture         | ProjectIntelligenceReader, composition root, Dashboard.razor; the `Pages_Resolve_State_Through_Reader` and `Dashboard_Resolves_State_Through_Reader` architecture tests |
| M2.5  | 18 bUnit + 3 architecture (skipped)        | EmptyRoutes, AppLayout_ThemeToggleWiring, AppLayout_ResponsiveMatrix, AriaCurrentInvariant, KeyboardSmoke; the `AxeCoreAuditTests` registered-but-disabled |
| M2.6  | 0                                          | M2.6 is docs + workflow; no source code or test changes     |

### Removed tests

- 6 `AppTopBarSlotTests` removed in M2.3 (the
  placeholder component is replaced by
  `AppTopBar`).

## 9. Validation Results

The M2.6 closeout's validation gate, executed
end-to-end on 2026-07-11.

| Gate                         | Command                                       | Result                                                  |
| ---------------------------- | --------------------------------------------- | ------------------------------------------------------- |
| CSS build                    | `npm run css:build`                           | Exits 0; `app.css` rebuilt in 449 ms.                    |
| Restore                      | `dotnet restore`                              | Exits 0; every project is up-to-date.                    |
| Build                        | `dotnet build`                                | Exits 0; 0 warnings, 0 errors (4.0 s).                   |
| Test                         | `dotnet test`                                 | 197 passed, 0 failed, 7 skipped (3 axe-core + 4 provider-boundary). |
| Format                       | `dotnet format --verify-no-changes`           | Exits 0; format is clean.                                |
| Visual smoke                 | 5 routes hit on `localhost:5211`              | `GET /` 200; `GET /dashboard` 200; `GET /design-system` 200; `GET /counter` 200; `GET /weather` 200. |
| Theme toggle markup presence | 4 `AppLayout` pages checked                   | Theme toggle markup is present on every `AppLayout` page. |

## 10. Implementation Reports

The M2.6 closeout's evidence: the per-slice
implementation reports the M2 sessions shipped.

- `implementation-report-m2-1-application-shell-foundation.md`
- `implementation-report-m2-2-navigation-registry-sidebar.md`
- `implementation-report-m2-3-topbar-breadcrumbs.md`
- `implementation-report-m2-4-project-intelligence-dashboard.md`
- `implementation-report-m2-5-empty-routes-responsive-accessibility.md`
- `implementation-report-m2-6-m2-closeout.md` (M2.6)

## 11. Commit Range

The M2 commit range is the Git range from the M0.5
closeout (the previous milestone) to the M2.5
closeout (the last slice before M2.6).

- **First commit (after M0.5):** `ba6c1e8` —
  `docs(m2): reconcile M2 task breakdown into six
  non-overlapping slices` (the M2 delivery
  reconciliation).
- **Last commit (M2.5 closeout):** `1614f93` —
  `feat(m2.5): add empty routes, responsive matrix,
  a11y audit, and theme toggle fix`.
- **M2.6 closeout commit (on the feature branch):**
  the M2.6 closeout's coherent commit
  (`chore(m2.6): close M2 with retrospective, milestone
  closeout standard, and M3 plan`).
- **M2 closeout commit (on `main`):** the M2.6 commit
  fast-forwarded into `main` at the M2.6 closeout.
- **M2 milestone tag:** `m2` (annotated; the
  Milestone Closeout Standard's `m2` tag at the M2
  closeout commit on `main`).

The 11 commits in the M2.1 → M2.5 range are listed
in § 9 of this retrospective under the per-slice
implementation reports.

## 12. Readiness for M3

M3 (Project Registration) is the next milestone
per `ROADMAP.md` and the master delivery plan. M2
is **structurally and procedurally ready** for M3:

- **Capabilities the M3 plan consumes** are
  delivered by M2: the application shell
  (`AppLayout`, `AppSidebar`, `AppTopBar`), the
  navigation registry (`INavigationRegistry`,
  `RouteMetadata`), the design system (M1), the
  project intelligence reader (M2.4). M3 composes
  these without re-implementing them.
- **The M3 plan** (`.ai/plans/M3-project-registration.md`)
  is in `Awaiting Approval` at the end of the M2.6
  closeout.
- **The first M3 task** is `Ready` in
  `tasks.json`. The first M3 task is the one the
  M3 plan's acceptance criteria name; the
  promotion is the M2.6 closeout's last step.
- **The M2 closeout's evidence** (the per-slice
  implementation reports, the handoffs, the
  retrospective) is in place. M3's first session
  reads the M2.6 handoff and this retrospective
  first.
- **The M2 milestone tag** (`m2`) is at the M2
  closeout commit on `main`. M3 starts from a
  clean baseline.

The M3 plan fleshes out the M3 deliverable surface
(`AppProjectCard`, `AppProjectList`,
`IProjectService`, `IProjectStore` — the in-memory
implementation; the on-disk implementation is
M4-A's responsibility). The M3 plan is at
`.ai/plans/M3-project-registration.md`.

## 13. Recommendations for the Next Milestone

Concrete recommendations the M3 plan should
account for. Each recommendation cites the M2
evidence that motivates it.

1. **M3 should follow the M2.6 closeout pattern
   at its own closeout.** M3's M3.x slices (the
   plan will name them) each ship a single coherent
   commit; M3's own closeout slice is the M3
   retrospective at
   `retrospective-m3-project-registration.md`. The
   Milestone Closeout Standard (`.ai/workflows/milestone-closeout.md`)
   is the template.
2. **M3 should consume `IProjectIntelligenceReader`
   (C-022) for the dashboard and the new
   `IProjectService` for the project list.** The
   M2.4 reader is the read-side single seam; M3's
   `IProjectService` is the write-side seam. The
   M3 plan should not introduce a third read-side
   path; it should extend the M2.4 reader with
   project-specific fields if needed.
3. **M3 should introduce an architecture test
   analogous to `Pages_Resolve_State_Through_Reader`.**
   The M3 architecture test should enforce that
   the project-registration page resolves state
   through `IProjectService`, not through direct
   access to the in-memory `IProjectStore`. The
   pattern is the same: the test fails the build
   if a page bypasses the seam.
4. **M3 should ship an `AppProjectCard` design-
   system extension, not a new component.** The
   M2.5 lesson (no new components without three
   uses) generalises: the project card composes
   `AppCard` + `AppEmptyState` + a project-name
   `AppStack`. M3 does not introduce a new
   primitive; it composes existing ones.
5. **M3 should keep the in-memory store for
   now.** M4-A replaces the in-memory `IProjectStore`
   with the on-disk `IProjectStore`. M3's
   in-memory store is the smoke test for the
   contract; the durable storage is M4-A's
   responsibility.
6. **M3 should run `npm run css:build` and
   `dotnet format` in the validation gate.** M2.6
   established the milestone-level validation
   pattern; M3 inherits the pattern.
7. **M3 should add a per-slice implementation
   report and a per-slice handoff.** The M2
   pattern is the canonical M3 pattern.
8. **M3 should not begin the M4 work.** M3 is a
   boundary; the M4-A work begins in a separate
   session after M3's closeout.

The M3 plan
(`.ai/plans/M3-project-registration.md`) accounts
for these recommendations. M3's first session
reads the M2.6 handoff + this retrospective first.
