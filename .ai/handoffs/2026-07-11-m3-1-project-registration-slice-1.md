# Session Handoff — M3.1 — Project Registration Slice 1

> **Per-session handoff for the M3.1 implementation
> session, 2026-07-11.** M3.1 — Project
> Registration Slice 1 is **Delivered**. The M3
> milestone status moves from `Planned` to
> `Active`. The coherent commit
> `feat(m3.1): add project registration surface`
> is on
> `feature/m3-1-project-registration-slice-1`
> (now fast-forwarded into `main`; the feature
> branch is deleted per the branching strategy).

---

## 1. What was delivered

M3.1 ships the M3 surface end-to-end as a single
slice: the **contract**, the **in-memory store**,
the **domain entity**, the **composition root**,
the **UI surface** (`AppProjectCard`,
`AppProjectList`, `/projects` page), the
**architecture test** enforcing the single-seam
rule, the **unit + bUnit test coverage**, and the
**surface documentation**.

The M3 surface is a project is the smallest piece
of state the platform needs to be useful on its
own. A user registers a project by giving the
platform a **name** and a **folder path**; the
platform owns a `Project` entity in the
application layer; the `/projects` page lists
every registered project, sorted by name, in one
of four state slots (Loading, Empty, Error,
Populated). The sidebar entry **Projects** appears
in the registry between **Dashboard** and
**Design system** (per ADR-005 / M2 sidebar
ordering; `Order = 1`).

M3.1 is the **contract, the in-memory store, and
the smoke-test surface** for the M3 domain entity.
The durable on-disk store lands in M4-A behind the
same `IProjectStore` contract; the M3 in-memory
store is the smoke test for the contract.

The M3 plan is the approved plan for the M3
surface across multiple slices; M3.1 is the
**first slice** delivered here. M3.2 is the
registration form + rename + unregister; M3.3 is
the M3 retrospective per the Milestone Closeout
Standard.

## 2. Test and build status

- **Build:** 0 warnings, 0 errors.
- **Tests:** **240 passed, 0 failed, 7 skipped**
  (34 unit + 198 bUnit + 8 active architecture
  + 3 axe-core harness tests
  registered-but-disabled + 4 provider-boundary
  tests registered-but-disabled per ADR-016 /
  M4-D).
  - **+27 unit** (16 new in `IProjectServiceTests`,
    11 new in `InMemoryProjectStoreTests`).
  - **+13 bUnit** (5 new in `AppProjectCardTests`,
    4 new in `AppProjectListTests`, 4 new in
    `ProjectsPageTests`).
  - **+2 architecture** (`Pages_Resolve_Projects_Through_Service`
    enforces the single-seam rule on the page
    and the list).
- **Format:** clean (verified with
  `dotnet format --verify-no-changes`).
- **CSS:** `npm run css:build` exits 0.
- **Restore:** `dotnet restore` exits 0.
- **Visual smoke:** `curl http://localhost:5286/projects`
  returns HTTP 200; the expected markers are
  present (1 list slot, 1 empty state,
  1 `data-state="empty"`, 1 breadcrumb, 1 page
  header, 1 `/projects` in the sidebar). The
  disabled Register button is present in the page
  header's `Actions` slot.

The validation gate is the M3.1 closeout's
canonical evidence. Every gate in the
implementation report is green.

## 3. Deviations

The M3.1 implementation lands with three
documented deviations. The M3 plan is updated
to reflect the deviations in a follow-up.

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
  detail is the new shape.
- **Deviation 2 — `IClock` is realised
  through `TimeProvider`.** The plan
  referenced an `IClock` abstraction in
  passing. The implementation uses .NET 8+'s
  built-in `TimeProvider` (registered as a
  singleton in `AddProjects`) plus a
  `Common.IClock` interface (a single-method
  `UtcNow` abstraction) that wraps it. The
  unit tests inject a `FakeClock` that
  returns a fixed `DateTimeOffset`. The
  API surface is unchanged from the plan;
  the underlying primitive is .NET's
  `TimeProvider`, not a hand-rolled
  abstraction. The choice reduces bespoke
  code.
- **Deviation 3 — Disabled tests are
  unchanged.** The M3 plan named the M3
  surface as "the slice that does not
  activate the axe-core audit or the
  provider-boundary tests." The 7
  registered-but-disabled tests (3
  axe-core + 4 provider-boundary) remain
  skipped per ADR-016 / M4-D, as planned.

No other deviations.

## 4. Files added

### Source (domain + application + composition)

- `src/AiEng.Platform.Domain/Projects/Project.cs`
  — the `Project` aggregate root (immutable
  `Id`, human `Name`, absolute `Path`,
  immutable `CreatedAt`, mutable
  `LastUsedAt?` updated by `Touch(at)`).
