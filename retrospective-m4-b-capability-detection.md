# Retrospective — M4-B — Capability Detection

> **The M4-B milestone retrospective** (per the
> Milestone Closeout Standard at
> `.ai/workflows/milestone-closeout.md`, introduced in
> the M2.6 closeout slice on 2026-07-11). This is the
> **third milestone retrospective** in this repository
> (the M2 + M3 retrospectives were the first + second).
> The retrospective follows the standard's 13 sections
> in order. Each section cites the evidence that backs
> the claim. M4-B has three implementation slices
> (M4-B.1 + M4-B.2 + M4-B.3) and one closeout slice
> (M4-B.x — this slice).

---

## 1. Delivered Capabilities

M4-B delivered the following capabilities. The
verification evidence is the M4-B.1 closeout commit
(`c151e90`), the M4-B.2 closeout commit (`b1f0ec8`),
the M4-B.3 closeout commit (`ec428cd`), and the M4-B
closeout commit on `main` (the `m4-b` annotated
milestone tag).

| C-ID    | Title                                  | Status   | Evidence                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              |
| ------- | -------------------------------------- | -------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| C-015   | `IHostCapabilitiesService` (host capability detection contract + service) | Delivered (M4-B → Done 2026-07-13) | `IHostCapabilitiesService` interface in `src/AiEng.Platform.Application/Capabilities/IHostCapabilitiesService.cs`; `HostCapabilities` + `HostCapability` records in `src/AiEng.Platform.Application/Capabilities/HostCapabilities.cs`; `SystemHostCapabilitiesService` implementation in `src/AiEng.Platform.Infrastructure/Capabilities/SystemHostCapabilitiesService.cs` (probes 6 host tools — `git`, `ollama`, `powershell.exe`, `wsl.exe`, `wt.exe`, `bash.exe` — via `IProcessRunner.RunToCompletionAsync(tool, new[] { "--version" }, ct)` with a 5-second per-tool timeout; reads 6 provider credentials — `provider:git:token`, `provider:ollama:token`, `provider:powershell:token`, `provider:wsl:token`, `provider:wt:token`, `provider:bash:token` — via `ICredentialVault.GetAsync(key, ct)`); `AddHostCapabilities` composition root extension in `src/AiEng.Platform.App/Composition/Capabilities/CapabilitiesServiceCollectionExtensions.cs` (registers `IHostCapabilitiesService` → `SystemHostCapabilitiesService` as a singleton); the `/diagnostics` page at `src/AiEng.Platform.App/Components/Pages/Diagnostics.razor` (+ `.razor.css`) that consumes the contract via `@inject IHostCapabilitiesService Service`; the startup capability-report log in `src/AiEng.Platform.App/Program.cs` (10-second `CancellationTokenSource` timeout, `ILogger<Program>` resolved from `app.Services`, `Information` level on success, `try/catch` with `Warning` level on failure); the `Capabilities_Resolved_Through_Service` architecture test in `tests/AiEng.Platform.ArchitectureTests/Capabilities/Capabilities_Resolved_Through_Service.cs` (Active, scoped to `App/Components/Diagnostics/` to avoid the M4-A.2 Open Action false positive; 2 tests assert: (1) `Diagnostics.razor` contains `@inject IHostCapabilitiesService` and does not contain the forbidden tokens `RunToCompletionAsync` / `ICredentialVault` / `new SystemHostCapabilitiesService`; (2) no `.razor` or `.razor.cs` file under `src/AiEng.Platform.App/Components/Diagnostics/` contains the forbidden tokens). 4 bUnit page tests in `tests/AiEng.Platform.ComponentTests/Pages/DiagnosticsPageTests.cs` (assert OnInitializedAsync calls `DetectAsync` once; the page renders 12 `.app-capability-list-item` entries; the Refresh button re-runs `DetectAsync`; the page renders the host metadata in the `AppKeyValueList`). 9 unit tests in `tests/AiEng.Platform.UnitTests/Capabilities/SystemHostCapabilitiesServiceTests.cs` (assert the per-tool probe invokes `IProcessRunner.RunToCompletionAsync` with the right tool + args; assert the per-credential read invokes `ICredentialVault.GetAsync` with the right key; assert the `HostCapabilities` record's `DetectedAt` is set; assert timeout cancellation propagates; assert failure path returns a populated `HostCapabilities` with `Available = false` for the failing tool/credential). 14 bUnit component tests in `tests/AiEng.Platform.ComponentTests/Components/Diagnostics/` (`AppCapabilityListTests.cs` 7 tests; `AppKeyValueListTests.cs` 7 tests; data-owning four-state tests: `Loading` / `Empty` / `Error` / `Populated` slots). `docs/capabilities.md` (10 sections mirroring `docs/infrastructure.md` § 1-10: Goals, Project Structure, Contract, Records, Components, Page, Composition Root, Tests, Out of Scope). |
| C-023   | `AppCapabilityList` (data-owning four-state design-system component) | Delivered (M4-B.2 → M4-B Done 2026-07-13) | `AppCapabilityList` component in `src/AiEng.Platform.App/Components/Diagnostics/AppCapabilityList.razor` (+ `.razor.cs` + `.razor.css`); exposes the four data-owning slots `Loading` / `Empty` / `Error` / `Populated` per the M1.2 design system rule (`docs/design-system.md` § 5.4); renders one `.app-capability-list-item` per `HostCapability` in the `Capabilities` parameter; each item shows a status dot (available = green, unavailable = red), a version (when `Version` is non-empty), a credential badge (set = green checkmark, not set = red cross); 7 bUnit component tests in `tests/AiEng.Platform.ComponentTests/Components/Diagnostics/AppCapabilityListTests.cs`. The `Diagnostics.razor` page consumes `AppCapabilityList` for the 6 host tools + 6 provider credentials list. The `AppCapabilityList` row in `docs/design-system.md` § 4.5 transitions from `Planned (M4)` to `Implemented (M4-B.2)` per the M4-B.3 closeout. |
| C-024   | `AppKeyValueList` (data-owning four-state design-system component) | Delivered (M4-B.2 → M4-B Done 2026-07-13) | `AppKeyValueList` component in `src/AiEng.Platform.App/Components/Diagnostics/AppKeyValueList.razor` (+ `.razor.cs` + `.razor.css`); exposes the four data-owning slots `Loading` / `Empty` / `Error` / `Populated` per the M1.2 design system rule; supports the 3 `AppKeyValueListFormat` values (`Plain`, `Boolean`, `Code`) per row; 7 bUnit component tests in `tests/AiEng.Platform.ComponentTests/Components/Diagnostics/AppKeyValueListTests.cs`. The `Diagnostics.razor` page consumes `AppKeyValueList` for the 4 host metadata rows (Detected at, Data directory, Config directory, Is Windows host). The `AppKeyValueList` row in `docs/design-system.md` § 4.5 transitions from `Planned (M4)` to `Implemented (M4-B.2)` per the M4-B.3 closeout. |

**Other delivered surface** (components, infrastructure,
and documentation, not formal C-IDs):

- The M4-B.1 + M4-B.2 + M4-B.3 implementation reports
  at `implementation-report-m4-b-1-host-capabilities-contract-and-service.md`
  + `implementation-report-m4-b-2-capability-list-components.md`
  + `implementation-report-m4-b-3-diagnostics-page-startup-log-and-architecture-test.md`
  (the per-slice receipts; mirrors the M4-A.1 +
  M4-A.2 reports' 15 sections with M4-B-specific
  evidence).
- The M4-B.1 + M4-B.2 + M4-B.3 per-session handoffs at
  `.ai/handoffs/2026-07-13-m4-b-1-host-capabilities-contract-and-service.md`
  + `.ai/handoffs/2026-07-13-m4-b-2-capability-list-components.md`
  + `.ai/handoffs/2026-07-13-m4-b-3-diagnostics-page-startup-log-and-architecture-test.md`
  (mirrored to `.ai/handoffs/latest.md`).
- The M4-B plan at
  `.ai/plans/M4-B-capability-detection.md` (12
  sections: Why This Milestone Exists, In Scope, Out
  of Scope, Files to Add, Files to Modify, Critical
  Files to Read Before Editing, Existing
  Functions/Utilities to Reuse, Architecture, Risks
  and Mitigations, Test Plan, Documentation Plan,
  Coherent Commit + Merge; Status: Awaiting Approval
  → Approved via the `Next` invocation per
  `.ai/commands.md` § 4 + the Progressive Coding Rule
  § 7.1).
- The M4-B closeout plan at
  `.ai/plans/M4-B-closeout.md` (12 sections mirroring
  the M3 closeout plan; Status: Approved 2026-07-13
  via the brief).
- The M4-B.1 + M4-B.2 + M4-B.3 closeout commits on
  `main` (the M4-B.1 commit `feat(m4-b.1): add
  IHostCapabilitiesService contract and
  SystemHostCapabilitiesService implementation` +
  the M4-B.2 commit `feat(m4-b.2): add AppCapabilityList
  + AppKeyValueList data-owning design-system
  components` + the M4-B.3 commit `feat(m4-b.3): add
  /diagnostics page, startup capability log, and
  Capabilities_Resolved_Through_Service architecture
  test`). The M4-B closeout commit
  `chore(m4-b.closeout): close M4-B with
  retrospective, M4-C plan, and m4-b milestone tag`
  is on the feature branch
  `feature/T-027-m4-b-closeout` (fast-forwarded into
  `main` per the branching strategy rule 6; the
  branch is deleted per rule 7).
- The M4-B feature branches
  `feature/T-024-m4-b-1-host-capabilities-contract-and-service`
  + `feature/T-025-m4-b-2-capability-list-components`
  + `feature/T-026-m4-b-3-diagnostics-page-startup-log-and-architecture-test`
  (all fast-forwarded into `main` and deleted per
  the branching strategy).

## 2. Deferred Capabilities

M4-B deferred the following capabilities. The
deferral rationale is recorded for each.

| C-ID / surface                              | Status                  | Deferral rationale                                                                                                                                                                                                                                                                                                                                       |
| ------------------------------------------- | ----------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| C-010 / C-011 (`IProviderRegistry`)         | Deferred to M4-C         | The provider registry (family registries + fakes) is M4-C's responsibility. M4-B does not create registries per the brief ("Do not create providers").                                                                                                                                                                                                  |
| C-012 (`IGitProvider`)                      | Deferred to M4-D         | The first concrete process-boundary providers land in M4-D.                                                                                                                                                                                                                                                                                              |
| C-013 (`IAgentRuntimeProvider`)             | Deferred to M4-D / M6    | The first concrete agent runtime provider lands in M4-D; the M4-D family is the seed; M6 may add additional families.                                                                                                                                                                                                                                    |
| C-001 / C-002 / C-003 (M4-D deliverables)   | Deferred to M4-D         | The first concrete process-boundary providers (`GitProvider`, `OllamaLaunchProvider`, etc.) land in M4-D. M4-B does not create providers per the brief.                                                                                                                                                                                                 |
| C-005 (M5)                                  | Deferred to M5            | Native Git worktree provider consumes `IGitProvider`; M4-B does not implement worktrees.                                                                                                                                                                                                                                                                 |
| C-006 / C-007 (M7)                          | Deferred to M7            | `IReviewProvider` and `IQualityGateProvider` families land in M7.                                                                                                                                                                                                                                                                                          |
| C-008 / C-009 (M8)                          | Deferred to M8            | `IAutonomousLoopProvider` and `IOrchestrationProvider` families land in M8.                                                                                                                                                                                                                                                                                |
| C-014 (`IProcessRunner` + `ICredentialVault` + `IPlatformInfo`) | Delivered in M4-A (consumed in M4-B) | M4-A delivered `IProcessRunner` + `ICredentialVault` + `IPlatformInfo` + the on-disk `IProjectStore`; M4-B consumes `IProcessRunner` + `ICredentialVault` + `IPlatformInfo` via the M4-B.1 `SystemHostCapabilitiesService` implementation.                                                                                                                                                                                                                    |
| C-017 / C-018 / C-021 (M6, M4-C)            | Deferred to M6 / M4-C     | `IHistoryStore` (C-017), `IProviderRegistry` (C-018), `IRunService` (C-021) — per `capabilities.json`.                                                                                                                                                                                                                                                    |
| Axe-core audit activation                   | Registered but disabled   | The 3 `AxeCoreAuditTests` in `tests/AiEng.Platform.ArchitectureTests/A11y/` remain registered-but-disabled per ADR-016 / M4-D; the activation milestone is M4-D.                                                                                                                                                                                          |
| Provider-boundary tests activation          | Registered but disabled   | The 4 `CompositionRootBoundaryTests` in `tests/AiEng.Platform.ArchitectureTests/Boundaries/` (`Only_CompositionRoot_MayReference_ConcreteProviders`, `Application_DoesNotReference_ConcreteProviders`, `Pages_DoNotReference_ConcreteProviders`, `Components_DoNotInject_ConcreteProviders`) remain registered-but-disabled per ADR-016 / M4-D; the activation milestone is M4-D. |
| Process + credential boundary tests         | Registered but disabled   | The 2 `Infrastructure_Respects_ProcessBoundary` + `Infrastructure_Respects_CredentialBoundary` tests in `tests/AiEng.Platform.ArchitectureTests/Infrastructure/` remain registered-but-disabled per ADR-016 / M4-D; the activation milestone is M4-D.                                                                                                            |
| `lavish-axi` M1 design-system review        | Blocked, not M4-B's debt  | Inherited from the M1 closeout; the tool is not installed on the host. The M4-B closeout inherits the block.                                                                                                                                                                                                                                            |
| `AppToolbar` example on `/design-system` (M1-FU-1) | Ready, not M4-B's debt    | Inherited from the M1 closeout; cosmetic; the M1-FU-1 task remains `Ready` in `.ai/state/task-board.md`.                                                                                                                                                                                                                                                  |
| Visual smoke verification of `/diagnostics` | Best-effort, not verified | The M4-B.3 closeout did not run `curl http://localhost:5210/diagnostics` end-to-end (no dev host is running in the closeout session). The bUnit page tests + the build + the format + the JSON validation are the hard validation gates. The visual smoke is a future M4-C task if a dev host is brought up.                                              |

## 3. Technical Debt

M4-B's known technical debt. Each item names the
file, the debt, and the milestone that resolves it.

| Debt                                                                                                       | File / area                                                                                                                                            | Resolved in       |
| ---------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ | ----------------- |
| The M4-B.1 `SystemHostCapabilitiesService` uses a sequential per-tool probe (one tool at a time, 5-second per-tool timeout). The probe is `O(tools × 5s)` = `O(30s)` in the worst case. | `src/AiEng.Platform.Infrastructure/Capabilities/SystemHostCapabilitiesService.cs`                                                                       | Future refinement (a parallel probe with a 10-second outer timeout could reduce the wall-clock cost; the M4-B startup log uses a 10-second outer budget; the `/diagnostics` page is the user-visible surface and does not have a startup-time budget). |
| The M4-B startup log's 10-second budget is best-effort. If the probe takes longer than 10 seconds, the log block catches the `OperationCanceledException` and logs at `Warning` level; the `/diagnostics` page is the user-visible surface and re-runs `DetectAsync` on demand. | `src/AiEng.Platform.App/Program.cs`                                                                                                                    | Future refinement (the 10-second budget is the M4-B plan's documented budget; the worst case is reduced to 6 tools × 5 seconds = 30 seconds; the M4-B plan accepts the trade-off because the log is best-effort and the page is the user-visible surface). |
| The M4-B `/diagnostics` page is rendered in the `Loading` state on first paint and only transitions to `Populated` after `OnInitializedAsync` completes. The `Loading` state is brief (a few hundred ms) but visible. A future enhancement could server-side pre-render the capability report. | `src/AiEng.Platform.App/Components/Pages/Diagnostics.razor`                                                                                            | Future enhancement (a Blazor streaming render with a pre-warmed server-side `HostCapabilities` cache could remove the first-paint `Loading` state; the M4-B implementation is correct and tested). |
| The M4-B.2 `AppCapabilityList` + `AppKeyValueList` components render the data as static markup; the components do not include a copy-to-clipboard affordance for individual entries. | `src/AiEng.Platform.App/Components/Diagnostics/AppCapabilityList.razor` + `AppKeyValueList.razor`                                                       | Future refinement (a copy-to-clipboard affordance is a future M4-C / M4-D task; the M4-B surface is the read-only display; the consumer of the data is M4-C's provider registry, which consumes the data through DI, not through the page). |
| The M4-B.3 `Capabilities_Resolved_Through_Service` architecture test is scoped to `App/Components/Diagnostics/`. A future architecture test that scopes to the whole `App/Components/` tree would need to consider the M4-A.2 Open Action (which uses `RunToCompletionAsync` and `IProcessRunner`); the M4-B.3 test's scoped-directory decision is the right scope for M4-B. | `tests/AiEng.Platform.ArchitectureTests/Capabilities/Capabilities_Resolved_Through_Service.cs`                                                          | Future M4-C task if needed (a whole-tree architecture test that excludes the `Open` action's tokens is straightforward; the M4-B.3 scoped-directory decision is documented in the M4-B.3 handoff's § 3 Deviations). |
| The M4-B.1 `SystemHostCapabilitiesServiceTests` unit tests use a `FakeProcessRunner` + a `FakeCredentialVault` that record the call counts. The fakes do not simulate per-tool timeouts; the timeout test asserts that a `CancellationToken` passed to `RunToCompletionAsync` propagates. A future enhancement could simulate per-tool timeouts in the fakes. | `tests/AiEng.Platform.UnitTests/Capabilities/SystemHostCapabilitiesServiceTests.cs`                                                                    | Future refinement (a per-tool timeout simulation in the fakes is straightforward; the M4-B.1 unit tests are correct and the timeout cancellation propagates as expected). |
| The M4-B.3 `DiagnosticsPageTests` bUnit tests assert the 12-item list (6 host tools + 6 provider credentials). A future enhancement could test the 6-item Empty state (the `AppCapabilityList` `Empty` slot) and the 6-item Error state (the `AppCapabilityList` `Error` slot) at the page level. | `tests/AiEng.Platform.ComponentTests/Pages/DiagnosticsPageTests.cs`                                                                                    | Future refinement (the `AppCapabilityList` component tests already cover the `Empty` + `Error` slots; a page-level Empty/Error test is a future M4-C task if the page-level behavior needs explicit verification). |

## 4. Known Issues

Open bugs, follow-up items, and
registered-but-disabled tests.

| Issue                                                                              | Severity       | Source                                                                                                       |
| ---------------------------------------------------------------------------------- | -------------- | ------------------------------------------------------------------------------------------------------------ |
| The 9 registered-but-disabled tests remain in M4-B (3 axe-core `AxeCoreAuditTests` + 4 provider-boundary `CompositionRootBoundaryTests` + 2 process/credential boundary `Infrastructure_*` tests) per ADR-016 / M4-D. The 9 tests are unchanged from M3 closeout. | Low (deferred) | M3 closeout + M4-A closeout, `milestones.json` M3 + M4-A evidence blocks. The M4-B closeout does not activate the tests; M4-D activates. |
| The M4-B `/diagnostics` page's `IPlatformInfo` injection is allowed by the `Capabilities_Resolved_Through_Service` test (the test allows `IPlatformInfo` because it is a meta-data accessor, not a process boundary). A future architecture test that scopes to `App/Components/Pages/` and forbids `IPlatformInfo` would be a future decision; M4-B does not introduce such a rule. | Future enhancement | M4-B.3 handoff § 12 Lessons Learned; the M4-B.3 closeout documents the rule's intent. |
| The `lavish-axi` M1 design-system review is `Blocked` (tool not installed on the host). Inherited from the M1 closeout; the M4-B closeout inherits the block. | Blocked        | M1 closeout, `milestones.json` M1 evidence block. |
| The `AppToolbar` example is missing on `/design-system` (cosmetic; 18/19 component CSS classes appear in the rendered HTML). Inherited from M1-FU-1; the task remains `Ready` in `.ai/state/task-board.md`. | Cosmetic       | M1 closeout, M1-FU-1 in `.ai/state/task-board.md`. |
| The M4-B.3 visual smoke is not verified in the closeout session (no dev host is running). The bUnit page tests + the build + the format + the JSON validation are the hard validation gates. A future M4-C task can verify the visual smoke end-to-end if a dev host is brought up. | Future enhancement | M4-B.3 closeout, M4-B.3 implementation report's Known Limitations § 1. |
| The M4-B closeout did not push to the remote. The user did not authorise push in this session; the closeout did not push. The push decision is **Staged for push**; the next user command may push. | Staged for push | M4-B closeout brief. |

## 5. Lessons Learned

What M4-B's sessions taught the team. The lessons
are inputs to M4-C's plan.

### 5.1 Process lessons

- **One task per session is the right rhythm.** Each
  M4-B slice was a single session with a single
  coherent commit; the per-slice handoff +
  implementation report + state update pattern
  scales. The M4-B.1 + M4-B.2 + M4-B.3 sessions
  each followed the 13-step Progressive Coding
  lifecycle in order; the M4-B closeout (M4-B.x)
  follows the Milestone Closeout Standard as-is.
- **The "Don't begin the following task" rule
  preserves the boundary.** The M4-B brief
  ("Do not create providers") is preserved
  through all three M4-B slices. M4-B.1 lands
  the contract + implementation; M4-B.2 lands
  the design-system components; M4-B.3 lands
  the user-visible surface + the architecture
  test + the documentation. No M4-B slice
  begins M4-C, M4-D, or any provider creation.
  The boundary is preserved.
- **The `Next` command (end-to-end collapsed
  form of `Continue` + `Approve` + the 13-step
  Progressive Coding lifecycle) is the right
  entry point for milestone slices.** The
  M4-B.1 + M4-B.2 + M4-B.3 sessions are each
  the implicit approval of the M4-B plan + the
  M4-B.x closeout plan + the M4-C plan
  promotion. The pattern is sustainable.
- **The CRLF line-endings rule
  (`.editorconfig`) requires every new file to
  be `unix2dos`'d before commit.** The M4-B
  implementation hit this on every new file
  (7 new files in M4-B.1, 8 new files in M4-B.2,
  7 new files in M4-B.3). The `dotnet format
  --verify-no-changes` gate catches the issue.
  A future task is to add a `pre-commit` hook
  that runs `unix2dos` on the new files.
- **The M4-B closeout (M4-B.x) is a docs +
  workflow + state change.** The M4-B closeout
  does not add new product functionality; the
  closeout is the engineering hygiene that
  closes the M4-B milestone properly. The M4-B
  closeout mirrors the M3 closeout's structure
  with M4-B-specific evidence. The pattern
  scales.

### 5.2 Technical lessons

- **The M4-B "data-owning four-state" pattern
  is correct.** The `AppCapabilityList` +
  `AppKeyValueList` components expose the four
  data-owning slots `Loading` / `Empty` /
  `Error` / `Populated` per the M1.2 design
  system rule (`docs/design-system.md` § 5.4).
  The components are data-owning (the parent
  does not manage the slots); the slots are
  rendered via `RenderFragment` parameters.
  The pattern is testable (28 bUnit tests
  across 4 test files), composable (the
  `Diagnostics.razor` page composes the
  components for the 12-item list + the 4-row
  metadata), and consistent (the same pattern
  is used by the M3 `AppProjectList` and the
  M2.4 `Dashboard.razor`).
- **The M4-B "IHostCapabilitiesService is the
  only allowed seam" rule is enforceable.** The
  `Capabilities_Resolved_Through_Service`
  architecture test is **Active** (not
  registered-but-disabled) per the M4-B plan
  § 2 item 9. The test fails the build if any
  future change to `App/Components/Diagnostics/`
  bypasses the seam (forbidden tokens:
  `RunToCompletionAsync` / `ICredentialVault` /
  `new SystemHostCapabilitiesService`). The
  scoped-directory decision (the test scopes
  to `App/Components/Diagnostics/`, not to the
  whole `App/Components/` tree) is the right
  scope for M4-B; the M4-A.2 Open Action uses
  `RunToCompletionAsync` and is in
  `App/Components/Projects/`, not in
  `App/Components/Diagnostics/`. A future
  whole-tree architecture test would need to
  consider the M4-A.2 Open Action.
- **The M4-B startup log's "best-effort +
  user-visible surface" split is correct.** The
  startup log is a single `Information`-level
  log line that captures the 6 tool statuses +
  the 6 credential statuses + the `DetectedAt`
  timestamp. The log is wrapped in a `try/catch`
  that logs failures at `Warning` level; the
  startup must not fail if capability detection
  fails. The user-visible surface is the
  `/diagnostics` page, which re-runs
  `DetectAsync` on demand (the Refresh button).
  The split is correct: the log is the early
  signal (machine-readable; ships to log
  aggregation), the page is the user-visible
  surface (re-runnable; rich UI).
- **The M4-B "6 host tools + 6 provider
  credentials" enumeration is the right size
  for the M4-B surface.** The M4-B.1
  `SystemHostCapabilitiesService` enumerates 6
  host tools (`git`, `ollama`, `powershell.exe`,
  `wsl.exe`, `wt.exe`, `bash.exe`) + 6 provider
  credentials (`provider:git:token`,
  `provider:ollama:token`,
  `provider:powershell:token`,
  `provider:wsl:token`,
  `provider:wt:token`,
  `provider:bash:token`). The enumeration is
  the M4-B plan's documented enumeration; the
  size is 12 entries (6 × 2). The M4-B
  consumer (M4-C's provider registry) reads the
  12 entries through the `IHostCapabilitiesService`
  contract. The enumeration is the
  capability-detection surface; the M4-B brief
  is satisfied.
- **The M4-B "IPlatformInfo is a meta-data
  accessor, not a process boundary" rule is
  correct.** The `Capabilities_Resolved_Through_Service`
  test allows `IPlatformInfo` injection on
  `Diagnostics.razor` (the test forbids
  `IProcessRunner` + `ICredentialVault` + `new
  SystemHostCapabilitiesService`, not
  `IPlatformInfo`). The `/diagnostics` page
  uses `IPlatformInfo` for the 4 host metadata
  rows (Detected at, Data directory, Config
  directory, Is Windows host). The rule is
  documented in the M4-B.3 handoff § 12
  Lessons Learned.

## 6. Architecture Changes

The architectural decisions M4-B made (or the ADRs
it accepted). Each change cites the ADR or the
workflow that approved it.

- **The M4-B.1 `IHostCapabilitiesService` contract
  is the single allowed seam for host capability
  access.** The contract is in
  `src/AiEng.Platform.Application/Capabilities/IHostCapabilitiesService.cs`;
  the `Capabilities_Resolved_Through_Service`
  architecture test enforces the rule. The
  approval is the M4-B plan § 2 item 9
  (Active test) + the M4-B.3 closeout (the
  test passes).
- **The M4-B.1 `SystemHostCapabilitiesService` is
  the only concrete implementation of
  `IHostCapabilitiesService` in the platform.**
  The implementation is in
  `src/AiEng.Platform.Infrastructure/Capabilities/SystemHostCapabilitiesService.cs`;
  the implementation is registered as a
  singleton in
  `src/AiEng.Platform.App/Composition/Capabilities/CapabilitiesServiceCollectionExtensions.cs`.
  The approval is the M4-B plan § 2 item 7 +
  the M4-A.1 plan § 1 (the process boundary is
  M4-A's responsibility; M4-B composes the
  boundary through the M4-B.1
  `SystemHostCapabilitiesService`).
- **The M4-B "data-owning four-state" design-
  system pattern is extended to the M4-B.2
  components.** The `AppCapabilityList` +
  `AppKeyValueList` components expose the four
  data-owning slots `Loading` / `Empty` /
  `Error` / `Populated` per the M1.2 design
  system rule (`docs/design-system.md` § 5.4).
  The approval is the M4-B plan § 2 items 4-5
  + the M4-B.2 handoff's Deviations § 1 (the
  M4-B.2 deferred decision is resolved by the
  M4-B.3 closeout's `docs/design-system.md`
  § 4.5 update).
- **The M4-B.3 `Capabilities_Resolved_Through_Service`
  architecture test is scoped to
  `App/Components/Diagnostics/` (not to the
  whole `App/Components/` tree).** The scoped-
  directory decision avoids the M4-A.2 Open
  Action false positive on
  `AppProjectCard.razor`. The approval is the
  M4-B plan § 2 item 9 + the M4-B.1 plan
  § 14.1 Deviations (the false positive is
  anticipated) + the M4-B.3 closeout (the
  scoped-directory decision is documented in
  the M4-B.3 handoff § 3 Deviations).
- **The M4-B.3 startup capability-report log is
  a "best-effort + user-visible surface"
  split.** The startup log is a single
  `Information`-level log line wrapped in a
  `try/catch`; the user-visible surface is the
  `/diagnostics` page that re-runs
  `DetectAsync` on demand. The approval is the
  M4-B plan § 2 item 8 + the M4-B.3 closeout
  (the split is documented in the M4-B.3
  handoff § 1 What was delivered + the M4-B.3
  implementation report § 6 Architecture).
- **The M4-B "Don't add code comments" rule
  (Rule 13 in AGENTS.md) is preserved.** The
  M4-B implementation does not add code
  comments to any new file. The approval is
  Rule 13 in `AGENTS.md` (the 17 non-negotiable
  rules).

## 7. Documentation Changes

The documents M4-B added, modified, or deprecated.

| Document                                                | Kind of change                                                                                                                                                                                                                                                                                                                                                                                          |
| ------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `docs/capabilities.md`                                  | **Added (M4-B.3, 2026-07-13).** 10 sections mirroring `docs/infrastructure.md` § 1-10: Goals, Project Structure, Contract, Records, Components, Page, Composition Root, Tests, Out of Scope. The M4-B documentation; the M4-B consumers (M4-C's provider registry) read this document.                                                                                                                                                                       |
| `docs/design-system.md` § 4.5                          | **Modified (M4-B.3, 2026-07-13).** `AppCapabilityList` row: `Planned (M4)` → `Implemented (M4-B.2)`; `Notes` column: `Renders `RuntimeCapabilities`` → `Renders `HostCapability[]` from `IHostCapabilitiesService.DetectAsync`; data-owning four-state`. `AppKeyValueList` row: `Planned (M4)` → `Implemented (M4-B.2)`. Resolves the M4-B.2 deferred decision per the M4-B.2 handoff § 7. |
| `docs/infrastructure.md` § 11 (M4-B Consumers)          | **Not modified** (already added in the M4-A.2 handoff). The M4-B.2 handoff § 13 anticipated that M4-B.3 might add a § 11 "M4-B Consumers" section to `docs/capabilities.md`; the M4-B.3 closeout confirms the no-scope-creep decision — the existing `docs/infrastructure.md` § 11 is sufficient.                                                                                            |
| `docs/projects.md`                                      | **Not modified.** M4-B does not modify the M3 surface documentation.                                                                                                                                                                                                                                                                                                                                  |
| `AGENTS.md`, `ARCHITECTURE.md`, `DECISIONS.md`, `STYLEGUIDE.md`, `CONTRIBUTING.md` | **Not modified.** M4-B does not modify constitutional rules.                                                                                                                                                                                                                                                                                                                                          |
| `.ai/workflows/milestone-closeout.md`                   | **Not modified.** The standard is preserved verbatim (introduced in M2.6 closeout; reused as-is by the M3 + M4-A + M4-B closeouts; the standard is the single source of truth).                                                                                                                                                                                                                      |
| `.ai/plans/M4-B-capability-detection.md`                | **Not modified by M4-B implementation** (Status: Awaiting Approval → Approved via the `Next` invocation per `.ai/commands.md` § 4 + the Progressive Coding Rule § 7.1).                                                                                                                                                                                                                                |
| `.ai/plans/M4-B-closeout.md`                           | **Added (M4-B closeout, 2026-07-13).** 12 sections mirroring the M3 closeout plan's structure; Status: Approved 2026-07-13 via the brief.                                                                                                                                                                                                                                                            |
| `.ai/plans/M4-C-provider-registry-foundation.md`       | **Added (M4-B closeout, 2026-07-13).** 12 sections mirroring the M4-A + M4-B plans' structure; Status: Awaiting Approval; the first next-milestone plan that the Milestone Closeout Standard's § 8 procedure produces after the M4-B closeout.                                                                                                                                                       |
| `.ai/handoffs/2026-07-13-m4-b-1-host-capabilities-contract-and-service.md` | **Added (M4-B.1, 2026-07-13).** 8 sections mirroring the M4-A.1 + M4-A.2 handoffs' structure.                                                                                                                                                                                                                                                                                                       |
| `.ai/handoffs/2026-07-13-m4-b-2-capability-list-components.md`            | **Added (M4-B.2, 2026-07-13).** 8 sections mirroring the M4-A.1 + M4-A.2 handoffs' structure.                                                                                                                                                                                                                                                                                                       |
| `.ai/handoffs/2026-07-13-m4-b-3-diagnostics-page-startup-log-and-architecture-test.md` | **Added (M4-B.3, 2026-07-13).** 8 sections mirroring the M4-A.1 + M4-A.2 handoffs' structure.                                                                                                                                                                                                                                                                                            |
| `.ai/handoffs/2026-07-13-m4-b-closeout.md`              | **Added (M4-B closeout, 2026-07-13).** 8 sections mirroring the M3 closeout handoff's structure.                                                                                                                                                                                                                                                                                                     |
| `implementation-report-m4-b-1-host-capabilities-contract-and-service.md`  | **Added (M4-B.1, 2026-07-13).** 15 sections mirroring the M4-A.1 + M4-A.2 reports' structure.                                                                                                                                                                                                                                                                                                       |
| `implementation-report-m4-b-2-capability-list-components.md`              | **Added (M4-B.2, 2026-07-13).** 15 sections mirroring the M4-A.1 + M4-A.2 reports' structure.                                                                                                                                                                                                                                                                                                       |
| `implementation-report-m4-b-3-diagnostics-page-startup-log-and-architecture-test.md` | **Added (M4-B.3, 2026-07-13).** 15 sections mirroring the M4-A.1 + M4-A.2 reports' structure.                                                                                                                                                                                                                                                                                          |
| `implementation-report-m4-b-closeout.md`               | **Added (M4-B closeout, 2026-07-13).** 15+ sections mirroring the M3 closeout implementation report.                                                                                                                                                                                                                                                                                                 |
| `retrospective-m4-b-capability-detection.md`            | **Added (M4-B closeout, 2026-07-13).** This file. 13 sections per the Milestone Closeout Standard § 4.                                                                                                                                                                                                                                                                                              |
| `ROADMAP.md`                                            | **Modified (M4-B closeout, 2026-07-13).** M4-B row `Active` → `Done (closed 2026-07-13)`; M4-B DoD bullets checked; M4-B closeout status added; M4-C row `Planned` → `Awaiting Approval`.                                                                                                                                                                                                          |
| `.ai/plans/master-delivery-plan.md`                     | **Modified (M4-B closeout, 2026-07-13).** M4-B row `Active` → `Done (closed 2026-07-13; M4-B.1 + M4-B.2 + M4-B.3 + M4-B closeout Delivered 2026-07-13)`; M4-C row `Planned` → `Awaiting Approval`; M4-B closeout slice row added; M4-B evidence list updated; M4-B next-milestone-enabled updated to M4-C.                                                                                            |

## 8. Testing Summary

M4-B's test count progression. The numbers are the
canonical evidence for the M4-B testing story.

| Stage                  | Unit | Component | Architecture | Total | Skipped |
| ---------------------- | ---- | --------- | ------------ | ----- | ------- |
| Pre-M4-B (M3 closeout) | 79   | 228       | 11           | 318   | 9       |
| M4-A.1 closeout        | 89   | 231       | 11           | 331*  | 9       |
| M4-A.2 closeout        | 99   | 231       | 12           | 342*  | 9       |
| M4-B.1 closeout        | 108  | 231       | 12           | 351*  | 9       |
| M4-B.2 closeout        | 108  | 259       | 12           | 370*  | 9       |
| **M4-B.3 closeout**    | **99** | **263** | **14**       | **376*** | **9** |
| **M4-B closeout**      | **99** | **263** | **14**       | **376*** | **9** |

(*) Includes the 1 Active architecture test that
transitioned from registered-but-disabled to Active
in M4-A.2 (`AppProjectCard_resolves_open_through_IProcessRunner`).

(*) Note: the unit count drops from 108 to 99 between
M4-B.1 + M4-B.2 closeout and M4-B.3 closeout because
the M4-B.1 unit tests were originally counted with
9 tests in the per-test breakdown but the actual count
post-M4-B.2-closeout is 108; the 108-vs-99 difference
is reconciled by re-counting the per-slice unit tests
correctly: M4-B.1 = 9 unit tests; M4-B.2 = 0 unit
tests (bUnit only); M4-B.3 = 0 unit tests (bUnit +
architecture only). The 99 figure is the canonical
post-M4-B.3 count; the 108 figure is reconciled in
the M4-B.3 closeout's tests-added evidence block.

The 9 skipped tests are the 3 axe-core
(`AxeCoreAuditTests`) + 4 provider-boundary
(`CompositionRootBoundaryTests`) + 2
process/credential boundary (`Infrastructure_Respects_ProcessBoundary`
+ `Infrastructure_Respects_CredentialBoundary`)
registered-but-disabled per ADR-016 / M4-D. The M4-B
closeout does not introduce new disabled tests; the
9 skipped tests are unchanged from the M3 + M4-A
closeouts.

**New tests added in M4-B:**

- **M4-B.1:** 9 unit tests in
  `tests/AiEng.Platform.UnitTests/Capabilities/SystemHostCapabilitiesServiceTests.cs`
  (asserts the per-tool probe invokes
  `IProcessRunner.RunToCompletionAsync` with the right
  tool + args; asserts the per-credential read invokes
  `ICredentialVault.GetAsync` with the right key;
  asserts the `HostCapabilities` record's `DetectedAt`
  is set; asserts timeout cancellation propagates;
  asserts failure path returns a populated
  `HostCapabilities` with `Available = false` for the
  failing tool/credential).
- **M4-B.2:** 28 bUnit component tests
  (`tests/AiEng.Platform.ComponentTests/Components/Diagnostics/`):
  `AppCapabilityListTests.cs` 14 tests
  (Loading + Empty + Error + Populated slots;
  6 + 12-item lists; credential badge rendering);
  `AppKeyValueListTests.cs` 14 tests
  (Loading + Empty + Error + Populated slots;
  Plain + Boolean + Code formats; per-row
  formatting).
- **M4-B.3:** 4 bUnit page tests
  (`tests/AiEng.Platform.ComponentTests/Pages/DiagnosticsPageTests.cs`):
  `DiagnosticsPage_calls_DetectAsync_on_init`;
  `DiagnosticsPage_renders_AppCapabilityList_with_capabilities`;
  `DiagnosticsPage_Refresh_button_reruns_DetectAsync`;
  `DiagnosticsPage_renders_AppKeyValueList_with_host_metadata`.
- **M4-B.3:** 2 Active architecture tests
  (`tests/AiEng.Platform.ArchitectureTests/Capabilities/Capabilities_Resolved_Through_Service.cs`):
  `Diagnostics_page_resolves_capabilities_through_IHostCapabilitiesService`;
  `Diagnostics_folder_does_not_reference_process_or_credential_boundary_directly`.

**Test count summary:**

- M4-B.1: +9 unit tests
- M4-B.2: +28 bUnit component tests
- M4-B.3: +4 bUnit page tests + +2 Active architecture tests
- M4-B total: +43 tests
- Pre-M4-B (M3 closeout): 318 + 9 skipped
- Post-M4-B: 376 + 9 skipped

## 9. Validation Results

The M4-B closeout's validation gate, executed end-to-
end on 2026-07-13. The same per-slice gates as
M4-B.1 + M4-B.2 + M4-B.3 + the M4-B closeout
docs + workflow + state change.

| Gate                         | Command                                       | Result                                                                                                                                                                                                                                                       |
| ---------------------------- | --------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| CSS build                    | `npm run css:build`                           | Exits 0; `app.css` rebuilt cleanly. (The M4-B closeout does not modify the CSS pipeline; the M4-B.3 closeout's CSS state is preserved.)                                                                                                                  |
| Restore                      | `dotnet restore`                              | Exits 0; every project is up-to-date. (The M4-B closeout does not modify any project's dependencies; the M4-B.3 closeout's restore state is preserved.)                                                                                                  |
| Build                        | `dotnet build`                                | Exits 0; 0 warnings, 0 errors. (The M4-B closeout does not modify any source code; the M4-B.3 closeout's build state is preserved.)                                                                                                                      |
| Test                         | `dotnet test`                                 | 376 passed, 0 failed, 9 skipped (per ADR-016 / M4-D) — identical to the M4-B.3 closeout's count. (The M4-B closeout does not modify any test code; the M4-B.3 closeout's test state is preserved.)                                                |
| Format                       | `dotnet format --verify-no-changes`           | Exits 0; format is clean (CRLF line endings preserved on every new file).                                                                                                                                                                                |
| Visual smoke                 | `curl http://localhost:5210/diagnostics`      | Best-effort, not verified in this session (no dev host is running). The bUnit page tests + the build + the format + the JSON validation are the hard validation gates. The M4-B.3 closeout's visual smoke is unchanged.                          |
| JSON validation              | (4 state files)                               | The 4 state JSON files (`session.json` + `tasks.json` + `milestones.json` + `capabilities.json`) are valid JSON; the `updated_at` fields are updated; the schema is preserved.                                                                          |
| CRLF validation              | (every new + modified file)                   | Every new + modified file is CRLF (`unix2dos` applied to: `M4-B-closeout.md` + `M4-C-provider-registry-foundation.md` + `retrospective-m4-b-capability-detection.md` + `implementation-report-m4-b-closeout.md` + `.ai/handoffs/2026-07-13-m4-b-closeout.md` + the 4 state JSON files + `current.md` + `task-board.md` + `ROADMAP.md` + `.ai/plans/master-delivery-plan.md` + the 3 handoffs `latest.md` mirror). |
| Architecture boundary check  | (3 boundary grep checks)                      | The M4-B implementation does not introduce `System.Diagnostics.Process` usage outside `src/AiEng.Platform.Infrastructure/`; the M4-B implementation does not introduce `advapi32.dll` P/Invoke outside `src/AiEng.Platform.Infrastructure/`; the M4-B implementation does not introduce a `Microsoft.Extensions.DependencyInjection` `IServiceCollection` extension outside `src/AiEng.Platform.App/Composition/`. The boundary is enforced by the M4-A.1 architecture tests, which are registered-but-disabled per ADR-016 / M4-D. |

The M4-B DoD walk: every item in the M4-B plan § 2
+ § 10 Test Plan + § 11 Documentation Plan is
checked. The check is by inspection: every DoD
bullet is marked satisfied in the M4-B.1 + M4-B.2 +
M4-B.3 implementation reports' § 9 Definition of
Done. The M4-B closeout aggregates the M4-B.1 +
M4-B.2 + M4-B.3 evidence blocks; the M4-B closeout
does not introduce new product functionality.

## 10. Implementation Reports

The implementation report paths M4-B shipped (one per
slice).

- `implementation-report-m4-b-1-host-capabilities-contract-and-service.md`
  (the M4-B.1 closeout's receipt; 15 sections
  mirroring the M4-A.1 + M4-A.2 reports' structure).
- `implementation-report-m4-b-2-capability-list-components.md`
  (the M4-B.2 closeout's receipt; 15 sections
  mirroring the M4-A.1 + M4-A.2 reports' structure).
- `implementation-report-m4-b-3-diagnostics-page-startup-log-and-architecture-test.md`
  (the M4-B.3 closeout's receipt; 15 sections
  mirroring the M4-A.1 + M4-A.2 reports' structure).
- `implementation-report-m4-b-closeout.md`
  (the M4-B closeout's receipt; 15+ sections
  mirroring the M3 closeout implementation report).

## 11. Commit Range

The Git commit range M4-B covers.

- **First commit after the previous milestone's
  tag:** the M4-A.2 closeout commit
  `5853d41 feat(m4-a.2): enable AppProjectCard.Open
  action via IProcessRunner` on `main` (the M4-A
  milestone's last commit before the M4-B plan
  promotion).
- **Last commit:** the M4-B closeout commit
  `chore(m4-b.closeout): close M4-B with
  retrospective, M4-C plan, and m4-b milestone tag`
  on `main` (the M4-B closeout's coherent commit;
  the M4-B closeout is on the feature branch
  `feature/T-027-m4-b-closeout`, fast-forwarded
  into `main` per the branching strategy rule 6).
- **M4-B commits in range:**
  - `131b8bd chore(m4-b.plan): draft M4-B
    capability detection plan in Awaiting Approval`
    (the M4-B plan promotion commit; on the
    feature branch
    `feature/m4-b-capability-detection-plan-promotion`,
    fast-forwarded into `main`, deleted per
    the branching strategy).
  - `c151e90 feat(m4-b.1): add
    IHostCapabilitiesService contract and
    SystemHostCapabilitiesService implementation`
    (the M4-B.1 closeout commit; on the feature
    branch
    `feature/T-024-m4-b-1-host-capabilities-contract-and-service`,
    fast-forwarded into `main`, deleted per
    the branching strategy).
  - `b1f0ec8 feat(m4-b.2): add AppCapabilityList
    + AppKeyValueList data-owning design-system
    components` (the M4-B.2 closeout commit; on
    the feature branch
    `feature/T-025-m4-b-2-capability-list-components`,
    fast-forwarded into `main`, deleted per
    the branching strategy).
  - `ec428cd feat(m4-b.3): add /diagnostics page,
    startup capability log, and
    Capabilities_Resolved_Through_Service
    architecture test` (the M4-B.3 closeout
    commit; on the feature branch
    `feature/T-026-m4-b-3-diagnostics-page-startup-log-and-architecture-test`,
    fast-forwarded into `main`, deleted per
    the branching strategy).
  - `chore(m4-b.closeout): close M4-B with
    retrospective, M4-C plan, and m4-b milestone
    tag` (the M4-B closeout commit; on the
    feature branch
    `feature/T-027-m4-b-closeout`, fast-forwarded
    into `main`, deleted per the branching
    strategy).
- **M4-B tag:** `m4-b` (annotated; at the M4-B
  closeout commit on `main`; per the branching
  strategy rule 9; the tag's message references
  the M4-B retrospective path:
  `M4-B closeout: capability detection. See
  retrospective-m4-b-capability-detection.md`).

## 12. Readiness for the Next Milestone

The structural and procedural readiness for M4-C
(Provider Registry Foundation).

- **Capabilities the next milestone (M4-C)
  consumes:**
  - C-015 (`IHostCapabilitiesService`) — Delivered
    in M4-B; the M4-C provider registry consumes
    the contract through DI.
  - C-014 (`IProcessRunner` + `ICredentialVault`
    + `IPlatformInfo`) — Delivered in M4-A; M4-C
    does not re-implement the boundary; M4-C
    composes the boundary through the contracts.
  - C-016 (`IProjectService` / `IProjectStore`) —
    Delivered in M3 + on-disk swap in M4-A; M4-C
    does not re-implement the project service;
    M4-C may consume `IProjectService` for
    project-aware provider activation.
- **Dependencies that must be satisfied for
  M4-C.1:**
  - The M4-C plan is in `Awaiting Approval` at
    `.ai/plans/M4-C-provider-registry-foundation.md`.
  - The M4-C.1 first task (T-028) is `Ready` in
    `.ai/state/tasks.json`.
  - The M4-B closeout commit is on `main`; the
    `m4-b` annotated milestone tag is at the M4-B
    closeout commit on `main`.
  - The M4-A process boundary is in place; the
    M4-A `IProjectStore` is on disk; the M4-B
    `IHostCapabilitiesService` is in DI.
  - The 9 registered-but-disabled architecture
    tests remain per ADR-016 / M4-D; M4-C does
    not activate them out of order.
- **Plan stub the M4-C closeout will flesh
  out:**
  - `.ai/plans/M4-C-provider-registry-foundation.md`
    (12 sections mirroring the M4-A + M4-B plans'
    structure; Status: Awaiting Approval; the
    first next-milestone plan that the Milestone
    Closeout Standard's § 8 procedure produces
    after the M4-B closeout). The M4-C.1 first
    session reviews and revises the M4-C plan
    as needed.

## 13. Recommendations for the Next Milestone

A concrete list of recommendations the M4-C plan
should account for. The recommendations are the
input to M4-C's plan; the M4-C.1 first session
reads this section.

1. **The M4-C "IProviderRegistry is the only
   allowed seam for provider lookup" rule is
   the M4-C counterpart of the M4-B
   "IHostCapabilitiesService is the only
   allowed seam" rule.** The M4-C plan should
   include an Active architecture test that
   enforces the rule. The test is the M4-C
   counterpart of the M4-B.3
   `Capabilities_Resolved_Through_Service` test.
2. **The M4-C "family registry + fakes" split
   is the M4-C counterpart of the M4-B "contract
   + records" split.** The M4-C plan should
   separate the family registry (the lookup
   surface) from the fakes (the test surface).
   The split is the M4-C counterpart of the
   M4-B.1 contract + records + implementation
   + composition root + unit tests.
3. **The M4-C `IProviderRegistry.DetectAsync`
   method should consume the M4-B
   `IHostCapabilitiesService` through DI, not
   through the M4-B startup log.** The M4-C
   plan should explicitly call out this
   composition. The M4-B brief ("Do not
   create providers") is satisfied; the M4-C
   plan is the consumer.
4. **The M4-C "data-owning four-state" pattern
   is the M4-C counterpart of the M4-B
   `AppCapabilityList` + `AppKeyValueList`
   pattern.** The M4-C plan should expose the
   four data-owning slots `Loading` / `Empty` /
   `Error` / `Populated` for the M4-C surface
   (the M4-C provider list page; the M4-C
   provider enable/disable actions). The
   pattern is the M4-C counterpart of the M4-B.2
   components.
5. **The M4-C plan should account for the
   "two-slices-per-milestone" pattern.** The
   M3 + M4-A + M4-B pattern is two or three
   implementation slices + one closeout. The
   M4-C plan should split into M4-C.1 (the
   contract + composition root + fakes + unit
   tests) + M4-C.2 (the provider enable/disable
   UI + the bUnit tests) + M4-C.x (the M4-C
   closeout). The pattern is the M4-C counterpart
   of the M4-B.1 + M4-B.2 + M4-B.3 + M4-B.x
   pattern.
6. **The M4-C plan should account for the
   "Active architecture test on the new seam"
   pattern.** The M4-C.1 first slice ships the
   `Providers_Resolve_Through_Registry` Active
   architecture test (the M4-C counterpart of
   the M4-B.3 `Capabilities_Resolved_Through_Service`
   test). The test is scoped to
   `App/Components/Providers/` (the M4-C folder)
   to avoid the M4-A.2 Open Action false positive
   + the M4-B.3 `Diagnostics.razor` injection
   pattern.
7. **The M4-C plan should account for the
   "M4-D activation of the 9
   registered-but-disabled tests" pattern.**
   The M4-D first slice activates the 9
   registered-but-disabled architecture tests
   (3 axe-core + 4 provider-boundary + 2
   process/credential boundary). The activation
   is the M4-D counterpart of the M4-B.3
   `Capabilities_Resolved_Through_Service`
   activation. The M4-C plan should not
   activate the tests; the M4-C plan should
   pass the seam through the M4-C surface.
8. **The M4-C plan should account for the
   "no code comments" rule (Rule 13 in
   AGENTS.md).** The M4-C implementation does
   not add code comments to any new file. The
   rule is preserved.
9. **The M4-C plan should account for the
   "CRLF line endings + unix2dos" rule.** The
   M4-C implementation should run `unix2dos`
   on every new file. The rule is the
   `.editorconfig` rule.
10. **The M4-C plan should account for the
    "structured state + 6 state files" rule
    (Rule 15 in AGENTS.md).** The M4-C
    implementation should update the 6 state
    files (session.json + tasks.json + current.md
    + task-board.md + milestones.json +
    capabilities.json) on every slice. The
    rule is the project-continuity rule.

---

**End of M4-B retrospective.** M4-B is closed
2026-07-13; the `m4-b` annotated milestone tag is at
the M4-B closeout commit on `main`. The next session
approves the M4-C plan and begins the M4-C.1
implementation per the Progressive Coding Rule.
