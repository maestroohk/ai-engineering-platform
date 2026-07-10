# `.ai/plans/` — Approved Implementation Plans

> **Canonical, version-controlled home for approved
> implementation plans.** This directory is part of the
> repository. The temporary Claude-owned plans live
> under `.claude/plans/` and are **not** tracked.

---

## 1. Why two plan locations

The platform distinguishes between two kinds of
planning state:

| Location          | Owner       | Lifetime            | Tracked in git | Purpose                                                                 |
| ----------------- | ----------- | ------------------- | -------------- | ----------------------------------------------------------------------- |
| `.claude/plans/`  | Claude      | One session         | No             | Scratch space. Drafter's working copy. May be deleted between sessions. |
| `.ai/plans/`      | Repository  | Permanent           | Yes            | Source of truth. The approved plan that the implementation must follow. |

The temporary copy is the AI's working area. The
repository copy is what the implementation report
links back to and what code review verifies
against. Once a plan is approved, the repository
copy is authoritative; the temporary copy may be
kept for traceability, but the implementation
report cites the `.ai/plans/` file.

---

## 2. Naming Convention

`<Milestone-or-Task-Name>.md`

- Use the milestone code (e.g. `M1.2`) when the plan
  is for a numbered roadmap milestone.
- Use a kebab-case task name (e.g.
  `bugfix-app-button-disabled-state.md`,
  `refactor-application-handler-pipeline.md`) when
  the plan is for an ad-hoc change.
- File names are stable: a plan is **never renamed
  silently** after approval, and a superseded plan
  is annotated in its `Approval` block (status:
  `Superseded`) rather than overwritten.

Examples already present:

- `.ai/plans/M1.2-design-system-core.md`

---

## 3. Plan Status Field

Every plan in this directory carries an `Approval`
block with a `Status` field. Allowed values:

- `Draft` — the plan is being written or revised.
  Not yet approved. The AI may continue to edit.
- `Awaiting Approval` — the plan is finished and
  presented to the human for review. **The AI
  MUST NOT begin implementation while the plan is
  in this state.** Edits are limited to
  clarification, not scope expansion.
- `Approved` — the human has approved the plan.
  Implementation may begin. Edits require a
  deviation note in the matching implementation
  report.
- `Superseded` — a newer plan replaces this one.
  The plan is preserved for traceability; the
  newer plan's `Linked Artefacts` block points
  back to it.

A plan transitions through:

```
Draft → Awaiting Approval → Approved → (Superseded)
```

The transition is recorded in the plan's
`Approval` block. A plan that is "Approved" is
**historical**: changes to its content are a
deviation and must be reported.

---

## 4. Relationship to Implementation Reports

Per `.ai/templates/implementation-report.md`, every
implementation report includes:

- **Approved plan** — the plan's display name
  (e.g. "M1.2 — Design System Core").
- **Plan path** — the path to the approved plan in
  this directory, e.g.
  `.ai/plans/M1.2-design-system-core.md`.
- **Deviations from plan** — any change the
  implementation made that the plan did not
  foresee. A deviation is not a failure; an
  unreported deviation is.

This makes the plan the contract and the
implementation report the receipt. The two pair
together in code review.

---

## 5. Plans Are Historical Artefacts

Once a plan's `Status` becomes `Approved`, the
plan is **not silently rewritten**. The plan is
a record of what the team agreed to build. If the
implementation diverges from the plan, the
deviation is recorded in the implementation
report, not by editing the plan.

Allowed edits to an `Approved` plan:

- Fixing a typo or broken link in
  non-substantive text.
- Adding a `Notes` or `Post-Approval Amendments`
  appendix that records changes without rewriting
  the original decision.

Disallowed edits to an `Approved` plan:

- Changing the file's `Status` from `Approved` to
  anything else. (Use `Superseded` if a new plan
  replaces it.)
- Removing or weakening planned tests.
- Removing planned components or files.
- Changing the architecture-test scope to be
  more permissive.

If the team wants to substantively change an
approved plan, the right move is to write a
new plan that explicitly supersedes the old one
and reference the old one in the new plan's
`Linked Artefacts` block.

---

## 6. Authoring a New Plan

1. Create the plan in `.claude/plans/` first (the
   AI's working area). The plan's status is
   `Draft`.
2. Revise the plan until the human is satisfied.
3. When ready for review, set the status to
   `Awaiting Approval` and write the canonical
   copy to `.ai/plans/<name>.md`. The two files
   may differ in formatting (the canonical copy is
   tidier); the content must match.
4. The human approves or rejects. If rejected,
   the plan returns to `Draft`; the canonical
   copy may be edited or deleted.
5. Once approved, the canonical copy's status
   becomes `Approved`. The temporary copy may be
   deleted or kept for traceability.

---

## 7. Index of Approved Plans

| Plan                                                          | Status   | Milestone |
| ------------------------------------------------------------- | -------- | --------- |
| `.ai/plans/M1.2-design-system-core.md`                        | Awaiting Approval | M1.2      |

The index is updated as plans are added.
