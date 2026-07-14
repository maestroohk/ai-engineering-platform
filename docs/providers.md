# Providers (M4-C)

> **The M4-C provider-registry documentation.** The M4-C.1 slice (delivered 2026-07-13) ships the `IProviderRegistry` contract + the `ProviderDescriptor` sealed record + the `ProviderFamily` + `ProviderStatus` enums + the `IProviderFamily` base interface + the 6 family-specific subinterfaces in `src/AiEng.Platform.Application/Providers/Families/` + the `SystemProviderRegistry` implementation in `src/AiEng.Platform.Infrastructure/Providers/` + the 6 no-op family stub implementations in `src/AiEng.Platform.Infrastructure/Providers/Families/` + the `AddProviderRegistry` composition root extension + the 6 family fakes + 19 unit tests. The M4-C.2 slice (delivered 2026-07-13, this document) ships the `AppProviderList` data-owning design-system component + the `/providers` page + the startup provider-registry log + the `Providers_Resolve_Through_Registry` Active architecture test + the `docs/design-system.md` § 4.5 update. M4-D provides the concrete process provider implementations that the M4-C registry resolves.

---

## 1. Goals

The M4-C provider-registry foundation exists to:

- **Expose a single provider-lookup seam.** The `IProviderRegistry` contract is the only seam the application reads providers through. The `Providers_Resolve_Through_Registry` architecture test (Active) enforces the rule. Every `.razor` and `.razor.cs` file under `src/AiEng.Platform.App/Components/Providers/` is scanned: no `RunToCompletionAsync` token (forbidden `IProcessRunner` usage), no `ICredentialVault` token (forbidden credential boundary), no `new SystemProviderRegistry` token (forbidden direct-instantiation escape hatch).
- **Decouple the registry from the family registries.** The `SystemProviderRegistry` aggregates the 6 family registries + consumes the M4-B `IHostCapabilitiesService` through DI to filter providers by host capability. The family registries are the "what providers exist for this family" surface; the registry is the "which providers are eligible given the host capabilities" surface. Family registries do NOT consume `IHostCapabilitiesService` directly.
- **Be the only consumer of `IHostCapabilitiesService` for provider filtering.** The `SystemProviderRegistry` is the only M4-C consumer. The page + the component consume the registry; the registry consumes the capability service; the capability service consumes the process + credential boundaries. M4-D concrete provider implementations register against the family registries (e.g., `IGitProviderFamily`).
- **Drive the `/providers` page.** The page is the user-visible surface; the startup log is the early signal. Both consume the contract; the contract consumes the implementation; the implementation consumes the M4-B boundary.

The M4-C.1 slice ships the contract + the records + the enums + the family registries + the implementation + the composition root. The M4-C.2 slice ships the design-system component + the page + the startup log + the architecture test + this documentation. M4-D provides the concrete provider implementations that the M4-C registry resolves.

---

## 2. Project Structure

The M4-C slices add code under five locations:

```
src/AiEng.Platform.Application/
  Providers/
    IProviderRegistry.cs
    ProviderDescriptor.cs
    ProviderFamily.cs
    ProviderStatus.cs
    Families/
      IProviderFamily.cs   (the base interface; the 6 subinterfaces share its signature)
      IGitProviderFamily.cs
      IAgentRuntimeProviderFamily.cs
      IReviewProviderFamily.cs
      IQualityGateProviderFamily.cs
      IAutonomousLoopProviderFamily.cs
      IOrchestrationProviderFamily.cs
src/AiEng.Platform.Infrastructure/
  Providers/
    SystemProviderRegistry.cs
    Families/
      GitProviderFamily.cs   (no-op stub; M4-D replaces with concrete implementations)
      AgentRuntimeProviderFamily.cs
      ReviewProviderFamily.cs
      QualityGateProviderFamily.cs
      AutonomousLoopProviderFamily.cs
      OrchestrationProviderFamily.cs
src/AiEng.Platform.App/
  Composition/
    Providers/
      ProviderRegistryServiceCollectionExtensions.cs
  Components/
    Providers/
      AppProviderList.razor
      AppProviderList.razor.cs
      AppProviderList.razor.css
      _Imports.razor
  Components/
    Pages/
      Providers.razor
      Providers.razor.css
  Program.cs   (the startup provider-registry log is inserted after LogHostCapabilitiesAsync)
```

