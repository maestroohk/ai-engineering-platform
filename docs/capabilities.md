# Capabilities (M4-B)

> **The M4-B capability-detection documentation.** The M4-B.1 slice (delivered 2026-07-13) ships the `IHostCapabilitiesService` contract + the `HostCapabilities` + `HostCapability` records + the `SystemHostCapabilitiesService` implementation + the `AddHostCapabilities` composition root. The M4-B.2 slice (delivered 2026-07-13) ships the `AppCapabilityList` + `AppKeyValueList` data-owning design-system components + the `AppKeyValueListFormat` enum + 28 bUnit tests. The M4-B.3 slice (delivered 2026-07-13) ships the `/diagnostics` page + the startup capability-report log + the `Capabilities_Resolved_Through_Service` architecture test. M4-C consumes the report through the contract; the report itself is read by the page, not by the provider registry.

---

## 1. Goals

The M4-B capability-detection seam exists to:

- **Expose a single host-capability seam.** The `IHostCapabilitiesService` contract is the only seam the application reads host capabilities through. The `Capabilities_Resolved_Through_Service` architecture test (Active) enforces the rule. Every `.razor` and `.razor.cs` file under `src/AiEng.Platform.App/Components/Diagnostics/` is scanned: no `RunToCompletionAsync` token (forbidden `IProcessRunner` usage), no `ICredentialVault` token (forbidden credential boundary), no `new SystemHostCapabilitiesService` token (forbidden direct-instantiation escape hatch).
- **Decouple the report from the process + credential boundaries.** The `SystemHostCapabilitiesService` is the single place that composes `IProcessRunner` + `ICredentialVault` (per the M4-A seam) to probe the host. The application layer consumes only the typed `HostCapabilities` + `HostCapability` records.
- **Be the only consumer of `IProcessRunner` + `ICredentialVault` outside the M4-A.2 Open Action.** The `AppProjectCard.razor` Open action is the M4-A.2 process boundary activation. The `SystemHostCapabilitiesService` is the only M4-B consumer. M4-C and later compose `IProviderRegistry` through the `IHostCapabilitiesService` dependency.
- **Drive the `/diagnostics` page.** The page is the user-visible surface; the startup log is the early signal. Both consume the contract; the contract consumes the implementation; the implementation consumes the M4-A boundary.

The M4-B.1 slice ships the contract. The M4-B.2 slice ships the design-system components. The M4-B.3 slice ships the page, the startup log, the architecture test, and this documentation. M4-C consumes the report through DI, not through the startup log.

---

## 2. Project Structure

The M4-B slices add code under four locations:

```
src/AiEng.Platform.Application/
  Capabilities/
    IHostCapabilitiesService.cs
    HostCapabilities.cs
src/AiEng.Platform.Infrastructure/
  Capabilities/
    SystemHostCapabilitiesService.cs
src/AiEng.Platform.App/
  Composition/
    Capabilities/
      CapabilitiesServiceCollectionExtensions.cs
  Components/
    Diagnostics/
      AppCapabilityList.razor
      AppCapabilityList.razor.cs
      AppCapabilityList.razor.css
      AppKeyValueList.razor
      AppKeyValueList.razor.cs
      AppKeyValueList.razor.css
      _Imports.razor
  Components/
    Common/
      Enums.cs   (the AppKeyValueListFormat enum is appended)
  Components/
    Pages/
      Diagnostics.razor
      Diagnostics.razor.css
  Program.cs   (the startup capability-report log is inserted after app.Build())
```

The M4-B Application additions live alongside the M4-A `IPlatformInfo` contract in the same project. The M4-B Infrastructure additions live alongside the M4-A `SystemPlatformInfo` implementation. The App additions compose both through DI.

---

## 3. The `IHostCapabilitiesService` Contract

`src/AiEng.Platform.Application/Capabilities/IHostCapabilitiesService.cs` defines the seam:

```csharp
namespace AiEng.Platform.Application.Capabilities;

public interface IHostCapabilitiesService
{
    Task<HostCapabilities> DetectAsync(CancellationToken cancellationToken = default);
}
```

The contract is async; it accepts a `CancellationToken`; it returns the typed `HostCapabilities` record. The default `CancellationToken` lets the page-level call omit the parameter; the startup log supplies a 10-second `CancellationTokenSource` to bound the startup budget.

---

## 4. The `HostCapabilities` and `HostCapability` Records

`src/AiEng.Platform.Application/Capabilities/HostCapabilities.cs` defines the data envelope:

```csharp
namespace AiEng.Platform.Application.Capabilities;

public sealed record class HostCapabilities(
    IReadOnlyList<HostCapability> Capabilities,
    DateTimeOffset DetectedAt);

public sealed record class HostCapability(
    string Key,
    bool Available,
    string? Version,
    bool CredentialAvailable,
    string? CredentialName);
```

The `HostCapabilities` record carries the full probe result (the list of capabilities + the timestamp). The `HostCapability` record is the per-tool entry: a `Key` (e.g. `git`, `ollama`, `provider:git`), a `bool Available` flag, an optional `Version` string, and the `CredentialAvailable` + `CredentialName` fields used by the M4-B.2 `AppCapabilityList` "Credential set" badge.

The `SystemHostCapabilitiesService` probes six host tools (`git`, `ollama`, `powershell`, `wsl`, `wt`, `bash`) and six provider credentials (`provider:git`, `provider:ollama`, `provider:powershell`, `provider:wsl`, `provider:wt`, `provider:bash`). The `Key` is the stable identifier across both groups; the `provider:` prefix distinguishes the credential probes from the tool probes.

---

## 5. The `AppCapabilityList` Component

`src/AiEng.Platform.App/Components/Diagnostics/AppCapabilityList.razor` (+ .razor.cs + .razor.css) is the M4-B.2 data-owning four-state component. It accepts:

- `Capabilities` (`IReadOnlyList<HostCapability>`, `[EditorRequired]`) — the source data.
- `IsLoading` (`bool`) — when true, renders the `Loading` slot or the default `<AppLoading>` fallback.
- `ErrorMessage` + `ErrorCode` + `CorrelationId` (`string?`) — when `ErrorMessage` is non-empty, renders the `Error` slot or the default `<AppErrorState>` fallback.
- `Loading` + `Empty` + `Error` + `Populated` (`RenderFragment?`) — the four data-owning slots per `docs/design-system.md` § 5.4.
- `AdditionalAttributes` — the standard `CaptureUnmatchedValues` splat.

The populated list is rendered as an `<AppStack>` of `<div class="app-capability-list-item">` entries (one per capability). Each entry wraps an `<AppCard>` with an `AppStatusDot` Success/Error (driven by `Available`), the `Version` (or a muted "version unknown" placeholder), and an `AppBadge Variant="Success"` "Credential set" badge (driven by `CredentialAvailable`). The populated container has `role="list"` + `aria-live="polite"` for screen-reader accessibility.

---

## 6. The `AppKeyValueList` Component

`src/AiEng.Platform.App/Components/Diagnostics/AppKeyValueList.razor` (+ .razor.cs + .razor.css) is the M4-B.2 data-owning four-state component. It accepts:

- `Items` (`IReadOnlyList<KeyValuePair<string, string>>`, `[EditorRequired]`) — the source data.
- `Format` (`AppKeyValueListFormat`) — `Plain` (default, literal text), `Boolean` (renders ✓ for `"true"`, ✗ for `"false"`, case-insensitive; non-boolean values render as literal text), or `Code` (renders inside a monospaced `<code>` element).
- The same four-state contract as `AppCapabilityList`.

The populated list is rendered as a `<dl>` of `<div class="app-key-value-list-row">` entries, each containing a `<dt>` (the key) + `<dd>` (the value, formatted by `Format`). The populated container has `aria-live="polite"`.

The `AppKeyValueListFormat` enum is appended to `src/AiEng.Platform.App/Components/Common/Enums.cs`:

```csharp
public enum AppKeyValueListFormat
{
    Plain,
    Boolean,
    Code
}
```

---

## 7. The `/diagnostics` Page

`src/AiEng.Platform.App/Components/Pages/Diagnostics.razor` (+ .razor.css) is the M4-B.3 user-visible surface. The page:

- Declares `@page "/diagnostics"` and `@attribute [RouteMetadata("/diagnostics", "Diagnostics", Order = 4, ShowInSidebar = true, Icon = "◆", Description = "Detected host capabilities (tools, versions, provider credentials).")]`. The `Order = 4` follows the M2.4 `Order = 0` (Dashboard) and M3 `Order = 1` (Projects) precedence.
- Inherits `@layout AppLayout` + `@rendermode InteractiveServer` to match the M2.4 + M3 page patterns.
- Injects `IHostCapabilitiesService Service` and `IPlatformInfo PlatformInfo`. The architecture test allows `IPlatformInfo` (it is a meta-data accessor, not a process boundary); the test forbids `IProcessRunner` + `ICredentialVault` + `new SystemHostCapabilitiesService`.
- Calls `Service.DetectAsync()` in `OnInitializedAsync` (and on the Refresh button click). On success, the result is stored in `_result` and the metadata array is built (`Detected at` + `Data directory` + `Config directory` + `Is Windows host`); on failure, `_result` is set to `null` and `_error` + `_errorCode` are populated.
- Renders an `<AppPageHeader>` with the title "Diagnostics", a description, and an `Actions` slot holding the Refresh `<AppButton>` (testid `refresh-diagnostics`, `Variant="Outline"`, with `Loading` flipped while the call is in flight).
- Renders the capability list in the first `<AppCard>` ("Host tools and provider credentials") and the host metadata in the second `<AppCard>` ("Host metadata", only when `_result` is non-null).

