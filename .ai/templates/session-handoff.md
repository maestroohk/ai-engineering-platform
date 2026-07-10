# Session Handoff

> Produced when an AI session cannot reach a clean end and
> must transfer its state to the next session. The handoff
> is the contract between the two sessions: the next session
> reads this file first and resumes exactly where this one
> stopped.

---

## Task

A single declarative sentence. What was the session trying
to do?

## Branch

The Git branch the session is on. The next session checks
out the same branch.

## Current Status

One of:

- `In progress` — the work continues.
- `Blocked` — the work is waiting on a decision or an
  external input.
- `Partially complete` — some of the planned work is done,
  the rest is not.
- `Awaiting review` — the work is finished and waiting for
  a human to review.

## Work Completed

The concrete work the session did. Cite files.

- Completed 1
- Completed 2

## Files Changed

The actual files, with the type of change.

- Created: `path/to/file.cs`
- Modified: `path/to/file.cs`
- Deleted: `path/to/file.cs`

## Commands Run

The commands that mattered. Skip the noise.

- `dotnet build`
- `dotnet test --filter "AppButton"`
- `git add -A`

## Test Status

- Last build: pass / fail (with the specific error if fail)
- Last test run: passed X / failed Y
- Last format check: clean / dirty
- Open regressions: list of failing tests, if any

## Unresolved Issues

- Issue 1 — what is blocking and what unblocks it
- Issue 2

## Exact Next Step

The **single most concrete** thing the next session should
do first. "Continue the feature" is not a next step. "Open
`Services/Workspace/WorkspaceService.cs`, add the
`OpenAsync` method per the plan in
`.ai/templates/implementation-plan.md`, then run
`dotnet test --filter "WorkspaceService"`" is a next step.

## Documents the Next Session Must Read

In the order they must be read.

1. `AGENTS.md`
2. `.ai/session-start.md`
3. `.ai/state/current.md` — the one-page snapshot
4. `.ai/state/task-board.md` — the live work queue
5. `.ai/handoffs/latest.md` — the most recent
   handoff (this file, or its mirror)
6. `.ai/prompts/<matching prompt>.md`
7. The matching `task-brief.md`
8. The matching `implementation-plan.md`
9. Any ADR referenced in the plan
10. Any other document the next session needs

## Linked Artefacts

- `task-brief.md`
- `implementation-plan.md`
- The current commit hash: `git rev-parse HEAD`
- The current branch: `git branch --show-current`
- Any in-flight worktree or stash: `git worktree list`,
  `git stash list`
- `.ai/state/current.md` and
  `.ai/state/task-board.md` — the project-continuity
  state (Rule 15 in `AGENTS.md`).
- The implementation report that pairs with this
  handoff, if any.
