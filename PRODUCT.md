# Product Name

**AiEng.Platform** — a Windows-first control centre for AI-assisted
software development.

It is **not** itself primarily a coding agent. It is the
**environment in which coding agents work**.

# Product Purpose

AiEng.Platform gives a developer a single, native Windows surface for
the full lifecycle of an AI-assisted coding task: registering the
repository the agent will work on, preparing an isolated workspace,
picking a runtime and a model, launching the agent, observing what it
does, deciding whether the work is good, and merging or cleaning up.

The product removes the friction of orchestrating AI agents with ad-hoc
shell scripts, copy-pasted CLI invocations, and lost context. It turns
the workflow into a navigable, observable, recoverable, repeatable
operation that lives inside a professional developer tool.

# Problem Being Solved

A developer who uses a coding agent today assembles the workflow by
hand:

- They find the repository they want to work on.
- They create a Git worktree (or skip the worktree and contaminate
  the working tree).
- They pick a runtime (Ollama Launch, Claude, OpenAI-compatible,
  Codex) and a model.
- They launch the agent from a terminal, often in the wrong
  directory.
- They watch the output scroll by in a separate window.
- They stop, cancel, or recover the run with shell signals that
  are easy to get wrong.
- They `git diff` and `git log` in another window to inspect the
  agent's changes.
- They run quality gates from another terminal.
- They merge or clean up the worktree by hand.

The workflow is real, but the toolchain is fragmented. The
operational state — the run, its output, its review, its
disposition — is scattered across terminals, shells, and the
developer's memory. A restart loses the run history. A misclick
loses the worktree. A subtle bug in the launch invocation produces
a model that drifts from the one the developer intended.

AiEng.Platform replaces the ad-hoc toolchain with a single,
Windows-native surface that owns the entire lifecycle, persists
operational state, and integrates external agents and quality
tools through capability-based provider contracts.

# Target User

The product is built for **professional developers** who:

- Work on Windows 10/11 as their primary development environment.
- Use one or more AI coding agents (Ollama Launch, Claude, OpenAI,
  Codex) as part of a daily workflow.
- Maintain a non-trivial set of Git repositories and need to
  orchestrate AI work across them.
- Value repeatability, observability, and recoverability of
  agent-driven changes.
- Are comfortable with PowerShell, Windows Terminal, WSL, and Git
  for Windows, and expect a native integration with all of them.

The product is not built for:

- Casual users who want a chat-style interface to a model.
- Marketing copy generation, image generation, or other
  non-engineering AI use cases.
- Cloud-only, browser-only, or mobile-first workflows.

# Product Principles

The product is governed by the same principles documented in
[`AGENTS.md`](./AGENTS.md) and the architecture decision records in
[`DECISIONS.md`](./DECISIONS.md). The principles that shape the
product are:

- **The platform is a developer tool, not a chat app.** Every
  surface is tuned for the Windows desktop, large windows, and
  power users. Mobile and chat-style interactions are out of scope.
- **The platform orchestrates, it does not replace.** External
  tools (Ollama, Claude, OpenAI, Codex, Treehouse, Lavish Axi, No
  Mistakes, GNHF, Firstmate) are integrated through capability-based
  provider contracts, never modified. The platform never tries to
  be a coding agent itself.
- **Composition, not bypass.** Every later milestone consumes the
  reusable capabilities delivered by earlier milestones. A direct
  bypass — a page importing a provider implementation, a feature
  re-implementing a registered service inline — is rejected by
  the architecture tests, not by convention.
- **State is observable, recoverable, and persisted.** Every run
  has a history. Every worktree has a disposition. Every
  review has findings. Every decision is recorded.
- **Windows-native, not Windows-only.** The product's primary
  surface is Windows, but its design does not preclude Linux
  or macOS development machines when the user opts in.

# Core User Journey

The end-to-end journey a developer takes through the product is:

```
Register project
→ create task
→ prepare worktree
→ choose runtime and model
→ launch agent
→ observe execution
→ review changes
→ run quality gate
→ approve or reject
→ merge or clean up
```

Each step is a navigable surface in the product. Each step is
reversible. Each step produces a record that survives an application
restart.

# Final Product Capabilities

When the product is complete, a developer can do the following from
a single Windows application:

1. **Register an existing Git repository** — provide a name and a
   folder path; the platform owns the `Project` entity.
