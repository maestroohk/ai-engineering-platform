# Handoff — M4-C.1 — `m4-c-1-provider-registry-contract-and-family-registries` (2026-07-13)

> **The M4-C.1 first-session per-slice handoff.**
> M4-C.1 (T-028) is the first M4-C slice.
> M4-C.1 follows M4-B closeout per the
> Progressive Coding Rule. M4-C.1 ships the
> boundary slice: the `IProviderRegistry`
> contract + the 6 family registry contracts +
> the `SystemProviderRegistry` implementation +
> the 6 no-op family stubs + the
> `AddProviderRegistry` composition root + the
> 6 family fakes + 19 unit tests. M4-C.1 is a
> code + state + workflow change — no UI
> surface, no M4-C.2 implementation, no M4-D
> plan promotion, no push.

---

## 1. What was delivered

The M4-C.1 first session (`M4-C.1` — T-028) is
**Done** (2026-07-13).

The M4-C.1 first session ships:

- **The `IProviderRegistry` contract** at
  `src/AiEng.Platform.Application/Providers/IProviderRegistry.cs`.
  The contract is the single allowed seam
  between the application and the provider
  layer; the M4-C architecture requires that
  all provider access flows through
  `IProviderRegistry`, never through the
  concrete `SystemProviderRegistry` or any
  `IProvider` implementation. The contract
  exposes a single method:
  `Task<IReadOnlyList<ProviderDescriptor>>
  ListProvidersAsync(ProviderFamily family,
  CancellationToken cancellationToken =
  default)`.
- **The `ProviderDescriptor` sealed record**
  at
  `src/AiEng.Platform.Application/Providers/ProviderDescriptor.cs`.
  The descriptor is the data envelope the
  M4-C.2 `AppProviderList` component renders.
  The record has 6 fields: `Id` (string) +
  `DisplayName` (string) + `Family`
  (`ProviderFamily`) + `Status`
  (`ProviderStatus`) + `Version?` (string?) +
  `Metadata`
  (`IReadOnlyDictionary<string, string>`).
  The `Metadata` dictionary allows
  per-descriptor extension (e.g., `"Path"`,
  `"Command"`, `"Args"`); the M4-C.1 default
  metadata is empty.
- **The `ProviderFamily` enum** at
  `src/AiEng.Platform.Application/Providers/ProviderFamily.cs`.
  6 values, ordered: `Git = 0`,
  `AgentRuntime = 1`, `Review = 2`,
  `QualityGate = 3`, `AutonomousLoop = 4`,
  `Orchestration = 5`. The 6 families map to
  the 6 capability families in the M4-B
  `IHostCapabilitiesService` contract.
- **The `ProviderStatus` enum** at
  `src/AiEng.Platform.Application/Providers/ProviderStatus.cs`.
  3 values: `Available = 0`, `Unavailable = 1`,
  `Disabled = 2`. The 3 states map to the
  M4-B `HostCapability.Available` +
  `HostCapability.CredentialAvailable` flags.
  `Disabled` is forward-compatible with the
  M4-C.2 `AppProviderList` enable/disable
  action.
- **The `IProviderFamily` base interface** at
  `src/AiEng.Platform.Application/Providers/Families/IProviderFamily.cs`.
  The base interface declares the
  `ListProvidersAsync(CancellationToken)` method
  shared by all 6 family-specific
  subinterfaces. The base is required to
  resolve the C# 14 switch expression
  common-type issue in `SystemProviderRegistry`
  (CS8506).
- **The 6 family registry contracts** in
  `src/AiEng.Platform.Application/Providers/Families/`:
  `IGitProviderFamily` +
  `IAgentRuntimeProviderFamily` +
  `IReviewProviderFamily` +
  `IQualityGateProviderFamily` +
  `IAutonomousLoopProviderFamily` +
  `IOrchestrationProviderFamily`. Each
  family-specific subinterface is an empty
  marker interface that extends
  `IProviderFamily` (the subinterface
  pattern enables the M4-C.2 page to
  `@inject IGitProviderFamily` etc. for
  per-family direct access if needed; the
  M4-C.1 `SystemProviderRegistry` aggregates
  the 6 family registries through the
  `IProviderFamily` base).
