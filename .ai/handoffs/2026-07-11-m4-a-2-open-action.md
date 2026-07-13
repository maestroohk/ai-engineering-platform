# Handoff — M4-A.2 Open Action on AppProjectCard — `m4-a-2-open-action` (2026-07-11)

> **The M4-A.2 per-session handoff.** M4-A.2
> (T-022) is the second M4-A slice. M4-A.2
> follows M4-A.1 per the Progressive Coding
> Rule. M4-A.2 is the **first user-facing
> `IProcessRunner` activation**: the Open
> button on `AppProjectCard` (wired-but-
> disabled since M3.1; explicitly deferred
> to M4-A.2 in the M3.2 closeout Limitation 2)
> is enabled and wired to
> `IProcessRunner.RunToCompletionAsync("explorer.exe",
> new[] { Project.Path }, default)`. The
> Open action is gated on
> `IPlatformInfo.IsWindows` (the `IPlatformInfo`
> contract is extended with the `IsWindows`
> property in this slice; the M4-A.1
> `SystemPlatformInfo` implementation is
> extended with the property). The click
> handler wraps the call in `try/catch` for
> `Win32Exception` + `InvalidOperationException`
> + `IOException`; on failure, logs to
> `ILogger<AppProjectCard>` and surfaces a
> transient inline `OpenError` rendered as
> a single scoped CSS class
> `.app-project-card-open-error`. No new
> design-system `AppInlineError` primitive
> is added; the M3.2 minimum-blast-radius
> decision is preserved.
>
> The M4-A.2 ships 5 new bUnit tests + 1
> new active architecture test
> (`AppProjectCard_resolves_open_through_IProcessRunner`).
> The M3.2 slice-boundary marker test
> `Open_Button_Remains_Disabled_In_M3_2` is
> deleted and replaced with the inverse
> assertion. Test count: 323 passed, 0
> failed, 9 skipped (per ADR-016 / M4-D);
> the M4-A.2 delta is +5 bUnit + 1 new
> active architecture.
>
> The M4-A.2 stops here. The M4-A.2 brief's
> "Do not begin the following task" rule is
> preserved; the M4-A.2 does **not** begin
> M4-A.3 (not yet defined), M4-B (Capability
> Detection), M4-C (Provider Registry
> Foundation), or M4-D (First Concrete
> Process Providers). The next session is
> the M4-A.3 implementation (if defined) or
> the M4-B plan promotion.

---

## 1. What was delivered

The M4-A.2 Open action on `AppProjectCard`
(T-022) is **Done** (2026-07-11).

The M4-A.2 ships:

- **`IPlatformInfo.IsWindows` contract
  extension**
  (`src/AiEng.Platform.Application/Infrastructure/IPlatformInfo.cs`):
  the interface gains the
  `bool IsWindows { get; }` property
  (documented with an XML doc comment). The
  extension is backwards-compatible: the
  existing two methods (`GetDataDirectory`,
  `GetConfigDirectory`) are unchanged; the
  extension is a read-only property that
  consumers use to gate Windows-only
  behaviour (the Open action is the first
  consumer).

- **`SystemPlatformInfo.IsWindows`
  implementation**
  (`src/AiEng.Platform.Infrastructure/Platform/SystemPlatformInfo.cs`):
  the implementation is
  `RuntimeInformation.IsOSPlatform(OSPlatform.Windows)`
  (single-line expression-bodied property;
  the `using System.Runtime.InteropServices;`
  directive is added).

