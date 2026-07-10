# Capability Mapping

> **The dependency model that connects every
> platform capability.**
>
> A capability depends on other capabilities. A
> "Launch Agent" capability depends on a "Runtime
> Registry" capability, which depends on a
> "Provider Registry" capability, which depends
> on the "Infrastructure" capabilities
> (`IProcessRunner`, `ICredentialVault`,
> `IClock`). The dependency graph is the map; the
> matrix in [`ROADMAP.md`](./../../ROADMAP.md) § 4
> is the operationalisation; the
> [`.ai/backlog/capabilities.md`](./../backlog/capabilities.md)
> file is the catalogue; the
> [`capabilities.json`](./capabilities.json) file
> is the machine-readable source.
>
> This document is the **readable model**. The
> JSON is the canonical schema. A new contributor
> reads this document to understand the
> dependency graph; a tool reads the JSON to
> enumerate it.

---

## 1. The Three Layers of Capabilities

The capabilities are organised in three layers.
A capability in a higher layer may depend on a
capability in a lower layer; the inverse is
forbidden.

| Layer | Layer name | Capabilities |
| ----- | ---------- | ------------ |
| 0 | **Infrastructure** | `IProcessRunner`, `ICredentialVault`, `IClock`, `IProjectStore` (durable) |
| 1 | **Provider Model** | `IProvider` (base), the eight family contracts, `IProviderRegistry` and family-scoped registries, `IProviderHealthService`, composition root |
| 2 | **Domain Orchestration** | `IProjectService`, `IWorktreeService`, `IRunService`, `IReviewService`, `IQualityGateService`, `IAutonomousLoopService`, `IOrchestrationService`, `IHistoryStore`, `IStreamingChannel`, `INavigationService` |
| 3 | **User Surface** | The Blazor pages, components, and forms that compose the application services. |

The dependency direction is **downward**:
Layer 3 depends on Layer 2; Layer 2 depends on
Layer 1; Layer 1 depends on Layer 0. The
compiler enforces the direction through the
project-reference graph (per
[`ARCHITECTURE.md`](./../../ARCHITECTURE.md) §
2.5 and
[`DECISIONS.md`](./../../DECISIONS.md) ADR-011,
ADR-016).

---

## 2. The Capability Dependency Graph

The full graph:

```
        Layer 3: User Surface
   +-------------------------------+
   | Pages, Components, Forms      |
   +---------------+---------------+
                   |
                   v
        Layer 2: Domain Orchestration
   +-------------------------------+
   | IProjectService               |
   | IWorktreeService              |
   | IRunService                   |
   | IReviewService                |
   | IQualityGateService           |
   | IAutonomousLoopService        |
   | IOrchestrationService         |
   | IHistoryStore                 |
   | IStreamingChannel             |
   | INavigationService            |
   +---------------+---------------+
                   |
                   v
        Layer 1: Provider Model
   +-------------------------------+
   | IProvider (base contract)     |
   | IAgentRuntimeProvider         |
   | IGitProvider                  |
   | ITerminalProvider             |
   | IWorktreeProvider             |
   | IQualityGateProvider          |
   | IReviewProvider               |
   | IAutonomousLoopProvider       |
   | IOrchestrationProvider        |
   | IProviderRegistry             |
   | IProviderHealthService        |
   | Composition root              |
   +---------------+---------------+
                   |
                   v
        Layer 0: Infrastructure
   +-------------------------------+
   | IProcessRunner                |
   | ICredentialVault              |
   | IClock                        |
   | IProjectStore (durable)       |
   | IHostCapabilitiesService      |
   +-------------------------------+
```

The arrows are **mandatory**. A page that
bypasses `IRunService` and calls a provider
directly violates the graph. A service that
bypasses `IProviderRegistry` and imports a
provider implementation violates the graph.
A provider that calls `Process.Start` directly
violates the graph.

---

## 3. The Five Composition Chains

The most important chains in the graph are the
five **composition chains** the platform
exercises. Each chain is a path from a user
action to the platform's effect on the host.

### 3.1 The Launch Chain

The chain that turns "the user picked a runtime
and a model" into "the agent is running".

```
User picks runtime + model
  → AppRuntimePicker
  → IRunService.LaunchAsync(...)
    → IAgentRuntimeProviderRegistry.Resolve(...)
      → IAgentRuntimeProvider.RunAsync(...)
        → IProcessRunner.RunAsync(...)
          → Process spawned
```

**Dependencies:**

- `AppRuntimePicker` (Layer 3) depends on
  `IRunService` (Layer 2).
- `IRunService` (Layer 2) depends on
  `IAgentRuntimeProviderRegistry` (Layer 1).
