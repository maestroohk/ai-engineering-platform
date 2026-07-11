# Implementation Report — M3.1 Project Registration Slice 1

> **M3 first implementation slice.** The M3
> surface — a project is the smallest piece of
> state the platform needs to be useful on its
> own — is delivered as a single coherent unit.
> The slice lands the contract, the in-memory
> store, the domain entity, the composition
> root, the UI surface, the architecture test,
> the unit + bUnit test coverage, and the
> surface documentation. The M3 closeout slice
> (M3.x — the M3 retrospective per the
> Milestone Closeout Standard) is the
> M3 retrospective deliverable.

---

## Plan Reference

- **Approved plan:** M3.1 — Project Registration
  Slice 1 (the first M3 implementation slice;
  one of multiple M3 slices, not the entire M3
  milestone)
- **Plan path:** `.ai/plans/M3-project-registration.md`
- **Deviations from plan:** see **Deviations**
  section below

The plan and the report are paired: the plan
is the contract, the report is the receipt.
The M3 plan defines the M3 surface across
multiple slices; M3.1 is the first slice
delivered here.

---

## Summary

M3.1 lands the smallest piece of state the
platform needs to be useful on its own: a
**project**. A user registers a project by
giving the platform a **name** and a **folder
path**; the platform owns a `Project` entity in
the application layer; the `/projects` page
lists every registered project, sorted by name,
in one of four state slots (Loading, Empty,
Error, Populated); the sidebar entry
**Projects** appears in the registry between
**Dashboard** and **Design system** (per
ADR-005 / M2 sidebar ordering).

M3.1 is the **contract, the in-memory store,
and the smoke-test surface** for the M3 domain
entity. The durable on-disk store lands in
M4-A behind the same `IProjectStore` contract;
the M3 in-memory store is the smoke test for
the contract. The architecture test
`Pages_Resolve_Projects_Through_Service`
enforces the single-seam rule: the `/projects`
page and the `AppProjectList` consume
`IProjectService` through the contract, never
through `InMemoryProjectStore` or the file
system directly.

The slice advances the M3 milestone (which
delivers C-016 — Project Registration) and
consumes C-019 (the M2 application shell) and
C-020 (the M1 design system).

## Files Created

### Domain

- `src/AiEng.Platform.Domain/Projects/Project.cs`
  — the `Project` aggregate root (immutable
  `Id`, human `Name`, absolute `Path`,
  immutable `CreatedAt`, mutable
  `LastUsedAt?` updated by `Touch(at)`).
  Constructor validates that `Id`, `Name`, and
  `Path` are non-empty (`ArgumentException` on
  any empty field).

### Application

- `src/AiEng.Platform.Application/Projects/IProjectStore.cs`
  — the persistence seam
  (`ListAsync`, `GetAsync`, `AddAsync`,
  `UpdateAsync`, `RemoveAsync`).
- `src/AiEng.Platform.Application/Projects/InMemoryProjectStore.cs`
  — the M3 in-memory store
  (`ConcurrentDictionary<Guid, Project>`);
  round-trips projects; returns a snapshot
  ordered by `Name` (OrdinalIgnoreCase) per
  the contract.
- `src/AiEng.Platform.Application/Projects/IProjectService.cs`
  — the application-layer facade
  (`RegisterAsync`, `ListAsync`, `GetAsync`,
  `RenameAsync`, `UnregisterAsync`). All write
  methods return `Result<Project>`; validation
  failures return
  `Result.Failure(ValidationError(...))`.
- `src/AiEng.Platform.Application/Projects/ProjectService.cs`
  — the `IProjectService` implementation. Wires
  the `IProjectStore`; validates inputs;
  produces the right `ValidationError` on each
  failure path (`required`, `invalid_path`,
  `not_found`).
- `src/AiEng.Platform.Application/Projects/Result.cs`
  — the `Result<T>` envelope + the
  `ValidationError` class (with factory
  methods `Required`, `InvalidPath`,
  `NotFound`).

### Composition

- `src/AiEng.Platform.App/Composition/Projects/ProjectsServiceCollectionExtensions.cs`
  — `AddProjects` extension method that
  registers `IProjectStore` →
  `InMemoryProjectStore` (singleton) and
  `IProjectService` → `ProjectService`
  (singleton). M4-A swaps the `IProjectStore`
  registration in the same composition root;
  `IProjectService` and the UI are unchanged.

### App — UI surface

- `src/AiEng.Platform.App/Components/Projects/_Imports.razor`
  — Razor imports for the `Components/Projects/`
  folder (Common, Primitive, Layout, Display,
  Feedback, Inputs, AiEng.Platform.Application.Projects).
