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
├── state/                 # live project-continuity state (Rule 15)
│   ├── README.md
│   ├── current.md         # one-page snapshot
│   └── task-board.md      # live work queue
├── handoffs/              # per-session handoffs (Rule 15)
│   ├── README.md
│   ├── latest.md          # mirror of the most recent handoff
│   └── YYYY-MM-DD-<slug>.md
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
│   ├── feature-lifecycle.md
│   ├── ui-design-review.md
│   ├── provider-onboarding.md
│   ├── tool-dogfooding.md
│   ├── documentation-update.md
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
