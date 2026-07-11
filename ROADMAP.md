# ROADMAP.md

> The ordered journey from an empty repository to a production AI
> Engineering Platform. This document is read **after** `AGENTS.md` and
> `ARCHITECTURE.md`. It does not define the rules; it sequences the work
> that the rules govern.

---

## 1. How to Read This Roadmap

The roadmap is divided into **milestones**. Each milestone:

- Has a clear, demonstrable outcome.
- Builds only on the work of previous milestones.
- Introduces specific reusable components, services, and providers.
- Is a reasonable unit of work for one to three focused sessions.

Milestones are ordered by **enablement**, not by feature size. The first
milestone is small because it must be **unblock everything that comes
after it**.

A milestone is **complete** only when:

1. Its outcome is observable (you can run it and see it work).
2. Its components are documented in `docs/design-system.md` and
   `docs/component-guidelines.md`.
3. Its providers are documented in `docs/provider-guidelines.md`.
4. Its decisions are recorded in `DECISIONS.md`.
5. Its tests are green.
6. The progressive self-dogfooding matrix in § 4 is satisfied
   for the capabilities the milestone consumes.

---

## 2. Milestone Map

| #   | Name                                                  | Status     | Primary outcome                                                            |
| --- | ----------------------------------------------------- | ---------- | -------------------------------------------------------------------------- |
| M0  | Doc foundation                                        | Done       | This document set exists.                                                  |
| M1  | Design System Core                                    | **Done (closed 2026-07-10)** | A runnable Blazor shell with the base design-system components.            |
| M2  | Application Shell and Navigation                      | Active (M2.1 / M2.2 / M2.3 / M2.4 / M2.5 delivered 2026-07-11) | A navigable app shell on Windows desktop; pages reach an empty state; the layout is responsive and accessible. |
| M3  | Project Registration                                  | Planned    | A user can register a project; the platform owns a `Project` entity.       |
| M4  | Process Execution, Capability Detection, Provider Registry | Planned | The platform spawns processes safely, detects capabilities, registers providers. Divided into four slices: |
|     | &nbsp;&nbsp;M4-A: Infrastructure / Process Execution   | Planned    | `AiEng.Platform.Infrastructure` lands; `IProcessRunner`, `ICredentialVault`, `IClock`, on-disk `IProjectStore`. |
|     | &nbsp;&nbsp;M4-B: Capability Detection                | Planned    | `IHostCapabilitiesService` detects `git`, `ollama`, `powershell.exe`, `wsl.exe`, `wt.exe`, `bash.exe`. |
|     | &nbsp;&nbsp;M4-C: Provider Registry Foundation        | Planned    | `IProviderRegistry`, family-scoped registries, fake providers for every family, composition root `App/Composition/`. |
|     | &nbsp;&nbsp;M4-D: First Concrete Process Providers    | Planned    | `GitProvider`, `OllamaLaunchProvider` smoke, `ProviderContractTests`; composition-root architecture tests activate. |
| M5  | Native Git Worktrees                                   | Planned    | A worktree provider built on `git worktree` consumes `IGitProvider`.       |
| M6  | Agent Runtime Launching                               | Planned    | The first coding-agent vertical slice: Ollama Launch as `IAgentRuntimeProvider`. |
| M7  | Review and Quality Gates                              | Planned    | `IReviewProvider` and `IQualityGateProvider` land with the native baselines. |
| M8  | Autonomous Loops, Orchestration, Production Hardening | Planned    | The remaining families land; the platform is production-ready.             |

M0 is the milestone that ends with this document. The rest are sequenced
in [`docs/architecture-principles.md`](./docs/architecture-principles.md)
and detailed below. **As of 2026-07-11, M1 is done** (see
`implementation-report-m1-closeout.md`); M2 is the active milestone
and is divided into six sequential slices. M2.1 ("Application Shell
Foundation") is **Delivered** (see
`implementation-report-m2-1-application-shell-foundation.md`);
M2.2 ("Navigation Registry and Sidebar") is **Delivered** (see
`implementation-report-m2-2-navigation-registry-sidebar.md`);
M2.3 ("Top Bar, Breadcrumbs, and Page Headers") is **Delivered**
(see `implementation-report-m2-3-topbar-breadcrumbs.md`).
M2.4 ("Project Intelligence Dashboard") is **Delivered** (see
`implementation-report-m2-4-project-intelligence-dashboard.md`).
M2.5 ("Empty Routes, Responsive, and Accessibility") is
**Delivered** (see
`implementation-report-m2-5-empty-routes-responsive-accessibility.md`).
M2.6 is the M2 closeout and external Treehouse dogfooding
checkpoint; the plan stub is recorded in
`.ai/state/task-board.md` and `.ai/state/milestones.json` as
`Deferred`; the next session promotes the stub to a full plan
in `Awaiting Approval` and implements per the plan's own
order.

The sequence is deliberate. M1 builds the design system because every
later milestone composes its components. M2 builds the shell because
every later milestone is reached through it. M3 makes the platform
useful on its own (a registered project) before the architecture
grows. M4 introduces the provider registry and the process-runner
abstraction; the registry is the spine of M5–M8, the process runner
is the only legal way to spawn a host process. M5 proves a provider
that consumes another provider (the native worktree on top of Git).
M6 is the platform's first end-to-end coding-agent feature. M7
adds the review and quality-gate families. M8 lands the remaining
families and the production hardening.

---

## 3. Milestone Details

### M0 — Doc foundation (this milestone)

**Outcome:** The repository contains the engineering standards,
architecture foundation, and reusable development rules. Nothing is
implemented; the rules are in place.

**Delivered in M0:**

- `AGENTS.md`
- `ARCHITECTURE.md`
- `ROADMAP.md`
- `STYLEGUIDE.md`
- `CONTRIBUTING.md`
- `DECISIONS.md`
- `docs/*` (eight design and engineering documents)
- `.ai/prompts/*` (ten prompt templates)
- `.ai/workflows/*` (six workflow procedures)
- `.ai/templates/*` (five reusable templates)

**Definition of done:** A new contributor can read `AGENTS.md` and
understand what to do next. This document is reviewed and merged.

---

### M1 — Design System Core

> **Status: Done (closed 2026-07-10).** See
> `implementation-report-m1-closeout.md` and
> `.ai/handoffs/2026-07-10-m1-closeout.md` for the
> receipt. The 19 M1.2 components, 77 bUnit tests,
> `/design-system` page, and 3 active + 4
> registered-but-disabled architecture tests are
> in place. The M2.1 plan is the next session's
> first action.

**Outcome:** A runnable Blazor shell on .NET 10 that renders the design
system documentation page. The base primitive components exist and are
used by the documentation page. The catalogue is the spec; the
documentation page is its rendering.

**Why it is first after M0:** Everything visible in the platform is built
from the design system. The design system must exist before any feature
page can be honest about its UI. Building it first forces every later
feature to compose components, not invent them.

**Enables:** M2, M3, M4, M5, M6, M7, M8. Every later milestone
composes the design system.

**Solution and project set (per ADR-011):** M1 introduces the
canonical solution and **four source projects** plus **three test
projects**:

- `AiEng.Platform.sln`
- Source: `AiEng.Platform.App`, `AiEng.Platform.Application`,
  `AiEng.Platform.Domain`, `AiEng.Platform.Providers.Abstractions`.
- Tests: `AiEng.Platform.UnitTests`,
  `AiEng.Platform.ComponentTests`,
  `AiEng.Platform.ArchitectureTests`.

Two projects that the architecture will eventually need are
**deferred** (per ADR-011):

- `AiEng.Platform.Infrastructure` is created in M4, when
  `IProcessRunner`, `ICredentialVault`, `IClock`, and
  persistence are first consumed.
