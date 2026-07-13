# Projects

> **The M3 product surface definition.**
>
> A *project* is the smallest piece of state the
> platform needs to be useful on its own. A user
> registers a project by giving the platform a
> **name** and a **folder path**; the platform owns
> a `Project` entity in the application layer. The
> M3 in-memory store is the smoke test for the
> `IProjectStore` contract; the on-disk store
> lands in M4-A behind the same contract.
>
> M3 ships the **contract** (`IProjectService`,
> `IProjectStore`), the **in-memory
> implementation** of the store, the **domain
> entity** (`Project`), the **composition root**
> (`AddProjects`), and the **UI surface**
> (`AppProjectCard`, `AppProjectList`, the
> `/projects` page). M3.1 composes the M2 shell
> (sidebar, top bar, breadcrumb, page header) and
> ships the read-only list. M3.2 ships the
> registration form, the rename form, and the
> unregister confirmation; the three mutations
> reach the store through `IProjectService`. M3
> does not introduce a new shell surface.
>
> This document is the **definition** of the M3
> product surface. The implementation lives in
> `src/AiEng.Platform.App/Components/Projects/`
> and `src/AiEng.Platform.App/Components/Pages/Projects.razor`.

---

## 1. Goals

The `/projects` surface exists to:

- **Register a project.** A user names a folder
  and the platform records it. The platform owns
  the project list in the application layer.
  **Enabled in M3.2** via the
  `RegisterProjectForm` modal.
- **List registered projects.** A user sees every
  project the platform knows about, sorted by
  name. **Enabled in M3.1** via `AppProjectList`.
- **Open a project** (wired in M3.1; enabled in
  M4-A when durable storage lands).
- **Rename a project** (wired in M3.1; **enabled
  in M3.2** via the `RenameProjectForm` modal).
- **Unregister a project** (wired in M3.1;
  **enabled in M3.2** via the
  `ConfirmUnregisterProject` confirmation).
- **Be the smallest piece of state the platform
  needs to be useful.** Every later milestone
  (M4 process runner, M5 worktree, M6 launch, M7
  review, M8 orchestration) consumes the
  `IProjectService.ListAsync` output as its
  input.

---

## 2. The Project Entity

A `Project` is a pure domain type
(`src/AiEng.Platform.Domain/Projects/Project.cs`).

| Field | Type | Notes |
| --- | --- | --- |
| `Id` | `Guid` | Immutable. Set at registration time. |
| `Name` | `string` | Human label; trimmed at registration. |
| `Path` | `string` | Absolute path on the host file system. |
| `CreatedAt` | `DateTimeOffset` | Immutable. Set at registration time. |
| `LastUsedAt` | `DateTimeOffset?` | Updated by `Touch(at)`. Null until the project is opened. |

The `Project` constructor validates that `Id`,
`Name`, and `Path` are non-empty. The constructor
throws `ArgumentException` on any empty field.

---

## 3. The Contract

### 3.1 `IProjectStore`

The persistence seam (`AiEng.Platform.Application.Projects.IProjectStore`).

| Method | Signature | Notes |
| --- | --- | --- |
| `ListAsync` | `Task<IReadOnlyList<Project>>` | Returns a snapshot ordered by `Name` (ordinal, case-insensitive). |
| `GetAsync` | `Task<Project?>` | Returns the project by `Id`; `null` when absent. |
| `AddAsync` | `Task` | Throws `InvalidOperationException` on a duplicate `Id`. |
| `UpdateAsync` | `Task` | Overwrites the stored project by `Id`. |
| `RemoveAsync` | `Task` | No-op when the `Id` is absent. |

### 3.2 `IProjectService`

The application-layer facade
(`AiEng.Platform.Application.Projects.IProjectService`).

| Method | Signature | Returns |
| --- | --- | --- |
| `RegisterAsync` | `(string name, string path, CancellationToken)` | `Result<Project>` — `Success` on a new project, `Failure(ValidationError)` on validation failure. |
| `ListAsync` | `(CancellationToken)` | `IReadOnlyList<Project>` snapshot. |
| `GetAsync` | `(Guid id, CancellationToken)` | `Project?` |
| `RenameAsync` | `(Guid id, string newName, CancellationToken)` | `Result<Project>` — `Success` on rename, `Failure(ValidationError("not_found", ...))` on a missing project, `Failure(ValidationError("required", ...))` on an empty new name. |
| `UnregisterAsync` | `(Guid id, CancellationToken)` | `Result<Project>` — `Success` on removal, `Failure(ValidationError("not_found", ...))` on a missing project. |

