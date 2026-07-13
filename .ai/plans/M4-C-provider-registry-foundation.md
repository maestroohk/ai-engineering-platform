# M4-C — Provider Registry Foundation

> **The M4-C plan.** M4-C introduces the
> platform's **provider registry foundation**:
> the `IProviderRegistry` contract + the
> `ProviderDescriptor` + `ProviderFamily` +
> `ProviderStatus` records in
> `src/AiEng.Platform.Application/Providers/`;
> the family registries (one per family:
> `IGitProviderFamily`, `IAgentRuntimeProviderFamily`,
> `IReviewProviderFamily`, `IQualityGateProviderFamily`,
> `IAutonomousLoopProviderFamily`, `IOrchestrationProviderFamily`)
> that enumerate the available providers for each
> family; the `SystemProviderRegistry`
> implementation in
> `src/AiEng.Platform.Infrastructure/Providers/`
> that aggregates the family registries and
> consumes the M4-B `IHostCapabilitiesService`
> through DI to filter providers by host
> capability; the fakes (one per family) in
> `tests/AiEng.Platform.UnitTests/Providers/`
> that record the lookup call counts and return
> a configured `ProviderDescriptor` list; the
> `AddProviderRegistry` composition root
> extension in
> `src/AiEng.Platform.App/Composition/Providers/`;
> the `AppProviderList` data-owning four-state
> design-system component; the `/providers`
> page at `src/AiEng.Platform.App/Components/Pages/Providers.razor`
> (+ `.razor.css`); the startup provider-registry
> log through `ILogger<Program>`; the
> `Providers_Resolve_Through_Registry`
> Active architecture test (scoped to
> `App/Components/Providers/` to avoid the
> M4-A.2 Open Action + M4-B.3 `Diagnostics.razor`
> false positives); the `docs/providers.md`
> documentation.
>
> M4-C is the **second consumer of
> `IHostCapabilitiesService` outside the M4-B.3
> `Diagnostics.razor` page**. The M4-C
> `SystemProviderRegistry` consumes the M4-B
> contract through DI to filter providers by
> host capability. The M4-C surface is the
> `/providers` page that displays the available
> providers grouped by family.
>
> M4-C does not implement any `Providers.<X>`
> project. M4-D introduces the first concrete
> process-boundary providers and activates the
> four registered-but-disabled
> composition-root architecture tests per
> ADR-016.
>
> **Status:** Awaiting Approval (2026-07-13; the
> M4-C plan promotion is the M4-B closeout's
> "next concrete step"; the plan is produced by
> the M4-B closeout session and committed
> on the feature branch
> `feature/m4-c-provider-registry-plan-promotion`).
> The M4-C plan is approved implicitly on the
> user's next `Next` invocation per
> `.ai/commands.md` § 4 and the Progressive
> Coding Rule § 7.1; the M4-C implementation
> begins in a future session.
>
> **Branch:** (the M4-C.1 branch is created from
> `main` at the M4-B closeout commit when
> M4-C.1 starts; the branch is named
> `feature/T-028-m4-c-1-provider-registry-contract-and-composition`
> per the branching strategy rule 4).

---

## 1. Why This Milestone Exists

M4-B introduced the platform's host capability
detection: the `IHostCapabilitiesService`
contract + the `SystemHostCapabilitiesService`
implementation that probes 6 host tools + 6
provider credentials; the `AppCapabilityList` +
`AppKeyValueList` data-owning design-system
components; the `/diagnostics` page; the
startup capability-report log. M4-B is the
**first consumer of `IProcessRunner` +
`ICredentialVault` outside the M4-A.2 Open
Action**.

M4-B does not register any provider. M4-B
detects what the host can do; the platform needs
a separate concern that **decides which
providers are eligible for enablement** based
on the host's detected capabilities. That
concern is the **provider registry**: a single
DI seam that aggregates family-specific
registries, filters providers by host
capability, and exposes the available providers
to the UI.

M4-C ships the provider registry foundation:

- The `IProviderRegistry` contract in
  `src/AiEng.Platform.Application/Providers/`.
- The `ProviderDescriptor` + `ProviderFamily` +
  `ProviderStatus` records in
  `src/AiEng.Platform.Application/Providers/`.
- The family registries (one per family) in
  `src/AiEng.Platform.Application/Providers/Families/`
  (the lookup surface; one method per family:
  `ListProvidersAsync` returns the available
  `ProviderDescriptor`s for the family,
  filtered by host capability).
- The `SystemProviderRegistry` implementation
  in
  `src/AiEng.Platform.Infrastructure/Providers/`
  that aggregates the family registries and
  consumes the M4-B `IHostCapabilitiesService`
  through DI to filter providers by host
  capability.
- The fakes (one per family) in
  `tests/AiEng.Platform.UnitTests/Providers/`
  that record the lookup call counts and return
  a configured `ProviderDescriptor` list.
- The `AddProviderRegistry` composition root
  extension in
  `src/AiEng.Platform.App/Composition/Providers/`
  that registers `IProviderRegistry` →
  `SystemProviderRegistry` as a singleton, plus
  the 6 family registries as singletons.
- The `AppProviderList` data-owning four-state
  design-system component in
  `src/AiEng.Platform.App/Components/Providers/`
  that renders the available providers grouped
  by family, with the four data-owning slots
  `Loading` / `Empty` / `Error` / `Populated` per
  the M1.2 design system rule.
- The `/providers` page at
  `src/AiEng.Platform.App/Components/Pages/Providers.razor`
  (+ `.razor.css`) that registers the page
  through the M2.2 `[RouteMetadata]` attribute
  (`Href /providers`, `Order 5`,
  `ShowInSidebar = true`, `Icon ◇`,
  `Description = "Available providers grouped
  by family; filtered by host capability."`).
- The startup provider-registry log through
  `ILogger<Program>` (the M4-C counterpart of
  the M4-B startup log; 10-second
  `CancellationTokenSource` timeout; logs the
  per-family provider count at `Information`
  level; failures are caught and logged at
  `Warning` level).
- The `Providers_Resolve_Through_Registry`
  Active architecture test in
  `tests/AiEng.Platform.ArchitectureTests/Providers/Providers_Resolve_Through_Registry.cs`
  (the M4-C counterpart of the M4-B.3
  `Capabilities_Resolved_Through_Service` test;
  the test is scoped to
  `App/Components/Providers/` to avoid the
  M4-A.2 Open Action + M4-B.3 `Diagnostics.razor`
  false positives).
