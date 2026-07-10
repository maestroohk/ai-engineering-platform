# docs/folder-structure.md

> The folder layout of the AI Engineering Platform. This document
> describes **what lives where and why**. Read it after
> `AGENTS.md` and `ARCHITECTURE.md`.

---

## 1. The Principle

Every folder in this repository has a single, documented
responsibility. Files belong in the folder that matches the
responsibility they serve, not the folder that is closest to where
they are imported from.

`Shared/` is not a junk drawer. It is a tightly-scoped location for
components and utilities that are genuinely shared across the
entire application. If a thing is shared by **two** areas, it does
not belong in `Shared/` until it is shared by **all** areas.

---

## 2. Top-Level Layout

```
/
├── AGENTS.md
├── ARCHITECTURE.md
├── CONTRIBUTING.md
├── DECISIONS.md
├── README.md
├── ROADMAP.md
├── STYLEGUIDE.md
├── tailwind.config.js
├── docs/
├── .ai/
├── src/
│   ├── AiEng.Platform.App/
│   │   ├── AiEng.Platform.App.csproj
│   │   ├── Program.cs
│   │   ├── App.razor
│   │   ├── _Imports.razor
│   │   ├── wwwroot/
│   │   │   ├── css/
│   │   │   │   └── app.css           # compiled by tailwind.config.js
│   │   │   ├── icons/
│   │   │   └── images/
│   │   ├── Components/
│   │   ├── Composition/              # the composition root (per ADR-016):
│   │   │                              #   the only folder that may reference
│   │   │                              #   Providers.<X> projects directly
│   │   ├── Dialogs/
│   │   ├── Layouts/
│   │   ├── Pages/
│   │   ├── Navigation/
│   │   └── Configuration/
│   ├── AiEng.Platform.Application/
│   │   ├── AiEng.Platform.Application.csproj
│   │   ├── Services/
│   │   ├── ViewModels/
│   │   ├── Dtos/
│   │   └── Models/
│   ├── AiEng.Platform.Domain/
│   │   ├── AiEng.Platform.Domain.csproj
│   │   ├── Primitives/
│   │   ├── ValueObjects/
│   │   └── Entities/                  # Project, Session, Run, Message
│   ├── AiEng.Platform.Providers.Abstractions/
│       ├── AiEng.Platform.Providers.Abstractions.csproj
│       ├── IProvider.cs
│       ├── ProviderId.cs
│       ├── ProviderResult.cs
│       ├── ProviderHealth.cs
│       ├── AgentRuntime/              # IAgentRuntimeProvider
│       ├── Git/                       # IGitProvider
│       ├── Terminal/                  # ITerminalProvider
│       ├── Worktree/                  # IWorktreeProvider
│       ├── QualityGate/               # IQualityGateProvider
│       ├── Review/                    # IReviewProvider
│       ├── AutonomousLoop/            # IAutonomousLoopProvider
│       └── Orchestration/             # IOrchestrationProvider
├── tests/
│   ├── AiEng.Platform.UnitTests/
│   │   ├── Application/
│   │   └── Domain/
│   ├── AiEng.Platform.ComponentTests/
│   │   └── Components/                # bUnit
│   └── AiEng.Platform.ArchitectureTests/
│       ├── Boundaries/                # NetArchTest project-reference rules
│       ├── Conventions/               # namespace, folder, naming
│       └── SelfDogfooding/            # progressive self-dogfooding matrix
└── AiEng.Platform.sln
```

The M1 project set is the four source projects plus the three
test projects. **Two projects that the architecture will
eventually need are deferred** (per ADR-011):

- `AiEng.Platform.Infrastructure` is created in **M4** when
  the first infrastructure abstraction
  (`IProcessRunner`, `ICredentialVault`, `IClock`, or
  persistence) is consumed.
- `AiEng.Platform.ProviderContractTests` is created in **M4**
  when the first concrete provider
  (`GitProvider` and `OllamaLaunchProvider`) lands.

