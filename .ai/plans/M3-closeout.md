# M3 Closeout — M3 Retrospective and M3 → Done

> **The M3 closeout plan.** This plan promotes the
> M3 closeout work (the M3 retrospective per the
> Milestone Closeout Standard § 4, the M3 → Done
> state transition, the `m3` annotated milestone
> tag, the M4-A plan, and the project-continuity
> state update) from a summary entry to a full
> closeout slice. M3 has two implementation slices
> (M3.1 + M3.2) and one closeout slice (M3.x — this
> slice). M3.x is the M2-style wrap-up for the M3
> milestone: the verification, the retrospective,
> the project-continuity update, the handoff, the
> implementation report, and the M4-A plan in
> `Awaiting Approval`.
>
> **Status:** Approved (2026-07-11; user pre-
> authorised in the M3 closeout brief — the brief
> is the approval, no separate plan-review step).
>
> **Branch:** `feature/T-020-m3-closeout-and-retrospective`
> (created from `main` at the M3.2 closeout commit
> `ff9010a`; per the branching strategy in
> `.ai/workflows/branching-strategy.md`).

---

## 1. Why This Slice Exists

M3 has two implementation slices (M3.1 + M3.2)
and one closeout slice (M3.x). M3.x is the
M2-style wrap-up: it verifies the M3 implementation
against the ROADMAP M3 definition of done, records
deferred reviews, introduces the M3 retrospective
(the second milestone retrospective in this
repository, after the M2 retrospective), and ships
the M4-A plan in `Awaiting Approval`.

M3.x does not add new product functionality. M3.x
is the **engineering hygiene** that closes the M3
milestone properly: a verified, evidence-backed,
and retrospected M3 that the next milestone (M4-A)
can build on with confidence.

The M3 closeout follows the Milestone Closeout
Standard (introduced in M2.6; the canonical
procedure every future milestone closeout must
follow). The standard is unchanged by the M3
closeout. The M3 closeout mirrors the M2.6
closeout's structure with M3-specific evidence.

## 2. In Scope

1. **Ship the M3 retrospective** at
   `retrospective-m3-project-registration.md`
   (13 sections, per the Milestone Closeout
   Standard § 4; the structure mirrors the M2
   retrospective with M3-specific evidence).
   The 13 sections: delivered capabilities,
   deferred capabilities, technical debt, known
   issues, lessons learned, architecture changes,
   documentation changes, testing summary,
   validation results, implementation reports,
   commit range, readiness for M4-A,
   recommendations for the next milestone.
2. **Produce the M4-A plan** at
   `.ai/plans/M4-A-infrastructure-process-execution.md`
   (12 sections, mirrors the M3 plan's
   structure; the plan is a first draft; the
   M4-A first session reviews and revises the
   plan as needed). The plan is `Awaiting
   Approval`; the M4-A first session approves
   the plan and begins the M4-A implementation.
3. **Update the project-continuity state per
   Rule 15** (the same per-slice state
   updates, plus the milestone-level updates):
   capability evidence, milestone evidence,
   task state, session state, structured JSON,
   markdown projections, ROADMAP.md, master
   delivery plan, current.md, task-board.md,
   latest handoff, milestone history,
   implementation report, retrospective.
4. **Move M3 from `Active` to `Done`** in
   `.ai/state/milestones.json` with
   `closed_at: 2026-07-11` and the M3
   evidence block updated with the M3
   closeout's handoff, implementation
   report, retrospective, and the M3
   closeout slice entry.
5. **Create the `m3` annotated milestone
   tag** at the M3 closeout commit on `main`
   per the branching strategy rule 9. The
   tag's message references the M3
   retrospective path.
6. **Promote the first M4-A task** (T-021 —
   the M4-A.1 infrastructure project
   skeleton) to `Ready` in
   `.ai/state/tasks.json`. The task is the
   first dependency-satisfied task the M4-A
   plan names; per the Milestone Closeout
   Standard § 8, the closeout does not
   invent a new task.
7. **Validate the complete M3 milestone.**
   The validation gate is the same as the
   per-slice gate plus a milestone-level
   gate:
   - `npm run css:build` — the Tailwind +
     PostCSS pipeline emits `app.css`
     cleanly.
   - `dotnet restore` — every project's
     dependencies resolve.
   - `dotnet build` — 0 warnings, 0
     errors.
   - `dotnet test` — every test passes
     (the registered axe-core and
     provider-boundary tests remain
     registered-but-disabled per ADR-016
     / M4-D; 273 passed, 0 failed, 7
     skipped).
   - `dotnet format --verify-no-changes`
     — the format is clean.
   - **Visual smoke** — the M3 route
     (`/projects`) returns 200 and the
     critical interactions (Register a
     project button enabled, registration
     modal opens, registering a project
     navigates to the populated state,
     renaming updates the name,
     unregistering removes the project)
     work end-to-end.