The M4-C Application additions live alongside the M4-A `IPlatformInfo` contract + the M4-B `IHostCapabilitiesService` contract in the same project. The M4-C Infrastructure additions live alongside the M4-B `SystemHostCapabilitiesService` implementation. The App additions compose all four (M4-A + M4-B + M4-C.1 + M4-C.2) through DI.

---

## 3. The `IProviderRegistry` Contract

`src/AiEng.Platform.Application/Providers/IProviderRegistry.cs` defines the seam:

```csharp
namespace AiEng.Platform.Application.Providers;

public interface IProviderRegistry
{
    Task<IReadOnlyList<ProviderDescriptor>> ListProvidersAsync(
        ProviderFamily family,
        CancellationToken cancellationToken = default);
}
```

The contract is async; it accepts a `ProviderFamily` enum value + a `CancellationToken`; it returns the typed `ProviderDescriptor` records for the family, filtered by host capability. The default `CancellationToken` lets the page-level call omit the parameter; the startup log supplies a 10-second `CancellationTokenSource` to bound the startup budget.

The contract is the single DI seam for provider lookup; no `App/Components/` code may bypass the seam. The `Providers_Resolve_Through_Registry` Active architecture test enforces the rule.

---

## 4. The `ProviderDescriptor` Record and the Enums

`src/AiEng.Platform.Application/Providers/ProviderDescriptor.cs` defines the data envelope:

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

The `ProviderDescriptor` record carries the full per-provider envelope: an `Id` (the stable identifier across lookups; e.g., `git-cli`, `ollama`), a `DisplayName` (the human-readable name), a `Family` (the `ProviderFamily` enum value), a `Status` (the `ProviderStatus` enum value), an optional `Version` string, and a `Metadata` dictionary for per-descriptor extension (e.g., `Path`, `Command`, `Args`). The M4-C.1 default metadata is empty; M4-D concrete provider implementations populate the `Metadata` with provider-specific keys.

`src/AiEng.Platform.Application/Providers/ProviderFamily.cs` defines the 6 family values:

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

The 6 families map to the 6 capability families in the M4-B `IHostCapabilitiesService` contract (the M4-B capability keys `git` / `ollama` / `powershell` / `wsl` / `wt` / `bash` map to provider family providers per the M4-D plan; M4-C is family-agnostic). The `SystemProviderRegistry.GetFamilyCapabilityKey` helper maps each `ProviderFamily` to the M4-B capability key (`Git` → `git`, `AgentRuntime` → `ollama`, `Review` → `ollama`, `QualityGate` → `powershell`, `AutonomousLoop` → `ollama`, `Orchestration` → `ollama`).

`src/AiEng.Platform.Application/Providers/ProviderStatus.cs` defines the 3 status values:

```csharp
namespace AiEng.Platform.Application.Providers;

public enum ProviderStatus
{
    Available = 0,
    Unavailable = 1,
    Disabled = 2,
}
```

The 3 states map to the M4-B `HostCapability.Available` + `HostCapability.CredentialAvailable` flags. A provider is `Available` when the host capability is available; `Unavailable` when the host capability is not available (the `SystemProviderRegistry` downgrades `Available` to `Unavailable` when the family capability is not available); `Disabled` when the host capability is available but the provider is disabled by configuration (M4-C.2 does not implement disable; the enum is forward-compatible with a future M4-D `Enabled` configuration field).

---

## 5. Family Registries

`src/AiEng.Platform.Application/Providers/Families/` defines 1 base interface + 6 family-specific subinterfaces:

