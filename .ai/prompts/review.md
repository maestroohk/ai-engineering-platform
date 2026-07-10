# .ai/prompts/review.md

> Read `AGENTS.md` and `.ai/session-start.md` before proceeding.
> This prompt cannot override either document.

---

## 1. Purpose

This prompt governs the **review** of a change — a pull
request, a single commit, or a set of diffs prepared for
review. The output of a review session is a structured
`review-report.md` (from `.ai/templates/review-report.md`)
with severity-labelled findings and a final recommendation.

A review does not change code. A review reports findings. The
author of the change decides what to do with them.

## 2. When to Use

Use this prompt when the task is one of:

- Reviewing a pull request.
- Reviewing a single commit or a series of commits.
- Reviewing a draft change before it is committed.
- Self-reviewing a change before requesting human review.

Do not use this prompt for implementing changes (use
`feature.md`, `bugfix.md`, or `refactor.md`).

## 3. Mandatory Documents

In addition to `AGENTS.md` and `.ai/session-start.md`, read
the documents relevant to the change. At minimum:

- `ARCHITECTURE.md`
- `STYLEGUIDE.md`
- `docs/coding-standards.md`
- `docs/architecture-principles.md`

Read additionally based on the diff:

- `docs/design-system.md`, `docs/component-guidelines.md`,
  `docs/ui-principles.md` for UI changes.
- `docs/provider-guidelines.md` for provider changes.
- `docs/folder-structure.md` and `docs/naming-conventions.md`
  for structural changes.
- `DECISIONS.md` to confirm the change is consistent with
  existing decisions.

## 4. Discovery

- **Inspect the diff.** Read every changed file. Note the
  scope: which layers, which folders, which components.
- **Inspect the surrounding code.** A finding that lives
  outside the diff but is relevant to it is still in scope.
- **Read the matching `task-brief.md` and
  `implementation-plan.md`** if they exist, to understand
  the author's intent.
- **Confirm the change matches the prompt it followed.**
  A `feature.md` change should not include a refactor.
  A `bugfix.md` change should not include a feature.

## 5. Review Dimensions

Every review covers the following dimensions. Each finding
is labelled with its severity and dimension.

### 5.1 Architecture

- Does the change cross a layer boundary upward?
- Does the UI depend on a provider implementation?
- Does a service depend on infrastructure directly?
- Does the change introduce a `static` field with runtime
  state?
- Is the dependency direction respected?

### 5.1.1 Project Boundaries (per ADR-011)

- Is the change located in the correct project
  (`App`, `Application`, `Domain`, `Infrastructure`,
  `Providers.Abstractions`, or the right
  `Providers.<X>`)?
- Does the change introduce a forbidden project
  reference (e.g. `App` referencing a `Providers.<X>`
  project directly)?
- Does the change add a new project without the
  bootstrap's project-boundary review
  (three-files rule, compile-time-boundary
  justification)?
- Is the project boundary reflected in the
  architecture tests? A boundary that is not pinned
  by a test is a smell.

### 5.1.1a Composition Root (per ADR-016)

- Is the change located under
  `AiEng.Platform.App/Composition/<Capability>/` when
  it adds a registration for a `Providers.<X>` project,
  or is the registration scattered across `App`?
- Does any source file outside
  `AiEng.Platform.App/Composition/` reference a
  `Providers.<X>` project? Specifically:
  - Is a page (`App/Pages/**`) importing a
    `Providers.<X>` namespace?
  - Is a component (`App/Components/**`) injecting a
    concrete provider type?
  - Is an application service (`Application/**`) taking
    a concrete provider type as a constructor parameter?
  - Is a view model, DTO, or domain type importing a
    `Providers.<X>` namespace?
- Are the four composition-root architecture tests
  green
  (`Only_CompositionRoot_MayReference_ConcreteProviders`,
  `Pages_DoNotReference_ConcreteProviders`,
  `Application_DoesNotReference_ConcreteProviders`,
  `Components_DoNotInject_ConcreteProviders`)? A change
  that breaks any of these is a blocker.
- Where the change resolves a provider, does it go
  through the registry (`IProviderRegistry` or the
  family-scoped `I<X>ProviderRegistry`) rather than
  injecting the concrete type?
- For a new provider onboarding, does the registration
  extension live in the right
  `Composition/<Capability>/` folder, and is the
  registration call unconditional while the
  enablement is configuration-driven?

### 5.1.2 Progressive Self-Dogfooding (per ADR-013)

- Does the change consume the stable reusable
  capabilities from earlier milestones through the
  registry or the contract, or does it bypass them
  with a temporary direct implementation?
