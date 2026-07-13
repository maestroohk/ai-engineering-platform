# M4-B — Capability Detection

> **The M4-B plan.** M4-B introduces the platform's
> **host capability detection**: at host startup (and
> on demand from the `/diagnostics` page) the platform
> probes the host's external tools (`git`, `ollama`,
> `powershell.exe`, `wsl.exe`, `wt.exe`, `bash.exe`)
> through the M4-A.1 `IProcessRunner` contract, and
> reads provider credentials through the M4-A.1
> `ICredentialVault` contract. The probes produce a
> typed `HostCapabilities` report. The report is
> surfaced through the design-system `AppCapabilityList`
> + `AppKeyValueList` components on the `/diagnostics`
> page, and is logged at startup (Information level).
>
> M4-B is the **first consumer of `IProcessRunner` +
> `ICredentialVault` outside the M4-A.2 Open Action**.
> The M4-A.2 Open Action is the one existing seam
> activation; M4-B adds the second and third seam
> activations (six host-tool probes + provider
> credential reads).
>
> M4-B does not implement any `Providers.<X>` project
> or any family registry. M4-C introduces the
> family registries and the fakes; M4-D replaces the
> fakes with real process-boundary providers and
> activates the four registered-but-disabled
> composition-root architecture tests per ADR-016.
>
> **Status:** Awaiting Approval (2026-07-13; the
> M4-B plan promotion is the M4-A.2 closeout's
> "next concrete step"; the plan is produced by
> the M4-B plan promotion session and committed
> on the feature branch
> `feature/m4-b-capability-detection-plan-promotion`).
> The M4-B plan is approved implicitly on the user's
> next `Next` invocation per `.ai/commands.md` § 4
> and the Progressive Coding Rule § 7.1; the M4-B
> implementation begins in a future session.
>
> **Branch:** (the M4-B.1 branch is created from
> `main` at the M4-B plan promotion commit when
> M4-B.1 starts; the branch is named
> `feature/T-023-m4-b-1-host-capabilities-contract-and-service`
> per the branching strategy rule 4).

---

## 1. Why This Milestone Exists

M4-A shipped the infrastructure seam every later
milestone composes: the `AiEng.Platform.Infrastructure`
csproj; the four contracts (`IProcessRunner`,
`ICredentialVault`, `IPlatformInfo`, the on-disk
`IProjectStore`); the four implementations
(`SystemProcessRunner`, `WindowsCredentialVault`,
`SystemPlatformInfo`, `JsonFileProjectStore`); the
`AddInfrastructure` composition root; the M3
in-memory store swap for the on-disk store; the
M4-A.2 Open action on `AppProjectCard` (the first
`IProcessRunner` activation; the only consumer of
`IProcessRunner` outside the Infrastructure project
as of M4-A.2 closeout).

To introduce the family registry in M4-C and the
first concrete providers in M4-D, the platform
needs to **know which external tools are installed
on the host**. A `GitProvider` cannot register
itself if `git` is not on `PATH`. An
`OllamaLaunchProvider` cannot register itself if
`ollama` is not on `PATH`. A `WT` terminal
provider cannot register itself if the user is on
Windows 10 and `wt.exe` does not exist (Windows 11
only). The provider registry in M4-C must consult
a **capability report** to decide which providers
are eligible for enablement; the capability report
is M4-B's deliverable.

M4-B ships:

- `IHostCapabilitiesService` — the contract that
  every later consumer of host capabilities uses
  (the M4-C provider registry; the `/diagnostics`
  page; the startup capability-report log). The
  contract exposes a single method,
  `DetectAsync(CancellationToken)`, returning a
  typed `HostCapabilities` record.
- `HostCapabilities` + `HostCapability` records —
  the typed envelope. `HostCapabilities` is the
  result envelope; `HostCapability` is a single
  tool's detection result (`Key`, `Available`,
  `Version`, `CredentialAvailable`).
- `SystemHostCapabilitiesService` — the production
  implementation. The implementation composes
  `IProcessRunner`, `ICredentialVault`, and
  `IPlatformInfo` (the M4-A.1 seams). The
  implementation probes six host tools
  (`git`, `ollama`, `powershell.exe`, `wsl.exe`,
  `wt.exe`, `bash.exe`) with `--version` (the
  `--version` flag is the conventional version-
  reporting flag for every tool in the list).
  The implementation reads the provider
  credentials from `ICredentialVault` (the
  `provider:<key>:token` naming convention).
- `AppCapabilityList` + `AppKeyValueList` — the
  two design-system components that surface the
  report. Both are **data-owning** components
  (per `docs/design-system.md` § 5.4) and expose
  the four child-content slots (`Loading`,
  `Empty`, `Error`, `Populated`).
- The `/diagnostics` page — the user-visible
  surface. The page is registered in the M2.2
  navigation registry via `[RouteMetadata]`
  (Href `/diagnostics`, Title `Diagnostics`,
  Order 4, ShowInSidebar = true, Icon `◆`,
  Description `Detected host capabilities
  (tools, versions, provider credentials).`).
- The startup capability-report log — the
  `IHostCapabilitiesService.DetectAsync` result
  is logged at startup (Information level)
  through `ILogger<Program>`. The log is the
  early signal that the host's toolset is
  inadequate.
- `Capabilities_Resolved_Through_Service` — the
  architecture test that asserts no
  `IProcessRunner` or `ICredentialVault` direct
  call in `App/Components/Diagnostics/` (the
  diagnostics surface is the only target of the
  test; the M4-A.2 Open Action is unaffected
  because the test is scoped to the diagnostics
  folder).

M4-B does not implement any `Providers.<X>` project
or any family registry. The M4-C family registry
consumes the M4-B `IHostCapabilitiesService`
through DI; the M4-D real providers replace the
M4-C fakes behind the same family contracts.
M4-B is the **smallest milestone that introduces
host capability detection**.

---

## 2. In Scope

