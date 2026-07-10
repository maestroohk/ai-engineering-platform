# .ai/workflows/provider-onboarding.md

> The end-to-end process of adding a new provider to the AI
> Engineering Platform. The workflow is structured so the
> contract, the test, and the documentation all exist
> before the implementation lands, and so the user can
> enable the provider through configuration alone.

---

## 1. Purpose

This workflow sequences the work `.ai/prompts/provider.md`
describes. It exists so that every provider lands in the
same shape, with the same artefacts, and with the same
quality of testing. A provider that lands outside this
workflow is a provider that bypassed the contract.

## 2. Stages

### Stage 1 — Capability Analysis

- **Inputs:** the external tool the provider integrates
  with.
- **Outputs:** a written mapping from the tool's
  capabilities to the platform's family contract methods.
  Per ADR-012, the family is named after the capability
  (`IAgentRuntimeProvider`, `IGitProvider`,
  `ITerminalProvider`, `IWorktreeProvider`,
  `IQualityGateProvider`, `IReviewProvider`,
  `IAutonomousLoopProvider`, `IOrchestrationProvider`).
  The tool-to-family mapping is recorded:
  - Treehouse → `IWorktreeProvider`.
  - No Mistakes → `IQualityGateProvider`.
  - Lavish Axi → `IReviewProvider`.
  - GNHF → `IAutonomousLoopProvider`.
  - Firstmate → `IOrchestrationProvider`.
- **Workflow step:** list every method the tool exposes.
  List every method the platform needs. The intersection
  is the contract scope. The differences are noted as
  either deferred (out of scope for v1) or as gaps in the
  contract (filed as an ADR).

### Stage 2 — Contract

- **Inputs:** the capability analysis.
- **Outputs:** the contract interface (or the change to an
  existing family contract).
- **Workflow step:** write the contract in the right
  family folder. The contract is reviewed and merged
  before any implementation. If the contract requires
  a new family, file an ADR first.

### Stage 3 — Fake Implementation

- **Inputs:** the contract.
- **Outputs:** a deterministic fake or in-process server
  that the contract test will exercise.
- **Workflow step:** the fake is fast, deterministic, and
  supports the documented failure paths. The fake lives
  in
  `tests/AiEng.Platform.ProviderContractTests/<Family>/<Provider>/`,
  alongside the contract test, **not** in
  `tests/AiEng.Platform.Tests/Providers/Fakes/`. The
  previous single-fakes folder is replaced by
  per-provider test folders.

### Stage 4 — Real Adapter

- **Inputs:** the contract and the fake.
- **Outputs:** the real provider implementation, added as
  a new project `src/AiEng.Platform.Providers.<X>/`
  according to ADR-011. The contract interface itself
  lives in
  `src/AiEng.Platform.Providers.Abstractions/<Family>/`.
- **Workflow step:** the implementation owns the
  process boundary. It depends on the contract, the
  options class, and a private client wrapper. It does
  not depend on the UI. Process-boundary providers
  inject `IProcessRunner` from
  `AiEng.Platform.Infrastructure/Process/`, not direct
  `Process.Start` calls.

### Stage 5 — Health Check

- **Inputs:** the real adapter.
- **Outputs:** `HealthAsync` returning a typed
  `ProviderHealth` for the happy path and the
  documented failure paths.
- **Workflow step:** the health check is fast (under 1s
  by default) and respects a short timeout. It is
  exercised by the contract test.

### Stage 6 — Conformance Tests

- **Inputs:** the contract, the fake, and the real
  adapter.
- **Outputs:** the provider's contract test class
  inheriting the family contract test base. The test
  passes for the real adapter against the fake.
- **Workflow step:** the test covers the happy path,
  every documented error path, and cancellation. The
  test runs in the default CI pipeline.

### Stage 7 — Documentation

- **Inputs:** the contract, the adapter, the options.
- **Outputs:** the provider entry in
  `docs/provider-guidelines.md`. The entry includes
  contract, transport, auth, status, fallback, and
  notes.
- **Workflow step:** also update
  `docs/architecture-principles.md` if a new pattern
  emerged (rare), and `docs/folder-structure.md` if the
  layout changed (rare).

### Stage 8 — Controlled Enablement

- **Inputs:** the documented provider.
- **Outputs:** a registration extension that the user
  can call from `Program.cs` only if their
  configuration includes the provider's section.
- **Workflow step:** the provider is **not** enabled by
  default. The user opts in by adding the configuration
  section. A provider that is not configured is
  invisible to the UI.

### Stage 9 — Progressive Self-Dogfooding (per ADR-013)

- **Inputs:** the merged provider.
- **Outputs:** an updated row in the progressive
  self-dogfooding matrix in `ROADMAP.md` § 4.
- **Workflow step:** if the new provider delivers a
  reusable capability (a new family, a new process
  boundary, a new way of consuming an existing
  capability), the matrix gains a row listing the later
  milestones that must use it, the direct bypass that is
  prohibited, the validation that confirms consumption,
  and the architecture test that fails the build on
  bypass. A provider that lands without a matrix row
  (where one is required) is rejected.

## 3. Definition of Done

A provider is done when:

- The contract is merged.
- The fake is merged.
- The real adapter is merged.
- The contract test passes.
- The documentation entry is merged.
- The registration extension is called from `Program.cs`
  behind a configuration check.
- An integration test (tagged appropriately) passes
  against a real instance, or is explicitly deferred.

## 4. Anti-Patterns

- Implementing the provider before the contract.
- Skipping the fake.
- Skipping the contract test.
- Enabling the provider by default.
- Reading secrets from configuration files.
- Importing a UI namespace from the provider.
- Returning exceptions instead of `ProviderResult`.
- Bundling the provider with a feature that uses it.