Provider implementation projects
(`AiEng.Platform.Providers.<X>`) and surface projects
(`AiEng.Platform.Cli`, `AiEng.Platform.PowerShellHost`) are
also deferred to the milestone that implements them; they are
listed in ADR-011.

The detailed responsibilities are below.

---

## 2.1 The `.ai/` Folder

The `.ai/` folder lives at the repository root and contains
**operational workflows, prompts, and templates for AI-assisted
development**. It is the only canonical location for AI
collaboration documents. The folder has a single responsibility:

- **Operationalise `AGENTS.md` and the standards in `docs/` for
  AI-assisted sessions.** It tells an AI *how* to perform work
  while complying with the constitution. It does not modify,
  duplicate, or override either.

The folder has three sub-folders, each with its own
responsibility:

- **`.ai/prompts/`** — task-type templates (`bootstrap.md`,
  `feature.md`, `bugfix.md`, `refactor.md`, `review.md`,
  `architecture.md`, `ui.md`, `testing.md`, `provider.md`,
  `release.md`). One prompt per task type. Each prompt begins
  with the precedence statement from `.ai/README.md`.
- **`.ai/workflows/`** — multi-step operating procedures that
  sequence the work the prompts describe
  (`feature-lifecycle.md`, `ui-design-review.md`,
  `provider-onboarding.md`, `tool-dogfooding.md`,
  `documentation-update.md`, `release-checklist.md`).
- **`.ai/templates/`** — reusable document templates filled in
  per session (`task-brief.md`, `implementation-plan.md`,
  `implementation-report.md`, `review-report.md`,
  `session-handoff.md`).

The precedence hierarchy is:

1. `AGENTS.md`
2. `DECISIONS.md`
3. `ARCHITECTURE.md` and `STYLEGUIDE.md`
4. `docs/`
5. `.ai/workflows/`
6. `.ai/prompts/`
7. Individual task instructions

A prompt that conflicts with `AGENTS.md` is a bug in the
prompt, not an exception to the rule. The conflict is filed
as an ADR before the prompt is treated as authoritative.

The `.ai/` folder is **not**:

- A location for application source code.
- A location for provider implementations.
- A location for tests.
- A location for runtime artefacts, logs, caches, secrets,
  credentials, or personal data.
- A free-form scratch space. Every file in `.ai/` is
  reviewed like any other change.

## 3. Source Folders

Source folders are organised by **project**. The project is the
primary unit of compilation; the folder is the primary unit of
navigation. Every project has one root folder named after the
project, and every source file lives under that root. The
project map is the canonical source of truth; this section
documents the folder responsibilities **per project**.

### 3.0 Project Map (per ADR-011)

| Project                                  | Responsibility                                                  | Milestone   |
| ---------------------------------------- | --------------------------------------------------------------- | ----------- |
| `AiEng.Platform.App`                     | Presentation: pages, components, layouts, dialogs, navigation. The only place Blazor lives in M1. | M1          |
| `AiEng.Platform.Application`             | Application services, view models, DTOs. The orchestration layer. | M1          |
| `AiEng.Platform.Domain`                  | Pure domain types, value objects, entities. No external dependencies. | M1          |
| `AiEng.Platform.Infrastructure`          | Cross-cutting infrastructure: logging, time, persistence, `IProcessRunner`, `ICredentialVault`. | M4 (deferred from M1) |
| `AiEng.Platform.Providers.Abstractions`  | The `IProvider` base contract, `ProviderResult`, and every capability-oriented family contract. | M1          |
| `AiEng.Platform.Providers.<X>`           | One project per provider implementation. Created when the provider is implemented, not before. | per `ROADMAP.md` (deferred) |
| `AiEng.Platform.ProviderContractTests`   | Contract tests for every provider implementation, sharing a base test class per family. | M4 (deferred from M1) |

