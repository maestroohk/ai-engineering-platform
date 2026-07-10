# Master Delivery Plan

> **The master delivery plan connects the product vision in
> [`PRODUCT.md`](./PRODUCT.md) to the milestone map in
> [`ROADMAP.md`](./ROADMAP.md).**
>
> The roadmap in `ROADMAP.md` is the source of truth for the
> ordering, the scope, and the definition of done of every
> milestone. This document is a **delivery view** of the same
> information: for every milestone, it restates the purpose, the
> user-visible outcome, the major capabilities delivered, the
> dependencies, the completion status, the evidence, and the next
> milestone the work enables. Milestones are listed in their
> accepted order. They are not re-ordered here; they are not
> re-scoped here. When this document and `ROADMAP.md` disagree,
> `ROADMAP.md` wins.

---

## 1. Delivery Summary

| #     | Milestone                                                | Status                     | Last stable evidence                          |
| ----- | -------------------------------------------------------- | -------------------------- | --------------------------------------------- |
| M0    | Documentation Foundation                                | **Done**                   | This document set.                            |
| M1    | Design System Core                                      | **Done (closed 2026-07-10)** | `implementation-report-m1-closeout.md`; first commits `1722bd2`, `2ba1fad`. |
| M2    | Application Shell and Navigation                        | Planned (M2.1 in plan)     | `.ai/plans/M2.1-application-shell-skeleton.md` (Awaiting Approval). |
| M3    | Project Registration                                    | Planned                    | No evidence yet.                              |
| M4-A  | Infrastructure / Process Execution                      | Planned                    | No evidence yet.                              |
| M4-B  | Capability Detection                                    | Planned                    | No evidence yet.                              |
| M4-C  | Provider Registry Foundation                            | Planned                    | No evidence yet.                              |
| M4-D  | First Concrete Process Providers                        | Planned                    | No evidence yet.                              |
| M5    | Native Git Worktrees                                     | Planned                    | No evidence yet.                              |
| M6    | Agent Runtime Launching                                 | Planned                    | No evidence yet.                              |
| M7    | Review and Quality Gates                                | Planned                    | No evidence yet.                              |
| M8    | Autonomous Loops, Orchestration, Production Hardening   | Planned                    | No evidence yet.                              |

---

## 2. The Final Product Journey

The final product journey, taken end-to-end, is the spine of the
master delivery plan. Every milestone delivers a slice of this
journey. The first slice a developer can take is the
**project-registration → run → review → disposition** path that is
the M3 + M4 + M5 + M6 + M7 subset of the journey; M8 adds
autonomous loops and orchestration on top.

```
Register project        (M3)
→ create task           (M3)
→ prepare worktree      (M5)
→ choose runtime, model (M4-C registry, M6 picker)
→ launch agent          (M4-A process runner, M4-D / M6 runtime provider)
→ observe execution     (M6)
→ review changes        (M7)
→ run quality gate      (M7)
→ approve or reject     (M7)
→ merge or clean up     (M5, M7)
```

Plus, in M8:

```
→ run bounded autonomous loop   (M8)
→ coordinate multiple agents    (M8)
```

The journey is supported by the cross-cutting capabilities the
non-vertical milestones deliver:

```
Provider registry + composition root (M4-C)
Host capability detection (M4-B)
Process execution boundary (M4-A)
```

---

## 3. Per-Milestone Detail

### M0 — Documentation Foundation

- **Purpose.** Establish the engineering standards, the
  architecture foundation, and the reusable development rules
  before any code is written. Everything that follows builds on
  this; the rules outlive any particular feature, sprint, or
  contributor.
- **User-visible outcome.** A new contributor can read
  `AGENTS.md` and the documents it references and understand
  what to do next.
- **Major capabilities delivered.**
  - `AGENTS.md` — the constitution (15 non-negotiable rules
    after the M1 closeout).
  - `ARCHITECTURE.md` — the layered architecture.
  - `ROADMAP.md` — the ordered milestone map.
  - `STYLEGUIDE.md`, `CONTRIBUTING.md`, `DECISIONS.md`.
  - `docs/*` — eight engineering and design documents.
  - `.ai/prompts/*`, `.ai/workflows/*`, `.ai/templates/*` —
    the AI collaboration hub.
- **Dependencies.** None. M0 is the starting state.
- **Completion status.** **Done.**
- **Evidence.** The document set exists; this `PRODUCT.md` and
  the master delivery plan are written against it.
