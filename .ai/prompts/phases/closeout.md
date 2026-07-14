# Closeout Phase Prompt

> **Profile hint:** `economy` (trivial diff) or
> `standard` (non-trivial diff). The router selects
> the tier from the diff size and the changed paths.

## Objective

Review the Git diff summary, create a single focused
commit on the feature branch, merge the feature branch
into `main` per the branching strategy, update the
final evidence, prepare the next task plan, and stop.
The router does **not** begin the next task; the user
must invoke `Next` again.

## Required Context

- `.ai/context/active-task.json`
- `.ai/receipts/phases/<task-id>-document.json`
  (or `<task-id>-review.json` when review ran)
- `.ai/handoffs/YYYY-MM-DD-<short-slug>.md`
- The implementation report
- `.ai/workflows/branching-strategy.md`
- `.ai/state/tasks.json` (next-id assignment)

## Allowed Actions

- Read the previous phase receipts and the handoff.
- Run `git status`, `git diff --stat`, `git log
  --oneline -5`.
- Create a single focused commit on the active
  feature branch (one commit per task per the
  branching strategy).
- Fast-forward merge the feature branch into
  `main` (per `.ai/workflows/branching-strategy.md`).
- Delete the merged feature branch locally.
- Tag the merge commit with a milestone tag (when
  this is a milestone closeout).
- Append the final `Done` task record to
  `.ai/state/tasks.json`.
- Update `.ai/state/session.json` with the new
  envelope; `intended_next_action` is `Stop. The next
  session is the next Ready task on the user's
  'Approve' or 'Next' invocation.`
- Write the closeout phase receipt.
- Stop.

## Forbidden Actions

- Editing source or test code (the implement phase
  finished them).
- Beginning the next task (the closeout phase never
  starts a new task).
- Pushing to a remote (the router respects
  `execution.push_authorization_required`).
- Creating more than one commit per task.
- Force-pushing or rewriting published history.
- Editing the plan after the commit.
- Editing the implementation report after the commit.

## Expected Output

Write the closeout phase receipt at
`.ai/receipts/phases/<task-id>-closeout.json`. The
receipt must declare:

- `phase: "closeout"`.
- `status: "completed"`.
- `commit_sha`: the merge commit SHA on `main`.
- `branch_deleted`: the deleted feature branch.
- `milestone_tag`: the tag (when applied).
- `next_task_id`: the next `Ready` task ID (when one
  exists).
- `decisions`: any merge decision.
- `next_phase: null` (the router stops after
  closeout).

## Stop Conditions

- The active branch has no commits ahead of `main`:
  stop and write `status: "blocked"`; the user must
  run `Next` again to restart the task.
- The merge is not a fast-forward (the branching
  strategy forbids merge commits): stop and write
  `status: "blocked"`.
- The `main` branch is dirty: stop and write
  `status: "blocked"`.
- A push is requested and
  `execution.push_authorization_required` is `true`:
  stop and write `status: "blocked"`; the user must
  authorise the push with `-NoPush` or a separate
  `git push` invocation.
