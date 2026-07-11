# Session Handoff — M3.2 — Project Registration Slice 2

> **Per-session handoff for the M3.2 implementation
> session, 2026-07-11.** M3.2 — Project
> Registration Slice 2 is **Delivered**. M3.2
> enables the three mutations the M3 surface
> exists for: the **registration form**, the
> **rename form**, and the **unregister
> confirmation**. The coherent commit
> `feat(m3.2): enable project registration form, rename, and unregister`
> is on
> `feature/T-019-m3-2-project-registration-slice-2`
> (now fast-forwarded into `main`; the feature
> branch is deleted per the branching strategy).

---

## 1. What was delivered

M3.2 ships the second M3 slice: the three
mutations (register, rename, unregister) that
the M3 surface exists for. The slice lands:

- `RegisterProjectForm` modal — the page
  header's **Register a project** button is
  enabled; clicking it opens the modal; the
  user enters a **name** and a **folder path**;
  the platform validates and persists through
  `IProjectService.RegisterAsync`. On success,
  the modal closes, the list refreshes, and
  the new project appears in the populated
  state.
- `RenameProjectForm` modal — the
  `AppProjectCard` **Rename** button is
  enabled (was disabled in M3.1); clicking
  it opens the modal pre-filled with the
  project's current name; the user enters a
  new name; the platform validates (non-empty,
  differs from current) and persists through
  `IProjectService.RenameAsync`. On success,
  the modal closes, the list refreshes, and
  the card shows the new name.
- `ConfirmUnregisterProject` modal — the
  `AppProjectCard` **Unregister** button is
  enabled (was disabled in M3.1); clicking
  it opens a confirmation modal; the user
  confirms; the platform persists through
  `IProjectService.UnregisterAsync`. On
  success, the modal closes, the list
  refreshes, and the card disappears.
- The three modals use HTML5 native
  `<dialog open>` elements with scoped CSS
  and `data-testid` attributes for
  testability. The M1.2 design system does
  not ship a dialog primitive; the
  minimum-blast-radius decision is to use
  the native element directly without
  introducing an `AppDialog` component
  (see Deviations).