- **Next milestone enabled.** M1 (the design system cannot be
  built without the rules that govern it).

### M1 — Design System Core

- **Purpose.** Build the design system that every later
  milestone composes against. The design system is the
  substrate of every visible surface; building it first forces
  every later feature to compose components rather than
  invent them.
- **User-visible outcome.** A runnable Blazor Server
  application on .NET 10 that renders the design-system
  documentation page (`/design-system`) built exclusively
  from the 19 M1.2 components. Light and dark themes.
  Keyboard-navigable, ARIA-labelled, focus-visible.
- **Major capabilities delivered.**
  - The four source projects (`AiEng.Platform.App`,
    `AiEng.Platform.Application`, `AiEng.Platform.Domain`,
    `AiEng.Platform.Providers.Abstractions`) and the three
    test projects (`UnitTests`, `ComponentTests`,
    `ArchitectureTests`).
  - Tailwind v3 + PostCSS pipeline; the design-token
    catalogue; light and dark themes via the data-attribute
    theme switcher.
  - 19 reusable Blazor components (Primitives 7, Layout 4,
    Display 2, Feedback 5, Inputs 1).
  - 77 bUnit component tests.
  - 3 active architecture tests
    (`App_DoesNotReference_Providers_Implementations`,
    `Pages_Use_DesignSystem_Components_Not_DOM`, and the
    M1.2 design-system test).
  - 4 registered-but-disabled composition-root architecture
    tests, citing ADR-016 and M4-D as the activation
    milestone.
- **Dependencies.** M0.
- **Completion status.** **Done (closed 2026-07-10).**
- **Evidence.**
  - `implementation-report-m1-bootstrap.md`.
  - `implementation-report-m1-1-frontend-foundation.md`.
  - `implementation-report-m1-2-design-system-core.md`.
  - `implementation-report-m1-closeout.md`.
  - `.ai/handoffs/2026-07-10-m1-closeout.md`.
  - First commits `1722bd2`, `2ba1fad` on `master`.
- **Next milestone enabled.** M2 (the application shell is
  built by composing the M1.2 components).

### M2 — Application Shell and Navigation

- **Purpose.** Replace the M1.1 chrome with a navigable
  application shell — a sidebar, a top bar, a content panel,
  and a route-aware navigation registry. Every later
  milestone is reached through this shell; the shell is the
  prerequisite for the M3 project-registration page and
  beyond.
- **User-visible outcome.** The application is a navigable
  professional developer tool on Windows desktop. Every
  navigation target renders an `AppEmptyState` that uses the
  design system. Sidebar items are data-driven from a route
  registry, not hard-coded. Keyboard navigation works across
  the sidebar. The shell is usable down to a 1280x720
  window.
- **Major capabilities delivered.**
  - `AppSidebar`, `AppSidebarItem`, `AppTopBar`,
    `AppBreadcrumb`.
  - `AppLayout`, `EmptyLayout`.
  - `AppDialog` (modal), `AppTabs`, `AppTab`.
  - `INavigationService` (route-aware).
  - The architecture test
    `Pages_AreReachable_Through_Registry` (active).
- **Dependencies.** M1.
- **Completion status.** **Planned** (M2.1 in plan,
  `Awaiting Approval`).
- **Evidence.** `.ai/plans/M2.1-application-shell-skeleton.md`.
- **Next milestone enabled.** M3 (project-registration page
  composes the M2 shell).

#### M2 slice breakdown (per the immediate-next task brief)

| Slice | Title                                  | Status             |
| ----- | -------------------------------------- | ------------------ |
| M2.1  | Application shell skeleton             | Awaiting Approval. |
| M2.2  | Sidebar and navigation registry        | Ready (summary).   |
| M2.3  | Top bar and page header integration    | Ready (summary).   |
| M2.4  | Empty application routes               | Ready (summary).   |
| M2.5  | Responsive and keyboard validation     | Ready (summary).   |
| M2.6  | M2 closeout and external Treehouse dogfooding checkpoint | Ready (summary). |

### M3 — Project Registration

- **Purpose.** Introduce the smallest piece of state the
  platform needs to be useful on its own: a registered
  project. M3 ships the contract (`IProjectService`,
  `IProjectStore`) and the in-memory implementation of the
  store; M4-A replaces the in-memory store with a durable
  on-disk implementation behind the same contract.
