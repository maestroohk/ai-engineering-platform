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
