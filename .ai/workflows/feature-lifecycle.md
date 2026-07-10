# .ai/workflows/feature-lifecycle.md

> The end-to-end lifecycle of a feature: discovery, plan,
> approval, implementation, tests, documentation, review,
> report. Every feature in the AI Engineering Platform moves
> through these stages. No stage is optional.

---

## 1. Purpose

This workflow sequences the work the `.ai/prompts/feature.md`
prompt describes. It exists so that a feature does not skip a
stage, so that the artefacts produced at each stage are
consistent across features, and so that a session can be
resumed from any stage.

The workflow is also the structure the
`implementation-plan.md` and `implementation-report.md`
templates are organised around.

## 2. Stages

### Stage 1 — Discovery

- **Inputs:** the user's request.
- **Outputs:** a restated task, a list of inspected files,
  acceptance criteria in the AI's own words.
- **Prompt:** `.ai/prompts/feature.md` (sections 1–4).
- **Workflow step:** read `AGENTS.md`, read
  `.ai/session-start.md`, classify the request, read the
  prompt, read the supporting standards, inspect the
  repository and the Git state.

### Stage 2 — Plan

- **Inputs:** the restated task.
- **Outputs:** `implementation-plan.md` (from
  `.ai/templates/implementation-plan.md`).
- **Prompt:** `.ai/prompts/feature.md` (section 5).
- **Workflow step:** write the plan. The plan must include
  acceptance criteria, components reused and added, services
  added, providers used, files to add/modify/delete, tests
  planned, documentation to update, and out of scope.
  The plan must also include:
  - **Project boundary review** (per ADR-011). Which
    existing project does the feature live in? If a new
    project is needed, the plan justifies it (at least
    three files would naturally belong to it; the project
    provides a compile-time boundary the platform
    needs). Speculative projects are rejected.
  - **Progressive self-dogfooding (per ADR-013).** Does
    the feature consume a stable reusable capability
    delivered by an earlier milestone? The plan names
    the capability, the contract, and the registry or
    service the feature resolves it through. A direct
    bypass — a feature that re-implements a registered
    provider inline, or that calls `Process.Start` from
    a non-Infrastructure project — is rejected.

### Stage 3 — Approval

- **Inputs:** the plan.
- **Outputs:** explicit approval from the human.
- **Workflow step:** the AI presents the plan and waits. No
  code is written before approval. Pre-authorised tasks may
  skip the wait, but the plan is still recorded.

### Stage 4 — Implementation

- **Inputs:** the approved plan.
- **Outputs:** the diff.
- **Prompt:** `.ai/prompts/feature.md` (section 6).
- **Workflow step:** implement in layers — contracts, services,
  components, pages, tests. Keep changes small enough to
  review. No code comments. No magic strings. No upward
  dependencies.

### Stage 5 — Tests

- **Inputs:** the diff.
- **Outputs:** green tests, including the new tests in the
  plan.
- **Prompt:** `.ai/prompts/testing.md` (as a reference).
- **Workflow step:** write the tests alongside the code.
  Cover the happy path, the failure paths, and the
  boundary conditions. Run the full suite before
  declaring done.

### Stage 6 — Documentation

- **Inputs:** the diff and the documentation plan.
- **Outputs:** the documents updated per the plan.
- **Workflow:** see `.ai/workflows/documentation-update.md`.
- **Workflow step:** update the design system, the component
  guidelines, the provider guidelines, the architecture
  principles, the roadmap, and any ADR that captures a
  non-obvious decision.

### Stage 7 — Review

- **Inputs:** the diff and the documentation changes.
- **Outputs:** `review-report.md` (from
  `.ai/templates/review-report.md`).
- **Prompt:** `.ai/prompts/review.md`.
- **Workflow step:** review every dimension (architecture,
  DRY, components, accessibility, security, tests,
  documentation, style). File findings by severity.

### Stage 8 — Report

- **Inputs:** the diff, the tests, the documentation
  changes, the review.
- **Outputs:**
  - `implementation-report.md` (from
    `.ai/templates/implementation-report.md`).
  - **Updated `.ai/state/current.md`** — the
    one-page snapshot of the project right now.
  - **Updated `.ai/state/task-board.md`** — the
    task the session worked moves to `Done` (or
    `Blocked`); any new `Ready` items are added.
  - **Session handoff** at
    `.ai/handoffs/YYYY-MM-DD-<slug>.md` (and
    mirrored to `.ai/handoffs/latest.md`).
- **Workflow step:** produce the report and
  the state updates. The report includes the
  validation results, the documentation updated,
  the deviations, the known limitations, and the
  next recommended step. The state files capture
  the **live** state — short, high-signal, no
  prose. The handoff is the bridge to the next
  session and follows
  `.ai/templates/session-handoff.md`.

## 3. Stage Gates

A stage is "gated" if its predecessor must be complete
before it starts. The feature-lifecycle is fully gated:

```
Discovery → Plan → Approval → Implementation → Tests →
Documentation → Review → Report
```

A feature that skips a gate is incomplete. A feature that
pretends to have completed a stage (e.g. "tests are passing"
when the suite was not run) is rejected in review.

## 4. Resumption

If a session is paused, it produces a `session-handoff.md`
(from `.ai/templates/session-handoff.md`) that records:

- The current stage.
- The work completed.
- The files changed.
- The test status.
- The exact next step.
- The documents the next session must read.

The handoff is written to
`.ai/handoffs/YYYY-MM-DD-<slug>.md` and mirrored to
`.ai/handoffs/latest.md`. The next session reads
`.ai/handoffs/latest.md` first, then `.ai/state/current.md`
and `.ai/state/task-board.md`, then the matching handoff
and the matching implementation report. The next session
resumes at the recorded stage.

## 5. Anti-Patterns

- Implementing before the plan is approved.
- Running tests only at the end, after a long implementation
  block.
- Treating "the code is self-explanatory" as a reason to
  skip documentation.
- Producing an implementation report that does not name
  the files or the tests.
- Skipping the review stage because the change is small.
- Closing a session without a report, a handoff, or an
  update to `.ai/state/current.md` and
  `.ai/state/task-board.md` (Rule 15 in `AGENTS.md`).
