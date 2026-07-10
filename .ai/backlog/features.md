# Backlog — Features

> **User-facing capabilities the platform must provide.**
>
> A feature is what the user sees. A feature is
> delivered by one or more milestones. A feature
> that is `Done` is one the user can use today.
>
> See [README.md](./README.md) for the rules.

---

## Format

- **ID** — `F-###`.
- **Title** — a one-line statement of what the
  user can do.
- **Status** — `Proposed`, `Accepted`, `Deferred`,
  `Rejected`, or `Done`.
- **Source / Traceability** — the document that
  introduced or implied the feature. The
  traceability is the audit trail.
- **Notes** — the contributor's thinking.
- **Milestones** — the milestones that deliver the
  feature.

---

<!-- New features are appended below. -->

## F-001 — Register a Git Repository

- **Title:** A developer can register a Git
  repository by providing a name and a folder
  path; the platform owns the `Project` entity
  and lists it on the projects page.
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`PRODUCT.md`](./../../PRODUCT.md) § "Final
  Product Capabilities" (1). [`ROADMAP.md`](./../../ROADMAP.md)
  § M3.
- **Notes:** The smallest piece of state the
  platform needs to be useful on its own. M3
  ships an in-memory store; M4-A migrates to
  durable storage behind the same contract.
- **Milestones:** M3, M4-A.

## F-002 — Create a Coding Task for a Registered Project

- **Title:** A developer can create a coding task
  (a description in human terms) for a registered
  project; the platform owns the `Task` entity.
- **Status:** `Proposed`.
- **Source / Traceability:**
  [`PRODUCT.md`](./../../PRODUCT.md) § "Final
  Product Capabilities" (2), § "Core User
  Journey" (step 2).
- **Notes:** A `Task` is the input to the
  worktree preparation and the agent launch.
  The task's lifecycle (open, in-progress,
  review, gated, approved, merged, cleaned-up)
  is the disposition the platform owns. The
  feature is sequenced after F-001 and before
  the worktree feature; the roadmap records
  the slice.
- **Milestones:** TBD (likely M3 or M6).

## F-003 — Prepare an Isolated Worktree

- **Title:** A developer can prepare a Git worktree
  for a task; the worktree is the unit of
  isolation for the run.
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`PRODUCT.md`](./../../PRODUCT.md) § "Final
  Product Capabilities" (3). [`ROADMAP.md`](./../../ROADMAP.md)
  § M5.
- **Notes:** The worktree feature is delivered
  by the `IWorktreeProvider` family; the
  native implementation is the baseline. M5
  ships the native worktree; the Treehouse
  product integration is added later when
  the user opts in.
- **Milestones:** M5.

## F-004 — Select an AI Runtime and Model

- **Title:** A developer can pick an AI runtime
  (Ollama Launch, Claude-compatible,
  OpenAI-compatible, Codex) and a model; the
  platform lists the registered runtimes and
  the available models for each.
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`PRODUCT.md`](./../../PRODUCT.md) § "Final
  Product Capabilities" (4). [`ROADMAP.md`](./../../ROADMAP.md)
  § M6.
- **Notes:** The runtime picker is the
  user-facing surface over the
  `IAgentRuntimeProviderRegistry`; the
  model picker is the surface over the
  selected runtime's model list. The
  registry's `Enabled` and `Healthy`
  states drive the picker's affordances.
- **Milestones:** M6.

## F-005 — Launch an Agent Inside a Worktree

- **Title:** A developer can launch the selected
  agent inside the worktree; the platform owns
  the process boundary.
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`PRODUCT.md`](./../../PRODUCT.md) § "Final
  Product Capabilities" (5). [`ROADMAP.md`](./../../ROADMAP.md)
  § M4-D, § M6.
- **Notes:** The launch is mediated by
  `IProcessRunner`; no code outside
  `AiEng.Platform.Infrastructure/Process/`
  may call `Process.Start`. The launch is
  cancellable mid-stream.
- **Milestones:** M4-D, M6.

## F-006 — Observe Streamed Output and Status

- **Title:** A developer can watch the agent's
  stdout, stderr, exit code, and status
  transitions in a terminal panel inside the
  application.
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`PRODUCT.md`](./../../PRODUCT.md) § "Final
  Product Capabilities" (6). [`ROADMAP.md`](./../../ROADMAP.md)
  § M6.
- **Notes:** Streaming uses
  `IAsyncEnumerable<ProcessEvent>`; status
  transitions are reported through the run
  service. The terminal panel is rendered
  through `AppTerminalPanel` and
  `AppTerminalLine`.
- **Milestones:** M6.

## F-007 — Stop, Cancel, Resume, or Recover a Run

- **Title:** A developer can cancel a run
  mid-stream, resume a cancelled run, or
  recover a crashed run; the platform owns the
  cancellation token and the recovery
  semantics.
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`PRODUCT.md`](./../../PRODUCT.md) § "Final
  Product Capabilities" (7). [`ROADMAP.md`](./../../ROADMAP.md)
  § M6.
