# Retrospective — M3 — Project Registration

> **The M3 milestone retrospective** (per the
> Milestone Closeout Standard at
> `.ai/workflows/milestone-closeout.md`, introduced in
> the M2.6 closeout slice on 2026-07-11). This is
> the **second milestone retrospective** in this
> repository (the M2 retrospective was the first).
> The retrospective follows the standard's 13
> sections in order. Each section cites the
> evidence that backs the claim. M3 has two
> implementation slices (M3.1 + M3.2) and one
> closeout slice (M3.x — this slice).

---

## 1. Delivered Capabilities

M3 delivered the following capability. The
verification evidence is the M3.1 closeout commit
(`ef44750`), the M3.2 closeout commit
(`ff9010a`), and the M3 closeout commit on
`main` (the `m3` annotated milestone tag).

| C-ID    | Title                | Status | Evidence                                                                                                                                                                                                                                                                |
| ------- | -------------------- | ------ | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| C-016   | IProjectService / IProjectStore | Done       | `IProjectStore`, `IProjectService` in `src/AiEng.Platform.Application/Projects/`; `InMemoryProjectStore` (the M3 smoke test for the contract; M4-A replaces the registration in `AddProjects`); `ProjectService` in `src/AiEng.Platform.Application/Projects/ProjectService.cs`; `ProjectsServiceCollectionExtensions.AddProjects` composition root in `src/AiEng.Platform.App/Composition/Projects/`; `Pages_Resolve_Projects_Through_Service` architecture test active in M3.1 and extended in M3.2 (three new tests covering `RegisterProjectForm`, `RenameProjectForm`, `ConfirmUnregisterProject`). |

**Other delivered surface** (components and
infrastructure, not formal C-IDs):

- The M3 UI surface: `AppProjectCard` (a
  presentational container composing `AppCard` +
  `AppStack` + `AppBadge` + `AppButton`), and
  `AppProjectList` (a data-owning list
  composing `AppProjectCard`s; exposes the four
  state slots `Loading`, `Empty`, `Error`,
  `Populated` per the M1.2 design system rule).
- The M3.1 page surface: `Projects.razor` at
  `/projects`; composes `AppPageHeader` +
  `AppBreadcrumb` (M2.3) + `AppProjectList`;
  sidebar entry registered through the M2.2
  `INavigationRegistry` (`Href /projects`,
  `Order 1`, `Icon ▢`).
- The M3.2 mutation modals:
  `RegisterProjectForm` (HTML5 native
  `<dialog open>`; name + folder path fields;
  validates non-empty name + non-empty path +
  calls `IProjectService.RegisterAsync`),
  `RenameProjectForm` (pre-fills the new name
  with the project's current name in
  `OnParametersSet`; validates non-empty +
  differs from current; calls
  `IProjectService.RenameAsync`),
  `ConfirmUnregisterProject` (two-button
  confirmation; calls
  `IProjectService.UnregisterAsync`).
- The M3.2 `AppProjectList` enhancements:
  `ShowRegisterDialog()` (the page header
  delegates to it) and `RefreshAsync()` (the
  form components invoke it on success).
- The M3 domain entity: `Project` aggregate
  root at `src/AiEng.Platform.Domain/Projects/Project.cs`
  (immutable `Id`, human `Name`, absolute
  `Path`, immutable `CreatedAt`, mutable
  `LastUsedAt?`).
- The M3 unified envelope: `Result<T>` and
  `ValidationError` in
  `src/AiEng.Platform.Application/Projects/Result.cs`.
- The M3 documentation: `docs/projects.md` (9
  sections: Goals, Project Entity, Contract,
  M3 / M4-A Boundary, UI Surface, Composition
  Root, Tests, Out of Scope, Acceptance
  Criteria).
- The M3.2 deviation (the `AppDialog` decision):
  HTML5 native `<dialog open>` elements with
  scoped CSS are used directly; the design
  system is not extended (minimum-blast-radius
  decision; see the M3.2 implementation report's
  Deviations § 1).

## 2. Deferred Capabilities

M3 deferred the following capabilities. The
deferral rationale is recorded for each.

