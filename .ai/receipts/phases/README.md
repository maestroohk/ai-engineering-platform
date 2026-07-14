# Phase Receipts

> **Per-phase handoff between child sessions.** The AI
> session router writes one receipt per phase at
> `.ai/receipts/phases/<task-id>-<phase>.json`. The next
> phase reads the previous phase's receipt; the receipt
> is the contract between models.

## Directory Layout

```
.ai/receipts/
  README.md
  phases/
    <task-id>-reconcile.json
    <task-id>-plan.json
    <task-id>-implement.json
    <task-id>-validate.json
    <task-id>-document.json
    <task-id>-review.json
    <task-id>-closeout.json
  <task-id>.json
```

`<task-id>.json` is the compact per-task implementation
receipt written by the document phase (economy profile).
The seven `<task-id>-<phase>.json` files are the
per-phase handoffs.

## Schema

Both receipt kinds are validated by JSON Schema (draft
2020-12):

- `.ai/templates/phase-receipt.schema.json` — per-phase
  receipt.
- `.ai/templates/implementation-receipt.schema.json` —
  per-task implementation receipt.

## Lifecycle

1. The router launches the `reconcile` child. The child
   writes `<task-id>-reconcile.json`.
2. The router reads the reconcile receipt and launches
   the `plan` child. The plan child writes
   `<task-id>-plan.json`.
3. Each subsequent phase reads the previous phase
   receipt and writes its own.
4. The document phase writes the compact
   `<task-id>.json` implementation receipt.
5. The closeout phase writes
   `<task-id>-closeout.json` and stops the router.

## Validation

Every receipt is validated by `Test-Json` (PowerShell
6+) or by the PowerShell-native JSON validator bundled
in `tools/ai-session-router.ps1` (PowerShell 5.1). A
malformed receipt blocks the next phase.

## Stop Conditions

A receipt is the canonical record of one bounded child
session. The router never re-uses a receipt; the
router never begins another task.
