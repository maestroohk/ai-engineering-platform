# .ai/workflows/branching-strategy.md

> **The canonical Git workflow for the AI Engineering Platform.**
> This document is the **single source of truth** for branching
> and merging. If another document in this repository contradicts
> this one, this one wins. If a contributor changes this document,
> the change is recorded in `DECISIONS.md` and announced on the
> next session-start.

---

## 1. Topology

```
main
    ↑
    │   merge (one coherent commit per task)
    │
feature/T-001-<short-kebab-description>
    ↓
    │   work happens here, on top of main
    │
main  ←  start of every feature branch
```

`main` is the default branch. `main` is always the latest stable,
releasable version of the product. Every commit on `main` is a
completed task that has passed the full validation gate (build +
tests + format + smoke).

## 2. Rules

1. **`main` is always the latest stable, releasable version of the
   product.** A commit lands on `main` only when it has passed the
   full validation gate.
2. **Every task starts from the latest `main`.** A feature branch
   is created from `main`'s tip at the moment the task begins; the
   branch is rebased onto `main` if `main` advances before the
   task merges.
3. **Every task has its own feature branch.** No two tasks share a
   feature branch. No work happens directly on `main`.
4. **Branch names use the task ID followed by a short kebab-case
   description:** `feature/T-001-application-shell`,
   `feature/T-002-navigation-registry`,
   `feature/T-003-topbar-breadcrumbs`. The task ID is the canonical
   reference in `tasks.json`; the description is a short,
   human-readable summary that does not include the milestone
   (the milestone is in the task's `delivered_by_milestone`
   field).
5. **Every task ends with one coherent commit.** A coherent commit
   is a single atomic change that includes the implementation, the
   tests, the documentation, the implementation report, the
   handoff, and the project-continuity state updates (per Rule 15
   in `AGENTS.md` and per the `feature-lifecycle.md` workflow).
6. **Every completed task is merged into `main`.** The merge is
   a fast-forward (no merge commits in `main`); the feature
   branch is rebased onto `main` first if necessary, then
   fast-forwarded into `main`.
7. **After merging, the feature branch is deleted.** Long-lived
   feature branches are not permitted. A feature branch is the
   working space of a single task; once the task is on `main`,
   the branch is gone.
8. **Every milestone leaves `main` in a releasable state.** The
   milestone-closeout commit on `main` carries the implementation
   report, the handoff, the state updates, and the milestone
   retrospective (per `CONTRIBUTING.md` § 9).
9. **Every milestone receives a milestone tag.** A milestone tag
   is `m<milestone-number>` (e.g. `m2`, `m2.5`, `m3`). The tag
   points at the milestone's closeout commit on `main`. Tags are
   not pushed without explicit authorisation.
10. **Future sessions automatically assume this workflow.** A
    session that begins work on a new task does not re-derive the
    workflow; it reads this document once and applies the rules.
11. **This workflow is documented once, in this file.** No other
    document restates the branching strategy in full. References
    to "the branching strategy" or "the branching model" point
    here. Any document that previously restated the workflow in
    full has been updated to reference this file.
12. **Conflicting or obsolete guidance is removed.** When this
    document is updated, the session that updates it scans the
    repository for documents that restate or contradict the
    changed rules and updates them to reference this file or
    deletes the conflicting guidance.

## 3. Branch Lifecycle

A feature branch has exactly four states:

1. **Created.** `git checkout -b feature/T-001-<description>`
   from `main`'s tip. The branch carries no work yet.
2. **Active.** Commits land on the feature branch. There is
   exactly one commit per task (per rule 5); the commit is the
   coherent commit.
3. **Merged.** `git checkout main && git merge --ff-only
   feature/T-001-<description>` fast-forwards `main` to the
   feature branch's tip. The branch is now redundant.
4. **Deleted.** `git branch -d feature/T-001-<description>`
   removes the branch. The work lives on `main`; the branch is
   not needed.

A branch in state 1 or 2 may be force-deleted at any time (the
work is preserved in the reflog for 90 days by default). A
branch in state 3 is redundant; the deletion is automatic and
immediate.

## 4. Milestone Tags

A milestone tag is created at the closeout commit of a
milestone. The tag is named `m<milestone-number>` (e.g. `m2`,
`m2.5`, `m3`). The tag is annotated (not lightweight) and
carries the milestone's implementation-report path in its
message.

```bash
git tag -a m2.5 -m "M2.5 closeout: empty routes, responsive, a11y, T-017 fix. See implementation-report-m2-5-empty-routes-responsive-accessibility.md"
```

A tag is a permanent reference to a milestone's stable state. A
tag is never moved or deleted; if a milestone is re-cut, the
new tag is `m2.5.1` (or similar) and the old tag is preserved.

## 5. Re-keying the Default Branch

The repository renamed the default branch from `master` to
`main` on 2026-07-11. The rename is one-time; future
contributors do not re-derive the history. The legacy `master`
branch was deleted in the same infrastructure commit; the
docs and state in this repository reflect the new naming.

Branch-name references in older reports (M0.5, M1, M2.1, M2.2)
that name the legacy `master` branch have been re-keyed to
`main`. The phrase "master delivery plan" (the file
`.ai/plans/master-delivery-plan.md`) is preserved; the Git
branch is what changed, not the file.

## 6. Legacy Branch Names (M0.5 → M2.5)

The M0.5 → M2.5 feature branches use the older naming
convention `feature/<milestone>-<short-kebab>` (e.g.
`feature/m2-1-application-shell`,
`feature/m2-5-empty-routes-responsive-accessibility`). These
branches are part of the historical record; they are not
renamed retroactively. The new convention (rule 4) applies to
all branches created after the rename on 2026-07-11.

## 7. Anti-Patterns

- Working directly on `main` (no feature branch).
- Two tasks sharing a feature branch.
- A feature branch that lives across multiple milestone
  closeouts.
- A feature branch whose name does not start with a task ID
  (e.g. `feature/awesome-refactor`).
- A commit on `main` that does not pass the full validation
  gate.
- A milestone that lands without a tag.
- A re-statement of the branching strategy in any document
  other than this one.