2. **Create a coding task for that repository** — describe the work
   in human terms; the platform owns the `Task` entity.
3. **Prepare an isolated Git worktree** — the platform creates a
   worktree through the worktree provider family; the worktree is
   the unit of isolation for the run.
4. **Select an AI runtime and model** — the platform lists the
   registered runtimes (Ollama Launch, Claude-compatible,
   OpenAI-compatible, Codex); the platform lists the models
   available for each runtime.
5. **Launch the selected agent inside the worktree** — the platform
   spawns the agent process through the process runner; the
   process boundary is the only legal way to launch a host
   process.
6. **Observe process status and streamed output** — the platform
   renders the agent's stdout, stderr, and exit code in a
   terminal panel; the platform reports run status transitions
   (queued, running, succeeded, failed, cancelled, recovered).
7. **Stop, cancel, resume, or recover a run** — the platform owns
   the cancellation token; a run can be cancelled mid-stream;
   a run that was cancelled or crashed can be recovered.
8. **Inspect files and Git changes** — the platform renders the
   diff and the log of the worktree the agent ran in; the
   developer can review the changes the agent produced.
9. **Submit the work for review** — the platform routes the
   worktree to the registered review provider (Lavish Axi, the
   native baseline, or another opted-in tool).
10. **Run quality gates** — the platform routes the worktree to
    the registered quality-gate provider (No Mistakes, the
    native baseline, or another opted-in tool); the gate's
    pass/fail is rendered.
11. **Approve, reject, merge, retain, or clean up the worktree** —
    the platform owns the disposition; the developer can
    merge the worktree back into the source repository, retain
    it for further work, or clean it up.
12. **Maintain execution and task history** — the platform persists
    every run, every review, every gate, and every disposition;
    the history survives an application restart.
13. **Run bounded autonomous loops** — the platform can drive a
    series of launches and gates under a registered autonomous
    loop provider (GNHF, the native baseline, or another opted-in
    tool); the loop is bounded and recoverable.
14. **Coordinate multiple agents** — the platform can orchestrate
    a graph of agents, worktrees, and gates under a registered
    orchestration provider (Firstmate through WSL, the native
    baseline, or another opted-in tool); the orchestration is
    observable and recoverable.

# Capability Map

The product's capabilities are organised by the eight
capability-oriented provider families defined in
[`DECISIONS.md`](./DECISIONS.md) (ADR-012):

| Family                    | Purpose                                                  | M4-D → M8 native baseline   | External candidates                  |
| ------------------------- | -------------------------------------------------------- | --------------------------- | ------------------------------------ |
| `IGitProvider`            | Read and write Git state                                 | `GitProvider` (M4-D)        | —                                    |
| `IAgentRuntimeProvider`   | Launch a coding agent                                    | `OllamaLaunchProvider` (M4-D, deepened in M6) | Claude-compatible, OpenAI-compatible, Codex (M8) |
| `IWorktreeProvider`       | Create and manage isolated Git worktrees                 | `NativeWorktreeProvider` (M5) | Treehouse (M5 dogfood; product later) |
| `IReviewProvider`         | Review agent output and surface findings                 | `NativeReviewProvider` (M7) | Lavish Axi (M1 / M7 dogfood)         |
| `IQualityGateProvider`    | Run quality gates against a worktree                     | `NativeQualityGateProvider` (M7) | No Mistakes (M3 / M7 dogfood)        |
| `IProviderRegistry` (cross-family) | Resolve providers by family, lifecycle, and selection | Built-in (M4-C)             | —                                    |
| `IAutonomousLoopProvider` | Drive bounded autonomous loops                           | `NativeAutonomousLoopProvider` (M8) | GNHF (M5 / M8 dogfood)               |
| `IOrchestrationProvider`  | Coordinate multiple agents and gates                     | `NativeOrchestrationProvider` (M8) | Firstmate through WSL (M8 dogfood)   |

# Product Completion Model

> **The structured answer to "what product are we building, how
> far are we, and what is the next step?"** This section
> mirrors the structured state in
> [`.ai/state/capabilities.json`](./.ai/state/capabilities.json),
> the milestone slice block in
> [`.ai/state/milestones.json`](./.ai/state/milestones.json),
> and the task queue in
> [`.ai/state/tasks.json`](./.ai/state/tasks.json). The
> per-step table below is hand-derived from those JSON files;
> no progress percentage is invented. The
> `Status`, `Continue`, `Approve`, `Resume`, and `Finish`
> commands defined in
> [`.ai/commands.md`](./.ai/commands.md) read the same JSON
> files and answer the same questions.

