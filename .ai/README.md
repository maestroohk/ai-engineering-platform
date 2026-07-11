# `.ai/` — AI Collaboration Hub

> **Operational workflows, prompts, and templates for AI-assisted
> development of the AI Engineering Platform.**
>
> This directory is **not** the project's constitution. It is the
> **playbook** an AI follows to comply with the constitution
> (`AGENTS.md`) and the engineering standards (`docs/`).

---

## 1. Why `.ai/` Exists

The repository has three documentation layers with distinct
purposes. They must stay distinct.

| Layer        | Purpose                                                                  | Mutability                |
| ------------ | ------------------------------------------------------------------------ | ------------------------- |
| `AGENTS.md`  | Permanent, non-negotiable rules. The constitution.                       | Changes only via ADR.    |
| `docs/`      | Detailed engineering standards, architecture explanations, design system. | Changes via PR review.   |
| `.ai/`       | Task-specific prompts, repeatable workflows, templates, handoff formats. | Changes via PR review.   |

`.ai/` is the layer that **operationalises** the other two. It
tells an AI *how* to perform work while *complying* with them. It
must never duplicate, contradict, or override them.

---

## 2. Precedence Hierarchy

When two instructions conflict, the higher-precedence document
wins. Lower-precedence documents may elaborate but never
override.

1. `AGENTS.md` — the constitution.
2. `DECISIONS.md` — accepted ADRs.
3. `ARCHITECTURE.md` and `STYLEGUIDE.md` — the layered architecture and
   the code style.
4. `docs/` — detailed engineering standards.
5. `.ai/workflows/` — multi-step operating procedures.
6. `.ai/prompts/` — task-type templates.
7. Individual task instructions from a human user.

This hierarchy is recorded in `AGENTS.md` and is the single
source of truth. If a prompt in `.ai/prompts/` appears to
conflict with `AGENTS.md`, `AGENTS.md` wins and the prompt is
treated as a bug.

---

## 3. Directory Layout

```
.ai/
├── README.md              # this file
├── session-start.md       # the first file an AI reads after AGENTS.md
├── commands.md            # the command protocol for short user instructions
├── state/                 # live project-continuity state (Rule 15)
│   ├── README.md
│   ├── current.md         # one-page snapshot
│   └── task-board.md      # live work queue
├── handoffs/              # per-session handoffs (Rule 15)
│   ├── README.md
│   ├── latest.md          # mirror of the most recent handoff
│   └── YYYY-MM-DD-<slug>.md
├── plans/                 # approved implementation plans
│   ├── README.md
│   └── <milestone-or-task-name>.md
├── prompts/               # task-type templates (one per task type)
│   ├── bootstrap.md
│   ├── feature.md
│   ├── bugfix.md
│   ├── refactor.md
│   ├── review.md
│   ├── architecture.md
│   ├── ui.md
│   ├── testing.md
│   ├── provider.md
│   └── release.md
├── workflows/             # multi-step operating procedures
│   ├── branching-strategy.md
│   ├── feature-lifecycle.md
│   ├── milestone-closeout.md
│   ├── ui-design-review.md
│   ├── provider-onboarding.md
│   ├── tool-dogfooding.md
│   ├── documentation-update.md
│   ├── progressive-coding.md
│   └── release-checklist.md
└── templates/             # reusable document templates
    ├── task-brief.md
    ├── implementation-plan.md
    ├── implementation-report.md
    ├── review-report.md
    └── session-handoff.md
```

---

## 4. Task Routing

Use this table to pick the right prompt, workflow, and supporting
documents for the task at hand.