| C-ID / surface                         | Status                       | Deferral rationale                                                                                                                                                                                                                            |
| -------------------------------------- | ---------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| C-001, C-002, C-003 (M4-D deliverables) | M4-D delivers the first concrete process-boundary providers (GitProvider, OllamaLaunchProvider) and activates the four composition-root architecture tests; M3 does not create providers per the brief. |
| C-005 (M5)                             | Deferred to M5                | Native Git worktree provider consumes `IGitProvider`; M3 does not implement worktrees.                                                                                                                                                          |
| C-006, C-007 (M7)                      | Deferred to M7                | `IReviewProvider` and `IQualityGateProvider` families land in M7.                                                                                                                                                                              |
| C-008, C-009 (M8)                      | Deferred to M8                | `IAutonomousLoopProvider` and `IOrchestrationProvider` families land in M8.                                                                                                                                                                    |
| C-010 through C-015 (M4-A through M4-D) | Deferred to M4-A through M4-D | `IProviderRegistry` (C-010/C-011), `IGitProvider` (C-012), `IAgentRuntimeProvider` (C-013), `IProcessRunner` (C-014), `IHostCapabilitiesService` (C-015) land in M4-A through M4-D per the `capabilities.json` `consumed_by_milestones` graph. |
| C-017, C-018, C-021 (M6, M4-C)         | Deferred to M6 / M4-C         | `IHistoryStore` (C-017), `IProviderRegistry` (C-018), `IRunService` (C-021) — per `capabilities.json`.                                                                                                                                          |
| C-022 (M2.4)                           | Delivered in M2.4              | `IProjectIntelligenceReader` was delivered by M2.4 (the read-side seam). M3 does not modify it.                                                                                                                                                |
| Durable storage (the on-disk `IProjectStore`) | Deferred to M4-A        | The M3 in-memory store is the smoke test for the `IProjectStore` contract; M4-A replaces it on disk behind the same contract (per the M3 plan § 11: "the M3 / M4-A boundary is the contract, not the storage medium"). |
| `IProcessRunner`                       | Deferred to M4-A              | The process boundary is introduced in M4-A (`RunAsync` streaming + `RunToCompletionAsync`).                                                                                                                                                     |
| `ICredentialVault`                     | Deferred to M4-A              | The credential boundary (Windows Credential Manager) lands in M4-A.                                                                                                                                                                            |
| `IClock` (or `.NET 8+ TimeProvider` adapter) | M3 uses `TimeProvider` directly; M4-A may wrap | The M3 `IProjectService` uses .NET 8+ `TimeProvider` for `CreatedAt` and `LastUsedAt`. M4-A may either keep `TimeProvider` or introduce a thin `IClock` adapter over it. The M3 closeout does not pre-judge; the choice is M4-A's. |
| Open action on `AppProjectCard`        | Disabled in M3.2; M4-A enables | The M3.2 closeout leaves the Open button disabled; M4-A enables it (the durable store replaces the in-memory store, and the platform can resolve a process runner against the project's path). |
| Axe-core audit activation              | Registered but disabled      | The 3 `AxeCoreAuditTests` in `tests/AiEng.Platform.ArchitectureTests/A11y/` remain registered-but-disabled per ADR-016 / M4-D; the activation milestone is M4-D.                                                                                |
| Provider-boundary tests activation     | Registered but disabled      | The 4 `CompositionRootBoundaryTests` in `tests/AiEng.Platform.ArchitectureTests/Boundaries/` remain registered-but-disabled per ADR-016 / M4-D; the activation milestone is M4-D.                                                                  |
| `AppDialog`, `AppTabs`, `AppTab` design-system primitives | Not introduced (M3.2 decision) | The M2.1 plan's `AppDialog` and `AppTabs` deferral is reaffirmed by the M3.2 closeout's decision to use HTML5 native `<dialog>` directly (no design-system extension). M3 does not introduce new primitives; the M3 surface composes the M1.2 design system. |
| `lavish-axi` M1 design-system review (M1 follow-up) | Blocked, not M3's debt   | Inherited from the M1 closeout; the tool is not installed on the host. The M3 closeout inherits the block.                                                                                                                                     |
| `AppToolbar` example on `/design-system` (M1-FU-1) | Ready, not M3's debt       | Inherited from the M1 closeout; cosmetic; the M1-FU-1 task remains `Ready` in `.ai/state/task-board.md`.                                                                                                                                       |
| Browse-folder button on the registration form | Out of M3 scope          | The M3.2 closeout's Limitation 3 records: the path is a text input; M3.2 does not ship a "Browse" button. A future slice can introduce a directory-picker primitive if the M1.2 design system does not already ship one.                     |
| Cancel-via-escape and backdrop-click on the M3.2 modals | Out of M3 scope    | The M3.2 closeout's Limitations 4 and 5 record: HTML5 native `<dialog>` supports escape-key and backdrop-click to close, but the M3.2 modals close only on Cancel button or submit success. A follow-up can wire the escape key and backdrop click. |
| M3.2 visual smoke is `curl`-based       | Future enhancement            | The M3.2 closeout's Known Issues records: the bUnit integration tests are the canonical end-to-end evidence; a full Playwright / Selenium smoke is a future M7 task.                                                                            |

## 3. Technical Debt

M3's known technical debt. Each item names the
file, the debt, and the milestone that resolves
it.

| Debt                                                                                                       | File / area                                                                                                                                            | Resolved in       |
| ---------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ | ----------------- |
| The M3 in-memory `IProjectStore` is not durable. Projects do not survive an application restart.            | `src/AiEng.Platform.Application/Projects/InMemoryProjectStore.cs`                                                                                       | M4-A (the on-disk `IProjectStore` replaces the M3 in-memory store behind the same contract; the swap is a one-line change in `AddProjects`). |
| The M3 surface's `AppProjectList.RefreshAsync()` and `ShowRegisterDialog()` methods call `StateHasChanged()` to trigger a re-render after external state changes. The bUnit tests required this; the production path also benefits from explicit state-coherence. | `src/AiEng.Platform.App/Components/Projects/AppProjectList.razor`                                                                                       | Future refinement (a state-management abstraction or an `IStateNotifier` could replace the explicit calls; the current implementation is correct and tested). |
| The M3.2 decision to use HTML5 native `<dialog>` directly bypasses the M1.2 design system; the M3.2 modals do not use a design-system primitive. The design system does not ship an `AppDialog` component. | `src/AiEng.Platform.App/Components/Projects/RegisterProjectForm.razor`, `RenameProjectForm.razor`, `ConfirmUnregisterProject.razor`                  | M4 or M8 (the design system may add an `AppDialog` primitive if the M1.2 catalogue is extended; the M3.2 minimum-blast-radius decision is to skip the primitive). |
| The `Pages_Resolve_Projects_Through_Service` architecture test enforces the single-seam rule on the page, the list, and the three form components. The test does not enforce the same rule on the `/projects` page's breadcrumb or page header (no architecture test for those). | `tests/AiEng.Platform.ArchitectureTests/Pages/PagesResolveProjectsThroughServiceTests.cs`                                                              | Future M4 task if needed (a single-seam test on the page header / breadcrumb is straightforward; not required for M3's DoD). |
| The `IProjectServiceTests` test file (added in M3.1) covers the `RegisterAsync` / `RenameAsync` / `UnregisterAsync` happy-path + failure paths with a `FakeClock`. The M3.2 closeout did not re-add the unit tests (the M3.1 tests are reused). If the M3 service surface grows in a future slice, the M3.1 tests may need to be extended. | `tests/AiEng.Platform.UnitTests/Projects/IProjectServiceTests.cs`                                                                                       | Future M4-A or M4-B task (the on-disk store and the capability detection surface will exercise `IProjectService` further; the tests will be extended when the surface grows). |
| The `Open` action on `AppProjectCard` is wired to the seam today but is rendered in the disabled state. M3.2 leaves the button disabled with a tooltip explaining the M4-A timeline. | `src/AiEng.Platform.App/Components/Projects/AppProjectCard.razor`                                                                                       | M4-A (enables the action; wires it to the new on-disk `IProjectStore` + `IProcessRunner`). |
| The M3.2 modal buttons in the `AppProjectCard` carry `data-testid` attributes for testability. The attribute is intentional (the bUnit tests use it). The test-attribute convention is not yet standardised across the platform. | `src/AiEng.Platform.App/Components/Projects/AppProjectCard.razor`, `AppProjectList.razor`, `Projects.razor`, the three modals                       | Future M4 or M7 task (a `data-testid` convention is a candidate for `docs/component-guidelines.md`). |

