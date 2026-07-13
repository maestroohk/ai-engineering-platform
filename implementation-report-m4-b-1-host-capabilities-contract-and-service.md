# Implementation Report — M4-B.1 IHostCapabilitiesService Contract + Implementation + Composition Root + Unit Tests

> **The M4-B.1 implementation report.** M4-B.1
> is the first slice of M4-B. M4-B.1 ships the
> **host capability detection seam** every
> later consumer composes: the
> `IHostCapabilitiesService` contract + the
> `HostCapabilities` + `HostCapability` sealed
> records in
> `src/AiEng.Platform.Application/Capabilities/`;
> the `SystemHostCapabilitiesService`
> implementation in
> `src/AiEng.Platform.Infrastructure/Capabilities/`
> (composes the three M4-A contracts
> `IProcessRunner` + `ICredentialVault` +
> `IPlatformInfo`; probes six host tools — `git`,
> `ollama`, `powershell.exe`, `wsl.exe`,
> `wt.exe`, `bash.exe` — via
> `IProcessRunner.RunToCompletionAsync(tool,
  new[] { arguments }, linkedCts.Token)` with a
> 5-second per-tool `CancellationTokenSource`
> timeout linked with the outer token; reads
> six provider credentials via
> `ICredentialVault.GetAsync("provider:<key>:token",
> ct)`); the `CapabilityProbe` internal record
> types; the `AddHostCapabilities` composition
> root extension in
> `src/AiEng.Platform.App/Composition/Capabilities/`;
> the wire-up in `AddPlatformServices`; 20 unit
> tests + 3 in-line test doubles in
> `tests/AiEng.Platform.UnitTests/Capabilities/`.
>
> **M4-B.1 is the boundary, not the activation.**
> The `AppCapabilityList` + `AppKeyValueList`
> data-owning four-state design-system
> components are the M4-B.2 slice's
> responsibility (T-025 is `Ready`; the M4-B.1
> session does **not** begin M4-B.2 per the
> brief and the Progressive Coding Rule). The
> `/diagnostics` page + the startup
> capability-report log + the
> `docs/capabilities.md` documentation + the
> `Capabilities_Resolved_Through_Service`
> architecture test are the M4-B.3 slice's
> responsibility (the architecture test was
> deferred from M4-B.1 to M4-B.3 per the
> M4-B.1 plan § 14.1 Deviations — the test
> asserts `Diagnostics.razor` contains
> `@inject IHostCapabilitiesService`, and
> `Diagnostics.razor` does not exist in M4-B.1).
> M4-B.1 does **not** create providers (per
> the M4-B brief: "Do not create providers").
>
> **Session:**
> `m4-b-1-host-capabilities-contract-and-service`
> (2026-07-13).
> **M4-B.1 task ID:** T-024.
> **Branch:**
> `feature/T-024-m4-b-1-host-capabilities-contract-and-service`
> (created from `main` at the M4-B plan
> promotion commit `131b8bd`; the M4-B.1
> closeout commit `feat(m4-b.1): add
> IHostCapabilitiesService contract and
> SystemHostCapabilitiesService
> implementation` is on this branch; the
> branch is fast-forwarded into `main` per
> the branching strategy rule 6; the branch
> is deleted per rule 7).
> **Plan reference:**
> `.claude/plans/generic-seeking-oasis.md`
> (the M4-B.1 implementation plan; approved
> via `ExitPlanMode` on 2026-07-13).

## 1. Plan Reference

- **M4-B plan:**
  `.ai/plans/M4-B-capability-detection.md`
  (Status: Awaiting Approval; canonical
  M4-B scope).
- **M4-B.1 implementation plan:**
  `.claude/plans/generic-seeking-oasis.md`
  (12 sections; approved via `ExitPlanMode`
  on 2026-07-13).
- **M4-A plan:**
  `.ai/plans/M4-A-infrastructure-process-execution.md`
  (canonical M4-A reference; M4-B.1
  composes the M4-A contracts).
- **M4-B.1 task record:** `.ai/state/tasks.json`
  T-024 (M4-B.1 transitions
  `Ready` → `InProgress` → `Done` with the
  full evidence block).
- **M4-B.1 milestone record:**
  `.ai/state/milestones.json` (the M4-B
> slice block M4-B.1 is added with
> `status: Delivered` + the full evidence
> block).
- **M4-B.1 capability:** `.ai/state/capabilities.json`
> C-015 (the
> `IHostCapabilitiesService` evidence
> block is populated with `source_paths`
> + `tests`).

## 2. Summary

The M4-B.1 first session is a single coherent
slice that produces a working
`IHostCapabilitiesService` — the seam every
later capability-detection consumer composes.
M4-B.1 is structured in seven layers:

- **Layer 1 — Contract:** the
  `IHostCapabilitiesService` interface
  in
  `src/AiEng.Platform.Application/Capabilities/`.
- **Layer 2 — Records:** the
  `HostCapabilities` + `HostCapability`
  sealed record classes (the data
  envelope the contract returns).
- **Layer 3 — Probe types:** the internal
  `HostToolProbe` +
  `ProviderCredentialProbe` record types
  in
  `src/AiEng.Platform.Infrastructure/Capabilities/`.
- **Layer 4 — Implementation:** the
  `SystemHostCapabilitiesService` sealed
  class (6 host tool probes + 6 provider
> credential probes; 5-second per-tool
> linked `CancellationTokenSource`
> timeout; `IPlatformInfo.IsWindows`
> gating; per-tool `Regex` version
> parsing; outer-cancellation
> propagation via re-throw; exception
> swallowing with `LogWarning`).
- **Layer 5 — Composition root:** the
> `AddHostCapabilities` extension
> (`TryAddSingleton<IHostCapabilitiesService,
> SystemHostCapabilitiesService>`).
- **Layer 6 — Wire-up:** one
> `using` + one `services.AddHostCapabilities();`
> call in
> `ServiceCollectionExtensions.AddPlatformServices`.
- **Layer 7 — Tests:** 20 unit tests +
> 3 in-line test doubles
> (`FakeProcessRunner`,
> `FakeCredentialVault`,
> `FakePlatformInfo`) covering
> constructor argument validation, the
> happy path, non-zero exit, exception,
> timeout, non-Windows host gating,
> credential available / absent, version
> parsing, version pattern miss,
> `DetectedAt` `TimeProvider`
> injection, `DetectedAt` call window,
> deterministic order, cancellation
> before, cancellation during, and
> timeout swallowed.

