# ARCHITECTURE.md

> The architectural foundation of the AI Engineering Platform. This document
> describes **how the system is organised, why it is organised that way, and
> what trade-offs were accepted**. It is the second document any agent must
> read, after `AGENTS.md`.

---

## 1. Architectural Goals

The platform is designed to satisfy five long-term goals. Every architectural
decision must be evaluated against this list.

1. **Survive years of development.** Hundreds of components, dozens of
   providers, multiple AI runtimes, and thousands of users.
2. **Stay extensible without rewrites.** New providers, new runtimes, new
   surfaces (CLI, web, IDE plugin) must be addable without touching the UI.
3. **Stay testable in isolation.** Layers and providers must be replaceable
   behind interfaces.
4. **Stay professional.** Desktop-first, accessible, fast, predictable.
5. **Stay understandable.** New contributors — human or AI — must be
   productive on day one by reading `AGENTS.md` and the documents it points to.

If a decision does not advance one of these goals, it is the wrong decision.

---

## 2. High-Level System View

```
+--------------------------------------------------------------+
|                         Presentation                         |
|  (Blazor Pages, Components, Layouts, Dialogs, Sidebar)        |
+----------------------------+---------------------------------+
                             |
                             v
+--------------------------------------------------------------+
|                       Application Layer                       |
|  (Services, Orchestrators, ViewModels, DTOs, Models)          |
+----------------------------+---------------------------------+
                             |
                             v
+--------------------------------------------------------------+
|                       Provider Contracts                      |
|  (IProvider, IAgentRuntimeProvider, IGitProvider,            |
|   ITerminalProvider, IWorktreeProvider, IQualityGateProvider,|
|   IReviewProvider, IAutonomousLoopProvider,                  |
|   IOrchestrationProvider)                                    |
+----------------------------+---------------------------------+
                             |
                             v
+--------------------------------------------------------------+
|                     Provider Implementations                  |
|  (Native + Ollama Launch, Ollama API, Claude, OpenAI, Codex,  |
|   Git, PowerShell, WSL, Windows Terminal, Git Bash,          |
|   Treehouse, No Mistakes, Lavish Axi, GNHF, Firstmate)       |
+----------------------------+---------------------------------+
                             |
                             v
+--------------------------------------------------------------+
|                          Infrastructure                       |
|  (Configuration, Logging, Persistence, IProcessRunner,        |
|   ICredentialVault, IClock)                                  |
+--------------------------------------------------------------+
```

The dependency direction is **downward and one-way only**. Upper layers
depend on lower layers. Lower layers never reference upper layers.

This logical layering is enforced **at compile time** by the solution
structure described in § 2.5 below. Folder separation is not a
substitute for project separation; the boundary is real, and the
compiler enforces it.

---

## 3. Layered Architecture in Detail

### 3.1 Presentation Layer

**Responsibility:** Render UI, capture user input, and translate user
intention into application-layer calls.

**Contains:**

- Pages (`Pages/**.razor` + `Pages/**.razor.cs`).
- Reusable components (`Components/**`).
- Layouts (`Layouts/**`).
- Dialogs (`Dialogs/**`).
- Sidebar/navigation (`Navigation/**`).

**Forbidden in this layer:**

- Direct calls to external tools or HTTP clients.
- Provider implementations.
- Business logic.
- Direct file system or registry access.
- Inline `<style>` blocks for anything that should be a design-system class.

**Allowed dependencies:**

- Application layer services (via DI).
- Components (only the ones documented in `docs/design-system.md`).
- The Blazor framework.

See [`docs/component-guidelines.md`](./docs/component-guidelines.md) and
[`docs/design-system.md`](./docs/design-system.md).

### 3.2 Application Layer

**Responsibility:** Coordinate work, hold business rules, and translate
between presentation DTOs and provider contracts.

**Contains:**

- Services (`Services/**`).
- ViewModels for components that need non-trivial state
  (`ViewModels/**`, co-located with their owner component when single-use).
- DTOs (`Dtos/**`).
- Domain models (`Models/**`).
- Orchestrators that compose multiple providers.