- `AiEng.Platform.ProviderContractTests` is created in M4,
  when the first concrete providers land: the `GitProvider`
  (smoke test for the family contract and the
  `IProcessRunner` seam) and the `OllamaLaunchProvider`
  (smoke test for the process boundary and the
  `ollama launch claude --model minimax-m3:cloud` command).
  The first contract tests live in this project; both providers
  ship together because both share the same `IProcessRunner`
  consumer surface.

Provider implementation projects
(`AiEng.Platform.Providers.<X>`) are deferred to the milestone
that introduces them.

**Reusable components introduced:**

- `AppButton`, `AppIconButton`
- `AppBadge`, `AppStatusDot`
- `AppCard`, `AppSection`
- `AppPageHeader`
- `AppLoading`, `AppSkeleton`
- `AppEmptyState`
- `AppErrorState`

**Services introduced:** None.

**Providers introduced:** None.

**Progressive self-dogfooding (per ADR-013):** M1 is the
first milestone; the matrix in § 4 records the contract
this milestone delivers. The architecture test
`App_DoesNotReference_Providers_Implementations` is the
contract that fails any later milestone that imports a
provider implementation directly from `App` or
`Application`. M1 is also the first milestone that consumes
the precedent: every page built during M1 composes
design-system components only, never raw HTML, and the
`Pages_Use_DesignSystem_Components_Not_DOM` architecture
test is in place from the start. The four-state rule
(`Loading`, `Empty`, `Error`, `Populated`) is **conditional**
on data ownership (per the design-system rule recorded
in `docs/design-system.md` § 5.4 and
`docs/component-guidelines.md` § 4): pure primitives and
presentational containers introduced in M1 do not require
the four state slots; the `AppEmptyState` and
`AppErrorState` introduced in M1 are the fallback surfaces
that data-owning components use.

The four composition-root architecture tests introduced
by ADR-016
(`Only_CompositionRoot_MayReference_ConcreteProviders`,
`Pages_DoNotReference_ConcreteProviders`,
`Application_DoesNotReference_ConcreteProviders`,
`Components_DoNotInject_ConcreteProviders`) are
**registered but disabled in M1**: the test bodies exist
with explicit skip messages referencing the milestone
that will activate them, and the active test suite
documents the activation milestone (M4-D). This is the
same "registered but disabled" pattern M1 uses for
`No_DirectProcessStart_OutsideInfrastructure` (which
activates in M4-D) and for the family-scoped provider
registry tests (which activate in M4-D).

**Definition of done:**

- A Blazor Server project runs and shows a documentation page
  built exclusively from the components above.
- Tailwind is wired with `@apply`-driven semantic classes.
- A bUnit test verifies that `AppButton` renders all variants.
- The solution compiles with the four source projects and the
  three test projects listed under "Solution and project set"
  above.
- The architecture test
  `App_DoesNotReference_Providers_Implementations` is in place
  and green.
- The architecture test
  `Pages_Use_DesignSystem_Components_Not_DOM` is in place and
  green.
- The four composition-root architecture tests
  (`Only_CompositionRoot_MayReference_ConcreteProviders`,
  `Pages_DoNotReference_ConcreteProviders`,
  `Application_DoesNotReference_ConcreteProviders`,
  `Components_DoNotInject_ConcreteProviders`) are
  **registered but disabled**; their skip messages cite
  ADR-016 and M4-D as the activation point.

**Out of scope for M1 (verified preserved):** No
concrete provider implementation projects
(`AiEng.Platform.Providers.<X>`); no `AiEng.Platform.Infrastructure`;
no `AiEng.Platform.ProviderContractTests`; no provider
registry behaviour; no process execution; no persistence;
no `IProjectStore` (M3 introduces the in-memory store; M4-A
migrates to durable storage); no Git, Ollama, or any
external tool integration; no worktrees; no runtime
launching; no review providers; no quality gates.

**Dogfooding checkpoint (M1):** Once a working design-system
documentation page renders, the development team may use
**Lavish Axi** to review UI artefacts where practical. The
session records the review findings in an
`implementation-report.md` or a `review-report.md`. The
platform does not yet implement Lavish Axi as a Provider.
Lavish Axi is an `IReviewProvider` candidate; product
integration is a future task. See
[`.ai/workflows/tool-dogfooding.md`](./.ai/workflows/tool-dogfooding.md)
for the invocation policy.

---

### M2 — Application Shell and Navigation

**Outcome:** The shell has a sidebar, a top bar, and a content panel.
The platform is navigable; every navigation target renders an empty
state that uses the design system. The application layout follows
`docs/ui-principles.md` § 2.1.

**Why it is second:** Without navigation, nothing else can be found.
Without a navigable shell, components cannot be composed into a
professional developer tool.

**Enables:** Every later milestone (all features are reached through
navigation).

**Slice breakdown (M2 is delivered as six sequential slices):**

| Slice | Title                                         | Status         | Major outcome                                                            |
| ----- | --------------------------------------------- | -------------- | ------------------------------------------------------------------------ |
| M2.1  | Application Shell Foundation                  | Delivered (M2.1, 2026-07-11) | Two layouts (`AppLayout`, `AppEmptyLayout`), two placeholder shell components (`AppSidebarSlot`, `AppTopBarSlot`), one presentational helper (`AppShellRegion`), and the M1.1 chrome migration. |
| M2.2  | Navigation Registry and Sidebar               | Delivered (M2.2, 2026-07-11) | `INavigationRegistry`, `RouteMetadata`, `RouteMetadataAttribute`, `RouteRegistry`, `AppSidebar`, `AppSidebarItem`, `AppNavItem`, the `Pages_AreReachable_Through_Registry` architecture test. |
| M2.3  | Top Bar, Breadcrumbs, and Page Headers        | Delivered (M2.3, 2026-07-11) | `AppTopBar`, `AppThemeToggle`, `AppUserAvatarSlot`, `AppBreadcrumb`; theme toggle relocated to the top bar; breadcrumb walks the M2.2 registry's `Parent` chain; page-header integration with the navigation registry. |
| M2.4  | Project Intelligence Dashboard                | Delivered (M2.4, 2026-07-11) | `IProjectIntelligenceReader`, `ProjectIntelligenceSnapshot`, `ProjectIntelligenceReader`, `AddProjectIntelligence`, `Dashboard.razor` at `/dashboard`; the dashboard renders the M0.5-data sections in the **Populated** state and the M3+-data sections in the **Empty** state; the theme toggle bug is fixed (`appTheme.current` JS function; synchronous `IsDark` flip; `JSDisconnectedException` handled); the `Pages_Resolve_State_Through_Reader` architecture test enforces the single-seam rule. |
| M2.5  | Empty Routes, Responsive, and Accessibility   | Delivered (M2.5, 2026-07-11) | All routes reach an `AppEmptyState`; the shell is usable at 1280x720, 1440x900, and 1920x1080; the sidebar narrows progressively below 1280px (8rem at 1024–1279px); the top bar remains horizontal at every breakpoint; the content area scrolls vertically; keyboard navigation works; `aria-current="page"` invariant enforced on the breadcrumb last segment, the active `NavLink`, and the active sidebar link; axe-core audit harness is registered but disabled per ADR-016 / M4-D; the T-017 theme toggle bug is fixed via `@rendermode InteractiveServer` on `AppThemeToggle.razor` (the layout's `@Body` is a `RenderFragment` delegate that Blazor refuses to serialize across the SSR → interactive boundary; the directive on the toggle itself is the minimum-blast-radius fix). |
| M2.6  | M2 Closeout and Treehouse Dogfooding          | Summary entry  | The M2 implementation report, the Treehouse dogfooding checkpoint (per `.ai/workflows/tool-dogfooding.md`), and the closeout commit. |

**Reusable components introduced (across all six slices):**

- `AppSidebar`, `AppSidebarItem`, `AppNavItem` (M2.2)
- `AppTopBar`, `AppBreadcrumb` (M2.3)
- `AppLayout`, `AppEmptyLayout`, `AppSidebarSlot`, `AppTopBarSlot`, `AppShellRegion` (M2.1)
- `AppDialog` (modal; deferred to a later M2 slice or removed if unused)
- `AppTabs`, `AppTab` (deferred to a later M2 slice or removed if unused)

**Services introduced:** `INavigationRegistry`, `RouteMetadata`,
`RouteRegistry` (M2.2). M2.4 introduces
`IProjectIntelligenceReader` as the only M2-introduced read-side
service.

**Providers introduced:** None.

**Progressive self-dogfooding (per ADR-013):** This milestone
consumes the design system components from M1 (`AppButton`,
`AppCard`, `AppPageHeader`, `AppEmptyState`, `AppErrorState`).
The architecture test
`Pages_Use_DesignSystem_Components_Not_DOM` fails the build
if a page file contains a literal `<button>`, `<input>`, or
inline-style attribute (Tailwind class names are
acceptable; raw HTML is not). Sidebar items are data-driven
from a route registry (M2.2), not hard-coded in the layout.
M2.4 reads `.ai/state/*.json` through `IProjectIntelligenceReader`;
no page reads the JSON directly.

**Definition of done:**

- M2.1 is closed: the shell foundation renders; the existing
  M1.1 chrome is migrated to `AppLayout`. (Delivered 2026-07-11;
  `implementation-report-m2-1-application-shell-foundation.md`.)
- M2.2 is closed: sidebar items are data-driven from a registry,
  not hard-coded; the `Pages_AreReachable_Through_Registry`
  architecture test passes. (Delivered 2026-07-11;
  `implementation-report-m2-2-navigation-registry-sidebar.md`.)
- M2.3 is closed: the top bar, breadcrumb, and page header are
  integrated; the theme toggle is relocated to the top bar; the
  `AppTopBar` replaces the M2.1 `AppTopBarSlot` placeholder; the
  `AppBreadcrumb` walks the M2.2 registry's `Parent` chain. (Delivered
  2026-07-11; `implementation-report-m2-3-topbar-breadcrumbs.md`.)
