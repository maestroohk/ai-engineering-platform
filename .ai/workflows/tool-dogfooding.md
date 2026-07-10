# .ai/workflows/tool-dogfooding.md

> How the AI Engineering Platform team uses external tools
> while building this repository. This workflow exists to
> keep three distinct activities clearly separated, to set
> the bar for invoking any tool, to keep dogfooding off
> the critical path until the platform is ready, and to
> define the **evolution path** every dogfooded tool
> follows from "external tool" to "native platform
> capability".

---

## 1. Two Distinct Concepts

This workflow deliberately distinguishes two activities that
look similar but are not the same.

### 1.1 Development-Time Dogfooding

The development team uses an external tool **while building
this repository**. The tool is invoked by a developer, in a
shell, on the developer's machine. The platform does not
invoke the tool; the developer does. The artefact is the
developer's experience, captured in notes and commit
messages, not in platform code.

Examples:

- A developer pastes a UI screenshot into Lavish Axi for
  review.
- A developer runs Treehouse to create a worktree for an
  isolated experiment.
- A developer runs No Mistakes against a branch as a
  quality gate.
- A developer runs GNHF on a bounded, repetitive task.

### 1.2 Product Integration

The application invokes an external tool **as a platform
Provider**. The tool is registered through the provider
model, the UI lists it through the provider registry, and
its behaviour is exercised by the contract test. The tool
is part of the platform.

Examples (future):

- The platform invokes Lavish Axi through an
  `IReviewProvider` to review a UI artefact inside the
  application.
- The platform invokes Treehouse through an
  `IWorktreeProvider` to create a session workspace.
- The platform invokes No Mistakes through an
  `IQualityGateProvider` before merging a run.

The two concepts **must not be confused**. A
development-time use of a tool is not product integration.
A product integration is not "we ran it once and liked it".

This workflow governs the first concept. Product
integration is governed by `.ai/prompts/provider.md` and
`.ai/workflows/provider-onboarding.md`.

### 1.3 Platform Self-Dogfooding (per ADR-013)

The platform dogfoods **its own abstractions** as they
mature. Every milestone that delivers a reusable
capability is consumed by every later milestone that
needs it. Later milestones must not bypass earlier
platform abstractions with temporary direct
implementations. This is **internal** dogfooding: the
product using its own contracts, registries, and
abstractions, not external tools.

Examples:

- M3 introduces the `IProviderRegistry`. M4, M5, M7, and
  M8 resolve every provider through the registry, not by
  importing a provider implementation directly.
- M4 introduces the `IProcessRunner` abstraction. M5,
  M6, M7, and M8 spawn every shell process through
  `IProcessRunner`, never through a direct
  `Process.Start` call in a non-Infrastructure project.
- M6 introduces the `IGitProvider`. The
  `NativeWorktreeProvider` in M8 consumes `IGitProvider`
  through the registry, not by importing `GitProvider`
  directly.

Platform self-dogfooding is **enforced by code**, not by
convention. The progressive self-dogfooding matrix in
`ROADMAP.md` § 4 records the contract; the
`AiEng.Platform.ArchitectureTests/SelfDogfooding/` folder
holds the test classes that fail the build on bypass.

The matrix is capability-oriented. The rows map to the eight
provider families (`IAgentRuntimeProvider`, `IGitProvider`,
`ITerminalProvider`, `IWorktreeProvider`,
`IQualityGateProvider`, `IReviewProvider`,
`IAutonomousLoopProvider`, `IOrchestrationProvider`) defined
in `ARCHITECTURE.md` § 3.3 and `docs/provider-guidelines.md`
§ 2. Vague family names (`Assistant`, `Deployment`,
`Internal`, `Workspace`) are rejected per ADR-012.

The two dogfooding disciplines (external-tool dogfooding
and platform self-dogfooding) must not be confused. An
external-tool dogfooding event (a developer running
Treehouse in a shell) is **not** the same as a milestone
that consumes the `IWorktreeProvider` family. The former
is governed by this workflow; the latter is governed by
the matrix in `ROADMAP.md` § 4.

### 1.4 The Dogfooding Evolution Path