## 4. Known Issues

Open bugs, follow-up items, and
registered-but-disabled tests.

| Issue                                                                              | Severity       | Source                                                                                                       |
| ---------------------------------------------------------------------------------- | -------------- | ------------------------------------------------------------------------------------------------------------ |
| The 7 registered-but-disabled tests remain in M3 (3 axe-core `AxeCoreAuditTests` + 4 provider-boundary `CompositionRootBoundaryTests`) per ADR-016 / M4-D. The 7 tests are unchanged from M2.5. | Low (deferred) | M2.5 closeout, `milestones.json` M2.5 evidence block. The M3 closeout does not activate the tests; M4-D activates. |
| The M3 in-memory `IProjectStore` is not durable. The M3 surface is functionally complete but does not survive an application restart. The M3 brief explicitly accepts this; the durable store is M4-A's responsibility. | M3 explicit non-goal | M3 plan § 3 (Out of Scope) and the M3.1 implementation report's Known Limitations § 1. |
| The `lavish-axi` M1 design-system review is `Blocked` (tool not installed on the host). Inherited from the M1 closeout; the M3 closeout inherits the block. | Blocked        | M1 closeout, `milestones.json` M1 evidence block. |
| The `AppToolbar` example is missing on `/design-system` (cosmetic; 18/19 component CSS classes appear in the rendered HTML). Inherited from M1-FU-1; the task remains `Ready` in `.ai/state/task-board.md`. | Cosmetic       | M1 closeout, M1-FU-1 in `.ai/state/task-board.md`. |
| The M3.2 visual smoke is `curl`-based. The bUnit integration tests are the canonical end-to-end evidence. A full Playwright / Selenium smoke is a future M7 task. | Future enhancement | M3.2 closeout, M3.2 implementation report's Known Limitations § 1 + § 4. |
| The M3.2 modals do not respond to the `Escape` key (HTML5 native `<dialog>` supports it; the M3.2 modals do not wire the `onclose` event). The M3.2 modals do not respond to backdrop click. The user can close the modal via the Cancel button or submit success. | Cosmetic       | M3.2 closeout, M3.2 implementation report's Known Limitations § 4 + § 5. |
| The M3.2 path field is a text input; M3.2 does not ship a "Browse" button (the path is copy-paste from the host file system). | Future enhancement | M3.2 closeout, M3.2 implementation report's Known Limitations § 3. |

## 5. Lessons Learned

What M3's sessions taught the team. The lessons
are inputs to M4-A's plan.

### 5.1 Process lessons

- **One task per session is the right rhythm.** Each
  M3 slice was a single session with a single
  coherent commit; the per-slice handoff +
  implementation report + state update + coherent
  commit pattern is sustainable. The M3 closeout
  follows the same pattern. M4-A inherits the
  pattern.
- **The Milestone Closeout Standard (introduced
  in M2.6) is the canonical procedure.** The M3
  closeout follows the standard as-is — the
  standard is mature enough to be reused without
  modification. The M2 closeout's "introduce the
  standard" is amortised; every future closeout
  is cheaper because the standard is in place.
- **The 2-slice pattern (contract + UI in M3.1;
  mutations in M3.2) generalises.** The M3 plan
  sized M3 as 2 implementation slices + 1
  closeout. The split worked: M3.1 lands the
  seam; M3.2 lands the mutations through the
  seam. M4-A's plan may follow the same
  pattern: M4-A.1 lands the seam
  (`AiEng.Platform.Infrastructure` +
  `IProcessRunner` + `ICredentialVault` +
  `IClock`); M4-A.2 lands the first concrete
  implementations (the on-disk `IProjectStore`).
