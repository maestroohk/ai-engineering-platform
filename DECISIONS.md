# DECISIONS.md

> Architecture Decision Records (ADRs) for the AI Engineering Platform.
> Every architectural choice that affects more than one file is recorded
> here. Future contributors read this file to understand **why** the
> repository looks the way it does.

---

## How to Use This Document

Each decision is a numbered ADR with the following structure:

- **Status** — Proposed, Accepted, Superseded, or Deprecated.
- **Context** — The situation that required a decision.
- **Decision** — What was decided.
- **Consequences** — The trade-offs accepted.
- **Supersedes / Superseded by** — Links to related ADRs.

ADRs are append-only. A decision that turns out to be wrong is not
deleted; it is marked **Superseded** with a link to its replacement.

The most recent ADR is at the bottom of the file.

---

## ADR Index

| ID  | Title                                                | Status   |
| --- | ---------------------------------------------------- | -------- |
| 000 | Record the architecture decision process             | Accepted |
| 001 | Adopt .NET 10 and Blazor as the platform foundation  | Accepted |
| 002 | Adopt a provider-based integration architecture     | Accepted |
| 003 | Adopt a component-first Blazor UI strategy           | Accepted |
| 004 | Adopt Tailwind with semantic `@apply` classes        | Accepted |
| 005 | Adopt desktop-first responsive design                | Accepted |
| 006 | Adopt Windows as the primary deployment target       | Accepted |
| 007 | Adopt trunk-based development with short-lived PRs   | Accepted |
| 008 | Forbid code comments in source files                 | Accepted |
| 009 | Treat documentation as a first-class deliverable     | Accepted |
| 010 | Sequence work by enablement, not feature size        | Accepted |
| 011 | Adopt a multi-project solution for compile-time layer boundaries (M1 = 4 src + 3 test; Infrastructure and ProviderContractTests deferred) | Accepted |
| 012 | Adopt capability-oriented provider families          | Accepted |
| 013 | Adopt progressive self-dogfooding of platform capabilities | Accepted |
| 014 | Make the four-state rule conditional on data ownership | Accepted |
| 015 | Version the design-system catalogue by implementation status | Accepted |
| 016 | Composition root may register multiple provider implementations; document the five provider lifecycle states | Accepted |

---

## ADR-000 — Record the architecture decision process

**Status:** Accepted

**Context:** A platform with this scope will accumulate dozens of
architectural decisions over its lifetime. Without a record, future
contributors — human and AI — will rediscover the same trade-offs,
make contradictory choices, and erode the foundations.

**Decision:** Maintain a `DECISIONS.md` file as the canonical record
of architectural decisions. Every decision that:

- Touches more than one folder, or
- Sets a rule referenced by `AGENTS.md`, or
- Changes a previously recorded decision,

must be added as a new ADR with the structure defined above.

**Consequences:**

- `DECISIONS.md` becomes a required reading for any deep
  architectural change.
- The file grows monotonically. This is acceptable; the index at the
  top is the navigation surface.
- The cost of recording a decision is low; the cost of not recording
  one compounds.

**Supersedes:** None.
**Superseded by:** None.

---

## ADR-001 — Adopt .NET 10 and Blazor as the platform foundation

**Status:** Accepted

**Context:** The platform must run on Windows desktop first, support a
professional developer UI, and remain maintainable for years. The
runtime, language, and UI framework choices lock in the productivity
and the ceiling of the codebase for the foreseeable future.

**Decision:** Build the platform on **.NET 10** with **Blazor** as the
primary UI framework. Use Blazor Server for the main shell and
evaluate WebAssembly components for offline-capable surfaces as the
platform grows.

**Rationale:**

- .NET 10 provides the latest language features, performance
  improvements, and long-term support.
- Blazor lets the team ship a single, type-safe UI layer in C#,
  avoiding context switches between JavaScript and C#.
- Blazor's component model is the right primitive for the
  design-system-first strategy in ADR-003.
- Server-side Blazor delivers desktop-quality density and latency
  for the primary Windows experience.

**Consequences:**

- All UI is C# and Razor. No React, Vue, or Angular code.
- The team commits to .NET release cadence; major version upgrades
  are tracked in `ROADMAP.md`.
- Offline-only surfaces require a separate evaluation (likely a
  WASM headless component).

**Supersedes:** None.
**Superseded by:** None.

---

## ADR-002 — Adopt a provider-based integration architecture

**Status:** Accepted

**Context:** The platform integrates with many external tools: AI
runtimes (Ollama, Claude, OpenAI), internal products (Treehouse, No
Mistakes, Lavish Axi, GNHF, Firstmate), and host facilities (Git,
PowerShell, WSL, Windows Terminal). If the UI depends on any of these
directly, every change ripples through the entire codebase.

**Decision:** Define a stable **provider contract** for each
integration family. The UI depends only on contracts. Each concrete
tool is an implementation registered through a **provider registry**.

**Rationale:**

- The contract is the unit of change. A new runtime, or a change in
  an existing one, is contained inside one provider implementation.
- The registry enables data-driven provider discovery in the UI
  (provider cards, health, configuration) without hardcoded names.
- The model is testable: a contract test suite validates every
  implementation against the same shape.

**Consequences:**

- Every external integration adds at least three artefacts: a
  contract, a registry, and an implementation.
- The UI can never import a provider type. This is enforced by code
  review and the folder structure.
- The provider model is the foundation of M3 in `ROADMAP.md`.

**Supersedes:** None.
**Superseded by:** None.

---

## ADR-003 — Adopt a component-first Blazor UI strategy

**Status:** Accepted

**Context:** In a Blazor codebase, inline markup is the path of least
resistance. It is also the path to a maintenance tax that compounds
within weeks. The platform will have hundreds of components; the only
way to keep them maintainable is to treat them as the primary
artefact of the UI layer.

