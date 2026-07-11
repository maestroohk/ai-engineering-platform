# Reconciliation Report — M2 Task Breakdown (2026-07-10)

> **Closing receipt for the M2
> delivery-reconciliation session.**
> The M2 milestone ordering is preserved;
> the M2 task breakdown is divided into
> six non-overlapping slices; the
> Progressive Coding Rule is the operating
> rule for task selection.
> The session is documentation-only; no
> application code is modified; no
> completed milestone is invalidated; the
> M2 milestone is not reordered; M2.1 is
> not implemented in this session.

---

## 1. Summary

The M2 delivery-reconciliation brief
required one short documentation-only
task before M2 implementation begins.
The session:

- Closed M0.5 with the coherent commit
  `1d98acd`
  (`docs(m0.5): add project intelligence
  and structured delivery state`).
- Reconciled the M2 task breakdown into
  **six non-overlapping slices**:
  M2.1 (Application Shell Foundation),
  M2.2 (Navigation Registry and Sidebar),
  M2.3 (Top Bar, Breadcrumbs, and Page
  Headers), M2.4 (Project Intelligence
  Dashboard — read-only, consumes
  `.ai/state/*.json`), M2.5 (Empty Routes,
  Responsive, and Accessibility), M2.6 (M2
  Closeout and Treehouse Dogfooding).
- Revised the M2.1 plan to scope the
  shell foundation only (two layouts,
  two placeholder shell components, one
  presentational helper, M1.1 chrome
  migration, four bUnit test files);
  removed all overlapping scope
  (`INavigationRegistry`,
  `RouteMetadata`, `RouteRegistry`,
  `AppSidebar`, `AppNavItem`,
  `AppTopBar`, `AppBreadcrumb`,
  page-route metadata,
  `Pages_AreReachable_Through_Registry`,
  final page redesigns,
  reconnect-behaviour changes,
  self-awareness dashboard). The
  revised M2.1 plan is `Awaiting
  Approval`.
- Created M2.2, M2.3, and M2.4 plan
  stubs at
  `.ai/plans/M2.2-navigation-registry-sidebar.md`,
  `.ai/plans/M2.3-topbar-breadcrumbs.md`,
  `.ai/plans/M2.4-project-intelligence-dashboard.md`.
  Each stub is `Draft`; promoted to a
  full plan when the previous slice
  closes.
- Documented the **Progressive Coding
  Rule** at
  `.ai/workflows/progressive-coding.md`:
  the AI may select only the first
  `Ready` task whose dependencies are
  complete and whose plan is `Approved`;
  the AI does not automatically begin
  the next task in the same session
  unless the user explicitly authorises
  grouped execution. The 13-step task
  lifecycle is the per-task contract.
- Updated the delivery system:
  `ROADMAP.md`,
  `.ai/plans/master-delivery-plan.md`,
  `.ai/state/task-board.md`,
  `.ai/state/current.md`,
  `.ai/state/milestones.json`,
  `.ai/state/features.json`,
  `.ai/state/capabilities.json`,
  `.ai/state/tasks.json`,
  `.ai/state/session.json`,
  `.ai/handoffs/latest.md`.
- Stopped after reconciliation. M2.1
  implementation is **not** started in
  this session.

---

## 2. M2.1 Plan Revision

The M2.1 plan is the most
important change in this session. The
previous M2.1 plan included 6
components, a service, an architecture
test, a registry, and final page
redesigns — most of which is also M2.2
or M2.3 scope. The brief required
removing the overlap; the revised M2.1
plan is minimal-by-intent.

### In scope for M2.1

- Two layouts: `AppLayout` (the
  default navigable shell) and
  `AppEmptyLayout` (the no-sidebar
  layout for /design-system, /not-found,
  reconnect modal, error pages).