- `src/AiEng.Platform.Application/Projects/IProjectStore.cs`
  — the persistence seam.
- `src/AiEng.Platform.Application/Projects/InMemoryProjectStore.cs`
  — the M3 in-memory store
  (`ConcurrentDictionary<Guid, Project>`);
  round-trips projects; returns a snapshot
  ordered by `Name` (OrdinalIgnoreCase).
- `src/AiEng.Platform.Application/Projects/IProjectService.cs`
  — the application-layer facade.
- `src/AiEng.Platform.Application/Projects/ProjectService.cs`
  — the `IProjectService` implementation.
- `src/AiEng.Platform.Application/Projects/Result.cs`
  — the `Result<T>` envelope + the
  `ValidationError` class.
- `src/AiEng.Platform.App/Composition/Projects/ProjectsServiceCollectionExtensions.cs`
  — `AddProjects` extension method.

### Source (UI surface)

- `src/AiEng.Platform.App/Components/Projects/_Imports.razor`
- `src/AiEng.Platform.App/Components/Projects/AppProjectCard.razor`
  + `.razor.css`
- `src/AiEng.Platform.App/Components/Projects/AppProjectList.razor`
  + `.razor.css`
- `src/AiEng.Platform.App/Components/Pages/Projects.razor`
  + `.razor.css`

### Tests

- `tests/AiEng.Platform.UnitTests/Projects/IProjectServiceTests.cs`
  — 16 unit tests.
- `tests/AiEng.Platform.UnitTests/Projects/InMemoryProjectStoreTests.cs`
  — 11 unit tests.
- `tests/AiEng.Platform.ComponentTests/Projects/AppProjectCardTests.cs`
  — 5 bUnit tests.
- `tests/AiEng.Platform.ComponentTests/Projects/AppProjectListTests.cs`
  — 4 bUnit tests (Loading, Empty, Error,
  Populated).
- `tests/AiEng.Platform.ComponentTests/Pages/ProjectsPageTests.cs`
  — 4 bUnit tests (page header, empty,
  populated, register disabled).
- `tests/AiEng.Platform.ArchitectureTests/Pages/PagesResolveProjectsThroughServiceTests.cs`
  — 2 architecture tests (page + list).

### Documentation

- `docs/projects.md` — the M3 product surface
  definition (9 sections).
- `implementation-report-m3-1-project-registration-slice-1.md`
  — this session's implementation report.
- `.ai/handoffs/2026-07-11-m3-1-project-registration-slice-1.md`
  — this file.
- `.ai/handoffs/latest.md` — the mirror of
  this file.

### State

- `.ai/plans/M3.2-project-registration-slice-2.md`
  — the M3.2 plan (Awaiting Approval; the
  next slice; the next session's first
  action is to approve and start).

## 5. Files modified (non-additive)

- `src/AiEng.Platform.App/Composition/ServiceCollectionExtensions.cs`
  — `AddPlatformServices` now also calls
  `services.AddProjects();` alongside
  `AddNavigation` and `AddProjectIntelligence`.
- `src/AiEng.Platform.Application/Projects/ProjectService.cs`
  — `RenameAsync` and `UnregisterAsync`
  use the new `ValidationError.NotFound`
  factory method (see Deviations).
- `.ai/state/session.json` — M3.1 envelope
  (session_id,
  `m3-1-project-registration-slice-1`;
  `session_type: implementation`;
  `previous_session:
  m2-6-m2-closeout-and-treehouse-dogfooding`).
- `.ai/state/tasks.json` — T-018 moved from
  `Ready` to `Done` with the full evidence
  block; T-019 (M3.2) promoted to `Ready`.
- `.ai/state/current.md` — active milestone
  promoted from M2 (closed 2026-07-11) to
  **M3** (Active, 2026-07-11); last
  completed task → `T-018`; active branch
  → `main` (after the fast-forward merge);
  last stable commit → the M3.1 closeout
  commit; active plan status →
  `M3.1: Delivered; M3.2: Awaiting Approval`;
  last implementation report → the M3.1
  report; next recommended task → `T-019`
  (M3.2); last updated → 2026-07-11.
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
- `ROADMAP.md` — § 2 M3 row status
  updated; § 3 M3.1 row added
  (`Delivered (M3.1, 2026-07-11)`).
- `.ai/plans/master-delivery-plan.md` —
  § 1 M3 row status updated; § 3 M3
  completion status updated; M3.1 slice
  row added (`Delivered`).
- `.ai/handoffs/latest.md` — mirror of
  the M3.1 handoff (overwrites the prior
  M2.6 mirror).

## 6. Files NOT touched (cross-cutting)

