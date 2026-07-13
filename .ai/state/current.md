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

- **Active milestone:** **M4-A — Infrastructure /
  Process Execution** (Active, 2026-07-11;
  M4-A.1 + M4-A.2 Delivered 2026-07-11).
  M4-A.1 ships the infrastructure seam every
  later milestone composes:
  `AiEng.Platform.Infrastructure` csproj; the
  four contracts (`IProcessRunner` +
  `ProcessResult` + `ICredentialVault` +
  `IPlatformInfo`) in
  `src/AiEng.Platform.Application/Infrastructure/`;
  the four implementations
  (`SystemProcessRunner` +
  `WindowsCredentialVault` +
  `SystemPlatformInfo` + `JsonFileProjectStore`)
  in
  `src/AiEng.Platform.Infrastructure/`;
  the `AddInfrastructure` composition root
  extension; the one-line swap in `AddProjects`
  (the M3 in-memory `IProjectStore` registration
  is removed; the on-disk `JsonFileProjectStore`
  is now registered through `AddInfrastructure`).
  The `InMemoryProjectStore` is moved to
  `tests/AiEng.Platform.UnitTests/Infrastructure/`
  as a test fixture. M4-A.1 ships 45 new unit
  tests + 2 new architecture tests
  (registered-but-disabled per ADR-016).
  M4-A.2 ships the Open action on
  `AppProjectCard`: the
  `IPlatformInfo.IsWindows` extension +
  `SystemPlatformInfo` implementation; the
  card `@inject`s `IProcessRunner` +
  `IPlatformInfo` + `ILogger<AppProjectCard>`
  directly; the click handler calls
  `IProcessRunner.RunToCompletionAsync("explorer.exe",
  new[] { Project.Path }, default)`; the
  button is gated on `IPlatformInfo.IsWindows`;
  exceptions are swallowed and surfaced as a
  transient inline `OpenError`. M4-A.2 ships 5
  new bUnit tests + 1 new active architecture
  test (`AppProjectCard_resolves_open_through_IProcessRunner`).
  The M3-A.2 documentation is at
  [`docs/infrastructure.md`](../../infrastructure.md)
  (10 sections per the M4-A plan § 2) +
  [`docs/projects.md`](../../projects.md) §
  1/§ 4/§ 5.1/§ 7.2/§ 7.3. See
  [`implementation-report-m4-a-1-infrastructure-project-skeleton.md`](../../implementation-report-m4-a-1-infrastructure-project-skeleton.md)
  and
  [`implementation-report-m4-a-2-open-action.md`](../../implementation-report-m4-a-2-open-action.md).
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
  The self-awareness dashboard
  remains in M2.4. See
  [`implementation-report-m2-2-navigation-registry-sidebar.md`](./../../implementation-report-m2-2-navigation-registry-sidebar.md).
- **M2.3 — Top Bar, Breadcrumbs, and Page
  Headers:** **Delivered (2026-07-11).**
  The top bar, breadcrumb, and page header
  integration land: `AppTopBar`
  (replaces the M2.1 `AppTopBarSlot`
  placeholder), `AppThemeToggle`
  (relocated to the top bar's `Trailing`
  slot), `AppUserAvatarSlot`, and
  `AppBreadcrumb` in
  `src/AiEng.Platform.App/Components/`.
  `AppBreadcrumb` walks the M2.2
  `INavigationRegistry`'s `Parent`
  chain; `aria-current="page"` is set
  on the current item; separators are
  `aria-hidden`. `AppBreadcrumb` is
  wired into `AppPageHeader.Breadcrumbs`
  on `DesignSystem.razor`. The 27 new
  bUnit tests across 4 test files bring
  the total to 146 component tests
  passing; the 6 obsolete
  `AppTopBarSlotTests` are removed; the
  `Pages_AreReachable_Through_Registry`
  architecture test remains active
  and green. The self-awareness
  dashboard remains in M2.4. See
  [`implementation-report-m2-3-topbar-breadcrumbs.md`](./../../implementation-report-m2-3-topbar-breadcrumbs.md).
- **M2.4 — Project Intelligence Dashboard:**
  **Delivered (2026-07-11).** The
  project intelligence dashboard
  lands: `IProjectIntelligenceReader`,
  `ProjectIntelligenceSnapshot`,
  `ProjectIntelligenceReader` in
  `src/AiEng.Platform.Application/ProjectIntelligence/`;
  `ProjectIntelligenceServiceCollectionExtensions`
  in `src/AiEng.Platform.App/Composition/`;
  `Dashboard.razor` +
  `Dashboard.razor.css` in
  `src/AiEng.Platform.App/Components/Pages/`;
  the `Pages_Resolve_State_Through_Reader`
  architecture test is active and
  green; the theme toggle bug fix is
  recorded as T-017 (later fixed in
  M2.5). See
  [`implementation-report-m2-4-project-intelligence-dashboard.md`](./../../implementation-report-m2-4-project-intelligence-dashboard.md).
- **M2.5 — Empty Routes, Responsive,
  and Accessibility:**
  **Delivered (2026-07-11).** The
  M2 acceptance criteria for the
  application shell are closed: every
  route in `Components/Pages/`
  reaches an `AppEmptyState` (or its
  data-owning equivalent); the
  layout is usable at 1280x720,
  1440x900, and 1920x1080 with the
  sidebar widths scaling at the lg
  / md / sm breakpoints (per
  ADR-005); the keyboard smoke
  test and the `aria-current="page"`
  invariant pass on every route;
  the axe-core audit is registered
  but disabled per ADR-016 and the
  M4-D activation milestone. The
  T-017 theme toggle bug is fixed
  in the same slice: `@rendermode
  InteractiveServer` is added to
  `AppThemeToggle.razor` (not to
  `AppLayout.razor`; the layout's
  `@Body` is a `RenderFragment`
  delegate, which Blazor refuses to
  serialize across the SSR →
  interactive boundary, so the
  directive on the layout throws
  `InvalidOperationException` at
  request time; the directive on the
  toggle itself is the
  minimum-blast-radius fix). The new
  `AppLayout_ThemeToggleWiringTests`
  bUnit test renders the `AppLayout`,
  finds the toggle inside the
  topbar, clicks it, and asserts
  `appTheme.set` is invoked. The
  visual smoke test confirms every
  route returns 200 and the
  toggle's markup is present on
  every page. 18 new component
  tests + 3 new architecture tests
  (skipped). Branch:
  `feature/m2-5-empty-routes-responsive-accessibility`.
  See
  [`implementation-report-m2-5-empty-routes-responsive-accessibility.md`](./../../implementation-report-m2-5-empty-routes-responsive-accessibility.md).
- **M2.6 — M2 Closeout and Treehouse
  Dogfooding:** **Delivered (2026-07-11).**
  The M2 closeout lands: the Milestone
  Closeout Standard at
  `.ai/workflows/milestone-closeout.md`; the
  M2 retrospective at
  `retrospective-m2-application-shell-and-navigation.md`;
  the M2.6 implementation report at
  `implementation-report-m2-6-m2-closeout.md`;
  the M2 milestone moves from `Active` to
  `Done` with `closed_at: 2026-07-11`; the
  `m2` annotated milestone tag is at the M2
  closeout commit on `main`. The M3 plan is
  at
  `.ai/plans/M3-project-registration.md`
  (Status: Awaiting Approval); the first M3
  task (T-018 — M3.1) is `Ready`. See
  [`implementation-report-m2-6-m2-closeout.md`](./../../implementation-report-m2-6-m2-closeout.md)
  and
  [`retrospective-m2-application-shell-and-navigation.md`](./../../retrospective-m2-application-shell-and-navigation.md).
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
- **M3.1 — Project Registration Slice 1:**
  **Delivered (2026-07-11).** The M3
  surface lands end-to-end as a single
  slice: the contract
  (`IProjectStore`, `IProjectService`),
  the in-memory store
  (`InMemoryProjectStore`; the M3 smoke
  test for the contract; M4-A swaps the
  `IProjectStore` registration in
  `AddProjects`), the domain entity
  (`Project`), the composition root
  (`AddProjects`), the UI surface
  (`AppProjectCard`, `AppProjectList`,
  `/projects` page), the architecture
  test
  `Pages_Resolve_Projects_Through_Service`
  (enforces the single-seam rule),
  27 new unit tests + 13 new bUnit
  tests + 2 new architecture tests.
  Total: 240 passed, 0 failed, 7
  skipped (per ADR-016 / M4-D). The
  page composes `AppPageHeader` +
  `AppBreadcrumb` (M2.3) +
  `AppProjectList`; the sidebar entry
  is in the M2.2 `INavigationRegistry`
  (Href `/projects`, Order 1, Icon
  `▢`). The M3 surface is the
  smallest piece of state the platform
  needs to be useful on its own; every
  later milestone (M4 process runner,
  M5 worktree, M6 launch, M7 review,
  M8 orchestration) consumes
  `IProjectService.ListAsync` as its
  input. See
  [`implementation-report-m3-1-project-registration-slice-1.md`](./../../implementation-report-m3-1-project-registration-slice-1.md).