**Decision:**

- Every piece of UI is a named, documented component.
- Components live under `Components/` (reusable) or `Pages/`
  (route-bound).
- Pages are compositions of components. Pages do not contain
  presentation logic beyond orchestration.
- Components are paired: `Component.razor` for markup,
  `Component.razor.cs` for behaviour, when behaviour is non-trivial.
- Components are documented in `docs/design-system.md` and
  `docs/component-guidelines.md` on creation.

**Consequences:**

- The initial cost of building a page is higher than writing inline
  markup. The long-term cost is dramatically lower.
- Component design becomes a first-class skill on the team.
- Component proliferation is a known risk; the design system and the
  rules in `AGENTS.md` are the guardrail.

**Supersedes:** None.
**Superseded by:** None.

---

## ADR-004 — Adopt Tailwind with semantic `@apply` classes

**Status:** Accepted

**Context:** Styling consistency at scale requires a single engine
and a layer of semantic abstraction. Inline utility chains produce
visually consistent code that is hard to read and hard to refactor.

**Decision:**

- Tailwind CSS is the only styling engine.
- Repeated utility combinations are extracted into **semantic
  classes** using `@apply`.
- Semantic classes are the primary styling surface in markup.
- Raw Tailwind utilities are acceptable in markup only when the
  combination is genuinely one-off.

**Rationale:**

- Tailwind gives us a single source of truth for tokens, scales,
  and utilities.
- Semantic classes let markup read like a description of intent
  (`class="app-card"`) rather than a wall of utilities.
- The design system is implemented as semantic classes, which makes
  it portable, themeable, and refactorable.

**Consequences:**

- A custom lint rule rejects long utility chains in markup.
- Component CSS files contain `@apply` blocks, not raw CSS rules.
- Theming (light/dark, brand variants) is achieved by swapping the
  design-system CSS, not by editing components.

**Supersedes:** None.
**Superseded by:** None.

---

## ADR-005 — Adopt desktop-first responsive design

**Status:** Accepted

**Context:** The platform is a professional developer tool for
Windows. Marketing-style responsive design misallocates effort and
produces layouts that are second-best on every surface. The platform
must feel right on a developer laptop and remain usable on smaller
windows.

**Decision:**

- Windows desktop is the primary surface. Layouts are tuned for
  large screens first.
- The shell must remain usable down to a 1280x720 window.
- Touch and mobile are explicitly out of scope.
- Responsive adjustments are documented per breakpoint and are
  applied at the layout level, not the component level.

**Consequences:**

- Density is high. Information per square inch is maximised.
- Components are designed for keyboard and mouse, not touch.
- Future "small screen" surfaces (e.g. a tablet companion) require a
  separate evaluation.

**Supersedes:** None.
**Superseded by:** None.

---

## ADR-006 — Adopt Windows as the primary deployment target

**Status:** Accepted

**Context:** The platform's audience is Windows developers. The
primary experience must feel native to that audience, even when the
underlying stack is cross-platform.

**Decision:** The primary deployment target is Windows 10/11.
PowerShell, WSL, and Windows Terminal are first-class providers. The
platform installs as a signed MSIX package.

**Consequences:**

- Path handling, line endings, and shell conventions are Windows
  conventions.
- macOS and Linux are explicitly out of scope for v1. The
  architecture does not preclude future cross-platform work.
- Provider contracts are designed to be host-agnostic so that a
  non-Windows provider can be added later.

**Supersedes:** None.
**Superseded by:** None.

---

## ADR-007 — Adopt trunk-based development with short-lived PRs

**Status:** Accepted

**Context:** A long-lived platform with many contributors needs a
branching model that minimises merge debt, encourages small changes,
and keeps `main` releasable.

**Decision:** Trunk-based development. `main` is always green. Feature
branches are short-lived (ideally under a day). Long-lived branches
are forbidden. Releases are cut from `main`.

**Consequences:**

- Every change is small, reviewable, and revertable.
- Feature flags are used for work that needs to ship before it is
  finished.
- The team commits to keeping `main` deployable at all times.

**Supersedes:** None.
**Superseded by:** None.

---

## ADR-008 — Forbid code comments in source files

**Status:** Accepted

**Context:** Code comments are widely accepted as a "best practice",
yet in long-lived codebases they consistently:

- Go stale faster than the code they describe.
- Restate what well-named code already says.
- Provide false confidence that the code is "documented".
- Add visual noise that makes diffs harder to read.
- Drift from the code on refactors, becoming a maintenance tax with
  no payoff.

The user has stated explicitly that they do not want comments in
their code, considering them ugly and unwanted. This decision
codifies that preference as a permanent rule.

**Decision:**

- Comments are forbidden in source files. This includes `//` line
  comments, `/* */` block comments, and narrative XML doc comments
  (`<summary>`, `<remarks>`).
- The only XML documentation permitted is **contractual**
  (`<param>`, `<returns>`, `<exception>`, `<typeparam>`) on
  **public API surfaces only**. These describe what the compiler
  cannot infer, not what the code does.
- All explanation that would otherwise live in comments must live in
  `docs/` (architecture) or `DECISIONS.md` (decisions).
- Code must be self-explanatory through naming, structure, and
  type signatures. When it is not, the code is refactored.

**Consequences:**

- Reviewers reject any change that introduces a code comment.
- The CI pipeline includes a check that fails on `//` or `/*`
  patterns in `.cs` and `.razor` files.
- Explanatory documents in `docs/` and `DECISIONS.md` grow to
  absorb what comments would have carried.
- New contributors must learn to express intent through code shape
  and naming, not prose.

**Supersedes:** None.
**Superseded by:** None.

---

## ADR-009 — Treat documentation as a first-class deliverable

**Status:** Accepted

**Context:** Documentation that drifts from the code is worse than no
documentation. In a platform this large, drift is the default unless
documentation is part of the definition of done.

