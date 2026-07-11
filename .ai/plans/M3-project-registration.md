# M3 — Project Registration

> **The M3 plan.** M3 introduces the smallest
> piece of state the platform needs to be useful
> on its own: a registered project. A user can
> register a project (name + folder path) and the
> platform owns a `Project` entity in the
> application layer. The M3 in-memory store is
> the smoke test for the `IProjectStore` contract;
> durable storage is the M4-A deliverable. M3
> composes the M2 shell (sidebar, top bar, breadcrumb,
> page header) and the M2.4 project intelligence
> reader; M3 does not introduce a new shell surface.
>
> **Status:** Awaiting Approval (2026-07-11;
> produced by the M2.6 closeout per
> `.ai/workflows/milestone-closeout.md` § 8).
>
> **Branch:** (the M3.1 branch is created from
> `main` at the M2 closeout commit when M3.1
> starts; the branch is named
> `feature/T-018-m3-1-project-registration-slice-1`
> per the branching strategy rule 4).

---

## 1. Why This Milestone Exists

M3 introduces the smallest piece of state the
platform needs to be useful on its own: a
**registered project**. A registered project is
the input to every later milestone (M4 process
runner, M5 worktree, M6 launch, M7 review, M8
orchestration). Without project registration,
the platform is a configuration tool; with it,
the platform is an engineering tool.

M3 ships the **contract** (`IProjectService`,
`IProjectStore`), the **in-memory
implementation** of the store (the smoke test
for the contract; not durable across restart),
and the **UI surface** (`AppProjectCard`,
`AppProjectList`, the project-registration
page, the projects list). The M3 surface
composes the M2 shell: the page lives at
`/projects`, the sidebar item is added through
the M2.2 `INavigationRegistry`, the page
header is `AppPageHeader` + `AppBreadcrumb` (M2.3).

M3 does not implement providers, processes,
worktrees, runs, reviews, or quality gates.
Those are M4, M5, M6, M7, M8 respectively.
M3 does not introduce `AiEng.Platform.Infrastructure`
(M4-A) or the on-disk `IProjectStore` (M4-A).
M3 is the **smallest milestone that makes the
platform useful**.

---

## 2. In Scope

1. **Domain model.** A `Project` entity
   (`src/AiEng.Platform.Domain/Projects/Project.cs`)
   with `Id` (Guid), `Name` (string), `Path`
   (absolute path on disk), `CreatedAt`
   (DateTimeOffset), `LastUsedAt` (DateTimeOffset,
   nullable). The `Project` is a pure domain type;
   it has no dependencies on `App` or `Application`.
2. **Application layer contracts.**
   - `IProjectStore` in
     `src/AiEng.Platform.Application/Projects/IProjectStore.cs`
     — the persistence seam. Methods:
     `ListAsync(CancellationToken)`,
     `GetAsync(Guid id, CancellationToken)`,
     `AddAsync(Project, CancellationToken)`,
     `UpdateAsync(Project, CancellationToken)`,
     `RemoveAsync(Guid id, CancellationToken)`.
   - `IProjectService` in
     `src/AiEng.Platform.Application/Projects/IProjectService.cs`
     — the application-layer facade. Methods:
     `RegisterAsync(string name, string path,
     CancellationToken)`, `ListAsync(...)`,
     `GetAsync(...)`, `RenameAsync(...)`,
     `UnregisterAsync(...)`. The service
     validates input (name non-empty, path
     non-empty, path is an existing directory
     on the host file system) and returns
     `Result<Project>` (the unified
     `Result<T>` envelope from the application
     layer) on success or
     `Result<Project>.Failure(ValidationError)`
     on validation failure.
3. **In-memory store implementation.**
   - `InMemoryProjectStore` in
     `src/AiEng.Platform.Application/Projects/InMemoryProjectStore.cs`
     — a `ConcurrentDictionary<Guid, Project>`
     implementation of `IProjectStore`. The
     store is **not durable**: projects do
     not survive an application restart. The
     store is the smoke test for the contract;
     M4-A replaces it with the on-disk
     implementation behind the same contract.
4. **Composition root.**
   - `ProjectsServiceCollectionExtensions.AddProjects`
     in
     `src/AiEng.Platform.App/Composition/Projects/ProjectsServiceCollectionExtensions.cs`
     — registers `IProjectStore` (the in-memory
     implementation) and `IProjectService` as
     singletons. The extension is called from
     `ServiceCollectionExtensions.AddPlatformServices`
     (the M2.1 composition root) per the M2
     pattern. The on-disk implementation in
     M4-A swaps the `IProjectStore`
     registration in the same composition
     root; `IProjectService` and the UI are
     unchanged.