| Task                                | Prompt                              | Workflow                            | Main documents                                                                                       |
| ----------------------------------- | ----------------------------------- | ----------------------------------- | ---------------------------------------------------------------------------------------------------- |
| Short user instruction (e.g. `Continue`, `Approve`, `Status`, `Plan`, `Resume`, `Review`, `Validate`, `Finish`) | (none — command-driven) | (none — command-driven) | [`.ai/commands.md`](../.ai/commands.md), [`.ai/session-start.md`](../.ai/session-start.md), [`.ai/workflows/progressive-coding.md`](../.ai/workflows/progressive-coding.md) |
| New milestone or project area       | `bootstrap.md`                      | `feature-lifecycle.md`              | `ARCHITECTURE.md`, `docs/folder-structure.md`                                                        |
| Feature                             | `feature.md`                        | `feature-lifecycle.md`              | Task-dependent                                                                                       |
| UI feature                          | `ui.md`                             | `ui-design-review.md`               | `docs/design-system.md`, `docs/component-guidelines.md`, `docs/ui-principles.md`                     |
| Provider                            | `provider.md`                       | `provider-onboarding.md`            | `docs/provider-guidelines.md`, `docs/architecture-principles.md`                                     |
| Bug                                 | `bugfix.md`                         | `feature-lifecycle.md`              | `docs/coding-standards.md`                                                                           |
| Refactor                            | `refactor.md`                       | `documentation-update.md`           | `docs/architecture-principles.md`, `docs/coding-standards.md`                                         |
| Tests                               | `testing.md`                        | `feature-lifecycle.md`              | `docs/coding-standards.md`                                                                           |
| Review                              | `review.md`                         | `ui-design-review.md` when relevant | Standards relevant to the diff                                                                       |
| Architecture decision               | `architecture.md`                   | `documentation-update.md`           | `ARCHITECTURE.md`, `DECISIONS.md`                                                                    |
| Release                             | `release.md`                        | `release-checklist.md`              | `CONTRIBUTING.md`, `ROADMAP.md`                                                                      |
| Branching / merging decision        | (none — read directly)              | (none — read directly)              | [`.ai/workflows/branching-strategy.md`](../.ai/workflows/branching-strategy.md) (single source of truth) |
| Milestone closeout / retrospective  | (none — read directly)              | (none — read directly)              | [`.ai/workflows/milestone-closeout.md`](../.ai/workflows/milestone-closeout.md) (single source of truth) |
| Documentation update                | `refactor.md` (as guidance)         | `documentation-update.md`           | The documents affected by the change                                                                 |
| Tool dogfooding (development-time)  | `feature.md` + workflow note        | `tool-dogfooding.md`                | `docs/provider-guidelines.md` (for the boundary)                                                     |
| Product integration of a tool       | `provider.md`                       | `provider-onboarding.md`            | `docs/provider-guidelines.md`                                                                        |

When two rows apply, pick the more specific one. A "UI feature
that is also a provider" is a UI feature with a provider inside
it: `ui.md` is the entry point, and the provider section of the
prompt escalates to `provider.md` for the contract.

---

## 5. Templates

Templates are filled in, not edited in place. A new session creates
a new file from the template and commits it alongside the work.

- `task-brief.md` — the human's specification for a task.
- `implementation-plan.md` — the AI's plan, produced before
  implementation begins.
- `implementation-report.md` — the AI's completion record,
  required at the end of every session.
- `review-report.md` — the structured output of a code review.
- `session-handoff.md` — the bridge between sessions, used when
  work pauses or transfers.

A session that ends without an implementation report is treated
as incomplete, regardless of the diff's quality.

---

## 6. Starting a New AI Session

A human who wants an AI to do work should:

1. Pick a task type from the table above.
2. Write a `task-brief.md` (or paste an equivalent into the
   prompt).
3. Open the matching prompt in `.ai/prompts/` and paste it at
   the start of the conversation.
4. Confirm the AI has read `AGENTS.md` and
   `.ai/session-start.md` before allowing any code changes.

A human who wants an AI to *continue* work should paste the
most recent `session-handoff.md` first, then the original
`task-brief.md`, then the matching prompt.

The first six files a new AI session reads, in order, are:

1. `AGENTS.md` — the constitution.
2. `.ai/session-start.md` — the operational sequence.
3. [`PRODUCT.md`](../PRODUCT.md) — the product definition.
4. [`.ai/state/current.md`](../.ai/state/current.md) — the
   one-page snapshot.
5. [`.ai/state/task-board.md`](../.ai/state/task-board.md) —
   the live work queue.
6. [`.ai/handoffs/latest.md`](../.ai/handoffs/latest.md) —
   the most recent session handoff.

The full session cycle is:

```
Read product and current state
→ select one Ready task
→ load or create its plan
→ request approval
→ mark task In Progress
→ implement
→ validate
→ update documentation
→ update state
→ write implementation report
→ commit
→ push when authorised
→ update latest handoff
```

---

## 7. Closing an AI Session

An AI session ends correctly when:

1. The implementation is complete **and** validated.
2. The affected documentation is updated.
3. An `implementation-report.md` is produced and committed (or
   pasted into the conversation if a commit is not possible).
4. If the session cannot reach a clean end, a `session-handoff.md`
   is produced instead, listing the exact next step.
5. **The project-continuity state is updated** (Rule 15 in
   `AGENTS.md`): `.ai/state/current.md` and
   `.ai/state/task-board.md` are updated, and a handoff is
   written to `.ai/handoffs/YYYY-MM-DD-<slug>.md` and
   mirrored to `.ai/handoffs/latest.md`.
6. **The session's work is committed** (Rule 17 in
   `AGENTS.md`): a single coherent commit per task (or per
   task slice) that includes the implementation, the
   documentation, the implementation report, the state
   updates, and the handoff. The commit is local; pushing
   requires explicit authorisation and is a separate
   step.