## Status Definitions

Each capability carries a `completion_status` field with one
of eight values. The mapping from the canonical 5-value
`status` (the `Done / Accepted / Proposed / Deferred / Rejected`
field on every capability) to the 8-value `completion_status`
is the single source of truth for the
`Status` / `Continue` / `Approve` / `Resume` / `Finish`
commands.

| `completion_status` | Meaning |
| --- | --- |
| `NotStarted` | The capability is not in `capabilities.json` yet; the product will need it, but it has not been admitted to the graph. |
| `Planned` | The capability is `Accepted`, its `delivered_by_milestone` is `Planned`, and there is no plan yet — or the plan stub is `Draft`. The work is sequenced for a milestone but is not on the queue. |
| `Ready` | The capability is `Accepted`, its `delivered_by_milestone` is `Planned` or `Active`, a `Ready` or `Awaiting Approval` plan exists, and the task that advances the capability is in `tasks.json` with status `Ready`. Dependencies are satisfied. |
| `InProgress` | A task with this capability in its `delivered_by_tasks` is `In Progress` in `tasks.json`. Implementation has started on a branch. |
| `Delivered` | Implementation is merged on a branch with a commit; the implementation report exists; the `milestones.json` evidence array lists the commit. The receipts have not been linked in every consumer, or one of the consumer milestones is still open. |
| `Verified` | The capability is `Delivered`, **and** every `consumed_by` dependency is also `Verified`, **and** the receipts are linked. |
| `Blocked` | A `Blocked` task in `tasks.json` references the capability in its `blocker` field. The next task cannot start until the blocker is resolved. |
| `Deferred` | The capability's `delivered_by_milestone` is `Deferred` in `milestones.json`. The work is in the backlog, not in the active queue. |

## Mapping Rule

The `completion_status` is derived from the canonical
`status`, the `delivered_by_milestone` status in
`milestones.json`, the matching plan status (if any), and
the matching task status (if any) in `tasks.json`. The
derivation is rule-based, not hand-typed, and is recorded
per capability in
[`.ai/state/capabilities.json`](./.ai/state/capabilities.json).

| Existing state | Mapped `completion_status` | Reason |
| --- | --- | --- |
| `status: Done` | `Verified` | The canonical state is `Done`; the capability is implemented, validated, and evidenced in the closed-milestone receipts. |
| `status: Accepted`, milestone `Done`, evidence linked in `milestones.json`, all `consumed_by` deps `Verified` | `Verified` | The capability is in place and consumed by closed milestones. |
| `status: Accepted`, milestone `Done`, no evidence link in `milestones.json` | `Delivered` | The implementation is recorded but the receipts have not been linked. |
| `status: Accepted`, milestone `Planned` or `Active`, plan `Ready` or `Awaiting Approval`, task `Ready` in `tasks.json` | `Ready` | The plan is on the queue; dependencies are satisfied. |
| `status: Accepted`, milestone `Planned`, plan `Draft` (or no plan yet) | `Planned` | Sequenced for a milestone, plan not yet promoted. |
| `status: Accepted`, task `Blocked` in `tasks.json` references the capability in its `blocker` field | `Blocked` | A named blocker must be resolved. |
| `status: Accepted`, milestone `Deferred` | `Deferred` | The roadmap defers the milestone. |
| Not in `capabilities.json` | `NotStarted` | Reserved for capabilities the product will need but which have not yet been admitted to the graph. |

## Per-Step Checklist (User Journey)

Each row below maps a step in the **Core User Journey** (above)
to the capabilities that make it work, the current state of
those capabilities, the milestone that delivers the missing
ones, the blocking dependencies, and the evidence that
proves the step is complete. The required-capabilities list
is the union of the matching feature's
`depends_on_capabilities` in
[`.ai/state/features.json`](./.ai/state/features.json);
`Delivered / Required` counts the capabilities whose
`completion_status` is `Verified` or `Delivered`.

