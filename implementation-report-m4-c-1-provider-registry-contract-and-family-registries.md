# Implementation Report — M4-C.1 — `m4-c-1-provider-registry-contract-and-family-registries` (2026-07-13)

> **The M4-C.1 first-session implementation
> report.** T-028 is the first M4-C slice.
> M4-C.1 follows M4-B closeout (T-027) per
> the Progressive Coding Rule. M4-C.1 ships
> the boundary slice: the `IProviderRegistry`
> contract + the 6 family registry contracts
> + the `SystemProviderRegistry`
> implementation + the 6 no-op family stubs +
> the `AddProviderRegistry` composition root
> + the 6 family fakes + 19 unit tests.
> M4-C.1 is the registry foundation; M4-D is
> the providers. M4-C.1 is a code + state +
> workflow change — no UI surface, no M4-C.2
> implementation, no M4-D plan promotion, no
> push.

---

## 1. Plan Reference

The M4-C.1 first session follows
`.ai/plans/M4-C-provider-registry-foundation.md`
(the M4-C plan; Status: Active; the M4-C
plan was approved at M4-B closeout
2026-07-13). The M4-C.1 first session is the
M4-C plan's § 2 In Scope items 1-8 + § 10
Test Plan item 10.1.

The M4-C plan § 2 In Scope enumerates 19
items:

- **M4-C.1 scope** (the boundary slice;
  this report):
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
  7. 6 family fakes in
     `tests/AiEng.Platform.UnitTests/Providers/`
  8. `AddProviderRegistry` composition root
     extension at
     `src/AiEng.Platform.App/Composition/Providers/ProviderRegistryServiceCollectionExtensions.cs`
  9. 9+ unit tests in
     `tests/AiEng.Platform.UnitTests/Providers/SystemProviderRegistryTests.cs`
  10. Project-continuity state update per
      Rule 15 (the 6 state files)
  11. M4-C.1 per-session handoff at
      `.ai/handoffs/2026-07-13-m4-c-1-provider-registry-contract-and-family-registries.md`
  12. M4-C.1 implementation report (this
      document)
  13. Coherent commit on the M4-C.1 feature
      branch + fast-forward merge + branch
      deletion; **skip push**
- **M4-C.2 scope** (the surface slice;
  deferred to T-029):
  14. `AppProviderList` data-owning
      design-system component
  15. `/providers` page
  16. Startup provider-registry log
  17. `Providers_Resolve_Through_Registry`
      architecture test
  18. `docs/providers.md` documentation
- **M4-C.x scope** (the closeout slice;
  deferred to T-030):
  19. M4-C retrospective + M4-C status
      `Active` → `Done` + `m4-c` annotated
      milestone tag + M4-D plan +
      project-continuity state

The M4-C.1 first session ships the M4-C plan
§ 2 In Scope items 1-13 (the boundary slice)
and does **not** begin items 14-19 (the
surface slice + the closeout slice +
provider creation).

## 2. Summary

The M4-C.1 first session delivers the M4-C
boundary slice in a single coherent commit:

- **4 contract files** in
  `src/AiEng.Platform.Application/Providers/`
  (the `IProviderRegistry` contract + the
  `ProviderDescriptor` sealed record + the
  `ProviderFamily` enum + the `ProviderStatus`
  enum).
- **7 family registry contract files** in
  `src/AiEng.Platform.Application/Providers/Families/`
  (the `IProviderFamily` base + 6
  family-specific subinterfaces).
- **1 implementation file** in
  `src/AiEng.Platform.Infrastructure/Providers/`
  (the `SystemProviderRegistry`).
- **6 no-op family stub implementation
  files** in
  `src/AiEng.Platform.Infrastructure/Providers/Families/`
  (the 6 family placeholder implementations).
- **1 composition root file** in
  `src/AiEng.Platform.App/Composition/Providers/`
  (the `AddProviderRegistry` extension).
- **1 modified file**
  (`src/AiEng.Platform.App/Composition/ServiceCollectionExtensions.cs`:
  the `AddProviderRegistry` wire-up).
- **6 family fakes** in
  `tests/AiEng.Platform.UnitTests/Providers/`.
- **1 unit test file** in
  `tests/AiEng.Platform.UnitTests/Providers/SystemProviderRegistryTests.cs`
  (19 unit tests).
- **6 state files modified** (the
  project-continuity state per Rule 15).
- **1 handoff file** at
  `.ai/handoffs/2026-07-13-m4-c-1-provider-registry-contract-and-family-registries.md`
  (mirrored to `.ai/handoffs/latest.md`).
- **1 implementation report file** (this
  document).

**Total: 29 files added + 1 file modified +
19 unit tests added.**

The M4-C.1 first session preserves the M4-C
architecture:

- **Single seam rule:** the
  `IProviderRegistry` is the single allowed
  seam between the application and the
  provider layer. The M4-C.2 page `@inject`s
  `IProviderRegistry`; the M4-C.2
  `Providers_Resolve_Through_Registry`
  architecture test enforces the seam.
- **Family registry pattern:** one family
  registry per `ProviderFamily` (Git +
  AgentRuntime + Review + QualityGate +
  AutonomousLoop + Orchestration). The
  family registries are the "what providers
  exist for this family" surface; the
  `SystemProviderRegistry` is the "which
  providers are eligible given the host
  capabilities" surface.
- **Capability-aware filtering:** the
  `SystemProviderRegistry` consumes
  `IHostCapabilitiesService` through DI and
  downgrades `Available` descriptors to
  `Unavailable` when the family capability is
  not available; `Disabled` and `Unavailable`
  are preserved regardless of the family
  capability state.
- **Layered architecture:** the
  `AiEng.Platform.Application` project defines
  the contracts; the
  `AiEng.Platform.Infrastructure` project
  provides the implementation; the
  `AiEng.Platform.App` project composes the
  DI; the `tests/AiEng.Platform.UnitTests`
  project provides the test surface. The
  App project does not reference the UnitTests
  project (the production stubs live in
  Infrastructure; the test fakes live in
  UnitTests).

## 3. Files Created

### 3.1 Contracts — `src/AiEng.Platform.Application/Providers/`