M4-B.1 is the smallest M4-B implementation
slice that produces a working
`IHostCapabilitiesService`. M4-B.1 does
**not** ship the design-system
components, the `/diagnostics` page, the
startup log, the documentation, or the
architecture test (all of these are
M4-B.2 or M4-B.3 scope).

## 3. Files Created

### 3.1 Contracts

- `src/AiEng.Platform.Application/Capabilities/IHostCapabilitiesService.cs`
  (10 lines): the
  `IHostCapabilitiesService` pure
  interface with the single async
  method
  `Task<HostCapabilities>
  DetectAsync(CancellationToken
  cancellationToken = default)`. The
  `Capabilities/` directory is a new
  directory under
  `src/AiEng.Platform.Application/`.

### 3.2 Records

- `src/AiEng.Platform.Application/Capabilities/HostCapabilities.cs`
  (15 lines): the `HostCapabilities`
  sealed record class (carries
  `IReadOnlyList<HostCapability>
  Capabilities` + `DateTimeOffset
  DetectedAt`) and the `HostCapability`
  sealed record class (carries
  `string Key` + `bool Available` +
  `string? Version` + `bool
  CredentialAvailable` + `string?
  CredentialName`). Both are pure data
  envelopes; the `Capabilities` list is
  never null (empty list when nothing
  is detected).

### 3.3 Implementation

- `src/AiEng.Platform.Infrastructure/Capabilities/CapabilityProbe.cs`
  (15 lines): the internal
  `HostToolProbe` sealed record (carries
  `string Key` + `string Executable` +
  `string Arguments` + `bool
  WindowsOnly` + `Regex VersionPattern`)
  and the `ProviderCredentialProbe`
  sealed record (carries `string Key` +
  `string CredentialName`). The records
  are `internal` to the
  `AiEng.Platform.Infrastructure`
  assembly (visible to the test project
  via the existing `InternalsVisibleTo`
  attribute in the csproj).
- `src/AiEng.Platform.Infrastructure/Capabilities/SystemHostCapabilitiesService.cs`
  (190+ lines): the
  `SystemHostCapabilitiesService` sealed
  class. The ctor takes `IProcessRunner`
  + `ICredentialVault` + `IPlatformInfo`
  + `ILogger<SystemHostCapabilitiesService>`
  + an optional `TimeProvider` (the
  secondary ctor with the
  `TimeProvider` parameter defaults to
  `TimeProvider.System` when omitted).
  The `DetectAsync` method iterates the
  `HostToolProbes` (sequentially) +
  the `ProviderCredentialProbes`
  (sequentially) and returns a
  `HostCapabilities` with the current
  `DetectedAt` timestamp. The probe
  order is stable: `git` → `ollama` →
  `powershell` → `wsl` → `wt` → `bash`
  → `provider:git` →
  `provider:ollama` →
  `provider:powershell` → `provider:wsl`
  → `provider:wt` → `provider:bash`.

### 3.4 Composition root

- `src/AiEng.Platform.App/Composition/Capabilities/CapabilitiesServiceCollectionExtensions.cs`
  (25 lines): the
  `CapabilitiesServiceCollectionExtensions`
  static class with the
  `AddHostCapabilities` extension. The
  extension is `null`-safe, registers
  `IHostCapabilitiesService` →
  `SystemHostCapabilitiesService` via
  `TryAddSingleton` (idempotent), and
  returns `services` for chaining. The
  `Capabilities/` directory is a new
  directory under
  `src/AiEng.Platform.App/Composition/`.

### 3.5 Unit tests

- `tests/AiEng.Platform.UnitTests/Capabilities/SystemHostCapabilitiesServiceTests.cs`
  (410+ lines): the 20 unit tests + 3
  in-line test doubles. The test
  class mirrors the M4-A.1
  `WindowsCredentialVaultTests` +
  `IProcessRunnerTests` patterns
  (file-scoped namespace
  `AiEng.Platform.UnitTests.Capabilities`;
  `public class
  SystemHostCapabilitiesServiceTests`;
  field-level initialisers; `[Fact]`
  test methods; `Assert.True / False /
  Equal / Contains`;
  `Assert.ThrowsAnyAsync`;
  `NullLogger<SystemHostCapabilitiesService>`
  from
  `Microsoft.Extensions.Logging.Abstractions`).

### 3.6 Documentation

None. M4-B.1 does not modify any
documentation. The M4-B plan
promotion already updated
`docs/infrastructure.md` § 11 (M4-B
Consumers), `ROADMAP.md` M4-B row +
M4-B Definition of done, and
`.ai/plans/master-delivery-plan.md`
section 1 M4-B row + section 3 M4-B
block + M4-B slice breakdown table.
M4-B.1 does not ship
`docs/capabilities.md` (M4-B.3's
responsibility).

### 3.7 State

- `.ai/handoffs/2026-07-13-m4-b-1-host-capabilities-contract-and-service.md`
  (the M4-B.1 per-session handoff;
  mirrored to `.ai/handoffs/latest.md`).
- `implementation-report-m4-b-1-host-capabilities-contract-and-service.md`
  (this file; the M4-B.1
  implementation report).
- `.ai/state/session.json` (M4-B.1
  envelope; the previous M4-B plan
  promotion envelope is preserved).
- `.ai/state/tasks.json` (T-024
  record added; T-025 M4-B.2 record
  added in `Ready`).
- `.ai/state/current.md` (active
  slice M4-B → M4-B.1; last completed
  task T-024; next recommended task
  T-025).