5. **UI surface (composes the M2 shell).**
   - `AppProjectCard` in
     `src/AiEng.Platform.App/Components/Projects/AppProjectCard.razor`
     — composes `AppCard` + `AppStack` +
     `AppBadge` (for the `LastUsedAt` status)
     + `AppButton` (for the actions: "Open",
     "Rename", "Unregister"). The card is
     a presentational container (no four
     state slots; the four-state rule is for
     data-owning components per the design
     system).
   - `AppProjectList` in
     `src/AiEng.Platform.App/Components/Projects/AppProjectList.razor`
     — a data-owning list of `AppProjectCard`s.
     The list exposes the four state slots
     (`Loading`, `Empty`, `Error`, `Populated`)
     per the design system rule. The list
     consumes `IProjectService` through
     constructor injection; the architecture
     test `Pages_Resolve_Projects_Through_Service`
     (this milestone) enforces the
     single-seam rule.
   - The `/projects` page at
     `src/AiEng.Platform.App/Components/Pages/Projects.razor`
     — composes `AppPageHeader` +
     `AppBreadcrumb` (M2.3) + `AppProjectList`.
     The page is a data-owning page; it
     exposes the four state slots in
     `AppPageHeader.Content`. The page
     consumes `IProjectService` through
     constructor injection.
   - The project-registration surface: a
     dialog (or inline form) that the user
     invokes to register a new project. The
     dialog composes `AppCard` + `AppStack` +
     `AppButton` + `AppInput` (M1.2). The
     dialog is a child of the `/projects`
     page; the page is the routed entry
     point. The dialog is an `AppDialog`
     composition; the M3.1 slice introduces
     `AppDialog` if missing, or composes
     `AppCard` + `AppStack` + a click
     handler if the M2.1 plan's `AppDialog`
     deferral is resolved (per the M2
     retrospective, the `AppDialog` component
     is not yet implemented; the M3.1 slice
     either introduces `AppDialog` per the
     M2.1 plan or uses the `AppCard` +
     `AppStack` composition per the
     simpler path).
   - Sidebar entry: `[RouteMetadata]` on
     `Projects.razor` adds the
     `/projects` route to the M2.2
     `INavigationRegistry`. The sidebar
     entry appears between `Dashboard` and
     `Design system` per the M2 sidebar
     ordering.
6. **Architecture tests.**
   - `Pages_Resolve_Projects_Through_Service`
     in
     `tests/AiEng.Platform.ArchitectureTests/Pages/PagesResolveProjectsThroughServiceTests.cs`
     — asserts the `/projects` page
     consumes `IProjectService` through the
     contract, not through direct access to
     `InMemoryProjectStore`. The test fails
     the build if a page imports
     `InMemoryProjectStore` directly.
7. **Unit + component tests.**
   - Unit tests for `IProjectService`
     (validation, the in-memory store
     round-trip, the success + failure
     paths).
   - Unit tests for `InMemoryProjectStore`
     (the contract round-trip; the
     concurrency safety).
   - bUnit tests for `AppProjectCard`
     (primary render, every variant: "Open",
     "Rename", "Unregister" actions).
   - bUnit tests for `AppProjectList`
     (every state slot: `Loading`, `Empty`,
     `Error`, `Populated`).
   - bUnit tests for the `/projects` page
     (state slots; the breadcrumb; the
     page header).
   - The registered-but-disabled tests
     from M1 / M2.5 remain disabled per
     ADR-016 / M4-D (axe-core; provider-
     boundary).
8. **M3.1 implementation report +
   per-session handoff + state updates.** The
   M3.1 work follows the M1 / M2.6 closeout
   pattern: one coherent commit per slice;
   the implementation report at
   `implementation-report-m3-1-project-registration-slice-1.md`;
   the per-session handoff at
   `.ai/handoffs/2026-07-11-m3-1-project-registration-slice-1.md`;
   the state updates per Rule 15.
9. **M3 closeout (M3.x).** The M3 closeout
   slice produces the M3 retrospective at
   `retrospective-m3-project-registration.md`
   per the Milestone Closeout Standard
   (`.ai/workflows/milestone-closeout.md`).
   The M3 milestone tag (`m3`) is at the M3
   closeout commit on `main`. The M4 plan is
   the M3 closeout's deliverable.

---

## 3. Out of Scope

- **M4 implementation.** The M3 closeout
  produces the M4 plan in
  `.ai/plans/M4-A-infrastructure-process-execution.md`
  (Status: Awaiting Approval); M4 is the next
  milestone after M3 closes.
