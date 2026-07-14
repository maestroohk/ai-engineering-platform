# Reconcile Phase Prompt

> **Profile hint:** `economy` (default) or `standard` (when
> the active packet is malformed). The router selects the
> tier; the child does not override it.

## Objective

Verify the active task packet is well-formed and consistent
with the repository state. If the packet is malformed,
repair it from canonical sources (state files, plan,
previous receipts). If a contradiction exists, stop and
write a `fallback_recommended: true` receipt.

## Required Context

- `.ai/context/active-task.json` (the active packet)
- `.ai/context/repository-map.json` (canonical paths)
- `.ai/state/session.json` (current session envelope)
- `.ai/state/tasks.json` (task registry, read-only)
- `.ai/receipts/phases/<task-id>-<previous-phase>.json`
  (last receipt, when present)

## Allowed Actions

- Read `git status`, `git log -1`, `git diff --stat`.
- Read the active packet and the previous phase receipt.
- Validate the active packet against
  `.ai/context/active-task.schema.json` (if present).
- Repair small, mechanical packet fields (paths, dates,
  branch names) from canonical sources.
- Refresh the active packet's
  `last_successful_phase` and `current_phase` from the
  most recent receipt.
- Write the reconcile phase receipt.

## Forbidden Actions

- Broad repository exploration.
- Planning, design, or implementation work.
- Editing source or test code.
- Beginning another phase.
- Committing or merging.

## Expected Output

Write the reconcile phase receipt at
`.ai/receipts/phases/<task-id>-reconcile.json` per
`.ai/templates/phase-receipt.schema.json`. The receipt
must declare:

- `phase: "reconcile"`.
- `status: "completed"` (or `"blocked"` on failure).
- `files_changed`: only files under `.ai/context/` and
  `.ai/receipts/`.
- `decisions`: any repair applied to the active packet.
- `next_phase`: the phase the router should run next
  (default: `plan`).
- `retry_recommended: false`.
- `fallback_recommended: true` only on contradiction.

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
     -ReceiptPath .ai/receipts/phases/<task-id>-reconcile.json `
     -ExpectedTaskId <task-id> `
     -ExpectedPhase reconcile
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

- The active packet references a non-existent plan or
  handoff: stop and write `fallback_recommended: true`.
- The active task ID is not in `.ai/state/tasks.json`:
  stop and write `status: "blocked"`.
- The previous phase receipt reports `status: "blocked"`
  and the active packet has not been re-baselined.
- The receipt helper exited non-zero and could not be
  fixed: stop and write `status: "blocked"`. Do not
  fabricate a success.