**Forbidden in this layer:**

- Direct rendering of Razor components.
- Provider implementations.
- HTTP, file system, or process calls without going through a provider.

**Allowed dependencies:**

- Provider contracts.
- Infrastructure abstractions (logging, time, persistence).
- Domain models and DTOs of its own layer.

### 3.3 Provider Contracts

**Responsibility:** Define the shape of every external integration as a
.NET interface.

**Contains (capability-oriented families, per ADR-012):**

- `IProvider` — small base metadata contract. Every family inherits
  from it. Carries `Id`, `DisplayName`, `DescribeAsync`,
  `HealthAsync`, `ConfigureAsync`. Exists so the registry, the
  health poller, the configuration UI, and the diagnostics page
  can consume a uniform shape.
- `IAgentRuntimeProvider` — providers that execute agent models
  (Ollama Launch as a process boundary in M6, Ollama API over
  HTTP in a later milestone, Claude, OpenAI, Codex, custom
  command runtimes).
- `IGitProvider` — providers that read and operate on source
  control (Git).
- `ITerminalProvider` — providers that run shell commands
  (PowerShell, Windows Terminal, WSL, Git Bash).
- `IWorktreeProvider` — providers that create and manage
  isolated development worktrees (native Git worktree, Treehouse).
- `IQualityGateProvider` — providers that run a quality gate
  against a branch (No Mistakes, built-in pass-through or
  disabled provider).
- `IReviewProvider` — providers that review an artefact (native
  HTML review, Lavish Axi).
- `IAutonomousLoopProvider` — providers that perform bounded
  autonomous iteration (GNHF).
- `IOrchestrationProvider` — providers that perform multi-agent
  orchestration (native orchestration, Firstmate through WSL).

A new family is added only when the capability is genuinely
shared by at least two providers. Vague names such as
`Assistant`, `Deployment`, or `Internal` are rejected.

**Rule:** Every contract is a pure interface. No implementation, no
external dependency, no logic. A contract is a promise; the implementation
is the proof.

### 3.4 Provider Implementations

**Responsibility:** Implement provider contracts against specific external
tools.

**Contains:**

- One folder per provider family under `Providers/<Family>/`.
- HTTP clients, SDK wrappers, process runners as private implementation
  detail.
- Provider-specific configuration objects.

**Rule:** A provider never references another provider. If two providers
must cooperate, that cooperation belongs in the application layer.

### 3.5 Infrastructure

**Responsibility:** Cross-cutting concerns that are not domain-specific.

**Contains:**

- Logging (`ILogger<T>`).
- Configuration binding.
- Persistence (settings, workspaces, history).
- Process hosting.
- Time abstraction (`IClock`).
- The `IProcessRunner` contract. **No code outside the
  infrastructure project may call `Process.Start` directly.**
  This is enforced by the architecture tests in
  `AiEng.Platform.ArchitectureTests`.
- The `ICredentialVault` contract. Secrets are read through
  this contract, never from configuration files, never from
  environment variables outside the configuration layer.

**Rule:** Infrastructure implements abstractions defined in the application
layer or the provider contracts, never the other way around.

---

## 2.5 Solution and Project Boundaries (per ADR-011)

The five logical layers above are realised as **four projects** in
M1, plus a test project per kind of test that the M1 work
requires. The mapping from logical layer to project is fixed; a
future project is added through an ADR, never by convention.

```
AiEng.Platform.sln

src/
  AiEng.Platform.App/                       # Presentation (Blazor Server)
  AiEng.Platform.Application/               # Application services, view models, DTOs
  AiEng.Platform.Domain/                    # Pure domain types, value objects
  AiEng.Platform.Providers.Abstractions/    # IProvider + every family contract

tests/
  AiEng.Platform.UnitTests/
  AiEng.Platform.ComponentTests/            # bUnit
  AiEng.Platform.ArchitectureTests/         # NetArchTest + Roslyn analyzers
```

**Deferred to the milestone that introduces the capability:**