8. **Update ROADMAP.md** (M3 row `Active`
   → `Done`; M3 paragraph updated; the M3
   closeout row in the M3 slice table
   updated from "M3.x — M3 retrospective"
   to `Delivered (M3 closeout, 2026-07-11)`)
   and § 3 (M3 DoD bullets: every DoD item
   checked satisfied; the Open action is
   M4-A's responsibility and remains
   explicitly out of scope; the M3
   retrospective is named; the M4-A plan
   path is named).
9. **Update `.ai/plans/master-delivery-plan.md`**
   (§ 1 M3 row `Active` → `Done (closed
   2026-07-11)`; M3 last-stable-evidence
   column updated with the M3 closeout
   commit + the M3 retrospective path) and
   § 3 (M3 completion status `Active` →
   `Done (closed 2026-07-11)`; M3 evidence
   list updated; M3 closeout slice row
   `Summary entry` → `Delivered (2026-07-11)`).
10. **Coherent commit on the feature branch**
    with a focused, reviewable diff (per
    Rule 17).
11. **Fast-forward merge to `main`** per the
    branching strategy (rule 6: every
    completed task is merged into `main`;
    rule 7: the feature branch is deleted
    after the merge).
12. **Skip push** (not authorised in this
    session). The push decision recorded in
    the implementation report is **Staged
    for push** (the user did not authorise in
    this session; the closeout did not push;
    the next user command may push).

## 3. Out of Scope

1. **M4-A implementation.** M4-A begins in a
   separate session after the M3 closeout.
   The M3 closeout produces the M4-A plan
   in `Awaiting Approval` and promotes the
   first M4-A task (T-021) to `Ready`; the
   M3 closeout does not begin the M4-A
   implementation. The Progressive Coding
   Rule applies: one task per session,
   13-step lifecycle, stop after the
   coherent commit.
2. **Provider creation.** M4-D is the
   activation milestone for the four
   registered-but-disabled composition-root
   architecture tests. M3 closeout does not
   create providers. M3 closeout does not
   activate the four tests.
3. **Reordering accepted milestones.** M0,
   M0.5, M1, M2 are `Done` and immutable
   in their order. M3 closeout does not
   modify the order. M3 is appended as
   the next `Done` milestone.
4. **Modifying any source code.** M3 closeout
   is a docs + workflow + state change; no
   source code is modified. M3 closeout
   does not modify any test code. M3
   closeout does not modify any build
   configuration. M3 closeout does not
   modify any file under `src/`, `tests/`,
   `package.json`, `tailwind.config.js`, or
   `Directory.Build.props`.
5. **Modifying any constitutional rule.**
   M3 closeout is a workflow + state change;
   no constitutional rule is added or
   removed. M3 closeout does not modify
   `AGENTS.md`, `ARCHITECTURE.md`,
   `DECISIONS.md`, `STYLEGUIDE.md`, or
   `CONTRIBUTING.md`. M3 closeout does not
   modify the Milestone Closeout Standard
   (`.ai/workflows/milestone-closeout.md`).
6. **Modifying the M3 plans.** The M3.1
   plan (`.ai/plans/M3-project-registration.md`)
   and the M3.2 plan
   (`.ai/plans/M3.2-project-registration-slice-2.md`)
   are the M3 implementation plans; they
   are unchanged by the M3 closeout. The M3
   closeout plan
   (`.ai/plans/M3-closeout.md`) is the new
   plan; the M3 closeout implementation
   follows the plan.
7. **Modifying capabilities, features, or
   providers state.** M3 closeout does not
   modify `.ai/state/project.json`,
   `.ai/state/providers.json`,
   `.ai/state/features.json`, or
   `.ai/state/capabilities.json`. The
   project identity, providers, features,
   and capabilities are unchanged. The
   M3 closeout updates the M3 row in
   `milestones.json` and the C-016 status
   in `capabilities.json` is preserved
   (the M3 closeout does not add a
   `completion_status` field; the existing
   `status: Accepted` is accurate; the
   project-intelligence refinement task
   in the task board's `Deferred` section
   is the right place for the
   `completion_status` field; the M3
   closeout does not pre-empt that task).
