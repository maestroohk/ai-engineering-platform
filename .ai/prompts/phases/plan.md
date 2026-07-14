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

## Stop Conditions

- The active packet's `included_scope` is empty: stop
  and write `plan_incomplete: true`.
- The plan requires a new ADR that does not exist: stop
  and write `fallback_recommended: true`.
- Cross-project reasoning is required (capability
  cross-references span more than one milestone): stop
  and write `fallback_recommended: true`.
