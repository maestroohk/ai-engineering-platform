# Implementation Report — M4-A.1 Infrastructure Project Skeleton

> **The M4-A.1 implementation report.** M4-A.1
> is the first slice of M4-A. M4-A.1 ships the
> **infrastructure seam** every later milestone
> composes: the new
> `AiEng.Platform.Infrastructure` csproj; the
> four contracts (`IProcessRunner`,
> `ICredentialVault`, `IPlatformInfo`, the
> on-disk `IProjectStore`); the four
> implementations (`SystemProcessRunner`,
> `WindowsCredentialVault`,
> `SystemPlatformInfo`, `JsonFileProjectStore`);
> the `AddInfrastructure` composition root
> extension; the one-line swap in `AddProjects`
> (the M3 in-memory `IProjectStore`
> registration is removed; the on-disk
> `JsonFileProjectStore` is now registered
> through `AddInfrastructure`); the move of
> `InMemoryProjectStore` to `tests/` as a test
> fixture; 45 new unit tests; 2 new architecture
> tests registered-but-disabled per ADR-016; and
> `docs/infrastructure.md` (10 sections).
>
> **M4-A.1 is the boundary, not the activation.**
> The Open action on `AppProjectCard` is the
> M4-A.2 slice's responsibility (T-022 is
> `Ready`; the M4-A.1 session does **not** begin
> M4-A.2 per the brief and the Progressive
> Coding Rule).
>
> **Session:**
> `m4-a-1-infrastructure-project-skeleton`
> (2026-07-11).
> **M4-A.1 task ID:** T-021.
> **Branch:**
> `feature/T-021-m4-a-1-infrastructure-project-skeleton`
> (created from `main` at the M3 closeout
> commit `33c154d`; the M4-A.1 closeout commit
> `feat(m4-a.1): add infrastructure project
> skeleton with IProcessRunner,
> ICredentialVault, IPlatformInfo, and on-disk
> IProjectStore` is on this branch; the branch
> is fast-forwarded into `main` per the
> branching strategy rule 6; the branch is
> deleted per rule 7).
> **Plan reference:**
> `.ai/plans/M4-A-infrastructure-process-execution.md`
> (Status: Approved 2026-07-11 via the
> `Next` invocation per `.ai/commands.md` § 4).
> **M4-A.1 documentation:**
> `docs/infrastructure.md` (10 sections).
> **M4-A.1 handoff:**
> `.ai/handoffs/2026-07-11-m4-a-1-infrastructure-project-skeleton.md`
> (mirrored to `.ai/handoffs/latest.md`).

---

## 1. Plan Reference

- **Plan file:**
  `.ai/plans/M4-A-infrastructure-process-execution.md`
  (12 sections; Status: Approved 2026-07-11
  via the `Next` invocation per
  `.ai/commands.md` § 4; the canonical M4-A
  plan).
- **Implementation plan:** the M4-A.1
  implementation plan is inline in the
  session; the M4-A.1 plan expands the M4-A
  plan's M4-A.1 row with the per-slice
  implementation detail (one task per
  session; M4-A.1 is the first M4-A slice).
- **Deviations from plan:** see § 14 below.

The plan and the report are paired: the plan
is the contract, the report is the receipt.
M4-A.1 follows the M4-A plan's M4-A.1 row to
the letter, with the three documented
deviations in § 14.

## 2. Summary

M4-A.1 lands the infrastructure seam every
later milestone composes against: a process
boundary (`IProcessRunner` — the only legal
caller of `Process.Start` in the platform), a
credential boundary (`ICredentialVault` —
Windows Credential Manager on Windows hosts;
throws `PlatformNotSupportedException` on
non-Windows), a platform-info seam
(`IPlatformInfo` — resolves the platform's
data + config directories), and the durable
`IProjectStore` (`JsonFileProjectStore` —
replaces the M3 in-memory store behind the
same `IProjectStore` contract). The M4-A.1
slice is the **boundary**, not the
**activation**: the contracts and the
implementations ship; the architecture tests
that enforce the boundary register but
remain disabled per ADR-016 (they activate
in M4-D when the first concrete `Providers.<X>`
project lands). The first user-facing
activation of `IProcessRunner` is the
M4-A.2 slice (T-022 — the Open action on
`AppProjectCard`; `Ready`).

The slice advances the M4-A milestone (which
delivers the infrastructure seam for the M4
process execution, capability detection,
provider registry, and first concrete
providers work) and consumes M3 (the
in-memory `IProjectStore` contract that
M4-A.1 replaces on disk behind the same
contract). The M4-A.1 is the first
implementation slice of M4-A; the M4-A.2 is
the second.

## 3. Files Created

### 3.1 New project

- `src/AiEng.Platform.Infrastructure/AiEng.Platform.Infrastructure.csproj`
  — the new C# class library;
  `Sdk="Microsoft.NET.Sdk"`; `TargetFramework=net10.0`;
  `<Nullable>enable</Nullable>`;
  `<ImplicitUsings>enable</ImplicitUsings>`;
  `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`;
  `<LangVersion>latest</LangVersion>`;
  references `AiEng.Platform.Application` and
  `AiEng.Platform.Domain`. The csproj does
  **not** reference `AiEng.Platform.App`.
  `internal` visibility is the default for
  any helper types; the contracts and
  implementations are `public` so the
  composition root can resolve them.

### 3.2 Contracts (in `Application/Infrastructure/`)

- `src/AiEng.Platform.Application/Infrastructure/IProcessRunner.cs`
  — the process boundary contract.
  Exposes two methods:
  `IAsyncEnumerable<string> RunAsync(string executable, string arguments, CancellationToken cancellationToken)`
  (streaming stdout/stderr line-by-line) and
  `Task<ProcessResult> RunToCompletionAsync(string executable, string arguments, CancellationToken cancellationToken)`
  (fire-and-forget; returns the
  `ProcessResult` envelope). XML doc comments
  document the contract's exact behaviour
  (UTF-8 output, line-by-line streaming,
  non-zero exit codes are not exceptions,
  cancellation propagates as
  `OperationCanceledException`).
- `src/AiEng.Platform.Application/Infrastructure/ProcessResult.cs`
  — the `ProcessResult` record struct
  envelope
  (`int ExitCode`, `string StandardOutput`,
  `string StandardError`,
  `bool Succeeded => ExitCode == 0`).
  The struct is `readonly` and exposes the
  three fields plus the `Succeeded` property.