7. A short message confirms what was done, what was not, and
   what to read next.

A session that ends silently — no implementation report, no
handoff, no state update, no commit — is a session that did
not end.

---

## 8. What Does Not Belong in `.ai/`

`.ai/` is documentation. It is committed to Git. It is reviewed.
The following never appear in `.ai/`:

- Secrets, API keys, tokens, or credentials of any kind.
- Personal data about contributors or users.
- Temporary runtime state (logs, caches, build outputs).
- Generated content that should live elsewhere (compiled output,
  restored NuGet packages, IDE state).
- Local paths, machine names, or environment-specific
  configuration.

If a workflow produces any of these, the workflow is wrong, the
output is wrong, or both. Surface the issue in the implementation
report.

---

## 9. Editing `.ai/`

A change to `.ai/` is a documentation change. It follows the same
review rules as any other change:

- The diff is read against the precedence hierarchy.
- A change that narrows an existing rule without an ADR is
  rejected.
- A change that adds a new prompt or workflow must include a
  matching entry in the task routing table above.
- A change to `tool-dogfooding.md` must not be the trigger for
  tool invocation; the workflow is the *permission* to use a
  tool under stated conditions, not an instruction to use one
  unprompted.

---

## 10. Documentation Architecture

The repository holds more than code; it holds the
documents that *define* what the code does, why it
exists, how it is built, and what it will become.
These documents are organised into **nine tiers**.
Each tier has a single purpose, a single canonical
file (or set of files), and a single
mutation rule. The map below is the source of
truth for "what document owns what kind of
information" — when in doubt, walk the map from
the top and place the new information at the
lowest tier that can hold it.

| #  | Tier                       | Canonical file(s)                                                                                       | Owns                                                                                | Mutation rule                                                    |
| -- | -------------------------- | ------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------- | ---------------------------------------------------------------- |
| 1  | **Vision**                 | [`VISION.md`](../VISION.md)                                                                              | The destination. Why this project exists. What success looks like. The principles that never change. | Changes only when the destination changes; reviewed by humans. |
| 2  | **Constitution**           | [`AGENTS.md`](../AGENTS.md)                                                                               | The 17 non-negotiable rules every contributor and AI follows.                       | Changes only via ADR (ADR is itself in tier 4).                  |
| 3  | **Architecture**           | [`ARCHITECTURE.md`](../ARCHITECTURE.md), [`STYLEGUIDE.md`](../STYLEGUIDE.md)                              | The layered architecture, the boundaries, the dependency direction, the style.      | Changes via ADR; reviewed by humans.                             |
| 4  | **Decisions**              | [`DECISIONS.md`](../DECISIONS.md)                                                                         | The accepted ADRs. The *why* behind every architectural or constitutional change.  | Append-only; a new ADR is added for every new decision. An ADR is rejected by superseding, never by deletion. |
| 5  | **Product**                | [`PRODUCT.md`](../PRODUCT.md)                                                                             | The product definition. Who is the user. What problem is solved. What does success mean at the product level. | Changes via PR review; reviewed against Vision.                  |
| 6  | **Roadmap / Delivery**     | [`ROADMAP.md`](../ROADMAP.md), [`.ai/plans/master-delivery-plan.md`](../.ai/plans/master-delivery-plan.md) | The milestone plan (M0-M8) and the master delivery plan that ties the backlog to the milestones. | Changes via PR review; reviewed against Product.                 |
| 7  | **Standards**              | [`docs/`](./..) (folder)                                                                                 | Engineering standards: design system, component guidelines, UI principles, provider guidelines, coding standards, architecture principles, folder structure, dashboard definition. | Changes via PR review; per-document versioning.                  |
| 8  | **Operating layer**        | [`.ai/`](./)                                                                                              | The AI collaboration hub: prompts, workflows, templates, command protocol, state, handoffs, backlog, decision log, capability mapping. The instructions an AI follows *while* doing the work. | Changes via PR review; reviewed against Constitution (tier 2) and Standards (tier 7). |
| 9  | **Evidence / History**     | [`.ai/handoffs/`](../.ai/handoffs/), implementation reports, review reports                              | The *record* of what happened. Every session leaves a handoff. Every milestone leaves an implementation report. Every review leaves a review report. | Append-only. Older evidence archives by handoff; nothing is rewritten. |

### 10.1 Direction of Authority

Authority flows **down** the tiers: Vision informs
Constitution; Constitution constrains
Architecture; Architecture constrains
Standards; Standards inform the Operating layer;
Evidence records what the Operating layer did
under those constraints.

The direction is one-way. Evidence never informs
Vision; the Operating layer never overrides the
Constitution; a Standard never contradicts the
Architecture. A document at a lower tier that
contradicts a document at a higher tier is a bug
in the lower tier; the lower tier is fixed or
removed, never the higher.

