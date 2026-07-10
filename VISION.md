# VISION.md

> **The destination. The "why" that almost never changes.**
>
> `VISION.md` describes the platform we are building toward.
> It is the document an engineer reads when they want to
> understand what the project is for. It is **not** a
> description of the platform as it exists today —
> `PRODUCT.md` plays that role. It is also **not** the
> roadmap — `ROADMAP.md` sequences the work; the vision
> is the destination the roadmap serves.
>
> A change to `VISION.md` is a project-founding change.
> The document is expected to change only when the
> project's purpose changes. The current content is the
> destination as understood on **2026-07-10** and
> recorded in the M0.5 architecture refinement.

---

## 1. Why This Project Exists

A professional developer who uses a coding agent today
assembles the workflow by hand. They find the repository,
create a worktree, pick a runtime and a model, launch the
agent from a terminal, watch the output scroll by in a
separate window, cancel and recover through shell signals
that are easy to get wrong, inspect the diff in another
window, run quality gates from another terminal, and merge
or clean up by hand. The workflow is real; the toolchain
is fragmented; the operational state — the run, its
output, its review, its disposition — is scattered across
terminals, shells, and the developer's memory. A restart
loses the run history; a misclick loses the worktree; a
subtle bug in the launch invocation produces a model that
drifts from the one the developer intended.

This project exists to **replace that ad-hoc toolchain
with a single, native, recoverable, observable surface
that owns the entire lifecycle of an AI-assisted coding
task** — the registration of the project, the creation of
the task, the preparation of the workspace, the launch of
the agent, the observation of the run, the review of the
changes, the gating of the result, and the disposition of
the worktree.

The platform **orchestrates** the workflow. It does not
**replace** the agents. The agents remain external
products that the platform integrates through stable
contracts. The platform's value is the orchestration.

---

## 2. What Success Looks Like

Success is a developer on a Windows laptop, working on a
Git repository, performing the full lifecycle of an
AI-assisted coding task in a single native application —
**without ever leaving the application to do work the
platform should own.**

Success is a run that is cancelled mid-stream and
recovered without losing the worktree or the history. It
is a worktree whose diff is reviewed from inside the
application, with the review findings persisted across a
restart. It is a quality gate whose pass/fail is
rendered inside the same surface that ran the agent. It
is a developer who can switch from one runtime to another
— from Ollama Launch to a Claude-compatible model to an
OpenAI-compatible model — by selecting a different
provider through a registry, without any code change.

Success is a platform whose architecture tests **fail
the build** on any direct bypass of its own abstractions.
A platform that has no `Process.Start` calls outside
`Infrastructure`. A platform that has no secrets in
configuration files or logs. A platform that has no
provider implementation imported from a page, a
component, an application service, a view model, a DTO,
or a domain type. A platform that is not ashamed of its
own shape.

Success is a platform that ships and survives — that
absorbs new runtimes, new worktree strategies, new review
tools, new quality gates, and new autonomous-loop
mechanisms without rewriting the application, the UI, or
the architecture. The provider model is the substrate;
the platform's longevity is the proof that the substrate
was right.

---

## 3. Principles That Never Change

The principles below are the **non-negotiable
foundation**. They are the reason the project exists.
A change to any of them is a project-founding change
and must be recorded as an ADR with explicit human
approval.

### 3.1 The Platform Is a Developer Tool, Not a Chat App

The platform serves professional developers on Windows
desktop. Every surface is tuned for high information
density, keyboard navigation, large windows, and power
users. Chat-style interactions, mobile surfaces, and
marketing copy are not targets.

### 3.2 The Platform Orchestrates, It Does Not Replace

External tools — Ollama, Claude, OpenAI, Codex, Git,
PowerShell, WSL, Windows Terminal, Treehouse, No
Mistakes, Lavish Axi, GNHF, Firstmate — are integrated
through capability-based provider contracts. The
platform never tries to be a coding agent. The
platform's value is the orchestration of the agents
the developer already uses.

### 3.3 Composition Over Bypass

Every later milestone consumes the reusable
capabilities delivered by earlier milestones. A direct
bypass — a page importing a provider implementation,
a feature re-implementing a registered service inline —
is rejected by the architecture tests, not by
convention. The platform survives on the discipline of
composition; the discipline is enforced by code.

### 3.4 State Is Observable, Recoverable, and Persisted

Every run has a history. Every worktree has a
disposition. Every review has findings. Every decision
is recorded. A restart does not lose operational
state. A misclick does not lose work. The platform
treats state as a first-class deliverable, not as a
side-effect of execution.

### 3.5 Windows-Native, Not Windows-Only

The primary surface is Windows. The architecture does
not preclude Linux or macOS development machines when
the user opts in. The contracts are host-agnostic; the
implementations are host-aware.

### 3.6 Documentation Is the Product

A platform whose documentation is out of date is a
platform that does not exist. Documentation is
first-class; it ships in the same change as the code;
it is reviewed; it is the surface through which new
contributors understand the work.

### 3.7 Constraints Are a Feature

The constraints documented in `AGENTS.md` — no code
comments, no `Process.Start` outside Infrastructure,
no secrets in configuration, no direct provider
imports, no architectural change without an ADR — are
not obstacles. They are the platform's immune system.
They prevent the slow drift that turns a coherent
platform into a fragmented codebase. A constraint
that is repeatedly violated is a signal the constraint
is wrong; the fix is to amend the rule, not to bypass
it.

---

## 4. What Kind of Software We Are Building

The platform is a **single-purpose, Windows-native
developer tool** that:

- Owns the lifecycle of an AI-assisted coding task
  end-to-end.
- Renders dense, keyboard-navigable, accessible UI
  built on a component-first design system.
- Integrates external tools through capability-based
  provider contracts; the contracts are the platform's
  public API.
- Persists operational state (projects, tasks, runs,
  reviews, gates, dispositions) so the developer's work
  survives restarts.
- Treats its own architecture as a deliverable. The
  architecture tests fail the build on bypass.
- Scales to hundreds of components, dozens of
  providers, and multiple AI runtimes without
  rewrites.
- Ships as a signed Windows installer; the primary
  surface is a Blazor Server application, with
  offline-capable surfaces considered when needed.
- Is built on .NET 10 and C# 14, on Blazor, on
  Tailwind, on a layered architecture enforced at
  compile time.

---

## 5. What Kind of Software We Are Not Building

The platform is explicitly **not**:

- A coding agent. The platform orchestrates agents.
  It does not generate code itself.
- A chat app. The platform's surfaces are
  desktop-first, dense, and keyboard-driven. Chat is
  not a target.
- A cloud-only, browser-only, or multi-tenant
  service. The platform is a Windows application on
  the developer's machine. Hosting is out of scope.
- A mobile or touch-first surface. Touch is out of
  scope for v1.
- A replacement for the host's existing tools.
  PowerShell, Git, Windows Terminal, WSL, Ollama,
  Claude, and OpenAI remain installed and used. The
  platform integrates them, not replaces them.
- A marketing site, a notebook, a documentation
  generator, a generic AI surface, or a
  general-purpose automation tool. The platform is
  single-purpose.
- A product that adds features faster than it
  documents them. The documentation lags the code
  in a healthy platform only by one PR. The
  documentation cannot outpace the architecture
  tests.

---

## 6. The Relationship to Other Documents

`VISION.md` is the **destination**. The other documents
translate the vision into action.

| Document         | Role                                                                                        |
| ---------------- | ------------------------------------------------------------------------------------------- |
| `VISION.md`      | **This document.** The destination. Why the project exists and what it must never become.   |
| `PRODUCT.md`     | The product as it is **today** — its purpose, its users, its principles, its journey.       |
| `ROADMAP.md`     | The ordered journey from the empty repository to the destination.                          |
| `MASTER DELIVERY PLAN` (`.ai/plans/master-delivery-plan.md`) | The delivery view of the roadmap: what ships in each milestone and when.        |
| `ARCHITECTURE.md` | The structure that lets the destination be reached without rewrites.                      |
| `DECISIONS.md`   | The architectural decisions that shaped the structure. Append-only.                         |
| `AGENTS.md`      | The constitution: the rules every change must satisfy.                                     |
| `docs/`          | The detailed engineering standards, design system, and provider guidelines.                 |
| `.ai/`           | The AI operating layer: prompts, workflows, templates, plans, handoffs, state, and backlog. |
| `STATE`          | The live project state — current snapshot, task board, handoffs, session.                  |

The chain is: **Vision** → **Product** → **Roadmap** →
**Milestones** → **Features** → **Tasks** → **Plans** →
**Implementation** → **Reports** → **State** → **History**.

A document that contradicts its neighbour is a bug. The
higher-level document wins; the lower-level document is
updated to match.

---

## 7. How `VISION.md` Is Changed

A change to `VISION.md` is **rare** and **load-bearing**.
It happens only when the project's purpose changes — when
the destination moves.

The procedure:

1. The change is proposed as a task with an explicit
   human approval.
2. The change is recorded as an ADR in `DECISIONS.md`
   with the rationale, the alternatives, and the
   consequences.
3. The downstream documents (`PRODUCT.md`, `ROADMAP.md`,
   `ARCHITECTURE.md`, `AGENTS.md`, `docs/`, `.ai/`) are
   reviewed for consistency and amended in the same
   change.
4. The implementation report records the change as a
   deviation from the previous vision and cites the
   approving message.

A change to a single principle (§ 3) follows the same
procedure. A change to the structure of the document
itself follows the same procedure.

A change to this document is **not** an opportunity to
re-design the product. The product is described in
`PRODUCT.md`; redesigning it is a separate decision
governed by `AGENTS.md` Rule 16 (Scope Discipline) and
the corresponding ADRs.

---

## 8. The Document in One Sentence

> **The platform is a Windows-native, single-purpose
> developer tool that owns the end-to-end lifecycle of an
> AI-assisted coding task by orchestrating external
> agents through capability-based provider contracts —
> and that treats its own architecture, documentation,
> and state as first-class deliverables.**

---

## 9. Linked Artefacts

- [`PRODUCT.md`](./PRODUCT.md) — the product as it is
  today.
- [`ROADMAP.md`](./ROADMAP.md) — the ordered journey.
- [`.ai/plans/master-delivery-plan.md`](./.ai/plans/master-delivery-plan.md) —
  the delivery view of the roadmap.
- [`ARCHITECTURE.md`](./ARCHITECTURE.md) — the structure
  that supports the vision.
- [`DECISIONS.md`](./DECISIONS.md) — the ADRs that
  shaped the structure.
- [`AGENTS.md`](./AGENTS.md) — the constitution.

## 10. Last Updated

- **2026-07-10** — created in the M0.5 architecture
  refinement. The destination is recorded before the next
  milestone starts; M2.1 ("Application Shell Skeleton",
  plan `Awaiting Approval`) is the first slice of the
  roadmap that builds the surface the vision describes.
