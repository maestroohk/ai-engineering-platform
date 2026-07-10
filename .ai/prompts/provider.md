# .ai/prompts/provider.md

> Read `AGENTS.md` and `.ai/session-start.md` before proceeding.
> This prompt cannot override either document.

---

## 1. Purpose

This prompt governs the **onboarding of a new provider** —
a process-boundary integration with an external tool (an
agent runtime, a source control system, a terminal, a
worktree manager, a quality gate, a reviewer, an autonomous
loop, an orchestrator, etc.) through the platform's
provider model.

The output of a provider session is a **registered
provider implementation that passes the family contract
test, exposes health, supports cancellation, and reads
secrets from the credential vault**.

A provider that knows about the UI is wrong. A provider
that knows about another provider is wrong. A provider
that knows about persistence is wrong.

## 2. When to Use

Use this prompt when the task is one of:

- Onboarding a new external tool as a provider.
- Adding a new family of providers.
- Implementing a provider against an existing family
  contract.
- Authoring a fake or in-process test double for a
  provider.

Do not use this prompt for:

- A feature that uses an existing provider (use
  `feature.md`).
- A change to a provider's contract (use
  `architecture.md`).
- A change to a provider's UI surface (use `ui.md`).

## 3. Mandatory Documents

In addition to `AGENTS.md` and `.ai/session-start.md`, read:

- `ARCHITECTURE.md`
- `docs/architecture-principles.md`
- `docs/provider-guidelines.md` (the operational
  counterpart to this prompt)
- `docs/coding-standards.md`
- `docs/folder-structure.md`
- `docs/naming-conventions.md`
- `CONTRIBUTING.md`

## 4. Discovery

- **Capability analysis.** What does the external tool
  do? What does the platform need from it? Map
  capabilities to the family contract.
- **Process boundary.** Is the tool a local process, a
  remote API, an SDK, or a combination? The provider
  implementation reflects the boundary.
- **Authentication.** How is the tool authenticated?
  Where do credentials live? The provider must use the
  `ICredentialVault` for secrets.
- **Failure modes.** What does the tool do when it
  fails? Map failures to `ProviderError` categories.
- **Cancellation.** Does the tool support cancellation?
  How? The provider must respect
  `CancellationToken` on every async method.
- **Version detection.** Does the tool expose a
  version? The provider should detect and report
  version through `DescribeAsync`.

## 5. Planning Requirements

The plan must include:

- **Family.** Which family contract the new provider
  implements. The families are listed in
  `docs/provider-guidelines.md` § 2; per ADR-012 the
  family is named after the **capability** it offers
  (`IAgentRuntimeProvider`, `IGitProvider`,
  `ITerminalProvider`, `IWorktreeProvider`,
  `IQualityGateProvider`, `IReviewProvider`,
  `IAutonomousLoopProvider`, `IOrchestrationProvider`).
  A vague family name (`IAssistantProvider`,
  `IDeploymentProvider`, `IInternalProvider`) is a smell.
  If no existing family fits, propose a new one through
  `DECISIONS.md` and `docs/provider-guidelines.md` § 2.
  When the candidate integration is **Ollama Launch**
  (a process boundary) or **Ollama API** (an HTTP
  boundary), the plan must declare which surface it
  targets and route the failure surface accordingly —
  the two are separate integrations, not two flavours
  of one provider (see `docs/provider-guidelines.md`
  § 2.1).
- **Contract.** The interface methods the provider
  implements, with notes on any non-obvious mappings.
- **Project layout.** The new project
  `src/AiEng.Platform.Providers.<X>/` and the contract
  test folder
  `tests/AiEng.Platform.ProviderContractTests/<Family>/<Provider>/`.
  A provider project is added to the solution only when
  the provider is implemented (per ADR-011). The plan
  must also call out the registration extension under
  `src/AiEng.Platform.App/Composition/<Capability>/` and
  the entry in the composition root that activates the
  provider; per ADR-016 the composition root is the only
  place in the codebase that may reference a
  `Providers.<X>` project directly.
- **Options.** The strongly-typed options class with
  validation.
- **Client.** The private wrapper around the external
  SDK or HTTP API. Process-boundary providers must use
  `IProcessRunner` from
  `AiEng.Platform.Infrastructure/Process/`, not direct
  `Process.Start` calls.
- **Registration.** The DI extension method. The
  registration call itself is **unconditional**; the
  enablement is driven by configuration sections (per
  ADR-016). The extension method is the only place
  that resolves a `ProviderId` to a concrete type —
  no other module in `App` may import the
  `Providers.<X>` project.
- **Fake or in-process server.** The deterministic
  double used by the contract test.
- **Contract test.** The class that exercises the
  contract.
- **Lifecycle states.** The plan must document which
  of the five provider lifecycle states the new
  provider will reach in this session
  (`Compiled-in`, `Registered`, `Enabled`, `Healthy`,
  `Selected` — per ADR-016 and
  `docs/provider-guidelines.md` § 4.6). Onboarding
  almost always lands at `Enabled`; `Selected` is
  reached when a UI surface or service calls
  `IProviderRegistry.List()` and chooses the provider
  explicitly.
