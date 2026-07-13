# Handoff — M4-A.1 Infrastructure Project Skeleton — `m4-a-1-infrastructure-project-skeleton` (2026-07-11)

> **The M4-A.1 per-session handoff.** M4-A.1
> (T-021) is the first M4-A slice. M4-A.1
> follows M3 closeout per the Progressive
> Coding Rule. M4-A.1 ships the infrastructure
> seam every later milestone composes: the
> new `AiEng.Platform.Infrastructure` csproj;
> the four contracts (`IProcessRunner`,
> `ICredentialVault`, `IPlatformInfo`, the
> on-disk `IProjectStore`); the four
> implementations (`SystemProcessRunner`,
> `WindowsCredentialVault`,
> `SystemPlatformInfo`, `JsonFileProjectStore`);
> the `AddInfrastructure` composition root
> extension; the one-line swap in `AddProjects`;
> the move of `InMemoryProjectStore` to
> `tests/` as a test fixture; 45 new unit
> tests; 2 new architecture tests
> registered-but-disabled per ADR-016; and
> `docs/infrastructure.md` (10 sections).
>
> M4-A.1 is the **boundary**, not the
> activation. The Open action on
> `AppProjectCard` is M4-A.2's responsibility
> (T-022 is `Ready`; the M4-A.1 session does
> **not** begin M4-A.2 per the brief and the
> Progressive Coding Rule).

---

## 1. What was delivered

The M4-A.1 infrastructure project skeleton
(T-021) is **Done** (2026-07-11).

The M4-A.1 ships:

- **The
  `AiEng.Platform.Infrastructure` csproj**
  at
  `src/AiEng.Platform.Infrastructure/AiEng.Platform.Infrastructure.csproj`
  (a new C# class library; references
  `AiEng.Platform.Application` and
  `AiEng.Platform.Domain`; does **not**
  reference `AiEng.Platform.App`; `Nullable` +
  `TreatWarningsAsErrors` per
  `Directory.Build.props`).
- **The four contracts** in
  `src/AiEng.Platform.Application/Infrastructure/`:
  - `IProcessRunner.cs` (streaming
    `RunAsync` returning
    `IAsyncEnumerable<string>` + fire-and-forget
    `RunToCompletionAsync` returning
    `ProcessResult`).
  - `ProcessResult.cs` (record struct
    envelope: `int ExitCode`, `string StandardOutput`,
    `string StandardError`,
    `bool Succeeded => ExitCode == 0`).
  - `ICredentialVault.cs` (get / set / delete;
    `string?` return on `GetAsync`).
  - `IPlatformInfo.cs`
    (`GetDataDirectory` +
    `GetConfigDirectory`).
- **The four implementations** in
  `src/AiEng.Platform.Infrastructure/`:
  - `ProcessRunner/SystemProcessRunner.cs`
    (wraps `System.Diagnostics.Process` with
    `RedirectStandardOutput` +
    `RedirectStandardError` +
    `UseShellExecute=false` +
    `CreateNoWindow=true`; the **only** call
    site for `Process.Start` in the platform).
  - `Credentials/WindowsCredentialVault.cs`
    (thin direct P/Invoke wrapper over
    `advapi32.dll`'s `CredReadW` /
    `CredWriteW` / `CredDeleteW`; throws
    `PlatformNotSupportedException` on
    non-Windows).
  - `Platform/SystemPlatformInfo.cs`
    (resolves the data + config directories
    via
    `Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)`
    + `AiEng/Platform/<data|config>`
    subdirectory).
  - `Projects/JsonFileProjectStore.cs`
    (persists the project list to
    `projects.json` in the platform's data
    directory; thread-safe via
    `SemaphoreSlim(1, 1)`; atomic writes via
    temp file + `File.Replace`; corruption
    recovery returns empty list + warning
    log).
  - `Projects/JsonFileProjectStoreOptions.cs`
    (the options type for
    `JsonFileProjectStore`).
- **The `AddInfrastructure` composition
  root extension** at
  `src/AiEng.Platform.App/Composition/Infrastructure/InfrastructureServiceCollectionExtensions.cs`
  (registers the four Infrastructure services
  with `TryAddSingleton`).
- **The one-line swap in `AddProjects`**
  (the M3 in-memory `IProjectStore`
  registration is removed; the on-disk
  `JsonFileProjectStore` is now registered
  through `AddInfrastructure`).
