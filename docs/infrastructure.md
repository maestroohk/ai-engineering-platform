# Infrastructure (M4-A)

> **The M4-A infrastructure documentation.** The
> M4-A.1 slice (delivered 2026-07-11) ships the
> infrastructure seam every later milestone
> composes: the `AiEng.Platform.Infrastructure`
> csproj; the `IProcessRunner`, `ICredentialVault`,
> `IPlatformInfo` contracts; the
> `SystemProcessRunner`, `WindowsCredentialVault`,
> `SystemPlatformInfo`, `JsonFileProjectStore`
> implementations; the `AddInfrastructure`
> composition root; and the M3 in-memory
> `IProjectStore` swap for the on-disk store. The
> M4-A.2 slice (delivered 2026-07-11) ships the
> Open action on `AppProjectCard` — the first
> `IProcessRunner` activation. M4-D activates the
> architecture tests when the first concrete
> provider implementation project
> (`Providers.<X>`) lands.

---

## 1. Goals

The M4-A infrastructure seam exists to:

- **Introduce the process boundary.** Every
  `Process.Start` call in the platform lives
  behind `IProcessRunner`. The Infrastructure
  project is the only project that references
  `System.Diagnostics.Process`. The
  `Infrastructure_Respects_ProcessBoundary`
  architecture test (registered-but-disabled per
  ADR-016; activates in M4-D) enforces the rule.
- **Introduce the credential boundary.** Every
  Windows Credential Manager access lives behind
  `ICredentialVault`. The
  `Infrastructure_Respects_CredentialBoundary`
  architecture test (registered-but-disabled per
  ADR-016; activates in M4-D) enforces the rule.
- **Make the project list durable.** The M3
  in-memory `IProjectStore` is replaced by the
  on-disk `JsonFileProjectStore` behind the
  **same contract**. Projects survive an
  application restart.
- **Expose a platform-info seam.** The
  `IPlatformInfo` contract resolves the data and
  config directories on the current platform;
  the `JsonFileProjectStore` composes
  `IPlatformInfo.GetDataDirectory()` for the
  store file path.

The M4-A.1 slice ships the boundary. The
M4-A.2 slice ships the Open action (the first
process boundary activation). M4-D activates the
architecture tests when the first concrete
provider implementation project (`Providers.<X>`)
lands.

---

## 2. Project Structure

The M4-A.1 slice adds the new `AiEng.Platform.Infrastructure` csproj
under `src/AiEng.Platform.Infrastructure/`. The
new csproj references `AiEng.Platform.Application`
and `AiEng.Platform.Domain`; it does **not**
reference the App csproj. The App csproj adds a
`<ProjectReference>` to the new csproj. The new
csproj inherits `Directory.Build.props` (the
shared build properties: `net10.0`, `Nullable`,
`TreatWarningsAsErrors`).

The directory layout:

```
src/AiEng.Platform.Infrastructure/
  AiEng.Platform.Infrastructure.csproj
  Credentials/
    WindowsCredentialVault.cs
  Platform/
    SystemPlatformInfo.cs
  ProcessRunner/
    SystemProcessRunner.cs
  Projects/
    JsonFileProjectStore.cs
    JsonFileProjectStoreOptions.cs
```

The contracts live in `Application/Infrastructure/`
(alongside the M3 contracts in `Application/Projects/`,
`Application/ProjectIntelligence/`, etc.):

```
src/AiEng.Platform.Application/
  Infrastructure/
    IProcessRunner.cs
    ProcessResult.cs
    ICredentialVault.cs
    IPlatformInfo.cs
```

The composition root extension lives in
`App/Composition/Infrastructure/`:

```
src/AiEng.Platform.App/Composition/Infrastructure/
  InfrastructureServiceCollectionExtensions.cs
```

---

## 3. Process Boundary

`IProcessRunner` is the platform's only process
boundary contract. It exposes two methods:

- `IAsyncEnumerable<string> RunAsync(executable, arguments, CancellationToken)` — streaming
  output. Yields each line of stdout/stderr as
  the child process writes it. Used by the Open
  action (M4-A.2) to stream `explorer.exe`
  output.
- `Task<ProcessResult> RunToCompletionAsync(executable, arguments, CancellationToken)` —
  fire-and-forget. Returns once the child process
  exits with `ProcessResult { ExitCode, StandardOutput, StandardError }`. Used for
  short-lived, non-streaming invocations.

