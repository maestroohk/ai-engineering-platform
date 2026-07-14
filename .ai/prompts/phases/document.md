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

## Stop Conditions

- A required template is missing: stop and write
  `status: "blocked"`.
- A schema validation fails: stop and write
  `status: "blocked"`.
- The implementation report exceeds 200 lines: stop
  and write `status: "blocked"`.