**Decision:**

- Documentation updates ship in the same PR as the code they
  describe.
- `CONTRIBUTING.md` lists the documents that must be updated for
  each kind of change.
- A PR that introduces a new component without updating
  `docs/design-system.md` is incomplete.
- A PR that changes the architecture without updating
  `ARCHITECTURE.md` and `DECISIONS.md` is incomplete.

**Consequences:**

- The definition of done is heavier, but the codebase stays
  internally consistent.
- AI agents must treat documentation as code: read it, update it,
  reference it.

**Supersedes:** None.
**Superseded by:** None.

---

## ADR-010 — Sequence work by enablement, not feature size

**Status:** Accepted

**Context:** It is tempting to start with the most visible feature
(a chat UI, a provider card, a dashboard). In a platform with a long
lifetime, the right first step is the one that **unblocks the most
later work**.

**Decision:** Milestones in `ROADMAP.md` are ordered by enablement.
The design system comes before the layout. The provider registry
comes before the conversation UI. The conversation UI comes before
the terminal integration. The terminal integration comes before the
provider configuration surface.

**Rationale:**

- The first mistake in a new platform is building a feature before
  the design system. Every later feature then reinvents the same
  components.
- The second mistake is building a feature before the provider model
  is proven. Every later feature then has to retrofit contracts.
- The third mistake is building a feature before the persistence
  model is settled. Every later feature then has to migrate.

**Consequences:**

- Early milestones are visually underwhelming. They are foundation
  work, not demos.
- The team commits to the discipline of finishing the foundation
  before building the visible product.
- Marketing or demo needs are met by temporary shells, never by
  short-circuiting the foundation.

**Supersedes:** None.
**Superseded by:** None.

---

## ADR-011 — Adopt a multi-project solution for compile-time layer boundaries

**Status:** Accepted

**Context:** The current `docs/folder-structure.md` placed
presentation, application services, provider contracts, provider
implementations, and infrastructure inside a single
`src/AiEng.Platform/` project. Folders provide naming separation
but not compile-time separation — a Razor component can still
`using AiEng.Platform.Providers.Ollama;` and the compiler will
happily accept it. Architecture tests catch the violation
**after** the code exists, never before.

A platform that is expected to grow to include a Blazor UI, an
orchestration layer, a domain layer, provider contracts, multiple
provider implementations, a process runner, persistence, a CLI, a
PowerShell host, and tests needs **real** compile-time boundaries
between the layers, not folder-level intent.

The first commit, however, must be **small enough to verify in
isolation**. A first commit that creates five source projects and
four test projects, plus a solution, plus providers deferred, plus
infrastructure shared across layers, is too large to bootstrap
honestly. The M1 commit must introduce only the projects the M1
work actually requires and **defer** every other project — even
projects that the architecture will eventually need — to the
milestone that genuinely needs them.

**Decision:** Adopt **Option C — a hybrid solution** with a small
initial project set. M1 creates a solution and exactly **four
source projects** plus **three test projects**. Two further
projects that the architecture will need — `Infrastructure` and
`ProviderContractTests` — are **deferred** to the milestone that
introduces them. The M1 project set is:

```
AiEng.Platform.sln

src/
  AiEng.Platform.App/            # the host (Blazor Server entry point, DI wiring, configuration, app shell)
  AiEng.Platform.Application/    # application services, view models, DTOs, domain models
  AiEng.Platform.Domain/         # pure domain types, value objects, capability contracts that
                                 #   the application layer depends on (no external dependencies)
  AiEng.Platform.Providers.Abstractions/   # the provider family contracts and the shared
                                          #   ProviderResult / ProviderError types
```

The first commit also creates the **three** test projects that
the M1 work requires:

```
tests/
  AiEng.Platform.UnitTests/           # unit tests for application services and domain logic
  AiEng.Platform.ComponentTests/      # bUnit tests for Blazor components
  AiEng.Platform.ArchitectureTests/   # NetArchTest + bUnit; enforces project boundaries,
                                      #   layer rules, the no-comments rule, the
                                      #   no-provider-in-UI rule
```

The M1 commit explicitly **does not** create:

```
Deferred to the milestone that introduces the capability:

  AiEng.Platform.Infrastructure/   # Created in M4, when IProcessRunner, ICredentialVault,
                                   #   IClock, and persistence are first consumed. Creating
                                   #   it in M1 with no consumer is a speculative project.
  AiEng.Platform.ProviderContractTests/  # Created in M4 alongside the first concrete providers
                                        #   (GitProvider and OllamaLaunchProvider). No provider
                                        #   means no contract test to host.

Deferred to the milestone that needs them (added when implemented):
  AiEng.Platform.Providers.Ollama/
  AiEng.Platform.Providers.Claude/
  AiEng.Platform.Providers.OpenAi/
  AiEng.Platform.Providers.Codex/
  AiEng.Platform.Providers.Git/
  AiEng.Platform.Providers.PowerShell/
  AiEng.Platform.Providers.Wsl/
  AiEng.Platform.Providers.WindowsTerminal/
  AiEng.Platform.Providers.GitBash/
  AiEng.Platform.Providers.NativeWorktree/
  AiEng.Platform.Providers.Treehouse/
  AiEng.Platform.Providers.NoMistakes/
  AiEng.Platform.Providers.NativeReview/
  AiEng.Platform.Providers.LavishAxi/
  AiEng.Platform.Providers.Gnhf/
  AiEng.Platform.Providers.NativeOrchestration/
  AiEng.Platform.Providers.Firstmate/
  AiEng.Platform.Cli/                 # CLI entry point
  AiEng.Platform.PowerShellHost/      # PowerShell module integration
```

The rule that governs the deferred list is **the same rule that
governs every project**: a project is created when the work
requires it, not before. A project without a consumer is a
speculative project; speculative projects are rejected (see
`AGENTS.md` Rule 7 and `docs/architecture-principles.md` § 4.2).

