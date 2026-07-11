# Task Board

> **Live work queue for the AI Engineering Platform.**
> Updated at the end of every AI session that changes
> project state. The most recent update wins. The
> state files reflect the actual state of the
> repository; the repository wins when the two
> disagree (see `.ai/session-start.md` step 6 —
> state reconciliation).
>
> **State architecture (M0.5).** This file is the
> human-readable projection. The canonical machine-readable
> work queue is in
> [`.ai/state/tasks.json`](./tasks.json). The
> capability graph is in
> [`.ai/state/capabilities.json`](./capabilities.json);
> the milestone list is in
> [`.ai/state/milestones.json`](./milestones.json). The
> two layers are kept in sync by every session that
> changes the work queue.
>
> **Status codes:**
>
> - **Ready** — task is defined, no blocker, no owner.
> - **In Progress** — a session is actively working
>   the task.
> - **Blocked** — task cannot proceed; the row names
>   the blocker.
> - **Done Recently** — completed, merged, and
>   committed. Recent items live here; older items
>   archive to `.ai/handoffs/`.
> - **Deferred** — task is intentionally out of
>   scope for the current plan; it lives here so it
>   is not forgotten.

---

## Ready

### M4-A.1 — Infrastructure project skeleton (IProcessRunner, ICredentialVault, IPlatformInfo, on-disk IProjectStore)

- **Task ID:** `T-021`
- **Milestone:** M4-A — Infrastructure /
  Process Execution
- **Title:** M4-A.1 — the first M4-A
  implementation slice; introduces the
  `AiEng.Platform.Infrastructure` csproj,
  the `IProcessRunner` /
  `ICredentialVault` / `IPlatformInfo`
  contracts in
  `Application/Infrastructure/`, the
  `SystemProcessRunner` /
  `WindowsCredentialVault` /
  `SystemPlatformInfo` /
  `JsonFileProjectStore` implementations,
  the `AddInfrastructure` composition root
  extension, and the one-line swap in
  `AddProjects` (the M3 in-memory
  `InMemoryProjectStore` registration is
  removed; the on-disk
  `JsonFileProjectStore` is now
  registered through `AddInfrastructure`).
- **Why it matters:** M4-A is the
  first milestone that introduces a
  process boundary. The boundary is
  designed to be testable and to keep
  the UI (Blazor Server) free of
  process-boundary types. The on-disk
  `IProjectStore` is the **first durable
  surface** in this repository.
- **Objective:** Add the
  `AiEng.Platform.Infrastructure` csproj;
  add the four contracts in
  `Application/Infrastructure/`;
  add the four implementations in
  `Infrastructure/`; add the
  `AddInfrastructure` composition root
  extension; swap the in-memory
  `IProjectStore` registration in
  `AddProjects`; add the unit + bUnit
  tests (50+ tests); preserve the
  `InMemoryProjectStore` as a test
  fixture in
  `tests/AiEng.Platform.UnitTests/`.
- **Acceptance criteria:** see
  `.ai/plans/M4-A-infrastructure-process-execution.md`
  § 2 (the M4-A plan). M4-A.1 does **not**
  enable the Open action on
  `AppProjectCard` (that is the M4-A.2
  slice). M4-A.1 is the boundary, not the
  activation.
- **Dependencies:** M3 (Done 2026-07-11);
  M2 (Done 2026-07-11); M1.2 (Done). No
  new dependencies. The M3 in-memory
  `IProjectStore` is the smoke test for
  the `IProjectStore` contract; M4-A.1
  replaces it on disk behind the same
  contract.
- **Expected affected areas:**
  `src/AiEng.Platform.Infrastructure/`
  (new csproj + contracts implementations);
  `src/AiEng.Platform.App/Composition/`
  (the `AddInfrastructure` extension; the
  `AddProjects` swap); the
  `src/AiEng.Platform.Application/` project
  reference updates; the unit + bUnit
  tests; the `AppProjectCard.razor`
  test extension (the Open action
  remains disabled in M4-A.1; the
  bUnit test extension is in M4-A.2).
- **Validation:** the per-slice
  validation gate — `npm run css:build`
  (exit 0); `dotnet restore` (exit 0);
  `dotnet build` (0 warnings, 0 errors);
  `dotnet test` (273+M4-A.1 tests passed,
  0 failed, 7 skipped); `dotnet format
  --verify-no-changes` (exit 0); visual
  smoke on `/projects` (200; the Open
  action is still disabled in M4-A.1; the
  M4-A.1 visual smoke asserts the page
  loads 200 and the project list persists
  across an application restart).
- **Approved plan path:**
  `.ai/plans/M4-A-infrastructure-process-execution.md`
  (Awaiting Approval; produced by the M3
  closeout per the Milestone Closeout
  Standard § 8).
- **Status:** Ready (the M3 closeout
  promoted T-021 to Ready; the next
  session approves the M4-A plan and
  starts the M4-A.1 implementation per
  the Progressive Coding Rule).

### M1 follow-up — Add `AppToolbar` example to `/design-system`

- **Task ID:** `M1-FU-1`
- **Milestone:** M1 — Design System Core
  (closed; this is a follow-up)
- **Title:** Add `AppToolbar` example to
  `/design-system`