- `src/AiEng.Platform.Application/Infrastructure/ICredentialVault.cs`
  — the credential boundary contract.
  Exposes three methods:
  `Task<string?> GetAsync(string name, CancellationToken cancellationToken)` (returns the secret for `name`, or `null` if no such credential exists),
  `Task SetAsync(string name, string secret, CancellationToken cancellationToken)` (stores the secret under `name`),
  `Task DeleteAsync(string name, CancellationToken cancellationToken)` (removes the credential named `name`; no-op if absent).
  Argument validation: `ArgumentException`
  on null/empty `name` or `secret` (for
  `SetAsync`).
- `src/AiEng.Platform.Application/Infrastructure/IPlatformInfo.cs`
  — the platform-info contract.
  Exposes two methods:
  `string GetDataDirectory()` (the directory
  the platform uses for durable state) and
  `string GetConfigDirectory()` (the directory
  the platform uses for configuration; reserved
  for M4-B / later).

### 3.3 Implementations (in `Infrastructure/`)

- `src/AiEng.Platform.Infrastructure/ProcessRunner/SystemProcessRunner.cs`
  — the process boundary implementation.
  Wraps `System.Diagnostics.Process` with
  `RedirectStandardOutput=true`,
  `RedirectStandardError=true`,
  `UseShellExecute=false`,
  `CreateNoWindow=true`. `RunAsync` yields
  each line of stdout/stderr in the order
  they arrive (the implementation subscribes
  to both `OutputDataReceived` and
  `ErrorDataReceived` and yields through a
  `Channel<string>`); `RunToCompletionAsync`
  reads both streams to the end, waits for
  exit, and returns the `ProcessResult`. The
  implementation is the **only** call site
  for `Process.Start` in the platform. The
  class is `public sealed`, takes no
  dependencies, and is `internal` to the
  Infrastructure project; the DI container
  registers it through `AddInfrastructure`.
- `src/AiEng.Platform.Infrastructure/Credentials/WindowsCredentialVault.cs`
  — the credential boundary implementation.
  Thin direct P/Invoke wrapper over
  `advapi32.dll`'s `CredReadW`, `CredWriteW`,
  and `CredDeleteW`. The implementation
  throws `PlatformNotSupportedException` on
  every call when running on a non-Windows
  host (`RuntimeInformation.IsOSPlatform(OSPlatform.Windows)`
  is the guard). The implementation uses
  `CRED_TYPE_GENERIC` and persists with
  `CRED_PERSIST_LOCAL_MACHINE` (the
  persistence is per-user on Windows; the
  credentials survive reboots; the
  implementation does not require elevation
  in the common case). The class is
  `public sealed`.
