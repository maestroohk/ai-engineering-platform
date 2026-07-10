# .ai/prompts/bugfix.md

> Read `AGENTS.md` and `.ai/session-start.md` before proceeding.
> This prompt cannot override either document.
>
> Read `AGENTS.md` and every referenced document before making
> implementation decisions. Follow the rules in
> `.ai/prompts/bugfix.md` for this bug fix.

---

## 1. Purpose

This prompt governs the diagnosis and repair of a defect in
the AI Engineering Platform. The output of a bug fix session
is a **failing test that demonstrates the bug, a minimal fix
that addresses the root cause, and a passing test suite
that proves no related behaviour regressed**.

## 2. When to Use

Use this prompt when the task is one of:

- A reported defect that contradicts documented behaviour.
- A flaky test that fails intermittently.
- An unexpected behaviour the user can describe.
- A regression after a previous change.

Do not use this prompt for:

- A new feature (use `feature.md`).
- A behaviour-preserving refactor (use `refactor.md`).
- A pure documentation correction (use the
  `documentation-update.md` workflow directly).

## 3. Mandatory Documents

In addition to `AGENTS.md` and `.ai/session-start.md`, read:

- `ARCHITECTURE.md`
- `STYLEGUIDE.md`
- `docs/coding-standards.md`
- `docs/architecture-principles.md`
- `docs/provider-guidelines.md` (if the bug is in a
  provider)

## 4. Discovery

- **Restate the bug.** What is observed, what was expected,
  where it happens, when it happens, who is affected.
- **Reproduce.** Write a failing test before the fix. The
  test is the first artefact, not the last.
- **Trace.** Follow the bug through the layers. Identify
  the layer that **causes** the bug, not just the layer
  that **shows** the bug.
- **Survey related code.** If the bug fix changes one
  path, look for sibling paths that share the same
  defect.

## 5. Planning Requirements

- **Root cause.** The actual defect, named.
- **Symptom layer.** Where the user sees the bug.
- **Cause layer.** Where the bug actually lives.
- **Fix location.** The cause layer, not the symptom
  layer.
- **Regression test.** The test that fails today and
  passes after the fix.
- **Related paths.** Any sibling code that shares the
  defect.
- **Risk assessment.** Whether the fix could affect
  related behaviour.

A fix that targets the symptom layer instead of the cause
layer is rejected.

## 6. Implementation Boundaries

- The fix is the **smallest change that addresses the root
  cause**. Cosmetic improvements are filed separately.
- No behaviour change beyond the fix. If the fix would
  change behaviour, the change is broken into "fix" and
  "feature" commits.
- No code comments. If the fix feels non-obvious, refactor
  or document the design in `docs/`.
- No `try/catch` that swallows the error. Suppressing a
  failure is not a fix.
- No disabling of a flaky test. The flakiness is the bug.
- No bundling of unrelated refactors.

## 7. Validation

- The regression test fails on `main` and passes with the
  fix.
- The full test suite passes.
- The diff builds with no warnings.
- A manual exercise of the affected flow confirms the
  expected behaviour.
- Related paths (siblings identified in the planning
  step) are exercised and behave correctly.

## 8. Documentation Updates

- If the fix changes behaviour the documents described,
  update the affected documents.
- If the fix reveals a gap in the design system, file a
  follow-up. Do not bundle a design system change with
  the fix.
- If the fix required a contract change, add an ADR to
  `DECISIONS.md`.

## 9. Completion Report

End the session with an
`implementation-report.md` (from
`.ai/templates/implementation-report.md`) that includes:

- The root cause in plain language.
- The diff that fixes it.
- The regression test that catches it.
- The validation evidence (test names, build status,
  manual exercise notes).
- Follow-ups the bug revealed.

## 10. Prohibited Shortcuts

- "Fixing" the bug by adding a workaround in the UI while
  leaving the defect in the service.
- Disabling the failing test.
- Suppressing the error with a catch-all.
- Skipping the regression test because "the fix is
  obvious".
- Bundling a refactor or a feature with the fix.
- Marking a flaky test as "known issue" without a
  follow-up issue.