1. **`IHostCapabilitiesService` contract.** The
   interface lives in
   `src/AiEng.Platform.Application/Capabilities/IHostCapabilitiesService.cs`.
   The contract exposes one method:
   `Task<HostCapabilities> DetectAsync(CancellationToken)`.
   The XML doc comment documents that the
   implementation probes the six host tools and
   reads the provider credentials; the contract
   is the **only** legal way to read host
   capabilities in the App layer (the
   `Capabilities_Resolved_Through_Service`
   architecture test enforces the rule; the test
   is scoped to `App/Components/Diagnostics/`).

2. **`HostCapabilities` + `HostCapability`
   records.** The records live in
   `src/AiEng.Platform.Application/Capabilities/`.
   `HostCapabilities` exposes a
   `IReadOnlyList<HostCapability> Capabilities`
   property and a `DetectedAt` (DateTimeOffset)
   timestamp. `HostCapability` exposes a `Key`
   (string; one of `git`, `ollama`,
   `powershell.exe`, `wsl.exe`, `wt.exe`,
   `bash.exe`, plus the provider credential
   names), an `Available` (bool), a `Version`
   (string; empty when `Available` is false), and
   a `CredentialAvailable` (bool; true when the
   corresponding provider credential is present
   in `ICredentialVault`). Both records are
   `sealed record class` (reference type;
   collection semantics).

3. **`SystemHostCapabilitiesService`
   implementation.** The implementation lives in
   `src/AiEng.Platform.Infrastructure/Capabilities/SystemHostCapabilitiesService.cs`.
   The implementation is a `sealed class` that
   implements `IHostCapabilitiesService` through
   composition: it takes `IProcessRunner`,
   `ICredentialVault`, `IPlatformInfo`, and an
   `ILogger<SystemHostCapabilitiesService>` through
   constructor injection. The `DetectAsync`
   method iterates the six host tools; for each
   tool it calls
   `IProcessRunner.RunToCompletionAsync(tool, new[] { "--version" }, ct)`
   with a 5-second `CancellationTokenSource`-
   based timeout. The exit code 0 returns
   `Available: true` + parsed `Version`; non-zero
   or thrown exception returns
   `Available: false`. The implementation then
   reads the six provider credentials from
   `ICredentialVault.GetAsync("provider:<key>:token", ct)`;
   `null` returns `CredentialAvailable: false`,
   non-null returns `CredentialAvailable: true`.
   The implementation gates the Windows-only
   tools (`powershell.exe`, `wsl.exe`, `wt.exe`)
   on `IPlatformInfo.IsWindows`; on non-Windows
   hosts the Windows-only tools return
   `Available: false` with `Version = string.Empty`
   and a `Note = "Windows-only"` comment in the
   log.

4. **`AppCapabilityList` component.** The
   component lives in
   `src/AiEng.Platform.App/Components/Diagnostics/AppCapabilityList.razor`.
   The component is **data-owning** per
   `docs/design-system.md` § 5.4 (the component
   takes a `IReadOnlyList<HostCapability>` as a
   `Capabilities` parameter; the consumer passes
   the data; the component renders the data).
   The component exposes the four child-content
   slots: `Loading`, `Empty`, `Error`,
   `Populated`. The `Populated` slot's default
   rendering is an `AppStack` of `AppCard`
   entries (one per `HostCapability`); each card
   shows the `Key`, an `AppStatusDot` (success
   for `Available: true`, error for
   `Available: false`), the `Version`, and a
   `AppBadge` for `CredentialAvailable: true`.
   The component is keyboard navigable;
   `aria-live="polite"` is set on the populated
   list so screen readers announce capability
   changes.

5. **`AppKeyValueList` component.** The component
   lives in
   `src/AiEng.Platform.App/Components/Diagnostics/AppKeyValueList.razor`.
   The component is **data-owning** per
   `docs/design-system.md` § 5.4. The component
   takes an `IReadOnlyList<KeyValuePair<string, string>>`
   as an `Items` parameter (the key + value
   pair; the value is a string; the consumer
   formats the value before passing). The
   component exposes the four child-content slots
   (`Loading`, `Empty`, `Error`, `Populated`).
   The `Populated` slot's default rendering is
   a `AppCard` with a `AppStack` of key-value
   rows; each row is a flexbox container with
   the key on the left (font-medium) and the
   value on the right (text-right). The
   component supports a `Format` enum parameter
   (`Plain`, `Boolean`, `Code`) that controls
   how the value is rendered: `Plain` renders
   the value as literal text; `Boolean` renders
   a check / cross icon; `Code` renders the
   value in a monospaced font.

6. **`Diagnostics.razor` page.** The page lives in
   `src/AiEng.Platform.App/Components/Pages/Diagnostics.razor`.
   The page is registered in the M2.2 navigation
   registry via
   `[RouteMetadata("/diagnostics", "Diagnostics", Order = 4, ShowInSidebar = true, Icon = "◆", Description = "Detected host capabilities (tools, versions, provider credentials).")]`.
   The page injects `IHostCapabilitiesService`;
   on `OnInitializedAsync` it calls
   `DetectAsync` and stores the result in a
   `HostCapabilities?` field. The page renders
   an `AppPageHeader` (title `Diagnostics`,
   description `The host's detected capabilities.
   The capability report is consumed by the
   provider registry (M4-C) to decide which
   providers are eligible for enablement.`,
   actions slot: a `Refresh` `AppButton` that
   re-runs `DetectAsync`). The page renders the
   `AppCapabilityList` for the six host tools +
   the six provider credentials. The page
   renders the `AppKeyValueList` for the
   supplementary host metadata (detected-at
   timestamp; host OS via `IPlatformInfo`; the
   M4-A.2 `IsWindows` flag).

7. **`AddHostCapabilities` composition root.** The
   extension lives in
   `src/AiEng.Platform.App/Composition/Capabilities/CapabilitiesServiceCollectionExtensions.cs`.
   The extension registers
   `IHostCapabilitiesService` →
   `SystemHostCapabilitiesService` via
   `TryAddSingleton`. The extension is called
   from `ServiceCollectionExtensions.AddPlatformServices`
   after `AddInfrastructure`. The extension is
   minimum-blast-radius: it adds one method to
   the existing extension; no other
   composition-root file is modified.