- **User-visible outcome.** A user can register a project
  (name + folder path) from a project-registration page
  composed of M2 shell + M3 surface. The project appears
  in the projects list. Projects are not durable across an
  application restart in M3 (the in-memory store is the
  smoke test for the contract; durable storage is M4-A).
- **Major capabilities delivered.**
  - `AppProjectCard`, `AppProjectList` (virtualised),
    extended `AppEmptyState` (the registration prompt).
  - `IProjectService`, `IProjectStore` (in-memory).
  - The architecture test
    `Application_DoesNotReference_Providers_Implementations`
    remains green.
- **Dependencies.** M2.
- **Completion status.** **Planned.**
- **Evidence.** No evidence yet.
- **Next milestone enabled.** M4 (the process runner and
  the capability detector work on registered projects).
- **Dogfooding checkpoint.** No Mistakes may be initialised
  and run as a quality gate against a registered project
  (M3 dogfood; M7 product integration).

### M4 — Process Execution, Capability Detection, Provider Registry

- **Purpose.** Introduce the platform's process boundary,
  the host's capability detection, the provider registry,
  and the first concrete process-boundary providers. M4 is
  split into four sequential slices (M4-A, M4-B, M4-C,
  M4-D) because the original M4 was too large to
  implement, review, and roll back as a single unit.

#### M4-A — Infrastructure / Process Execution

- **Purpose.** Land the
  `AiEng.Platform.Infrastructure` project and the platform's
  process-boundary, credential-vault, clock, and durable
  `IProjectStore` abstractions.
- **User-visible outcome.** No new UI surface in M4-A.
  The M3 in-memory `IProjectStore` is replaced by the
  on-disk implementation behind the same contract; the
  one-line DI change in `Program.cs` is the visible
  evidence.
- **Major capabilities delivered.**
  - `AiEng.Platform.Infrastructure` project.
  - `IProcessRunner`
    (`RunAsync(ProcessRequest, CancellationToken)`
    streaming + `RunToCompletionAsync`).
  - `ICredentialVault` (Windows Credential Manager).
  - `IClock`.
  - On-disk `IProjectStore` (replaces the M3 in-memory
    store).
  - Architecture test
    `No_DirectProcessStart_OutsideInfrastructure`
    (registered-but-disabled; activates in M4-D).
- **Dependencies.** M3.
- **Completion status.** **Planned.**
- **Evidence.** No evidence yet.
- **Next milestone enabled.** M4-B (capability detection
  consumes the infrastructure abstractions).

#### M4-B — Capability Detection

- **Purpose.** Detect the external capabilities present on
  the host (`git`, `ollama`, `powershell.exe`, `wsl.exe`,
  `wt.exe`, `bash.exe`) through `IProcessRunner` and
  `ICredentialVault`.
- **User-visible outcome.** A diagnostics page surfaces
  `AppCapabilityList` and `AppKeyValueList` showing the
  host's detected capabilities. The host's capability
  report is logged at startup.
- **Major capabilities delivered.**
  - `IHostCapabilitiesService`, `HostCapabilities` record.
  - `AppCapabilityList`, `AppKeyValueList` components.
  - Architecture test
    `Capabilities_Resolved_Through_Service`.
- **Dependencies.** M4-A.
- **Completion status.** **Planned.**
- **Evidence.** No evidence yet.
- **Next milestone enabled.** M4-C (the provider registry
  consumes the capability report).

#### M4-C — Provider Registry Foundation

- **Purpose.** Introduce `IProviderRegistry` and the
  family-scoped registries with **fake** providers. Land
  the composition-root pattern (per ADR-016) in
  `App/Composition/`. The four composition-root
  architecture tests stay registered-but-disabled until
  M4-D.
- **User-visible outcome.** A diagnostics page renders
  `AppProviderCard` for every registered provider (the
  fakes in M4-C, the real providers in M4-D).
- **Major capabilities delivered.**
  - `IProviderRegistry` (cross-family).
  - Family-scoped registries:
    `IAgentRuntimeProviderRegistry`,
    `IGitProviderRegistry`,
    `ITerminalProviderRegistry`,
    `IWorktreeProviderRegistry`,
    `IQualityGateProviderRegistry`,
    `IReviewProviderRegistry`,
    `IAutonomousLoopProviderRegistry`,
    `IOrchestrationProviderRegistry`.
  - Fake providers for every family (one
    `FakeServiceCollectionExtensions` per family under
    `Composition/<Family>/Fake/`).
  - The five lifecycle states (`Compiled-in`, `Registered`,
    `Enabled`, `Healthy`, `Selected`).
  - `AppProviderCard`, `AppHealthDot`.
  - Composition-root architecture tests (registered-but-
    disabled; activate in M4-D).