- **Why it matters:** The `/design-system`
  page is the design-system catalogue's
  rendering; missing component examples hide
  shipped components from reviewers.
  `AppToolbar` ships and is unit-tested but
  is not exercised on the doc page (18/19
  component CSS classes appear in the
  rendered HTML).
- **Objective:** Add an `AppToolbar` example to
  `src/AiEng.Platform.App/Components/Pages/DesignSystem.razor`
  in a new "Toolbar" section; rebuild CSS;
  verify the `app-toolbar` class appears in the
  rendered output.
- **Acceptance criteria:** `app-toolbar`
  appears in the `/design-system` HTML; the
  example matches the level of detail of the
  existing section examples; all M1 validation
  remains green.
- **Dependencies:** M1 (closed). No new
  dependencies.
- **Expected affected areas:**
  `src/AiEng.Platform.App/Components/Pages/DesignSystem.razor`,
  possibly
  `src/AiEng.Platform.App/wwwroot/css/app.css`
  (rebuilt by `npm run css:build`).
- **Validation:** `npm run css:build`; visual
  smoke test on `/design-system`; the M1
  bUnit tests for `AppToolbar` remain green.
- **Approved plan path:** (cosmetic; no
  detailed plan required; can be folded into
  M2.6 if appropriate).
- **Status:** Ready (cosmetic; can be picked
  up at any time).

---

## In Progress

(none — M3 closeout delivered in the
m3-closeout-and-retrospective session,
2026-07-11; M3 is Done (closed
2026-07-11); the M4-A plan is in
`Awaiting Approval`; T-021 (M4-A.1
infrastructure project skeleton) is the
next `Ready` task; the next session
approves the M4-A plan and begins the
M4-A.1 implementation per the
Progressive Coding Rule.)

---

## Blocked

### Run M1 design-system `lavish-axi` review (deferred from M1 closeout)

- **Task ID:** `M1-REV-1`
- **Milestone:** M1 — Design System Core
- **Title:** Run `lavish-axi` design-system
  review of the M1 deliverable
- **Why it matters:** The M1 dogfooding
  checkpoint in `ROADMAP.md` § 3 authorises
  the development team to use `lavish-axi`
  externally to review the M1 deliverable.
  The review's findings inform the M2 design
  decisions.
- **Blocker:** `lavish-axi` is not installed on
  the host. The only artefact on the filesystem
  is `agent-workbench/tools/lavish-axi.md`, a
  spec document for an event-bus daemon, not a
  review tool. No review command is documented.
  Per `.ai/workflows/tool-dogfooding.md`, the
  no-silent-fallback rule applies.
- **Unblock path:** (a) install `lavish-axi`
  on the host; (b) the user picks a substitute
  review tool; (c) the user decides the
  `lavish-axi` dogfooding is not the right
  step and removes PART 2 from the brief.
- **Expected affected areas:**
  `.ai/reviews/` (the review record, when
  produced).
- **Validation:** the review report is
  produced; findings are filed with severity
  labels.
- **Approved plan path:** (none — a review
  record is the deliverable, not a plan).
- **Status:** Blocked.

---

## Done Recently

### M3.2 closeout session — 2026-07-11

- **Task ID:** `T-019`
- **Milestone:** M3 — Project Registration
  (Active 2026-07-11)
- **Title:** Project registration slice 2
  (the registration form, the rename
  form, the unregister confirmation)
- **Status:** **Done (delivered 2026-07-11).**
  M3.2 is the second M3 implementation
  slice. M3 is **Active**; M3.x (the M3
  closeout) is the next `Ready` task.
- **Outcome:** M3.2 ships the three
  mutations the M3 surface exists for
  end-to-end: (1) **The registration
  modal** (`RegisterProjectForm.razor`;
  HTML5 native `<dialog open>`; name
  + folder path fields; validates
  non-empty name + non-empty path;
  resolves `IProjectService` through
  the composition root; calls
  `RegisterAsync`; emits
  `OnRegistered` on success); (2) **The
  rename modal** (`RenameProjectForm.razor`;
  pre-fills the project's current
  name; validates the new name is
  non-empty + differs from the current
  name; calls `RenameAsync`; emits
  `OnRenamed` on success); (3) **The
  unregister confirmation**
  (`ConfirmUnregisterProject.razor`;
  shows the project name; calls
  `UnregisterAsync`; emits
  `OnUnregistered` on success); (4) **The
  `AppProjectCard` Rename + Unregister
  buttons** are enabled; the Open
  button remains disabled (M4-A's
  responsibility); (5) **The page
  header's Register a project
  button** is enabled; clicking it
  opens the registration modal; (6)
  **The `AppProjectList` exposes
  `ShowRegisterDialog()`** (the page
  header delegates to it) and
  `RefreshAsync()` (the form
  components invoke it on success);
  (7) **The architecture test
  `Pages_Resolve_Projects_Through_Service`**
  is extended with three new tests
  covering the three new form
  components; the single-seam rule
  holds for every form; (8) **The
  surface documentation** at
  `docs/projects.md` documents the
  enabled M3.2 actions. 273 total
  tests pass (34 unit + 228 bUnit +
  11 architecture); 7 skipped per
  ADR-016 / M4-D; 0 warnings, 0
  errors; format clean; visual smoke
  (`curl http://localhost:5286/projects`
  returns 200; the Register a project
  button is enabled; clicking it
  opens the registration modal;
  registering / renaming /
  unregistering a project works
  end-to-end). Three documented
  deviations: (1) `AppDialog` is
  not introduced; HTML5 native
  `<dialog>` is used directly
  (minimum-blast-radius; the M1.2
  design system does not ship a
  dialog primitive; the design
  system is not extended); (2) M3.2
  unit tests are reused from M3.1
  (the M3.1 `IProjectServiceTests`
  already cover the `RegisterAsync`
  / `RenameAsync` / `UnregisterAsync`
  happy-path + failure paths); (3)
  Disabled tests are unchanged (the
  7 registered-but-disabled tests
  remain skipped per ADR-016 / M4-D).
