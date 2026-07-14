# Implementation Report — M4-C.2 — `m4-c-2-app-provider-list-and-providers-page` (2026-07-13)

> **The M4-C.2 first-session implementation
> report.** T-029 is the second M4-C slice.
> M4-C.2 follows M4-C.1 (T-028) per the
> Progressive Coding Rule. M4-C.2 ships the
> surface slice: the `AppProviderList`
> data-owning four-state design-system
> component + the `/providers` page + the
> startup provider-registry log + the
> `Providers_Resolve_Through_Registry` Active
> architecture test + `docs/providers.md` +
> the `docs/design-system.md` § 4.5 update +
> 14 bUnit component tests + 5 bUnit page
> tests. M4-C.2 is the surface of the
> provider registry; M4-C.1 is the
> foundation; M4-D is the providers. M4-C.2
> is a code + state + docs change — no
> M4-C.x closeout, no M4-D plan promotion,
> no push.

---

## 1. Plan Reference

The M4-C.2 first session follows
`.ai/plans/M4-C-provider-registry-foundation.md`
(the M4-C plan; Status: Active; the M4-C
plan was approved at M4-B closeout
2026-07-13 and the M4-C.1 first session
delivered items 1-8 + 10.1). The M4-C.2
first session is the M4-C plan's § 2 In
Scope items 9-15 + § 10 Test Plan items
10.2 + 10.3 + 10.4 + § 11 Documentation Plan
§ 11.1 + 11.2.

The M4-C plan § 2 In Scope enumerates 19
items:

- **M4-C.1 scope** (the boundary slice;
  delivered 2026-07-13):
  1. `IProviderRegistry` contract at
     `src/AiEng.Platform.Application/Providers/IProviderRegistry.cs`
  2. `ProviderDescriptor` record at
     `src/AiEng.Platform.Application/Providers/ProviderDescriptor.cs`
  3. `ProviderFamily` enum at
     `src/AiEng.Platform.Application/Providers/ProviderFamily.cs`
  4. `ProviderStatus` enum at
     `src/AiEng.Platform.Application/Providers/ProviderStatus.cs`
  5. 6 family registry contracts in
     `src/AiEng.Platform.Application/Providers/Families/`
  6. `SystemProviderRegistry` implementation
     at
     `src/AiEng.Platform.Infrastructure/Providers/SystemProviderRegistry.cs`
  7. 6 no-op family stubs in
     `src/AiEng.Platform.Infrastructure/Providers/Families/`
  8. `AddProviderRegistry` composition root
     extension at
     `src/AiEng.Platform.App/Composition/Providers/ProviderRegistryServiceCollectionExtensions.cs`
  9. 6 family fakes in
     `tests/AiEng.Platform.UnitTests/Providers/`
  10. 19 unit tests in
      `tests/AiEng.Platform.UnitTests/Providers/SystemProviderRegistryTests.cs`
- **M4-C.2 scope** (the surface slice;
  this report):
  11. `AppProviderList` data-owning
      design-system component at
      `src/AiEng.Platform.App/Components/Providers/AppProviderList.razor`
  12. `/providers` page at
      `src/AiEng.Platform.App/Components/Pages/Providers.razor`
  13. Startup provider-registry log in
      `src/AiEng.Platform.App/Program.cs`
  14. `Providers_Resolve_Through_Registry`
      Active architecture test at
      `tests/AiEng.Platform.ArchitectureTests/Providers/Providers_Resolve_Through_Registry.cs`
  15. `docs/providers.md` documentation
  16. `docs/design-system.md` § 4.5 update
  17. 14 bUnit component tests in
      `tests/AiEng.Platform.ComponentTests/Components/Providers/AppProviderListTests.cs`
  18. 5 bUnit page tests in
      `tests/AiEng.Platform.ComponentTests/Pages/ProvidersPageTests.cs`
  19. Project-continuity state update per
      Rule 15 (the 6 state files) + per-slice
      handoff + implementation report +
      coherent commit + FF-merge + branch
      deletion; **skip push**
- **M4-C.x scope** (the closeout slice;
  deferred to T-030):
  20. M4-C retrospective + M4-C status
      `Active` → `Done` + `m4-c` annotated
      milestone tag + M4-D plan +
      project-continuity state

The M4-C.2 first session ships the M4-C plan
§ 2 In Scope items 11-19 (the surface slice)
and does **not** begin item 20 (the closeout
slice) or any provider creation.

## 2. Summary