8. **Pushing to the remote.** The user did
   not authorise push in this session;
   the closeout did not push. The push
   decision is **Staged for push**;
   the next user command may push.

## 4. Files to Add

### The Retrospective

- `retrospective-m3-project-registration.md` —
  the M3 milestone retrospective
  (13 sections, per the Milestone Closeout
  Standard § 4). The structure mirrors the
  M2 retrospective with M3-specific evidence.

### The M4-A Plan

- `.ai/plans/M4-A-infrastructure-process-execution.md` —
  the M4-A plan, expanded from the
  existing summary entry. Sections mirror
  the M3 plan's 12 sections. The plan is
  a **first draft**; the M4-A first
  session reviews and revises the plan
  as needed.

### The M3 Closeout Plan

- `.ai/plans/M3-closeout.md` — this file.
  The M3 closeout plan mirrors the M2.6
  plan's structure; the M3 closeout
  implementation follows the plan.

### The M3 Closeout Implementation Report

- `implementation-report-m3-closeout.md` —
  the M3 closeout's implementation
  report (15+ sections, mirrors the M2.6
  report).

### The M3 Closeout Handoff

- `.ai/handoffs/2026-07-11-m3-closeout.md` —
  the M3 closeout per-session handoff.
  Mirror to
  `.ai/handoffs/latest.md` (overwrites
  the M3.2 handoff mirror).

## 5. Files to Modify

### Project-Continuity State (per Rule 15)

- `.ai/state/session.json` — the M3
  closeout envelope
  (`session_id: m3-closeout-and-retrospective`;
  `session_type: implementation`;
  `previous_session: m3-2-project-registration-slice-2`).
  The in_scope list names the 14 M3
  closeout items. The out_of_scope list
  names the 8 M3 closeout exclusions. The
  current_understanding records M3 closed,
  M4-A in `Awaiting Approval`, T-020
  Done, 273 tests passed, 7 skipped.
- `.ai/state/tasks.json` — T-020
  (`M3 closeout — M3 retrospective`):
  `Ready` → `In Progress` → `Done`
  with the full evidence block. T-021
  (`M4-A.1 — Infrastructure project
  skeleton`): new task, `Ready` in
  `.ai/state/tasks.json`. T-007 (M3
  summary) note: updated to reflect
  M3 closed.
- `.ai/state/current.md` — active
  milestone `M3` → `M3 closed; M4-A
  is the next milestone`; last
  completed task → `T-020`; active
  branch → `main` (after the
  fast-forward merge); last stable
  commit → the M3 closeout commit on
  `main`; active plan status → `M4-A
  plan: Awaiting Approval`; last
  implementation report → the M3
  closeout report; next recommended
  task → `T-021`; last updated →
  2026-07-11; linked artefacts
  updated to reference the M3
  retrospective + the M4-A plan + the
  M3 closeout handoff.
- `.ai/state/task-board.md` — `In
  Progress` block empty (M3 closeout
  is `Done Recently`); M3 closeout
  added to `Done Recently` with the
  full outcome; T-021 in `Ready`;
  the existing M3 summary in
  `Deferred` is archived (the M3
  milestone is closed; the summary
  is no longer in `Deferred`).
- `.ai/state/milestones.json` — M3
  `Active` → `Done` with
  `closed_at: 2026-07-11`; M3
  evidence block updated with the
  M3 closeout's handoff,
  implementation report,
  retrospective, and the M3 closeout
  slice entry; M3 closeout slice
  `planned` → `delivered` with the
  full evidence block; M3 closes
  block records the `m3` tag, the
  M3 closeout commit, the M4-A plan
  path, and the M3 retrospective
  path.

### ROADMAP.md and Master Delivery Plan

- `ROADMAP.md` § 2 (M3 row `Active` →
  `Done`; M3 paragraph updated; the M3
  closeout row in the M3 slice table
  updated from "M3.x — M3 retrospective"
  to `Delivered (M3 closeout, 2026-07-11)`)
  and § 3 (M3 DoD bullets: every DoD
  item checked satisfied; the Open action
  is M4-A's responsibility and remains
  explicitly out of scope; the M3
  retrospective is named; the M4-A plan
  path is named).