**Allowed project references (the M1 reference graph; later
milestones extend this graph when the deferred projects are
created):**

- `AiEng.Platform.App` → `Application`, `Providers.Abstractions`
  (in M1, before `Infrastructure` is created; `App` consumes
  `Infrastructure` once it lands in M4)
- `AiEng.Platform.App` → provider implementation projects,
  **but only through the composition root** (see ADR-016).
  The composition root is the only place in `App` that may
  reference a `Providers.<X>` project directly. The composition
  root may register **any number of** provider implementations
  simultaneously; pages, components, application services, view
  models, DTOs, and domain types must never reference a
  concrete provider implementation project. The composition
  root is the only place that may register concrete
  implementations; everything else resolves through the
  registry.
- `AiEng.Platform.Application` → `Domain`, `Providers.Abstractions`
  (in M1; `Application` consumes `Infrastructure` once it lands
  in M4)
- `AiEng.Platform.Domain` → (no project references; pure types)
- `AiEng.Platform.Infrastructure` (deferred to M4) → `Domain`
- `AiEng.Platform.Providers.Abstractions` → (no project references; pure contracts and the result envelope)
- `AiEng.Platform.Providers.<X>` → `Providers.Abstractions`, `Infrastructure`
- `AiEng.Platform.UnitTests` → `Application`, `Domain`, `Providers.Abstractions`
- `AiEng.Platform.ComponentTests` → `App`, `Application`, `Domain`, `Providers.Abstractions`
- `AiEng.Platform.ArchitectureTests` → every project that exists
- `AiEng.Platform.ProviderContractTests` (deferred to M4) →
  `Providers.Abstractions`, the provider implementation under
  test, and any shared fakes (fakes live in a `Tests.Common`
  project added when the first provider is implemented)

**Forbidden project references (architecture-test enforced):**

- `Domain` referencing anything except BCL types.
- `Providers.Abstractions` referencing anything except BCL types
  and the shared result envelope.
- Any layer referencing the App project (the App is the composition
  root, not a library).
- A provider implementation referencing the App or any
  component.
- A test project referencing a test target through a path that
  bypasses the public API (tests assert behaviour, not internals).
- A provider implementation referencing another provider
  implementation. Provider cooperation belongs in the application
  layer.
- A page, component, application service, view model, DTO, or
  domain type referencing a `Providers.<X>` project. The
  composition root (per ADR-016) is the only place in the
  solution that may reference concrete provider implementation
  projects. This rule is enforced by the
  `Only_CompositionRoot_MayReference_ConcreteProviders`,
  `Pages_DoNotReference_ConcreteProviders`,
  `Application_DoesNotReference_ConcreteProviders`, and
  `Components_DoNotInject_ConcreteProviders` tests in
  `AiEng.Platform.ArchitectureTests`.

**Where things live:**

- **Shared contracts** — `AiEng.Platform.Providers.Abstractions` for
  provider family contracts, `AiEng.Platform.Domain` for domain
  types and value objects, `AiEng.Platform.Application` for
  application-layer interfaces.
- **UI components** — `AiEng.Platform.App` (the Blazor host is the
  only place Razor components live in M1; later, a `Library`
  project may be added if a component is shared with another
  surface such as the CLI).
- **Tailwind assets** — `AiEng.Platform.App/wwwroot/css/` for the
  application-level CSS. The `tailwind.config.js` lives at the
  solution root and emits a single `app.css` consumed by the App
  project. The `docs/design-system.md` tokens are the canonical
  source of truth; `tailwind.config.js` reads them.

**How architecture tests complement compile-time boundaries:**

The compile-time boundaries catch the **upward dependency**:
`Domain` cannot reference `Application`, a provider cannot
reference the App, and a test cannot reach into internals. The
architecture tests in `AiEng.Platform.ArchitectureTests` add three
rules that the compiler cannot easily express:

1. The architecture tests cover everything the **Roslyn analyzers**
   cannot: namespace conventions, the no-code-comments rule, the
   presence of `Loading`/`Empty`/`Error`/`Populated` slots on
   every component that owns data, the no-`Process.Start` rule
   in the App and Application projects (only `IProcessRunner` may
   start a process, and `IProcessRunner` itself is in the
   `Infrastructure` project that lands in M4 — until then, the
   architecture test for the rule is registered but disabled;
   its activation milestone is recorded in `ROADMAP.md`), and
   the registration of every provider through `IProviderRegistry`.
2. The architecture tests verify the **public surface** of every
   contract is what `docs/provider-guidelines.md` documents.
3. The architecture tests are part of the definition of done for
   every milestone. A milestone is not done until the
   architecture tests for its layer pass.

**Consequences:**

- The first commit is a `dotnet new sln` and seven `dotnet new`
  calls: four source projects, three test projects. The two
  projects the architecture will eventually need
  (`Infrastructure` and `ProviderContractTests`) are deferred to
  the milestones that need them. This is a one-time cost, paid
  earlier than the previous correction (which deferred nothing)
  and later than the original (which deferred nothing either).
- A project is created only when the work requires it. The
  project is not created speculatively. `Infrastructure` is
  created in M4 when its first consumer
  (`IProcessRunner`/`ICredentialVault`/`IClock`) lands;
  `ProviderContractTests` is created in M4 when the first
  concrete providers (the `GitProvider` and
  `OllamaLaunchProvider`) land.
- A change to a project boundary (adding a project, removing one,
  changing a reference) is an architectural decision and goes
  through an ADR.
- The architecture tests run in the default CI pipeline. A broken
  test is a release blocker.
- M1 ships a smaller, more honest bootstrap than the previous
  correction suggested. The trade-off is that the architecture
  test for the no-`Process.Start` rule is registered-but-disabled
  for M1, M2, and M3; it activates in M4 when
  `AiEng.Platform.Infrastructure` is created. The activation
  schedule is recorded in `ROADMAP.md`.