- `src/AiEng.Platform.App/Components/Projects/AppProjectCard.razor`
  — presentational card; renders the
  `Project` parameter (name, path, created
  timestamp, last-used timestamp, status
  badge, three action buttons). Status badge
  is `New` (Neutral) when `LastUsedAt` is
  null; `Active` (Success) when `LastUsedAt`
  is present. The card does not own state.
- `src/AiEng.Platform.App/Components/Projects/AppProjectCard.razor.css`
  — scoped CSS (`.app-project-card-header`,
  `.app-project-card-title`,
  `.app-project-card-path`,
  `.app-project-card-meta`,
  `.app-project-card-actions`).
- `src/AiEng.Platform.App/Components/Projects/AppProjectList.razor`
  — data-owning list of `AppProjectCard`s
  with the four state slots per the design
  system rule (Loading, Empty, Error,
  Populated). Injects `IProjectService`
  through the contract; loads via
  `ProjectService.ListAsync()`; catches
  exceptions into the Error state.
- `src/AiEng.Platform.App/Components/Projects/AppProjectList.razor.css`
  — scoped CSS (state-slot selectors keyed
  on `data-state="..."`).
- `src/AiEng.Platform.App/Components/Pages/Projects.razor`
  — the `/projects` page. Composes
  `AppPageHeader` + `AppBreadcrumb` (M2.3) +
  `AppProjectList`. `[RouteMetadata]` with
  `Href = "/projects"`, `Order = 1`,
  `ShowInSidebar = true`, `Icon = "▢"`,
  per the M2.2 registry.
- `src/AiEng.Platform.App/Components/Pages/Projects.razor.css`
  — scoped CSS
  (`.app-projects-page-actions`).

### Tests

- `tests/AiEng.Platform.UnitTests/Projects/IProjectServiceTests.cs`
  — 16 unit tests covering validation
  (empty name, empty path, missing
  directory), success path, list/get,
  rename (validation, not found, success),
  unregister (not found, success),
  constructor null check, `Project` domain
  rules (empty id/name/path rejected,
  `Rename` rejects empty, `Touch` records
  time).
- `tests/AiEng.Platform.UnitTests/Projects/InMemoryProjectStoreTests.cs`
  — 11 unit tests covering round-trip,
  ordering by `Name` (OrdinalIgnoreCase),
  `GetAsync` found/missing, duplicate id
  throws, `UpdateAsync` replaces,
  `RemoveAsync` removes + no-op, null
  checks, concurrent adds (50 items).
- `tests/AiEng.Platform.ComponentTests/Projects/AppProjectCardTests.cs`
  — 5 bUnit tests covering primary render,
  every badge variant (New + Active), and
  every action button.
- `tests/AiEng.Platform.ComponentTests/Projects/AppProjectListTests.cs`
  — 4 bUnit tests covering every state
  slot (Loading, Empty, Error, Populated).
- `tests/AiEng.Platform.ComponentTests/Pages/ProjectsPageTests.cs`
  — 4 bUnit tests covering page header,
  empty state, populated state, and the
  disabled Register button. Uses
  `using ProjectsPage = …` to disambiguate
  from the `Projects` namespace.
- `tests/AiEng.Platform.ArchitectureTests/Pages/PagesResolveProjectsThroughServiceTests.cs`
  — 2 architecture tests enforcing the
  single-seam rule: the `/projects` page
  and the `AppProjectList` consume
  `IProjectService` through the contract,
  not through `InMemoryProjectStore` or the
  file system directly.

### Documentation

- `docs/projects.md` — the M3 product
  surface definition. 9 sections: Goals,
  Project Entity, Contract
  (`IProjectStore` + `IProjectService` +
  Validation Rules), M3/M4-A Boundary, UI
  Surface (`AppProjectCard`,
  `AppProjectList`, `/projects` page),
  Composition Root, Tests, Out of Scope
  (M3), Acceptance Criteria. The doc is the
  the M3 surface definition; the M3 plan
  is the source-of-truth roadmap entry.

## Files Modified

- `src/AiEng.Platform.App/Composition/ServiceCollectionExtensions.cs`
  — `AddPlatformServices` now also calls
  `services.AddProjects();` alongside
  `AddNavigation` and `AddProjectIntelligence`.
- `src/AiEng.Platform.Application/Projects/ProjectService.cs`
  — `RenameAsync` and `UnregisterAsync`
  use the new `ValidationError.NotFound`
  factory method (see Deviations).

## Reusable Components Introduced