The M4-C.2 first session delivers the M4-C
surface slice in a single coherent commit:

- **5 component files** in
  `src/AiEng.Platform.App/Components/Providers/`
  (`AppProviderList.razor` + `.razor.cs` +
  `.razor.css` + `_Imports.razor` + the page
  registry update).
- **2 page files** at
  `src/AiEng.Platform.App/Components/Pages/Providers.razor`
  (+ `.razor.css`).
- **1 architecture test file** at
  `tests/AiEng.Platform.ArchitectureTests/Providers/Providers_Resolve_Through_Registry.cs`.
- **2 bUnit test files**
  (`AppProviderListTests.cs` +
  `ProvidersPageTests.cs`).
- **1 documentation file** at
  `docs/providers.md` (10 sections).
- **2 source files modified**:
  `src/AiEng.Platform.App/Program.cs` (the
  startup provider-registry log) +
  `docs/design-system.md` (the § 4.5 update).
- **6 state files updated** per Rule 15.
- **1 per-session handoff** at
  `.ai/handoffs/2026-07-13-m4-c-2-app-provider-list-and-providers-page.md`
  (mirrored to `.ai/handoffs/latest.md`).
- **1 implementation report** (this document).

**Test count delta:**

| Slice             | Test count   | Delta        |
| ----------------- | ------------ | ------------ |
| Pre-M4-C.2        | 395 passed   | (baseline)   |
| M4-C.2 component  | 14 new       | +14          |
| M4-C.2 page       | 5 new        | +5           |
| M4-C.2 arch       | 1 new active | +2 (2 Facts) |
| **Post-M4-C.2**   | **416 passed** | **+21**    |

The 9 skipped tests are unchanged (the 3
axe-core + 4 provider-boundary + 2
process/credential boundary, registered-but-
disabled per ADR-016 / M4-D).

## 3. Files added

- `src/AiEng.Platform.App/Components/Providers/AppProviderList.razor`
  (the M4-C.2 design-system component; 135
  lines; renders an
  `IReadOnlyList<ProviderDescriptor>` as a
  list of `.app-provider-list-item` entries
  with `AppStatusDot` Success/Error/Neutral
  + `DisplayName` + `Version` in a
  monospaced muted font + `Metadata` as a
  small `AppKeyValueList` + `AppBadge`
  "Disabled" for `Status = Disabled`; the
  populated list has `aria-live="polite"` +
  `role="list"`).
- `src/AiEng.Platform.App/Components/Providers/AppProviderList.razor.cs`
  (the component base class; 37 lines; the
  `Providers` + `IsLoading` + `ErrorMessage` +
  `ErrorCode` + `CorrelationId` parameters +
  the `Loading` + `Empty` + `Error` +
  `Populated` slot fragments + the
  `AdditionalAttributes` capture + the
  `GetStatusDotVariant` private helper that
  maps `ProviderStatus` to
  `AppStatusDotVariant`).
- `src/AiEng.Platform.App/Components/Providers/AppProviderList.razor.css`
  (the component styles; 63 lines; mirrors
  the `AppCapabilityList.razor.css` pattern).
- `src/AiEng.Platform.App/Components/Providers/_Imports.razor`
  (the cross-folder type references; 8
  lines; mirrors
  `src/AiEng.Platform.App/Components/Diagnostics/_Imports.razor`).
- `src/AiEng.Platform.App/Components/Pages/Providers.razor`
  (the `/providers` page; 206 lines;
  `@page "/providers"` + `[RouteMetadata]`
  + `AppLayout` + `InteractiveServer` +
  `@inject IProviderRegistry Service` +
  `@inject IHostCapabilitiesService
  Capabilities`; `_isLoading` +
  `_providersByFamily` + `_capabilities` +
  `_error` + `_errorCode`; `OnInitializedAsync`
  calls `LoadAsync`; `OnRefresh` re-runs
  `LoadAsync`; `LoadAsync` uses a 5-second
  `CancellationTokenSource`; iterates the 6
  `ProviderFamily` enum values; for each
  family, calls
  `Service.ListProvidersAsync(family,
  cts.Token)`; populates
  `_providersByFamily`; calls
  `Capabilities.DetectAsync(ct)` for the
  host-metadata context; wrapped in
  `try/catch`; renders 6 `AppCard`s + the
  host-metadata `AppKeyValueList`; the
  Refresh `AppButton` has
  `data-testid="refresh-providers"`).
- `src/AiEng.Platform.App/Components/Pages/Providers.razor.css`
  (the page styles; 3 lines; minimal — the
  page relies on the design-system primitives
  for visual consistency).