- M2.4 is closed: the `/dashboard` page renders the current
  milestone, the active slice, the test status, and the
  self-awareness state read through `IProjectIntelligenceReader`.
- M2.5 is closed: all routes reach an `AppEmptyState`; the shell
  is usable at 1280x720, 1440x900, and 1920x1080 (per the
  matrix in `docs/ui-principles.md` § 10.1); the top bar
  remains horizontal at every breakpoint; the content area
  scrolls vertically; keyboard navigation works;
  `aria-current="page"` is set on the breadcrumb last segment,
  the active `NavLink`, and the active sidebar link; the
  axe-core audit harness is registered but disabled per
  ADR-016 / M4-D. (Delivered 2026-07-11;
  `implementation-report-m2-5-empty-routes-responsive-accessibility.md`.)
- M2.6 is closed: the M2 implementation report is written; the
  Treehouse dogfooding checkpoint has been exercised (or
  explicitly deferred, recorded, and reasoned); the closeout
  commit is coherent.

**Dogfooding checkpoint (M2):** When Git worktrees are useful
for an isolated task on the shell, the development team may
use **Treehouse** externally to create an isolated development
worktree. The session records the exact commands used. The
session confirms the worktree is local to the developer's
machine and does not modify the upstream repository. Treehouse
is an `IWorktreeProvider` candidate; product integration is
the M5 milestone. See
[`.ai/workflows/tool-dogfooding.md`](./.ai/workflows/tool-dogfooding.md).

---

### M3 — Project Registration

**Outcome:** A user can register a project (a folder on disk) and
the platform owns a `Project` entity in the application layer.
The projects page lists registered projects; a registered
project is the input to every later provider call.

**Why it is third:** Project registration is the smallest
piece of state the platform needs to be useful on its own.
The platform becomes a "thing you can register a project with"
before it becomes a "thing you can launch an agent on". This
ordering also gives M4 a real input: the process runner and
the capability detector work on registered projects.

**Enables:** M4 (process execution targets a registered
project's path), M5 (worktree creation starts from a
registered project), M6 (agent launching runs in a
registered project's context), M7 (review and quality
gates target a registered project), M8 (orchestration
runs across registered projects).

**Reusable components introduced:**

- `AppProjectCard`
- `AppProjectList` (a virtualised list of `AppProjectCard`s)
- `AppEmptyState` (extended for the projects page; the
  empty state is the registration prompt)

**Services introduced:**

- `IProjectService`
- `IProjectStore`

**Providers introduced:** None.

**Progressive self-dogfooding (per ADR-013):** This milestone
introduces the project's first persistence boundary. The
project store is implemented as an **in-memory** store in
M3. The store is **not durable** — projects do not survive
an application restart. The contract
(`IProjectStore`) is the durable seam; the M3 in-memory
store is the smoke test for the contract. Durable
persistence is the responsibility of M4, which introduces
`AiEng.Platform.Infrastructure` and the on-disk
`IProjectStore` implementation that replaces the M3
in-memory store behind the same contract. M3 and M4 are
not in conflict: M3 ships the in-memory store, M4
introduces the durable store. The architecture test
`Application_DoesNotReference_Providers_Implementations`
is in place; the project service does not import a provider
directly. The four-state rule applies: `AppProjectList` is
a data-owning component and exposes `Loading`, `Empty`,
`Error`, `Populated` slots; `AppProjectCard` is a
presentational container and does not.

**Definition of done:**

- A user can register a project (name + folder path).
- The project appears in the projects list.
- The project does **not** persist across application
  restart. The M3 in-memory store is a smoke test for the
  `IProjectStore` contract; the durable store that
  replaces it is the M4 deliverable (see M4-A and M4-D).
  The M3 / M4 boundary is the contract, not the storage
  medium; the M3 store and the M4 store both implement
  `IProjectStore`, and M4-D is the migration from
  in-memory to on-disk behind the same contract.
- The project service returns `Project` entities through
  the application layer; the UI does not touch the store
  directly.

**Dogfooding checkpoint (M3):** When the build and test
suite are stable, the development team may initialise and
run **No Mistakes** as a quality gate against a registered
project. The session shows the resulting workflow to the
user. The session does not yet make No Mistakes mandatory;
the build is still under active construction. No Mistakes
is an `IQualityGateProvider` candidate; product
integration is the M7 milestone. See
[`.ai/workflows/tool-dogfooding.md`](./.ai/workflows/tool-dogfooding.md).

---

### M4 — Process Execution, Capability Detection, Provider Registry (split into M4-A, M4-B, M4-C, M4-D)

**Outcome:** M4 introduces the platform's process boundary,
the host's capability detection, the provider registry, and
the first concrete process-boundary providers. The
milestone is split into four sequential slices — M4-A,
M4-B, M4-C, and M4-D — because the original M4 was too
large to implement safely as one undifferentiated unit. The
four slices are each a reasonable unit of work for one to
three focused sessions; each delivers an independent
artefact that the later slices consume; each has its own
prerequisites, outcome, deliverables, excluded scope,
definition of done, tests, dogfooding checkpoint, and
handoff.

**Why M4 is split:** the previous M4 combined four
independent deliverables — the `Infrastructure` project
with `IProcessRunner` and friends, the host's capability
detection, the provider registry, and the first two
concrete providers (`GitProvider` and `OllamaLaunchProvider`)
— into a single milestone. The combination is too large to
review and too large to roll back if one deliverable
proves problematic. The four slices separate the four
deliverables so that each can land, be reviewed, be
dogfooded, and be rolled back independently.