- **Durable storage.** The M3 in-memory
  store is the smoke test for the contract;
  the on-disk `IProjectStore` is M4-A's
  responsibility. The M3 / M4-A boundary is
  the contract, not the storage medium.
- **Provider implementation projects
  (`AiEng.Platform.Providers.<X>`).** M3
  does not introduce a single
  `Providers.<X>` project; M3 ships no
  provider. The first provider lands in M4-D.
- **Process execution.** M3 does not call
  `Process.Start`; M3 is a pure
  in-process state-management milestone. M4-A
  introduces `IProcessRunner` and the
  process boundary.
- **Worktree creation, agent launching,
  review, quality gates, autonomous loops,
  orchestration.** M5, M6, M7, M8. M3 is
  the input to all of them.
- **The Tailwind content path change.** The
  M2.1 content path already includes
  `Layouts/**` and `Components/**`; the M3
  components are under `Components/Projects/`
  and are already covered.
- **The axe-core audit activation.** The
  M2.5 `AxeCoreAuditTests` remain
  registered-but-disabled per ADR-016 /
  M4-D. The M3 plan does not activate the
  audit; M4-D is the activation milestone.
- **The provider-boundary tests
  activation.** Same — M4-D.

---

## 4. Acceptance Criteria

M3.1 (the first M3 slice) is **Done** when
all of the following are true:

- [ ] `Project` domain entity at
      `src/AiEng.Platform.Domain/Projects/Project.cs`.
- [ ] `IProjectStore` and `IProjectService`
      contracts at
      `src/AiEng.Platform.Application/Projects/`.
- [ ] `InMemoryProjectStore` in-memory
      implementation.
- [ ] `ProjectsServiceCollectionExtensions.AddProjects`
      composition root extension.
- [ ] `AppProjectCard` and `AppProjectList`
      components at
      `src/AiEng.Platform.App/Components/Projects/`.
- [ ] `/projects` page at
      `src/AiEng.Platform.App/Components/Pages/Projects.razor`.
- [ ] `[RouteMetadata]` applied to
      `Projects.razor`; the sidebar entry
      appears in the M2.2 registry.
- [ ] `Pages_Resolve_Projects_Through_Service`
      architecture test is active and green.
- [ ] Unit tests for `IProjectService` and
      `InMemoryProjectStore` pass.
- [ ] bUnit tests for `AppProjectCard`,
      `AppProjectList`, and the `/projects`
      page pass.
- [ ] `npm run css:build` exits 0.
- [ ] `dotnet restore` exits 0.
- [ ] `dotnet build` exits 0 with 0
      warnings, 0 errors.
- [ ] `dotnet test` reports all active
      tests passing.
- [ ] `dotnet format --verify-no-changes`
      exits 0.
- [ ] Visual smoke: the `/projects` route
      returns 200; the page renders the
      "Empty" state when no projects are
      registered; the page renders the
      "Populated" state when at least one
      project is registered.
- [ ] The M3.1 coherent commit
      (`feat(m3.1): add project registration
      surface`) is on the feature branch.
- [ ] The M3.1 branch is fast-forwarded
      into `main` per the branching strategy.
- [ ] The M3.1 feature branch is deleted
      per the branching strategy rule 7.
- [ ] The M3.1 implementation report at
      `implementation-report-m3-1-project-registration-slice-1.md`.
- [ ] The M3.1 handoff at
      `.ai/handoffs/2026-07-11-m3-1-project-registration-slice-1.md`
      (mirrored to `latest.md`).
- [ ] The state files updated per Rule 15.
- [ ] No push (push is not authorised in
      the M3.1 session by default).

M3 is **closed** when the M3.x slices are all
delivered, the M3 retrospective is written,
and the M3 milestone tag (`m3`) is at the M3
closeout commit on `main`.

---

## 5. Files to Add (M3.1)

### Source

- `src/AiEng.Platform.Domain/Projects/Project.cs`
- `src/AiEng.Platform.Application/Projects/IProjectStore.cs`
- `src/AiEng.Platform.Application/Projects/IProjectService.cs`
- `src/AiEng.Platform.Application/Projects/Result.cs`
  (the unified `Result<T>` envelope; this
  may be already in the application layer
  per the M0.5 refinement — the M3.1
  slice either reuses the existing type or
  introduces it as a small addition)
