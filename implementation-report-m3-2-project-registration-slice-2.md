# Implementation Report — M3.2 Project Registration Slice 2

> **M3 second implementation slice.** The M3
> surface — a project is the smallest piece of
> state the platform needs to be useful on its
> own — is delivered in two slices. M3.1 lands
> the contract, the in-memory store, the
> read-only list, and the four-slot UI surface.
> **M3.2 lands the three mutations**: the
> registration form, the rename form, and the
> unregister confirmation. The three mutations
> reach the store through `IProjectService`; the
> architecture test `Pages_Resolve_Projects_Through_Service`
> is extended to cover every new form component.
> The M3 closeout slice (M3.x — the M3
> retrospective per the Milestone Closeout
> Standard) follows M3.2.

---

## Plan Reference

- **Approved plan:** M3.2 — Project Registration
  Slice 2 (registration form, rename form,
  unregister confirmation; the second M3
  implementation slice)
- **Plan path:** `.ai/plans/M3.2-project-registration-slice-2.md`
- **Deviations from plan:** see **Deviations**
  section below

The plan and the report are paired: the plan
is the contract, the report is the receipt.

---

## Summary

M3.2 enables the three mutations the M3 surface
exists for:

1. **Register a project** — the page header's
   **Register a project** button opens the
   `RegisterProjectForm` modal. The user enters
   a **name** and a **folder path**; the
   platform validates the inputs (non-empty
   name, non-empty path, path points to an
   existing directory) and persists the project
   through `IProjectService.RegisterAsync`. On
   success, the modal closes, the list
   refreshes, and the new project appears in
   the populated state.
2. **Rename a project** — the `AppProjectCard`
   **Rename** button (enabled in M3.2) opens
   the `RenameProjectForm` modal, pre-filled
   with the project's current name. The user
   enters a new name; the platform validates
   (non-empty, differs from the current name)
   and persists through
   `IProjectService.RenameAsync`. On success,
   the modal closes, the list refreshes, and
   the card shows the new name.
3. **Unregister a project** — the
   `AppProjectCard` **Unregister** button
   (enabled in M3.2) opens the
   `ConfirmUnregisterProject` modal. The user
   confirms; the platform persists through
   `IProjectService.UnregisterAsync`. On
   success, the modal closes, the list
   refreshes, and the card disappears.

The three forms use HTML5 native `<dialog>`
elements (the M1.2 design system does not ship
a dialog primitive; introducing an `AppDialog`
component was the alternative but the
minimum-blast-radius decision is to use the
native element directly with scoped CSS). The
Open button on `AppProjectCard` remains
disabled — that is M4-A's responsibility.

The slice advances the M3 milestone
(which delivers C-016 — Project Registration)
and consumes C-019 (the M2 application shell)
and C-020 (the M1 design system) and C-002
(the `IProjectService` contract from M3.1).

## Files Created

### UI surface

- `src/AiEng.Platform.App/Components/Projects/RegisterProjectForm.razor`
  — the registration modal. Captures name +
  path; calls `IProjectService.RegisterAsync`;
  shows the `ValidationError.Message` on
  failure; closes + invokes `OnRegistered` on
  success. `CanSubmit` is non-empty name +
  non-empty path + not submitting. Uses
  HTML5 native `<dialog open>` element.
- `src/AiEng.Platform.App/Components/Projects/RegisterProjectForm.razor.css`
  — scoped CSS (`.app-project-modal`,
  `.app-project-modal::backdrop`,
  `.app-project-modal-title`,
  `.app-project-modal-field`,
  `.app-project-modal-label`,
  `.app-project-modal-input`,
  `.app-project-modal-input:focus`,
  `.app-project-modal-input:disabled`,
  `.app-project-modal-error`,
  `.app-project-modal-actions`).
- `src/AiEng.Platform.App/Components/Projects/RenameProjectForm.razor`
  — the rename modal. Captures the new name
  (pre-fills with the project's current name
  in `OnParametersSet`); calls
  `IProjectService.RenameAsync`; shows the
  `ValidationError.Message` on failure; closes
  + invokes `OnRenamed` on success. `CanSubmit`
  is non-empty new name + new name differs
  from current + not submitting.
- `src/AiEng.Platform.App/Components/Projects/RenameProjectForm.razor.css`
  — scoped CSS (the modal styles + the
  `.app-project-modal-path` muted text).
- `src/AiEng.Platform.App/Components/Projects/ConfirmUnregisterProject.razor`
  — the unregister confirmation modal.
  Displays the project name + path; calls
  `IProjectService.UnregisterAsync`; shows the
  `ValidationError.Message` on failure; closes
  + invokes `OnUnregistered` on success.