8. **`IHostCapabilitiesService` startup hook.** The
   `Program.cs` file calls
   `IHostCapabilitiesService.DetectAsync` once
   at startup and logs the result through
   `ILogger<Program>`. The log is Information
   level; the message includes the six tool
   statuses + the six credential statuses. The
   log is the early signal that the host's
   toolset is inadequate; the M4-C provider
   registry consumes the report through DI, not
   through the log.

9. **`Capabilities_Resolved_Through_Service`
   architecture test.** The test lives in
   `tests/AiEng.Platform.ArchitectureTests/Capabilities/Capabilities_Resolved_Through_Service.cs`.
   The test asserts:
   - `src/AiEng.Platform.App/Components/Diagnostics/`
     contains no `RunToCompletionAsync` token
     (no direct `IProcessRunner` call).
   - `src/AiEng.Platform.App/Components/Diagnostics/`
     contains no `ICredentialVault` direct
     reference (no `using
     AiEng.Platform.Application.Infrastructure`
     for `ICredentialVault`).
   - `Diagnostics.razor` contains
     `@inject IHostCapabilitiesService` (the
     single allowed seam).
   - The test is `Active` (not
     registered-but-disabled) because the
     M4-B scope is the only consumer of
     `IProcessRunner` outside
     `App/Components/Projects/AppProjectCard.razor`;
     the test is scoped to
     `App/Components/Diagnostics/` to avoid the
     M4-A.2 false positive. The test's
     activation milestone is M4-B (per
     `docs/design-system.md` § 5.4 + the
     progressive coding rule; the test is
     active when the M4-B contract lands).

10. **`docs/capabilities.md` documentation.** The
    documentation lives in
    `docs/capabilities.md` (10 sections mirroring
    `docs/infrastructure.md` § 1-10: Goals,
    Project Structure, `IHostCapabilitiesService`,
    `HostCapabilities`, `AppCapabilityList`,
    `AppKeyValueList`, `/diagnostics` Page,
    Composition Root, Tests, Out of Scope).

---

## 3. Out of Scope

1. **Provider creation.** M4-B does not create
   any `Providers.<X>` project. The first
   concrete providers land in M4-D. M4-B's
   brief explicitly says "Do not create
   providers."
2. **M4-C / M4-D work.** M4-B does not begin
   the family registry work (M4-C) or the
   first concrete provider work (M4-D). M4-C
   consumes the M4-B `IHostCapabilitiesService`
   through DI; the consumption is an M4-C
   responsibility.
3. **Worktree creation.** M5.
4. **Agent launching.** M6.
5. **Review / quality gates.** M7.
6. **Autonomous loops / orchestration.** M8.
7. **Activation of the four registered-but-
   disabled composition-root architecture
   tests.** The four `CompositionRootBoundaryTests`
   are activated in M4-D, not M4-B. M4-B
   introduces one new architecture test
   (`Capabilities_Resolved_Through_Service`)
   that is `Active` in M4-B (per the
   `docs/design-system.md` § 5.4 four-state
   rule + the progressive coding rule; the
   test is active when the contract lands).
8. **Activation of the two M4-A.1
   `Infrastructure_Respects_*` architecture
   tests.** The two M4-A.1 architecture tests
   are registered-but-disabled per ADR-016 and
   activate in M4-D, not M4-B. M4-B does not
   enable them.
9. **Activation of the three `AxeCoreAuditTests`.**
   The three accessibility-audit tests are
   registered-but-disabled per ADR-016 and the
   M4-D activation milestone.
10. **macOS / Linux credential vault.** The
    `WindowsCredentialVault` is Windows-only;
    on non-Windows hosts the implementation
    throws `PlatformNotSupportedException`. The
    M4-B `SystemHostCapabilitiesService` gates
    Windows-only tools on `IPlatformInfo.IsWindows`
    (a non-Windows host returns `Available: false`
    for `powershell.exe`, `wsl.exe`, `wt.exe`).
    Cross-platform credential storage is a
    future capability (M4-A plan § 3 out-of-scope
    item 9); a `MacOSKeychainCredentialVault`
    and a `LinuxSecretServiceCredentialVault`
    land in a future slice.
11. **M3 modal Escape-key / backdrop-click /
    Browse-folder follow-ups.** These are M3
    known issues that the M3.2 closeout recorded
    as M3 follow-ups; the follow-ups are a
    future task. M4-B does not address them.
12. **A design-system `AppDialog` primitive.**
    The M4-B `/diagnostics` page does not use
    a confirmation dialog; the M3.2 minimum-
    blast-radius decision (HTML5 native
    `<dialog>`) is preserved; no new
    design-system component is added in M4-B.
13. **Push to remote.** M4-B's first session
    (M4-B.1) does not push. The push decision
    is `Staged for push`; the next user
    command may push.

---

## 4. Files to Add

### Contracts (in Application)

- `src/AiEng.Platform.Application/Capabilities/IHostCapabilitiesService.cs` —
  the `IHostCapabilitiesService` contract.
- `src/AiEng.Platform.Application/Capabilities/HostCapabilities.cs` —
  the `HostCapabilities` record + the
  `HostCapability` record.

### Implementation (in Infrastructure)

- `src/AiEng.Platform.Infrastructure/Capabilities/SystemHostCapabilitiesService.cs` —
  the `IHostCapabilitiesService` implementation.
- `src/AiEng.Platform.Infrastructure/Capabilities/CapabilityProbe.cs` —
  the internal `CapabilityProbe` enum
  (`Found`, `NotFound`, `Failed`, `Timeout`)
  used by the implementation to record the
  per-tool probe result.

### Composition root (in App)

- `src/AiEng.Platform.App/Composition/Capabilities/CapabilitiesServiceCollectionExtensions.cs` —
  the `AddHostCapabilities` extension.

### Components (in App)

- `src/AiEng.Platform.App/Components/Diagnostics/AppCapabilityList.razor` —
  the data-owning capability-list component.
- `src/AiEng.Platform.App/Components/Diagnostics/AppCapabilityList.razor.css` —
  the scoped CSS.