- **`AppProjectCard.razor` rewrite**
  (`src/AiEng.Platform.App/Components/Projects/AppProjectCard.razor`):
  three new `@inject` directives
  (`IProcessRunner ProcessRunner`,
  `IPlatformInfo PlatformInfo`,
  `ILogger<AppProjectCard> Logger`); the
  M3.2 `Disabled="true"` on the Open
  `AppButton` is replaced with the computed
  `Disabled="@(!IsWindowsHost)"`; the Open
  button gains a `Title` attribute bound to
  `OpenButtonTitle` (the tooltip text
  changes based on the host platform); the
  Open button gains `@onclick="OnOpenClick"`
  (the click handler); a new private
  `OnOpenClick` method calls `await
  OpenAsync()`; a new private `OpenAsync`
  method clears `OpenError` at the start,
  calls
  `ProcessRunner.RunToCompletionAsync("explorer.exe",
  new[] { Project.Path }, default)`, wraps
  the call in `try/catch` for
  `Win32Exception` +
  `InvalidOperationException` + `IOException`,
  logs to `Logger.LogError(ex, "Open action
  failed for project {ProjectId} at
  {ProjectPath}.", Project.Id, Project.Path)`
  on catch, and sets a transient `OpenError`
  string rendered inline as
  `<div class="app-project-card-open-error"
  role="alert">`; a new private `OpenError`
  state property (`string? OpenError`); a
  new private `IsWindowsHost` computed
  property (`=> PlatformInfo.IsWindows`); a
  new private `OpenButtonTitle` computed
  property (the tooltip text; Windows →
  "Open the project folder in File
  Explorer."; non-Windows → "The Open action
  is Windows-only."); the `ChildContent`
  block renders the `OpenError` when
  non-null.

- **`AppProjectCard.razor.css` scoped CSS**
  (`src/AiEng.Platform.App/Components/Projects/AppProjectCard.razor.css`):
  the `.app-project-card-open-error` scoped
  CSS class is added (small, error-coloured
  text below the meta `<dl>`; uses
  `--app-error` for the text + border-left
  color and `--app-surface-2` for the
  background). 6 lines of CSS.

- **`_Imports.razor` addition**
  (`src/AiEng.Platform.App/Components/Projects/_Imports.razor`):
  `@using AiEng.Platform.Application.Infrastructure`
  is added (to keep the card's `@using`
  minimal; the `_Imports.razor` is the
  preferred place per the existing pattern).

- **5 new bUnit tests in
  `AppProjectCardTests.cs`**
  (`tests/AiEng.Platform.ComponentTests/Projects/AppProjectCardTests.cs`):
  - `Open_Button_Is_Enabled_When_Host_Is_Windows`
  - `Open_Button_Is_Disabled_When_Host_Is_Not_Windows`
  - `Clicking_Open_Invokes_IProcessRunner_With_Explorer_And_ProjectPath`
  - `Open_Click_Passes_ProjectPath_Single_Element_As_Argument`
  - `Open_Click_Swallows_Process_Exceptions`

- **`FakeProcessRunner` and `FakePlatformInfo`
  test doubles in `AppProjectCardTests.cs`**:
  `FakeProcessRunner : IProcessRunner`
  (records `LastExecutable`, `LastArguments`,
  `RunToCompletionCallCount`; has a
  `ThrowOnRunToCompletionAsync` toggle for
  the exception test); `FakePlatformInfo :
  IPlatformInfo` (configurable `IsWindows`;
  stub `GetDataDirectory` + `GetConfigDirectory`).
  A constructor is added that registers a
  default `IPlatformInfo` (Windows) +
  `IProcessRunner` (`FakeProcessRunner`) in
  the `Services` collection; the
  constructor ensures the pre-existing tests
  pass without per-test service registration.

- **1 new active architecture test in
  `PagesResolveProjectsThroughServiceTests.cs`**
  (`tests/AiEng.Platform.ArchitectureTests/Pages/PagesResolveProjectsThroughServiceTests.cs`):
  `AppProjectCard_resolves_open_through_IProcessRunner`
  (asserts `AppProjectCard.razor` contains
  `@inject IProcessRunner` and does **not**
  contain `Process.Start` or `ProcessStartInfo`;
  mirrors the M3.2
  `AppProjectList_resolves_projects_through_IProjectService`
  pattern).

- **Deletion of the M3.2
  `Open_Button_Remains_Disabled_In_M3_2` test
  in `AppProjectCardTests.cs`** (the M3.2
  slice-boundary marker; the M4-A.2 replaces
  the assertion with the inverse
  `Open_Button_Is_Enabled_When_Host_Is_Windows`).

- **`docs/infrastructure.md` § 7 rewrite
  (and § 6, § 9, § 10 update)**: § 7 Open
  Action is rewritten from "future tense"
  to "delivered tense"; § 6 Platform Info
  is updated to document the new `IsWindows`
  property; § 9 Tests cumulative count is
  updated to 323 passed; § 10 Out of Scope
  is updated to reflect M4-A.2 delivered.

- **`docs/projects.md` § 1, § 4, § 5.1,
  § 7.2, § 7.3 update**: § 1 (Open status)
  is updated to "enabled in M4-A.2 via the
  `IProcessRunner` seam; gated on
  `IPlatformInfo.IsWindows`"; § 4 (M3 /
  M4-A Boundary) is updated to document the
  M4-A.2 delivery; § 5.1 (card description)
  is updated (all three action buttons are
  now enabled); § 7.2 (component test list)
  is updated; § 7.3 (architecture tests) is
  updated to include the new test.

