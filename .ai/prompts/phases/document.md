# Document Phase Prompt

> **Profile hint:** `economy`. The router selects
> `economy` when the deliverable is a receipt, a report,
> a handoff, a state projection, a changelog entry, an
> index entry, or a markdown refresh.

## Objective

Write the compact implementation receipt, the
implementation report, the handoff, the structured
state projection, and the markdown refresh (when
needed). This phase produces the human-readable record
and the structured evidence; it does not edit source.

## Required Context

- `.ai/context/active-task.json`
- `.ai/receipts/phases/<task-id>-validate.json`
- `.ai/receipts/phases/<task-id>-implement.json`
- `.ai/receipts/phases/<task-id>-plan.json`
- `.ai/templates/implementation-receipt.schema.json`
- `.ai/templates/implementation-report.md`
- `.ai/templates/session-handoff.md`

## Allowed Actions

- Read all the phase receipts for the active task.
- Write the implementation receipt at
  `.ai/receipts/<task-id>.json` per
  `.ai/templates/implementation-receipt.schema.json`.
- Write the implementation report at
  `implementation-report-<short-slug>.md` (under
  200 lines) per
  `.ai/templates/implementation-report.md`.
- Write the handoff at
  `.ai/handoffs/YYYY-MM-DD-<short-slug>.md` per
  `.ai/templates/session-handoff.md`.
- Mirror the implementation report to
  `.ai/handoffs/latest.md` (single pointer).
- Update `.ai/index/reports.json`,
  `.ai/index/plans.json`, `.ai/index/handoffs.json`
  (append the new entry to each).
- Update `.ai/state/session.json` with the new
  envelope; preserve `previous_session`.
- Append a one-line summary to
  `.ai/state/current.md`; do not reorganise the file.

## Forbidden Actions

- Editing source or test code.
- Editing the plan (the plan is approved).
- Committing, merging, or pushing.
- Beginning another phase.
- Inventing new documentation conventions; the
  templates are canonical.
- Writing a long-form report (over 200 lines).

## Expected Output

Write the document phase receipt at
`.ai/receipts/phases/<task-id>-document.json`. The
receipt must declare:

- `phase: "document"`.
- `status: "completed"` (or `"blocked"` on failure).
- `files_changed`: only files under
  `.ai/receipts/`, `.ai/handoffs/`, `.ai/index/`,
  `.ai/state/`, and the implementation report.
- `decisions`: any document / index update.
- `next_phase: "closeout"`.
- `retry_recommended: false`.
- `fallback_recommended: true` only on missing
  template or schema.

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
     -ReceiptPath .ai/receipts/phases/<task-id>-document.json `
     -ExpectedTaskId <task-id> `
     -ExpectedPhase document
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

- A required template is missing: stop and write
  `status: "blocked"`.
- A schema validation fails: stop and write
  `status: "blocked"`.
- The implementation report exceeds 200 lines: stop
  and write `status: "blocked"`.
- The receipt helper exited non-zero and could not be
  fixed: stop and write `status: "blocked"`. Do not
  fabricate a success.