Every tool the team dogfoods follows the **same
four-stage evolution path**. A tool that does not move
through the path is, by definition, a tool the team
has chosen to use but not to adopt. That is a
deliberate choice with its own cost (no
platform-level integration, no contract test, no
upgrade story) and the choice is recorded per
tool below.

The four stages, in order:

| Stage | Name | What it means |
| --- | --- | --- |
| 1 | **External tool** | The tool is on the developer's machine. The developer invokes it in a shell. The platform does not know the tool exists. The tool produces value for the developer but no platform capability is registered. |
| 2 | **Manual usage** | The tool is invoked by the developer through a documented procedure. The procedure is recorded in a workflow or a runbook. The platform does not invoke the tool, but the team has a known-good way of using the tool that other sessions can repeat. |
| 3 | **Integrated provider** | The tool is wrapped behind a platform `IProvider` interface (`IWorktreeProvider`, `IReviewProvider`, `IQualityGateProvider`, `IAgentRuntimeProvider`, etc.). The platform invokes the tool. The tool is registered through the `IProviderRegistry`. A contract test exercises the provider in CI. |
| 4 | **Native platform capability** | The capability the tool provided is now implemented inside the platform itself, with the external tool as a fallback. The platform no longer requires the external tool to operate. The external tool may be retained as an optional alternate. |

The evolution is one-way. A tool that has reached
Stage 4 is never re-opened to an earlier stage; the
fallback to the external tool is opt-in, not
opt-out. A tool that has been rejected at any stage
is recorded as a rejected idea in
[`.ai/backlog/ideas.md`](./../backlog/ideas.md) so the
team remembers the decision.

The current stage of every tool the team is
tracking is in
[`.ai/state/providers.json`](./../state/providers.json);
the `lifecycle_state` field of each provider row
encodes the stage. The transition between stages
is a milestone deliverable, recorded in
`ROADMAP.md`.

A tool can stall at any stage. Stalls are
recorded as **Deferred** in `.ai/state/providers.json`
and are revisited at the milestone that triggers
the stage transition for that tool family.

## 2. The Tool Invocation Policy

A tool is invoked only when **all** of the following are
true:

- The workflow explicitly identifies the tool for the
  stage of the project.
- The tool's prerequisites (binary, license, network) have
  been verified.
- The user has explicitly approved the action in the
  current session.
- The exact command is shown in the conversation **before**
  the command is executed.
- The action runs in an isolated branch or worktree where
  the action is destructive or branch-mutating.
- A cancellation and cleanup plan is understood (the tool
  can be killed, the worktree can be removed, the branch
  can be deleted).
- The tool's actions are observable (logs, output, exit
  code) and the session records what the tool did.
- The session confirms the upstream repository of the
  tool was not modified (unless that is the explicit
  purpose of the dogfooding event).

A tool that fails any of these conditions is not invoked.
The session asks the human to clarify or to perform the
invocation themselves.

## 3. Staged Dogfooding Plan

The plan below maps dogfooding events to the milestones
where they become safe. Earlier events are not attempted
before the milestone is reached. The plan is the
authorisation to use a tool, not a mandate.

### M1 / M2 — UI and Application Shell

When a working design-system surface or a dashboard HTML
preview exists:

- **Lavish Axi** may be used to review UI artefacts
  where practical.
- The session records the review findings in an
  `implementation-report.md` (or a `review-report.md`).
- The platform does **not** yet implement Lavish Axi as
  a Provider. Product integration is a future task.
- Prerequisite: a human has provided access to Lavish
  Axi (account, license, or local installation).

### First Real Isolated Implementation Task

When Git worktrees are useful for an isolated task:

- **Treehouse** may be used externally to create an
  isolated development worktree.
- The session records the exact commands used.
- The session confirms the worktree is local to the
  developer's machine and does not modify the upstream
  repository.
- Product integration of Treehouse is a future task.

### First Tested Production Feature

When the repository has a real build and test suite:

- **No Mistakes** may be initialised and run.
- The session shows the resulting quality-gate workflow
  to the user.
- The session does **not** make No Mistakes mandatory
  until the repository is ready (the build is stable,
  the test suite is green, the team has agreed the
  gate).
- Product integration of No Mistakes is a future task.

### Later Bounded Refactor or Repetitive Task

For a bounded, repetitive task that is not
architecture-sensitive:

- **GNHF** may be used with explicit caps (run time,
  token budget, scope of files touched).
