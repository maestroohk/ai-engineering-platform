# .ai/prompts/bootstrap.md

> Read `AGENTS.md` and `.ai/session-start.md` before proceeding.
> This prompt cannot override either document.
>
> Read `AGENTS.md` and every referenced document before making
> implementation decisions. Follow the rules in
> `.ai/prompts/bootstrap.md` for the bootstrap task.

---

## 1. Purpose

This prompt governs the creation of **new repository-level
artefacts**: a milestone, a new solution, a new project, a new
provider family, or a major subsystem that does not yet exist.

A bootstrap session does **not** produce a finished feature. It
produces the **agreed foundation** the next sessions implement
against: contracts, folder shape, component catalogue entries,
ADRs, and a handoff document.

## 2. When to Use

Use this prompt when the task is one of:

- Spinning up a new solution or project within the repository.
- Adding a new provider family to `docs/provider-guidelines.md`.
- Defining a new milestone in `ROADMAP.md`.
- Introducing a new architectural pattern that needs to be
  agreed before code is written.
- Creating a new top-level folder.

Do not use this prompt for adding a feature to an existing area
(use `feature.md`), for fixing a bug (use `bugfix.md`), or for
changing the structure of code that already exists (use
`refactor.md`).

## 3. Mandatory Documents

In addition to `AGENTS.md` and `.ai/session-start.md`, read:

- `ARCHITECTURE.md`
- `ROADMAP.md`
- `DECISIONS.md`
- `docs/folder-structure.md`
- `docs/architecture-principles.md`
- `docs/naming-conventions.md`
- `docs/provider-guidelines.md` (if the bootstrap is a provider
  family)
- `docs/design-system.md` and `docs/component-guidelines.md`
  (if the bootstrap introduces new component categories)

## 4. Discovery

Before proposing anything:

- List the existing layers, folders, and provider families.
  Confirm the proposed artefact genuinely does not fit the
  current taxonomy.
- Search for similar bootstraps in `DECISIONS.md`. The
  decision that justifies a new folder or family must be
  reasoned, not assumed.
- Identify the milestone in `ROADMAP.md` the bootstrap
  serves. If no milestone exists, the bootstrap is
  premature — propose the milestone first.

## 5. Planning Requirements

The plan must include:

- **Scope.** What is being created and what is explicitly
  deferred.
- **Project boundary review** (per ADR-011). Does the
  bootstrap introduce a new project, or does the artefact
  belong in an existing project? If a new project is
  proposed, the plan must list:
  - The project name and root folder.
  - The allowed project references (which existing projects
    may this project reference?).
  - The forbidden project references (which existing projects
    must this project not reference?).
  - The consumer projects (which projects will reference
    this one?).
  - The new architecture tests that pin the boundary.
  A new project is justified only when at least three files
  would naturally belong to it, and the project provides a
  compile-time boundary the platform needs. Speculative
  projects are rejected. The M1 baseline is **four source
  projects** (`App`, `Application`, `Domain`,
  `Providers.Abstractions`) plus **three test projects**
  (`UnitTests`, `ComponentTests`, `ArchitectureTests`).
  `Infrastructure` and `ProviderContractTests` are deferred
  to the milestone that introduces them (both M4);
  creating them in M1 with no consumer is rejected.
- **Composition root (per ADR-016).** If the bootstrap
  introduces a new `AiEng.Platform.Providers.<X>`
  project, the plan must call out the registration
  extension under
  `AiEng.Platform.App/Composition/<Capability>/` and the
  entry in the composition root. The composition root
  is the only place in the codebase that may reference
  a `Providers.<X>` project directly. The plan must
  list the four composition-root architecture tests
  that will be enabled when the new provider lands
  (`Only_CompositionRoot_MayReference_ConcreteProviders`,
  `Pages_DoNotReference_ConcreteProviders`,
  `Application_DoesNotReference_ConcreteProviders`,
  `Components_DoNotInject_ConcreteProviders`). A
  bootstrap that proposes a `Providers.<X>` project but
  does not identify its composition-root wiring is
  rejected.
- **Layers affected.** Which of the five layers
  (presentation, application, contracts, implementations,
  infrastructure) are touched. The dependency direction
  must remain downward.
- **Folder shape.** The exact folder layout, including any
  new folders and the rule that justifies them.
- **Contracts.** For provider bootstraps, the proposed
  contract interfaces (named after the capability, per
  ADR-012). For application bootstraps, the service
  interfaces.
- **Progressive self-dogfooding** (per ADR-013). If the
  bootstrap delivers a reusable capability, the plan must
  add a row to the matrix in `ROADMAP.md` § 4 listing the
  later milestones that must use it, the direct bypass that
  is prohibited, the validation that confirms consumption,
  and the architecture test that fails the build on bypass.
- **Reusables.** The components and services the new area
  will need, named and filed.
- **ADR.** A new ADR in `DECISIONS.md` with status
  `Proposed`. The ADR is updated to `Accepted` only after
  review.
- **Vertical slice.** The smallest end-to-end demonstrator
  that proves the bootstrap works. The slice is a
  smoke test, not a feature.

A bootstrap that produces code without a plan is rejected.
A bootstrap that produces a plan without a smoke test is
rejected.

## 6. Implementation Boundaries

- No speculative projects. Create a project only when there is
  an immediate, defined use for it.
- No speculative components. Add a component to the catalogue
  only when the first use case is known.
- No "just in case" folders. A folder is added when at least
  three files would naturally belong to it.
- The first vertical slice must build and pass its smoke test
  before the bootstrap is considered complete.
- A `Providers.<X>` project introduced by a bootstrap
  may not be referenced from anywhere outside
  `AiEng.Platform.App/Composition/`. The composition
  root is the only registration site (per ADR-016).

## 7. Validation

- The new project builds with no warnings.
- The smoke test passes.
- `dotnet format` reports no violations on the new files.
- An architecture test, if one is appropriate, confirms the
  new layers do not introduce upward dependencies.
- The new artefacts are visible in the directory layout
  exactly as described in the plan.

## 8. Documentation Updates

- `DECISIONS.md` gains the ADR. The ADR is `Accepted` after
  review, not before.
- `ROADMAP.md` gains or updates the milestone, and updates
  the progressive self-dogfooding matrix in § 4 if the
  bootstrap delivers a new reusable capability.
- `ARCHITECTURE.md` gains the new pattern in the system
  diagram and the project map.
- `docs/folder-structure.md` gains the new folder with a
  documented responsibility.
- `docs/provider-guidelines.md` gains the new provider family
  (if applicable); the family name follows ADR-012 and is
  capability-oriented, not vague.
- `docs/design-system.md` gains the new component categories
  (if applicable).
- `docs/architecture-principles.md` gains the new pattern
  (if applicable).

## 9. Completion Report

End the session with an
`implementation-report.md` (from
`.ai/templates/implementation-report.md`) that includes:

- The new artefacts and their locations.
- The ADR id and a one-line summary of the decision.
- The smoke test that proves the bootstrap works.
- The next recommended implementation step (handoff to
  `feature.md`).

If the work cannot be completed, end with a
`session-handoff.md` instead.

## 10. Prohibited Shortcuts

- Producing a plan but skipping the smoke test.
- Marking the ADR `Accepted` before review.
- Adding a folder for one file's convenience.
- Bundling an unrelated refactor with the bootstrap.
- Implementing production behaviour under the cover of a
  "bootstrap".