The M1 project set is the four source projects in the leftmost
column whose milestone is **M1**, plus the three test projects
listed under § 4. `Infrastructure` and `ProviderContractTests`
are deliberately absent from M1; they are created when their
first consumer or first test target lands. A project that
ships without a consumer or a target is a speculative project;
speculative projects are rejected.

### 3.1 `AiEng.Platform.App/Components/`

Reusable UI components. Sub-categorised:

- `Components/Primitive/` — atoms (`AppButton`, `AppBadge`).
- `Components/Container/` — structures (`AppCard`, `AppDialog`).
- `Components/Domain/` — domain-specific reusable
  (`AppProviderCard`, `AppMessageBubble`).
- `Components/Forms/` — form controls (`AppTextField`,
  `AppSelect`).
- `Components/Feedback/` — loading, empty, error, toast.
- `Components/Navigation/` — sidebar, topbar, breadcrumb.

A component is filed in the most specific sub-folder that matches
its role. If a component does not fit any sub-folder, it is filed
in `Components/` directly and a new sub-folder is considered.

### 3.2 `AiEng.Platform.App/Dialogs/`

Modal dialogs that wrap a domain operation. A dialog composes
design-system components; it is not a new component type.

- `Dialogs/Confirm/`
- `Dialogs/Provider/`
- `Dialogs/Workspace/`

Dialogs that wrap a single component may live alongside the
component. Dialogs that orchestrate multiple components live in
`Dialogs/`.

### 3.3 `AiEng.Platform.App/Layouts/`

Application-wide layout components.

- `Layouts/AppLayout.razor` — main shell.
- `Layouts/AppEmptyLayout.razor` — chrome-free shell for
  onboarding, login, splash.

A layout is the outermost wrapper of a page. Pages declare which
layout they use via `@layout`.

### 3.4 `AiEng.Platform.App/Pages/`

Route-bound components. Sub-foldered by feature area.

- `Pages/Dashboard/`
- `Pages/Workspace/`
- `Pages/Session/`
- `Pages/Providers/`
- `Pages/Settings/`
- `Pages/Diagnostics/`

A page is the **only** place that calls services. Pages compose
components; they do not contain presentation logic beyond
orchestration.

### 3.5 `AiEng.Platform.Application/Services/`

Application services that coordinate work.

- `Services/Workspace/`
- `Services/Session/`
- `Services/Conversation/`
- `Services/Provider/`

A service is a class that implements a contract from the provider
or application layer, or it is the implementation of an internal
contract defined in `Services/<Area>/Contracts/`.

### 3.6 `AiEng.Platform.Application/Dtos/`

Cross-layer data transfer objects. DTOs are records, immutable,
and behaviour-free. They are the only types that cross a layer
boundary.

### 3.7 `AiEng.Platform.Domain/`

Domain types. Sub-categorised:

- `Domain/Primitives/` — strongly-typed ids, durations,
  enumerations, value objects.
- `Domain/ValueObjects/` — composite value types (paths,
  identifiers, ranges).
- `Domain/Entities/` — entities with identity (Project,
  Session, Run, Message).

A model represents a real concept in the platform. Models
are records or immutable classes. The Domain project has
**no external dependencies** — it is the contract the
platform makes with itself.

### 3.8 `AiEng.Platform.Application/ViewModels/`

State containers owned by a single page or a small group of pages.
A view-model is the page's local model of the world; it is not
shared globally.

### 3.9 `AiEng.Platform.Providers.<X>/`

Each provider project is created when the provider is
implemented. The families are capability-oriented (per
ADR-012):

```
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
```

Each provider folder contains:

- `<Provider>Provider.cs` — implementation of the contract.
- `<Provider>Options.cs` — strongly-typed configuration.
- `<Provider>Client.cs` — HTTP / SDK wrapper (private).
- `<Provider>ContractTests.cs` — tests proving the implementation
  satisfies the contract. Lives in the test project
  `AiEng.Platform.ProviderContractTests`, not in the
  provider project.

A provider project is added to the solution only when the
provider is implemented. A speculative project is rejected.