| # | Step | Required C-IDs | Status (current) | Delivered / Required | Milestone | Dependencies | Evidence required |
| - | ---- | -------------- | ---------------- | -------------------- | --------- | ------------ | ----------------- |
| 1 | **Register project** | C-016, C-019, C-020 | Verified, Ready, Verified | **2 / 3** | M3 | C-016 (Planned) | IProjectService + durable IProjectStore (M4-A); project-registration page rendered through INavigationService. |
| 2 | **Create task** | C-019, C-020 (and F-001) | Ready, Verified | **1 / 2** | M3 | C-016 (Planned) | Task entity + persistence (M3); task-create page in the navigation registry. |
| 3 | **Prepare worktree** | C-003, C-005, C-019, C-020 | Planned, Planned, Ready, Verified | **1 / 4** | M5 | C-003 (Planned) | IWorktreeProvider + IGitProvider wired through the family-scoped registry; worktree page composed from the navigation registry. |
| 4 | **Choose runtime and model** | C-002, C-010, C-019, C-020 | Planned, Planned, Ready, Verified | **1 / 4** | M4-C, M6 | C-002, C-010 (Planned) | IProviderRegistry (M4-C) + IAgentRuntimeProvider smoke (M4-D) and runtime picker page (M6) in the navigation registry. |
| 5 | **Launch agent** | C-002, C-012, C-019, C-020, C-021 | Planned, Planned, Ready, Verified, Planned | **1 / 5** | M4-D, M6 | C-002, C-012, C-021 (Planned) | IProcessRunner (M4-A) + IAgentRuntimeProvider smoke (M4-D) + run-launch page (M6) + IStreamingChannel (M6). |
| 6 | **Observe execution** | C-002, C-012, C-019, C-020, C-021 | Planned, Planned, Ready, Verified, Planned | **1 / 5** | M6 | C-002, C-012, C-021 (Planned) | IStreamingChannel (M6); run-observation page in the navigation registry. |
| 7 | **Stop / cancel / resume / recover** | C-002, C-012, C-019, C-020, C-021 | Planned, Planned, Ready, Verified, Planned | **1 / 5** | M6 | C-002, C-012, C-021 (Planned) | Cancellation token owned by the run service (M6); IHistoryStore persists the transition. |
| 8 | **Inspect diff** | C-003, C-019, C-020 | Planned, Ready, Verified | **1 / 3** | M6, M7 | C-003 (Planned) | IGitProvider diff/log (M4-D) + diff-inspector page (M6/M7) in the navigation registry. |
| 9a | **Submit for review** | C-007, C-013, C-019, C-020 | Planned, Planned, Ready, Verified | **1 / 4** | M7 | C-007, C-013 (Planned) | IReviewProvider (M7) + ICredentialVault (M4-A); review-page in the navigation registry. |
| 9b | **Run quality gate** | C-006, C-013, C-019, C-020 | Planned, Planned, Ready, Verified | **1 / 4** | M7 | C-006, C-013 (Planned) | IQualityGateProvider (M7) + ICredentialVault (M4-A); quality-gate page in the navigation registry. |
| 10 | **Approve / reject / merge / retain / clean up** | C-003, C-005, C-019, C-020 | Planned, Planned, Ready, Verified | **1 / 4** | M5, M7 | C-003, C-005 (Planned) | IWorktreeProvider dispose (M5) + IGitProvider merge (M4-D) + disposition service (M7); disposition page in the navigation registry. |
| 12 | **Execution and task history** | C-017, C-019, C-020 | Planned, Ready, Verified | **1 / 3** | M6 | C-017 (Planned) | IHistoryStore (M6); history page in the navigation registry. |
| 13 | **Bounded autonomous loops** | C-002, C-006, C-008, C-019, C-020 | Planned, Planned, Planned, Ready, Verified | **1 / 5** | M8 | C-002, C-006, C-008 (Planned) | IAutonomousLoopProvider (M8); autonomous-loop page in the navigation registry. |
| 14 | **Coordinate multiple agents** | C-005, C-008, C-009, C-019, C-020 | Planned, Planned, Planned, Ready, Verified | **1 / 5** | M8 | C-005, C-008, C-009 (Planned) | IOrchestrationProvider (M8); orchestration page in the navigation registry. |

> **Steps 11 and the un-numbered 9c–11 transitions** are
> covered by step 10's "Approve / reject / merge / retain /
> clean up" row above. The Core User Journey list in this
> file counts the steps; this table groups step 9 into
> review (9a) and quality gate (9b) so each capability
> dependency is named.