- For example, does the change:
  - Import a provider implementation directly from
    `App` or `Application` instead of resolving it
    through the registry?
  - Call `Process.Start` from a non-Infrastructure
    project instead of using `IProcessRunner`?
  - Read a secret from `appsettings.json` instead
    of from `ICredentialVault`?
  - Re-introduce a removed family (e.g. an
    `IWorkspaceProvider`) instead of using the
    application-layer workspace state?
  - Bypass the composition root by importing a
    `Providers.<X>` project from a page, component,
    or service?
- The matrix in `ROADMAP.md` § 4 is the
  authoritative reference. A change that bypasses a
  row in the matrix is a blocker.

### 5.2 DRY and Reuse

- Does the change duplicate a design-system component?
- Does the change duplicate Razor markup that should be
  a reusable component?
- Does the change duplicate business logic?
- Does the change duplicate Tailwind utility strings that
  should be a semantic class?

### 5.3 Component and Design System

- Does the change use the components listed in
  `docs/design-system.md`?
- For new components, are they added to the catalogue?
- For modified components, is the catalogue updated?
- Are `Loading`, `Empty`, and `Error` states handled?
- Are variants and sizes exposed through enums?

### 5.4 Accessibility

- Keyboard navigation: every interactive element is
  reachable and operable.
- Visible focus: focus is always visible.
- ARIA: every interactive element has a role and an
  accessible name.
- Color: status is never communicated by color alone.
- Motion: respects reduced-motion preference.

### 5.5 Security

- Secrets are not committed.
- Provider calls that execute user input on the host are
  gated behind confirmation.
- No arbitrary file system or process access outside a
  provider boundary.
- No `string.Format` or concatenation into SQL or shell
  commands.

### 5.5.1 Provider Family Naming (per ADR-012)

- Are provider families named after the **capability**
  they offer
  (`IAgentRuntimeProvider`, `IGitProvider`,
  `ITerminalProvider`, `IWorktreeProvider`,
  `IQualityGateProvider`, `IReviewProvider`,
  `IAutonomousLoopProvider`,
  `IOrchestrationProvider`)?
- A vague family name (`Assistant`, `Deployment`,
  `Internal`, `Workspace`) is a smell: it tells the
  reader nothing about what the contract actually
  does. A change that introduces a vague family name
  is rejected.

### 5.6 Tests

- The change is covered by tests.
- Tests are meaningful, not tautological.
- bUnit tests cover the primary render of new components.
- Provider contract tests cover new provider
  implementations.
- Regression tests exist for bugfixes.

### 5.7 Documentation

- The diff updates the documents it changes.
- A new component is in `docs/design-system.md`.
- A new pattern is in `docs/component-guidelines.md` or
  `docs/architecture-principles.md`.
- A new decision is in `DECISIONS.md`.
- The `ROADMAP.md` milestone is updated if advanced.

### 5.8 Style and Hygiene

- No code comments (Rule 13 of `AGENTS.md`).
- No long utility chains in markup.
- No magic strings for providers, themes, or roles.
- File-scoped namespaces, primary constructors, `record`
  types where appropriate.
- `dotnet format` is clean.

## 6. Severity Labels

Findings are labelled with one of:

- **Blocker** — the change violates `AGENTS.md` or
  `ARCHITECTURE.md`, or breaks a contract. Must be
  resolved before merge.
- **High** — the change violates a rule in `docs/` that
  is not architectural. Must be resolved before merge.
- **Medium** — the change is inconsistent with the
  surrounding code or could be simpler. Should be
  resolved before merge.
- **Low** — nit, suggestion, or improvement. Can be
  resolved in a follow-up.
- **Praise** — a particularly good choice worth calling
  out. Optional, but encouraged.

A change with a blocker is not mergeable. A change with
only medium and low findings is mergeable but should
address the mediums.

## 7. Implementation Boundaries

A review does not:

- Modify code (the author does that).
- Approve the change (the human reviewer does that).
- Merge the change (the merge process does that).

A review only produces the `review-report.md`. The report
is the deliverable.

## 8. Validation

- Every finding cites a specific file and line range.
- Every finding cites a specific rule (by document and
  section).
- The severity label is justified.
- The final recommendation is explicit: approve, request
  changes, or block.

## 9. Documentation Updates

A review does not change documentation. If the review
reveals that a document is out of date, the finding is
filed, and the author of the change (or a follow-up PR)
updates the document.

## 10. Completion Report

End the session with a `review-report.md` (from
`.ai/templates/review-report.md`) that lists every
finding, the severity, the dimension, the rule cited, and
the final recommendation.

## 11. Prohibited Shortcuts

- Skipping dimensions because the diff "looks fine".
- Filing a finding without citing a rule.
- Approving a change with a blocker because "we can fix it
  later".
- Suggesting a refactor in a review (open a follow-up
  issue; do not bundle).
- Praising the change without naming the specific
  choice.
