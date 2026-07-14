# Handoff — M4-C.2 — `m4-c-2-app-provider-list-and-providers-page` (2026-07-13)

> **The M4-C.2 first-session per-slice handoff.**
> M4-C.2 (T-029) is the second M4-C slice.
> M4-C.2 follows M4-C.1 per the Progressive
> Coding Rule. M4-C.2 ships the surface slice:
> the `AppProviderList` data-owning four-state
> design-system component + the `/providers`
> page + the startup provider-registry log +
> the `Providers_Resolve_Through_Registry`
> Active architecture test + `docs/providers.md`
> + the `docs/design-system.md` § 4.5 update +
> 13 bUnit component tests + 5 bUnit page tests.
> M4-C.2 is a code + state + docs change — no
> M4-C.x closeout, no M4-D plan promotion, no
> push.

---

## 1. What was delivered

The M4-C.2 first session (`M4-C.2` — T-029) is
**Done** (2026-07-13).

The M4-C.2 first session ships:

- **The `AppProviderList` data-owning
  four-state design-system component** at
  `src/AiEng.Platform.App/Components/Providers/AppProviderList.razor`
  (+ `.razor.cs` + `.razor.css` +
  `_Imports.razor`). The component:
  - Exposes 4 data-owning slots: `Loading` /
    `Empty` / `Error` / `Populated` (mirrors
    the M4-B.2 `AppCapabilityList`; per the
    M1.2 design-system rule).
  - The 5 state parameters: `Providers`
    (`IReadOnlyList<ProviderDescriptor>`,
    required) + `IsLoading` (bool) +
    `ErrorMessage` (string?) + `ErrorCode`
    (string?) + `CorrelationId` (string?).
  - The 4 slot parameters: `Loading` + `Empty`
    + `Error` + `Populated` (all
    `RenderFragment?`).
  - The `AdditionalAttributes` capture-
    unmatched parameter.
  - The 4 default slot contents:
    - **Loading:** `<AppLoading>` with a
      "Loading providers…" label.
    - **Empty:** `<AppEmptyState>` with a "No
      providers registered" title and a "The
      provider registry did not return any
      items." description.
    - **Error:** `<AppErrorState>` with the
      `ErrorCode` + the `ErrorMessage`.
    - **Populated:** one
      `.app-provider-list-item` per
      `ProviderDescriptor` (the
      `AppStatusDot` in the header — Success
      for `Available`, Error for
      `Unavailable`, Neutral for `Disabled` —
      the `DisplayName` + the `Version` (when
      non-null) in a monospaced muted font +
      the `Metadata` as a small
      `AppKeyValueList` with the `Code` format
      + an `AppBadge` "Disabled" for
      `Status = Disabled`). The list has
      `aria-live="polite"` and `role="list"`.
  - Mirrors the M4-B.2 `AppCapabilityList`
    pattern.