## Overall Progress

`Verified + Delivered` capabilities: **2 of 21** (C-001
`IProvider base contract`; C-020 `Design system catalogue`).
The remaining 19 capabilities are sequenced for milestones
M2 through M8; one (C-019 `INavigationService`) is `Ready`
(M2.2 plan `Awaiting Approval`; T-002 `Ready`); the other
18 are `Planned`.

# What the Product Is Not

The product is explicitly **not**:

- A coding agent. The product orchestrates agents; it does not
  generate code itself.
- A chat app. The product's surfaces are desktop-first, dense,
  keyboard-driven, and tuned for engineering work; chat-style
  interactions are not a target.
- A cloud-only service. The product is a Windows application that
  runs on the developer's machine; it is not a hosted
  multi-tenant system.
- A mobile or touch-first surface. The product's UI is designed
  for Windows desktop.
- A replacement for the host's existing tools. PowerShell, Git
  for Windows, Windows Terminal, WSL, Ollama, Claude, and OpenAI
  remain installed and used; the product integrates them through
  process boundaries, not by replacing them.
- A marketing site, a notebook, a docs generator, or a generic
  AI surface. The product is a single-purpose developer tool.

# Success Criteria

The product is successful when:

- A developer can register a Git repository and launch an agent
  against it in fewer steps than assembling the workflow by
  hand.
- A run can be cancelled mid-stream and recovered without losing
  the worktree or the run history.
- A worktree's diff can be reviewed and gated from a single
  surface, with the review and gate results persisted across
  restarts.
- Every external tool is integrated through a provider contract;
  swapping one tool for another does not require code changes
  outside the provider's implementation project.
- The architecture tests in
  `tests/AiEng.Platform.ArchitectureTests/` fail the build on
  any direct bypass of the platform's abstractions.
- The platform is accessible (keyboard navigation, visible focus,
  ARIA labels, loading and error states) and the accessibility
  audit passes.
- The progressive self-dogfooding matrix in
  [`ROADMAP.md`](./ROADMAP.md) § 4 is fully satisfied.

# Current Delivery Stage

**Active milestone:** M2 — Application Shell and Navigation
(`completion_status: Ready` for the `INavigationService`
capability, C-019; the shell foundation C-020 is
`Verified`).

**Last closed milestone:** M1 — Design System Core (closed
2026-07-10; first commits
`1722bd235830cfd8b180191953116c058c92edef` and
`2ba1fad3cc45bee513ba38c7269e024bf8667ef9` on `master`).

**M2 slices:** M2.1 — Application Shell Foundation
**Delivered** 2026-07-11 (commits
`ef1063c`, `de082fd` on
`feature/m2-1-application-shell`; see
`implementation-report-m2-1-application-shell-foundation.md`).
M2.2 — Navigation Registry and Sidebar is the next
`Ready` slice (plan in
[`.ai/plans/M2.2-navigation-registry-sidebar.md`](./.ai/plans/M2.2-navigation-registry-sidebar.md)
status `Awaiting Approval`; T-002 in
[`.ai/state/tasks.json`](./.ai/state/tasks.json)
status `Ready`). M2.3, M2.4 remain `Draft` plan stubs;
M2.5, M2.6 are `Deferred` summary entries.

# Link to Delivery State

The product's delivery state is the single landing point for any
future AI session:

- **Product definition:** this file (`PRODUCT.md`).
- **Master delivery plan:**
  [`.ai/plans/master-delivery-plan.md`](./.ai/plans/master-delivery-plan.md).
- **Architecture:** [`ARCHITECTURE.md`](./ARCHITECTURE.md).
- **Decisions:** [`DECISIONS.md`](./DECISIONS.md).
- **Roadmap:** [`ROADMAP.md`](./ROADMAP.md).
- **Current state (one-page snapshot):**
  [`.ai/state/current.md`](./.ai/state/current.md).
- **Live task board:**
  [`.ai/state/task-board.md`](./.ai/state/task-board.md).
- **Most recent handoff:**
  [`.ai/handoffs/latest.md`](./.ai/handoffs/latest.md).
- **Approved plans (canonical):**
  [`.ai/plans/`](./.ai/plans/).