- `.ai/state/task-board.md` (M4-B.1
  row in `Done Recently`; M4-B.2 stub
  row in `Ready`).
- `.ai/state/milestones.json` (M4-B
  slice block M4-B.1 added with
  `status: Delivered` + the full
  evidence block; M4-B.3 title updated
  to include the deferred
  `Capabilities_Resolved_Through_Service`
  architecture test).
- `.ai/state/capabilities.json`
  (C-015 `evidence.source_paths` +
  `evidence.tests` populated; C-015
  `next_task` set to `T-025`; C-015
  `last_updated` set to `2026-07-13`;
  top-level `updated_at` +
  `updated_by_session` set to the
  M4-B.1 session).

## 4. Files Modified

### 4.1 ServiceCollectionExtensions.cs

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
  `AddHostCapabilities` is last
  because it composes the seam
  registered by `AddInfrastructure`
  (`IProcessRunner` + `ICredentialVault`
  + `IPlatformInfo`).

## 5. Files Deleted

None.

## 6. Architecture

### 6.1 Capability detection contract

The
`IHostCapabilitiesService` contract is
the seam every later capability-detection
consumer composes. The contract has a
single async method:
`Task<HostCapabilities>
DetectAsync(CancellationToken
cancellationToken = default)`. The
contract is a pure interface (no `using`
directives; file-scoped namespace
`AiEng.Platform.Application.Capabilities`).
The contract follows the M4-A pattern
(no XML doc comments; `CancellationToken`
parameter with `= default`; pure
interface; no `using` directives). The
contract is the smallest possible API
surface that the M4-B.2 components, the
M4-B.3 page, the M4-C provider registry,
and the M5+ worktree / agent launch can
compose.

### 6.2 Capability record type

The `HostCapabilities` +
`HostCapability` sealed record classes
are the data envelope the contract
returns. `HostCapabilities` carries
`IReadOnlyList<HostCapability>
Capabilities` (never null; empty list
when nothing is detected) +
`DateTimeOffset DetectedAt`.
`HostCapability` carries
`string Key` (the tool's logical key,
e.g., `"git"`, `"ollama"`,
`"powershell"`, `"wsl"`, `"wt"`,
`"bash"`, `"provider:git"`,
`"provider:ollama"`, etc.) +
`bool Available` + `string? Version`
(null when not available) +
`bool CredentialAvailable` +
`string? CredentialName` (the
credential key that was probed, e.g.,
`"provider:git:token"`; null for
non-credential probes). The `Key`
distinguishes host tools from provider
credentials: a host tool `Key` is the
lowercase tool name; a credential `Key`
is `"provider:<provider>:<slot>"`
(e.g., `"provider:git:token"`).

### 6.3 System implementation

The `SystemHostCapabilitiesService`
sealed class composes the three M4-A
contracts (`IProcessRunner` +
`ICredentialVault` + `IPlatformInfo`).
The implementation probes six host
tools (`git`, `ollama`,
`powershell.exe`, `wsl.exe`, `wt.exe`,
`bash.exe`) via
`IProcessRunner.RunToCompletionAsync(tool,
new[] { arguments }, linkedCts.Token)`
with a 5-second per-tool
`CancellationTokenSource` timeout
linked with the outer token, and reads
six provider credentials via
`ICredentialVault.GetAsync("provider:<key>:token",
ct)`. Windows-only tools
(`powershell.exe`, `wsl.exe`, `wt.exe`)
are gated on `IPlatformInfo.IsWindows`;
on non-Windows hosts the Windows-only
tools return `Available: false` +
`Version: null` without invoking the
process. Exit 0 →
`Available: true` + parsed `Version`;
non-zero / exception / timeout →
`Available: false` + `Version: null`.
The `ExtractVersion` helper uses a
`Regex.Match` on the process standard
output and returns the first capture
group's value, trimmed. The `Regex` is
generated at probe-table construction
time and is reused across invocations.

### 6.4 Composition root

The
`AddHostCapabilities` composition root
extension registers
`IHostCapabilitiesService` →
`SystemHostCapabilitiesService` via
`TryAddSingleton` (idempotent). The
extension is `null`-safe (throws
`ArgumentNullException` on null
`services`) and returns `services` for
chaining. The extension follows the
M4-A.1 `AddInfrastructure` pattern
(null-check + `TryAddSingleton` +
return). The wire-up in
`ServiceCollectionExtensions.AddPlatformServices`
adds the `using` directive + the
`services.AddHostCapabilities();` call
after the existing
`services.AddInfrastructure();` call.

### 6.5 TimeProvider injection

The `DetectedAt` timestamp is captured
via `_clock.GetUtcNow()` where
`_clock` is a `TimeProvider` field.
The ctor accepts an optional
`TimeProvider` parameter; when null
(or omitted), `TimeProvider.System` is
used. The unit tests inject a
`FixedTimeProvider` (a 3-line subclass
that overrides `GetUtcNow` to return a
fixed timestamp) for the
`DetectedAt_equals_TimeProvider_value`
test, and use the default
`TimeProvider.System` for the
`DetectedAt_within_call_window` test.
The `TimeProvider` injection is
minimum-blast-radius: the production
code path uses `TimeProvider.System`
(the standard .NET 8+ time abstraction),
and the test code path can inject a
`FixedTimeProvider` to make the
timestamp deterministic.

## 7. Validation Results

1. **Format gate:**
   `dotnet format --verify-no-changes`
   passed. The new files use 4-space
   indent + CRLF (per `.editorconfig`);
   the modified
   `ServiceCollectionExtensions.cs`
   retains the existing format.
2. **Build gate:**
   `dotnet build AiEng.Platform.slnx`
   passed with 0 warnings, 0 errors
   (with `TreatWarningsAsErrors=true`
   from `Directory.Build.props`).