**M4 — high-level structure:**

| Slice | Focus                                              | Major outcome                                                            |
| ----- | -------------------------------------------------- | ------------------------------------------------------------------------ |
| M4-A  | Infrastructure / Process Execution                 | `AiEng.Platform.Infrastructure` lands with `IProcessRunner`, `ICredentialVault`, `IClock`, and a `IProjectStore` on-disk implementation. |
| M4-B  | Capability Detection                               | The host detects `git`, `ollama`, `powershell.exe`, `wsl.exe`, `wt.exe`, `bash.exe` through `IProcessRunner` and the credential vault. |
| M4-C  | Provider Registry Foundation                       | `IProviderRegistry` and the family-scoped registries are introduced with fake providers; the composition root pattern (per ADR-016) is in place. |
| M4-D  | First Concrete Process Providers                   | `GitProvider` and `OllamaLaunchProvider` land as concrete process-boundary providers, contract tests pass, and the four composition-root architecture tests activate. |

**Enables:** M5 (uses `IProcessRunner` and `IGitProvider`),
M6 (uses `IProviderRegistry` and the detected runtimes),
M7 (uses `ICredentialVault` and the registry), M8 (uses
the registry and persistence).

**Solution and project set:** M4-A adds the
`AiEng.Platform.Infrastructure` project. M4-D adds the
`AiEng.Platform.ProviderContractTests` project and the
first two `AiEng.Platform.Providers.<X>` projects
(`AiEng.Platform.Providers.Git` and
`AiEng.Platform.Providers.Ollama`). The M1 source
project set grows to **five** in M4-A and to **seven**
in M4-D.

**Reusable components introduced (across all four
slices):**

- `AppProviderCard` (M4-C)
- `AppHealthDot` (M4-C)
- `AppCapabilityList` (M4-B)
- `AppKeyValueList` (M4-B)

**Services introduced (across all four slices):**

- `IProviderRegistry` (M4-C, the cross-family registry)
- `IAgentRuntimeProviderRegistry` and the matching
  family-scoped registries (M4-C)
- `IProviderHealthService` (M4-C)
- `IProjectStore` on-disk implementation (M4-A,
  replacing the M3 in-memory store behind the same
  contract)

**Providers introduced (across all four slices):**

- `GitProvider` (concrete `IGitProvider`; uses
  `IProcessRunner`, not direct `Process.Start` calls).
  The contract test framework is established in M4-D;
  `AiEng.Platform.ProviderContractTests` is created in
  M4-D because `GitProvider` is the first provider
  with a contract test. M4-D.
- `OllamaLaunchProvider` (concrete
  `IAgentRuntimeProvider` for the Ollama Launch process
  boundary). The implementation is the smoke test
  (process spawn, model flag, output capture,
  cancellation); the UI surface for the launch flow
  lands in M6. M4-D.

**Progressive self-dogfooding (per ADR-013):** Each
slice delivers a row in the matrix in § 4:

- M4-A: `IProcessRunner`, `ICredentialVault`, `IClock`
  rows.
- M4-B: `IProjectStore` durable row (replaces the M3
  in-memory row).
- M4-C: `IProviderRegistry` row; composition-root
  rule rows (`Only_CompositionRoot_MayReference_ConcreteProviders`,
  `Pages_DoNotReference_ConcreteProviders`,
  `Application_DoesNotReference_ConcreteProviders`,
  `Components_DoNotInject_ConcreteProviders`).
- M4-D: `IGitProvider` row;
  `No_DirectProcessStart_OutsideInfrastructure` row
  (activates in M4-D when the first provider that
  actually calls `IProcessRunner` lands); the
  composition-root rule rows activate in M4-D when
  the first concrete `Providers.<X>` project lands.

#### M4-A — Infrastructure / Process Execution

**Outcome:** The
`AiEng.Platform.Infrastructure` project is created.
`IProcessRunner`, `ICredentialVault`, `IClock`, and the
on-disk `IProjectStore` implementation land. The M3
in-memory `IProjectStore` is replaced by the on-disk
implementation behind the same contract.

**Prerequisites:** M3 is done (the in-memory
`IProjectStore` and the `IProjectService` are in
place).

**Outcome in detail:**

- `AiEng.Platform.Infrastructure` is added to the
  solution.
- `IProcessRunner` is defined in
  `Infrastructure/Process/`. It exposes
  `RunAsync(ProcessRequest, CancellationToken)`
  returning `IAsyncEnumerable<ProcessEvent>` for
  streaming and a one-shot `RunToCompletionAsync`
  for non-streaming use. The contract test covers
  the happy path, the documented failure paths
  (non-zero exit, cancellation, invalid binary),
  and the streaming contract.
- `ICredentialVault` is defined in
  `Infrastructure/Credentials/`. It exposes
  `GetAsync`, `SetAsync`, and `RemoveAsync`. The
  contract test covers the happy path and the
  `null` (no-such-secret) case.
- `IClock` is defined in `Infrastructure/Time/`.
  It exposes `UtcNow`. The contract test verifies
  that the value is monotonic.
- The on-disk `IProjectStore` implementation
  replaces the M3 in-memory store behind the same
  contract. The migration is a one-line DI change
  in `Program.cs`: the M3 in-memory registration
  is replaced by the M4-A on-disk registration.
  The `IProjectService` and the UI are unchanged.

**Excluded scope:** capability detection (M4-B),
the provider registry (M4-C), the first concrete
providers (M4-D). M4-A does not introduce a single
`Providers.<X>` project. No `Process.Start` call
may be made from outside `Infrastructure/Process/`
yet, but no such call exists either; the
`No_DirectProcessStart_OutsideInfrastructure` test
is registered but disabled and activates in M4-D.

**Definition of done:**

- `AiEng.Platform.Infrastructure` exists; the
  solution compiles; the M1 / M3 tests remain green.
- `IProcessRunner` and the on-disk `IProjectStore`
  are added; the M3 in-memory store is removed.
- The on-disk `IProjectStore` round-trips a project
  through a save / load / list cycle.
- `ICredentialVault` round-trips a secret through
  the Windows Credential Manager.
- `IClock` is in use everywhere a timestamp is
  produced.
- The architecture test
  `No_DirectProcessStart_OutsideInfrastructure` is
  **registered but disabled**; the activation
  milestone is M4-D.

**Tests added:**

- Contract tests for `IProcessRunner`,
  `ICredentialVault`, `IClock`.
- Integration tests for the on-disk
  `IProjectStore` (round-trip a project).
- Regression tests: every M3 unit test that
  exercised the in-memory store must remain green
  after the migration to the on-disk store (the
  contract is the seam; the tests are blind to
  the storage medium).

**Dogfooding checkpoint (M4-A):** none. The
infrastructure abstractions are not yet wired to
any concrete external tool; dogfooding happens in
M4-D when the first concrete provider lands.

**Handoff to M4-B:** the
`AiEng.Platform.Infrastructure` project is in
place; `IProcessRunner` is the only way to spawn a
process; `ICredentialVault` and `IClock` are
available. The capability detection in M4-B
consumes all three.

#### M4-B — Capability Detection

**Outcome:** The host detects the external capabilities
present on the host machine (`git`, `ollama`,
`powershell.exe`, `wsl.exe`, `wt.exe`, `bash.exe`)
through `IProcessRunner` and `ICredentialVault`. The
detection runs at host startup and produces a typed
`HostCapabilities` report.

**Prerequisites:** M4-A is done
(`AiEng.Platform.Infrastructure` and `IProcessRunner`
are in place).

**Outcome in detail:**

