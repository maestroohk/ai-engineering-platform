# AGENTS.md

> **The constitution of this repository.**
>
> Every AI assistant, human contributor, and future maintainer MUST read this
> document — and every document it references — before writing, suggesting, or
> reviewing any code, configuration, or design decision in this project.
>
> The rules in this document are **permanent**. They apply for the lifetime of
> the project. They may only be changed through a documented entry in
> [`DECISIONS.md`](./DECISIONS.md). If a rule is violated, the change is
> rejected in code review — regardless of who made it.

---

## 1. Purpose of this Document

This repository will become a **production-quality, Windows-first AI Engineering
Platform** built with .NET 10 and Blazor. It is designed to survive years of
development by hundreds of contributors and AI agents working in parallel.

The goal of `AGENTS.md` is to make the **first ten seconds of any future
session** productive. After reading it, an agent must know:

- The project's structure and architecture.
- The standards for writing code, components, and UI.
- The non-negotiable rules that govern every change.
- Which document to read next for any given task.

This is not a quick-start guide. It is a contract.

---

## 2. Mandatory Reading Order

Before any implementation, suggestion, or architectural decision, read the
documents in this exact order. **Skipping or skimming is a violation.**

1. [`AGENTS.md`](./AGENTS.md) *(this file)*
2. [`ARCHITECTURE.md`](./ARCHITECTURE.md)
3. [`STYLEGUIDE.md`](./STYLEGUIDE.md)
4. [`docs/design-system.md`](./docs/design-system.md)
5. [`docs/component-guidelines.md`](./docs/component-guidelines.md)
6. [`docs/coding-standards.md`](./docs/coding-standards.md)
7. [`docs/folder-structure.md`](./docs/folder-structure.md)
8. [`docs/naming-conventions.md`](./docs/naming-conventions.md)
9. [`docs/provider-guidelines.md`](./docs/provider-guidelines.md)
10. [`ROADMAP.md`](./ROADMAP.md)

For UI tasks, also read:

- [`docs/ui-principles.md`](./docs/ui-principles.md)
- [`docs/architecture-principles.md`](./docs/architecture-principles.md)

For contribution and refactoring tasks, also read:

- [`CONTRIBUTING.md`](./CONTRIBUTING.md)
- [`DECISIONS.md`](./DECISIONS.md)

For guidance on how to invoke a structured workflow, read the relevant prompt
template in [`.ai/prompts/`](./.ai/prompts/).

> **No implementation occurs before these documents have been read.**

---

## 2.1 The AI Session Operational Sequence

The reading order above governs the **constitutional documents** — the
permanent rules and standards. Every AI session also follows an
**operational sequence** that turns those rules into disciplined
action. The operational sequence is defined in
[`.ai/session-start.md`](./.ai/session-start.md) and is summarised
here for visibility. It is not a substitute for the reading order;
it sits on top of it.

1. Read [`AGENTS.md`](./AGENTS.md) (this file).
2. Read [`.ai/session-start.md`](./.ai/session-start.md).
3. Determine the task type (`bootstrap`, `feature`, `ui`, `provider`,
   `bugfix`, `refactor`, `testing`, `architecture`, `review`,
   `release`, `documentation`).