### 10.2 Anti-Drift Rules

| Drift                                                                  | Tier it would put out of order                      | Rule                                                                                |
| ---------------------------------------------------------------------- | --------------------------------------------------- | ----------------------------------------------------------------------------------- |
| Re-stating a rule from `AGENTS.md` in a prompt                          | Operating layer duplicates Constitution             | A prompt that re-states a rule is treated as a bug; the prompt references the rule by its AGENTS.md number instead. |
| Putting "what success looks like" in a roadmap row                      | Roadmap duplicates Vision / Product                 | The roadmap row links to the Vision section and the Product section; it does not re-state them. |
| Putting "the architecture has four layers" in a workflow               | Operating layer duplicates Architecture              | The workflow references `ARCHITECTURE.md` by section number; it does not re-state the layer names. |
| Putting a per-tool decision (e.g. "we will use Treehouse") in `AGENTS.md` | Constitution is asked to absorb a per-tool choice  | Per-tool choices go in the per-tool profile in `.ai/workflows/tool-dogfooding.md` § 4, or in an ADR, or in `.ai/state/providers.json`. The constitution stays generic. |
| Putting the current implementation status in a design document         | Standards is asked to absorb the Evidence tier      | The current status lives in `.ai/state/current.md` and in the implementation report. The design document links to it. |
| Putting a session handoff in `AGENTS.md`                                | Evidence leaks into Constitution                    | Handoffs live in `.ai/handoffs/`. The constitution has no per-session content.      |
| Writing a rule in a workflow that contradicts `AGENTS.md`              | Operating layer overrides Constitution              | The workflow is rejected. The rule belongs in `AGENTS.md` (with an ADR) or in the workflow, but not both with disagreement. |

The seven rows above are the only
anti-drift rules this document maintains. A new
rule is added when a real drift happens; the
rule cites the drift.

### 10.3 The Map in One Sentence

> **Vision → Constitution → Architecture → Decisions
> → Product → Roadmap/Delivery → Standards →
> Operating layer → Evidence/History.**

The map is the order in which an AI reads the
repository's documents when starting a session
(the first six are mandatory per
[`.ai/session-start.md`](../.ai/session-start.md);
the remaining three are read on demand). The map
is also the order in which a new document is
*placed* in the repository when it is created —
a "what success looks like" sentence is placed
in `VISION.md`; a "how do I run this" sentence
is placed in `docs/`.

### 10.4 Validation Status

This map was added in the M0.5 architecture
refinement (2026-07-10). The validation pass
found that:

- The 17 `AGENTS.md` rules are restated in zero
  other documents (verified by a manual review of
  `.ai/prompts/*.md`, `.ai/workflows/*.md`, and
  `docs/*.md` for verbatim overlap with the
  numbered rules).
- The M0 / M1 / M1.1 / M1.2 implementation
  reports do not contradict `PRODUCT.md`'s
  product definition (verified by reading the
  reports end-to-end).
- The M2.1 plan does not contradict
  `ARCHITECTURE.md` § 6 (the application-shell
  layers) or `DECISIONS.md` ADR-005 (the
  accessibility principle).
- The per-tool decisions in
  `.ai/state/providers.json` (the
  `external_tool_name` and `binary` fields)
  match the per-tool profiles in
  `.ai/workflows/tool-dogfooding.md` § 4.

A repeat of the validation pass is required
whenever a new tier is added, when a new
document is added that does not have a clear
home in the map, or when an existing document
changes tier (e.g. an architecture document
becomes a standard). The validation pass is
recorded in the implementation report of the
session that performed it.

---

## 11. The Command Protocol

For short user instructions (`Continue`,
`Approve` / `Approved`, `Status`, `Plan`,
`Resume`, `Review`, `Validate`, `Finish`),
the command protocol in
[`.ai/commands.md`](../.ai/commands.md) is the
recognised front door. The full
`.ai/session-start.md` sequence is the
default; the commands are the
short-form.

The commands are operational shortcuts.
They do **not** override the constitution
(`AGENTS.md`), accepted ADRs, the approved
roadmap, an approved task plan, the safety
rules in `AGENTS.md`, or the Git rules.
The command protocol is subordinate to all
of the above.

A session that begins with a recognised
command still reads `AGENTS.md` and
`.ai/session-start.md` first. The command
then selects the task and the response
shape; the rest of the session follows the
13-step task lifecycle in
[`.ai/workflows/progressive-coding.md`](../.ai/workflows/progressive-coding.md)
as if the user had issued a full brief.

A session closeout is not a permanent stop.
A closeout ends the session, updates the
state, prepares the next task, and **awaits
the next user command**. A later
`Continue` (or any other recognised
command) begins a new operational cycle.