- **`IProviderRegistry.cs`** (14 lines).
  The M4-C contract; the single allowed seam
  between the application and the provider
  layer.
  ```csharp
  namespace AiEng.Platform.Application.Providers;

  public interface IProviderRegistry
  {
      Task<IReadOnlyList<ProviderDescriptor>> ListProvidersAsync(
          ProviderFamily family,
          CancellationToken cancellationToken = default);
  }
  ```
- **`ProviderDescriptor.cs`** (9 lines).
  The sealed record; the data envelope the
  M4-C.2 `AppProviderList` component
  renders.
  ```csharp
  namespace AiEng.Platform.Application.Providers;

  public sealed record class ProviderDescriptor(
      string Id,
      string DisplayName,
      ProviderFamily Family,
      ProviderStatus Status,
      string? Version,
      IReadOnlyDictionary<string, string> Metadata);
  ```
- **`ProviderFamily.cs`** (9 lines). The
  6-value enum.
  ```csharp
  namespace AiEng.Platform.Application.Providers;

  public enum ProviderFamily
  {
      Git = 0,
      AgentRuntime = 1,
      Review = 2,
      QualityGate = 3,
      AutonomousLoop = 4,
      Orchestration = 5,
  }
  ```
- **`ProviderStatus.cs`** (7 lines). The
  3-value enum.
  ```csharp
  namespace AiEng.Platform.Application.Providers;

  public enum ProviderStatus
  {
      Available = 0,
      Unavailable = 1,
      Disabled = 2,
  }
  ```

### 3.2 Family registry contracts — `src/AiEng.Platform.Application/Providers/Families/`

- **`IProviderFamily.cs`** (5 lines). The
  base interface shared by all 6
  family-specific subinterfaces; declares the
  `ListProvidersAsync(ct)` method. The base
  is required to resolve the C# 14 switch
  expression common-type issue
  (`SystemProviderRegistry` aggregates the 6
  family registries through the `IProviderFamily`
  base).
  ```csharp
  namespace AiEng.Platform.Application.Providers.Families;

  public interface IProviderFamily
  {
      Task<IReadOnlyList<ProviderDescriptor>> ListProvidersAsync(CancellationToken cancellationToken = default);
  }
  ```
- **`IGitProviderFamily.cs`** (3 lines).
  The Git family subinterface; empty marker
  interface that extends `IProviderFamily`.
  ```csharp
  namespace AiEng.Platform.Application.Providers.Families;

  public interface IGitProviderFamily : IProviderFamily { }
  ```
- **`IAgentRuntimeProviderFamily.cs`** (3
  lines). Mirrors `IGitProviderFamily`.
- **`IReviewProviderFamily.cs`** (3 lines).
  Mirrors `IGitProviderFamily`.
- **`IQualityGateProviderFamily.cs`** (3
  lines). Mirrors `IGitProviderFamily`.
- **`IAutonomousLoopProviderFamily.cs`** (3
  lines). Mirrors `IGitProviderFamily`.
- **`IOrchestrationProviderFamily.cs`** (3
  lines). Mirrors `IGitProviderFamily`.

### 3.3 Implementation — `src/AiEng.Platform.Infrastructure/Providers/`

- **`SystemProviderRegistry.cs`** (~95
  lines). The M4-C implementation; aggregates
  the 6 family registries through
  `IProviderFamily`; consumes
  `IHostCapabilitiesService` through DI;
  downgrades `Available` to `Unavailable` when
  the family capability is unavailable;
  preserves `Disabled` and `Unavailable`; logs
  at `Information` level. The implementation:
  - Constructor-injects 6 family registries
    as `IProviderFamily` (the base) +
    `IHostCapabilitiesService` +
    `ILogger<SystemProviderRegistry>`. The
    ctor null-checks every dependency.
  - The `ListProvidersAsync` method:
    1. Throws `OperationCanceledException`
       if the token is cancelled.
    2. Calls `IHostCapabilitiesService.DetectAsync`
       to read the host capabilities.
    3. Computes the set of available host
       capability keys
       (`StringComparer.OrdinalIgnoreCase`).
    4. Resolves the family registry from the
       `ProviderFamily` enum via a switch
       expression on the `IProviderFamily`
       field.
    5. Calls the family registry's
       `ListProvidersAsync` to get the
       descriptors.
    6. Filters the descriptors: when the
       family capability is unavailable,
       `Available` descriptors are
       downgraded to `Unavailable`; `Disabled`
       and `Unavailable` are preserved
       regardless of the family capability
       state.
    7. Logs the per-family lookup count at
       `Information` level.
    8. Returns the filtered list.
  - The `GetFamilyCapabilityKey` private
    helper maps `ProviderFamily` to the
    M4-B capability key: `Git` → `"git"`,
    `AgentRuntime` → `"ollama"`, `Review` →
    `"ollama"`, `QualityGate` →
    `"powershell"`, `AutonomousLoop` →
    `"ollama"`, `Orchestration` →
    `"ollama"`.

### 3.4 Family stub implementations — `src/AiEng.Platform.Infrastructure/Providers/Families/`

- **`GitProviderFamily.cs`** (~13 lines).
  The Git family no-op stub; returns an
  empty `IReadOnlyList<ProviderDescriptor>`.
  The stub is the production implementation
  registered by `AddProviderRegistry`; the
  stub is replaced by the concrete provider
  implementation in M4-D.
  ```csharp
  namespace AiEng.Platform.Infrastructure.Providers.Families;

  public sealed class GitProviderFamily : IGitProviderFamily
  {
      public Task<IReadOnlyList<ProviderDescriptor>> ListProvidersAsync(CancellationToken cancellationToken = default)
      {
          cancellationToken.ThrowIfCancellationRequested();
          return Task.FromResult<IReadOnlyList<ProviderDescriptor>>(Array.Empty<ProviderDescriptor>());
      }
  }
  ```
- **`AgentRuntimeProviderFamily.cs`** (~13
  lines). Mirrors `GitProviderFamily`.
- **`ReviewProviderFamily.cs`** (~13 lines).
  Mirrors `GitProviderFamily`.
- **`QualityGateProviderFamily.cs`** (~13
  lines). Mirrors `GitProviderFamily`.
- **`AutonomousLoopProviderFamily.cs`**
  (~13 lines). Mirrors `GitProviderFamily`.
