# Context Directory

> **Per-task packet, canonical paths, validation cache.**
> The router and the child sessions read these files
> instead of re-deriving repository state from scratch.

## Files

- `README.md` — this file.
- `repository-map.json` — canonical paths (`repo_root`,
  `ai_dir`, `tools_dir`, `handoffs_dir`, `plans_dir`,
  `receipts_dir`, `state_dir`, `prompts_dir`).
- `active-task.json` — the per-task packet the router
  reads before every phase. The reconcile phase can
  repair this packet from canonical sources; the
  implement / validate / document / closeout phases
  read it but do not edit it.
- `validation-cache.json` — the last full closeout
  validation result, keyed by working-tree fingerprint
  and commit hash. The router uses the cache to skip
  full validation when the implement phase did not
  change source or test files.

## Lifecycle

1. The router launches the `reconcile` child. The child
   reads `active-task.json` and may repair small,
   mechanical fields. The child writes the reconcile
   phase receipt.
2. Each subsequent phase reads the previous phase
   receipt and re-reads `active-task.json` to confirm
   scope.
3. The `closeout` phase archives the active packet by
   copying it to
   `.ai/archive/context/<task-id>-active-task.json`
   and resets `active-task.json` for the next `Ready`
   task.

## Validation

`active-task.json` is validated by
`.ai/context/active-task.schema.json` (when present)
and the `repository-map.json` paths are validated
against the actual filesystem by the router at start.

## Stop Conditions

- `active-task.json` references a path that does not
  exist: the router writes a malformed packet and
  selects `economy` (reconcile) or `standard`
  (repair) to fix it.
- `active-task.json` is missing: the router stops with
  `status: "blocked"`.
- The cached validation result is older than the
  current working-tree commit: the router re-runs
  full validation.