- The `docs/providers.md` documentation (10
  sections mirroring `docs/infrastructure.md` +
  `docs/capabilities.md` § 1-10: Goals, Project
  Structure, Contract, Records, Family
  Registries, Component, Page, Composition
  Root, Tests, Out of Scope).

M4-C is the **first consumer of the M4-B
`IHostCapabilitiesService` outside the M4-B.3
`Diagnostics.razor` page**. The M4-C
`SystemProviderRegistry` consumes the contract
through DI to filter providers by host
capability. The M4-C surface is the
`/providers` page that displays the available
providers grouped by family.

M4-C does not implement any `Providers.<X>`
project. M4-D introduces the first concrete
process-boundary providers and activates the
four registered-but-disabled
composition-root architecture tests per
ADR-016.

The anticipated M4-C slice breakdown is
M4-C.1 (contract + family registries +
implementation + fakes + composition root +
unit tests), M4-C.2 (the `AppProviderList`
design-system component + bUnit tests +
`/providers` page), and M4-C.x (the M4-C
closeout). The M4-C.1 first session may
revise the breakdown; the M4-C plan is a
first draft.

## 2. In Scope

1. **The `IProviderRegistry` contract** in
   `src/AiEng.Platform.Application/Providers/IProviderRegistry.cs`:
   `Task<IReadOnlyList<ProviderDescriptor>>
   ListProvidersAsync(ProviderFamily family,
   CancellationToken ct)`. The contract is the
   single DI seam for provider lookup; no
   `App/Components/` code may bypass the seam.
2. **The `ProviderDescriptor` record** in
   `src/AiEng.Platform.Application/Providers/ProviderDescriptor.cs`:
   `string Id`, `string DisplayName`,
   `ProviderFamily Family`,
   `ProviderStatus Status`, `string? Version`,
   `IReadOnlyDictionary<string, string>
   Metadata`. The descriptor is the data
   envelope the M4-C surface renders.
3. **The `ProviderFamily` enum** in
   `src/AiEng.Platform.Application/Providers/ProviderFamily.cs`:
   `Git`, `AgentRuntime`, `Review`,
   `QualityGate`, `AutonomousLoop`,
   `Orchestration`. The 6 families map to the
   6 capability families in the M4-B
   `IHostCapabilitiesService` contract.
4. **The `ProviderStatus` enum** in
   `src/AiEng.Platform.Application/Providers/ProviderStatus.cs`:
   `Available`, `Unavailable`, `Disabled`. The
   3 states map to the M4-B `HostCapability.Available`
   + `HostCapability.CredentialAvailable`
   flags.
5. **The family registries** (one per family)
   in
   `src/AiEng.Platform.Application/Providers/Families/`:
   `IGitProviderFamily`, `IAgentRuntimeProviderFamily`,
   `IReviewProviderFamily`,
   `IQualityGateProviderFamily`,
   `IAutonomousLoopProviderFamily`,
   `IOrchestrationProviderFamily`. Each
   family registry has a `ListProvidersAsync`
   method that returns the available
   `ProviderDescriptor`s for the family,
   filtered by host capability.
6. **The `SystemProviderRegistry`
   implementation** in
   `src/AiEng.Platform.Infrastructure/Providers/SystemProviderRegistry.cs`:
   the implementation aggregates the 6
   family registries + consumes the M4-B
   `IHostCapabilitiesService` through DI to
   filter providers by host capability. The
   implementation is the M4-C counterpart of
   the M4-B.1 `SystemHostCapabilitiesService`.
7. **The fakes** (one per family) in
   `tests/AiEng.Platform.UnitTests/Providers/`:
   `FakeGitProviderFamily`,
   `FakeAgentRuntimeProviderFamily`,
   `FakeReviewProviderFamily`,
   `FakeQualityGateProviderFamily`,
   `FakeAutonomousLoopProviderFamily`,
   `FakeOrchestrationProviderFamily`. Each
   fake records the lookup call count and
   returns a configured `ProviderDescriptor`
   list. The fakes are the M4-C test
   surface; the M4-D first concrete providers
   replace the fakes.
8. **The `AddProviderRegistry` composition
   root extension** in
   `src/AiEng.Platform.App/Composition/Providers/ProviderRegistryServiceCollectionExtensions.cs`:
   registers `IProviderRegistry` →
   `SystemProviderRegistry` as a singleton,
   plus the 6 family registries as
   singletons. The composition root is
   called from `AddPlatformServices` after
   `AddInfrastructure` + `AddHostCapabilities`.
9. **The `AppProviderList` data-owning
   four-state design-system component** in
   `src/AiEng.Platform.App/Components/Providers/AppProviderList.razor`
   (+ `.razor.cs` + `.razor.css`): exposes
   the four data-owning slots `Loading` /
   `Empty` / `Error` / `Populated` per the
   M1.2 design system rule (`docs/design-system.md`
   § 5.4); renders one `.app-provider-list-item`
   per `ProviderDescriptor` in the
   `Providers` parameter, grouped by
   `ProviderFamily`; each item shows a status
   dot (available = green, unavailable = red,
   disabled = grey), the `DisplayName`, the
   `Version` (when non-null), and the
   `Metadata` as a small key-value list. The
   component is the M4-C counterpart of the
   M4-B.2 `AppCapabilityList`.
10. **The `/providers` page** at
    `src/AiEng.Platform.App/Components/Pages/Providers.razor`
    (+ `.razor.css`): `@page "/providers"`;
    `@attribute [RouteMetadata("/providers",
    "Providers", Order = 5, ShowInSidebar =
    true, Icon = "◇", Description = "Available
    providers grouped by family; filtered by
    host capability.")]` (icon per M4-C
    plan; order 5 follows the M3 logical
    ordering with `Order = 0` (Dashboard) →
    `Order = 1` (Projects) → `Order = 4`
    (Diagnostics) → `Order = 5` (Providers));
    `@layout AppLayout` +
    `@rendermode InteractiveServer` +
    `@inject IProviderRegistry Service` +
    `@inject IHostCapabilitiesService
    Capabilities` (the page consumes both the
    registry + the M4-B capability service
    for the host-metadata context). The page
    composes the `AppPageHeader` with
    `Breadcrumbs` + `Title` + `Description` +
    `Actions` (the `Actions` slot holds a
    Refresh `AppButton` that re-runs the
    registry lookup); the page renders an
    `AppProviderList` per family (6 lists in
    total).