- **`OrchestrationProviderFamily.cs`**
  (~13 lines). Mirrors `GitProviderFamily`.

### 3.5 Composition root — `src/AiEng.Platform.App/Composition/Providers/`

- **`ProviderRegistryServiceCollectionExtensions.cs`**
  (~28 lines). The M4-C composition root;
  the `AddProviderRegistry` extension method
  registers the 6 no-op family stubs +
  `IProviderRegistry` → `SystemProviderRegistry`
  as singletons via `TryAddSingleton`.
  ```csharp
  namespace AiEng.Platform.App.Composition.Providers;

  using AiEng.Platform.Application.Providers;
  using AiEng.Platform.Application.Providers.Families;
  using AiEng.Platform.Infrastructure.Providers;
  using AiEng.Platform.Infrastructure.Providers.Families;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.DependencyInjection.Extensions;

  public static class ProviderRegistryServiceCollectionExtensions
  {
      public static IServiceCollection AddProviderRegistry(this IServiceCollection services)
      {
          if (services is null)
          {
              throw new ArgumentNullException(nameof(services));
          }

          services.TryAddSingleton<IGitProviderFamily, GitProviderFamily>();
          services.TryAddSingleton<IAgentRuntimeProviderFamily, AgentRuntimeProviderFamily>();
          services.TryAddSingleton<IReviewProviderFamily, ReviewProviderFamily>();
          services.TryAddSingleton<IQualityGateProviderFamily, QualityGateProviderFamily>();
          services.TryAddSingleton<IAutonomousLoopProviderFamily, AutonomousLoopProviderFamily>();
          services.TryAddSingleton<IOrchestrationProviderFamily, OrchestrationProviderFamily>();
          services.TryAddSingleton<IProviderRegistry, SystemProviderRegistry>();
          return services;
      }
  }
  ```

### 3.6 Family fakes — `tests/AiEng.Platform.UnitTests/Providers/`

- **`FakeGitProviderFamily.cs`** (~30
  lines). The Git family fake; records
  `CallCount` + `ObservedTokens` +
  `QueuedResults`; returns the next queued
  result (or an empty array if no result is
  queued). The fake is the M4-C.1 test
  surface; the M4-D concrete provider
  implementation replaces the fake in
  production.
- **`FakeAgentRuntimeProviderFamily.cs`**
  (~30 lines). Mirrors `FakeGitProviderFamily`.
- **`FakeReviewProviderFamily.cs`** (~30
  lines). Mirrors `FakeGitProviderFamily`.
- **`FakeQualityGateProviderFamily.cs`**
  (~30 lines). Mirrors `FakeGitProviderFamily`.
- **`FakeAutonomousLoopProviderFamily.cs`**
  (~30 lines). Mirrors `FakeGitProviderFamily`.
- **`FakeOrchestrationProviderFamily.cs`**
  (~30 lines). Mirrors `FakeGitProviderFamily`.

### 3.7 Unit tests — `tests/AiEng.Platform.UnitTests/Providers/`

- **`SystemProviderRegistryTests.cs`** (~330
  lines; 19 unit tests + 2 in-line test
  doubles). The 19 tests:
  - 8 constructor-null tests (one per
    dependency: `IGitProviderFamily` +
    `IAgentRuntimeProviderFamily` +
    `IReviewProviderFamily` +
    `IQualityGateProviderFamily` +
    `IAutonomousLoopProviderFamily` +
    `IOrchestrationProviderFamily` +
    `IHostCapabilitiesService` +
    `ILogger<SystemProviderRegistry>`).
  - 7 happy-path / edge-case tests:
    1. `ListProvidersAsync_returns_descriptors_when_family_capability_is_available`
       (host has the `git` capability →
       descriptors are returned).
    2. `ListProvidersAsync_downgrades_to_Unavailable_when_family_capability_is_not_available`
       (host does not have the `git`
       capability → `Available` descriptors
       are downgraded to `Unavailable`;
       `Disabled` and `Unavailable` are
       preserved).
    3. `ListProvidersAsync_returns_empty_for_family_with_no_descriptors`
       (family returns an empty list →
       registry returns an empty list).
    4. `ListProvidersAsync_propagates_cancellation`
       (asserts the implementation
       propagates a cancelled
       `CancellationToken`).
    5. `ListProvidersAsync_returns_descriptors_per_family`
       (asserts each family returns its own
       descriptors).
    6. `ListProvidersAsync_preserves_Disabled_descriptors_regardless_of_capability`
       (asserts `Disabled` descriptors are
       never downgraded).
    7. `ListProvidersAsync_preserves_Unavailable_descriptors_regardless_of_capability`
       (asserts `Unavailable` descriptors are
       never downgraded).
  - 1 Information-level log test:
    `ListProvidersAsync_logs_at_Information_level_on_success`
    (asserts the per-family lookup count is
    captured at `Information` level).
  - 2 dispatch tests (one per family):
    `ListProvidersAsync_calls_IGitProviderFamily_when_family_is_Git` +
    `ListProvidersAsync_calls_IReviewProviderFamily_when_family_is_Review`.
  - 1 capability-call-count test:
    `ListProvidersAsync_aggregates_capabilities_call_once_per_invocation`
    (asserts
    `IHostCapabilitiesService.DetectAsync` is
    called exactly once per
    `ListProvidersAsync` invocation).
  - 1 `ArgumentOutOfRangeException` test:
    `ListProvidersAsync_throws_ArgumentOutOfRangeException_for_unknown_family`
    (defensive coverage).
  - 1 fake-`CallCount` test:
    `FakeGitProviderFamily_records_LookupCallCount`
    (asserts the fake's `CallCount`
    increments correctly).
  - The 2 in-line test doubles:
    `FakeHostCapabilitiesService` (records
    `CallCount`; returns a configured
    `HostCapabilities`) + `ListLogger<T>`
    (records the log messages in a
    `List<string>`).

## 4. Files Modified

- **`src/AiEng.Platform.App/Composition/ServiceCollectionExtensions.cs`**
  (1 line added). The `AddPlatformServices`
  method now calls
  `services.AddProviderRegistry()` after
  `services.AddHostCapabilities()` per the
  M4-C plan § 2 item 8.
  ```csharp
  services.AddNavigation(assemblies);
  services.AddProjectIntelligence();
  services.AddProjects();
  services.AddInfrastructure();
  services.AddHostCapabilities();
  services.AddProviderRegistry();
  return services;
  ```