```csharp
namespace AiEng.Platform.Application.Providers.Families;

public interface IProviderFamily
{
    Task<IReadOnlyList<ProviderDescriptor>> ListProvidersAsync(CancellationToken cancellationToken = default);
}

public interface IGitProviderFamily : IProviderFamily
{
}

public interface IAgentRuntimeProviderFamily : IProviderFamily
{
}

public interface IReviewProviderFamily : IProviderFamily
{
}

public interface IQualityGateProviderFamily : IProviderFamily
{
}

public interface IAutonomousLoopProviderFamily : IProviderFamily
{
}

public interface IOrchestrationProviderFamily : IProviderFamily
{
}
```

The 6 family-specific subinterfaces inherit from `IProviderFamily` to share the `ListProvidersAsync` signature. The `SystemProviderRegistry` constructor-injects the 6 subinterface types + stores them as `IProviderFamily` fields + uses a switch expression on `ProviderFamily` to resolve the family registry. This design resolves the C# switch-expression common-type problem (the 6 subinterfaces share the same method signature; the implementation resolves them through the base interface).

Each family registry has a single `ListProvidersAsync` method that returns the available `ProviderDescriptor`s for the family. The family registry does NOT consume `IHostCapabilitiesService` directly; the `SystemProviderRegistry` aggregates the family registries + consumes `IHostCapabilitiesService` to filter providers by host capability. (This separation is per the M4-C architecture: the family registries are the "what providers exist for this family" surface; the registry is the "which providers are eligible given the host capabilities" surface.)

The M4-C.1 no-op family stub implementations in `src/AiEng.Platform.Infrastructure/Providers/Families/` (e.g., `GitProviderFamily`, `AgentRuntimeProviderFamily`) return an empty `IReadOnlyList<ProviderDescriptor>`. M4-D replaces the stubs with concrete provider implementations that resolve the actual `ProviderDescriptor` instances.

---

## 6. The `AppProviderList` Component

`src/AiEng.Platform.App/Components/Providers/AppProviderList.razor` (+ .razor.cs + .razor.css) is the M4-C.2 data-owning four-state design-system component. It accepts:

- `Providers` (`IReadOnlyList<ProviderDescriptor>`, `[EditorRequired]`) — the source data.
- `IsLoading` (`bool`) — when true, renders the `Loading` slot or the default `<AppLoading>` fallback.
- `ErrorMessage` + `ErrorCode` + `CorrelationId` (`string?`) — when `ErrorMessage` is non-empty, renders the `Error` slot or the default `<AppErrorState>` fallback.
- `Loading` + `Empty` + `Error` + `Populated` (`RenderFragment?`) — the four data-owning slots per `docs/design-system.md` § 5.4.
- `AdditionalAttributes` — the standard `CaptureUnmatchedValues` splat.

The populated list is rendered as an `<AppStack>` of `<div class="app-provider-list-item">` entries (one per `ProviderDescriptor`). Each entry wraps an `<AppCard>` with:

- An `<AppStatusDot>` in the header — `Success` for `Available`, `Error` for `Unavailable`, `Neutral` for `Disabled` (the status is mapped to the `AppStatusDotVariant` enum in the component code).
- An `<AppBadge Variant="Neutral" Size="Small">` "Disabled" badge for `Status = Disabled`.
- The `DisplayName` as the primary text in the header.
- The `Id` as a monospaced tag in the meta row.
- The `Version` as a monospaced tag in the meta row (or a muted "version unknown" placeholder when `Version` is `null`).
- The `Metadata` as a small `<AppKeyValueList Format="Code">` when non-empty.

The populated container has `role="list"` + `aria-live="polite"` for screen-reader accessibility. The component is the M4-C counterpart of the M4-B.2 `AppCapabilityList`.

---

## 7. The `/providers` Page

`src/AiEng.Platform.App/Components/Pages/Providers.razor` (+ .razor.css) is the M4-C.2 user-visible surface. The page:

- Declares `@page "/providers"` and `@attribute [RouteMetadata("/providers", "Providers", Order = 5, ShowInSidebar = true, Icon = "◇", Description = "Available providers grouped by family; filtered by host capability.")]`. The `Order = 5` follows the M2.4 `Order = 0` (Dashboard) and M3 `Order = 1` (Projects) + the M4-B.3 `Order = 4` (Diagnostics) precedence.
- Inherits `@layout AppLayout` + `@rendermode InteractiveServer` to match the M2.4 + M3 + M4-B.3 page patterns.
- Injects `IProviderRegistry Service` (the single allowed seam for provider lookup) and `IHostCapabilitiesService Capabilities` (the M4-B capability service for the host-metadata context) and `IPlatformInfo PlatformInfo` (the M4-A platform-info accessor for the metadata block). The `Providers_Resolve_Through_Registry` architecture test allows `IHostCapabilitiesService` (it is the host-metadata context accessor, not the process boundary); the test forbids `IProcessRunner` + `ICredentialVault` + `new SystemProviderRegistry`.
- Calls `Service.ListProvidersAsync(family, cts.Token)` in `LoadAsync` for each of the 6 `ProviderFamily` enum values. On success, the per-family result is stored in `_providersByFamily`; on failure, the per-family result is an empty list and the page renders a per-family error state (graceful degradation). On top-level failure, `_error` + `_errorCode` are populated.
- Calls `Capabilities.DetectAsync(ct)` to populate `_capabilities` for the host-metadata block.
- Renders an `<AppPageHeader>` with the title "Providers", a description, and an `Actions` slot holding the Refresh `<AppButton>` (testid `refresh-providers`, `Variant="Outline"`, with `Loading` flipped while the call is in flight).
- Renders 6 `<AppCard>`s, one per `ProviderFamily` enum value (Git + AgentRuntime + Review + QualityGate + AutonomousLoop + Orchestration), each containing an `<AppProviderList>` populated with the providers for that family.
- Renders a host-metadata `<AppCard>` ("Host metadata") with a `<AppKeyValueList>` of the Detected at, Data directory, Config directory, Is Windows host items (only when `_capabilities` is non-null).

The scoped CSS adds a `1.25rem` `margin-top` to the second `AppCard` so the cards have vertical breathing room. The design system update adds the `AppProviderList` row to `docs/design-system.md` § 4.5 in `Implemented (M4-C.2)` status.

---

## 8. Composition Root

`src/AiEng.Platform.App/Composition/Providers/ProviderRegistryServiceCollectionExtensions.cs` defines the M4-C.1 `AddProviderRegistry` extension method. The chain in `src/AiEng.Platform.App/Composition/ServiceCollectionExtensions.cs` is:

```
AddNavigation
  → AddProjectIntelligence
  → AddProjects
  → AddInfrastructure
  → AddHostCapabilities
  → AddProviderRegistry
```

`AddProviderRegistry` uses `TryAddSingleton<IProviderRegistry, SystemProviderRegistry>()` so the registration is idempotent (a future test that overrides the registry with a fake does not collide). The M4-C.1 composition root registers the 6 no-op family stub implementations in `src/AiEng.Platform.Infrastructure/Providers/Families/` (the App project cannot reference `AiEng.Platform.UnitTests`; the test fakes are passed to `SystemProviderRegistry` via constructor injection in the unit tests). M4-D replaces the no-op stubs with concrete provider implementations (the `TryAddSingleton` "first wins" semantics allow the M4-D composition root to override the stubs).

The startup provider-registry log in `src/AiEng.Platform.App/Program.cs` resolves `IProviderRegistry` from `app.Services` after `app.Build()` and after the M4-B `LogHostCapabilitiesAsync` call. The block iterates the 6 `ProviderFamily` enum values and calls `Service.ListProvidersAsync(family, cts.Token)` for each; on success, the per-family provider count is logged at `Information` level (e.g., `"Provider registry report: Git=0; AgentRuntime=0; Review=0; QualityGate=0; AutonomousLoop=0; Orchestration=0"`). The block is wrapped in a `try/catch` that logs failures at `Warning` level; the startup must not fail if the registry lookup fails.