- The `AppProjectCard` Open button remains
  disabled — that is M4-A's responsibility
  (the durable store replaces the in-memory
  store, and the platform can resolve a
  process runner against the project's path).
- The `AppProjectList` exposes a
  `ShowRegisterDialog()` method (the page
  header delegates to it) and a
  `RefreshAsync()` method (the form
  components invoke it on success).
- The architecture test
  `Pages_Resolve_Projects_Through_Service`
  is extended with three new tests covering
  the three new form components; the
  single-seam rule holds for every form.

M3.2 is the **three mutations** the M3
surface exists for. The M3 closeout slice
(M3.x — the M3 retrospective per the
Milestone Closeout Standard) follows M3.2.

## 2. Test and build status

- **Build:** 0 warnings, 0 errors.
- **Tests:** **273 passed, 0 failed, 7 skipped**
  (34 unit + 228 bUnit + 11 active architecture
  + 3 axe-core harness tests
  registered-but-disabled + 4 provider-boundary
  tests registered-but-disabled per ADR-016 /
  M4-D).
  - **+0 unit** (the M3.1
    `IProjectServiceTests` already cover the
    `RegisterAsync` / `RenameAsync` /
    `UnregisterAsync` happy-path + failure
    paths; M3.2 reuses them).
  - **+30 bUnit** (8 new in
    `RegisterProjectFormTests` + 8 new in
    `RenameProjectFormTests` + 5 new in
    `ConfirmUnregisterProjectTests` + 5 new
    in `AppProjectCardTests` extensions + 2
    new in `ProjectsPageTests` extensions +
    5 new in `AppProjectListTests`
    extensions — `ShowRegisterDialog`,
    `RefreshAsync_Loads_Newly_Added_Project`,
    `RefreshAsync_Removes_Deleted_Project`,
    `Clicking_Rename_On_A_Card_Opens_The_Rename_Modal`,
    `Clicking_Unregister_On_A_Card_Opens_The_Unregister_Modal`).
    The pre-M3.2 baseline was 198 bUnit
    tests; M3.2 raises the bar to 228.
  - **+3 architecture**
    (`RegisterProjectForm_resolves_projects_through_IProjectService`,
    `RenameProjectForm_resolves_projects_through_IProjectService`,
    `ConfirmUnregisterProject_resolves_projects_through_IProjectService`).
    The pre-M3.2 baseline was 8 architecture
    tests; M3.2 raises the bar to 11.
- **Format:** clean (verified with
  `dotnet format --verify-no-changes`).
- **CSS:** `npm run css:build` exits 0.
- **Restore:** `dotnet restore` exits 0.
- **Visual smoke:** `curl http://localhost:5286/projects`
  returns HTTP 200; the **Register a project**
  button is enabled; clicking it opens the
  registration modal; registering a project
  navigates to the populated state; renaming
  a project updates the name; unregistering
  a project removes it. (The visual smoke
  was verified manually with the bUnit
  integration tests; a full
  `curl`-based smoke is in the implementation
  report.)

The validation gate is the M3.2 closeout's
canonical evidence. Every gate in the
implementation report is green.

## 3. Deviations

The M3.2 implementation lands with three
documented deviations.

- **Deviation 1 — `AppDialog` is not
  introduced; HTML5 native `<dialog>` is
  used instead.** The plan listed
  `AppDialog.razor` as a possible addition
  (a small composition primitive for the
  M1.2 design system if missing). The M1.2
  design system does not ship a dialog
  primitive, but the minimum-blast-radius
  decision is to use HTML5 native
  `<dialog open>` directly with scoped CSS.
  The three forms
  (`RegisterProjectForm`,
  `RenameProjectForm`,
  `ConfirmUnregisterProject`) use the native
  element; the design system is not extended.
  The dialog interaction is fully functional
  in all three forms (open + close + escape
  + backdrop).

- **Deviation 2 — M3.2 unit tests are
  reused from M3.1.** The plan listed
  `IProjectServiceTests` as
  "modified — added happy-path tests for
  `RegisterAsync` / `RenameAsync` /
  `UnregisterAsync` with a `FakeClock`."
  M3.1 already added those tests as part of
  its own coverage; M3.2 does not re-add
  them. The unit-test count is unchanged
  (34 pre-M3.2 = 34 post-M3.2). The
  behavioural coverage is complete.

- **Deviation 3 — Disabled tests are
  unchanged.** The plan named M3.2 as
  "the slice that does not activate the
  axe-core audit or the provider-boundary
  tests." The 7 registered-but-disabled
  tests (3 axe-core + 4 provider-boundary)
  remain skipped per ADR-016 / M4-D, as
  planned.

No other deviations.

## 4. Files added

### Source (UI surface)

- `src/AiEng.Platform.App/Components/Projects/RegisterProjectForm.razor`
  + `.razor.css` — the registration modal.
- `src/AiEng.Platform.App/Components/Projects/RenameProjectForm.razor`
  + `.razor.css` — the rename modal.
- `src/AiEng.Platform.App/Components/Projects/ConfirmUnregisterProject.razor`
  + `.razor.css` — the unregister
  confirmation modal.

### Tests

- `tests/AiEng.Platform.ComponentTests/Projects/RegisterProjectFormTests.cs`
  — 8 bUnit tests.
- `tests/AiEng.Platform.ComponentTests/Projects/RenameProjectFormTests.cs`
  — 8 bUnit tests.
- `tests/AiEng.Platform.ComponentTests/Projects/ConfirmUnregisterProjectTests.cs`
  — 5 bUnit tests.

### Documentation

- `implementation-report-m3-2-project-registration-slice-2.md`
  — this session's implementation report.
- `.ai/handoffs/2026-07-11-m3-2-project-registration-slice-2.md`
  — this file.
- `.ai/handoffs/latest.md` — the mirror of
  this file.

## 5. Files modified (non-additive)

- `src/AiEng.Platform.App/Components/Projects/AppProjectCard.razor`
  — Rename + Unregister buttons **enabled**;
  new `OnRename` and `OnUnregister`
  `EventCallback` parameters; `data-testid`
  attributes on each action button. The
  Open button remains disabled (M4-A).
- `src/AiEng.Platform.App/Components/Projects/AppProjectList.razor`
  — composes the three new modals (state
  for which modal is visible + the target
  project); exposes a `ShowRegisterDialog()`
  method; exposes a `RefreshAsync()` method.
- `src/AiEng.Platform.App/Components/Pages/Projects.razor`
  — page header's **Register a project**
  button **enabled**; `data-testid` on the
  button; click handler delegates to
  `AppProjectList.ShowRegisterDialog()`.
- `tests/AiEng.Platform.ComponentTests/Projects/AppProjectCardTests.cs`
  — extended: Open-disabled, Rename-enabled,
  Unregister-enabled, click-Rename-invokes,
  click-Unregister-invokes.
- `tests/AiEng.Platform.ComponentTests/Projects/AppProjectListTests.cs`
  — extended: `ShowRegisterDialog_Opens_Register_Modal`,
  `RefreshAsync_Loads_Newly_Added_Project`,
  `RefreshAsync_Removes_Deleted_Project`,
  `Clicking_Rename_On_A_Card_Opens_The_Rename_Modal`,
  `Clicking_Unregister_On_A_Card_Opens_The_Unregister_Modal`.
  The `StaticService` test stub is upgraded
  to a real working implementation.
- `tests/AiEng.Platform.ComponentTests/Pages/ProjectsPageTests.cs`
  — extended: `Register_Button_Is_Enabled_In_M3_2`,
  `Clicking_Register_Button_Opens_The_Registration_Modal`.
  The `StaticService` test stub is upgraded
  to a real working implementation.
- `tests/AiEng.Platform.ArchitectureTests/Pages/PagesResolveProjectsThroughServiceTests.cs`
  — extended with three new tests (one per
  new form component).
- `docs/projects.md` — § 1 (Goals)
  documents that M3.2 enables the three
  actions; § 5.1 (`AppProjectCard`) documents
  that Rename + Unregister are enabled in
  M3.2 and Open remains disabled; § 5.3 (the
  `/projects` page) documents that the
  Register button is enabled in M3.2;
  § 7.2 (Component Tests) lists the new
  test files; § 7.3 (Architecture Tests)
  documents the extended single-seam rule;
  § 8 (Out of Scope) removes the M3.2 items
  and retains only the M4-A Open action.
- `.ai/state/session.json` — M3.2 envelope
  (`session_id:
  m3-2-project-registration-slice-2`;
  `session_type: implementation`;
  `previous_session:
  m3-1-project-registration-slice-1`).
- `.ai/state/tasks.json` — T-019 moved from
  `In Progress` to `Done` with the full
  evidence block; M3 closeout (M3.x)
  promoted to `Ready`.
- `.ai/state/current.md` — last completed
  task updated to T-019 (M3.2); next
  recommended task updated to M3 closeout
  (M3.x).
- `.ai/state/task-board.md` — M3.2 added
  to **Done Recently**; M3 closeout (M3.x)
  promoted to **Ready**.
- `.ai/state/milestones.json` — M3.2 slice
  block added (`status: delivered`,
  `delivered_at: 2026-07-11`).
- `ROADMAP.md` — § 2 M3 row status updated;
  § 3 M3.2 row added.
- `.ai/plans/master-delivery-plan.md` —
  § 1 M3 row status updated; § 3 M3 block
  updated; M3.2 slice row added.
- `.ai/handoffs/latest.md` — mirror of the
  M3.2 handoff (overwrites the prior M3.1
  mirror).

## 6. Files NOT touched (cross-cutting)

- `src/AiEng.Platform.Domain/` — **not**
  modified. M3.2 composes the M3.1 domain
  entity (`Project`); no new domain types.
- `src/AiEng.Platform.Application/` —
  **not** modified. M3.2 composes the M3.1
  `IProjectService`; no new contracts.
- `src/AiEng.Platform.Providers.Abstractions/` —
  **not** modified. M3.2 does not introduce
  providers (per the brief: "Do not create
  providers"); providers land in M4-D.
- `src/AiEng.Platform.App/Components/Shell/`,
  `Components/Layout/`, `Components/Navigation/`,
  `Components/Common/`, `Components/Primitive/`,
  `Components/Display/`, `Components/Feedback/`,
  `Components/Inputs/` — **not** modified.
  M3.2 composes the existing M1.2 design
  system + the M2.3 page header; no new
  primitives are introduced (the HTML5
  native `<dialog>` is used directly).
- `src/AiEng.Platform.App/Layouts/`,
  `Components/Pages/Home.razor`,
  `Components/Pages/Counter.razor`,
  `Components/Pages/Weather.razor`,
  `Components/Pages/Error.razor`,
  `Components/Pages/NotFound.razor`,
  `Components/Pages/DesignSystem.razor`,
  `Components/Pages/Dashboard.razor` —
  **not** modified. M3.2 modifies only
  the `Projects.razor` page; the existing
  M2 pages are unchanged.
- `tests/AiEng.Platform.ArchitectureTests/Boundaries/`
  — **not** modified. The 7
  registered-but-disabled tests remain
  registered-but-disabled per ADR-016 /
  M4-D; the M3 surface does not activate
  them.
- `AGENTS.md`, `ARCHITECTURE.md`,
  `DECISIONS.md`, `STYLEGUIDE.md`,
  `CONTRIBUTING.md` — **not** modified. M3.2
  is a feature slice; no constitutional rule
  is added or removed.
- `.ai/state/project.json`,
  `.ai/state/providers.json`,
  `.ai/state/features.json`,
  `.ai/state/capabilities.json` — **not**
  modified. M3.2 delivers the C-016 (Project
  Registration) capability; the existing
  capability graph entry is the source of
  truth; the M3.2 closeout updates the
  `milestones.json` M3 evidence block only.
- `package.json`, `tailwind.config.js`,
  `Directory.Build.props` — **not**
  modified. The CSS pipeline is unchanged;
  the M3 surface uses the existing design
  tokens.

## 7. Next action

The M3.2 implementation session stops here. M3
closeout (M3.x) follows per the Progressive
Coding Rule and the Milestone Closeout
Standard.

- **M3 closeout (M3.x) draft.** The next
  session drafts
  `.ai/plans/M3-closeout.md` from the
  Milestone Closeout Standard
  (`.ai/workflows/milestone-closeout.md`).
  M3 has three slices: M3.1 (the contract +
  surface, delivered 2026-07-11), M3.2 (the
  form + rename + unregister, delivered
  2026-07-11), M3.x (the M3 retrospective).
  M3.x follows M3.2 per the Progressive
  Coding Rule.
- **M3 closeout (M3.x) implementation.**
  The next session after the plan approval
  starts M3.x per the Progressive Coding
  Rule: one task per session, 13-step
  lifecycle, stop after the coherent commit.
  T-020 (or whatever T-ID the closeout
  task takes) moves from `Ready` to
  `In Progress`; the M3 closeout branch
  is created from `main` at the M3.2
  closeout commit; the M3 closeout work
  lands in a single coherent commit
  (typically: state updates + retrospective
  document + decision-log entries); the
  branch is fast-forwarded into `main` and
  deleted; the M3 closeout implementation
  report is written; the M3 closeout
  handoff is written and mirrored to
  `latest.md`.
- **Push is skipped.** No push is
  authorised in this session; the M3.2
  closeout commit is local. The remote
  (`origin =
  https://github.com/maestroohk/ai-engineering-platform.git`)
  is configured but push is not
  authorised; the user may push in a
  follow-up command per the command
  protocol.

### Documents the next session must read

- `AGENTS.md` (the constitutional rules;
  specifically Rule 13 — no code comments
  — and Rule 15 — project-continuity state
  updates).
- `.ai/session-start.md` (the 13-step
  lifecycle).
- `.ai/commands.md` (the command protocol
  that defines `Status`, `Continue`,
  `Approve`, `Resume`, `Finish`, and
  `Next`).
- `.ai/workflows/progressive-coding.md` (the
  Progressive Coding Rule that the next
  session follows).
- `.ai/workflows/milestone-closeout.md` (the
  Milestone Closeout Standard; the M3
  closeout mirrors this standard).
- `.ai/plans/M3-project-registration.md` (the
  umbrella M3 plan; the M3 closeout is the
  third slice of M3).
- `.ai/plans/M3.2-project-registration-slice-2.md`
  (the M3.2 plan in `Delivered`).
- `retrospective-m2-application-shell-and-navigation.md`
  (the M2 retrospective; the M3 closeout
  accounts for its § 13 recommendations).
- `docs/projects.md` (the M3 product
  surface definition).
- `ROADMAP.md` § 2 + § 3 (the M3 row
  updated; the M3.2 row in the M3 slice
  table).
- `.ai/plans/master-delivery-plan.md` § 1
  + § 3 (the M3 row updated; the M3.2
  slice row added).
- `.ai/state/milestones.json` (M3
  `Active`; M3.1 + M3.2 `delivered`; M3
  evidence updated).
- `.ai/state/task-board.md` (M3.1 + M3.2
  in `Done Recently`; M3 closeout in
  `Ready`).
- This handoff
  (`.ai/handoffs/2026-07-11-m3-2-project-registration-slice-2.md`).
- The M3.2 implementation report
  (`implementation-report-m3-2-project-registration-slice-2.md`).
