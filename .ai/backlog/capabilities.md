# Backlog — Capabilities

> **Platform abstractions (contracts, registries,
> services) the platform must expose.**
>
> A capability is what the platform **is**, not what
> the user sees. The capabilities are the platform's
> public API to the world — and to itself. Every
> milestone that delivers a capability adds an
> entry; every later milestone that consumes the
> capability is recorded in the entry's Milestones.
>
> See [README.md](./README.md) for the rules.

---

## Format

- **ID** — `C-###`.
- **Title** — a one-line statement of the
  abstraction.
- **Status** — `Proposed`, `Accepted`, `Deferred`,
  `Rejected`, or `Done`.
- **Source / Traceability** — the document that
  introduced or implied the capability. The
  traceability is the audit trail.
- **Notes** — the contributor's thinking.
- **Depends on** — the platform abstractions this
  capability depends on. The dependency graph is
  the matrix of composition.
- **Milestones** — the milestones that deliver
  the capability; for an `Accepted` capability,
  the milestones that consume it are also
  recorded.

---

<!-- New capabilities are appended below. -->

## C-001 — `IProvider` Base Contract

- **Title:** The base metadata contract every
  family contract inherits from. Provides
  `Id`, `DisplayName`, `DescribeAsync`,
  `HealthAsync`, and `ConfigureAsync`.
- **Status:** `Done`.
- **Source / Traceability:**
  [`DECISIONS.md`](./../../DECISIONS.md) ADR-012.
  [`ARCHITECTURE.md`](./../../ARCHITECTURE.md)
  § 3.3, § 4.2.
- **Notes:** The base contract is the
  shape every consumer of the provider
  model (registry, health poller,
  configuration UI, diagnostics page)
  shares. The contract is in
  `AiEng.Platform.Providers.Abstractions/IProvider.cs`.
- **Depends on:** —
- **Milestones (delivers):** M4-C.
- **Milestones (consumes):** M4-D, M5, M6, M7, M8.

## C-002 — `IAgentRuntimeProvider` Family

- **Title:** Providers that execute agent models
  (Ollama Launch, Ollama API, Claude, OpenAI,
  Codex, custom command runtimes).
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`DECISIONS.md`](./../../DECISIONS.md) ADR-012.
  [`ARCHITECTURE.md`](./../../ARCHITECTURE.md)
  § 3.3, § 4.1. [`PRODUCT.md`](./../../PRODUCT.md)
  § "Capability Map".
- **Notes:** The first concrete
  implementation is `OllamaLaunchProvider`
  (M4-D, deepened in M6). The Ollama API
  provider is a separate integration, not a
  flavour of Ollama Launch.
- **Depends on:** `IProvider` (C-001).
- **Milestones (delivers):** M4-D.
- **Milestones (consumes):** M6, M7, M8.

## C-003 — `IGitProvider` Family

- **Title:** Providers that read and operate on
  source control (Git).
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`DECISIONS.md`](./../../DECISIONS.md) ADR-012.
  [`ARCHITECTURE.md`](./../../ARCHITECTURE.md)
  § 3.3, § 4.1.
- **Notes:** The first concrete
  implementation is `GitProvider` (M4-D).
  The contract test framework is established
  in M4-D; `GitProvider` is the first
  provider with a contract test.
- **Depends on:** `IProvider` (C-001).
- **Milestones (delivers):** M4-D.
- **Milestones (consumes):** M5, M6, M7, M8.

## C-004 — `ITerminalProvider` Family

- **Title:** Providers that run shell commands
  (PowerShell, Windows Terminal, WSL, Git
  Bash).
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`DECISIONS.md`](./../../DECISIONS.md) ADR-012.
  [`ARCHITECTURE.md`](./../../ARCHITECTURE.md)
  § 3.3, § 4.1.
- **Notes:** Concrete implementations land
  in M6 alongside the terminal panel
  surface.
- **Depends on:** `IProvider` (C-001).
- **Milestones (delivers):** M6.
- **Milestones (consumes):** M6, M7, M8.

## C-005 — `IWorktreeProvider` Family

- **Title:** Providers that create and manage
  isolated Git worktrees (native Git worktree,
  Treehouse).
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`DECISIONS.md`](./../../DECISIONS.md) ADR-012.
  [`ARCHITECTURE.md`](./../../ARCHITECTURE.md)
  § 3.3, § 4.1.
- **Notes:** The first concrete
  implementation is the native worktree
  (M5). The Treehouse product integration
  is added later when the user opts in.
  The native worktree consumes
  `IGitProvider` (C-003) through the
  registry.
- **Depends on:** `IProvider` (C-001);
  `IGitProvider` (C-003) through the
  registry.