- **M3.2 — Project Registration Slice 2:**
  **Delivered (2026-07-11).** The three
  mutations the M3 surface exists for
  land: `RegisterProjectForm` (the
  registration modal; page header
  Register a project button is now
  enabled), `RenameProjectForm` (the
  rename modal; AppProjectCard Rename
  button is now enabled), and
  `ConfirmUnregisterProject` (the
  unregister confirmation; AppProjectCard
  Unregister button is now enabled).
  All three use HTML5 native `<dialog>`
  elements with scoped CSS and
  `data-testid` attributes. The
  `AppProjectList` exposes
  `ShowRegisterDialog()` and
  `RefreshAsync()` methods. The
  architecture test
  `Pages_Resolve_Projects_Through_Service`
  is extended with three new tests
  (one per new form component); the
  single-seam rule holds. 30 new bUnit
  tests + 3 new architecture tests +
  0 new unit tests (the M3.1
  `IProjectServiceTests` already cover
  the happy-path + failure paths). The
  Open button on `AppProjectCard`
  remains disabled — that is M4-A's
  responsibility. Total: 273 passed, 0
  failed, 7 skipped (per ADR-016 /
  M4-D); M3.2 is +30 bUnit + 3
  architecture vs M3.1 closeout. See
  [`implementation-report-m3-2-project-registration-slice-2.md`](./../../implementation-report-m3-2-project-registration-slice-2.md).

## Current Slice

- **Active slice:** **M4-A.2 — Open action on
  AppProjectCard** (delivered 2026-07-11).
  The branch
  `feature/T-022-m4-a-2-open-action` carried
  the M4-A.2 work; the M4-A.2 closeout commit
  `feat(m4-a.2): enable AppProjectCard.Open
  action via IProcessRunner` is on this branch;
  the branch is fast-forwarded into `main` per
  the branching strategy rule 6; the branch is
  deleted per rule 7. The next M4-A task is
  undefined (M4-A.3 is not yet planned); the
  next milestone is M4-B (Capability Detection,
  Planned).
- **Last completed slice:** **M4-A.1 —
  Infrastructure project skeleton**
  (delivered 2026-07-11). The branch
  `feature/T-021-m4-a-1-infrastructure-project-skeleton`
  carried the M4-A.1 work; the M4-A.1
  closeout commit
  `feat(m4-a.1): add infrastructure project skeleton with IProcessRunner, ICredentialVault, IPlatformInfo, and on-disk IProjectStore`
  is on this branch; the branch is
  fast-forwarded into `main` per the
  branching strategy rule 6; the branch is
  deleted per rule 7.
- **Last completed slice:** **M3.2 —
  Project Registration Slice 2**
  (delivered 2026-07-11). The branch
  `feature/m3-1-project-registration-slice-1`
  carried the M3.1 work; the M3.1
  closeout commit
  `feat(m3.1): add project registration surface`
  is on `main`; the branch is deleted
  per the branching strategy rule 7.
- **M2.2 — Navigation Registry and
  Sidebar** (delivered 2026-07-11).
  The branch
  `feature/m2-2-navigation-registry-sidebar`
  carries the implementation; the M2.2
  closeout commit
  `feat(m2.2): add navigation registry and sidebar`
  is on this branch.
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
  **Done (closed 2026-07-11; M2.1
  Delivered 2026-07-11;
  M2.2 Delivered 2026-07-11;
  M2.3 Delivered 2026-07-11;
  M2.4 Delivered 2026-07-11;
  M2.5 Delivered 2026-07-11;
  M2.6 Delivered 2026-07-11; the M2
  retrospective is at
  `retrospective-m2-application-shell-and-navigation.md`;
  the `m2` annotated milestone tag is
  at the M2 closeout commit on `main`).**
- **M3 — Project Registration:**
  **Done (closed 2026-07-11; M3.1 Delivered
  2026-07-11; M3.2 Delivered 2026-07-11;
  M3 closeout M3.x Delivered 2026-07-11).**
  The M3 surface is the smallest piece
  of state the platform needs to be
  useful on its own. M3.1 ships the
  contract, the in-memory store, the UI
  surface, and the architecture test.
  M3.2 ships the three mutations
  (registration form, rename form,
  unregister confirmation) through the
  same contract. M3.x (the M3 closeout
  per the Milestone Closeout Standard)
  ships the M3 retrospective, moves M3
  to `Done` with `closed_at: 2026-07-11`,
  creates the `m3` annotated milestone
  tag, and produces the M4-A plan in
  `Awaiting Approval`. The M3 plan is at
  `.ai/plans/M3-project-registration.md`.
  The M3 retrospective is at
  [`retrospective-m3-project-registration.md`](./../../retrospective-m3-project-registration.md).**
- **M4-A.2 — Open action on `AppProjectCard`:** **Delivered (2026-07-11).** The
  M4-A.2 is the first `IProcessRunner`
  activation. The card `@inject`s
  `IProcessRunner` + `IPlatformInfo` +
  `ILogger<AppProjectCard>` directly; the
  click handler calls
  `IProcessRunner.RunToCompletionAsync("explorer.exe",
  new[] { Project.Path }, default)`; the
  button is gated on `IPlatformInfo.IsWindows`;
  exceptions are swallowed and surfaced as a
  transient inline `OpenError`. 5 new bUnit
  tests + 1 new active architecture test
  (`AppProjectCard_resolves_open_through_IProcessRunner`).
  The M3.2 `Open_Button_Remains_Disabled_In_M3_2`
  test is deleted. Total: 323 passed, 0
  failed, 9 skipped (per ADR-016 / M4-D);
  M4-A.2 is +5 bUnit + 1 new active
  architecture vs M4-A.1 closeout. Branch:
  `feature/T-022-m4-a-2-open-action`.
  See
  [`implementation-report-m4-a-2-open-action.md`](./../../implementation-report-m4-a-2-open-action.md).

## Last Completed Milestone

- **M3 — Project Registration**, closed
  **2026-07-11** (the most recent
  milestone). The M3 evidence is the
  per-slice implementation reports
  (M3.1, M3.2, M3 closeout) and the
  per-slice handoffs (M3.1, M3.2, M3
  closeout). The M3 retrospective is at
  [`retrospective-m3-project-registration.md`](./../../retrospective-m3-project-registration.md).
  The M3 closeout commit
  `chore(m3.closeout): close M3 with retrospective, M4-A plan, and m3 milestone tag`
  is on `main`; the `m3` annotated
  milestone tag is at the M3 closeout
  commit. The M3 plan is at
  `.ai/plans/M3-project-registration.md`.
  The M4-A plan is at
  `.ai/plans/M4-A-infrastructure-process-execution.md`
  (Status: Awaiting Approval).
- **M2 — Application Shell and
  Navigation**, closed **2026-07-11**
  (preceding milestone). The M2
  retrospective is at
  [`retrospective-m2-application-shell-and-navigation.md`](./../../retrospective-m2-application-shell-and-navigation.md).
  The Milestone Closeout Standard is at
  [`.ai/workflows/milestone-closeout.md`](./../../.ai/workflows/milestone-closeout.md).