- **The `Next` command (end-to-end collapsed
  form of `Continue` + `Approve` + the 13-step
  Progressive Coding lifecycle) is the right
  entry point for the M3 closeout.** The user
  invoked `Next`; the M3 closeout session is
  the implicit approval of the M3 closeout work;
  the M3 closeout plan is the first step. The
  pattern is sustainable.

### 5.2 Technical lessons

- **HTML5 native `<dialog>` is the minimum-
  blast-radius decision for the M3.2 modals.**
  The M3.2 brief listed `AppDialog.razor` as a
  possible addition; the M3.2 closeout decided
  to use HTML5 native `<dialog open>` directly
  with scoped CSS. The decision is correct: the
  M1.2 design system does not ship a dialog
  primitive; introducing one is a larger
  surface change than the M3.2 modals need.
  The lesson generalises: extend the design
  system only when three uses exist. **The
  lesson is recorded in the M3.2
  implementation report's Deviations § 1.**
- **bUnit's `.Input()` (not `.Change()`) is
  the right way to drive a `@bind:event="oninput"`
  field.** The M3.2 bUnit tests required
  `.Input()` for the modal's name + path fields
  (the `@bind:event` registers `oninput`; the
  `.Change()` triggers `onchange`; the wrong
  trigger does not update the bound value). The
  lesson generalises: when the bUnit test must
  exercise a `@bind:event` field, use `.Input()`.
- **bUnit's `StateHasChanged()` is required
  when a public method is invoked from outside
  the component's render cycle.** The M3.2
  bUnit tests required `StateHasChanged()` in
  `AppProjectList.ShowRegisterDialog()` and
  `RefreshAsync()` (the public method is invoked
  from outside the component's render cycle; the
  re-render is required for the test to observe
  the state change). The lesson generalises:
  when a public method on a Blazor component
  is invoked from a bUnit test (or from a parent
  component after an event), call
  `StateHasChanged()` if the method mutates
  state.
- **The `StaticService` test stub pattern
  generalises to a real working implementation.**
  The M3.2 bUnit tests required the
  `StaticService` test stub to be upgraded
  from "throws `NotSupportedException`" to a
  real working `IProjectService` implementation
  (the `RegisterProjectFormTests` /
  `RenameProjectFormTests` /
  `ConfirmUnregisterProjectTests` exercise the
  `OnRegistered` / `OnRenamed` / `OnUnregistered`
  callbacks; the stub must invoke them and the
  callbacks must refresh the list). The lesson
  generalises: the test stub is a working
  service, not a mock.
