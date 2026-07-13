# M4-B Closeout — M4-B Retrospective and M4-B → Done

> **The M4-B closeout plan.** This plan promotes the
> M4-B closeout work (the M4-B retrospective per the
> Milestone Closeout Standard § 4, the M4-B → Done
> state transition, the `m4-b` annotated milestone
> tag, the M4-C plan in `Awaiting Approval`, and the
> project-continuity state update) from a summary
> entry to a full closeout slice. M4-B has three
> implementation slices (M4-B.1 + M4-B.2 + M4-B.3) and
> one closeout slice (M4-B.x — this slice). M4-B.x is
> the M2/M3-style wrap-up for the M4-B milestone: the
> verification, the retrospective, the project-
> continuity update, the handoff, the implementation
> report, and the M4-C plan in `Awaiting Approval`.
>
> **Status:** Approved (2026-07-13; user pre-
> authorised in the M4-B closeout brief — the brief
> is the approval, no separate plan-review step).
>
> **Branch:** `feature/T-027-m4-b-closeout`
> (created from `main` at the M4-B.3 closeout commit
> `ec428cd`; per the branching strategy in
> `.ai/workflows/branching-strategy.md`).

---

## 1. Why This Slice Exists

M4-B has three implementation slices (M4-B.1 + M4-B.2
+ M4-B.3) and one closeout slice (M4-B.x). M4-B.x is
the M2/M3-style wrap-up: it verifies the M4-B
implementation against the M4-B plan's definition of
done, records deferred work, introduces the M4-B
retrospective (the fourth milestone retrospective in
this repository, after the M2 + M3 + M4-A
retrospectives), and ships the M4-C plan in `Awaiting
Approval`.

M4-B.x does not add new product functionality. M4-B.x
is the **engineering hygiene** that closes the M4-B
milestone properly: a verified, evidence-backed, and
retrospected M4-B that the next milestone (M4-C — the
provider registry foundation) can build on with
confidence.

The M4-B closeout follows the Milestone Closeout
Standard (introduced in M2.6; the canonical procedure
every future milestone closeout must follow). The
standard is unchanged by the M4-B closeout. The M4-B
closeout mirrors the M3 closeout's structure with
M4-B-specific evidence.

## 2. In Scope

1. **Ship the M4-B retrospective** at
   `retrospective-m4-b-capability-detection.md`
   (13 sections, per the Milestone Closeout Standard
   § 4; the structure mirrors the M2 + M3 + M4-A
   retrospectives with M4-B-specific evidence). The
   13 sections: delivered capabilities, deferred
   capabilities, technical debt, known issues, lessons
   learned, architecture changes, documentation
   changes, testing summary, validation results,
   implementation reports, commit range, readiness for
   M4-C, recommendations for the next milestone.
2. **Produce the M4-C plan** at
   `.ai/plans/M4-C-provider-registry-foundation.md`
   (12 sections, mirrors the M4-B plan's structure;
   the plan is a first draft; the M4-C first session
   reviews and revises the plan as needed). The plan
   is `Awaiting Approval`; the M4-C first session
   approves the plan and begins the M4-C
   implementation.
3. **Update the project-continuity state per Rule 15**
   (the same per-slice state updates, plus the
   milestone-level updates): capability evidence,
   milestone evidence, task state, session state,
   structured JSON, markdown projections,
   `ROADMAP.md`, `.ai/plans/master-delivery-plan.md`,
   `current.md`, `task-board.md`, `latest.md` handoff,
   milestone history, implementation report,
   retrospective.
4. **Move M4-B from `Active` to `Done`** in
   `.ai/state/milestones.json` with
   `closed_at: 2026-07-13` and the M4-B evidence block
   updated with the M4-B closeout's handoff,
   implementation report, retrospective, and the M4-B
   closeout slice entry.
5. **Create the `m4-b` annotated milestone tag** at the
   M4-B closeout commit on `main` per the branching
   strategy rule 9. The tag's message references the
   M4-B retrospective path.
6. **Promote the first M4-C task** (T-028 — the M4-C.1
   provider registry foundation) to `Ready` in
   `.ai/state/tasks.json`. The task is the first
   dependency-satisfied task the M4-C plan names; per
   the Milestone Closeout Standard § 8, the closeout
   promotes the first dependency-satisfied task (the
   closeout does not invent a new task).