- `src/AiEng.Platform.App/Components/Diagnostics/AppCapabilityList.razor.cs` —
  the code-behind (the `Capabilities`,
  `Loading`, `Empty`, `Error`, `Populated`
  parameters; the `OnInitializedAsync` no-op
  per the data-owning contract).
- `src/AiEng.Platform.App/Components/Diagnostics/AppKeyValueList.razor` —
  the data-owning key-value-list component.
- `src/AiEng.Platform.App/Components/Diagnostics/AppKeyValueList.razor.css` —
  the scoped CSS.
- `src/AiEng.Platform.App/Components/Diagnostics/AppKeyValueList.razor.cs` —
  the code-behind.
- `src/AiEng.Platform.App/Components/Diagnostics/_Imports.razor` —
  the imports (mirroring
  `App/Components/Projects/_Imports.razor`).
- `src/AiEng.Platform.App/Components/Pages/Diagnostics.razor` —
  the `/diagnostics` page.
- `src/AiEng.Platform.App/Components/Pages/Diagnostics.razor.css` —
  the scoped CSS (one or two layout tweaks
  for the page; no design-system change).

### Tests

- `tests/AiEng.Platform.UnitTests/Capabilities/SystemHostCapabilitiesServiceTests.cs` —
  10+ unit tests for the
  `SystemHostCapabilitiesService`
  implementation.
- `tests/AiEng.Platform.ComponentTests/Diagnostics/AppCapabilityListTests.cs` —
  5+ bUnit tests for `AppCapabilityList`.
- `tests/AiEng.Platform.ComponentTests/Diagnostics/AppKeyValueListTests.cs` —
  5+ bUnit tests for `AppKeyValueList`.
- `tests/AiEng.Platform.ComponentTests/Pages/DiagnosticsPageTests.cs` —
  3+ bUnit tests for the `/diagnostics` page.
- `tests/AiEng.Platform.ArchitectureTests/Capabilities/Capabilities_Resolved_Through_Service.cs` —
  1 new active architecture test.

### Documentation

- `docs/capabilities.md` — the M4-B
  documentation (10 sections mirroring
  `docs/infrastructure.md` § 1-10).
- `implementation-report-m4-b-1-<slice>.md` —
  the M4-B.1 implementation report (15+
  sections, mirroring the M4-A.1 / M4-A.2
  closeout reports).
- `.ai/handoffs/2026-07-13-m4-b-1-<slice>.md` —
  the M4-B.1 per-session handoff (mirrored to
  `latest.md`).

---

## 5. Files to Modify

- `AiEng.Platform.slnx` — add the
  `AiEng.Platform.Infrastructure/Capabilities/`
  directory (the directory is created by the
  M4-B.1 first commit; the `.slnx` does not
  need an update because the csproj already
  includes the new folder via the wildcard
  pattern in `Directory.Build.props`).
- `src/AiEng.Platform.App/AiEng.Platform.App.csproj` —
  no change; the App csproj already
  references `AiEng.Platform.Application`
  and `AiEng.Platform.Infrastructure`.
- `src/AiEng.Platform.App/Composition/ServiceCollectionExtensions.cs` —
  add the `AddHostCapabilities()` call after
  the existing `AddInfrastructure()` call.
- `src/AiEng.Platform.App/Program.cs` — add the
  startup capability-report log: the `Program.cs`
  resolves `IHostCapabilitiesService` from the
  DI container after `app.Build()` and before
  `app.Run()`, calls `DetectAsync` with a
  10-second timeout, and logs the result.
- `src/AiEng.Platform.App/Components/Pages/_Imports.razor` —
  no change; the `Diagnostics.razor` page
  uses the existing imports.
- `src/AiEng.Platform.App/Components/Shell/AppSidebar.razor` —
  no change; the M2.2 `INavigationRegistry`
  auto-discovers the new `/diagnostics` page
  via the `[RouteMetadata]` attribute.
- `docs/infrastructure.md` — add § 11
  (M4-B Consumers): one paragraph noting the
  M4-B `IHostCapabilitiesService` is the
  first consumer of `IProcessRunner` +
  `ICredentialVault` outside the M4-A.2 Open
  Action.
- `docs/design-system.md` — update § 4.5
  (`AppCapabilityList` + `AppKeyValueList`
  rows) from `Planned (M4)` to
  `Implemented (M4-B)` (M4-B.1 first session).
- `ROADMAP.md` — § 2 (M4-B row `Planned` →
  `Active`); § 3 (M4-B DoD bullets added
  per the M4-B closeout).
- `.ai/plans/master-delivery-plan.md` — § 1
  (M4-B row `Planned` → `Active`); § 3 (M4-B
  block updated; M4-B evidence list
  initialised; M4-B slice breakdown table
  initialised with M4-B.1 / M4-B.2 / M4-B.3
  rows).
- `.ai/state/capabilities.json` — C-015
  (IHostCapabilitiesService) evidence
  block initialised; `evidence.plans` =
  `[".ai/plans/M4-B-capability-detection.md"]`;
  `evidence.source_paths` and `evidence.tests`
  populated by the M4-B.1 first session
  closeout.
- `.ai/state/session.json` (M4-B.1 envelope;
  the M4-B plan promotion session envelope
  is recorded in the M4-B plan promotion
  handoff at
  `.ai/handoffs/2026-07-13-m4-b-plan-promotion.md`).
- `.ai/state/tasks.json` (the M4-B.1 / M4-B.2
  / M4-B.3 task records; T-023 → M4-B.1
  InProgress → Done in M4-B.1; T-024 → M4-B.2
  InProgress → Done in M4-B.2; T-025 → M4-B.3
  InProgress → Done in M4-B.3).
- `.ai/state/current.md` (active milestone
  M4-A → M4-B in M4-B.1; last completed task
  T-022; next recommended task T-023; the
  M4-B closeout updates the active task +
  milestone to the next milestone).
- `.ai/state/task-board.md` (M4-B.1 in Ready →
  In Progress → Done Recently in M4-B.1; the
  M4-B summary in `Deferred` archived to
  `Done Recently` in M4-B closeout).