- `AppProjectCard` — presentational
  container; composes `AppCard` + `AppStack` +
  `AppBadge` + `AppButton`. Lives in
  `Components/Projects/`. Variants: `New`
  badge (Neutral) and `Active` badge
  (Success) for the status slot. Three
  action buttons (Open, Rename, Unregister)
  are all disabled in M3.1; they are wired to
  the seam today. The card is a pure render
  of the `Project` parameter; no state.
- `AppProjectList` — data-owning list with
  the design-system four-slot rule
  (Loading, Empty, Error, Populated). Lives
  in `Components/Projects/`. Injects
  `IProjectService` through the contract.

## Services Introduced

- `IProjectStore` — methods: `ListAsync`,
  `GetAsync`, `AddAsync`, `UpdateAsync`,
  `RemoveAsync`. Lifetime: singleton. No
  external dependencies.
- `InMemoryProjectStore` — `IProjectStore`
  implementation backed by
  `ConcurrentDictionary<Guid, Project>`. The
  M3 smoke-test store; not durable.
- `IProjectService` — methods:
  `RegisterAsync`, `ListAsync`, `GetAsync`,
  `RenameAsync`, `UnregisterAsync`. Lifetime:
  singleton. Depends on `IProjectStore` +
  `IClock` (a clock abstraction; the
  `ProjectService` uses `TimeProvider` to
  obtain the current UTC time; the test
  doubles inject a controllable `IClock`).
- `ProjectService` — `IProjectService`
  implementation. Wires the `IProjectStore`;
  validates inputs; produces the right
  `ValidationError` on each failure path.

## Providers Touched

- **None.** Per the user's brief: *"Do not
  create providers."* M3.1 is the platform
  surface for project registration; providers
  land in M4-D. The M3/M4 boundary is the
  contract, not the storage medium.

## Tests Added

- **Unit:** 27 (16 in
  `IProjectServiceTests`, 11 in
  `InMemoryProjectStoreTests`).
- **Component (bUnit):** 13 (5 in
  `AppProjectCardTests`, 4 in
  `AppProjectListTests`, 4 in
  `ProjectsPageTests`).
- **Architecture:** 2 (in
  `PagesResolveProjectsThroughServiceTests`).
- **Contract:** 0 (the M3 surface does not
  introduce a new contract test framework;
  the existing `Pages_Resolve_Projects_Through_Service`
  test is the architecture-level contract
  test).
- **Integration:** 0 (M3.1 is unit +
  component + architecture; integration
  tests land with the durable store in M4-A).
- **Regression:** 0 (no regression tests;
  the M3 surface is new).

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
  — exit 0; **240 passed, 7 skipped, 0
  failed**.
- `dotnet format --verify-no-changes` —
  exit 0; CRLF line endings preserved on
  every new file.
- `git status --short` — clean at session
  end (all work committed; see Git section
  below).
- Visual smoke: `curl http://localhost:5286/projects`
  — HTTP 200; `/projects` route renders the
  empty state with the breadcrumb, the page
  header, the disabled Register button, and
  the sidebar entry.

## Validation Results

The actual results. Be honest; if something
failed and was fixed, say so.

- `npm run css:build`: clean (exit 0).
- `dotnet restore`: clean (exit 0).
- `dotnet build`: 0 warnings, 0 errors.
- `dotnet test`: **240 passed, 0 failed**.
  - Unit: 34 passed (7 existing + 16 new
    `IProjectServiceTests` + 11 new
    `InMemoryProjectStoreTests`).
  - Component (bUnit): 198 passed
    (185 existing + 5 new
    `AppProjectCardTests` + 4 new
    `AppProjectListTests` + 4 new
    `ProjectsPageTests`).
  - Architecture: 8 passed (6 existing + 2
    new in
    `PagesResolveProjectsThroughServiceTests`).
  - **7 skipped** (3 axe-core + 4
    provider-boundary per ADR-016 / M4-D).
- `dotnet format --verify-no-changes`:
  clean.
- Visual smoke: HTTP 200 on
  `http://localhost:5286/projects`; the
  expected markers are present (1 list slot,
  1 empty state, 1 `data-state="empty"`, 1
  breadcrumb, 1 page header, 1 `/projects`
  in the sidebar). The disabled Register
  button is present in the page header's
  `Actions` slot.
- `git status --short`: clean at session
  end (all work committed; see Git section
  below).

## Documentation Updated

- `docs/projects.md` — **new** (the M3
  product surface definition; the canonical
  reference for the M3 contract, the UI
  surface, the composition root, the tests,
  and the M3/M4-A boundary).