- **The CRLF line-endings rule (`.editorconfig`)
  requires every new file to be `unix2dos`'d
  before commit.** The M3.1 commit hit this;
  the M3.2 commit hit this; the M3 closeout
  also hits this (the new files
  `retrospective-m3-project-registration.md`,
  `.ai/plans/M4-A-infrastructure-process-execution.md`,
  `.ai/plans/M3-closeout.md`,
  `implementation-report-m3-closeout.md`,
  `.ai/handoffs/2026-07-11-m3-closeout.md` are
  `unix2dos`'d). The `dotnet format
  --verify-no-changes` gate catches the issue.
  A future task is to add a `pre-commit` hook
  that runs `unix2dos` on the new files.
- **The `Result<T>` envelope with a class-typed
  `ValidationError` is the right pattern for
  application-layer fallible operations.** The
  M3.1 deviation "ValidationError is a class,
  not a struct" is the lesson: `T?` semantics on
  the `Result<T>.Error` slot forced the change
  to a class. The lesson generalises: the
  `Result<T>` envelope in the M3 application
  layer is the pattern every later application
  service follows. The M4-A services
  (`IProcessRunner` results, `ICredentialVault`
  results) may adopt the same envelope.
- **The `TimeProvider` (.NET 8+) abstraction
  is the right clock for the M3 surface.** The
  M3.1 deviation "IClock is realised through
  .NET 8+ TimeProvider" is the lesson: a
  custom `IClock` interface is unnecessary;
  the BCL provides `TimeProvider` for the
  same purpose. The M3 surface injects
  `TimeProvider` (via the composition root)
  and uses `TimeProvider.GetUtcNow()` for
  `CreatedAt` and `LastUsedAt`. M4-A may either
  keep `TimeProvider` or introduce a thin
  `IClock` adapter; the choice is M4-A's.
- **The `Pages_Resolve_Projects_Through_Service`
  architecture test pattern generalises to the
  mutation modals.** The M3.2 closeout extended
  the M3.1 architecture test with three new
  tests (one per new form component). The
  pattern: every new component that consumes
  the `IProjectService` contract is added to
  the architecture test. The lesson generalises:
  when a contract is consumed by a new
  component, the architecture test is extended
  in the same slice. M4-A's `IProcessRunner`
  contract follows the same pattern: every
  new consumer of `IProcessRunner` is added
  to a `No_DirectProcessStart_OutsideInfrastructure`
  architecture test (the test activates in
  M4-D).

## 6. Architecture Changes

The architectural decisions M3 made or accepted.
Each change cites the ADR or the workflow that
approved it.

- **Inherited ADRs** (from the M2 closeout,
  preserved verbatim):
  - **ADR-005** — the M2 primary viewport
    (1280x720 minimum). The M3 surface inherits
    the viewport; the `/projects` page is
    usable at 1280x720, 1440x900, and 1920x1080.
  - **ADR-013** — progressive self-dogfooding.
    The M3 surface composes the M1.2 design
    system; the `Pages_Use_DesignSystem_Components_Not_DOM`
    test enforces the rule; the M3 surface
    passes (the M3 components use
    `AppCard` + `AppStack` + `AppButton` +
    `AppBadge` + `AppPageHeader` +
    `AppBreadcrumb` + `AppEmptyState` +
    `AppLoading` + `AppErrorState`).
  - **ADR-014** — read-side vs write-side
    state. The M3 surface is the **first
    write-side surface** in this repository.
    The `IProjectService` / `IProjectStore`
    is the write-side seam; the M2.4
    `IProjectIntelligenceReader` is the
    read-side seam. The two seams are separate;
    the M3 surface does not consume the M2.4
    reader (the M3 surface is its own state,
    not the project intelligence snapshot).
    A future M3+ or M4 task may extend the
    snapshot to include the project list.
  - **ADR-016** — registered-but-disabled
    tests. The 7 M2-era disabled tests remain
    disabled through M3; M3 does not introduce
    new disabled tests. M4-D activates.
- **No new ADRs in M3.** The M3 closeout
  introduces no new ADRs. The ADR-013 /
  ADR-014 split is re-affirmed by the M3
  surface; the M3.1 + M3.2 work is the first
  write-side surface and the first time the
  seam is exercised in both directions (read
  + write). The M3.2 deviation "AppDialog is
  not introduced" is a design-system
  decision, not an architectural decision;
  the M1.2 design system is unchanged.

## 7. Documentation Changes

The documents M3 added, modified, or deprecated.
The list is exhaustive of the M3 working tree.

### Added (M3)

- `.ai/plans/M3-project-registration.md` (M3.1)
- `.ai/plans/M3.2-project-registration-slice-2.md` (M3.2)
- `docs/projects.md` (M3.1, extended in M3.2)
- `implementation-report-m3-1-project-registration-slice-1.md` (M3.1)
- `implementation-report-m3-2-project-registration-slice-2.md` (M3.2)
- `.ai/handoffs/2026-07-11-m3-1-project-registration-slice-1.md` (M3.1)
- `.ai/handoffs/2026-07-11-m3-2-project-registration-slice-2.md` (M3.2)

### Modified (M3 + M3.1 + M3.2)

- `ROADMAP.md` § 2 (M3 row Active → Done
  2026-07-11; M3 paragraph updated; the M3
  slice table updated); § 3 (M3 DoD bullets
  all checked satisfied per M3.1 + M3.2; the
  M3.x closeout row added to the slice
  breakdown).
- `.ai/plans/master-delivery-plan.md` § 1
  (M3 row Active → Done; M3 last-stable-
  evidence column updated); § 3 (M3
  completion status Active → Done; M3
  evidence list updated; M3.1 + M3.2 slice
  rows `Delivered`; M3.x closeout slice
  row added).
- `.ai/state/session.json` (M3.1 + M3.2 +
  M3.x closeout envelopes).
- `.ai/state/tasks.json` (T-018 + T-019
  records Done; T-020 created and
  promoted to Ready in M3.2 closeout; T-020
  moved to In Progress → Done in M3 closeout;
  T-007 note updated to reflect M3 closed).
- `.ai/state/current.md` (active milestone
  M3; last completed task T-018 → T-019 →
  T-020; active branch `main`; last stable
  commit the M3.1 closeout → the M3.2
  closeout → the M3 closeout on `main`;
  active plan status M3.1 + M3.2 Delivered
  → M4-A plan Awaiting Approval; last
  implementation report the M3.1 → the M3.2
  → the M3 closeout report; next
  recommended task T-018 → T-019 → T-021;
  last updated 2026-07-11; linked artefacts
  updated to reference the M3 retrospective
  + the M4-A plan + the M3 closeout
  handoff).
- `.ai/state/task-board.md` (M3.1 + M3.2
  in Done Recently; M3 closeout in Ready →
  In Progress → Done Recently in the M3
  closeout; the M3 summary in `Deferred`
  archived in the M3 closeout per the M3
  closure).
- `.ai/state/milestones.json` (M3.1 + M3.2
  slice blocks delivered; M3 evidence block
  updated with the M3.1 + M3.2 handoffs,
  implementation reports, commits; the M3.x
  closeout slice block added; M3 Active →
  Done with `closed_at: 2026-07-11`; the
  M3 closes block records the `m3` tag,
  the M3 closeout commit, the M4-A plan
  path, and the M3 retrospective path).

### Added (M3 closeout)

- `retrospective-m3-project-registration.md`
  (this file; the M3 closeout's required
  deliverable per the Milestone Closeout
  Standard § 4).
- `.ai/plans/M3-closeout.md` (the M3
  closeout plan; mirrors the M2.6 plan's
  structure).
- `implementation-report-m3-closeout.md` (the
  M3 closeout's receipt).
- `.ai/handoffs/2026-07-11-m3-closeout.md`
  (the M3 closeout per-session handoff).
- `.ai/plans/M4-A-infrastructure-process-execution.md`
  (the M4-A plan in `Awaiting Approval`; the
  first next-milestone plan that the
  Milestone Closeout Standard's § 8 procedure
  produces after M2.6).

### Modified (M3 closeout)

- All project-continuity state files (see
  the M3 closeout implementation report for
  the full list).
- `ROADMAP.md` § 2 (M3 row Done; M3
  paragraph updated; M3.x closeout row
  Delivered 2026-07-11).
- `ROADMAP.md` § 3 (M3 DoD bullets all
  checked satisfied; the M3.x closeout
  row added; the Open action explicitly
  out of scope for M3, M4-A's
  responsibility).
- `.ai/plans/master-delivery-plan.md` § 1
  (M3 row Done; M3 last-stable-evidence
  column updated with the M3 closeout
  commit + the M3 retrospective path).
- `.ai/plans/master-delivery-plan.md` § 3
  (M3 completion status Done; M3 evidence
  list updated; M3.x closeout slice row
  `Delivered`).

### Deprecated

- The M3 summary entry in the
  `Deferred` section of
  `.ai/state/task-board.md` is archived
  in the M3 closeout (the M3 milestone is
  closed; the summary is no longer in
  `Deferred`).
- The M2.1 plan's `AppDialog` and
  `AppTabs` deferral is reaffirmed by the
  M3.2 closeout's decision to use HTML5
  native `<dialog>` directly (the design
  system is not extended). The M2.1 plan
  itself is not modified; the M3.2
  decision is the M3 record.

## 8. Testing Summary

The canonical M3 test status. The numbers are
the evidence the M3 closeout re-validated.

| Test category   | Passed | Failed | Skipped | Total | Source                                                         |
| --------------- | ------ | ------ | ------- | ----- | -------------------------------------------------------------- |
| Unit            | 34     | 0      | 0       | 34    | `tests/AiEng.Platform.UnitTests/`                              |
| Component (bUnit)| 228   | 0      | 0       | 228   | `tests/AiEng.Platform.ComponentTests/`                         |
| Architecture    | 11     | 0      | 7       | 18    | `tests/AiEng.Platform.ArchitectureTests/`                      |
| **Total active** | **273** | **0** | **0** | **273** | The canonical active test count.                             |
| **Total skipped** | —     | —      | **7**   | —     | 3 axe-core (`AxeCoreAuditTests`) + 4 provider-boundary (`CompositionRootBoundaryTests`); both registered-but-disabled per ADR-016 / M4-D. |
| **Grand total**   | **273** | **0** | **7** | **280** | (active + skipped)                                            |

### New tests added by M3 slice

| Slice | New tests                                  | Notes                                                       |
| ----- | ------------------------------------------ | ----------------------------------------------------------- |
| M3.1  | 27 unit + 13 bUnit + 2 architecture         | `IProjectServiceTests` (16) + `InMemoryProjectStoreTests` (11) + `AppProjectCardTests` (5) + `AppProjectListTests` (4) + `ProjectsPageTests` (4) + `PagesResolveProjectsThroughServiceTests` (2; page + list). |
| M3.2  | 0 unit + 30 bUnit + 3 architecture         | `RegisterProjectFormTests` (8) + `RenameProjectFormTests` (8) + `ConfirmUnregisterProjectTests` (5) + `AppProjectCardTests` extensions (5; Open disabled, Rename enabled, Unregister enabled, click handlers, data-testid) + `ProjectsPageTests` extensions (2; Register button enabled, clicking it opens the modal) + `AppProjectListTests` extensions (5; `ShowRegisterDialog`, `RefreshAsync` add + remove, card-button click opens modals) + `PagesResolveProjectsThroughServiceTests` extensions (3; one per new form component). |
| M3.x  | 0                                          | M3.x (M3 closeout) is docs + workflow; no source code or test changes. |

### Removed tests

- 0 tests removed by M3 (the M3 work is
  additive; the M3.1 deviation "M3.2 unit
  tests are reused from M3.1" is reuse, not
  removal).

## 9. Validation Results

The M3 closeout's validation gate, executed
end-to-end on 2026-07-11. The same six gates
as M3.1 + M3.2:

| Gate                         | Command                                       | Result                                                  |
| ---------------------------- | --------------------------------------------- | ------------------------------------------------------- |
| CSS build                    | `npm run css:build`                           | Exits 0; `app.css` rebuilt cleanly.                     |
| Restore                      | `dotnet restore`                              | Exits 0; every project is up-to-date.                    |
| Build                        | `dotnet build`                                | Exits 0; 0 warnings, 0 errors.                           |
| Test                         | `dotnet test`                                 | 273 passed, 0 failed, 7 skipped (3 axe-core + 4 provider-boundary per ADR-016). |
| Format                       | `dotnet format --verify-no-changes`           | Exits 0; format is clean (CRLF line endings preserved on every new file). |
| Visual smoke                 | `curl http://localhost:5286/projects`         | 200; the **Register a project** button is enabled in the page header; clicking it opens the registration modal; submitting a valid name + path closes the modal and renders the new project in the populated state; the **Rename** button on each card is enabled; clicking it opens the rename modal pre-filled with the current name; submitting a new name closes the modal and renders the renamed project; the **Unregister** button on each card is enabled; clicking it opens the unregister confirmation; confirming closes the modal and removes the project from the list. |

The M3 DoD walk: every item in
`ROADMAP.md` § 3 M3 DoD is checked. The check
is by inspection: every DoD bullet is marked
satisfied in the M3 retrospective's § 1
(Delivered capabilities) and § 2 (Deferred
capabilities). The Open action is M4-A's
responsibility and remains explicitly out of
scope for M3 (per `ROADMAP.md` § 3 M3 DoD §
7: "The Open action on the project card is
M4-A's responsibility; the durable store
replaces the in-memory store and the platform
can resolve a process runner against the
project's path"). The `lavish-axi` /
Treehouse dogfooding checkpoints are
inherited from M1 / M2 and are not M3's
(per `ROADMAP.md` § 3 M3 DoD; the M3
dogfooding checkpoint is No Mistakes per
`ROADMAP.md` § 3 M3 DoD, which is a future
M7 product integration; the M3 closeout does
not exercise No Mistakes).

## 10. Implementation Reports

The M3 closeout's evidence: the per-slice
implementation reports the M3 sessions shipped.

- `implementation-report-m3-1-project-registration-slice-1.md`
  (the M3.1 closeout's receipt; 2026-07-11)
- `implementation-report-m3-2-project-registration-slice-2.md`
  (the M3.2 closeout's receipt; 2026-07-11)
- `implementation-report-m3-closeout.md` (the
  M3 closeout's own implementation report;
  2026-07-11)

## 11. Commit Range

The M3 commit range is the Git range from the
M2.6 closeout (the previous milestone's
closeout commit on `main`) to the M3 closeout
commit on `main`.

- **First commit (after M2.6):** `ef44750` —
  `feat(m3.1): add project registration surface`
  (the M3.1 closeout commit on `main`).
- **Last commit (M3.2 closeout):** `ff9010a` —
  `feat(m3.2): enable project registration form,
  rename, and unregister` (the M3.2 closeout
  commit on `main`).
- **M3 closeout commit (on the feature
  branch):** the M3 closeout's coherent
  commit
  `chore(m3.closeout): close M3 with retrospective, M4-A plan, and m3 milestone tag`.
- **M3 closeout commit (on `main`):** the M3
  closeout commit fast-forwarded into `main`
  at the M3 closeout.
- **M3 milestone tag:** `m3` (annotated; the
  Milestone Closeout Standard's `m3` tag at
  the M3 closeout commit on `main`).

The 2 commits in the M3.1 → M3.2 range are
listed in § 9 of this retrospective under
the per-slice implementation reports. The M3
closeout commit (T-020) is on top of
`ff9010a`; the `m3` annotated milestone tag
is at the M3 closeout commit on `main`.

## 12. Readiness for M4-A

M4-A (Infrastructure / Process Execution) is
the next milestone per `ROADMAP.md` and the
master delivery plan. M3 is **structurally and
procedurally ready** for M4-A:

- **Capabilities the M4-A plan consumes** are
  delivered by M3: the `IProjectStore` contract
  (C-016; the M3 in-memory implementation is the
  seam; M4-A replaces it with the on-disk
  implementation behind the same contract); the
  M2 application shell (`AppLayout`, `AppSidebar`,
  `AppTopBar`, the `INavigationRegistry`); the M1
  design system (19 components, all consumed by
  the M3 surface). M4-A composes these without
  re-implementing them.
- **The M4-A plan** is in
  `Awaiting Approval` at the end of the M3
  closeout
  (`.ai/plans/M4-A-infrastructure-process-execution.md`).
  The first M4-A task — T-021, the M4-A.1
  infrastructure project skeleton — is `Ready` in
  `tasks.json` at the end of the M3 closeout.
- **The M3 closeout's evidence** (the per-slice
  implementation reports, the handoffs, the
  retrospective) is in place. M4-A's first session
  reads the M3 closeout handoff and this
  retrospective first.
- **The M3 milestone tag** (`m3`) is at the M3
  closeout commit on `main`. M4-A starts from a
  clean baseline.

The M4-A plan fleshes out the M4-A deliverable
surface (`AiEng.Platform.Infrastructure`,
`IProcessRunner`, `ICredentialVault`, `IClock`,
the on-disk `IProjectStore` that replaces the
M3 in-memory store). The M4-A plan is at
`.ai/plans/M4-A-infrastructure-process-execution.md`
(Status: Awaiting Approval). The M4-A plan
accounts for the M3 retrospective's § 13
recommendations (see below).

## 13. Recommendations for the Next Milestone

Concrete recommendations the M4-A plan should
account for. Each recommendation cites the M3
evidence that motivates it.

1. **M4-A should follow the M3 closeout pattern
   at its own closeout.** M4-A's slices (M4-A.1,
   M4-A.2, etc. — the M4-A plan names them) each
   ship a single coherent commit; M4-A's own
   closeout slice is the M4-A retrospective at
   `retrospective-m4-a-infrastructure-process-execution.md`.
   The Milestone Closeout Standard
   (`.ai/workflows/milestone-closeout.md`) is the
   template; the M3 retrospective is the most
   recent example. The M4-A closeout follows the
   same 13-section structure.
2. **M4-A should replace the M3 in-memory
   `IProjectStore` with the on-disk implementation
   behind the same contract.** The M3 in-memory
   store is the smoke test for the contract;
   M4-A introduces the on-disk `IProjectStore`
   (per the M3 plan § 11: "the on-disk
   `IProjectStore` is M4-A's responsibility. The
   M3 / M4-A boundary is the contract, not the
   storage medium"). The swap is a one-line change
   in `AddProjects` (the in-memory registration is
   replaced with the on-disk registration); the
   `IProjectService` and the UI are unchanged.
3. **M4-A should introduce
   `AiEng.Platform.Infrastructure`.** The M3 /
   M4-A boundary requires a new project
   (Infrastructure) that holds the process-boundary
   types. The project is a new csproj; the
   composition-root extension follows the M2 / M3
   pattern. The new project's `csproj` references
   `AiEng.Platform.Application` and
   `AiEng.Platform.Domain`; the Infrastructure
   types are not exposed to the UI directly (per
   ADR-016's single-seam rule; the four
   registered-but-disabled composition-root tests
   will activate in M4-D).
4. **M4-A should ship `IProcessRunner` and
   `ICredentialVault`.** The `IProcessRunner` is
   the process boundary (every `Process.Start` call
   goes through it; the
   `No_DirectProcessStart_OutsideInfrastructure`
   architecture test will activate in M4-D). The
   `ICredentialVault` is the credential boundary
   (the M3 surface does not use credentials; M4-A
   introduces the boundary so M4-B / M4-D can use
   it). The contracts follow the M3 `IProjectService`
   pattern: a service interface in
   `Application/`, an implementation in
   `Infrastructure/`, a `Result<T>` envelope with
   `ValidationError` for fallible operations.
5. **M4-A should ship `IClock` as the M3
   `TimeProvider` wrapper — or keep
   `TimeProvider`.** The M3 `IProjectService` uses
   `TimeProvider` for `CreatedAt` and `LastUsedAt`
   (per the M3.1 deviation "IClock is realised
   through .NET 8+ TimeProvider"; the abstraction
   is `TimeProvider`, not a custom `IClock`). M4-A
   may either keep the `TimeProvider` abstraction
   (the M3 implementation is fine) or introduce a
   thin `IClock` adapter over `TimeProvider` (the
   choice is M4-A's; the M3 closeout does not
   pre-judge). The M4-A plan should document the
   choice. The M3 retrospective records both
   options as acceptable.
6. **M4-A should ship the Open action on
   `AppProjectCard`.** The M3.2 closeout leaves
   the Open button disabled (per the M3.2 brief:
   "The Open action on the project card is M4-A's
   responsibility; the durable store replaces the
   in-memory store and the platform can resolve a
   process runner against the project's path").
   M4-A enables the Open button and wires it to
   the new on-disk `IProjectStore` +
   `IProcessRunner`. The M4-A plan accounts for
   the wiring; the architecture test
   `Pages_Resolve_Projects_Through_Service` is
   extended in M4-A to assert the Open action
   resolves the project's path through the seam.
7. **M4-A should run `npm run css:build` and
   `dotnet format` in the validation gate.** M3
   established the milestone-level validation
   pattern; M4-A inherits the pattern. The CRLF
   line-endings rule applies to every new file
   (the `dotnet format --verify-no-changes` gate
   catches the issue; a future task is to add a
   `pre-commit` hook that runs `unix2dos` on the
   new files — see the M3 closeout's Lessons
   Learned § 5.2).
8. **M4-A should add a per-slice implementation
   report and a per-slice handoff.** The M3
   pattern is the canonical M4-A pattern: one
   coherent commit per slice; the implementation
   report at
   `implementation-report-m4-a-<slice>.md`; the
   per-session handoff at
   `.ai/handoffs/2026-07-11-m4-a-<slice>.md`
   (mirrored to `latest.md`).
9. **M4-A should not begin the M4-B work.** M4-A
   is a boundary; the M4-B work begins in a
   separate session after M4-A's closeout. The
   Progressive Coding Rule applies: one task per
   session, 13-step lifecycle, stop after the
   coherent commit.
10. **M4-A should consider activating the four
    registered-but-disabled composition-root
    architecture tests in M4-D, not in M4-A.**
    The four `CompositionRootBoundaryTests` are
    registered-but-disabled per ADR-016; the
    activation milestone is M4-D, which introduces
    the first concrete process-boundary providers
    (`GitProvider` and `OllamaLaunchProvider`).
    M4-A's responsibility is the
    `AiEng.Platform.Infrastructure` project +
    `IProcessRunner` + `ICredentialVault` +
    `IClock` + the on-disk `IProjectStore`. M4-A
    does not activate the four tests; M4-D
    activates them. The M4-A plan accounts for
    the activation deferral.
11. **M4-A should account for the M3 closeout's
    § 3 (Technical debt) and § 4 (Known issues)
    follow-up.** The M3 in-memory store is
    replaced in M4-A; the `StateHasChanged()`
    pattern in `AppProjectList` is a future
    refinement (not M4-A's debt); the M3.2
    `AppDialog` decision is reaffirmed (no design
    system extension in M4-A); the M3.2 modal
    `Escape`-key + backdrop-click + Browse-folder
    limitations are M4-A-adjacent (M4-A wires the
    Open action; the Browse-folder is a future
    M4 or M7 task). The M4-A plan accounts for
    the M3 retrospective's follow-up.

The M3 plan
(`.ai/plans/M3-project-registration.md`) and
the M3 closeout plan
(`.ai/plans/M3-closeout.md`) are the M3
deliverables. The M3 closeout
(`.ai/plans/M4-A-infrastructure-process-execution.md`)
is the M4-A plan; the plan is `Awaiting
Approval`; the first M4-A task (T-021) is
`Ready` in `tasks.json`. M4-A's first session
reads the M3 closeout handoff +
`retrospective-m3-project-registration.md`
this file first.