7. **Write the M4-B closeout handoff** at
   `.ai/handoffs/2026-07-13-m4-b-closeout.md`
   (mirrored to `.ai/handoffs/latest.md`). The handoff
   follows the existing template
   (`.ai/templates/session-handoff.md`).
8. **Write the M4-B closeout implementation report** at
   `implementation-report-m4-b-closeout.md` (15+
   sections, mirroring the M3 closeout implementation
   report at `implementation-report-m3-closeout.md`).
9. **Update `ROADMAP.md` and
   `.ai/plans/master-delivery-plan.md`** to reflect
   M4-B closed; M4-C `Awaiting Approval`.
10. **Coherent commit on the closeout's feature
    branch** per Rule 17. Commit subject:
    `chore(m4-b.closeout): close M4-B with retrospective, M4-C plan, and m4-b milestone tag`.
    Fast-forward merge into `main` per the branching
    strategy rule 6; delete the feature branch per
    rule 7.
11. **Skip push** (not authorised in this session; the
    push decision is **Staged for push**).

## 3. Out of Scope

- **Provider creation.** Per the M4-B brief: "Do not
  create providers" — M4-B detects capabilities, it
  does not create providers. M4-C's responsibility.
- **M4-C plan promotion.** Not in M4-B's scope. The
  M4-C plan is in `Awaiting Approval`; the M4-C.1
  first session approves the M4-C plan and begins the
  M4-C implementation.
- **M4-D plan promotion.** Not in M4-B's scope.
- **Architecture test activation.** The 4 registered-
  but-disabled `CompositionRootBoundaryTests` remain
  per ADR-016 / M4-D; the activation milestone is
  M4-D.
- **New design-system primitives.** M4-B composes the
  M1.2 primitives + the M4-B.2 components; M4-B
  does not introduce a new design-system primitive.
- **Push to remote.** Push is not authorised in this
  session; the closeout does not push.
- **Beginning the M4-C implementation.** Per the
  brief: "Do not begin the following task" + the
  Progressive Coding Rule. M4-B closeout is a
  boundary; the next session is the M4-C.1 first
  session.

## 4. Acceptance Criteria

The M4-B closeout is **Done** when every item in
this list is checked satisfied.

1. **`retrospective-m4-b-capability-detection.md`**
   exists at the repository root and has 13 sections
   per the Milestone Closeout Standard § 4.
2. **`.ai/plans/M4-C-provider-registry-foundation.md`**
   exists and has `Status: Awaiting Approval` in its
   frontmatter.
3. **`.ai/state/session.json`** has the M4-B closeout
   envelope (`session_id: m4-b-closeout`,
   `session_type: implementation` or
   `milestone-closeout`, `previous_session:
   m4-b-3-diagnostics-page-startup-log-and-architecture-test`).
4. **`.ai/state/tasks.json`** has T-027 `Done` with the
   full evidence block; T-028 (M4-C.1) `Ready`.
5. **`.ai/state/current.md`** has M4-B `Done (closed
   2026-07-13)`; M4-C `Awaiting Approval`; the next
   recommended task is T-028.
6. **`.ai/state/task-board.md`** has the M4-B closeout
   in `Done Recently`; T-028 in `Ready`; the M4-B
   closeout handoff in `Done Recently` block.
7. **`.ai/state/milestones.json`** has M4-B
   `Active` → `Done` with `closed_at: 2026-07-13`; the
   M4-B closeout slice `delivered`; M4-C `Planned` →
   `Awaiting Approval`.
8. **`.ai/state/capabilities.json`** has C-015 +
   C-023 + C-024 evidence blocks finalised with the
   M4-B.1 + M4-B.2 + M4-B.3 commits + reports +
   tests + paths. The C-015 `next_task` is cleared
   on close.
9. **`ROADMAP.md`** has M4-B row `Done`; M4-C row
   `Awaiting Approval`; M4-B DoD bullets checked; M4-B
   closeout status added.
10. **`.ai/plans/master-delivery-plan.md`** has M4-B
    row `Done (closed 2026-07-13)`; M4-C row
    `Awaiting Approval`; M4-B closeout slice row
    added; M4-B evidence list updated; M4-B
    next-milestone-enabled updated to M4-C.