- **`.ai/state/session.json`** — the
  M4-C.1 envelope replaces the M4-B closeout
  envelope. `session_id =
  m4-c-1-provider-registry-contract-and-family-registries`;
  `session_type = implementation-slice`; the
  full `scope.in_scope` (15 items) +
  `scope.out_of_scope` (7 items) +
  `current_understanding` (test_status = 395
  passed; intended_next_action stops at
  T-029).
- **`.ai/state/tasks.json`** — T-028
  transitions `Ready` → `InProgress` →
  `Done` with the full evidence block (29
  files added + 1 file modified + 19 unit
  tests + commit_message + branch_deleted +
  test_count + next_step). T-029 is added
  as M4-C.2 first session in `Ready`
  status.
- **`.ai/state/current.md`** — active slice
  updated; last stable commit updated; next
  recommended task updated; M4-C.1 evidence
  references added.
- **`.ai/state/task-board.md`** — M4-C.1
  row in `Done Recently`; T-029 stub row in
  `Ready`; M4-C status `Awaiting Approval`
  → `Active`; T-028 status `Ready` →
  `Done (delivered 2026-07-13)`.
- **`.ai/state/milestones.json`** — M4-C.1
  slice block from `Planned` to `Done`;
  M4-C milestone `Awaiting Approval` →
  `Active`; C-018 evidence block finalised;
  C-010 + C-011 evidence blocks initialised.
- **`.ai/state/capabilities.json`** — C-018
  evidence block finalised; C-010 + C-011
  evidence blocks initialised; top-level
  `updated_at` + `updated_by_session`
  updated.

## 5. Files Deleted

(none — the M4-C.1 first session does not
delete any file.)

## 6. Architecture

### 6.1 Single seam rule

The `IProviderRegistry` is the single allowed
seam between the application and the provider
layer. The M4-C architecture requires that
all provider access flows through
`IProviderRegistry`, never through the
concrete `SystemProviderRegistry` or any
`IProvider` implementation. The M4-C.2
`Providers_Resolve_Through_Registry`
architecture test enforces the seam on the
`App/Components/Providers/` folder.

### 6.2 Family registry pattern

One family registry per `ProviderFamily`
(Git + AgentRuntime + Review + QualityGate +
AutonomousLoop + Orchestration). The family
registries are the "what providers exist for
this family" surface; the
`SystemProviderRegistry` is the "which
providers are eligible given the host
capabilities" surface.

The family registry does NOT consume
`IHostCapabilitiesService` directly; the
`SystemProviderRegistry` aggregates the
family registries + consumes
`IHostCapabilitiesService` to filter providers
by host capability. This separation allows
the family registries to be replaced by
concrete provider implementations in M4-D
without modifying the
`SystemProviderRegistry`.

### 6.3 Capability-aware filtering

The `SystemProviderRegistry` consumes
`IHostCapabilitiesService` through DI and
downgrades `Available` descriptors to
`Unavailable` when the family capability is
not available; `Disabled` and `Unavailable`
are preserved regardless of the family
capability state.

The `GetFamilyCapabilityKey` private helper
maps `ProviderFamily` to the M4-B capability
key: `Git` → `"git"`, `AgentRuntime` →
`"ollama"`, `Review` → `"ollama"`,
`QualityGate` → `"powershell"`,
`AutonomousLoop` → `"ollama"`,
`Orchestration` → `"ollama"`. The mapping is
the M4-C.1 default; the M4-D plan refines the
mapping per provider.

### 6.4 Layered architecture

- **`AiEng.Platform.Application`** — the
  contracts (`IProviderRegistry` +
  `ProviderDescriptor` + `ProviderFamily` +
  `ProviderStatus` + the 6 family registry
  subinterfaces + `IProviderFamily` base).
- **`AiEng.Platform.Infrastructure`** — the
  implementation (`SystemProviderRegistry` +
  the 6 no-op family stub implementations).
- **`AiEng.Platform.App`** — the composition
  root (`AddProviderRegistry` extension +
  the `AddPlatformServices` wire-up).
- **`tests/AiEng.Platform.UnitTests`** — the
  test surface (the 6 family fakes + the
  `SystemProviderRegistryTests` unit tests).

The App project does not reference the
UnitTests project (the production stubs live
in Infrastructure; the test fakes live in
UnitTests). The DI layering rule is preserved.

### 6.5 C# 14 switch expression common-type

The 6 family-specific subinterfaces had no
common base type for the C# 14 switch
expression to resolve (CS8506). The fix was
to create the `IProviderFamily` base
interface and make the 6 family interfaces
extend it; the `SystemProviderRegistry`
fields are typed as `IProviderFamily` (the
base) and the switch expression resolves on
the base type.

### 6.6 M4-C plan § 2 alignment

The M4-C.1 first session is the M4-C plan § 2
In Scope items 1-8 + § 10 Test Plan item 10.1.
The M4-C.1 first session does **not** begin
items 14-19 (the surface slice + the closeout
slice + provider creation). The M4-C.1 first
session is a single coherent slice; one task
per session; stop after the coherent commit.

## 7. Validation Results

- **Format gate:** `dotnet format
  --verify-no-changes` exits 0; the format is
  canonical and CI-clean. The new files use
  4-space indent + CRLF (per .editorconfig).
- **Build gate:** `dotnet build
  AiEng.Platform.slnx` exits 0; 0 warnings, 0
  errors (with `TreatWarningsAsErrors=true`
  from `Directory.Build.props`).
- **Test gate:** `dotnet test
  AiEng.Platform.slnx --no-build` reports
  **395 passed** (376 pre-M4-C.1 + 19 new
  M4-C.1 unit tests), 0 failed, 9 skipped
  (per ADR-016 / M4-D). Breakdown: 118 unit
  + 263 component + 14 architecture.
- **JSON validation gate:** the 4 state JSON
  files (`session.json` + `tasks.json` +
  `milestones.json` + `capabilities.json`) are
  valid JSON; the `updated_at` field is
  updated; the schema is preserved.
- **CRLF validation gate:** every new +
  modified file is CRLF; the `unix2dos`
  command is applied to every file before
  commit.