- `IAgentRuntimeProviderRegistry` (Layer 1)
  depends on `IProviderRegistry` (Layer 1)
  and the `IProvider` base contract (Layer 1).
- `IProviderRegistry` (Layer 1) is populated
  by the composition root (Layer 1) at host
  startup.
- The concrete `IAgentRuntimeProvider`
  (Layer 1) depends on `IProcessRunner`
  (Layer 0).

**Architecture tests:**

- `Only_CompositionRoot_MayReference_ConcreteProviders`
  — fails the build if the chain's
  registration site is outside the
  composition root.
- `Pages_Resolve_Providers_Through_Registry`
  — fails the build if `AppRuntimePicker`
  imports a concrete provider.
- `No_DirectProcessStart_OutsideInfrastructure`
  — fails the build if the concrete provider
  bypasses `IProcessRunner`.

**Milestones that consume the chain:** M6,
M7, M8.

### 3.2 The Worktree Chain

The chain that turns "the user picked a
project" into "the worktree is created".

```
User picks project + worktree options
  → AppWorktreeForm
  → IWorktreeService.CreateAsync(...)
    → IWorktreeProviderRegistry.Resolve(...)
      → IWorktreeProvider.CreateAsync(...)
        → IGitProvider (registry resolution)
          → IProcessRunner.RunAsync(...)
            → git worktree add
```

**Dependencies:**

- `AppWorktreeForm` (Layer 3) depends on
  `IWorktreeService` (Layer 2).
- `IWorktreeService` (Layer 2) depends on
  `IWorktreeProviderRegistry` (Layer 1).
- `IWorktreeProvider` (Layer 1) depends on
  `IGitProvider` (Layer 1) **through the
  registry**, not by direct import.
- `IGitProvider` (Layer 1) depends on
  `IProcessRunner` (Layer 0).

**Architecture tests:**

- `Only_CompositionRoot_MayReference_ConcreteProviders`
  — the chain's registration site is
  outside the composition root.
- `NativeProviders_Use_Contracts_Not_Implementations`
  — fails the build if
  `NativeWorktreeProvider` imports
  `GitProvider` directly.

**Milestones that consume the chain:** M5,
M6, M7, M8.

### 3.3 The Review Chain

The chain that turns "the user submitted the
worktree for review" into "the review
findings are rendered".

```
User submits worktree for review
  → AppReviewSubmit
  → IReviewService.SubmitAsync(...)
    → IReviewProviderRegistry.Resolve(...)
      → IReviewProvider.ReviewAsync(...)
        → Provider-specific
          (native HTML review or Lavish Axi)
```

**Dependencies:**

- `AppReviewSubmit` (Layer 3) depends on
  `IReviewService` (Layer 2).
- `IReviewService` (Layer 2) depends on
  `IReviewProviderRegistry` (Layer 1).
- `IReviewProvider` (Layer 1) may depend on
  `ICredentialVault` (Layer 0) when the
  provider uses user-supplied secrets.

**Architecture tests:**

- `Only_CompositionRoot_MayReference_ConcreteProviders`.
- `Pages_Resolve_Providers_Through_Registry`.
- `No_Secrets_In_Configuration`,
  `No_Secrets_In_Logs` — review secrets flow
  through `ICredentialVault`.

**Milestones that consume the chain:** M7,
M8.

### 3.4 The Quality-Gate Chain

The chain that turns "the user ran a gate
against the worktree" into "the gate's
pass/fail is rendered".

```
User runs gate
  → AppQualityGateRun
  → IQualityGateService.RunAsync(...)
    → IQualityGateProviderRegistry.Resolve(...)
      → IQualityGateProvider.RunAsync(...)
        → Provider-specific
          (native pass-through or No Mistakes)
```

**Dependencies:**

- `AppQualityGateRun` (Layer 3) depends on
  `IQualityGateService` (Layer 2).
- `IQualityGateService` (Layer 2) depends on
  `IQualityGateProviderRegistry` (Layer 1).
- `IQualityGateProvider` (Layer 1) may depend
  on `ICredentialVault` (Layer 0).

**Architecture tests:**

- `Only_CompositionRoot_MayReference_ConcreteProviders`.
- `Pages_Resolve_Providers_Through_Registry`.
- `No_Secrets_In_Configuration`,
  `No_Secrets_In_Logs`.

**Milestones that consume the chain:** M7,
M8.

### 3.5 The Autonomous-Loop / Orchestration Chain

The chain that turns "the user started a
bounded loop" into "the loop has run N
iterations with reviews and gates between
them".

```
User starts loop
  → AppAutonomousLoopForm
  → IAutonomousLoopService.StartAsync(...)
    → IAutonomousLoopProviderRegistry.Resolve(...)
      → IAutonomousLoopProvider.RunAsync(...)
        → IRunService.LaunchAsync(...)
          (recursively invokes the Launch chain)
        → IQualityGateService.RunAsync(...)
          (recursively invokes the Quality-Gate chain)
        → IReviewService.SubmitAsync(...)
          (recursively invokes the Review chain)
```