### 3.10 `AiEng.Platform.Infrastructure/`

Cross-cutting infrastructure. This project is **deferred to M4**
(per ADR-011) and does not exist in M1. When it lands, it
contains:

- `Infrastructure/Logging/`
- `Infrastructure/Persistence/`
- `Infrastructure/Process/` — the only project in the solution
  that may call `Process.Start`. The `IProcessRunner` contract
  is defined here and consumed by everyone.
- `Infrastructure/Time/`
- `Infrastructure/Credentials/`

Infrastructure never references UI. UI never references
infrastructure. They meet at the service layer.

### 3.11 `AiEng.Platform.Application/Helpers/`

Pure-function utilities. A helper has no state, no I/O, and no
dependencies. If a helper grows state, it becomes a service. If
a helper grows dependencies, it becomes a service.

### 3.12 `AiEng.Platform.Application/Extensions/`

Extension methods grouped by the type they extend.

- `Extensions/StringExtensions.cs`
- `Extensions/TaskExtensions.cs`
- `Extensions/ServiceCollectionExtensions.cs`

A multi-method extension file is acceptable; a multi-purpose
extension file is not.

### 3.13 `AiEng.Platform.App/Navigation/`

Route definitions, sidebar item registry, breadcrumb builders.
This is where the application's URL surface is defined.

### 3.14 `AiEng.Platform.App/Configuration/`

Strongly-typed options classes and validation, scoped to the
host. Domain-level options live in the Domain project;
infrastructure-level options live in the Infrastructure
project.

- `Configuration/ProviderOptions.cs`
- `Configuration/WorkspaceOptions.cs`
- `Configuration/HostingOptions.cs`

### 3.15 `AiEng.Platform.App/Composition/`

The **composition root** (per `ARCHITECTURE.md` § 2.5 and
ADR-016). The only folder in the solution that may
reference `AiEng.Platform.Providers.<X>` projects
directly. The folder is organised by capability or by
concrete implementation:

- `Composition/Ollama/` — `OllamaServiceCollectionExtensions`
  for the Ollama Launch provider.
- `Composition/Git/` — `GitServiceCollectionExtensions` for
  the Git provider.
- `Composition/Terminal/` — registration extensions for
  the PowerShell, Windows Terminal, WSL, and Git Bash
  providers.
- `Composition/Worktree/` — registration extensions for
  the native and Treehouse worktree providers.
- `Composition/Review/` — registration extensions for the
  native and Lavish Axi review providers.
- `Composition/QualityGate/` — registration extensions
  for the native and No Mistakes quality-gate providers.

The composition root may register **any number of**
provider implementations simultaneously. The folder's
responsibility is the single, documented answer to
"where in the codebase do concrete provider
implementations get wired into the DI container, and
where does the host decide which providers are enabled
for this run". The folder is reviewed aggressively:
growth is expected (every new provider adds a file);
the rule is that no file outside this folder may
`using AiEng.Platform.Providers.<X>;`. The rule is
enforced by the
`Only_CompositionRoot_MayReference_ConcreteProviders`
architecture test in
`AiEng.Platform.ArchitectureTests`, alongside its
companion tests
`Pages_DoNotReference_ConcreteProviders`,
`Application_DoesNotReference_ConcreteProviders`, and
`Components_DoNotInject_ConcreteProviders`. The four
tests are part of the definition of done for M4-D
(`ROADMAP.md`); they are registered but disabled until
the first concrete provider implementation lands.

---

## 4. Tests Folders

```
tests/
├── AiEng.Platform.UnitTests/
│   ├── Application/        // unit tests, mirroring Services/
│   └── Domain/             // unit tests, mirroring Domain/
├── AiEng.Platform.ComponentTests/
│   └── Components/         // bUnit tests, mirroring Components/
└── AiEng.Platform.ArchitectureTests/
    ├── Boundaries/         // NetArchTest project-reference rules
    ├── Conventions/        // namespace, folder, naming
    └── SelfDogfooding/     // progressive self-dogfooding matrix
```

