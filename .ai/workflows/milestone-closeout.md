# .ai/workflows/milestone-closeout.md

> **The canonical Milestone Closeout Standard for the
> AI Engineering Platform.** This document is the
> **single source of truth** for "what a milestone
> closeout is" and the procedures every future
> milestone closeout must follow. If another document
> in this repository contradicts this one, this one
> wins (per the precedence hierarchy in
> `.ai/README.md` § 2). A change to this document is
> recorded in `DECISIONS.md` and announced on the
> next session-start.

---

## 1. Purpose

A milestone is a unit of product delivery in the
platform. The platform's roadmap is divided into
milestones (M0 → M8) and each milestone is divided
into slices (M2.1 → M2.6, etc.). A milestone is
**not done** when the last slice lands; it is done
when the **closeout** verifies the milestone end-to-end
and ships a **retrospective** that downstream
milestones can build on.

The Milestone Closeout Standard exists so that:

1. A milestone is not declared done on a partial
   verification. The closeout is the verification.
2. A milestone's lessons are recorded before the next
   milestone starts. The retrospective is the recording.
3. The platform's quality is observable. The closeout
   is the audit point.
4. The next milestone starts from a clean, evidence-
   backed baseline. The closeout is the baseline.

## 2. Definition

A milestone is **closed** when every gate in § 3 is
green, the retrospective in § 4 is committed, the
project-continuity state in § 5 is updated, the handoff
and implementation report in § 6 are committed, the
coherent commit and milestone tag in § 7 are pushed
(or staged for push), the next-milestone plan in § 8
is in `Awaiting Approval`, and the push decision in
§ 9 is made (or deferred). The closeout ends with
**zero open items** in the milestone's DoD checklist.

## 3. The Closeout Gates

The closeout's validation gate is the same as the
per-slice gate plus a milestone-level gate. Every
item must be green.

### 3.1 The build gate

- `npm run css:build` exits 0. The Tailwind + PostCSS
  pipeline emits `app.css` cleanly.
- `dotnet restore` exits 0. Every project's
  dependencies resolve.
- `dotnet build` exits 0 with **0 warnings, 0 errors**.
  The `TreatWarningsAsErrors` rule in
  `Directory.Build.props` is enforced.

### 3.2 The test gate

- `dotnet test` reports every active test passing.
  The registered-but-disabled tests
  (e.g. `AxeCoreAuditTests`, the four
  `CompositionRootBoundaryTests`) remain disabled per
  ADR-016 and the milestone that activates them; the
  closeout does not activate them out of order.
- The test count is recorded in the implementation
  report. The count is the canonical number for the
  milestone.

### 3.3 The format gate

- `dotnet format --verify-no-changes` exits 0. The
  format is canonical and CI-clean.

### 3.4 The visual smoke gate