- **Report:**
  `implementation-report-m3-2-project-registration-slice-2.md`.
- **Handoff:**
  `.ai/handoffs/2026-07-11-m3-2-project-registration-slice-2.md`
  (mirrored at
  `.ai/handoffs/latest.md`).
- **Git:** branch
  `feature/T-019-m3-2-project-registration-slice-2`
  (created from `main` at the M3.1
  closeout commit; the M3.2 closeout
  commit
  `feat(m3.2): enable project registration form, rename, and unregister`
  is on this branch; the branch is
  fast-forwarded into `main` per the
  branching strategy rule 6; the
  branch is deleted per rule 7). No
  remote push (push is not authorised
  in this session; the user may push
  in a follow-up command per the
  command protocol).
- **Next action:** the M3.2 closeout
  promotes the M3 closeout (T-020) to
  `Ready`. The next session reads the
  Milestone Closeout Standard, drafts
  `.ai/plans/M3-closeout.md`, and
  follows the standard to close M3
  with a retrospective. The M3.2
  session does **not** close M3 (per
  the brief: "Do not begin the
  following task").

### M3 closeout — M3 retrospective (per the Milestone Closeout Standard) — 2026-07-11

- **Task ID:** `T-020`
- **Milestone:** M3 — Project Registration
  (closed 2026-07-11)
- **Title:** M3 closeout — the M3
  retrospective per the Milestone
  Closeout Standard; the third M3
  slice (M3.1 + M3.2 + M3.x)
- **Status:** **Done (delivered
  2026-07-11).** M3 is **Done (closed
  2026-07-11)**. The M4-A plan is in
  `Awaiting Approval`; T-021 (M4-A.1
  infrastructure project skeleton) is
  the next `Ready` task.
- **Outcome:** The M3 closeout ships
  per the Milestone Closeout Standard
  § 4 + § 8: (1) **The M3 retrospective**
  at
  `retrospective-m3-project-registration.md`
  (13 sections: delivered
  capabilities, deferred capabilities,
  technical debt, known issues,
  lessons learned, architecture
  changes, documentation changes,
  testing summary, validation results,
  implementation reports, commit
  range, readiness for M4-A,
  recommendations for the next
  milestone); the M3 retrospective is
  the second milestone retrospective
  in the repository (the M2
  retrospective was the first). (2)
  **The M4-A plan** at
  `.ai/plans/M4-A-infrastructure-process-execution.md`
  (12 sections; Status: Awaiting
  Approval; the first M4-A task T-021
  is `Ready`). (3) **The M3 closeout
  plan** at `.ai/plans/M3-closeout.md`
  (mirrors the M2.6 closeout plan's
  structure). (4) **The M3 closeout
  implementation report** at
  `implementation-report-m3-closeout.md`
  (15+ sections; mirrors the M2.6
  closeout report). (5) **The M3
  closeout per-session handoff** at
  `.ai/handoffs/2026-07-11-m3-closeout.md`
  (mirrored to
  `.ai/handoffs/latest.md`). (6)
  **M3 moved from `Active` to `Done`**
  in `.ai/state/milestones.json` with
  `closed_at: 2026-07-11`. (7) **The
  `m3` annotated milestone tag** at
  the M3 closeout commit on `main`
  per the branching strategy rule 9.
  (8) **The project-continuity state
  updated** per Rule 15: session.json,
  tasks.json, current.md, task-board.md,
  milestones.json, ROADMAP.md,
  master delivery plan, the M3
  closeout handoff, the M3 closeout
  implementation report. (9)
  **Validation gate passed:** 273
  passed, 0 failed, 7 skipped (per
  ADR-016 / M4-D); 0 warnings, 0
  errors; format clean; visual smoke
  on `/projects` green.
- **Report:**
  `implementation-report-m3-closeout.md`.
- **Handoff:**
  `.ai/handoffs/2026-07-11-m3-closeout.md`
  (mirrored at
  `.ai/handoffs/latest.md`).
- **Git:** branch
  `feature/T-020-m3-closeout-and-retrospective`
  (created from `main` at the M3.2
  closeout commit `ff9010a`; the M3
  closeout commit
  `chore(m3.closeout): close M3 with retrospective, M4-A plan, and m3 milestone tag`
  is on this branch; the branch is
  fast-forwarded into `main` per the
  branching strategy rule 6; the
  branch is deleted per rule 7). The
  `m3` annotated milestone tag is at
  the M3 closeout commit on `main`
  per rule 9. No remote push (push
  is not authorised in this session;
  the user may push in a follow-up
  command per the command protocol).
- **Next action:** the M3 closeout
  promotes the M4-A.1 (T-021) to
  `Ready`. The next session approves
  the M4-A plan and begins the
  M4-A.1 implementation per the
  Progressive Coding Rule. The M3
  closeout session does **not**
  begin M4-A (per the brief: "Do not
  begin the following task").

### M3.1 closeout session — 2026-07-11

- **Task ID:** `T-018`
- **Milestone:** M3 — Project Registration
  (Active 2026-07-11)
- **Title:** Project registration slice 1
  (the contract, the in-memory store, the
  project-registration page, the projects
  list)
- **Status:** **Done (delivered 2026-07-11).**
  M3.1 is the first M3 implementation slice.
  M3 is **Active**; M3.2 is the next
  `Ready` task.
- **Outcome:** M3 surface lands end-to-end
  as a single slice: (1) **The contract**
  (`IProjectStore`, `IProjectService`) at
  `src/AiEng.Platform.Application/Projects/`;
  (2) **The in-memory store**
  (`InMemoryProjectStore`;
  `ConcurrentDictionary<Guid, Project>`;
  the M3 smoke test for the contract; M4-A
  swaps the `IProjectStore` registration
  in `AddProjects`); (3) **The domain
  entity** (`Project` aggregate root;
  immutable `Id`, human `Name`, absolute
  `Path`, immutable `CreatedAt`, mutable
  `LastUsedAt?`); (4) **The composition
  root**
  (`ProjectsServiceCollectionExtensions.AddProjects`); (5)
  **The UI surface** (`AppProjectCard`,
  `AppProjectList`, `/projects` page;
  page composes `AppPageHeader` +
  `AppBreadcrumb` (M2.3) +
  `AppProjectList`; sidebar entry
  registered through the M2.2
  `INavigationRegistry`); (6) **The
  architecture test**
  `Pages_Resolve_Projects_Through_Service`
  (enforces the single-seam rule on the
  page and the list); (7) **The unit + bUnit
  test coverage** (27 new unit tests: 16
  `IProjectServiceTests` + 11
  `InMemoryProjectStoreTests`; 13 new bUnit
  tests: 5 `AppProjectCardTests` + 4
  `AppProjectListTests` + 4
  `ProjectsPageTests`; 2 new architecture
  tests in
  `PagesResolveProjectsThroughServiceTests`);
  (8) **The surface documentation** at
  `docs/projects.md` (9 sections: Goals,
  Project Entity, Contract, M3/M4-A
  Boundary, UI Surface, Composition Root,
  Tests, Out of Scope, Acceptance Criteria).
  240 total tests pass (34 unit + 198
  bUnit + 8 architecture); 7 skipped per
  ADR-016 / M4-D; 0 warnings, 0 errors;
  format clean; visual smoke
  (`curl http://localhost:5286/projects`
  returns 200 with the expected markers).
  Three documented deviations: (1)
  `ValidationError` is a class, not a
  struct (`T?` semantics on the
  `Result<T>.Error` slot forced the
  change); (2) `IClock` is realised
  through .NET 8+ `TimeProvider`; (3)
  Disabled tests unchanged.
- **Report:**
  `implementation-report-m3-1-project-registration-slice-1.md`.
- **Handoff:**
  `.ai/handoffs/2026-07-11-m3-1-project-registration-slice-1.md`
  (mirrored at `.ai/handoffs/latest.md`).
- **Git:** branch
  `feature/m3-1-project-registration-slice-1`
  (created from `main` at the M2.6
  closeout commit; the M3.1 closeout
  commit
  `feat(m3.1): add project registration surface`
  is on this branch; the branch is
  fast-forwarded into `main` per the
  branching strategy rule 6; the branch
  is deleted per rule 7). No remote
  push (push is not authorised in this
  session; the user may push in a
  follow-up command per the command
  protocol).
- **Next action:** the M3.1 closeout
  promotes M3.2 (T-019) to `Ready`
  and creates the M3.2 plan at
  `.ai/plans/M3.2-project-registration-slice-2.md`
  (Status: Awaiting Approval); the
  next session approves the M3.2 plan
  and starts the M3.2 implementation
  per the Progressive Coding Rule. The
  M3.1 session does **not** implement
  M3.2 (per the brief: "Do not begin
  the following task").

### M2.6 closeout session — 2026-07-11

- **Task ID:** `M2.6`
- **Milestone:** M2 — Application Shell and
  Navigation (closed 2026-07-11)
- **Title:** M2 closeout and external
  Treehouse dogfooding checkpoint
- **Status:** **Done (closed 2026-07-11).**
  M2 is closed. The `m2` annotated
  milestone tag is at the M2 closeout
  commit on `main`.
- **Outcome:** Four sub-deliverables
  in one slice: (1) **The Milestone
  Closeout Standard** at
  `.ai/workflows/milestone-closeout.md`
  (10 sections; the canonical procedure
  every future milestone closeout must
  follow; the 13-section retrospective is
  the standard's required deliverable;
  the standard is the single source of
  truth for milestone closeout procedures;
  the standard is referenced from
  `.ai/README.md`'s workflows directory
  and task-routing table); (2) **The M2
  retrospective** at
  `retrospective-m2-application-shell-and-navigation.md`
  (13 sections, all populated; the first
  milestone retrospective in this
  repository; delivered capabilities C-019
  + C-022; deferred capabilities; technical
  debt; known issues; lessons learned —
  process + technical; architecture
  changes — ADR-005, ADR-013, ADR-014,
  ADR-016; documentation changes; testing
  summary 197 + 7 skipped; validation
  results 6 gates; implementation reports
  the 6 paths; commit range 11 commits
  from M0.5 closeout to M2 closeout;
  readiness for M3; recommendations for
  M3 — 8 concrete recommendations the M3
  plan accounts for); (3) **Project-
  continuity state + ROADMAP.md +
  master-delivery-plan.md updates**
  (M2.6 from in_progress to delivered;
  M2 from Active to Done with
  closed_at 2026-07-11; M2 evidence block
  updated; M3 plan path added; T-016
  Done; T-006 M2 summary Done; T-018
  M3.1 promoted to Ready; ROADMAP.md
  M2 row Done + M2.6 row Delivered +
  M2 DoD expanded; master-delivery-plan
  M2 row Done + M2.6 slice row Delivered
  + M2 evidence list updated); (4) **M3
  plan + first M3 task promotion** (the
  M3 plan at
  `.ai/plans/M3-project-registration.md`
  Status: Awaiting Approval; the first M3
  task T-018 M3.1 Ready; the M2 closeout
  commit
  `chore(m2.6): close M2 with retrospective,
  milestone closeout standard, and M3 plan`
  on the feature branch
  `feature/T-016-m2-closeout-and-treehouse-dogfooding`
  fast-forwarded into `main`; the
  feature branch deleted per the
  branching strategy rule 7; the `m2`
  annotated milestone tag at the M2
  closeout commit on `main` per rule 9).
- **Validation results:** the milestone-
  level validation gate per the Milestone
  Closeout Standard § 3 — `npm run
  css:build` (exit 0), `dotnet restore`
  (exit 0), `dotnet build` (0 warnings,
  0 errors), `dotnet test` (197 passed,
  0 failed, 7 skipped per ADR-016),
  `dotnet format --verify-no-changes`
  (exit 0), visual smoke (5 routes
  return 200 on `localhost:5211`; theme
  toggle markup present on every
  `AppLayout` page). Every gate is
  green.
- **Report:**
  `implementation-report-m2-6-m2-closeout.md`.
- **Retrospective:**
  `retrospective-m2-application-shell-and-navigation.md`.
- **Standard:**
  `.ai/workflows/milestone-closeout.md`.
- **Handoff:**
  `.ai/handoffs/2026-07-11-m2-6-m2-closeout.md`
  (mirrored at `.ai/handoffs/latest.md`).
- **Git:** branch
  `feature/T-016-m2-closeout-and-treehouse-dogfooding`
  (created from `main` at the M2.5
  closeout commit; the M2 closeout commit
  is on this branch; the branch is
  fast-forwarded into `main` per the
  branching strategy rule 6; the branch
  is deleted per rule 7; the `m2`
  annotated milestone tag is at the M2
  closeout commit on `main` per rule 9).
  No remote push (push is not authorised
  in this session; the user may push in
  a follow-up command per the command
  protocol).
- **Next action:** the M3 session
  follows per the Progressive Coding
  Rule. The M3 plan is in `Awaiting
  Approval`; the first M3 task
  (T-018 — M3.1) is `Ready`; the next
  session reads the M3 plan and the M2
  retrospective's § 13 recommendations,
  approves the M3 plan, and starts the
  M3.1 implementation per the
  Progressive Coding Rule.

### M2.5 closeout session — 2026-07-11

- **Task ID:** `M2.5`
- **Milestone:** M2 — Application Shell and
  Navigation
- **Title:** Empty routes, responsive,
  and accessibility (with T-017 theme
  toggle fix included at user opt-in)
- **Status:** **Done (closed 2026-07-11).**
- **Outcome:** Five sub-deliverables:
  (1) **Empty routes** —
  `Home.razor` and `NotFound.razor`
  rewritten to use `AppCard` +
  `AppEmptyState` with links to
  `/dashboard` and `/design-system`;
  (2) **Responsive matrix** —
  `AppLayout.razor.css` now has
  `@media` rules for the `lg` (≥1440),
  `md` (1280–1439), and `sm`
  (1024–1279) breakpoints; the content
  area gets `overflow-y: auto`; the
  topbar remains horizontal; the
  sidebar widths are 14rem / 12rem /
  10rem / 8rem across the breakpoints;
  `docs/ui-principles.md` § 10.1
  documents the matrix with the M2.5
  implementation;
  (3) **A11y audit** —
  `KeyboardSmokeTests` (4 tests),
  `AriaCurrentInvariantTests` (5 tests
  covering breadcrumb `aria-current`,
  `NavLink` active state, sidebar
  active link), `AxeCoreAuditTests`
  (3 tests registered but skipped
  per ADR-016 / M4-D);
  (4) **T-017 theme toggle fix** —
  `@rendermode InteractiveServer` added
  to `AppThemeToggle.razor` (not to
  `AppLayout.razor`; the layout's
  `@Body` is a `RenderFragment` delegate
  that Blazor refuses to serialize
  across the SSR → interactive boundary;
  declaring the directive on the layout
  throws `InvalidOperationException`
  at request time; the directive on
  the toggle itself is the
  minimum-blast-radius fix);
  (5) **Project-continuity state +
  implementation report + per-session
  handoff.** 18 new component tests
  (5 `EmptyRoutesTests` + 4
  `AppLayout_ThemeToggleWiringTests` +
  4 `AppLayout_ResponsiveMatrixTests` +
  5 `AriaCurrentInvariantTests`) + 3
  new architecture tests
  (`AxeCoreAuditTests`, all skipped).
  Total test count is now 197
  passing + 7 skipped, 0 failed
  (6 unit + 185 bUnit + 6 active
  architecture + 7
  registered-but-disabled). The
  visual smoke test confirms every
  route returns 200 and the theme
  toggle's markup is present on every
  page. The theme toggle's click
  handler is now wired: clicking the
  toggle in the running app changes
  the document theme immediately and
  persists across navigation and
  browser refresh (via the IIFE in
  `App.razor` that reads
  `localStorage["app-theme"]`).
- **Report:**
  `implementation-report-m2-5-empty-routes-responsive-accessibility.md`.
- **Handoff:**
  `.ai/handoffs/2026-07-11-m2-5-empty-routes-responsive-accessibility.md`
  (mirrored at
  `.ai/handoffs/latest.md`).
- **Git:** branch
  `feature/m2-5-empty-routes-responsive-accessibility`;
  closeout commit
  `feat(m2.5): add empty routes, responsive matrix, a11y audit, and theme toggle fix`.
  No remote configured; push skipped (per
  the brief).
- **Next action:** the M2.5 closeout
  promotes M2.6 to the next session
  (M2.6 plan promotion + closeout
  template + Treehouse dogfooding
  checkpoint). The M2.5 session does
  **not** implement M2.6 (per the
  Progressive Coding Rule).

### M2.4 closeout session — 2026-07-11

- **Task ID:** `M2.4`
- **Milestone:** M2 — Application Shell and
  Navigation
- **Title:** Project intelligence
  dashboard
- **Status:** **Done (closed 2026-07-11).**
- **Outcome:** 3 new types in
  `src/AiEng.Platform.Application/ProjectIntelligence/`
  (`IProjectIntelligenceReader`,
  `ProjectIntelligenceSnapshot`,
  `ProjectIntelligenceReader`); 1 new
  composition-root extension in
  `src/AiEng.Platform.App/Composition/`
  (`AddProjectIntelligence`); 1 new
  page at
  `src/AiEng.Platform.App/Components/Pages/Dashboard.razor`
  (the M0.5-data sections in
  **Populated** state; the M3+-data
  sections in **Empty** state); the
  theme toggle bug is fixed (added
  `appTheme.current` JS function;
  component reads the resolved theme
  in `OnAfterRenderAsync(firstRender)`;
  click handler updates `IsDark`
  synchronously before the JSInterop
  call; `JSDisconnectedException` is
  handled); 6 new unit tests for
  `ProjectIntelligenceReader`; 13 new
  bUnit tests (3 composition + 9
  dashboard + 4 theme toggle); 2 new
  architecture tests
  (`Dashboard_Resolves_State_Through_Reader`
  and
  `No_Page_Reaches_State_Directly`).
  Total test count is now 175
  passing + 4 skipped, 0 failed
  (6 unit + 163 bUnit + 6 active
  architecture). The dashboard
  consumes state only through
  `IProjectIntelligenceReader`
  (architecture test enforces this).
- **Report:**
  `implementation-report-m2-4-project-intelligence-dashboard.md`.
- **Handoff:**
  `.ai/handoffs/2026-07-11-m2-4-project-intelligence-dashboard.md`
  (mirrored at
  `.ai/handoffs/latest.md`).
- **Git:** branch
  `feature/m2-4-project-intelligence-dashboard`;
  closeout commit
  `feat(m2.4): add project intelligence dashboard`.
  No remote configured; push skipped (per
  the brief).
- **Next action:** the M2.4 closeout
  promotes T-015 (M2.5) to `Ready`
  and promotes the M2.5 plan stub
  to a full plan in `Awaiting
  Approval`; the next session
  approves the M2.5 plan and starts
  M2.5 implementation. The M2.4
  session does **not** implement
  M2.5 (per the Progressive Coding
  Rule).

### M2.3 closeout session — 2026-07-11

- **Task ID:** `M2.3`
- **Milestone:** M2 — Application Shell and
  Navigation
- **Title:** Top bar, breadcrumbs, and
  page header integration
- **Status:** **Done (closed 2026-07-11).**
- **Outcome:** 4 new components in
  `src/AiEng.Platform.App/Components/`:
  `AppTopBar` (replaces the M2.1
  `AppTopBarSlot` placeholder), the
  relocated `AppThemeToggle`
  (light/dark theme toggle; persists
  to `localStorage`; reads
  `data-theme` on `documentElement`),
  `AppUserAvatarSlot` (the user
  avatar placeholder; M3+ replaces
  it with the real user identity
  surface), and `AppBreadcrumb` (walks
  the M2.2 registry's `Parent` chain;
  `aria-current="page"` on the current
  item; separators are `aria-hidden`).
  `AppBreadcrumb` wired into
  `AppPageHeader.Breadcrumbs` on
  `DesignSystem.razor`. `AppTopBarSlot`
  and `AppTopBarSlotTests.cs` deleted;
  `AppLayout` updated to use
  `AppTopBar`. 27 new bUnit tests
  across 4 test files
  (`AppTopBarTests`,
  `AppThemeToggleTests`,
  `AppUserAvatarSlotTests`,
  `AppBreadcrumbTests`); 6 obsolete
  `AppTopBarSlotTests` removed. Total
  test count is now 150 passing + 4
  skipped, 0 failed (77 M1.2 + 25
  M2.1 + 23 M2.2 bUnit + 27 M2.3
  bUnit − 6 obsolete removed + 4
  active architecture). Two
  documented deviations: (1)
  `AppTopBar` uses `div.app-topbar`
  + `Leading` / `Trailing` slots
  rather than `AppStack` +
  `AppPageHeader`; the surface still
  composes `AppTopBar` +
  `AppPageHeader` + `AppBreadcrumb`,
  matching the plan's intent. (2)
  Optional architecture test
  `Breadcrumb_Follows_Registry_Parent_Chain`
  was skipped per plan § 8 step 11
  which marked it optional.
- **Report:**
  `implementation-report-m2-3-topbar-breadcrumbs.md`.
- **Handoff:**
  `.ai/handoffs/2026-07-11-m2-3-topbar-breadcrumbs.md`
  (mirrored at
  `.ai/handoffs/latest.md`).
- **Git:** branch
  `feature/m2-3-topbar-breadcrumbs`;
  closeout commit
  `feat(m2.3): add top bar, breadcrumb, and page header integration`.
  No remote configured; push skipped (per
  the brief).
- **Next action:** the M2.3 closeout
  promotes T-014 (M2.4) to `Ready`
  and promotes the M2.4 plan stub
  to a full plan in `Awaiting
  Approval`; the next session
  approves the M2.4 plan and starts
  M2.4 implementation. The M2.3
  session does **not** implement
  M2.4 (per the Progressive Coding
  Rule).

### M2.2 closeout session — 2026-07-11

- **Task ID:** `M2.2`
- **Milestone:** M2 — Application Shell and
  Navigation
- **Title:** Navigation registry and
  sidebar
- **Status:** **Done (closed 2026-07-11).**
- **Outcome:** 4 new types in
  `src/AiEng.Platform.Application/Navigation/`
  (`INavigationRegistry`,
  `RouteMetadata`,
  `RouteMetadataAttribute`,
  `RouteRegistry`); 2 new extension
  methods in
  `src/AiEng.Platform.App/Composition/`
  (`AddPlatformServices` +
  `AddNavigation`); 3 new components in
  `src/AiEng.Platform.App/Components/Navigation/`
  (`AppSidebar`, `AppSidebarItem`,
  `AppNavItem`); `[RouteMetadata]`
  applied to all 6 pages; the
  composition root wired in
  `Program.cs`; the
  `Pages_AreReachable_Through_Registry`
  architecture test is active and green;
  28 new bUnit / integration tests +
  1 new architecture test; the M2.1
  `AppSidebarSlot` placeholder is
  replaced by the registry-driven
  `AppSidebar`; the 19 M1.2 components,
  77 bUnit tests, and 3 active + 4
  registered-but-disabled architecture
  tests are preserved. Total test count
  is now 129 passing + 4 skipped, 0
  failed (77 M1.2 + 25 M2.1 + 23 M2.2
  bUnit + 4 active architecture; 4
  registered-but-disabled
  architecture).
- **Report:**
  `implementation-report-m2-2-navigation-registry-sidebar.md`.
- **Handoff:**
  `.ai/handoffs/2026-07-11-m2-2-navigation-registry-sidebar.md`
  (mirrored at
  `.ai/handoffs/latest.md`).
- **Git:** branch
  `feature/m2-2-navigation-registry-sidebar`;
  closeout commit
  `feat(m2.2): add navigation registry and sidebar`.
  No remote configured; push skipped (per
  the brief).
- **Next action:** the M2.2 closeout
  promotes T-003 (M2.3) to `Ready` and
  promotes the M2.3 plan stub to a
  full plan in `Awaiting Approval`;
  the next session approves the M2.3
  plan and starts M2.3
  implementation. The M2.2 session
  does **not** implement M2.3 (per
  the Progressive Coding Rule).

### M2.1 closeout session — 2026-07-11

- **Task ID:** `M2.1`
- **Milestone:** M2 — Application Shell and
  Navigation
- **Title:** Application shell foundation
- **Status:** **Done (closed 2026-07-11).**
- **Outcome:** 5 new Blazor components /
  layouts in the application shell
  foundation (2 layouts: `AppLayout`,
  `AppEmptyLayout`; 2 placeholder shell
  components: `AppSidebarSlot`,
  `AppTopBarSlot`; 1 presentational helper:
  `AppShellRegion`); 25 new bUnit
  component tests across 4 test files
  (8 + 6 + 5 + 6); the M1.1 chrome
  (`MainLayout.razor`, `MainLayout.razor.css`,
  `NavMenu.razor`, `NavMenu.razor.css`) is
  removed; the Tailwind content path
  includes the new `Layouts/` directory;
  the `Layouts/_Imports.razor` resolves
  cross-folder type references; the five
  M1 template pages and `/design-system`
  reach the new layout root in place; the
  19 M1.2 components, 77 bUnit tests, and
  3 active + 4 registered-but-disabled
  architecture tests are preserved.
  Total test count is now 105 passing +
  4 skipped, 0 failed (77 M1.2 + 25
  M2.1 + 3 active architecture; 4
  registered-but-disabled architecture).
- **Report:**
  `implementation-report-m2-1-application-shell-foundation.md`.
- **Handoff:**
  `.ai/handoffs/2026-07-11-m2-1-application-shell-foundation.md`
  (mirrored at `.ai/handoffs/latest.md`).
- **Git:** branch
  `feature/m2-1-application-shell`;
  closeout commit
  `feat(m2.1): add application shell foundation`.
  No remote configured; push skipped (per
  the brief).
- **Next action:** the M2.1 closeout
  promotes T-002 (M2.2) to `Ready` and
  expands the M2.2 plan stub to a full
  plan in `Awaiting Approval`; the next
  session approves the M2.2 plan and
  starts M2.2 implementation. The
  M2.1 session does **not** implement
  M2.2 (per the Progressive Coding
  Rule).

### M1 closeout session — 2026-07-10

- **Task ID:** `M1-CLOSEOUT`
- **Milestone:** M1 — Design System Core
- **Title:** Close M1, introduce project
  continuity, prepare M2.1 plan
- **Outcome:** 19 reusable Blazor components
  (Primitives 7, Layout 4, Display 2, Feedback
  5, Inputs 1), 77 bUnit component tests, 3
  active architecture tests + 4
  registered-but-disabled composition-root
  tests, the `/design-system` documentation
  page, the Tailwind v3 + PostCSS pipeline,
  the design-token catalogue, light + dark
  themes. All seven ROADMAP M1 DoD items
  satisfied. The project-continuity system
  (Rule 15 in `AGENTS.md`) is in place.
- **Reports:**
  `implementation-report-m1-bootstrap.md`,
  `implementation-report-m1-1-frontend-foundation.md`,
  `implementation-report-m1-2-design-system-core.md`,
  `implementation-report-m1-closeout.md`.
- **Handoff:**
  `.ai/handoffs/2026-07-10-m1-closeout.md`.
- **Git:** first two commits
  `1722bd235830cfd8b180191953116c058c92edef`
  and
  `2ba1fad3cc45bee513ba38c7269e024bf8667ef9`
  on `main`. 166 files committed. Working
  tree clean. No remote configured.

### M1 follow-up — Re-anchor composition-root tests in `ROADMAP.md` matrix

- **Task ID:** `M1-FU-2`
- **Milestone:** M1 — Design System Core
- **Title:** Re-anchor composition-root
  architecture tests in the
  `ROADMAP.md` § 4 matrix
- **Outcome:** `ROADMAP.md` § 4
  ("Progressive self-dogfooding matrix") was
  updated during the M1 closeout session to
  list the four composition-root tests as
  `Delivered in M1 closeout — Active in M4-D`.
  See the M1 closeout report's
  "Files Modified" entry for `ROADMAP.md`.

---

## Deferred

For later milestones, a single summary task is
kept here so the work is not forgotten but the
task board does not become a speculative
backlog. Each summary task is fleshed out into
detailed tasks when the milestone approaches.

### M3 — Project Registration (summary) — Done (archived 2026-07-11)

- **Milestone:** M3 — Project Registration
  (Done 2026-07-11; closed 2026-07-11).
- **Status:** Done (M3.1 Delivered
  2026-07-11; M3.2 Delivered 2026-07-11;
  M3 closeout M3.x Delivered 2026-07-11).
  M3 is moved to `Done` with
  `closed_at: 2026-07-11`; the `m3`
  annotated milestone tag is at the M3
  closeout commit on `main`. The M3
  retrospective is at
  `retrospective-m3-project-registration.md`.
  This summary entry is archived
  (M3 is closed; the summary is no
  longer in `Deferred`).

### M4-A — Infrastructure / Process Execution (summary) — Awaiting Approval

- **Milestone:** M4-A.
- **Status:** Awaiting Approval (M4-A plan
  produced 2026-07-11 by the M3 closeout
  per the Milestone Closeout Standard
  § 8; the plan is at
  `.ai/plans/M4-A-infrastructure-process-execution.md`;
  the first M4-A task T-021, M4-A.1
  infrastructure project skeleton, is
  `Ready` in `.ai/state/tasks.json`).
- **First action (later):** the next
  session approves the M4-A plan and
  begins the M4-A.1 implementation per
  the Progressive Coding Rule.

### M4-B — Capability Detection (summary)

- **Milestone:** M4-B.
- **First action (later):** draft
  `.ai/plans/M4-B-capability-detection.md`.

### M4-C — Provider Registry Foundation (summary)

- **Milestone:** M4-C.
- **First action (later):** draft
  `.ai/plans/M4-C-provider-registry-foundation.md`.

### M4-D — First Concrete Process Providers (summary)

- **Milestone:** M4-D.
- **First action (later):** draft
  `.ai/plans/M4-D-first-concrete-process-providers.md`.

### M5 — Native Git Worktrees (summary)

- **Milestone:** M5.
- **First action (later):** draft
  `.ai/plans/M5-native-git-worktrees.md`.

### M6 — Agent Runtime Launching (summary)

- **Milestone:** M6.
- **First action (later):** draft
  `.ai/plans/M6-agent-runtime-launching.md`.

### M7 — Review and Quality Gates (summary)

- **Milestone:** M7.
- **First action (later):** draft
  `.ai/plans/M7-review-and-quality-gates.md`.

### M8 — Autonomous Loops, Orchestration, Production Hardening (summary)

- **Milestone:** M8.
- **First action (later):** draft
  `.ai/plans/M8-autonomous-loops-orchestration.md`.