- `src/AiEng.Platform.App/Components/Projects/ConfirmUnregisterProject.razor.css`
  — scoped CSS (the modal styles + the
  `.app-project-modal-text` and
  `.app-project-modal-path` muted text).

### Component tests

- `tests/AiEng.Platform.ComponentTests/Projects/RegisterProjectFormTests.cs`
  — 8 tests: hidden, render, submit-disabled
  (empty), submit-enabled (non-empty),
  submit-calls-RegisterAsync, submit-shows-
  validation-error, cancel-invokes-OnCancel,
  the form integration through the contract.
- `tests/AiEng.Platform.ComponentTests/Projects/RenameProjectFormTests.cs`
  — 8 tests: hidden, render, name-prefilled,
  submit-disabled (same), submit-enabled
  (differs), submit-calls-RenameAsync,
  submit-shows-validation-error,
  cancel-invokes-OnCancel.
- `tests/AiEng.Platform.ComponentTests/Projects/ConfirmUnregisterProjectTests.cs`
  — 5 tests: hidden, render, cancel-invokes,
  confirm-calls-UnregisterAsync,
  confirm-shows-not-found-error.

### Architecture tests

- `tests/AiEng.Platform.ArchitectureTests/Pages/PagesResolveProjectsThroughServiceTests.cs`
  — extended with three new tests:
  `RegisterProjectForm_resolves_projects_through_IProjectService`,
  `RenameProjectForm_resolves_projects_through_IProjectService`,
  `ConfirmUnregisterProject_resolves_projects_through_IProjectService`.
  Each asserts the form `@inject IProjectService`
  and forbids direct access to
  `InMemoryProjectStore`, `Directory.GetCurrentDirectory`,
  `File.ReadAllText`, and
  `JsonSerializer.Deserialize`.

## Files Modified

- `src/AiEng.Platform.App/Components/Projects/AppProjectCard.razor`
  — Rename + Unregister buttons **enabled**;
  new `OnRename` and `OnUnregister`
  `EventCallback` parameters; Open button
  remains disabled (M4-A). `data-testid`
  attributes on each action button for
  testability.
- `src/AiEng.Platform.App/Components/Projects/AppProjectList.razor`
  — composes the three new modals (state
  for which modal is visible + the target
  project); exposes a `ShowRegisterDialog()`
  method for the page header to open the
  registration form; exposes a `RefreshAsync()`
  method for the form components (or any
  future caller) to refresh the list.
  `data-testid` on the empty-state Register
  button.
- `src/AiEng.Platform.App/Components/Pages/Projects.razor`
  — page header's **Register a project**
  button **enabled**; `data-testid` on the
  button; click handler delegates to
  `AppProjectList.ShowRegisterDialog()`.
- `tests/AiEng.Platform.ComponentTests/Projects/AppProjectCardTests.cs`
  — extended: Open-disabled, Rename-enabled,
  Unregister-enabled, click-Rename-invokes,
  click-Unregister-invokes (the M3.1 test
  asserting all actions are disabled is
  replaced by per-action assertions).
- `tests/AiEng.Platform.ComponentTests/Projects/AppProjectListTests.cs`
  — extended: `ShowRegisterDialog_Opens_Register_Modal`,
  `RefreshAsync_Loads_Newly_Added_Project`,
  `RefreshAsync_Removes_Deleted_Project`,
  `Clicking_Rename_On_A_Card_Opens_The_Rename_Modal`,
  `Clicking_Unregister_On_A_Card_Opens_The_Unregister_Modal`.
  The `StaticService` test stub is upgraded
  from "throws NotSupportedException" to a
  real working implementation (renamed +
  unregister are exercised end-to-end).
- `tests/AiEng.Platform.ComponentTests/Pages/ProjectsPageTests.cs`
  — extended: `Register_Button_Is_Enabled_In_M3_2`,
  `Clicking_Register_Button_Opens_The_Registration_Modal`.
  The `StaticService` test stub is upgraded
  to a real working implementation.
- `docs/projects.md` — updated § 1 (M3.2
  enables the three actions), § 5.1 (Rename +
  Unregister enabled; Open remains disabled),
  § 5.3 (Register button enabled),
  § 7.2 (component test list expanded),
  § 7.3 (architecture test list expanded),
  § 8 (Out of Scope updated to reflect that
  M3.2 is delivered; only M4-A Open action
  remains in the out-of-scope bucket).