The scoped CSS adds a `1.25rem` `margin-top` to the second `AppCard` so the two cards have vertical breathing room. No design-system change.

---

## 8. Composition Root

`src/AiEng.Platform.App/Composition/Capabilities/CapabilitiesServiceCollectionExtensions.cs` defines the M4-B.1 `AddHostCapabilities` extension method. The chain in `src/AiEng.Platform.App/Composition/ServiceCollectionExtensions.cs` is:

```
AddNavigation
  → AddProjectIntelligence
  → AddProjects
  → AddInfrastructure
  → AddHostCapabilities
```

`AddHostCapabilities` uses `TryAddSingleton<IHostCapabilitiesService, SystemHostCapabilitiesService>()` so the registration is idempotent (a future test that overrides the service with a fake does not collide). The `SystemHostCapabilitiesService` is constructor-injected with `IProcessRunner` + `ICredentialVault` + `IPlatformInfo` (the M4-A seams) + `ILogger<SystemHostCapabilitiesService>`.

The startup capability-report log in `src/AiEng.Platform.App/Program.cs` resolves `IHostCapabilitiesService` from `app.Services` after `app.Build()` and before the middleware pipeline. The block is wrapped in a `try/catch` that logs failures at `Warning` level; the startup must not fail if capability detection fails.

---

## 9. Tests

The M4-B test inventory:

- **M4-B.1 unit tests** (`tests/AiEng.Platform.UnitTests/Capabilities/`): 8 tests covering `SystemHostCapabilitiesService` (the 6 host tools + 6 provider credentials; the cancellation token; the error path).
- **M4-B.2 bUnit component tests** (`tests/AiEng.Platform.ComponentTests/Components/Diagnostics/`): 13 tests for `AppCapabilityList` + 15 tests for `AppKeyValueList`. The 28 bUnit tests cover the four data-owning states (Loading, Empty, Error, Populated) + the slot overrides + the accessibility attributes.
- **M4-B.3 bUnit page tests** (`tests/AiEng.Platform.ComponentTests/Pages/DiagnosticsPageTests.cs`): 4 tests covering the page-level wiring (`OnInitializedAsync` calls `DetectAsync`, the page renders the `AppCapabilityList` with the 12 capabilities, the Refresh button re-runs `DetectAsync`, the page renders the `AppKeyValueList` with the host metadata).
- **M4-B.3 architecture test** (`tests/AiEng.Platform.ArchitectureTests/Capabilities/Capabilities_Resolved_Through_Service.cs`): 2 Active tests. The first asserts `Diagnostics.razor` contains `@inject IHostCapabilitiesService` and does not contain `RunToCompletionAsync` / `ICredentialVault` / `new SystemHostCapabilitiesService`. The second scans every `.razor` + `.razor.cs` file under `src/AiEng.Platform.App/Components/Diagnostics/` for the same forbidden tokens.

Total tests after M4-B.3: 374 passed, 0 failed, 9 skipped (per ADR-016 / M4-D).

---

## 10. Out of Scope

The M4-B slices do **not** include:

- **Provider registry.** `IProviderRegistry` + `IProviderRegistration` + the M4-C composition root are deferred to M4-C. The M4-C provider registry consumes the `IHostCapabilitiesService` through DI, not through the startup log.
- **Provider creation.** Per the M4-B brief: "Do not create providers" — M4-B detects capabilities, it does not create providers. The M4-C + M4-D + M7 work creates the providers and the `Providers.<X>` concrete implementation projects.
- **Agent launch.** `IRunService` + `IHistoryStore` + the autonomous loops are M3 (the launch-and-monitor slice) and M5+ (the agentic loops).
- **Architecture test activation for the M4-A process + credential boundaries.** The `Infrastructure_Respects_ProcessBoundary` + `Infrastructure_Respects_CredentialBoundary` tests are registered-but-disabled per ADR-016; they activate in M4-D when the first `Providers.<X>` project lands.

The M4-B.3 first session does **not** begin M4-C, M4-D, or any provider creation. The next session after M4-B.3 is the M4-B closeout session.
