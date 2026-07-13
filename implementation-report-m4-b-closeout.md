# M4-B Closeout — Implementation Report

> **The M4-B closeout implementation report.**
> The M4-B closeout (`M4-B.x` — T-027) is the
> fourth M4-B slice (M4-B.1 + M4-B.2 + M4-B.3
> + M4-B.x). M4-B.x follows M4-B.3 per the
> Progressive Coding Rule. M4-B.x ships the
> M4-B retrospective per the Milestone Closeout
> Standard § 4, moves M4-B from `Active` to
> `Done` with `closed_at: 2026-07-13`, creates
> the `m4-b` annotated milestone tag at the
> M4-B closeout commit on `main` per the
> branching strategy rule 9, produces the M4-C
> plan in `Awaiting Approval`, and promotes
> the first M4-C task (T-028 — M4-C.1 provider
> registry contract + family registries +
> composition root + unit tests) to `Ready`.
> M4-B closeout is a docs + workflow + state
> change — no source code, no M4-C
> implementation, no provider creation, no
> push.
>
> **Session:** `m4-b-closeout` (2026-07-13).
> **Branch:** `feature/T-027-m4-b-closeout`
> (created from `main` at the M4-B.3 closeout
> commit `ec428cd`; the M4-B closeout commit
> `chore(m4-b.closeout): close M4-B with retrospective, M4-C plan, and m4-b milestone tag`
> is on this branch; the branch is
> fast-forwarded into `main` per the branching
> strategy rule 6; the branch is deleted per
> rule 7).
> **Tag:** `m4-b` (annotated; at the M4-B
> closeout commit on `main`; per rule 9).
> **Plan reference:**
> `.ai/plans/M4-B-closeout.md`.
> **Retrospective:** `retrospective-m4-b-capability-detection.md`
> (13 sections per the Milestone Closeout
> Standard § 4; the third milestone
> retrospective in the repository).
> **Handoff:** `.ai/handoffs/2026-07-13-m4-b-closeout.md`
> (mirrored to `.ai/handoffs/latest.md`).

---

## 1. Plan Reference

- **Plan file:**
  `.ai/plans/M4-B-closeout.md` (12 sections:
  Why This Slice Exists, In Scope, Out of
  Scope, Acceptance Criteria, Files to Add,
  Files to Modify, Critical Files to Read
  Before Editing, Existing
  Functions/Utilities to Reuse, Milestone
  Closeout Standard (Compliance), Risks and
  Mitigations, Coherent Commit + Merge, Stop
  Condition).
- **Status:** Approved (2026-07-13; the brief
  is the approval, no separate plan-review
  step; the brief follows the M3 closeout
  pattern).
- **Session:** `m4-b-closeout`.

## 2. Summary

The M4-B closeout follows the Milestone
Closeout Standard
(`.ai/workflows/milestone-closeout.md`,
introduced in the M2.6 closeout slice on
2026-07-11; reused as-is by the M3 closeout
on 2026-07-11; the canonical procedure every
future milestone closeout must follow). The
M4-B closeout mirrors the M3 closeout's
pattern with M4-B-specific evidence. The M4-B
closeout produces:

- **The M4-B retrospective** at
  `retrospective-m4-b-capability-detection.md`
  (13 sections, per the standard § 4; the
  structure mirrors the M2 + M3 retrospectives
  with M4-B-specific evidence; the third
  milestone retrospective in the repository).
  The 13 sections: delivered capabilities,
  deferred capabilities, technical debt,
  known issues, lessons learned, architecture
  changes, documentation changes, testing
  summary, validation results, implementation
  reports, commit range, readiness for the
  next milestone, recommendations for the
  next milestone.
- **The M4-C plan** at
  `.ai/plans/M4-C-provider-registry-foundation.md`
  (12 sections mirroring the M4-A + M4-B
  plans; Status: Awaiting Approval; the
  first M4-C task T-028 is `Ready` in
  `.ai/state/tasks.json`).
- **The M4-B closeout plan** at
  `.ai/plans/M4-B-closeout.md` (12 sections;
  Status: Approved 2026-07-13 via the brief).
- **The M4-B closeout implementation report**
  (this file).
- **The M4-B closeout per-session handoff** at
  `.ai/handoffs/2026-07-13-m4-b-closeout.md`
  (mirrored to
  `.ai/handoffs/latest.md`).
- **The M4-B closeout's state updates:**
  `session.json`, `tasks.json`, `current.md`,
  `task-board.md`, `milestones.json`,
  `capabilities.json`, `ROADMAP.md`,
  `.ai/plans/master-delivery-plan.md`.
- **The M4-B closeout's branch + tag:**
  the feature branch
  `feature/T-027-m4-b-closeout`; the M4-B
  closeout commit
  `chore(m4-b.closeout): close M4-B with retrospective, M4-C plan, and m4-b milestone tag`;
  the `m4-b` annotated milestone tag.