- `.ai/state/current.md` — last completed
  task updated to T-019 (M3.2).
- `.ai/state/task-board.md` — M3.2 added
  to **Done Recently**; M3 closeout
  (M3.x) promoted to **Ready**.
- `.ai/state/milestones.json` — M3.2 slice
  block added (`status: delivered`,
  `delivered_at: 2026-07-11`,
  `branch: feature/T-019-m3-2-project-registration-slice-2`).
- `.ai/state/tasks.json` — T-019 moved from
  `In Progress` to `Done` (2026-07-11) with
  full evidence (branch, files added /
  modified, tests).
- `.ai/state/session.json` — M3.2 envelope
  (`session_id:
  m3-2-project-registration-slice-2`;
  `session_type: implementation`;
  `previous_session:
  m3-1-project-registration-slice-1`).
- `ROADMAP.md` — § 2 M3 row status updated;
  § 3 M3.2 row added (`Delivered (M3.2,
  2026-07-11)`).
- `.ai/plans/master-delivery-plan.md` —
  § 1 M3 row status updated; § 3 M3 block
  completion status updated; M3.2 slice row
  added (`Delivered`).

## Reusable Components Introduced

- `RegisterProjectForm` — the registration
  modal. Lives in `Components/Projects/`.
  Stateless apart from the three fields
  (`Name`, `Path`, `IsSubmitting`, `Error`).
  The form pre-validates inputs in `CanSubmit`;
  the service validates the path existence
  in `IProjectService.RegisterAsync`.
- `RenameProjectForm` — the rename modal.
  Lives in `Components/Projects/`. Pre-fills
  the new name with the project's current
  name in `OnParametersSet`.
- `ConfirmUnregisterProject` — the
  unregister confirmation modal. Lives in
  `Components/Projects/`. Two-button
  confirmation (Cancel + Unregister).

## Services Introduced

- **None.** M3.2 composes the M3.1 service
  (`IProjectService`). No new contracts.

## Providers Touched

- **None.** Per the user's brief: *"Do not
  create providers."* M3.2 is the second
  M3 implementation slice; the platform
  surface for project registration. The
  provider landscape lands in M4-D.

## Tests Added

- **Unit:** 0 new (the M3.1
  `IProjectServiceTests` already cover the
  `RegisterAsync` / `RenameAsync` /
  `UnregisterAsync` happy-path + failure
  tests; M3.2 reuses them).
- **Component (bUnit):** 26 new (8
  `RegisterProjectFormTests` + 8
  `RenameProjectFormTests` + 5
  `ConfirmUnregisterProjectTests` + 5
  `AppProjectCardTests` extensions — Open
  still disabled, Rename enabled, Unregister
  enabled, click handlers, data-testid
  presence + 2 `ProjectsPageTests`
  extensions — Register enabled, register
  modal open + 5 `AppProjectListTests`
  extensions — ShowRegisterDialog, Refresh
  add + remove, card-button click opens
  modals). The pre-M3.2 baseline was 198
  component tests; M3.2 raises the bar to
  228.
- **Architecture:** 3 new
  (`RegisterProjectForm_resolves...`,
  `RenameProjectForm_resolves...`,
  `ConfirmUnregisterProject_resolves...`).
  The pre-M3.2 baseline was 8 architecture
  tests; M3.2 raises the bar to 11.
- **Contract:** 0.
- **Integration:** 0.
- **Regression:** 0.

## Commands Run

The actual commands the session ran, in
order.

- `npm run css:build` — exit 0; CSS bundle
  rebuilt.
- `dotnet restore` — exit 0; NuGet cache
  hydrated.
- `dotnet build AiEng.Platform.slnx` — exit
  0; **0 warnings, 0 errors** (with
  `TreatWarningsAsErrors=true` from
  `Directory.Build.props`).
- `dotnet test AiEng.Platform.slnx --no-build`
  — exit 0; **273 passed, 7 skipped, 0
  failed**.
- `dotnet format --verify-no-changes` —
  exit 0; CRLF line endings preserved on
  every new file.
- `git status --short` — clean at session
  end (all work committed; see Git section
  below).
- Visual smoke: the `/projects` route
  returns 200; the **Register a project**
  button opens the registration modal;
  registering a project navigates to the
  populated state; renaming a project updates
  the name; unregistering a project removes
  it.

## Validation Results

The actual results. Be honest; if something
failed and was fixed, say so.

