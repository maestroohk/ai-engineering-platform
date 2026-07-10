# docs/architecture-principles.md

> The architectural principles that govern how the platform is
> structured, how layers interact, and how new functionality is
> added. This document expands on `ARCHITECTURE.md` with the
> operational rules. Read it after `AGENTS.md` and
> `ARCHITECTURE.md`.

---

## 1. The Architectural Goals

The platform is designed to be:

1. **Long-lived.** A change to one area must not ripple across
   the codebase.
2. **Extensible.** New providers, new runtimes, new surfaces
   (CLI, web, IDE plugin) must be addable without touching the
   existing code.
3. **Testable in isolation.** Every layer can be exercised
   without the layers around it.
4. **Understandable.** A new contributor — human or AI — must be
   productive on day one.

Every architectural decision is checked against this list before
it is adopted.

---

## 2. The Five Layers and the Project Map

The platform has five layers. Each layer depends only on the
layers below it.

```
+--------------------------------------------------------------+
|  Presentation     (Blazor Pages, Components, Layouts)        |
+--------------------------------------------------------------+
|  Application      (Services, ViewModels, DTOs, Models)       |
+--------------------------------------------------------------+
|  Contracts        (Provider interfaces, abstractions)       |
+--------------------------------------------------------------+
|  Implementations  (Provider classes, infrastructure)         |
+--------------------------------------------------------------+
|  Infrastructure   (Logging, Persistence, Process, Config)   |
+--------------------------------------------------------------+
```

The dependency direction is enforced at **compile time** by the
project boundaries in `ARCHITECTURE.md` § 2.5. The five layers
map to **four projects in M1**, plus two projects the
architecture will eventually need but that are **deferred** to
the milestone that introduces them:

| Layer            | Project                                | Milestone   |
| ---------------- | -------------------------------------- | ----------- |
| Presentation     | `AiEng.Platform.App`                   | M1          |
| Application      | `AiEng.Platform.Application`           | M1          |
| Contracts        | `AiEng.Platform.Providers.Abstractions` + `AiEng.Platform.Domain` | M1 |
| Implementations  | `AiEng.Platform.Providers.<X>` (per provider) | per `ROADMAP.md` (deferred) |
| Infrastructure   | `AiEng.Platform.Infrastructure`        | M4 (deferred from M1) |

The M1 project set is the four source projects in the **M1**
row plus the three test projects listed in
`docs/folder-structure.md` § 4. `Infrastructure` and
`ProviderContractTests` are deliberately absent from M1; they
are created when their first consumer or first test target
lands. A project that ships without a consumer or a target is a
speculative project; speculative projects are rejected (per
`AGENTS.md` and ADR-011).

