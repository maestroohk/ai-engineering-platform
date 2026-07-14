# Archive Directory

> **Historical long-form records.** The fast path
> consults the indexes at `.ai/index/*.json`; the slow
> path reads the archive. The archive stores the
> historical implementation reports, plans, handoffs,
> and closed active-task packets that the indexes
> point to.

## Layout

```
.ai/archive/
  README.md
  reports/
    <year>-<month>-<task-id>-<short-slug>.md
  plans/
    <year>-<month>-<task-id>-<short-slug>.md
  handoffs/
    <year>-<month>-<task-id>-<short-slug>.md
  context/
    <task-id>-active-task.json
  receipts/
    <task-id>.json
    phases/
      <task-id>-<phase>.json
```

## Lifecycle

The closeout phase copies the active task's
implementation report, plan, handoff, and the final
active-task packet into the archive, then appends the
same record to the corresponding index.

## Validation

The indexes at `.ai/index/*.json` are validated by
JSON parse. The archive contents are not
re-validated; the canonical artifacts live at
`.ai/handoffs/`, `.ai/plans/`, `.ai/receipts/`, and
`.ai/context/`.

## Stop Conditions

- The archive directory is full and a write would
  fail: stop the closeout phase and write
  `status: "blocked"`.
- The corresponding index is missing: stop and write
  `status: "blocked"`.
