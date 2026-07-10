# .ai/prompts/testing.md

> Read `AGENTS.md` and `.ai/session-start.md` before proceeding.
> This prompt cannot override either document.

---

## 1. Purpose

This prompt governs the design and implementation of the
platform's test suites. The output of a testing session is
a **comprehensive, deterministic, layered test strategy**
that catches regressions, exercises contracts, and
documents intent.

Coverage is treated as a **diagnostic**, not as the only
quality measure. A test that exercises the right path with
the wrong data is worse than no test.

## 2. When to Use

Use this prompt when the task is one of:

- Adding a new test project or test category.
- Writing tests for a feature, refactor, or bug fix.
- Improving an existing test suite.
- Adding a new kind of test (for example, an architecture
  test).
- Investigating a flaky or unreliable test.

Do not use this prompt for tests that are part of a
larger feature implementation; those tests are written
under `feature.md`, `bugfix.md`, `refactor.md`, `ui.md`,
or `provider.md`, and use this prompt as a reference.

## 3. Mandatory Documents

In addition to `AGENTS.md` and `.ai/session-start.md`, read:

- `docs/coding-standards.md` (test pyramid, naming,
  structure)
- `docs/architecture-principles.md` (architecture tests)
- `docs/provider-guidelines.md` (contract tests)
- `docs/component-guidelines.md` (bUnit tests)
- `docs/design-system.md` (components under test)
- `CONTRIBUTING.md` (the definition of done)

## 4. Test Pyramid

| Layer            | Test type                | Tooling                       |
| ---------------- | ------------------------ | ----------------------------- |
| Presentation     | Component tests          | xUnit + bUnit                 |
| Application      | Unit tests               | xUnit + FluentAssertions      |
| Contracts        | Contract tests           | xUnit (abstract test class)   |
| Implementations  | Integration tests        | xUnit + `WebApplicationFactory` |
| Infrastructure   | Integration tests        | xUnit                         |
| Architecture     | Architecture tests       | xUnit + NetArchTest           |

The pyramid is not negotiable. Higher layers depend on
lower layers; tests follow the same direction.

## 5. Discovery

- **Inspect the test project.** Confirm what already
  exists. A new test that duplicates an existing test is
  rejected.
- **Inspect the production code.** The test exercises
  the real shape, not an imagined one.
- **Identify the failure modes.** Every testable
  function has at least one happy-path test and at
  least one failure-path test. The failure paths come
  from the contracts, not from imagination.
- **Identify the boundaries.** Components, services,
  providers, and infrastructure each have a different
  test type. The test matches the layer.

## 6. Planning Requirements

The plan must include, per test:

- **Layer** (unit, bUnit, contract, integration,
  architecture).
- **Target** (which type or method under test).
- **Scenario** (the input and the expected output).
- **Fixture** (what setup is required; the fixture is
  deterministic).
- **Naming** (`<ClassName>Tests` for the class,
  `<MethodName>_<Scenario>_<ExpectedResult>` for the
  method).

## 7. Test Categories

### 7.1 Unit Tests

- Exercise a single class or method in isolation.
- Use FluentAssertions for readable assertions.
- No I/O, no time-based behaviour, no shared mutable
  state.

### 7.2 bUnit Component Tests

- Exercise a component through its public surface:
  parameters, event callbacks, and rendered output.
- Cover the primary render, every variant, every size,
  and every state slot.
- Cover keyboard interaction (focus, `Enter`, `Escape`,
  arrow keys) where relevant.
- Use `TestContext` and `RenderComponent` from bUnit.

### 7.3 Provider Contract Tests

- Every provider implementation passes the contract
  test suite for its family.
- The contract test is an abstract xUnit class in
  `tests/AiEng.Platform.Tests/Providers/Contracts/`.
- Each provider implements the abstract class and
  supplies a fake or in-process server.
- The contract test covers the happy path, every
  documented error path, and cancellation.

