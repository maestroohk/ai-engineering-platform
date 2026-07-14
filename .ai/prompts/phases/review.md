# Review Phase Prompt

> **Profile hint:** `review` (preferred) or `high`
> (when the review profile is disabled). The router
> selects `review` only when a high-risk review is
> required.

## Objective

Read the diff, the plan, and the receipts. Produce a
review report. Do not implement; do not commit.

## Required Context

- `.ai/context/active-task.json`
- The approved plan
- All phase receipts under
  `.ai/receipts/phases/<task-id>-*.json`
- `git status`, `git log -1 --stat`, `git diff
  --stat` against `main`
- `git diff` (full diff against `main`)

## Allowed Actions

- Read the plan, the receipts, and the full diff.
- Inspect up to 12 files named in the diff.
- Produce a review report at
  `.ai/handoffs/YYYY-MM-DD-<task-id>-review.md`
  with these sections:
  1. Summary (1–2 sentences).
  2. Plan adherence (was the plan followed exactly?).
  3. Test coverage (do the tests cover the change?).
  4. Risk assessment (likelihood + impact + mitigation).
  5. Architecture boundaries (is a boundary changed
     without an ADR?).
  6. Security (any credential, secret, key, or PII
     exposed?).
  7. Decision: `Approve`, `Approve with comments`, or
     `Block`.
- Write the review phase receipt.

## Forbidden Actions

- Editing source or test code.
- Editing state files (`.ai/state/`).
- Committing, merging, or pushing.
- Beginning another phase.
- Skipping a section of the report.
- Producing a review without reading the full diff.

## Expected Output

Write the review phase receipt at
`.ai/receipts/phases/<task-id>-review.json`. The
receipt must declare:

- `phase: "review"`.
- `status: "completed"` (or `"blocked"` on failure).
- `report_path`: the canonical review report path.
- `decisions`: the review verdict and rationale.
- `next_phase`: `closeout` (when verdict is `Approve`)
  or `implement` (when verdict is `Block`).
- `retry_recommended: false`.
- `fallback_recommended: true` only on a `Block`
  verdict with architecture-shaped reasons.

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
     -ReceiptPath .ai/receipts/phases/<task-id>-review.json `
     -ExpectedTaskId <task-id> `
     -ExpectedPhase review
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

- The diff is empty: stop and write `status:
  "blocked"`.
- An architecture boundary is changed without an ADR:
  stop and write `fallback_recommended: true`.
- A secret, credential, or PII is exposed: stop and
  write `status: "blocked"` immediately; do not
  proceed to other sections.
- The receipt helper exited non-zero and could not be
  fixed: stop and write `status: "blocked"`. Do not
  fabricate a success.