- **The `SystemProviderRegistry`
  implementation** at
  `src/AiEng.Platform.Infrastructure/Providers/SystemProviderRegistry.cs`.
  The implementation:
  - Constructor-injects the 6 family
    registries as `IProviderFamily` (the
    base) + `IHostCapabilitiesService` +
    `ILogger<SystemProviderRegistry>`. The
    ctor null-checks every dependency.
  - The `ListProvidersAsync` method:
    1. Throws `OperationCanceledException`
       if the token is cancelled.
    2. Calls `IHostCapabilitiesService.DetectAsync`
       to read the host capabilities (one
       call per `ListProvidersAsync`
       invocation).
    3. Computes the set of available host
       capability keys (case-insensitive).
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
- **The 6 no-op family stub implementations**
  in
  `src/AiEng.Platform.Infrastructure/Providers/Families/`:
  `GitProviderFamily` +
  `AgentRuntimeProviderFamily` +
  `ReviewProviderFamily` +
  `QualityGateProviderFamily` +
  `AutonomousLoopProviderFamily` +
  `OrchestrationProviderFamily`. Each stub
  returns
  `Task.FromResult<IReadOnlyList<ProviderDescriptor>>(Array.Empty<ProviderDescriptor>())`.
  The stubs are the production
  implementations registered by
  `AddProviderRegistry`; the stubs are
  replaced by concrete provider
  implementations in M4-D per the brief:
  "Do not create providers" (M4-C.1 is the
  registry foundation; M4-D is the
  providers).
- **The 6 family fakes** in
  `tests/AiEng.Platform.UnitTests/Providers/`:
  `FakeGitProviderFamily` +
  `FakeAgentRuntimeProviderFamily` +
  `FakeReviewProviderFamily` +
  `FakeQualityGateProviderFamily` +
  `FakeAutonomousLoopProviderFamily` +
  `FakeOrchestrationProviderFamily`. Each
  fake records `CallCount` +
  `ObservedTokens` + `QueuedResults`; returns
  the next queued result (or an empty array
  if no result is queued). The fakes are the
  M4-C.1 test surface; the M4-D concrete
  provider implementations replace the fakes
  in production.
- **The `AddProviderRegistry` composition
  root extension** at
  `src/AiEng.Platform.App/Composition/Providers/ProviderRegistryServiceCollectionExtensions.cs`.
  The composition root registers the 6
  no-op family stubs + `IProviderRegistry` →
  `SystemProviderRegistry` as singletons via
  `TryAddSingleton` (the "first wins"
  semantics allows M4-D to override the
  stubs with concrete provider
  implementations).
- **The `AddProviderRegistry` wire-up** in
  `src/AiEng.Platform.App/Composition/ServiceCollectionExtensions.cs`.
  `AddPlatformServices` now calls
  `services.AddProviderRegistry()` after
  `services.AddHostCapabilities()` per the
  M4-C plan § 2 item 8.
- **19 unit tests** in
  `tests/AiEng.Platform.UnitTests/Providers/SystemProviderRegistryTests.cs`.
  Test breakdown: 8 constructor-null tests +
  7 happy-path/edge-case tests + 1
  Information-level log test + 2 dispatch
  tests (one per family) + 1
  capability-call-count test + 1
  `ArgumentOutOfRangeException` test + 1
  fake `CallCount` test. The test file
  includes 2 in-line test doubles: a
  `FakeHostCapabilitiesService` (records
  `CallCount`; returns a configured
  `HostCapabilities`) + a `ListLogger<T>`
  (records the log messages in a
  `List<string>`).
