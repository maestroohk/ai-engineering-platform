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

**Active milestone:** M2 — Application Shell and Navigation.

**Last closed milestone:** M1 — Design System Core (closed
2026-07-10; first commits
`1722bd235830cfd8b180191953116c058c92edef` and
`2ba1fad3cc45bee513ba38c7269e024bf8667ef9` on `master`).

**Next planned slice:** M2.1 — Application Shell Skeleton
(plan in
[`.ai/plans/M2.1-application-shell-skeleton.md`](./.ai/plans/M2.1-application-shell-skeleton.md),
status `Awaiting Approval`).

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