- **`ROADMAP.md` and
  `.ai/plans/master-delivery-plan.md` update**:
  the M4-A row in the milestones table is
  updated to "Active (M4-A.1 + M4-A.2
  Delivered 2026-07-11)"; the M4-A.2 slice
  breakdown entry is `Delivered`; the M4-A
  DoD bullets are updated to add the Open
  action item.

- **`.ai/state/capabilities.json` C-012
  (`IProcessRunner`) update**:
  `completion_status: "Planned" → "Delivered"`;
  `delivered_by_tasks: ["T-021"] → ["T-021",
  "T-022"]`;
  `evidence.plans: [] → [".ai/plans/M4-A-infrastructure-process-execution.md"]`;
  `evidence.reports: [] → ["implementation-report-m4-a-1-infrastructure-project-skeleton.md", "implementation-report-m4-a-2-open-action.md"]`;
  `evidence.commits: [] → ["feat(m4-a.1): add infrastructure project skeleton with IProcessRunner, ICredentialVault, IPlatformInfo, and on-disk IProjectStore", "feat(m4-a.2): enable AppProjectCard.Open action via IProcessRunner"]`;
  `evidence.tests: [] → ["IProcessRunnerTests", "Open_Button_Is_Enabled_When_Host_Is_Windows", "Open_Button_Is_Disabled_When_Host_Is_Not_Windows", "Clicking_Open_Invokes_IProcessRunner_With_Explorer_And_ProjectPath", "Open_Click_Passes_ProjectPath_Single_Element_As_Argument", "Open_Click_Swallows_Process_Exceptions", "AppProjectCard_resolves_open_through_IProcessRunner"]`;
  `evidence.source_paths: [] → ["src/AiEng.Platform.Application/Infrastructure/IProcessRunner.cs", "src/AiEng.Platform.Application/Infrastructure/ProcessResult.cs", "src/AiEng.Platform.Infrastructure/ProcessRunner/SystemProcessRunner.cs", "src/AiEng.Platform.App/Composition/Infrastructure/InfrastructureServiceCollectionExtensions.cs", "src/AiEng.Platform.App/Components/Projects/AppProjectCard.razor"]`;
  `architecture_tests: ["No_DirectProcessStart_OutsideInfrastructure"] → ["No_DirectProcessStart_OutsideInfrastructure", "AppProjectCard_resolves_open_through_IProcessRunner"]`;
  `completed_criteria` updated; the
  `acceptance_criteria` extends with the
  third criterion (at least one
  `IProcessRunner` call site exists in the
  App layer).