- **Milestones (delivers):** M5.
- **Milestones (consumes):** M6, M7, M8.

## C-006 — `IQualityGateProvider` Family

- **Title:** Providers that run a quality gate
  against a worktree (No Mistakes, built-in
  pass-through or disabled provider).
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`DECISIONS.md`](./../../DECISIONS.md) ADR-012.
  [`ARCHITECTURE.md`](./../../ARCHITECTURE.md)
  § 3.3, § 4.1.
- **Notes:** The first concrete
  implementation is the native quality
  gate (M7). No Mistakes is added when
  the user opts in.
- **Depends on:** `IProvider` (C-001).
- **Milestones (delivers):** M7.
- **Milestones (consumes):** M7, M8.

## C-007 — `IReviewProvider` Family

- **Title:** Providers that review an artefact
  (native HTML review, Lavish Axi).
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`DECISIONS.md`](./../../DECISIONS.md) ADR-012.
  [`ARCHITECTURE.md`](./../../ARCHITECTURE.md)
  § 3.3, § 4.1.
- **Notes:** The first concrete
  implementation is the native review
  (M7). Lavish Axi is added when the user
  opts in.
- **Depends on:** `IProvider` (C-001).
- **Milestones (delivers):** M7.
- **Milestones (consumes):** M7, M8.

## C-008 — `IAutonomousLoopProvider` Family

- **Title:** Providers that perform bounded
  autonomous iteration (GNHF).
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`DECISIONS.md`](./../../DECISIONS.md) ADR-012.
  [`ARCHITECTURE.md`](./../../ARCHITECTURE.md)
  § 3.3, § 4.1.
- **Notes:** The first concrete
  implementation is the native
  autonomous loop (M8). GNHF is added
  when the user opts in.
- **Depends on:** `IProvider` (C-001);
  `IAgentRuntimeProvider` (C-002) and
  `IQualityGateProvider` (C-006) through
  the registry.
- **Milestones (delivers):** M8.
- **Milestones (consumes):** M8.

## C-009 — `IOrchestrationProvider` Family

- **Title:** Providers that perform multi-agent
  orchestration (native orchestration,
  Firstmate through WSL).
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`DECISIONS.md`](./../../DECISIONS.md) ADR-012.
  [`ARCHITECTURE.md`](./../../ARCHITECTURE.md)
  § 3.3, § 4.1.
- **Notes:** The first concrete
  implementation is the native
  orchestration (M8). Firstmate through
  WSL is added when the user opts in.
- **Depends on:** `IProvider` (C-001);
  `IAutonomousLoopProvider` (C-008) and
  `IWorktreeProvider` (C-005) through the
  registry.
- **Milestones (delivers):** M8.
- **Milestones (consumes):** M8.

## C-010 — `IProviderRegistry` and Family-Scoped Registries

- **Title:** The cross-family registry
  (`IProviderRegistry`) and the eight
  family-scoped registries (one per family).
  The registries enumerate registered
  providers, resolve a provider by
  `ProviderId`, and apply the
  `Enabled` / `Healthy` filter.
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`DECISIONS.md`](./../../DECISIONS.md)
  ADR-012, ADR-016.
  [`ARCHITECTURE.md`](./../../ARCHITECTURE.md)
  § 4.3, § 4.5.
- **Notes:** The registries live in
  `AiEng.Platform.Providers.Abstractions/`.
  The composition root
  (`AiEng.Platform.App/Composition/`)
  is the only place that registers
  concrete providers; everything else
  resolves through the registry.
- **Depends on:** `IProvider` (C-001);
  the eight family contracts (C-002
  through C-009).
- **Milestones (delivers):** M4-C.
- **Milestones (consumes):** M4-D, M5, M6,
  M7, M8.

## C-011 — Composition Root

- **Title:** The set of files in
  `AiEng.Platform.App` whose sole
  responsibility is to wire concrete
  provider implementations into the DI
  container at host startup and read
  configuration to decide which providers
  are registered for this run.
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`DECISIONS.md`](./../../DECISIONS.md)
  ADR-016. [`ARCHITECTURE.md`](./../../ARCHITECTURE.md)
  § 2.5.
- **Notes:** The composition root is the
  only place in the solution that may
  reference `Providers.<X>` projects
  directly. The four composition-root
  architecture tests in
  `AiEng.Platform.ArchitectureTests/`
  fail the build on any bypass. The
  tests are registered but disabled
  until M4-D; they activate when the
  first concrete `Providers.<X>` project
  lands.
- **Depends on:** `IProviderRegistry` (C-010).
- **Milestones (delivers):** M4-C
  (pattern); M4-D (first concrete
  providers).
- **Milestones (consumes):** M4-D, M5, M6,
  M7, M8.