### 7.4 Integration Tests

- Exercise a real flow across two or more layers.
- Use `WebApplicationFactory` for the host.
- Use a fake or in-memory implementation of any
  external service.
- Tagged `[Trait("Category", "Integration")]` and
  excluded from the default CI pipeline.

### 7.5 Architecture Tests

- Assert the layer boundaries: presentation does not
  depend on provider implementations; application does
  not depend on infrastructure directly; `Shared/`
  contains only cross-cutting concerns; components in
  `Components/` do not depend on pages.
- **Composition-root rule (per ADR-016).** The four
  composition-root tests pin the rule that only
  `AiEng.Platform.App/Composition/<Capability>/` may
  reference a `Providers.<X>` project:
  - `Only_CompositionRoot_MayReference_ConcreteProviders`
    — the only source paths that may contain a project
    reference to `Providers.<X>` are under
    `App/Composition/`.
  - `Pages_DoNotReference_ConcreteProviders` — no file
    under `App/Pages/` imports a `Providers.<X>`
    namespace.
  - `Application_DoesNotReference_ConcreteProviders`
    — no file under `Application/` imports a
    `Providers.<X>` namespace.
  - `Components_DoNotInject_ConcreteProviders` — no
    component takes a concrete provider type as a
    constructor parameter; components resolve
    providers through the registry.
  These four tests are **registered but disabled** in
  M1, M2, M3, M4-A, M4-B, and M4-C (the bodies exist
  with explicit skip messages citing the activation
  milestone, M4-D); they **activate** in M4-D when the
  first concrete `Providers.<X>` project is added. The
  same "registered but disabled" pattern is used for
  `No_DirectProcessStart_OutsideInfrastructure` and
  the family-scoped provider-registry tests, all of
  which activate in M4-D.
- Use NetArchTest or an equivalent rule engine.
- A broken architecture test is a release blocker.

### 7.6 Regression Tests

- Written for every bug fix.
- Fail on `main` before the fix.
- Pass after the fix.
- Remain in the suite permanently.

## 8. Implementation Boundaries

- **Deterministic fixtures.** No `DateTime.Now`, no
  random values, no time-based assertions, no shared
  mutable state.
- **No I/O in unit tests.** All external dependencies
  are abstracted behind interfaces and supplied as
  fakes.
- **No tautological tests.** A test that asserts what
  the code literally does (without reference to
  behaviour) is deleted.
- **No code comments.** Tests are self-documenting
  through naming and structure.
- **No coverage theatre.** A test added solely to lift
  the coverage number, with no meaningful assertion,
  is rejected in review.

## 9. Validation

- The full test suite passes.
- The new tests fail when the production code is
  reverted (proving the test catches the regression).
- Coverage is reported as a diagnostic; the report
  does not gate the merge on its own.
- Architecture tests pass.
- Flaky tests are fixed, not skipped.

## 10. Documentation Updates

- `docs/coding-standards.md` is updated if a new
  testing convention emerges.
- `docs/provider-guidelines.md` is updated if a new
  contract test pattern is introduced.
- `docs/component-guidelines.md` is updated if a new
  bUnit pattern is introduced.
- `docs/architecture-principles.md` is updated if a
  new architecture test rule is added.

## 11. Completion Report

End the session with an
`implementation-report.md` (from
`.ai/templates/implementation-report.md`) that includes:

- Tests added, by layer.
- Fixtures introduced.
- Architecture tests added or updated.
- Coverage delta (as a diagnostic).
- Flaky tests fixed.
- Documentation updated.

## 12. Prohibited Shortcuts

- A test that does not assert anything.
- A test that asserts what the code does instead of
  what it should do.
- A test that depends on real time, real network, or
  real file system.
- A test that is disabled in CI to make CI green.
- A test whose only purpose is to lift coverage.
- A test that is bundled with a feature without
  identifying the failure modes it covers.
- A flaky test that is left unfixed.