The M4-B closeout does **not** modify any
source code, test code, build configuration,
or constitutional rule. The M4-B closeout is
a docs + workflow + state change.

## 3. Files Added

- `retrospective-m4-b-capability-detection.md`
  (the M4-B milestone retrospective; 13
  sections; the third milestone retrospective
  in the repository; mirrors the M2 + M3
  retrospectives' structure with M4-B-specific
  evidence).
- `.ai/plans/M4-C-provider-registry-foundation.md`
  (the M4-C plan; 12 sections; Status:
  Awaiting Approval; the first next-milestone
  plan that the Milestone Closeout Standard's
  § 8 procedure produces after the M4-B
  closeout).
- `.ai/plans/M4-B-closeout.md` (the M4-B
  closeout plan; 12 sections; mirrors the M3
  closeout plan's structure; the M4-B closeout
  implementation follows the plan; Status:
  Approved 2026-07-13 via the brief).
- `implementation-report-m4-b-closeout.md`
  (this file).
- `.ai/handoffs/2026-07-13-m4-b-closeout.md`
  (the M4-B closeout per-session handoff;
  mirrors the M3 closeout handoff's structure;
  mirrored to `.ai/handoffs/latest.md`).

## 4. Files Modified

- `.ai/state/session.json` — the M4-B closeout
  envelope (session_id: `m4-b-closeout`;
  session_type: `milestone-closeout`;
  previous_session:
  `m4-b-3-diagnostics-page-startup-log-and-architecture-test`).
- `.ai/state/tasks.json` — T-027 (`M4-B
  closeout — M4-B retrospective`): `Ready`
  → `In Progress` → `Done` with the full
  evidence block. T-028 (`M4-C.1 first
  session — IProviderRegistry contract +
  family registries + composition root +
  unit tests`): new task, `Ready` in
  `.ai/state/tasks.json`.
- `.ai/state/current.md` — active milestone
  `M4-B` → `M4-B closed 2026-07-13; M4-C is
  the next milestone`; last completed
  milestone `M3` → `M4-B`; last completed
  task → `T-027`; active slice `M4-B.3` →
  `M4-B closeout`; last stable commit → the
  M4-B closeout commit on `main`; active plan
  status → `M4-C plan: Awaiting Approval`;
  last implementation report → the M4-B
  closeout report; next recommended task →
  `T-028`; last updated → 2026-07-13; linked
  artefacts updated to reference the M4-B
  retrospective + the M4-C plan + the M4-B
  closeout handoff; Status section updated
  (M4-B.1 + M4-B.2 + M4-B.3 + M4-B closeout
  in Done; M4-C in Awaiting Approval);
  Current Milestone section updated; Last
  Completed Milestone section updated; Last
  Completed Task section updated; Last Stable
  Commit section updated; Last Implementation
  Report section updated (added M4-B.1 +
  M4-B.2 + M4-B.3 + M4-B closeout reports at
  the top); Last Updated section rewritten;
  Active Plan section updated (added M4-B +
  M4-B closeout + M4-C plan entries).
- `.ai/state/task-board.md` — `In Progress`
  block empty (M4-B closeout is `Done
  Recently`); M4-B closeout added to `Done
  Recently` with the full outcome; T-028
  added to `Ready`; T-027 stub removed; the
  M4-B.3 stub in `Ready` removed; the M4-B
  summary in `Deferred` updated from
  `Active` to `Done` with closeout evidence;
  the M4-C summary in `Deferred` updated
  from `Planned` to `Awaiting Approval` with
  the M4-C plan path.
- `.ai/state/milestones.json` — M4-B
  `Active` → `Done` with
  `closed_at: 2026-07-13`; M4-B evidence
  block updated with the M4-B closeout's
  handoff, implementation report,
  retrospective, the `m4-b` tag, and the
  M4-C plan path; M4-B closeout slice
  `Planned` → `Delivered` with the full
  evidence block; M4-B.3 slices
  `Planned` → `Delivered` (the M4-B.1 +
  M4-B.2 + M4-B.3 + M4-B closeout rows in
  the slice table); M4-B closes block
  records the `m4-b` tag, the M4-B closeout
  commit, the M4-C plan path, the M4-B
  retrospective path, the M4-B closeout
  plan path, the M4-B closeout
  implementation report path, the M4-B
  closeout handoff path, the test count
  (376 + 0 failed + 9 skipped), the build
  status (passing; 0 warnings, 0 errors;
  format clean), and the push decision
  (Staged for push, not authorised in this
  session). M4-C `Planned` → `Awaiting
  Approval` with `plan_path`,
  `plan_status`, `plan_promoted_at`,
  `plan_promoted_by_session`,
  `plan_promoted_by_commit`, and `first_task`
  populated.