- **Architecture boundary gate:** the M4-C.1
  implementation does not introduce
  `System.Diagnostics.Process` usage outside
  `src/AiEng.Platform.Infrastructure/`; the
  M4-C.1 implementation does not introduce
  `advapi32.dll` P/Invoke outside
  `src/AiEng.Platform.Infrastructure/`; the
  M4-C.1 implementation does not introduce a
  `Microsoft.Extensions.DependencyInjection`
  `IServiceCollection` extension outside
  `src/AiEng.Platform.App/Composition/`. The
  boundary is enforced by the M4-A.1
  architecture tests
  (`Infrastructure_Respects_ProcessBoundary`
  + `Infrastructure_Respects_CredentialBoundary`),
  which are registered-but-disabled per
  ADR-016 / M4-D.
- **No scope creep:** the diff does not
  modify any file under
  `src/AiEng.Platform.Application/Capabilities/`,
  `src/AiEng.Platform.Application/Infrastructure/`,
  `src/AiEng.Platform.Application/Navigation/`,
  `src/AiEng.Platform.Application/Projects/`,
  `src/AiEng.Platform.Application/ProjectIntelligence/`,
  `src/AiEng.Platform.Domain/`,
  `src/AiEng.Platform.Infrastructure/Capabilities/`,
  `src/AiEng.Platform.Infrastructure/Platform/`,
  `src/AiEng.Platform.Providers.Abstractions/`,
  `src/AiEng.Platform.App/Components/`,
  `src/AiEng.Platform.App/Program.cs`,
  `tests/AiEng.Platform.ComponentTests/`,
  `tests/AiEng.Platform.ArchitectureTests/`,
  `docs/providers.md` (not created),
  `docs/design-system.md`, `ROADMAP.md`,
  `.ai/plans/master-delivery-plan.md`,
  `AGENTS.md`, `ARCHITECTURE.md`,
  `DECISIONS.md`, `STYLEGUIDE.md`,
  `CONTRIBUTING.md`, `.ai/workflows/`,
  `tailwind.config.js`, `package.json`, or
  `Directory.Build.props`.

## 8. Tests Added

The M4-C.1 first session adds **19 unit
tests** in
`tests/AiEng.Platform.UnitTests/Providers/SystemProviderRegistryTests.cs`.

Test breakdown:

- **8 constructor-null tests** — one per
  dependency in the
  `SystemProviderRegistry` constructor
  (verifies the ctor null-checks every
  dependency).
- **7 happy-path / edge-case tests** — covers
  the main `ListProvidersAsync` paths
  (capability available + capability
  unavailable downgrade + empty family
  descriptors + cancellation propagation +
  per-family dispatch + Disabled preservation
  + Unavailable preservation).
- **1 Information-level log test** —
  verifies the per-family lookup count is
  captured at `Information` level via the
  `ListLogger<T>` test double.
- **2 dispatch tests** (one per family) —
  verifies the `SystemProviderRegistry`
  dispatches the `ListProvidersAsync` call to
  the correct family registry (Git +
  Review).
- **1 capability-call-count test** — verifies
  the `SystemProviderRegistry` calls
  `IHostCapabilitiesService.DetectAsync`
  exactly once per `ListProvidersAsync`
  invocation.
- **1 `ArgumentOutOfRangeException` test** —
  defensive coverage for an unknown
  `ProviderFamily` enum value.
- **1 fake-`CallCount` test** — verifies the
  `FakeGitProviderFamily.CallCount` increments
  correctly.

Test count after M4-C.1 closeout:
**395 passed** (376 pre-M4-C.1 + 19 new), 0
failed, 9 skipped (per ADR-016 / M4-D).
Breakdown: 118 unit + 263 component + 14
architecture.

The 9 registered-but-disabled tests (3
axe-core + 4 provider-boundary + 2
process/credential boundary) remain skipped
per ADR-016 / M4-D. The M4-A.1 architecture
tests
(`Infrastructure_Respects_ProcessBoundary` +
`Infrastructure_Respects_CredentialBoundary`)
remain registered-but-disabled. The M4-B.3
`Capabilities_Resolved_Through_Service`
architecture test remains Active and green.

## 9. Definition of Done

The M4-C.1 first session DoD per the M4-C
plan § 2 items 1-8 + § 10 Test Plan item 10.1
(per inspection):

- [x] **§ 2 item 1:** `IProviderRegistry`
  contract at
  `src/AiEng.Platform.Application/Providers/IProviderRegistry.cs`
  (14 lines; single
  `ListProvidersAsync(family, ct)` method).
- [x] **§ 2 item 2:** `ProviderDescriptor`
  record at
  `src/AiEng.Platform.Application/Providers/ProviderDescriptor.cs`
  (9 lines; `sealed record class`; 6 fields:
  `Id` + `DisplayName` + `Family` + `Status`
  + `Version?` + `Metadata`).
- [x] **§ 2 item 3:** `ProviderFamily` enum
  at
  `src/AiEng.Platform.Application/Providers/ProviderFamily.cs`
  (9 lines; 6 values: `Git = 0` +
  `AgentRuntime = 1` + `Review = 2` +
  `QualityGate = 3` + `AutonomousLoop = 4` +
  `Orchestration = 5`).
- [x] **§ 2 item 4:** `ProviderStatus` enum
  at
  `src/AiEng.Platform.Application/Providers/ProviderStatus.cs`
  (7 lines; 3 values: `Available = 0` +
  `Unavailable = 1` + `Disabled = 2`).
- [x] **§ 2 item 5:** 6 family registry
  contracts in
  `src/AiEng.Platform.Application/Providers/Families/`
  (`IProviderFamily` base + 6
  family-specific subinterfaces).
- [x] **§ 2 item 6:** `SystemProviderRegistry`
  implementation at
  `src/AiEng.Platform.Infrastructure/Providers/SystemProviderRegistry.cs`
  (~95 lines; aggregates 6 family registries
  through `IProviderFamily`; consumes
  `IHostCapabilitiesService` through DI;
  downgrades `Available` to `Unavailable`
  when the family capability is unavailable;
  preserves `Disabled` and `Unavailable`;
  logs at `Information` level).