- `IHostCapabilitiesService` is added to
  `AiEng.Platform.Application/Capabilities/`. It
  exposes
  `DetectAsync(CancellationToken)` returning a
  `HostCapabilities` record that names every
  external tool the platform may integrate with
  and the version reported by `--version`.
- The detection runs each candidate binary through
  `IProcessRunner.RunToCompletionAsync` with a
  short timeout (default 1s). The result is a
  `CapabilityProbe` (`Found`, `Version`,
  `NotFound`, `Failed`).
- The `HostCapabilities` report is consumed by
  M4-C's provider registry to decide which
  providers are eligible for enablement.
- The diagnostics page surfaces
  `AppCapabilityList` and `AppKeyValueList` to
  show the host's capabilities to the user.

**Excluded scope:** the provider registry (M4-C),
the first concrete providers (M4-D). M4-B does
not introduce a single `Providers.<X>` project.
The detection produces a report; the report is
not yet consumed by a registry.

**Definition of done:**

- `IHostCapabilitiesService` and
  `HostCapabilities` are added; the unit tests
  cover the happy path, the missing binary, the
  timeout, and the non-zero exit.
- The diagnostics page renders
  `AppCapabilityList` and `AppKeyValueList` for
  the host's capabilities.
- The host's capability report is logged at
  startup (Information level) and is not yet
  consumed by a registry.

**Tests added:**

- Unit tests for `HostCapabilitiesService`
  (happy path, missing binary, timeout, non-zero
  exit, version parsing).
- bUnit tests for `AppCapabilityList` and
  `AppKeyValueList` (primary render, every
  state slot).

**Dogfooding checkpoint (M4-B):** when the
diagnostics page renders the host's capabilities,
the development team may run the platform on a
second machine to verify the detection is robust
to a different set of installed tools. The
session records the observed report.

**Handoff to M4-C:** the `HostCapabilities`
report is available. M4-C's provider registry
consumes the report to decide which providers
are eligible for enablement.

#### M4-C — Provider Registry Foundation

**Outcome:** The `IProviderRegistry` and the
family-scoped registries are introduced with
**fake** providers (per the lifecycle states in
ADR-016 and `ARCHITECTURE.md` § 4.5). The
composition root pattern (per ADR-016) is in
place: registration extensions live in
`AiEng.Platform.App/Composition/`, and no other
folder in `App` or `Application` may reference a
`Providers.<X>` project directly. The four
composition-root architecture tests are
**registered but disabled**; they activate in
M4-D when the first concrete `Providers.<X>`
project lands.

**Prerequisites:** M4-B is done (`HostCapabilities`
is available).

**Outcome in detail:**

- `IProviderRegistry` is added to
  `AiEng.Platform.Providers.Abstractions/`. It
  exposes `List()` and `Resolve(ProviderId)` and
  is the cross-family registry. The matching
  family-scoped registries
  (`IAgentRuntimeProviderRegistry`,
  `IGitProviderRegistry`,
  `ITerminalProviderRegistry`,
  `IWorktreeProviderRegistry`,
  `IQualityGateProviderRegistry`,
  `IReviewProviderRegistry`,
  `IAutonomousLoopProviderRegistry`,
  `IOrchestrationProviderRegistry`) are added.
- **Fake providers** are added to
  `AiEng.Platform.Application/Fakes/` (or to a
  dedicated `Fakes/` folder at the solution
  level — the choice is recorded when the slice
  lands). Each fake is a `sealed` class that
  implements one of the family contracts and
  returns deterministic results. The fakes are
  not registered through the composition root;
  they are the test doubles used by the unit
  tests to prove the registry enumerates
  registered providers correctly. A fake that is
  compiled into the production build is a
  smell and is rejected.
- The composition root pattern is in place. The
  folder `AiEng.Platform.App/Composition/` is
  created. The first registration extensions
  are the fake-provider registration extensions
  (one per family) under
  `Composition/<Family>/Fake/FakeServiceCollectionExtensions.cs`.
  The four composition-root architecture tests
  are registered but disabled.
- The five lifecycle states (`Compiled-in`,
  `Registered`, `Enabled`, `Healthy`, `Selected`)
  are implemented in the registry. The M4-C
  fakes transition through `Compiled-in` →
  `Registered` → `Enabled` (because the
  configuration is present and valid for every
  fake in M4-C). The M4-C fakes do not exercise
  `Healthy` (the M4-D real providers do) or
  `Selected` (the M6 runtime picker does).
- The diagnostics page surfaces
  `AppProviderCard` for every registered
  provider.

**Excluded scope:** the first concrete
providers (M4-D). M4-C ships only fake
providers. No real provider implementation
landed in M4-C. The `Providers.<X>` projects
that host the real implementations are
created in M4-D.

**Definition of done:**

- `IProviderRegistry` and the family-scoped
  registries are added; the unit tests cover
  the happy path (every registered provider is
  listed) and the failure path (a disabled
  provider is absent from the listing).
- The fake providers for every family are
  added; the unit tests that consume them are
  green.
- The composition root pattern is in place;
  the folder `AiEng.Platform.App/Composition/`
  exists with at least one registration
  extension per family.
- The four composition-root architecture
  tests are registered but disabled; the
  activation milestone is M4-D.
- The diagnostics page renders
  `AppProviderCard` for every registered fake
  provider.

**Tests added:**

- Unit tests for `IProviderRegistry` and
  every family-scoped registry (enumeration,
  resolution, enablement).
- bUnit tests for `AppProviderCard` (primary
  render, every state slot, every variant).
- Architecture tests:
  `Only_CompositionRoot_MayReference_ConcreteProviders`,
  `Pages_DoNotReference_ConcreteProviders`,
  `Application_DoesNotReference_ConcreteProviders`,
  `Components_DoNotInject_ConcreteProviders`
  (registered but disabled).

**Dogfooding checkpoint (M4-C):** when the
diagnostics page renders every registered
provider, the development team may interact
with the page to verify the registry's
behaviour. The session records the observed
listing.

**Handoff to M4-D:** the registry, the
composition root, and the fakes are in place.
M4-D replaces the fakes with the first real
providers (`GitProvider` and
`OllamaLaunchProvider`), activates the
composition-root tests, and ships the contract
test framework.

#### M4-D — First Concrete Process Providers

**Outcome:** The first two concrete
process-boundary providers land —
`GitProvider` (concrete `IGitProvider`) and
`OllamaLaunchProvider` (concrete
`IAgentRuntimeProvider` for the Ollama Launch
process boundary). The
`AiEng.Platform.ProviderContractTests` project
is created; the contract test framework is
established. The four composition-root
architecture tests are activated. The
`No_DirectProcessStart_OutsideInfrastructure`
test is activated.

**Prerequisites:** M4-C is done (the
registry, the composition root, and the fakes
are in place).

**Outcome in detail:**

- `AiEng.Platform.Providers.Git` is added to
  the solution. The project is created
  specifically for the M4-D deliverable.
  `GitProvider` implements `IGitProvider` and
  uses `IProcessRunner` to invoke the host's
  `git` CLI. `GitOptions` is bound from
  configuration. The contract test class
  (`GitProviderContractTests`) inherits the
  family contract test base; the test passes
  against a fake `git` process.
- `AiEng.Platform.Providers.Ollama` is added
  to the solution. `OllamaLaunchProvider`
  implements `IAgentRuntimeProvider` and owns
  the Ollama Launch process boundary. The
  provider spawns
  `ollama launch <runtime> --model <model>`
  through `IProcessRunner`, captures the
  output stream, and supports cancellation.
  The contract test class
  (`OllamaLaunchProviderContractTests`)
  inherits the family contract test base; the
  test passes against a stub process.
- `AiEng.Platform.ProviderContractTests` is
  added to the solution. The contract test
  framework is the xUnit + NetArchTest +
  `IAsyncDisposable` pattern documented in
  `docs/provider-guidelines.md` § 8.