- **Project-continuity state update per
  Rule 15**: `session.json` (M4-A.2
  envelope), `tasks.json` (T-022 → `Done`
  with the full evidence block; the next
  M4-A task is undefined; T-008 M4 summary
  note updated to "M4-A.1 (T-021) + M4-A.2
  (T-022) Delivered 2026-07-11; M4-A.3 is
  not yet defined"), `current.md` (M4-A.2
  in `Active Slice`, `Last Stable Commit`,
  `Last Completed Task`, `Last Updated`,
  `Last Implementation Report`, `Linked
  Artefacts`; the M4-A.2 details are
  spread across the snapshot), `task-board.md`
  (M4-A.2 in `Done Recently`; M4-A.3 —
  undefined — placeholder in `Ready`), and
  `milestones.json` (M4-A.2 slice block
  added; M4-A evidence block updated).

- **Validation results**: 323 passed, 0
  failed, 9 skipped (per ADR-016 / M4-D);
  0 warnings, 0 errors; format clean;
  visual smoke on `/projects` returns 200;
  the Open action is **enabled** in
  M4-A.2 (the M3.2 `Disabled="true"` is
  replaced by a computed
  `Disabled="@(!IsWindowsHost)"` which is
  `false` on Windows hosts); the visual
  smoke clicks the Open button on a
  populated `/projects` route; the project
  folder opens in File Explorer; on
  non-Windows hosts (manual verification;
  the dev environment is Windows), the
  Open button is disabled with a tooltip.

---

## 2. Architecture decisions

The M4-A.2 follows the M4-A plan's
architectural decisions and adds four
M4-A.2-specific decisions:

### 2.1 Direct `IProcessRunner` injection is the right call