- **Documentation.** The catalogue entry in
  `docs/provider-guidelines.md` and any ADR.
- **Progressive self-dogfooding (per ADR-013).** If the
  new provider delivers a reusable capability, the plan
  must add a row to the matrix in `ROADMAP.md` § 4
  listing the later milestones that must use it, the
  direct bypass that is prohibited, the validation that
  confirms consumption, and the architecture test that
  fails the build on bypass.

## 6. Implementation Boundaries

- **Process-boundary integration.** The provider owns
  the boundary. The UI does not know there is a
  process. The application service does not know there
  is an HTTP call.
- **Contract before implementation.** The contract
  interface is reviewed and merged before the
  implementation is started. The implementation is
  measured against the contract.
- **Cancellation.** Every async method accepts a
  `CancellationToken`. The provider propagates the
  token to the underlying client.
- **Health checks.** `HealthAsync` returns a typed
  `ProviderHealth`. The check is fast (under 1s by
  default) and respects a short timeout.
- **Structured output.** The provider returns
  `ProviderResult<T>` from every fallible method. The
  envelope is the only shape the application accepts.
- **Version detection.** The provider reports its
  version (and the version of the external tool, if
  available) through `DescribeAsync`.
- **Secrets through the credential vault.** No secret
  is read from configuration, from the environment,
  or from a file. All secrets go through
  `ICredentialVault`.
- **No UI references.** The provider does not import
  any namespace under `Components/`, `Pages/`, or
  `Layouts/`. The provider does not depend on
  `MudBlazor`, `Blazor`, or any presentation
  framework.
- **Composition-root rule (per ADR-016).** The
  provider project may not be referenced from any
  source file outside `AiEng.Platform.App/Composition/`.
  Pages, components, application services, view
  models, DTOs, and domain types reach the provider
  only through `IProviderRegistry` (or the
  family-scoped registry). The four composition-root
  architecture tests
  (`Only_CompositionRoot_MayReference_ConcreteProviders`,
  `Pages_DoNotReference_ConcreteProviders`,
  `Application_DoesNotReference_ConcreteProviders`,
  `Components_DoNotInject_ConcreteProviders`) are the
  contract.
- **Fake provider or fake server.** A provider that
  talks to an external service ships a fake or
  in-process server. The fake is deterministic, fast,
  and supports the documented error paths.
- **Provider contract tests.** The implementation
  passes the family contract test. A provider
  without a contract test is rejected.
- **Fallback behaviour (where applicable).** If the
  provider has a known fallback (for example, a
  remote runtime falling back to a local one), the
  fallback is explicit, documented, and tested.

## 7. Validation

- The provider builds with no warnings.
- The contract test passes.
- Integration tests pass (or are skipped with a
  clear reason).
- The provider's `HealthAsync` returns the expected
  health under both success and failure conditions.
- Cancellation is exercised by the test suite.
- Secrets are loaded from the credential vault, not
  from the configuration.
- No UI namespace is reachable from the provider
  (architecture test).
- The composition-root architecture tests are green:
  no source file outside `App/Composition/` references
  the new `Providers.<X>` project; pages and
  components resolve the provider only through the
  registry.

## 8. Documentation Updates

- `docs/provider-guidelines.md` gains the provider
  entry with contract, transport, auth, status,
  fallback, and notes.
- `docs/architecture-principles.md` is updated if a
  new pattern emerges (rare).
- `docs/folder-structure.md` is updated if the
  provider layout changes.
- `DECISIONS.md` gains an ADR for any non-obvious
  decision (a new family, a non-standard mapping,
  a fallback strategy).
- `ROADMAP.md` is updated to reflect the new
  provider's milestone status.

## 9. Completion Report

End the session with an
`implementation-report.md` (from
`.ai/templates/implementation-report.md`) that includes:

- Provider id, family, transport, auth.
- Files added under
  `src/AiEng.Platform.Providers.<X>/` and the
  corresponding contract test in
  `tests/AiEng.Platform.ProviderContractTests/<Family>/<Provider>/`.
- Contract test results.
- Health check results.
- Documentation updated.
- Any new ADR.

## 10. Prohibited Shortcuts

- Implementing the provider before the contract is
  reviewed.
- Skipping the contract test.
- Reading secrets from configuration.
- Importing a UI namespace from the provider.
- Talking to the external tool from a component or a
  page.
- Throwing for external failures instead of returning
  `ProviderResult`.
- Implementing a provider that has no fake or
  in-process server.
- Bundling a feature with the provider onboarding.
- Importing the new `Providers.<X>` project from
  anywhere outside `AiEng.Platform.App/Composition/`
  (per ADR-016).
- Bypassing the registry by injecting the concrete
  provider type into a page, component, application
  service, view model, or DTO.