3. **Test gate:**
   `dotnet test AiEng.Platform.slnx
   --no-build` reports **343 passed**,
   0 failed, 9 skipped. The M4-B.1
   ships 20 new unit tests; the
   pre-M4-B.1 baseline was 323 passed
   (79 unit + 232 bUnit + 12
   architecture). The post-M4-B.1 total
   is 343 passed (99 unit + 232 bUnit
   + 12 architecture). The 9 skipped
   are: 3 `AxeCoreAuditTests` (activate
   in M4-D) + 4
   `CompositionRootBoundaryTests`
   (activate in M4-D) + 2
   `Infrastructure_Respects_*` (activate
   in M4-D).
4. **JSON validation gate:** the
   state files (`session.json`,
   `tasks.json`, `milestones.json`,
   `capabilities.json`) are valid
   JSON; the `updated_at` field is
   updated; the schema is preserved.
5. **Markdown validation gate:** the
   M4-B.1 handoff + the M4-B.1
   implementation report are
   well-formed markdown; the headings
   are nested correctly; the code
   blocks are fenced with three
   backticks; the links use the
   `[text](url)` format.
6. **CRLF validation gate:** every new
   `.cs` file is CRLF; the modified
   `ServiceCollectionExtensions.cs` is
   CRLF (`unix2dos` applied to all new
   + modified files).
7. **Architecture boundary gate:** the
   M4-B.1 implementation does not
   introduce `System.Diagnostics.Process`
   usage outside
   `src/AiEng.Platform.Infrastructure/`
   (the `SystemHostCapabilitiesService`
   composes `IProcessRunner`, not
   `Process.Start`); the M4-B.1
   implementation does not introduce
   `advapi32.dll` P/Invoke outside
   `src/AiEng.Platform.Infrastructure/`
   (the `SystemHostCapabilitiesService`
   composes `ICredentialVault`, not the
   `advapi32` API); the M4-B.1
   implementation does not introduce a
   `Microsoft.Extensions.DependencyInjection`
   `IServiceCollection` extension
   outside
   `src/AiEng.Platform.App/Composition/`
   (the `AddHostCapabilities` extension
   is in
   `App/Composition/Capabilities/`).
8. **DoD gate:** every item in the
   M4-B.1 scope (per the plan § Goal
   § 1-11) is checked. The check is
   by inspection: every DoD bullet is
   marked satisfied below in
   § 9 Definition of Done.
9. **No scope creep:** the diff does
   not modify any file under
   `src/AiEng.Platform.Application/Infrastructure/`,
   `src/AiEng.Platform.Infrastructure/ProcessRunner/`,
   `src/AiEng.Platform.Infrastructure/Credentials/`,
   `src/AiEng.Platform.Infrastructure/Platform/`,
   `src/AiEng.Platform.App/Components/`,
   `src/AiEng.Platform.App/Components/Pages/`,
   `src/AiEng.Platform.App/Program.cs`,
   `src/AiEng.Platform.Providers.Abstractions/`,
   `src/AiEng.Platform.Domain/`,
   `docs/`, `ROADMAP.md`,
   `.ai/plans/`, `AGENTS.md`,
   `ARCHITECTURE.md`, `DECISIONS.md`,
   `STYLEGUIDE.md`, `CONTRIBUTING.md`,
   `.ai/workflows/`, `tailwind.config.js`,
   `package.json`, or
   `Directory.Build.props`. The diff
   is limited to the four new source
   files + the new composition root
   + the new test file + the modified
   `ServiceCollectionExtensions.cs` +
   the new handoff + this
   implementation report + the six
   state file updates.
10. **Push decision:** push is **not**
    authorised in this session. The
    push decision recorded in the
    handoff is **Staged for push** (the
    user did not authorise in this
    session; the M4-B.1 did not push;
    the next user command may push).

## 8. Tests Added

### 8.1 Unit tests

The M4-B.1 ships 20 unit tests in
`tests/AiEng.Platform.UnitTests/Capabilities/SystemHostCapabilitiesServiceTests.cs`:

1. `Constructor_throws_ArgumentNullException_when_processRunner_is_null`.
2. `Constructor_throws_ArgumentNullException_when_credentialVault_is_null`.
3. `Constructor_throws_ArgumentNullException_when_platformInfo_is_null`.
4. `Constructor_throws_ArgumentNullException_when_logger_is_null`.
5. `DetectAsync_returns_all_six_host_tools_with_Available_true_when_each_returns_exit_zero`.
6. `DetectAsync_returns_Available_false_for_host_tool_with_nonzero_exit_code`.
7. `DetectAsync_returns_Available_false_for_host_tool_that_throws_exception`.
8. `DetectAsync_returns_Available_false_for_host_tool_that_times_out`.
9. `DetectAsync_on_non_Windows_host_returns_Available_false_for_windows_only_tools`.
10. `DetectAsync_on_non_Windows_host_still_probes_cross_platform_tools`.
11. `DetectAsync_returns_CredentialAvailable_true_when_credential_vault_returns_a_value`.
12. `DetectAsync_returns_CredentialAvailable_false_when_credential_vault_returns_null`.
13. `DetectAsync_parses_git_version_from_standard_output`.
14. `DetectAsync_returns_Version_null_when_version_pattern_does_not_match`.
15. `DetectAsync_sets_DetectedAt_to_the_TimeProvider_value`.
16. `DetectAsync_sets_DetectedAt_to_a_timestamp_within_the_call_window_when_using_default_clock`.
17. `DetectAsync_returns_capabilities_in_deterministic_order`.
18. `DetectAsync_throws_OperationCanceledException_when_cancellation_token_is_cancelled_before_call`.
19. `DetectAsync_propagates_cancellation_when_cancellation_token_is_cancelled_during_call`.
20. `DetectAsync_swallows_timeout_without_propagating_when_cancellation_token_is_not_cancelled`.

The test class uses
`NullLogger<SystemHostCapabilitiesService>`
from
`Microsoft.Extensions.Logging.Abstractions`
(per the M4-A test pattern).

### 8.2 Test doubles

The M4-B.1 ships 3 in-line test
doubles in the same test file (the
test doubles are not extracted to a
shared test fixture; the M4-A.1
pattern of inline test doubles is
preserved):