- The four composition-root architecture
  tests activate: the
  `Only_CompositionRoot_MayReference_ConcreteProviders`
  test now fails the build if any source file
  outside `AiEng.Platform.App/Composition/`
  contains a `using AiEng.Platform.Providers.<X>;`
  statement. The companion tests
  `Pages_DoNotReference_ConcreteProviders`,
  `Application_DoesNotReference_ConcreteProviders`,
  and `Components_DoNotInject_ConcreteProviders`
  activate in the same change.
- The `No_DirectProcessStart_OutsideInfrastructure`
  architecture test activates. The test now
  fails the build if any source file outside
  `AiEng.Platform.Infrastructure/Process/`
  contains the literal `Process.Start`.

**Excluded scope:** the full UI surface for
the launch flow (M6), the worktree
integration (M5), the review and quality
gates (M7). M4-D ships the providers and
the contract test framework; the UI work
that uses them is in M5 / M6 / M7.

**Definition of done:**

- `GitProvider` and `OllamaLaunchProvider`
  pass their contract tests against fake /
  stub external processes.
- The four composition-root architecture
  tests are green.
- The `No_DirectProcessStart_OutsideInfrastructure`
  architecture test is green.
- The diagnostics page renders
  `AppProviderCard` for `GitProvider` and
  `OllamaLaunchProvider` with their actual
  `ProviderHealth`.
- The `OllamaLaunchProvider` smoke test
  (spawn
  `ollama launch claude --model minimax-m3:cloud`,
  capture output, stop the process) passes.
  The full UI surface for the launch flow
  lands in M6.
- The credential vault can store and retrieve
  a secret (real backing store: Windows
  Credential Manager).
- The `GitProvider` consumes `IProcessRunner`
  through the registry, not by direct import.

**Tests added:**

- Contract tests for `GitProvider` and
  `OllamaLaunchProvider` (happy path, every
  documented failure path, cancellation).
- Architecture tests:
  `Only_CompositionRoot_MayReference_ConcreteProviders`
  (active),
  `Pages_DoNotReference_ConcreteProviders`
  (active),
  `Application_DoesNotReference_ConcreteProviders`
  (active),
  `Components_DoNotInject_ConcreteProviders`
  (active),
  `No_DirectProcessStart_OutsideInfrastructure`
  (active).

**Dogfooding checkpoint (M4-D):** when the
first `OllamaLaunchProvider` smoke test
passes, the development team may run an
Ollama Launch invocation interactively to
verify the process boundary behaves as
expected on the host machine. The session
records the exact command and the observed
output. The platform does not yet render a
session UI for the launch; that is the M6
milestone. See
[`.ai/workflows/tool-dogfooding.md`](./.ai/workflows/tool-dogfooding.md).

**Handoff to M5:** the registry, the
composition root, the contract test
framework, and the first two concrete
process providers are in place. M5 consumes
`IGitProvider` through the registry to ship
the `NativeWorktreeProvider`; M6 consumes
`IAgentRuntimeProvider` through the registry
to ship the launch UI.

---

### M5 — Native Git Worktrees

**Outcome:** A user can create a native Git worktree from a
registered project, the worktree is the unit of work for
later milestones (sessions, runs, reviews), and the
worktree is consumed by the platform through the
`IWorktreeProvider` family.

**Why it is fifth:** A worktree provider that consumes
`IGitProvider` (from M4) is the platform's first
demonstration of a provider composing another provider.
The discipline of the matrix — every later milestone
consumes the abstractions earlier milestones delivered —
is exercised for the first time. If the worktree provider
bypasses `IGitProvider` and shells out directly, the
architecture test catches it.

**Enables:** M6 (agent launching runs in a worktree),
M7 (review and quality gates run in a worktree), M8
(orchestration runs across worktrees).

**Reusable components introduced:**

- `AppWorktreeCard`
- `AppWorktreeList`
- `AppDiffViewer` (initial; full implementation in M7)

**Services introduced:**

- `IWorktreeService`
- `IWorktreeStore`

**Providers introduced:**

- `NativeWorktreeProvider` (concrete `IWorktreeProvider`,
  built on `IGitProvider` from M4). The native
  implementation is the baseline; the Treehouse
  `IWorktreeProvider` is added later when the user opts
  in.

**Progressive self-dogfooding (per ADR-013):** The
`NativeWorktreeProvider` consumes `IGitProvider` (M4)
**through the registry**, not by importing
`GitProvider` directly. The architecture test
`NativeProviders_Use_Contracts_Not_Implementations`
enforces this. The matrix row added by M4 (`IGitProvider`
must be consumed by the registry) gains the M5
worktree provider as the consumer.

**Definition of done:**

- A user can create a worktree from a registered project.
- The worktree is listed in the worktrees page.
- The worktree provider resolves `IGitProvider` through
  the registry.
- The worktree contract test passes.
- The architecture test
  `NativeProviders_Use_Contracts_Not_Implementations`
  passes.

**Dogfooding checkpoint (M5):** When the worktree
provider ships, the development team may use
**GNHF** externally for a bounded, repetitive task that
is not architecture-sensitive. The branch is preserved
for human review. Autonomous loops are never used for
architecture-sensitive work. GNHF is an
`IAutonomousLoopProvider` candidate; product
integration is the M8 milestone. See
[`.ai/workflows/tool-dogfooding.md`](./.ai/workflows/tool-dogfooding.md).

---

### M6 — Agent Runtime Launching

**Outcome:** A user can:

1. Select a registered project.
2. Acquire or use its worktree (the M5 worktree is the
   default workspace).
3. Select an agent runtime through the
   `IAgentRuntimeProviderRegistry` (the Ollama Launch
   provider is the first runtime).
4. Select a model (e.g. `minimax-m3:cloud`).
5. Launch the runtime by invoking
   `ollama launch claude --model minimax-m3:cloud`
   through `IProcessRunner` (the only project that may
   call `Process.Start` is
   `AiEng.Platform.Infrastructure`).
6. Stream or expose logs through the design system
   (`AppTerminalPanel`, `AppTerminalLine`).
7. Stop the process safely; cancellation flows through
   the `CancellationToken` on the launch call.
8. Preserve the execution history in the history store.

This is the platform's **first useful coding-agent
vertical slice** — the moment the platform stops being
a configuration tool and starts being an engineering
tool. It is also the first milestone where a third-party
process is owned by the platform across its full
lifecycle.

**Why it is sixth:** The runtime launching path is the
most "developer" integration in the platform; it is
also the one with the most risk (a long-running process
that must be cancellable, observable, and recoverable).
Landing it after the registry, the process runner, the
worktree model, and the project registration means the
launch flow has a real substrate: a project to run in,
a worktree to keep the run isolated, a registry to
resolve the runtime through, a process runner to own
the boundary, and a credential vault to hold the model
secret if one is needed.

**Enables:** M7 (review and quality gates run against
launched-runs), M8 (autonomous loops drive a series of
launches, orchestration coordinates them).

**Reusable components introduced:**

- `AppTerminalPanel`
- `AppTerminalLine`
- `AppRunStatus`
- `AppRunHistory`
- `AppModelPicker`
- `AppRuntimePicker`

**Services introduced:**

- `IRunService`
- `IStreamingChannel`
- `IHistoryStore`

**Providers introduced:**

- The M4 `OllamaLaunchProvider` is deepened into a
  full provider: process spawn, model flag, output
  stream, cancellation, error mapping, exit-code
  handling. The contract remains
  `IAgentRuntimeProvider`.

**Progressive self-dogfooding (per ADR-013):** The
runtime launching path consumes the M4 abstractions
end-to-end:

- `IProcessRunner` from
  `AiEng.Platform.Infrastructure/Process/`
  (the only legal caller of `Process.Start`).
- `IProviderRegistry` and
  `IAgentRuntimeProviderRegistry` to resolve the
  runtime (no direct provider import).
- `IGitProvider` (M4) through the registry to read
  the worktree's repository state.
- `IWorktreeProvider` (M5) through the registry to
  acquire the workspace.
- `ICredentialVault` (M4) to read the model secret if
  one is configured.
- `IClock` (M4) to stamp run history.
- `IHistoryStore` (this milestone) to persist
  execution history.

The architecture tests in
`AiEng.Platform.ArchitectureTests/SelfDogfooding/`
fail the build if any of these is bypassed. The matrix
in § 4 records each consumption explicitly.

**Definition of done:**

- A user can launch an agent runtime against a
  registered project's worktree.
- The launch is cancellable mid-stream.
- The terminal output is rendered through
  `AppTerminalPanel` and `AppTerminalLine`.
- The execution history is persisted in
  `IHistoryStore` and survives an application restart.
- The architecture test
  `No_DirectProcessStart_OutsideInfrastructure` passes
  on the run-service implementation.
- The architecture test
  `App_DoesNotReference_Providers_Implementations`
  passes on the page that exposes the launch flow.
- A contract test for the launch path passes.

**Dogfooding checkpoint (M6):** When the first launch
flow ships, the development team may use the launch
flow interactively to drive a real coding-agent task
on a registered project. The session records the
commands, the model, and the observed output. The
session confirms the cancellation behaviour and the
history persistence. The launch flow is the
**M6 end-to-end milestone**; the result is the first
piece of evidence that the architecture is honest. See
[`.ai/workflows/tool-dogfooding.md`](./.ai/workflows/tool-dogfooding.md).

---

### M7 — Review and Quality Gates

**Outcome:** A launched run can be reviewed (the
`IReviewProvider` family) and gated (the
`IQualityGateProvider` family). The native baselines
are the first implementations; external tools (Lavish
Axi, No Mistakes) are added through the registry when
the user opts in.

**Why it is seventh:** Review and quality gates are
the natural next step after a run. They are also the
two families that benefit most from the
credential-vault boundary: review secrets and
quality-gate tokens are user-supplied and must not be
in configuration.

**Enables:** M8 (autonomous loops invoke review and
quality gates between iterations; orchestration
coordinates the gate across sub-tasks).

**Reusable components introduced:**

- `AppReviewPanel`
- `AppReviewFindingList`
- `AppQualityGateBadge`
- `AppProviderSettingsForm` (the unified provider
  configuration form, including the secret field)
- `AppSecretField`
- `AppConnectionTestButton`

**Services introduced:**

- `IReviewService`
- `IQualityGateService`
- `IProviderConfigurationService`

**Providers introduced:**

- `NativeReviewProvider` (concrete `IReviewProvider`,
  built on the Application layer's review orchestrator
  and the file system). The native implementation
  is the baseline.