- Every route the milestone introduces returns 200
  (or the milestone's documented HTTP success code).
- Every critical interaction the milestone introduces
  works end-to-end. The interactions are the ones
  the milestone's DoD names; the closeout's smoke
  is the canonical evidence.

### 3.5 The DoD gate

- Every item in the milestone's `Definition of done`
  bullet list in `ROADMAP.md` § 3 is checked.
- The check is by inspection, not by assertion. The
  closeout walks the DoD and marks each item
  satisfied or surfaces it as a known issue.

## 4. The Retrospective

A milestone retrospective is a **required deliverable
for every future milestone closeout.** The
retrospective lands at
`retrospective-m<milestone>-<milestone-slug>.md` at the
repository root (e.g.
`retrospective-m2-application-shell-and-navigation.md`).

The retrospective has the following **13 sections**.
The list is the canonical minimum; a milestone may add
sections, but never remove or rename these 13.

1. **Delivered capabilities.** The C-IDs the milestone
   delivers, with the evidence (commit hashes, test
   counts, implementation report paths) that proves
   each one is `Verified`.
2. **Deferred capabilities.** The C-IDs the milestone
   intended to deliver but did not, with the
   rationale (e.g. a follow-up milestone, a
   registered-but-disabled test, an out-of-scope
   decision).
3. **Technical debt.** The work the milestone
   knowingly left for a future milestone. Technical
   debt is tracked, not buried. The retrospective
   names the file, the line, and the debt; the next
   milestone's plan accounts for the debt.
4. **Known issues.** Open bugs, follow-up items,
   and registered-but-disabled tests. The
   retrospective names the issue, the affected area,
   and the milestone that resolves it.
5. **Lessons learned.** What the milestone's sessions
   taught the team. Process lessons (workflow
   changes) and technical lessons (architectural
   decisions) are both recorded. The lessons inform
   the next milestone's plan.
6. **Architecture changes.** The architectural
   decisions the milestone made (or the ADRs it
   accepted). Each change cites the ADR or the
   workflow that approved it.
7. **Documentation changes.** The documents the
   milestone added, modified, or deprecated. The
   retrospective lists every document path and the
   kind of change.
8. **Testing summary.** The test count, the test
   categories, the new tests, the removed tests, the
   registered-but-disabled tests. The summary is the
   canonical test status for the milestone.
9. **Validation results.** The build, test, format,
   and visual-smoke results. The numbers are the
   canonical evidence; the paths are the canonical
   artefacts.
10. **Implementation reports.** The implementation
    report paths the milestone shipped (one per
    slice). The retrospective lists every path.
11. **Commit range.** The Git commit range the
    milestone covers (the first commit after the
    previous milestone's tag to the milestone's
    closeout commit). The range is the canonical
    evidence for "what the milestone touched."
12. **Readiness for the next milestone.** The
    structural and procedural readiness for the
    next milestone (M3 after M2, M4-A after M3, etc.).
    The retrospective names the capabilities the next
    milestone consumes, the dependencies that must
    be satisfied, and the plan stub the next
    milestone's closeout will flesh out.
13. **Recommendations for the next milestone.** A
    concrete list of recommendations the next
    milestone's plan should account for. The
    recommendations are the input to the next
    milestone's plan; the next milestone's first
    session reads this section.

A milestone closeout that does not ship a retrospective
is **not done.** A retrospective that omits a
section is **not done** unless the section is
explicitly marked "not applicable" with a reason.

## 5. The Project-Continuity Update (Rule 15)

The closeout updates the project-continuity state per
Rule 15 in `AGENTS.md`. The updates are the same as
the per-slice updates plus the milestone-level
updates:

- `.ai/state/session.json` — the closeout envelope
  (session_id, scope, current_understanding,
  last_action, intended_next_action, session_notes).
- `.ai/state/tasks.json` — the closeout task
  (e.g. T-016 for M2.6) moves from `In Progress`
  to `Done` with the full evidence block.
- `.ai/state/current.md` — active milestone, last
  completed task, active branch, last stable commit,
  application status, build / test status, active
  plan status, last implementation report, next
  recommended task, last updated, linked artefacts.
- `.ai/state/task-board.md` — the closeout task
  moves from `In Progress` to `Done Recently`; the
  next-milestone plan summary is recorded.
- `.ai/state/milestones.json` — the milestone
  status moves from `Active` to `Done` with
  `closed_at`; the closeout slice moves from
  `in_progress` to `delivered` with the full
  evidence block; the milestone's evidence block
  is updated with the handoff, the implementation
  report, the retrospective, and the slice entry.

## 6. The Handoff + Implementation Report

The closeout ships two artefacts at the repository
root:

- `implementation-report-m<milestone>-<milestone-slug>.md`
  (the closeout's receipt; mirrors the
  per-slice implementation-report template).
- `retrospective-m<milestone>-<milestone-slug>.md`
  (the milestone retrospective per § 4).

The closeout also writes the per-session handoff at
`.ai/handoffs/YYYY-MM-DD-<session-slug>.md` and
mirrors it to `.ai/handoffs/latest.md`. The handoff
follows the existing template
(`.ai/templates/session-handoff.md`).

## 7. The Coherent Commit, Merge, and Milestone Tag

The closeout's work is a single coherent commit per
Rule 17 in `AGENTS.md`. The commit message is
`chore(m<milestone>.<slice>): <one-line summary>`
(e.g.
`chore(m2.6): close M2 with retrospective, milestone closeout standard, and M3 plan`).

The commit lives on the closeout's feature branch
(e.g. `feature/T-016-m2-closeout-and-treehouse-dogfooding`
for M2.6). The branch is then fast-forwarded into
`main` per `.ai/workflows/branching-strategy.md` rule
6, and the branch is deleted per rule 7.

A milestone tag is created at the milestone's closeout
commit on `main` per
`.ai/workflows/branching-strategy.md` rule 9. The tag
name is `m<milestone>` (e.g. `m2`, `m3`). The tag is
annotated and carries the milestone's retrospective
path in its message.

## 8. The Next-Milestone Plan

The closeout prepares the next milestone's plan at
`.ai/plans/M<next-milestone>-<next-slug>.md` with
frontmatter `Status: Awaiting Approval`. The plan
fleshed-out is the one the next session will approve
and execute.

The closeout also promotes the first task of the
next milestone to `Ready` in `.ai/state/tasks.json`.
The task is the first dependency-satisfied task; the
closeout does not invent a new task — the task is
the first one the next milestone's plan names.

## 9. The Push Decision

The closeout's commit and tag may be pushed to the
remote **only when the user has explicitly authorised
the push in the current session.** A push is not
part of the closeout by default. The default is:
**local commit + local tag + local merge**; the user
authorises the push in a follow-up command.

The push decision is recorded in the implementation
report. The decision is one of three:

- **Pushed** (the user authorised; the closeout
  pushed).
- **Staged for push** (the user did not authorise in
  this session; the closeout did not push; the next
  user command may push).
- **No push required** (no remote; the closeout did
  not push; this option is the default for the
  platform's early milestones).

## 10. Anti-Patterns

- A milestone declared done without a retrospective.
- A retrospective that omits a § 4 section.
- A closeout that does not validate the milestone
  end-to-end (the § 3 gates).
- A closeout that leaves a known issue unrecorded.
- A closeout that begins the next milestone's
  implementation (the closeout is a boundary; the
  next session is the next milestone's first
  session).
- A closeout that pushes without authorisation.
- A closeout that names the next milestone's
  "first task" without that task being the first
  dependency-satisfied task in the next milestone's
  plan.
- A closeout that re-states this standard in another
  document. The standard is documented once, in this
  file.
