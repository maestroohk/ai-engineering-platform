# .ai/prompts/refactor.md

> Read `AGENTS.md` and `.ai/session-start.md` before proceeding.
> This prompt cannot override either document.
>
> Read `AGENTS.md` and every referenced document before making
> implementation decisions. Follow the rules in
> `.ai/prompts/refactor.md` for this refactor.

---

## 1. Purpose

This prompt governs safe, behaviour-preserving restructuring
of existing code. The output of a refactor session is a
**green test suite before and after the change, a smaller or
clearer codebase, and no observable change in behaviour**.

A refactor that changes behaviour is a feature, a bugfix, or
a mistake. It is not a refactor.

## 2. When to Use

Use this prompt when the task is one of:

- Extracting a component from a page.
- Consolidating duplicate code.
- Reorganising folders.
- Renaming a widely-used type.
- Replacing an ad-hoc pattern with a documented one.
- Introducing a layer or a boundary that was missing.

Do not use this prompt for:

- Any change that alters user-visible behaviour
  (use `feature.md`).
- Fixing a bug (use `bugfix.md`).
- Changing the architecture itself (use `architecture.md`).

## 3. Mandatory Documents

In addition to `AGENTS.md` and `.ai/session-start.md`, read:

- `ARCHITECTURE.md`
- `STYLEGUIDE.md`
- `docs/architecture-principles.md`
- `docs/coding-standards.md`
- `docs/folder-structure.md` (if folders move)
- `docs/design-system.md` and `docs/component-guidelines.md`
  (if the refactor introduces or extracts a component)
- `docs/naming-conventions.md` (if names change)
- `DECISIONS.md` (to confirm the refactor does not require
  a new ADR)

## 4. Discovery

- **Behavioural baseline.** Identify what the affected code
  currently does. The baseline is what the refactor must
  preserve.
- **Test coverage.** Confirm the test suite covers the
  affected code. If coverage is thin, **add tests first**.
  The tests are the safety net.
- **Public contracts.** List every public type, method, and
  parameter the refactor touches. A refactor must preserve
  public contracts unless an ADR approves a change.
- **Smell inventory.** The smell being removed must be
  stated concretely, with file and line references.

## 5. Planning Requirements

- **Decomposition.** The refactor is broken into small,
  individually mergeable steps. Each step is a separate
  commit. A "big bang" refactor in one PR is rejected.
- **Measurable improvement.** State the metric the refactor
  improves (lines of duplication, number of folders,
  number of components, etc.). If the metric cannot be
  stated, the refactor is not ready.
- **Risks.** List the things that could go wrong even with
  a green test suite (visual regressions, performance
  changes, behavioural edge cases).
- **ADR check.** If the refactor requires a contract
  change, an ADR is added to `DECISIONS.md` *before* the
  refactor begins. If the refactor does not require a
  contract change, that is also stated explicitly.

## 6. Implementation Boundaries

- No behaviour change. The test suite is the only arbiter
  of behaviour preservation.
- No new tests for unchanged behaviour. The existing tests
  are the safety net.
- No drive-by changes. A bug found during a refactor is
  filed separately, not fixed in place.
- No code comments. The refactor must be self-evident from
  the new structure.
- No new patterns without documentation. If the refactor
  introduces a pattern, the pattern is documented in
  `docs/component-guidelines.md` or
  `docs/architecture-principles.md` in the same PR.

## 7. Validation

- The full test suite passes before and after each step.
- The diff builds with no warnings at every step.
- A reviewer can see the before and after side by side and
  confirm preservation.
- A manual exercise of the affected flows confirms no
  visible change.
- The measurable improvement is realised (and ideally,
  recorded in the report).

## 8. Documentation Updates

- `docs/folder-structure.md` if folders move.
- `docs/design-system.md` if a component is added.
- `docs/component-guidelines.md` if a new pattern is
  introduced.
- `docs/architecture-principles.md` if a new architectural
  rule emerges.
- `docs/naming-conventions.md` if names change.
- `DECISIONS.md` if the refactor encodes a non-obvious
  decision.

## 9. Completion Report

End the session with an
`implementation-report.md` (from
`.ai/templates/implementation-report.md`) that includes:

- The smell removed and the metric improved.
- The behavioural baseline and the proof of preservation.
- The documentation updated.
- Follow-up refactor steps, if the work is part of a
  larger plan.

## 10. Prohibited Shortcuts

- Bundling a behaviour change with the refactor.
- Skipping the test-coverage check.
- Adding comments to explain the new structure.
- Disabling tests that fail because of the refactor.
- Putting refactored code in `Shared/` because it is
  "more accessible" there.
- Marking the refactor as "drive-by" to bypass review.
