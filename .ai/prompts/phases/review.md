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

## Stop Conditions

- The diff is empty: stop and write `status:
  "blocked"`.
- An architecture boundary is changed without an ADR:
  stop and write `fallback_recommended: true`.
- A secret, credential, or PII is exposed: stop and
  write `status: "blocked"` immediately; do not
  proceed to other sections.