- The session preserves the branch for human review.
- The session does **not** use autonomous loops for
  architecture-sensitive work. ADR-level changes
  always involve a human in the loop.

### Multi-Agent Experimentation

For experimental multi-agent work:

- **Firstmate** may be used only after the native
  single-agent workflow and the worktree model are
  stable.
- The session uses Firstmate through WSL where
  required.
- The session treats Firstmate as an experiment until
  the platform's orchestration contract exists
  (a future milestone, not yet in `ROADMAP.md`).
- Product integration of Firstmate is a future task.

## 4. Per-Tool Dogfooding Profile

This section gives each tool the team is
tracking its own profile. The profile records the
tool's current stage in the evolution path
(§ 1.4), the milestone that activates its next
transition, the manual usage procedure, the
future platform integration plan, the success
criteria for the transition, and the
retirement strategy if the tool is rejected or
superseded.

The profile is the source of truth for "how
do I use tool X right now, and what does the
platform do with it later". The per-event
checklist (§ 5) records the **evidence** for
the profile; the profile records the
**expectation**.

### 4.1 Lavish Axi (UI review)

| Field | Value |
| --- | --- |
| Current stage | External tool (Stage 1). Manual usage (Stage 2) is in development. |
| Activation milestone | M1 closeout checkpoint (deferred — tool not installed on host). |
| Manual usage | A developer pastes a UI screenshot into the Lavish Axi CLI; the tool returns findings; the developer records the findings in a review report. |
| Future platform integration | `IReviewProvider` in M4-C; provider implementation in M7. The platform invokes Lavish Axi through a configurable endpoint. |
| Success criteria for Stage 2 | (a) the manual usage procedure is documented in a runbook; (b) the runbook has been used by at least one developer on a real UI artefact; (c) the findings are recorded in `.ai/reviews/`. |
| Success criteria for Stage 3 | (a) the `IReviewProvider` is in place; (b) the Lavish Axi implementation is registered through `IProviderRegistry`; (c) the contract test in `AiEng.Platform.ArchitectureTests` exercises the provider in CI. |
| Success criteria for Stage 4 | (a) the platform's native review capability produces equivalent findings to Lavish Axi on the M7 test corpus; (b) Lavish Axi becomes opt-in, not required. |
| Retirement strategy | If Lavish Axi is rejected at any stage, the rejection is recorded in `.ai/backlog/ideas.md` with the reason (e.g. "license incompatible with the platform's distribution model", "tool abandoned by upstream"). The next-stage deliverable is replaced by a different tool or a native implementation. |

### 4.2 Treehouse (Git worktree)

| Field | Value |
| --- | --- |
| Current stage | External tool (Stage 1). Manual usage (Stage 2) is in development. |
| Activation milestone | M2 closeout checkpoint. |
| Manual usage | A developer runs `treehouse new <branch>` to create an isolated worktree; the worktree is local; the developer reports the commands in the session handoff. |
| Future platform integration | `IWorktreeProvider` in M4-C; provider implementation in M5. The platform's `NativeWorktreeProvider` (M8) supersedes Treehouse for the default path. |
| Success criteria for Stage 2 | (a) the manual usage procedure is documented; (b) at least one M2 closeout session has used Treehouse for an isolated change. |
| Success criteria for Stage 3 | (a) the `IWorktreeProvider` is in place; (b) the Treehouse implementation is registered; (c) the contract test passes. |
| Success criteria for Stage 4 | (a) the `NativeWorktreeProvider` in M8 implements the worktree model using only `IGitProvider` and `IProcessRunner`; (b) the platform's defaults use the native provider. |
| Retirement strategy | Once the native provider is in M8, Treehouse is retained as an alternate provider. The alternate is opt-in; the platform's first-run experience does not require it. |

### 4.3 No Mistakes (quality gate)