- **The project-continuity state update per
  Rule 15**:
  - `.ai/state/session.json` — the M4-C.1
    envelope replaces the M4-B closeout
    envelope. `session_id =
    m4-c-1-provider-registry-contract-and-family-registries`;
    `session_type = implementation-slice`;
    the full `scope.in_scope` (15 items) +
    `scope.out_of_scope` (7 items) +
    `current_understanding` (test_status =
    395 passed; intended_next_action stops
    at T-029).
  - `.ai/state/tasks.json` — T-028
    transitions `Ready` → `InProgress` →
    `Done` with the full evidence block (29
    files added + 1 file modified + 19 unit
    tests + commit_message + branch_deleted
    + test_count + next_step). T-029 is
    added as M4-C.2 first session in `Ready`
    status.
  - `.ai/state/current.md` — active slice
    updated; last stable commit updated; next
    recommended task updated; M4-C.1 evidence
    references added.
  - `.ai/state/task-board.md` — M4-C.1 row
    in `Done Recently`; T-029 stub row in
    `Ready`; M4-C status `Awaiting Approval`
    → `Active`.
  - `.ai/state/milestones.json` — M4-C.1
    slice block from `Planned` to `Done`;
    M4-C milestone `Awaiting Approval` →
    `Active`; C-018 evidence block
    finalised; C-010 + C-011 evidence blocks
    initialised.
  - `.ai/state/capabilities.json` — C-018
    evidence block finalised; C-010 + C-011
    evidence blocks initialised; top-level
    `updated_at` + `updated_by_session`
    updated.
- **This handoff** at
  `.ai/handoffs/2026-07-13-m4-c-1-provider-registry-contract-and-family-registries.md`
  (mirrored to `.ai/handoffs/latest.md`).
- **The M4-C.1 implementation report** at
  `implementation-report-m4-c-1-provider-registry-contract-and-family-registries.md`
  (15+ sections mirroring the M4-B.1 +
  M4-B.2 + M4-B.3 + M4-B closeout reports).

## 2. Where the code lives

- **Contracts:** `src/AiEng.Platform.Application/Providers/`
  - `IProviderRegistry.cs` (the M4-C
    contract; 14 lines; the single
    `ListProvidersAsync(family, ct)`
    method).
  - `ProviderDescriptor.cs` (the sealed
    record; 9 lines; 6 fields).
  - `ProviderFamily.cs` (the 6-value enum;
    9 lines).
  - `ProviderStatus.cs` (the 3-value enum;
    7 lines).
  - `Families/IProviderFamily.cs` (the
    base interface; 5 lines; the
    `ListProvidersAsync(ct)` method).
  - `Families/IGitProviderFamily.cs` (the
    Git family subinterface; 3 lines; empty
    marker interface).
  - `Families/IAgentRuntimeProviderFamily.cs`
    (mirrors `IGitProviderFamily`).
  - `Families/IReviewProviderFamily.cs`
    (mirrors `IGitProviderFamily`).
  - `Families/IQualityGateProviderFamily.cs`
    (mirrors `IGitProviderFamily`).
  - `Families/IAutonomousLoopProviderFamily.cs`
    (mirrors `IGitProviderFamily`).
  - `Families/IOrchestrationProviderFamily.cs`
    (mirrors `IGitProviderFamily`).
- **Implementation:**
  `src/AiEng.Platform.Infrastructure/Providers/`
  - `SystemProviderRegistry.cs` (the M4-C
    implementation; ~95 lines; aggregates
    6 family registries through
    `IProviderFamily`; consumes
    `IHostCapabilitiesService` through DI;
    downgrades `Available` to `Unavailable`
    when the family capability is
    unavailable; preserves `Disabled` and
    `Unavailable`; logs at `Information`
    level).
  - `Families/GitProviderFamily.cs` (the
    Git family no-op stub; 13 lines;
    returns an empty `IReadOnlyList`).
  - `Families/AgentRuntimeProviderFamily.cs`
    (mirrors `GitProviderFamily`).
  - `Families/ReviewProviderFamily.cs`
    (mirrors `GitProviderFamily`).
  - `Families/QualityGateProviderFamily.cs`
    (mirrors `GitProviderFamily`).
  - `Families/AutonomousLoopProviderFamily.cs`
    (mirrors `GitProviderFamily`).
  - `Families/OrchestrationProviderFamily.cs`
    (mirrors `GitProviderFamily`).