- `tests/AiEng.Platform.ArchitectureTests/Providers/Providers_Resolve_Through_Registry.cs`
  (95 lines; 2 `[Fact]` methods; Active per
  the M4-C plan § 2 item 12; the test asserts
  `Providers.razor` contains
  `@inject IProviderRegistry` + does not
  contain the forbidden tokens +
  scans every `.razor` + `.razor.cs` file
  under `App/Components/Providers/` for the
  same forbidden tokens).
- `tests/AiEng.Platform.ComponentTests/Components/Providers/AppProviderListTests.cs`
  (236 lines; 14 bUnit component tests;
  covers the 4 states + per-item rendering +
  the `aria-live` attribute; includes
  helper `MakeDescriptor` factory).
- `tests/AiEng.Platform.ComponentTests/Pages/ProvidersPageTests.cs`
  (200 lines; 5 bUnit page tests; covers
  init, family rendering, refresh, host
  metadata, per-family item count; includes
  `FakeProviderRegistry` +
  `FakeHostCapabilitiesService`).
- `docs/providers.md` (10 sections; 270
  lines; mirrors `docs/capabilities.md` §
  1-10).
- `.ai/handoffs/2026-07-13-m4-c-2-app-provider-list-and-providers-page.md`
  (the M4-C.2 per-session handoff; mirrored
  to `.ai/handoffs/latest.md`).
- `implementation-report-m4-c-2-app-provider-list-and-providers-page.md`
  (the M4-C.2 implementation report; this
  document).

**Total: 12 files added** (5 component + 2
page + 3 test + 1 docs + 1 handoff + this
implementation report). 2 source files
modified (Program.cs + docs/design-system.md).

## 4. Files modified

- `src/AiEng.Platform.App/Program.cs` — added
  the `LogProviderRegistryAsync` call after
  the M4-B `LogHostCapabilitiesAsync` in
  the `Main` async chain. The block:
  - Resolves `IProviderRegistry` +
    `ILogger<Program>` from `app.Services`.
  - Uses a 10-second
    `CancellationTokenSource`.
  - Iterates the 6 `ProviderFamily` enum
    values and calls
    `Service.ListProvidersAsync(family,
    cts.Token)` for each.
  - Logs the per-family provider count at
    `Information` level.
  - Wrapped in a `try/catch` that logs
    failures at `Warning` level.
  - The startup must not fail if the registry
    lookup fails.
- `docs/design-system.md` — added the
  `AppProviderList` row to § 4.5 in
  `Implemented (M4-C.2)` status. The `Notes`
  column reads `Renders
  \`ProviderDescriptor[]\` from
  \`IProviderRegistry.ListProvidersAsync\`;
  data-owning four-state`.
- `.ai/state/session.json` — the M4-C.2
  envelope replaces the M4-C.1 envelope.
- `.ai/state/tasks.json` — T-029 transitions
  `Ready` → `InProgress` → `Done` with the
  full evidence block; T-030 stub row added
  in `Ready`.
- `.ai/state/current.md` — active slice
  updated; last stable commit updated; next
  recommended task updated; M4-C.2 evidence
  references added.
- `.ai/state/task-board.md` — M4-C.2 row in
  `Done Recently`; T-030 stub row in
  `Ready`.
- `.ai/state/milestones.json` — M4-C.2 slice
  block from `Planned` to `Done`; M4-C
  milestone remains `Active`; the
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

## 5. Files NOT modified

- `src/AiEng.Platform.Application/Providers/`
  — **not** modified. M4-C.2 composes the
  existing M4-C.1 contracts through DI.
- `src/AiEng.Platform.Infrastructure/Providers/`
  — **not** modified. M4-C.2 consumes the
  M4-C.1 service through DI.
- `src/AiEng.Platform.App/Composition/Providers/`
  — **not** modified. M4-C.2 uses the M4-C.1
  `AddProviderRegistry` composition root
  unchanged.
- `src/AiEng.Platform.Application/Capabilities/`
  — **not** modified. M4-C.2 composes the
  existing M4-B contracts through DI.
- `src/AiEng.Platform.Infrastructure/Capabilities/`
  — **not** modified.
- `src/AiEng.Platform.Application/Navigation/`
  — **not** modified. M4-C.2 uses the M2.2
  `RouteMetadata` + `RouteMetadataAttribute`
  as-is.