11. **The startup provider-registry log** in
    `src/AiEng.Platform.App/Program.cs`: the
    M4-C counterpart of the M4-B startup
    capability-report log. The block is
    inserted after the M4-B
    `LogHostCapabilitiesAsync` call. The
    block resolves `IProviderRegistry` +
    `ILogger<Program>` from `app.Services`;
    uses a 10-second `CancellationTokenSource`;
    calls `Service.ListProvidersAsync(ProviderFamily.Git,
    cts.Token)` for each of the 6 families;
    logs the per-family provider count at
    `Information` level. The block is wrapped
    in a `try/catch` that logs failures at
    `Warning` level. The startup must not
    fail if the registry lookup fails.
12. **The `Providers_Resolve_Through_Registry`
    Active architecture test** in
    `tests/AiEng.Platform.ArchitectureTests/Providers/Providers_Resolve_Through_Registry.cs`:
    the M4-C counterpart of the M4-B.3
    `Capabilities_Resolved_Through_Service`
    test. The test is **Active** (not
    `[Fact(Skip=...)]`) per the M4-C plan.
    The test has 2 `[Fact]` methods: (1)
    `Providers_page_resolves_providers_through_IProviderRegistry`
    asserts `Providers.razor` contains
    `@inject IProviderRegistry` and does not
    contain the forbidden tokens
    `RunToCompletionAsync` / `ICredentialVault`
    / `new SystemProviderRegistry` /
    `IHostCapabilitiesService` direct call
    (the page may inject `IHostCapabilitiesService`
    for the host-metadata context, but the
    test asserts no direct `DetectAsync` call
    on the page; the page may read
    `Capabilities` only for the metadata
    context, not for the registry lookup);
    (2) `Providers_folder_does_not_reference_process_or_credential_boundary_directly`
    scans every `.razor` + `.razor.cs` file
    under `src/AiEng.Platform.App/Components/Providers/`
    for the same forbidden tokens. The test
    is scoped to the `Providers` folder to
    avoid the M4-A.2 Open Action +
    M4-B.3 `Diagnostics.razor` false
    positives.
13. **The `docs/providers.md`
    documentation** (new, 10 sections
    mirroring `docs/infrastructure.md` +
    `docs/capabilities.md` § 1-10: Goals,
    Project Structure, Contract, Records,
    Family Registries, Component, Page,
    Composition Root, Tests, Out of Scope).
14. **Update `docs/design-system.md` § 4.5**:
    the `AppProviderList` row from
    `Planned (M4-C)` to `Implemented (M4-C.2)`
    in the M4-C.2 closeout. The M4-C.1
    first slice does not update
    `docs/design-system.md`; the M4-C.2
    second slice does (per the M4-C plan
    § 11 Documentation Plan).
15. **Update the project-continuity state per
    Rule 15** on every M4-C slice: the 6
    state files (`session.json` +
    `tasks.json` + `current.md` +
    `task-board.md` + `milestones.json` +
    `capabilities.json`).
16. **Write the per-slice handoff + implementation
    report** for every M4-C slice. The
    M4-C.1 + M4-C.2 + M4-C.x handoffs +
    implementation reports mirror the M4-B.1
    + M4-B.2 + M4-B.3 + M4-B.x pattern.
17. **Coherent commit on every M4-C slice's
    feature branch** per Rule 17 + the
    branching strategy. Commit subjects:
    `feat(m4-c.1): add IProviderRegistry
    contract and family registries` (M4-C.1) +
    `feat(m4-c.2): add AppProviderList
    data-owning design-system component and
    /providers page` (M4-C.2) +
    `chore(m4-c.closeout): close M4-C with
    retrospective, M4-D plan, and m4-c
    milestone tag` (M4-C.x). Fast-forward
    merge into `main` per the branching
    strategy rule 6; delete feature branch
    per rule 7; skip push (not authorised in
    this session).
18. **Create the `m4-c` annotated milestone
    tag** at the M4-C closeout commit on
    `main` per the branching strategy rule 9.
    The tag's message references the M4-C
    retrospective path.
19. **Stop.** M4-C does **not** begin M4-D
    or any concrete provider creation. The
    M4-C closeout produces the M4-D plan in
    `Awaiting Approval`. The M4-D plan
    promotion is the M4-D.1 first session's
    responsibility on the user's `Approve`
    or `Next` invocation after the M4-C
    closeout.

## 3. Out of Scope

- **Concrete provider creation.** Per the
  M4-C plan § 1: "M4-C does not implement
  any `Providers.<X>` project. M4-D
  introduces the first concrete
  process-boundary providers." M4-C is the
  registry foundation; M4-D is the
  providers.
- **M4-D plan promotion.** Not in M4-C's
  scope.
- **Provider enable/disable actions.** Not
  in M4-C's scope. The M4-C.2 surface
  displays the available providers; the
  M4-D surface enables / disables the
  providers.
- **Architecture test activation for the
  M4-A process + credential boundaries.**
  The `Infrastructure_Respects_ProcessBoundary`
  + `Infrastructure_Respects_CredentialBoundary`
  tests are registered-but-disabled per
  ADR-016; they activate in M4-D when the
  first `Providers.<X>` project lands.
- **Architecture test activation for the
  4 composition-root provider-boundary
  tests.** The 4
  `CompositionRootBoundaryTests` are
  registered-but-disabled per ADR-016; they
  activate in M4-D.
- **Architecture test activation for the 3
  axe-core tests.** The 3
  `AxeCoreAuditTests` are registered-but-
  disabled per ADR-016; they activate in
  M4-D.
- **New design-system primitives.** M4-C
  composes the M1.2 primitives +
  the M4-B.2 components; M4-C does not
  introduce a new design-system primitive
  (the `AppProviderList` is a new
  data-owning component, not a new
  primitive).
- **Push to remote.** Push is not
  authorised in this session; the remote is
  configured but pushing is not authorised.

## 4. Files to Add

- `src/AiEng.Platform.Application/Providers/IProviderRegistry.cs`
  (the M4-C contract).
- `src/AiEng.Platform.Application/Providers/ProviderDescriptor.cs`
  (the M4-C record).
- `src/AiEng.Platform.Application/Providers/ProviderFamily.cs`
  (the M4-C enum).
- `src/AiEng.Platform.Application/Providers/ProviderStatus.cs`
  (the M4-C enum).