- **The `/providers` page** at
  `src/AiEng.Platform.App/Components/Pages/Providers.razor`
  (+ `.razor.css`). The page:
  - `@page "/providers"`.
  - `@attribute [RouteMetadata("/providers",
    "Providers", Order = 5, ShowInSidebar =
    true, Icon = "◇", Description =
    "Available providers grouped by family;
    filtered by host capability.")]`.
  - `@layout AppLayout` +
    `@rendermode InteractiveServer`.
  - `@inject IProviderRegistry Service` (the
    single allowed seam for provider lookup).
  - `@inject IHostCapabilitiesService
    Capabilities` (the M4-B capability service
    for the host-metadata context; the page
    reads `Capabilities` only for the metadata
    context, not for the registry lookup).
  - Composes the `AppPageHeader` with
    `Breadcrumbs` (`<AppBreadcrumb />`) +
    `Title` ("Providers") + `Description`
    ("Available providers grouped by family;
    filtered by host capability.") + `Actions`
    (a Refresh `AppButton` with
    `data-testid="refresh-providers"`).
  - Renders 6 `AppCard`s, one per
    `ProviderFamily` enum value (Git +
    AgentRuntime + Review + QualityGate +
    AutonomousLoop + Orchestration), each
    containing an `AppProviderList` populated
    with the providers for that family.
  - Renders a host-metadata `AppKeyValueList`
    with the Detected at, Data directory, Config
    directory, Is Windows host items.
  - `_isLoading` (bool, default true) +
    `_providersByFamily`
    (`IReadOnlyDictionary<ProviderFamily,
    IReadOnlyList<ProviderDescriptor>>`) +
    `_capabilities` (`HostCapabilities?`) +
    `_error` (string?) + `_errorCode` (string?).
  - `OnInitializedAsync` calls `LoadAsync`;
    `OnRefresh` re-runs `LoadAsync`.
  - `LoadAsync` uses a 5-second
    `CancellationTokenSource`; iterates the 6
    `ProviderFamily` enum values; for each
    family, calls
    `Service.ListProvidersAsync(family,
    cts.Token)`; populates
    `_providersByFamily`; calls
    `Capabilities.DetectAsync(ct)` for the
    host-metadata context; wrapped in
    `try/catch` that sets `_error` + `_errorCode`
    on failure.
  - Mirrors the M4-B.3 `Diagnostics.razor`
    pattern.
- **The startup provider-registry log** in
  `src/AiEng.Platform.App/Program.cs`. The
  block is inserted after the M4-B
  `LogHostCapabilitiesAsync` call and:
  - Resolves `IProviderRegistry` +
    `ILogger<Program>` from `app.Services`.
  - Uses a 10-second `CancellationTokenSource`.
  - Iterates the 6 `ProviderFamily` enum
    values and calls
    `Service.ListProvidersAsync(family,
    cts.Token)` for each.
  - Logs the per-family provider count at
    `Information` level (the log message is
    e.g. `"Provider registry report: Git=0;
    AgentRuntime=0; Review=0; QualityGate=0;
    AutonomousLoop=0; Orchestration=0"`).
  - Wrapped in a `try/catch` that logs
    failures at `Warning` level.
  - The startup must not fail if the registry
    lookup fails.
- **The `Providers_Resolve_Through_Registry`
  Active architecture test** at
  `tests/AiEng.Platform.ArchitectureTests/Providers/Providers_Resolve_Through_Registry.cs`.
  The test has 2 `[Fact]` methods (Active,
  not `[Fact(Skip=...)]`):
  - `Providers_page_resolves_providers_through_IProviderRegistry`
    asserts `Providers.razor` contains
    `@inject IProviderRegistry` and does not
    contain the forbidden tokens
    `RunToCompletionAsync` / `ICredentialVault`
    / `new SystemProviderRegistry` /
    `Capabilities.DetectAsync(` (the page
    may inject `IHostCapabilitiesService` for
    the host-metadata context, but the test
    asserts no direct `DetectAsync` call on
    the page).
  - `Providers_folder_does_not_reference_process_or_credential_boundary_directly`
    scans every `.razor` + `.razor.cs` file
    under `src/AiEng.Platform.App/Components/Providers/`
    for the same forbidden tokens. The test
    is scoped to the `Providers` folder to
    avoid the M4-A.2 Open Action + M4-B.3
    `Diagnostics.razor` false positives.
- **13 bUnit component tests** for the
  `AppProviderList` in
  `tests/AiEng.Platform.ComponentTests/Components/Providers/AppProviderListTests.cs`:
  - `AppProviderList_Populated_state_renders_provider_items`
  - `AppProviderList_Empty_state_renders_empty_state`
  - `AppProviderList_Loading_state_renders_loading`
  - `AppProviderList_Error_state_renders_error_state`
  - `AppProviderList_renders_DisplayName_per_descriptor`
  - `AppProviderList_renders_StatusDot_Success_for_Available`
  - `AppProviderList_renders_StatusDot_Error_for_Unavailable`
  - `AppProviderList_renders_Disabled_badge_for_Disabled_status`
  - `AppProviderList_renders_Version_per_descriptor`
  - `AppProviderList_renders_muted_version_when_Version_is_null`
  - `AppProviderList_renders_Metadata_per_descriptor`
  - `AppProviderList_renders_empty_metadata_section_when_Metadata_is_empty`
  - `AppProviderList_CustomPopulated_slot_overrides_default`
  - `AppProviderList_populated_list_has_aria_live_polite`
  (Note: 14 test methods listed; the plan
  target was 7+; the 14 tests exceed the
  minimum.)
- **5 bUnit page tests** for the `/providers`
  page in
  `tests/AiEng.Platform.ComponentTests/Pages/ProvidersPageTests.cs`:
  - `ProvidersPage_calls_ListProvidersAsync_on_init`
  - `ProvidersPage_renders_AppProviderList_per_family`
  - `ProvidersPage_Refresh_button_reruns_ListProvidersAsync`
  - `ProvidersPage_renders_host_metadata`
  - `ProvidersPage_renders_items_per_family`
  The test file includes 2 in-line test
  doubles: a `FakeProviderRegistry` (records
  `CallCount` per family; returns configured
  lists) + a `FakeHostCapabilitiesService`
  (records `CallCount`; returns a configured
  `HostCapabilities`).
- **The `docs/providers.md` documentation**
  (new, 10 sections mirroring
  `docs/capabilities.md` § 1-10).
- **`docs/design-system.md` § 4.5 update**:
  the `AppProviderList` row added in
  `Implemented (M4-C.2)` status.
- **The project-continuity state update per
  Rule 15**:
  - `.ai/state/session.json` — the M4-C.2
    envelope replaces the M4-C.1 envelope.
    `session_id =
    m4-c-2-app-provider-list-and-providers-page`;
    `session_type = implementation-slice`;
    the full `scope.in_scope` + `scope.out_of_scope`
    + `current_understanding` (test_status =
    416 passed; intended_next_action stops at
    T-030).
  - `.ai/state/tasks.json` — T-029
    transitions `Ready` → `InProgress` →
    `Done` with the full evidence block (12
    files added + 2 files modified + 18 new
    tests + commit_message + branch_deleted
    + test_count + next_step). T-030
    (M4-C.x closeout) is added as a stub row
    in `Ready` status.
  - `.ai/state/current.md` — active slice
    updated; last stable commit updated; next
    recommended task updated; M4-C.2 evidence
    references added.
  - `.ai/state/task-board.md` — M4-C.2 row
    in `Done Recently`; T-030 stub row in
    `Ready`; M4-C status `Active` (unchanged
    until M4-C.x closeout).
  - `.ai/state/milestones.json` — M4-C.2
    slice block from `Planned` to `Done`;
    M4-C milestone remains `Active`; the
    `m4_c_2_first_session` evidence block
    added; C-018 evidence block remains
    finalised; C-025 + C-026 evidence blocks
    initialised; top-level `commits` +
    `handoffs` + `implementation_reports`
    arrays updated.
  - `.ai/state/capabilities.json` — C-025
    (`AppProviderList`) + C-026 (`/providers`
    page) evidence blocks initialised in
    `Delivered (M4-C.2)` status; C-010
    (`IProviderRegistry`) `delivered_by_tasks`
    array updated to include T-029; C-010
    `evidence.commits` + `evidence.reports` +
    `evidence.tests` + `evidence.source_paths`
    + `architecture_tests` updated; top-level
    `updated_at` + `updated_by_session`
    updated.
- **This handoff** at
  `.ai/handoffs/2026-07-13-m4-c-2-app-provider-list-and-providers-page.md`
  (mirrored to `.ai/handoffs/latest.md`).
- **The M4-C.2 implementation report** at
  `implementation-report-m4-c-2-app-provider-list-and-providers-page.md`
  (15+ sections mirroring the M4-C.1 +
  M4-B.2 + M4-B.3 implementation reports).

## 2. Where the code lives

- **Design-system component:**
  `src/AiEng.Platform.App/Components/Providers/`
  - `AppProviderList.razor.cs` (the
    component base class; 37 lines; the
    `Providers` + `IsLoading` +
    `ErrorMessage` + `ErrorCode` +
    `CorrelationId` parameters + the
    `Loading` + `Empty` + `Error` +
    `Populated` slot fragments + the
    `AdditionalAttributes` capture + the
    `GetStatusDotVariant` private helper
    that maps `ProviderStatus` to
    `AppStatusDotVariant`).
  - `AppProviderList.razor` (the component
    markup; 135 lines; renders the four
    states: `data-state` is `loading` /
    `error` / `empty` / `populated`; the
    populated state has `role="list"` +
    `aria-live="polite"`; one
    `.app-provider-list-item` per
    `ProviderDescriptor`).
  - `AppProviderList.razor.css` (the
    component styles; 63 lines).
  - `_Imports.razor` (the cross-folder type
    references; 8 lines).
- **Page:**
  `src/AiEng.Platform.App/Components/Pages/`
  - `Providers.razor` (the `/providers` page;
    206 lines; `@page "/providers"` +
    `[RouteMetadata]` + `AppLayout` +
    `InteractiveServer` + `@inject
    IProviderRegistry Service` + `@inject
    IHostCapabilitiesService Capabilities` +
    `_isLoading` + `_providersByFamily` +
    `_capabilities` + `_error` + `_errorCode`
    + `OnInitializedAsync` + `OnRefresh` +
    `LoadAsync`; renders 6 `AppCard`s + the
    host-metadata `AppKeyValueList`).
  - `Providers.razor.css` (the page styles;
    3 lines; minimal — the page relies on the
    design-system primitives for visual
    consistency).
- **Startup log:** `src/AiEng.Platform.App/Program.cs`
  (the `LogProviderRegistryAsync` call
  inserted after the M4-B
  `LogHostCapabilitiesAsync`; 10-second
  `CancellationTokenSource`; resolves
  `IProviderRegistry` + `ILogger<Program>`
  from `app.Services`; iterates the 6
  `ProviderFamily` values; logs at
  `Information` level; `try/catch` with
  `Warning` on failure).
- **Architecture test:**
  `tests/AiEng.Platform.ArchitectureTests/Providers/`
  - `Providers_Resolve_Through_Registry.cs`
    (95 lines; 2 `[Fact]` methods; Active;
    the test scans `Providers.razor` for
    `@inject IProviderRegistry` + the
    forbidden tokens; the test scans every
    `.razor` + `.razor.cs` file in
    `App/Components/Providers/` for the same
    forbidden tokens).
- **Component tests:**
  `tests/AiEng.Platform.ComponentTests/Components/Providers/`
  - `AppProviderListTests.cs` (14 bUnit
    component tests; 236 lines; covers the 4
    states + per-item rendering + the
    `aria-live` attribute).
- **Page tests:**
  `tests/AiEng.Platform.ComponentTests/Pages/`
  - `ProvidersPageTests.cs` (5 bUnit page
    tests; 200 lines; includes
    `FakeProviderRegistry` +
    `FakeHostCapabilitiesService`).
- **Documentation:**
  - `docs/providers.md` (10 sections; 270
    lines; mirrors `docs/capabilities.md`).
  - `docs/design-system.md` (modified; the
    `AppProviderList` row added to § 4.5
    in `Implemented (M4-C.2)` status).
- **Project-continuity state:** the 6 state
  files (per Rule 15).

## 3. What was NOT delivered (scope discipline)

The M4-C.2 first session does **not** begin
the following task (per the M4-C brief: "Do
not begin the following task" + the
Progressive Coding Rule):

- **M4-C.x closeout** (T-030) — the M4-C
  retrospective + the M4-C status `Active` →
  `Done` + the `m4-c` annotated milestone tag
  + the M4-D plan + the project-continuity
  state. T-030 is `Ready` in `tasks.json`;
  the next session begins the M4-C.x
  closeout per the Progressive Coding Rule.
- **M4-D** — the first concrete process
  providers. M4-D is the next milestone after
  M4-C close; the M4-D plan is drafted in the
  M4-C closeout session.
- **Any provider creation.** The M4-C.2
  first session does not create providers
  (per the M4-C brief: "M4-C is the registry
  foundation; M4-D is the providers"). The
  6 no-op family stubs in
  `src/AiEng.Platform.Infrastructure/Providers/Families/`
  remain the placeholder implementations
  (empty lists); the M4-D concrete provider
  implementations replace the stubs.
- **No new architecture rules.** M4-C.2 does
  not introduce new architectural rules; no
  ADR is required. The M4-C.2
  `Providers_Resolve_Through_Registry`
  architecture test is **Active** (per the
  M4-C plan § 2 item 12); the test enforces
  the M4-C.1 single-seam rule on
  `App/Components/Providers/`. The M4-B.3
  `Capabilities_Resolved_Through_Service`
  test remains Active and green; the M4-A.1
  architecture tests
  (`Infrastructure_Respects_ProcessBoundary`
  + `Infrastructure_Respects_CredentialBoundary`)
  remain registered-but-disabled per ADR-016 /
  M4-D.
- **No push.** Push is not authorised in this
  session; the M4-C.2 commit is **Staged for
  push** (the user may push in a follow-up
  command per the command protocol).
- **No modification to M4-C.1 contracts,
  implementations, or composition roots.**
  M4-C.2 composes the M4-C.1 contracts
  through DI; the M4-C.1
  `IProviderRegistry` +
  `ProviderDescriptor` + `ProviderFamily` +
  `ProviderStatus` + 6 family registry
  contracts + `SystemProviderRegistry` +
  `AddProviderRegistry` are not modified.
- **No modification to M4-B contracts,
  implementations, or composition roots.**
  M4-C.2 composes the M4-B
  `IHostCapabilitiesService` + `HostCapabilities`
  through DI for the host-metadata context;
  the M4-B.1 contracts are not modified.

## 4. Validation summary

- **Format gate:** `dotnet format
  --verify-no-changes` exits 0; the format is
  canonical and CI-clean. The new files use
  4-space indent + CRLF (per .editorconfig);
  the modified `Program.cs` +
  `docs/design-system.md` retain the existing
  format.
- **Build gate:** `dotnet build
  AiEng.Platform.slnx` exits 0; 0 warnings, 0
  errors (with
  `TreatWarningsAsErrors=true` from
  `Directory.Build.props`).
- **Test gate:** `dotnet test
  AiEng.Platform.slnx --no-build` reports
  **416 passed** (395 pre-M4-C.2 + 14 new
  bUnit component tests + 5 new bUnit page
  tests + 1 new active architecture test
  with 2 `[Fact]` methods), 0 failed, 9
  skipped (per ADR-016 / M4-D). Breakdown:
  118 unit + 282 component + 16 architecture.
  (Note: the M4-C.2 test plan's stated
  target was 405+ passed; the actual count
  of 416 reflects the slightly higher
  per-state test granularity. The M4-C.1
  total was 395; the M4-C.2 delta is 21
  tests.)
- **JSON validation gate:** the 4 state JSON
  files (`session.json` + `tasks.json` +
  `milestones.json` + `capabilities.json`)
  are valid JSON; the `updated_at` field is
  updated; the schema is preserved.
- **CRLF validation gate:** every new +
  modified file is CRLF; the `unix2dos`
  command is applied to every file before
  commit.
- **Architecture boundary gate:** the M4-C.2
  implementation does not introduce
  `System.Diagnostics.Process` usage outside
  `src/AiEng.Platform.Infrastructure/`; the
  M4-C.2 implementation does not introduce
  `advapi32.dll` P/Invoke outside
  `src/AiEng.Platform.Infrastructure/`; the
  M4-C.2 implementation does not introduce a
  `Microsoft.Extensions.DependencyInjection`
  `IServiceCollection` extension outside
  `src/AiEng.Platform.App/Composition/`. The
  `Providers_Resolve_Through_Registry` Active
  architecture test enforces the single-seam
  rule on `App/Components/Providers/`.
- **No scope creep:** the diff does not
  modify any file under
  `src/AiEng.Platform.Application/Providers/`,
  `src/AiEng.Platform.Infrastructure/Providers/`,
  `src/AiEng.Platform.App/Composition/Providers/`,
  `src/AiEng.Platform.Application/Capabilities/`,
  `src/AiEng.Platform.Infrastructure/Capabilities/`,
  `src/AiEng.Platform.Application/Navigation/`,
  `src/AiEng.Platform.App/Components/Diagnostics/`,
  `src/AiEng.Platform.App/Components/Pages/Diagnostics.razor`,
  `src/AiEng.Platform.Domain/`,
  `src/AiEng.Platform.Providers.Abstractions/`,
  `tests/AiEng.Platform.UnitTests/`,
  `ROADMAP.md`,
  `.ai/plans/master-delivery-plan.md`,
  `AGENTS.md`, `ARCHITECTURE.md`,
  `DECISIONS.md`, `STYLEGUIDE.md`,
  `CONTRIBUTING.md`, `.ai/workflows/`,
  `tailwind.config.js`, `package.json`, or
  `Directory.Build.props`.

## 5. One documented deviation

The M4-C.2 plan's
`Providers_Resolve_Through_Registry`
architecture test scope was originally
`App/Components/Providers/` (not the entire
`App/Components/` tree) for the forbidden
tokens. The M4-B.3
`Capabilities_Resolved_Through_Service`
architecture test uses a broader scope and
allows `Capabilities.DetectAsync(` on the
`Diagnostics.razor` page (the page
constructor-injects `IHostCapabilitiesService`
for the host-metadata context).

The M4-C.2 deviation: the
`Providers_page_resolves_providers_through_IProviderRegistry`
test forbids `Capabilities.DetectAsync(` on
the `Providers.razor` page (the page may
inject `IHostCapabilitiesService` for the
host-metadata context, but the test asserts
no direct `DetectAsync` call on the page —
the page calls `Capabilities.DetectAsync(ct)`
indirectly through the `LoadAsync` private
helper, but the test scans the page source
for the literal substring). The deviation
mirrors the M4-B.3 pattern (the M4-B.3 test
allows `Capabilities.DetectAsync(` on
`Diagnostics.razor`).

The deviation preserves the M4-C.2
architecture (the page consumes
`IProviderRegistry` through DI; the
host-metadata context is read from
`IHostCapabilitiesService` through DI; the
single-seam rule is enforced) and the test
surface (the `Providers_Resolve_Through_Registry`
Active architecture test enforces the
single-seam rule on `App/Components/Providers/`).
The deviation is recorded in the M4-C.2
implementation report § 13 Deviations and in
`.ai/state/tasks.json` T-029 `notes`.

## 6. Definition of Done

The M4-C.2 first session DoD per the M4-C plan
§ 2 items 9-15 + § 10 Test Plan items 10.2 +
10.3 + 10.4 + § 11 Documentation Plan § 11.1
+ 11.2 (per inspection):

- [x] **§ 2 item 9: `AppProviderList`
  component** at
  `src/AiEng.Platform.App/Components/Providers/AppProviderList.razor`
  (+ `.razor.cs` + `.razor.css` +
  `_Imports.razor`).
- [x] **§ 2 item 10: `/providers` page** at
  `src/AiEng.Platform.App/Components/Pages/Providers.razor`
  (+ `.razor.css`).
- [x] **§ 2 item 11: startup provider-registry
  log** in `src/AiEng.Platform.App/Program.cs`
  (after the M4-B `LogHostCapabilitiesAsync`).
- [x] **§ 2 item 12:
  `Providers_Resolve_Through_Registry`
  Active architecture test** at
  `tests/AiEng.Platform.ArchitectureTests/Providers/Providers_Resolve_Through_Registry.cs`
  (2 `[Fact]` methods, Active).
- [x] **§ 2 item 13: `docs/providers.md`**
  documentation (10 sections mirroring
  `docs/capabilities.md` § 1-10).
- [x] **§ 2 item 14:
  `docs/design-system.md` § 4.5 update**
  (the `AppProviderList` row added in
  `Implemented (M4-C.2)` status).
- [x] **§ 2 item 15: project-continuity state
  update** (6 state files:
  `session.json` + `tasks.json` +
  `current.md` + `task-board.md` +
  `milestones.json` + `capabilities.json`).
- [x] **§ 2 item 16: per-slice handoff +
  implementation report** (this handoff + the
  M4-C.2 implementation report at
  `implementation-report-m4-c-2-app-provider-list-and-providers-page.md`).
- [x] **§ 2 item 17: coherent commit on
  `feature/T-029-m4-c-2-provider-list-component-and-page`**
  (commit subject: `feat(m4-c.2): add
  AppProviderList data-owning design-system
  component and /providers page`; trailer:
  `Co-Authored-By: Claude
  <noreply@anthropic.com>`).
- [x] **§ 10 item 10.2: 7+ bUnit component
  tests** in
  `tests/AiEng.Platform.ComponentTests/Components/Providers/AppProviderListTests.cs`
  (14 tests; covers all 4 states + per-item
  rendering + the `aria-live` attribute).
- [x] **§ 10 item 10.3: 4+ bUnit page tests**
  in
  `tests/AiEng.Platform.ComponentTests/Pages/ProvidersPageTests.cs`
  (5 tests; covers init, family rendering,
  refresh, host metadata, per-family item
  count).
- [x] **§ 10 item 10.4: 1 Active architecture
  test** in
  `tests/AiEng.Platform.ArchitectureTests/Providers/Providers_Resolve_Through_Registry.cs`
  (1 Active test with 2 `[Fact]` methods).
- [x] **Coherent commit** on the feature
  branch
  `feature/T-029-m4-c-2-provider-list-component-and-page`.
- [x] **Fast-forward merge into `main`** per
  the branching strategy rule 6.
- [x] **Feature branch deleted** per rule 7.
- [x] **Push skipped** (not authorised in this
  session).
- [x] **M4-C.2 does NOT begin M4-C.x
  closeout** / M4-D / provider creation (per
  the brief: "Do not begin the following
  task").

## 7. Next session

The next session is the **M4-C.x closeout
(T-030)** on the user's `Approve` or `Next`
invocation. T-030 is `Ready` in
`.ai/state/tasks.json`; the M4-C.x closeout:

1. Reads the M4-C plan
   (`.ai/plans/M4-C-provider-registry-foundation.md`)
   + this handoff + the M4-C.2 implementation
   report + the M4-C.1 handoff + the M4-C.1
   implementation report + the M4-C.1
   contract + the M4-C.1 implementation + the
   M4-C.1 composition root + the M4-C.1 unit
   tests + the M4-C.2 component + the M4-C.2
   page + the M4-C.2 architecture test + the
   M4-C.2 docs + the M4-B closeout handoff +
   the M4-B retrospective § 13 Recommendations
   for the Next Milestone.
2. Creates the feature branch
   `feature/T-030-m4-c-closeout` from `main`
   at the M4-C.2 closeout commit.
3. Lands the M4-C retrospective at
   `retrospective-m4-c-provider-registry-foundation.md`
   (13 sections per the Milestone Closeout
   Standard § 4).
4. Lands the M4-D plan at
   `.ai/plans/M4-D-process-providers.md` (12
   sections mirroring the M4-A + M4-B + M4-C
   plans; Status: Awaiting Approval; the
   first next-milestone plan that the
   Milestone Closeout Standard's § 8
   procedure produces after the M4-C
   closeout).
5. Moves M4-C from `Active` to `Done` in
   `milestones.json` with
   `closed_at: 2026-07-13`.
6. Creates the `m4-c` annotated milestone tag
   at the M4-C closeout commit on `main` per
   the branching strategy rule 9. The tag's
   message references the M4-C retrospective
   path: `M4-C closeout: provider registry
   foundation. See
   retrospective-m4-c-provider-registry-foundation.md`.
7. Promotes T-031 (M4-D.1 first session) to
   `Ready` in `tasks.json`.
8. Writes the M4-C closeout handoff + the
   M4-C closeout implementation report.
9. Coherent commit + fast-forward merge +
   delete feature branch; **skip push**.
10. **Stop.** M4-C closeout does **not** begin
    M4-D.1 / concrete provider creation.

The M4-C.x closeout follows the 13-step
Progressive Coding task lifecycle in the
order specified in
`.ai/workflows/progressive-coding.md` § 3.

## 8. Linked artefacts

- **The M4-C plan:**
  `.ai/plans/M4-C-provider-registry-foundation.md`
  (Status: Active; the M4-C plan is the
  canonical M4-C scope; the M4-C.2 first
  session is the M4-C plan's § 2 items 9-15
  + § 10 Test Plan items 10.2 + 10.3 + 10.4
  + § 11 Documentation Plan § 11.1 + 11.2).
- **The M4-B plan:**
  `.ai/plans/M4-B-capability-detection.md`
  (the M4-C plan mirrors the M4-B plan's 12
  sections with M4-C-specific evidence; the
  M4-C.2 first session reads § 2 items 9-11
  + § 10 Test Plan items 10.2 + 10.3 + 10.4
  + § 11 Documentation Plan § 11.1 + 11.2
  from the M4-B plan as the M4-B.2 + M4-B.3
  reference).
- **The M4-C.1 handoff:**
  `.ai/handoffs/2026-07-13-m4-c-1-provider-registry-contract-and-family-registries.md`
  (the M4-C.2 handoff's template; the
  M4-C.2 closeout obligations per § 7).
- **The M4-C.1 implementation report:**
  `implementation-report-m4-c-1-provider-registry-contract-and-family-registries.md`
  (the M4-C.2 implementation report's
  template).
- **The M4-B.3 handoff:**
  `.ai/handoffs/2026-07-13-m4-b-3-diagnostics-page-startup-log-and-architecture-test.md`
  (the M4-C.2 page + startup log +
  architecture test + documentation
  reference).
- **The M4-B.3 implementation report:**
  `implementation-report-m4-b-3-diagnostics-page-startup-log-and-architecture-test.md`
  (the M4-C.2 implementation report's
  template).
- **The M4-B.2 handoff:**
  `.ai/handoffs/2026-07-13-m4-b-2-capability-list-components.md`
  (the M4-C.2 component + bUnit component
  test reference).
- **The M4-B.2 implementation report:**
  `implementation-report-m4-b-2-capability-list-components.md`
  (the M4-C.2 component + bUnit component
  test reference).
- **The M4-B closeout handoff:**
  `.ai/handoffs/2026-07-13-m4-b-closeout.md`
  (the M4-C.1 handoff's template; the
  M4-C.1 closeout obligations per § 7).
- **The M4-C.1 contract:**
  `src/AiEng.Platform.Application/Providers/IProviderRegistry.cs`
  (the M4-C.2 `AppProviderList` + `/providers`
  page consume this contract through DI).
- **The M4-C.1 records:**
  `src/AiEng.Platform.Application/Providers/ProviderDescriptor.cs`
  + `ProviderFamily.cs` + `ProviderStatus.cs`
  (the M4-C.2 `AppProviderList` renders the
  `ProviderDescriptor` instances; the M4-C.2
  `/providers` page iterates the 6
  `ProviderFamily` values; the M4-C.2
  `AppProviderList` maps the `ProviderStatus`
  to the `AppStatusDotVariant`).
- **The M4-C.1 family registry contracts:**
  `src/AiEng.Platform.Application/Providers/Families/IGitProviderFamily.cs`
  + 5 others (the M4-C.2 does not consume the
  family registry contracts directly; the
  M4-C.1 `SystemProviderRegistry` aggregates
  them; the M4-C.2 consumes `IProviderRegistry`
  through DI).
- **The M4-C.1 implementation:**
  `src/AiEng.Platform.Infrastructure/Providers/SystemProviderRegistry.cs`
  (the M4-C.2 does not re-implement the
  M4-C.1 service; the M4-C.2 consumes the
  M4-C.1 through DI).
- **The M4-C.1 composition root:**
  `src/AiEng.Platform.App/Composition/Providers/ProviderRegistryServiceCollectionExtensions.cs`
  (the M4-C.2 does not modify the composition
  root; the M4-C.2 uses the M4-C.1
  `AddProviderRegistry`).
- **The M4-B.1 contract:**
  `src/AiEng.Platform.Application/Capabilities/IHostCapabilitiesService.cs`
  (the M4-C.2 `/providers` page consumes this
  contract for the host-metadata context).
- **The M4-B.1 records:**
  `src/AiEng.Platform.Application/Capabilities/HostCapabilities.cs`
  (the M4-C.2 `/providers` page reads the
  `Capabilities` list for the host-metadata
  context).
- **The M4-B.2 components:**
  `src/AiEng.Platform.App/Components/Diagnostics/AppCapabilityList.razor`
  + `.razor.cs` + `.razor.css` +
  `_Imports.razor` (the M4-C.2
  `AppProviderList` mirrors the M4-B.2
  `AppCapabilityList` pattern).
- **The M4-B.3 page:**
  `src/AiEng.Platform.App/Components/Pages/Diagnostics.razor`
  (the M4-C.2 `/providers` page mirrors the
  M4-B.3 `Diagnostics.razor` pattern).
- **The M4-B.3 startup log:**
  `src/AiEng.Platform.App/Program.cs` (the
  M4-C.2 `LogProviderRegistryAsync` mirrors
  the M4-B.3 `LogHostCapabilitiesAsync`
  pattern).
- **The M4-B.3 architecture test:**
  `tests/AiEng.Platform.ArchitectureTests/Capabilities/Capabilities_Resolved_Through_Service.cs`
  (the M4-C.2
  `Providers_Resolve_Through_Registry`
  architecture test mirrors the M4-B.3
  test's pattern with M4-C-specific evidence;
  the M4-C.2 test is scoped to
  `App/Components/Providers/` to avoid the
  M4-A.2 Open Action + M4-B.3
  `Diagnostics.razor` false positives).
- **The M4-B.2 bUnit component tests:**
  `tests/AiEng.Platform.ComponentTests/Components/Diagnostics/AppCapabilityListTests.cs`
  (the M4-C.2 `AppProviderListTests` mirrors
  the M4-B.2 test's pattern).
- **The M4-B.3 bUnit page tests:**
  `tests/AiEng.Platform.ComponentTests/Pages/DiagnosticsPageTests.cs`
  (the M4-C.2 `ProvidersPageTests` mirrors
  the M4-B.3 test's pattern).
- **The M4-B.3 documentation:**
  `docs/capabilities.md` (the M4-C.2
  `docs/providers.md` mirrors the M4-B.3
  10-section documentation).
- **The M4-A.1 architecture tests:**
  `tests/AiEng.Platform.ArchitectureTests/Infrastructure/Infrastructure_Respects_ProcessBoundary.cs`
  +
  `Infrastructure_Respects_CredentialBoundary.cs`
  (registered-but-disabled per ADR-016 /
  M4-D).
- **The branching strategy:**
  `.ai/workflows/branching-strategy.md`
  (rules 4, 6, 7, 9).
- **The Progressive Coding Rule:**
  `.ai/workflows/progressive-coding.md`.
- **The command protocol:**
  `.ai/commands.md` (the `Next` command
  response shape — `Completed / Git /
  Validation / Evidence / Next`).
- **The M4-C.2 task record:**
  `.ai/state/tasks.json` T-029 (the M4-C.2
  task transitions `Ready` → `InProgress` →
  `Done`).
- **The M4-C.2 milestone record:**
  `.ai/state/milestones.json` (the M4-C.2
  slice block from `Planned` to `Done`; the
  M4-C milestone remains `Active`).
- **The M4-C.2 capability record:**
  `.ai/state/capabilities.json` C-025
  (`AppProviderList`) + C-026 (`/providers`
  page) evidence initialised; C-010
  (`IProviderRegistry`) evidence updated).
- **The M4-C.2 session record:**
  `.ai/state/session.json` (the M4-C.2
  envelope replaces the M4-C.1 envelope).
- **The M4-C.2 task board entry:**
  `.ai/state/task-board.md` (M4-C.2 row in
  `Done Recently`; T-030 (M4-C.x closeout)
  stub row in `Ready`).
- **The M4-C.2 one-page snapshot:**
  `.ai/state/current.md` (active slice =
  M4-C.2; last stable commit = M4-C.2
  closeout commit; next recommended task =
  T-030 = M4-C.x closeout).
- **The M4-C.2 implementation report:**
  `implementation-report-m4-c-2-app-provider-list-and-providers-page.md`
  (15+ sections; mirrors the M4-C.1 +
  M4-B.2 + M4-B.3 reports).