- `src/AiEng.Platform.Providers.Abstractions/` —
  **not** modified. M3.1 does not introduce
  providers (per the brief: "Do not create
  providers"); providers land in M4-D.
- `src/AiEng.Platform.App/Components/Shell/`,
  `Components/Layout/`, `Components/Navigation/`,
  `Components/Common/`, `Components/Primitive/`,
  `Components/Display/`, `Components/Feedback/`,
  `Components/Inputs/` — **not** modified
  beyond the `_Imports.razor` for the new
  `Components/Projects/` folder. M3.1 is
  additive to the M2 + M1 design-system
  surface.
- `src/AiEng.Platform.App/Layouts/`,
  `Components/Pages/Home.razor`,
  `Components/Pages/Counter.razor`,
  `Components/Pages/Weather.razor`,
  `Components/Pages/Error.razor`,
  `Components/Pages/NotFound.razor`,
  `Components/Pages/DesignSystem.razor`,
  `Components/Pages/Dashboard.razor` —
  **not** modified. M3.1 introduces the
  `Projects.razor` page; the existing M2
  pages are unchanged.
- `tests/AiEng.Platform.ArchitectureTests/Boundaries/`
  — **not** modified. The 7
  registered-but-disabled tests remain
  registered-but-disabled per ADR-016 /
  M4-D; the M3 surface does not activate
  them.
- `AGENTS.md`, `ARCHITECTURE.md`,
  `DECISIONS.md`, `STYLEGUIDE.md`,
  `CONTRIBUTING.md` — **not** modified. M3.1
  is a feature slice; no constitutional rule
  is added or removed.
- `.ai/state/project.json`,
  `.ai/state/providers.json`,
  `.ai/state/features.json`,
  `.ai/state/capabilities.json` — **not**
  modified. M3.1 delivers C-016 (Project
  Registration); the existing capability
  graph entry is the source of truth; the
  M3.1 closeout updates the M3 evidence
  block in `milestones.json` only.
- `package.json`, `tailwind.config.js`,
  `Directory.Build.props` — **not**
  modified. The CSS pipeline is unchanged;
  the M3 surface uses the existing design
  tokens.

## 7. Next action

The M3.1 implementation session stops here. M3.2
follows per the Progressive Coding Rule. The M3.2
plan is in `Awaiting Approval`; the next session
approves the M3.2 plan and starts the M3.2
implementation per the 13-step task lifecycle.

- **M3.2 plan approval.** The next session
  reads
  `.ai/plans/M3.2-project-registration-slice-2.md`
  and this handoff; the user issues an
  `Approve` command per the command protocol
  (`.ai/commands.md`); the M3.2 plan status
  moves from `Awaiting Approval` to
  `Approved`.
- **M3.2 implementation.** The next session
  after approval starts M3.2 per the
  Progressive Coding Rule: one task per
  session, 13-step lifecycle, stop after
  the coherent commit. T-019 moves from
  `Ready` to `In Progress`; the M3.2 plan
  is fleshed out per the M3.2 acceptance
  criteria; the M3.2 branch is created
  from `main` at the M3.1 closeout commit;
  the M3.2 work lands in a single
  coherent commit; the branch is
  fast-forwarded into `main` and deleted;
  the M3.2 implementation report is
  written; the M3.2 handoff is written
  and mirrored to `latest.md`.
- **M3.3 — M3 closeout + retrospective.**
  M3.3 is the M3 closeout slice (the M3
  retrospective per the Milestone Closeout
  Standard). M3.3 follows M3.2 per the
  Progressive Coding Rule.
- **Push is skipped.** No push is
  authorised in this session; the M3.1
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
  Milestone Closeout Standard; the M3.3
  closeout mirrors this standard).
- `.ai/plans/M3.2-project-registration-slice-2.md`
  (the M3.2 plan in `Awaiting Approval`).
- `retrospective-m2-application-shell-and-navigation.md`
  (the M2 retrospective; the M3 plan
  accounts for its § 13 recommendations).
- `docs/projects.md` (the M3 product
  surface definition).
- `ROADMAP.md` § 2 + § 3 (the M3 row
  updated; the M3.1 row in the M3 slice
  table).
- `.ai/plans/master-delivery-plan.md` § 1
  + § 3 (the M3 row updated; the M3.1
  slice row added).
- `.ai/state/milestones.json` (M3
  `Active`; M3.1 `delivered`; M3 evidence
  updated).
- `.ai/state/task-board.md` (M3.1 in
  `Done Recently`; T-019 M3.2 in
  `Ready`).
- This handoff
  (`.ai/handoffs/2026-07-11-m3-1-project-registration-slice-1.md`).
- The M3.1 implementation report
  (`implementation-report-m3-1-project-registration-slice-1.md`).