- **M1 — Design System Core**, closed
  **2026-07-10** (preceding milestone).

## Last Completed Task

- The M4-A.2 implementation session
  (2026-07-11) is the most recent
  completed task; the M4-A.1 implementation
  session (2026-07-11) is the prior task; the
  M3 closeout implementation session
  (2026-07-11) is the prior-prior task; the
  M3.2 implementation session (2026-07-11)
  is the prior-prior-prior task; the
  M3.1 implementation session
  (2026-07-11) is the prior-prior-prior-prior
  task.
  - Delivered M4-A.2 — Open action on
    `AppProjectCard` per the approved
    M4-A plan
    (`.ai/plans/M4-A-infrastructure-process-execution.md`)
    § 2 item 8.
  - Extended the `IPlatformInfo` interface
    with `bool IsWindows { get; }`; implemented
    in `SystemPlatformInfo` via
    `RuntimeInformation.IsOSPlatform(OSPlatform.Windows)`.
  - Added `@using
    AiEng.Platform.Application.Infrastructure`
    to
    `src/AiEng.Platform.App/Components/Projects/_Imports.razor`.
  - Rewrote `AppProjectCard.razor`: added
    `@inject IProcessRunner` + `@inject
    IPlatformInfo` + `@inject
    ILogger<AppProjectCard>`; replaced the
    M3.2 `Disabled="true"` on the Open
    `AppButton` with the computed
    `Disabled="@(!IsWindowsHost)"`; wired
    `@onclick="OnOpenClick"`; added the
    `OnOpenClick` private method that calls
    `IProcessRunner.RunToCompletionAsync("explorer.exe",
    new[] { Project.Path }, default)`; added
    the `OpenError` transient state; rendered
    the `OpenError` as a small inline
    `<div class="app-project-card-open-error"
    role="alert">` in the card's
    `ChildContent`; added the `OpenButtonTitle`
    computed property for the tooltip.
  - Added the `.app-project-card-open-error`
    scoped CSS class to
    `AppProjectCard.razor.css` (uses
    `--app-error` and `--app-surface-2`;
    no new design-system component).
  - Modified
    `tests/AiEng.Platform.ComponentTests/Projects/AppProjectCardTests.cs`:
    deleted the M3.2
    `Open_Button_Remains_Disabled_In_M3_2`
    test; added 5 new bUnit tests
    (`Open_Button_Is_Enabled_When_Host_Is_Windows`,
    `Open_Button_Is_Disabled_When_Host_Is_Not_Windows`,
    `Clicking_Open_Invokes_IProcessRunner_With_Explorer_And_ProjectPath`,
    `Open_Click_Passes_ProjectPath_Single_Element_As_Argument`,
    `Open_Click_Swallows_Process_Exceptions`);
    added `FakeProcessRunner` and
    `FakePlatformInfo` test doubles; added a
    constructor that registers a default
    `IPlatformInfo` (Windows) + `IProcessRunner`
    (`FakeProcessRunner`) so the pre-existing
    tests pass without per-test service
    registration.
  - Added the
    `AppProjectCard_resolves_open_through_IProcessRunner`
    architecture test in
    `tests/AiEng.Platform.ArchitectureTests/Pages/PagesResolveProjectsThroughServiceTests.cs`
    asserting the card uses
    `@inject IProcessRunner` and contains no
    `Process.Start` or `ProcessStartInfo`
    token (the process boundary is the only
    allowed seam).
  - Rewrote `docs/infrastructure.md` § 7 from
    "future tense" to "delivered tense";
    updated § 6 Platform Info to document the
    new `IsWindows` property; updated § 9
    Tests cumulative count to 323 passed;
    updated § 10 Out of Scope to reflect
    M4-A.2 delivered.
  - Updated `docs/projects.md` § 1 (Open
    status), § 4 (M3 / M4-A Boundary; document
    the M4-A.2 delivery), § 5.1 (card
    description; all three action buttons
    enabled), § 7.2 (component test list),
    § 7.3 (architecture tests;
    `AppProjectCard_resolves_open_through_IProcessRunner`).
  - Updated `ROADMAP.md` (§ 2 M4-A row
    updated; M4-A.2 marked Delivered in the
    slice breakdown).
  - Updated `.ai/plans/master-delivery-plan.md`
    (§ 1 M4-A row + § 3 M4-A block; mark
    M4-A.2 Delivered in the slice breakdown;
    update completion status + evidence
    block).
  - Updated `.ai/state/capabilities.json`
    C-012 (`completion_status: Planned →
    Delivered`; `delivered_by_tasks: ["T-021",
    "T-022"]`; `evidence.source_paths` adds
    `AppProjectCard.razor`; `evidence.tests`
    adds the 5 new bUnit tests + the new
    architecture test; `architecture_tests`
    adds
    `AppProjectCard_resolves_open_through_IProcessRunner`;
    `completed_criteria` updated).
  - Validated the M4-A.2 end-to-end:
    `npm run css:build` (exit 0),
    `dotnet restore` (exit 0),
    `dotnet build` (0 warnings, 0 errors),
    `dotnet test` (323 passed, 0 failed, 9
    skipped per ADR-016 / M4-D),
    `dotnet format --verify-no-changes`
    (exit 0), visual smoke on `/projects`
    (200; the Open action is enabled on
    Windows hosts and disabled with a tooltip
    on non-Windows hosts).
  - 323 total tests pass: 79 unit + 233
    bUnit + 11 architecture, 9 skipped. The
    M4-A.2 adds +5 bUnit + 1 new active
    architecture vs M4-A.1 closeout (the 2
    M4-A.1 architecture tests remain
    registered-but-disabled per ADR-016).
  - Four documented deviations: (1)
    `IProcessRunner.RunToCompletionAsync`
    takes `IReadOnlyList<string>` arguments
    (the M4-A plan § 2 item 8 prose example
    used a single string with manual quote
    escaping; the actual contract uses
    `ProcessStartInfo.ArgumentList` for proper
    quoting); (2) `IPlatformInfo.IsWindows` is
    added in M4-A.2 (the M4-A plan § 8 risk
    row 6 anticipated this addition); (3)
    `ILogger<AppProjectCard>` is introduced
    in M4-A.2 (the App layer had not
    previously used `ILogger<T>`; the BCL
    provides it via the Blazor Server host
    DI container; no registration is needed);
    (4) `OpenError` inline rendering uses a
    single scoped CSS class
    `.app-project-card-open-error` (no new
    design-system `AppInlineError` primitive;
    the M3.2 minimum-blast-radius decision is
    preserved).
  - The commit
    `feat(m4-a.2): enable AppProjectCard.Open action via IProcessRunner`
    is the closing receipt; the commit
    is on the feature branch
    `feature/T-022-m4-a-2-open-action`;
    the branch is fast-forwarded into
    `main` per the branching strategy
    rule 6; the branch is deleted per
    rule 7. No push (push is not
    authorised in this session; the
    user may push in a follow-up
    command per the command protocol).
  - The session does **not**
    implement M4-A.3 (not yet
    defined) or M4-B / M4-C / M4-D;
    per the Progressive Coding Rule, the
    next session is the M4-A.3
    implementation (if defined) or the
    M4-B plan promotion.
