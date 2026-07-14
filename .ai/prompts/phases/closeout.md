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

## Mandatory Final Action (BEFORE EXIT)

The phase is not complete until the receipt has been
written to disk and validated. The router will refuse
to advance and will report the child stdout/stderr log
paths if the receipt is missing or invalid. Do all of
the following steps, in order, before exiting:

1. Build the receipt JSON. Replace
   `REPLACE_AT_FINISH` with the current UTC ISO 8601
   timestamp. Replace `REPLACE_WITH_NEXT_PHASE_OR_NULL`
   with `null` (this is the final phase; the router
   stops after closeout). Fill `files_read`,
   `files_changed`, `commands_run`, `targeted_tests`,
   `decisions`, and `blockers` with the values recorded
   during the phase. Every field listed in
   `.ai/templates/phase-receipt.schema.json` under
   `required` must be present. The schema does not permit
   `additionalProperties`; an unknown field will fail
   validation.
2. Write the receipt using the repository-side helper
   (do NOT rely on inline JSON, do NOT skip validation):

   ```powershell
   $receipt | powershell.exe -NoProfile -ExecutionPolicy Bypass `
     -File tools/Write-PhaseReceipt.ps1 `
     -ReceiptPath .ai/receipts/phases/<task-id>-closeout.json `
     -ExpectedTaskId <task-id> `
     -ExpectedPhase closeout
   ```

   The helper creates the parent directory if it is
   missing, validates the receipt against
   `.ai/templates/phase-receipt.schema.json`, writes the
   file as UTF-8 (no BOM), and exits non-zero on any
   failure. The path is repository-relative; do not
   convert it to an absolute Windows path.
3. Confirm the helper exited with code `0` and printed
   the absolute receipt path on its stdout. If the
   helper exited non-zero, read its stderr, fix the
   receipt, and re-run the helper. Do not exit the phase
   until the helper has exited `0`.
4. Re-read the receipt from disk and confirm every
   required field is present and well-typed. Do not exit
   until the file is present on disk, parses as JSON,
   and validates against the schema.

The router distinguishes these failure modes and will
refuse to advance on any of them: `missing`,
`malformed_json`, `missing_fields`, `wrong_task`,
`wrong_phase`, `incomplete_status`. Do not produce a
synthetic success message; do not exit before the
receipt is on disk.

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
- The receipt helper exited non-zero and could not be
  fixed: stop and write `status: "blocked"`. Do not
  fabricate a success.
