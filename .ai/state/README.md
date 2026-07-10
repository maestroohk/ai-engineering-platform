# `.ai/state/` — Project Continuity State

> **Purpose.** This directory is the single landing
> point for any future AI session (or human returning
> to the project) to determine **where the project
> stopped** and **what task comes next**. It is
> intentionally short, intentionally high-signal, and
> intentionally short on prose. A new session reads
> `current.md` first, then `task-board.md`, then
> `.ai/handoffs/latest.md`.
>
> The state directory has **two layers**: a
> human-readable Markdown layer (`current.md`,
> `task-board.md`, `handoffs/`, `decision-log.md`,
> `capability-mapping.md`) and a machine-readable
> JSON layer (`*.json` and `*.schema.json`).
> **The JSON is the canonical source; the Markdown
> is the human-readable projection.** When the two
> disagree, the JSON wins; the Markdown is
> regenerated. (The regeneration tool is a future
> task; until then, a human updates both in the
> same change.)
>
> The directory is the **live** state — the most
> recent update wins. Older snapshots belong in
> `.ai/handoffs/` (one file per session) and the
> project history (git).

## Files in This Directory

### Human-readable (Markdown)

| File                     | Role                                                                                                              | Update cadence                                  |
| ------------------------ | ----------------------------------------------------------------------------------------------------------------- | ----------------------------------------------- |
| `README.md`              | This file.                                                                                                        | Updated only when the format changes.           |
| `current.md`             | One-page snapshot of the project right now.                                                                       | Updated at the end of every AI session.         |
| `task-board.md`          | The live work queue. Ready / In Progress / Blocked / Review / Done.                                                | Updated at the end of every AI session.         |
| `decision-log.md`        | Small, in-flight decisions. Promotes to ADRs when the decision becomes architectural.                             | Updated when a small decision is made.          |
| `capability-mapping.md`  | The capability dependency graph (human-readable projection of `capabilities.json`).                                | Updated when capabilities or dependencies change. |

### Machine-readable (JSON)

| File                       | Role                                                                                                              | Schema                                            |
| -------------------------- | ----------------------------------------------------------------------------------------------------------------- | ------------------------------------------------- |
| `project.json`             | The project identity, stack, and document map.                                                                    | `project.schema.json`                             |
| `milestones.json`          | The ordered list of milestones with their status, deliverables, and evidence.                                     | `milestones.schema.json`                          |
| `features.json`            | The user-facing features with their PRODUCT.md traceability.                                                     | `features.schema.json`                            |
| `capabilities.json`        | The platform capabilities with the dependency graph (`depends_on` / `consumed_by`).                               | `capabilities.schema.json`                        |
| `providers.json`           | The concrete provider implementations with their five lifecycle states.                                          | `providers.schema.json`                           |
| `tasks.json`               | The live work queue (mirrors `task-board.md` machine-readably).                                                   | `tasks.schema.json`                               |
| `session.json`             | The current AI session's self-awareness: id, type, scope, last action, current understanding.                    | `session.schema.json`                             |

### Schemas

Every JSON file has a `*.schema.json` companion that
defines the contract. A change to the schema is a
change to the data model and is recorded in this
README and in the architecture review (M0.5).

## How to Use

### For Humans

- **Starting a new session.** Read `current.md`. It
  names the milestone, the branch, the last commit,
  the last validation result, and the exact next
  step.
- **Picking up a task.** Read `task-board.md`. Pick
  the topmost `Ready` item; claim it by setting
  `Status: In Progress` and adding your name.
- **Closing a session.** Update `current.md` and
  `task-board.md`, then write a handoff to
  `.ai/handoffs/latest.md` per the
  `.ai/templates/session-handoff.md` template. Older
  handoffs move to
  `.ai/handoffs/<YYYY-MM-DD>-<slug>.md`.
- **For the JSON layer:** also update the matching
  `*.json` file in the same change. The two layers
  are kept in sync by the session that owns the
  change.

### For Tools

- **Reading the project.** Read `project.json` for
  the project identity. Read `milestones.json` for
  the milestone order. Read `capabilities.json` for
  the capability graph. Read `providers.json` for
  the provider registry state. Read `tasks.json`
  for the work queue. Read `session.json` for the
  current session's self-awareness.
- **Validating the state.** Use the `*.schema.json`
  files to validate the `*.json` files. The JSON
  schema is the contract; a tool that produces a
  JSON file that does not match the schema is
  broken.

## The Cardinal Rule

> **A session that closes without updating
> `current.md`, `task-board.md`, and the matching
> `*.json` files is a session that left the project
> in an unknown state.**

This rule is enforced by
`.ai/workflows/feature-lifecycle.md` stage 8
("Report"), which includes a "Update project
state" sub-step.

## Layer Boundaries

The state directory has clear boundaries between
files. A file may elaborate but never override
another file:

- `current.md` may reference
  `task-board.md`, `decision-log.md`, the JSON
  files, and the handoffs.
- `task-board.md` may reference `tasks.json` and
  the plans in `.ai/plans/`.
- `decision-log.md` may reference `DECISIONS.md`
  (when an entry promotes to an ADR) and the
  decision log's own history.
- `capability-mapping.md` may reference
  `capabilities.json`,
  `.ai/backlog/capabilities.md`, and the ADRs in
  `DECISIONS.md`.
- The JSON files may reference each other by ID.
  The schema is the contract; the projection in
  the Markdown is the human-friendly view.

A file that overrides a higher-precedence file is
a bug. The precedence hierarchy in
[`AGENTS.md`](./../../AGENTS.md) § 2.2 wins.

## What This Directory Is Not

- **Not a journal.** Long-form session narrative
  lives in
  `.ai/handoffs/<date>-<slug>.md` (one file per
  session) and in `implementation-report-*.md` at
  the repository root.
- **Not a planning surface.** Plans live in
  `.ai/plans/` (canonical) or `.claude/plans/`
  (Claude-owned scratch drafts).
- **Not a changelog.** The git log is the
  changelog. This directory is the live state;
  git is the history.
- **Not a backlog.** The backlog lives in
  `.ai/backlog/`. The backlog is unsorted
  thinking; the task board is sorted work; the
  state is the live state of the work.

## Last Updated

- **2026-07-10** — restructured in the M0.5
  architecture refinement to introduce the JSON
  layer, the capability mapping, the decision log,
  and the self-awareness state. The two-layer
  model (human-readable Markdown + machine-readable
  JSON) is the canonical state model going forward.
