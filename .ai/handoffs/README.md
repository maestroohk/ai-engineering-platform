# `.ai/handoffs/` — Session Handoffs

> **Purpose.** A handoff is the single most important
> artefact a closing AI session produces. It tells the
> next session **exactly** what changed, what was
> validated, what is broken, and what to do next. It is
> short, high-signal, and follows the
> `.ai/templates/session-handoff.md` template.

## Files in This Directory

| File | Role | Update cadence |
| ---- | ---- | -------------- |
| `README.md` | This file. | Updated only when the format changes. |
| `latest.md` | A symlink-equivalent copy of the most recent handoff. The next session reads `latest.md` first. | Updated at the end of every AI session. |
| `YYYY-MM-DD-<slug>.md` | Per-session handoff. `<slug>` is the milestone or task name. | One per session. Never overwritten. |

## How to Use

- **Starting a new session.** Read `latest.md`. It
  names the milestone, the branch, the last commit,
  the last validation result, and the exact next
  step.
- **Closing a session.** Write the new handoff to
  `YYYY-MM-DD-<slug>.md`, then overwrite `latest.md`
  with the same content. Older handoffs stay
  permanently in this directory.
- **Format.** Follow `.ai/templates/session-handoff.md`.
  Do not invent new sections.

## What a Handoff Is Not

- **Not a journal.** The implementation report (at the
  repository root, `implementation-report-*.md`) is the
  long-form session narrative. The handoff is the
  short-form state.
- **Not a plan.** Plans live in `.ai/plans/`. A
  handoff that introduces new work writes a
  Ready-row into `.ai/state/task-board.md` and
  references the plan.
- **Not a changelog.** The git log is the changelog.
  A handoff may list the commits made in the session,
  but it is not a substitute for the git log.

## The Cardinal Rule

> **A session that closes without writing a handoff
> has not closed.**

This rule is enforced by
`.ai/workflows/feature-lifecycle.md` stage 8
("Report"), which now includes a "Write session
handoff" sub-step.