- `.ai/plans/master-delivery-plan.md` § 1
  (M3 row `Active` → `Done (closed
  2026-07-11)`; M3 last-stable-evidence
  column updated with the M3 closeout
  commit + the M3 retrospective path) and
  § 3 (M3 completion status `Active` →
  `Done (closed 2026-07-11)`; M3 evidence
  list updated with the M3 closeout's
  handoff, implementation report,
  retrospective; M3 closeout slice row
  `Summary entry` → `Delivered (2026-07-11)`).

## 6. Critical Files to Read Before Editing

- `AGENTS.md` — the 17 non-negotiable rules;
  specifically Rule 13 (no code comments),
  Rule 15 (project-continuity state), and
  Rule 17 (coherent commit).
- `.ai/session-start.md` — the 13-step
  lifecycle.
- `.ai/commands.md` — the command protocol
  that defines `Status`, `Continue`,
  `Approve`, `Resume`, `Finish`, and
  `Next`.
- `.ai/workflows/progressive-coding.md` —
  the Progressive Coding Rule that the
  M3 closeout follows (one task per
  session; 13-step lifecycle; stop
  after the coherent commit).
- `.ai/workflows/milestone-closeout.md` —
  the Milestone Closeout Standard
  (10 sections; the canonical procedure
  every milestone closeout must follow;
  the 13-section retrospective is the
  standard's required deliverable).
- `.ai/workflows/branching-strategy.md` —
  the 12 rules; specifically rule 6
  (fast-forward merge), rule 7 (delete
  the feature branch), rule 9
  (annotated milestone tag).
- `.ai/plans/M3-project-registration.md` —
  the M3 plan; the M3 closeout plan is
  the third M3 slice.
- `.ai/plans/M3.2-project-registration-slice-2.md`
  — the M3.2 plan (Delivered, 2026-07-11).
- `.ai/plans/M2.6-m2-closeout-and-treehouse-dogfooding.md`
  — the M2 closeout plan (the template
  the M3 closeout mirrors).
- `retrospective-m2-application-shell-and-navigation.md`
  — the M2 retrospective (the template
  the M3 retrospective mirrors; the
  13 sections are in the canonical
  order; the M3 retrospective's
  structure is a direct mirror with
  M3-specific evidence).
- `implementation-report-m2-6-m2-closeout.md`
  — the M2 closeout implementation
  report (the template the M3
  closeout's implementation report
  mirrors).
- `.ai/handoffs/2026-07-11-m2-6-m2-closeout.md`
  — the M2 closeout handoff (the
  template the M3 closeout's handoff
  mirrors).
- `implementation-report-m3-1-project-registration-slice-1.md`
  — the M3.1 closeout report (the M3
  retrospective's § 1, § 7, § 10,
  § 11 evidence).
- `implementation-report-m3-2-project-registration-slice-2.md`
  — the M3.2 closeout report (the M3
  retrospective's § 1, § 7, § 10,
  § 11 evidence; the M3.2 deviation 1
  "AppDialog is not introduced" is
  recorded in the M3 retrospective's
  § 3 (Technical debt) verbatim).
- `.ai/handoffs/2026-07-11-m3-1-project-registration-slice-1.md`
  and
  `.ai/handoffs/2026-07-11-m3-2-project-registration-slice-2.md`
  — the M3.1 + M3.2 handoffs.
- `ROADMAP.md` § 2 + § 3 — the M3 row
  + the M3 block.
- `.ai/plans/master-delivery-plan.md`
  § 1 + § 3 — the M3 row + the M3
  block.
- `.ai/state/milestones.json` — the M3
  milestone block; the M3.1 + M3.2
  evidence; the M4-A summary entry.
- `.ai/state/capabilities.json` — the
  C-016 entry (the M3 capability).

## 7. Existing Functions and Utilities to Reuse

- The **M2.6 closeout pattern** is the
  template the M3 closeout follows:
  implementation report + retrospective
  + per-session handoff + state update
  + coherent commit + `m3` annotated
  milestone tag + M4-A plan in
  `Awaiting Approval` + first M4-A task
  in `Ready`. The M2.6 plan
  (`.ai/plans/M2.6-m2-closeout-and-treehouse-dogfooding.md`)
  is the structural template for the
  M3 closeout plan.
- The **M2 retrospective structure** is
  the template the M3 retrospective
  mirrors. The 13 sections are in the
  canonical order; the M2
  retrospective's wording is the basis
  for the M3 retrospective's wording,
  with M3-specific evidence substituted
  in. The M2 retrospective's § 13
  (Recommendations for the Next
  Milestone) is the input the M3 plan
  accounted for; the M3 retrospective's
  § 13 is the input the M4-A plan
  accounts for.
- The **`AddProjects` composition root**
  is the single seam the M4-A plan's
  on-disk `IProjectStore` swaps. The
  swap is a one-line change in
  `AddProjects` (the in-memory
  registration is replaced with the
  on-disk registration); the
  `IProjectService` and the UI are
  unchanged.
- The **M2.2 `INavigationRegistry`**
  is unchanged by the M3 closeout; the
  M3 surface is reachable through the
  registry (per the M3.1 closeout).
- The **M1.2 design system** is
  unchanged by the M3 closeout; the
  M3 surface composes the design
  system (per the M2 retrospective's
  ADR-013 recommendation 4).
- The **Milestone Closeout Standard
  (`.ai/workflows/milestone-closeout.md`)**
  is unchanged; the M3 closeout follows
  the standard as-is.

## 8. Milestone Closeout Standard (Preview)

The Milestone Closeout Standard's § 4
prescribes the 13 retrospective sections;
the M3 closeout produces all 13. The
standard's § 8 prescribes the
next-milestone plan; the M3 closeout
produces the M4-A plan in `Awaiting
Approval` and promotes the first M4-A
task to `Ready`.

The M3 closeout is the **second milestone
closeout** in this repository (the M2.6
closeout was the first). The M3 closeout
follows the standard without modification;
the standard is mature enough to be reused
as-is. The standard's structure:

1. Purpose
2. Definition
3. Closeout gates (build / test / format /
   visual smoke / DoD)
4. Retrospective (13 sections)
5. Project-continuity update
6. Handoff + implementation report
7. Coherent commit / merge / tag
8. Next-milestone plan
9. Push decision
10. Anti-patterns

## 9. Risks and Mitigations

| Risk                                                                                                            | Mitigation                                                                                                                                                  |
| --------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------- |
| The M3 closeout forgets to update a state file (e.g. `milestones.json` not updated to `Done`).                  | The M3 closeout's Implementation Order (§ 10) names every state file; the M3 closeout's validation gate (`grep` checks) verifies the updates landed.       |
| The M3 closeout forgets to `unix2dos` a new file (CRLF rule).                                                   | The M3 closeout runs `unix2dos` on every new file before commit; the `dotnet format --verify-no-changes` gate catches the issue.                            |
| The M3 closeout accidentally modifies source code or test code.                                                  | The M3 closeout's Files NOT Touched (§ below) names the immutable paths; the M3 closeout's validation gate (No scope creep § below) verifies the diff is docs + workflow + state. |
| The `m3` tag is created on the wrong commit.                                                                     | The M3 closeout's Tag gate (§ 11) verifies the tag points to the M3 closeout commit; the M3 closeout's `git show m3 --no-patch` checks the tag's commit hash. |
| The M4-A plan duplicates content from the M3 plan / M2 retrospective.                                            | The M4-A plan is a new file (`.ai/plans/M4-A-infrastructure-process-execution.md`); the plan cites the M3 retrospective and the M3 plan in the Critical Files to Read section. |
| The M3 closeout begins the M4-A implementation.                                                                  | The M3 closeout brief's "Do not begin the following task" rule is preserved; the M3 closeout's Stop Condition (§ 10) names the M3 closeout's end-state.       |

## 10. Coherent Commit + Merge

M3.x is a single coherent commit on a
feature branch per the standard § 7 (Rule
17 in `AGENTS.md`). The commit message is
`chore(m3.closeout): close M3 with retrospective, M4-A plan, and m3 milestone tag`
(per the standard § 7:
`chore(m<milestone>.<slice>): <summary>`).
The commit lives on the feature branch
`feature/T-020-m3-closeout-and-retrospective`;
the branch is fast-forwarded into `main`
per the branching strategy rule 6; the
branch is deleted per rule 7. The `m3`
annotated milestone tag is created at the
M3 closeout commit on `main` per rule 9.

The M3 closeout commit lives on the
branch; the merge is a fast-forward; the
branch is deleted; the tag is created at
the M3 closeout commit on `main`.

## 11. Stop Condition

The M3 closeout session stops after:
1. The M3 retrospective at
   `retrospective-m3-project-registration.md`
   is written (13 sections, per the
   Milestone Closeout Standard § 4).
2. The M4-A plan at
   `.ai/plans/M4-A-infrastructure-process-execution.md`
   is written (12 sections; status:
   `Awaiting Approval`).
3. The M3 closeout plan at
   `.ai/plans/M3-closeout.md` is written
   (this file).
4. The state files are updated
   (`session.json`, `tasks.json`,
   `current.md`, `task-board.md`,
   `milestones.json`).
5. `ROADMAP.md` is updated (M3 row Done;
   M3 DoD bullets checked; M3 closeout
   row Delivered).
6. `.ai/plans/master-delivery-plan.md` is
   updated (M3 row Done; M3 block Done;
   M3 closeout slice row Delivered).
7. The M3 closeout implementation report
   is written at
   `implementation-report-m3-closeout.md`.
8. The M3 closeout per-session handoff is
   written at
   `.ai/handoffs/2026-07-11-m3-closeout.md`
   and mirrored to
   `.ai/handoffs/latest.md`.
9. The feature branch
   `feature/T-020-m3-closeout-and-retrospective`
   is created from `main` at the M3.2
   closeout commit `ff9010a`.
10. The M3 closeout commit is created
    on the feature branch
    (`chore(m3.closeout): close M3 with
    retrospective, M4-A plan, and m3
    milestone tag`).
11. The branch is fast-forwarded into
    `main`.
12. The `m3` annotated milestone tag is
    created at the M3 closeout commit on
    `main` (per the branching strategy
    rule 9).
13. The feature branch is deleted.
14. Push is skipped (not authorised in
    this session).
15. The validation gate passes (six
    gates: `npm run css:build` exit 0;
    `dotnet restore` exit 0; `dotnet
    build` 0 warnings, 0 errors; `dotnet
    test` 273 passed, 0 failed, 7
    skipped; `dotnet format
    --verify-no-changes` exit 0; visual
    smoke on `/projects`).
16. The § 5.5 Next response shape is
    returned to the user.

The M3 closeout session does **not**
begin the M4-A implementation. M4-A
begins in a separate session after the
M3 closeout, per the Progressive Coding
Rule.

## 12. Files NOT Touched

The M3 closeout's immutable paths. A
diff that touches any of these is a
defect.

- `src/AiEng.Platform.App/`,
  `src/AiEng.Platform.Application/`,
  `src/AiEng.Platform.Domain/`,
  `src/AiEng.Platform.Providers.Abstractions/`
  — **not** modified. M3 closeout is a
  docs + workflow + state change; no
  source code.
- `tests/AiEng.Platform.UnitTests/`,
  `tests/AiEng.Platform.ComponentTests/`,
  `tests/AiEng.Platform.ArchitectureTests/`
  — **not** modified. M3 closeout does
  not add or modify any test; the M3
  closeout's test count is identical to
  the M3.2 closeout's (273 + 7 skipped).
- `package.json`, `tailwind.config.js`,
  `Directory.Build.props`,
  `appsettings*.json` — **not** modified.
  The CSS pipeline and the .NET build
  configuration are unchanged.
- `AGENTS.md`, `ARCHITECTURE.md`,
  `DECISIONS.md`, `STYLEGUIDE.md`,
  `CONTRIBUTING.md` — **not** modified.
  M3 closeout is a workflow + state
  change; no constitutional rule is
  added or removed.
- `.ai/workflows/milestone-closeout.md`
  — **not** modified. The standard is
  preserved verbatim (introduced in
  M2.6 closeout; reused as-is by the
  M3 closeout; the standard is the
  single source of truth).
- `.ai/plans/M3-project-registration.md`,
  `.ai/plans/M3.2-project-registration-slice-2.md`
  — **not** modified. The M3.1 + M3.2
  plans remain as they are; the M3
  closeout plan
  (`.ai/plans/M3-closeout.md`) is the
  new plan.
- `.ai/state/project.json`,
  `.ai/state/providers.json`,
  `.ai/state/features.json`,
  `.ai/state/capabilities.json` —
  **not** modified. The project
  identity, providers, features, and
  capabilities are unchanged. The M3
  closeout updates the M3 row in
  `milestones.json` and the C-016
  status in `capabilities.json` is
  preserved (the M3 closeout does not
  add a `completion_status` field; the
  existing `status: Accepted` is
  accurate; the project-intelligence
  refinement task in the task board's
  `Deferred` section is the right place
  for the `completion_status` field;
  the M3 closeout does not pre-empt
  that task).

---

**End of M3 closeout plan.** The plan is
`Approved` (2026-07-11). The M3 closeout
session follows the plan; the M3
retrospective + the M4-A plan + the
project-continuity update + the `m3`
milestone tag are the M3 closeout's
deliverables. M4-A begins in a separate
session after the M3 closeout.