- `.ai/state/current.md` — active milestone
  promoted from M2 (closed 2026-07-11) to
  **M3** (Active, 2026-07-11). Last
  completed task T-018 (M3.1) added. Last
  stable commit updated. Next recommended
  task updated.
- `.ai/state/task-board.md` — M3.1 added
  to **Done Recently**; M3.2 (next M3
  slice) promoted to **Ready**; the M3
  Deferred entry removed.
- `.ai/state/milestones.json` — M3 status
  flipped from `Planned` to `Active`
  (2026-07-11); M3.1 slice block added
  (`status: delivered`,
  `delivered_at: 2026-07-11`); M3 evidence
  block updated with the M3.1 report +
  handoff + commit.
- `.ai/state/tasks.json` — T-018 moved
  from `Ready` to `Done` (2026-07-11) with
  full evidence (branch, files added /
  modified, tests, notes). T-019 (M3.2)
  promoted to `Ready` (created and
  expanded from the M3 plan).
- `.ai/state/session.json` — M3.1
  envelope (session_id,
  `m3-1-project-registration-slice-1`;
  `session_type: implementation`;
  `previous_session:
  m2-6-m2-closeout-and-treehouse-dogfooding`).
- `ROADMAP.md` — § 2 M3 row status
  updated; § 3 M3.1 row added (`Delivered
  (M3.1, 2026-07-11)`).
- `.ai/plans/master-delivery-plan.md` —
  § 1 M3 row status updated; § 3 M3
  completion status updated; M3.1 slice
  row added (`Delivered`).
- `.ai/handoffs/2026-07-11-m3-1-project-registration-slice-1.md`
  — **new** (per-session handoff).
- `.ai/handoffs/latest.md` — mirror of the
  M3.1 handoff.

## Deviations

Anything the implementation did that the plan
did not foresee. A deviation is not a failure;
an unreported deviation is.

- **Deviation 1 — `ValidationError` is a
  class, not a struct.** The plan described
  `ValidationError` as a small record type
  (Code + Message). The first implementation
  used `readonly record struct ValidationError(string Code, string Message)`,
  but the combination with `T?` semantics on
  the `Result<T>.Error` slot triggered
  `error CS1061: 'ValidationError?' does
  not contain a definition for 'Code'`
  (the compiler interprets
  `ValidationError?` as
  `Nullable<ValidationError>`, not as a
  nullable reference annotation; the
  `Nullable<T>` value type does not have a
  `Code` property). The fix is
  `sealed class ValidationError` with
  private constructor + public factory
  methods (`Required`, `InvalidPath`,
  `NotFound`). The API surface is unchanged
  (the public factory methods are the only
  construction path); the implementation
  detail is the new shape. The M3 plan is
  updated to reflect the
  class-with-factories shape in a
  follow-up.