- `.ai/state/capabilities.json` — C-015
  (`IHostCapabilitiesService`)
  `completion_status` finalised to
  `Delivered (M4-B closed 2026-07-13; C-015
  verified by T-024 + T-025 + T-026 + T-027)`;
  C-015 `next_task` cleared on close (the
  M4-B boundary is closed; the next concrete
  step is T-028, which is the M4-C.1 first
  session, not a C-015 task); C-015 +
  C-023 + C-024 `delivered_by_tasks` add
  T-027; C-015 + C-023 + C-024 evidence
  blocks add the M4-B closeout commit + the
  M4-B closeout implementation report + the
  M4-B retrospective; C-015 evidence `plans`
  adds the M4-C plan path
  (`.ai/plans/M4-C-provider-registry-foundation.md`).
- `ROADMAP.md` § 2 (M4-B row `Active` →
  `Done (closed 2026-07-13; M4-B.1 + M4-B.2
  + M4-B.3 + M4-B closeout Delivered
  2026-07-13)`; M4-B paragraph updated; M4-C
  row `Planned` → `Awaiting Approval` with
  the M4-C plan path) and § 3 (M4-B DoD
  bullets: every DoD item checked satisfied;
  the M4-B closeout status added; the Open
  action is M4-A's responsibility and remains
  explicitly out of scope for M4-B; the M4-B
  retrospective is named; the M4-C plan path
  is named; the `m4-b` annotated milestone
  tag is named). M4-C row description updated
  with the M4-C architecture.
- `.ai/plans/master-delivery-plan.md` § 1
  (M4-B row `Active` → `Done (closed
  2026-07-13)`; M4-B last-stable-evidence
  column updated with the M4-B closeout
  commit + the M4-B closeout handoff + the
  M4-B closeout implementation report + the
  M4-B retrospective + the M4-C plan path)
  and § 3 (M4-B completion status `Active`
  → `Done (closed 2026-07-13)`; M4-B
  evidence list updated with all four M4-B
  slices; M4-B closeout slice row added;
  M4-B next-milestone-enabled → M4-C). M4-C
  `Planned` → `Awaiting Approval` with the
  M4-C plan path.

## 5. Files NOT Touched

The M4-B closeout's immutable paths. A diff
that touches any of these is a defect.

- `src/AiEng.Platform.App/`,
  `src/AiEng.Platform.Application/`,
  `src/AiEng.Platform.Domain/`,
  `src/AiEng.Platform.Infrastructure/`,
  `src/AiEng.Platform.Providers.Abstractions/`
  — **not** modified. M4-B closeout is a
  docs + workflow + state change; no source
  code.
- `tests/AiEng.Platform.UnitTests/`,
  `tests/AiEng.Platform.ComponentTests/`,
  `tests/AiEng.Platform.ArchitectureTests/`
  — **not** modified. M4-B closeout does
  not add or modify any test; the M4-B
  closeout's test count is identical to the
  M4-B.3 closeout's (376 + 0 failed + 9
  skipped).
- `package.json`, `tailwind.config.js`,
  `Directory.Build.props`,
  `appsettings*.json` — **not** modified. The
  CSS pipeline and the .NET build
  configuration are unchanged.
- `AGENTS.md`, `ARCHITECTURE.md`,
  `DECISIONS.md`, `STYLEGUIDE.md`,
  `CONTRIBUTING.md` — **not** modified. M4-B
  closeout is a workflow + state change; no
  constitutional rule is added or removed.
- `.ai/workflows/milestone-closeout.md` —
  **not** modified. The standard is preserved
  verbatim (introduced in M2.6 closeout;
  reused as-is by the M3 closeout; reused
  as-is by the M4-B closeout; the standard
  is the single source of truth).
- `.ai/plans/M4-B-capability-detection.md`
  — **not** modified. The M4-B plan remains
  as it is; the M4-B closeout plan
  (`.ai/plans/M4-B-closeout.md`) is the new
  plan.
- `.ai/state/project.json`,
  `.ai/state/providers.json`,
  `.ai/state/features.json` — **not**
  modified. The project identity, providers,
  and features are unchanged.

## 6. Validation Results

The M4-B closeout's validation gate,
executed end-to-end on 2026-07-13. The same
six gates as the M4-A + M4-B.1 + M4-B.2 +
M4-B.3 closeouts.

| Gate          | Command                                       | Result                                                  |
| ------------- | --------------------------------------------- | ------------------------------------------------------- |
| Restore       | `dotnet restore`                              | Exits 0; every project is up-to-date.                    |
| Build         | `dotnet build`                                | Exits 0; 0 warnings, 0 errors.                           |
| Test          | `dotnet test`                                 | 376 passed (99 unit + 263 component + 14 architecture), 0 failed, 9 skipped (3 axe-core + 4 provider-boundary + 2 process/credential boundary per ADR-016 / M4-D). |
| Format        | `dotnet format --verify-no-changes`           | Exits 0; format is clean (CRLF line endings preserved on every new file). |
| JSON          | The 4 state JSON files (`session.json` + `tasks.json` + `milestones.json` + `capabilities.json`) are valid JSON. |
| CRLF          | Every new + modified file is CRLF (unix2dos applied to the 4 state JSON files + the 4 new docs files + the 4 modified docs files). |