**Supersedes:** None.
**Superseded by:** ADR-016 (the composition-root clause
only; the rest of ADR-011 stands).

---

## ADR-016 — Composition root may register multiple provider implementations; document the five provider lifecycle states

**Status:** Accepted

**Context:** ADR-011 recorded the M1 project set and
the rule that the `App` project may reference **at
most one** provider implementation project at a
time. The rule was motivated by a desire to enforce
provider isolation: the host wires the chosen
provider through DI, and the boundary is the
compile-time separation between `App` and a single
`Providers.<X>` project.

In practice, the rule is wrong on two counts. First,
it is too restrictive. A typical host run registers
several provider implementations at the same time —
the agent runtime (`OllamaLaunchProvider`), the
source control (`GitProvider`), the terminals
(`PowerShellProvider`, `WindowsTerminalProvider`),
the worktree (`NativeWorktreeProvider`), the review
(`NativeReviewProvider`), and the quality gate
(`NoMistakesProvider`) all coexist in the same DI
container. The UI lists them through the family
registries, and the user picks one per operation.
Forcing the host to reference only one `Providers.<X>`
project at a time would require either (a) a single
"kitchen sink" provider project that ships every
implementation, or (b) a build-time selection step
that swaps projects per run. Both are worse than the
problem the rule was trying to solve.

Second, the rule conflates two distinct things. It
talks about a "host project" referencing "provider
implementation projects", but it does not name the
**composition root** — the set of files in `App` whose
sole purpose is to wire concrete provider
implementations into DI and read configuration to
decide which providers to register. The composition
root is the only place in `App` that may reference
`Providers.<X>` projects. UI pages, components,
application services, view models, DTOs, and domain
types must never reference a concrete provider
implementation. The compile-time boundary the rule
was trying to enforce belongs at this finer
granularity, not at the project level.

The rule is also silent on a related concern: the
**lifecycle of a provider within a single host run**.
The platform's health poller, configuration UI, and
diagnostics page all need a shared vocabulary for
"is this provider in the binary", "did the host
register it", "is the user opted in", "is it
operational right now", and "did the user pick it
for the current operation". Without a documented
state model, every consumer invents its own.

**Decision:** Supersede the composition-root clause
of ADR-011 with the following rules. The rest of
ADR-011 stands.

1. **The composition root is the only place that
   may reference `Providers.<X>` projects.** The
   composition root is the set of files in
   `AiEng.Platform.App` whose sole responsibility is
   to wire concrete provider implementations into the
   DI container at host startup and read configuration
   to decide which providers to register for this run.
   It consists of:
   - `Program.cs` (the host entry point).
   - Dedicated registration extension methods under
     `AiEng.Platform.App/Composition/`, one per
     provider family or per concrete provider
     implementation, each named after the capability
     or the implementation.
   - Host-specific configuration modules under
     `AiEng.Platform.App/Configuration/`.

   The composition root is the only place in the
   solution that may `using
   AiEng.Platform.Providers.<X>;`, reference a
   `Providers.<X>` project, register a concrete
   `*Provider` class in DI, call a `*Provider`
   constructor directly, or resolve a concrete
   `*Provider` from the service collection.

2. **The composition root may register any number of
   provider implementations simultaneously.** A
   typical host run registers the agent runtime, the
   source control provider, the terminals, the
   worktree, the review, and the quality gate all at
   once. Every registered implementation is visible
   to the UI through the family-scoped registry; the
   UI selects one at runtime when the user makes a
   choice. The previous "at most one" restriction
   is removed.

3. **UI pages, components, application services,
   view models, DTOs, and domain types must never
   reference a concrete provider implementation
   directly.** They may reference
   `Providers.Abstractions` for the family contracts,
   and they may resolve a provider through the
   `IProviderRegistry` and the family-scoped
   registries. They may not `using
   AiEng.Platform.Providers.<X>;` from any of the
   listed locations, may not take a concrete
   `*Provider` type in a constructor, may not
   instantiate a `*Provider` with `new`, and may not
   resolve a provider through anything other than
   the registry.

4. **The composition root is not a place for
   business logic, rendering logic, or domain
   rules.** It registers providers and reads
   configuration; it does not orchestrate them.
   Provider cooperation belongs in the application
   layer.

5. **Provider lifecycle states.** A provider
   progresses through five distinct states during
   the lifetime of a host run:

   - **Compiled-in** — the provider implementation
     is a referenced project in the solution's
     compile graph. Reachable from the composition
     root and from the family contract test
     project.
   - **Registered** — the composition root has
     added the provider to DI. Reachable through DI
     but may not be visible to the user.
   - **Enabled** — the provider's configuration
     section is present and valid; the user has
     opted in. The only providers the UI sees
     through the family-scoped registry.
   - **Healthy** — a periodic health check has
     confirmed the provider is reachable and
     operational. Reported through `ProviderHealth`
     (`Healthy`, `Degraded`, `Unhealthy`) by the
     `IProviderRegistry` and the family-scoped
     registries. A provider may be enabled and
     unhealthy at the same time; the UI surfaces
     the health state and may offer the user a way
     to disable an unhealthy provider.
   - **Selected** — the user has chosen this
     provider for a specific operation. Selection
     is per-operation: the same provider may be
     selected for one operation and not for another.
     A provider that is not enabled cannot be
     selected.

   The states are distinct, not cumulative. A
   provider is exactly one of these states at any
   moment; the next state is reached through a
   specific event (compile, register, enable,
   health-check, select).

**The composition-root rule is operationalised in:**

- `ARCHITECTURE.md` § 2.5 — defines the composition
  root, the may/may-not list, the project-reference
  graph, and the architecture test that pins the
  rule.
