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

## Mandatory Final Action (BEFORE EXIT)

The phase is not complete until the receipt has been
written to disk and validated. The router will refuse
to advance and will report the child stdout/stderr log
paths if the receipt is missing or invalid. Do all of
the following steps, in order, before exiting:

1. Build the receipt JSON. Replace
   `REPLACE_AT_FINISH` with the current UTC ISO 8601
   timestamp. Replace `REPLACE_WITH_NEXT_PHASE_OR_NULL`
   with the next phase name (one of
   `reconcile | plan | implement | validate | document |
   review | closeout`) or `null` when this is the final
   phase. Fill `files_read`, `files_changed`,
   `commands_run`, `targeted_tests`, `decisions`, and
   `blockers` with the values recorded during the phase.
   Every field listed in
   `.ai/templates/phase-receipt.schema.json` under
   `required` must be present. The schema does not permit
   `additionalProperties`; an unknown field will fail
   validation.
2. Write the receipt using the repository-side helper
   (do NOT rely on inline JSON, do NOT skip validation):

   ```powershell
   $receipt | powershell.exe -NoProfile -ExecutionPolicy Bypass `
     -File tools/Write-PhaseReceipt.ps1 `
     -ReceiptPath .ai/receipts/phases/<task-id>-implement.json `
     -ExpectedTaskId <task-id> `
     -ExpectedPhase implement
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

- A targeted test fails twice on the same input: stop
  and write `fallback_recommended: true`.
- A required file is missing: stop and write
  `status: "blocked"`.
- The plan is materially incomplete: stop and write
  `fallback_recommended: true`.
- A security-sensitive file (credential, secret, key)
  appears in scope: stop and write
  `fallback_recommended: true`.
- The receipt helper exited non-zero and could not be
  fixed: stop and write `status: "blocked"`. Do not
  fabricate a success.