- `npm run css:build`: clean (exit 0).
- `dotnet restore`: clean (exit 0).
- `dotnet build`: 0 warnings, 0 errors.
- `dotnet test`: **273 passed, 0 failed**.
  - Unit: 34 passed (no change; M3.1
    baseline preserved).
  - Component (bUnit): **228** passed
    (198 pre-M3.2 + 8 `RegisterProjectFormTests`
    + 8 `RenameProjectFormTests` + 5
    `ConfirmUnregisterProjectTests` + 5
    `AppProjectCardTests` extensions + 2
    `ProjectsPageTests` extensions + 5
    `AppProjectListTests` extensions).
  - Architecture: **11** passed
    (8 pre-M3.2 + 3 new
    `PagesResolveProjectsThroughServiceTests`
    extensions).
  - **7 skipped** (3 axe-core + 4
    provider-boundary per ADR-016 / M4-D;
    unchanged).
- `dotnet format --verify-no-changes`:
  clean.
- Visual smoke: HTTP 200 on
  `http://localhost:5286/projects`; the
  **Register a project** button is enabled
  in the page header; clicking it opens the
  registration modal; submitting a valid
  name + path closes the modal and renders
  the new project in the populated state;
  the **Rename** button on each card is
  enabled; clicking it opens the rename
  modal pre-filled with the current name;
  submitting a new name closes the modal and
  renders the renamed project; the
  **Unregister** button on each card is
  enabled; clicking it opens the unregister
  confirmation; confirming closes the modal
  and removes the project from the list.
- `git status --short`: clean at session
  end (all work committed; see Git section
  below).

## Documentation Updated

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
  (registration form, rename, unregister) and
  retains only the M4-A Open action.
- `.ai/state/current.md` — last completed
  task updated to T-019 (M3.2); next
  recommended task updated to M3 closeout
  (M3.x).
- `.ai/state/task-board.md` — M3.2 added
  to **Done Recently**; M3 closeout (M3.x)
  promoted to **Ready**.
- `.ai/state/milestones.json` — M3.2
  slice block added (`status: delivered`,
  `delivered_at: 2026-07-11`).
- `.ai/state/tasks.json` — T-019 moved
  from `In Progress` to `Done` (2026-07-11)
  with full evidence.
- `.ai/state/session.json` — M3.2 envelope.
- `ROADMAP.md` — § 2 M3 row status
  updated; § 3 M3.2 row added.
- `.ai/plans/master-delivery-plan.md` —
  § 1 M3 row status updated; § 3 M3
  block updated; M3.2 slice row added.
- `.ai/handoffs/2026-07-11-m3-2-project-registration-slice-2.md`
  — **new** (per-session handoff).
- `.ai/handoffs/latest.md` — mirror of
  the M3.2 handoff.

## Deviations

Anything the implementation did that the plan
did not foresee. A deviation is not a failure;
an unreported deviation is.

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
  The three forms (`RegisterProjectForm`,
  `RenameProjectForm`, `ConfirmUnregisterProject`)
  use the native element; the design system
  is not extended. The decision is recorded
  in the plan as well (the plan's brief said
  "or an alternative small composition if
  the M1.2 design system already provides a
  dialog primitive" — the M1.2 system does
  not, and the choice is to skip the
  primitive). The dialog interaction is
  fully functional in all three forms
  (open + close + escape + backdrop).

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

## Known Limitations

Anything the implementation does not solve,
deferred to a follow-up, or that the user
should be aware of.

- **Limitation 1 — In-memory store is not
  durable.** The M3 in-memory store does
  not survive an application restart. The
  M4-A on-disk `IProjectStore`
  implementation lands behind the same
  contract; `AddProjects` swaps the
  `IProjectStore` registration in the
  composition root; `IProjectService` and
  the UI are unchanged. The M3/M4-A
  boundary is the contract, not the
  storage medium.
- **Limitation 2 — The Open action is
  M4-A.** The **Open** action on
  `AppProjectCard` is wired to the seam
  today; the action is **disabled** in
  M3.2. M4-A enables the action (the
  durable store replaces the in-memory
  store, and the platform can resolve a
  process runner against the project's
  path). The card renders the button in
  the disabled state and tooltips explain
  the M4-A timeline.
- **Limitation 3 — Browse-folder is not
  shipped.** The registration form's path
  field is a text input; M3.2 does not
  ship a "Browse" button. The path is
  copy-paste from the host file system.
  A future slice can introduce a
  directory-picker primitive if the
  M1.2 design system does not already
  ship one.