- `FakeProcessRunner` (implements
  `IProcessRunner.RunToCompletionAsync`
  + `IProcessRunner.RunAsync`): records
  each invocation in
  `Calls: List<(string Executable,
  IReadOnlyList<string> Arguments)>`;
  increments `CallCount`; returns a
  scripted `ProcessResult` per
  executable (via
  `QueueByExecutable`); throws a
  scripted `Exception` per executable
  (via `QueueExceptionForExecutable`);
  awaits a scripted `TimeSpan` delay
  per executable (via
  `QueueDelayForExecutable`). The
  `RunAsync` method is implemented as
  an empty async enumerable (the M4-B
  service does not use `RunAsync`; the
  test double still has to implement
  the interface).
- `FakeCredentialVault` (implements
  `ICredentialVault.GetAsync` +
  `SetAsync` + `DeleteAsync`): stores
  credentials in a `Dictionary<string,
  string>` (case-sensitive); returns
  the stored value or `null` (per the
  `ICredentialVault` contract: `null`
  = absent).
- `FakePlatformInfo` (implements
  `IPlatformInfo.IsWindows` +
  `GetDataDirectory` + `GetConfigDirectory`):
  `IsWindows` is a settable property
  (defaults to `true`); the directory
  methods return paths under
  `Path.GetTempPath()` + `"aieng"`.

A 4th test double is the
`FixedTimeProvider` (extends
`TimeProvider`; overrides
`GetUtcNow` to return a fixed
`DateTimeOffset`). This is a 3-line
class with no test-state; it is
declared as a nested `private sealed
class` in the test class.

### 8.3 Test count

- **Pre-M4-B.1 baseline:** 323
  passed (79 unit + 232 bUnit + 12
  architecture), 0 failed, 9 skipped
  (per ADR-016 / M4-D).
- **M4-B.1 delta:** +20 unit tests
  (the 18 original plan tests + 2
  from splitting the `DetectedAt`
  test).
- **Post-M4-B.1 total:** 343 passed
  (99 unit + 232 bUnit + 12
  architecture), 0 failed, 9 skipped
  (per ADR-016 / M4-D).

## 9. Definition of Done

The M4-B.1 DoD is the 11 items in the
M4-B.1 plan § Goal § 1-11. Every item
is satisfied:

- [x] The `IHostCapabilitiesService`
  contract is at
  `src/AiEng.Platform.Application/Capabilities/IHostCapabilitiesService.cs`
  (single async method
  `Task<HostCapabilities>
  DetectAsync(CancellationToken
  cancellationToken = default)`).
- [x] The `HostCapabilities` +
  `HostCapability` sealed records are
  at
  `src/AiEng.Platform.Application/Capabilities/HostCapabilities.cs`.
- [x] The `SystemHostCapabilitiesService`
  implementation is at
  `src/AiEng.Platform.Infrastructure/Capabilities/SystemHostCapabilitiesService.cs`
  (6 host tool probes + 6 provider
  credential probes; 5-second per-tool
  linked `CancellationTokenSource`
  timeout; `IPlatformInfo.IsWindows`
  gating; per-tool `Regex` version
  parsing; outer-cancellation
  propagation via re-throw; exception
  swallowing with `LogWarning`).
- [x] The `AddHostCapabilities`
  composition root extension is at
  `src/AiEng.Platform.App/Composition/Capabilities/CapabilitiesServiceCollectionExtensions.cs`
  (`TryAddSingleton<IHostCapabilitiesService,
  SystemHostCapabilitiesService>`;
  mirrors `AddInfrastructure` pattern).
- [x] The wire-up is in
  `src/AiEng.Platform.App/Composition/ServiceCollectionExtensions.cs`
  (one `using` + one
  `services.AddHostCapabilities();`
  call).
- [x] 20 unit tests + 3 in-line test
  doubles are at
  `tests/AiEng.Platform.UnitTests/Capabilities/SystemHostCapabilitiesServiceTests.cs`.
- [x] The project-continuity state is
  updated per Rule 15: `session.json`
  (M4-B.1 envelope), `tasks.json`
  (T-024 transitions `Ready` →
  `InProgress` → `Done` with the full
  evidence block; T-025 M4-B.2 record
  added in `Ready`),
  `current.md` (active slice = M4-B.1;
  last stable commit = M4-B.1 closeout
  commit; next recommended task = T-025
  M4-B.2),
  `task-board.md` (M4-B.1 in `Done
  Recently`; M4-B.2 in `Ready`),
  `milestones.json` (M4-B slice block
  M4-B.1 added with `status: Delivered`
  + the full evidence block),
  `capabilities.json` (C-015
  `evidence.source_paths` + `evidence.tests`
  populated; C-015 `next_task` set to
  `T-025`; C-015 `last_updated` set to
  `2026-07-13`; top-level `updated_at` +
  `updated_by_session` updated).
- [x] The M4-B.1 per-session handoff
  is at
  `.ai/handoffs/2026-07-13-m4-b-1-host-capabilities-contract-and-service.md`
  (mirrored to
  `.ai/handoffs/latest.md`).
- [x] The M4-B.1 implementation report
  is at
  `implementation-report-m4-b-1-host-capabilities-contract-and-service.md`
  (this file; 15 sections mirroring
  the M4-A.1 + M4-A.2 reports).
- [x] The coherent commit is
  `feat(m4-b.1): add
  IHostCapabilitiesService contract and
  SystemHostCapabilitiesService
  implementation` on
  `feature/T-024-m4-b-1-host-capabilities-contract-and-service`;
  the branch is fast-forwarded into
  `main` per the branching strategy
  rule 6; the branch is deleted per
  rule 7.
- [x] Push is **skipped** (not
  authorised in this session). The
  push decision recorded in the
  handoff is **Staged for push**.

## 10. Git

- **Branch:**
  `feature/T-024-m4-b-1-host-capabilities-contract-and-service`
  (created from `main` at the M4-B
  plan promotion commit `131b8bd`).