- `AiEng.Platform.Infrastructure` — created in **M4** when the
  first infrastructure abstraction (`IProcessRunner`,
  `ICredentialVault`, `IClock`, or persistence) is consumed. M1
  does not need any of them. A project with no consumer is a
  speculative project; the rule from `AGENTS.md` and ADR-011
  rejects it.
- `AiEng.Platform.ProviderContractTests` — created in **M4**
  when the first concrete provider (`GitProvider` and
  `OllamaLaunchProvider`) lands. A contract-test project
  without a contract to test is a speculative project.

Provider implementation projects
(`AiEng.Platform.Providers.<X>`) and other surface projects
(CLI, PowerShell host) are **deferred** to the milestone that
implements them. They are not created speculatively. The
deferred list lives in ADR-011.

**Allowed project references** (the only references that
compile in M1; the graph is extended when the deferred
projects are created in their milestones):

- `App` → `Application`, `Providers.Abstractions` (M1).
  `App` → `Infrastructure` (added in M4).
- `App` → **any number of** provider implementation
  projects, **but only through the composition root**
  (see "The Composition Root" below). The composition
  root is the only place in `App` that may reference
  `Providers.<X>` projects directly. UI pages, components,
  application services, domain types, and provider
  contracts must never reference concrete provider
  implementations directly.
- `Application` → `Domain`, `Providers.Abstractions` (M1).
  `Application` → `Infrastructure` (added in M4).
- `Domain` → (no project references; pure types).
- `Infrastructure` (M4) → `Domain`.
- `Providers.Abstractions` → (no project references).
- `Providers.<X>` → `Providers.Abstractions`, `Infrastructure`.
- `UnitTests` → `Application`, `Domain`, `Providers.Abstractions`.
- `ComponentTests` → `App`, `Application`, `Domain`, `Providers.Abstractions`.
- `ArchitectureTests` → every project that exists in the
  current milestone.
- `ProviderContractTests` (M4) → `Providers.Abstractions`, the
  implementation under test, and `Tests.Common` (added when
  the first provider is implemented).

**The Composition Root.** The composition root is the
set of files in `AiEng.Platform.App` that exist for the
sole purpose of **wiring concrete provider
implementations into the DI container at startup** and
**reading the user's configuration to decide which
providers are registered for this run**. In M1 and
beyond, the composition root consists of:

- `Program.cs` (the host entry point).
- Dedicated registration extension methods under
  `AiEng.Platform.App/Composition/` (for example,
  `OllamaServiceCollectionExtensions`, `GitServiceCollectionExtensions`),
  one per provider family or per concrete provider
  implementation, each named after the capability
  or the implementation.
- Host-specific configuration modules under
  `AiEng.Platform.App/Configuration/` (for example,
  `ProviderOptions`).

The composition root is the **only** place in the
solution that may:

- Add a `using AiEng.Platform.Providers.<X>;`
  statement.
- Reference a `Providers.<X>` project.
- Register a concrete `*Provider` class in DI.
- Call a `*Provider`'s constructor directly.
- Resolve a concrete `*Provider` from the service
  collection.

