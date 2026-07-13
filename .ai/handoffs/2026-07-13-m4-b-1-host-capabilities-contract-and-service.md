# Handoff — M4-B.1 IHostCapabilitiesService Contract + Implementation + Composition Root + Unit Tests — `m4-b-1-host-capabilities-contract-and-service` (2026-07-13)

> **The M4-B.1 per-session handoff.** M4-B.1
> (T-024) is the first M4-B implementation
> slice. M4-B.1 follows the M4-B plan
> promotion per the Progressive Coding Rule.
> M4-B.1 ships the `IHostCapabilitiesService`
> contract + the `HostCapabilities` +
> `HostCapability` sealed records + the
> `SystemHostCapabilitiesService`
> implementation (the first M4-B consumer
> of `IProcessRunner` + `ICredentialVault` +
> `IPlatformInfo` outside the M4-A.2 Open
> Action) + the `AddHostCapabilities`
> composition root extension + the wire-up
> in `AddPlatformServices` + the
> `CapabilityProbe` internal record types
> + 20 unit tests + 3 in-line test doubles
> (`FakeProcessRunner`, `FakeCredentialVault`,
> `FakePlatformInfo`).
>
> M4-B.1 is the **boundary slice**, not the
> activation. The `AppCapabilityList` +
> `AppKeyValueList` data-owning
> design-system components are M4-B.2's
> responsibility (T-025 is `Ready`; the
> M4-B.1 session does **not** begin M4-B.2
> per the brief and the Progressive Coding
> Rule). The `/diagnostics` page + the
> startup capability-report log + the
> `docs/capabilities.md` documentation +
> the `Capabilities_Resolved_Through_Service`
> architecture test are M4-B.3's
> responsibility (the architecture test
> was deferred from M4-B.1 to M4-B.3 per
> the M4-B.1 plan section 14.1 Deviations
> — the test asserts `Diagnostics.razor`
> contains `@inject
> IHostCapabilitiesService`, and
> `Diagnostics.razor` does not exist in
> M4-B.1). M4-B.1 does **not** create
> providers (per the M4-B brief: "Do not
> create providers").

---

## 1. What was delivered

The M4-B.1 first session (T-024) is **Done**
(2026-07-13).

The M4-B.1 ships:

- **The `IHostCapabilitiesService`
  contract** at
  `src/AiEng.Platform.Application/Capabilities/IHostCapabilitiesService.cs`
  (a pure interface; single async method
  `Task<HostCapabilities>
  DetectAsync(CancellationToken
  cancellationToken = default)`; the
  contract is the seam every later
  capability-detection consumer
  composes).
- **The `HostCapabilities` +
  `HostCapability` sealed records** at
  `src/AiEng.Platform.Application/Capabilities/HostCapabilities.cs`
  (`HostCapabilities:
  IReadOnlyList<HostCapability> Capabilities
  + DateTimeOffset DetectedAt`;
  `HostCapability: string Key + bool
  Available + string? Version + bool
  CredentialAvailable + string?
  CredentialName`).
- **The internal `HostToolProbe` +
  `ProviderCredentialProbe` record types**
  at
  `src/AiEng.Platform.Infrastructure/Capabilities/CapabilityProbe.cs`
  (the probe definitions; not exposed
  to the App layer; `internal` to the
  Infrastructure assembly).
- **The `SystemHostCapabilitiesService`
  implementation** at
  `src/AiEng.Platform.Infrastructure/Capabilities/SystemHostCapabilitiesService.cs`
  (sealed class; ctor injects
  `IProcessRunner` + `ICredentialVault` +
  `IPlatformInfo` +
  `ILogger<SystemHostCapabilitiesService>`
  + optional `TimeProvider`; six
  `HostToolProbes` for `git` + `ollama` +
  `powershell.exe` + `wsl.exe` + `wt.exe` +
  `bash.exe` via
  `IProcessRunner.RunToCompletionAsync(tool,
  new[] { arguments }, linkedCts.Token)`
  with a 5-second per-tool
  `CancellationTokenSource` timeout linked
  with the outer token; six
  `ProviderCredentialProbes` for the
  corresponding providers via
  `ICredentialVault.GetAsync("provider:<key>:token",
  ct)`; `IPlatformInfo.IsWindows` gating
  for Windows-only tools (`powershell.exe`,
  `wsl.exe`, `wt.exe`); per-tool `Regex`
  version parsing; outer-cancellation
  propagation via re-throw; exception
  swallowing with `LogWarning` for
  `Win32Exception` + `InvalidOperationException`
  + `IOException`).
- **The `AddHostCapabilities` composition
  root extension** at
  `src/AiEng.Platform.App/Composition/Capabilities/CapabilitiesServiceCollectionExtensions.cs`
  (`TryAddSingleton<IHostCapabilitiesService,
  SystemHostCapabilitiesService>`; mirrors
  the `AddInfrastructure` pattern;
  null-check on `services`; returns
  `services` for chaining).
- **The wire-up in `AddPlatformServices`**
  in
  `src/AiEng.Platform.App/Composition/ServiceCollectionExtensions.cs`
  (one `using
  AiEng.Platform.App.Composition.Capabilities;`
  directive + one `services.AddHostCapabilities();`
  call after the existing
  `services.AddInfrastructure();` call).
- **20 unit tests + 3 in-line test
  doubles** at
  `tests/AiEng.Platform.UnitTests/Capabilities/SystemHostCapabilitiesServiceTests.cs`
  (`FakeProcessRunner` records each
  invocation + returns a scripted
  `ProcessResult` per executable + throws
  scripted exceptions + awaits scripted
  delays; `FakeCredentialVault` returns a
  scripted `string?` per credential name
  (null for absent); `FakePlatformInfo`
  returns a scripted `IsWindows` value;
  `FixedTimeProvider` extends
  `TimeProvider` for the `DetectedAt`
  deterministic test).

## 2. Test and build status

- **Format:** `dotnet format
  --verify-no-changes` passed.
- **Build:** 0 warnings, 0 errors.
- **Test:** **343 passed**, 0 failed,
  9 skipped (per ADR-016 / M4-D). The
  M4-B.1 ships 20 new unit tests; the
  pre-M4-B.1 baseline was 323 passed
  (79 unit + 232 bUnit + 12 architecture).
  The post-M4-B.1 total is 343 passed
  (99 unit + 232 bUnit + 12 architecture).
  The 9 skipped are: 3
  `AxeCoreAuditTests` (activate in M4-D)
  + 4 `CompositionRootBoundaryTests`
  (activate in M4-D) + 2
  `Infrastructure_Respects_*` (activate
  in M4-D).
- **Architecture tests:** 0 new
  architecture tests. The M4-B plan
  § 2 In Scope § 9 `Capabilities_Resolved_Through_Service`
  architecture test is deferred to M4-B.3
  (the test asserts `Diagnostics.razor`
  contains `@inject
  IHostCapabilitiesService`;
  `Diagnostics.razor` does not exist in
  M4-B.1). This is documented as the
  M4-B.1 plan § 14.1 Deviations.
- **CRLF:** all new + modified files are
  CRLF (`unix2dos` applied).

## 3. Deviations

1. **The `Capabilities_Resolved_Through_Service`
   architecture test is deferred to M4-B.3.**
   The M4-B plan § 2 In Scope § 9 lists the
   architecture test in the M4-B.1 scope.
   On inspection, the test asserts that
   `Diagnostics.razor` contains
   `@inject IHostCapabilitiesService`,
   that no `RunToCompletionAsync` token
   appears in `App/Components/Diagnostics/`,
   and that no `ICredentialVault` direct
   call appears in
   `App/Components/Diagnostics/`. The file
   `Diagnostics.razor` does not exist in
   M4-B.1 (it lands in M4-B.3). M4-B.1
   would have to either create a fake
   `Diagnostics.razor` only to make the
   test pass (scope creep) or skip the
   test (which violates the M4-B plan
   § 2 In Scope § 9). The M4-B.1 plan
   resolves this by deferring the
   architecture test to M4-B.3, where
   `Diagnostics.razor` exists. The
   deviation is recorded in the M4-B.1
   implementation report § 14 Deviations
   and in `.ai/state/milestones.json` and
   `.ai/state/capabilities.json` C-015
   `architecture_tests`.
2. **The `DetectedAt` test was split
   into two tests.** The original plan
   test asserted that `DetectedAt` was
   set within the call window when the
   default `TimeProvider` was used. The
   test was split: (a) `DetectedAt`
   equals the `TimeProvider` value
   (deterministic with `FixedTimeProvider`);
   (b) `DetectedAt` falls within the call
   window when the default clock is used
   (non-deterministic but bounded). This
   separation makes the TimeProvider
   injection seam explicit (the test for
   the seam) and keeps the non-deterministic
   test for the regression (the test for
   the implementation's default behavior).
3. **The outer-cancellation catch block
   re-throws explicitly.** The original
   plan's `catch
   (OperationCanceledException) when
   (timeoutCts.IsCancellationRequested &&
   !cancellationToken.IsCancellationRequested)`
   was insufficient: when the outer token
   is cancelled, the filter is `false`
   (because `cancellationToken.IsCancellationRequested`
   is `true`), and the exception falls
   through to the generic
   `catch (Exception ex)` block, which
   silently swallows the cancellation.
   The implementation adds an explicit
   `catch
   (OperationCanceledException) when
   (cancellationToken.IsCancellationRequested)`
   that re-throws, ensuring the outer
   cancellation propagates to the caller.
   The order of the two filters matters:
   the outer-cancellation check must come
   first so the generic catch is reached
   only for non-cancellation exceptions.

## 4. Files added

### 4.1 New contracts

- `src/AiEng.Platform.Application/Capabilities/IHostCapabilitiesService.cs`
  (10 lines): the
  `IHostCapabilitiesService` interface
  with the single async
  `DetectAsync` method. The
  `Capabilities/` directory is a new
  directory under
  `src/AiEng.Platform.Application/`.

### 4.2 New records

- `src/AiEng.Platform.Application/Capabilities/HostCapabilities.cs`
  (15 lines): the
  `HostCapabilities` and
  `HostCapability` sealed record
  classes. `HostCapabilities` exposes
  the read-only list of capabilities
  and the `DetectedAt` timestamp;
  `HostCapability` exposes the per-item
  `Key` + `Available` + `Version` +
  `CredentialAvailable` + `CredentialName`.

### 4.3 New implementation

- `src/AiEng.Platform.Infrastructure/Capabilities/CapabilityProbe.cs`
  (15 lines): the internal
  `HostToolProbe` + `ProviderCredentialProbe`
  record types. The `HostToolProbe`
  carries the `Key` + `Executable` +
  `Arguments` + `WindowsOnly` +
  `VersionPattern`; the
  `ProviderCredentialProbe` carries the
  `Key` + `CredentialName`. The records
  are `internal` to the
  `AiEng.Platform.Infrastructure`
  assembly.
- `src/AiEng.Platform.Infrastructure/Capabilities/SystemHostCapabilitiesService.cs`
  (190+ lines): the
  `SystemHostCapabilitiesService` sealed
  class. The ctor takes
  `IProcessRunner` + `ICredentialVault` +
  `IPlatformInfo` +
  `ILogger<SystemHostCapabilitiesService>`
  + an optional `TimeProvider` (defaults
  to `TimeProvider.System`); the
  `DetectAsync` method iterates the
  `HostToolProbes` + the
  `ProviderCredentialProbes` and returns
  a `HostCapabilities` with the current
  `DetectedAt` timestamp.

### 4.4 New composition root

- `src/AiEng.Platform.App/Composition/Capabilities/CapabilitiesServiceCollectionExtensions.cs`
  (25 lines): the
  `CapabilitiesServiceCollectionExtensions`
  static class with the
  `AddHostCapabilities` extension. The
  extension is `null`-safe, registers
  `IHostCapabilitiesService` →
  `SystemHostCapabilitiesService` via
  `TryAddSingleton`, and returns
  `services` for chaining. The
  `Capabilities/` directory is a new
  directory under
  `src/AiEng.Platform.App/Composition/`.

### 4.5 New unit tests

- `tests/AiEng.Platform.UnitTests/Capabilities/SystemHostCapabilitiesServiceTests.cs`
  (410+ lines): the 20 unit tests + 3
  in-line test doubles. The
  `FakeProcessRunner` implements
  `IProcessRunner.RunToCompletionAsync`
  + `IProcessRunner.RunAsync` (the
  latter returns an empty async
  enumerable); the
  `FakeCredentialVault` implements
  `ICredentialVault.GetAsync` +
  `SetAsync` + `DeleteAsync`; the
  `FakePlatformInfo` implements
  `IPlatformInfo.IsWindows` +
  `GetDataDirectory` + `GetConfigDirectory`;
  the `FixedTimeProvider` extends
  `TimeProvider` and overrides
  `GetUtcNow` to return a fixed
  timestamp. The test class uses
  `NullLogger<SystemHostCapabilitiesService>`
  from
  `Microsoft.Extensions.Logging.Abstractions`.

## 5. Files modified

### 5.1 ServiceCollectionExtensions.cs

- `src/AiEng.Platform.App/Composition/ServiceCollectionExtensions.cs`:
  added one
  `using
  AiEng.Platform.App.Composition.Capabilities;`
  directive (after the existing
  `using
  AiEng.Platform.App.Composition.Infrastructure;`
  on line 3) + one
  `services.AddHostCapabilities();` call
  after the existing
  `services.AddInfrastructure();` call
  (the `AddInfrastructure()` call is on
  line 27; the new call is on line 28).
  The order is now:
  `AddNavigation` →
  `AddProjectIntelligence` →
  `AddProjects` → `AddInfrastructure` →
  `AddHostCapabilities`. The
  `AddHostCapabilities` is last because
  it composes the seam registered by
  `AddInfrastructure` (`IProcessRunner` +
  `ICredentialVault` + `IPlatformInfo`).

## 6. Files deleted

None.

## 7. Files NOT touched

- `src/AiEng.Platform.Application/Infrastructure/`
  (the M4-A contracts): **not**
  modified. M4-B.1 composes the
  existing M4-A contracts.
- `src/AiEng.Platform.Infrastructure/ProcessRunner/`,
  `Credentials/`, `Platform/`
  (the M4-A implementations): **not**
  modified. M4-B.1 composes the existing
  M4-A implementations.
- `src/AiEng.Platform.App/Composition/Infrastructure/`
  (the M4-A composition root): **not**
  modified. M4-B.1 composes the existing
  `AddInfrastructure`.
- `src/AiEng.Platform.App/Components/`:
  **not** modified. M4-B.1 does not
  ship components (M4-B.2's
  responsibility).
- `src/AiEng.Platform.App/Components/Pages/`,
  `src/AiEng.Platform.App/Program.cs`:
  **not** modified. M4-B.1 does not
  ship the `/diagnostics` page (M4-B.3's
  responsibility); `Diagnostics.razor`
  does **not** exist in M4-B.1.
- `src/AiEng.Platform.Providers.Abstractions/`,
  `src/AiEng.Platform.Domain/`: **not**
  modified. M4-B does not create
  providers (per the brief).
- `tests/`: **not** modified except for
  the new test file. M4-B.1 does not
  modify any existing test.
- `docs/capabilities.md`,
  `docs/infrastructure.md`,
  `docs/design-system.md`, `docs/projects.md`,
  `ROADMAP.md`, `.ai/plans/`:
  **not** modified. M4-B.1 is a code
  change, not a doc change. The M4-B
  plan promotion already updated the
  doc surface.
- `AGENTS.md`, `ARCHITECTURE.md`,
  `DECISIONS.md`, `STYLEGUIDE.md`,
  `CONTRIBUTING.md`, `.ai/workflows/`:
  **not** modified. The 17
  non-negotiable rules and the
  workflows are preserved.
- `tailwind.config.js`, `package.json`,
  `Directory.Build.props`,
  `.editorconfig`: **not** modified.
  The CSS pipeline and the .NET build
  configuration are unchanged.

## 8. Next action

**Stop.** The M4-B.1 first session
delivers the boundary slice. The M4-B.1
session does **not** begin M4-B.2
(design-system components) / M4-B.3
(page + startup log + documentation +
architecture test) / M4-C / M4-D / any
provider creation (per the brief: "Do
not begin the following task" and the
Progressive Coding Rule).

The next session is the **M4-B.2 first
session** (T-025) on the user's
`Approve` or `Next` invocation. M4-B.2
ships the `AppCapabilityList` +
`AppKeyValueList` data-owning four-state
design-system components composing the
M1.2 primitives (per the M4-B plan § 2
items 4 + 5 + the M4-B.2 stub row in
`.ai/state/tasks.json`).

Push is **staged for push** (not
authorised in this session). The next
user command may push the M4-B.1 closeout
commit per the command protocol.

---

**End of M4-B.1 per-session handoff.**
The M4-B.1 first session is the
boundary slice of M4-B; the closeout
commit `feat(m4-b.1): add
IHostCapabilitiesService contract and
SystemHostCapabilitiesService
implementation` is on `main`; the
feature branch is fast-forwarded into
`main` and deleted. The handoff is
mirrored to `.ai/handoffs/latest.md`.
The M4-B.1 implementation report is at
`implementation-report-m4-b-1-host-capabilities-contract-and-service.md`.