- `src/AiEng.Platform.Application/Providers/Families/IGitProviderFamily.cs`
  + `IAgentRuntimeProviderFamily.cs` +
  `IReviewProviderFamily.cs` +
  `IQualityGateProviderFamily.cs` +
  `IAutonomousLoopProviderFamily.cs` +
  `IOrchestrationProviderFamily.cs` (the 6
  family registries).
- `src/AiEng.Platform.Infrastructure/Providers/SystemProviderRegistry.cs`
  (the M4-C implementation).
- `tests/AiEng.Platform.UnitTests/Providers/FakeGitProviderFamily.cs`
  + `FakeAgentRuntimeProviderFamily.cs` +
  `FakeReviewProviderFamily.cs` +
  `FakeQualityGateProviderFamily.cs` +
  `FakeAutonomousLoopProviderFamily.cs` +
  `FakeOrchestrationProviderFamily.cs` (the
  6 family fakes).
- `src/AiEng.Platform.App/Composition/Providers/ProviderRegistryServiceCollectionExtensions.cs`
  (the M4-C composition root).
- `src/AiEng.Platform.App/Components/Providers/AppProviderList.razor`
  (+ `.razor.cs` + `.razor.css`) (the M4-C
  data-owning four-state design-system
  component).
- `src/AiEng.Platform.App/Components/Providers/_Imports.razor`
  (the M4-C `_Imports.razor`).
- `src/AiEng.Platform.App/Components/Pages/Providers.razor`
  (+ `.razor.css`) (the M4-C page).
- `tests/AiEng.Platform.ComponentTests/Components/Providers/AppProviderListTests.cs`
  (the M4-C bUnit tests for the
  `AppProviderList` component).
- `tests/AiEng.Platform.ComponentTests/Pages/ProvidersPageTests.cs`
  (the M4-C bUnit tests for the `/providers`
  page).
- `tests/AiEng.Platform.ArchitectureTests/Providers/Providers_Resolve_Through_Registry.cs`
  (the M4-C Active architecture test).
- `docs/providers.md` (the M4-C
  documentation; 10 sections).
- `.ai/plans/M4-C-closeout.md` (the M4-C
  closeout plan; 12 sections mirroring the
  M4-B closeout plan).
- `retrospective-m4-c-provider-registry-foundation.md`
  (the M4-C retrospective; 13 sections per
  the Milestone Closeout Standard § 4).
- `implementation-report-m4-c-1-provider-registry-contract-and-family-registries.md`
  + `implementation-report-m4-c-2-provider-list-component-and-page.md`
  + `implementation-report-m4-c-closeout.md`
  (the M4-C.1 + M4-C.2 + M4-C.x implementation
  reports).
- `.ai/handoffs/2026-07-13-m4-c-1-provider-registry-contract-and-family-registries.md`
  + `.ai/handoffs/2026-07-13-m4-c-2-provider-list-component-and-page.md`
  + `.ai/handoffs/2026-07-13-m4-c-closeout.md`
  (the M4-C.1 + M4-C.2 + M4-C.x handoffs).
- `.ai/plans/M4-D-first-concrete-process-providers.md`
  (the M4-D plan; 12 sections; Status:
  Awaiting Approval; the first next-milestone
  plan that the Milestone Closeout Standard's
  § 8 procedure produces after the M4-C
  closeout).

## 5. Files to Modify

- `src/AiEng.Platform.App/Composition/ServiceCollectionExtensions.cs` —
  add the `AddProviderRegistry` call to
  `AddPlatformServices` after
  `AddInfrastructure` + `AddHostCapabilities`.
- `src/AiEng.Platform.App/Program.cs` — add
  the startup provider-registry log block
  after the M4-B `LogHostCapabilitiesAsync`
  call.
- `docs/design-system.md` § 4.5 — add the
  `AppProviderList` row in the M4-C.2
  closeout (the M4-C.1 closeout does not
  update `docs/design-system.md`; the
  M4-C.2 closeout does).
- `.ai/state/session.json` — the M4-C.1 +
  M4-C.2 + M4-C.x envelopes replace the
  M4-C closeout's envelope on every slice.
- `.ai/state/tasks.json` — T-028 (M4-C.1)
  transitions `Ready` → `InProgress` → `Done`
  on the M4-C.1 closeout; T-029 (M4-C.2)
  transitions `Ready` → `InProgress` → `Done`
  on the M4-C.2 closeout; T-030 (M4-C.x)
  transitions `Ready` → `InProgress` → `Done`
  on the M4-C closeout.
- `.ai/state/current.md` — active slice +
  last completed task + next recommended task
  updated on every M4-C slice.
- `.ai/state/task-board.md` — M4-C.1 +
  M4-C.2 + M4-C.x rows in `Done Recently`
  on every slice.
- `.ai/state/milestones.json` — M4-C.1 +
  M4-C.2 + M4-C.x slice blocks from
  `Planned` to `Done` on every slice; M4-C
  milestone `Planned` → `Awaiting Approval`
  → `Active` → `Done` on the M4-C closeout.
- `.ai/state/capabilities.json` — C-010 +
  C-011 + C-018 evidence blocks finalised on
  the M4-C.1 + M4-C.2 + M4-C closeouts.
- `ROADMAP.md` — M4-C row `Planned` →
  `Awaiting Approval` → `Active` → `Done` on
  the M4-C closeout; M4-C DoD bullets
  checked; M4-C closeout status added.
- `.ai/plans/master-delivery-plan.md` —
  M4-C row `Planned` → `Awaiting Approval` →
  `Active` → `Done` on the M4-C closeout;
  M4-C closeout slice row added; M4-C
  evidence list updated; M4-C
  next-milestone-enabled updated to M4-D.

## 6. Critical Files to Read Before Editing

- `AGENTS.md` — the 17 non-negotiable
  rules; specifically Rule 13 (no code
  comments), Rule 15 (project-continuity
  state), Rule 16 (scope discipline), Rule
  17 (evidence of completion).
- `.ai/session-start.md` — the 13-step
  lifecycle.
- `.ai/workflows/progressive-coding.md` — the
  Progressive Coding Rule.
- `.ai/workflows/branching-strategy.md` — the
  12 rules; specifically rules 4, 6, 7, 9.
- `.ai/workflows/milestone-closeout.md` — the
  canonical Milestone Closeout Standard.
- `.ai/plans/M4-A-infrastructure-process-execution.md`
  — the M4-A plan (the M4-C plan mirrors the
  M4-A plan's 12 sections with M4-C-specific
  evidence).