The M4-A.2 introduces a direct
`IProcessRunner` injection on
`AppProjectCard`. The M4-A plan § 2 item 8
escape hatch ("the M4-A first session may
introduce a `IOpenProjectAction` seam if
the click handler grows") is **not**
exercised — the click handler is a single
`explorer.exe` invocation; the seam is
unnecessary. The direct injection is the
**minimum-blast-radius** pattern; the seam
can be introduced later if a second caller
emerges (e.g., M4-D's `GitProvider` that
needs to launch `git.exe` in a worktree
context).

### 2.2 `IPlatformInfo.IsWindows` is a platform-agnostic, testable seam

The Open action's Windows-only guard uses
`IPlatformInfo.IsWindows` (not
`OperatingSystem.IsWindows()` directly).
The bUnit tests use
`FakePlatformInfo(isWindows: true/false)`
to exercise both branches; the production
code uses the real
`SystemPlatformInfo.IsWindows`. The
abstraction is testable, the abstraction
is platform-agnostic, and the abstraction
composes with the rest of the M4-A.1
`IPlatformInfo` interface.

### 2.3 `ILogger<T>` from the BCL is the right logging seam for Blazor components

The M4-A.2 introduces `ILogger<AppProjectCard>`
for error logging on the Open action's
catch path. The BCL provides `ILogger<T>`
via the Blazor Server host's DI container;
no registration is needed. The pattern is
consistent with the .NET ecosystem; the
pattern composes with the platform's
existing logging infrastructure.

### 2.4 Transient inline error pattern is the minimum-blast-radius pattern

The M4-A.2 introduces a single scoped CSS
class `.app-project-card-open-error` (no
new design-system `AppInlineError`
primitive). The pattern is consistent with
the M3.2 minimum-blast-radius decision
("compose the existing HTML5 native
`<dialog>`; no new design-system component
is added"). The pattern is consistent with
the M1.2 design-system principle ("the
design system grows by one component at a
time; each component is justified by a real
consumer"). A future M4-A.3 (or later) may
introduce an `AppInlineError` primitive if
a second consumer emerges; the M4-A.2 does
not pre-empt that decision.

### 2.5 The M4-A.2 process boundary is the first concrete activation of the M4-A.1 seam

The M4-A.1 delivered the `IProcessRunner`
contract + the `SystemProcessRunner`
implementation + the DI registration; the
M4-A.2 activates the seam by injecting
`IProcessRunner` into `AppProjectCard` and
calling
`IProcessRunner.RunToCompletionAsync` from
the click handler. The activation is the
**evidence** that the M4-A.1 seam is
correct (the seam is correct iff a real
consumer can use it). The M4-A.2 is the
first concrete `IProcessRunner` call site
in the App layer; the M4-D `Providers.<X>`
projects are the next concrete call sites.

---

## 3. Deviations from the M4-A plan

The M4-A.2 has four documented deviations
from the M4-A plan § 2 item 8:

1. **`IProcessRunner.RunToCompletionAsync`
   argument signature.** The M4-A plan § 2
   item 8 prose example uses a single
   `string` argument:
   `RunToCompletionAsync("explorer.exe",
   $"\"{project.Path}\"")`. The actual
   M4-A.1 contract takes
   `IReadOnlyList<string>` arguments; the
   `ProcessStartInfo.ArgumentList` handles
   quoting for paths with spaces. The
   M4-A.2 uses the actual contract:
   `await ProcessRunner.RunToCompletionAsync("explorer.exe",
   new[] { Project.Path }, default)`. The
   manual quote escaping in the M4-A plan
   prose is unnecessary (and would have
   been a security vulnerability if it had
   been used).

2. **`IPlatformInfo.IsWindows` extension.**
   The M4-A plan § 8 risk row 6 anticipates
   the M4-A first session introduces an
   `IPlatformInfo.IsWindows` extension.
   The M4-A.1 left the extension out. The
   M4-A.2 delivers the extension as
   `bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows)`.
   The extension is backwards-compatible.

3. **`ILogger<AppProjectCard>` introduction.**
   The M4-A plan does not anticipate the
   App layer using `ILogger<T>`. The
   M4-A.2 introduces it for the
   error-logging path of the Open action.
   The BCL provides `ILogger<T>` via the
   Blazor Server host's DI container; no
   registration is needed. The introduction
   is a one-time event; future App
   components that need logging can use the
   same pattern.

4. **`OpenError` inline rendering.** The
   M4-A plan does not prescribe a specific
   error UX for the Open action. The M4-A.2
   introduces a transient `OpenError`
   string rendered as a small
   `<div class="app-project-card-open-error"
   role="alert">` in the card's
   `ChildContent`. The deviation is
   minimum-blast-radius; no new
   design-system primitive is added.

The M4-A.2 deviations are recorded in the
M4-A.2 implementation report § 14. The
M4-A.2 is approved as-is; the deviations
are implementation-time decisions, not
plan-level changes.

---

## 4. Test results

The M4-A.2 test results:

- **Total:** 323 passed, 0 failed, 9
  skipped (per ADR-016 / M4-D).
- **Delta vs M4-A.1 closeout:** +5 bUnit
  + 1 new active architecture (the M4-A.2
  deletes the M3.2 slice-boundary marker
  test; the M4-A.2 net is +5 bUnit
  effective).
- **Unit tests:** 79 (unchanged from
  M4-A.1).
- **Component (bUnit) tests:** 233 (was
  228; +5 from M4-A.2; −1 from the
  deleted M3.2 marker; net +5; actually
  +6 in the bUnit test file, of which
  one is the M3.2 marker deletion).
- **Architecture tests:** 12 active (was
  11; +1 from the new
  `AppProjectCard_resolves_open_through_IProcessRunner`)
  + 9 registered-but-disabled (unchanged
  from M4-A.1; the 7 M3 disabled + the 2
  M4-A.1 `Infrastructure_Respects_*`
  disabled remain registered-but-disabled
  per ADR-016 and activate in M4-D).
- **Build:** 0 warnings, 0 errors (with
  `TreatWarningsAsErrors=true` from
  `Directory.Build.props`).
- **Format:** `dotnet format
  --verify-no-changes` exits 0.
- **CSS:** `npm run css:build` exits 0;
  the new `.app-project-card-open-error`
  class is generated in the `app.css`
  bundle.

The 5 new bUnit tests are:

1. `Open_Button_Is_Enabled_When_Host_Is_Windows`
2. `Open_Button_Is_Disabled_When_Host_Is_Not_Windows`
3. `Clicking_Open_Invokes_IProcessRunner_With_Explorer_And_ProjectPath`
4. `Open_Click_Passes_ProjectPath_Single_Element_As_Argument`
5. `Open_Click_Swallows_Process_Exceptions`

The 1 new architecture test is:

- `AppProjectCard_resolves_open_through_IProcessRunner`
  (active; passes immediately; reads
  `AppProjectCard.razor` source and asserts
  `@inject IProcessRunner` is present and
  `Process.Start` + `ProcessStartInfo`
  tokens are absent).

The 1 deleted bUnit test is:

- `Open_Button_Remains_Disabled_In_M3_2`
  (the M3.2 slice-boundary marker;
  replaced with the inverse assertion
  `Open_Button_Is_Enabled_When_Host_Is_Windows`).

---

## 5. Files modified

The M4-A.2 modifies 12 source files + 7
state / documentation files = **19 files
modified**. The M4-A.2 creates 2 new files
in the repository root (the implementation
report + the per-session handoff). The
M4-A.2 does **not** delete any source
files.

### 5.1 Source files modified (5 files)

- `src/AiEng.Platform.Application/Infrastructure/IPlatformInfo.cs`
  (add `bool IsWindows { get; }`)
- `src/AiEng.Platform.Infrastructure/Platform/SystemPlatformInfo.cs`
  (implement `IsWindows` via
  `RuntimeInformation.IsOSPlatform(OSPlatform.Windows)`)
- `src/AiEng.Platform.App/Components/Projects/_Imports.razor`
  (add `@using AiEng.Platform.Application.Infrastructure`)
- `src/AiEng.Platform.App/Components/Projects/AppProjectCard.razor`
  (enable the Open button; add
  `@inject IProcessRunner` +
  `@inject IPlatformInfo` +
  `@inject ILogger<AppProjectCard>`; add
  `OnOpenClick` + `OnOpenAsync` +
  `IsWindowsHost` + `OpenButtonTitle` +
  `OpenError`; render the error inline)
- `src/AiEng.Platform.App/Components/Projects/AppProjectCard.razor.css`
  (add the `.app-project-card-open-error`
  scoped CSS class)

### 5.2 Test files modified (2 files)

- `tests/AiEng.Platform.ComponentTests/Projects/AppProjectCardTests.cs`
  (delete
  `Open_Button_Remains_Disabled_In_M3_2`;
  add 5 new M4-A.2 bUnit tests; add
  `FakeProcessRunner` + `FakePlatformInfo`
  test doubles; add a constructor that
  registers the default services)
- `tests/AiEng.Platform.ArchitectureTests/Pages/PagesResolveProjectsThroughServiceTests.cs`
  (add
  `AppProjectCard_resolves_open_through_IProcessRunner`)

### 5.3 Documentation files modified (4 files)

- `docs/infrastructure.md` (4 sections
  updated: preamble; § 6 Platform Info;
  § 7 Open Action; § 9 Tests; § 10 Out of
  Scope)
- `docs/projects.md` (5 sections updated:
  § 1, § 4, § 5.1, § 7.2, § 7.3)
- `ROADMAP.md` (2 sections updated: § 2
  milestone table; § 3 M4-A section)
- `.ai/plans/master-delivery-plan.md` (2
  sections updated: § 1 milestone table; §
  3 M4-A section)

### 5.4 State files modified (5 files)

- `.ai/state/session.json` (M4-A.2
  envelope)
- `.ai/state/tasks.json` (T-022 → `Done`
  with the full evidence block; T-008 M4
  summary note updated)
- `.ai/state/current.md` (M4-A.2 details
  in `Active Slice`, `Last Stable Commit`,
  `Last Completed Task`, `Last Updated`,
  `Last Implementation Report`, `Linked
  Artefacts`)
- `.ai/state/task-board.md` (M4-A.2 in
  `Done Recently`; M4-A.3 — undefined —
  placeholder in `Ready`)
- `.ai/state/milestones.json` (M4-A.2
  slice block added; M4-A evidence block
  updated)

### 5.5 Capabilities file modified (1 file)

- `.ai/state/capabilities.json` (C-012
  `IProcessRunner` updated to `Delivered`
  with the full evidence block; the
  M4-A.1 + M4-A.2 commit messages +
  implementation reports + handoffs +
  tests + source paths are listed)

### 5.6 Files created (2 files)

- `implementation-report-m4-a-2-open-action.md`
  (the M4-A.2 implementation report; 15+
  sections; CRLF line endings)
- `.ai/handoffs/2026-07-11-m4-a-2-open-action.md`
  (this handoff; CRLF line endings)

### 5.7 Files deleted

- The M4-A.2 deletes 1 test method
  (`Open_Button_Remains_Disabled_In_M3_2`
  in `AppProjectCardTests.cs`); the
  deletion is a slice-boundary marker; the
  M4-A.2 replaces the assertion with the
  inverse. The M4-A.2 does **not** delete
  any source files, documentation files,
  state files, or contract files.

---

## 6. Files NOT touched

- `src/AiEng.Platform.App/Components/Projects/AppProjectList.razor`
  — not modified. The card owns the Open
  click; the list does not pass a callback.
- `src/AiEng.Platform.Application/Projects/`
  — not modified. The M3 contracts are
  unchanged; no new
  `IProjectService.OpenAsync` method is
  added; no new `IOpenProjectAction` seam
  is added.
- `src/AiEng.Platform.Application/Infrastructure/IProcessRunner.cs`
  — not modified. The contract is unchanged
  (the M4-A.2 uses
  `RunToCompletionAsync(string,
  IReadOnlyList<string>,
  CancellationToken)` unchanged).
- `src/AiEng.Platform.Application/Infrastructure/ProcessResult.cs`
  — not modified. The `ProcessResult`
  contract is unchanged.
- `src/AiEng.Platform.Infrastructure/ProcessRunner/SystemProcessRunner.cs`
  — not modified. The M4-A.1 implementation
  is used unchanged; the M4-A.2 uses the
  M4-A.1 implementation as-is.
- `src/AiEng.Platform.Infrastructure/Projects/`
  — not modified.
- `src/AiEng.Platform.Infrastructure/Credentials/`
  — not modified.
- `src/AiEng.Platform.App/Composition/`
  — not modified. The M4-A.1 `AddInfrastructure`
  composition root is unchanged; no new DI
  registrations are added; `IProcessRunner`
  + `IPlatformInfo` are already registered.
- `src/AiEng.Platform.Domain/` — not
  modified. M4-A.2 does not add domain
  types.
- `src/AiEng.Platform.Providers.Abstractions/`
  — not modified. M4-A.2 does not create
  providers; the first concrete providers
  land in M4-D.
- `src/AiEng.Platform.App/Components/Common/`,
  `src/AiEng.Platform.App/Components/Primitive/`,
  `src/AiEng.Platform.App/Components/Layout/`,
  `src/AiEng.Platform.App/Components/Display/`,
  `src/AiEng.Platform.App/Components/Feedback/`,
  `src/AiEng.Platform.App/Components/Inputs/`
  — not modified. No new design-system
  components; the Open action uses the
  M1.2 `AppButton` primitive unchanged.
- `AGENTS.md`, `ARCHITECTURE.md`,
  `DECISIONS.md`, `STYLEGUIDE.md`,
  `CONTRIBUTING.md` — not modified. The
  17 non-negotiable rules are preserved.
- `.ai/workflows/` — not modified. The
  standard is preserved.
- `.ai/plans/M4-A-infrastructure-process-execution.md`
  — not modified. The M4-A plan is the
  canonical plan; the M4-A.2 implementation
  follows it.
- `.ai/plans/M3-*.md` — not modified. The
  M3 plans are unchanged.
- `tailwind.config.js`, `package.json`,
  `Directory.Build.props` — not modified.
  The CSS pipeline and the .NET build
  configuration are unchanged.

---

## 7. Git operations

The M4-A.2's git operations per the
branching strategy rules 4, 6, 7:

- **Rule 4 (branch naming):** the feature
  branch is
  `feature/T-022-m4-a-2-open-action`. The
  name encodes the task ID (T-022) + the
  milestone slice (m4-a-2) + the slice
  title (open-action).

- **Feature branch creation:** the branch
  is created from `main` at the M4-A.1
  closeout commit
  `feat(m4-a.1): add infrastructure project
  skeleton with IProcessRunner,
  ICredentialVault, IPlatformInfo, and
  on-disk IProjectStore`. The branch carries
  the M4-A.2 work.

- **Commit message:** `feat(m4-a.2): enable
  AppProjectCard.Open action via
  IProcessRunner`. The message follows the
  Conventional Commits format (the M4-A.1 +
  M3 closeout used the same format). The
  commit body is empty (the M4-A.1
  closeout's commit body was also empty).

- **Rule 6 (fast-forward merge):** the
  feature branch is fast-forwarded into
  `main` per rule 6. The merge preserves
  the linear history; the M4-A.2 commit is
  the new HEAD of `main`.

- **Rule 7 (delete feature branch):** the
  feature branch is deleted locally per
  rule 7. The branch does not appear in
  `git branch --list` after the deletion.

- **Push:** skipped per the operational
  constraint. The push decision recorded in
  the implementation report is **Staged
  for push** (the user did not authorise in
  this session; the M4-A.2 did not push;
  the next user command may push).

---

## 8. Next action

**The M4-A.2 stops here.** The M4-A.2
brief's "Do not begin the following task"
rule is preserved; the M4-A.2 does **not**
begin the next M4-A task (M4-A.3 is not
yet defined) or M4-B (Capability
Detection) or M4-C (Provider Registry
Foundation) or M4-D (First Concrete
Process Providers).

The next session is **either**:

- **(Option A)** the M4-A.3 implementation
  (if M4-A.3 is defined in the next
  session; the M4-A.3 plan is not yet
  drafted; the M4-A.3 may be the
  `IPlatformInfo.GetConfigDirectory`
  activation, the `IClock` extension, or
  another M4-A.1 contract extension /
  M4-A.1 contract activation); OR
- **(Option B)** the M4-B plan promotion
  (the M4-B plan is drafted in `Awaiting
  Approval`; the M4-B plan introduces
  `IHostCapabilitiesService` +
  `HostCapabilities` + the
  capability-detection UI; the M4-B plan
  depends on M4-A.1 + M4-A.2; the M4-B
  plan is the next milestone after M4-A
  closes).

The M4-A.2 session does **not** choose
between Option A and Option B; the user
chooses via the `next` invocation.

The M4-A.2's per-session handoff (this
document) is mirrored to
`.ai/handoffs/latest.md` per the
established convention (the M3.1 / M3.2 /
M3 closeout / M4-A.1 handoffs were each
mirrored).

The M4-A.2's per-session handoff is the
**predecessor** handoff to the next
session. The handoff is at
`.ai/handoffs/2026-07-11-m4-a-2-open-action.md`.

The M4-A.1's per-session handoff is
preserved at
`.ai/handoffs/2026-07-11-m4-a-1-infrastructure-project-skeleton.md`.

The M4-A.2's implementation report is at
`implementation-report-m4-a-2-open-action.md`
(15+ sections; CRLF line endings; mirrors
the M4-A.1 / M3 closeout report format).

---

**End of M4-A.2 per-session handoff.** The
M4-A.2 session is the implicit approval of
the M4-A.2 work that flows from the `Next`
invocation's end-to-end collapsed form.
M4-A.2 is delivered 2026-07-11; the M4-A.2
closeout commit
`feat(m4-a.2): enable AppProjectCard.Open
action via IProcessRunner` is on `main`; the
M4-A.2 feature branch is deleted. The M4-A.2's
per-session handoff is the canonical artifact
the next session reads first.