The `SystemProcessRunner` implementation
wraps `System.Diagnostics.Process`. The
implementation uses `RedirectStandardOutput` +
`RedirectStandardError` + `UseShellExecute=false`
+ `CreateNoWindow=true`. The process is started
with `process.Start()`; the implementation is the
**only** call site for `Process.Start` in the
platform. The architecture test
`Infrastructure_Respects_ProcessBoundary`
(registered-but-disabled per ADR-016) scans every
`.cs` file in the repository for `using System.Diagnostics`
and asserts the only file outside the
Infrastructure project that contains the using is
the architecture test itself.

The `ProcessResult` envelope is a `readonly record
struct { int ExitCode, string StandardOutput, string StandardError }`. The
`Succeeded` property returns `ExitCode == 0`.

---

## 4. Credential Boundary

`ICredentialVault` is the platform's only
credential boundary contract. It exposes three
methods:

- `Task<string?> GetAsync(string name, CancellationToken)` — returns the secret
  for `name`, or `null` if no such credential
  exists.
- `Task SetAsync(string name, string secret, CancellationToken)` — stores the
  secret under `name`.
- `Task DeleteAsync(string name, CancellationToken)` — removes the credential
  named `name` (no-op if absent).

The `WindowsCredentialVault` implementation
wraps the Windows Credential Manager via P/Invoke
(`advapi32.dll`'s `CredReadW`, `CredWriteW`,
`CredDeleteW`). On non-Windows hosts the
implementation throws `PlatformNotSupportedException`
on every call. The architecture test
`Infrastructure_Respects_CredentialBoundary`
(registered-but-disabled per ADR-016) scans every
`.cs` file in the repository for `CredRead`,
`CredWrite`, `CredDelete`, or `advapi32.dll` and
asserts the only file outside the Infrastructure
project that contains the references is the
architecture test itself.

The `WindowsCredentialVault` is a thin
direct-P/Invoke wrapper over the Windows
Credential Manager (no NuGet dependency). The
trade-off: zero third-party surface; direct Win32
calls; minimal binary footprint. The
`CredentialManagement` and
`Meziantou.Framework.Win32.CredentialManager`
NuGet packages were considered and deferred
(M4-A plan § 8 risk 4). The direct P/Invoke
approach was selected because it has no
dependency surface, works in all-elevation
scenarios, and is well-understood.

The vault is **Windows-only**; the platform is
a Windows-first app (per `package.json`'s
cross-platform CSS pipeline + the Blazor Server
host on Windows). Cross-platform credential
storage is a future capability (the M4-A plan
§ 3 out-of-scope item).

---

## 5. On-Disk Project Store

`JsonFileProjectStore` is the M4-A on-disk
implementation of the M3 `IProjectStore`
contract. The M3 `IProjectStore` is unchanged;
the composition root swaps the
`InMemoryProjectStore` registration for the
`JsonFileProjectStore` registration (the
`InMemoryProjectStore` is preserved as a test
fixture in
`tests/AiEng.Platform.UnitTests/Infrastructure/InMemoryProjectStore.cs`
for the M3 unit tests that depend on it).

The store resolves the data directory through
`IPlatformInfo.GetDataDirectory()`. On Windows
the path is
`%LOCALAPPDATA%\AiEng\Platform\data\projects.json`.
The store serializes the project list to JSON
with snake_case property naming + indented
format; the JSON shape matches the
`.ai/state/*.json` family.

**Concurrency:** the store uses a `SemaphoreSlim`
(1, 1) to serialize all writes. Reads acquire
the same lock. Concurrent reads of the same
file are serialized but never block for long
(the file is read fully into memory then
deserialized).

**Atomic writes:** the store writes the snapshot
to a temp file
(`projects.json.tmp-<guid>`) then uses
`File.Replace` to atomically swap the temp file
for the destination. On Windows `File.Replace` is
atomic at the file system level; the destination
is never observed in a partially-written state.

**Corruption recovery:** the store's
`ReadSnapshotNoLockAsync` catches `JsonException`
when reading a corrupt file, logs a warning, and
returns an empty list. The next write overwrites
the corrupt file with valid JSON. The store
**does not** silently delete a corrupt file; the
operator can inspect the file before the next
write.