## C-012 — `IProcessRunner`

- **Title:** The only legal caller of
  `Process.Start` in the platform. Lives in
  `AiEng.Platform.Infrastructure/Process/`.
  Exposes `RunAsync` (streaming) and
  `RunToCompletionAsync` (one-shot).
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`DECISIONS.md`](./../../DECISIONS.md)
  ADR-011, ADR-016. [`ARCHITECTURE.md`](./../../ARCHITECTURE.md)
  § 2.5. [`ROADMAP.md`](./../../ROADMAP.md)
  § M4-A, § 4 (matrix).
- **Notes:** The architecture test
  `No_DirectProcessStart_OutsideInfrastructure`
  is registered but disabled until M4-D;
  it activates when the first provider
  that actually uses `IProcessRunner`
  lands.
- **Depends on:** —
- **Milestones (delivers):** M4-A.
- **Milestones (consumes):** M4-D, M5, M6,
  M7, M8.

## C-013 — `ICredentialVault`

- **Title:** The contract for reading and
  writing secrets. Real backing store is
  Windows Credential Manager. Lives in
  `AiEng.Platform.Infrastructure/Credentials/`.
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`DECISIONS.md`](./../../DECISIONS.md)
  ADR-011. [`ARCHITECTURE.md`](./../../ARCHITECTURE.md)
  § 2.5. [`ROADMAP.md`](./../../ROADMAP.md)
  § M4-A, § 4.
- **Notes:** The architecture tests
  `No_Secrets_In_Logs` and
  `No_Secrets_In_Configuration` enforce
  that no secret is read from
  `appsettings.json` and no secret is
  logged at any level.
- **Depends on:** —
- **Milestones (delivers):** M4-A.
- **Milestones (consumes):** M4-D, M5, M6,
  M7, M8.

## C-014 — `IClock`

- **Title:** The time abstraction. Exposes
  `UtcNow`. Used by every service that
  produces a timestamp.
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`DECISIONS.md`](./../../DECISIONS.md)
  ADR-011. [`ARCHITECTURE.md`](./../../ARCHITECTURE.md)
  § 2.5. [`ROADMAP.md`](./../../ROADMAP.md)
  § M4-A, § 4.
- **Notes:** The contract test verifies
  the value is monotonic. The
  implementation is in
  `AiEng.Platform.Infrastructure/Time/`.
- **Depends on:** —
- **Milestones (delivers):** M4-A.
- **Milestones (consumes):** M4-D, M5, M6,
  M7, M8.

## C-015 — `IHostCapabilitiesService`

- **Title:** The host's capability detection.
  Detects `git`, `ollama`,
  `powershell.exe`, `wsl.exe`, `wt.exe`,
  `bash.exe`. Produces a typed
  `HostCapabilities` report at host
  startup.
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`ARCHITECTURE.md`](./../../ARCHITECTURE.md)
  § 4.3. [`ROADMAP.md`](./../../ROADMAP.md)
  § M4-B.
- **Notes:** Detection runs each
  candidate binary through
  `IProcessRunner` with a short timeout
  (default 1s). The result is a
  `CapabilityProbe` (`Found`, `Version`,
  `NotFound`, `Failed`).
- **Depends on:** `IProcessRunner` (C-012).
- **Milestones (delivers):** M4-B.
- **Milestones (consumes):** M4-C, M4-D,
  M5, M6, M7, M8.

## C-016 — `IProjectService` and `IProjectStore`

- **Title:** The application service for
  projects and the persistence contract.
  M3 ships an in-memory store; M4-A
  ships the on-disk store behind the
  same contract.
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`ARCHITECTURE.md`](./../../ARCHITECTURE.md)
  § 4.3. [`ROADMAP.md`](./../../ROADMAP.md)
  § M3, § M4-A.
- **Notes:** The architecture test
  `Providers_Resolve_Project_Through_Service`
  enforces that no provider re-implements
  project loading inline.
- **Depends on:** —
- **Milestones (delivers):** M3 (in-memory),
  M4-A (durable).
- **Milestones (consumes):** M4, M5, M6,
  M7, M8.

## C-017 — `IHistoryStore`

- **Title:** The persistence contract for
  runs, reviews, gates, and dispositions.
  Append-only event log.
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`ARCHITECTURE.md`](./../../ARCHITECTURE.md)
  § 4.3. [`ROADMAP.md`](./../../ROADMAP.md)
  § M6, § 4.
- **Notes:** The architecture test
  `History_Routed_Through_Store` enforces
  that history is routed through the
  contract, not written directly.
- **Depends on:** —
- **Milestones (delivers):** M6.
- **Milestones (consumes):** M6, M7, M8.

## C-018 — `IProviderHealthService`