The Domain project is a peer of the Providers.Abstractions
project because it hosts pure domain types and value objects
that are conceptually part of the contract surface (the
domain is the platform's contract with itself).

A layer may depend on the layers **below** it. A layer may not
depend on the layers **above** it. The dependency direction is
strictly downward.

### 2.1 Presentation

- Blazor pages, components, layouts, dialogs.
- No external I/O.
- No provider knowledge.
- Allowed dependencies: Application layer services (via DI),
  Components (only those in the design system), Blazor framework.

### 2.2 Application

- Services, view models, DTOs, domain models, orchestrators.
- Translates UI intent into provider calls and back.
- No rendering, no provider implementations, no direct I/O
  without an abstraction.

### 2.3 Contracts

- Pure interfaces that describe the shape of provider families.
- No implementation, no external dependencies, no logic.
- One folder per family under `Providers/<Family>/Contracts/`.

### 2.4 Implementations

- Concrete provider classes.
- HTTP clients, SDK wrappers, process runners — all private.
- Allowed dependencies: Contracts, Infrastructure.

### 2.5 Infrastructure

- Logging, configuration, persistence, process, time, credentials.
- Implementations of abstractions defined in Contracts or
  Application.
- No knowledge of UI, no knowledge of providers.

---

## 3. The Dependency Rule

Dependencies flow downward. A class in a higher layer may depend
on a class in a lower layer. A class in a lower layer must not
know that a class in a higher layer exists.

This rule is enforced by:

- **Code review.** Reviewers reject any import that crosses a
  layer in the wrong direction.
- **Architecture tests.** A test scans the codebase and fails if
  it finds an upward dependency.
- **Folder structure.** The structure makes upward dependencies
  awkward to write.

A dependency that genuinely must cross a layer is a signal that
the architecture is wrong, not that the rule is wrong.

---

## 4. The Provider Model in Depth

### 4.1 The Contract

A provider contract is an interface that:

- Lives in `AiEng.Platform.Providers.Abstractions/<Family>/`.
- Is named after the **capability**, not after a vague category
  (per ADR-012): `IAgentRuntimeProvider`, `IGitProvider`,
  `ITerminalProvider`, `IWorktreeProvider`, `IQualityGateProvider`,
  `IReviewProvider`, `IAutonomousLoopProvider`,
  `IOrchestrationProvider`. `Assistant`, `Deployment`, and
  `Internal` are not valid family names.
- Inherits from `IProvider` (the small base metadata contract).
- Has a stable `ProviderId` constant.
- Exposes a uniform shape: `Id`, `DisplayName`, `DescribeAsync`,
  `HealthAsync`, `ConfigureAsync`, and the family-specific methods.

A new family is added only when the capability is genuinely
shared by at least two providers. A one-off provider is filed
under the closest existing family or its own contract is
filed as an ADR for review.

**Tool-to-family mapping** (per ADR-012; the canonical mapping
lives in `docs/provider-guidelines.md` § 2, restated here so
the architectural principle is in one place):

- **Treehouse** is a **worktree** provider. It manages isolated
  per-task copies of a repository. The platform also offers a
  native worktree implementation built on `git worktree`.
- **No Mistakes** is a **quality gate** provider. It evaluates
  proposed changes against a configured quality rubric and
  reports pass/fail.
- **Lavish Axi** is a **review** provider. It produces
  structured code review output that the application can
  render alongside human review. It is **not** an assistant,
  not a deployment tool, and not an internal utility.
- **GNHF** is an **autonomous loop** provider. It runs the
  iterative plan-build-review loop on a task.
- **Firstmate** is an **orchestration** provider. It
  coordinates multiple sub-tasks across the team.
- **Ollama, Claude, OpenAI, Codex** are **agent runtime**
  providers. Their capability is executing an agent model
  against a prompt. The first runtime the platform ships
  with is the **Ollama Launch** path
  (`ollama launch claude --model minimax-m3:cloud`), which
  is an `IAgentRuntimeProvider` that owns the local process
  boundary (see `ARCHITECTURE.md` § 4 and
  `docs/provider-guidelines.md` § 2).
- **Git** is a **source control** provider.
- **PowerShell, Windows Terminal, WSL, Git Bash** are
  **terminal execution** providers.

### 4.2 The Implementation

A provider implementation:

- Lives in `src/AiEng.Platform.Providers.<X>/` (one
  project per provider implementation, added when the
  provider is implemented per ADR-011).
- Implements the family contract.
- Owns its HTTP clients, SDK wrappers, and process runners as
  private members.
- Is registered in DI keyed by `ProviderId`.
- Has a strongly-typed options class bound from configuration.
- Has a contract test that proves it satisfies the contract.

### 4.3 The Registry

A provider registry:

- Exposes `IEnumerable<ProviderDescriptor> List()`.
- Exposes `ProviderDescriptor? Resolve(ProviderId id)`.
- Is itself a service, not a static singleton.
- Is the only way the UI learns that providers exist.

### 4.4 Discovery at Runtime

When the application starts:

1. Configuration is loaded.
2. Provider options are validated.
3. Each configured provider is registered in DI.
4. The registry is built from the configured providers.
5. The UI renders the registry through `AppProviderCard` and
   friends.

A provider that is not configured is invisible to the UI. A
provider that fails configuration validation is logged and
omitted. The UI never sees a half-configured provider.

### 4.5 The Composition Root

The composition root is the set of files in
`AiEng.Platform.App` whose sole purpose is to wire concrete
provider implementations into the DI container at host
startup and read configuration to decide which providers
are registered for this run. It consists of:

- `Program.cs` (the host entry point).
- Dedicated registration extension methods under
  `AiEng.Platform.App/Composition/`, one per provider
  family or per concrete provider implementation, each
  named after the capability or the implementation.
- Host-specific configuration modules under
  `AiEng.Platform.App/Configuration/`.

The composition root is the **only** place in the solution
that may:

- Add a `using AiEng.Platform.Providers.<X>;` statement.
- Reference a `Providers.<X>` project.
- Register a concrete `*Provider` class in DI.
- Call a `*Provider`'s constructor directly.
- Resolve a concrete `*Provider` from the service
  collection.

The composition root is also the only place that may
**register more than one provider implementation at a
time**. A typical host run registers several provider
implementations simultaneously (for example,
`OllamaLaunchProvider`, `GitProvider`, `PowerShellProvider`,
`WindowsTerminalProvider`, `NativeWorktreeProvider`,
`NativeReviewProvider`, `NoMistakesProvider`). Every
registered implementation is visible to the UI through the
family-scoped registry; the UI selects one at runtime when
the user makes a choice.

The composition root is **not** a place for business
logic, rendering logic, or domain rules. It registers
providers and reads configuration; it does not orchestrate
them. Provider cooperation belongs in the application layer.

The composition-root rule is the basis of the
`Only_CompositionRoot_MayReference_ConcreteProviders`,
`Pages_DoNotReference_ConcreteProviders`,
`Application_DoesNotReference_ConcreteProviders`, and
`Components_DoNotInject_ConcreteProviders` architecture
tests in `AiEng.Platform.ArchitectureTests`. The four
tests are part of the definition of done for M4-D
(`ROADMAP.md`); they are registered but disabled until the
first concrete provider implementation lands. See
`ARCHITECTURE.md` § 2.5 and ADR-016 for the full rule.

### 4.6 Provider Lifecycle States

A provider progresses through five distinct states during
the lifetime of a host run. The states are the vocabulary
the registry, the health poller, the configuration UI, and
the diagnostics page share. The states are distinct, not
cumulative: a provider is exactly one of these states at
any moment; the next state is reached through a specific
event (compile, register, enable, health-check, select).

1. **Compiled-in.** The provider implementation is a
   referenced project in the solution's compile graph.
   Reachable from the composition root and from the
   family contract test project. A provider that is not
   compiled in cannot be registered, enabled, healthy, or
   selected. Every concrete provider implementation added
   to the solution enters this state the moment the
   `AiEng.Platform.Providers.<X>/` project is created.

2. **Registered.** The composition root has added the
   provider to DI. Reachable through DI but may not be
   visible to the user. Registration is the act of binding
   the concrete `*Provider` class to the family contract
   (`services.AddSingleton<IGitProvider, GitProvider>()`).
   Registration happens once at host startup; the
   configuration section's presence is checked at
   enablement time.

3. **Enabled.** The provider's configuration section is
   present and valid; the user (or the deployment) has
   opted in. The only providers the UI sees through the
   family-scoped registry. A registered provider that is
   not enabled is invisible to the UI but is still present
   in DI. The configuration UI's "disabled" toggle removes
   the configuration section and transitions the provider
   from `Enabled` back to `Registered`.

4. **Healthy.** A periodic health check has confirmed the
   provider is reachable and operational. The
   `IProviderRegistry` and the family-scoped registries
   expose the provider's current `ProviderHealth`
   (`Healthy`, `Degraded`, `Unhealthy`). A provider may
   be enabled and unhealthy at the same time; the UI
   renders the health state through `AppHealthDot` and
   `AppProviderCard` and may offer the user a way to
   disable an unhealthy provider. The health poller is
   the source of truth for this state; the registry
   caches the result.

5. **Selected.** The user has chosen this provider for a
   specific operation. A runtime is selected when the user
   picks it from `AppRuntimePicker`; a worktree provider
   is selected when the user picks it from the worktree
   page; a quality-gate provider is selected when the user
   runs a gate. Selection is per-operation: the same
   provider may be selected for one operation and not for
   another. A provider that is not enabled cannot be
   selected; a provider that is selected but unhealthy
   produces a `ProviderResult<T>.Failure` with a category
   that the application layer translates into an
   `AppErrorState`.

The state model is recorded in `ARCHITECTURE.md` § 4.5
and ADR-016 and is operationalised in
`docs/provider-guidelines.md` § 4.6. The catalogue in
`docs/provider-guidelines.md` § 10 records the state of
every provider as it moves through the milestones: a
planned provider is `Compiled-in` in the future; an
implemented provider is `Compiled-in` on day one; the
runtime UI in M6 surfaces `Enabled` and `Healthy`; M7
surfaces `Selected`.

---

## 4.7 Progressive Self-Dogfooding (per ADR-013)

Every milestone must consume the stable reusable capabilities
delivered by earlier milestones. Later milestones must not
bypass earlier platform abstractions with temporary direct
implementations.

The rule is operationalised as a **progressive self-dogfooding
matrix** in `ROADMAP.md`. Each row records:

- the capability delivered by the milestone,
- the later milestones that must use it,
- the direct bypass that is prohibited,
- the validation that confirms consumption,
- the architecture test or contract test that fails the build
  on bypass.

The matrix is enforced by the architecture tests in
`AiEng.Platform.ArchitectureTests`. The tests assert, for
example, that:

- The `App` and `Application` projects do not contain a
  literal `Process.Start` call (the only legal way to start
  a process is through `IProcessRunner` from
  `AiEng.Platform.Infrastructure`).
- Provider implementations do not call other providers
  directly.
- The provider registry surface is the only way the UI
  learns that providers exist.
- The domain types are the only way a provider or a service
  accesses platform entities (no raw `string` IDs in
  cross-layer calls).

External-tool dogfooding (the development team uses an
external tool manually while building the platform) is a
**separate** discipline, governed by
[`.ai/workflows/tool-dogfooding.md`](./../.ai/workflows/tool-dogfooding.md)
and the dogfooding checkpoints in `ROADMAP.md`. The two
disciplines must not be confused.

---

## 5. The Result Envelope

Every provider method that can fail returns a `ProviderResult<T>`.
The envelope has three outcomes:

- `Success` — the call succeeded; `Value` is present.
- `Failure` — the call failed; `Error` describes why.
- `Unavailable` — the provider is not configured or not
  reachable; the UI may offer to enable it.

The envelope is the **only** shape the application layer accepts
from providers. Exceptions are reserved for programmer errors.

```csharp
public readonly record struct ProviderResult<T>(
    ProviderOutcome Outcome,
    T? Value,
    ProviderError? Error);
```

The application layer translates `ProviderResult` into domain
outcomes the UI understands. The UI renders outcomes through
`AppErrorState` and `AppEmptyState`.

---

## 6. Dependency Injection Conventions

### 6.1 Lifetime

| Service kind        | Lifetime     |
| ------------------- | ------------ |
| Provider implementation | Singleton |
| Provider registry   | Singleton    |
| Application service | Scoped       |
| View model          | Scoped       |
| State holder        | Scoped       |
| Component state     | Transient (component lifetime) |
| HTTP client         | Singleton (via `IHttpClientFactory`) |
| Configuration       | Singleton    |

The default for a new service is `Scoped`. Use a different
lifetime deliberately and document it.

### 6.2 Registration

- Services are registered in `Program.cs` through extension
  methods grouped by area (`services.AddWorkspaceServices()`,
  `services.AddOllamaProvider()`).
- A registration extension is co-located with the service it
  registers when the service is private.
- Provider implementations are registered through a generic
  helper (`AddRuntimeProvider<OllamaProvider>(...)`).

### 6.3 Resolution

- Components resolve services through constructor injection in
  the code-behind.
- Services resolve other services through constructor injection.
- Static service location (`MyServiceProvider.Get<T>()`) is
  forbidden.

---

## 7. State Management

### 7.1 The Three Tiers

| Tier            | Lifetime                  | Example                          |
| --------------- | ------------------------- | -------------------------------- |
| Local           | Component lifetime        | A form's draft value             |
| Session         | User session              | The active workspace             |
| Persistent      | Application lifetime      | User preferences, history        |

A state is **session-tier** when more than one component reads
it. A state is **persistent-tier** when it survives a restart.

### 7.2 State Services

- Session-tier state is held in a scoped service
  (`IWorkspaceState`, `ISessionState`).
- The state service exposes observable properties or events; it
  does not implement `INotifyPropertyChanged` directly (Blazor
  rerenders on event callbacks).
- Persistent-tier state is held in an infrastructure store
  (`IUserSettingsStore`, `IHistoryStore`).

### 7.3 What State Is Not

- State is not a `static` field. Ever.
- State is not held in a singleton service when it should be
  scoped.
- State is not stored in the URL when it is large (use the
  state service, not query parameters).

---

## 8. Streaming and Concurrency

- Long-running provider work is exposed as
  `IAsyncEnumerable<StreamEvent>`.
- A `Channel<T>` is used internally to multiplex events from
  the provider to the consumer.
- A `SemaphoreSlim` is used to limit concurrent calls to a
  provider that has rate limits.
- Components that own streams implement `IAsyncDisposable` and
  cancel the stream in `Dispose`.

---

## 9. Error Handling Principles

- Errors flow through the `ProviderResult<T>` envelope.
- Exceptions are reserved for programmer errors.
- The UI renders errors through a uniform `AppErrorState`.
- Unhandled exceptions are caught at the application boundary
  and turned into a graceful `AppCrashBoundary`.

---

## 10. Persistence

Three stores, three responsibilities:

- **Settings store** — user preferences, provider configuration
  (not secrets).
- **Workspace store** — per-workspace state.
- **History store** — append-only event log of runs, sessions,
  provider calls.

Each store:

- Has a typed interface defined in the application layer.
- Has a single implementation in infrastructure.
- Is registered as `Scoped` (settings, workspace) or `Singleton`
  (history).
- Never references the UI.

---

## 11. Security Principles

- Secrets live in the OS credential vault. The platform never
  persists them to disk in plaintext.
- Provider calls that execute user-provided input on the host
  (PowerShell, WSL, Windows Terminal) are gated behind explicit
  user confirmation per call.
- Every external call is logged with its provider, target, and
  outcome.
- The platform runs with the user's normal user permissions. It
  does not require elevation.

---

## 12. Testing Strategy

| Layer            | Test type                | Tooling                |
| ---------------- | ------------------------ | ---------------------- |
| Presentation     | Component tests          | xUnit + bUnit          |
| Application      | Unit tests               | xUnit + FluentAssertions |
| Contracts        | Contract tests           | xUnit (test interface) |
| Implementations  | Integration tests        | xUnit + WebApplicationFactory |
| Infrastructure   | Integration tests        | xUnit                  |
| Architecture     | Architecture tests       | xUnit + NetArchTest    |

### 12.1 Contract Tests

A contract test is an xUnit class that exercises the contract
through the interface, with the concrete implementation
substituted. Every implementation of a contract passes the same
test suite. The test class is named
`<Provider>ContractTests` and lives in
`tests/AiEng.Platform.Tests/Providers/Contracts/`.

### 12.2 Architecture Tests

Architecture tests assert:

- **Composition-root rule (per ADR-016).** The composition
  root (`AiEng.Platform.App/Composition/`) is the only
  place in the solution that may reference a concrete
  `Providers.<X>` project. The tests
  `Only_CompositionRoot_MayReference_ConcreteProviders`,
  `Pages_DoNotReference_ConcreteProviders`,
  `Application_DoesNotReference_ConcreteProviders`, and
  `Components_DoNotInject_ConcreteProviders` fail the
  build if any source file outside the composition root
  imports or instantiates a concrete `*Provider` type.
  The four tests are part of the definition of done for
  M4-D (`ROADMAP.md`); they are registered but disabled
  until the first concrete provider implementation
  lands.
- **No upward dependencies.** Presentation does not
  depend on application services directly except through
  DI; application does not depend on infrastructure
  directly except through abstractions; a provider
  implementation does not depend on the `App` or any
  component.
- **Namespace and folder conventions.** Every namespace
  matches the folder, every type lives in the file named
  after it.
- **Component folder discipline.** Components in
  `Components/` do not depend on pages; `Shared/` is
  small (size assertion) and contains only
  cross-cutting concerns (content assertion).
- **The no-code-comments rule** (`AGENTS.md` Rule 13)
  and the no-`Process.Start`-outside-infrastructure
  rule. The `Process.Start` rule activates in M4 when
  `AiEng.Platform.Infrastructure` lands; before then the
  rule's test is registered but disabled.
- **The progressive self-dogfooding rule** (ADR-013):
  the abstractions delivered by each milestone are
  actually consumed by the code that ships in later
  milestones, and no direct bypass exists.

A broken architecture test is a release blocker.

---

## 13. Anti-Patterns

- A page that calls an `HttpClient` directly.
- A service that returns a `JsonElement` to a page.
- A provider that references a Razor component.
- A `static` field that holds runtime state.
- A `Manager` or `Helper` class with no clear responsibility.
- A magic string in a component that names a provider, theme,
  or role.
- A `Shared/` folder that contains anything but cross-cutting
  concerns.

---

## 14. Architectural Change Process

A change to the architecture:

1. Is proposed as a new ADR in `DECISIONS.md`.
2. Lists the layers affected and the trade-offs accepted.
3. Includes a migration plan for any code that the change
   invalidates.
4. Updates `ARCHITECTURE.md`, this document, and any other
   affected document in the same PR.
5. Passes review from at least one human architect.

A change that does not document itself in `DECISIONS.md` is
rejected.