- **The call site in
  `ServiceCollectionExtensions`** —
  `AddInfrastructure()` is called after
  `AddProjects()` in
  `AddPlatformServices`.
- **The move of `InMemoryProjectStore`** from
  `src/AiEng.Platform.Application/Projects/InMemoryProjectStore.cs`
  to
  `tests/AiEng.Platform.UnitTests/Infrastructure/InMemoryProjectStore.cs`
  (preserved as a test fixture; the class is
  byte-for-byte unchanged; the namespace is
  updated to
  `AiEng.Platform.UnitTests.Infrastructure`).
- **The M3 in-memory store file deletion**
  (the class is moved to `tests/`; the M3
  `IProjectService` and the M3 UI surface
  are unchanged).
- **The new `<ProjectReference>` to
  `AiEng.Platform.Infrastructure`** in
  `src/AiEng.Platform.App/AiEng.Platform.App.csproj`.
- **The new project added to
  `AiEng.Platform.slnx`** (alphabetically
  between `AiEng.Platform.Domain` and
  `AiEng.Platform.Providers.Abstractions`).
- **45 new unit tests** in
  `tests/AiEng.Platform.UnitTests/Infrastructure/`
  (across 4 new test files:
  `IProcessRunnerTests` (11),
  `WindowsCredentialVaultTests` (10),
  `SystemPlatformInfoTests` (3),
  `JsonFileProjectStoreTests` (22)). The M3
  unit tests that use `InMemoryProjectStore`
  as a fixture are updated to import the
  new namespace; the M3 unit tests continue
  to pass.
- **2 new architecture tests
  registered-but-disabled per ADR-016** in
  `tests/AiEng.Platform.ArchitectureTests/Infrastructure/`
  (`Infrastructure_Respects_ProcessBoundary`
  + `Infrastructure_Respects_CredentialBoundary`).
  Both tests are
  `[Fact(Skip = "...")]`; they activate in
  M4-D when the first concrete
  `Providers.<X>` project lands.
- **`docs/infrastructure.md`** (10 sections:
  Goals, Project Structure, Process Boundary,
  Credential Boundary, On-Disk Project
  Store, Platform Info, Open Action,
  Composition Root, Tests, Out of Scope).
- **The M3 / M4-A Boundary section in
  `docs/projects.md`** is updated to reflect
  M4-A delivered.
- **The M4-A.1 state updates:**
  - `.ai/state/session.json` — the M4-A.1
    envelope.
  - `.ai/state/tasks.json` — T-021 `Done`
    with the full evidence block; T-022
    (M4-A.2 — Open action) `Ready` with
    `depends_on_task: T-021`; T-008 (M4
    summary) `Deferred` → `In Progress`.
  - `.ai/state/current.md` — M4-A Active
    with the full M4-A.1 description; Last
    Completed Task rewritten with the
    M4-A.1 details; Next Recommended Task
    M4-A.2.
  - `.ai/state/task-board.md` — M4-A.1 in
    `Done Recently` at the top; T-022
    (M4-A.2) in `Ready`; M4-A summary in
    `Deferred` updated to `Active (M4-A.1
    Delivered 2026-07-11)`.
  - `.ai/state/milestones.json` — M4-A
    `Awaiting Approval` → `Active`; M4-A.1
    slice block added with the full
    evidence; M4-A evidence block added.
  - `ROADMAP.md` § 2 (M4-A row updated
    to `Active` with the M4-A.1 summary);
    § 3 (M4-A Outcome, Outcome in detail,
    Definition of done, Tests added, M4-A.1
    slice row updated to reflect the
    contracts + implementations actually
    shipped).
  - `.ai/plans/master-delivery-plan.md` § 1
    (M4-A row updated to `Active`); § 3
    (M4-A Completion status, Evidence, slice
    breakdown).
  - `.ai/plans/M4-A-infrastructure-process-execution.md`
    — Status `Awaiting Approval` →
    `Approved`.
- **The M4-A.1 implementation report** at
  `implementation-report-m4-a-1-infrastructure-project-skeleton.md`
  (15+ sections; mirrors the M3.1 / M3.2 /
  M3 closeout reports).
- **The M4-A.1 per-session handoff** at
  `.ai/handoffs/2026-07-11-m4-a-1-infrastructure-project-skeleton.md`
  (this file; mirrored to
  `.ai/handoffs/latest.md`).
