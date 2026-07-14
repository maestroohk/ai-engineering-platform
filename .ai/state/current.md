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

- **Active milestone:** **M4-C — Provider
  Registry Foundation** (Active,
  2026-07-13; the M4-C plan is at
  [`.ai/plans/M4-C-provider-registry-foundation.md`](../../.ai/plans/M4-C-provider-registry-foundation.md);
  Status: Active; the M4-C plan was
  approved at M4-B closeout 2026-07-13;
  the M4-C.1 first session (T-028) was
  delivered 2026-07-13 and transitioned
  M4-C from `Awaiting Approval` to
  `Active`). M4-C ships the
  provider registry foundation: the
  `IProviderRegistry` contract (the
  single allowed seam between the
  application and the provider layer; per
  the M4-C architecture, all provider
  access flows through `IProviderRegistry`,
  never through the concrete
  `SystemProviderRegistry` or any
  `IProvider` implementation); the
  `ProviderDescriptor` + `ProviderFamily`
  + `ProviderStatus` records; the 6 family
  registries (`GitProviderFamily` +
  `AgentRuntimeProviderFamily` +
  `ReviewProviderFamily` +
  `QualityGateProviderFamily` +
  `AutonomousLoopProviderFamily` +
  `OrchestrationProviderFamily`); the
  `SystemProviderRegistry` implementation
  that composes the 6 family registries
  and consumes `IHostCapabilitiesService`
  through DI to filter eligible providers
  per host capabilities; the 6 family
  fakes; the `AddProviderRegistry`
  composition root extension; the
  `AppProviderList` data-owning
  design-system component; the
  `/providers` page; the startup
  provider-report log; the
  `Providers_Resolve_Through_Registry`
  architecture test; `docs/providers.md`
  documentation. The M4-C.1 first
  session (T-028, Done 2026-07-13)
  delivered the boundary slice: the
  `IProviderRegistry` + the 6 family
  registry contracts + the
  `SystemProviderRegistry`
  implementation + the 6 no-op family
  stubs + the `AddProviderRegistry`
  composition root + the 6 family
  fakes + 19 unit tests (395 passed
  total). The M4-C.2 first session
  (T-029, Done 2026-07-13) delivered
  the surface slice: the
  `AppProviderList` data-owning
  four-state design-system component +
  the `/providers` page + the startup
  provider-registry log in `Program.cs`
  + the `Providers_Resolve_Through_Registry`
  Active architecture test + 13 bUnit
  component tests + 5 bUnit page tests
  + `docs/providers.md` + the
  `docs/design-system.md` § 4.5
  `AppProviderList` row in
  `Implemented (M4-C.2)` status +
  416 passed total. The M4-C closeout
  session (T-030, `Ready`) is the next
  session on the user's `Approve` or
  `Next` invocation.
- **Last completed milestone:** **M4-B —
  Capability Detection** (Done,
  closed 2026-07-13; the M4-B plan is
  Approved at
  `.ai/plans/M4-B-capability-detection.md`;
  the M4-B closeout plan is at
  `.ai/plans/M4-B-closeout.md`; the
  M4-B retrospective is at
  [`retrospective-m4-b-capability-detection.md`](../../retrospective-m4-b-capability-detection.md);
  the `m4-b` annotated milestone tag is
  at the M4-B closeout commit on `main`
  per the branching strategy rule 9).
  M4-B delivered the host capability
  detection: the `IHostCapabilitiesService`
  contract + the `HostCapabilities` +
  `HostCapability` records in
  `src/AiEng.Platform.Application/Capabilities/`
  (M4-B.1); the
  `SystemHostCapabilitiesService`
  implementation in
  `src/AiEng.Platform.Infrastructure/Capabilities/`
  (probes six host tools — `git`, `ollama`,
  `powershell.exe`, `wsl.exe`, `wt.exe`,
  `bash.exe` — via
  `IProcessRunner.RunToCompletionAsync(tool,
  new[] { "--version" }, ct)` with a
  5-second per-tool timeout and reads six
  provider credentials via
  `ICredentialVault.GetAsync("provider:<key>:token",
  ct)`) (M4-B.1); the
  `AddHostCapabilities` composition root
  extension (M4-B.1); the
  `AppCapabilityList` + `AppKeyValueList`
  data-owning four-state design-system
  components composing the M1.2
  primitives + the `AppKeyValueListFormat`
  enum (M4-B.2); the `/diagnostics` page
  registered via `[RouteMetadata]`
  (Href `/diagnostics`, Order 4,
  ShowInSidebar = true, Icon `◆`)
  (M4-B.3); the startup capability-report
  log through `ILogger<Program>` (M4-B.3);
  the `Capabilities_Resolved_Through_Service`
  architecture test (Active; scoped to
  `App/Components/Diagnostics/` to avoid
  the M4-A.2 Open Action false positive)
  (M4-B.3); the `docs/capabilities.md`
  documentation (M4-B.3); the
  `docs/design-system.md` § 4.5 component
  status update (M4-B.3). M4-B is the
  first consumer of `IProcessRunner` +
  `ICredentialVault` outside the M4-A.2
  Open Action on `AppProjectCard.razor`.
  See
  [`implementation-report-m4-b-closeout.md`](../../implementation-report-m4-b-closeout.md),
  [`retrospective-m4-b-capability-detection.md`](../../retrospective-m4-b-capability-detection.md),
  and the M4-B closeout handoff at
  [`.ai/handoffs/2026-07-13-m4-b-closeout.md`](../../.ai/handoffs/2026-07-13-m4-b-closeout.md).
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

