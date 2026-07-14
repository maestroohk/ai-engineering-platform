# Plan Phase Prompt

> **Profile hint:** `high`. The router escalates to `high`
> when the plan crosses a milestone boundary or the task
> type is `architecture` / `bootstrap`.

## Objective

Produce (or repair) an execution-ready implementation
plan for the active task. The plan must be small,
testable, and scoped to the included scope of the active
packet. The plan must reference the relevant ADRs,
capabilities, and existing tests.

## Required Context

- `.ai/context/active-task.json`
- `.ai/context/repository-map.json`
- `.ai/plans/README.md` (the plan index rules)
- `.ai/templates/implementation-plan.md` (canonical
  template)
- `.ai/state/capabilities.json` (capability registry)
- `DECISIONS.md` (architectural decisions)
- `.ai/receipts/phases/<task-id>-reconcile.json`

## Allowed Actions

- Read the active packet, the previous reconcile receipt,
  the relevant ADRs, and the related capabilities.
- Inspect 6–14 files named in the active packet's
  `required_context_files` and `optional_context_files`.
- Read `git log -1 --stat` to see the most recent commit
  on the active branch.
- Write the plan to
  `.ai/plans/<YYYY-MM-DD>-<task-id>-<short-slug>.md`.
- Write the plan phase receipt.

## Forbidden Actions

- Editing source or test code.
- Editing state files (`.ai/state/`).
- Committing, merging, or pushing.
- Beginning another phase.
- Reordering milestones.
- Skipping tests or validation steps.

## Plan Must Declare

- Files added (with one-line purpose each).
- Files modified (with rationale).
- Files deleted (with rationale).
- Components and services added or changed.
- Risks (likelihood + impact + mitigation).
- Test plan (targeted tests + full closeout validation).
- Documentation plan (README, ADR, ROADMAP, state).
- Approval envelope (submitted / reviewed / date /
  status / canonical path).

## Expected Output

Write the plan phase receipt at
`.ai/receipts/phases/<task-id>-plan.json`. The receipt
must declare:

- `phase: "plan"`.
- `status: "completed"` (or `"blocked"` on failure).
- `plan_path`: the canonical plan path.
- `decisions`: the key design decisions taken.
- `next_phase: "implement"`.
- `retry_recommended: false`.
- `fallback_recommended: true` only on plan
  incompleteness.

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
     -ReceiptPath .ai/receipts/phases/<task-id>-plan.json `
     -ExpectedTaskId <task-id> `
     -ExpectedPhase plan
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

- The active packet's `included_scope` is empty: stop
  and write `plan_incomplete: true`.
- The plan requires a new ADR that does not exist: stop
  and write `fallback_recommended: true`.
- Cross-project reasoning is required (capability
  cross-references span more than one milestone): stop
  and write `fallback_recommended: true`.
- The receipt helper exited non-zero and could not be
  fixed: stop and write `status: "blocked"`. Do not
  fabricate a success.