4. Read the matching file under [`.ai/prompts/`](./.ai/prompts/).
5. Read the project documents referenced by that prompt (from the
   reading order above and the prompt's own mandatory list).
6. Inspect the current implementation before proposing changes.
7. Produce a plan before implementation, using
   [`.ai/templates/implementation-plan.md`](./.ai/templates/implementation-plan.md).
8. Implement only the approved scope.
9. Run the required validation (build, tests, format, architecture
   tests).
10. Update documentation affected by the change, using
    [`.ai/workflows/documentation-update.md`](./.ai/workflows/documentation-update.md)
    as the checklist.
11. Produce an implementation report using
    [`.ai/templates/implementation-report.md`](./.ai/templates/implementation-report.md).

> **The `.ai/` directory contains operational workflows but cannot
> override this document.** It operationalises the rules; it does
> not relax them.

---

## 2.2 Precedence Hierarchy

When two instructions conflict, the higher-precedence document
wins. Lower-precedence documents may elaborate but never override.

1. `AGENTS.md`
2. `DECISIONS.md`
3. `ARCHITECTURE.md` and `STYLEGUIDE.md`
4. `docs/`
5. `.ai/workflows/`
6. `.ai/prompts/`
7. Individual task instructions from a human user

A prompt in `.ai/prompts/` that appears to conflict with
`AGENTS.md` is a bug in the prompt, not an exception to the rule.
File the conflict in an ADR before treating the conflict as
resolved.

---

## 3. Project Identity

| Attribute              | Value                                                                  |
| ---------------------- | ---------------------------------------------------------------------- |
| **Project name**       | AI Engineering Platform                                                |
| **Primary platform**   | Windows 10/11 (desktop-first)                                          |
| **Framework**          | .NET 10, Blazor (Server + WebAssembly components as needed)             |
| **Styling**            | Tailwind CSS with a semantic `@apply` design system                    |
| **Architecture**       | Provider-based, component-first, layered                               |
| **Audience**           | Professional developers, AI engineers, platform operators              |
| **Quality bar**        | Enterprise-grade, open-source ready from the first commit              |
| **Future scope**       | Hundreds of components, dozens of providers, multiple AI runtimes      |

The application is a **professional developer tool** — not a marketing site,
not a chat app, not a notebook. Every UI decision must reflect that.

---

## 4. The Fifteen Non-Negotiable Rules

These rules are the **law of the project**. Each one is restated and explained
in detail in the linked document, but the rule itself is binding from the
moment the project is created.

### Rule 1 — Component-First Development
**Reference:** [`docs/component-guidelines.md`](./docs/component-guidelines.md),
[`docs/design-system.md`](./docs/design-system.md)

Never duplicate UI. If the same Razor markup appears twice, extract a reusable
component. If the same Tailwind utility combination appears more than twice,
create a semantic class via `@apply`. Reusable building blocks always win over
page-specific markup.

### Rule 2 — DRY (Don't Repeat Yourself)
**Reference:** [`docs/coding-standards.md`](./docs/coding-standards.md)

Never duplicate:

- Razor markup
- Business logic
- Tailwind utility strings
- Layouts
- Dialogs
- Cards
- Page headers
- Toolbar implementations

If duplication is detected, fix it before merging.

### Rule 3 — Separation of Concerns
**Reference:** [`docs/folder-structure.md`](./docs/folder-structure.md),
[`docs/architecture-principles.md`](./docs/architecture-principles.md)

Pages orchestrate. Components render. Models describe. DTOs transport. Services
coordinate. Infrastructure implements. Helpers assist. Extensions enrich.
Providers integrate. **Each folder has exactly one responsibility.**

### Rule 4 — Design System First
**Reference:** [`docs/design-system.md`](./docs/design-system.md)

No page may introduce raw HTML that re-implements an existing component.
Everything comes from reusable building blocks:

`AppButton`, `AppCard`, `AppMetricCard`, `AppSidebar`, `AppToolbar`,
`AppPageHeader`, `AppBadge`, `AppStatusDot`, `AppDialog`, `AppSection`,
`AppProviderCard`, `AppProjectCard`, `AppTaskCard`, `AppSessionCard`,
`AppEmptyState`, `AppSkeleton`, `AppLoading`.

If a needed building block does not exist, **create it first**, then use it.

### Rule 5 — Tailwind Discipline
**Reference:** [`STYLEGUIDE.md`](./STYLEGUIDE.md)

Tailwind is the styling engine. **Semantic classes via `@apply` are mandatory**
whenever a style combination repeats. Long utility chains in markup are a
design smell. Use design tokens for colors, spacing, radius, and shadow.

### Rule 6 — Desktop First
**Reference:** [`docs/ui-principles.md`](./docs/ui-principles.md)

The primary experience is Windows desktop. Layout, density, and interaction
are tuned for large screens and power users. Responsive behaviour is required
but is never the default; desktop is always prioritised.

### Rule 7 — Accessibility is Required
**Reference:** [`docs/ui-principles.md`](./docs/ui-principles.md)

Keyboard navigation, visible focus, ARIA labels, loading states, error
states, empty states, consistent spacing, and consistent typography are **not
optional**. They ship with every component on day one.

### Rule 8 — Provider-Based Architecture
**Reference:** [`docs/provider-guidelines.md`](./docs/provider-guidelines.md),
[`docs/architecture-principles.md`](./docs/architecture-principles.md)

Everything external is integrated through a provider. The UI never depends
directly on Treehouse, No Mistakes, Lavish Axi, GNHF, Firstmate, Ollama,
Claude, OpenAI, Git, Windows Terminal, PowerShell, or WSL. Providers expose
contracts; the UI consumes contracts.

### Rule 9 — Future-Proofing
**Reference:** [`docs/architecture-principles.md`](./docs/architecture-principles.md)

Every component assumes future functionality will be added. **Never hardcode
layouts, providers, or runtime assumptions.** Extensibility always wins over
convenience.

### Rule 10 — Naming Standards
**Reference:** [`docs/naming-conventions.md`](./docs/naming-conventions.md)

Names are descriptive and self-documenting. `AppMetricCard` is good;
`Card2`, `Widget`, `PanelNew`, and `Component1` are forbidden. The name
must communicate intent at the call site.

### Rule 11 — Folder Discipline
**Reference:** [`docs/folder-structure.md`](./docs/folder-structure.md)

Every folder has a documented responsibility. `Shared` contains only what is
genuinely shared across the application. Dumping files into `Shared` is a
violation of the architecture.

### Rule 12 — Documentation is Code
**Reference:** [`CONTRIBUTING.md`](./CONTRIBUTING.md),
[`DECISIONS.md`](./DECISIONS.md)

When a reusable component is added, update `docs/design-system.md` and
`docs/component-guidelines.md`. When architecture changes, update
`ARCHITECTURE.md`, `DECISIONS.md`, and `ROADMAP.md`. Documentation that lies
is worse than no documentation.

### Rule 13 — No Code Comments
**Reference:** [`STYLEGUIDE.md`](./STYLEGUIDE.md),
[`docs/coding-standards.md`](./docs/coding-standards.md)

Code comments are forbidden in this repository. They are considered visual
noise, a maintenance burden, and a sign that the code itself is not
self-explanatory. Code must be written so that its names, structure, and
type signatures are sufficient explanation. The only place explanation lives
is in these architecture and design documents. Do not add `//` or `/* */`
comments to source files. XML doc comments on **public API surfaces only**
are permitted when they convey contractual information (parameters, return
values, exceptions) that the type system cannot express — never narrative
explanation.

### Rule 14 — Progressive Self-Dogfooding
**Reference:** [`DECISIONS.md`](./DECISIONS.md) (ADR-013),
[`ROADMAP.md`](./ROADMAP.md) § 4 (Progressive Self-Dogfooding Matrix),
[`docs/architecture-principles.md`](./docs/architecture-principles.md) § 4.6

Every milestone must consume the stable reusable capabilities delivered by
earlier milestones. Later milestones must not bypass earlier platform
abstractions with temporary direct implementations.

Concretely:

- If a stable contract, registry, service, or abstraction exists, the
  feature consumes it through the contract, not by re-implementing the
  capability inline.
- The progressive self-dogfooding matrix in `ROADMAP.md` § 4 is the
  authoritative reference. Each row records the capability delivered,
  the later milestones that must use it, the direct bypass that is
  prohibited, the validation that confirms consumption, and the
  architecture test that fails the build on bypass.
- Examples of bypasses that are blocked by this rule:
  - Importing a provider implementation directly from `App` or
    `Application` instead of resolving it through `IProviderRegistry`.
  - Calling `Process.Start` from a non-Infrastructure project instead
    of using `IProcessRunner`.
  - Reading a secret from `appsettings.json` instead of from
    `ICredentialVault`.
  - Re-introducing a removed family (e.g. an `IWorkspaceProvider`)
    instead of using application-layer workspace state.
- External-tool dogfooding (the development team using an external tool
  manually while building the platform) is a **separate** discipline,
  governed by `.ai/workflows/tool-dogfooding.md` and the per-milestone
  "Dogfooding checkpoint" subsections in `ROADMAP.md`. The two
  disciplines must not be confused. This rule is about **platform
  self-dogfooding** — the product using its own abstractions.

A change that bypasses a row in the matrix is a blocker. The matrix is
enforced by code, not by convention.

### Rule 15 — Project Continuity State
**Reference:** [`.ai/state/README.md`](./.ai/state/README.md),
[`.ai/handoffs/README.md`](./.ai/handoffs/README.md),
[`.ai/workflows/feature-lifecycle.md`](./.ai/workflows/feature-lifecycle.md)
stage 8, [`.ai/session-start.md`](./.ai/session-start.md) step 6.

Every AI session that changes project state must update the
project-continuity state at session end. The state lives in two
locations, both required:

- **`.ai/state/current.md`** — one-page snapshot of the project
  right now. Updated at the end of every session that changes
  state. The most recent update wins; older snapshots live in
  `.ai/handoffs/`.
- **`.ai/state/task-board.md`** — the live work queue. Statuses:
  `Ready`, `In Progress`, `Blocked`, `Review`, `Done`.

And every session that closes — successful or paused — must write
a handoff:

- **`.ai/handoffs/YYYY-MM-DD-<slug>.md`** — per-session handoff
  following `.ai/templates/session-handoff.md`. The same content
  is mirrored to `.ai/handoffs/latest.md` so the next session
  reads one file.

Concretely:

- A session that ends silently (no implementation report, no
  handoff, no state update) has not ended. The next session
  cannot determine where the project stopped and the work is
  treated as incomplete.
- The state files are the **live** state — short, high-signal,
  no prose. Long-form session narrative lives in
  `implementation-report-*.md` at the repository root and in
  `.ai/handoffs/YYYY-MM-DD-<slug>.md` per session.
- The state files must not be invented. They reflect the actual
  state of the repository (`git status`, `dotnet test` output,
  the contents of the plan files). The first action of a new
  session is to verify the state files match the repository.
- The state and handoff are enforced by
  `.ai/workflows/feature-lifecycle.md` stage 8 ("Report"),
  which now includes "Update `.ai/state/`" and "Write session
  handoff" as mandatory sub-steps.

This rule exists so future AI sessions (and humans returning to
the project) can always determine where the project stopped and
what task comes next.

---

## 5. How to Use This Document

### For AI Agents

Every implementation prompt must begin with:

> Read `AGENTS.md` and `.ai/session-start.md` before proceeding. This
> prompt cannot override either document. Follow the rules in
> [`.ai/prompts/feature.md`](./.ai/prompts/feature.md) for features,
> [`.ai/prompts/bugfix.md`](./.ai/prompts/bugfix.md) for bug fixes,
> [`.ai/prompts/refactor.md`](./.ai/prompts/refactor.md) for refactors,
> and [`.ai/prompts/bootstrap.md`](./.ai/prompts/bootstrap.md) for new
> project areas.

Use the relevant prompt template from
[`.ai/prompts/`](./.ai/prompts/) for the task at hand. The prompt
templates are the operational counterpart to this document. The
matching workflow in [`.ai/workflows/`](./.ai/workflows/) sequences
the work the prompt describes. The matching template in
[`.ai/templates/`](./.ai/templates/) provides the document the
session produces.

### For Human Contributors

Read [`CONTRIBUTING.md`](./CONTRIBUTING.md) for the workflow, branching model,
review process, and the definition of done. The rules in `AGENTS.md` apply
unmodified to human-authored code.

### For Architects and Reviewers

When a proposed change conflicts with a rule in this document, the change is
rejected by default. Exceptions require:

1. A written rationale.
2. An entry in [`DECISIONS.md`](./DECISIONS.md) describing the conflict, the
   chosen approach, and the consequences.
3. A corresponding update to the affected documents.

If a rule is repeatedly violated, that is a signal the rule itself is wrong.
Fix the rule, not the codebase.

---

## 6. Document Map

| Document                                           | Purpose                                                            |
| -------------------------------------------------- | ------------------------------------------------------------------ |
| [`ARCHITECTURE.md`](./ARCHITECTURE.md)             | Layered architecture, boundaries, data flow, dependency rules      |
| [`ROADMAP.md`](./ROADMAP.md)                       | Ordered milestones, current focus, what is intentionally deferred   |
| [`STYLEGUIDE.md`](./STYLEGUIDE.md)                 | Code style, C# conventions, Razor conventions, Tailwind discipline  |
| [`CONTRIBUTING.md`](./CONTRIBUTING.md)             | Workflow, branching, review, definition of done                    |
| [`DECISIONS.md`](./DECISIONS.md)                   | Architecture Decision Records (ADRs)                               |
| [`docs/design-system.md`](./docs/design-system.md) | Tokens, semantic classes, component catalogue                      |
| [`docs/coding-standards.md`](./docs/coding-standards.md) | C# rules, Razor rules, async, exceptions, comments policy     |
| [`docs/component-guidelines.md`](./docs/component-guidelines.md) | Component anatomy, lifecycle, slots, parameter rules  |
| [`docs/folder-structure.md`](./docs/folder-structure.md) | Folder responsibilities and boundaries                       |
| [`docs/naming-conventions.md`](./docs/naming-conventions.md) | Naming standards across all artefacts                  |
| [`docs/ui-principles.md`](./docs/ui-principles.md) | Layout, density, motion, accessibility, empty/loading/error states  |
| [`docs/architecture-principles.md`](./docs/architecture-principles.md) | Provider model, layering, dependency direction, DI         |
| [`docs/provider-guidelines.md`](./docs/provider-guidelines.md) | Authoring, registering, configuring, testing providers       |
| [`.ai/README.md`](./.ai/README.md)                       | AI collaboration hub; precedence hierarchy; task routing table        |
| [`.ai/session-start.md`](./.ai/session-start.md)       | First file an AI reads after `AGENTS.md`; operational sequence        |
| [`.ai/prompts/bootstrap.md`](./.ai/prompts/bootstrap.md) | Template for bootstrapping a new project area                       |
| [`.ai/prompts/feature.md`](./.ai/prompts/feature.md)   | Template for implementing a new feature end-to-end                    |
| [`.ai/prompts/bugfix.md`](./.ai/prompts/bugfix.md)     | Template for diagnosing and fixing a bug                              |
| [`.ai/prompts/refactor.md`](./.ai/prompts/refactor.md) | Template for safe, behaviour-preserving refactors                     |
| [`.ai/prompts/review.md`](./.ai/prompts/review.md)     | Template for reviewing a change with severity-labelled findings       |
| [`.ai/prompts/architecture.md`](./.ai/prompts/architecture.md) | Template for changes to the architecture itself                |
| [`.ai/prompts/ui.md`](./.ai/prompts/ui.md)             | Template for UI work; design system first, four-state coverage        |
| [`.ai/prompts/testing.md`](./.ai/prompts/testing.md)   | Template for the test pyramid and architecture tests                  |
| [`.ai/prompts/provider.md`](./.ai/prompts/provider.md) | Template for onboarding a new provider through the contract model     |
| [`.ai/prompts/release.md`](./.ai/prompts/release.md)   | Template for cutting a release; full pre-release validation           |
| [`.ai/workflows/feature-lifecycle.md`](./.ai/workflows/feature-lifecycle.md) | Discovery → plan → approval → impl → tests → docs → review → report |
| [`.ai/workflows/ui-design-review.md`](./.ai/workflows/ui-design-review.md)     | UI review workflow with the four-state visual checklist         |
| [`.ai/workflows/provider-onboarding.md`](./.ai/workflows/provider-onboarding.md) | Capability → contract → fake → real → health → tests → docs    |
| [`.ai/workflows/tool-dogfooding.md`](./.ai/workflows/tool-dogfooding.md)     | Development-time vs product integration; staged dogfooding plan |
| [`.ai/workflows/documentation-update.md`](./.ai/workflows/documentation-update.md) | Which documents change for which kind of change            |
| [`.ai/workflows/release-checklist.md`](./.ai/workflows/release-checklist.md) | Full release procedure; no automation                            |
| [`.ai/templates/task-brief.md`](./.ai/templates/task-brief.md)                 | The human's specification for a task                          |
| [`.ai/templates/implementation-plan.md`](./.ai/templates/implementation-plan.md) | The AI's plan, produced before implementation                |
| [`.ai/templates/implementation-report.md`](./.ai/templates/implementation-report.md) | The AI's completion record; required at the end of a session |
| [`.ai/templates/review-report.md`](./.ai/templates/review-report.md)           | The structured output of a code review                        |
| [`.ai/templates/session-handoff.md`](./.ai/templates/session-handoff.md)       | The bridge between sessions when work pauses or transfers      |

---

## 7. What This Document Does Not Contain

This document does not describe:

- Specific feature implementations (see `ROADMAP.md`).
- The current sprint's task list (use the issue tracker).
- Provider-specific configuration (see `docs/provider-guidelines.md`).

It contains only the rules that **outlive** any particular feature, sprint, or
contributor.

---

## 8. Versioning of the Rules

This document evolves through Architecture Decision Records. The current
version of every rule is the one that ships in this file. Past decisions,
including those that have been superseded, live in [`DECISIONS.md`](./DECISIONS.md).

When this document is updated, update `DECISIONS.md` in the same change.