- `src/AiEng.Platform.Application/Projects/InMemoryProjectStore.cs`
- `src/AiEng.Platform.App/Composition/Projects/ProjectsServiceCollectionExtensions.cs`
- `src/AiEng.Platform.App/Components/Projects/AppProjectCard.razor`
- `src/AiEng.Platform.App/Components/Projects/AppProjectCard.razor.css`
- `src/AiEng.Platform.App/Components/Projects/AppProjectList.razor`
- `src/AiEng.Platform.App/Components/Projects/AppProjectList.razor.css`
- `src/AiEng.Platform.App/Components/Projects/_Imports.razor`
- `src/AiEng.Platform.App/Components/Pages/Projects.razor`
- `src/AiEng.Platform.App/Components/Pages/Projects.razor.css`

### Tests

- `tests/AiEng.Platform.UnitTests/Projects/IProjectServiceTests.cs`
- `tests/AiEng.Platform.UnitTests/Projects/InMemoryProjectStoreTests.cs`
- `tests/AiEng.Platform.ComponentTests/Projects/AppProjectCardTests.cs`
- `tests/AiEng.Platform.ComponentTests/Projects/AppProjectListTests.cs`
- `tests/AiEng.Platform.ComponentTests/Pages/ProjectsPageTests.cs`
- `tests/AiEng.Platform.ArchitectureTests/Pages/PagesResolveProjectsThroughServiceTests.cs`

### Documentation

- `docs/projects.md` — the M3 product
  surface definition (the M3 page
  contract; the registration flow; the
  validation rules; the M3 / M4-A
  boundary).
- `implementation-report-m3-1-project-registration-slice-1.md`
  — the M3.1 closeout receipt.
- `.ai/handoffs/2026-07-11-m3-1-project-registration-slice-1.md`
  — the M3.1 per-session handoff.

### State

- `.ai/state/session.json` — the M3.1
  closeout envelope.
- `.ai/state/tasks.json` — T-018
  `Ready` → `In Progress` → `Done`.
- `.ai/state/current.md` — active
  milestone `M3`; last completed task
  `T-018`; active branch `main` (after
  merge); active plan status `M3.1
  Delivered`; next recommended task
  `M3.2` (the next M3 slice).
- `.ai/state/task-board.md` — M3.1 in
  `Done Recently`; M3.2 promoted to
  `Ready`.
- `.ai/state/milestones.json` — M3.1
  slice from `planned` to `delivered`;
  M3 evidence block updated.
- `ROADMAP.md` § 2 (M3 row status
  updated) and § 3 (M3.1 row status
  updated; M3 DoD bullet expanded).
- `.ai/plans/master-delivery-plan.md`
  § 1 (M3 row status updated) and § 3
  (M3 completion status updated;
  M3.1 slice row `Delivered`).

---

## 6. Critical Files to Read Before Editing

- `AGENTS.md` — the 17 non-negotiable
  rules; specifically Rule 13 (no code
  comments) and Rule 15 (project-
  continuity state).
- `.ai/workflows/feature-lifecycle.md`
  — the per-feature workflow.
- `.ai/workflows/branching-strategy.md`
  — the branching and merging rules.
- `.ai/workflows/milestone-closeout.md`
  — the Milestone Closeout Standard
  (the M3 closeout mirrors the M2.6
  closeout per this standard).
- `ROADMAP.md` § 2 + § 3 — the M3
  block.
- `.ai/plans/master-delivery-plan.md`
  § 1 + § 3 — the M3 block.
- `.ai/state/milestones.json` — the M3
  milestone block; the M3.1 evidence.
- `retrospective-m2-application-shell-and-navigation.md`
  — the M2 retrospective; specifically
  § 13 (Recommendations for the Next
  Milestone) — the M3 plan accounts for
  these recommendations.

---

## 7. Existing Functions / Utilities to Reuse

- The **M2.2 `INavigationRegistry`** is the
  single seam for adding the `/projects`
  sidebar entry; the M3.1 slice adds the
  `[RouteMetadata]` attribute to
  `Projects.razor` and the registry picks
  it up automatically.
- The **M2.3 `AppPageHeader` +
  `AppBreadcrumb`** is the page header
  integration; the M3.1 slice composes
  both on `Projects.razor`.
- The **M1.2 design system** (19 components)
  is the substrate; the M3.1 slice composes
  `AppCard` + `AppStack` + `AppBadge` +
  `AppButton` + `AppInput` + `AppEmptyState`
  + `AppLoading` + `AppErrorState`.
- The **M2.4 `IProjectIntelligenceReader`**
  is the read-side seam for `.ai/state/*.json`;
  the M3.1 slice does not consume the
  reader (the M3 surface is its own state,
  not the project intelligence snapshot).
  The two readers are separate; M3 may
  extend the snapshot to include the
  project list in a future slice.
