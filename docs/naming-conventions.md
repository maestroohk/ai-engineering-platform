# docs/naming-conventions.md

> The naming standards for the AI Engineering Platform. This document
> is exhaustive: every artefact in the project follows these rules.
> Read it after `AGENTS.md`, `STYLEGUIDE.md`, and
> `docs/folder-structure.md`.

---

## 1. The Test of a Good Name

A good name answers three questions at the call site:

1. **What is it?** — kind (component, service, provider, model).
2. **What is it for?** — purpose.
3. **What is its context?** — area or family.

A name that requires a comment to explain has failed. The name
must communicate its intent without prose.

---

## 2. Namespaces

Namespaces are **PascalCase**, dot-separated, in the form
`<Root>.<Layer>.<Area>`.

```
AiEng.Platform
AiEng.Platform.Components.Primitive
AiEng.Platform.Components.Domain
AiEng.Platform.Services.Workspace
AiEng.Platform.Services.Session
AiEng.Platform.Providers.Ollama
AiEng.Platform.Providers.Git
AiEng.Platform.Infrastructure.Persistence
AiEng.Platform.Tests.Components
```

The root is the company/product prefix. The layer matches the
folder. The area matches the feature.

A namespace must not skip a layer. A component in
`AiEng.Platform.Components.Domain` is not imported as
`AiEng.Platform.Components`; the import is explicit.

---

## 3. Types

### 3.1 Classes

`PascalCase`. One responsibility, named in the class name.

| Concept                | Name pattern                                | Example                              |
| ---------------------- | ------------------------------------------- | ------------------------------------ |
| Page                   | `<Area>Page` or `<Purpose>Page`             | `WorkspacePage`, `DashboardPage`     |
| Reusable component     | `App<Thing>`                                | `AppButton`, `AppProviderCard`       |
| Domain model           | `<Thing>`                                   | `Workspace`, `Session`, `Run`        |
| DTO                    | `<Thing>Dto` or `<Action><Thing>Dto`        | `WorkspaceDto`, `CreateRunDto`       |
| Service                | `<Area>Service`                             | `WorkspaceService`                   |
| Service contract       | `I<Area>Service`                            | `IWorkspaceService`                  |
| Provider               | `<Provider>Provider`                        | `OllamaProvider`                     |
| Provider contract      | `I<Family>Provider`                         | `IAgentRuntimeProvider`              |
| Registry               | `<Family>ProviderRegistry`                  | `AgentRuntimeProviderRegistry`       |
| Result envelope        | `<Domain>Result`                            | `ProviderResult<T>`                  |
| Options                | `<Area>Options`                             | `WorkspaceOptions`                   |
| ViewModel              | `<Area>ViewModel`                           | `WorkspaceViewModel`                 |
| Background worker      | `<Area>Worker`                              | `SessionCleanupWorker`               |
| Hosted service         | `<Area>HostedService`                       | `ProviderWarmupHostedService`        |
| Exception              | `<What>Exception`                           | `ProviderUnavailableException`       |

### 3.2 Interfaces

`I` + `PascalCase`. The name describes the **capability**, not
the implementation.

- `IAgentRuntimeProvider`, not `IOllamaProvider`.
- `IWorkspaceService`, not `IWorkspaceManager`.
- Per ADR-012, provider families are named after the
  **capability** they offer. The eight families are
  `IAgentRuntimeProvider`, `IGitProvider`,
  `ITerminalProvider`, `IWorktreeProvider`,
  `IQualityGateProvider`, `IReviewProvider`,
  `IAutonomousLoopProvider`, and `IOrchestrationProvider`.
  Vague family names (`Assistant`, `Deployment`,
  `Internal`, `Workspace`) are rejected because they
  tell the reader nothing about what the contract
  actually does.

### 3.3 Records and Structs

Records follow the same rules as classes. A `record` is the
default for DTOs and immutable values.

### 3.4 Enums

`PascalCase` for the type, `PascalCase` for the values.

```csharp
public enum ButtonVariant
{
    Primary,
    Secondary,
    Ghost,
    Danger
}
```

Flags enums end in `Flags` (`ProviderCapabilities`). Single-value
enums end in no special suffix.

### 3.5 Delegates

`PascalCase`, verb-led. Use delegates sparingly; prefer
`EventCallback` and `Func<>` in component APIs.

---

## 4. Members

### 4.1 Methods

`PascalCase`, verb-led.

| Kind        | Convention                                  | Example                          |
| ----------- | ------------------------------------------- | -------------------------------- |
| Get         | `Get<Thing>` or simply the property         | `GetSession`                     |
| Create      | `Create<Thing>`                             | `CreateWorkspace`                |
| List        | `List<Thing>Async`                          | `ListSessionsAsync`              |
| Find        | `Find<Thing>Async`                          | `FindProviderAsync`              |
| Open / Close | `OpenAsync` / `CloseAsync`                | `OpenWorkspaceAsync`             |
| Start / Stop | `StartAsync` / `StopAsync`                | `StartRunAsync`                  |
| Send        | `SendAsync`                                 | `SendMessageAsync`               |
| Stream      | `StreamAsync` (returns `IAsyncEnumerable`)  | `StreamRunAsync`                 |
| Subscribe   | `SubscribeAsync`                            | `SubscribeEventsAsync`           |
| Health      | `HealthAsync`                               | `HealthAsync`                    |
| Describe    | `DescribeAsync`                             | `DescribeAsync`                  |

`Async` suffix is mandatory on every async method.

### 4.2 Properties