- `.ai/state/milestones.json` (M4-B `Planned`
  → `Active` in M4-B.1; M4-B.1 slice block
  added in M4-B.1 closeout; M4-B evidence
  block populated in M4-B closeout).
- `.ai/handoffs/latest.md` (mirrored from
  `.ai/handoffs/2026-07-13-m4-b-<slice>.md`).

---

## 6. Coherent Commit

The M4-B plan is committed as a single coherent
commit on the feature branch
`feature/m4-b-capability-detection-plan-promotion`.
The commit message:

```
chore(m4-b.plan): draft M4-B capability detection plan in Awaiting Approval
```

The M4-B plan is approved implicitly on the user's
next `Next` invocation per `.ai/commands.md` § 4
and the Progressive Coding Rule § 7.1; the M4-B
implementation begins in a future session.

The M4-B implementation commits are per-slice on
the per-slice feature branches per the branching
strategy rule 4. The M4-B.1 commit message
template: `feat(m4-b.1): <title>`. The M4-B.2
commit message template:
`feat(m4-b.2): <title>`. The M4-B.3 commit
message template: `feat(m4-b.3): <title>`. The
M4-B closeout commit message:
`chore(m4-b.closeout): <title>`.

---

## 7. Critical Files to Read

- `AGENTS.md` — the 17 non-negotiable rules;
  specifically Rule 13 (no code comments), Rule
  15 (project-continuity state), Rule 16
  (scope discipline), Rule 17 (evidence of
  completion).
- `.ai/session-start.md` — the 13-step
  lifecycle.
- `.ai/workflows/progressive-coding.md` — the
  Progressive Coding Rule.
- `.ai/workflows/branching-strategy.md` — the
  12 rules; specifically rules 4, 6, 7.
- `.ai/workflows/milestone-closeout.md` — the
  Milestone Closeout Standard (the M4-B
  closeout follows the standard).