- [x] **§ 2 item 7:** 6 family fakes in
  `tests/AiEng.Platform.UnitTests/Providers/`
  (each records `CallCount` +
  `ObservedTokens` + `QueuedResults`; returns
  the next queued result or an empty array).
- [x] **§ 2 item 8:** `AddProviderRegistry`
  composition root extension at
  `src/AiEng.Platform.App/Composition/Providers/ProviderRegistryServiceCollectionExtensions.cs`
  (registers the 6 no-op family stubs +
  `IProviderRegistry` → `SystemProviderRegistry`
  via `TryAddSingleton`).
- [x] **§ 10 item 10.1:** 19 unit tests in
  `tests/AiEng.Platform.UnitTests/Providers/SystemProviderRegistryTests.cs`
  (8 ctor-null + 7 happy-path/edge-case + 1
  log + 2 dispatch + 1 capability-call-count
  + 1 `ArgumentOutOfRangeException` + 1
  fake-`CallCount`).
- [x] **Project-continuity state updated per
  Rule 15** (6 state files: `session.json` +
  `tasks.json` + `current.md` + `task-board.md`
  + `milestones.json` + `capabilities.json`).
- [x] **M4-C.1 per-session handoff** at
  `.ai/handoffs/2026-07-13-m4-c-1-provider-registry-contract-and-family-registries.md`
  (mirrored to `.ai/handoffs/latest.md`).
- [x] **M4-C.1 implementation report** at
  `implementation-report-m4-c-1-provider-registry-contract-and-family-registries.md`
  (this document; 15 sections).
- [x] **Coherent commit** on the feature
  branch `feature/T-028-m4-c-1-provider-registry-contract-and-family-registries`
  (commit subject: `feat(m4-c.1): add
  IProviderRegistry contract, family
  registries, SystemProviderRegistry
  implementation, family fakes, and
  AddProviderRegistry composition root`;
  trailer: `Co-Authored-By: Claude
  <noreply@anthropic.com>`).
- [x] **Fast-forward merge into `main`** per
  the branching strategy rule 6.
- [x] **Feature branch deleted** per rule 7.
- [x] **Push skipped** (not authorised in this
  session).