### 3.3 Validation Rules

- `name` must be non-empty after trimming
  (otherwise `ValidationError("required", "name is required.")`).
- `path` must be non-empty after trimming
  (otherwise `ValidationError("required", "path is required.")`).
- `path` must point to an existing directory
  (otherwise `ValidationError("invalid_path", "path is not an existing directory: {path}.")`).

---

## 4. The M3 / M4-A Boundary

The M3 in-memory store (`InMemoryProjectStore`)
**was the smoke test for the contract**. M4-A.1
(delivered 2026-07-11) shipped the on-disk
`IProjectStore` implementation
(`JsonFileProjectStore`) behind the **same
contract**; the M3 / M4-A boundary was the
**contract, not the storage medium**.

The M4-A.1 slice delivered:

- The new `AiEng.Platform.Infrastructure`
  csproj (the infrastructure seam every later
  milestone composes).
- The `IProcessRunner`, `ICredentialVault`,
  `IPlatformInfo` contracts in
  `Application/Infrastructure/`.
- The `SystemProcessRunner`,
  `WindowsCredentialVault`, `SystemPlatformInfo`,
  `JsonFileProjectStore` implementations in the
  Infrastructure csproj.
- The `AddInfrastructure` composition root
  extension; the `IProjectStore` registration
  moved from `AddProjects` to `AddInfrastructure`
  (the on-disk store is now the production
  registration; the in-memory store is preserved
  as a test fixture in
  `tests/AiEng.Platform.UnitTests/Infrastructure/InMemoryProjectStore.cs`).
- 50+ new tests; 2 registered-but-disabled
  architecture tests
  (`Infrastructure_Respects_ProcessBoundary`,
  `Infrastructure_Respects_CredentialBoundary`).

The Open action on `AppProjectCard` remains
**disabled in M4-A.1** (the Open action is
M4-A.2's responsibility; per the M3 retrospective
§ 13 recommendation 6). The on-disk store is
durable: the project list persists across an
application restart.

See `docs/infrastructure.md` for the M4-A.1
architecture documentation.

---

## 5. The UI Surface

### 5.1 `AppProjectCard`

A presentational container that composes `AppCard`
+ `AppStack` + `AppBadge` + `AppButton`. The card
shows the project name, the path, the created
timestamp, the last-used timestamp (when present),
and three action buttons: **Open**, **Rename**,
**Unregister**.

The card does **not** own state. The card is a
pure render of the `Project` parameter.