- `docs/architecture-principles.md` § 4 — restates
  the composition-root rule in the architectural
  principles.
- `docs/provider-guidelines.md` § 4.6 — documents
  the five provider lifecycle states as the
  vocabulary the registry, the health poller, the
  configuration UI, and the diagnostics page share.
- `docs/folder-structure.md` § 3.0 and § 3.14 —
  documents the `Composition/` folder and the
  Configuration folder responsibilities.
- `ROADMAP.md` § 4 — adds four architecture-test
  rows to the progressive self-dogfooding matrix:
  `Only_CompositionRoot_MayReference_ConcreteProviders`,
  `Pages_DoNotReference_ConcreteProviders`,
  `Application_DoesNotReference_ConcreteProviders`,
  `Components_DoNotInject_ConcreteProviders`.
- `.ai/prompts/provider.md` § 5, 6, 7 — the
  provider onboarding prompt lists the composition
  root as the only registration site and
  configuration check as the only enablement
  trigger.
- `.ai/prompts/feature.md` § 5, 6 — the feature
  prompt lists the composition root as the only
  place a feature may be wired and the registry as
  the only resolution path.
- `.ai/prompts/review.md` § 5.1, § 5.1.1, §
  5.1.2 — the review prompt adds the
  composition-root rule to the architecture and
  progressive self-dogfooding review dimensions.
- `.ai/prompts/bootstrap.md` § 5, § 6 — the
  bootstrap prompt lists the composition root as
  the only place that may reference concrete
  provider projects in a bootstrap session.

**Architecture tests (composition-root rule):**

- `Only_CompositionRoot_MayReference_ConcreteProviders`
  — fails the build if any source file outside
  `AiEng.Platform.App/Composition/` contains a
  `using AiEng.Platform.Providers.<X>;` statement
  or instantiates a `*Provider` type.
- `Pages_DoNotReference_ConcreteProviders` — fails
  the build if any file under
  `AiEng.Platform.App/Pages/` contains a
  `using AiEng.Platform.Providers.<X>;` statement.
- `Application_DoesNotReference_ConcreteProviders` —
  fails the build if any source file under
  `AiEng.Platform.Application/` contains a
  `using AiEng.Platform.Providers.<X>;` statement.
- `Components_DoNotInject_ConcreteProviders` —
  fails the build if any code-behind file under
  `AiEng.Platform.App/Components/` takes a
  concrete `*Provider` type in a constructor.

The four tests are part of the definition of done
for M4-D (per `ROADMAP.md`) and any milestone
that introduces a new concrete provider
implementation. The tests are **registered but
disabled** until the first concrete provider
implementation lands; before then, no
`Providers.<X>` project exists, so the rule is
satisfied by construction, and the tests are
enabled when the first project lands.

**Consequences:**

- The composition root is the only place in the
  solution that references `Providers.<X>`
  projects. The previous "at most one" rule is
  removed; the new rule is "the composition root
  may register any number of provider
  implementations, and only the composition root
  may reference concrete provider projects".
- The compile-time boundary is now finer: the
  rule is enforced by file location, not by
  project count. The architecture tests in
  `AiEng.Platform.ArchitectureTests` enforce the
  rule; the four tests above fail the build on
  violation.
- The five lifecycle states
  (`Compiled-in`, `Registered`, `Enabled`,
  `Healthy`, `Selected`) are the shared vocabulary
  of the registry, the health poller, the
  configuration UI, and the diagnostics page. The
  previous wording in `docs/provider-guidelines.md`
  § 4.4 conflated registration with enablement and
  was silent on selection; the new vocabulary
  separates the four events and the resulting
  states.
- A provider that is registered but not enabled
  is invisible to the UI; a provider that is
  enabled but not selected is the only one the UI
  lists; a provider that is selected but unhealthy
  produces a `ProviderResult<T>.Failure` that the
  application layer translates into an
  `AppErrorState`. The states make the user-facing
  affordances precise: a "disabled" toggle removes
  the configuration section; a "test connection"
  button triggers a health check; a runtime picker
  lists only enabled providers, sorted by
  `Healthy` first.
- The four architecture tests are part of the
  composition-root rule. They are added to the
  progressive self-dogfooding matrix in
  `ROADMAP.md` § 4 as M4-D rows. The
  matrix-level enforcement of the rule is
  recorded alongside the rule.
- A reviewer who approves a change that bypasses
  the composition root must cite the ADR that
  justifies the bypass. No silent exceptions.

**Supersedes:** ADR-011 (the composition-root
clause only; the rest of ADR-011 stands).
**Superseded by:** None.

---

## ADR-012 — Adopt capability-oriented provider families

**Status:** Accepted

**Context:** The current provider documentation grouped the
integrations into three vague families — `Assistant`
(Treehouse, No Mistakes), `Deployment` (Lavish Axi, GNHF), and
`Internal` (Firstmate) — and one precise family per shape
(`Runtime`, `Source`, `Terminal`, `Workspace`). The vague names
did not represent a capability; they represented a vague sense
of "things we built". A consumer of the provider model cannot
tell from the family name what the provider does.

A capability-oriented naming scheme is the only way the provider
model survives the addition of new tools. A new tool belongs
to a family when the family describes a real capability the
tool shares with other tools. A family is created only when
the capability is shared.

**Decision:** Replace `Assistant`, `Deployment`, and `Internal`
with capability-oriented families. The new family contracts are:

| Capability               | Contract                  | Implementations                                                                                  |
| ------------------------ | ------------------------- | ------------------------------------------------------------------------------------------------ |
| Agent runtime            | `IAgentRuntimeProvider`   | Ollama, Claude, OpenAI, Codex, custom command runtimes                                           |
| Source control           | `IGitProvider`            | Git                                                                                              |
| Terminal execution       | `ITerminalProvider`       | PowerShell, Windows Terminal, WSL, optional Git Bash                                             |
| Worktree                 | `IWorktreeProvider`       | Native Git worktree, Treehouse                                                                   |
| Quality gate             | `IQualityGateProvider`    | No Mistakes, built-in pass-through or disabled provider                                          |
| Review                   | `IReviewProvider`         | Native HTML review, Lavish Axi                                                                   |
| Autonomous loop          | `IAutonomousLoopProvider` | GNHF                                                                                             |
| Orchestration            | `IOrchestrationProvider`  | Native orchestration, Firstmate through WSL                                                      |

The base `IProvider` contract is retained as a small metadata
contract. Every family contract inherits from it. The base
contract provides the four members the registry, the health
poller, the configuration UI, and the diagnostics page all
consume uniformly:

- `ProviderId Id`
- `string DisplayName`
- `Task<ProviderDescriptor> DescribeAsync(CancellationToken)`
- `Task<ProviderHealth> HealthAsync(CancellationToken)`
- `Task ConfigureAsync(ProviderConfiguration, CancellationToken)`

Removing `IProvider` would force the registry to use reflection
or `dynamic` to access these members; keeping it preserves the
uniform shape that makes the platform's health, configuration,
and discovery surfaces possible.

A new family is created **only** when at least two providers
would share it. A family with one provider is not a family; it
is a one-off, and the provider is filed under the closest
existing family or a new contract is filed as an ADR for review.

The mappings are explicit:

- **Treehouse** is a **worktree** provider, not an assistant.
  Its capability is creating and managing isolated development
  worktrees.
- **No Mistakes** is a **quality gate** provider, not an
  assistant. Its capability is running a quality gate against
  a branch and reporting pass/fail.
- **Lavish Axi** is a **review** provider, not a deployment
  tool. Its capability is reviewing an artefact (a UI
  surface, a diff, a plan) and producing findings.
- **GNHF** is an **autonomous loop** provider, not a
  deployment tool. Its capability is bounded autonomous
  iteration against a defined task.
- **Firstmate** is an **orchestration** provider, not an
  internal utility. Its capability is multi-agent
  orchestration, accessed through WSL.
- **Ollama, Claude, OpenAI, Codex** are **agent runtime**
  providers. Their capability is executing an agent model
  against a prompt.
- **Git** is a **source control** provider.
- **PowerShell, Windows Terminal, WSL, Git Bash** are
  **terminal execution** providers.
- **Native Git worktree** is a **worktree** provider alongside
  Treehouse.

The native implementations (native Git worktree, native HTML
review, native orchestration, built-in quality gate) exist so
the platform can ship a complete, working experience without
requiring the external tool to be present. The external tool,
when present, replaces or augments the native implementation.

**Consequences:**

- The provider registry surface in the UI is reorganised by
  capability, not by the old vague families.
- `IRuntimeProvider` and `ISourceProvider` are renamed to
  `IAgentRuntimeProvider` and `IGitProvider`. The old names are
  not aliased; this is a clean rename. Any first-party code
  that referenced the old names is updated in the same change.
- `IWorkspaceProvider` is removed. Workspace state is
  application-layer state, not a provider capability. The
  workspace store and the project records live in the
  application and domain layers.
- The provider contracts are part of the platform's public API.
  A future change to a contract name is a breaking change and
  is recorded as an ADR.

**Supersedes:** None.
**Superseded by:** None.

---

## ADR-013 — Adopt progressive self-dogfooding of platform capabilities

**Status:** Accepted

**Context:** A long-lived platform accumulates abstractions
because earlier abstractions are needed by later features. The
discipline that makes the abstractions actually used is **the
rule that later milestones must consume the abstractions
delivered by earlier milestones**, not bypass them with
temporary direct implementations.

Without this rule, the platform grows a parallel, ad-hoc
implementation of every abstraction alongside the
"production" abstraction. The two implementations drift. The
abstraction rots because no later code exercises it.

**Decision:** Every milestone must consume the stable reusable
capabilities delivered by earlier milestones. Later
milestones must not bypass earlier platform abstractions with
temporary direct implementations.

The principle is recorded as Rule 14 in `AGENTS.md`. A
**progressive self-dogfooding matrix** is maintained in
`ROADMAP.md` and records, for every milestone, the
capabilities it delivers, the later milestones that must use
them, the prohibited direct bypass, the validation, and the
enforcing test.

The matrix is not a list of aspirational guidelines. Each
row is enforced by a specific architecture test, contract
test, or convention that fails the build when violated.

External-tool dogfooding (the development team uses an
external tool manually while building the platform) is
**separate** from platform self-dogfooding (later platform
milestones use earlier platform abstractions). The two are
governed by separate workflows:

- External-tool dogfooding: `.ai/workflows/tool-dogfooding.md`
  and the dogfooding checkpoints in `ROADMAP.md`.
- Platform self-dogfooding: the matrix in `ROADMAP.md` and
  the architecture tests in
  `AiEng.Platform.ArchitectureTests`.

Both require explicit user approval where commands or
repository changes are involved.

**Consequences:**

- The matrix is part of the definition of done for every
  milestone. A milestone is not done until the matrix row is
  satisfied.
- An architecture test that fails the build on a bypass is a
  release blocker.
- A reviewer who approves a change that bypasses an earlier
  abstraction must cite the ADR that justifies the bypass.
  No silent exceptions.

**Supersedes:** None.
**Superseded by:** None.

---

## ADR-014 — Make the four-state rule conditional on data ownership

**Status:** Accepted