The M4-B DoD walk: every item in
`ROADMAP.md` § 3 M4-B DoD is checked
satisfied in the M4-B closeout status
section (the 9 DoD bullets are listed
with `[x]` markers; the 9 bullets cover
the `IHostCapabilitiesService` + the
`AppCapabilityList` + `AppKeyValueList`
components + the `/diagnostics` page +
the startup capability-report log + the
`Capabilities_Resolved_Through_Service`
architecture test + `docs/capabilities.md`
+ the C-015 evidence block + the "M4-B
does not create any `Providers.<X>`
project" guardrail). The check is by
inspection: every DoD bullet is marked
satisfied in the M4-B retrospective's § 1
(Delivered capabilities) and § 2
(Deferred capabilities).

## 7. Stale-State Check

After the edits, the following commands
succeed:

- `grep -n "M4-B" .ai/state/milestones.json`
  shows the M4-B row as `Done` with
  `closed_at: 2026-07-13`.
- `grep -n "M4-C" .ai/state/milestones.json`
  shows the M4-C row as `Awaiting Approval`
  with the M4-C plan path.
- `grep -n "T-028" .ai/state/tasks.json`
  shows the T-028 row as `Ready` in the M4-C
  section.
- `grep -n "T-027" .ai/state/tasks.json`
  shows the T-027 row as `Done` with the full
  evidence block.
- `grep -n "M4-B closeout" .ai/state/task-board.md`
  shows the M4-B closeout entry in
  `Done Recently`; the `Ready` section shows
  T-028 (the first M4-C task).
- `grep -n "chore(m4-b.closeout)" .ai/handoffs/latest.md`
  returns the M4-B closeout handoff header.
- `git tag --list m4-b` returns `m4-b`
  (annotated).
- `git show m4-b --no-patch` shows the M4-B
  closeout commit hash and the M4-B
  retrospective path in the tag's message.
- `grep -n "M4-B closed 2026-07-13" ROADMAP.md`
  returns the M4-B closeout status.
- `grep -n "M4-B Done (closed 2026-07-13)" .ai/plans/master-delivery-plan.md`
  returns the M4-B closeout evidence row.

## 8. Deviations

**Zero deviations.** The M4-B closeout
follows the Milestone Closeout Standard
as-is (the standard is mature enough to be
reused without modification; the M2.6
closeout's "introduce the standard" is
amortised). The M4-B closeout mirrors the
M3 closeout's structure with M4-B-specific
evidence; the M4-B retrospective mirrors
the M2 + M3 retrospectives' structure with
M4-B-specific evidence.

The M4-B closeout's deviations from the
M3 closeout template are explicitly
recorded in the M4-B retrospective's
§ 0 (introductory note) and § 1 (Delivered
capabilities) and § 6 (Architecture
changes) and § 8 (Testing summary) and
§ 11 (Commit range). The deviations are
*content* deviations (M4-B has different
evidence from M3), not *process*
deviations (the closeout procedure is
identical).

The M4-B closeout's *content* deviations
from the M3 closeout:

- **C-015 + C-023 + C-024** as the
  delivered capabilities (the
  `IHostCapabilitiesService` + the
  `AppCapabilityList` + the
  `AppKeyValueList`) — three capabilities
  versus the M3's single deliverable. The
  M4-B retrospective § 1 records the
  three capabilities with their full
  evidence blocks.
- **376 tests** at M4-B closeout (was 273
  at M3 closeout; +103 M4-B tests
  cumulatively: +20 M4-B.1 unit + 28 M4-B.2
  bUnit + 4 M4-B.3 bUnit + 1 M4-B.3
  architecture + 1 M4-B.3 architecture + 49
  M4-B cumulative; the count includes the
  9 registered-but-disabled tests per
  ADR-016 / M4-D).
- **The `m4-b` annotated milestone tag** is
  the third milestone tag in the
  repository (after `m2` and `m3`); the
  M4-B closeout follows the same pattern
  as the M2 and M3 closeouts.
- **M4-B.3 deviation** (not a M4-B closeout
  deviation; carried over from the M4-B.3
  closeout): the `Diagnostics.razor` page
  renders 12 capability list items (6 host
  tools + 6 provider credentials) instead
  of the 6 anticipated. The 4 bUnit page
  tests assert the 12-item list. The
  architecture test scopes the
  forbidden-token check to the
  `App/Components/Diagnostics/` folder
  (not `App/Components/`) to avoid the
  M4-A.2 Open Action false positive on
  `AppProjectCard.razor`. The deviation is
  recorded in the M4-B retrospective § 1 +
  § 8 + § 11.
- **The M4-B closeout does not push** (the
  brief's "Push only if a remote is
  configured and pushing is authorised" —
  push is not authorised in this session;
  the push decision is **Staged for push**;
  the next user command may push).

## 9. Known Limitations

The M4-B closeout's known limitations are
inherited from M4-B.3 (the M4-B.3 closeout's
deviation: the `/diagnostics` page renders
12 capability list items instead of 6; the
9 registered-but-disabled tests remain per
ADR-016 / M4-D; the visual smoke on
`/diagnostics` is `curl`-based; the M4-B
closeout does not push to the remote). The
M4-B closeout does not introduce new known
limitations; the M4-B closeout inherits the
M4-B known limitations.

The M4-B closeout's specific known
limitations:

- The M4-B closeout did not push to the
  remote. The user did not authorise push
  in this session; the closeout did not
  push. The push decision is **Staged
  for push**; the next user command may
  push.
- The M4-B closeout did not begin the M4-C
  implementation. The M4-B closeout brief's
  "Do not begin the following task" rule
  is preserved; the M4-B closeout stops at
  the M4-B closeout receipt. The M4-C.1
  (T-028) is `Ready` in
  `.ai/state/tasks.json`; the M4-C plan is
  in `Awaiting Approval`; the next session
  approves the M4-C plan and begins the
  M4-C.1 implementation per the Progressive
  Coding Rule.
- The 9 registered-but-disabled tests (3
  axe-core + 4 provider-boundary + 2
  process/credential boundary) remain per
  ADR-016 / M4-D. The M4-B.3
  `Capabilities_Resolved_Through_Service`
  architecture test is `Active`; the
  remaining 9 stay registered-but-disabled
  until M4-D activates them.
- The `lavish-axi` M1 design-system review
  remains `Blocked` (carried over from
  M1). The `AppToolbar` example missing
  on `/design-system` remains
  `Ready` (carried over from M1-FU-1).

## 10. Lessons Learned

The M4-B closeout is the third milestone
closeout in the repository. The lessons are
recorded in the M4-B retrospective's
§ 5 (Lessons Learned). The headline
lessons:

- **The Milestone Closeout Standard
  (introduced in M2.6) is the canonical
  procedure.** The M4-B closeout follows
  the standard as-is — the standard is
  mature enough to be reused without
  modification. The M2 closeout's
  "introduce the standard" is amortised;
  the M3 closeout's "second milestone
  closeout" is amortised; the M4-B closeout
  is the third milestone closeout and
  reuses the same procedure verbatim.
  Every future closeout is cheaper because
  the standard is in place.
- **The 3-slice pattern (contract + UI
  components + page in M4-B; vs the M3
  2-slice pattern of contract + mutations
  in M3.1 + M3.2) is correct.** The M4-B
  plan sized M4-B as 3 implementation
  slices + 1 closeout. The split worked:
  M4-B.1 lands the seam
  (`IHostCapabilitiesService` + the records
  + the implementation + the composition
  root + the unit tests); M4-B.2 lands the
  data-owning four-state design-system
  components; M4-B.3 lands the user-visible
  surface (the `/diagnostics` page + the
  startup log + the architecture test +
  the documentation); M4-B.x lands the
  closeout. M4-C's plan follows the same
  3-slice + closeout pattern (M4-C.1
  boundary + M4-C.2 surface + M4-C.x
  closeout).
- **The `Next` command (end-to-end
  collapsed form of `Continue` +
  `Approve` + the 13-step Progressive
  Coding lifecycle) is the right entry
  point for milestone closeouts.** The
  M4-B closeout session is the implicit
  approval of the M4-B closeout work; the
  M4-B closeout plan is the first step.
  The pattern is sustainable.
- **The CRLF line-endings rule
  (`.editorconfig`) requires every new
  file to be `unix2dos`'d before
  commit.** The M4-B closeout hit this on
  the 4 state JSON files (rewritten by
  the node scripts which used `writeFileSync`
  with LF only) + the 5 new docs files
  (the M4-B retrospective + the M4-B
  closeout plan + the M4-C plan + the
  M4-B closeout implementation report +
  the M4-B closeout handoff). The
  `unix2dos` command is applied explicitly
  to every new + modified file. A future
  task is to add a `pre-commit` hook that
  runs `unix2dos` on the new files.
- **The data-owning four-state pattern
  (Loading / Empty / Error / Populated)
  composes well across milestones.** The
  M4-B.2 `AppCapabilityList` and
  `AppKeyValueList` components use the
  pattern; the M4-C.2 `AppProviderList`
  component will reuse the pattern. The
  pattern is documented in
  `docs/design-system.md` § 5.4; the
  M4-B.3 closeout updated the
  `AppCapabilityList` + `AppKeyValueList`
  rows from `Planned (M4)` to
  `Implemented (M4-B.2)`.
- **The Active architecture test pattern
  (the M4-B.3
  `Capabilities_Resolved_Through_Service`
  test is `Active`, not
  registered-but-disabled) is the right
  pattern for milestone-architectural-rule
  tests.** The M4-B.3 architecture test
  asserts the architectural rule "all
  host capability access flows through the
  `IHostCapabilitiesService` contract,
  never the concrete `IProcessRunner` or
  `ICredentialVault`". The M4-C.2
  `Providers_Resolve_Through_Registry`
  test will reuse the same pattern.
- **The capability report log at startup
  (M4-B.3) is the early signal; the
  `/diagnostics` page (M4-B.3) is the
  user-visible surface; the M4-C provider
  registry consumes the report through DI,
  not through the log.** The M4-B plan
  § 2 item 8 + § 11 documentation plan
  document the boundary. The M4-C plan
  will reuse the pattern (M4-C.2 startup
  provider-report log + `/providers` page
  + M4-C.2 `Providers_Resolve_Through_Registry`
  architecture test).

## 11. Next Recommended Step

The M4-B closeout's next recommended step
is the M4-C.1 implementation
(`T-028 — M4-C.1 first session —
IProviderRegistry contract + family
registries + composition root + unit
tests`).

**M4-C.1 begins in a separate session
after the M4-B closeout.** The M4-B
closeout is the boundary between M4-B and
M4-C; the Progressive Coding Rule applies.
The M4-B closeout brief's "Do not begin
the following task" rule is preserved; the
M4-B closeout does not begin the M4-C
implementation. The M4-C plan is in
`Awaiting Approval`; the M4-C first session
approves the M4-C plan and begins the
M4-C.1 implementation per the Progressive
Coding Rule: one task per session, 13-step
lifecycle, stop after the coherent commit.

The M4-C.1 first session:

1. Reads the M4-B closeout handoff
   (`.ai/handoffs/2026-07-13-m4-b-closeout.md`)
   + the M4-B retrospective
   (`retrospective-m4-b-capability-detection.md`)
   + the M4-B closeout implementation
   report
   (`implementation-report-m4-b-closeout.md`)
   first.
2. Reviews and revises the M4-C plan
   (`.ai/plans/M4-C-provider-registry-foundation.md`)
   as needed. The M4-C plan is a first
   draft; the M4-C first session may
   adjust the scope, the contracts, the
   records, the family registries, the
   implementation, the composition root,
   the fakes, the risk mitigations, and
   the validation gate. The M4-C plan
   accounts for the M4-B retrospective's
   § 13 recommendations.
3. Approves the M4-C plan
   (Status: Approved).
4. Begins the M4-C.1 implementation per
   the M4-C plan. The M4-C.1 slice
   ships: the `IProviderRegistry` contract
   in
   `src/AiEng.Platform.Application/Providers/`;
   the 6 family registry contracts in
   `src/AiEng.Platform.Application/Providers/Families/`;
   the `ProviderDescriptor` +
   `ProviderFamily` + `ProviderStatus`
   records in
   `src/AiEng.Platform.Application/Providers/`;
   the 6 family registry implementations
   in
   `src/AiEng.Platform.Infrastructure/Providers/Families/`;
   the `SystemProviderRegistry`
   implementation in
   `src/AiEng.Platform.Infrastructure/Providers/`
   (composes the 6 family registries +
   consumes `IHostCapabilitiesService`
   through DI to filter eligible providers
   per host capabilities); the 6 family
   fakes; the `AddProviderRegistry`
   composition root extension; the
   wire-up in `AddPlatformServices`; 9+
   unit tests + the 6 family fakes in
   `tests/AiEng.Platform.UnitTests/Providers/`.
   M4-C.1 does **not** ship the
   `AppProviderList` component, the
   `/providers` page, the startup
   provider-report log, the
   `Providers_Resolve_Through_Registry`
   architecture test, or
   `docs/providers.md` — those are in
   M4-C.2.
5. Validates the M4-C.1 slice end-to-end
   (`dotnet restore` exit 0; `dotnet
   build` 0 warnings, 0 errors; `dotnet
   test` 376 + 9+ new = 385+ passed, 0
   failed, 9 skipped; `dotnet format
   --verify-no-changes` exit 0; JSON
   validation; CRLF validation).
6. Writes the M4-C.1 implementation
   report at
   `implementation-report-m4-c-1-provider-registry-contract-and-family-registries.md`.
7. Writes the M4-C.1 per-session handoff
   at
   `.ai/handoffs/2026-07-13-m4-c-1-provider-registry-contract-and-family-registries.md`
   (mirrored to `.ai/handoffs/latest.md`).
8. Promotes M4-C.2 (the
   `AppProviderList` + `/providers` page
   slice) to `Ready` in
   `.ai/state/tasks.json`.
9. Coherent commit on the feature branch
   `feature/T-028-m4-c-1-provider-registry-contract-and-family-registries`:
   `feat(m4-c.1): add IProviderRegistry contract, family registries, SystemProviderRegistry implementation, family fakes, and AddProviderRegistry composition root`.
10. Fast-forward merge the M4-C.1 feature
    branch into `main` per the branching
    strategy rule 6.
11. Delete the M4-C.1 feature branch per
    rule 7.
12. Stop. The next session is the M4-C.2
    implementation (the `AppProviderList` +
    `/providers` page slice).

## 12. Commit Range

The M4-B closeout commit range (the 4
M4-B commits on `main`):

- `c151e90` (M4-B.1 closeout):
  `feat(m4-b.1): add IHostCapabilitiesService contract and SystemHostCapabilitiesService implementation`.
- `b1f0ec8` (M4-B.2 closeout):
  `feat(m4-b.2): add AppCapabilityList + AppKeyValueList data-owning design-system components`.
- `ec428cd` (M4-B.3 closeout):
  `feat(m4-b.3): add /diagnostics page, startup capability log, and Capabilities_Resolved_Through_Service architecture test`.
- The M4-B closeout commit
  `chore(m4-b.closeout): close M4-B with retrospective, M4-C plan, and m4-b milestone tag`
  is the closing receipt for the M4-B
  milestone.

The M4-B closeout's parent commit is
`ec428cd` (the M4-B.3 closeout). The M4-B
closeout's commit hash is the HEAD of
`main` after the fast-forward merge; the
`m4-b` annotated milestone tag is at this
commit on `main` per the branching
strategy rule 9.

## 13. Capability Verification

The M4-B closeout's capability verification
(C-015 + C-023 + C-024):

- **C-015 (`IHostCapabilitiesService`):**
  `completion_status` finalised to
  `Delivered (M4-B closed 2026-07-13; C-015
  verified by T-024 + T-025 + T-026 + T-027)`.
  `delivered_by_tasks` =
  `["T-024", "T-025", "T-026", "T-027"]`.
  `next_task` = `null` (cleared on close).
  `evidence.commits` adds the M4-B closeout
  commit. `evidence.reports` adds the M4-B
  closeout implementation report + the
  M4-B retrospective. `evidence.plans` adds
  the M4-C plan path
  (`.ai/plans/M4-C-provider-registry-foundation.md`).
- **C-023 (`AppCapabilityList`):**
  `completion_status` finalised to
  `Delivered (M4-B closed 2026-07-13; C-023
  verified by T-025 + T-026 + T-027)`.
  `delivered_by_tasks` =
  `["T-025", "T-026", "T-027"]`.
  `evidence.commits` adds the M4-B closeout
  commit. `evidence.reports` adds the M4-B
  retrospective.
- **C-024 (`AppKeyValueList`):**
  `completion_status` finalised to
  `Delivered (M4-B closed 2026-07-13; C-024
  verified by T-025 + T-026 + T-027)`.
  `delivered_by_tasks` =
  `["T-025", "T-026", "T-027"]`.
  `evidence.commits` adds the M4-B closeout
  commit. `evidence.reports` adds the M4-B
  retrospective.

## 14. Architecture Boundary Verification

The M4-B closeout's architecture boundary
verification (the M4-A.1 architecture tests
remain registered-but-disabled per ADR-016 /
M4-D):

- The M4-B closeout does not introduce
  `System.Diagnostics.Process` usage outside
  `src/AiEng.Platform.Infrastructure/`.
- The M4-B closeout does not introduce
  `advapi32.dll` P/Invoke outside
  `src/AiEng.Platform.Infrastructure/`.
- The M4-B closeout does not introduce a
  `Microsoft.Extensions.DependencyInjection`
  `IServiceCollection` extension outside
  `src/AiEng.Platform.App/Composition/`.
- The boundary is enforced by the M4-A.1
  architecture tests
  (`Infrastructure_Respects_ProcessBoundary`
  + `Infrastructure_Respects_CredentialBoundary`
  + `CompositionRootBoundaryTests`); the 6
  registered-but-disabled tests remain per
  ADR-016 / M4-D; M4-D activates them.

The M4-B.3 architecture test
`Capabilities_Resolved_Through_Service` is
`Active` (not registered-but-disabled) and
passes; the test asserts the architectural
rule "all host capability access flows
through the `IHostCapabilitiesService`
contract, never the concrete
`IProcessRunner` or `ICredentialVault`";
the test is scoped to
`App/Components/Diagnostics/` to avoid the
M4-A.2 Open Action false positive on
`AppProjectCard.razor`.

## 15. Linked Artefacts

- The M4-B retrospective:
  [`retrospective-m4-b-capability-detection.md`](./retrospective-m4-b-capability-detection.md)
  (13 sections; the M4-B closeout's
  required deliverable per the Milestone
  Closeout Standard § 4).
- The M4-B closeout plan:
  [`.ai/plans/M4-B-closeout.md`](./.ai/plans/M4-B-closeout.md)
  (12 sections; mirrors the M3 closeout
  plan's structure; Status: Approved
  2026-07-13 via the brief).
- The M4-C plan:
  [`.ai/plans/M4-C-provider-registry-foundation.md`](./.ai/plans/M4-C-provider-registry-foundation.md)
  (12 sections; Status: Awaiting Approval;
  the first next-milestone plan that the
  Milestone Closeout Standard's § 8
  procedure produces after the M4-B
  closeout).
- The M4-B closeout per-session handoff:
  [`.ai/handoffs/2026-07-13-m4-b-closeout.md`](./.ai/handoffs/2026-07-13-m4-b-closeout.md)
  (mirrored to
  [`.ai/handoffs/latest.md`](./.ai/handoffs/latest.md)).
- The M4-B.1 implementation report:
  [`implementation-report-m4-b-1-host-capabilities-contract-and-service.md`](./implementation-report-m4-b-1-host-capabilities-contract-and-service.md)
  (the M4-B.1 closeout's receipt).
- The M4-B.2 implementation report:
  [`implementation-report-m4-b-2-capability-list-components.md`](./implementation-report-m4-b-2-capability-list-components.md)
  (the M4-B.2 closeout's receipt).
- The M4-B.3 implementation report:
  [`implementation-report-m4-b-3-diagnostics-page-startup-log-and-architecture-test.md`](./implementation-report-m4-b-3-diagnostics-page-startup-log-and-architecture-test.md)
  (the M4-B.3 closeout's receipt).
- The M4-B plan (the M4-B implementation
  plan; the M4-B closeout composes the
  M4-B plan):
  [`.ai/plans/M4-B-capability-detection.md`](./.ai/plans/M4-B-capability-detection.md).
- The M3 closeout plan (the template the
  M4-B closeout plan mirrors):
  [`.ai/plans/M3-closeout.md`](./.ai/plans/M3-closeout.md).
- The M3 retrospective (the template the
  M4-B retrospective mirrors):
  [`retrospective-m3-project-registration.md`](./retrospective-m3-project-registration.md).
- The M3 closeout implementation report
  (the template this report mirrors):
  [`implementation-report-m3-closeout.md`](./implementation-report-m3-closeout.md).
- The M3 closeout handoff (the template
  the M4-B closeout handoff mirrors):
  [`.ai/handoffs/2026-07-11-m3-closeout.md`](./.ai/handoffs/2026-07-11-m3-closeout.md).
- The Milestone Closeout Standard (the
  canonical procedure every milestone
  closeout must follow):
  [`.ai/workflows/milestone-closeout.md`](./.ai/workflows/milestone-closeout.md).
- The branching strategy (rules 6, 7, 9
  are the M4-B closeout's branch
  operations):
  [`.ai/workflows/branching-strategy.md`](./.ai/workflows/branching-strategy.md).
- The Progressive Coding Rule (the rule
  the M4-B closeout follows):
  [`.ai/workflows/progressive-coding.md`](./.ai/workflows/progressive-coding.md).
- The M4-B project plan (the M4-B plan;
  the M4-B closeout composes the M4-B
  plan):
  [`.ai/plans/M4-B-capability-detection.md`](./.ai/plans/M4-B-capability-detection.md).
- The M4-B surface documentation:
  [`docs/capabilities.md`](./docs/capabilities.md)
  (10 sections mirroring
  `docs/infrastructure.md` § 1-10; the
  M4-B brief: "the host capability report
  is the only consumer of `IProcessRunner`
  + `ICredentialVault` outside the M4-A.2
  Open Action; the report is consumed by
  the M4-C provider registry through DI,
  not through the startup log").
- The M4-B milestone record (the M4-B
  row is now `Done` with
  `closed_at: 2026-07-13`):
  [`.ai/state/milestones.json`](./.ai/state/milestones.json).
- The M4-B closeout task record (T-027
  is `Done`; T-028 is `Ready`):
  [`.ai/state/tasks.json`](./.ai/state/tasks.json).
- The M4-B closeout session record:
  [`.ai/state/session.json`](./.ai/state/session.json).
- The M4-B closeout's one-page snapshot:
  [`.ai/state/current.md`](./.ai/state/current.md).
- The M4-B closeout's task board (M4-B
  closeout in `Done Recently`; T-028 in
  `Ready`):
  [`.ai/state/task-board.md`](./.ai/state/task-board.md).
- The M4-B milestone plan summary:
  [`.ai/plans/master-delivery-plan.md`](./.ai/plans/master-delivery-plan.md).
- The M4-B milestone plan summary (the
  milestone map):
  [`ROADMAP.md`](./ROADMAP.md).

---

**End of M4-B closeout implementation
report.** The M4-B closeout ships the M4-B
retrospective, the M4-C plan in `Awaiting
Approval`, the project-continuity state
update, the M4-B closeout's branch + tag,
the M4-B closeout per-session handoff, and
this implementation report. M4-B is closed
2026-07-13; the `m4-b` annotated milestone
tag is at the M4-B closeout commit on
`main`. The next session approves the M4-C
plan and begins the M4-C.1 implementation
per the Progressive Coding Rule.
