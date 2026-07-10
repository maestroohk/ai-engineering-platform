# `.ai/state/` — Project Continuity State

> **Purpose.** This directory is the single landing point
> for any future AI session (or human returning to the
> project) to determine **where the project stopped** and
> **what task comes next**. It is intentionally short,
> intentionally high-signal, and intentionally short on
> prose. A new session reads `current.md` first, then
> `task-board.md`, then `.ai/handoffs/latest.md`.
>
> The directory is the **live** state — the most recent
> update wins. Older snapshots belong in
> `.ai/handoffs/` (one file per session) and the project
> history (git).

## Files in This Directory

| File | Role | Update cadence |
| ---- | ---- | -------------- |
| `current.md` | One-page snapshot of the project right now. | Updated at the end of every AI session that changes project state. |
| `task-board.md` | The live work queue. Ready / In Progress / Blocked / Done. | Updated at the end of every AI session. |
| `README.md` | This file. | Updated only when the format changes. |

## How to Use

- **Starting a new session.** Read `current.md`. It
  names the milestone, the branch, the last commit, the
  last validation result, and the exact next step.
- **Picking up a task.** Read `task-board.md`. Pick
  the topmost `Ready` item; claim it by setting
  `Status: In Progress` and adding your name.
- **Closing a session.** Update `current.md` and
  `task-board.md`, then write a handoff to
  `.ai/handoffs/latest.md` per the
  `.ai/templates/session-handoff.md` template. Older
  handoffs move to `.ai/handoffs/<YYYY-MM-DD>-<slug>.md`
  (the date is the session date; the slug is the
  milestone or task name).

## The Cardinal Rule

> **A session that closes without updating
> `current.md` and `task-board.md` is a session that
> left the project in an unknown state.**

This rule is enforced by
`.ai/workflows/feature-lifecycle.md` stage 8
("Report"), which now includes a "Update project
state" sub-step.

## What This Directory Is Not

- **Not a journal.** Long-form session narrative lives
  in `.ai/handoffs/<date>-<slug>.md` (one file per
  session) and in `implementation-report-*.md` at the
  repository root.
- **Not a planning surface.** Plans live in
  `.ai/plans/` (canonical) or `.claude/plans/`
  (Claude-owned scratch drafts).
- **Not a changelog.** The git log is the changelog.
  This directory is the live state; git is the history.