**Dependencies:**

- `AppAutonomousLoopForm` (Layer 3) depends
  on `IAutonomousLoopService` (Layer 2).
- `IAutonomousLoopService` (Layer 2)
  depends on
  `IAutonomousLoopProviderRegistry` (Layer 1).
- `IAutonomousLoopProvider` (Layer 1)
  composes `IRunService`,
  `IQualityGateService`, and
  `IReviewService` (Layer 2) through DI.
- The orchestration chain
  (`IOrchestrationProvider`) composes
  `IAutonomousLoopProvider` (Layer 1) and
  `IWorktreeProvider` (Layer 1) through DI.

**Architecture tests:**

- `Only_CompositionRoot_MayReference_ConcreteProviders`.
- `NativeProviders_Use_Contracts_Not_Implementations`
  — autonomous loop composes other providers
  through contracts, not direct imports.

**Milestones that consume the chain:** M8.

---

## 4. The Capability Schema (machine-readable)

The JSON schema for a capability is:

```json
{
  "id": "string (kebab-case-slug)",
  "title": "string",
  "status": "Proposed | Accepted | Done | Deferred | Rejected",
  "layer": 0 | 1 | 2 | 3,
  "category": "Infrastructure | ProviderModel | DomainOrchestration | UserSurface",
  "description": "string",
  "depends_on": ["capability-id-1", "capability-id-2"],
  "consumed_by": ["capability-id-3"],
  "delivered_by_milestone": "M4-A | M4-B | M4-C | M4-D | M5 | M6 | M7 | M8 | M0.5 | M1",
  "consumed_by_milestones": ["M5", "M6", "M7", "M8"],
  "adr": "ADR-NNN (or null if not graduated)",
  "architecture_tests": ["test-name-1", "test-name-2"]
}
```

The `depends_on` field is **mandatory** for
every Layer 1+ capability. A Layer 0 capability
has `depends_on: []`. The `consumed_by` field
is **mandatory** for every capability that
later milestones consume.

The JSON file
([`capabilities.json`](./capabilities.json))
is the canonical source; this document is the
human-readable projection.

---

## 5. Adding a New Capability

A new capability is added in three steps:

1. **Append** the new capability to
   [`.ai/backlog/capabilities.md`](./../backlog/capabilities.md)
   with status `Proposed`.
2. **Append** the new capability to
   [`capabilities.json`](./capabilities.json)
   with the same ID and status.
3. **Update** the dependency graph in
   `§ 2` of this document and the
   composition chains in `§ 3` if the new
   capability introduces a new chain or
   extends an existing one.

When the work is approved and sequenced:

4. The capability is added to the master
   delivery plan with a milestone.
5. The capability's status changes to
   `Accepted` in both files.
6. The architecture tests are added if the
   capability requires new ones.

When the work ships:

7. The capability's status changes to
   `Done` in both files.
8. The implementation report cites the
   capability ID.

---

## 6. Why a Capability Model

The capability model exists for three reasons:

- **Discoverability.** A new contributor can
  read the graph and understand what the
  platform is, what it does, and what depends
  on what. The graph is the platform's
  readable shape.
- **Enforceability.** The architecture tests
  pin the graph. A change that bypasses a
  dependency fails the build.
- **Roadmapability.** The roadmap sequences
  the capabilities; the matrix in
  [`ROADMAP.md`](./../../ROADMAP.md) § 4 is
  the capability-by-capability operational
  view. The capability model and the
  roadmap are two views of the same
  ordering.

A platform that does not have a capability
model is a platform whose dependencies are
implied by the code. The code is the model
only by accident. The capability model makes
the dependencies explicit and reviewable.

---

## 7. Linked Artefacts

- [`.ai/backlog/capabilities.md`](./../backlog/capabilities.md) —
  the catalogue of capabilities (the
  backlog's view).
- [`capabilities.json`](./capabilities.json) —
  the machine-readable schema (the
  canonical source).
- [`ROADMAP.md`](./../../ROADMAP.md) § 4 —
  the matrix that operationalises the
  capability model.
- [`ARCHITECTURE.md`](./../../ARCHITECTURE.md) —
  the architecture that the model
  describes.
- [`DECISIONS.md`](./../../DECISIONS.md) —
  the ADRs that graduated from the
  decision log to the architectural
  record.

## 8. Last Updated

- **2026-07-10** — created in the M0.5
  architecture refinement. The graph is
  derived from the existing ADRs and
  roadmap; no architectural rule has
  changed.