The three action buttons are **disabled** for
the Open action in M3.2 (lands in M4-A). The
**Rename** and **Unregister** buttons are
**enabled in M3.2**; the Open button remains
disabled (M4-A's responsibility). All three
buttons are wired to the seam today.

The status badge is **New** (`AppBadgeVariant.Neutral`)
when `LastUsedAt` is `null`; **Active**
(`AppBadgeVariant.Success`) when `LastUsedAt` is
present.

### 5.2 `AppProjectList`

A data-owning list of `AppProjectCard`s. The list
exposes the four state slots per the design system
rule:

| State | DOM marker | Renders |
| --- | --- | --- |
| `Loading` | `data-state="loading"` | `<AppLoading>` |
| `Empty` | `data-state="empty"` | `<AppEmptyState>` |
| `Error` | `data-state="error"` | `<AppErrorState ErrorCode="m3.load_failed" />` |
| `Populated` | `data-state="populated"` | The list of `<AppProjectCard>`s |

The list consumes `IProjectService` through
constructor injection. The architecture test
`Pages_Resolve_Projects_Through_Service` enforces
the single-seam rule.

### 5.3 The `/projects` Page

The page composes `AppPageHeader` + `AppBreadcrumb`
(M2.3) + `AppProjectList`. The page is a routed
entry point; the `[RouteMetadata]` attribute on
`Projects.razor` registers `/projects` in the M2.2
`INavigationRegistry`. The sidebar entry appears
between `Dashboard` and `Design system` per the M2
sidebar ordering (M2.2 sets the order: `Order = 1`).

The page header has a **Register a project**
action button that is **enabled in M3.2** via
the `RegisterProjectForm` modal. The button is
wired to the seam today; the modal opens when
the user clicks.

---

## 6. Composition Root

`ProjectsServiceCollectionExtensions.AddProjects`
registers:

- `IProjectStore` → `InMemoryProjectStore` (singleton)
- `IProjectService` → `ProjectService` (singleton)

`AddProjects` is called from
`ServiceCollectionExtensions.AddPlatformServices`
(the M2.1 composition root) per the M2 pattern.

---

## 7. Tests

### 7.1 Unit Tests

- `IProjectServiceTests` — validation, success +
  failure paths, constructor argument-null check,
  the `Project` constructor + `Rename` + `Touch`
  rules.
- `InMemoryProjectStoreTests` — round-trip,
  ordering, concurrency safety, error paths.

### 7.2 Component Tests

- `AppProjectCardTests` — primary render, every
  badge variant (New + Active), every action
  button (Open disabled, Rename + Unregister
  enabled in M3.2), the click handlers
  (`OnRename`, `OnUnregister`).
- `AppProjectListTests` — every state slot
  (Loading, Empty, Error, Populated), modal
  openings (Register, Rename, Unregister),
  refresh-on-mutation (`RefreshAsync`).
- `RegisterProjectFormTests` — every form state
  (idle, submitting, success, validation
  error); the form is hidden when `Visible` is
  false.
- `RenameProjectFormTests` — pre-fill of the
  current name, the new-name-must-differ
  rule, success + not-found paths.
- `ConfirmUnregisterProjectTests` — the
  confirmation prompt, the cancel + confirm
  actions, the not-found error path.
- `ProjectsPageTests` — page header, breadcrumb
  integration, sidebar registration, the
  **enabled** Register button (M3.2), the
  register-modal open path.

### 7.3 Architecture Tests

- `Pages_Resolve_Projects_Through_Service` —
  asserts the `/projects` page, the
  `AppProjectList`, the `RegisterProjectForm`,
  the `RenameProjectForm`, and the
  `ConfirmUnregisterProject` modal all consume
  `IProjectService` through the contract, not
  through direct access to `InMemoryProjectStore`
  or the file system.

### 7.4 Disabled Tests

The M3 surface does not activate the axe-core
audit or the provider-boundary tests; those
remain registered-but-disabled per ADR-016 / M4-D.

---

## 8. Out of Scope (M3)

- The on-disk `IProjectStore` (M4-A).
- The Open action — the durable store replaces
  the in-memory store, and the platform can
  resolve a process runner against the path
  (M4-A).
- Providers, processes, worktrees, runs, reviews,
  quality gates, autonomous loops, orchestration
  (M4–M8).

---

## 9. Acceptance Criteria

The M3 surface is **Done** when:

- The `Project` domain entity lives at
  `src/AiEng.Platform.Domain/Projects/Project.cs`.
- The `IProjectStore` and `IProjectService`
  contracts live at
  `src/AiEng.Platform.Application/Projects/`.
- The `InMemoryProjectStore` round-trips
  projects.
- The `ProjectsServiceCollectionExtensions.AddProjects`
  composition root is wired into
  `AddPlatformServices`.
- The `AppProjectCard` and `AppProjectList`
  components live at
  `src/AiEng.Platform.App/Components/Projects/`.
- The `/projects` page lives at
  `src/AiEng.Platform.App/Components/Pages/Projects.razor`.
- The `[RouteMetadata]` on `Projects.razor`
  registers the page in the M2.2
  `INavigationRegistry`.
- The `Pages_Resolve_Projects_Through_Service`
  architecture test is active and green.
- The unit + component tests pass.
- `npm run css:build`, `dotnet restore`,
  `dotnet build`, `dotnet test`, and
  `dotnet format --verify-no-changes` all exit 0.
- The visual smoke renders the `/projects`
  route with the empty state when no projects are
  registered, and the populated state when at
  least one project is registered.
