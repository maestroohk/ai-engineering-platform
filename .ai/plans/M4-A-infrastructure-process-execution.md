# M4-A — Infrastructure / Process Execution

> **The M4-A plan.** M4-A introduces the
> infrastructure seam that every later milestone
> composes: `AiEng.Platform.Infrastructure` (a
> new csproj), `IProcessRunner` (the process
> boundary; every `Process.Start` call goes
> through it), `ICredentialVault` (the
> credential boundary; Windows Credential
> Manager on Windows hosts, abstractable for
> non-Windows hosts), and the on-disk
> `IProjectStore` (which replaces the M3
> in-memory store behind the same contract).
> M4-A is the **first milestone that introduces
> a process boundary**; the boundary is
> designed to be testable and to keep the UI
> (Blazor Server) free of process-boundary
> types.
>
> **Status:** Approved (2026-07-11; the
> approval is implicit in the `Next`
> invocation per `.ai/commands.md` § 4
> and the Progressive Coding Rule § 7.1;
> the M4-A.1 implementation follows the
> plan). M4-A.1 Delivered 2026-07-11
> (T-021; the infrastructure seam
> ships); M4-A.2 — the Open action on
> `AppProjectCard` (T-022) — is the next
> `Ready` task.
>
> **Branch:** (the M4-A.1 branch is created from
> `main` at the M3 closeout commit when M4-A.1
> starts; the branch is named
> `feature/T-021-m4-a-1-infrastructure-project-skeleton`
> per the branching strategy rule 4).

---

## 1. Why This Milestone Exists