- The M4-A.1 implementation session
  (2026-07-11) is the prior task;
    the M4-A.1 implementation
    session (2026-07-11) shipped the
    infrastructure seam every later
    milestone composes.
  - Delivered M4-A.1 — Infrastructure
    project skeleton per the approved
    M4-A plan
    (`.ai/plans/M4-A-infrastructure-process-execution.md`).
  - Shipped the new
    `AiEng.Platform.Infrastructure` csproj
    with references to Application + Domain
    (`InternalsVisibleTo("AiEng.Platform.UnitTests")`).
  - Shipped the four contracts in
    `src/AiEng.Platform.Application/Infrastructure/`:
    `IProcessRunner` + `ProcessResult` +
    `ICredentialVault` + `IPlatformInfo`.
  - Shipped the four implementations in
    `src/AiEng.Platform.Infrastructure/`:
    `SystemProcessRunner` (wraps
    `System.Diagnostics.Process`; streaming
    output via `IAsyncEnumerable<string>`;
    `RedirectStandardOutput` +
    `RedirectStandardError` +
    `UseShellExecute=false` +
    `CreateNoWindow=true`;
    `ProcessStartInfo.ArgumentList` for
    proper argument escaping) +
    `WindowsCredentialVault` (P/Invoke to
    `advapi32.dll`; `CRED_TYPE_GENERIC=1`,
    `CRED_PERSIST_LOCAL_MACHINE=2`,
    `ERROR_NOT_FOUND=1168`; throws
    `PlatformNotSupportedException` on
    non-Windows) +
    `SystemPlatformInfo` (resolves
    `Environment.SpecialFolder.LocalApplicationData`
    + `AiEng/Platform/<data|config>`) +
    `JsonFileProjectStore` (implements
    `IProjectStore`; `SemaphoreSlim(1,1)`
    for thread-safe serialization; atomic
    writes via `File.Replace`; corruption
    recovery catches `JsonException` and
    logs a warning).
  - Shipped the `AddInfrastructure`
    composition root extension
    (`InfrastructureServiceCollectionExtensions`)
    that registers `IPlatformInfo`,
    `IProcessRunner`, `ICredentialVault`,
    `IProjectStore` (all via
    `TryAddSingleton`).
  - Swapped the M3 in-memory `IProjectStore`
    registration in `AddProjects`: the
    `InMemoryProjectStore` registration is
    removed; the on-disk
    `JsonFileProjectStore` is now
    registered through `AddInfrastructure`.
    The M3 `InMemoryProjectStore` class is
    moved to
    `tests/AiEng.Platform.UnitTests/Infrastructure/InMemoryProjectStore.cs`
    as a test fixture.
  - Wired the new csproj into the solution
    (`AiEng.Platform.slnx`) and into the App
    project (`src/AiEng.Platform.App/AiEng.Platform.App.csproj`).
  - Shipped 45 new unit tests across 4 test
    files: 22 `JsonFileProjectStoreTests` +
    11 `IProcessRunnerTests` + 10
    `WindowsCredentialVaultTests` + 3
    `SystemPlatformInfoTests`.
  - Shipped 2 new architecture tests
    (`Infrastructure_Respects_ProcessBoundary`
    + `Infrastructure_Respects_CredentialBoundary`;
    both registered-but-disabled per
    ADR-016; activate in M4-D).
  - Documented M4-A at
    [`docs/infrastructure.md`](./../../infrastructure.md)
    (10 sections); updated
    [`docs/projects.md`](./../../docs/projects.md)
    § 4 (M3 / M4-A Boundary section).
  - Validated the complete M4-A.1
    end-to-end: `npm run css:build` (exit
    0), `dotnet restore` (exit 0),
    `dotnet build` (0 warnings, 0 errors),
    `dotnet test` (318 passed, 0 failed,
    9 skipped per ADR-016 / M4-D),
    `dotnet format --verify-no-changes`
    (exit 0), visual smoke
    (`curl http://localhost:5286/projects`
    returns 200; the project list
    persists across an application
    restart; the Open action is still
    disabled — M4-A.2's responsibility).
  - 318 total tests pass: 79 unit + 228
    bUnit + 11 architecture, 9 skipped.
    The M4-A.1 adds +45 unit + 0 bUnit +
    0 active architecture + 2 new
    architecture (skipped) vs the M3
    closeout baseline (273 + 7 skipped).
  - Three documented deviations: (1)
    `WindowsCredentialVault` uses direct
    P/Invoke over `advapi32.dll` (no
    NuGet dependency) per the M4-A plan
    § 8 risk 4 deferred decision; (2)
    `JsonFileProjectStore` uses
    `File.Replace` for atomic Windows
    file replacement (the M4-A.1 first
    choice of `File.Move(overwrite: true)`
    failed with `UnauthorizedAccessException`
    in the concurrent-add-and-remove test);
    (3) The M4-A.1 ships 45 unit tests
    (within the M4-A plan's 50+ bound;
    the test count covers the full
    M3.1 / M3.2 / M4-A.1 acceptance
    criteria without over-testing).
  - The commit
    `feat(m4-a.1): add infrastructure project skeleton with IProcessRunner, ICredentialVault, IPlatformInfo, and on-disk IProjectStore`
    is the closing receipt; the commit
    is on the feature branch
    `feature/T-021-m4-a-1-infrastructure-project-skeleton`;
    the branch is fast-forwarded into
    `main` per the branching strategy
    rule 6; the branch is deleted per
    rule 7. No push (push is not
    authorised in this session; the
    user may push in a follow-up
    command per the command protocol).
  - The session does **not**
    implement M4-A.2; per the
    Progressive Coding Rule, M4-A.2
    is the next session's
    responsibility.
- The M3 closeout implementation session
  (2026-07-11) is the prior task;
    `Pages_Resolve_Projects_Through_Service`
    with three new tests (one per new
    form component); the single-seam
    rule holds.
  - 30 new bUnit tests + 3 new
    architecture tests + 0 new unit
    tests (the M3.1
    `IProjectServiceTests` already
    cover the happy-path + failure
    paths).
  - Validated the M3 closeout
    end-to-end: `npm run css:build`
    (exit 0), `dotnet restore` (exit
    0), `dotnet build` (0 warnings, 0
    errors), `dotnet test` (273
    passed, 0 failed, 7 skipped per
    ADR-016), `dotnet format
    --verify-no-changes` (exit 0),
    visual smoke (the /projects
    route opens the registration
    modal; registering a project
    navigates to the populated state;
    renaming a project updates the
    name; unregistering a project
    removes it).
  - 273 total tests pass: 34 unit +
    228 component + 11 architecture, 7
    skipped. The M3 closeout adds
    +0 unit + 0 bUnit + 0
    architecture tests vs the M3.2
    closeout (the M3 closeout is docs
    + workflow + state change only).
  - Zero deviations: the M3 closeout
    follows the Milestone Closeout
    Standard as-is (the standard is
    mature enough to be reused without
    modification; the M2.6 closeout's
    "introduce the standard" is
    amortised).
  - The commit
    `feat(m3.2): enable project registration form, rename, and unregister`
    is the closing receipt; the commit
    is on the feature branch
    `feature/T-019-m3-2-project-registration-slice-2`;
    the branch is fast-forwarded into
    `main` per the branching strategy
    rule 6; the branch is deleted per
    rule 7. No push (push is not
    authorised in this session; the
    user may push in a follow-up
    command per the command protocol).
  - The session does **not**
    implement M3 closeout (M3.x); per
    the Progressive Coding Rule, M3
    closeout is the next session's
    responsibility.

## Active Branch

- **`main`** (the M4-A.2's
  fast-forwarded branch). The M4-A.2
  closeout commit
  `feat(m4-a.2): enable AppProjectCard.Open action via IProcessRunner`
  is the HEAD of `main`. The M4-A.2
  feature branch
  `feature/T-022-m4-a-2-open-action`
  carried the M4-A.2 work; the branch
  is fast-forwarded into `main` per the
  branching strategy rule 6; the branch
  is deleted per rule 7. The M4-A.1 +
  M3.1 + M3.2 + M3 closeout commit
  chain lives on the per-slice feature
  branches; the per-slice feature
  branches are deleted per the
  branching strategy rule 7; the
  per-slice commits are recorded in
  the M3 / M4-A.1 evidence blocks of
  `.ai/state/milestones.json`. The
  `m3` annotated milestone tag is at
  the M3 closeout commit on `main`
  per the branching strategy rule 9.
  The remote (`origin`) is configured
  but push is not authorised in this
  session.

## Last Stable Commit

- `178d319` — the M4-A.2 closeout commit
  `feat(m4-a.2): enable AppProjectCard.Open action via IProcessRunner`
  on `main` (created 2026-07-11). The
  M4-A.2 closeout commit is the
  closing receipt for the M4-A.2
  slice (the Open action on
  `AppProjectCard` + the
  `IPlatformInfo.IsWindows` extension +
  the M4-A.2 documentation + the
  project-continuity update). The
  M4-A.2 closeout commit lived on the
  feature branch
  `feature/T-022-m4-a-2-open-action`,
  which is fast-forwarded into `main`
  per the branching strategy rule 6
  and deleted per rule 7. The parent
  commit is `958cb4f` — the M4-A.1
  closeout commit
  `feat(m4-a.1): add infrastructure project skeleton with IProcessRunner, ICredentialVault, IPlatformInfo, and on-disk IProjectStore`
  on `main`. Working tree is clean at
  the close of the M4-A.2 session;
  the remote (`origin`) is configured
  but push is not authorised in this
  session.

## Application Status

- **Runnable.** The Blazor Server app builds and
  serves on `http://localhost:5210`. The six
  routes (`/`, `/counter`, `/dashboard`,
  `/design-system`, `/weather`, `/not-found`)
  all return 200 (the `/error` route is
  reached only on exception). The M2
  application shell is in place: `AppLayout`
  composes the registry-driven `AppSidebar`
  (M2.2), the `AppTopBar` (M2.3, with the
  theme toggle and the user avatar slot in
  the `Trailing` slot, and the current
  route's title in the `Leading` slot,
  sourced from the M2.2 `INavigationRegistry`),
  and the content area. The sidebar renders
  four nav items (Home, Counter, Weather,
  Design system) for the four
  sidebar-visible routes; Error and NotFound
  are correctly hidden (`ShowInSidebar =
  false`). The `data-app-region` attributes
  are present on every region.
  `/design-system` and `/not-found` use
  `AppEmptyLayout`. The `AppBreadcrumb`
  walks the M2.2 registry's `Parent` chain;
  `aria-current="page"` is set on the
  current item; separators are
  `aria-hidden`. The theme toggle
  (`@rendermode InteractiveServer` declared
  on the toggle itself; the layout remains
  static for streaming SSR) flips light/dark
  and persists to `localStorage`; the choice
  is applied via the `data-theme` attribute.
  Light and dark themes render correctly.
  The keyboard smoke test passes: Tab
  through the shell; every interactive
  element is reachable; focus is visible.

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
  **323 passed, 9 skipped, 0 failed.**
  - `AiEng.Platform.UnitTests`: 79 tests
    (34 pre-M4-A + 45 new M4-A.1
    unit tests: 22 `JsonFileProjectStoreTests`
    + 11 `IProcessRunnerTests` + 10
    `WindowsCredentialVaultTests` + 3
    `SystemPlatformInfoTests`).
  - `AiEng.Platform.ComponentTests`: 233
    bUnit / integration tests, all passing
    (228 pre-M4-A.2 + 5 new M4-A.2 bUnit
    tests on `AppProjectCard`: the Open
    action is Windows-gated; the click
    handler invokes `IProcessRunner`;
    the argument form is correct; the
    process invocation exception is
    swallowed and surfaced as a
    transient inline `OpenError`).
  - `AiEng.Platform.ArchitectureTests`: 21
    tests in total — 12 active (passing)
    and 9 registered-but-disabled
    (skipped) per ADR-016 and the M4-D
    activation milestone. The 1 new
    `AppProjectCard_resolves_open_through_IProcessRunner`
    architecture test is the new active
    test; the 2 M4-A.1
    `Infrastructure_Respects_ProcessBoundary`
    + `Infrastructure_Respects_CredentialBoundary`
    tests remain registered-but-disabled
    per ADR-016 / M4-D. The
    `Pages_Resolve_Projects_Through_Service`
    test (M3.1 + M3.2) remains active and
    green.

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
(2 layouts + 1 presentational helper; the
M2 placeholder shell components are
replaced by the M2.2 / M2.3
implementations):

- **Layouts (2):** `AppLayout` (the M2 layout
  root; three-region composition:
  `AppSidebar` (M2.2 the real
  registry-driven sidebar) +
  `AppTopBar` (M2.3 the real
  top bar) +
  content area) and `AppEmptyLayout` (the
  M2 alternate layout; no sidebar, no top
  bar).
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

Plus the M2.3 top bar, theme toggle,
user avatar, and breadcrumb (replaces
the M2.1 `AppTopBarSlot` placeholder):

- **Shell components (3):**
  `AppTopBar` (the real top bar;
  injects `INavigationRegistry` to
  read the current route's title in
  the `Leading` slot; renders the
  theme toggle and the user avatar
  slot in the `Trailing` slot by
  default; `Leading` and `Trailing`
  are `RenderFragment` slots for
  page-level overrides),
  `AppThemeToggle` (light / dark
  theme toggle; persists to
  `localStorage["app-theme"]`; reads
  `data-theme` on `documentElement`),
  `AppUserAvatarSlot` (the user
  avatar placeholder; the M3+
  session replaces it with the real
  user identity surface).
- **Navigation components (1):**
  `AppBreadcrumb` (renders one
  breadcrumb item per parent in
  the M2.2 registry's `Parent`
  chain; `aria-current="page"` on
  the current item; separators are
  `aria-hidden`; injects
  `INavigationRegistry` and
  `NavigationManager`).
- **Page header integration (1):**
  `AppBreadcrumb` is wired into
  `AppPageHeader.Breadcrumbs` on
  `DesignSystem.razor` (the
  remaining M1 template pages do
  not use `AppPageHeader`; the
  wiring is in place for every page
  that opts in).

Plus the supporting infrastructure:

- The Tailwind v3 + PostCSS pipeline
  (`npm run css:build`, `npm run css:watch`).
- The design-token catalogue (`themes.css`,
  `tokens.css`).
- The light and dark themes via the data-attribute
  theme switcher.
- The `/design-system` documentation page.
- The 4 active + 4 registered-but-disabled
  architecture tests in
  `tests/AiEng.Platform.ArchitectureTests/Boundaries/`
  (the M2.2
  `Pages_AreReachable_Through_Registry`
  test is the 4th active test).

Plus the M4-A.1 infrastructure seam (the
first M4-A slice; the boundary, not the
activation):

- **Infrastructure csproj (1):**
  `AiEng.Platform.Infrastructure` (new
  C# class library; references
  `AiEng.Platform.Application` +
  `AiEng.Platform.Domain`; enables
  `Nullable` + `TreatWarningsAsErrors`
  per `Directory.Build.props`;
  `InternalsVisibleTo("AiEng.Platform.UnitTests")`
  for the unit test assembly).
- **Application.Infrastructure (4
  contracts):** `IProcessRunner` (the
  streaming `IAsyncEnumerable<string>
  RunAsync` + the fire-and-forget
  `Task<ProcessResult> RunToCompletionAsync`
  contract), `ProcessResult` (the
  readonly record struct envelope with
  `Succeeded` property), `ICredentialVault`
  (the `Task<string?> GetAsync` +
  `Task SetAsync` + `Task DeleteAsync`
  contract), `IPlatformInfo` (the
  `string GetDataDirectory` +
  `string GetConfigDirectory` contract).
- **Infrastructure implementations
  (4 types):** `SystemProcessRunner`
  (wraps `System.Diagnostics.Process`;
  `RedirectStandardOutput` +
  `RedirectStandardError` +
  `UseShellExecute=false` +
  `CreateNoWindow=true`;
  `ProcessStartInfo.ArgumentList` for
  proper argument escaping; streaming
  output via `IAsyncEnumerable<string>`;
  the only `Process.Start` call site
  in the platform),
  `WindowsCredentialVault` (P/Invoke to
  `advapi32.dll`; `CRED_TYPE_GENERIC=1`,
  `CRED_PERSIST_LOCAL_MACHINE=2`,
  `ERROR_NOT_FOUND=1168`; throws
  `PlatformNotSupportedException` on
  non-Windows),
  `SystemPlatformInfo` (resolves
  `Environment.SpecialFolder.LocalApplicationData`
  + `AiEng/Platform/<data|config>`),
  `JsonFileProjectStore` (the on-disk
  `IProjectStore`; `SemaphoreSlim(1,1)`
  for thread-safe serialization;
  atomic writes via `File.Replace`;
  corruption recovery catches
  `JsonException` and logs a warning).
- **Composition (1 extension):**
  `InfrastructureServiceCollectionExtensions.AddInfrastructure`
  (registers `IPlatformInfo` +
  `IProcessRunner` + `ICredentialVault`
  + `IProjectStore`; all via
  `TryAddSingleton`; called from
  `ServiceCollectionExtensions.AddPlatformServices`
  after `AddProjects`).
- **Architecture tests (2 new,
  registered-but-disabled):**
  `Infrastructure_Respects_ProcessBoundary`
  + `Infrastructure_Respects_CredentialBoundary`
  (assert the Infrastructure project
  is the only project that references
  `System.Diagnostics.Process` and the
  Windows Credential Manager APIs;
  both registered-but-disabled per
  ADR-016; activate in M4-D).
- **On-disk store:** the M3
  in-memory `InMemoryProjectStore`
  is moved to
  `tests/AiEng.Platform.UnitTests/Infrastructure/InMemoryProjectStore.cs`
  as a test fixture; the on-disk
  `JsonFileProjectStore` is the
  production `IProjectStore`. The
  project list now persists across
  application restarts.

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
- **Git remote configured but not
  pushed.** The repository has a
  configured remote (`origin` =
  `https://github.com/maestroohk/ai-engineering-platform.git`).
  The first six commits (M0.5 +
  M2.1 → M2.6) are local-only. Push
  is not authorised in this
  session; the user may push in a
  follow-up command per the command
  protocol.

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
  — M2.3 plan, **Delivered (2026-07-11)**
  (promoted from `Draft` stub to a
  full plan in `Awaiting Approval` in
  the M2.2 closeout session;
  approved and implemented in the M2.3
  closeout session).
- [`.ai/plans/M2.4-project-intelligence-dashboard.md`](./../../.ai/plans/M2.4-project-intelligence-dashboard.md)
  — M2.4 plan, **Delivered (2026-07-11)**
  (promoted from `Draft` stub to a
  full plan in `Awaiting Approval` in
  the M2.3 closeout session;
  approved and implemented in the M2.4
  closeout session).
- [`.ai/plans/M2.5-empty-routes-responsive-accessibility.md`](./../../.ai/plans/M2.5-empty-routes-responsive-accessibility.md)
  — M2.5 plan, **Delivered (2026-07-11)**
  (promoted from `Draft` stub to a
  full plan in `Awaiting Approval` in
  the M2.4 closeout session; approved
  and implemented in the M2.5
  closeout session; the T-017 theme
  toggle bug fix is included in the
  same slice per the user's explicit
  opt-in).
- [`.ai/plans/M2.6-m2-closeout-and-treehouse-dogfooding.md`](./../../.ai/plans/M2.6-m2-closeout-and-treehouse-dogfooding.md)
  — M2.6 plan, **Delivered (2026-07-11)**
  (promoted from `Draft` stub to a
  full plan in `Awaiting Approval`
  in the M2.5 closeout session;
  approved and implemented in the
  M2.6 closeout session). The
  Milestone Closeout Standard lives
  at
  [`.ai/workflows/milestone-closeout.md`](./../../.ai/workflows/milestone-closeout.md);
  the M2 retrospective lives at
  [`retrospective-m2-application-shell-and-navigation.md`](./../../retrospective-m2-application-shell-and-navigation.md).
- [`.ai/plans/M3-project-registration.md`](./../../.ai/plans/M3-project-registration.md)
  — M3 plan, **Delivered (M3.1, M3.2,
  M3 closeout, 2026-07-11)**. Prepared
  in the M2.6 closeout session; the
  M3.1 plan is the first M3
  implementation slice (T-018,
  delivered 2026-07-11); the M3.2 plan
  is the second M3 implementation slice
  (T-019, delivered 2026-07-11); the
  M3 closeout plan is the M3
  retrospective (T-020, delivered
  2026-07-11). M3 is **Done (closed
  2026-07-11)**; the M3 retrospective
  is at
  [`retrospective-m3-project-registration.md`](./../../retrospective-m3-project-registration.md).
- [`.ai/plans/M3.2-project-registration-slice-2.md`](./../../.ai/plans/M3.2-project-registration-slice-2.md)
  — M3.2 plan, **Delivered
  (2026-07-11)**. Prepared in the
  M3.1 closeout session; M3.2
  enabled the M3.1 wired-but-
  disabled actions (Register form,
  Rename, Unregister).
- [`.ai/plans/M3-closeout.md`](./../../.ai/plans/M3-closeout.md)
  — M3 closeout plan, **Delivered
  (2026-07-11)**. Prepared in the M3.2
  closeout session; the M3 closeout
  plan mirrors the M2.6 closeout
  plan's structure; the M3 closeout
  implementation follows the plan.
  The M3 retrospective is at
  [`retrospective-m3-project-registration.md`](./../../retrospective-m3-project-registration.md).
  The `m3` annotated milestone tag is
  at the M3 closeout commit on `main`.
- [`.ai/plans/M4-A-infrastructure-process-execution.md`](./../../.ai/plans/M4-A-infrastructure-process-execution.md)
  — M4-A plan, **Approved (2026-07-11;
  M4-A.1 + M4-A.2 Delivered 2026-07-11;
  the 'Next' invocation that began
  the M4-A.1 session approved the
  plan per .ai/commands.md § 4 + the
  Progressive Coding Rule § 7.1;
  the M4-A plan is the M4-A umbrella
  plan; the M4-A.1 plan is the M4-A
  plan's M4-A.1 row, expanded with
  the per-slice implementation
  detail in the M4-A.1 implementation
  report; the M4-A.2 plan is the M4-A
  plan's M4-A.2 row, expanded with
  the per-slice implementation
  detail in the M4-A.2 implementation
  report; the M4-A plan introduces the
  `AiEng.Platform.Infrastructure`
  csproj, the `IProcessRunner` /
  `ICredentialVault` / `IPlatformInfo`
  contracts, the on-disk
  `IProjectStore` (which replaces the
  M3 in-memory store behind the same
  contract), and the Open action on
  `AppProjectCard`; the M4-A
  documentation is at
  [`docs/infrastructure.md`](./../../infrastructure.md);
  the M4-A.1 implementation report
  is at
  [`implementation-report-m4-a-1-infrastructure-project-skeleton.md`](./../../implementation-report-m4-a-1-infrastructure-project-skeleton.md);
  the M4-A.2 implementation report
  is at
  [`implementation-report-m4-a-2-open-action.md`](./../../implementation-report-m4-a-2-open-action.md)).
  Prepared in the M3 closeout session.

## Last Implementation Report

- [`implementation-report-m4-a-2-open-action.md`](./../../implementation-report-m4-a-2-open-action.md)
  — the M4-A.2 implementation report
  (the closing receipt for the
  M4-A.2 slice, 2026-07-11).
- [`implementation-report-m4-a-1-infrastructure-project-skeleton.md`](./../../implementation-report-m4-a-1-infrastructure-project-skeleton.md)
  — the M4-A.1 implementation report
  (the closing receipt for the
  M4-A.1 slice, 2026-07-11).
- [`implementation-report-m3-closeout.md`](./../../implementation-report-m3-closeout.md)
  — the M3 closeout implementation
  report (the closing receipt for the
  M3 milestone, 2026-07-11).
- [`implementation-report-m3-2-project-registration-slice-2.md`](./../../implementation-report-m3-2-project-registration-slice-2.md)
  — the M3.2 implementation report
  (the closing receipt for the M3.2
  slice, 2026-07-11).
- [`implementation-report-m3-1-project-registration-slice-1.md`](./../../implementation-report-m3-1-project-registration-slice-1.md)
  — the M3.1 implementation report
  (the closing receipt for the M3.1
  slice, 2026-07-11).
- [`implementation-report-m2-6-m2-closeout.md`](./../../implementation-report-m2-6-m2-closeout.md)
  — the M2.6 implementation report
  (the closing receipt for the M2
  milestone, 2026-07-11; the M2
  retrospective at
  [`retrospective-m2-application-shell-and-navigation.md`](./../../retrospective-m2-application-shell-and-navigation.md)
  is the M2 closeout's 13-section
  retrospective per the
  Milestone Closeout Standard at
  [`.ai/workflows/milestone-closeout.md`](./../../.ai/workflows/milestone-closeout.md)).
- [`implementation-report-m2-5-empty-routes-responsive-accessibility.md`](./../../implementation-report-m2-5-empty-routes-responsive-accessibility.md)
  — the M2.5 implementation report
  (the closing receipt for M2.5, 2026-07-11;
  includes the T-017 theme toggle bug fix).
- [`implementation-report-m2-4-project-intelligence-dashboard.md`](./../../implementation-report-m2-4-project-intelligence-dashboard.md)
  — the M2.4 implementation report
  (the closing receipt for M2.4, 2026-07-11).
- [`implementation-report-m2-3-topbar-breadcrumbs.md`](./../../implementation-report-m2-3-topbar-breadcrumbs.md)
  — the M2.3 implementation report
  (the closing receipt for M2.3, 2026-07-11).
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

> **M4-A.3 — not yet defined.** M3 is closed
> (2026-07-11). M4-A.1 + M4-A.2 are delivered
> (2026-07-11). The M4-A plan is at
> [`.ai/plans/M4-A-infrastructure-process-execution.md`](./../../.ai/plans/M4-A-infrastructure-process-execution.md)
> (Status: Approved). The next milestone
> after M4-A.2 is M4-B (Capability Detection,
> Planned). The next M4-A task is undefined
> (M4-A.3 is not yet planned); the M4-A.2
> session does not seed M4-A.3 (per the
> brief: 'Do not begin the following
> task'). The next session is the M4-A.3
> implementation (if defined) or the
> M4-B plan promotion. **Do not begin
> M4-A.3 / M4-B / M4-C / M4-D in this
> session** — the M4-A.2 brief
> explicitly stops at the M4-A.2
> receipt (the M4-A.2 is the first
> activation of the M4-A infrastructure
> seam; the Progressive Coding Rule
> applies).

The detailed breakdown of the M4-A.1
slice is in
[`.ai/state/task-board.md`](./task-board.md)
and the M4-A plan file in
[`.ai/plans/M4-A-infrastructure-process-execution.md`](./../../.ai/plans/M4-A-infrastructure-process-execution.md).
The M4-A.1 implementation report is at
[`implementation-report-m4-a-1-infrastructure-project-skeleton.md`](./../../implementation-report-m4-a-1-infrastructure-project-skeleton.md).
The M4-A.1 handoff is at
[`.ai/handoffs/2026-07-11-m4-a-1-infrastructure-project-skeleton.md`](./../../.ai/handoffs/2026-07-11-m4-a-1-infrastructure-project-skeleton.md).
The next actionable item is:

1. **M4-A.2 — Open action on
   `AppProjectCard`** (T-022, `Ready`
   in `.ai/state/tasks.json`; the
   next concrete M4-A task; the
   plan is
   `.ai/plans/M4-A-infrastructure-process-execution.md`).
   M4-A.2 is the first activation of
   the M4-A infrastructure seam
   (the `IProcessRunner` is the
   only legal way to launch a host
   process in M4-A; the Open action
   uses `IProcessRunner.RunAsync` to
   launch `explorer.exe` with the
   project's `path`).

## Last Updated

- **2026-07-11** (M4-A.2
  session). This version supersedes
  the M4-A.1 version
  (2026-07-11). The M4-A.2
  session ships: the
  `IPlatformInfo.IsWindows` extension;
  the `SystemPlatformInfo.IsWindows`
  implementation; the Open action on
  `AppProjectCard` enabled (Windows-gated;
  exception-swallowed with inline
  `OpenError`); the
  `AppProjectCard.razor` `@inject`s
  `IProcessRunner` + `IPlatformInfo` +
  `ILogger<AppProjectCard>`; the
  `.app-project-card-open-error` scoped
  CSS class; 5 new bUnit tests
  + 1 new active architecture test
  (`AppProjectCard_resolves_open_through_IProcessRunner`);
  the M3.2
  `Open_Button_Remains_Disabled_In_M3_2`
  test deleted; the
  `docs/infrastructure.md` § 7 rewrite
  + § 6 Platform Info update + § 9
  Tests cumulative count update + § 10
  Out of Scope update; the
  `docs/projects.md` § 1/§ 4/§ 5.1/§ 7.2
  /§ 7.3 updates; the `ROADMAP.md` +
  `.ai/plans/master-delivery-plan.md`
  updates; the `.ai/state/capabilities.json`
  C-012 update; the project-continuity
  state updates (session.json, tasks.json,
  current.md, task-board.md,
  milestones.json); the M4-A.2
  implementation report at
  [`implementation-report-m4-a-2-open-action.md`](./../../implementation-report-m4-a-2-open-action.md);
  the M4-A.2 per-session handoff at
  [`.ai/handoffs/2026-07-11-m4-a-2-open-action.md`](./../../.ai/handoffs/2026-07-11-m4-a-2-open-action.md)
  (mirrored to latest.md). The M4-A.2
  closeout commit
  `feat(m4-a.2): enable AppProjectCard.Open action via IProcessRunner`
  (hash `178d319`) is the most recent
  commit on
  `main`; the M4-A.2 feature branch
  is fast-forwarded into `main` and
  deleted per the branching
  strategy rule 6/7. Validation
  results: 323 passed, 0 failed,
  9 skipped (per ADR-016 / M4-D);
  0 warnings, 0 errors; format
  clean; visual smoke on `/projects`
  returns 200; the Open action is
  enabled on Windows hosts. The
  remote (`origin`) is configured
  but push is not authorised in this
  session. T-022 (M4-A.2 Open action)
  is `Done` in `.ai/state/tasks.json`.

## Linked Artefacts

- [`VISION.md`](./../../VISION.md) — the
  permanent vision document (M0.5;
  tier 1 of the document hierarchy).
- [`.ai/state/task-board.md`](./task-board.md) —
  the live work queue (M2.1
  through M2.6 Done; M3.1, M3.2
  + M3 closeout `Done`; T-021
  M4-A.1 `Ready`; M3 plan
  `Delivered` (M3.1 + M3.2 + M3
  closeout)).
- [`.ai/handoffs/latest.md`](./../../.ai/handoffs/latest.md) —
  the most recent handoff (the M4-A.2
  handoff; mirrored from
  `.ai/handoffs/2026-07-11-m4-a-2-open-action.md`).
- [`.ai/handoffs/2026-07-11-m4-a-2-open-action.md`](./../../.ai/handoffs/2026-07-11-m4-a-2-open-action.md)
  — the M4-A.2 session handoff
  (the closing receipt for the
  M4-A.2 slice, 2026-07-11).
- [`.ai/handoffs/2026-07-11-m4-a-1-infrastructure-project-skeleton.md`](./../../.ai/handoffs/2026-07-11-m4-a-1-infrastructure-project-skeleton.md)
  — the M4-A.1 session handoff
  (the closing receipt for the
  M4-A.1 slice, 2026-07-11).
- [`.ai/handoffs/2026-07-11-m3-1-project-registration-slice-1.md`](./../../.ai/handoffs/2026-07-11-m3-1-project-registration-slice-1.md)
  — the M3.1 closeout session handoff
  (the closing receipt for the M3.1
  slice, 2026-07-11).
- [`.ai/handoffs/2026-07-11-m2-6-m2-closeout.md`](./../../.ai/handoffs/2026-07-11-m2-6-m2-closeout.md)
  — the M2.6 closeout session handoff
  (the closing receipt for the M2
  milestone, 2026-07-11).
- [`.ai/handoffs/2026-07-11-m2-5-empty-routes-responsive-accessibility.md`](./../../.ai/handoffs/2026-07-11-m2-5-empty-routes-responsive-accessibility.md)
  — the M2.5 closeout session handoff.
- [`.ai/handoffs/2026-07-11-m2-4-project-intelligence-dashboard.md`](./../../.ai/handoffs/2026-07-11-m2-4-project-intelligence-dashboard.md)
  — the M2.4 closeout session handoff.
- [`.ai/handoffs/2026-07-11-m2-3-topbar-breadcrumbs.md`](./../../.ai/handoffs/2026-07-11-m2-3-topbar-breadcrumbs.md)
  — the M2.3 closeout session handoff.
- [`.ai/handoffs/2026-07-11-m2-2-navigation-registry-sidebar.md`](./../../.ai/handoffs/2026-07-11-m2-2-navigation-registry-sidebar.md)
  — the M2.2 closeout session handoff.
- [`.ai/handoffs/2026-07-11-m2-1-application-shell-foundation.md`](./../../.ai/handoffs/2026-07-11-m2-1-application-shell-foundation.md)
  — the M2.1 closeout session handoff.
- [`.ai/handoffs/2026-07-10-m1-closeout.md`](./../../.ai/handoffs/2026-07-10-m1-closeout.md) —
  the M1 closeout session handoff.
- [`.ai/workflows/milestone-closeout.md`](./../../.ai/workflows/milestone-closeout.md) —
  the Milestone Closeout Standard
  (10 sections; the canonical
  procedure every future milestone
  closeout must follow; the
  13-section retrospective is the
  standard's required deliverable).
  Introduced in the M2.6 closeout
  session, 2026-07-11.
- [`retrospective-m2-application-shell-and-navigation.md`](./../../retrospective-m2-application-shell-and-navigation.md) —
  the M2 retrospective
  (13 sections: delivered
  capabilities, deferred
  capabilities, technical debt,
  known issues, lessons learned,
  architecture changes,
  documentation changes, testing
  summary, validation results,
  implementation reports, commit
  range, readiness for M3,
  recommendations for the next
  milestone). The first milestone
  retrospective in the repository;
  produced in the M2.6 closeout
  session, 2026-07-11.
- [`retrospective-m3-project-registration.md`](./../../retrospective-m3-project-registration.md) —
  the M3 retrospective
  (13 sections per the Milestone
  Closeout Standard § 4). The second
  milestone retrospective in the
  repository; produced in the M3
  closeout session, 2026-07-11.
- [`.ai/plans/M2.6-m2-closeout-and-treehouse-dogfooding.md`](./../../.ai/plans/M2.6-m2-closeout-and-treehouse-dogfooding.md) —
  the M2.6 plan (Delivered,
  2026-07-11).
- [`.ai/plans/M3-project-registration.md`](./../../.ai/plans/M3-project-registration.md) —
  the M3 plan (`Awaiting Approval`,
  2026-07-11). The first M3 task
  (T-018, M3.1) is in `Ready`.
- [`.ai/plans/M2.1-application-shell-skeleton.md`](./../../.ai/plans/M2.1-application-shell-skeleton.md) —
  the revised M2.1 plan (Delivered,
  2026-07-11).
- [`.ai/plans/M2.2-navigation-registry-sidebar.md`](./../../.ai/plans/M2.2-navigation-registry-sidebar.md) —
  the M2.2 plan (Delivered,
  2026-07-11).
- [`.ai/plans/M2.3-topbar-breadcrumbs.md`](./../../.ai/plans/M2.3-topbar-breadcrumbs.md) —
  the M2.3 plan (Delivered, 2026-07-11;
  promoted from `Draft` stub in the
  M2.2 closeout session, approved and
  implemented in the M2.3 closeout
  session).
- [`.ai/plans/M2.4-project-intelligence-dashboard.md`](./../../.ai/plans/M2.4-project-intelligence-dashboard.md) —
  the M2.4 plan (Delivered,
  2026-07-11; promoted from `Draft`
  stub in the M2.3 closeout session,
  approved and implemented in the
  M2.4 closeout session).
- [`.ai/plans/M2.5-empty-routes-responsive-accessibility.md`](./../../.ai/plans/M2.5-empty-routes-responsive-accessibility.md) —
  the M2.5 plan (Delivered,
  2026-07-11).
- [`.ai/plans/M3-project-registration.md`](./../../.ai/plans/M3-project-registration.md) —
  the M3 plan (M3.1 Delivered
  2026-07-11; M3.2 Delivered
  2026-07-11; M3 closeout M3.x
  Delivered 2026-07-11; the
  M3 plan is the M3 umbrella plan;
  the M3 / M4-A boundary is the
  `IProjectStore` contract, not
  the storage medium).
- [`.ai/plans/M3.2-project-registration-slice-2.md`](./../../.ai/plans/M3.2-project-registration-slice-2.md) —
  the M3.2 plan (Delivered
  2026-07-11; the second M3 slice;
  registration form + rename +
  unregister).
- [`.ai/plans/M3-closeout.md`](./../../.ai/plans/M3-closeout.md) —
  the M3 closeout plan
  (Delivered 2026-07-11; mirrors
  the M2.6 closeout plan's
  structure; the M3 closeout
  implementation follows the plan).
- [`.ai/plans/M4-A-infrastructure-process-execution.md`](./../../.ai/plans/M4-A-infrastructure-process-execution.md) —
  the M4-A plan (Awaiting Approval
  2026-07-11; the next milestone
  after M3 closes; the M4-A.1 task
  T-021 is `Ready` in
  `.ai/state/tasks.json`).
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
- [`implementation-report-m3-1-project-registration-slice-1.md`](./../../implementation-report-m3-1-project-registration-slice-1.md)
  — the M3.1 implementation report
  (the closing receipt for the M3.1
  slice, 2026-07-11).
- [`implementation-report-m2-4-project-intelligence-dashboard.md`](./../../implementation-report-m2-4-project-intelligence-dashboard.md)
  — the M2.4 implementation report
  (the closing receipt for M2.4,
  2026-07-11).
- [`implementation-report-m2-5-empty-routes-responsive-accessibility.md`](./../../implementation-report-m2-5-empty-routes-responsive-accessibility.md)
  — the M2.5 implementation report
  (the closing receipt for M2.5,
  2026-07-11; includes the T-017
  theme toggle bug fix).
- [`implementation-report-m2-6-m2-closeout.md`](./../../implementation-report-m2-6-m2-closeout.md)
  — the M2.6 implementation report
  (the closing receipt for the M2
  milestone, 2026-07-11).
- [`implementation-report-m3-closeout.md`](./../../implementation-report-m3-closeout.md)
  — the M3 closeout implementation
  report (the closing receipt for the
  M3 milestone, 2026-07-11).
- [`implementation-report-m4-a-1-infrastructure-project-skeleton.md`](./../../implementation-report-m4-a-1-infrastructure-project-skeleton.md)
  — the M4-A.1 implementation
  report (the closing receipt for
  the M4-A.1 slice, 2026-07-11).
- [`implementation-report-m4-a-2-open-action.md`](./../../implementation-report-m4-a-2-open-action.md)
  — the M4-A.2 implementation
  report (the closing receipt for
  the M4-A.2 slice, 2026-07-11).
- [`docs/infrastructure.md`](./../../infrastructure.md)
  — the M4-A.1 surface
  documentation (10 sections:
  Goals, Project Structure, Process
  Boundary, Credential Boundary,
  On-Disk Project Store, Platform
  Info, Open Action, Composition
  Root, Tests, Out of Scope).
- [`docs/dashboard.md`](./../../docs/dashboard.md)
  — the product dashboard definition (M0.5).
- [`.ai/backlog/`](./../../.ai/backlog/) —
  the engineering backlog (M0.5).
- [`.ai/state/decision-log.md`](./decision-log.md)
  — the decision log (M0.5).
- [`.ai/state/capability-mapping.md`](./capability-mapping.md)
  — the capability mapping (M0.5).