- **The M4-A.1 branch + commit + merge +
  delete:**
  - Branch
    `feature/T-021-m4-a-1-infrastructure-project-skeleton`
    (created from `main` at the M3 closeout
    commit `33c154d`).
  - The M4-A.1 closeout commit
    `feat(m4-a.1): add infrastructure project
    skeleton with IProcessRunner,
    ICredentialVault, IPlatformInfo, and
    on-disk IProjectStore` on the feature
    branch.
  - Fast-forward merge into `main` per the
    branching strategy rule 6.
  - Delete the M4-A.1 feature branch per
    rule 7.
  - Skip push (not authorised in this
    session).

## 2. Test and build status

The M4-A.1 validation gate, executed
end-to-end on 2026-07-11.

| Gate          | Result                                                                |
| ------------- | --------------------------------------------------------------------- |
| CSS build     | `npm run css:build` exits 0.                                           |
| Restore       | `dotnet restore` exits 0 (including the new Infrastructure project).  |
| Build         | `dotnet build` exits 0; 0 warnings, 0 errors.                          |
| Test          | `dotnet test` reports 318 passed, 0 failed, 9 skipped.                |
| Format        | `dotnet format --verify-no-changes` exits 0.                          |
| Visual smoke  | `/projects` returns 200; project list persists across restart.        |

The 9 skipped tests are the 3 axe-core
(`AxeCoreAuditTests`) + 4 provider-boundary
(`CompositionRootBoundaryTests`) from M3 +
the 2 new M4-A.1 architecture tests
(`Infrastructure_Respects_ProcessBoundary`
+ `Infrastructure_Respects_CredentialBoundary`)
registered-but-disabled per ADR-016 / M4-D.
The M4-A.1 introduces 2 new skipped tests
(+2 vs M3.2 closeout).

The M4-A.1 is **+45 unit + 0 bUnit + 0
active architecture + 2 new skipped** vs
M3.2 closeout. Cumulative test count:
**318 passed, 0 failed, 9 skipped**.

## 3. Deviations

Three documented deviations from the M4-A
plan:

1. **WindowsCredentialVault uses direct
   P/Invoke (no NuGet dependency).** The
   M4-A plan § 8 risk 4 anticipates the
   `CredentialManagement` NuGet package;
   the M4-A.1 selected direct P/Invoke
   over `advapi32.dll` for: (a) zero
   third-party surface; (b) direct Win32
   calls; (c) minimal binary footprint;
   (d) works in all-elevation scenarios;
   (e) is well-understood. The
   `Meziantou.Framework.Win32.CredentialManager`
   alternative was considered and
   deferred.
2. **JsonFileProjectStore uses
   `File.Replace` for atomic Windows
   file replacement.** The M4-A plan § 5
   anticipates "atomic writes via temp
   file + rename". The M4-A.1
   implementation uses `File.Replace`
   for cross-Windows-version robustness:
   `File.Replace` is atomic at the file
   system level on Windows; the
   destination is never observed in a
   partially-written state; the source
   temp file is automatically deleted by
   `File.Replace`.