- `NativeQualityGateProvider` (concrete
  `IQualityGateProvider`, built on the Application
  layer's quality-gate orchestrator). The native
  implementation is the baseline.

**Progressive self-dogfooding (per ADR-013):** Every
secret read by the configuration form flows through
`ICredentialVault` (M4); no secret is read from
`appsettings.json` and no secret is logged. The
architecture tests `No_Secrets_In_Logs` and
`No_Secrets_In_Configuration` enforce this. The
configuration form resolves every provider through
the registry; the architecture test
`Pages_Resolve_Providers_Through_Registry` fails
the build if any page constructor takes a concrete
provider type.

**Definition of done:**

- A user can submit a launched run for review; the
  review findings are rendered through
  `AppReviewPanel`.
- A user can run a quality gate against a launched
  run; the gate's pass/fail is rendered through
  `AppQualityGateBadge`.
- A user can configure any provider through the
  configuration form, including entering secrets
  through `AppSecretField`.
- Disabling a provider removes it from all UI
  surfaces.
- The architecture tests for secrets and registry
  resolution pass.

**Dogfooding checkpoint (M7):** The development team
may now use **No Mistakes** (an
`IQualityGateProvider` candidate) as a real quality
gate against a launched run, and **Lavish Axi** (an
`IReviewProvider` candidate) to review a launched
run interactively. Both are still external-tool
dogfooding events; the platform's provider
implementations of No Mistakes and Lavish Axi are
added in a later milestone when the user opts in. See
[`.ai/workflows/tool-dogfooding.md`](./.ai/workflows/tool-dogfooding.md).

---

### M8 — Autonomous Loops, Orchestration, Production Hardening

**Outcome:** The platform ships its remaining provider
families (`IAutonomousLoopProvider`,
`IOrchestrationProvider`) through native baselines, and
the production-hardening work lands: logging is
structured across the platform, errors flow through
the unified `ProviderResult<T>` envelope, persistence
is durable, packaging is signed, and accessibility
audits pass.

**Why it is last:** The remaining families depend on
the others; the production-hardening work is meaningful
only after there is a real codebase to instrument,
profile, and stabilise.

**Enables:** Ongoing development at production quality.

**Reusable components introduced:**

- `AppToast`, `AppToastHost`
- `AppDiagnosticDrawer`
- `AppCrashBoundary`
- `AppAutonomousLoopCard`
- `AppOrchestrationGraph`

**Services introduced:**

- `IAutonomousLoopService`
- `IOrchestrationService`
- `ITelemetryService`
- `IUpdateService`

**Providers introduced:**

- `NativeAutonomousLoopProvider` (concrete
  `IAutonomousLoopProvider`, built on the Application
  layer's autonomous-loop orchestrator). The native
  implementation is the baseline; GNHF is added
  through the registry when the user opts in.
- `NativeOrchestrationProvider` (concrete
  `IOrchestrationProvider`, built on the Application
  layer's task orchestrator). The native
  implementation is the baseline; Firstmate through
  WSL is added when the user opts in.

**Progressive self-dogfooding (per ADR-013):** Every
provider in this milestone composes an earlier
provider family. The
`NativeAutonomousLoopProvider` consumes
`IReviewProvider` and `IQualityGateProvider` (M7)
through the registry; the
`NativeOrchestrationProvider` consumes
`IAutonomousLoopProvider` (this milestone) and
`IWorktreeProvider` (M5) through the registry. The
architecture test
`NativeProviders_Use_ApplicationServices_Not_Implementation`
enforces the composition direction. The GNHF and
Firstmate providers are not added in M8; the native
implementations are the baselines they will be
tested against.

**Definition of done:**

- All async paths handle cancellation correctly.
- All errors flow through the unified
  `ProviderResult<T>` envelope.
- The application installs on Windows and launches
  in under two seconds on a developer laptop.
- An accessibility audit passes for the main shell.
- The autonomous loop and orchestration providers
  pass their contract tests.

---

## 4. Progressive Self-Dogfooding Matrix

Per ADR-013, every milestone must consume the stable
reusable capabilities delivered by earlier milestones.
Later milestones must not bypass earlier platform
abstractions with temporary direct implementations. The
rule is operationalised as a matrix: each row records
the capability delivered by the milestone in the
**Capability** column, the later milestones that must
use it, the direct bypass that is prohibited, the
validation that confirms consumption, and the
architecture test that fails the build on bypass.

External-tool dogfooding (the development team using
an external tool manually) is a separate discipline,
governed by `.ai/workflows/tool-dogfooding.md` and the
per-milestone "Dogfooding checkpoint" subsections
above. The two disciplines must not be confused. This
matrix is **platform self-dogfooding**: the product
using its own abstractions.

| Capability delivered (milestone) | Later milestones that must use it | Direct bypass that is prohibited | Validation | Architecture test |
| -------------------------------- | --------------------------------- | -------------------------------- | ---------- | ----------------- |
| Project boundary between `App` / `Application` and `Providers.Abstractions` (M1) | M3+ | `App` or `Application` importing a `Providers.<X>` project directly | Registry resolution at runtime | `App_DoesNotReference_Providers_Implementations` (active in M1+); `Only_CompositionRoot_MayReference_ConcreteProviders`, `Pages_DoNotReference_ConcreteProviders`, `Application_DoesNotReference_ConcreteProviders`, `Components_DoNotInject_ConcreteProviders` (registered but disabled in M1–M4-C, **delivered in M1 closeout**; activate in M4-D per ADR-016) |
| Composition root (per ADR-016) | M3+ | A page, component, application service, view model, DTO, or domain type that imports a `Providers.<X>` project directly | Composition root is the only registration site; everything else resolves through the registry | `Only_CompositionRoot_MayReference_ConcreteProviders`, `Pages_DoNotReference_ConcreteProviders`, `Application_DoesNotReference_ConcreteProviders`, `Components_DoNotInject_ConcreteProviders` (delivered as registered-but-disabled in the M1 closeout; activate in M4-D) |
| Design-system components catalogue (M1) | M2+ | A page that uses raw `<button>`, `<input>`, or inline-style attribute | Pages compose `App*` components only | `Pages_Use_DesignSystem_Components_Not_DOM` |
| Application shell, sidebar, navigation, route registry (M2.1–M2.3) | M3+ | A page that is not routable through the sidebar registry | All pages reachable through `INavigationRegistry` (M2.2) | `Pages_AreReachable_Through_Registry` (M2.2). M2.1 delivers the shell foundation (`AppLayout`, `AppEmptyLayout`, `AppSidebarSlot`, `AppTopBarSlot`, `AppShellRegion`); the placeholder slots are populated by M2.2 / M2.3. |
| Project intelligence state (M2.4, read-only, `.ai/state/*.json`) | M3+ | A page that reads `.ai/state/*.json` directly | Pages read through `IProjectIntelligenceReader` (M2.4) | `Pages_Resolve_State_Through_Reader` (M2.4) |
| Project entity, `IProjectService`, `IProjectStore` (M3 in-memory, M4-A durable) | M4, M5, M6, M7, M8 | A provider that re-implements project loading inline | Providers resolve the project through `IProjectService` | `Providers_Resolve_Project_Through_Service` |
| `IProcessRunner` in `AiEng.Platform.Infrastructure/Process/` (M4-A) | M4-A, M4-D, M5, M6, M7, M8 | Any source file outside `Infrastructure/Process/` containing the literal `Process.Start` | Every shell call routed through `IProcessRunner` | `No_DirectProcessStart_OutsideInfrastructure` (registered but disabled in M1–M4-C; activates in M4-D) |
| `ICredentialVault` in `AiEng.Platform.Infrastructure/Credentials/` (M4-A) | M4-A, M4-D, M5, M6, M7, M8 | A secret read from `appsettings.json`; a secret logged at any level | All secrets routed through `ICredentialVault` | `No_Secrets_In_Logs`, `No_Secrets_In_Configuration` |
| `IHostCapabilitiesService` (M4-B) | M4-C, M4-D, M5, M6, M7, M8 | The host's capability detection re-implemented inline; a provider that bypasses the detection | Detection is the source of truth for `Compiled-in` → `Registered` transitions | `Capabilities_Resolved_Through_Service` |
| `IProviderRegistry`, family-scoped registries (M4-C) | M4-D, M5, M6, M7, M8 | A page constructor or service that takes a concrete provider type | All provider lookups go through the registry | `Pages_Resolve_Providers_Through_Registry` |
| `IGitProvider` (M4-D) | M5, M6, M7, M8 | `NativeWorktreeProvider` importing `GitProvider` directly; the launch service importing `GitProvider` directly | Resolve through the registry | `NativeProviders_Use_Contracts_Not_Implementations` |
| `IAgentRuntimeProvider` with the `OllamaLaunchProvider` (M4-D contract, M6 depth) | M6, M7, M8 | `RunService` importing `OllamaLaunchProvider` directly; the launch flow bypassing the registry | Resolve through `IAgentRuntimeProviderRegistry` | `Runtime_Resolved_Through_Registry` |
| `IWorktreeProvider` (M5, native) | M6, M7, M8 | The launch service or the review orchestrator reaching into the file system for a worktree | Acquire the worktree through `IWorktreeProvider` registry | `Worktree_Resolved_Through_Registry` |
| `IHistoryStore` (M6) | M6, M7, M8 | A run history written through a service that talks to a local file directly | History routed through `IHistoryStore` | `History_Routed_Through_Store` |
| `IReviewProvider` family, native baseline (M7) | M7, M8 | A review step that bypasses the contract | Review consumed through the registry | `Review_Resolves_Through_Registry` |
| `IQualityGateProvider` family, native baseline (M7) | M7, M8 | A quality-gate check that bypasses the contract and runs inline | Gate consumed through the registry | `QualityGate_Resolves_Through_Registry` |
| `IAutonomousLoopProvider` family, native baseline (M8) | M8+ | An autonomous loop that bypasses the contract | Loop consumed through the registry | `AutonomousLoop_Resolves_Through_Registry` |
| `IOrchestrationProvider` family, native baseline (M8) | M8+ | An orchestration step that bypasses the contract | Orchestration consumed through the registry | `Orchestration_Resolves_Through_Registry` |

A row in the matrix is **enforced by code**, not by
convention. The architecture test column is the
contract that fails the build on bypass; the
`AiEng.Platform.ArchitectureTests/SelfDogfooding/`
folder is where the test classes live.

The matrix is a living document. A milestone that
delivers a new reusable capability adds a row; a
milestone that needs a capability updates its row's
"Validation" cell.

---

## 5. What Is Intentionally Deferred

The following are out of scope for the milestones above
and are tracked separately:

- Multi-user / server-side rendering for multiple
  concurrent users.
- Cloud-hosted deployments and multi-tenant concerns.
- Plugin marketplace and third-party provider signing.
- Telemetry and analytics beyond local diagnostics.
- Mobile / touch-first UI.
- Migration of the design system into Storybook or a
  similar tool.
- The **Ollama API** provider (HTTP boundary to
  `http://localhost:11434`). The platform's first
  runtime is the **Ollama Launch** process boundary
  (M4 contract, M6 depth). The Ollama API provider is
  added when the user opts in; it is a separate
  integration, not a flavour of Ollama Launch
  (see `docs/provider-guidelines.md` § 2.1).
- The **external** `IReviewProvider` implementations
  (Lavish Axi), the **external**
  `IQualityGateProvider` implementations (No
  Mistakes), the **external**
  `IAutonomousLoopProvider` implementations (GNHF), and
  the **external** `IOrchestrationProvider`
  implementations (Firstmate). The native baselines
  ship in M7 (review, quality gate) and M8
  (autonomous loop, orchestration); the external
  tools are added through the registry when the user
  opts in.

Adding these later must not require architectural
change. The provider model and the design system are
deliberately structured to absorb them.

---

## 6. How Milestones Are Updated

When a milestone is reached:

1. Update its status to **Done**.
2. Move any unfinished work into a new milestone or
   remove it.
3. Add a short retrospective to `DECISIONS.md`.
4. Update `docs/design-system.md` and
   `docs/component-guidelines.md` with every new
   reusable component.
5. Update the progressive self-dogfooding matrix in
   § 4 if the milestone delivered a new reusable
   capability or if its consumers are now known.

When a milestone is reordered:

1. Record the reason in `DECISIONS.md`.
2. Update the milestone map and any dependent
   documents.

The roadmap is a living document, but every change is
reasoned and recorded.