| Field | Value |
| --- | --- |
| Current stage | External tool (Stage 1). |
| Activation milestone | M7 (Review and Quality Gates). |
| Manual usage | A developer runs `no-mistakes` against a branch; the tool reports the gate result; the developer records the result in a quality-gate review. |
| Future platform integration | `IQualityGateProvider` in M4-C; provider implementation in M7. The platform invokes No Mistakes before merging a run. |
| Success criteria for Stage 2 | (a) the manual usage procedure is documented; (b) at least one M1 or M2 closeout session has run No Mistakes against a stable branch. |
| Success criteria for Stage 3 | (a) the `IQualityGateProvider` is in place; (b) the No Mistakes implementation is registered; (c) the contract test passes; (d) the platform renders the gate result in the dashboard. |
| Success criteria for Stage 4 | (a) the platform's native quality gate produces equivalent results to No Mistakes on the M7 test corpus; (b) No Mistakes becomes opt-in. |
| Retirement strategy | If No Mistakes is rejected, the platform's native quality gate (M7) is the only gate. The rejection is recorded in `.ai/backlog/ideas.md`. |

### 4.4 GNHF (bounded autonomous task)

| Field | Value |
| --- | --- |
| Current stage | External tool (Stage 1). |
| Activation milestone | M8 (Autonomous Loops and Orchestration). |
| Manual usage | A developer invokes GNHF through WSL on a bounded task with explicit caps (run time, token budget, file scope). The session preserves the branch for human review. |
| Future platform integration | `IAutonomousLoopProvider` in M4-C; provider implementation in M8. The platform runs bounded autonomous tasks through the registry. |
| Success criteria for Stage 2 | (a) the manual usage procedure is documented; (b) at least one M2-M7 task has used GNHF for a bounded refactor. |
| Success criteria for Stage 3 | (a) the `IAutonomousLoopProvider` is in place; (b) the GNHF implementation is registered; (c) the contract test exercises the cap-enforcement path. |
| Success criteria for Stage 4 | (a) the platform's native autonomous loop implements equivalent bounded execution; (b) GNHF is an alternate provider. |
| Retirement strategy | The native autonomous loop is the platform's default; GNHF is opt-in. The opt-in path is documented. |

### 4.5 Firstmate (multi-agent orchestration)

| Field | Value |
| --- | --- |
| Current stage | External tool (Stage 1). |
| Activation milestone | M8 (Autonomous Loops and Orchestration). |
| Manual usage | A developer runs Firstmate through WSL on an experimental multi-agent task. The session records the orchestration graph. |
| Future platform integration | `IOrchestrationProvider` in M4-C; provider implementation in M8. The platform's native orchestration supersedes Firstmate. |
| Success criteria for Stage 2 | (a) the manual usage procedure is documented; (b) at least one M8 experiment has used Firstmate. |
| Success criteria for Stage 3 | (a) the `IOrchestrationProvider` is in place; (b) the Firstmate implementation is registered; (c) the contract test exercises the orchestration graph contract. |
| Success criteria for Stage 4 | (a) the platform's native orchestration implements equivalent graph execution; (b) Firstmate is an alternate provider. |
| Retirement strategy | The native orchestration is the platform's default; Firstmate is opt-in. The opt-in path is documented. |

## 5. Per-Event Checklist

Every dogfooding event records the following in the
`implementation-report.md` (or in a dedicated
`session-handoff.md` if the event is a session in
itself):

- [ ] The tool used and the version.
- [ ] The prerequisite check that was performed.
- [ ] The exact commands, shown before execution.
- [ ] The branch or worktree the event ran in.
- [ ] The user's explicit approval (cite the message).
- [ ] The cancellation and cleanup plan.
- [ ] The observable logs, output, or exit codes.
- [ ] A summary of what the tool did.
- [ ] Confirmation that the tool's upstream repository
      was not modified (unless that was the explicit
      purpose).
- [ ] Any follow-up the event revealed.

## 5. Updating the Roadmap

Each milestone that introduces a dogfooding checkpoint
gains a short "Dogfooding checkpoint" subsection in
`ROADMAP.md`. The subsection lists the tools that may be
used at that milestone, the conditions, and the limits.
The subsection is added in the same change that adds
the checkpoint to this workflow, so the two stay
consistent.

## 6. Anti-Patterns

- Invoking a tool because it is installed.
- Using a tool without the user knowing.
- Treating a development-time use as product integration.
- Treating a product integration as a one-off use.
- Hiding the exact command from the conversation.
- Running an autonomous loop on architecture-sensitive
  code.
- Failing to verify that the tool's upstream repository
  was not modified.
- Recording a dogfooding event without the per-event
  checklist.