- **Commit:**
  `feat(m4-b.1): add
  IHostCapabilitiesService contract and
  SystemHostCapabilitiesService
  implementation` (push is staged for
  push, not authorised in this
  session).
- **Coherent commit contents:**
  4 new source files + 1 new
  composition root file + 1 new test
  file + 1 modified composition root
  file (`ServiceCollectionExtensions.cs`)
  + 1 new implementation report + 1
  new handoff + 6 state file updates
  (`session.json`, `tasks.json`,
  `current.md`, `task-board.md`,
  `milestones.json`, `capabilities.json`).
- **Coherent commit trailers:**
  `Co-Authored-By: Claude
  <noreply@anthropic.com>`.
- **Merge:** fast-forward into `main`
  per the branching strategy rule 6.
- **Branch delete:** the feature
  branch is deleted per the branching
  strategy rule 7.
- **Push:** skipped (not authorised
  in this session; the push decision
  is **Staged for push**).

## 11. Out of Scope

The M4-B.1 first session does **not**:

- Begin M4-B.2 (the
  `AppCapabilityList` + `AppKeyValueList`
  data-owning four-state design-system
  components).
- Begin M4-B.3 (the `/diagnostics`
  page + the startup capability-report
  log + the `docs/capabilities.md`
  documentation + the deferred
  `Capabilities_Resolved_Through_Service`
  architecture test).
- Begin M4-C (provider registry
  foundation).
- Begin M4-D (first concrete process
  providers).
- Create any `Providers.<X>` project.
- Activate any of the 4 disabled
  `CompositionRootBoundaryTests`.
- Activate any of the 3 disabled
  `AxeCoreAuditTests`.
- Activate any of the 2 disabled
  M4-A.1 `Infrastructure_Respects_*`
  architecture tests.
- Modify `docs/capabilities.md`
  (does not yet exist; M4-B.3 creates
  it).
- Modify `docs/infrastructure.md` §
  11 (the M4-B plan promotion already
  added it).
- Modify `ROADMAP.md` (the M4-B plan
  promotion already updated the M4-B
  row + the M4-B Definition of done).
- Modify
  `.ai/plans/master-delivery-plan.md`
  (the M4-B plan promotion already
  updated § 1 + § 3 + the M4-B slice
  breakdown table).
- Push to remote (not authorised in
  this session).

## 12. Lessons Learned

1. **Outer-cancellation catch order
   matters.** The original plan's
   `catch
   (OperationCanceledException) when
   (timeoutCts.IsCancellationRequested &&
   !cancellationToken.IsCancellationRequested)`
   is insufficient: when the outer
   token is cancelled, the filter is
   `false` (because
   `cancellationToken.IsCancellationRequested`
   is `true`), and the exception falls
   through to the generic
   `catch (Exception ex)` block, which
   silently swallows the cancellation.
   The fix is to add an explicit
   `catch
   (OperationCanceledException) when
   (cancellationToken.IsCancellationRequested)`
   that re-throws, ordered **before**
   the timeout catch (the timeout catch
   filter is mutually exclusive: the
   outer-cancellation catch is true
   when the outer is cancelled,
   regardless of whether the timeout
   also fired; the timeout catch is
   true when the timeout fired AND the
   outer did NOT). The order of the
   two filters matters; reversing them
   would re-introduce the bug. This is
   a subtle C# `catch when` filter
   composition lesson that would have
   been caught earlier by the
   `cancellation_during_call` test.
