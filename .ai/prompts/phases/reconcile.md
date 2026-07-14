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

## Stop Conditions

- The active packet references a non-existent plan or
  handoff: stop and write `fallback_recommended: true`.
- The active task ID is not in `.ai/state/tasks.json`:
  stop and write `status: "blocked"`.
- The previous phase receipt reports `status: "blocked"`
  and the active packet has not been re-baselined.