- **Dependencies.** M4-B.
- **Completion status.** **Planned.**
- **Evidence.** No evidence yet.
- **Next milestone enabled.** M4-D (the first real
  providers replace the fakes behind the same contracts).

#### M4-D — First Concrete Process Providers

- **Purpose.** Replace the M4-C fakes with the first two
  real process-boundary providers (`GitProvider` and
  `OllamaLaunchProvider`). Land the contract test
  framework (`AiEng.Platform.ProviderContractTests`).
  Activate the four composition-root architecture tests
  and `No_DirectProcessStart_OutsideInfrastructure`.
- **User-visible outcome.** The diagnostics page renders
  `AppProviderCard` for the real `GitProvider` and
  `OllamaLaunchProvider` with their actual
  `ProviderHealth`. The `OllamaLaunchProvider` smoke test
  (spawn
  `ollama launch claude --model minimax-m3:cloud`,
  capture output, stop) passes against a stub process.
- **Major capabilities delivered.**
  - `AiEng.Platform.Providers.Git` project.
  - `AiEng.Platform.Providers.Ollama` project.
  - `AiEng.Platform.ProviderContractTests` project.
  - `GitProvider` (concrete `IGitProvider`).
  - `OllamaLaunchProvider` (concrete
    `IAgentRuntimeProvider`).
  - Architecture tests (all four composition-root
    activated, `No_DirectProcessStart_OutsideInfrastructure`
    activated).
- **Dependencies.** M4-C.
- **Completion status.** **Planned.**
- **Evidence.** No evidence yet.
- **Next milestone enabled.** M5 (the worktree provider
  consumes `IGitProvider` through the registry).

### M5 — Native Git Worktrees

- **Purpose.** Introduce `IWorktreeProvider` and ship the
  native baseline (`NativeWorktreeProvider`, built on
  `IGitProvider` from M4). The worktree becomes the unit
  of isolation for the run.
- **User-visible outcome.** A user can create a worktree
  from a registered project; the worktree is listed on
  the worktrees page. The worktree provider resolves
  `IGitProvider` through the registry.
- **Major capabilities delivered.**
  - `NativeWorktreeProvider` (concrete
    `IWorktreeProvider`).
  - `IWorktreeService`, `IWorktreeStore`.
  - `AppWorktreeCard`, `AppWorktreeList`,
    `AppDiffViewer` (initial; full in M7).
  - Architecture test
    `NativeProviders_Use_Contracts_Not_Implementations`.
- **Dependencies.** M4-D.
- **Completion status.** **Planned.**
- **Evidence.** No evidence yet.
- **Next milestone enabled.** M6 (agent launching runs in
  a worktree).
- **Dogfooding checkpoint.** Treehouse externally to
  create an isolated development worktree on the shell
  (M2 dogfood; M5 product integration).

### M6 — Agent Runtime Launching

- **Purpose.** Ship the platform's first end-to-end
  coding-agent vertical slice: select a project, acquire
  or use its worktree, pick a runtime and a model,
  launch the agent, stream or expose the output, stop
  safely, persist the history.
- **User-visible outcome.** A user can launch an agent
  runtime against a registered project's worktree. The
  launch is cancellable mid-stream. The terminal output
  is rendered through `AppTerminalPanel` and
  `AppTerminalLine`. The execution history is persisted
  in `IHistoryStore` and survives an application
  restart.
- **Major capabilities delivered.**
  - `AppTerminalPanel`, `AppTerminalLine`,
    `AppRunStatus`, `AppRunHistory`, `AppModelPicker`,
    `AppRuntimePicker`.
  - `IRunService`, `IStreamingChannel`, `IHistoryStore`.
  - The M4 `OllamaLaunchProvider` deepened into a full
    provider (process spawn, model flag, output stream,
    cancellation, error mapping, exit-code handling).
  - Architecture test `History_Routed_Through_Store`.
- **Dependencies.** M5.
- **Completion status.** **Planned.**
- **Evidence.** No evidence yet.
- **Next milestone enabled.** M7 (review and quality
  gates run against launched runs).