- **Composition root:**
  `src/AiEng.Platform.App/Composition/Providers/`
  - `ProviderRegistryServiceCollectionExtensions.cs`
    (the M4-C composition root; 28 lines;
    `AddProviderRegistry` extension method).
  - `ServiceCollectionExtensions.cs`
    (modified; 1 line added: the
    `services.AddProviderRegistry()` call
    in `AddPlatformServices`).
- **Tests:**
  `tests/AiEng.Platform.UnitTests/Providers/`
  - `FakeGitProviderFamily.cs` (the Git
    family fake; ~30 lines; records
    `CallCount` + `ObservedTokens` +
    `QueuedResults`).
  - `FakeAgentRuntimeProviderFamily.cs`
    (mirrors `FakeGitProviderFamily`).
  - `FakeReviewProviderFamily.cs` (mirrors
    `FakeGitProviderFamily`).
  - `FakeQualityGateProviderFamily.cs`
    (mirrors `FakeGitProviderFamily`).
  - `FakeAutonomousLoopProviderFamily.cs`
    (mirrors `FakeGitProviderFamily`).
  - `FakeOrchestrationProviderFamily.cs`
    (mirrors `FakeGitProviderFamily`).
  - `SystemProviderRegistryTests.cs` (19
    unit tests; ~330 lines; includes
    `FakeHostCapabilitiesService` +
    `ListLogger<T>`).
- **Project-continuity state:** the 6
  state files (per Rule 15).
- **Documentation:** this handoff + the
  M4-C.1 implementation report.

## 3. What was NOT delivered (scope discipline)

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
  M4-C close; the M4-D plan is drafted in the
  M4-C closeout session.
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
  push** (the user may push in a follow-up
  command per the command protocol).
- **No architectural rule changes.** M4-C.1
  does not introduce new architectural rules;
  no ADR is required. The M4-B.3
  `Capabilities_Resolved_Through_Service`
  architecture test remains Active and green;
  the M4-A.1 architecture tests
  (`Infrastructure_Respects_ProcessBoundary` +
  `Infrastructure_Respects_CredentialBoundary`)
  remain registered-but-disabled per ADR-016 /
  M4-D.

## 4. Validation summary

- **Format gate:** `dotnet format
  --verify-no-changes` exits 0; the format is
  canonical and CI-clean. The new files use
  4-space indent + CRLF (per .editorconfig).
- **Build gate:** `dotnet build
  AiEng.Platform.slnx` exits 0; 0 warnings, 0
  errors (with
  `TreatWarningsAsErrors=true` from
  `Directory.Build.props`).
- **Test gate:** `dotnet test
  AiEng.Platform.slnx --no-build` reports
  **395 passed** (376 pre-M4-C.1 + 19 new
  M4-C.1 unit tests), 0 failed, 9 skipped
  (per ADR-016 / M4-D). Breakdown: 118 unit +
  263 component + 14 architecture.
- **JSON validation gate:** the 4 state JSON
  files (`session.json` + `tasks.json` +
  `milestones.json` + `capabilities.json`) are
  valid JSON; the `updated_at` field is
  updated; the schema is preserved.
- **CRLF validation gate:** every new +
  modified file is CRLF; the `unix2dos` command
  is applied to every file before commit.
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
  (`Infrastructure_Respects_ProcessBoundary` +
  `Infrastructure_Respects_CredentialBoundary`),
  which are registered-but-disabled per
  ADR-016 / M4-D.
- **No scope creep:** the diff does not modify
  any file under
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

## 5. One documented deviation

The M4-C.1 plan registered the 6 family fakes
(test-only types in
`tests/AiEng.Platform.UnitTests/Providers/`)
as the family registry implementations in
the `AddProviderRegistry` composition root.
This violated the DI layering rule (the
`App` project cannot reference
`AiEng.Platform.UnitTests`).