The composition root is also the only place that
may **register more than one provider implementation
at a time**. A typical host run may register several
provider implementations simultaneously — for example,
`OllamaLaunchProvider` (the `IAgentRuntimeProvider` for
the Ollama Launch process boundary), `GitProvider`
(the `IGitProvider` for the host's `git` CLI),
`PowerShellProvider` (the `ITerminalProvider` for
PowerShell), `WindowsTerminalProvider` (a second
`ITerminalProvider`), `NativeWorktreeProvider` (the
native `IWorktreeProvider`), `NativeReviewProvider`
(the native `IReviewProvider`), and `NoMistakesProvider`
(the `IQualityGateProvider` for the No Mistakes HTTP
API). Every registered implementation is visible to
the UI through the family-scoped registry; the UI
selects one at runtime when the user makes a choice.

The composition root is **not**:

- A place for business logic.
- A place for UI rendering.
- A place for application services (those live in
  `AiEng.Platform.Application`).
- A place for orchestration between providers (that
  cooperation belongs in the application layer).
- A place to bypass the family contract. The
  composition root registers concrete implementations
  **as the family contract** (`services.AddSingleton<IGitProvider, GitProvider>()`),
  never as themselves for direct consumer use.

**May and may not (composition root):**

- May reference any number of `Providers.<X>` projects.
- May register multiple provider implementations in
  the same DI container.
- May read configuration to decide which providers to
  register.
- May reference `Application`, `Domain`, and
  `Providers.Abstractions` for the contracts the
  providers satisfy.
- May not contain business logic, rendering logic,
  or domain rules.
- May not be the only consumer of a provider
  implementation; an implementation that is only
  registered and never used through the registry is
  a smell and is reviewed.

**May and may not (everywhere else in `App` and in
`Application`):**

- May reference `Providers.Abstractions` for the
  family contracts (`IGitProvider`,
  `IAgentRuntimeProvider`, etc.).
- May resolve a provider through the
  `IProviderRegistry` and the family-scoped registries
  (`IAgentRuntimeProviderRegistry`,
  `IGitProviderRegistry`, etc.).
- May not `using AiEng.Platform.Providers.<X>;` from
  a page, a component, a service, a view model, a
  DTO, or a domain type.
- May not take a concrete `*Provider` type in a
  constructor.
- May not instantiate a `*Provider` with `new`.
- May not have a code-behind file that resolves a
  provider through anything other than the registry.

**Forbidden project references (architecture-test enforced):**

- `Domain` referencing anything except BCL types.
- `Providers.Abstractions` referencing anything except BCL
  types and the shared result envelope.
- Any layer referencing `App` (the App is the composition
  root, not a library).
- A provider referencing the `App` or any component.
- A provider referencing another provider.
- A page, component, application service, view model,
  DTO, or domain type referencing a `Providers.<X>`
  project. The composition root is the only place
  that may reference a concrete provider implementation
  project.
- A test reaching into internals through reflection or
  `InternalsVisibleTo`.

**Shared contracts.** Provider family contracts live in
`AiEng.Platform.Providers.Abstractions`. Domain types live in
`AiEng.Platform.Domain`. Application-layer interfaces (services,
view models) live in `AiEng.Platform.Application`. The
separation is enforced by project references; a contract in the
wrong project is a build failure.

**UI components.** Live in `AiEng.Platform.App` in M1. A
component library project is added only when a second surface
(CLI, IDE plugin) needs the same component.

**Tailwind assets.** `tailwind.config.js` lives at the
solution root. It reads the design tokens documented in
`docs/design-system.md` and emits a single `app.css` into
`AiEng.Platform.App/wwwroot/css/`. The CSS is the single
source of compiled styles; component CSS is co-located
scoped CSS for `@apply` rules that compose tokens.

**How architecture tests complement compile-time boundaries.**
Compile-time boundaries catch the **upward** dependency
(`Domain` cannot reference `Application`; a provider cannot
reference the `App`). The architecture tests in
`AiEng.Platform.ArchitectureTests` add four rules the
compiler cannot easily express:

1. **Namespace and folder conventions.** Every namespace
   matches the folder, every type lives in the file named
   after it, every public surface matches the catalogue in
   `docs/design-system.md` and `docs/provider-guidelines.md`.
2. **The no-code-comments rule** (AGENTS.md Rule 13) and the
   no-`Process.Start`-outside-infrastructure rule. The
   no-`Process.Start` rule is enforced as soon as
   `AiEng.Platform.Infrastructure` lands in M4; before then,
   the rule's test is registered but disabled, and the
   activation milestone is recorded in `ROADMAP.md`.
3. **The composition-root rule** (per ADR-016): the test
   `Only_CompositionRoot_MayReference_ConcreteProviders`
   fails the build if any source file outside the
   `AiEng.Platform.App/Composition/` folder contains
   a `using AiEng.Platform.Providers.<X>;` statement
   or instantiates a `*Provider` type. The companion
   tests `Pages_DoNotReference_ConcreteProviders`,
   `Application_DoesNotReference_ConcreteProviders`, and
   `Components_DoNotInject_ConcreteProviders` are part of
   the same rule. The four tests are recorded in the
   matrix in `ROADMAP.md` § 4. The composition-root test
   family is **registered but disabled** until the first
   concrete provider implementation lands (M4-D per
   `ROADMAP.md`); before then, no `Providers.<X>` project
   exists, so the rule is satisfied by construction, and
   the tests are enabled when the first project lands.
4. **The progressive self-dogfooding rule** (ADR-013): the
   architecture tests verify that the abstractions delivered
   by each milestone are actually consumed by the code that
   ships in later milestones, and that no direct bypass
   exists.

The architecture tests are part of the definition of done
for every milestone. A milestone is not done until the
architecture tests for its layer pass.

---

## 2.6 Progressive Self-Dogfooding (per ADR-013)

Every milestone must consume the stable reusable
capabilities delivered by earlier milestones. Later
milestones must not bypass earlier platform abstractions
with temporary direct implementations.

The rule is recorded as Rule 14 in `AGENTS.md`. The
**progressive self-dogfooding matrix** in `ROADMAP.md` records
the capabilities each milestone delivers, the later
milestones that must use them, the prohibited direct bypass,
the validation, and the enforcing test. The matrix is not
aspirational; each row is enforced by a specific test that
fails the build when violated.

External-tool dogfooding (the development team uses an
external tool manually while building the platform) is
**separate** from platform self-dogfooding (later platform
milestones use earlier platform abstractions). The two are
governed by separate workflows:

- External-tool dogfooding:
  [`.ai/workflows/tool-dogfooding.md`](./.ai/workflows/tool-dogfooding.md)
  and the dogfooding checkpoints in `ROADMAP.md`.
- Platform self-dogfooding: the matrix in `ROADMAP.md` and
  the architecture tests in
  `AiEng.Platform.ArchitectureTests`.

Both require explicit user approval where commands or
repository changes are involved.

---

## 4. The Provider Model

### 4.1 Why Provider-Based

The platform is a long-lived shell around **changing** external tools.
The Ollama product surface will evolve. The Claude contract will
change. New runtimes will appear. The internal products
(Treehouse, No Mistakes, Lavish Axi, GNHF, Firstmate) are
evolving tools whose shapes we cannot fully predict. Git,
PowerShell, WSL, and Windows Terminal are host facilities whose
availability and configuration change across machines and
policies.

If the UI depends on any of these directly, every change ripples through
the entire application. If the UI depends on a contract, only the
provider implementation needs to change.

This is the entire justification for the provider model.

The families are capability-oriented, per ADR-012. The mappings are
explicit:

- **Ollama Launch** → `IAgentRuntimeProvider` (M6, the
  platform's first coding-agent runtime). The provider owns a
  process boundary; the application invokes it through
  `ollama launch claude --model minimax-m3:cloud` (and the
  matching flag combinations the platform supports).
- **Ollama API** → a future `IAgentRuntimeProvider` that
  speaks to the Ollama HTTP API directly (a different code
  path from Ollama Launch). It is distinct from Ollama
  Launch because the transport and the failure surface are
  different: Launch is a process boundary, the API is an
  HTTP boundary. The two are **separate integrations**,
  not two flavours of one provider. The Ollama API
  provider is deferred to a later milestone.
- Claude, OpenAI, Codex → `IAgentRuntimeProvider`.
- Git → `IGitProvider`.
- PowerShell, Windows Terminal, WSL, Git Bash → `ITerminalProvider`.
- Treehouse, native Git worktree → `IWorktreeProvider`.
- No Mistakes → `IQualityGateProvider`.
- Lavish Axi, native HTML review → `IReviewProvider`.
- GNHF → `IAutonomousLoopProvider`.
- Firstmate through WSL, native orchestration → `IOrchestrationProvider`.

Vague family names such as `Assistant`, `Deployment`, and `Internal`
are rejected.

### 4.2 Provider Anatomy

Every provider:

1. Implements a capability-oriented contract interface (for example,
   `IAgentRuntimeProvider`). Every family contract inherits from
   `IProvider` to share the metadata members.
2. Is registered through DI in `Program.cs` keyed by a stable
   `ProviderId` (for example, `"ollama"`, `"claude"`).
3. Exposes its capabilities through a uniform `DescribeAsync()` method.
4. Reports health through a uniform `HealthAsync()` method.
5. Is configurable through a strongly-typed options class.
6. Is testable in isolation by mocking the contract.

### 4.3 Provider Discovery

Providers are discovered at startup, not hardcoded in the UI. A page that
shows "available runtimes" calls the family registry
(`IAgentRuntimeProviderRegistry` for runtimes, the matching registry
for each capability) and renders the result through `AppProviderCard`.
The UI never references an implementation type directly.

This means a user (or an enterprise deployment) can disable a provider, and
the UI simply stops showing it. The page does not need to know.

### 4.4 Provider Authoring Rules

See [`docs/provider-guidelines.md`](./docs/provider-guidelines.md) for the
full authoring guide. The summary:

- One folder per provider.
- One file per public type.
- No provider-to-provider references.
- No provider references the UI.
- All async work returns `Task<T>` and respects cancellation tokens.
- All errors are mapped to a common `ProviderResult<T>` envelope.
- Configuration is strongly typed and validated at startup.

### 4.5 Provider Lifecycle States

A provider moves through five distinct states during the
lifetime of a host run. The states are the vocabulary
the registry, the health poller, the configuration UI,
and the diagnostics page share:

1. **Compiled-in.** The provider implementation is a
   referenced project in the solution's compile graph.
   The implementation is reachable from the composition
   root (`AiEng.Platform.App/Composition/`) and from
   the family contract test project. A provider that is
   not compiled in cannot be registered, enabled,
   healthy, or selected. Every concrete provider
   implementation added to the solution enters this
   state the moment the `AiEng.Platform.Providers.<X>/`
   project is created.

2. **Registered.** The composition root has added the
   provider to the DI container. Registration is the
   act of binding the concrete `*Provider` class to
   the family contract
   (`services.AddSingleton<IGitProvider, GitProvider>()`).
   A registered provider is reachable through DI but
   may not be visible to the user — it is registered
   even when its configuration section is missing,
   because registration happens once at host startup
   and the section's presence is checked at
   enablement time.

3. **Enabled.** The provider's configuration section
   is present and valid. The user (or the deployment)
   has opted in. An enabled provider is the only
   provider the UI can see through the family-scoped
   registry; disabled providers are absent from the
   registry's listing. A registered provider that is
   not enabled is invisible to the UI but is still
   present in DI; an enabled provider is present in
   the registry and reachable through
   `IAgentRuntimeProviderRegistry.List()` and the
   matching registries for the other families.

4. **Healthy.** A periodic health check has confirmed
   the provider is reachable and operational. The
   `IProviderRegistry` and the family-scoped registries
   expose the provider's current `ProviderHealth`
   (`Healthy`, `Degraded`, `Unhealthy`). A provider
   may be enabled and unhealthy at the same time;
   the UI renders the health state through
   `AppHealthDot` and `AppProviderCard` and may offer
   the user a way to disable the unhealthy provider.
   The health poller is the source of truth for this
   state; the registry caches the result.

5. **Selected.** The user has chosen this provider
   for a specific operation. A runtime is selected
   when the user picks it from `AppRuntimePicker`; a
   worktree provider is selected when the user picks
   it from the worktree page; a quality-gate provider
   is selected when the user runs a gate. Selection
   is per-operation: the same provider may be
   selected for one operation and not for another.
   A provider that is not enabled cannot be
   selected; a provider that is selected but
   unhealthy produces a `ProviderResult<T>.Failure`
   with a category that the application layer
   translates into an `AppErrorState`.

The states are distinct, not cumulative. A provider
is one of these states at any moment; the next state
is reached through a specific event (compile, register,
enable, health-check, select). The states are the
basis of the configuration UI's affordances: a
"disabled" toggle removes the configuration section
and transitions the provider from `Enabled` back to
`Registered`; a "test connection" button triggers a
health check and surfaces the `Healthy` /
`Degraded` / `Unhealthy` state; a runtime picker
lists only providers that are `Enabled`, sorted by
`Healthy` first.

The state model is recorded as part of ADR-016 and
is operationalised in
`docs/provider-guidelines.md` § 4.6. The catalogue
in `docs/provider-guidelines.md` § 10 records the
state of every provider as it moves through the
milestones: a planned provider is `Compiled-in` in
the future; an implemented provider is `Compiled-in`
on day one; the runtime UI in M6 surfaces
`Enabled` and `Healthy`; M7 surfaces `Selected`.

---

## 5. Component Model

### 5.1 Why Component-First

In a Blazor codebase, the fastest path to a working page is to write
inline markup. The fastest path to **a long-lived codebase** is to do the
opposite. Inline markup becomes a maintenance tax within weeks.

Every page in this platform is an **orchestration of named, documented
components**. If a piece of UI is used twice, it is a component. If a
piece of UI is complex enough to need state, it is a component with a
code-behind file.

### 5.2 Component Categories

| Category       | Lives in          | Examples                                           |
| -------------- | ----------------- | -------------------------------------------------- |
| Primitives     | `Components/`     | `AppButton`, `AppBadge`, `AppStatusDot`            |
| Containers     | `Components/`     | `AppCard`, `AppSection`, `AppDialog`               |
| Domain         | `Components/`     | `AppProviderCard`, `AppProjectCard`, `AppTaskCard` |
| Navigation     | `Navigation/`     | `AppSidebar`, `AppBreadcrumb`, `AppTopBar`         |
| Feedback       | `Components/Feedback/` | `AppLoading`, `AppSkeleton`, `AppEmptyState`   |
| Layouts        | `Layouts/`        | `MainLayout`, `EmptyLayout`                        |
| Page-specific  | `Pages/`          | `Dashboard.razor`                                  |

The full rules are in [`docs/component-guidelines.md`](./docs/component-guidelines.md).

### 5.3 Component Lifecycle Contract

Every component answers four questions in its public API:

1. **What does it render?** (Razor + parameters)
2. **What does it do?** (Methods, not just events)
3. **How does it fail?** (Error slot or `AppErrorState`,
   when the component owns a data fetch.)
4. **How does it wait?** (Loading slot or `AppLoading`,
   when the component owns a data fetch.)

A component that does not own a data fetch — a pure
primitive (`AppButton`, `AppBadge`, `AppStatusDot`,
`AppTooltip`) or a presentational container (`AppCard`,
`AppSection`, `AppDialog`) — answers the four questions
through the slots it actually has (`Header`, `Footer`,
`Actions`) and does not invent data-fetching slots of
its own. The `Loading`, `Empty`, `Error`, and `Populated`
state slots are required only on **data-owning
components**; primitives and containers render whatever
the parent gives them. The four-state rule is
conditional on data ownership (see ADR-014,
`docs/design-system.md` § 5.4, and
`docs/component-guidelines.md` § 4.3).

---

## 6. Data Flow

### 6.1 Read Path (UI receives data)

```
Page  →  Service  →  Provider  →  External tool
                                  ↓
Page  ←  ViewModel/DTO  ←  Provider  ←  response
```

### 6.2 Write Path (UI triggers work)

```
Page  →  Service  →  Provider  →  External tool
                                  ↓
Page  ←  result/error  ←  Provider  ←  response
```

### 6.3 Streaming Path (long-running work)

```
Page subscribes to IService.StreamAsync(...)
   ↓
Service subscribes to provider IAsyncEnumerable<>
   ↓
Provider yields events as they arrive
   ↓
Page renders incrementally
```

Long-running work is never blocked behind a single awaited call.

### 6.4 Cancellation

Every async call in the platform accepts a `CancellationToken`. Every
component that initiates a long-running call owns the token. Navigating
away from a page cancels its in-flight work.

---

## 7. Dependency Injection

- Services are registered with explicit lifetimes (`Singleton`,
  `Scoped`, `Transient`). `Scoped` is the default for application
  services.
- Provider implementations are registered as themselves, but resolved
  through a **registry** keyed by `ProviderId`. The UI never injects a
  specific provider.
- Components receive their dependencies through constructor injection in
  the code-behind. They do not use `IHttpClientFactory` directly; the
  service they consume already has the configured client.

---

## 8. State Management

- **Local state** lives in the component that owns it.
- **Cross-component state** lives in a scoped service (e.g.
  `IWorkspaceState`, `ISessionState`).
- **Cross-session state** lives in infrastructure (settings store, history
  store).
- **Server-pushed state** is delivered through a scoped event bus, not
  through static singletons.

`CascadingValue` is used sparingly. When a value is needed by more than
two unrelated components, it is a service.

---

## 9. Error Handling

- All provider methods return `ProviderResult<T>` with a discriminated
  outcome (`Success`, `Failure`, `Unavailable`).
- Application services translate `ProviderResult` into domain outcomes
  the UI can render.
- The UI renders every error through a uniform `AppErrorState` pattern.
- Unhandled exceptions never reach the user as stack traces. The
  presentation layer logs and shows a friendly message; the application
  layer logs structured data; the infrastructure layer persists the full
  exception for diagnostics.

---

## 10. Persistence

Three stores, each with a single responsibility:

- **Settings store** — user preferences, provider configuration.
- **Workspace store** — per-project state (open files, recent sessions,
  active providers).
- **History store** — append-only event log for sessions, runs, and
  provider calls.

Stores live in infrastructure, behind interfaces defined in the
application layer. The UI never touches them directly.

---

## 11. Security Model

- All provider calls go through contracts that enforce validation.
- Secrets (API keys, tokens) are loaded from the operating system
  credential store. The platform never persists plaintext secrets to
  disk.
- Every external command that runs on the user's machine is mediated by a
  provider and rendered to the user before execution.
- Process execution (PowerShell, WSL, Windows Terminal) is opt-in per
  provider and per call.

---

## 12. Performance Principles

- The UI thread never blocks.
- Long-running work streams.
- Lists virtualise.
- Re-renders are minimised through `ShouldRender` and parameter
  immutability.
- The design system tokens are loaded once at startup.

---

## 13. Testing Strategy

| Layer            | Test type           | Tools                              |
| ---------------- | ------------------- | ---------------------------------- |
| Presentation     | bUnit component tests | xUnit + bUnit                    |
| Application      | Unit tests          | xUnit + FluentAssertions           |
| Provider         | Contract tests + integration tests against a fake external tool | xUnit |
| Infrastructure   | Integration tests   | xUnit + Testcontainers where applicable |

Provider implementations are tested against the contract they implement,
not against the UI. A passing contract test is the minimum bar.

---

## 14. Future Considerations

These are intentionally **not** implemented today, but the architecture
must not prevent them:

- **Multi-surface delivery.** A WebAssembly build, a CLI, and an IDE
  plugin all consume the same provider contracts.
- **Remote workspaces.** Providers that work over a remote bridge
  (e.g. SSH, WSL) plug in identically to local providers.
- **Plugin model.** Third-party providers delivered as a loadable
  assembly, discovered at startup, registered through the same registry.
- **Telemetry.** Cross-cutting telemetry implemented in infrastructure
  and consumed by application services.

These are documented so that today's decisions do not paint us into
tomorrow's corners.

---

## 15. Anti-Patterns (Architecture Smells)

If a code review sees any of these, the change is rejected:

- A Razor page calling an external HTTP client directly.
- A provider referencing a Razor component.
- A service returning `HttpResponseMessage` to a page.
- A `static` field holding runtime state.
- A `Shared/` folder used as a dumping ground.
- A `Manager` or `Helper` class with no clear responsibility.
- A magic string in a component that names a provider.

---

## 16. Diagram Index

A picture of the layers, the provider model, the data flow, and the
deployment shape is rendered in the project wiki and exported into
`docs/diagrams/`. The text in this document is the source of truth; the
diagrams are derived from it. If they disagree, this document wins.
