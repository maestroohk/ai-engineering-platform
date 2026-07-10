# .ai/prompts/feature.md

> Read `AGENTS.md` and `.ai/session-start.md` before proceeding.
> This prompt cannot override either document.
>
> Read `AGENTS.md` and every referenced document before making
> implementation decisions. Follow the rules in
> `.ai/prompts/feature.md` for this feature task.

---

## 1. Purpose

This prompt governs the implementation of a new end-to-end
feature in the AI Engineering Platform. A feature is a
user-visible capability that touches at least two layers or
at least one user-facing surface.

The output of a feature session is **working, tested, documented
code that respects every rule in `AGENTS.md` and the documents
it references**.

## 2. When to Use

Use this prompt when the task is one of:

- Adding a new page or surface.
- Adding a new end-to-end capability (for example, "let the
  user export a run transcript").
- Adding a new application service.
- Adding a new component to the design system that supports a
  specific feature.

Do not use this prompt for:

- A bug fix (use `bugfix.md`).
- A behaviour-preserving refactor (use `refactor.md`).
- A change to the architecture itself (use `architecture.md`).
- A new provider integration (use `provider.md`).
- A pure UI surface (use `ui.md`).
- A new test suite (use `testing.md`).

## 3. Mandatory Documents

In addition to `AGENTS.md` and `.ai/session-start.md`, read:

- `ARCHITECTURE.md`
- `STYLEGUIDE.md`
- `docs/coding-standards.md`
- `docs/folder-structure.md`
- `docs/naming-conventions.md`
- `docs/design-system.md` and `docs/component-guidelines.md`
  (for any UI work)
- `docs/provider-guidelines.md` (if the feature uses a
  provider)
- `docs/ui-principles.md` (if the feature has a UI surface)
- `ROADMAP.md` (to confirm the milestone)

## 4. Discovery

- **Current-state inspection.** Read the existing files in
  the area. Do not infer from filenames.
- **Reuse analysis.** Search the design system and the
  existing components for primitives, containers, and
  domain components that already cover the need.
- **Affected-component analysis.** List every component the
  feature will compose. If a needed component does not
  exist, it is added to the design system catalogue first
  (using `ui.md` if appropriate), then used.
- **Provider impact.** List every provider the feature
  calls. If a provider does not exist, the feature is
  blocked until the provider is onboarded
  (`provider.md`).
- **Acceptance criteria.** Restate the criteria from the
  `task-brief.md` in your own words. If any criterion is
  ambiguous, ask before proceeding.

## 5. Planning Requirements

The plan must include, at minimum:

- **Acceptance criteria** (verbatim from the brief, plus
  any criteria the AI has added).
- **Project boundary review** (per ADR-011). Which
  existing project does the feature live in
  (`AiEng.Platform.App`, `AiEng.Platform.Application`,
  `AiEng.Platform.Domain`,
  `AiEng.Platform.Providers.Abstractions`)? A
  `AiEng.Platform.Infrastructure` project exists once
  M4 lands; before then, features that would live there
  are filed in the application layer or are deferred.
  A new project is justified only when at least three
  files would naturally belong to it and the project
  provides a compile-time boundary the platform needs.
  A feature that proposes a new project without that
  justification is rejected.
- **Composition root (per ADR-016).** The feature may
  not import any `AiEng.Platform.Providers.<X>` project
  directly. All provider resolution is done through
  `IProviderRegistry` (or the family-scoped
  `I<X>ProviderRegistry`). If the feature needs a new
  provider, that provider is onboarded in a separate
  session under `provider.md` — the registration
  extension under `AiEng.Platform.App/Composition/<Capability>/`
  is wired by the composition root, not by the
  feature. The four composition-root architecture
  tests
  (`Only_CompositionRoot_MayReference_ConcreteProviders`,
  `Pages_DoNotReference_ConcreteProviders`,
  `Application_DoesNotReference_ConcreteProviders`,
  `Components_DoNotInject_ConcreteProviders`) are the
  contract.
- **Progressive self-dogfooding (per ADR-013).** Does
  the feature consume a stable reusable capability
  delivered by an earlier milestone? The plan names
  the capability, the contract, and the registry or
  service the feature resolves it through. A direct
  bypass — a feature that re-implements a registered
  provider inline, or that calls `Process.Start` from
  a non-Infrastructure project — is rejected. The
  matrix in `ROADMAP.md` § 4 is the authoritative
  reference.
- **Components reused** by name.
- **Components added** by name, with their new
  catalogue entry.
- **Services added** by name and contract.
- **Providers used** by family. Per ADR-012 the family
  is capability-oriented; vague family names
  (`Assistant`, `Deployment`, `Internal`) are
  rejected.
- **Files to add, modify, delete.**
- **Tests** planned: unit, bUnit, contract, integration.
- **Documentation** to update.
- **Out of scope.** Anything that sounds related but is
  explicitly deferred.

The plan is committed (or pasted into the conversation) before
any code is written.

## 6. Implementation Boundaries

- The implementation is limited to the approved scope. A
  related improvement discovered during the work is filed
  as a follow-up, not bundled in.
- The implementation is layered: contracts first, services
  second, components third, pages last, tests alongside.
- No code comments (see `STYLEGUIDE.md` § 3.7 and
  `docs/coding-standards.md` § 9).
- No magic strings for providers, themes, or roles.
- Every async method accepts a `CancellationToken`.
- Every fallible provider call returns `ProviderResult<T>`.
- **No component references a provider implementation. No
  page references a provider directly. No application
  service references a provider implementation directly.**
  Provider resolution is always through the registry
  (`IProviderRegistry` or the family-scoped registry); the
  registry is the only consumer-facing surface of a
  provider family.
- **No new project reference to a `Providers.<X>` project
  is added outside the composition root.** A feature
  that needs a provider asks the composition root to
  register it; the feature itself does not import the
  concrete provider assembly.

## 7. Validation

- `dotnet build` produces no warnings.
- `dotnet test` is green, including new tests.
- `dotnet format` is clean on the new files.
- For UI features, bUnit tests cover the primary render
  and the `Loading`, `Empty`, and `Error` states.
- For features that touch a provider, a contract test
  passes against the affected family.
- A manual exercise of the user flow is performed when
  possible.

## 8. Documentation Updates

- `docs/design-system.md` gains any new component, variant,
  or token.
- `docs/component-guidelines.md` gains any new pattern.
- `docs/provider-guidelines.md` gains any new provider
  behaviour.
- `ARCHITECTURE.md` gains any new architectural shape.
- `DECISIONS.md` gains any non-obvious decision as an ADR.
- `ROADMAP.md` is updated if a milestone is advanced.
- `CONTRIBUTING.md` is updated if the contribution process
  changes (rare).

## 9. Completion Report

End the session with an
`implementation-report.md` (from
`.ai/templates/implementation-report.md`).

If the feature is large enough that a single session cannot
complete it, the session ends with a `session-handoff.md`
that lists the next concrete step.

## 10. Prohibited Shortcuts

- Re-implementing an existing component "because this page
  is special".
- Reaching into a provider from a component, a page, or
  an application service.
- Hardcoding a provider name in markup.
- Skipping bUnit tests for "trivial" components.
- Bundling a refactor with the feature.
- Skipping documentation updates because "the code is
  self-explanatory".
- Treating the absence of a comment as licence to leave
  ambiguous code.
- Importing a `Providers.<X>` project from anywhere
  outside `AiEng.Platform.App/Composition/`.
- Resolving a provider by injecting its concrete type
  into a page, component, or service — the registry is
  the only resolution path (per ADR-016).