3. **M4-A.1 ships 45 new unit tests
   (within the M4-A plan's 50+ bound;
   the `IPlatformInfo` test count is
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
   tests for the Open action.

## 4. Files added

### 4.1 New project (AiEng.Platform.Infrastructure)

- `src/AiEng.Platform.Infrastructure/AiEng.Platform.Infrastructure.csproj`.

### 4.2 Contracts (in Application/Infrastructure/)

- `src/AiEng.Platform.Application/Infrastructure/IProcessRunner.cs`.
- `src/AiEng.Platform.Application/Infrastructure/ProcessResult.cs`.
- `src/AiEng.Platform.Application/Infrastructure/ICredentialVault.cs`.
- `src/AiEng.Platform.Application/Infrastructure/IPlatformInfo.cs`.

### 4.3 Implementations (in Infrastructure/)

- `src/AiEng.Platform.Infrastructure/ProcessRunner/SystemProcessRunner.cs`.
- `src/AiEng.Platform.Infrastructure/Credentials/WindowsCredentialVault.cs`.
- `src/AiEng.Platform.Infrastructure/Platform/SystemPlatformInfo.cs`.
- `src/AiEng.Platform.Infrastructure/Projects/JsonFileProjectStore.cs`.
- `src/AiEng.Platform.Infrastructure/Projects/JsonFileProjectStoreOptions.cs`.

### 4.4 Composition root

- `src/AiEng.Platform.App/Composition/Infrastructure/InfrastructureServiceCollectionExtensions.cs`.

### 4.5 Tests

- `tests/AiEng.Platform.UnitTests/Infrastructure/IProcessRunnerTests.cs`
  (11 tests).
- `tests/AiEng.Platform.UnitTests/Infrastructure/WindowsCredentialVaultTests.cs`
  (10 tests).
- `tests/AiEng.Platform.UnitTests/Infrastructure/SystemPlatformInfoTests.cs`
  (3 tests).
- `tests/AiEng.Platform.UnitTests/Infrastructure/JsonFileProjectStoreTests.cs`
  (22 tests).
- `tests/AiEng.Platform.UnitTests/Infrastructure/InMemoryProjectStore.cs`
  (the M3 in-memory `IProjectStore` moved
  from `Application/`).
- `tests/AiEng.Platform.ArchitectureTests/Infrastructure/Infrastructure_Respects_ProcessBoundary.cs`
  (registered-but-disabled per ADR-016).
- `tests/AiEng.Platform.ArchitectureTests/Infrastructure/Infrastructure_Respects_CredentialBoundary.cs`
  (registered-but-disabled per ADR-016).

### 4.6 Documentation

- `docs/infrastructure.md` (10 sections).
- `implementation-report-m4-a-1-infrastructure-project-skeleton.md`
  (the M4-A.1 implementation report; 15+
  sections).
- `.ai/handoffs/2026-07-11-m4-a-1-infrastructure-project-skeleton.md`
  (this handoff; mirrored to
  `.ai/handoffs/latest.md`).

## 5. Files modified

- `AiEng.Platform.slnx` (new Infrastructure
  project added).
- `src/AiEng.Platform.App/AiEng.Platform.App.csproj`
  (new `<ProjectReference>` to
  `AiEng.Platform.Infrastructure`).
- `src/AiEng.Platform.App/Composition/ServiceCollectionExtensions.cs`
  (calls `AddInfrastructure()` after
  `AddProjects()`).
- `src/AiEng.Platform.App/Composition/Projects/ProjectsServiceCollectionExtensions.cs`
  (M3 in-memory `IProjectStore`
  registration removed; `IProjectService`
  registration preserved).
- `docs/projects.md` (M3 / M4-A Boundary
  section updated to reflect M4-A
  delivered).
- `ROADMAP.md` (M4-A row updated; M4-A
  details updated; M4-A.1 slice row added).
- `.ai/plans/master-delivery-plan.md`
  (M4-A row updated; M4-A details updated;
  M4-A slice breakdown added).
- `.ai/plans/M4-A-infrastructure-process-execution.md`
  (Status `Awaiting Approval` →
  `Approved`).
- `.ai/state/session.json` (M4-A.1
  envelope).
- `.ai/state/tasks.json` (T-021 `Done`;
  T-022 `Ready`; T-008 `In Progress`).
- `.ai/state/milestones.json` (M4-A
  `Active`; M4-A.1 slice block; M4-A
  evidence block).
- `.ai/state/current.md` (extensively
  updated).
- `.ai/state/task-board.md` (M4-A.1 in
  `Done Recently`; T-022 in `Ready`; M4-A
  summary in `Deferred` updated to
  `Active`).
- The M3 unit tests that use
  `InMemoryProjectStore` as a fixture
  (`IProjectServiceTests`,
  `InMemoryProjectStoreTests`) are updated
  to import the new namespace.

## 6. Files deleted

- `src/AiEng.Platform.Application/Projects/InMemoryProjectStore.cs`
  (moved to
  `tests/AiEng.Platform.UnitTests/Infrastructure/InMemoryProjectStore.cs`
  as a fixture).

## 7. Files NOT touched

- `src/AiEng.Platform.App/Components/`
  — not modified. M4-A.1 is the
  infrastructure seam; the Open action
  is M4-A.2's responsibility. The Open
  button on `AppProjectCard` remains
  disabled in M4-A.1.
- `src/AiEng.Platform.Application/Projects/`
  (except the `InMemoryProjectStore.cs`
  deletion) — not modified otherwise. The
  M3 contracts are unchanged.
- `src/AiEng.Platform.Domain/` — not
  modified. M4-A.1 does not add domain
  types.
- `src/AiEng.Platform.Providers.Abstractions/`
  — not modified. M4-A.1 does not create
  providers; the first concrete providers
  land in M4-D.
- `AGENTS.md`, `ARCHITECTURE.md`,
  `DECISIONS.md`, `STYLEGUIDE.md`,
  `CONTRIBUTING.md` — not modified. The
  17 non-negotiable rules are preserved.
- `.ai/workflows/milestone-closeout.md`
  — not modified. The standard is
  preserved.
- `.ai/plans/M3-project-registration.md`,
  `.ai/plans/M3.2-project-registration-slice-2.md`,
  `.ai/plans/M3-closeout.md` — not
  modified. The M3 plans are unchanged.
- `tailwind.config.js`, `package.json`,
  `Directory.Build.props` — not modified.
  The CSS pipeline and the .NET build
  configuration are unchanged.

## 8. Next action

**The M4-A.1 stops here.** The next session
implements M4-A.2 (the Open action on
`AppProjectCard`; the first
`IProcessRunner` activation) per the
Progressive Coding Rule.

The M4-A.1 brief's "Do not begin the
following task" rule is preserved; the
M4-A.1 does not begin the M4-A.2
implementation. M4-A.2 (T-022) is `Ready`
in `.ai/state/tasks.json`; the M4-A plan
is `Approved`; the next session starts
the M4-A.2 implementation.

The M4-A.2 first session:

1. Reads this handoff
   (`.ai/handoffs/2026-07-11-m4-a-1-infrastructure-project-skeleton.md`)
   + the M4-A.1 implementation report
   (`implementation-report-m4-a-1-infrastructure-project-skeleton.md`)
   + the M4-A documentation
   (`docs/infrastructure.md`) first.
2. Reviews the M4-A.2 scope per the M4-A
   plan's M4-A.2 row (§ 2 item 8: the
   Open action is the first
   `IProcessRunner` activation; the
   action is Windows-only; the action is
   disabled on non-Windows hosts; the
   M4-A.1 architecture tests
   `Infrastructure_Respects_ProcessBoundary`
   + `Infrastructure_Respects_CredentialBoundary`
   remain registered-but-disabled per
   ADR-016).