- `.ai/plans/M4-B-capability-detection.md` —
  the M4-B plan (the M4-C plan mirrors the
  M4-B plan's 12 sections with M4-C-specific
  evidence; the M4-C plan is the M4-B
  plan's "next concrete step").
- `.ai/plans/M4-B-closeout.md` — the M4-B
  closeout plan (the M4-C closeout plan
  mirrors the M4-B closeout plan's 12
  sections with M4-C-specific evidence).
- `retrospective-m4-b-capability-detection.md`
  — the M4-B retrospective (the M4-C
  retrospective mirrors the M4-B
  retrospective's 13 sections with M4-C-
  specific evidence; the M4-B retrospective's
  § 13 Recommendations for the Next Milestone
  is the input to the M4-C plan).
- `implementation-report-m4-b-1-host-capabilities-contract-and-service.md`
  + `implementation-report-m4-b-2-capability-list-components.md`
  + `implementation-report-m4-b-3-diagnostics-page-startup-log-and-architecture-test.md`
  — the M4-B.1 + M4-B.2 + M4-B.3 implementation
  reports (the M4-C implementation reports
  mirror the M4-B reports' 15 sections with
  M4-C-specific evidence).
- `.ai/handoffs/2026-07-13-m4-b-closeout.md`
  — the M4-B closeout handoff (the M4-C
  handoffs mirror the M4-B closeout handoff's
  structure).
- `src/AiEng.Platform.Application/Capabilities/IHostCapabilitiesService.cs`
  — the M4-B contract (the M4-C
  `SystemProviderRegistry` consumes this
  contract through DI).
- `src/AiEng.Platform.Application/Capabilities/HostCapabilities.cs`
  — the M4-B record (the M4-C
  `SystemProviderRegistry` reads the
  `Capabilities` list to filter providers by
  host capability).
- `src/AiEng.Platform.Infrastructure/Capabilities/SystemHostCapabilitiesService.cs`
  — the M4-B implementation (the M4-C
  `SystemProviderRegistry` does not re-implement
  the M4-B service; the M4-C consumes the M4-B
  through DI).
- `src/AiEng.Platform.App/Components/Diagnostics/AppCapabilityList.razor`
  + `AppKeyValueList.razor` — the M4-B.2
  design-system components (the M4-C
  `AppProviderList` component mirrors the
  `AppCapabilityList`'s data-owning
  four-state pattern).
- `src/AiEng.Platform.App/Components/Pages/Diagnostics.razor`
  — the M4-B.3 page (the M4-C
  `Providers.razor` page mirrors the
  `Diagnostics.razor` pattern with the
  `AppProviderList` content).
- `src/AiEng.Platform.App/Composition/ServiceCollectionExtensions.cs`
  — the M4-C composition root is called from
  `AddPlatformServices` after
  `AddInfrastructure` + `AddHostCapabilities`.
- `src/AiEng.Platform.App/Composition/Capabilities/CapabilitiesServiceCollectionExtensions.cs`
  — the M4-B composition root (the M4-C
  composition root mirrors the M4-B
  composition root's pattern).
- `src/AiEng.Platform.Application/Navigation/RouteMetadataAttribute.cs`
  + `RouteRegistry.cs` — the M2.2 navigation
  registry (the M4-C `/providers` page
  registers through `[RouteMetadata]`).
- `src/AiEng.Platform.Application/Infrastructure/IPlatformInfo.cs`
  — the M4-A.1 host metadata contract (the
  M4-C `/providers` page may inject
  `IPlatformInfo` for the host-metadata
  context, like the M4-B.3
  `Diagnostics.razor`).
- `src/AiEng.Platform.App/Components/Primitive/AppButton.razor`
  + `AppButton.razor.cs` — the Refresh
  button (the M4-C `/providers` page uses
  the same Refresh pattern as the M4-B.3
  `Diagnostics.razor`).
- `src/AiEng.Platform.App/Components/Display/AppPageHeader.razor`
  — the page header (the M4-C `/providers`
  page uses the same `AppPageHeader` pattern
  as the M4-B.3 `Diagnostics.razor`).
- `src/AiEng.Platform.App/Components/Layout/AppCard.razor`
  — the layout primitive (the M4-C
  `/providers` page composes an `AppCard`
  per family, like the M4-B.3
  `Diagnostics.razor` composes an `AppCard`
  per capability + metadata block).
- `tests/AiEng.Platform.UnitTests/Capabilities/SystemHostCapabilitiesServiceTests.cs`
  — the M4-B.1 unit tests (the M4-C.1 unit
  tests mirror the M4-B.1 unit tests'
  pattern with M4-C-specific evidence).
- `tests/AiEng.Platform.ComponentTests/Components/Diagnostics/AppCapabilityListTests.cs`
  + `AppKeyValueListTests.cs` — the M4-B.2
  bUnit component tests (the M4-C.2 bUnit
  component tests for `AppProviderList`
  mirror the M4-B.2 tests' pattern).
- `tests/AiEng.Platform.ComponentTests/Pages/DiagnosticsPageTests.cs`
  — the M4-B.3 bUnit page tests (the M4-C.2
  bUnit page tests for `Providers.razor`
  mirror the M4-B.3 tests' pattern).
- `tests/AiEng.Platform.ArchitectureTests/Capabilities/Capabilities_Resolved_Through_Service.cs`
  — the M4-B.3 Active architecture test
  (the M4-C.2 Active architecture test
  `Providers_Resolve_Through_Registry`
  mirrors the M4-B.3 test's pattern with
  M4-C-specific evidence; the M4-C test is
  scoped to `App/Components/Providers/` to
  avoid the M4-B.3 + M4-A.2 false positives).
- `docs/infrastructure.md` + `docs/capabilities.md`
  — the M4-A + M4-B documentation (the M4-C
  `docs/providers.md` mirrors the M4-A +
  M4-B docs' 10 sections with M4-C-specific
  evidence).
- `docs/design-system.md` § 4.5 — the
  Domain components table (the M4-C.2
  closeout updates the `AppProviderList`
  row from `Planned (M4-C)` to
  `Implemented (M4-C.2)`).
- `.editorconfig` — CRLF + 4-space indent
  for `.cs`/`.razor`/`.json`/`.md` (use
  `unix2dos` on every new file).

## 7. Existing Functions and Utilities to Reuse

- The **M4-B `IHostCapabilitiesService`**
  contract is the only DI seam the M4-C
  `SystemProviderRegistry` uses to fetch
  host capabilities. The
  `Providers_Resolve_Through_Registry`
  architecture test enforces this (the
  test forbids `new SystemHostCapabilitiesService`
  direct-instantiation escape hatch on
  `Providers.razor` + on every file in
  `App/Components/Providers/`).
- The **M4-B `HostCapabilities` +
  `HostCapability` records** are the data
  envelope the M4-C `SystemProviderRegistry`
  consumes to filter providers by host
  capability. The M4-C `SystemProviderRegistry`
  reads the `Capabilities` list and maps
  each `HostCapability.Key` to a
  `ProviderDescriptor` (the mapping is the
  M4-C's family-specific configuration; the
  M4-C.1 closeout documents the mapping).
- The **M4-B.2 `AppCapabilityList`
  data-owning component** is the pattern
  template for the M4-C.2
  `AppProviderList` component. The
  `AppProviderList` exposes the four
  data-owning slots `Loading` / `Empty` /
  `Error` / `Populated` per the M1.2 design
  system rule (`docs/design-system.md`
  § 5.4).
- The **M1.2 `AppPageHeader`** is the
  page header primitive; the M4-C
  `Providers.razor` page uses it with
  `<Breadcrumbs>` + `<Title>` +
  `<Description>` + `<Actions>` (the
  `Actions` slot holds the Refresh
  `AppButton`).
- The **M1.2 `AppButton`** is the
  Refresh action (mirrors the M4-B.3
  `Diagnostics.razor` Refresh pattern).
- The **M1.2 `AppCard`** is the
  layout primitive; the M4-C
  `Providers.razor` page composes an
  `AppCard` per family, like the M4-B.3
  `Diagnostics.razor` composes an
  `AppCard` per capability + metadata
  block.
- The **M1.2 `AppBreadcrumb`** is the
  breadcrumb.
- The **M2.4 `Dashboard.razor` +
  M3 `Projects.razor` + M4-B.3
  `Diagnostics.razor` page pattern** is
  the M4-C `Providers.razor` page
  pattern (`@page` +
  `[RouteMetadata]` + `@layout AppLayout`
  + `@rendermode InteractiveServer` +
  `@using` + `@inject` +
  `OnInitializedAsync` + `AppPageHeader`
  + `AppCard` + data-owning component).
- The **M4-A.1 `IPlatformInfo`** is the
  host metadata contract; the M4-C
  `/providers` page may inject
  `IPlatformInfo` for the host-metadata
  context (the M4-C test allows
  `IPlatformInfo`, like the M4-B.3 test
  allows it).
- The **bUnit test pattern** in
  `tests/AiEng.Platform.ComponentTests/Pages/DashboardTests.cs`
  + `DiagnosticsPageTests.cs` is the
  pattern the M4-C
  `ProvidersPageTests` follows.
- The **architecture test pattern** in
  `tests/AiEng.Platform.ArchitectureTests/Capabilities/Capabilities_Resolved_Through_Service.cs`
  is the pattern the M4-C
  `Providers_Resolve_Through_Registry`
  test follows.
- The **docs `infrastructure.md` +
  `capabilities.md` structure** (10
  sections) is the pattern the M4-C
  `docs/providers.md` follows.

## 8. Architecture

The M4-C plan composes the M4-A + M4-B + M1.2 +
M2.2 + M2.4 + M3 seams. The M4-C plan introduces
no new architectural rules (no new ADR is
required).

The M4-C `IProviderRegistry` contract is the
single DI seam for provider lookup. The
`Providers_Resolve_Through_Registry`
architecture test enforces the seam: every
file in `App/Components/Providers/` may
reference `IProviderRegistry` (the only
allowed seam); no file in
`App/Components/Providers/` may reference
`RunToCompletionAsync` (forbidden
`IProcessRunner` usage), `ICredentialVault`
(forbidden `ICredentialVault` usage), or
`new SystemProviderRegistry` (forbidden
direct-instantiation escape hatch).

The M4-C `SystemProviderRegistry`
implementation composes the M4-B
`IHostCapabilitiesService` through DI to
filter providers by host capability. The
M4-C plan does not introduce
`System.Diagnostics.Process` usage outside
`src/AiEng.Platform.Infrastructure/`; the
M4-C plan does not introduce
`advapi32.dll` P/Invoke outside
`src/AiEng.Platform.Infrastructure/`; the
M4-C plan does not introduce a
`Microsoft.Extensions.DependencyInjection`
`IServiceCollection` extension outside
`src/AiEng.Platform.App/Composition/`. The
boundary is enforced by the M4-A.1
architecture tests
(`Infrastructure_Respects_ProcessBoundary`
+ `Infrastructure_Respects_CredentialBoundary`),
which are registered-but-disabled per
ADR-016 / M4-D.

## 9. Risks and Mitigations

| Risk | Mitigation |
| --- | --- |
| The M4-C `Providers.razor` page may bypass the `IProviderRegistry` seam by directly injecting `IProcessRunner` or `ICredentialVault`. | The M4-C.2 architecture test `Providers_Resolve_Through_Registry` (Active, scoped to `App/Components/Providers/`) asserts the seam is the only allowed path. The test fails the build if the page or any future change to `App/Components/Providers/` bypasses the seam. |
| The M4-C architecture test's forbidden-token list may produce a false positive on the M4-A.2 Open Action (`AppProjectCard.razor` is in `App/Components/Projects/`, not in `App/Components/Providers/`) or the M4-B.3 `Diagnostics.razor` page (`Diagnostics.razor` is in `App/Components/Pages/`, not in `App/Components/Providers/`). | The test is scoped to `App/Components/Providers/` (the M4-C folder), not `App/Components/` (the whole App project). The M4-A.2 Open Action is in `App/Components/Projects/`, the M4-B.3 `Diagnostics.razor` is in `App/Components/Pages/`; both are outside the scoped directory. |
| The M4-C startup provider-registry log may fail the application startup if the registry lookup times out or throws. | The log block is wrapped in a `try/catch` that logs failures at `Warning` level. The startup must not fail if the registry lookup fails; the log is the early signal, the `/providers` page is the user-visible surface. The 10-second timeout is enforced via a `CancellationTokenSource(TimeSpan.FromSeconds(10))`. |
| The M4-C `SystemProviderRegistry` may attempt to consume host capabilities on a non-Windows host. | The M4-A.1 `IPlatformInfo.IsWindows` is the host-OS check. The M4-C `SystemProviderRegistry` does not branch on `IPlatformInfo.IsWindows`; the M4-C consumes the M4-B `IHostCapabilitiesService` which is host-OS-agnostic (the M4-B.1 `SystemHostCapabilitiesService` branches on `IPlatformInfo.IsWindows` internally; the M4-B surface exposes a uniform `HostCapabilities` record). The M4-C consumes the uniform surface. |
| The M4-C plan may overlap with the M4-D plan. | The M4-C plan is the registry foundation; the M4-D plan is the first concrete providers. The M4-C plan stops at the registry + the fakes; the M4-D plan introduces the first `Providers.<X>` project. The M4-C closeout produces the M4-D plan in `Awaiting Approval`; the M4-D.1 first session approves the M4-D plan and begins the M4-D implementation. |
| The M4-C `AppProviderList` may not handle the 6 family groupings gracefully. | The `AppProviderList` component renders the providers grouped by `ProviderFamily`; the M4-C.2 bUnit tests cover the 6-family grouping + the Loading + Empty + Error + Populated slots. The visual smoke is best-effort; the bUnit tests are the canonical evidence. |
| The M4-C `Providers.razor` page's `IHostCapabilitiesService` injection may trigger the `Providers_Resolve_Through_Registry` architecture test (which forbids `IHostCapabilitiesService` direct call on `Providers.razor`). | The test allows `IHostCapabilitiesService` injection on `Providers.razor` (the test forbids `RunToCompletionAsync` / `ICredentialVault` / `new SystemProviderRegistry`, not `IHostCapabilitiesService`). The test asserts no direct `DetectAsync` call on the page; the page may inject `IHostCapabilitiesService` for the host-metadata context, but the page's registry lookup is through `IProviderRegistry`, not through `IHostCapabilitiesService`. |
| The M4-C `Providers.razor` page may fail the `PagesAreReachableThroughRegistryTests` test if the `[RouteMetadata]` attribute is missing or the Href is not registered. | The page adds `[RouteMetadata("/providers", "Providers", Order = 5, ShowInSidebar = true, Icon = "◇", Description = "Available providers grouped by family; filtered by host capability.")]` per the M4-C plan § 2 item 10. The `RouteRegistry` constructor in `AddNavigation(assemblies)` picks it up automatically. |
| The M4-C `Providers.razor` page may fail the `PagesUseDesignSystemComponentsTests` test (no literal `<button>` or inline `style` attributes). | The page uses `<AppButton>` for the Refresh action (no literal `<button>`); the inline `@using` directives are not style attributes. The test passes. |
| The M4-C closeout may push to the remote. | Push is not authorised in this session; the M4-C closeout does not push. The push decision is **Staged for push**; the next user command may push. |

## 10. Test Plan

The M4-C test plan is structured around the
two-slices-per-milestone pattern (contract in
M4-C.1; surface in M4-C.2; closeout in M4-C.x)
+ the architecture test in M4-C.2.

### 10.1 Unit tests (M4-C.1)

The M4-C.1 first slice ships 9+ unit tests for
the `SystemProviderRegistry` implementation:

- `SystemProviderRegistry_aggregates_family_registries`
  (asserts the implementation calls all 6
  family registries; aggregates the results).
- `SystemProviderRegistry_filters_by_host_capability`
  (asserts the implementation consumes the M4-B
  `IHostCapabilitiesService` through DI; filters
  out providers whose host capability is
  `Unavailable`).
- `SystemProviderRegistry_returns_empty_for_unregistered_family`
  (asserts the implementation returns an empty
  list for a `ProviderFamily` with no family
  registry).
- `SystemProviderRegistry_propagates_cancellation`
  (asserts the implementation propagates a
  cancelled `CancellationToken` to the family
  registries).
- `SystemProviderRegistry_logs_at_Information_level_on_success`
  (asserts the implementation logs the
  per-family provider count at `Information`
  level; the test uses a
  `FakeLogger<SystemProviderRegistry>`).
- `SystemProviderRegistry_logs_at_Warning_level_on_failure`
  (asserts the implementation logs a failure at
  `Warning` level; the test uses a
  `FakeLogger<SystemProviderRegistry>`).
- `FakeGitProviderFamily_records_LookupCallCount`
  (asserts the fake records the lookup call
  count; the test asserts the count is 1 after
  one lookup).
- `FakeAgentRuntimeProviderFamily_returns_configured_descriptors`
  (asserts the fake returns the configured
  `ProviderDescriptor` list; the test asserts
  the 3 configured descriptors are returned).
- `FakeReviewProviderFamily_propagates_cancellation`
  (asserts the fake propagates a cancelled
  `CancellationToken`).

### 10.2 bUnit component tests (M4-C.2)

The M4-C.2 second slice ships 7+ bUnit tests
for the `AppProviderList` component:

- `AppProviderList_Loading_state_renders_AppLoading`
  (asserts the component renders
  `AppLoading` when `IsLoading = true`).
- `AppProviderList_Empty_state_renders_AppEmptyState`
  (asserts the component renders
  `AppEmptyState` when `Providers.Count = 0`
  and `IsLoading = false`).
- `AppProviderList_Error_state_renders_AppErrorState`
  (asserts the component renders
  `AppErrorState` when `ErrorMessage` is set).
- `AppProviderList_Populated_state_renders_ProviderListItems`
  (asserts the component renders one
  `.app-provider-list-item` per
  `ProviderDescriptor` when `Providers.Count > 0`
  and `IsLoading = false` and `ErrorMessage`
  is `null`).
- `AppProviderList_groups_by_Family`
  (asserts the component groups the
  `ProviderDescriptor`s by `ProviderFamily`;
  the test uses 6 descriptors across 2 families).
- `AppProviderList_renders_StatusDot_per_descriptor`
  (asserts the component renders a status dot
  per `ProviderDescriptor`; the test asserts
  the status dot's colour matches the
  `ProviderStatus` value).
- `AppProviderList_renders_Version_per_descriptor`
  (asserts the component renders the `Version`
  for each `ProviderDescriptor`; the test
  asserts the `Version` is rendered as a
  small badge).

### 10.3 bUnit page tests (M4-C.2)

The M4-C.2 second slice ships 4+ bUnit page
tests for the `/providers` page:

- `ProvidersPage_calls_ListProvidersAsync_on_init`
  (asserts `OnInitializedAsync` calls
  `ListProvidersAsync` for each of the 6
  families).
- `ProvidersPage_renders_AppProviderList_per_family`
  (asserts the page renders 6
  `AppProviderList`s, one per family).
- `ProvidersPage_Refresh_button_reruns_ListProvidersAsync`
  (asserts the Refresh button re-runs
  `ListProvidersAsync` for each of the 6
  families; the call count doubles).
- `ProvidersPage_renders_host_metadata`
  (asserts the page renders the host metadata
  context block — Detected at, Data directory,
  Config directory, Is Windows host — from the
  M4-B `IHostCapabilitiesService`).

### 10.4 Architecture tests (M4-C.2)

The M4-C.2 second slice ships 1 Active
architecture test with 2 `[Fact]` methods:

- `Providers_Resolve_Through_Registry.cs`:
  - `Providers_page_resolves_providers_through_IProviderRegistry`
    (asserts `Providers.razor` contains
    `@inject IProviderRegistry` and does not
    contain `RunToCompletionAsync` /
    `ICredentialVault` / `new
    SystemProviderRegistry` / direct
    `IHostCapabilitiesService.DetectAsync` call).
  - `Providers_folder_does_not_reference_process_or_credential_boundary_directly`
    (scans every `.razor` + `.razor.cs` file
    under
    `src/AiEng.Platform.App/Components/Providers/`
    for the same forbidden tokens).

### 10.5 Test count progression

| Stage            | Unit | Component | Architecture | Total | Skipped |
| ---------------- | ---- | --------- | ------------ | ----- | ------- |
| M4-B closeout    | 99   | 263       | 14           | 376   | 9       |
| M4-C.1 closeout  | 108  | 263       | 14           | 385   | 9       |
| M4-C.2 closeout  | 108  | 270       | 16           | 394   | 9       |
| M4-C closeout    | 108  | 270       | 16           | 394   | 9       |

## 11. Documentation Plan

The M4-C documentation plan is structured around
the two-slices-per-milestone pattern (contract
in M4-C.1; surface in M4-C.2; closeout in
M4-C.x) + the documentation in M4-C.1.

### 11.1 `docs/providers.md` (M4-C.1)

The M4-C.1 first slice ships the 10-section
`docs/providers.md` documentation:

- § 1 Goals — the M4-C purpose (provider
  registry foundation; the M4-C is the
  consumer of the M4-B
  `IHostCapabilitiesService`).
- § 2 Project Structure —
  `AiEng.Platform.Application/Providers/`,
  `AiEng.Platform.Infrastructure/Providers/`,
  `AiEng.Platform.App/Composition/Providers/`,
  `AiEng.Platform.App/Components/Providers/`.
- § 3 `IProviderRegistry` (the contract) —
  `Task<IReadOnlyList<ProviderDescriptor>>
  ListProvidersAsync(ProviderFamily family,
  CancellationToken ct)`.
- § 4 `ProviderDescriptor` (the record) —
  the 5 fields + the 6 `ProviderFamily`
  values + the 3 `ProviderStatus` values.
- § 5 Family Registries — the 6 family
  registries; the per-family
  `ListProvidersAsync` method.
- § 6 `AppProviderList` Component (M4-C.2) —
  data-owning four-state; the 6 family
  groupings; the status dot per
  `ProviderDescriptor`.
- § 7 `/providers` Page (M4-C.2) — the
  user-visible surface; the `AppPageHeader`
  + the 6 `AppProviderList`s; the Refresh
  button.
- § 8 Composition Root —
  `AddProviderRegistry`; the DI chain order
  (after `AddInfrastructure` +
  `AddHostCapabilities`); the
  `TryAddSingleton<IProviderRegistry,
  SystemProviderRegistry>` registration.
- § 9 Tests — the test inventory (M4-C.1
  unit tests, M4-C.2 bUnit tests for the
  component + the page, M4-C.2 architecture
  test for the seam).
- § 10 Out of Scope — the M4-D / M5+ items
  (concrete provider creation, provider
  enable/disable actions, autonomous
  loops).

### 11.2 `docs/design-system.md` § 4.5 update
(M4-C.2)

The M4-C.2 second slice updates the
`AppProviderList` row from
`Planned (M4-C)` to `Implemented (M4-C.2)`.
The `Notes` column changes from
`Renders `RuntimeProviders`` to
`Renders `ProviderDescriptor[]` from
`IProviderRegistry.ListProvidersAsync`;
data-owning four-state`. (Resolves the
M4-C.1 deferred decision per the M4-C.1
handoff § 7.)

## 12. Coherent Commit + Merge

The M4-C implementation's work is one coherent
commit per slice per Rule 17 + the branching
strategy. The commit messages are:

- `feat(m4-c.1): add IProviderRegistry contract
  and family registries` (M4-C.1 closeout
  commit).
- `feat(m4-c.2): add AppProviderList
  data-owning design-system component and
  /providers page` (M4-C.2 closeout commit).
- `chore(m4-c.closeout): close M4-C with
  retrospective, M4-D plan, and m4-c
  milestone tag` (M4-C closeout commit).

The M4-C.1 + M4-C.2 + M4-C closeout commits
live on the per-slice feature branches:

- `feature/T-028-m4-c-1-provider-registry-contract-and-family-registries`
  (M4-C.1 branch; created from `main` at the
  M4-B closeout commit).
- `feature/T-029-m4-c-2-provider-list-component-and-page`
  (M4-C.2 branch; created from `main` at the
  M4-C.1 closeout commit).
- `feature/T-030-m4-c-closeout` (M4-C closeout
  branch; created from `main` at the M4-C.2
  closeout commit).

The per-slice feature branches are
fast-forwarded into `main` per the branching
strategy rule 6; the per-slice feature branches
are deleted per rule 7.

A milestone tag is created at the M4-C closeout
commit on `main` per the branching strategy
rule 9. The tag name is `m4-c`. The tag is
annotated and carries the M4-C retrospective
path in its message.

Push is **not** authorised in this session; the
M4-C.1 + M4-C.2 + M4-C closeout sessions do not
push. The push decision is **Staged for push**.

---

**End of M4-C plan.** The M4-C plan is in
`Awaiting Approval`; the M4-C.1 first session
reviews and revises the plan as needed and
begins the M4-C.1 implementation per the
Progressive Coding Rule. The next session
approves the M4-C plan and begins the M4-C.1
implementation on the user's `Approve` or
`Next` invocation.