- **Notes:** Cancellation flows through the
  `CancellationToken` on the launch call.
  A run that was cancelled or crashed can be
  recovered. The recovery is a re-launch
  against the same worktree with the same
  prompt.
- **Milestones:** M6.

## F-008 — Inspect Files and Git Changes

- **Title:** A developer can view the diff and
  the log of the worktree the agent ran in;
  the platform renders the changes the agent
  produced.
- **Status:** `Proposed`.
- **Source / Traceability:**
  [`PRODUCT.md`](./../../PRODUCT.md) § "Final
  Product Capabilities" (8).
- **Notes:** The diff viewer consumes
  `IGitProvider` (M4-D) to read the
  worktree's state. The viewer is built on
  the design system's primitives. The full
  diff-viewer feature lands in M7 with the
  review surface; the M6 launch surface
  shows a simple file list.
- **Milestones:** M6 (basic), M7 (full).

## F-009 — Submit Work for Review

- **Title:** A developer can submit the
  worktree for review through the registered
  review provider (Lavish Axi, the native
  baseline, or another opted-in tool).
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`PRODUCT.md`](./../../PRODUCT.md) § "Final
  Product Capabilities" (9). [`ROADMAP.md`](./../../ROADMAP.md)
  § M7.
- **Notes:** The review submission routes
  the worktree to the review provider
  through the `IReviewProvider` family. The
  findings are persisted and rendered
  through `AppReviewPanel` and
  `AppReviewFindingList`.
- **Milestones:** M7.

## F-010 — Run Quality Gates

- **Title:** A developer can run a quality gate
  against the worktree through the registered
  quality-gate provider (No Mistakes, the
  native baseline, or another opted-in tool);
  the gate's pass/fail is rendered.
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`PRODUCT.md`](./../../PRODUCT.md) § "Final
  Product Capabilities" (10). [`ROADMAP.md`](./../../ROADMAP.md)
  § M7.
- **Notes:** The gate submission routes the
  worktree to the quality-gate provider
  through the `IQualityGateProvider` family.
  The pass/fail is rendered through
  `AppQualityGateBadge`.
- **Milestones:** M7.

## F-011 — Approve, Reject, Merge, Retain, or Clean Up

- **Title:** A developer can approve or reject
  the work, merge the worktree back into the
  source repository, retain it for further
  work, or clean it up; the platform owns the
  disposition.
- **Status:** `Proposed`.
- **Source / Traceability:**
  [`PRODUCT.md`](./../../PRODUCT.md) § "Final
  Product Capabilities" (11), § "Core User
  Journey" (step 10).
- **Notes:** The disposition is the last
  step in the user journey. The merge
  operation uses `IGitProvider` (M4-D) to
  perform the merge; the cleanup operation
  removes the worktree through
  `IWorktreeProvider` (M5).
- **Milestones:** TBD (likely M7 or M8).

## F-012 — Execution and Task History

- **Title:** The platform persists every run,
  every review, every gate, and every
  disposition; the history survives an
  application restart.
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`PRODUCT.md`](./../../PRODUCT.md) § "Final
  Product Capabilities" (12). [`ROADMAP.md`](./../../ROADMAP.md)
  § M6 (`IHistoryStore`).
- **Notes:** `IHistoryStore` lands in M6
  alongside the run service. The history is
  surfaced through `AppRunHistory` and
  similar components.
- **Milestones:** M6.

## F-013 — Bounded Autonomous Loops

- **Title:** The platform can drive a series of
  launches and gates under a registered
  autonomous-loop provider (GNHF, the native
  baseline, or another opted-in tool); the
  loop is bounded and recoverable.
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`PRODUCT.md`](./../../PRODUCT.md) § "Final
  Product Capabilities" (13). [`ROADMAP.md`](./../../ROADMAP.md)
  § M8.
- **Notes:** The autonomous loop consumes
  the `IAgentRuntimeProvider` and
  `IQualityGateProvider` families through
  the registry. The loop is bounded by
  configurable limits (iterations, time,
  tokens).
- **Milestones:** M8.

## F-014 — Multi-Agent Orchestration

- **Title:** The platform can orchestrate a
  graph of agents, worktrees, and gates under
  a registered orchestration provider
  (Firstmate through WSL, the native baseline,
  or another opted-in tool); the orchestration
  is observable and recoverable.
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`PRODUCT.md`](./../../PRODUCT.md) § "Final
  Product Capabilities" (14). [`ROADMAP.md`](./../../ROADMAP.md)
  § M8.
- **Notes:** The orchestrator consumes
  `IAutonomousLoopProvider`, `IWorktreeProvider`,
  and the other families through the registry.
  The orchestration is rendered through
  `AppOrchestrationGraph`.
- **Milestones:** M8.

---

## Last Updated

- **2026-07-10** — created in the M0.5 architecture
  refinement with the 14 features derived from
  `PRODUCT.md` § "Final Product Capabilities" and
  `ROADMAP.md`. Twelve are `Accepted` (matching
  `ROADMAP.md`); two (`F-002`, `F-008`, `F-011`)
  are `Proposed` pending the milestone that
  delivers them.