- The **`Pages_Resolve_State_Through_Reader`**
  architecture test (M2.4) is the existing
  single-seam pattern; the M3.1 slice
  introduces the analogous
  `Pages_Resolve_Projects_Through_Service`
  test for the M3 surface.
- The **M1 / M2.6 closeout templates** are
  the M3.1 implementation report + handoff
  templates; the M3.1 work mirrors the
  per-slice evidence block pattern.

---

## 8. Risks and Mitigations

- **Risk: the M3.1 slice is too large to
  execute in one session.** Mitigation:
  the M3.1 work is bounded by the
  acceptance criteria; the components are
  small and the tests are mechanical.
  M3.2, M3.3, M3.x (the M3 closeout) are
  separate slices per the Progressive
  Coding Rule.
- **Risk: the M3 in-memory store is mistaken
  for the durable store.** Mitigation: the
  M3 in-memory store is documented as
  non-durable in the implementation report,
  the handoff, the `IProjectStore` XML doc,
  and the `M3` `ROADMAP.md` § 3 DoD bullet.
  The M3 / M4-A boundary is the contract,
  not the storage medium.
- **Risk: the M3 surface bypasses the M2.2
  `INavigationRegistry`.** Mitigation: the
  `Pages_Resolve_Projects_Through_Service`
  architecture test enforces the
  single-seam rule; the `[RouteMetadata]`
  on `Projects.razor` is the registry
  registration; the M2.2
  `Pages_AreReachable_Through_Registry`
  test (M2.2) already enforces the
  registration contract.
- **Risk: the M3 surface re-implements a
  component that the M1 design system
  already provides.** Mitigation: the M3
  acceptance criteria require the surface
  to compose the M1 design system; the
  `Pages_Use_DesignSystem_Components_Not_DOM`
  architecture test (M1) is in place.
- **Risk: the M3 surface introduces a new
  shell surface (a new layout, a new
  navigation pattern).** Mitigation: the
  M3 surface composes the M2 shell; no new
  layout, no new navigation pattern. The
  M3 surface lives at `/projects` and uses
  `AppPageHeader` + `AppBreadcrumb` like
  every other data-owning page.

---

## 9. Coherent Commit + Merge

- One coherent commit on the feature
  branch: `feat(m3.1): add project
  registration surface` (the M3.1 work).
- Fast-forward merge into `main` per
  `.ai/workflows/branching-strategy.md`
  rule 6.
- The feature branch is deleted per rule
  7.
- The M3.1 commit is on `main`; the M3
  milestone is **Active** (M3.1 is the
  first M3 slice; M3 closes when M3.x is
  the M3 closeout slice).

---

## 10. Stop Condition

The M3.1 session stops after the M3.1
coherent commit is fast-forwarded into
`main` and the feature branch is deleted.
The M3 plan is in `Awaiting Approval` for
each M3.x slice; the M3 plan is the
contract for the M3 milestone.

The M3 closeout slice is the M3.x slice
that produces the M3 retrospective at
`retrospective-m3-project-registration.md`
per the Milestone Closeout Standard. The M3
milestone tag (`m3`) is at the M3 closeout
commit on `main`. The M4-A plan is the M3
closeout's deliverable.

---

## 11. Dependencies

- M2 (closed 2026-07-11). The M2.2
  `INavigationRegistry`, the M2.3
  `AppPageHeader` + `AppBreadcrumb`, the
  M2.4 `IProjectIntelligenceReader` (not
  consumed by M3.1; consumed by a future
  M3 slice if the snapshot is extended),
  and the M2.5 accessibility patterns (the
  `aria-current="page"` invariant, the
  keyboard smoke, the `AppShellRegion`
  data-attribute pattern) are the M2
  deliverables M3 composes.
- M1 (closed 2026-07-10). The M1.2 design
  system is the substrate.
- M0.5 (closed 2026-07-10). The
  structured state is the M3 state-update
  mechanism.

---

## 12. Milestone Closeout Hook

The M3 closeout follows the Milestone
Closeout Standard at
`.ai/workflows/milestone-closeout.md`. The
M3 closeout produces the M3 retrospective
at
`retrospective-m3-project-registration.md`
(13 sections, per the standard); the M3
implementation report at
`implementation-report-m3-<slice>.md` per
M3 slice; the M3 per-session handoffs at
`.ai/handoffs/2026-07-11-m3-<slice>.md`;
the state updates per Rule 15; the
M3 closeout commit on the feature branch
fast-forwarded into `main`; the `m3`
annotated milestone tag at the M3
closeout commit on `main`; the feature
branch deleted; the M4-A plan in
`Awaiting Approval`; the first M4-A
task in `Ready`.