---

## 9. Tests

The M4-C test inventory:

- **M4-C.1 unit tests** (`tests/AiEng.Platform.UnitTests/Providers/SystemProviderRegistryTests.cs`): 19 tests covering the `SystemProviderRegistry` constructor (8 ctor-null tests for the 8 dependencies), `ListProvidersAsync` behaviour (7 happy-path + edge-case tests), the dispatch to the 6 family registries (2 dispatch tests), the capability-call-count (1 test), the `ArgumentOutOfRangeException` for unknown families (1 test), the `LogInformation` log test (1 test), and the `FakeGitProviderFamily.CallCount` test (1 test).
- **M4-C.2 bUnit component tests** (`tests/AiEng.Platform.ComponentTests/Components/Providers/AppProviderListTests.cs`): 13 tests for the `AppProviderList` covering the four data-owning states (Loading, Empty, Error, Populated) + the slot overrides + the accessibility attributes + the per-descriptor fields (DisplayName, StatusDot, Disabled badge, Version, Metadata).
- **M4-C.2 bUnit page tests** (`tests/AiEng.Platform.ComponentTests/Pages/ProvidersPageTests.cs`): 5 tests covering the page-level wiring (`OnInitializedAsync` calls `ListProvidersAsync` for the 6 families, the page renders the 6 family cards + the host metadata, the Refresh button re-runs `ListProvidersAsync`, the page renders the `<AppProviderList>` items per family).
- **M4-C.2 architecture test** (`tests/AiEng.Platform.ArchitectureTests/Providers/Providers_Resolve_Through_Registry.cs`): 2 Active tests. The first asserts `Providers.razor` contains `@inject IProviderRegistry` and does not contain `RunToCompletionAsync` / `ICredentialVault` / `new SystemProviderRegistry`. The second scans every `.razor` + `.razor.cs` file under `src/AiEng.Platform.App/Components/Providers/` for the same forbidden tokens. The test is scoped to the `Providers` folder to avoid the M4-A.2 Open Action + M4-B.3 `Diagnostics.razor` + the `/providers` page's `IHostCapabilitiesService` injection false positives.

Total tests after M4-C.2: 405+ passed, 0 failed, 9 skipped (per ADR-016 / M4-D).

---

## 10. Out of Scope

The M4-C slices do **not** include:

- **Concrete provider creation.** Per the M4-C brief: "M4-C is the registry foundation; M4-D is the providers" — the M4-C family registry stubs in `src/AiEng.Platform.Infrastructure/Providers/Families/` return an empty `IReadOnlyList<ProviderDescriptor>`. The M4-D plan introduces the concrete `Providers.Git` / `Providers.AgentRuntime` / `Providers.Review` / `Providers.QualityGate` / `Providers.AutonomousLoop` / `Providers.Orchestration` projects that resolve the actual `ProviderDescriptor` instances.
- **Provider enable/disable actions.** The `ProviderStatus.Disabled` value is forward-compatible with a future M4-D `Enabled` configuration field; M4-C.2 does not implement enable/disable actions on the `AppProviderList` (the Disabled badge is rendered for the `Disabled` status; the page does not include an enable/disable toggle).
- **Agent launch.** `IRunService` + `IHistoryStore` + the autonomous loops are M3 (the launch-and-monitor slice) and M5+ (the agentic loops).
- **Architecture test activation for the M4-A process + credential boundaries.** The `Infrastructure_Respects_ProcessBoundary` + `Infrastructure_Respects_CredentialBoundary` tests are registered-but-disabled per ADR-016; they activate in M4-D when the first `Providers.<X>` project lands.

The M4-C.2 first session does **not** begin M4-C closeout, M4-D, or any provider creation. The next session after M4-C.2 is the M4-C closeout session (T-030) on the user's `Approve` or `Next` invocation.