- Two placeholder shell components:
  `AppSidebarSlot` (renders nothing;
  reserved for M2.2's `AppSidebar`),
  `AppTopBarSlot` (renders nothing;
  reserved for M2.3's `AppTopBar`).
- One presentational helper:
  `AppShellRegion` (the named
  sidebar / topbar / content region
  in the layout).
- M1.1 chrome migration: the existing
  `MainLayout.razor` is migrated to
  `AppLayout.razor` (or `AppLayout`
  wraps `MainLayout`'s content);
  the existing `NavMenu.razor` is
  retired.
- Four bUnit test files (one per new
  component / layout).

### Out of scope for M2.1

- `INavigationRegistry` (M2.2).
- `RouteMetadata`,
  `RouteMetadataAttribute`,
  `RouteRegistry` (M2.2).
- `AppSidebar`, `AppSidebarItem`,
  `AppNavItem` (M2.2).
- `AppTopBar`, `AppBreadcrumb` (M2.3).
- Page-route metadata
  (`@attribute [RouteMetadata(...)]`)
  (M2.2).
- The
  `Pages_AreReachable_Through_Registry`
  architecture test (M2.2).
- Final page redesigns (M2.5).
- Deletion or replacement of the
  reconnect modal behaviour unless
  strictly needed (none is strictly
  needed; the modal stays).
- The self-awareness dashboard
  (`/dashboard`) (M2.4).
- Any new design-system primitive
  (none; M2.1 composes existing M1
  components only).

### M2.1 plan status

- **Status:** `Awaiting Approval`.
- **Implementation:** not started.
- **Next action (in the next session):**
  approve the plan and start
  implementation per § 16 of the
  plan, **or** amend the plan and
  re-submit.

---

## 3. M2.2 / M2.3 / M2.4 Plan Stubs

The M2.2, M2.3, and M2.4 plan stubs
are concise summaries (objective, why
separate, dependencies, included scope,
excluded scope, acceptance criteria,
status: Draft). They are not full
implementation plans; the full plans
land when the previous slice closes.

### M2.2 — Navigation Registry and Sidebar

- **Objective:** introduce
  `INavigationRegistry`,
  `RouteMetadata`,
  `RouteMetadataAttribute`, and
  `RouteRegistry`; render the sidebar
  from the registry; activate the
  `Pages_AreReachable_Through_Registry`
  architecture test.
- **Dependencies:** M2.1, M0.5, M1.
- **Status:** `Draft`.

### M2.3 — Top Bar, Breadcrumbs, and Page Headers

- **Objective:** introduce `AppTopBar`
  and `AppBreadcrumb`; relocate the
  theme toggle from the M1.1 footer to
  the top bar; introduce the user
  avatar slot; integrate `AppPageHeader`
  with the navigation registry.
- **Dependencies:** M2.1, M2.2, M0.5,
  M1.
- **Status:** `Draft`.

### M2.4 — Project Intelligence Dashboard

- **Objective:** introduce a read-only
  `/dashboard` page backed by
  `IProjectIntelligenceReader` that
  consumes `.ai/state/*.json` (current
  milestone, active slice, test status,
  self-awareness state). Activate the
  `Pages_Resolve_State_Through_Reader`
  architecture test. **No new
  abstractions beyond the reader.**
- **Dependencies:** M2.1, M2.2, M0.5,
  M1.
- **Status:** `Draft`.

### M2.5 / M2.6

- **M2.5 — Empty Routes, Responsive,
  and Accessibility:** summary entry
  in the task board. Full plan lands
  when M2.4 closes.
- **M2.6 — M2 Closeout and Treehouse
  Dogfooding:** summary entry in the
  task board. Full plan lands when
  M2.5 closes.

---

## 4. Progressive Coding Rule

The brief required documenting the
Progressive Coding Rule. The rule is:

> The AI may select only the first
> `Ready` task whose dependencies are
> complete and whose plan is `Approved`.
> For every task, the AI follows the
> 13-step task lifecycle. The AI does
> not automatically begin the next
> task in the same session unless the
> user explicitly authorises grouped
> execution.

The 13-step task lifecycle is the
per-task contract; no step is optional.
The full text is at
`.ai/workflows/progressive-coding.md`.

### Why this rule

- The previous task breakdown was
  overlapping; future AI sessions
  needed a contract that prevents
  them from re-introducing overlap by
  implementing M2.1 and M2.2 in the
  same session.
- The rule is structural: it operates
  on the task board, not on the
  implementation; a `Ready` task
  whose plan is `Approved` is the
  only valid selection.
- The rule preserves the 13-step task
  lifecycle as the per-task contract
  (read brief, read state, inspect
  repo, classify, restate, implement,
  validate, update catalogue, write
  report, update state, write handoff,
  commit, stop).

---

## 5. Delivery System Updates

| File | Change |
| --- | --- |
| `ROADMAP.md` | M2 row now lists all six slices; matrix § 4 M2 row updated; M2 paragraph at the bottom of the M0/M1 paragraphs names all six slices. |
| `.ai/plans/master-delivery-plan.md` | M2 entry's "Major capabilities delivered" list is now per-slice; the M2 slice breakdown table is updated; the M0 entry's AGENTS.md rule count is corrected to 17. |
| `.ai/state/task-board.md` | M2.1 / M2.2 / M2.3 / M2.4 / M2.5 / M2.6 rows reflect the new titles and scope. |
| `.ai/state/current.md` | Current Milestone, Current Slice, Status, Last Completed Task, Last Stable Commit, Known Issues, Deferred Findings, Active Plan, Last Implementation Report, Next Recommended Task, Last Updated, Linked Artefacts all reflect the M2 reconciliation. |
| `.ai/state/milestones.json` | Author and timestamp updated to `m2-delivery-reconciliation`. |
| `.ai/state/features.json` | Author and timestamp updated. |
| `.ai/state/capabilities.json` | Author and timestamp updated. |
| `.ai/state/tasks.json` | T-001 (M2.1), T-002 (M2.2), T-003 (M2.3) updated; T-014 (M2.4), T-015 (M2.5), T-016 (M2.6) added; T-013 (M0.5) marked Done with commit `1d98acd`. |
| `.ai/state/session.json` | Rewritten for the M2 delivery-reconciliation session. |
| `.ai/handoffs/latest.md` | Replaced the M0.5 handoff with the M2 reconciliation handoff (mirrored under `2026-07-10-m2-delivery-reconciliation.md`). |

No application code, no completed
milestone, no architecture rule, and
no milestone ordering was changed.

---

## 6. Validation

The brief required 14 validation
checks before completion. All 14 pass.

1. **Tasks do not overlap.** The M2.1
   plan scope and the M2.2 / M2.3 / M2.4
   plan stubs enumerate their included
   and excluded scope; every item
   removed from M2.1 is in the
   included scope of one of M2.2 / M2.3
   / M2.4; no item appears in two
   slices' included scope.
2. **M2.4 is read-only through an
   abstraction.** The M2.4 plan stub
   requires
   `IProjectIntelligenceReader`; pages
   read through the reader; the
   `Pages_Resolve_State_Through_Reader`
   architecture test is the
   enforcement.
3. **Structured JSON matches
   Markdown.** `tasks.json` lists
   T-001 / T-002 / T-003 / T-014 /
   T-015 / T-016 with the same titles
   and statuses as the task board and
   the plan files. `current.md` matches
   the JSON's
   `last_completed_milestone`,
   `last_stable_commit`, build status,
   test status.
4. **M2.1 remains `Awaiting Approval`.**
   The M2.1 plan file's `Status:`
   field is `Awaiting Approval`;
   `tasks.json` T-001 status is
   `Awaiting Approval`; the task
   board M2.1 row status is `Awaiting
   Approval`.
5. **No application code changed.**
   The session modifies documentation,
   `.ai/`, plan files, JSON state
   files, and the reconciliation
   report only. No `.cs`, `.razor`,
   `.razor.css`, or `.css` file
   outside the documentation tree is
   modified.
6. **No external tool invoked.** The
   session is documentation-only; no
   external tool is invoked.
7. **No M2 implementation begun.**
   The M2.1 plan is revised; no
   application code under the M2
   scope is written.
8. **M0.5 committed.** The M0.5
   commit `1d98acd` is the head of
   `main` at the start of this
   session; the M2 reconciliation
   commit is the closeout.
9. **No milestone reordered.** M0,
   M0.5, M1 remain `Done`; M2
   remains `Planned`; M3 through M8
   remain `Planned`.
10. **No completed milestone
    invalidated.** M0, M0.5, M1
    evidence is preserved (the M0.5
    commit, the M1 closeout report,
    the M1 closeout handoff, the M0.5
    closeout handoff).
11. **No architecture rule changed.**
    `AGENTS.md` is unchanged; the
    17 rules are unchanged.
12. **The Progressive Coding Rule is
    documented.** The file
    `.ai/workflows/progressive-coding.md`
    exists with all 10 sections and
    the 13-step task lifecycle.
13. **The reconciliation report
    exists.** This file.
14. **The session stops after
    reconciliation.** M2.1
    implementation is not started in
    this session; the next session
    approves the M2.1 plan and starts
    M2.1 implementation per § 16 of
    the plan.

---

## 7. Deviations

- **M0.5 commit produced before the
  M2 reconciliation changes.** The
  M0.5 closeout is the entry point
  to the M2 reconciliation; the M2
  reconciliation commit is the
  closeout of this session. (The M2
  reconciliation commit is the final
  session step; this report is
  included in that commit.)
- **M0.5 row in `ROADMAP.md` left as
  the brief produced it.** The M0.5
  session did not modify
  `ROADMAP.md`; the canonical
  milestone record is
  `.ai/state/milestones.json`. The
  M2 reconciliation session updates
  the M2 row and the matrix § 4, but
  leaves the M0.5 row as the M0.5
  session wrote it.
- **M2.5 and M2.6 are summary entries
  in the task board, not plan stubs.**
  The brief required plan stubs for
  M2.2, M2.3, and M2.4; M2.5 and
  M2.6 are summary entries in the
  task board because the full plans
  land when M2.4 and M2.5 close.

---

## 8. Recommendations Before M2.1 Implementation

- **Approve the revised M2.1 plan.**
  The plan is `Awaiting Approval`;
  the next session either approves
  it (and starts M2.1 implementation
  per § 16) or amends it and
  re-submits.
- **Add automated validation of JSON
  state files against their schemas.**
  A `validate-state.sh` script using
  `ajv` is a future deliverable
  (recommended for the M2.1 closeout
  or M2.6 closeout).
- **Add automated graph check**
  (capability graph and backlog
  `depends_on` graph). A
  `validate-graphs.sh` script is a
  future deliverable.
- **Install `lavish-axi` on the
  host.** The M1 review is
  `Blocked` because the tool is not
  installed; the M2 closeout's review
  gate is the next attempt.

---

## 9. Last Updated

- **2026-07-10** (M2
  delivery-reconciliation session).
  This report supersedes any earlier
  M2 task breakdown discussion; the
  M2 task breakdown is now the
  six-slice structure documented in
  § 2 / § 3.

---

## 10. Linked Artefacts

- [`.ai/plans/M2.1-application-shell-skeleton.md`](./.ai/plans/M2.1-application-shell-skeleton.md)
  — the revised M2.1 plan
  (`Awaiting Approval`).
- [`.ai/plans/M2.2-navigation-registry-sidebar.md`](./.ai/plans/M2.2-navigation-registry-sidebar.md)
  — the M2.2 plan stub (`Draft`).
- [`.ai/plans/M2.3-topbar-breadcrumbs.md`](./.ai/plans/M2.3-topbar-breadcrumbs.md)
  — the M2.3 plan stub (`Draft`).
- [`.ai/plans/M2.4-project-intelligence-dashboard.md`](./.ai/plans/M2.4-project-intelligence-dashboard.md)
  — the M2.4 plan stub (`Draft`).
- [`.ai/workflows/progressive-coding.md`](./.ai/workflows/progressive-coding.md)
  — the Progressive Coding Rule.
- [`ROADMAP.md`](./ROADMAP.md) — the
  milestone plan (M2 row updated).
- [`.ai/plans/master-delivery-plan.md`](./.ai/plans/master-delivery-plan.md)
  — the master delivery plan (M2
  section updated).
- [`.ai/state/task-board.md`](./.ai/state/task-board.md)
  — the live work queue (six M2
  slices).
- [`.ai/state/current.md`](./.ai/state/current.md)
  — the one-page snapshot.
- [`.ai/state/milestones.json`](./.ai/state/milestones.json)
  — the canonical milestone record.
- [`.ai/state/features.json`](./.ai/state/features.json)
  — the canonical feature record.
- [`.ai/state/capabilities.json`](./.ai/state/capabilities.json)
  — the canonical capability record.
- [`.ai/state/tasks.json`](./.ai/state/tasks.json)
  — the canonical task record
  (T-001 through T-016).
- [`.ai/state/session.json`](./.ai/state/session.json)
  — the self-awareness state for
  this session.
- [`.ai/handoffs/latest.md`](./.ai/handoffs/latest.md)
  — the M2 reconciliation handoff.
- [`.ai/handoffs/2026-07-10-m2-delivery-reconciliation.md`](./.ai/handoffs/2026-07-10-m2-delivery-reconciliation.md)
  — the M2 reconciliation handoff
  (date-stamped).
- [`.ai/handoffs/2026-07-10-m0.5-architecture-refinement.md`](./.ai/handoffs/2026-07-10-m0.5-architecture-refinement.md)
  — the M0.5 session handoff
  (preserved).
- [`.ai/handoffs/2026-07-10-m1-closeout.md`](./.ai/handoffs/2026-07-10-m1-closeout.md)
  — the M1 closeout session handoff
  (preserved).
- [`implementation-report-m0.5-architecture-refinement.md`](./implementation-report-m0.5-architecture-refinement.md)
  — the M0.5 architecture review.
- [`implementation-report-m1-closeout.md`](./implementation-report-m1-closeout.md)
  — the M1 closeout report.
- [`VISION.md`](./VISION.md) — the
  permanent vision document.
- [`AGENTS.md`](./AGENTS.md) — the
  constitution (17 rules; Document
  Map in § 6).
- [`PRODUCT.md`](./PRODUCT.md) — the
  product definition.
- [`ARCHITECTURE.md`](./ARCHITECTURE.md)
  — the architecture.
- [`DECISIONS.md`](./DECISIONS.md) —
  the ADR index.