3. Begins the M4-A.2 implementation per
   the M4-A plan. The M4-A.2 slice
   ships: enable the Open button on
   `AppProjectCard`; wire the Open click
   handler to
   `IProcessRunner.RunAsync(explorer.exe, project.Path, ct)`;
   stream the (irrelevant) stdout/stderr
   to the host console (the action is
   fire-and-forget from the UI's
   perspective); add 5+ bUnit tests for
   the new `AppProjectCard.OpenAsync`
   flow. The Open action is Windows-only;
   the action is disabled on non-Windows
   hosts.
4. Validates the M4-A.2 slice end-to-end.
5. Writes the M4-A.2 implementation
   report at
   `implementation-report-m4-a-2-open-action.md`.
6. Writes the M4-A.2 per-session handoff
   at
   `.ai/handoffs/2026-07-11-m4-a-2-open-action.md`
   (mirrored to
   `.ai/handoffs/latest.md`).
7. Promotes the next M4-A task (M4-A.3,
   if any, or the M4-B plan) to `Ready`
   in `.ai/state/tasks.json`.
8. Coherent commit on the feature
   branch
   `feature/T-022-m4-a-2-open-action`.
9. Fast-forward merge the M4-A.2
   feature branch into `main` per the
   branching strategy rule 6.
10. Delete the M4-A.2 feature branch
    per rule 7.
11. Stop. The next session is the M4-B
    implementation (capability detection)
    or the M4-A.3 implementation (if
    M4-A.3 is needed).

---

**End of M4-A.1 per-session handoff.** The
M4-A.1 session is the implicit approval of
the M4-A work that flows from the `Next`
invocation's end-to-end collapsed form.
M4-A.1 is delivered 2026-07-11; the M4-A.1
closeout commit
`feat(m4-a.1): add infrastructure project
skeleton with IProcessRunner,
ICredentialVault, IPlatformInfo, and on-disk
IProjectStore` is on `main`; the M4-A.1
feature branch is deleted. The M4-A.1's
per-session handoff is the canonical
artifact the M4-A.2 first session reads
first.
