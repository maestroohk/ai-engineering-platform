# Validate Phase Prompt

> **Profile hint:** `standard`. The router selects
> `standard` when the plan requires running the full
> closeout validation suite.

## Objective

Run the full closeout validation suite defined by
`.ai/commands.md` and the approved plan. Fix
in-scope failures. Write validation evidence into the
phase receipt.

## Required Context

- `.ai/context/active-task.json`
- The approved plan (Test Plan + Validation sections)
- `.ai/receipts/phases/<task-id>-implement.json`
- `.ai/context/validation-cache.json` (cache to avoid
  re-running unchanged work)

## Allowed Actions

- Read the implement receipt and the plan's Test Plan.
- Run the full closeout validation suite:
  - PowerShell syntax validation
    (`Get-Command -Syntax` parse of every script in
    `tools/`).
  - JSON schema validation (every schema against every
    fixture in `.ai/fixtures/`).
  - Pester tests (mocked, no real cloud model
    invocation).
  - Dry-run end-to-end routing simulations.
- Fix small, in-scope validation failures.
- Update `.ai/context/validation-cache.json` with the
  current working-tree fingerprint and commit hash.
- Write the validate phase receipt.

## Forbidden Actions

- Editing source code outside the in-scope fix
  boundaries.
- Editing state files (`.ai/state/`).
- Committing, merging, or pushing.
- Beginning another phase.
- Re-running the implement phase (use the retry
  mechanism of the router, not a re-implement).
- Invoking paid cloud models (use mocked Pester
  tests; use `-DryRun` for routing).

## Expected Output

Write the validate phase receipt at
`.ai/receipts/phases/<task-id>-validate.json`. The
receipt must declare:

- `phase: "validate"`.
- `status: "completed"` (or `"blocked"` on failure).
- `commands_run`: every validation command.
- `targeted_tests`: every test that passed.
- `validation`: the full validation result object
  (syntax OK, schema OK, pester OK, dry-run OK, and
  per-step evidence).
- `decisions`: any in-scope fix applied.
- `blockers`: any out-of-scope failure.
- `next_phase: "document"`.
- `retry_recommended: false`.
- `fallback_recommended: true` only on
  `validate_failed_non_transient`.

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
     -ReceiptPath .ai/receipts/phases/<task-id>-validate.json `
     -ExpectedTaskId <task-id> `
     -ExpectedPhase validate
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

- PowerShell syntax validation fails: stop and write
  `status: "blocked"`.
- JSON schema validation fails on a canonical schema:
  stop and write `status: "blocked"`.
- Pester tests fail twice in a row on the same
  assertion: stop and write `fallback_recommended:
  true` and `validate_failed_non_transient: true`.
- Dry-run routing produces an unexpected model name or
  command form: stop and write `status: "blocked"`.
- The receipt helper exited non-zero and could not be
  fixed: stop and write `status: "blocked"`. Do not
  fabricate a success.