M3 introduced the smallest piece of state the
platform needs: a registered project. The M3
in-memory store is the smoke test for the
`IProjectStore` contract; the M3 surface is
reachable through the M2 shell, but the Open
action on the project card is disabled (the
M3.2 closeout's Limitation § 2). To enable
the Open action — and to introduce every later
process-boundary type the platform needs — the
platform needs an **infrastructure seam**:
a place where process calls, credential
storage, and on-disk persistence live, behind
contracts the application layer exposes.

M4-A ships:
- `AiEng.Platform.Infrastructure` — a new
  csproj that holds the process-boundary
  types. The project references
  `AiEng.Platform.Application` and
  `AiEng.Platform.Domain`; the Infrastructure
  types are not exposed to the UI directly
  (per ADR-016's single-seam rule; the four
  registered-but-disabled composition-root
  tests will activate in M4-D).
- `IProcessRunner` — the process boundary.
  Every `Process.Start` call in the
  application code goes through
  `IProcessRunner`. The interface supports
  both streaming (`RunAsync` yields output
  lines as they are emitted) and
  fire-and-forget (`RunToCompletionAsync`
  returns a result envelope). The
  architecture test
  `No_DirectProcessStart_OutsideInfrastructure`
  activates in M4-D; the test is registered
  in M4-A as a placeholder.
- `ICredentialVault` — the credential
  boundary. The interface supports get / set /
  delete on a named credential; the M3 surface
  does not use credentials; M4-A introduces
  the boundary so M4-B / M4-D can use it.
- The on-disk `IProjectStore` — replaces the
  M3 in-memory store behind the same
  `IProjectStore` contract. The store
  persists projects to a JSON file in the
  user's platform data directory (the
  XDG_DATA_HOME / %APPDATA% / ~/Library
  convention; .NET's `Environment.SpecialFolder.LocalApplicationData`).
  The on-disk store is the **first durable
  surface** in this repository.
- `IClock` — the clock abstraction. M4-A may
  either keep the .NET 8+ `TimeProvider`
  abstraction the M3 surface uses or
  introduce a thin `IClock` adapter over
  `TimeProvider`. The choice is M4-A's; the
  M3 closeout does not pre-judge. The M4-A
  plan documents the choice in the M4-A
  closeout's Lessons Learned.
- The Open action on `AppProjectCard` —
  enabled and wired to the new on-disk
  `IProjectStore` + `IProcessRunner`. The
  M3.2 closeout's Limitation § 2 is
  resolved.

M4-A does not implement providers, worktrees,
launches, runs, reviews, or quality gates.
M4-B / M4-C / M4-D land the first concrete
providers; M5 lands the worktree; M6 lands the
launch; M7 lands the review; M8 lands the
orchestration. M4-A is the **smallest
milestone that introduces a process boundary**.

---

## 2. In Scope

1. **`AiEng.Platform.Infrastructure` csproj.**
   A new C# class library project at
   `src/AiEng.Platform.Infrastructure/`. The
   `csproj` references
   `AiEng.Platform.Application` and
   `AiEng.Platform.Domain`. The project ships
   no public types in M4-A; the project's
   `internal` types are the implementations
   of `IProcessRunner`, `ICredentialVault`,
   and the on-disk `IProjectStore`. The
   project enables `Nullable` and
   `TreatWarningsAsErrors` per
   `Directory.Build.props`.

2. **`IProcessRunner` contract.** The interface
   lives in
   `src/AiEng.Platform.Application/Infrastructure/IProcessRunner.cs`.
   Methods:
   - `IAsyncEnumerable<string> RunAsync(
     string executable, string arguments,
     CancellationToken)` — streaming output.
   - `Task<ProcessResult> RunToCompletionAsync(
     string executable, string arguments,
     CancellationToken)` — fire-and-forget.
   The `ProcessResult` envelope returns
   `int ExitCode`, `string StdOut`, `string
   StdErr`. The envelope is a value type
   (record struct).

3. **`IProcessRunner` implementation.** The
   implementation lives in
   `src/AiEng.Platform.Infrastructure/ProcessRunner/SystemProcessRunner.cs`.
   The implementation wraps
   `System.Diagnostics.Process`; the
   `Start()` call is the **only** direct
   `Process.Start` call in the platform
   (the architecture test enforces this).

4. **`ICredentialVault` contract.** The interface
   lives in
   `src/AiEng.Platform.Application/Infrastructure/ICredentialVault.cs`.
   Methods:
   - `Task<string?> GetAsync(string name,
     CancellationToken)`.
   - `Task SetAsync(string name, string
     value, CancellationToken)`.
   - `Task DeleteAsync(string name,
     CancellationToken)`.

5. **`ICredentialVault` implementation.** The
   implementation lives in
   `src/AiEng.Platform.Infrastructure/Credentials/WindowsCredentialVault.cs`.
   The implementation wraps the Windows
   Credential Manager via the
   `CredentialManagement` NuGet package
   (the M4-A first session selects the package
   based on the M4-A first session's research).
   On non-Windows hosts, the implementation
   throws `PlatformNotSupportedException`
   (the M4-A first session may add a
   `IPlatformInfo` seam to support macOS /
   Linux in a later slice; the M4-A scope is
   Windows).

6. **The on-disk `IProjectStore`.** The
   implementation lives in
   `src/AiEng.Platform.Infrastructure/Projects/JsonFileProjectStore.cs`.
   The implementation persists projects to a
   JSON file in the user's platform data
   directory. The file path is resolved
   through `IPlatformInfo` (the seam M4-A
   introduces; see § 4 Files to Add).
   The implementation is thread-safe (a
   `SemaphoreSlim` serialises writes; reads
   are lock-free).

7. **`IPlatformInfo` contract.** The interface
   lives in
   `src/AiEng.Platform.Application/Infrastructure/IPlatformInfo.cs`.
   Methods:
   - `string GetDataDirectory()` — returns
     the platform's data directory.
   - `string GetConfigDirectory()` —
     returns the platform's config directory.
   The interface is small; the implementation
   lives in
   `src/AiEng.Platform.Infrastructure/Platform/SystemPlatformInfo.cs`.

8. **The Open action on `AppProjectCard`.** The
   Open button is enabled and wired to the
   new on-disk `IProjectStore` +
   `IProcessRunner`. The click handler calls
   `IProcessRunner.RunToCompletionAsync(
   "explorer.exe", $"\"{project.Path}\"")`
   on Windows (the M4-A first session may
   introduce a `IOpenProjectAction` seam if
   the click handler grows; the M4-A scope
   is the Windows implementation).
   The bUnit test for the Open action is
   updated: the button is enabled, the click
   handler resolves the project's path
   through the seam, and the
   `IProcessRunner` is called with the
   expected executable + arguments.

9. **The M4-A `AddInfrastructure` composition
   root extension.** The extension lives in
   `src/AiEng.Platform.App/Composition/Infrastructure/InfrastructureServiceCollectionExtensions.cs`.
   The extension registers
   `IProcessRunner` → `SystemProcessRunner`,
   `ICredentialVault` →
   `WindowsCredentialVault`,
   `IPlatformInfo` → `SystemPlatformInfo`,
   `IProjectStore` → `JsonFileProjectStore`.
   The `AddProjects` extension is updated:
   the M3 in-memory
   `InMemoryProjectStore` registration is
   removed; the on-disk
   `JsonFileProjectStore` is now registered
   through `AddInfrastructure`. The
   `IProjectService` and the UI are
   unchanged.

10. **The M4-A architecture tests.** Three new
    architecture tests are added in
    `tests/AiEng.Platform.ArchitectureTests/Infrastructure/`:
    - `Infrastructure_Respects_ProcessBoundary`:
      asserts that
      `src/AiEng.Platform.Infrastructure/`
      is the only project that references
      `System.Diagnostics.Process` (the test
      is `Registered` in M4-A and `Active` in
      M4-D; the test follows the ADR-016
      pattern).
    - `Infrastructure_Respects_CredentialBoundary`:
      asserts that
      `src/AiEng.Platform.Infrastructure/`
      is the only project that references
      `CredentialManagement` (or the
      selected NuGet package).
    - `Pages_Resolve_Projects_Through_Service`
      extension: a new test asserts the
      `AppProjectCard.Open` action resolves
      the project's path through the seam
      (the test is `Active` in M4-A; the M3.2
      test extension is preserved).

11. **The M4-A unit + bUnit tests.** The
    `IProcessRunner` / `ICredentialVault` /
    `IPlatformInfo` / `JsonFileProjectStore`
    contracts are tested end-to-end:
    - `IProcessRunnerTests` (10+ tests) —
      streaming output, fire-and-forget,
      exit code propagation, cancellation,
      non-existent executable failure path.
    - `WindowsCredentialVaultTests` (5+
      tests) — get / set / delete round-trip,
      missing credential returns null.
    - `SystemPlatformInfoTests` (3+ tests) —
      `GetDataDirectory` returns a non-empty
      path on the current platform.
    - `JsonFileProjectStoreTests` (15+
      tests) — round-trip, missing file
      returns empty list, concurrent write
      serialization, file corruption
      recovery (returns empty list + logs a
      warning).
    - `AppProjectCardTests` extension (3+
      tests) — Open button is enabled, click
      handler calls `IProcessRunner` with
      the expected executable + arguments,
      Open button is disabled while the
      process is running.
    - `ProjectsPageTests` extension (2+
      tests) — registering a project
      persists to the on-disk store;
      unregistering a project removes from
      the on-disk store.

12. **The M4-A documentation.** The
    documentation is added in
    `docs/infrastructure.md` (10 sections:
    Goals, Project Structure, Process
    Boundary, Credential Boundary, On-Disk
    Project Store, Platform Info, Open
    Action, Composition Root, Tests, Out of
    Scope). The M3 `docs/projects.md` is
    updated: the "M3 / M4-A Boundary"
    section is updated to reflect M4-A
    delivered.

## 3. Out of Scope

1. **Provider creation.** M4-A does not
   create `IGitProvider`,
   `IAgentRuntimeProvider`, or any
   `IProvider` family. The M4-A first
   session's brief explicitly says
   "Do not create providers." The first
   concrete providers land in M4-D.
2. **M4-B / M4-C / M4-D work.** M4-B
   (provider registration), M4-C
   (capability detection), and M4-D
   (provider composition + activation)
   are separate sessions. M4-A does not
   begin any of them.
3. **Worktree creation.** M5.
4. **Agent launching.** M6.
5. **Review / quality gates.** M7.
6. **Autonomous loops / orchestration.**
   M8.
7. **Activation of the four registered-but-
   disabled composition-root tests.** The
   four `CompositionRootBoundaryTests` are
   activated in M4-D, not M4-A. M4-A
   introduces the infrastructure seam;
   M4-D introduces the first concrete
   providers and activates the tests.
8. **M3.2 modal Escape-key / backdrop-
   click / Browse-folder follow-ups.**
   These are M3 known issues that M4-A
   may or may not address; the M4-A
   plan does not pre-judge. The M3
   closeout records them as M3 known
   issues; the follow-up is a future
   task.
9. **macOS / Linux credential vault.**
   The M4-A scope is Windows. The
   `ICredentialVault` contract is
   platform-agnostic; the
   `WindowsCredentialVault`
   implementation is Windows-only. A
   future slice can introduce a
   `MacOSKeychainCredentialVault` and a
   `LinuxSecretServiceCredentialVault`.
   The M4-A first session may introduce
   a `IPlatformInfo.IsWindows` extension
   method to support the selection; the
   M4-A scope is the Windows
   implementation.
10. **A design-system `AppDialog` primitive.**
    The M3.2 closeout's decision (HTML5
    native `<dialog>`) is reaffirmed; M4-A
    does not introduce a new design-system
    primitive. M4-A's UI surface is the
    `AppProjectCard` Open action; the
    action does not require a new
    primitive.

## 4. Files to Add

### New project

- `src/AiEng.Platform.Infrastructure/
  AiEng.Platform.Infrastructure.csproj` —
  the new csproj; references
  `AiEng.Platform.Application` and
  `AiEng.Platform.Domain`; enables
  `Nullable` + `TreatWarningsAsErrors`.
- `src/AiEng.Platform.Infrastructure/Properties/
  AssemblyInfo.cs` — assembly metadata.

### Contracts (in Application)

- `src/AiEng.Platform.Application/Infrastructure/
  IProcessRunner.cs` — the process
  boundary contract.
- `src/AiEng.Platform.Application/Infrastructure/
  ProcessResult.cs` — the
  `ProcessResult` envelope
  (record struct).
- `src/AiEng.Platform.Application/Infrastructure/
  ICredentialVault.cs` — the credential
  boundary contract.
- `src/AiEng.Platform.Application/Infrastructure/
  IPlatformInfo.cs` — the platform-info
  contract.

### Implementations (in Infrastructure)

- `src/AiEng.Platform.Infrastructure/
  ProcessRunner/SystemProcessRunner.cs` —
  the process boundary implementation.
- `src/AiEng.Platform.Infrastructure/
  Credentials/WindowsCredentialVault.cs` —
  the credential boundary implementation.
- `src/AiEng.Platform.Infrastructure/
  Platform/SystemPlatformInfo.cs` — the
  platform-info implementation.
- `src/AiEng.Platform.Infrastructure/
  Projects/JsonFileProjectStore.cs` — the
  on-disk `IProjectStore` implementation.

### Composition root

- `src/AiEng.Platform.App/Composition/
  Infrastructure/InfrastructureServiceCollectionExtensions.cs`
  — the `AddInfrastructure` extension;
  registers the four Infrastructure
  services.

### Tests

- `tests/AiEng.Platform.UnitTests/
  Infrastructure/IProcessRunnerTests.cs`.
- `tests/AiEng.Platform.UnitTests/
  Infrastructure/WindowsCredentialVaultTests.cs`.
- `tests/AiEng.Platform.UnitTests/
  Infrastructure/SystemPlatformInfoTests.cs`.
- `tests/AiEng.Platform.UnitTests/
  Infrastructure/JsonFileProjectStoreTests.cs`.
- `tests/AiEng.Platform.ArchitectureTests/
  Infrastructure/Infrastructure_Respects_ProcessBoundary.cs`
  — registered-but-disabled per ADR-016.
- `tests/AiEng.Platform.ArchitectureTests/
  Infrastructure/Infrastructure_Respects_CredentialBoundary.cs`
  — registered-but-disabled per ADR-016.
- `tests/AiEng.Platform.ComponentTests/
  Components/Projects/AppProjectCardTests.cs`
  — Open action enabled, click handler
  resolves through the seam.
- `tests/AiEng.Platform.ComponentTests/
  Pages/ProjectsPageTests.cs` —
  registration persists, unregister
  removes.

### Documentation

- `docs/infrastructure.md` — the M4-A
  documentation.
- `implementation-report-m4-a-1-infrastructure-project-skeleton.md`
  — the M4-A.1 closeout report (mirrors
  the M3.1 closeout report).
- `.ai/handoffs/2026-07-11-m4-a-1-infrastructure-project-skeleton.md`
  — the M4-A.1 handoff (mirrored to
  `latest.md`).

## 5. Files to Modify

- `src/AiEng.Platform.App/AiEng.Platform.App.csproj`
  — add a project reference to
  `AiEng.Platform.Infrastructure`.
- `src/AiEng.Platform.App/Program.cs` — call
  `AddInfrastructure()` after `AddProjects()`.
- `src/AiEng.Platform.App/Composition/Projects/
  ProjectsServiceCollectionExtensions.cs` —
  remove the M3
  `InMemoryProjectStore` registration (the
  on-disk `JsonFileProjectStore` is now
  registered through `AddInfrastructure`).
- `src/AiEng.Platform.App/Components/Projects/
  AppProjectCard.razor` — enable the Open
  button; wire the click handler to the
  new on-disk `IProjectStore` +
  `IProcessRunner`.
- `tests/AiEng.Platform.ArchitectureTests/
  Pages/PagesResolveProjectsThroughServiceTests.cs`
  — add the M4-A Open-action test.
- `docs/projects.md` — update the "M3 / M4-A
  Boundary" section to reflect M4-A
  delivered.
- `ROADMAP.md` § 2 (M4-A row `Planned` →
  `Active`; M4-A paragraph updated); § 3
  (M4-A DoD bullets added).
- `.ai/plans/master-delivery-plan.md` § 1
  (M4-A row `Planned` → `Active`); § 3
  (M4-A block Active; M4-A evidence list
  added; M4-A.1 slice row added).
- `.ai/state/session.json` (M4-A.1
  envelope).
- `.ai/state/tasks.json` (T-021 +
  T-022 + T-023 records; T-021 → In
  Progress → Done in M4-A.1).
- `.ai/state/current.md` (active
  milestone M3 → M4-A; last completed
  task T-021; next recommended task
  T-022; etc.).
- `.ai/state/task-board.md` (M4-A.1 in
  Ready → In Progress → Done Recently;
  the M4-A summary in `Deferred`
  archived).
- `.ai/state/milestones.json` (M4-A
  Planned → Active; M4-A.1 slice block
  added; M4-A evidence block added).

## 6. Critical Files to Read Before Editing

- `AGENTS.md` — the 17 non-negotiable rules.
- `.ai/session-start.md` — the 13-step
  lifecycle.
- `.ai/commands.md` — the command
  protocol.
- `.ai/workflows/progressive-coding.md` —
  the Progressive Coding Rule.
- `.ai/workflows/branching-strategy.md` —
  the 12 rules.
- `.ai/workflows/milestone-closeout.md` —
  the Milestone Closeout Standard.
- `.ai/plans/M3-project-registration.md` —
  the M3 plan; the M4-A plan composes the
  M3 surface.
- `retrospective-m3-project-registration.md` —
  the M3 retrospective; the M4-A plan
  accounts for the M3 retrospective's
  § 13 recommendations.
- `implementation-report-m3-1-project-registration-slice-1.md`
  and
  `implementation-report-m3-2-project-registration-slice-2.md`
  — the M3 implementation reports.
- `retrospective-m2-application-shell-and-navigation.md` —
  the M2 retrospective; the M2 retrospective's
  § 13 recommendations are the M3 plan's
  input; the M3 retrospective's § 13
  recommendations are the M4-A plan's
  input.
- `docs/projects.md` — the M3 surface
  documentation; M4-A updates the
  boundary section.
- `src/AiEng.Platform.Application/Projects/`
  — the M3 `IProjectService` /
  `IProjectStore` / `Result<T>` / `ProjectService`
  — M4-A composes these.
- `src/AiEng.Platform.App/Composition/Projects/`
  — the M3 `AddProjects` extension — M4-A
  modifies this.
- `src/AiEng.Platform.App/Components/Projects/`
  — the M3 components — M4-A enables the
  Open action on `AppProjectCard`.

## 7. Existing Functions and Utilities to Reuse

- The **M3 `IProjectService`** is
  composed by M4-A. The interface is
  unchanged; the M3 in-memory store is
  swapped for the on-disk store; the
  `IProjectService` and the UI are
  unchanged.
- The **M3 `AddProjects` extension** is
  modified by M4-A: the in-memory
  `IProjectStore` registration is removed;
  the on-disk `IProjectStore` is now
  registered through `AddInfrastructure`.
  The `IProjectService` and
  `IProjectStore` registrations are
  preserved in `AddProjects` (the
  `AddProjects` extension still owns
  `IProjectService`; the
  `IProjectStore` registration is
  removed; the on-disk store is
  registered by `AddInfrastructure`).
- The **M3 `Result<T>` envelope** is
  composed by M4-A. The M4-A
  `IProcessRunner` returns
  `ProcessResult` (a value type) on
  success; the M4-A `ICredentialVault`
  returns `string?` on success (the
  M4-A scope is simple get / set /
  delete; the `Result<T>` envelope
  may be introduced in M4-D for the
  provider contracts).
- The **M3 `Pages_Resolve_Projects_Through_Service`
  architecture test** is extended by
  M4-A: a new test asserts the
  `AppProjectCard.Open` action resolves
  the project's path through the seam.
- The **M1.2 design system** is composed
  by M4-A. The M4-A UI surface is the
  `AppProjectCard` Open action; the
  action does not require a new
  primitive.
- The **M2.2 `INavigationRegistry`** is
  unchanged by M4-A. M4-A's surface is
  the `/projects` page; the page is
  reachable through the M2.2 registry.
- The **.NET 8+ `TimeProvider`** is the
  M3 clock. M4-A may either keep
  `TimeProvider` (the M3 implementation
  is fine) or introduce a thin
  `IClock` adapter over `TimeProvider`
  (the choice is M4-A's; the M3 closeout
  does not pre-judge). The M4-A plan
  documents the choice in the M4-A
  closeout's Lessons Learned.

## 8. Risks and Mitigations

| Risk                                                                 | Mitigation                                                                                                                       |
| -------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------- |
| The `IProcessRunner` is the only `Process.Start` call site; a regression that adds a direct `Process.Start` in `Application/` or `App/` breaks the boundary. | The architecture test `Infrastructure_Respects_ProcessBoundary` is `Registered` in M4-A and `Active` in M4-D (per ADR-016). M4-A's PR is the first to introduce the test stub. |
| The on-disk `IProjectStore` may corrupt on a crash mid-write.       | The on-disk store writes to a temp file + atomic rename. The crash-recovery test asserts the store returns the last consistent state. |
| The Windows Credential Manager requires elevation on some hosts.    | The M4-A first session's `WindowsCredentialVault` tests assume a normal user; the M4-A first session documents the elevation caveat in `docs/infrastructure.md`. |
| The `CredentialManagement` NuGet package may not be the right choice; an alternative is `Meziantou.Framework.Win32.CredentialManager`. | The M4-A first session selects the package based on the M4-A first session's research; the selection is documented in the M4-A.1 implementation report. |
| The on-disk store file path must not conflict with another application's store. | The M4-A first session selects a unique subdirectory (`AiEng/Platform/projects.json`); the M4-A.1 implementation report documents the path. |
| The Open action's `explorer.exe` call is Windows-specific.            | The M4-A first session introduces a `IPlatformInfo.IsWindows` extension method; the Open action checks `IsWindows` and shows a toast / error on non-Windows. |
| The M3 in-memory `InMemoryProjectStore` is removed in M4-A. The removal may break a future M3.2 regression test. | The `InMemoryProjectStore` is preserved as a test fixture (the class is moved to `tests/AiEng.Platform.UnitTests/Infrastructure/InMemoryProjectStore.cs`); the M3 tests that use it as a fixture continue to work. |
| M4-A's tests may be flaky on CI hosts where the data directory is read-only. | The M4-A tests use `Path.GetTempPath()` + a unique subdirectory; the M4-A.1 implementation report documents the test isolation. |

## 9. Coherent Commit + Merge

M4-A is split into slices per the M3
2-slice pattern. The M4-A.1 slice is the
**infrastructure project skeleton** (the
csproj + `IProcessRunner` +
`ICredentialVault` + `IPlatformInfo` + the
on-disk `IProjectStore` +
`AddInfrastructure`; the M3 in-memory
`IProjectStore` is removed from
`AddProjects` and the on-disk store is
registered through `AddInfrastructure`).
M4-A.2 is the **Open action** (the
`AppProjectCard` Open button is enabled +
wired to the new on-disk `IProjectStore` +
`IProcessRunner`; the architecture test
`Pages_Resolve_Projects_Through_Service`
is extended; the bUnit tests for the Open
action are added).

Each M4-A slice ships a single coherent
commit on its feature branch. The branch is
fast-forwarded into `main` per the
branching strategy rule 6. The branch is
deleted per rule 7. The `m4-a` annotated
milestone tag is created at the M4-A
closeout commit on `main` per rule 9
(M4-A's closeout is a future slice; the
`m4-a` tag is created at that future
commit, not at the M4-A.1 / M4-A.2
commits).

The M4-A.1 commit message is
`feat(m4-a.1): add infrastructure project skeleton with IProcessRunner, ICredentialVault, IPlatformInfo, and on-disk IProjectStore`.
The M4-A.2 commit message is
`feat(m4-a.2): enable AppProjectCard.Open action wired to on-disk IProjectStore and IProcessRunner`.

## 10. Stop Condition

The M4-A.1 implementation session stops
after:
1. The M4-A.1 branch
   `feature/T-021-m4-a-1-infrastructure-project-skeleton`
   is created.
2. The M4-A.1 changes are committed in a
   single coherent commit.
3. The branch is fast-forwarded into
   `main`.
4. The branch is deleted.
5. The state files are updated
   (`session.json`, `tasks.json`,
   `current.md`, `task-board.md`,
   `milestones.json`).
6. The M4-A.1 implementation report is
   written at
   `implementation-report-m4-a-1-infrastructure-project-skeleton.md`.
7. The M4-A.1 handoff is written at
   `.ai/handoffs/2026-07-11-m4-a-1-infrastructure-project-skeleton.md`
   and mirrored to
   `.ai/handoffs/latest.md`.
8. The validation gate passes
   (`npm run css:build` exit 0;
   `dotnet restore` exit 0;
   `dotnet build` 0 warnings, 0 errors;
   `dotnet test` 273+ passed, 0 failed,
   7 skipped; `dotnet format
   --verify-no-changes` exit 0; visual
   smoke on `/projects` — the Open
   action is still disabled in M4-A.1
   per the M4-A scope; the M4-A.1
   visual smoke asserts the page loads
   200 and the project list persists
   across an application restart).

The M4-A.1 session does **not** begin the
M4-A.2 work. M4-A.2 begins in a separate
session after the M4-A.1 closeout.

## 11. Dependencies

- M3 (delivered 2026-07-11; M3.1 + M3.2
  + M3 closeout).
- M2 (delivered 2026-07-11; M2.1 →
  M2.6; the M2 shell + the M2.2
  `INavigationRegistry` + the M2.4
  `IProjectIntelligenceReader`).
- M1.2 (the design system).
- The .NET 8+ `TimeProvider` BCL type.
- The `CredentialManagement` NuGet
  package (or the selected alternative).

## 12. Milestone Closeout Hook

M4-A's own closeout slice is a future
slice (M4-A.x — the M4-A retrospective).
The M4-A closeout follows the Milestone
Closeout Standard
(`.ai/workflows/milestone-closeout.md`)
and mirrors the M2.6 closeout and the
M3 closeout. The M4-A retrospective
lands at
`retrospective-m4-a-infrastructure-process-execution.md`
(13 sections, per the standard). The
`m4-a` annotated milestone tag is created
at the M4-A closeout commit on `main` per
the branching strategy rule 9. The M4-A
closeout produces the M4-B plan in
`Awaiting Approval` and promotes the first
M4-B task to `Ready`.

The M4-A closeout is **not** part of the
M4-A scope; the M4-A closeout is a
separate slice that begins after the
M4-A.2 slice is delivered.

---

**End of M4-A plan.** The plan is
`Awaiting Approval` (2026-07-11). The M4-A
first session reviews and revises the
plan as needed; the M4-A.1 first task
(T-021) is `Ready` in
`.ai/state/tasks.json`.