The three test projects are distinct, by design. Each has a single
responsibility, lives next to the production code it covers, and
ships with its own test runner configuration.

- `AiEng.Platform.UnitTests` exercises the application and domain
  layers in isolation. The `Providers/` sub-folder is added in
  M4 when the first concrete provider (`GitProvider` and
  `OllamaLaunchProvider`) has runtime dependencies to test.
- `AiEng.Platform.ComponentTests` runs the design-system and
  application components through bUnit. The folder mirrors
  `AiEng.Platform.App/Components/`.
- `AiEng.Platform.ArchitectureTests` enforces the dependency
  rules. The `SelfDogfooding/` sub-folder is the operational
  form of the matrix in `ROADMAP.md`: each milestone that
  delivers a reusable capability is paired with a test that
  fails the build if a later milestone bypasses it (for
  example, a test that fails when an `App` project file
  contains `Process.Start` once `Infrastructure` lands in M4).

The fourth test project in the previous correction,
`AiEng.Platform.ProviderContractTests`, is **deferred to M4**
(per ADR-011). Its folder layout, when it lands, will be:

```
tests/AiEng.Platform.ProviderContractTests/
└── <Family>/                // one folder per family
    └── <Provider>/          // one folder per provider
```

A broken architecture test is a release blocker.

---

## 5. `wwwroot/`

Static assets.

- `wwwroot/css/design-system/` — design-system CSS.
- `wwwroot/css/app.css` — application-level CSS (loaded after
  design-system).
- `wwwroot/icons/` — icon set.
- `wwwroot/images/` — illustrations, logos.

Static assets are not duplicated. A logo lives in one place. If
multiple sizes are needed, they are generated, not hand-maintained.

---

## 6. The `Shared/` Folder

`Shared/` exists only for things that are imported by **every**
layer of the application (e.g. global usings, an assembly
attribute, an `IClock` interface used everywhere).

It is not a place for "components I don't know where to put". If
you are tempted to put something in `Shared/`, the answer is one
of:

- A specific sub-folder under `Components/`.
- A specific service under `Services/`.
- A specific extension under `Extensions/`.

`Shared/` is reviewed aggressively. Growth in `Shared/` is a smell.

---

## 7. Co-Location

A component, its CSS, and its code-behind live together
(`AppCard.razor`, `AppCard.razor.cs`, `AppCard.razor.css`). They
are not split across folders.

A service, its contract, and its tests live together only when
the contract is private to the service. A public contract lives
in `Services/<Area>/Contracts/`.

A provider, its options, its client, and its contract tests live
together in `AiEng.Platform.Providers.<X>/`. The provider
contract interface itself lives in
`AiEng.Platform.Providers.Abstractions/<Family>/`. Contract
tests live in `AiEng.Platform.ProviderContractTests/<Family>/<Provider>/`.

---

## 8. Forbidden Locations

These are never acceptable:

- `Components/Misc/`
- `Components/Old/`
- `Components/WorkInProgress/`
- `Temp/`
- `Test/` (typo, not the test project).
- `Utils/` (use `Helpers/` and `Extensions/`).
- `Common/` (use `Shared/` only if truly shared; otherwise the
  matching domain folder).

These folders do not exist in the repository, and CI will fail if
they are created.

---

## 9. Adding a New Folder

Adding a folder is an architectural decision. The process is:

1. Open an ADR in `DECISIONS.md` describing the responsibility of
   the new folder.
2. Update this document with the new folder.
3. Update `AGENTS.md` if the new folder introduces a new rule.
4. Migrate any files that should move.

A folder is added when at least three files would naturally belong
to it. Two is not enough.

---

## 10. When the Structure Is Wrong

The structure is a tool, not a religion. If a feature genuinely
does not fit the existing taxonomy, propose a new folder through
an ADR. The review will check that the new folder has a real
responsibility, not a one-off convenience.