- `.ai/plans/M4-A-infrastructure-process-execution.md` —
  the M4-A plan (the canonical reference for
  the M4-B plan's 12-section structure).
- `.ai/handoffs/2026-07-11-m4-a-1-infrastructure-project-skeleton.md` —
  the M4-A.1 handoff (the M4-B.1 first session
  reads this first; § 8 is the M4-B.1 first
  session's 11-step list).
- `.ai/handoffs/2026-07-11-m4-a-2-open-action.md` —
  the M4-A.2 handoff (the only existing
  `IProcessRunner` activation; the M4-B
  architecture test is scoped to
  `App/Components/Diagnostics/` to avoid the
  M4-A.2 false positive).
- `implementation-report-m4-a-1-infrastructure-project-skeleton.md` —
  the M4-A.1 implementation report (the
  M4-B.1 closeout report mirrors the M4-A.1
  / M4-A.2 / M3 closeout reports).
- `docs/infrastructure.md` — the M4-A
  documentation (10 sections; the M4-B
  documentation mirrors the structure; § 7
  Open Action is the M4-A.2 reference; § 11
  M4-B Consumers is added in the M4-B plan
  promotion).
- `docs/design-system.md` — the design system
  catalogue (§ 4.5 Domain components; § 5.4
  the four-state rule for data-owning
  components; the `AppCapabilityList` +
  `AppKeyValueList` rows are updated from
  `Planned (M4)` to `Implemented (M4-B)` in
  the M4-B.1 first session).
- `src/AiEng.Platform.Application/Infrastructure/IProcessRunner.cs` —
  the `IProcessRunner` contract
  (`RunToCompletionAsync(string,
  IReadOnlyList<string>, CancellationToken) →
  Task<ProcessResult>`; the M4-B service uses
  the contract to probe each tool with
  `--version`).
- `src/AiEng.Platform.Application/Infrastructure/ICredentialVault.cs` —
  the `ICredentialVault` contract
  (`GetAsync(string, CancellationToken) →
  Task<string?>`; the M4-B service uses the
  contract to read provider credentials).
- `src/AiEng.Platform.Application/Infrastructure/IPlatformInfo.cs` —
  the `IPlatformInfo` contract (with the
  M4-A.2 `IsWindows` extension; the M4-B
  service uses `IsWindows` to gate
  Windows-only tools).
- `src/AiEng.Platform.Application/Navigation/RouteMetadataAttribute.cs` —
  the `[RouteMetadata]` attribute (the M4-B
  `/diagnostics` page is registered via this
  attribute; Href `/diagnostics`, Title
  `Diagnostics`, Order 4, ShowInSidebar =
  true, Icon `◆`).
- `src/AiEng.Platform.App/Composition/Infrastructure/InfrastructureServiceCollectionExtensions.cs` —
  the `AddInfrastructure` extension (the M4-B
  `AddHostCapabilities` extension follows the
  same `TryAddSingleton` pattern; the M4-B
  extension is called after `AddInfrastructure`
  in `ServiceCollectionExtensions.AddPlatformServices`).
- `src/AiEng.Platform.App/Components/Projects/AppProjectCard.razor` —
  the M4-A.2 Open Action (the only current
  consumer of `IProcessRunner` outside the
  Infrastructure project; the M4-B
  `Capabilities_Resolved_Through_Service`
  architecture test must scope the assertion
  to `App/Components/Diagnostics/` to avoid
  the M4-A.2 false positive).
- `src/AiEng.Platform.App/Components/Common/Enums.cs` —
  the M1.2 component enums (the M4-B
  `AppKeyValueList` `Format` enum follows the
  M1.2 enum pattern; `AppKeyValueListFormat` is
  added to `Enums.cs`).
- `.editorconfig` — CRLF + 4-space indent
  for `.cs`/`.razor`/`.md` (use `unix2dos`
  on every new file per the .editorconfig
  CRLF rule).
- `.ai/state/tasks.json` — the M4-B.1 / M4-B.2
  / M4-B.3 task records (the M4-B plan
  promotion does not create the records;
  the M4-B.1 first session creates the
  records).
- `.ai/state/task-board.md` — the M4-B row in
  `Deferred` (moved to `Ready` in the M4-B
  plan promotion; the M4-B.1 / M4-B.2 / M4-B.3
  rows in `Ready` are added in the M4-B plan
  promotion).
- `.ai/state/milestones.json` — the M4-B row
  in `Planned` (updated to `Active` in the
  M4-B plan promotion; the M4-B evidence block
  initialised in the M4-B plan promotion; the
  M4-B.1 slice block added in the M4-B.1 first
  session closeout).
- `.ai/state/capabilities.json` — C-015
  (`IHostCapabilitiesService`; the M4-B plan
  promotion updates C-015's evidence block to
  reference the new plan path; the M4-B.1
  closeout populates C-015's `source_paths`
  and `tests` fields).
- `.ai/handoffs/2026-07-11-m4-a-2-open-action.md` —
  the M4-A.2 handoff (the source of the M4-B
  plan promotion's "next action" — "the
  M4-B plan promotion").

---

## 8. Existing Functions and Utilities to Reuse

- The **M4-A.1 `IProcessRunner` contract** is
  the seam the M4-B
  `SystemHostCapabilitiesService` composes.
  The contract signature is
  `Task<ProcessResult> RunToCompletionAsync(string executable, IReadOnlyList<string> arguments, CancellationToken cancellationToken = default)`.
  The M4-B service uses this to probe each
  host tool with `--version` (e.g.,
  `RunToCompletionAsync("git", new[] { "--version" }, ct)`).
- The **M4-A.1 `ICredentialVault` contract** is
  the seam the M4-B
  `SystemHostCapabilitiesService` composes.
  The contract signature is
  `Task<string?> GetAsync(string name, CancellationToken cancellationToken = default)`.
  The M4-B service uses this to detect
  provider credentials (e.g.,
  `GetAsync("provider:git:token", ct)`).
- The **M4-A.2 `IPlatformInfo.IsWindows`** is
  the seam the M4-B
  `SystemHostCapabilitiesService` uses to
  gate Windows-only tools (`powershell.exe`,
  `wsl.exe`, `wt.exe`).
- The **M4-A.1 `SystemProcessRunner`** is the
  implementation the M4-B service composes
  (no custom process code in the M4-B service).
- The **M4-A.1 `WindowsCredentialVault`** is
  the implementation the M4-B service composes
  (no custom credential code in the M4-B
  service).
- The **M2.2 `RouteMetadata` +
  `RouteMetadataAttribute` + `RouteRegistry`**
  is the pattern the M4-B `/diagnostics`
  page uses for navigation registration
  (the page is registered via
  `[RouteMetadata]`; the M2.2 `RouteRegistry`
  auto-discovers it; the M2.2 `AppSidebar`
  renders the sidebar entry automatically).
- The **M1.2 `AppPanel` + `AppCard` +
  `AppStack` + `AppBadge` + `AppStatusDot`**
  are the layout primitives the M4-B
  `AppCapabilityList` + `AppKeyValueList`
  compose (the M4-B components use the
  M1.2 primitives; no new design-system
  primitive is added).
- The **M1.2 `AppEmptyState` + `AppErrorState`
  + `AppLoading` + `AppSkeleton`** are the
  four-state fallback components the M4-B
  `AppCapabilityList` (data-owning) uses
  (the `Empty` / `Error` / `Loading` slots
  fall back to the M1.2 components when
  the consumer does not provide the slot
  content).
- The **M2.1 `AppLayout` + `AppSidebar`** are
  the shell components the M4-B
  `/diagnostics` page renders inside (no
  change to `AppLayout`; the M2.2 navigation
  registry surfaces the new page
  automatically).
- The **M3.1 `AppButton`** is the primitive
  the M4-B `/diagnostics` page uses for the
  "Refresh" action (re-runs
  `IHostCapabilitiesService.DetectAsync`).
- The **M3.2 `AppPageHeader`** is the primitive
  the M4-B `/diagnostics` page uses for the
  title + description + actions.

---

## 9. Risks and Mitigations

| Risk | Mitigation |
| --- | --- |
| The `git --version` probe may take >1s on slow Windows hosts (the first probe after a fresh boot may be slow). | The probe uses a 5-second `CancellationTokenSource`-based timeout (the default; configurable via `SystemHostCapabilitiesServiceOptions.ProbeTimeout`). A timeout returns `Available: false` with a `Failed` `CapabilityProbe`; the log records the timeout. |
| `ollama` may not be on `PATH` (the typical case for a developer who has not installed Ollama). | The probe returns `ProcessResult.ExitCode != 0` → `Available: false`. The `AppCapabilityList` renders the row with an `AppStatusDot` error variant. The user can install `ollama` and click "Refresh" to re-probe. |
| `wsl.exe` requires Windows 10+ (the `wsl.exe` binary is shipped with Windows 10 and later). | The probe is gated on `IPlatformInfo.IsWindows`; on non-Windows hosts the `wsl.exe` row is hidden (not rendered). On Windows 10 hosts without WSL enabled, the probe returns `ExitCode != 0` → `Available: false`. |
| `wt.exe` is Windows 11 only. | The probe is gated on `IPlatformInfo.IsWindows`; on Windows 10 hosts the `wt.exe` row is hidden (not rendered) regardless of the probe result. The hide is a UI-side check (`IsWindows && Environment.OSVersion.Version.Build >= 22000`). |
| `ICredentialVault.GetAsync` may return `null` for missing credentials (the typical case for a fresh install). | The `CredentialAvailable` field records the `null` → `false` mapping. The `AppCapabilityList` row shows a "Credential not set" badge. The user can set the credential through the M7 `AppProviderSettingsForm` (a future slice). |
| The M4-B `IHostCapabilitiesService` is the first consumer of `IProcessRunner` outside `App/Components/Projects/AppProjectCard.razor`. The M4-A.2 Open Action is the only existing seam activation. | The `Capabilities_Resolved_Through_Service` architecture test is scoped to `App/Components/Diagnostics/` to avoid the M4-A.2 false positive. The test asserts no `RunToCompletionAsync` token in `App/Components/Diagnostics/`. |
| The M4-B `AppCapabilityList` is a **data-owning** component per `docs/design-system.md` § 5.4. The data-owning rule requires the four child-content slots (`Loading`, `Empty`, `Error`, `Populated`). | The M4-B first session confirms the data-owning classification. The four slots are exposed as `RenderFragment?` parameters; the consumer (`Diagnostics.razor`) provides the four slots or accepts the M1.2 fallback components. The data-owning classification is recorded in the M4-B closeout's Lessons Learned. |
| The M4-B `Capabilities_Resolved_Through_Service` architecture test asserts no `IProcessRunner` direct call in `App/Components/Diagnostics/`. The test is `Active` (not registered-but-disabled) because the M4-B scope is the only consumer of `IProcessRunner` outside `App/Components/Projects/AppProjectCard.razor`. | The M4-B first session confirms the test's `Active` classification. The test is scoped to `App/Components/Diagnostics/`. The test activates in M4-B.1 (the M4-B.1 first session closeout). |
| The M4-B `/diagnostics` page is registered via `[RouteMetadata]` (Href `/diagnostics`, Order 4). The Order 4 places the entry after the M3 `/projects` page (Order 1) and the M2.4 `/dashboard` page (Order 2) and the M2.3 home/counter/weather (Orders 0-2) and the `/design-system` page (Order 100). The Order 4 is a tentative choice. | The M4-B first session confirms the Order. The Order is a planning-surface change; the M4-B closeout may revise the Order based on the user feedback. |
| The M4-B plan may discover during drafting that the six host tools are not the only tools the platform needs to detect (e.g., `docker`, `python`, `node`). The M4-B plan documents the initial six tools. | The M4-B plan documents the initial six tools. A future slice (M4-C or a post-M8 dogfooding slice) may extend the list. The list is a planning-surface change; the M4-B closeout may record the extension as a deviation. |
| The M4-B plan may discover that the six provider credentials (`provider:git:token`, `provider:ollama:token`, etc.) are not the only provider credentials the platform needs. The M4-B plan documents the initial six credential names. | The M4-B plan documents the initial six credential names. The credential naming convention is `provider:<key>:token`; a future slice may extend the list. The list is a planning-surface change; the M4-B closeout may record the extension as a deviation. |
| The M4-B plan may discover during drafting that the `IClock` abstraction (C-014, M4-A Planned but not yet implemented) is needed for the `DetectedAt` timestamp. | The M4-B service uses `DateTimeOffset.UtcNow` directly (the M4-A.1 `IClock` is not yet implemented; the M4-A plan § 1 anticipates the choice but defers it). The M4-B closeout records the choice as a deviation if `IClock` is implemented in M4-B (the M4-B plan defers `IClock` to a future slice). |
| The M4-B plan promotion may push the feature branch to `origin/main` if the user has a remote configured. | Push is **not** authorised in the M4-B plan promotion session; the M4-B plan promotion does not push. The push decision is `Staged for push`; the next user command may push. The same constraint applies to the M4-B.1 / M4-B.2 / M4-B.3 / M4-B closeout sessions. |

---

## 10. Test Plan

1. **Unit tests for
   `SystemHostCapabilitiesService`
   (`tests/AiEng.Platform.UnitTests/Capabilities/SystemHostCapabilitiesServiceTests.cs`,
   10+ tests):**
   - `DetectAsync_returns_capability_for_each_of_six_tools_when_all_present`:
     the `IProcessRunner` returns `ExitCode 0`
     for every tool probe; the result
     `HostCapabilities.Capabilities` has
     6 entries with `Available: true` and
     non-empty `Version`.
   - `DetectAsync_returns_NotFound_for_missing_tool`:
     the `IProcessRunner` returns
     `ExitCode != 0` for `ollama`; the result
     has the `ollama` entry with
     `Available: false`.
   - `DetectAsync_returns_Timeout_for_slow_tool`:
     the `IProcessRunner` throws
     `OperationCanceledException` after the
     5-second timeout; the result has the
     entry with `Available: false` and
     `Note = "Timeout"`.
   - `DetectAsync_returns_Failed_for_exception`:
     the `IProcessRunner` throws
     `Win32Exception`; the result has the
     entry with `Available: false` and
     `Note = "Failed"`.
   - `DetectAsync_skips_windows_only_tools_on_non_windows_host`:
     `IPlatformInfo.IsWindows` is `false`;
     the `powershell.exe`, `wsl.exe`,
     `wt.exe` entries are not in the result
     (the implementation short-circuits).
   - `DetectAsync_reads_provider_credentials`:
     the `ICredentialVault` returns a
     non-null token for every credential
     probe; the result has 6
     `CredentialAvailable: true` entries.
   - `DetectAsync_records_missing_credential_as_false`:
     the `ICredentialVault` returns `null`
     for the `provider:ollama:token`
     credential; the result has the
     `ollama` entry with
     `CredentialAvailable: false`.
   - `DetectAsync_respects_cancellation`:
     the caller's `CancellationToken` is
     already cancelled; the result is
     `OperationCanceledException`.
   - `DetectAsync_sets_DetectedAt_to_UtcNow`:
     the result `DetectedAt` is within
     1 second of `DateTimeOffset.UtcNow`.
   - `DetectAsync_aggregates_results_in_order`:
     the result `Capabilities` list is in
     the order `[git, ollama,
     powershell.exe, wsl.exe, wt.exe,
     bash.exe, provider:git:token,
     provider:ollama:token, ...]`.

2. **bUnit tests for `AppCapabilityList`
   (`tests/AiEng.Platform.ComponentTests/Diagnostics/AppCapabilityListTests.cs`,
   5+ tests):**
   - `AppCapabilityList_renders_Populated_slot_when_capabilities_present`:
     6 `HostCapability` entries are passed;
     the `Populated` slot renders the
     `AppCard` for each entry.
   - `AppCapabilityList_renders_Empty_slot_when_no_capabilities`:
     0 `HostCapability` entries are passed;
     the `Empty` slot renders the
     `AppEmptyState`.
   - `AppCapabilityList_renders_Loading_slot_when_loading`:
     the `IsLoading` parameter is `true`;
     the `Loading` slot renders the
     `AppLoading` spinner.
   - `AppCapabilityList_renders_Error_slot_on_error`:
     the `ErrorMessage` parameter is
     "Failed to detect capabilities";
     the `Error` slot renders the
     `AppErrorState` with the message.
   - `AppCapabilityList_renders_status_dot_for_Available_true`:
     an entry with `Available: true` is
     passed; the `AppStatusDot` has the
     success variant.

3. **bUnit tests for `AppKeyValueList`
   (`tests/AiEng.Platform.ComponentTests/Diagnostics/AppKeyValueListTests.cs`,
   5+ tests):**
   - `AppKeyValueList_renders_Populated_slot_with_key_value_rows`:
     3 `KeyValuePair<string, string>` entries
     are passed; the `Populated` slot
     renders 3 rows.
   - `AppKeyValueList_renders_Empty_slot_when_no_items`:
     0 entries; the `Empty` slot renders
     the `AppEmptyState`.
   - `AppKeyValueList_renders_Boolean_format_as_check_or_cross`:
     the `Format` parameter is
     `AppKeyValueListFormat.Boolean`; the
     value `true` renders as a check
     icon; the value `false` renders as a
     cross icon.
   - `AppKeyValueList_renders_Code_format_in_monospace`:
     the `Format` parameter is
     `AppKeyValueListFormat.Code`; the
     rendered value uses a monospaced
     font.
   - `AppKeyValueList_renders_Plain_format_as_literal_text`:
     the `Format` parameter is
     `AppKeyValueListFormat.Plain`; the
     value renders as literal text.

4. **bUnit tests for `Diagnostics.razor`
   (`tests/AiEng.Platform.ComponentTests/Pages/DiagnosticsPageTests.cs`,
   3+ tests):**
   - `DiagnosticsPage_calls_DetectAsync_on_init`:
     a `FakeHostCapabilitiesService` is
     injected; the page is rendered;
     `DetectAsync` was called exactly
     once.
   - `DiagnosticsPage_renders_AppCapabilityList_with_capabilities`:
     a `FakeHostCapabilitiesService`
     returns 6 capabilities; the page
     renders the `AppCapabilityList` with
     the 6 capabilities.
   - `DiagnosticsPage_Refresh_button_reruns_DetectAsync`:
     a `FakeHostCapabilitiesService`
     returns 6 capabilities; the user
     clicks the "Refresh" button;
     `DetectAsync` was called exactly
     twice.

5. **Architecture tests:**
   - **1 new active test:**
     `Capabilities_Resolved_Through_Service`
     asserts `App/Components/Diagnostics/`
     contains no `RunToCompletionAsync` token,
     no `ICredentialVault` direct call, and
     `Diagnostics.razor` contains
     `@inject IHostCapabilitiesService`.
   - **0 new registered-but-disabled tests.**

6. **Regression tests:**
   - The M4-A.1 / M4-A.2 323 tests remain
     green.
   - The M4-B.1 / M4-B.2 / M4-B.3 sessions do
     not regress the M4-A.1 / M4-A.2
     closeout state.

7. **Visual smoke tests:**
   - `curl http://localhost:5210/diagnostics`
     returns 200; the page renders the
     `AppCapabilityList` with the host's
     detected capabilities.
   - Click the "Refresh" button; the page
     re-runs `DetectAsync`; the
     `AppCapabilityList` re-renders with
     the new capabilities.

---

## 11. Documentation Plan

1. **`docs/capabilities.md` (new, 10 sections
   mirroring `docs/infrastructure.md` § 1-10):**
   - § 1 Goals
   - § 2 Project Structure
   - § 3 `IHostCapabilitiesService` (the
     contract)
   - § 4 `HostCapabilities` (the record)
   - § 5 `AppCapabilityList` (the component)
   - § 6 `AppKeyValueList` (the component)
   - § 7 `/diagnostics` Page (the
     user-visible surface)
   - § 8 Composition Root (the
     `AddHostCapabilities` extension)
   - § 9 Tests (the unit + bUnit + architecture
     test inventory)
   - § 10 Out of Scope (the M4-C / M4-D / M5
     items)

2. **`docs/infrastructure.md` § 11 (new
   subsection: M4-B Consumers):** one
   paragraph noting the M4-B
   `IHostCapabilitiesService` is the first
   consumer of `IProcessRunner` +
   `ICredentialVault` outside the M4-A.2
   Open Action.

3. **`docs/design-system.md` § 4.5** (update
   on M4-B.1 first session closeout): the
   `AppCapabilityList` + `AppKeyValueList`
   rows change from `Planned (M4)` to
   `Implemented (M4-B)`.

4. **`ROADMAP.md` § 2 + § 3** (update on
   M4-B.1 first session closeout): M4-B row
   changes from `Planned` to `Active`;
   M4-B DoD bullets added in § 3.

5. **`.ai/plans/master-delivery-plan.md` § 1
   + § 3** (update on M4-B.1 first session
   closeout): M4-B row changes from
   `Planned` to `Active`; M4-B evidence
   list initialised; M4-B slice breakdown
   table initialised with M4-B.1 / M4-B.2
   / M4-B.3 rows.

6. **`DECISIONS.md`:** no new ADR; the
   M4-B plan references ADR-016 (the
   registered-but-disabled pattern) and
   ADR-011 (the four-source-projects
   structure).

---

## 12. Stop Condition

The M4-B plan promotion session stops after the
coherent commit on the feature branch
`feature/m4-b-capability-detection-plan-promotion`.
The next session is the M4-B implementation
(when the user invokes `Approve` or `Next`); the
M4-B plan promotion session does **not** begin
the M4-B implementation. Per the brief:
"Do not begin the following task."

The M4-B implementation sessions (M4-B.1, M4-B.2,
M4-B.3) each stop after the per-slice coherent
commit. The M4-B closeout session stops after the
M4-B closeout commit. The M4-B plan does **not**
begin the M4-C plan promotion, the M4-D plan
promotion, or any provider creation.

---

**End of M4-B plan.** The M4-B plan is the
canonical M4-B plan; the M4-B implementation
follows the plan. The M4-B plan promotion is
the M4-A.2 closeout's "next concrete step".
The M4-B plan is approved implicitly on the
user's next `Next` invocation.