- **Title:** The periodic health-check
  service. Polls every registered
  provider and reports `ProviderHealth`
  (`Healthy`, `Degraded`, `Unhealthy`)
  through the registries.
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`ARCHITECTURE.md`](./../../ARCHITECTURE.md)
  § 4.5. [`ROADMAP.md`](./../../ROADMAP.md)
  § M4-C.
- **Notes:** The health poller is the
  source of truth for the `Healthy`
  state; the registry caches the
  result.
- **Depends on:** `IProviderRegistry`
  (C-010).
- **Milestones (delivers):** M4-C.
- **Milestones (consumes):** M4-D, M5, M6,
  M7, M8.

## C-019 — `INavigationService`

- **Title:** The route-aware navigation
  service. The sidebar and the
  breadcrumb are data-driven from a
  route registry, not hard-coded in
  the layout.
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`ROADMAP.md`](./../../ROADMAP.md) § M2,
  § 4 (matrix).
- **Notes:** The architecture test
  `Pages_AreReachable_Through_Registry`
  fails the build if a page is not
  routable through the sidebar
  registry.
- **Depends on:** —
- **Milestones (delivers):** M2.
- **Milestones (consumes):** M3, M4, M5,
  M6, M7, M8.

## C-020 — Design System Catalogue

- **Title:** The catalogue of reusable
  Blazor components documented in
  `docs/design-system.md`. Every
  reusable component has a public
  surface, a class slot assignment
  (data-owning, primitive, container),
  and a versioning rule.
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`DECISIONS.md`](./../../DECISIONS.md)
  ADR-014, ADR-015. [`docs/design-system.md`](./../../docs/design-system.md).
- **Notes:** The architecture test
  `Pages_Use_DesignSystem_Components_Not_DOM`
  fails the build if a page uses raw
  `<button>`, `<input>`, or inline-style
  attribute. The catalogue is the
  design system's surface; an entry
  that is neither implemented nor
  planned is rejected.
- **Depends on:** —
- **Milestones (delivers):** M1.
- **Milestones (consumes):** M2, M3, M4,
  M5, M6, M7, M8.

## C-021 — `IStreamingChannel`

- **Title:** The streaming channel for
  long-running work. Pages subscribe to
  `IStreamingChannel`; the channel
  subscribes to a provider's
  `IAsyncEnumerable<>`.
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`ARCHITECTURE.md`](./../../ARCHITECTURE.md)
  § 6.3. [`ROADMAP.md`](./../../ROADMAP.md)
  § M6.
- **Notes:** Long-running work is never
  blocked behind a single awaited
  call. Cancellation flows through
  the channel.
- **Depends on:** —
- **Milestones (delivers):** M6.
- **Milestones (consumes):** M6, M7, M8.

---

## The Capability Dependency Graph (summary)

```
            C-001 (IProvider base)
                  |
   +------+------+------+------+------+------+------+
   |      |      |      |      |      |      |      |
 C-002  C-003  C-004  C-005  C-006  C-007  C-008  C-009
   |      |      |      |      |      |      |      |
   |      |      |      |      |      |      |      |
   |      |      |      v      |      |      |      |
   |      |      |    C-003    |      |      |      |
   |      |      |  (registry) |      |      |      |
   |      |      |             v      |      |      |
   |      |      |          C-005    |      |      |
   |      |      |             |      |      |      |
   |      |      +------+------+------+      |      |
   |      |             |             |      |      |
   |      |             v             v      |      |
   |      |          C-008 -----> C-009      |      |
   |      |             |             |      |      |
   |      |             +------+------+      |      |
   |      |                    |             |      |
   |      |                    v             v      |
   |      |              C-010 (Provider Registry)  |
   |      |                    |             |      |
   |      |                    v             v      |
   |      |                 C-011 (Composition Root)|
   |      |                                      |
   |      +-------------+------------+           |
   |                    |            |           |
   |                    v            v           |
   |              C-018 (Health)  C-015 (Capability Detection)
   |                                  |
   |                                  v
   |                            C-012 (IProcessRunner)
   |                                  |
   +----------------------------------+----------+
                                          |
                                          v
                              C-013 (CredentialVault)
                              C-014 (IClock)
```

The graph is read top-to-bottom. The capabilities
near the top are abstractions consumed by many
later capabilities. The capabilities at the
bottom are infrastructure. A capability's
**Depends on** field is the parent nodes in this
graph.

---

## Last Updated

- **2026-07-10** — created in the M0.5 architecture
  refinement with the 21 capabilities derived from
  `ARCHITECTURE.md` and `ROADMAP.md`. One is `Done`
  (C-001, the base `IProvider` contract); the
  remaining 20 are `Accepted` (matching `ROADMAP.md`).