- **Deviation 2 — `IClock` is realised
  through `TimeProvider`.** The plan
  referenced an `IClock` abstraction in
  passing (the M3.1 acceptance criteria
  say "the service records `CreatedAt`
  from a controllable clock"). The
  implementation uses .NET 8+'s built-in
  `TimeProvider` (registered as a
  singleton in `AddProjects`) and the
  `IClock` interface from
  `AiEng.Platform.Application.Common`
  (a single-method `UtcNow`
  abstraction). The unit tests inject a
  `FakeClock` that returns a fixed
  `DateTimeOffset`. The API surface is
  unchanged from the plan; the underlying
  primitive is .NET's `TimeProvider`,
  not a hand-rolled abstraction. The
  choice reduces bespoke code.

- **Deviation 3 — Disabled tests are
  unchanged.** The M3 plan named the M3
  surface as "the slice that does not
  activate the axe-core audit or the
  provider-boundary tests." The 7
  registered-but-disabled tests (3
  axe-core + 4 provider-boundary) remain
  skipped per ADR-016 / M4-D, as planned.

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
- **Limitation 2 — The registration form
  is M3.2.** The page header's
  **Register a project** action button is
  wired to the seam today; the action is
  **disabled** in M3.1 (per the plan § 5.3
  and the M3 slice breakdown). M3.2
  enables the form: a modal or a side
  panel with name + path inputs, a
  browse-folder button, validation
  feedback, and a submit handler that
  calls `ProjectService.RegisterAsync`.
- **Limitation 3 — The rename and
  unregister actions are M3.2.** The card
  buttons are wired to the seam today;
  the actions are **disabled** in M3.1.
  M3.2 enables both. The card renders
  the buttons in the disabled state and
  tooltips explain the M3.2 timeline.
- **Limitation 4 — The open action is
  M4-A.** The **Open** action on
  `AppProjectCard` is wired to the seam
  today; the action is **disabled** in
  M3.1. M4-A enables the action (the
  durable store replaces the in-memory
  store, and the platform can resolve a
  process runner against the project's
  path). The card renders the button in
  the disabled state and tooltips explain
  the M4-A timeline.
- **Limitation 5 — The M3 plan's
  `ValidationError` shape is a
  follow-up.** The plan described
  `ValidationError` as a small record
  type; the implementation uses a class
  with factory methods (see Deviations).
  A follow-up updates the plan's wording
  to match. The API surface is unchanged.

## Next Recommended Step

The single most important thing the next
session should do. If the work is complete,
this is "release M3" or "close the
milestone". If the work is paused, this is
the exact command or file the next session
should open.

- **Approve the M3.2 plan and start
  M3.2.** The M3.2 plan (registration
  form + rename + unregister actions on
  the project list) is at
  `.ai/plans/M3.2-project-registration-slice-2.md`
  (`Status: Awaiting Approval`, prepared
  in this session). The next session
  approves the M3.2 plan and starts the
  M3.2 implementation per the Progressive
  Coding Rule
  (`.ai/workflows/progressive-coding.md`).
  **M3.2 is the next dependency-satisfied
  Ready task** (T-019) per the
  `Next` command protocol
  (`.ai/commands.md`).
- The M3 closeout slice is the M3
  retrospective per the Milestone
  Closeout Standard at
  `.ai/workflows/milestone-closeout.md`.
  M3 has three slices in total: M3.1 (the
  contract + surface), M3.2 (the form +
  rename + unregister), M3.3 (the M3
  retrospective). M3.3 follows M3.2.

## Project Continuity (Rule 15) and Evidence (Rule 17)

A session that ends without updating the
project-continuity state and leaving
evidence has not ended. Confirm that the
following were done at session end:

- [x] `.ai/state/current.md` — updated to
      reflect the state of the repository
      right now (M3 Active; T-018 Done;
      main; M3.1 closeout commit; next
      recommended task T-019 M3.2).
- [x] `.ai/state/task-board.md` — T-018
      moved from `Ready` to `Done Recently`;
      T-019 (M3.2) promoted to `Ready`.
- [x] `.ai/handoffs/2026-07-11-m3-1-project-registration-slice-1.md`
      — the per-session handoff, written
      following this template.
- [x] `.ai/handoffs/latest.md` — mirror of
      the per-session handoff.
- [x] `implementation-report-m3-1-project-registration-slice-1.md`
      — the receipt (this file).
- [x] **Coherent commit** (Rule 17 in
      `AGENTS.md`) — `feat(m3.1): add
      project registration surface` on
      `feature/m3-1-project-registration-slice-1`
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

- `.ai/plans/M3-project-registration.md` —
  the approved M3 plan this report
  implements against (the M3 plan
  defines the M3 surface; M3.1 is the
  first slice of M3).
- `.ai/plans/M3.2-project-registration-slice-2.md`
  — the M3.2 plan (Awaiting Approval; the
  next slice; the next session approves
  and starts).
- `.ai/plans/master-delivery-plan.md` —
  the master delivery plan; the M3 row is
  updated to reflect the M3.1 closeout.
- `task-brief.md` — not produced; the
  user's brief was the next-session
  continuation note that promoted and
  executed the first actionable task.
- `session-handoff.md` — produced
  separately at
  `.ai/handoffs/2026-07-11-m3-1-project-registration-slice-1.md`.
- `review-report.md` — not produced;
  the lavish-axi review remains
  `Blocked` (T-005).
- ADR-### — no new ADR; the M3 plan's
  architecture test pattern mirrors
  the M2.4
  `Pages_Resolve_State_Through_Reader`
  pattern; no architectural change
  warrants a new ADR.
- `PRODUCT.md` — the product definition;
  the M3 surface delivers C-016
  (Project Registration).
- `ROADMAP.md` — the milestone plan; the
  M3 row is updated to reflect the
  M3.1 closeout.
- `.ai/state/current.md` and
  `.ai/state/task-board.md` — the live
  state, updated at session end (Rule 15
  in `AGENTS.md`).
- `.ai/handoffs/2026-07-11-m3-1-project-registration-slice-1.md`
  — the per-session handoff, written at
  session end (Rule 15 in `AGENTS.md`).
- The commit hash of the session's work
  (Rule 17 in `AGENTS.md`): the
  M3.1 closeout commit
  `feat(m3.1): add project registration surface`
  is on `main` (the feature branch
  `feature/m3-1-project-registration-slice-1`
  is fast-forwarded into `main` per the
  branching strategy rule 6 and deleted
  per rule 7).