The fix:

1. **6 no-op family stub implementations**
   were created in
   `src/AiEng.Platform.Infrastructure/Providers/Families/`
   (`GitProviderFamily` + 5 others) — the
   stubs return an empty
   `IReadOnlyList<ProviderDescriptor>` (no
   concrete providers ship in M4-C.1; the
   providers come in M4-D per the brief: "Do
   not create providers").
2. **`AddProviderRegistry` registers the
   Infrastructure no-op stubs**, not the
   test fakes.
3. **The 6 family fakes stay in
   `tests/AiEng.Platform.UnitTests/Providers/`**
   and are passed to `SystemProviderRegistry`
   via constructor injection in unit tests.

The deviation preserves the M4-C.1
architecture (the family registries exist;
the `SystemProviderRegistry` aggregates
them; `IProviderRegistry` is the single
seam) and the test surface (19 unit tests
assert the `SystemProviderRegistry` behavior
via constructor-injected fakes). The
deviation is recorded in the M4-C.1
implementation report § 13 Deviations and in
`.ai/state/tasks.json` T-028 `notes`.

## 6. Definition of Done

The M4-C.1 first session DoD per the M4-C plan
§ 2 items 1-8 + § 10 Test Plan item 10.1
(per inspection):

- [x] **§ 2 item 1: `IProviderRegistry`
  contract** at
  `src/AiEng.Platform.Application/Providers/IProviderRegistry.cs`
  (14 lines; single
  `ListProvidersAsync(family, ct)` method).
- [x] **§ 2 item 2: `ProviderDescriptor`
  record** at
  `src/AiEng.Platform.Application/Providers/ProviderDescriptor.cs`
  (9 lines; `sealed record class`; 6
  fields: `Id` + `DisplayName` + `Family` +
  `Status` + `Version?` + `Metadata`).
- [x] **§ 2 item 3: `ProviderFamily` enum** at
  `src/AiEng.Platform.Application/Providers/ProviderFamily.cs`
  (9 lines; 6 values: `Git = 0` +
  `AgentRuntime = 1` + `Review = 2` +
  `QualityGate = 3` + `AutonomousLoop = 4` +
  `Orchestration = 5`).
- [x] **§ 2 item 4: `ProviderStatus` enum** at
  `src/AiEng.Platform.Application/Providers/ProviderStatus.cs`
  (7 lines; 3 values: `Available = 0` +
  `Unavailable = 1` + `Disabled = 2`).
- [x] **§ 2 item 5: 6 family registry
  contracts** in
  `src/AiEng.Platform.Application/Providers/Families/`
  (`IProviderFamily` base + 6
  family-specific subinterfaces).
- [x] **§ 2 item 6: `SystemProviderRegistry`
  implementation** at
  `src/AiEng.Platform.Infrastructure/Providers/SystemProviderRegistry.cs`
  (~95 lines; aggregates 6 family registries
  through `IProviderFamily`; consumes
  `IHostCapabilitiesService` through DI;
  downgrades `Available` to `Unavailable` when
  the family capability is unavailable;
  preserves `Disabled` and `Unavailable`; logs
  at `Information` level).
- [x] **§ 2 item 7: 6 family fakes** in
  `tests/AiEng.Platform.UnitTests/Providers/`
  (each records `CallCount` +
  `ObservedTokens` + `QueuedResults`; returns
  the next queued result or an empty array).
- [x] **§ 2 item 8: `AddProviderRegistry`
  composition root extension** at
  `src/AiEng.Platform.App/Composition/Providers/ProviderRegistryServiceCollectionExtensions.cs`
  (registers the 6 no-op family stubs +
  `IProviderRegistry` → `SystemProviderRegistry`
  via `TryAddSingleton`).
- [x] **§ 10 item 10.1: 19 unit tests** in
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
  (15+ sections).
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

## 7. Next session

The next session is the **M4-C.2 first
session (T-029)** on the user's `Approve` or
`Next` invocation. T-029 is `Ready` in
`.ai/state/tasks.json`; the M4-C.2 first
session:

1. Reads the M4-C plan
   (`.ai/plans/M4-C-provider-registry-foundation.md`)
   + this handoff + the M4-C.1 implementation
   report + the M4-C.1 contract + the M4-C.1
   implementation + the M4-C.1 composition
   root + the M4-C.1 unit tests + the M4-B
   closeout handoff + the M4-B retrospective
   § 13 Recommendations for the Next
   Milestone.
2. Creates the feature branch
   `feature/T-029-m4-c-2-app-provider-list-and-providers-page`
   from `main` at the M4-C.1 closeout commit.
3. Lands the `AppProviderList` data-owning
   four-state design-system component in
   `src/AiEng.Platform.App/Components/DesignSystem/`.
4. Lands the `/providers` page at
   `src/AiEng.Platform.App/Components/Pages/Providers.razor`
   (+ `.razor.css`).
5. Wires the startup provider-registry log in
   `src/AiEng.Platform.App/Program.cs`.
6. Lands the
   `Providers_Resolve_Through_Registry`
   architecture test in
   `tests/AiEng.Platform.ArchitectureTests/Providers/`
   (Active per the M4-C plan § 2 item 12;
   scoped to `App/Components/Providers/` to
   avoid the M4-A.2 Open Action false
   positive).
7. Lands the `docs/providers.md` 10-section
   documentation mirroring
   `docs/infrastructure.md` § 1-10.
8. Updates `docs/design-system.md` § 4.5
   (the M4-B.2 deferred decision resolves to
   `Implemented (M4-C.2)`).
9. Lands 4+ bUnit page tests + 1+ active
   architecture test.
10. Runs the validation gate (`dotnet format` +
    `dotnet build` + `dotnet test` + JSON +
    CRLF).
11. Updates the project-continuity state per
    Rule 15 (the 6 state files).
12. Writes the M4-C.2 per-session handoff +
    the M4-C.2 implementation report.
13. Coherent commit + fast-forward merge +
    delete feature branch; **skip push**.
14. **Stop.** M4-C.2 does **not** begin
    M4-C.x closeout / M4-D / provider
    creation.

The M4-C.2 first session follows the 13-step
Progressive Coding task lifecycle in the
order specified in
`.ai/workflows/progressive-coding.md` § 3.

## 8. Linked artefacts

- **The M4-C plan:**
  `.ai/plans/M4-C-provider-registry-foundation.md`
  (Status: Active; the M4-C plan is the
  canonical M4-C scope; the M4-C.1 first
  session is the M4-C plan's § 2 items 1-8 +
  § 10 Test Plan item 10.1).
- **The M4-B plan:**
  `.ai/plans/M4-B-capability-detection.md`
  (the M4-C plan mirrors the M4-B plan's 12
  sections with M4-C-specific evidence).
- **The M4-B closeout handoff:**
  `.ai/handoffs/2026-07-13-m4-b-closeout.md`
  (the M4-C.1 handoff's template; the
  M4-C.1 closeout obligations per § 7).
- **The M4-B closeout implementation
  report:** `implementation-report-m4-b-closeout.md`
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
  test's pattern with M4-C-specific
  evidence; the M4-C.2 test is scoped to
  `App/Components/Providers/` to avoid the
  M4-A.2 Open Action + M4-B.3
  `Diagnostics.razor` false positives).
- **The branching strategy:**
  `.ai/workflows/branching-strategy.md`
  (rules 4, 6, 7, 9).
- **The Progressive Coding Rule:**
  `.ai/workflows/progressive-coding.md`.
- **The command protocol:**
  `.ai/commands.md` (the `Next` command
  response shape — `Completed / Git /
  Validation / Evidence / Next`).
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
- **The M4-C.1 implementation report:**
  `implementation-report-m4-c-1-provider-registry-contract-and-family-registries.md`
  (15+ sections; mirrors the M4-B.1 +
  M4-B.2 + M4-B.3 + M4-B closeout reports).