11. **The `m4-b` annotated milestone tag** is created
    at the M4-B closeout commit on `main` per the
    branching strategy rule 9.
12. **`implementation-report-m4-b-closeout.md`** exists
    at the repository root and has 15+ sections.
13. **`.ai/handoffs/2026-07-13-m4-b-closeout.md`**
    exists and is mirrored to `.ai/handoffs/latest.md`.
14. **All new files are CRLF** (`unix2dos` applied to
    every new file).
15. **`dotnet format --verify-no-changes`** exits 0.
16. **`dotnet build`** exits 0; 0 warnings, 0 errors.
17. **`dotnet test`** reports 376 passed, 0 failed, 9
    skipped (per ADR-016 / M4-D) — identical to the
    M4-B.3 closeout's count; M4-B closeout is a
    docs + workflow + state change with no new tests.
18. **No source code is modified.** The M4-B closeout
    is a docs + workflow + state change; no source
    code, no test code, no build configuration, no
    constitutional rule.
19. **The M4-B closeout commit** is
    `chore(m4-b.closeout): close M4-B with retrospective, M4-C plan, and m4-b milestone tag`
    on the feature branch, fast-forwarded into
    `main`; the feature branch is deleted.
20. **Push is not authorised in this session; the
    push decision recorded in the handoff is
    `Staged for push`.**

## 5. Files to Add

- `retrospective-m4-b-capability-detection.md` (the
  M4-B milestone retrospective; 13 sections; the
  fourth milestone retrospective in the repository;
  mirrors the M2 + M3 + M4-A retrospectives'
  structures with M4-B-specific evidence).
- `.ai/plans/M4-C-provider-registry-foundation.md`
  (the M4-C plan; 12 sections; Status: Awaiting
  Approval; the first next-milestone plan that the
  Milestone Closeout Standard's § 8 procedure produces
  after the M4-B closeout).
- `.ai/plans/M4-B-closeout.md` (the M4-B closeout
  plan; this file; 12 sections; mirrors the M3
  closeout plan's structure; the M4-B closeout
  implementation follows this plan).
- `implementation-report-m4-b-closeout.md` (the M4-B
  closeout implementation report; 15+ sections;
  mirrors the M3 closeout implementation report at
  `implementation-report-m3-closeout.md`).
- `.ai/handoffs/2026-07-13-m4-b-closeout.md` (the
  M4-B closeout per-session handoff; mirrors the M3
  closeout handoff's structure at
  `.ai/handoffs/2026-07-11-m3-closeout.md`).

## 6. Files to Modify

- `.ai/state/session.json` — the M4-B closeout
  envelope (session_id: m4-b-closeout;
  previous_session:
  m4-b-3-diagnostics-page-startup-log-and-architecture-test).
- `.ai/state/tasks.json` — T-027 (M4-B closeout):
  `Ready` → `In Progress` → `Done` with the full
  evidence block. T-028 (M4-C.1 — provider registry
  foundation): new task, `Ready` in
  `.ai/state/tasks.json`.
- `.ai/state/current.md` — active milestone
  `M4-B` → `M4-B closed; M4-C is the next
  milestone`; last completed task → `T-027`; active
  branch → `main` (after the fast-forward merge);
  last stable commit → the M4-B closeout commit on
  `main`; active plan status → `M4-C plan: Awaiting
  Approval`; last implementation report → the M4-B
  closeout report; next recommended task → `T-028`;
  last updated → 2026-07-13; linked artefacts
  updated to reference the M4-B retrospective + the
  M4-C plan + the M4-B closeout handoff.
- `.ai/state/task-board.md` — `In Progress` block
  empty (M4-B closeout is `Done Recently`); M4-B
  closeout added to `Done Recently` with the full
  outcome; T-028 in `Ready`; the M4-B summary in
  `Deferred` is archived (the M4-B milestone is
  closed; the summary is no longer in `Deferred`).