**Constructor:** `new JsonFileProjectStore(IPlatformInfo platformInfo, JsonFileProjectStoreOptions? options = null, ILogger<JsonFileProjectStore>? logger = null)`. The `options.DataDirectory` overrides the
`IPlatformInfo` resolution; the `options.FileName`
defaults to `projects.json`. The DI container
registration
(`InfrastructureServiceCollectionExtensions.AddInfrastructure`)
passes no `options`, so the production store uses
the platform data directory and the default file
name.

---

## 6. Platform Info

`IPlatformInfo` exposes two methods and one
property:

- `string GetDataDirectory()` — the directory
  the platform uses for durable state (the
  `JsonFileProjectStore` file lives here).
- `string GetConfigDirectory()` — the directory
  the platform uses for configuration (reserved
  for M4-B / later).
- `bool IsWindows { get; }` — `true` when the
  process host is Windows (added in M4-A.2). The
  `SystemPlatformInfo` implementation uses
  `RuntimeInformation.IsOSPlatform(OSPlatform.Windows)`.
  Consumers should inject `IPlatformInfo` rather
  than calling `OperatingSystem.IsWindows()`
  directly so the abstraction is testable and the
  platform check can be mocked in bUnit tests.

The `SystemPlatformInfo` implementation
resolves both directories through
`Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)`
and appends a `AiEng/Platform/<data|config>`
subdirectory. The subdirectory is unique to the
platform (no conflict with another app's store).

The path is **platform-agnostic** at the
contract level (the contract returns a path
string; the implementation is Windows-specific
via `Environment.SpecialFolder.LocalApplicationData`).
Cross-platform path resolution is a future
capability.

---

## 7. Open Action

The Open action on `AppProjectCard` is **M4-A.2
(delivered 2026-07-11)**. The Open button is
enabled on Windows hosts (gated on
`IPlatformInfo.IsWindows`); on non-Windows hosts
the button is disabled with a tooltip explaining
the Windows-only constraint.

The Open action's implementation:

- `@inject`s `IProcessRunner`, `IPlatformInfo`,
  and `ILogger<AppProjectCard>` directly on the
  card (the M4-A plan § 2 item 8 "direct
  `IProcessRunner` injection" decision; no
  `IProjectService.OpenAsync` facade and no
  `IOpenProjectAction` seam).
- The click handler
  (`OnOpenClick → OpenAsync`) calls
  `IProcessRunner.RunToCompletionAsync("explorer.exe",
  new[] { Project.Path }, default)`. The
  `IReadOnlyList<string>` argument form lets
  `ProcessStartInfo.ArgumentList` handle quoting
  for paths with spaces.
- A `try/catch` wraps the call for
  `Win32Exception`, `InvalidOperationException`,
  and `IOException` (the three exceptions a
  process invocation can raise when the path no
  longer exists or the executable is not on
  `PATH`). On catch, the handler logs to
  `ILogger<AppProjectCard>` and sets a transient
  `OpenError` string rendered inline in the card
  (`<div class="app-project-card-open-error"
  role="alert">`). The next click clears the
  error.

The M4-A.2 visual smoke clicks the Open button on
a populated `/projects` route; the project folder
opens in File Explorer. On non-Windows hosts the
Open button is disabled with a tooltip.

---

## 8. Composition Root

`InfrastructureServiceCollectionExtensions.AddInfrastructure`
registers the four Infrastructure services in the
DI container:

```csharp
services.TryAddSingleton<IPlatformInfo, SystemPlatformInfo>();
services.TryAddSingleton<IProcessRunner, SystemProcessRunner>();
services.TryAddSingleton<ICredentialVault, WindowsCredentialVault>();
services.TryAddSingleton<IProjectStore, JsonFileProjectStore>();
```

`TryAddSingleton` is used (not `AddSingleton`) so
test code can pre-register an alternate
implementation (e.g., the M3 unit tests that use
`InMemoryProjectStore` for the `IProjectStore`
fixture). The M4-A.1 swap in `AddProjects`
removes the `InMemoryProjectStore` registration;
the `IProjectStore` registration is now sourced
from `AddInfrastructure` exclusively in
production.

The composition root's call ordering is:

```
services.AddNavigation(assemblies);
services.AddProjectIntelligence();
services.AddProjects();
services.AddInfrastructure();
```

`AddInfrastructure` is called **after** `AddProjects`
so the `IProjectStore` registration in
`AddInfrastructure` is the sole source for the
service in the DI container.

---

## 9. Tests

The M4-A.1 slice ships 50+ new tests across four
test files:

- `JsonFileProjectStoreTests` (22 tests):
  round-trip, missing file, duplicate detection,
  corrupt file recovery, concurrent adds, atomic
  writes, cancellation, constructor validation.
- `IProcessRunnerTests` (11 tests): exit code
  propagation, stdout/stderr capture, streaming
  output, cancellation, non-existent executable
  failure, argument validation.
- `WindowsCredentialVaultTests` (10 tests): get /
  set / delete round-trip, missing credential
  returns null, non-Windows throws
  `PlatformNotSupportedException`, argument
  validation.
- `SystemPlatformInfoTests` (3 tests):
  `GetDataDirectory` and `GetConfigDirectory`
  return non-empty rooted paths, the two paths
  are distinct.

The M3 in-memory `IProjectStore` was moved to
`tests/AiEng.Platform.UnitTests/Infrastructure/InMemoryProjectStore.cs`
as a test fixture. The M3 unit tests that use it
(`IProjectServiceTests`, `InMemoryProjectStoreTests`)
were updated to import the new namespace; the
M3 unit tests continue to pass.

The M4-A.1 slice also adds 2 architecture tests
in
`tests/AiEng.Platform.ArchitectureTests/Infrastructure/`:

- `Infrastructure_Respects_ProcessBoundary`:
  asserts `AiEng.Platform.Infrastructure` is the
  only project that references
  `System.Diagnostics.Process`.
- `Infrastructure_Respects_CredentialBoundary`:
  asserts `AiEng.Platform.Infrastructure` is the
  only project that references the Windows
  Credential Manager APIs.

Both tests are **registered-but-disabled** per
ADR-016 (`[Fact(Skip = "...")]`); they activate
in M4-D when the first concrete provider
implementation project (`Providers.<X>`) lands.

The M4-A.2 slice adds 5 new bUnit tests in
`tests/AiEng.Platform.ComponentTests/Projects/AppProjectCardTests.cs`
plus 1 new architecture test in
`tests/AiEng.Platform.ArchitectureTests/Pages/PagesResolveProjectsThroughServiceTests.cs`:

- `Open_Button_Is_Enabled_When_Host_Is_Windows`
- `Open_Button_Is_Disabled_When_Host_Is_Not_Windows`
- `Clicking_Open_Invokes_IProcessRunner_With_Explorer_And_ProjectPath`
- `Open_Click_Passes_ProjectPath_Single_Element_As_Argument`
- `Open_Click_Swallows_Process_Exceptions`
- `AppProjectCard_resolves_open_through_IProcessRunner`
  (architecture test: asserts the card uses
  `@inject IProcessRunner` and contains no
  `Process.Start` or `ProcessStartInfo` token —
  the process boundary is the only allowed seam).

Cumulative test count after M4-A.2: 323 passed, 0
failed, 9 skipped (the 7 from M3 + the 2 M4-A.1
architecture tests remain registered-but-disabled
per ADR-016).

---

## 10. Out of Scope

The M4-A plan § 3 enumerates 10 out-of-scope
items. The M4-A.1 + M4-A.2 slices respect every
item:

- **Provider creation.** Neither M4-A.1 nor
  M4-A.2 create any `Providers.<X>` projects. The
  first concrete providers land in M4-D.
- **M4-B, M4-C, M4-D work.** Neither slice
  begins any work outside the M4-A boundary.
- **Activation of the 4 disabled
  `CompositionRootBoundaryTests`.** Neither slice
  enables these. They activate in M4-D.
- **Design-system `AppDialog` primitive.** The
  Open action's confirmation dialog (if any)
  composes the existing HTML5 native `<dialog>`
  (the M3.2 minimum-blast-radius decision); no
  new design-system component is added. The
  M4-A.2 Open action does not use a confirmation
  dialog at all (the action is a single
  `explorer.exe <path>` invocation; the blast
  radius is minimal).
- **macOS / Linux credential vault.** The
  `WindowsCredentialVault` is Windows-only; on
  non-Windows hosts it throws
  `PlatformNotSupportedException`. Cross-platform
  credential storage is a future capability.
- **Activation of axe-core tests.** The 3
  `AxeCoreAuditTests` remain disabled (the
  axe-core harness is not in the toolchain yet).
- **Push to remote.** Neither M4-A.1 nor M4-A.2
  push. The push decision is `Staged for push`;
  the next user command may push.

The M4-A.2 slice ends after the coherent commit
on the feature branch
`feature/T-022-m4-a-2-open-action`. The next
session is M4-A.3 (if defined) or the M4-B plan
promotion.