2. **The `DetectedAt` test was split
   into two tests for clarity.** The
   original plan test asserted that
   `DetectedAt` was set within the
   call window when the default
   `TimeProvider` was used. This is
   non-deterministic (the test depends
   on the wall clock). The split: (a)
   `DetectedAt_equals_TimeProvider_value`
   (deterministic with
   `FixedTimeProvider`); (b)
   `DetectedAt_within_call_window` (uses
   the default `TimeProvider.System`).
   This separation makes the
   `TimeProvider` injection seam
   explicit (the test for the seam) and
   keeps the non-deterministic test for
   the regression (the test for the
   implementation's default behavior).
   The pattern is reusable for any
   `TimeProvider`-aware service.
3. **Probe-table construction is
   done once at type-init time.** The
   `HostToolProbes` + `ProviderCredentialProbes`
   arrays are `private static readonly`,
   not `private const` (they are
   reference-type arrays). The arrays
   are constructed once at type
   initialisation and reused across
   all invocations. The `Regex`
   instances inside `HostToolProbes`
   are `RegexOptions.Compiled` (the
   standard compile-once pattern). The
   `Regex` instances are not cached
   globally (they are scoped to the
   `HostToolProbes` array); the cost
   is one `Regex` per probe × the
   number of probes = 6 compiled
   `Regex` instances at type
   initialisation, which is negligible.
4. **The Windows-only gating is a
   pre-check, not a try-catch.** The
   `IPlatformInfo.IsWindows` check is
   done in `ProbeHostToolAsync` **before**
   creating the `CancellationTokenSource`
   + linked CTS + process invocation.
   This avoids the overhead of
   creating a 5-second timer for a
   probe that we know will be skipped.
   The pre-check is also testable in
   isolation (the
   `DetectAsync_on_non_Windows_host_returns_Available_false_for_windows_only_tools`
   test verifies that the
   `IProcessRunner.CallCount` is 0
   for the Windows-only tools on a
   non-Windows host; the cross-platform
   `DetectAsync_on_non_Windows_host_still_probes_cross_platform_tools`
   test verifies that the
   `IProcessRunner.CallCount` is 3
   for the cross-platform tools on a
   non-Windows host).
5. **The credential `Key` uses
   `"provider:<provider>:<slot>"`
   to distinguish from host tool
   `Key`s.** The `Key` for a host
   tool probe is the lowercase tool
   name (e.g., `"git"`, `"ollama"`,
   `"powershell"`, `"wsl"`, `"wt"`,
   `"bash"`). The `Key` for a
   provider credential probe is
   `"provider:<provider>:<slot>"`
   (e.g., `"provider:git:token"`,
   `"provider:ollama:token"`,
   `"provider:powershell:token"`,
   `"provider:wsl:token"`,
   `"provider:wt:token"`,
   `"provider:bash:token"`). The
   prefix `"provider:"` is a
   namespace that distinguishes
   credential probes from host tool
   probes; the `<slot>` allows
   future extension (e.g.,
   `"provider:git:token"` +
   `"provider:git:email"` for a
   future "Git + GitHub" provider).
6. **The architecture test placement
   was resolved by deferring to
   M4-B.3.** The
   `Capabilities_Resolved_Through_Service`
   architecture test asserts that
   `Diagnostics.razor` exists, that
   it contains `@inject
   IHostCapabilitiesService`, that no
   `RunToCompletionAsync` token
   appears in
   `App/Components/Diagnostics/`, and
   that no `ICredentialVault` direct
   call appears in
   `App/Components/Diagnostics/`. The
   file `Diagnostics.razor` does not
   exist in M4-B.1 (it lands in
   M4-B.3). M4-B.1 would have had to
   either create a fake
   `Diagnostics.razor` only to make
   the test pass (scope creep) or
   skip the test (which violates the
   M4-B plan § 2 In Scope § 9). The
   M4-B.1 plan resolved this by
   deferring the architecture test to
   M4-B.3, where `Diagnostics.razor`
   exists. The deviation is recorded
   in § 14 Deviations below and in
   `.ai/state/milestones.json` and
   `.ai/state/capabilities.json`
   C-015 `architecture_tests`.
7. **The `InternalsVisibleTo` attribute
   is preserved across the
   Infrastructure layer.** The
   `AiEng.Platform.Infrastructure.csproj`
   already has
   `[assembly:
   InternalsVisibleTo("AiEng.Platform.UnitTests")]`
   (per the M4-A.1 setup). This
   allows the M4-B.1 test project to
   see the `internal` `HostToolProbe`
   + `ProviderCredentialProbe` record
   types (no public exposure is
   needed; the test project is the
   only consumer that needs to see
   the internal types, and even the
   test project does not need to see
   them — the test doubles are
   constructed in the test class
   without referencing the internal
   types).

## 13. Handoff to M4-B.2

The next session is the **M4-B.2 first
session** (T-025) on the user's
`Approve` or `Next` invocation. M4-B.2
ships the `AppCapabilityList` +
`AppKeyValueList` data-owning four-state
design-system components composing the
M1.2 primitives (per the M4-B plan § 2
items 4 + 5 + the M4-B.2 stub row in
`.ai/state/tasks.json`).

M4-B.2 will:

- Land the `AppCapabilityList`
  component in
  `src/AiEng.Platform.App/Components/Capabilities/AppCapabilityList.razor`
  (a data-owning four-state component
  that takes an
  `IReadOnlyList<HostCapability>` as
  its parameter and renders the
  capabilities in the design-system
  voice using the M1.2 primitives
  `AppPanel` + `AppCard` + `AppStack` +
  `AppBadge` + `AppStatusDot`).
- Land the `AppKeyValueList`
  component in
  `src/AiEng.Platform.App/Components/Capabilities/AppKeyValueList.razor`
  (a data-owning four-state component
  that takes an
  `IReadOnlyList<(string Key, string
  Value)>` as its parameter and
  renders the key-value pairs in the
  design-system voice using the M1.2
  primitive `AppStack` + the M1.2
  typography).
- Add the two components to the
  M1.2 design-system showcase on
  `/design-system` (M1 follow-up).
- Update `docs/design-system.md` §
  4.5 to add the two new components.
- Land 10+ bUnit tests for the two
  components (loading state +
  empty state + error state + data
  state for each component).

M4-B.2 is **not** the
`/diagnostics` page (M4-B.3) and does
**not** include the
`Capabilities_Resolved_Through_Service`
architecture test (M4-B.3).

## 14. Deviations

### 14.1 Architecture test placement

The M4-B plan § 2 In Scope § 9 lists
the
`Capabilities_Resolved_Through_Service`
architecture test in the M4-B.1 scope.
On inspection, the test asserts:

- `Diagnostics.razor` contains
  `@inject IHostCapabilitiesService`.
- No `RunToCompletionAsync` token
  appears in
  `App/Components/Diagnostics/`.
- No `ICredentialVault` direct call
  appears in
  `App/Components/Diagnostics/`.

The file `Diagnostics.razor` does not
exist in M4-B.1 (it lands in M4-B.3).
M4-B.1 would have had to either create
a fake `Diagnostics.razor` only to
make the test pass (scope creep) or
skip the test (which violates the M4-B
plan § 2 In Scope § 9).

**Resolution:** the architecture test
is deferred to M4-B.3 (the slice that
creates `Diagnostics.razor`). M4-B.1
ships 0 architecture tests. This is
recorded in:

- The M4-B.1 handoff § 3 Deviations
  (item 1).
- This M4-B.1 implementation report §
  12 Lessons Learned (item 6) + § 14
  Deviations (this section).
- `.ai/state/milestones.json` (the
  M4-B.3 title is updated to include
  the deferred architecture test; the
  M4-B.1 status is `Delivered`).
- `.ai/state/capabilities.json` C-015
  (`architecture_tests: ["Capabilities_Resolved_Through_Service
  (deferred to M4-B.3 per the M4-B.1
  plan section 14.1 Deviations)"]`).

The deviation is consistent with the
M4-B plan § 6 Coherent Commit § 3
("The M4-B plan is approved as-is; the
deviations are implementation-time
decisions, not plan-level changes") +
the M4-B.1 handoff § 3 ("the M4-B.1
first session does **not** implement
M4-B.2 / M4-B.3 / M4-C / M4-D").

### 14.2 DetectedAt test split

The M4-B.1 plan § Goal § 6 (l) lists
one `DetectedAt` test ("(i)
`DetectedAt` timestamp is set within
the call window"). The M4-B.1
implementation splits this into two
tests: (a) `DetectedAt` equals the
`TimeProvider` value (deterministic
with `FixedTimeProvider`); (b)
`DetectedAt` falls within the call
window when the default clock is used
(non-deterministic but bounded). The
split makes the `TimeProvider` injection
seam explicit (the test for the seam)
and keeps the non-deterministic test
for the regression (the test for the
implementation's default behavior).
The test count is +20 unit tests
(instead of the +18 in the original
plan).

### 14.3 Outer-cancellation catch
order

The M4-B.1 plan § Approach § 5 (the
`ProbeHostToolAsync` method body)
specifies a single
`catch
(OperationCanceledException) when
(timeoutCts.IsCancellationRequested
&& !cancellationToken.IsCancellationRequested)`
block. The M4-B.1 implementation
adds an explicit
`catch
(OperationCanceledException) when
(cancellationToken.IsCancellationRequested)`
block that re-throws, ordered
**before** the timeout catch. The
addition is necessary because the
single-filter approach silently
swallows outer cancellation (when the
outer token is cancelled, the filter
is `false`, and the exception falls
through to the generic `catch
(Exception)`). The fix is
documented in § 12 Lessons Learned
(item 1) + this § 14 Deviations
section.

## 15. Cross-References

- **M4-B plan:**
  `.ai/plans/M4-B-capability-detection.md`
  (Status: Awaiting Approval; canonical
  M4-B scope).
- **M4-B.1 implementation plan:**
  `.claude/plans/generic-seeking-oasis.md`
  (12 sections; approved via
  `ExitPlanMode` on 2026-07-13).
- **M4-A plan:**
  `.ai/plans/M4-A-infrastructure-process-execution.md`
  (canonical M4-A reference; M4-B.1
  composes the M4-A contracts).
- **M4-A.1 handoff:**
  `.ai/handoffs/2026-07-11-m4-a-1-infrastructure-project-skeleton.md`
  (the M4-B.1 handoff's template).
- **M4-A.1 implementation report:**
  `implementation-report-m4-a-1-infrastructure-project-skeleton.md`
  (the M4-B.1 implementation report's
  template).
- **M4-A.2 handoff:**
  `.ai/handoffs/2026-07-11-m4-a-2-open-action.md`
  (the second M4-A implementation
  slice; the M4-B.1 builds on the
  M4-A.2 `IPlatformInfo.IsWindows`
  property).
- **M4-A.2 implementation report:**
  `implementation-report-m4-a-2-open-action.md`
  (the second M4-A implementation
  slice's report).
- **M4-B plan promotion handoff:**
  `.ai/handoffs/2026-07-13-m4-b-plan-promotion.md`
  (the M4-B.1 handoff's "previous
  action" source).
- **M4-B.1 handoff:**
  `.ai/handoffs/2026-07-13-m4-b-1-host-capabilities-contract-and-service.md`
  (the M4-B.1 per-session handoff;
  mirrored to `.ai/handoffs/latest.md`).
- **M4-B.1 task record:**
  `.ai/state/tasks.json` T-024 (M4-B.1
  transitions `Ready` → `InProgress` →
  `Done` with the full evidence
  block).
- **M4-B.2 task record:**
  `.ai/state/tasks.json` T-025 (M4-B.2
  is `Ready`; the M4-B.1 promotes
  M4-B.2 from `Deferred` to `Ready`).
- **M4-B.1 milestone record:**
  `.ai/state/milestones.json` (the
  M4-B slice block M4-B.1 is added
  with `status: Delivered` + the full
  evidence block).
- **M4-B.1 capability:** `.ai/state/capabilities.json`
  C-015 (the
  `IHostCapabilitiesService` evidence
  block is populated with
  `source_paths` + `tests`).
- **M4-B.1 session record:**
  `.ai/state/session.json` (the
  M4-B.1 envelope is appended; the
  previous M4-B plan promotion
  envelope is preserved).
- **M4-B.1 task board entry:**
  `.ai/state/task-board.md` (M4-B.1
  row in `Done Recently`; M4-B.2 stub
  row in `Ready`).
- **M4-B.1 one-page snapshot:**
  `.ai/state/current.md` (active
  slice = M4-B.1; last stable commit
  = M4-B.1 closeout commit; next
  recommended task = T-025 M4-B.2).
- **The Milestone Closeout Standard:**
  `.ai/workflows/milestone-closeout.md`
  (the M4-B closeout will follow the
  standard; the M4-B.1 is a per-slice
  closeout, not a milestone closeout).
- **The branching strategy:**
  `.ai/workflows/branching-strategy.md`
  (rules 4, 6, 7 are the M4-B.1's
  branch operations).
- **The Progressive Coding Rule:**
  `.ai/workflows/progressive-coding.md`
  (the rule the M4-B.1 first session
  follows).
- **The command protocol:**
  `.ai/commands.md` (the `Next`
  command response shape — `Completed
  / Git / Validation / Evidence /
  Next`).
- **The M4-A documentation:**
  `docs/infrastructure.md` § 7 (the
  M4-A.2 Open Action reference; the
  M4-B.1 implementation composes the
  M4-A.1 + M4-A.2 contracts).

---

**End of M4-B.1 implementation report.**
The M4-B.1 first session is the
boundary slice of M4-B (the contract +
the records + the implementation + the
composition root + the unit tests; the
architecture test is deferred to
M4-B.3 per the § 14.1 Deviations
resolution). The M4-B.1 closeout
commit `feat(m4-b.1): add
IHostCapabilitiesService contract and
SystemHostCapabilitiesService
implementation` is on `main`; the
feature branch is fast-forwarded into
`main` and deleted. The M4-B.1
per-session handoff is at
`.ai/handoffs/2026-07-13-m4-b-1-host-capabilities-contract-and-service.md`
(mirrored to `.ai/handoffs/latest.md`).
The session does **not** begin M4-B.2
/ M4-B.3 / M4-C / M4-D / provider
creation (per the brief: "Do not begin
the following task"). The next session
is the M4-B.2 first session on the
user's `Approve` or `Next` invocation.