- `.ai/state/milestones.json` — M4-B
  `Active` → `Done` with `closed_at: 2026-07-13`;
  M4-B evidence block updated with the M4-B
  closeout's handoff, implementation report,
  retrospective, and the M4-B closeout slice entry;
  M4-B closeout slice `planned` → `delivered` with
  the full evidence block; M4-B closes block
  records the `m4-b` tag, the M4-B closeout
  commit, the M4-C plan path, and the M4-B
  retrospective path. M4-C `Planned` → `Awaiting
  Approval`.
- `.ai/state/capabilities.json` — C-015 +
  C-023 + C-024 evidence blocks finalised with the
  M4-B.1 + M4-B.2 + M4-B.3 commits + reports +
  tests + paths. C-015 `next_task` cleared on close
  (the M4-B is done; M4-C is the next milestone).
- `ROADMAP.md` — M4-B row `Active` → `Done`; M4-B
  paragraph updated; the M4-B closeout row in the
  M4-B slice table updated from "M4-B.x — M4-B
  retrospective" to `Delivered (M4-B closeout,
  2026-07-13)`) and § 3 (M4-B DoD bullets: every
  DoD item checked satisfied; the M4-B closeout
  status added; the capability-report consumers
  are M4-C's responsibility and remain explicitly
  out of scope; the M4-B retrospective is named;
  the M4-C plan path is named). M4-C row `Planned`
  → `Awaiting Approval (M4-C plan produced by the
  M4-B closeout, 2026-07-13)`.
- `.ai/plans/master-delivery-plan.md` — M4-B row
  `Active` → `Done (closed 2026-07-13; M4-B.1 +
  M4-B.2 + M4-B.3 + M4-B closeout Delivered
  2026-07-13)`; M4-B last-stable-evidence column
  updated with the M4-B closeout commit + the M4-B
  retrospective path) and § 3 (M4-B completion
  status `Active` → `Done (closed 2026-07-13)`;
  M4-B evidence list updated; M4-B closeout slice
  row added to the slice table; M4-B
  next-milestone-enabled M4-C). M4-C `Planned` →
  `Awaiting Approval (M4-C plan produced by the
  M4-B closeout, 2026-07-13)`.
- `.ai/handoffs/latest.md` — mirror the M4-B
  closeout handoff to `.ai/handoffs/latest.md`.

## 7. Critical Files to Read Before Editing

- `AGENTS.md` — the 17 non-negotiable rules;
  specifically Rule 13 (no code comments), Rule 15
  (project-continuity state), Rule 16 (scope
  discipline), Rule 17 (evidence of completion).
- `.ai/workflows/milestone-closeout.md` — the
  canonical Milestone Closeout Standard.
- `.ai/workflows/branching-strategy.md` — the 12
  rules; specifically rules 6, 7, 9.
- `.ai/workflows/progressive-coding.md` — the
  Progressive Coding Rule.
- `.ai/plans/M4-B-capability-detection.md` — the
  M4-B plan (canonical M4-B scope; the M4-B
  closeout validates against the M4-B DoD).
- `.ai/handoffs/2026-07-13-m4-b-3-diagnostics-page-startup-log-and-architecture-test.md`
  — the M4-B.3 handoff (the most recent M4-B
  session handoff; the M4-B closeout aggregates
  the M4-B.1 + M4-B.2 + M4-B.3 evidence blocks).
- `implementation-report-m4-b-1-host-capabilities-contract-and-service.md`
  — the M4-B.1 implementation report.
- `implementation-report-m4-b-2-capability-list-components.md`
  — the M4-B.2 implementation report.
- `implementation-report-m4-b-3-diagnostics-page-startup-log-and-architecture-test.md`
  — the M4-B.3 implementation report.
- `retrospective-m3-project-registration.md` — the
  M3 retrospective (the M4-B retrospective mirrors
  the M3 retrospective's 13 sections with M4-B-
  specific evidence).
- `retrospective-m2-application-shell-and-navigation.md`
  — the M2 retrospective (the first milestone
  retrospective; the M3 + M4-B retrospectives
  follow the M2 retrospective's structure).
- `implementation-report-m3-closeout.md` — the M3
  closeout implementation report (the M4-B closeout
  implementation report mirrors the M3 closeout
  implementation report's structure).
- `.ai/handoffs/2026-07-11-m3-closeout.md` — the
  M3 closeout handoff (the M4-B closeout handoff
  mirrors the M3 closeout handoff's structure).
- `.ai/plans/M3-closeout.md` — the M3 closeout plan
  (the M4-B closeout plan mirrors the M3 closeout
  plan's 12 sections).
- `.ai/plans/M4-A-infrastructure-process-execution.md`
  — the M4-A plan (the M4-C plan mirrors the M4-A
  plan's 12 sections with M4-C-specific evidence).
- `ROADMAP.md` — the milestone map; the M4-B
  closeout updates the M4-B + M4-C rows.
- `.ai/plans/master-delivery-plan.md` — the
  delivery view; the M4-B closeout updates the
  M4-B + M4-C rows.
- `.ai/state/milestones.json` — the milestone
  state; the M4-B closeout transitions M4-B to
  `Done` and M4-C to `Awaiting Approval`.
- `.ai/state/capabilities.json` — the capability
  state; the M4-B closeout finalises C-015 +
  C-023 + C-024.
- `.editorconfig` — CRLF + 4-space indent for
  `.cs`/`.razor`/`.json`/`.md` (use `unix2dos` on
  every new file).

## 8. Existing Functions and Utilities to Reuse

- The **Milestone Closeout Standard** at
  `.ai/workflows/milestone-closeout.md` is the
  canonical procedure. The M4-B closeout follows the
  standard as-is — the standard is mature enough to
  be reused without modification (the M2.6 closeout's
  "introduce the standard" is amortised).
- The **M3 closeout plan** at
  `.ai/plans/M3-closeout.md` is the template the
  M4-B closeout plan mirrors (12 sections).
- The **M3 retrospective** at
  `retrospective-m3-project-registration.md` is the
  template the M4-B retrospective mirrors (13
  sections).
- The **M3 closeout implementation report** at
  `implementation-report-m3-closeout.md` is the
  template the M4-B closeout implementation report
  mirrors (15+ sections).
- The **M3 closeout handoff** at
  `.ai/handoffs/2026-07-11-m3-closeout.md` is the
  template the M4-B closeout handoff mirrors.
- The **branching strategy** at
  `.ai/workflows/branching-strategy.md` is the
  ruleset the M4-B closeout's branch operations
  follow (rules 4, 6, 7, 9).

## 9. Milestone Closeout Standard (Compliance)

The M4-B closeout is the **fourth** milestone
closeout in the repository. The M4-B closeout
follows the Milestone Closeout Standard at
`.ai/workflows/milestone-closeout.md` as-is:

- **§ 3.1 Build gate:** `dotnet build` exits 0; 0
  warnings, 0 errors. **M4-B.3 closeout's build
  state is 0 warnings, 0 errors; the M4-B closeout
  does not modify any source code; the M4-B
  closeout's build state is identical to the
  M4-B.3 closeout's.**
- **§ 3.2 Test gate:** `dotnet test` reports 376
  passed, 0 failed, 9 skipped (per ADR-016 / M4-D)
  — identical to the M4-B.3 closeout's count.
- **§ 3.3 Format gate:** `dotnet format
  --verify-no-changes` exits 0.
- **§ 3.4 Visual smoke gate:** no new routes are
  introduced by the M4-B closeout; the M4-B
  closeout is a docs + workflow + state change.
  The M4-B.3 closeout's visual smoke (the
  `/diagnostics` page) is unchanged.
- **§ 3.5 DoD gate:** the M4-B plan § 10 (Test
  Plan) + § 11 (Documentation Plan) is walked; every
  DoD bullet is checked. The check is by inspection.
- **§ 4 Retrospective:** the M4-B retrospective is
  shipped with 13 sections per the standard.
- **§ 5 Project-continuity update (Rule 15):** the
  same per-slice state updates + the milestone-level
  updates.
- **§ 6 Handoff + implementation report:** the M4-B
  closeout handoff is shipped; the M4-B closeout
  implementation report is shipped.
- **§ 7 Coherent commit + merge + milestone tag:**
  the M4-B closeout commit is on the feature branch;
  the branch is fast-forwarded into `main`; the
  `m4-b` annotated milestone tag is at the M4-B
  closeout commit on `main`; the feature branch is
  deleted.
- **§ 8 Next-milestone plan:** the M4-C plan is in
  `Awaiting Approval`; T-028 (M4-C.1) is the first
  `Ready` task.
- **§ 9 Push decision:** the M4-B closeout does not
  push; the push decision is **Staged for push**.

## 10. Risks and Mitigations

| Risk                                                                              | Mitigation                                                                                            |
| --------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------- |
| The M4-B closeout may begin the M4-C implementation (the M4-C.1 first session's responsibility). | The M4-B closeout brief's "Do not begin the following task" rule is preserved; the M4-B closeout stops at the M4-B closeout receipt. The M4-C plan is in `Awaiting Approval`; the next session approves the M4-C plan and begins the M4-C.1 implementation per the Progressive Coding Rule. |
| The M4-B closeout may forget to update one of the state files.                    | The Acceptance Criteria § 4 lists every state file + every required change; the M4-B closeout implementation walks the list and marks each item satisfied. |
| The M4-B closeout may fail to finalise the C-015 + C-023 + C-024 evidence blocks in `.ai/state/capabilities.json`. | The M4-B.1 + M4-B.2 + M4-B.3 closeout commits + reports + tests + paths are already in the M4-B.3 closeout's `capabilities.json`; the M4-B closeout verifies the finalised state and clears the C-015 `next_task`. |
| The M4-B closeout may fail to create the `m4-b` annotated milestone tag.          | The Acceptance Criteria § 11 lists the tag as a required deliverable; the M4-B closeout implementation creates the tag and verifies it with `git show m4-b --no-patch`. |
| The M4-B closeout may push to the remote without authorisation.                   | The Acceptance Criteria § 20 explicitly records the push decision as **Staged for push**; the M4-B closeout does not push. |
| The M4-B retrospective may omit a § 4 section.                                    | The M4-B retrospective template is the M3 retrospective (13 sections, all present); the M4-B retrospective mirrors the template and includes all 13 sections. |
| The M4-C plan may overlap with the M4-A plan or the M4-B plan.                    | The M4-C plan is a new plan at `.ai/plans/M4-C-provider-registry-foundation.md`; the M4-C plan does not modify the M4-A + M4-B plans. The M4-C plan is a first draft; the M4-C.1 first session reviews and revises the plan as needed. |
| The M4-B closeout may modify source code (out of scope).                          | The M4-B closeout is a docs + workflow + state change; no source code, no test code, no build configuration, no constitutional rule. The Acceptance Criteria § 18 explicitly records this. |

## 11. Coherent Commit + Merge

The M4-B closeout's work is a single coherent commit
per Rule 17 + the branching strategy. The commit
message is
`chore(m4-b.closeout): close M4-B with retrospective, M4-C plan, and m4-b milestone tag`.
The commit lives on the closeout's feature branch
`feature/T-027-m4-b-closeout` (created from `main`
at the M4-B.3 closeout commit `ec428cd`). The branch
is then fast-forwarded into `main` per the branching
strategy rule 6, and the branch is deleted per rule
7.

A milestone tag is created at the M4-B closeout
commit on `main` per the branching strategy rule 9.
The tag name is `m4-b`. The tag is annotated and
carries the M4-B retrospective path in its message:

```
git tag -a m4-b -m "M4-B closeout: capability detection. See retrospective-m4-b-capability-detection.md"
```

A tag is a permanent reference to a milestone's
stable state. A tag is never moved or deleted.

## 12. Stop Condition

The M4-B closeout stops after the coherent commit is
on `main` and the `m4-b` annotated milestone tag is at
the M4-B closeout commit. The M4-B closeout does
**not** begin the M4-C implementation. The M4-B
closeout is a boundary; the next session is the
M4-C.1 first session on the user's `Approve` or
`Next` invocation.

The M4-B closeout does **not** push to the remote
(push is not authorised in this session). The push
decision recorded in the handoff is **Staged for
push**; the next user command may push.

The M4-B closeout ends with **zero open items** in
the Acceptance Criteria § 4 checklist.

---

**End of M4-B closeout plan.** The M4-B closeout
follows the Milestone Closeout Standard as-is; the
M4-B closeout is a docs + workflow + state change
with no new product functionality. The next session
approves the M4-C plan and begins the M4-C.1
implementation per the Progressive Coding Rule.