- **Dogfooding checkpoint.** The development team may
  use the launch flow interactively to drive a real
  coding-agent task on a registered project (M6
  end-to-end dogfood; M8 production hardening).

### M7 — Review and Quality Gates

- **Purpose.** Land the `IReviewProvider` and
  `IQualityGateProvider` families with native baselines.
  External tools (Lavish Axi, No Mistakes) are added
  through the registry when the user opts in.
- **User-visible outcome.** A user can submit a launched
  run for review (findings rendered through
  `AppReviewPanel`); run a quality gate (pass/fail
  rendered through `AppQualityGateBadge`); configure any
  provider through the unified configuration form
  (secrets entered through `AppSecretField`).
- **Major capabilities delivered.**
  - `AppReviewPanel`, `AppReviewFindingList`,
    `AppQualityGateBadge`,
    `AppProviderSettingsForm`, `AppSecretField`,
    `AppConnectionTestButton`.
  - `IReviewService`, `IQualityGateService`,
    `IProviderConfigurationService`.
  - `NativeReviewProvider` (concrete `IReviewProvider`).
  - `NativeQualityGateProvider` (concrete
    `IQualityGateProvider`).
  - Architecture tests `No_Secrets_In_Logs`,
    `No_Secrets_In_Configuration`,
    `Pages_Resolve_Providers_Through_Registry`.
- **Dependencies.** M6.
- **Completion status.** **Planned.**
- **Evidence.** No evidence yet.
- **Next milestone enabled.** M8 (autonomous loops invoke
  review and quality gates between iterations).
- **Dogfooding checkpoint.** No Mistakes as a real
  quality gate against a launched run; Lavish Axi to
  review a launched run interactively (M7 dogfood; the
  external providers' product integration lands in a
  later milestone when the user opts in).

### M8 — Autonomous Loops, Orchestration, Production Hardening

- **Purpose.** Ship the remaining provider families
  (`IAutonomousLoopProvider`, `IOrchestrationProvider`)
  with native baselines. Land the production-hardening
  work: structured logging, unified `ProviderResult<T>`
  error envelope, durable persistence, signed packages,
  accessibility audits.
- **User-visible outcome.** A user can run a bounded
  autonomous loop (driven by a registered autonomous loop
  provider) and coordinate multiple agents (driven by a
  registered orchestration provider). The application
  installs on Windows and launches in under two seconds
  on a developer laptop. An accessibility audit passes
  for the main shell.
- **Major capabilities delivered.**
  - `AppToast`, `AppToastHost`, `AppDiagnosticDrawer`,
    `AppCrashBoundary`, `AppAutonomousLoopCard`,
    `AppOrchestrationGraph`.
  - `IAutonomousLoopService`, `IOrchestrationService`,
    `ITelemetryService`, `IUpdateService`.
  - `NativeAutonomousLoopProvider` (concrete
    `IAutonomousLoopProvider`).
  - `NativeOrchestrationProvider` (concrete
    `IOrchestrationProvider`).
  - Architecture test
    `NativeProviders_Use_ApplicationServices_Not_Implementation`.
- **Dependencies.** M7.
- **Completion status.** **Planned.**
- **Evidence.** No evidence yet.
- **Next milestone enabled.** Ongoing development at
  production quality. M8 is the last roadmap milestone;
  the platform is the foundation for post-M8 work.
- **Dogfooding checkpoint.** GNHF for a bounded,
  repetitive, non-architecture-sensitive task; Firstmate
  through WSL for cross-agent orchestration (M8 dogfood;
  the external providers' product integration lands in a
  later milestone when the user opts in).

---

## 4. How This Document Is Updated

This document is a **delivery view** of `ROADMAP.md`. When the
roadmap changes, this document changes with it. The roadmap is
authoritative; this document tracks it. The order of milestones
is not changed here.

When a milestone is closed:

1. The `Completion status` cell in § 1 is updated to
   `Done (closed YYYY-MM-DD)`.
2. The `Last stable evidence` cell in § 1 is updated to
   point to the implementation report and the commit
   hashes.
3. The `Evidence` bullet in § 3 is updated.
4. `.ai/state/current.md` and `.ai/state/task-board.md`
   are updated per Rule 15 in `AGENTS.md`.
5. The next-milestone plan is moved from `Ready` to
   `In Progress` in the task board.

This document does not approve a milestone; it records the
delivery state. The roadmap approves the milestone; this
document mirrors it.