- **Active slice:** **M4-C.2 — AppProviderList data-owning four-state design-system component + /providers page + startup provider-registry log + Providers_Resolve_Through_Registry Active architecture test + docs/providers.md + docs/design-system.md § 4.5 AppProviderList row + 13 bUnit component tests + 5 bUnit page tests** (delivered 2026-07-13, T-029). The branch `feature/T-029-m4-c-2-provider-list-component-and-page` carried the M4-C.2 work; the M4-C.2 closeout commit `feat(m4-c.2): add AppProviderList data-owning design-system component and /providers page` is on this branch; the branch is fast-forwarded into `main` per the branching strategy rule 6; the branch is deleted per rule 7. M4-C.2 ships: the `AppProviderList` data-owning four-state design-system component in `src/AiEng.Platform.App/Components/Providers/` (the 4 data-owning slots Loading/Empty/Error/Populated; the 4 state parameters Providers/IsLoading/ErrorMessage/ErrorCode; the 4 RenderFragment slot overrides; the AdditionalAttributes capture; maps ProviderStatus to AppStatusDotVariant; renders the Disabled badge for Status=Disabled; renders the Version in a monospaced muted font; renders the Metadata as an `AppKeyValueList` with the Code format; the populated list has `role="list"` + `aria-live="polite"`); the `/providers` page in `src/AiEng.Platform.App/Components/Pages/Providers.razor` (`@page "/providers"` + `[RouteMetadata("/providers", "Providers", Order = 5, ShowInSidebar = true, Icon = "◇", Description = "...")]`; `@layout AppLayout` + `@rendermode InteractiveServer`; `@inject IProviderRegistry Service` + `@inject IHostCapabilitiesService Capabilities` + `@inject IPlatformInfo PlatformInfo`; 6 `AppProviderList` cards, one per `ProviderFamily`; 5-second CancellationTokenSource in `LoadAsync`; per-family try/catch with `PROVIDER_LOOKUP_FAILED` + top-level `PROVIDER_LOOKUP_TIMEOUT`; host-metadata `AppKeyValueList` block with Detected at/Data directory/Config directory/Is Windows host; Refresh `AppButton` with `data-testid="refresh-providers"`); the startup provider-registry log in `src/AiEng.Platform.App/Program.cs` (`LogProviderRegistryAsync` method after `LogHostCapabilitiesAsync`; 10-second CancellationTokenSource; iterates the 6 `ProviderFamily` values; logs the per-family provider count at Information level; try/catch with LogWarning on failure); the `Providers_Resolve_Through_Registry` Active architecture test in `tests/AiEng.Platform.ArchitectureTests/Providers/Providers_Resolve_Through_Registry.cs` (2 `[Fact]` methods: `Providers_page_resolves_providers_through_IProviderRegistry` + `Providers_folder_does_not_reference_process_or_credential_boundary_directly`; scoped to `App/Components/Providers/` to avoid the M4-A.2 Open Action + M4-B.3 `Diagnostics.razor` false positives; forbids `RunToCompletionAsync` + `ICredentialVault` + `new SystemProviderRegistry`); 13 bUnit component tests for `AppProviderList` in `tests/AiEng.Platform.ComponentTests/Components/Providers/AppProviderListTests.cs`; 5 bUnit page tests for `/providers` in `tests/AiEng.Platform.ComponentTests/Pages/ProvidersPageTests.cs` (with inline `FakeProviderRegistry` + `FakeHostCapabilitiesService` + `StaticPlatformInfo` + `EmptyNavigationRegistry` test doubles); `docs/providers.md` (10 sections mirroring `docs/capabilities.md` § 1-10: Goals, Project Structure, Contract, Records, Family Registries, Component, Page, Composition Root, Tests, Out of Scope); `docs/design-system.md` § 4.5 `AppProviderList` row in `Implemented (M4-C.2)` status. Total: 416 passed (395 pre-M4-C.2 + 13 bUnit component + 5 bUnit page + 3 new active architecture [1 test file with 2 `[Fact]` methods; 1 architecture test file = 1 architecture slot, +1 new active architecture slot vs M4-C.1 closeout where ArchitectureTests were 16 active, 9 skipped]), 0 failed, 9 skipped (per ADR-016 / M4-D). One documented deviation: the `Providers_Resolve_Through_Registry` architecture test forbids `new SystemProviderRegistry` (the direct-instantiation escape hatch) but does not forbid `Capabilities.DetectAsync(` on the `Providers.razor` page (the page may inject `IHostCapabilitiesService` for the host-metadata context, mirroring the M4-B.3 `Diagnostics.razor` pattern; the test enforces the single-seam rule for `IProviderRegistry`, not the capability service). M4-C.2 does NOT begin M4-C closeout / M4-D / provider creation (per the brief: 'Do not begin the following task'). The next session is the M4-C closeout (T-030, `Ready` in `tasks.json`) on the user's `Approve` or `Next` invocation.
- **Last completed slice:** **M4-C.1 — IProviderRegistry contract + ProviderDescriptor + ProviderFamily + ProviderStatus + 6 family registry contracts + IProviderFamily base + SystemProviderRegistry implementation + 6 no-op family stubs + AddProviderRegistry composition root + 6 family fakes + 19 unit tests** (delivered 2026-07-13, T-028). The branch `feature/T-028-m4-c-1-provider-registry-contract-and-family-registries` carried the M4-C.1 work; the M4-C.1 closeout commit `feat(m4-c.1): add IProviderRegistry contract, family registries, SystemProviderRegistry implementation, family fakes, and AddProviderRegistry composition root` is on this branch; the branch is fast-forwarded into `main` per the branching strategy rule 6; the branch is deleted per rule 7. M4-C.1 ships: the IProviderRegistry contract + the ProviderDescriptor sealed record + the ProviderFamily + ProviderStatus enums in `src/AiEng.Platform.Application/Providers/`; the IProviderFamily base interface + the 6 family-specific subinterfaces (IGitProviderFamily, IAgentRuntimeProviderFamily, IReviewProviderFamily, IQualityGateProviderFamily, IAutonomousLoopProviderFamily, IOrchestrationProviderFamily) in `src/AiEng.Platform.Application/Providers/Families/`; the SystemProviderRegistry implementation in `src/AiEng.Platform.Infrastructure/Providers/` (aggregates the 6 family registries through IProviderFamily; consumes IHostCapabilitiesService through DI; downgrades Available descriptors to Unavailable when the family capability is not available; preserves Disabled; logs at Information level); the 6 no-op family stub implementations in `src/AiEng.Platform.Infrastructure/Providers/Families/`; the AddProviderRegistry composition root extension in `src/AiEng.Platform.App/Composition/Providers/`; the AddProviderRegistry wire-up in AddPlatformServices after AddInfrastructure + AddHostCapabilities; the 6 family fakes in `tests/AiEng.Platform.UnitTests/Providers/`; 19 unit tests in `SystemProviderRegistryTests.cs`. Total: 395 passed (376 pre-M4-C.1 + 19 new), 0 failed, 9 skipped (per ADR-016 / M4-D). M4-C.1 does NOT begin M4-C.2 / M4-C closeout / M4-D / provider creation (per the brief: 'Do not begin the following task').
- **Last completed slice:** **M4-B.1 — IHostCapabilitiesService contract + implementation + composition root + unit tests** (delivered 2026-07-13). The branch `feature/T-024-m4-b-1-host-capabilities-contract-and-service` carried the M4-B.1 work; the M4-B.1 closeout commit `feat(m4-b.1): add IHostCapabilitiesService contract and SystemHostCapabilitiesService implementation` is on this branch; the branch is fast-forwarded into `main` per the branching strategy rule 6; the branch is deleted per rule 7. M4-B.1 ships the IHostCapabilitiesService contract + the HostCapabilities + HostCapability records in `src/AiEng.Platform.Application/Capabilities/`; the SystemHostCapabilitiesService implementation in `src/AiEng.Platform.Infrastructure/Capabilities/` (6 host tool probes + 6 provider credential probes; 5-second per-tool linked CancellationTokenSource timeout; per-tool Regex version parsing; IPlatformInfo.IsWindows gating for Windows-only tools; outer-cancellation propagation via re-throw); the CapabilityProbe internal record types; the AddHostCapabilities composition root extension; the wire-up in AddPlatformServices; 20 unit tests in `tests/AiEng.Platform.UnitTests/Capabilities/SystemHostCapabilitiesServiceTests.cs`. Total: 343 passed, 0 failed, 9 skipped (per ADR-016 / M4-D). M4-B.1 does NOT begin M4-B.2 / M4-B.3 / M4-C / M4-D (per the brief: 'Do not begin the following task').
- **Last completed slice:** **M4-B plan promotion (Capability Detection plan drafted in Awaiting Approval)** (delivered 2026-07-13). The branch `feature/m4-b-capability-detection-plan-promotion` carried the M4-B plan promotion work; the M4-B plan promotion commit `chore(m4-b.plan): draft M4-B capability detection plan in Awaiting Approval` is on this branch; the branch is fast-forwarded into `main` per the branching strategy rule 6; the branch is deleted per rule 7. The M4-B plan is in Awaiting Approval at `.ai/plans/M4-B-capability-detection.md` (12 sections mirroring the M4-A plan's 12-section structure).
- **Last completed slice:** **M4-A.2 — Open
  action on AppProjectCard** (delivered
  2026-07-11). The branch
  `feature/T-022-m4-a-2-open-action` carried
  the M4-A.2 work; the M4-A.2 closeout commit
  `feat(m4-a.2): enable AppProjectCard.Open
  action via IProcessRunner` is on this branch;
  the branch is fast-forwarded into `main` per
  the branching strategy rule 6; the branch is
  deleted per rule 7.
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
- **M4-A.1 — Infrastructure project skeleton:** **Delivered (2026-07-11).** The
  `AiEng.Platform.Infrastructure` csproj
  lands with the four contracts
  (`IProcessRunner` + `ICredentialVault`
  + `IPlatformInfo` + `IProjectStore`) in
  `AiEng.Platform.Application/Infrastructure/`,
  the four implementations in
  `AiEng.Platform.Infrastructure/` (the
  M3 in-memory `IProjectStore` is moved
  to `tests/` as a fixture; the on-disk
  `JsonFileProjectStore` is registered
  through `AddInfrastructure`); 45 new
  unit tests + 2 new architecture tests
  (registered-but-disabled per ADR-016).
  Total: 318 passed, 0 failed, 9 skipped.
  Branch: `feature/T-021-m4-a-1-infrastructure-project-skeleton`.
  See
  [`implementation-report-m4-a-1-infrastructure-project-skeleton.md`](./../../implementation-report-m4-a-1-infrastructure-project-skeleton.md).
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
- **M4-B.1 — `IHostCapabilitiesService` contract + `SystemHostCapabilitiesService` implementation + `AddHostCapabilities` composition root + 20 unit tests:** **Delivered (2026-07-13, T-024).** The first M4-B implementation slice; the boundary slice; the contract + records + implementation + composition root + unit tests; not the components / page / startup log / documentation / architecture test. Total: 343 passed, 0 failed, 9 skipped (per ADR-016 / M4-D). Branch: `feature/T-024-m4-b-1-host-capabilities-contract-and-service`. See [`implementation-report-m4-b-1-host-capabilities-contract-and-service.md`](./../../implementation-report-m4-b-1-host-capabilities-contract-and-service.md).
- **M4-B.2 — `AppCapabilityList` + `AppKeyValueList` data-owning design-system components + 28 bUnit tests:** **Delivered (2026-07-13, T-025).** The second M4-B implementation slice; the components slice. Total: 370 passed, 0 failed, 9 skipped (per ADR-016 / M4-D). Branch: `feature/T-025-m4-b-2-capability-list-components`. See [`implementation-report-m4-b-2-capability-list-components.md`](./../../implementation-report-m4-b-2-capability-list-components.md).
- **M4-B.3 — `/diagnostics` page + startup capability-report log + `Capabilities_Resolved_Through_Service` architecture test + `docs/capabilities.md` + 4 bUnit page tests:** **Delivered (2026-07-13, T-026).** The third M4-B implementation slice; the surface slice. Total: 376 passed, 0 failed, 9 skipped (per ADR-016 / M4-D). Branch: `feature/T-026-m4-b-3-diagnostics-page-startup-log-and-architecture-test`. See [`implementation-report-m4-b-3-diagnostics-page-startup-log-and-architecture-test.md`](./../../implementation-report-m4-b-3-diagnostics-page-startup-log-and-architecture-test.md).
- **M4-B closeout — M4-B retrospective + M4-B -> Done + `m4-b` tag + M4-C plan + project-continuity state:** **Delivered (2026-07-13, T-027).** The fourth M4-B slice; the closeout slice; the M4-B retrospective aggregates the M4-B.1 + M4-B.2 + M4-B.3 evidence blocks; the M4-C plan is in `Awaiting Approval` at `.ai/plans/M4-C-provider-registry-foundation.md`; the `m4-b` annotated milestone tag is at the M4-B closeout commit on `main` per the branching strategy rule 9. M4-B is **Done (closed 2026-07-13)**. Total: 376 passed, 0 failed, 9 skipped (per ADR-016 / M4-D; identical to the M4-B.3 closeout; the M4-B closeout is a docs + workflow + state change with no new tests). Branch: `feature/T-027-m4-b-closeout` (fast-forwarded into `main`; deleted per the branching strategy rule 7). See [`implementation-report-m4-b-closeout.md`](./../../implementation-report-m4-b-closeout.md) + [`retrospective-m4-b-capability-detection.md`](./../../retrospective-m4-b-capability-detection.md) (13 sections per the Milestone Closeout Standard § 4).
- **M4-C.2 — AppProviderList + /providers page + startup log + architecture test + docs:** **Delivered (2026-07-13, T-029).** The second M4-C implementation slice; the surface slice. The `AppProviderList` data-owning four-state design-system component in `src/AiEng.Platform.App/Components/Providers/`; the `/providers` page in `src/AiEng.Platform.App/Components/Pages/Providers.razor`; the startup provider-registry log in `src/AiEng.Platform.App/Program.cs`; the `Providers_Resolve_Through_Registry` Active architecture test in `tests/AiEng.Platform.ArchitectureTests/Providers/`; 13 bUnit component tests + 5 bUnit page tests; `docs/providers.md` (10 sections); `docs/design-system.md` § 4.5 AppProviderList row. Total: 416 passed, 0 failed, 9 skipped (per ADR-016 / M4-D). Branch: `feature/T-029-m4-c-2-provider-list-component-and-page`. See [`implementation-report-m4-c-2-app-provider-list-and-providers-page.md`](../../implementation-report-m4-c-2-app-provider-list-and-providers-page.md).
- **M4-C.1 — IProviderRegistry contract + SystemProviderRegistry + 6 family fakes + composition root + 19 unit tests:** **Delivered (2026-07-13, T-028).** The first M4-C implementation slice; the boundary slice. Total: 395 passed, 0 failed, 9 skipped (per ADR-016 / M4-D). Branch: `feature/T-028-m4-c-1-provider-registry-contract-and-family-registries`. See [`implementation-report-m4-c-1-provider-registry-contract-and-family-registries.md`](../../implementation-report-m4-c-1-provider-registry-contract-and-family-registries.md).
- **M4-C — Provider Registry Foundation:** **Active (2026-07-13; the M4-C plan is at `.ai/plans/M4-C-provider-registry-foundation.md`; Status: Active; the M4-C plan was approved at M4-B closeout 2026-07-13; M4-C.1 + M4-C.2 Delivered 2026-07-13; the M4-C closeout session (T-030) is the next session on the user's `Approve` or `Next` invocation)**. The M4-C plan introduces the `IProviderRegistry` contract (the single allowed seam between the application and the provider layer); the `ProviderDescriptor` + `ProviderFamily` + `ProviderStatus` records; the 6 family registries; the `SystemProviderRegistry` implementation that consumes `IHostCapabilitiesService` through DI; the 6 family fakes; the `AddProviderRegistry` composition root extension; the `AppProviderList` component; the `/providers` page; the startup provider-report log; the `Providers_Resolve_Through_Registry` architecture test; `docs/providers.md` documentation.

## Last Completed Milestone

- **M4-B — Capability Detection**,
  closed **2026-07-13** (the most recent
  milestone). The M4-B evidence is the
  per-slice implementation reports
  (M4-B.1, M4-B.2, M4-B.3, M4-B closeout)
  and the per-slice handoffs (M4-B plan
  promotion, M4-B.1, M4-B.2, M4-B.3,
  M4-B closeout). The M4-B retrospective
  is at
  [`retrospective-m4-b-capability-detection.md`](./../../retrospective-m4-b-capability-detection.md)
  (13 sections per the Milestone Closeout
  Standard § 4; the third milestone
  retrospective in the repository). The
  M4-B closeout commit
  `chore(m4-b.closeout): close M4-B with retrospective, M4-C plan, and m4-b milestone tag`
  is on `main`; the `m4-b` annotated
  milestone tag is at the M4-B closeout
  commit on `main` per the branching
  strategy rule 9. The M4-B plan is at
  `.ai/plans/M4-B-capability-detection.md`
  (Status: Approved). The M4-B closeout
  plan is at `.ai/plans/M4-B-closeout.md`
  (Status: Approved 2026-07-13 via the
  brief). The M4-C plan is at
  `.ai/plans/M4-C-provider-registry-foundation.md`
  (Status: Awaiting Approval; the M4-C
  plan is produced by the M4-B closeout
  session 2026-07-13). Cumulative test
  count at M4-B closeout: 376 passed, 0
  failed, 9 skipped (per ADR-016 / M4-D).
- **M3 — Project Registration**, closed
  **2026-07-11** (preceding milestone).
  The M3 evidence is the per-slice
  implementation reports (M3.1, M3.2, M3
  closeout) and the per-slice handoffs
  (M3.1, M3.2, M3 closeout). The M3
  retrospective is at
  [`retrospective-m3-project-registration.md`](./../../retrospective-m3-project-registration.md).
  The M3 closeout commit
  `chore(m3.closeout): close M3 with retrospective, M4-A plan, and m3 milestone tag`
  is on `main`; the `m3` annotated
  milestone tag is at the M3 closeout
  commit. The M3 plan is at
  `.ai/plans/M3-project-registration.md`.
  The M4-A plan is at
  `.ai/plans/M4-A-infrastructure-process-execution.md`
  (Status: Approved).
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

- The M4-C.2 first session
  (2026-07-13, T-029) is the most
  recent completed task; the M4-C.1
  first session (2026-07-13, T-028)
  is the prior task; the M4-B closeout
  session (2026-07-13, T-027) is the
  prior-prior task; the M4-B.3
  first session (2026-07-13, T-026)
  is the prior task; the M4-B.2
  first session (2026-07-13, T-025)
  is the prior-prior task; the M4-B.1
  implementation session (2026-07-13,
  T-024) is the prior-prior-prior
  task; the M4-B plan promotion
  session (2026-07-13, T-023) is
  the prior-prior-prior-prior task;
  the M4-A.2 implementation session
  (2026-07-11, T-022) is the
  prior-prior-prior-prior-prior task;
  the M4-A.1 implementation session
  (2026-07-11, T-021) is the
  prior-prior-prior-prior-prior-prior
  task; the M3 closeout implementation
  session (2026-07-11, T-020) is the
  prior-prior-prior-prior-prior-prior-prior
  task.
  - Delivered the M4-B closeout
    session (T-027) per the M4-B
    closeout plan at
    `.ai/plans/M4-B-closeout.md`
    (12 sections; Status: Approved
    2026-07-13 via the brief; the
    fourth M4-B session; the
    closeout slice that aggregates
    the M4-B.1 + M4-B.2 + M4-B.3
    evidence blocks; finalises the
    M4-B status to `Done` with
    `closed_at: 2026-07-13`;
    transitions the next-milestone
    handoff to M4-C).
  - Delivered the M4-B.1 first session
    (T-024) per the M4-B.1 implementation
    plan (the boundary slice: contract +
    records + implementation + composition
    root + unit tests; not the
    components / page / startup log /
    documentation / architecture test).
  - Landed the `IHostCapabilitiesService`
    contract in
    `src/AiEng.Platform.Application/Capabilities/IHostCapabilitiesService.cs`.
  - Landed the `HostCapabilities` +
    `HostCapability` sealed records in
    `src/AiEng.Platform.Application/Capabilities/HostCapabilities.cs`.
  - Landed the internal `HostToolProbe` +
    `ProviderCredentialProbe` record types
    in
    `src/AiEng.Platform.Infrastructure/Capabilities/CapabilityProbe.cs`.
  - Landed the `SystemHostCapabilitiesService`
    implementation in
    `src/AiEng.Platform.Infrastructure/Capabilities/SystemHostCapabilitiesService.cs`
    with 6 host tool probes + 6 provider
    credential probes, 5-second linked
    CancellationTokenSource timeout, per-tool
    Regex version parsing,
    IPlatformInfo.IsWindows gating for
    Windows-only tools, and outer-cancellation
    propagation via re-throw.
  - Landed the `AddHostCapabilities`
    composition root extension in
    `src/AiEng.Platform.App/Composition/Capabilities/CapabilitiesServiceCollectionExtensions.cs`
    (TryAddSingleton pattern).
  - Wired `AddHostCapabilities` into
    `AddPlatformServices` in
    `src/AiEng.Platform.App/Composition/ServiceCollectionExtensions.cs`.
  - Landed 20 unit tests + 3 in-line test
    doubles in
    `tests/AiEng.Platform.UnitTests/Capabilities/SystemHostCapabilitiesServiceTests.cs`.
  - Validation: 343 passed, 0 failed,
    9 skipped (per ADR-016 / M4-D);
    0 warnings, 0 errors; format clean.
  - Two documented deviations: (1) The
    `Capabilities_Resolved_Through_Service`
    architecture test is deferred to
    M4-B.3 (the test asserts
    `Diagnostics.razor` exists; the file
    does not exist in M4-B.1). (2) The
    `DetectedAt` test was split into two
    tests: (a) DetectedAt equals the
    TimeProvider value (deterministic);
    (b) DetectedAt falls within the call
    window when the default clock is used.
  - Delivered the M4-B.2 first session
    (T-025) per the M4-B.2 implementation
    plan (the boundary slice:
    AppCapabilityList + AppKeyValueList
    data-owning design-system components;
    not the /diagnostics page, the startup
    log, the documentation, or the
    architecture test).
  - Landed the `AppCapabilityList`
    data-owning four-state design-system
    component in
    `src/AiEng.Platform.App/Components/Diagnostics/AppCapabilityList.razor`
    (+ .razor.cs + .razor.css); it renders
    an `IReadOnlyList<HostCapability>` as a
    list of `AppCard` entries with
    `AppStatusDot` (Success for
    `Available=true`, Error for
    `Available=false`), the `Version` in a
    monospaced muted font, and an `AppBadge`
    "Credential set" for
    `CredentialAvailable=true`; the populated
    list has `aria-live="polite"`.
  - Landed the `AppKeyValueList`
    data-owning four-state design-system
    component in
    `src/AiEng.Platform.App/Components/Diagnostics/AppKeyValueList.razor`
    (+ .razor.cs + .razor.css); it renders
    an `IReadOnlyList<KeyValuePair<string,
    string>>` as a definition list
    (`<dl>`/`<dt>`/`<dd>`) of key-value rows;
    the `Format` parameter (Plain / Boolean /
    Code) controls value rendering; the
    populated container has
    `aria-live="polite"`.
  - Appended the `AppKeyValueListFormat`
    enum (Plain, Boolean, Code) to
    `src/AiEng.Platform.App/Components/Common/Enums.cs`.
  - Landed the `Diagnostics/_Imports.razor`
    mirroring the `Projects/_Imports.razor`
    pattern.
  - Landed 13 bUnit tests for
    `AppCapabilityList` in
    `tests/AiEng.Platform.ComponentTests/Components/Diagnostics/AppCapabilityListTests.cs`.
  - Landed 15 bUnit tests for
    `AppKeyValueList` in
    `tests/AiEng.Platform.ComponentTests/Components/Diagnostics/AppKeyValueListTests.cs`.
  - Validation: 370 passed, 0 failed,
    9 skipped (per ADR-016 / M4-D);
    0 warnings, 0 errors; format clean.
  - One documented deviation: the
    `AppCapabilityList` default Populated
    uses an `AppStack` of `AppCard` entries
    with the `AppStatusDot` in the header
    (not the `AppBadge`-style credential
    slot), and the `AppKeyValueList` default
    Populated uses a definition list
    (`<dl>`/`<dt>`/`<dd>`) with monospaced
    `<code>` elements for the Code format;
    the plan's intent (one card per
    capability with status dot + version +
    credential badge; one row per item with
    key left + value right) is preserved.
  - Delivered the M4-B plan promotion
    (T-023) per the M4-B plan promotion
    plan (the planning-surface change after
    the M4-A.2 closeout).
  - Drafted the M4-B plan at
    `.ai/plans/M4-B-capability-detection.md`
    with `Status: Awaiting Approval` (12
    sections mirroring the M4-A plan's
    12-section structure: Why This Milestone
    Exists; In Scope; Out of Scope; Files to
    Add; Files to Modify; Coherent Commit;
    Critical Files to Read; Existing
    Functions and Utilities to Reuse; Risks
    and Mitigations; Test Plan; Documentation
    Plan; Stop Condition). The plan covers
    `IHostCapabilitiesService` +
    `HostCapabilities` + `HostCapability`
    records; `SystemHostCapabilitiesService`
    implementation probing six host tools
    (`git`, `ollama`, `powershell.exe`,
    `wsl.exe`, `wt.exe`, `bash.exe`) via
    `IProcessRunner.RunToCompletionAsync(tool,
    new[] { "--version" }, ct)` with a
    5-second per-tool timeout and reading six
    provider credentials via
    `ICredentialVault.GetAsync("provider:<key>:token",
    ct)`; `AppCapabilityList` + `AppKeyValueList`
    data-owning four-state design-system
    components composing the M1.2
    primitives; `/diagnostics` page
    registered via `[RouteMetadata]`
    (Href `/diagnostics`, Order 4,
    ShowInSidebar = true, Icon `◆`);
    startup capability-report log through
    `ILogger<Program>`;
    `Capabilities_Resolved_Through_Service`
    architecture test (scoped to
    `App/Components/Diagnostics/` to avoid
    the M4-A.2 Open Action false positive);
    `docs/capabilities.md` documentation.
  - Updated `docs/infrastructure.md` § 11
    (M4-B Consumers): one paragraph noting
    the M4-B `IHostCapabilitiesService` is
    the first consumer of `IProcessRunner` +
    `ICredentialVault` outside the M4-A.2
    Open Action.
  - Updated `ROADMAP.md`: § 2 M4-B row
    `Planned` → `Active`; § 3 M4-B
    `Definition of done` expanded with 9
    bullets mirroring the M4-A.1 + M4-A.2
    DoD bullets.
  - Updated `.ai/plans/master-delivery-plan.md`:
    § 1 M4-B row `Planned` → `Active` with
    full evidence narrative; § 3 M4-B block
    expanded with the full Major capabilities
    delivered + Completion status + Evidence
    blocks; M4-B slice breakdown table added
    with the anticipated M4-B.1 / M4-B.2 /
    M4-B.3 rows.
  - Updated `.ai/state/capabilities.json`:
    C-015 `IHostCapabilitiesService` evidence
    block initialised with
    `evidence.plans: [".ai/plans/M4-B-capability-detection.md"]`;
    C-015 `next_task` set to `T-023`;
    C-015 `last_updated` set to `2026-07-13`;
    top-level `updated_at` +
    `updated_by_session` set to the M4-B plan
    promotion session.
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

- **`main`** (the M4-C.2's
  fast-forwarded branch). The M4-C.2
  closeout commit
  `feat(m4-c.2): add AppProviderList data-owning design-system component and /providers page`
  is the HEAD of `main`. The M4-C.2
  feature branch
  `feature/T-029-m4-c-2-provider-list-component-and-page`
  carried the M4-C.2 work; the branch
  is fast-forwarded into `main` per the
  branching strategy rule 6; the branch
  is deleted per rule 7. The M4-C.1 +
  M4-B closeout + M4-B.3 + M4-B.2 +
  M4-B.1 + M4-A.2 + M4-A.1 + M3.x +
  M3.2 + M3.1 + M2.6 + M2.5 + M2.4 +
  M2.3 + M2.2 + M2.1 + M1.x + M0.5
  commit chain lives on the per-slice
  feature branches; the per-slice
  feature branches are deleted per the
  branching strategy rule 7; the
  per-slice commits are recorded in
  the M3 / M4-A.1 / M4-A.2 / M4-B /
  M4-C.1 evidence blocks of
  `.ai/state/milestones.json`. The
  `m4-b` annotated milestone tag is at
  the M4-B closeout commit on `main`
  per the branching strategy rule 9.
  The remote (`origin`) is configured
  but push is not authorised in this
  session.

## Last Stable Commit

- The M4-C.2 closeout commit
  `feat(m4-c.2): add AppProviderList data-owning design-system component and /providers page`
  on `main` (created 2026-07-13;
  the M4-C.2 closeout commit is the
  closing receipt for the M4-C.2
  surface slice; the M4-C.2 first
  session ships the `AppProviderList`
  + the `/providers` page + the
  startup provider-registry log + the
  `Providers_Resolve_Through_Registry`
  Active architecture test +
  `docs/providers.md` + the
  `docs/design-system.md` § 4.5 update
  + 13 bUnit component tests + 5 bUnit
  page tests + the 6-state
  project-continuity update). The
  M4-C.2 closeout commit lived on the
  feature branch
  `feature/T-029-m4-c-2-provider-list-component-and-page`,
  which is fast-forwarded into `main`
  per the branching strategy rule 6
  and deleted per rule 7. The parent
  commit is the M4-C.1 closeout commit
  `feat(m4-c.1): add IProviderRegistry contract, family registries, SystemProviderRegistry implementation, family fakes, and AddProviderRegistry composition root`
  (hash `9ddb5c5`) on `main`. The
  `m4-b` annotated milestone tag is at
  the M4-B closeout commit on `main`
  per the branching strategy rule 9.
  Working tree is clean at the close
  of the M4-C.2 session; the remote
  (`origin`) is configured but push
  is not authorised in this session.

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
  **416 passed, 9 skipped, 0 failed.**
  - `AiEng.Platform.UnitTests`: 118 tests
    (99 pre-M4-C.1 + 19 new M4-C.1
    unit tests in
    `SystemProviderRegistryTests.cs`).
  - `AiEng.Platform.ComponentTests`: 282
    bUnit / integration tests, all
    passing (264 pre-M4-C.2 + 13 new
    M4-C.2 bUnit tests on
    `AppProviderList`: Populated / Empty
    / Loading / Error / DisplayName /
    StatusDotSuccess / StatusDotError /
    Disabled / Version / MutedVersion /
    Metadata / NoMetadata /
    CustomPopulated / AriaLive + 5 new
    M4-C.2 bUnit page tests on
    `Providers.razor`:
    calls_ListProvidersAsync_on_init /
    renders_AppProviderList_per_family /
    Refresh_reruns / host_metadata /
    items_per_family).
  - `AiEng.Platform.ArchitectureTests`: 16
    tests in total — 7 active (passing)
    and 9 registered-but-disabled
    (skipped) per ADR-016 and the M4-D
    activation milestone. The 1 new
    `Providers_Resolve_Through_Registry`
    architecture test (with 2 `[Fact]`
    methods) is the new active test;
    the M4-A.1
    `Infrastructure_Respects_ProcessBoundary`
    + `Infrastructure_Respects_CredentialBoundary`
    tests remain registered-but-disabled
    per ADR-016 / M4-D. The
    `Pages_Resolve_Projects_Through_Service`
    test (M3.1 + M3.2) remains active and
    green. The
    `Capabilities_Resolved_Through_Service`
    test (M4-B.3) remains active and
    green. The
    `Pages_AreReachable_Through_Registry`
    test (M2.2) remains active and green.

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
- [`.ai/plans/M4-B-capability-detection.md`](./../../.ai/plans/M4-B-capability-detection.md)
  — M4-B plan, **Approved (2026-07-13;
  M4-B.1 + M4-B.2 + M4-B.3 + M4-B
  closeout Delivered 2026-07-13;
  the 'Next' invocation that began
  the M4-B.1 session approved the
  plan per .ai/commands.md § 4 + the
  Progressive Coding Rule § 7.1).
  The M4-B plan introduces the
  `IHostCapabilitiesService` contract
  + the `HostCapabilities` +
  `HostCapability` records in
  `src/AiEng.Platform.Application/Capabilities/`;
  the `SystemHostCapabilitiesService`
  implementation in
  `src/AiEng.Platform.Infrastructure/Capabilities/`
  (probes 6 host tools via
  `IProcessRunner.RunToCompletionAsync`
  with 5-second per-tool timeout;
  reads 6 provider credentials via
  `ICredentialVault.GetAsync`); the
  `AddHostCapabilities` composition
  root extension; the
  `AppCapabilityList` +
  `AppKeyValueList` data-owning
  design-system components + the
  `AppKeyValueListFormat` enum; the
  `/diagnostics` page registered via
  `[RouteMetadata]` (Href
  `/diagnostics`, Order 4); the
  startup capability-report log
  through `ILogger<Program>`; the
  `Capabilities_Resolved_Through_Service`
  architecture test (Active; scoped
  to `App/Components/Diagnostics/`);
  the `docs/capabilities.md`
  documentation. The M4-B
  documentation is at
  [`docs/capabilities.md`](./../../docs/capabilities.md);
  the M4-B.1 implementation report
  is at
  [`implementation-report-m4-b-1-host-capabilities-contract-and-service.md`](./../../implementation-report-m4-b-1-host-capabilities-contract-and-service.md);
  the M4-B.2 implementation report
  is at
  [`implementation-report-m4-b-2-capability-list-components.md`](./../../implementation-report-m4-b-2-capability-list-components.md);
  the M4-B.3 implementation report
  is at
  [`implementation-report-m4-b-3-diagnostics-page-startup-log-and-architecture-test.md`](./../../implementation-report-m4-b-3-diagnostics-page-startup-log-and-architecture-test.md);
  the M4-B closeout implementation
  report is at
  [`implementation-report-m4-b-closeout.md`](./../../implementation-report-m4-b-closeout.md);
  the M4-B retrospective is at
  [`retrospective-m4-b-capability-detection.md`](./../../retrospective-m4-b-capability-detection.md)
  (13 sections per the Milestone
  Closeout Standard § 4). The `m4-b`
  annotated milestone tag is at the
  M4-B closeout commit on `main` per
  the branching strategy rule 9.
  M4-B is **Done (closed
  2026-07-13)**. Prepared in the
  M4-A.2 closeout session.
- [`.ai/plans/M4-B-closeout.md`](./../../.ai/plans/M4-B-closeout.md)
  — M4-B closeout plan, **Delivered
  (2026-07-13, T-027, the M4-B
  closeout session)**. The M4-B
  closeout plan mirrors the M3
  closeout plan's 12-section
  structure; the M4-B closeout
  implementation follows the plan;
  the M4-B closeout handoff is at
  [`.ai/handoffs/2026-07-13-m4-b-closeout.md`](./../../.ai/handoffs/2026-07-13-m4-b-closeout.md)
  (mirrored to
  `.ai/handoffs/latest.md`).
- [`.ai/plans/M4-C-provider-registry-foundation.md`](./../../.ai/plans/M4-C-provider-registry-foundation.md)
  — M4-C plan, **Awaiting Approval
  (2026-07-13; the M4-C plan is
  produced by the M4-B closeout
  session 2026-07-13; the M4-C.1
  first session begins in a future
  session on the user's `Approve`
  or `Next` invocation after the
  user has approved the M4-C plan;
  M4-C is the next milestone after
  M4-B)**. The M4-C plan introduces
  the `IProviderRegistry` contract
  (the single allowed seam between
  the application and the provider
  layer; per the M4-C architecture,
  all provider access flows through
  `IProviderRegistry`, never through
  the concrete
  `SystemProviderRegistry` or any
  `IProvider` implementation); the
  `ProviderDescriptor` +
  `ProviderFamily` + `ProviderStatus`
  records; the 6 family registries
  (`ShellProviderRegistry` +
  `EditorProviderRegistry` +
  `AgentRuntimeProviderRegistry` +
  `TerminalProviderRegistry` +
  `WorktreeProviderRegistry` +
  `CredentialProviderRegistry`); the
  `SystemProviderRegistry`
  implementation that composes the 6
  family registries and consumes
  `IHostCapabilitiesService` through
  DI to filter eligible providers per
  host capabilities; the 6 family
  fakes; the `AddProviderRegistry`
  composition root extension; the
  `AppProviderList` data-owning
  design-system component; the
  `/providers` page; the startup
  provider-report log; the
  `Providers_Resolve_Through_Registry`
  architecture test; the
  `docs/providers.md` documentation.
  The anticipated M4-C slice
  breakdown is M4-C.1 (contract +
  family registries + implementation
  + fakes + composition root + unit
  tests; the boundary slice; the
  M4-C.1 first session is T-028,
  `Ready` in `.ai/state/tasks.json`),
  M4-C.2 (AppProviderList +
  `/providers` page + bUnit tests +
  architecture test; the surface
  slice), M4-C.x (closeout; the M4-C
  retrospective + M4-C → Done +
  `m4-c` tag + M4-D plan +
  project-continuity state).
  Cumulative test count progression:
  M4-B closeout (376) → M4-C.1
  (385) → M4-C.2 (394) → M4-C
  closeout (394). Prepared in the
  M4-B closeout session (T-027,
  2026-07-13).

## Last Implementation Report

- [`implementation-report-m4-c-2-app-provider-list-and-providers-page.md`](./../../implementation-report-m4-c-2-app-provider-list-and-providers-page.md)
  — the M4-C.2 first session
  implementation report (the closing
  receipt for the M4-C.2 surface slice,
  2026-07-13; the `AppProviderList`
  data-owning four-state design-system
  component + the `/providers` page +
  the startup provider-registry log +
  the `Providers_Resolve_Through_Registry`
  Active architecture test +
  `docs/providers.md` + the
  `docs/design-system.md` § 4.5 update
  + 13 bUnit component tests + 5 bUnit
  page tests).
- [`implementation-report-m4-c-1-provider-registry-contract-and-family-registries.md`](./../../implementation-report-m4-c-1-provider-registry-contract-and-family-registries.md)
  — the M4-C.1 first session
  implementation report (the closing
  receipt for the M4-C.1 boundary
  slice, 2026-07-13).
- [`implementation-report-m4-b-closeout.md`](./../../implementation-report-m4-b-closeout.md)
  — the M4-B closeout implementation
  report (the closing receipt for the
  M4-B milestone, 2026-07-13; the M4-B
  closeout slice; the M4-B retrospective
  is at
  [`retrospective-m4-b-capability-detection.md`](./../../retrospective-m4-b-capability-detection.md)
  (13 sections per the Milestone Closeout
  Standard § 4)).
- [`implementation-report-m4-b-3-diagnostics-page-startup-log-and-architecture-test.md`](./../../implementation-report-m4-b-3-diagnostics-page-startup-log-and-architecture-test.md)
  — the M4-B.3 first session
  implementation report (the closing
  receipt for the M4-B.3 slice,
  2026-07-13; the `/diagnostics` page +
  startup capability-report log +
  `Capabilities_Resolved_Through_Service`
  architecture test + `docs/capabilities.md`
  + 4 bUnit page tests).
- [`implementation-report-m4-b-2-capability-list-components.md`](./../../implementation-report-m4-b-2-capability-list-components.md)
  — the M4-B.2 first session
  implementation report (the closing
  receipt for the M4-B.2 slice,
  2026-07-13; the `AppCapabilityList` +
  `AppKeyValueList` data-owning
  design-system components + the
  `AppKeyValueListFormat` enum + 28
  bUnit tests).
- [`implementation-report-m4-b-1-host-capabilities-contract-and-service.md`](./../../implementation-report-m4-b-1-host-capabilities-contract-and-service.md)
  — the M4-B.1 first session
  implementation report (the closing
  receipt for the M4-B.1 slice,
  2026-07-13; the `IHostCapabilitiesService`
  contract + the
  `SystemHostCapabilitiesService`
  implementation + the
  `AddHostCapabilities` composition root
  + 20 unit tests + 3 in-line test
  doubles).
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

> **M4-C closeout — ready to begin.** M4-C.1 + M4-C.2 are delivered (2026-07-13). The M4-C plan is in `Active` at [`.ai/plans/M4-C-provider-registry-foundation.md`](./../../.ai/plans/M4-C-provider-registry-foundation.md) (Status: Active; the M4-C plan was approved at M4-B closeout 2026-07-13; M4-C.1 + M4-C.2 Delivered 2026-07-13; the M4-C closeout session is the next session on the user's `Approve` or `Next` invocation). The next concrete step is the M4-C closeout (T-030, `Ready` in `.ai/state/tasks.json`) per the Milestone Closeout Standard (`.ai/workflows/milestone-closeout.md`) + the Progressive Coding Rule. The M4-C.2 first session does NOT begin M4-C closeout / M4-D / provider creation (per the brief: 'Do not begin the following task'). The M4-C.2 first session explicitly stopped at the M4-C.2 receipt; the M4-C closeout session begins in a future session. **Do not begin the M4-C closeout / M4-D / provider creation in this session** — the M4-C.2 brief explicitly stops at the M4-C.2 receipt (the Progressive Coding Rule applies).

The detailed breakdown of the M4-C.2 slice is in [`.ai/state/task-board.md`](./task-board.md) and the M4-C plan file in [`.ai/plans/M4-C-provider-registry-foundation.md`](./../../.ai/plans/M4-C-provider-registry-foundation.md). The M4-C.2 per-session handoff is at [`.ai/handoffs/2026-07-13-m4-c-2-app-provider-list-and-providers-page.md`](./../../.ai/handoffs/2026-07-13-m4-c-2-app-provider-list-and-providers-page.md) (mirrored to `.ai/handoffs/latest.md`). The next actionable item is:

1. **M4-C closeout** (T-030, `Ready` in `.ai/state/tasks.json`; the next concrete step after T-029 Done). The M4-C closeout session follows the Milestone Closeout Standard: the M4-C retrospective at `retrospective-m4-c-provider-registry-foundation.md` (13 sections per the Milestone Closeout Standard § 4); the M4-C status `Active` → `Done` with `closed_at: 2026-07-13`; the `m4-c` annotated milestone tag at the M4-C closeout commit on `main` per the branching strategy rule 9; the M4-D plan in `Awaiting Approval` at `.ai/plans/M4-D-process-providers.md` (12 sections mirroring the M4-A + M4-B + M4-C plans); the project-continuity state update.

## Last Updated

- **2026-07-13** (M4-C.2 first session).
  This version supersedes the M4-C.1
  version (2026-07-13). The M4-C.2
  first session ships: the
  `AppProviderList` data-owning
  four-state design-system component
  in
  `src/AiEng.Platform.App/Components/Providers/`
  (+ `.razor.cs` + `.razor.css` +
  `_Imports.razor`); the `/providers`
  page in
  `src/AiEng.Platform.App/Components/Pages/Providers.razor`
  (+ `.razor.css`); the startup
  provider-registry log in
  `src/AiEng.Platform.App/Program.cs`
  (`LogProviderRegistryAsync` method
  after `LogHostCapabilitiesAsync`);
  the `Providers_Resolve_Through_Registry`
  Active architecture test in
  `tests/AiEng.Platform.ArchitectureTests/Providers/Providers_Resolve_Through_Registry.cs`
  (2 `[Fact]` methods); 13 bUnit
  component tests for `AppProviderList`
  in
  `tests/AiEng.Platform.ComponentTests/Components/Providers/AppProviderListTests.cs`;
  5 bUnit page tests for `/providers`
  in
  `tests/AiEng.Platform.ComponentTests/Pages/ProvidersPageTests.cs`;
  `docs/providers.md` (10 sections
  mirroring `docs/capabilities.md`
  § 1-10); `docs/design-system.md`
  § 4.5 `AppProviderList` row in
  `Implemented (M4-C.2)` status; the
  project-continuity state update per
  Rule 15 (the 6 state files:
  `.ai/state/session.json` M4-C.2
  envelope; `.ai/state/tasks.json`
  T-029 Ready → InProgress → Done
  with full evidence; T-030 M4-C
  closeout stub added in `Ready`;
  this file; `.ai/state/task-board.md`
  M4-C.2 row in `Done Recently`;
  T-030 stub row in `Ready`;
  `.ai/state/milestones.json`
  M4-C.2 slice block from `Planned`
  to `Done`; M4-C milestone remains
  `Active`; new slice evidence
  entry for M4-C.2;
  `.ai/state/capabilities.json`
  C-021 `AppProviderList` + C-022
  `/providers` page evidence blocks
  initialised in
  `Delivered (M4-C.2)` status).
  Total: 416 passed, 0 failed,
  9 skipped (per ADR-016 / M4-D);
  0 warnings, 0 errors; format clean.
  The M4-C.2 closeout commit
  `feat(m4-c.2): add AppProviderList
  data-owning design-system
  component and /providers page`
  is the most recent commit on
  `main`; the M4-C.2 feature branch
  `feature/T-029-m4-c-2-provider-list-component-and-page`
  is fast-forwarded into `main` and
  deleted per the branching strategy
  rules 6 + 7. One documented
  deviation: the
  `Providers_Resolve_Through_Registry`
  architecture test forbids
  `new SystemProviderRegistry` (the
  direct-instantiation escape hatch)
  but does not forbid
  `Capabilities.DetectAsync(` on the
  `Providers.razor` page (the page
  may inject `IHostCapabilitiesService`
  for the host-metadata context,
  mirroring the M4-B.3 `Diagnostics.razor`
  pattern; the test enforces the
  single-seam rule for
  `IProviderRegistry`, not the
  capability service). The M4-C.2
  does NOT begin M4-C closeout /
  M4-D / provider creation (per the
  brief: 'Do not begin the
  following task' + the Progressive
  Coding Rule). The next session is
  the M4-C closeout (T-030, `Ready`)
  on the user's `Approve` or `Next`
  invocation.

- **2026-07-11** (M4-A.2
  session). The M4-A.2
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
  the most recent handoff (the M4-B plan
  promotion handoff; mirrored from
  `.ai/handoffs/2026-07-13-m4-b-plan-promotion.md`).
- [`.ai/handoffs/2026-07-13-m4-b-plan-promotion.md`](./../../.ai/handoffs/2026-07-13-m4-b-plan-promotion.md)
  — the M4-B plan promotion session handoff
  (the closing receipt for the M4-B plan
  promotion slice, 2026-07-13).
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

> **2026-07-14** — T-031 (autonomous model
> routing optimisation) is `Done`. The
> operating-layer AI session router ships
> (PowerShell 5.1+ supervisor at
> `tools/ai-session-router.ps1`; five profiles in
> `.ai/model-routing.json`; per-phase prompts in
> `.ai/prompts/phases/`; phase receipts in
> `.ai/receipts/phases/`; ADR-017 in `DECISIONS.md`).
> The future in-platform Blazor `IAiSessionRouter` is
> backlog only. The next Ready task remains T-030
> (M4-C closeout).