`PascalCase`, noun-led or adjective-led.

- `IsHealthy`, not `HealthStatus` (boolean).
- `DisplayName`, not `Name` (when the user sees it).
- `ProviderId`, not `Id` (when the type is a provider).
- `Items` for collections.
- Singular for one, plural for many.

### 4.3 Fields

- `private` instance fields: `_camelCase`.
- `private static` fields: `_camelCase` (still underscore).
- `public static` fields: `PascalCase` (rare; prefer properties).
- `const`: `PascalCase`.

Never expose fields as `public`. Never use Hungarian notation.

### 4.4 Parameters and Locals

`camelCase`. Names describe the value, not the type.

```csharp
public Task<Run> StartAsync(string sessionId, CancellationToken cancellationToken)
```

- `sessionId`, not `id`.
- `cancellationToken`, not `ct` (except in `private` methods
  where the scope is obvious).
- `workspacePath`, not `path`.

`i`, `j`, `k` are acceptable for indices. `x`, `y`, `z` are
acceptable for coordinates. Beyond that, use a descriptive name.

### 4.5 Type Parameters

`T` + `PascalCase`.

- `TItem`, `TResult`, `TKey`, `TValue`.
- Multi-letter only when `T` alone is genuinely ambiguous.

---

## 5. Components

Components are the most visible naming surface. Every component
follows these rules.

### 5.1 Prefix

Every reusable component is prefixed with `App`. This is the
single visual signal that "this is part of the platform's design
system".

- `AppButton`, not `Button`.
- `AppProviderCard`, not `ProviderCard`.

A page-bound component does not need the `App` prefix because it
lives in `Pages/`. Examples: `DashboardPage`, `WorkspacePage`.

### 5.2 Suffix by Kind

| Kind              | Suffix        | Example                            |
| ----------------- | ------------- | ---------------------------------- |
| Primitive         | (none)        | `AppButton`, `AppBadge`            |
| Container         | (none)        | `AppCard`, `AppDialog`             |
| Domain            | `Card`        | `AppProviderCard`, `AppTaskCard`   |
| Page              | `Page`        | `WorkspacePage`                    |
| Layout            | `Layout`      | `AppLayout`                        |
| Sidebar item      | `Item`        | `AppSidebarItem`                   |

A domain component is named for the **concept** it represents,
not for the page it lives on. `AppProviderCard` is reused on
diagnostics, settings, and the home page.

### 5.3 Variants and Sizes

- `Variant` is an enum.
- Enum name matches the component, suffixed with the variant kind.
  - `ButtonVariant`, `CardVariant`, `BadgeVariant`.
- `Size` is an enum named `<Component>Size` or simply `Size`
  inside the component.

### 5.4 Forbidden Component Names

These names are rejected in review:

- `Card2`, `Button1`, `Widget` — meaningless.
- `PanelNew`, `HeaderV2` — encodes history into the name.
- `Component` — too generic.
- `Helper` — too generic.
- `Manager` — too generic.
- `Utils` — too generic.
- `Common` — too generic.
- `My<Thing>` — never include a personal prefix.

---

## 6. Files

A file is named after its **primary** type, in `PascalCase`. A
secondary type in the same file (e.g. an exception) is allowed
only when the types are tightly coupled.

- `WorkspaceService.cs` contains `WorkspaceService`.
- `WorkspaceServiceTests.cs` contains `WorkspaceServiceTests`.
- `AppButton.razor`, `AppButton.razor.cs`, `AppButton.razor.css`.

A folder name is the plural or collective form of the contents.

- `Components/`, `Services/`, `Providers/`, `Models/`.
- Not `Component/`, `Service/`, `Provider/`.

---

## 7. CSS Classes

CSS classes follow two patterns:

- **Component classes:** `app-<thing>` or `app-<thing>--<variant>`.
  - `.app-button`, `.app-button--primary`, `.app-button--lg`.
- **Semantic classes:** `app-<concept>`.
  - `.app-card`, `.app-page-header`, `.app-status-dot`.

Raw Tailwind utilities do not need a naming convention; they
follow Tailwind's own. The convention is for **our** CSS.

---

## 8. Configuration and Settings

- Configuration keys are `PascalCase` in code
  (`Workspace:RootPath`).
- Settings files are `kebab-case`
  (`appsettings.Development.json`).
- Environment variables are `SCREAMING_SNAKE_CASE`
  (`AIENG_PLATFORM_LOG_LEVEL`).

---

## 9. Git and Workflow

- Branch names: `<type>/<milestone-or-area>-<short-kebab>`.
  - `feature/m3-provider-registry`
  - `bugfix/m5-cancel-token-leak`
  - `refactor/extract-message-bubble`
  - `docs/update-naming-conventions`
  - `adr/define-result-envelope`
- Commit subjects are imperative, under 72 characters.
- Commit bodies explain **why**, not what.

---

## 10. The Three Names of Every Public Surface

Every public type has three names that must agree:

- The **type name** (`AppButton`).
- The **file name** (`AppButton.razor`).
- The **CSS class** (`.app-button`).

If the three disagree, the type is renamed until they agree. The
name is the API; everything else follows.

---

## 11. When a Name Is Wrong

A name is wrong when:

- A reader cannot tell what it is from the name.
- The name is shorter than the concept.
- The name uses an abbreviation that the platform has not
  standardised.
- The name is reused for two different concepts.
- The name encodes a version (`V2`, `New`).

If a name is wrong, the name is fixed in a dedicated commit. Do
not bundle a rename with other changes.