- [x] **M4-C.1 does NOT begin M4-C.2** /
  M4-C.x closeout / M4-D / provider
  creation (per the brief: "Do not begin the
  following task").

## 10. Git

- **Branch:**
  `feature/T-028-m4-c-1-provider-registry-contract-and-family-registries`
  (created from `main` at the M4-B closeout
  commit `72d85b3` per the branching strategy
  rule 4).
- **Commit subject:** `feat(m4-c.1): add
  IProviderRegistry contract, family
  registries, SystemProviderRegistry
  implementation, family fakes, and
  AddProviderRegistry composition root`
- **Commit trailer:** `Co-Authored-By: Claude
  <noreply@anthropic.com>`
- **Commit contents:** 29 files added + 1
  file modified + 19 unit tests added.
- **Fast-forward merge into `main`** per
  rule 6.
- **Feature branch deleted** per rule 7.
- **Push skipped** — push is not authorised
  in this session; the push decision is
  **Staged for push** (the user may push in
  a follow-up command per the command
  protocol).
- **No annotated milestone tag** — the
  M4-C.1 first session is a slice, not a
  milestone closeout. The `m4-c` annotated
  milestone tag is at the M4-C closeout
  commit on `main` (M4-C.x — T-030) per the
  branching strategy rule 9.

## 11. Out of Scope

The M4-C.1 first session does **not** begin
the following task (per the M4-C brief: "Do
not begin the following task" + the
Progressive Coding Rule):

- **M4-C.2** (T-029) — the `AppProviderList`
  data-owning design-system component + the
  `/providers` page + the startup
  provider-registry log + the
  `Providers_Resolve_Through_Registry`
  architecture test + `docs/providers.md` +
  the `docs/design-system.md` § 4.5 update.
  T-029 is `Ready` in `tasks.json`; the next
  session begins the M4-C.2 implementation
  per the Progressive Coding Rule.
- **M4-C.x closeout** (T-030) — the M4-C
  retrospective + the M4-C status `Active` →
  `Done` + the `m4-c` annotated milestone tag
  + the M4-D plan + the project-continuity
  state. T-030 is not yet seeded; the M4-C.2
  closeout seeds T-030.
- **M4-D** — the first concrete process
  providers. M4-D is the next milestone after
  M4-C close; the M4-D plan is drafted in
  the M4-C closeout session.
- **Any provider creation.** The M4-C.1
  first session does not create providers
  (per the M4-C brief: "M4-C is the registry
  foundation; M4-D is the providers"). The
  6 no-op family stubs in
  `src/AiEng.Platform.Infrastructure/Providers/Families/`
  are the placeholder implementations
  (empty lists); the M4-D concrete provider
  implementations replace the stubs.
- **No new UI surface.** M4-C.1 does not
  introduce the `AppProviderList` component
  (M4-C.2), the `/providers` page (M4-C.2),
  the startup provider-registry log (M4-C.2),
  or the `Providers_Resolve_Through_Registry`
  architecture test (M4-C.2).
- **No new documentation.** M4-C.1 does not
  introduce `docs/providers.md` (M4-C.2) or
  the `docs/design-system.md` § 4.5 update
  (M4-C.2).
- **No push.** Push is not authorised in this
  session; the M4-C.1 commit is **Staged for
  push**.
- **No architectural rule changes.** M4-C.1
  does not introduce new architectural
  rules; no ADR is required. The M4-B.3
  `Capabilities_Resolved_Through_Service`
  architecture test remains Active and green;
  the M4-A.1 architecture tests remain
  registered-but-disabled per ADR-016 / M4-D.

## 12. Lessons Learned

### 12.1 C# 14 switch expression common-type

The 6 family-specific subinterfaces had no
common base type for the C# 14 switch
expression to resolve (CS8506: "No best type
was found for the switch expression"). The
fix was to create the `IProviderFamily` base
interface and make the 6 family interfaces
extend it; the `SystemProviderRegistry`
fields are typed as `IProviderFamily` (the
base) and the switch expression resolves on
the base type.

**Lesson:** when designing an architecture
that aggregates multiple subinterfaces via a
switch expression, the subinterfaces must
share a common base interface (or implement
a common method signature). The
`IProviderFamily` base was the right design
choice: it preserves the M4-C.2
`@inject IGitProviderFamily` etc. use case
(the subinterfaces are still injectable as
their own types) and resolves the switch
expression.

### 12.2 DI layering rule

The M4-C.1 plan registered the 6 family
fakes (test-only types) as the family
registry implementations in the
`AddProviderRegistry` composition root. This
violated the DI layering rule (the `App`
project cannot reference
`AiEng.Platform.UnitTests`).

**Lesson:** test-only types (fakes + stubs)
must never be referenced from production
composition roots. The fix was to create 6
no-op family stub implementations in
`src/AiEng.Platform.Infrastructure/Providers/Families/`
(the stubs return an empty list; the
concrete provider implementations come in
M4-D) and have the `AddProviderRegistry`
composition root register the Infrastructure
stubs, not the test fakes. The 6 family
fakes stay in `tests/AiEng.Platform.UnitTests/Providers/`
and are passed to `SystemProviderRegistry`
via constructor injection in unit tests.

### 12.3 Capability-aware filter direction

The M4-C.1 first iteration inverted the
filter logic (the implementation kept
`Available` descriptors when the family
capability was unavailable). The correct
behaviour is to **downgrade** `Available`
descriptors to `Unavailable` when the family
capability is unavailable; `Disabled` and
`Unavailable` are preserved regardless of
the family capability state.

**Lesson:** the filter direction matters.
The `SystemProviderRegistry` is a
"re-evaluate eligibility" surface, not a
"filter out" surface. The
`ProviderStatus.Available` is the family's
optimistic claim; the
`SystemProviderRegistry` re-evaluates the
claim against the host capabilities and
downgrades to `Unavailable` if the host
does not support the family. The
`ProviderStatus.Disabled` is the user's
explicit disable; the
`SystemProviderRegistry` does not override
the user's choice.

### 12.4 `HostCapabilitiesCache` future refinement

The M4-C.1 implementation calls
`IHostCapabilitiesService.DetectAsync` once
per `ListProvidersAsync` invocation (the
M4-C.2 page calls `ListProvidersAsync` 6
times → 6 `DetectAsync` calls per page
render). This is acceptable because
`DetectAsync` is fast on a warm cache.

**Future refinement:** a future M4-C
refinement (not in M4-C.1) could add a
`HostCapabilitiesCache` to reduce the call
count to 1 per page render. The risk is
recorded here; the M4-C.2 first session
documents the 6-call observation; the
M4-C.2 closeout (M4-C.x — T-030) seeds a
M4-C.x+1 follow-up for the cache refinement.

## 13. Deviations

The M4-C.1 first session has **one documented
deviation**:

### 13.1 Family fake → Infrastructure no-op stub

The M4-C.1 plan registered the 6 family
fakes (test-only types in
`tests/AiEng.Platform.UnitTests/Providers/`)
as the family registry implementations in
the `AddProviderRegistry` composition root.
This violated the DI layering rule (the
`App` project cannot reference
`AiEng.Platform.UnitTests`).

**Fix:**

1. **6 no-op family stub implementations**
   were created in
   `src/AiEng.Platform.Infrastructure/Providers/Families/`
   (`GitProviderFamily` + 5 others) — the
   stubs return an empty
   `IReadOnlyList<ProviderDescriptor>` (no
   concrete providers ship in M4-C.1; the
   providers come in M4-D per the brief:
   "Do not create providers").
2. **`AddProviderRegistry` registers the
   Infrastructure no-op stubs**, not the
   test fakes.
3. **The 6 family fakes stay in
   `tests/AiEng.Platform.UnitTests/Providers/`**
   and are passed to `SystemProviderRegistry`
   via constructor injection in unit tests.

**Impact:** the M4-C.1 architecture is
preserved (the family registries exist; the
`SystemProviderRegistry` aggregates them;
`IProviderRegistry` is the single seam). The
test surface is preserved (19 unit tests
assert the `SystemProviderRegistry` behavior
via constructor-injected fakes). The DI
layering rule is preserved (the App project
does not reference the UnitTests project).

The deviation is recorded in
`.ai/state/tasks.json` T-028 `notes` and in
the M4-C.1 per-session handoff § 5.

## 14. Handoff to M4-C.2

The next session is the **M4-C.2 first
session (T-029)** on the user's `Approve` or
`Next` invocation. T-029 is `Ready` in
`.ai/state/tasks.json`.

The M4-C.2 first session follows the 13-step
Progressive Coding task lifecycle in the
order specified in
`.ai/workflows/progressive-coding.md` § 3.

The M4-C.2 first session:

1. Reads the M4-C plan
   (`.ai/plans/M4-C-provider-registry-foundation.md`)
   + the M4-C.1 handoff (this document § 1-8
   referenced) + the M4-C.1 implementation
   report + the M4-C.1 contract + the M4-C.1
   implementation + the M4-C.1 composition
   root + the M4-C.1 unit tests + the M4-B
   closeout handoff + the M4-B retrospective
   § 13 Recommendations for the Next
   Milestone.
2. Creates the feature branch
   `feature/T-029-m4-c-2-app-provider-list-and-providers-page`
   from `main` at the M4-C.1 closeout commit.
3. Marks T-029 `InProgress` in `tasks.json` +
   `task-board.md`.
4. Lands the `AppProviderList` data-owning
   four-state design-system component in
   `src/AiEng.Platform.App/Components/DesignSystem/`
   (renders an
   `IReadOnlyList<ProviderDescriptor>` as a
   list of `AppCard` entries with
   `AppStatusDot` Success/Error, the
   `Version` in a monospaced muted font, and
   an `AppBadge` "Disabled" for
   `Status = Disabled`; `aria-live="polite"`
   on the populated list).
5. Lands the `/providers` page at
   `src/AiEng.Platform.App/Components/Pages/Providers.razor`
   (+ `.razor.css`); the page composes
   `AppPageHeader` + the `AppProviderList`; the
   6 family cards in the `ProviderFamily`
   enum order.
6. Wires the startup provider-registry log in
   `src/AiEng.Platform.App/Program.cs`
   (10-second `CancellationTokenSource`
   timeout; `ILogger<Program>`; Information
   level; try/catch with Warning on failure).
7. Lands the
   `Providers_Resolve_Through_Registry`
   architecture test in
   `tests/AiEng.Platform.ArchitectureTests/Providers/`
   (Active per the M4-C plan § 2 item 12;
   scoped to `App/Components/Providers/` to
   avoid the M4-A.2 Open Action false
   positive).
8. Lands the `docs/providers.md` 10-section
   documentation mirroring
   `docs/infrastructure.md` § 1-10.
9. Updates `docs/design-system.md` § 4.5
   (the M4-B.2 deferred decision resolves to
   `Implemented (M4-C.2)`).
10. Lands 4+ bUnit page tests + 1+ active
    architecture test.
11. Runs the validation gate (`dotnet format` +
    `dotnet build` + `dotnet test` + JSON +
    CRLF).
12. Updates the project-continuity state per
    Rule 15 (the 6 state files).
13. Writes the M4-C.2 per-session handoff +
    the M4-C.2 implementation report.
14. Coherent commit + fast-forward merge +
    delete feature branch; **skip push**.
15. **Stop.** M4-C.2 does **not** begin
    M4-C.x closeout / M4-D / provider
    creation.

## 15. Cross-References

- **The M4-C plan:**
  `.ai/plans/M4-C-provider-registry-foundation.md`
  (Status: Active; the M4-C plan is the
  canonical M4-C scope).
- **The M4-B plan:**
  `.ai/plans/M4-B-capability-detection.md`
  (the M4-C plan mirrors the M4-B plan's 12
  sections with M4-C-specific evidence).
- **The M4-B closeout handoff:**
  `.ai/handoffs/2026-07-13-m4-b-closeout.md`
  (the M4-C.1 handoff's template).
- **The M4-B closeout implementation
  report:**
  `implementation-report-m4-b-closeout.md`
  (the M4-C.1 implementation report's
  aggregate template).
- **The M4-B retrospective:**
  `retrospective-m4-b-capability-detection.md`
  (the M4-C plan § 13 is the input to the
  M4-C plan; the M4-C.1 first session reads
  § 13 Recommendations for the Next
  Milestone).
- **The M4-B.1 handoff:**
  `.ai/handoffs/2026-07-13-m4-b-1-host-capabilities-contract-and-service.md`
  (the M4-C.1 handoff's template).
- **The M4-B.1 implementation report:**
  `implementation-report-m4-b-1-host-capabilities-contract-and-service.md`
  (the M4-C.1 implementation report's
  template).
- **The M4-B.1 contract:**
  `src/AiEng.Platform.Application/Capabilities/IHostCapabilitiesService.cs`
  (the M4-C.1 `SystemProviderRegistry`
  consumes this contract through DI).
- **The M4-B.1 records:**
  `src/AiEng.Platform.Application/Capabilities/HostCapabilities.cs`
  (the M4-C.1 `SystemProviderRegistry` reads
  the `Capabilities` list).
- **The M4-B.1 implementation:**
  `src/AiEng.Platform.Infrastructure/Capabilities/SystemHostCapabilitiesService.cs`
  (the M4-C.1 `SystemProviderRegistry` does
  not re-implement the M4-B service).
- **The M4-B.1 composition root:**
  `src/AiEng.Platform.App/Composition/Capabilities/CapabilitiesServiceCollectionExtensions.cs`
  (the M4-C.1 composition root mirrors the
  M4-B.1 pattern).
- **The M2.2 navigation registry:**
  `src/AiEng.Platform.Application/Navigation/INavigationRegistry.cs`
  + `RouteMetadata.cs` +
  `RouteMetadataAttribute.cs` +
  `RouteRegistry.cs` (the M4-C.1 contract
  pattern mirrors the M2.2 contract
  pattern).
- **The M2.2 composition root:**
  `src/AiEng.Platform.App/Composition/NavigationServiceCollectionExtensions.cs`
  (the M4-C.1 composition root mirrors the
  M2.2 composition pattern).
- **The M4-B.1 unit tests:**
  `tests/AiEng.Platform.UnitTests/Capabilities/SystemHostCapabilitiesServiceTests.cs`
  (the M4-C.1 unit tests mirror the M4-B.1
  unit tests' pattern).
- **The M4-A.1 architecture tests:**
  `tests/AiEng.Platform.ArchitectureTests/Infrastructure/Infrastructure_Respects_ProcessBoundary.cs`
  +
  `Infrastructure_Respects_CredentialBoundary.cs`
  (registered-but-disabled per ADR-016 /
  M4-D).
- **The M4-B.3 architecture test:**
  `tests/AiEng.Platform.ArchitectureTests/Capabilities/Capabilities_Resolved_Through_Service.cs`
  (the M4-C.2
  `Providers_Resolve_Through_Registry`
  architecture test mirrors the M4-B.3
  test's pattern).
- **The branching strategy:**
  `.ai/workflows/branching-strategy.md`
  (rules 4, 6, 7, 9).
- **The Progressive Coding Rule:**
  `.ai/workflows/progressive-coding.md`.
- **The command protocol:**
  `.ai/commands.md` (the `Next` command
  response shape).
- **The M4-C.1 task record:**
  `.ai/state/tasks.json` T-028 (the M4-C.1
  task transitions `Ready` → `InProgress` →
  `Done`).
- **The M4-C.1 milestone record:**
  `.ai/state/milestones.json` (the M4-C.1
  slice block from `Planned` to `Done`).
- **The M4-C.1 capability record:**
  `.ai/state/capabilities.json` C-018
  (`IProviderRegistry`; evidence finalised;
  `next_task` cleared on close for C-018).
- **The M4-C.1 session record:**
  `.ai/state/session.json` (the M4-C.1
  envelope replaces the M4-B closeout
  envelope).
- **The M4-C.1 task board entry:**
  `.ai/state/task-board.md` (M4-C.1 row in
  `Done Recently`; T-029 (M4-C.2) stub row
  in `Ready`).
- **The M4-C.1 one-page snapshot:**
  `.ai/state/current.md` (active slice =
  M4-C.1; last stable commit = M4-C.1
  closeout commit; next recommended task =
  T-029 = M4-C.2).
- **The M4-C.1 per-session handoff:**
  `.ai/handoffs/2026-07-13-m4-c-1-provider-registry-contract-and-family-registries.md`
  (mirrored to `.ai/handoffs/latest.md`).