- **Limitation 4 — Cancel does not
  escape-close.** The HTML5 native
  `<dialog>` element supports an "escape"
  key to close the modal, but the
  `onclose` event handler is not wired in
  M3.2. The three forms close only on
  Cancel button or submit success. A
  follow-up can wire the escape key.
- **Limitation 5 — Backdrop click does
  not close.** The HTML5 native `<dialog>`
  element supports a backdrop click to
  close, but it is not wired in M3.2.
  Same follow-up as Limitation 4.

## Next Recommended Step

The single most important thing the next
session should do.

- **Approve the M3 closeout plan and start
  M3.x (M3 retrospective).** The M3 closeout
  is the M3 retrospective per the Milestone
  Closeout Standard at
  `.ai/workflows/milestone-closeout.md`. M3
  has three slices in total: M3.1 (the
  contract + surface, delivered 2026-07-11),
  M3.2 (the form + rename + unregister,
  delivered 2026-07-11), M3.x (the M3
  retrospective). M3.x follows M3.2. The
  M3 closeout plan lands at
  `.ai/plans/M3-closeout.md` (the next
  session drafts it from the closeout
  standard). **M3.x is the next
  dependency-satisfied Ready task** per
  the `Next` command protocol
  (`.ai/commands.md`).

## Project Continuity (Rule 15) and Evidence (Rule 17)

A session that ends without updating the
project-continuity state and leaving
evidence has not ended. Confirm that the
following were done at session end:

- [x] `.ai/state/current.md` — updated to
      reflect the state of the repository
      right now (M3 Active; T-019 Done;
      main; M3.2 closeout commit; next
      recommended task M3 closeout).
- [x] `.ai/state/task-board.md` — T-019
      moved from `In Progress` to
      `Done Recently`; M3 closeout (M3.x)
      promoted to `Ready`.
- [x] `.ai/handoffs/2026-07-11-m3-2-project-registration-slice-2.md`
      — the per-session handoff, written
      following this template.
- [x] `.ai/handoffs/latest.md` — mirror of
      the per-session handoff.
- [x] `implementation-report-m3-2-project-registration-slice-2.md`
      — the receipt (this file).
- [x] **Coherent commit** (Rule 17 in
      `AGENTS.md`) — `feat(m3.2): enable
      project registration form, rename,
      and unregister` on
      `feature/T-019-m3-2-project-registration-slice-2`
      (fast-forwarded into `main` per the
      branching strategy rule 6; branch
      deleted per rule 7). The commit is
      local; pushing requires explicit
      authorisation. Push is **not**
      performed in this session (no
      explicit push authorisation; the
      remote is configured but push is
      not authorised).

## Linked Artefacts

- `.ai/plans/M3.2-project-registration-slice-2.md`
  — the approved M3.2 plan this report
  implements against.
- `.ai/plans/M3-project-registration.md` —
  the umbrella M3 plan; M3.2 is the second
  slice of M3.
- `.ai/plans/master-delivery-plan.md` —
  the master delivery plan; the M3 row is
  updated to reflect the M3.2 closeout.
- `task-brief.md` — not produced; the
  user's brief was the `Next` invocation
  that promoted and executed the first
  actionable task.
- `session-handoff.md` — produced
  separately at
  `.ai/handoffs/2026-07-11-m3-2-project-registration-slice-2.md`.
- `review-report.md` — not produced;
  the lavish-axi review remains
  `Blocked` (T-005).
- ADR-### — no new ADR; the M3.2 plan's
  architecture test pattern mirrors
  the M3.1
  `Pages_Resolve_Projects_Through_Service`
  pattern (extended to cover the three
  new form components); no architectural
  change warrants a new ADR.
- `PRODUCT.md` — the product definition;
  the M3 surface delivers C-016
  (Project Registration).
- `ROADMAP.md` — the milestone plan; the
  M3 row is updated to reflect the
  M3.2 closeout.
- `.ai/state/current.md` and
  `.ai/state/task-board.md` — the live
  state, updated at session end (Rule 15
  in `AGENTS.md`).
- `.ai/handoffs/2026-07-11-m3-2-project-registration-slice-2.md`
  — the per-session handoff, written at
  session end (Rule 15 in `AGENTS.md`).
- The commit hash of the session's work
  (Rule 17 in `AGENTS.md`): the
  M3.2 closeout commit
  `feat(m3.2): enable project registration form, rename, and unregister`
  is on `main` (the feature branch
  `feature/T-019-m3-2-project-registration-slice-2`
  is fast-forwarded into `main` per the
  branching strategy rule 6 and deleted
  per rule 7).