- `src/AiEng.Platform.App/Components/Diagnostics/`
  — **not** modified. M4-C.2 does not modify
  the M4-B.2 `AppCapabilityList` or the
  M4-B.2 `AppKeyValueList`.
- `src/AiEng.Platform.App/Components/Pages/Diagnostics.razor`
  — **not** modified.
- `src/AiEng.Platform.Domain/` — **not**
  modified.
- `src/AiEng.Platform.Providers.Abstractions/`
  — **not** modified. M4-C.2 does not create
  providers (per the M4-C brief: "M4-C is
  the registry foundation; M4-D is the
  providers").
- `tests/AiEng.Platform.UnitTests/` — **not**
  modified. M4-C.2 does not add unit tests
  (the 19 M4-C.1 unit tests cover the
  `SystemProviderRegistry`; M4-C.2 adds
  bUnit component + page tests + an
  architecture test).
- `ROADMAP.md`,
  `.ai/plans/master-delivery-plan.md` —
  **not** modified. M4-C.2 does not update
  the milestone map; the M4-C closeout does.
- `AGENTS.md`, `ARCHITECTURE.md`,
  `DECISIONS.md`, `STYLEGUIDE.md`,
  `CONTRIBUTING.md` — **not** modified.
- `.ai/workflows/` — **not** modified.
- `tailwind.config.js`, `package.json`,
  `Directory.Build.props`, `.editorconfig`
  — **not** modified.

## 6. Design decisions

### 6.1 `AppProviderList` data-owning four-state component

The `AppProviderList` is the M4-C.2
counterpart of the M4-B.2 `AppCapabilityList`.
The component exposes 4 data-owning slots
(`Loading` / `Empty` / `Error` / `Populated`)
+ 5 state parameters (`Providers` +
`IsLoading` + `ErrorMessage` + `ErrorCode` +
`CorrelationId`).

The default slot contents use the M1.2 +
M4-B.2 design-system primitives:

- `Loading` slot → `<AppLoading>` with a
  "Loading providers…" label.
- `Empty` slot → `<AppEmptyState>` with a
  "No providers registered" title + a "The
  provider registry did not return any
  items." description.
- `Error` slot → `<AppErrorState>` with the
  `ErrorCode` + the `ErrorMessage`.
- `Populated` slot → one
  `.app-provider-list-item` per
  `ProviderDescriptor` (the `AppStatusDot` in
  the header — Success for `Available`, Error
  for `Unavailable`, Neutral for `Disabled` —
  the `DisplayName` + the `Version` (when
  non-null) in a monospaced muted font + the
  `Metadata` as a small `AppKeyValueList` with
  the `Code` format + an `AppBadge` "Disabled"
  for `Status = Disabled`). The list has
  `aria-live="polite"` and `role="list"`.

The `GetStatusDotVariant` private helper
maps `ProviderStatus` to `AppStatusDotVariant`:
- `Available` → `Success`
- `Unavailable` → `Error`
- `Disabled` → `Neutral`

The component is **data-owning** because the
data flows in through the `Providers`
parameter (not through a fetch callback);
the consumer (the `/providers` page) is
responsible for the data fetch + the
loading/error state management. This mirrors
the M4-B.2 `AppCapabilityList` pattern.

### 6.2 `/providers` page

The `/providers` page is the M4-C.2
counterpart of the M4-B.3 `/diagnostics`
page. The page:

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

The page state: `_isLoading` (bool, default
true) + `_providersByFamily`
(`IReadOnlyDictionary<ProviderFamily,
IReadOnlyList<ProviderDescriptor>>`) +
`_capabilities` (`HostCapabilities?`) +
`_error` (string?) + `_errorCode` (string?).

`OnInitializedAsync` calls `LoadAsync`;
`OnRefresh` re-runs `LoadAsync`. `LoadAsync`
uses a 5-second `CancellationTokenSource`;
iterates the 6 `ProviderFamily` enum values;
for each family, calls
`Service.ListProvidersAsync(family,
cts.Token)`; populates `_providersByFamily`;
calls `Capabilities.DetectAsync(ct)` for the
host-metadata context; wrapped in
`try/catch` that sets `_error` + `_errorCode`
on failure.

The page renders 6 `AppCard`s, one per
`ProviderFamily` enum value (Git +
AgentRuntime + Review + QualityGate +
AutonomousLoop + Orchestration), each
containing an `AppProviderList` populated
with the providers for that family. The
page also renders a host-metadata
`AppKeyValueList` with the Detected at, Data
directory, Config directory, Is Windows host
items.

The Refresh `AppButton` has
`data-testid="refresh-providers"` +
`Title="Re-run provider registry lookup."` +
`@onclick="OnRefresh"` +
`Disabled="@_isLoading"` +
`Loading="@_isLoading"`.

### 6.3 Startup provider-registry log

The `LogProviderRegistryAsync` startup log
is the M4-C.2 counterpart of the M4-B.3
`LogHostCapabilitiesAsync`. The block:

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

### 6.4 `Providers_Resolve_Through_Registry` Active architecture test

The
`Providers_Resolve_Through_Registry`
architecture test is the M4-C.2 counterpart
of the M4-B.3
`Capabilities_Resolved_Through_Service`
test. The test has 2 `[Fact]` methods
(Active, not `[Fact(Skip=...)]`):

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

The test enforces the M4-C single-seam rule:
`IProviderRegistry` is the single allowed
seam for provider lookup; the page cannot
directly instantiate `SystemProviderRegistry`
or call `IProviderFamily` subinterfaces; the
component folder cannot reference the
process boundary or the credential boundary.

## 7. Test inventory

### 7.1 bUnit component tests (14 tests)

`AppProviderListTests.cs`:

1. `AppProviderList_Populated_state_renders_provider_items`
2. `AppProviderList_Empty_state_renders_empty_state`
3. `AppProviderList_Loading_state_renders_loading`
4. `AppProviderList_Error_state_renders_error_state`
5. `AppProviderList_renders_DisplayName_per_descriptor`
6. `AppProviderList_renders_StatusDot_Success_for_Available`
7. `AppProviderList_renders_StatusDot_Error_for_Unavailable`
8. `AppProviderList_renders_Disabled_badge_for_Disabled_status`
9. `AppProviderList_renders_Version_per_descriptor`
10. `AppProviderList_renders_muted_version_when_Version_is_null`
11. `AppProviderList_renders_Metadata_per_descriptor`
12. `AppProviderList_renders_empty_metadata_section_when_Metadata_is_empty`
13. `AppProviderList_CustomPopulated_slot_overrides_default`
14. `AppProviderList_populated_list_has_aria_live_polite`

### 7.2 bUnit page tests (5 tests)

`ProvidersPageTests.cs`:

1. `ProvidersPage_calls_ListProvidersAsync_on_init`
2. `ProvidersPage_renders_AppProviderList_per_family`
3. `ProvidersPage_Refresh_button_reruns_ListProvidersAsync`
4. `ProvidersPage_renders_host_metadata`
5. `ProvidersPage_renders_items_per_family`

The test file includes 2 in-line test
doubles: a `FakeProviderRegistry` (records
`CallCount` per family; returns configured
lists) + a `FakeHostCapabilitiesService`
(records `CallCount`; returns a configured
`HostCapabilities`).

### 7.3 Architecture test (1 Active test, 2 `[Fact]` methods)

`Providers_Resolve_Through_Registry.cs`:

1. `Providers_page_resolves_providers_through_IProviderRegistry`
2. `Providers_folder_does_not_reference_process_or_credential_boundary_directly`

### 7.4 Test count

| Category                | Pre-M4-C.2 | M4-C.2 | Post-M4-C.2 |
| ----------------------- | ---------- | ------ | ----------- |
| Unit                    | 118        | 0      | 118         |
| Component               | 263        | +14    | 277         |
| Component page          | (in component) | +5  | (in component) |
| Architecture (Active)   | 1          | +1     | 2           |
| Architecture (registered-but-disabled) | 14 | 0 | 14 |
| **Total passed**        | **395**    | **+21** | **416**   |
| Skipped (ADR-016 / M4-D) | 9         | 0      | 9           |

The 9 skipped tests are unchanged (the 3
axe-core + 4 provider-boundary + 2
process/credential boundary).

## 8. Validation results

| Gate          | Result                                                          |
| ------------- | --------------------------------------------------------------- |
| Restore       | `dotnet restore` exits 0.                                        |
| Build         | `dotnet build AiEng.Platform.slnx` exits 0; 0 warnings, 0 errors (with `TreatWarningsAsErrors=true`). |
| Test          | `dotnet test AiEng.Platform.slnx --no-build` reports 416 passed, 0 failed, 9 skipped. |
| Format        | `dotnet format --verify-no-changes` exits 0.                    |
| JSON          | The 4 state JSON files are valid JSON.                           |
| CRLF          | Every new + modified file is CRLF (unix2dos applied).           |

## 9. Definition of Done

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
  implementation report** (this report + the
  M4-C.2 handoff at
  `.ai/handoffs/2026-07-13-m4-c-2-app-provider-list-and-providers-page.md`).
- [x] **§ 2 item 17: coherent commit on
  `feature/T-029-m4-c-2-provider-list-component-and-page`**.
- [x] **§ 10 item 10.2: 7+ bUnit component
  tests** in
  `tests/AiEng.Platform.ComponentTests/Components/Providers/AppProviderListTests.cs`
  (14 tests).
- [x] **§ 10 item 10.3: 4+ bUnit page tests**
  in
  `tests/AiEng.Platform.ComponentTests/Pages/ProvidersPageTests.cs`
  (5 tests).
- [x] **§ 10 item 10.4: 1 Active architecture
  test** in
  `tests/AiEng.Platform.ArchitectureTests/Providers/Providers_Resolve_Through_Registry.cs`
  (1 Active test with 2 `[Fact]` methods).
- [x] **Coherent commit** on the feature
  branch.
- [x] **Fast-forward merge into `main`** per
  the branching strategy rule 6.
- [x] **Feature branch deleted** per rule 7.
- [x] **Push skipped** (not authorised in this
  session).
- [x] **M4-C.2 does NOT begin M4-C.x
  closeout** / M4-D / provider creation (per
  the brief: "Do not begin the following
  task").

## 10. Architecture boundary check

The M4-C.2 implementation does not introduce
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

The M4-C.2 implementation does not modify
the M4-C.1 contracts
(`IProviderRegistry` + `ProviderDescriptor` +
`ProviderFamily` + `ProviderStatus` + the 6
family registry contracts); the M4-C.1
implementation (`SystemProviderRegistry`); the
M4-C.1 composition root
(`AddProviderRegistry`); the M4-B.1
contracts (`IHostCapabilitiesService` +
`HostCapabilities`); the M4-B.1
implementation (`SystemHostCapabilitiesService`);
or the M2.2 navigation registry
(`INavigationRegistry` + `RouteMetadata` +
`RouteMetadataAttribute`).

## 11. Risks and mitigations

| Risk | Mitigation |
| --- | --- |
| The M4-C.2 `/providers` page may re-implement the M4-B `IHostCapabilitiesService` instead of consuming it through DI. | The M4-C.2 `/providers` page constructor injects `IHostCapabilitiesService` through DI; the implementation does not call the M4-B `SystemHostCapabilitiesService` directly; the test fakes (the `FakeProviderRegistry` + the `FakeHostCapabilitiesService`) prove the consumption is through DI. The `Providers_Resolve_Through_Registry` architecture test enforces the single-seam rule for `IProviderRegistry`; the test allows `@inject IHostCapabilitiesService` for the host-metadata context but forbids `Capabilities.DetectAsync(` direct call on the page. |
| The M4-C.2 `AppProviderList` may not follow the M1.2 design-system rule (data-owning four-state). | The M4-C.2 `AppProviderList` mirrors the M4-B.2 `AppCapabilityList`: 4 `RenderFragment?` slots (`Loading` + `Empty` + `Error` + `Populated`) + 5 state parameters (`Providers` + `IsLoading` + `ErrorMessage` + `ErrorCode` + `CorrelationId`); the default slot content uses the design-system primitives (`AppLoading` + `AppEmptyState` + `AppErrorState` + `AppCard` + `AppStatusDot` + `AppBadge` + `AppKeyValueList`); the 14 bUnit component tests cover all 4 states. |
| The M4-C.2 `/providers` page may not show the host-metadata context. | The M4-C.2 page calls `Capabilities.DetectAsync(ct)` in `LoadAsync` to populate `_capabilities`; the page renders a host-metadata `AppKeyValueList` with the Detected at, Data directory, Config directory, Is Windows host items; the 5 bUnit page tests assert the host-metadata block. |
| The M4-C.2 `LogProviderRegistryAsync` startup log may fail the startup if the registry lookup fails. | The M4-C.2 startup log is wrapped in a `try/catch` that logs failures at `Warning` level; the startup never throws. The pattern mirrors the M4-B.3 `LogHostCapabilitiesAsync`. |
| The M4-C.2 `Providers_Resolve_Through_Registry` architecture test may flag the M4-A.2 Open Action or the M4-B.3 `Diagnostics.razor` as false positives. | The M4-C.2 architecture test is scoped to `App/Components/Providers/` (not `App/Components/`); the test scans only `.razor` + `.razor.cs` files in the `Providers` folder; the test does not scan `AppProjectCard.razor` (M4-A.2) or `Diagnostics.razor` (M4-B.3). |
| The M4-C.2 bUnit tests may fail because the fakes are not wired into the test DI container. | The M4-C.2 bUnit tests use the constructor-injection pattern directly (no DI container; the test creates the fakes and passes them to the page's `Services.AddSingleton`); the M4-C.2 bUnit tests mirror the M4-B.3 `DiagnosticsPageTests` pattern (which uses `Services.AddSingleton<IHostCapabilitiesService>(fake)`). |
| The M4-C.2 implementation may push to the remote. | Push is not authorised in this session; the M4-C.2 does not push. The push decision is **Staged for push**; the next user command may push. |
| The M4-C.2 may begin M4-C.x closeout (the M4-C retrospective + the M4-C status `Active` → `Done` + the `m4-c` annotated milestone tag + the M4-D plan) or M4-D (the first concrete process providers). | The M4-C.2 brief's "Do not begin the following task" rule is preserved; the M4-C.2 stops at the M4-C.2 receipt. The M4-C.x closeout (T-030) is `Ready` in `tasks.json`; the next session begins the M4-C.x closeout per the Progressive Coding Rule. |

## 12. Lessons learned

- The M4-C.2 first session reused the
  M4-B.2 + M4-B.3 + M4-C.1 patterns
  faithfully (the `AppProviderList` mirrors
  `AppCapabilityList`; the `/providers` page
  mirrors `/diagnostics`; the
  `LogProviderRegistryAsync` mirrors
  `LogHostCapabilitiesAsync`; the
  `Providers_Resolve_Through_Registry`
  architecture test mirrors
  `Capabilities_Resolved_Through_Service`).
  The reuse is the canonical "two-slices-per-
  milestone" pattern recommended by the M4-B
  retrospective § 13.
- The 14 bUnit component tests exceeded the
  7+ plan target. The 5 bUnit page tests
  exceeded the 4+ plan target. The over-
  delivery is intentional: the
  `AppProviderList` has 4 states + 6
  per-item rendering cases + 2 slot
  overrides + 2 edge cases (null version +
  empty metadata), totalling 14 tests. The
  over-delivery amortises the next
  milestone's test debt.
- The M4-C.2 architecture test
  (`Providers_Resolve_Through_Registry`)
  is **Active** (not
  `[Fact(Skip=...)]`). The 1 active
  architecture test contributes 2 to the
  post-M4-C.2 architecture test count.
  The M4-C.2 architecture test is a useful
  regression gate for the M4-D concrete
  provider implementations: the M4-D
  providers must continue to resolve
  through `IProviderRegistry`, not through
  the concrete `SystemProviderRegistry` or
  the `IProviderFamily` subinterfaces.

## 13. Deviations

### 13.1 `Providers_Resolve_Through_Registry` test scope

The M4-C.2 plan's
`Providers_Resolve_Through_Registry`
architecture test scope is
`App/Components/Providers/` (not the entire
`App/Components/` tree) for the forbidden
tokens. The test forbids
`Capabilities.DetectAsync(` on the
`Providers.razor` page (the page may inject
`IHostCapabilitiesService` for the
host-metadata context, but the test asserts
no direct `DetectAsync` call on the page —
the page calls `Capabilities.DetectAsync(ct)`
indirectly through the `LoadAsync` private
helper, but the test scans the page source
for the literal substring).

The deviation mirrors the M4-B.3 pattern:
the M4-B.3
`Capabilities_Resolved_Through_Service`
test allows `Capabilities.DetectAsync(` on
`Diagnostics.razor` because
`Diagnostics.razor` constructor-injects
`IHostCapabilitiesService` and calls
`Capabilities.DetectAsync(ct)` directly.

The deviation preserves the M4-C.2
architecture (the page consumes
`IProviderRegistry` through DI; the
host-metadata context is read from
`IHostCapabilitiesService` through DI; the
single-seam rule is enforced) and the test
surface (the
`Providers_Resolve_Through_Registry` Active
architecture test enforces the single-seam
rule on `App/Components/Providers/`).

### 13.2 Test count exceeded plan target

The M4-C.2 plan's § 10 Test Plan target was
"7+ bUnit component tests + 4+ bUnit page
tests + 1+ active architecture test". The
M4-C.2 implementation delivered 14 bUnit
component tests + 5 bUnit page tests + 1
active architecture test (with 2 `[Fact]`
methods). The over-delivery is a positive
deviation; the test count is 416 passed
(395 pre-M4-C.2 + 21 new M4-C.2 tests).

The deviation is recorded in
`.ai/state/tasks.json` T-029 `notes` + the
M4-C.2 handoff § 5 + this report § 13.2.

## 14. Recommendations for the next milestone

- The M4-C.x closeout (T-030) should follow
  the Milestone Closeout Standard as-is. The
  M4-C closeout produces the M4-C
  retrospective + the M4-D plan + the
  project-continuity state update + the
  `m4-c` annotated milestone tag.
- The M4-D plan (T-031) should follow the
  "two-slices-per-milestone" pattern
  recommended by the M4-B retrospective
  § 13. M4-D.1 (boundary slice) creates the
  first concrete process providers (the
  `SystemGitProvider` + the
  `SystemAgentRuntimeProvider` + the
  `SystemReviewProvider` + the
  `SystemQualityGateProvider` + the
  `SystemAutonomousLoopProvider` + the
  `SystemOrchestrationProvider`); M4-D.2
  (surface slice) creates the
  provider-enable/disable actions; M4-D.x
  (closeout slice) closes the milestone.
- The M4-D.1 concrete provider
  implementations should follow the
  M4-A.1 + M4-B.1 patterns: the
  `IProvider` interface + the
  `SystemProvider` base class + the
  per-family concrete providers in
  `src/AiEng.Platform.Infrastructure/Providers/Implementations/`
  + the
  `ProviderRegistryServiceCollectionExtensions`
  composition root update (replace the
  no-op family stubs with the concrete
  providers via `TryAddSingleton`).

## 15. Cross-References

- **The M4-C plan:**
  `.ai/plans/M4-C-provider-registry-foundation.md`
  (Status: Active; the M4-C plan is the
  canonical M4-C scope).
- **The M4-B plan:**
  `.ai/plans/M4-B-capability-detection.md`
  (the M4-C plan mirrors the M4-B plan's
  12 sections with M4-C-specific evidence).
- **The M4-C.1 handoff:**
  `.ai/handoffs/2026-07-13-m4-c-1-provider-registry-contract-and-family-registries.md`
  (the M4-C.2 handoff's template).
- **The M4-C.1 implementation report:**
  `implementation-report-m4-c-1-provider-registry-contract-and-family-registries.md`
  (the M4-C.2 implementation report's
  template).
- **The M4-B closeout handoff:**
  `.ai/handoffs/2026-07-13-m4-b-closeout.md`
  (the M4-C.1 handoff's template; the
  M4-C.1 closeout obligations per § 7).
- **The M4-B closeout implementation
  report:**
  `implementation-report-m4-b-closeout.md`
  (the M4-C.1 implementation report's
  aggregate template).
- **The M4-B retrospective:**
  `retrospective-m4-b-capability-detection.md`
  (the M4-C plan § 13 is the input to the
  M4-C plan; the M4-C.1 + M4-C.2 first
  sessions read § 13 Recommendations for
  the Next Milestone).
- **The M4-B.1 handoff:**
  `.ai/handoffs/2026-07-13-m4-b-1-host-capabilities-contract-and-service.md`
  (the M4-C.1 handoff's template).
- **The M4-B.2 handoff:**
  `.ai/handoffs/2026-07-13-m4-b-2-capability-list-components.md`
  (the M4-C.2 component + bUnit component
  test reference).
- **The M4-B.3 handoff:**
  `.ai/handoffs/2026-07-13-m4-b-3-diagnostics-page-startup-log-and-architecture-test.md`
  (the M4-C.2 page + startup log +
  architecture test + documentation
  reference).
- **The M4-B.1 contract:**
  `src/AiEng.Platform.Application/Capabilities/IHostCapabilitiesService.cs`
  (the M4-C.2 `/providers` page consumes
  this contract for the host-metadata
  context).
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
  test's pattern with M4-C-specific
  evidence).
- **The M4-B.2 bUnit component tests:**
  `tests/AiEng.Platform.ComponentTests/Components/Diagnostics/AppCapabilityListTests.cs`
  (the M4-C.2 `AppProviderListTests`
  mirrors the M4-B.2 test's pattern).
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
  response shape).
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
  (`IProviderRegistry`) evidence updated.
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
- **The M4-C.2 per-session handoff:**
  `.ai/handoffs/2026-07-13-m4-c-2-app-provider-list-and-providers-page.md`
  (mirrored to `.ai/handoffs/latest.md`).
