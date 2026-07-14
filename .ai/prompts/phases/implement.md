# Implement Phase Prompt

> **Profile hint:** `standard`. The router selects
> `standard` when the approved plan is execution-ready
> and the task type is `feature`, `bugfix`, `refactor`,
> `provider`, `testing`, or `release`.

## Objective

Implement the approved plan exactly as scoped. Read the
plan, the active packet, and the related files. Make
small, testable changes. Run targeted tests. Do not
write the final implementation report; the document
phase produces that.

## Required Context

- `.ai/context/active-task.json`
- The approved plan at
  `.ai/plans/<YYYY-MM-DD>-<task-id>-<short-slug>.md`
- `.ai/receipts/phases/<task-id>-plan.json`
- Files named in the plan's "Files to Add" /
  "Files to Modify" / "Files to Delete" sections

## Allowed Actions

- Read the plan, the previous plan receipt, and the
  affected files.
- Edit, add, or delete files exactly as scoped in the
  plan. No scope creep.
- Run the targeted tests listed in the plan's Test
  Plan section.
- Refresh the active packet's `files_read` and
  `files_changed` lists.
- Write the implement phase receipt.

## Forbidden Actions

- Writing the final implementation report (the
  document phase does this).
- Editing the plan (the plan is approved; if the plan
  is wrong, write `fallback_recommended: true` and
  stop).
- Editing state files (`.ai/state/`).
- Committing, merging, or pushing.
- Beginning another phase.
- Re-running the full closeout validation suite (the
  validate phase does this).

## Expected Output

Write the implement phase receipt at
`.ai/receipts/phases/<task-id>-implement.json`. The
receipt must declare:

- `phase: "implement"`.
- `status: "completed"` (or `"blocked"` on failure).
- `files_read`: the files inspected.
- `files_changed`: the files added, modified, deleted.
- `commands_run`: the targeted test commands.
- `targeted_tests`: the test names that passed.
- `decisions`: any small, in-scope decisions taken
  during implementation.
- `blockers`: any out-of-scope blocker discovered.
- `next_phase: "validate"`.
- `retry_recommended: false`.
- `fallback_recommended: true` only on scope creep,
  plan incompleteness, or repeated test failure.

## Stop Conditions

- A targeted test fails twice on the same input: stop
  and write `fallback_recommended: true`.
- A required file is missing: stop and write
  `status: "blocked"`.
- The plan is materially incomplete: stop and write
  `fallback_recommended: true`.
- A security-sensitive file (credential, secret, key)
  appears in scope: stop and write
  `fallback_recommended: true`.