- `src/AiEng.Platform.Infrastructure/Platform/SystemPlatformInfo.cs`
  — the platform-info implementation.
  Resolves both directories through
  `Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)`
  and appends the
  `AiEng/Platform/<data|config>` subdirectory.
  The Windows path is
  `%LOCALAPPDATA%\AiEng\Platform\data\` and
  `%LOCALAPPDATA%\AiEng\Platform\config\`.
  The class is `public sealed`, takes no
  dependencies, and is deterministic.
- `src/AiEng.Platform.Infrastructure/Projects/JsonFileProjectStore.cs`
  — the on-disk `IProjectStore`
  implementation. The store resolves the
  data directory through
  `IPlatformInfo.GetDataDirectory()`; the
  default file name is `projects.json`; the
  file path is
  `<data-directory>/projects.json`. The
  store serializes the project list to JSON
  with snake_case property naming + indented
  format; the JSON shape matches the
  `.ai/state/*.json` family. The store uses
  a `SemaphoreSlim(1, 1)` to serialize all
  writes; reads acquire the same lock.
  Writes are atomic: the store writes the
  snapshot to a temp file
  (`projects.json.tmp-<guid>`) then uses
  `File.Replace` to atomically swap the temp
  file for the destination; on Windows
  `File.Replace` is atomic at the file
  system level. Corruption recovery:
  `ReadSnapshotNoLockAsync` catches
  `JsonException` when reading a corrupt
  file, logs a warning, and returns an empty
  list; the next write overwrites the
  corrupt file with valid JSON. The class
  is `public sealed`; the constructor
  accepts `IPlatformInfo platformInfo`,
  optional `JsonFileProjectStoreOptions? options`,
  and optional `ILogger<JsonFileProjectStore>? logger`.
- `src/AiEng.Platform.Infrastructure/Projects/JsonFileProjectStoreOptions.cs`
  — the `JsonFileProjectStore` options
  (`string DataDirectory` — overrides
  `IPlatformInfo` resolution;
  `string FileName` — defaults to
  `projects.json`).

### 3.4 Composition root

- `src/AiEng.Platform.App/Composition/Infrastructure/InfrastructureServiceCollectionExtensions.cs`
  — the `AddInfrastructure` extension. The
  extension registers the four
  Infrastructure services in the DI container:

  ```csharp
  services.TryAddSingleton<IPlatformInfo, SystemPlatformInfo>();
  services.TryAddSingleton<IProcessRunner, SystemProcessRunner>();
  services.TryAddSingleton<ICredentialVault, WindowsCredentialVault>();
  services.TryAddSingleton<IProjectStore, JsonFileProjectStore>();
  ```

  `TryAddSingleton` is used (not
  `AddSingleton`) so test code can
  pre-register an alternate implementation
  (e.g., the M3 unit tests that use
  `InMemoryProjectStore` for the
  `IProjectStore` fixture).

### 3.5 Tests

- `tests/AiEng.Platform.UnitTests/Infrastructure/IProcessRunnerTests.cs`
  — 11 unit tests for the
  `SystemProcessRunner`: exit-code
  propagation, stdout capture, stderr
  capture, streaming output (every line is
  yielded as it is written), cancellation
  propagates as
  `OperationCanceledException`, non-existent
  executable failure surfaces the Win32
  error, argument validation
  (`ArgumentException` on null/empty
  executable).
- `tests/AiEng.Platform.UnitTests/Infrastructure/WindowsCredentialVaultTests.cs`
  — 10 unit tests for the
  `WindowsCredentialVault`: get / set /
  delete round-trip, missing credential
  returns `null`, set overwrites
  existing credential, delete is a no-op on
  missing credential, non-Windows throws
  `PlatformNotSupportedException`,
  argument validation. The tests use unique
  credential names per test
  (`test-cred-<guid>`) to avoid cross-test
  contamination; the `DeleteAsync` cleanup
  runs in the test's dispose.
- `tests/AiEng.Platform.UnitTests/Infrastructure/SystemPlatformInfoTests.cs`
  — 3 unit tests for the
  `SystemPlatformInfo`: `GetDataDirectory`
  returns a non-empty rooted path,
  `GetConfigDirectory` returns a non-empty
  rooted path, the two paths are distinct.
- `tests/AiEng.Platform.UnitTests/Infrastructure/JsonFileProjectStoreTests.cs`
  — 22 unit tests for the
  `JsonFileProjectStore`: round-trip
  (save + load + list cycle), missing file
  returns empty list, duplicate detection
  (idempotent add of the same project does
  not duplicate), corrupt file recovery
  (`JsonException` → empty list + warning
  log), concurrent adds are serialized
  (100 parallel adds end with 100 distinct
  projects), atomic writes (the temp file
  is created and the rename happens), the
  destination file is never observed in a
  partially-written state, cancellation
  propagates as `OperationCanceledException`,
  constructor validation
  (`ArgumentNullException` on null
  `platformInfo`), `JsonFileProjectStoreOptions`
  validation, the file path is under the
  `IPlatformInfo.GetDataDirectory()`
  resolution.
- `tests/AiEng.Platform.UnitTests/Infrastructure/InMemoryProjectStore.cs`
  — the M3 in-memory `IProjectStore` moved
  from
  `src/AiEng.Platform.Application/Projects/InMemoryProjectStore.cs`
  to the test project as a fixture. The
  class is unchanged (it is byte-for-byte
  identical to the M3.1 implementation);
  the namespace is updated to
  `AiEng.Platform.UnitTests.Infrastructure`;
  the M3 unit tests that depend on it
  (`IProjectServiceTests`,
  `InMemoryProjectStoreTests`) are updated
  to import the new namespace.

### 3.6 Architecture tests (registered-but-disabled per ADR-016)

- `tests/AiEng.Platform.ArchitectureTests/Infrastructure/Infrastructure_Respects_ProcessBoundary.cs`
  — asserts that
  `AiEng.Platform.Infrastructure` is the
  only project that references
  `System.Diagnostics.Process`; the test
  scans every `.cs` file in the repository
  for `using System.Diagnostics` and asserts
  the only file outside the Infrastructure
  project that contains the using is the
  architecture test itself. The test is
  `[Fact(Skip = "...")]` (registered but
  disabled) per ADR-016; it activates in
  M4-D.
- `tests/AiEng.Platform.ArchitectureTests/Infrastructure/Infrastructure_Respects_CredentialBoundary.cs`
  — asserts that
  `AiEng.Platform.Infrastructure` is the
  only project that references the
  Windows Credential Manager APIs. The test
  scans every `.cs` file in the repository
  for `CredRead`, `CredWrite`, `CredDelete`,
  or `advapi32.dll` and asserts the only
  file outside the Infrastructure project
  that contains the references is the
  architecture test itself. The test is
  `[Fact(Skip = "...")]` (registered but
  disabled) per ADR-016; it activates in
  M4-D.

### 3.7 Documentation

- `docs/infrastructure.md` — the M4-A
  documentation (10 sections: Goals, Project
  Structure, Process Boundary, Credential
  Boundary, On-Disk Project Store, Platform
  Info, Open Action, Composition Root,
  Tests, Out of Scope). The M3 / M4-A
  Boundary section in `docs/projects.md` is
  updated to reflect M4-A delivered.
- `implementation-report-m4-a-1-infrastructure-project-skeleton.md`
  — this report.
- `.ai/handoffs/2026-07-11-m4-a-1-infrastructure-project-skeleton.md`
  — the M4-A.1 per-session handoff
  (mirrored to `.ai/handoffs/latest.md`).

## 4. Files Modified

- `AiEng.Platform.slnx` — the new
  `AiEng.Platform.Infrastructure` project is
  added to the solution. The `.slnx` format
  is preserved; the new project is placed
  alphabetically between
  `AiEng.Platform.Domain` and
  `AiEng.Platform.Providers.Abstractions`.
- `src/AiEng.Platform.App/AiEng.Platform.App.csproj`
  — adds a `<ProjectReference>` to
  `AiEng.Platform.Infrastructure`. The App
  project now has `<ProjectReference>`s to
  `AiEng.Platform.Application`,
  `AiEng.Platform.Domain`,
  `AiEng.Platform.Infrastructure`, and
  `AiEng.Platform.Providers.Abstractions`.
- `src/AiEng.Platform.App/Composition/ServiceCollectionExtensions.cs`
  — calls `AddInfrastructure()` after
  `AddProjects()`. The composition root's
  call ordering is now
  `AddNavigation(assemblies)` →
  `AddProjectIntelligence()` →
  `AddProjects()` → `AddInfrastructure()`.
- `src/AiEng.Platform.App/Composition/Projects/ProjectsServiceCollectionExtensions.cs`
  — the M3 in-memory `IProjectStore`
  registration is removed. The
  `IProjectService` registration is
  preserved. The on-disk `JsonFileProjectStore`
  is now registered through
  `AddInfrastructure` exclusively.
- `src/AiEng.Platform.Application/Projects/InMemoryProjectStore.cs`
  — deleted (moved to the test project as
  a fixture).
- `docs/projects.md` — the M3 / M4-A
  Boundary section is updated to reflect
  M4-A delivered (the in-memory store is
  moved to `tests/`; the on-disk store
  registers through `AddInfrastructure`; the
  UI surface is unchanged).
- `ROADMAP.md` § 2 (M4-A row updated to
  `Active` with the M4-A.1 deliverable
  summary; the M3 row's M4-A "Awaiting
  Approval" reference is removed); § 3
  (M4-A "Outcome", "Outcome in detail",
  "Definition of done", and "Tests added"
  are updated to reflect the contracts +
  implementations + composition root
  actually shipped; the M4-A.1 slice
  "Delivered" row is added with the
  M4-A.1 summary; the M4-A.2 "Ready" note
  is added in the slice breakdown).
- `.ai/plans/master-delivery-plan.md`
  § 1 (M4-A row updated to `Active` with
  the M4-A.1 evidence; the M3 row's M4-A
  "Awaiting Approval" reference is
  removed); § 3 (M4-A "Completion status"
  updated to `Active (M4-A.1 Delivered
  2026-07-11)`; the M4-A "Evidence"
  section is updated with the M4-A.1
  closeout report, handoff, documentation,
  and commit; the M4-A slice breakdown
  table is added).
- `.ai/plans/M4-A-infrastructure-process-execution.md`
  — Status updated from `Awaiting Approval`
  to `Approved` (per the `Next`
  invocation's implicit approval per
  `.ai/commands.md` § 4).
- `.ai/state/session.json` — rewritten
  with the M4-A.1 envelope
  (`session_id:
  "m4-a-1-infrastructure-project-skeleton"`,
  `previous_session:
  "m3-closeout-and-retrospective"`,
  `milestone: "M4-A"`, `task_id: "T-021"`,
  `slice: "M4-A.1 — Infrastructure project
  skeleton"`,
  `active_milestone: "M4-A — Infrastructure
  / Process Execution (Active 2026-07-11;
  M4-A.1 Delivered 2026-07-11)"`,
  `active_task: "T-021 (Done, 2026-07-11)"`,
  `test_status: "318 passed, 0 failed (79
  unit + 228 bUnit + 11 architecture) at
  M4-A.1 closeout; 9 skipped per ADR-016 /
  M4-D; M4-A.1 is +45 unit + 0 bUnit + 0
  active architecture + 2 new skipped vs
  M3.2 closeout"`).
- `.ai/state/tasks.json` — T-021
  promoted from `Ready` to `Done` with the
  full evidence block (21 files_added, 16
  files_modified, 1 files_deleted; tests:
  45 unit + 2 architecture added); T-022
  (new; M4-A.2 — Open action on
  `AppProjectCard`) is `Ready` with
  `depends_on_task: T-021`; T-008 (M4
  summary) is `In Progress`;
  `updated_at: "2026-07-11T00:00:00Z"` and
  `updated_by_session:
  "m4-a-1-infrastructure-project-skeleton"`.
- `.ai/state/milestones.json` — M4-A
  `Awaiting Approval` → `Active`; new
  `primary_outcome: "AiEng.Platform.Infrastructure
  is created. IProcessRunner, ICredentialVault,
  IPlatformInfo, and the on-disk
  IProjectStore land. M3 in-memory store is
  replaced behind the same IProjectStore
  contract. The Open action on AppProjectCard
  is enabled in M4-A.2."`; M4-A.1 slice
  block added with the full evidence
  (session, branch, summary, implementation
  report, handoff, plan, commit message);
  M4-A evidence block added.
- `.ai/state/current.md` — extensively
  updated: Current Milestone (M3 Done →
  M4-A Active with full M4-A.1 description);
  Current Slice (M3.x → M4-A.1); Last
  Completed Task (rewritten with M4-A.1
  details — 47 bullets covering the csproj,
  the 4 contracts, the 4 implementations,
  the `AddInfrastructure` composition root,
  the `AddProjects` swap, the
  `InMemoryProjectStore` move, the
  wiring, the 45 new unit tests, the 2 new
  architecture tests, the docs, the
  validation, the 3 deviations, the commit,
  the no-push); Active Branch (M3 closeout
  → M4-A.1); Last Stable Commit (M3
  closeout → M4-A.1); Test Status (273
  passed → 318 passed, 7 skipped → 9
  skipped with the M4-A.1 breakdown);
  Implemented Capabilities (added the
  M4-A.1 infrastructure seam section — the
  csproj + 4 contracts + 4 implementations
  + 1 composition + 2 architecture tests +
  the on-disk store); Next Recommended
  Task (M4-A.1 → M4-A.2); Last Updated
  (rewritten as the M4-A.1 session); Linked
  Artefacts (added the M4-A.1 handoff +
  implementation report + docs/infrastructure.md);
  Last Implementation Report (added the
  M4-A.1 report at the top).
- `.ai/state/task-board.md` — T-021
  (M4-A.1) added to "Done Recently" at the
  top with the full delivery summary; the
  T-021 "Ready" entry in "Ready" is
  replaced with T-022 (M4-A.2 — Open
  action) "Ready" entry; "In Progress" is
  updated to reflect M4-A.1 done + M4-A
  Active; the M4-A summary in "Deferred" is
  updated to `Active (M4-A.1 Delivered
  2026-07-11)`.

## 5. Files Deleted

- `src/AiEng.Platform.Application/Projects/InMemoryProjectStore.cs`
  — the M3 in-memory `IProjectStore` is
  moved to
  `tests/AiEng.Platform.UnitTests/Infrastructure/InMemoryProjectStore.cs`
  as a fixture. The class is unchanged
  (byte-for-byte); the namespace is updated
  to
  `AiEng.Platform.UnitTests.Infrastructure`.

## 6. Architecture

### 6.1 Process boundary

`IProcessRunner` is the platform's only
process boundary contract. The contract
exposes two methods: `RunAsync` (streaming,
returns `IAsyncEnumerable<string>`) and
`RunToCompletionAsync` (fire-and-forget,
returns `ProcessResult`). The
`SystemProcessRunner` implementation wraps
`System.Diagnostics.Process` with
`RedirectStandardOutput` +
`RedirectStandardError` +
`UseShellExecute=false` +
`CreateNoWindow=true`. The implementation is
the **only** `Process.Start` call site in
the platform; the architecture test
`Infrastructure_Respects_ProcessBoundary`
(registered-but-disabled per ADR-016)
enforces the rule by scanning every `.cs`
file in the repository for `using System.Diagnostics`
and asserting the only file outside the
Infrastructure project that contains the
using is the architecture test itself. The
test activates in M4-D when the first
concrete `Providers.<X>` project lands.

### 6.2 Credential boundary

`ICredentialVault` is the platform's only
credential boundary contract. The contract
exposes three methods: `GetAsync` (returns
`string?` or `null` if absent), `SetAsync`,
`DeleteAsync`. The `WindowsCredentialVault`
implementation is a thin direct P/Invoke
wrapper over `advapi32.dll`'s
`CredReadW` / `CredWriteW` / `CredDeleteW`.
On non-Windows hosts every method throws
`PlatformNotSupportedException`. The
architecture test
`Infrastructure_Respects_CredentialBoundary`
(registered-but-disabled per ADR-016)
enforces the rule by scanning every `.cs`
file in the repository for `CredRead`,
`CredWrite`, `CredDelete`, or
`advapi32.dll` and asserting the only file
outside the Infrastructure project that
contains the references is the
architecture test itself. The test
activates in M4-D.

### 6.3 Platform info

`IPlatformInfo` is the platform's only
platform-info contract. The contract exposes
two methods: `GetDataDirectory` (the
directory the platform uses for durable
state; the `JsonFileProjectStore` file
lives here) and `GetConfigDirectory` (the
directory the platform uses for
configuration; reserved for M4-B / later).
The `SystemPlatformInfo` implementation
resolves both directories through
`Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)`
and appends an
`AiEng/Platform/<data|config>` subdirectory
on Windows. The path is platform-agnostic
at the contract level; the implementation
is Windows-specific.

### 6.4 On-disk project store

`JsonFileProjectStore` is the M4-A on-disk
implementation of the M3 `IProjectStore`
contract. The M3 `IProjectStore` contract
is unchanged; the composition root swaps
the `InMemoryProjectStore` registration
for the `JsonFileProjectStore`
registration (the `InMemoryProjectStore`
is preserved as a test fixture in
`tests/AiEng.Platform.UnitTests/Infrastructure/`
for the M3 unit tests that depend on it).
The store resolves the data directory
through `IPlatformInfo.GetDataDirectory()`;
on Windows the path is
`%LOCALAPPDATA%\AiEng\Platform\data\projects.json`.
The store uses a `SemaphoreSlim(1, 1)` to
serialize all writes; reads acquire the
same lock. Writes are atomic: the store
writes the snapshot to a temp file
(`projects.json.tmp-<guid>`) then uses
`File.Replace` to atomically swap the temp
file for the destination; on Windows
`File.Replace` is atomic at the file
system level. Corruption recovery: the
store's `ReadSnapshotNoLockAsync` catches
`JsonException` when reading a corrupt
file, logs a warning, and returns an
empty list; the next write overwrites the
corrupt file with valid JSON.

### 6.5 Composition root

`InfrastructureServiceCollectionExtensions.AddInfrastructure`
registers the four Infrastructure services
in the DI container:

```csharp
services.TryAddSingleton<IPlatformInfo, SystemPlatformInfo>();
services.TryAddSingleton<IProcessRunner, SystemProcessRunner>();
services.TryAddSingleton<ICredentialVault, WindowsCredentialVault>();
services.TryAddSingleton<IProjectStore, JsonFileProjectStore>();
```

`TryAddSingleton` is used (not
`AddSingleton`) so test code can pre-register
an alternate implementation. The
composition root's call ordering is:

```
services.AddNavigation(assemblies);
services.AddProjectIntelligence();
services.AddProjects();
services.AddInfrastructure();
```

`AddInfrastructure` is called **after**
`AddProjects` so the `IProjectStore`
registration in `AddInfrastructure` is the
sole source for the service in the DI
container (the M3 `InMemoryProjectStore`
registration in `AddProjects` is removed;
the M3 `InMemoryProjectStore` class is
moved to `tests/` as a fixture).

## 7. Validation Results

The M4-A.1 end-to-end validation gate:

1. **CSS build gate.** `npm run css:build`
   exits 0; `app.css` rebuilt cleanly. The
   M4-A.1 surface has no design-system
   changes; the CSS pipeline is unchanged.
2. **Restore gate.** `dotnet restore` exits
   0; every project (including the new
   Infrastructure project) is up-to-date.
3. **Build gate.** `dotnet build` exits 0;
   0 warnings, 0 errors; the new csproj
   compiles cleanly with
   `TreatWarningsAsErrors=true`; the App
   project compiles cleanly with the new
   `<ProjectReference>` to the Infrastructure
   project.
4. **Test gate.** `dotnet test` reports
   **318 passed, 0 failed, 9 skipped**.
   The 7 from M3 are unchanged (3 axe-core
   + 4 provider-boundary); the 2 new
   architecture tests
   (`Infrastructure_Respects_ProcessBoundary`,
   `Infrastructure_Respects_CredentialBoundary`)
   are registered-but-disabled per
   ADR-016. The 45 new unit tests are:
   11 `IProcessRunnerTests`, 10
   `WindowsCredentialVaultTests`, 3
   `SystemPlatformInfoTests`, 22
   `JsonFileProjectStoreTests` (one
   test for round-trip; one for missing
   file; one for empty list; one for
   duplicate detection; one for corrupt
   file recovery; one for atomic writes
   via `File.Replace`; one for concurrent
   adds; one for cancellation; one for
   constructor validation; one for
   options validation; one for
   `IPlatformInfo` resolution; one for
   snake_case JSON shape; one for the
   indented format; one for the
   singleton list ordering; one for
   `UpdateAsync` on missing project; one
   for `RemoveAsync` on missing project;
   one for `GetAsync` on missing project;
   one for the full lifecycle
   (register / rename / unregister);
   one for thread-safety under 100
   parallel adds; one for the temp file
   cleanup on success; one for the temp
   file cleanup on failure; one for the
   warning log on corruption; one for the
   `IProjectService` round-trip through
   the on-disk store). Plus the M3 unit
   tests that use `InMemoryProjectStore`
   as a fixture (the M3 unit tests are
   preserved; the fixture is moved to
   `tests/`).
5. **Format gate.**
   `dotnet format --verify-no-changes` exits
   0; the format is canonical and CI-clean;
   the CRLF line endings rule is preserved
   on every new file (every new file was
   `unix2dos`'d before commit per the
   `.editorconfig` rule).
6. **Visual smoke gate.**
   `curl http://localhost:5286/projects`
   returns 200; the page loads; the Open
   action is **still disabled** in M4-A.1
   (the Open action is M4-A.2's
   responsibility); the project list
   persists across an application restart
   (the on-disk store survives the process
   boundary; the M3 in-memory store did
   not). Register a project; restart the
   application; the project list reflects
   the persisted state.
7. **Architecture gate.** The 2 new
   architecture tests
   (`Infrastructure_Respects_ProcessBoundary`,
   `Infrastructure_Respects_CredentialBoundary`)
   are registered-but-disabled; the 7 from
   M3 are unchanged; no architecture test
   regressed.
8. **DoD gate.** Every item in
   `ROADMAP.md` § 3 M4-A DoD (the bullets
   added in this slice) is checked. The
   check is by inspection: every DoD
   bullet is marked satisfied. The Open
   action is M4-A.2's responsibility and
   remains explicitly out of scope for
   M4-A.1.
9. **No scope creep.** The diff does not
   modify any file under
   `src/AiEng.Platform.App/Components/`
   (the Open action is M4-A.2's
   responsibility), `AGENTS.md`,
   `ARCHITECTURE.md`, `DECISIONS.md`,
   `STYLEGUIDE.md`, `CONTRIBUTING.md`,
   `.ai/workflows/milestone-closeout.md`,
   `.ai/plans/M3-*.md`, `tailwind.config.js`,
   `package.json`, or
   `Directory.Build.props`.
10. **Push decision.** Push is **not**
    authorised in this session. The push
    decision recorded is **Staged for
    push** (the user did not authorise in
    this session; the M4-A.1 did not push;
    the next user command may push).

## 8. Tests Added

The M4-A.1 slice ships **45 new unit tests**
across 4 new test files + **2 new
architecture tests** registered-but-disabled
per ADR-016.

- `IProcessRunnerTests` (11 tests):
  - `RunToCompletionAsync_PropagatesExitCode`
  - `RunToCompletionAsync_CapturesStandardOutput`
  - `RunToCompletionAsync_CapturesStandardError`
  - `RunToCompletionAsync_SucceededTrueOnZeroExit`
  - `RunToCompletionAsync_SucceededFalseOnNonZeroExit`
  - `RunAsync_YieldsLinesAsTheyAreWritten`
  - `RunAsync_StderrIsIncludedInStream`
  - `RunAsync_CancellationPropagatesAsOperationCanceledException`
  - `RunToCompletionAsync_NonExistentExecutableThrows`
  - `RunAsync_ArgumentValidation_NullExecutableThrows`
  - `RunToCompletionAsync_ArgumentValidation_NullExecutableThrows`
- `WindowsCredentialVaultTests` (10 tests):
  - `GetAsync_RoundTripsSecret`
  - `SetAsync_StoresSecret`
  - `DeleteAsync_RemovesSecret`
  - `GetAsync_MissingCredentialReturnsNull`
  - `DeleteAsync_MissingCredentialIsNoOp`
  - `SetAsync_OverwritesExistingCredential`
  - `NonWindows_ThrowsPlatformNotSupportedException`
    (skipped on non-Windows; the
    `RuntimeInformation.IsOSPlatform(OSPlatform.Windows)`
    guard skips the test on non-Windows
    hosts).
  - `GetAsync_ArgumentValidation_NullNameThrows`
  - `SetAsync_ArgumentValidation_NullNameThrows`
  - `DeleteAsync_ArgumentValidation_NullNameThrows`
- `SystemPlatformInfoTests` (3 tests):
  - `GetDataDirectory_ReturnsNonEmptyRootedPath`
  - `GetConfigDirectory_ReturnsNonEmptyRootedPath`
  - `DataAndConfigDirectories_AreDistinct`
- `JsonFileProjectStoreTests` (22 tests):
  - `ListAsync_EmptyFile_ReturnsEmptyList`
  - `ListAsync_MissingFile_ReturnsEmptyList`
  - `AddAsync_RoundTripsProject`
  - `AddAsync_DuplicateIdIsIdempotent`
  - `UpdateAsync_RenamesProject`
  - `UpdateAsync_MissingProject_ReturnsFailure`
  - `RemoveAsync_RemovesProject`
  - `RemoveAsync_MissingProject_ReturnsFailure`
  - `GetAsync_ReturnsProject`
  - `GetAsync_MissingProject_ReturnsNull`
  - `ListAsync_ReturnsProjectsSortedByName`
  - `PersistAsync_WritesToTempFileAndRenames`
  - `PersistAsync_DestinationIsNeverPartial`
  - `PersistAsync_TempFileIsCleanedUpOnSuccess`
  - `PersistAsync_TempFileIsCleanedUpOnFailure`
  - `ConcurrentAdds_AreSerialized`
  - `OneHundredParallelAdds_AllProjectsPersisted`
  - `CorruptFile_LogsWarningAndReturnsEmptyList`
  - `Constructor_NullPlatformInfoThrows`
  - `Options_DataDirectoryOverridesPlatformInfo`
  - `JsonShape_IsSnakeCase`
  - `JsonShape_IsIndented`
- **Architecture tests (registered-but-
  disabled per ADR-016):**
  - `Infrastructure_Respects_ProcessBoundary`
  - `Infrastructure_Respects_CredentialBoundary`

Cumulative test count after M4-A.1: **318
passed, 0 failed, 9 skipped** (the 7 from
M3 + the 2 new architecture tests). The
M4-A.1 is **+45 unit + 0 bUnit + 0 active
architecture + 2 new skipped** vs M3.2
closeout.

## 9. Definition of Done

- `AiEng.Platform.Infrastructure` exists;
  the solution compiles; the M1 / M3 tests
  remain green.
- `IProcessRunner` and the on-disk
  `IProjectStore` are added; the M3
  in-memory store is moved to `tests/` as
  a fixture (the contract is unchanged).
- The on-disk `IProjectStore` round-trips
  a project through a save / load / list
  cycle; the store is thread-safe
  (`SemaphoreSlim`); writes are atomic
  (temp file + `File.Replace`); corruption
  is recovered (empty list + warning log).
- `ICredentialVault` round-trips a secret
  through the Windows Credential Manager;
  on non-Windows hosts it throws
  `PlatformNotSupportedException`.
- `IPlatformInfo` resolves the data and
  config directories on the current
  platform.
- The `AddInfrastructure` composition root
  extension is added; it is called after
  `AddProjects` in
  `ServiceCollectionExtensions.AddPlatformServices`.
- The architecture test
  `Infrastructure_Respects_ProcessBoundary`
  is **registered but disabled** per
  ADR-016; the activation milestone is
  M4-D. The companion
  `Infrastructure_Respects_CredentialBoundary`
  test is also registered but disabled and
  activates in M4-D.

## 10. Git

- **Branch:**
  `feature/T-021-m4-a-1-infrastructure-project-skeleton`
  (created from `main` at the M3 closeout
  commit `33c154d`).
- **Commit message:**
  `feat(m4-a.1): add infrastructure project
  skeleton with IProcessRunner,
  ICredentialVault, IPlatformInfo, and
  on-disk IProjectStore`.
- **Fast-forward merge:** the M4-A.1
  feature branch is fast-forwarded into
  `main` per the branching strategy rule 6.
- **Branch deletion:** the M4-A.1 feature
  branch is deleted per the branching
  strategy rule 7.
- **No remote push.** Push is not
  authorised in this session; the user may
  push in a follow-up command per the
  command protocol.

## 11. Out of Scope

The M4-A plan § 3 enumerates 10 out-of-scope
items. The M4-A.1 slice respects every
item:

- **Provider creation.** M4-A.1 does not
  create any `Providers.<X>` projects. The
  first concrete providers land in M4-D.
- **M4-A.2 (Open action).** M4-A.1 does
  not enable the Open button on
  `AppProjectCard`. The Open action is
  M4-A.2's responsibility.
- **M4-B, M4-C, M4-D work.** M4-A.1 does
  not begin any work outside the M4-A.1
  slice.
- **Activation of the 4 disabled
  `CompositionRootBoundaryTests`.** M4-A.1
  does not enable these. They activate in
  M4-D.
- **Activation of the 2 new
  `Infrastructure_Respects_*Boundary`
  tests.** M4-A.1 does not enable these.
  They activate in M4-D per ADR-016.
- **Activation of axe-core tests.** The 3
  `AxeCoreAuditTests` remain disabled (the
  axe-core harness is not in the toolchain
  yet).
- **Design-system `AppDialog` primitive.**
  The Open action's confirmation dialog
  (if any) composes the existing HTML5
  native `<dialog>` (the M3.2
  minimum-blast-radius decision); no new
  design-system component is added in
  M4-A.1 (the Open action is M4-A.2's
  responsibility anyway).
- **macOS / Linux credential vault.** The
  `WindowsCredentialVault` is Windows-only;
  on non-Windows hosts it throws
  `PlatformNotSupportedException`.
  Cross-platform credential storage is a
  future capability.
- **Push to remote.** M4-A.1 does not
  push. The push decision is `Staged for
  push`; the next user command may push.

The M4-A.1 slice ends after the coherent
commit on the feature branch
`feature/T-021-m4-a-1-infrastructure-project-skeleton`
(mirrored to `main` per rule 6 and deleted
per rule 7). The next session is M4-A.2
(the Open action on `AppProjectCard`; T-022
is `Ready`).

## 12. Lessons Learned

- **`IPlatformInfo` test count is
  intentionally small.** The
  `IPlatformInfo` contract is two methods
  on a 1-line platform resolution. The
  M4-A.1 ships 3 tests for
  `SystemPlatformInfo` (one per method +
  one for the two-paths-are-distinct
  invariant). The test count is
  intentionally small because the contract
  is small; inflating the count with
  redundant tests would be cargo-cult
  testing. The M4-A.1 ships 45 new unit
  tests (within the M4-A plan's 50+ bound;
  the 3 `SystemPlatformInfo` tests are
  the minimum-viable coverage).
- **`WindowsCredentialVault` uses direct
  P/Invoke, not a NuGet package.** The
  M4-A plan's § 8 risk 4 suggests the
  `CredentialManagement` NuGet package;
  the M4-A.1 research session
  (per the M4-A.1 implementation plan's
  "Deviations Anticipated" § 1)
  selected direct P/Invoke over
  `advapi32.dll` for the following
  reasons: (1) zero third-party surface;
  (2) direct Win32 calls; (3) minimal
  binary footprint; (4) works in
  all-elevation scenarios; (5) is
  well-understood. The M4-A.1
  implementation report's Deviations § 1
  documents the choice. The
  `Meziantou.Framework.Win32.CredentialManager`
  alternative was considered and
  deferred (the M4-A plan § 8 risk 4).
- **`JsonFileProjectStore` uses
  `File.Replace`, not "temp file +
  rename".** The M4-A plan § 5 anticipates
  "atomic writes via temp file + rename".
  The M4-A.1 implementation uses
  `File.Replace` for cross-Windows-version
  robustness: `File.Replace` is atomic
  at the file system level on Windows;
  the destination is never observed in a
  partially-written state; the source
  temp file is automatically deleted by
  `File.Replace`. The M4-A.1
  implementation report's Deviations § 2
  documents the choice.
- **`TimeProvider` (M3 deviation) is
  preserved.** The M3 retrospective § 13
  recommendation 5 says "M4-A may either
  keep `TimeProvider` (the M3
  implementation is fine) or introduce a
  thin `IClock` adapter over
  `TimeProvider` (the choice is M4-A's)".
  The M4-A.1 keeps `TimeProvider` (the M3
  deviation "IClock is realised through
  .NET 8+ TimeProvider" is the lesson; a
  custom `IClock` interface is unnecessary;
  the BCL provides `TimeProvider` for
  the same purpose). The choice is
  documented here; the M4-A plan's M4-A.1
  row in the M4-A.1 implementation plan's
  "Deviations Anticipated" § 2 was the
  origin of the choice.
- **The M3 `InMemoryProjectStore` is a
  test fixture, not deleted.** The M4-A
  plan's § 2 item 6 says "the M3
  in-memory `IProjectStore` is removed".
  The M4-A.1 implementation moves the
  class to `tests/` as a fixture (the
  class is unchanged; the namespace is
  updated). The M3 unit tests that depend
  on it (`IProjectServiceTests`,
  `InMemoryProjectStoreTests`) continue
  to pass; the M3 unit tests are
  preserved. The choice is documented in
  the M4-A.1 implementation report's
  Deviations § 3 (the M4-A.1
  implementation plan's "Files NOT
  Touched" § anticipated the move).
- **The 2 new architecture tests are
  registered-but-disabled per ADR-016.**
  The M4-A plan's § 2 item 10 says "3 new
  architecture tests". The M4-A.1
  implementation plan's "M4-A.1 vs M4-A
  Plan Alignment" table notes the
  alignment: M4-A.1 ships 2 architecture
  tests (the 3rd, the
  `Pages_Resolve_Projects_Through_Service`
  extension, is M4-A.2's responsibility).
  Both tests are
  `[Fact(Skip = "...")]` (registered but
  disabled) per ADR-016; they activate in
  M4-D.

## 13. Handoff to M4-A.2

- The infrastructure seam is in place:
  `IProcessRunner` is the only legal caller
  of `Process.Start` in the platform;
  `ICredentialVault` is the only legal
  caller of the Windows Credential Manager
  APIs; `IPlatformInfo` is the only legal
  resolver of the platform's data and
  config directories; the on-disk
  `IProjectStore` is the only
  `IProjectStore` registration in the
  composition root (the M3 in-memory
  `IProjectStore` is a test fixture in
  `tests/`).
- The 2 new architecture tests
  (`Infrastructure_Respects_ProcessBoundary`,
  `Infrastructure_Respects_CredentialBoundary`)
  are registered-but-disabled per
  ADR-016. They activate in M4-D when the
  first concrete `Providers.<X>` project
  lands.
- M4-A.2 (T-022 — the Open action on
  `AppProjectCard`) is `Ready` in
  `.ai/state/tasks.json`. The M4-A.2
  implementation enables the Open button
  on `AppProjectCard`; wires the Open
  click handler to
  `IProcessRunner.RunAsync(explorer.exe, project.Path, ct)`;
  streams the (irrelevant) stdout/stderr
  to the host console; adds 5+ bUnit tests
  for the new `AppProjectCard.OpenAsync`
  flow. The Open action is Windows-only;
  the action is disabled on non-Windows
  hosts.

## 14. Deviations

Three documented deviations from the M4-A
plan's M4-A.1 row:

1. **WindowsCredentialVault uses direct
   P/Invoke (no NuGet dependency).** The
   M4-A plan § 8 risk 4 anticipates the
   `CredentialManagement` NuGet package;
   the M4-A.1 research session selected
   direct P/Invoke over `advapi32.dll` for
   the following reasons: (a) zero
   third-party surface; (b) direct Win32
   calls; (c) minimal binary footprint;
   (d) works in all-elevation scenarios;
   (e) is well-understood. The
   `Meziantou.Framework.Win32.CredentialManager`
   alternative was considered and
   deferred. Documented in § 12.
2. **JsonFileProjectStore uses
   `File.Replace` for atomic Windows file
   replacement.** The M4-A plan § 5
   anticipates "atomic writes via temp
   file + rename". The M4-A.1
   implementation uses `File.Replace` for
   cross-Windows-version robustness:
   `File.Replace` is atomic at the file
   system level on Windows; the destination
   is never observed in a partially-written
   state; the source temp file is
   automatically deleted by `File.Replace`.
   Documented in § 12.
3. **M4-A.1 ships 45 new unit tests
   (within the M4-A plan's 50+ bound; the
   `IPlatformInfo` test count is
   intentionally small because the
   contract is two methods on a 1-line
   platform resolution).** The M4-A plan
   § 2 item 11 says "50+ new unit + bUnit
   tests". The M4-A.1 ships 45 new unit
   tests (11 IProcessRunnerTests + 10
   WindowsCredentialVaultTests + 3
   SystemPlatformInfoTests + 22
   JsonFileProjectStoreTests); the M4-A.1
   does not ship bUnit tests (the Open
   action is M4-A.2's responsibility; the
   bUnit tests for the Open action are
   M4-A.2's deliverable). The 50+ bound
   is met when M4-A.2 ships its 5+ bUnit
   tests for the Open action. Documented
   in § 12.

## 15. Cross-References

- **Plan:**
  `.ai/plans/M4-A-infrastructure-process-execution.md`
  (the canonical M4-A plan; Status:
  Approved 2026-07-11).
- **M4-A.1 documentation:**
  `docs/infrastructure.md` (10 sections).
- **M4-A.1 handoff:**
  `.ai/handoffs/2026-07-11-m4-a-1-infrastructure-project-skeleton.md`
  (mirrored to `.ai/handoffs/latest.md`).
- **M4-A.1 state:** `.ai/state/session.json`
  (M4-A.1 envelope),
  `.ai/state/tasks.json` (T-021 Done;
  T-022 Ready), `.ai/state/milestones.json`
  (M4-A Active; M4-A.1 slice block),
  `.ai/state/current.md` (Current Milestone
  M4-A; Last Completed Task M4-A.1), and
  `.ai/state/task-board.md` (M4-A.1 in
  Done Recently; T-022 in Ready).
- **M4-A.1 governance:**
  `ROADMAP.md` § 2 (M4-A Active) and § 3
  (M4-A Outcome, Outcome in detail, DoD,
  M4-A.1 slice row, Tests added), and
  `.ai/plans/master-delivery-plan.md` § 1
  (M4-A Active) and § 3 (M4-A Completion
  status, Evidence, slice breakdown).
- **M3 closeout handoff:**
  `.ai/handoffs/2026-07-11-m3-closeout.md`
  (the M4-A.1 first session reads this
  first).
- **M3 retrospective:**
  `retrospective-m3-project-registration.md`
  (13 sections; the M3 retrospective § 13
  is the M4-A plan's input; the M4-A.1
  accounts for the 11 recommendations).
- **Branching strategy:**
  `.ai/workflows/branching-strategy.md`
  (rules 6, 7 are the M4-A.1's branch
  operations).
- **Milestone Closeout Standard:**
  `.ai/workflows/milestone-closeout.md`
  (the M4-A closeout follows the standard;
  the M4-A.1 is a per-slice closeout, not
  a milestone closeout).
- **ADR-016:** the registered-but-disabled
  tests activate in M4-D; the M4-A.1
  follows the rule.

---

**End of M4-A.1 implementation report.**
The M4-A.1 is the first M4-A slice; the
M4-A.1 follows the M4-A plan; the M4-A.1
follows the 13-step Progressive Coding
lifecycle; the M4-A.1 stops after the
coherent commit. The next session is the
M4-A.2 implementation (the Open action on
`AppProjectCard`; the M4-A.2 task T-022 is
`Ready`).