**Context:** The design-system rules
(`docs/design-system.md` § 5.4, `docs/component-guidelines.md`
§ 4, `STYLEGUIDE.md` § 4.3, `.ai/prompts/ui.md` § 4 and § 6)
historically stated that every component exposes `Loading`,
`Empty`, and `Error` slots. The rule was absolute. In
practice, pure primitives (`AppButton`, `AppBadge`,
`AppStatusDot`, `AppIcon`, `AppTooltip`) and presentational
containers (`AppCard`, `AppSection`, `AppDialog`,
`AppDrawer`, `AppTabs`, `AppPanel`, `AppToolbar`) do not
own a data fetch and have no business exposing data-fetch
state slots. Forcing the rule on them produces
over-engineered components that lie about their
responsibility and pollute the API surface with slots
that are never used.

The platform's data-fetching concern belongs to the
parent that owns the fetch — typically a list page, a
project list, a run history, a session list. The parent
renders the four states and composes the primitive or
container inside the appropriate slot. The primitive or
container is pure: it renders whatever the parent gives
it.

**Decision:** Replace the absolute four-state rule with a
**conditional** rule:

- **Data-owning components** (components that fetch or own
  a data set) expose four child content slots:
  `Loading`, `Empty`, `Error`, `Populated`. The default
  body rendering is the populated state.
- **Pure primitives** and **presentational containers**
  expose the slots they have (`Header`, `Footer`,
  `Actions`, `Leading`, `Trailing`, etc.) and do not
  expose the four data-fetching slots.

The catalogue in `docs/design-system.md` § 4 marks each
entry as data-owning, primitive, or container so the rule
can be applied to the right component. A data-owning
component without the four slots is incomplete; a
primitive or container that nevertheless exposes them is
over-engineered. Both are rejected in review.

The rule is operationalised in:

- `docs/design-system.md` § 5.4 (the four-state rule is
  conditional on data ownership).
- `docs/component-guidelines.md` § 4 and § 4.3 (the
  four-answer contract is answered by every component,
  but the four state slots are conditional on data
  ownership; the authoring checklist records both
  conditions).
- `docs/ui-principles.md` § 8 (the four states apply to
  data-owning surfaces).
- `STYLEGUIDE.md` § 4.3 (markup rule).
- `.ai/prompts/ui.md` § 4 and § 6 (planning and
  implementation boundaries).

**Consequences:**

- A reviewer can verify the four-state rule was applied
  to the right component by checking the catalogue's
  classification.
- The `AppEmptyState` and `AppErrorState` components
  introduced in M1 are the fallback surfaces that
  data-owning components use. They are primitives, not
  data-owning components, and do not themselves expose
  the four state slots.
- The platform's pre-M1 design-system documentation
  has been updated to reflect the conditional rule. No
  code changes are part of this ADR; the change is to
  the rules the code will follow when it lands.

**Supersedes:** None.
**Superseded by:** None.

---

## ADR-015 — Version the design-system catalogue by implementation status

**Status:** Accepted

**Context:** The previous design-system rule
(`docs/design-system.md` § 4) was that the catalogue is
"append-only". The rule conflated two distinct kinds of
entry: **implemented** components (present in the
codebase, with a public API that other code depends on)
and **planned** components (entries the team has agreed
to build at some point, but which are not yet in the
codebase). An append-only rule that applies to both
treats a planned entry as if it were a public surface,
which produces three problems:

- A planned entry that turns out to be unnecessary
  cannot be removed without an ADR, even though no code
  depends on it.
- A planned entry that is renamed as the work that
  introduces it approaches cannot be renamed without an
  ADR, even though no code depends on the old name.
- The catalogue is polluted with entries that are not
  public surface; reviewers cannot tell the difference
  between "this is part of the platform" and "we plan
  to build this someday".

**Decision:** The catalogue distinguishes two kinds of
entry, and the two are governed by different rules:

- **Implemented entries** are present in the codebase.
  A rename, a removal, or a breaking API change to an
  implemented entry is a public-surface change. The
  design-system version is bumped (per `docs/design-system.md`
  § 10) and the change is recorded as an ADR.
- **Planned entries** are entries the team has agreed to
  build at some point, but which are not yet in the
  codebase. A planned entry may be renamed, removed, or
  re-scoped without an ADR as the work that introduces
  it approaches. A planned entry is a placeholder, not a
  commitment; it is not a public surface.
- An entry that is **neither implemented nor planned** is
  rejected. The catalogue is the design system's surface;
  an entry that does not correspond to either state is
  noise.

A planned entry is added to the catalogue when the team
agrees it should exist (typically at the milestone that
introduces it, per `ROADMAP.md`). The entry moves to
**implemented** in the same change that adds the source
files under `AiEng.Platform.App/Components/...`. An
unimplemented component must not be referenced from a
page as if it existed; an unimplemented component has
no slot to bind to.

The rule is operationalised in:

- `docs/design-system.md` § 4 (catalogue has Status
  column; entries are classified Implemented or
  Planned; planned entries are revisable).
- `docs/design-system.md` § 10 (versioning rule applies
  to implemented entries; planned entries are not a
  public surface).
- `docs/component-guidelines.md` § 6 (authoring
  checklist references the catalogue's classification).

**Consequences:**

- A reviewer can verify the catalogue's claims by
  checking the codebase: a "Planned" entry has no
  matching source file; an "Implemented" entry does.
- A planned entry that is not built is not a public
  surface and may be revised or removed without an
  ADR. The platform's session model landing in M3 (not
  centred on chat) is the reason the previous
  catalogue's `AppMessageBubble`, `AppMessageList`,
  `AppPromptInput`, `AppSessionCard`, `AppTaskCard`,
  `AppTokenUsage`, `AppTimeline`, `AppFileTree`, and
  `AppCommitList` entries are removed from the planned
  list. They were planned entries for a model that did
  not ship; the entries are removed because the model
  was revised.
- An implemented entry that is renamed, removed, or
  changed in a breaking way follows the version bump
  and ADR process. A platform that has been built
  around an implemented component cannot have that
  component silently changed.

**Supersedes:** None.
**Superseded by:** None.
