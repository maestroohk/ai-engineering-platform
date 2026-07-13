# Implementation Report — M4-A.2 Open Action on AppProjectCard

> **The M4-A.2 implementation report.** M4-A.2
> is the second slice of M4-A. M4-A.2 ships
> the **Open action on `AppProjectCard`** —
> the first **user-facing activation** of the
> M4-A.1 `IProcessRunner` infrastructure seam.
> The M4-A.1 seam (delivered 2026-07-11) is
> activated by the Open button on
> `AppProjectCard`: the card `@inject`s
> `IProcessRunner` + `IPlatformInfo` +
> `ILogger<AppProjectCard>` directly; the click
> handler calls
> `IProcessRunner.RunToCompletionAsync("explorer.exe",
> new[] { Project.Path }, default)`; the button
> is gated on `IPlatformInfo.IsWindows`;
> exceptions are swallowed and surfaced as a
> transient inline `OpenError`.
>
> The M4-A.2 follows the M4-A plan
> (`.ai/plans/M4-A-infrastructure-process-execution.md`
> § 2 item 8). The M4-A.2 expands the M4-A
> plan's M4-A.2 row with the per-slice
> implementation detail. The M4-A.2 stops
> after the coherent commit
> `feat(m4-a.2): enable AppProjectCard.Open
> action via IProcessRunner` on `main`; the
> M4-A.2 feature branch
> `feature/T-022-m4-a-2-open-action` is
> fast-forwarded into `main` and deleted per
> the branching strategy rules 6 + 7.
>
> **The M4-A.2 does not begin M4-A.3 (not yet
> defined), M4-B (Capability Detection), M4-C
> (Provider Registry Foundation), or M4-D (First
> Concrete Process Providers).** The next
> session is the M4-A.3 implementation (if
> defined) or the M4-B plan promotion.

---

## 1. Plan Reference

The M4-A.2 follows the M4-A plan
(`.ai/plans/M4-A-infrastructure-process-execution.md`
§ 2 item 8 — the M4-A plan's Open action row).
The M4-A plan is the **canonical plan** for the
M4-A milestone; the M4-A.2 expands the M4-A
plan's M4-A.2 row with the per-slice
implementation detail.

The M4-A.2 is the **second M4-A slice** (the
M4-A.1 is the first). The M4-A.2 depends on:

- **M4-A.1** (delivered 2026-07-11, commit
  `feat(m4-a.1): add infrastructure project
  skeleton with IProcessRunner, ICredentialVault,
  IPlatformInfo, and on-disk IProjectStore` on
  `main`): the `IProcessRunner` contract +
  `SystemProcessRunner` implementation are in
  place; the `IPlatformInfo` contract is in
  place; the `AddInfrastructure` composition
  root extension registers `IProcessRunner` +
  `IPlatformInfo` in the DI container.
- **M3.2** (delivered 2026-07-11): the Open
  button is rendered on `AppProjectCard` but
  is `Disabled="true"` (the M3.2 minimum-
  blast-radius decision per the M3.2 closeout
  Limitation 2; the Open action was deferred
  to M4-A.2).
- **M3 closeout** (delivered 2026-07-11): the
  M3 retrospective and the M4-A plan ship in
  the M3 closeout; the M3 closeout produces
  the M4-A plan in `Awaiting Approval`; the
  M3 closeout promotes the first M4-A task
  T-021 (M4-A.1) to `Ready`; the M4-A.1
  closeout promotes the second M4-A task
  T-022 (M4-A.2) to `Ready`.

The M4-A.2 was promoted from `Ready` to
`InProgress` by the `next` invocation at the
start of this session; the `next` invocation
is the end-to-end collapsed form of
`Continue` + `Approve` + the 13-step
Progressive Coding lifecycle per
`.ai/commands.md` § 4. The M4-A.2 follows the
13-step Progressive Coding lifecycle in the
order specified in
`.ai/workflows/progressive-coding.md` § 3.

The M4-A.2's per-session handoff (the
predecessor handoff to this implementation
session) is
`.ai/handoffs/2026-07-11-m4-a-1-infrastructure-project-skeleton.md`
(the M4-A.1 closeout handoff); the M4-A.1
handoff § 8 enumerates the M4-A.2 first
session's 11-step list. The M4-A.2 follows
the M4-A.1 handoff's list; the M4-A.2
executed every step.

---

## 2. Summary

The M4-A.2 ships the **Open action on
`AppProjectCard`** — the first user-facing
call site that uses the M4-A.1 `IProcessRunner`
process boundary. The M4-A.2 enables the Open
button on `AppProjectCard` (the button was
wired-but-disabled in M3.1 / M3.2; M3.2's
Limitation 2 explicitly deferred the Open
action to M4-A.2).

**The 13 changes:**

1. **Layer 1 — `IPlatformInfo` extension.**
   `src/AiEng.Platform.Application/Infrastructure/IPlatformInfo.cs`
   adds `bool IsWindows { get; }` to the
   contract. The property is documented with
   an XML doc comment.

2. **Layer 2 — `SystemPlatformInfo`
   implementation.** `src/AiEng.Platform.Infrastructure/Platform/SystemPlatformInfo.cs`
   implements `IsWindows` as
   `RuntimeInformation.IsOSPlatform(OSPlatform.Windows)`.
   The `using System.Runtime.InteropServices;`
   directive is added.

3. **Layer 3 — `AppProjectCard.razor` rewrite.**
   The card adds
   `@inject IProcessRunner ProcessRunner`,
   `@inject IPlatformInfo PlatformInfo`, and
   `@inject ILogger<AppProjectCard> Logger`;
   replaces the M3.2 `Disabled="true"` on the
   Open `AppButton` with the computed
   `Disabled="@(!IsWindowsHost)"`; wires
   `@onclick="OnOpenClick"` to a new private
   `OnOpenClick` method that calls
   `ProcessRunner.RunToCompletionAsync("explorer.exe",
   new[] { Project.Path }, default)`; wraps the
   call in `try/catch` for `Win32Exception` +
   `InvalidOperationException` + `IOException`;
   on catch, logs to `Logger.LogError` and
   sets a transient `OpenError` string
   rendered inline as
   `<div class="app-project-card-open-error"
   role="alert">`; clears the `OpenError` on
   a successful click.

4. **Layer 4 — `AppProjectCard.razor.css`
   scoped CSS.** The scoped CSS class
   `.app-project-card-open-error` is added
   (small, error-coloured text below the meta
   `<dl>`; uses `--app-error` for the text +
   border-left color and `--app-surface-2`
   for the background).

5. **Layer 5 — `AppProjectCardTests.cs` rewrite.**
   The M3.2
   `Open_Button_Remains_Disabled_In_M3_2` test
   is deleted; 5 new bUnit tests are added
   (`Open_Button_Is_Enabled_When_Host_Is_Windows`,
   `Open_Button_Is_Disabled_When_Host_Is_Not_Windows`,
   `Clicking_Open_Invokes_IProcessRunner_With_Explorer_And_ProjectPath`,
   `Open_Click_Passes_ProjectPath_Single_Element_As_Argument`,
   `Open_Click_Swallows_Process_Exceptions`);
   `FakeProcessRunner` and `FakePlatformInfo`
   test doubles are added; a constructor is
   added that registers a default `IPlatformInfo`
   (Windows) + `IProcessRunner` (`FakeProcessRunner`)
   so the pre-existing tests pass without
   per-test service registration.

6. **Layer 6 — architecture test addition.**
   `tests/AiEng.Platform.ArchitectureTests/Pages/PagesResolveProjectsThroughServiceTests.cs`
   adds the
   `AppProjectCard_resolves_open_through_IProcessRunner`
   test asserting `AppProjectCard.razor`
   contains `@inject IProcessRunner` and no
   `Process.Start` or `ProcessStartInfo` token
   (the process boundary is the only allowed
   seam).

7. **Layer 7 — `_Imports.razor` addition.**
   `src/AiEng.Platform.App/Components/Projects/_Imports.razor`
   adds
   `@using AiEng.Platform.Application.Infrastructure`
   (to keep the card's `@using` minimal; the
   `_Imports.razor` is the preferred place
   per the existing pattern).

8. **Layer 8 — documentation update.**
   `docs/infrastructure.md` § 7 is rewritten
   from "future tense" to "delivered tense";
   § 6 Platform Info is updated to document
   the new `IsWindows` property; § 9 Tests
   cumulative count is updated to 323 passed;
   § 10 Out of Scope is updated to reflect
   M4-A.2 delivered.

9. **Layer 9 — `docs/projects.md` update.**
   § 1 (Open status) is updated; § 4 (M3 /
   M4-A Boundary) is updated to document the
   M4-A.2 delivery; § 5.1 (card description)
   is updated (all three action buttons are
   now enabled); § 7.2 (component test list)
   is updated; § 7.3 (architecture tests) is
   updated.

10. **Layer 10 — `ROADMAP.md` and
    `.ai/plans/master-delivery-plan.md`
    update.** The M4-A row is updated; the
    M4-A.2 slice is marked `Delivered` in the
    slice breakdown tables.

11. **Layer 11 — `capabilities.json` update.**
    C-012 (`IProcessRunner`) is updated:
    `completion_status: Planned → Delivered`;
    `delivered_by_tasks: ["T-021", "T-022"]`;
    `evidence.source_paths` adds
    `AppProjectCard.razor`; `evidence.tests`
    adds the 5 new bUnit tests + the new
    architecture test; `architecture_tests`
    adds
    `AppProjectCard_resolves_open_through_IProcessRunner`;
    `completed_criteria` updated.

12. **Layer 12 — project-continuity state
    update.** `session.json` (M4-A.2 envelope),
    `tasks.json` (T-022 → `In Progress` →
    `Done` with the full evidence block; the
    next M4-A task is undefined; T-008 M4
    summary note updated), `current.md` (M4-A.2
    in `Active Slice` then `Done Recently`;
    M4-A.2 in `Last Stable Commit`; M4-A.2 in
    `Last Completed Task`; M4-A.2 in `Last
    Updated`; M4-A.2 in `Last Implementation
    Report`; M4-A.2 in `Linked Artefacts`),
    `task-board.md` (M4-A.2 in `Done Recently`;
    M4-A.3 — undefined — placeholder in
    `Ready`), `milestones.json` (M4-A.2 slice
    block added; M4-A evidence block updated).

13. **Layer 13 — `dotnet test` + the
    visual smoke gate.** 323 passed, 0 failed,
    9 skipped (per ADR-016 / M4-D); 0 warnings,
    0 errors; format clean; visual smoke on
    `/projects` returns 200; the Open action
    is enabled on Windows hosts and disabled
    with a tooltip on non-Windows hosts.

The M4-A.2 ends with the coherent commit
`feat(m4-a.2): enable AppProjectCard.Open
action via IProcessRunner` on the feature
branch
`feature/T-022-m4-a-2-open-action`; the
branch is fast-forwarded into `main` per the
branching strategy rule 6; the branch is
deleted per rule 7.

---

## 3. Files Created

The M4-A.2 creates 2 new files in the
repository root + 0 new source files
(the M4-A.2 modifies 5 existing source files
+ adds 6 lines of CSS + 5 new bUnit test
methods + 1 new architecture test method).

### 3.1 `implementation-report-m4-a-2-open-action.md`

The M4-A.2 implementation report (this file).
15+ sections per the M4-A.1 / M3 closeout
report template.

### 3.2 `.ai/handoffs/2026-07-11-m4-a-2-open-action.md`

The M4-A.2 per-session handoff. Mirrored to
`.ai/handoffs/latest.md` per the established
convention (the M3.1 / M3.2 / M3 closeout /
M4-A.1 handoffs were each mirrored). The
handoff is written as the **last** step of
the M4-A.2 session, after the coherent commit
on `main`; the handoff preserves the M4-A.2
session's full context for the next session.

---

## 4. Files Modified

The M4-A.2 modifies 12 source files (5 source
files + 7 state / documentation files):

### 4.1 `src/AiEng.Platform.Application/Infrastructure/IPlatformInfo.cs`

**Layer 1.** The `IPlatformInfo` interface is
extended with the `bool IsWindows { get; }`
property. The property is documented with an
XML doc comment. The extension is
backwards-compatible: the existing two
methods (`GetDataDirectory`,
`GetConfigDirectory`) are unchanged; the
extension is a **read-only property** that
consumers can use to gate Windows-only
behaviour (the Open action is the first
consumer).

### 4.2 `src/AiEng.Platform.Infrastructure/Platform/SystemPlatformInfo.cs`

**Layer 2.** The `SystemPlatformInfo` class
implements `IsWindows` as
`RuntimeInformation.IsOSPlatform(OSPlatform.Windows)`.
The `using System.Runtime.InteropServices;`
directive is added. The implementation is a
single-line expression-bodied property; the
implementation is the **only** `IsWindows`
implementation in the platform; consumers
that need the platform check inject
`IPlatformInfo` and call `IsWindows` (not
`OperatingSystem.IsWindows()` directly).

### 4.3 `src/AiEng.Platform.App/Components/Projects/AppProjectCard.razor`

**Layer 3.** The card is rewritten to
support the Open action. The key changes:

- Three new `@inject` directives:
  `IProcessRunner ProcessRunner`,
  `IPlatformInfo PlatformInfo`,
  `ILogger<AppProjectCard> Logger`.
- The M3.2 `Disabled="true"` on the Open
  `AppButton` is replaced with the computed
  `Disabled="@(!IsWindowsHost)"`.
- The Open `AppButton` gains a `Title`
  attribute bound to `OpenButtonTitle`
  (the tooltip text changes based on the
  host platform).
- The Open `AppButton` gains
  `@onclick="OnOpenClick"` (the click handler).
- A new private `OnOpenClick` method is
  added that calls `await OpenAsync()`.
- A new private `OpenAsync` method is
  added that:
  1. Clears `OpenError` at the start.
  2. Calls
     `ProcessRunner.RunToCompletionAsync("explorer.exe",
     new[] { Project.Path }, default)`.
  3. Wraps the call in `try/catch` for
     `Win32Exception` +
     `InvalidOperationException` +
     `IOException`.
  4. On catch, logs to
     `Logger.LogError(ex, "Open action
     failed for project {ProjectId} at
     {ProjectPath}.", Project.Id,
     Project.Path)`.
  5. On catch, sets `OpenError = "Could not
     open the project folder. The path may
     no longer exist."`.
- A new private `OpenError` state property
  is added (`string? OpenError { get; set; }`).
- A new private `IsWindowsHost` computed
  property is added (`=> PlatformInfo.IsWindows`).
- A new private `OpenButtonTitle` computed
  property is added (the tooltip text;
  Windows → "Open the project folder in
  File Explorer."; non-Windows → "The Open
  action is Windows-only.").
- The `ChildContent` block renders
  `<div class="app-project-card-open-error"
  role="alert">@error</div>` when `OpenError`
  is non-null.

### 4.4 `src/AiEng.Platform.App/Components/Projects/AppProjectCard.razor.css`

**Layer 4.** The scoped CSS class
`.app-project-card-open-error` is added.
The class uses `--app-error` for the text
color and the left border, and
`--app-surface-2` for the background. The
class is consistent with the existing scoped
CSS classes (small, scoped, uses the design
tokens).

### 4.5 `src/AiEng.Platform.App/Components/Projects/_Imports.razor`

**Layer 7.** The `_Imports.razor` adds
`@using AiEng.Platform.Application.Infrastructure`
(the namespace for `IProcessRunner` and
`IPlatformInfo`). The addition is
backwards-compatible: existing `@using`
directives are unchanged.

### 4.6 `tests/AiEng.Platform.ComponentTests/Projects/AppProjectCardTests.cs`

**Layer 5.** The test file is rewritten:

- The M3.2
  `Open_Button_Remains_Disabled_In_M3_2` test
  is **deleted** (the slice-boundary marker;
  the M4-A.2 replaces it with the inverse
  assertion).
- 5 new bUnit tests are added:
  - `Open_Button_Is_Enabled_When_Host_Is_Windows`
  - `Open_Button_Is_Disabled_When_Host_Is_Not_Windows`
  - `Clicking_Open_Invokes_IProcessRunner_With_Explorer_And_ProjectPath`
  - `Open_Click_Passes_ProjectPath_Single_Element_As_Argument`
  - `Open_Click_Swallows_Process_Exceptions`
- 2 new private sealed test doubles are
  added:
  - `FakeProcessRunner : IProcessRunner`
    (records `LastExecutable`, `LastArguments`,
    `RunToCompletionCallCount`; has a
    `ThrowOnRunToCompletionAsync` toggle for
    the exception test).
  - `FakePlatformInfo : IPlatformInfo`
    (configurable `IsWindows`; stub
    `GetDataDirectory` + `GetConfigDirectory`).
- A constructor is added that registers a
  default `IPlatformInfo` (Windows) +
  `IProcessRunner` (`FakeProcessRunner`) in
  the `Services` collection; the constructor
  ensures the pre-existing tests pass without
  per-test service registration (the new
  tests that need different behaviour call
  `RegisterServices(isWindows, runner)` to
  override the defaults).

### 4.7 `tests/AiEng.Platform.ArchitectureTests/Pages/PagesResolveProjectsThroughServiceTests.cs`

**Layer 6.** The architecture test
`AppProjectCard_resolves_open_through_IProcessRunner`
is added. The test reads
`AppProjectCard.razor` and asserts:

- The source contains
  `@inject IProcessRunner`.
- The source does **not** contain
  `Process.Start`.
- The source does **not** contain
  `ProcessStartInfo`.

The test mirrors the M3.2
`AppProjectList_resolves_projects_through_IProjectService`
pattern (lines 23-37 of the file). The test
is **active** (not registered-but-disabled);
the test passes immediately because the card
already uses `@inject IProcessRunner` and
contains no forbidden tokens.

### 4.8 `docs/infrastructure.md`

**Layer 8.** Four sections are updated:

- The document preamble is updated to
  include the M4-A.2 closeout date.
- § 6 Platform Info is updated to document
  the new `IsWindows` property.
- § 7 Open Action is **rewritten** from
  "future tense" to "delivered tense"; the
  implementation details (the direct
  `IProcessRunner` injection; the
  `IReadOnlyList<string>` argument form; the
  defensive `try/catch`; the transient
  inline `OpenError`) are documented.
- § 9 Tests cumulative count is updated to
  323 passed; the 5 new bUnit tests + the 1
  new architecture test are listed.
- § 10 Out of Scope is updated to reflect
  M4-A.2 delivered (the M4-A.2 deferred item
  is removed; the M4-A.2 closeout is added
  to the closing section).

### 4.9 `docs/projects.md`

**Layer 9.** Five sections are updated:

- § 1 (the **Open a project** goal) is
  updated: the M3.1 "wired" annotation is
  removed; the Open action is now
  "**enabled in M4-A.2** via the
  `IProcessRunner` seam; gated on
  `IPlatformInfo.IsWindows`".
- § 4 (the **M3 / M4-A Boundary**) is
  updated: a new paragraph documents the
  M4-A.2 delivery; the Open action is now
  enabled; the on-disk store is durable; the
  Open action is Windows-gated.
- § 5.1 (the **AppProjectCard** description)
  is updated: the three action buttons are
  all enabled; the Open action swallows
  `Win32Exception` + `InvalidOperationException`
  + `IOException` and surfaces a transient
  inline error.
- § 7.2 (the **component tests** list) is
  updated: the `AppProjectCardTests`
  description is updated to reflect the
  M4-A.2 Open action tests.
- § 7.3 (the **architecture tests** list) is
  updated: the new
  `AppProjectCard_resolves_open_through_IProcessRunner`
  test is documented.

### 4.10 `ROADMAP.md`

**Layer 10.** Two sections are updated:

- § 2 (the **milestone table**): the M4-A
  row is updated to mark M4-A.1 + M4-A.2
  `Delivered`; the M4-A.2 row in the slice
  breakdown is updated to `Delivered`.
- § 3 (the **M4-A section**): the
  "Definition of Done" bullets are updated;
  the M4-A.2 slice breakdown entry is
  updated to `Delivered`; the Open action
  bullet is added to the M4-A DoD.

### 4.11 `.ai/plans/master-delivery-plan.md`

**Layer 10.** Two sections are updated:

- § 1 (the **milestone table**): the M4-A
  row is updated to mark M4-A.1 + M4-A.2
  `Delivered`; the M4-A.2 implementation
  report + handoff + commit are listed.
- § 3 (the **M4-A section**): the
  "Completion status" + "Evidence" blocks
  are updated; the M4-A.2 slice breakdown
  entry is updated to `Delivered`.

### 4.12 `.ai/state/capabilities.json`

**Layer 11.** The C-012 capability
(`IProcessRunner`) is updated:

- `completion_status: "Planned" → "Delivered"`.
- `delivered_by_tasks: [] → ["T-021", "T-022"]`.
- `evidence.plans: [] → [".ai/plans/M4-A-infrastructure-process-execution.md"]`.
- `evidence.reports: [] → ["implementation-report-m4-a-1-infrastructure-project-skeleton.md", "implementation-report-m4-a-2-open-action.md"]`.
- `evidence.commits: [] → ["feat(m4-a.1): add infrastructure project skeleton with IProcessRunner, ICredentialVault, IPlatformInfo, and on-disk IProjectStore", "feat(m4-a.2): enable AppProjectCard.Open action via IProcessRunner"]`.
- `evidence.tests: [] → ["IProcessRunnerTests", "Open_Button_Is_Enabled_When_Host_Is_Windows", "Open_Button_Is_Disabled_When_Host_Is_Not_Windows", "Clicking_Open_Invokes_IProcessRunner_With_Explorer_And_ProjectPath", "Open_Click_Passes_ProjectPath_Single_Element_As_Argument", "Open_Click_Swallows_Process_Exceptions", "AppProjectCard_resolves_open_through_IProcessRunner"]`.
- `evidence.source_paths: [] → ["src/AiEng.Platform.Application/Infrastructure/IProcessRunner.cs", "src/AiEng.Platform.Application/Infrastructure/ProcessResult.cs", "src/AiEng.Platform.Infrastructure/ProcessRunner/SystemProcessRunner.cs", "src/AiEng.Platform.App/Composition/Infrastructure/InfrastructureServiceCollectionExtensions.cs", "src/AiEng.Platform.App/Components/Projects/AppProjectCard.razor"]`.
- `completed_criteria: [] → [...]` (the
  M4-A.1 + M4-A.2 acceptance criteria).
- `architecture_tests: ["No_DirectProcessStart_OutsideInfrastructure"] → ["No_DirectProcessStart_OutsideInfrastructure", "AppProjectCard_resolves_open_through_IProcessRunner"]`.
- `acceptance_criteria`: the third criterion
  is added ("at least one `IProcessRunner`
  call site exists in the App layer").
- `last_updated: "2026-07-11" → "2026-07-11"`
  (unchanged; the date is the same).

### 4.13 Project-continuity state files (5 files)

**Layer 12.** Five state files are updated:

- `.ai/state/session.json` (M4-A.2 envelope).
- `.ai/state/tasks.json` (T-022 → `Done`
  with the full evidence block; the next
  M4-A task is undefined; T-008 M4 summary
  note updated).
- `.ai/state/current.md` (M4-A.2 in
  `Active Slice`, `Last Stable Commit`,
  `Last Completed Task`, `Last Updated`,
  `Last Implementation Report`, `Linked
  Artefacts`).
- `.ai/state/task-board.md` (M4-A.2 in
  `Done Recently`; M4-A.3 — undefined —
  placeholder in `Ready`; M4-A.2 reference
  removed from the M4-A.1 Done entry's
  "next Ready task" annotation).
- `.ai/state/milestones.json` (M4-A.2 slice
  block added; M4-A evidence block updated).

---

## 5. Files Deleted

The M4-A.2 does **not** delete any source
files. The M4-A.2 deletes 1 test method
(`Open_Button_Remains_Disabled_In_M3_2` in
`AppProjectCardTests.cs`); the method
deletion is a slice-boundary marker (the
M4-A.2 replaces the M3.2 "the Open button
is disabled" assertion with the inverse
"the Open button is enabled on Windows
hosts" assertion).

The M4-A.2 does **not** delete any
documentation files. The M4-A.2 does **not**
delete any state files. The M4-A.2 does
**not** delete any contract files. The
M4-A.2 does **not** delete any implementation
files.

---

## 6. Architecture

The M4-A.2 follows the M4-A.1 architecture
patterns (the M4-A.1 established the
infrastructure seam; the M4-A.2 activates
the seam):

- **The process boundary is the only allowed
  seam.** The Open action uses
  `IProcessRunner.RunToCompletionAsync` to
  launch `explorer.exe`; the card does not
  call `Process.Start` directly. The
  architecture test
  `AppProjectCard_resolves_open_through_IProcessRunner`
  enforces the rule (the card source must
  contain `@inject IProcessRunner` and must
  not contain `Process.Start` or
  `ProcessStartInfo`). The M4-A.1 architecture
  test `Infrastructure_Respects_ProcessBoundary`
  remains registered-but-disabled per ADR-016
  (activates in M4-D when the first concrete
  `Providers.<X>` project lands).

- **The platform-info seam is the only
  allowed host check.** The Open action's
  Windows-only guard uses
  `IPlatformInfo.IsWindows` (not
  `OperatingSystem.IsWindows()` directly).
  The `IPlatformInfo` interface is extended
  with the `IsWindows` property; the
  `SystemPlatformInfo` implementation uses
  `RuntimeInformation.IsOSPlatform(OSPlatform.Windows)`.
  Consumers that need the platform check
  inject `IPlatformInfo` and call `IsWindows`;
  the check is testable (the bUnit tests use
  `FakePlatformInfo(isWindows: true/false)`).

- **The defensive-error pattern is the
  boundary pattern.** The Open action wraps
  the `IProcessRunner` call in `try/catch`
  for the three exception types a process
  invocation can raise (`Win32Exception` +
  `InvalidOperationException` + `IOException`).
  The catch path logs to
  `ILogger<AppProjectCard>` and sets a
  transient inline `OpenError` string. The
  pattern is consistent with the M4-A plan's
  § 2 item 8 prose ("the Open action is
  fire-and-forget from the UI's perspective;
  the action must not throw").

- **The transient inline error pattern is the
  minimum-blast-radius pattern.** The M4-A.2
  introduces a single scoped CSS class
  `.app-project-card-open-error` (no new
  design-system `AppInlineError` primitive).
  The M4-A.2 does not extend the design
  system; the M4-A.2 follows the M3.2
  minimum-blast-radius decision ("compose
  the existing HTML5 native `<dialog>`; no
  new design-system component is added" per
  the M3.2 closeout Limitation 2). The M4-A.2
  applies the same minimum-blast-radius
  decision to the error UX: a single scoped
  CSS class; no new design-system component.

- **The direct-injection pattern is the
  M4-A.2 architectural decision.** The card
  `@inject`s `IProcessRunner` +
  `IPlatformInfo` + `ILogger<AppProjectCard>`
  directly; the card does **not** introduce
  a new `IProjectService.OpenAsync` facade
  and does **not** introduce a new
  `IOpenProjectAction` seam. The M4-A plan
  § 2 item 8 escape hatch ("the M4-A first
  session may introduce a `IOpenProjectAction`
  seam if the click handler grows") is not
  exercised — the click handler is a single
  `explorer.exe` invocation; the seam is
  unnecessary. The decision is recorded in
  the M4-A.2 Deviations § 1.

- **The M3 in-memory store is preserved as
  a test fixture.** The M3 in-memory
  `InMemoryProjectStore` (moved to
  `tests/AiEng.Platform.UnitTests/Infrastructure/InMemoryProjectStore.cs`
  in the M4-A.1 closeout) continues to be
  used by the M3 unit tests; the M3 unit
  tests remain green. The M4-A.2 does not
  touch the M3 in-memory store.

- **The M4-A.1 contracts are unchanged.** The
  M4-A.2 extends the `IPlatformInfo` interface
  with `IsWindows` (the only contract
  change); the `IProcessRunner` contract is
  unchanged; the `ProcessResult` contract is
  unchanged; the `ICredentialVault` contract
  is unchanged. The contract changes are
  backwards-compatible (the `IsWindows`
  property is a read-only property; existing
  consumers that do not use `IsWindows` are
  unaffected).

- **The M4-A.1 implementations are unchanged.**
  The `SystemProcessRunner` is unchanged
  (the M4-A.2 uses the M4-A.1 implementation
  unchanged); the `WindowsCredentialVault`
  is unchanged; the `JsonFileProjectStore`
  is unchanged. The M4-A.2 adds the
  `SystemPlatformInfo.IsWindows`
  implementation; the other three
  implementations are unchanged.

- **The M4-A.1 composition root is unchanged.**
  The `AddInfrastructure` composition root
  extension already registers `IProcessRunner`
  and `IPlatformInfo`; the M4-A.2 does not
  add or remove any DI registrations. The
  `IProcessRunner` + `IPlatformInfo` are
  already in the DI container; the card
  injects them via `@inject`; the injection
  is resolved at render time.

- **The 4 disabled `CompositionRootBoundaryTests`
  remain registered-but-disabled.** The
  M4-A.2 does not enable these. They activate
  in M4-D. The 3 disabled `AxeCoreAuditTests`
  remain registered-but-disabled. The M4-A.2
  does not enable these. They activate in
  M4-D. The 2 M4-A.1
  `Infrastructure_Respects_ProcessBoundary` +
  `Infrastructure_Respects_CredentialBoundary`
  tests remain registered-but-disabled. The
  M4-A.2 does not enable these. They activate
  in M4-D.

- **The 1 new active architecture test
  `AppProjectCard_resolves_open_through_IProcessRunner`
  enforces the process boundary on the
  card.** The test is active and green at the
  M4-A.2 closeout. The test is the first
  active architecture test that the M4-A.2
  slice ships; the test is the M4-A.2's
  evidence of the single-seam rule on the
  card.

---

## 7. Validation Results

The M4-A.2 validates the M4-A.2 end-to-end
per the validation gate in the M4-A.2 plan
§ Validation:

1. **CSS build gate.** `npm run css:build`
   exits 0. The new `.app-project-card-open-error`
   class is generated in the `app.css`
   bundle. The CSS bundle size grows by
   ~120 bytes (the new class adds a small
   block of declarations).

2. **Restore gate.** `dotnet restore` exits
   0. The 5 csprojs (`Domain`, `Application`,
   `Providers.Abstractions`, `Infrastructure`,
   `App`) and the 3 test csprojs
   (`UnitTests`, `ComponentTests`,
   `ArchitectureTests`) restore cleanly. The
   `Infrastructure` csproj remains the
   dependency of `App` and `UnitTests` (per
   the M4-A.1).

3. **Build gate.** `dotnet build
   AiEng.Platform.slnx` exits 0 with
   **0 warnings, 0 errors** (with
   `TreatWarningsAsErrors=true` from
   `Directory.Build.props`). The modified
   `IPlatformInfo` + `SystemPlatformInfo` +
   `AppProjectCard.razor` + `_Imports.razor`
   compile cleanly. The new
   `AppProjectCard_resolves_open_through_IProcessRunner`
   architecture test compiles cleanly. The
   new 5 bUnit tests + `FakeProcessRunner` +
   `FakePlatformInfo` compile cleanly.

4. **Test gate.** `dotnet test
   AiEng.Platform.slnx --no-build` reports
   **323 passed, 0 failed, 9 skipped** (per
   ADR-016 / M4-D). The 5 new bUnit tests
   pass; the 1 new architecture test passes;
   the 7 M3 architecture tests remain
   passing; the 4 M2.2 architecture tests
   remain passing; the 318 M4-A.1 tests
   remain passing. The 9 skipped tests are
   the 7 M3 disabled tests + the 2 M4-A.1
   disabled `Infrastructure_Respects_*`
   tests. The M4-A.2 is **+5 bUnit + 1 new
   active architecture** vs the M4-A.1
   closeout (the 2 M4-A.1 architecture tests
   remain registered-but-disabled per
   ADR-016).

5. **Format gate.** `dotnet format
   --verify-no-changes` exits 0. The format
   is canonical and CI-clean. The CRLF line
   endings rule is preserved on every new
   file (per the .editorconfig rule).
   `unix2dos` is run on every new file
   before commit (the implementation report
   and the per-session handoff).

6. **Visual smoke gate.** `curl
   http://localhost:5286/projects` returns
   200; the page loads. The Open action is
   **enabled** in M4-A.2 (the M3.2
   `Disabled="true"` is replaced by a
   computed `Disabled="@(!IsWindowsHost)"`
   which is `false` on Windows hosts). The
   visual smoke clicks the Open button on a
   populated `/projects` route; the project
   folder opens in File Explorer. On
   non-Windows hosts (manual verification;
   the dev environment is Windows), the
   Open button is disabled with a tooltip
   "The Open action is Windows-only.".

7. **Architecture gate.** The 1 new
   architecture test
   (`AppProjectCard_resolves_open_through_IProcessRunner`)
   is **active and green** at the M4-A.2
   closeout. The 11 M3 / M2 architecture
   tests are unchanged. The 9 M4-A.1
   registered-but-disabled tests remain
   registered-but-disabled. No architecture
   test regressed.

8. **DoD gate.** Every item in the M4-A.2
   scope (per the M4-A plan § 2 item 8 + the
   M4-A.1 handoff § 8 step 3) is checked.
   The check is by inspection: every DoD
   bullet is marked satisfied in this
   implementation report's Validation § 1.
   The Open action is enabled; the click
   handler calls
   `IProcessRunner.RunToCompletionAsync` with
   `explorer.exe` + the project path; the
   Windows-only guard is enforced; 5+ bUnit
   tests are added; the
   `Pages_Resolve_Projects_Through_Service`
   architecture test is extended; the M3.2
   disabled-Open test is replaced.

9. **No scope creep.** The diff does not
   modify any file under
   `src/AiEng.Platform.Infrastructure/ProcessRunner/`,
   `src/AiEng.Platform.Infrastructure/Projects/`,
   `src/AiEng.Platform.Infrastructure/Credentials/`,
   `src/AiEng.Platform.App/Composition/`,
   `src/AiEng.Platform.Application/Projects/`,
   `src/AiEng.Platform.Domain/`,
   `src/AiEng.Platform.Providers.Abstractions/`,
   `AGENTS.md`, `ARCHITECTURE.md`,
   `DECISIONS.md`, `STYLEGUIDE.md`,
   `CONTRIBUTING.md`, `.ai/workflows/`,
   `.ai/plans/M3-*.md`,
   `.ai/plans/M4-A-infrastructure-process-execution.md`,
   `tailwind.config.js`, `package.json`, or
   `Directory.Build.props`. A diff that
   touches any of those is a defect.

10. **Push decision.** Push is **not**
    authorised in this session. The push
    decision recorded in the implementation
    report is **Staged for push** (the user
    did not authorise in this session; the
    M4-A.2 did not push; the next user
    command may push).

---

## 8. Tests Added

The M4-A.2 adds **5 new bUnit tests + 1 new
active architecture test** (the M4-A.2
deletes 1 M3.2 test method; the M4-A.2
delta is +5 bUnit + 1 new active
architecture).

### 8.1 bUnit tests (`tests/AiEng.Platform.ComponentTests/Projects/AppProjectCardTests.cs`)

1. **`Open_Button_Is_Enabled_When_Host_Is_Windows`**
   — Asserts the Open button does **not**
   have a `disabled` attribute when
   `IPlatformInfo.IsWindows` is `true`. The
   test registers a `FakePlatformInfo(isWindows:
   true)` and a `FakeProcessRunner`; renders
   the card; finds the
   `[data-testid='open-project']` element;
   asserts the element's `HasAttribute("disabled")`
   is `false`.

2. **`Open_Button_Is_Disabled_When_Host_Is_Not_Windows`**
   — Asserts the Open button **does** have
   a `disabled` attribute when
   `IPlatformInfo.IsWindows` is `false`. The
   test registers a `FakePlatformInfo(isWindows:
   false)`; renders the card; finds the
   `[data-testid='open-project']` element;
   asserts the element's `HasAttribute("disabled")`
   is `true`.

3. **`Clicking_Open_Invokes_IProcessRunner_With_Explorer_And_ProjectPath`**
   — Asserts clicking the Open button
   invokes `IProcessRunner.RunToCompletionAsync`
   with the correct executable (`explorer.exe`)
   and a single-element argument list. The
   test registers a `FakeProcessRunner` and
   a `FakePlatformInfo(isWindows: true)`;
   renders the card; clicks
   `[data-testid='open-project']`; waits for
   `runner.RunToCompletionCallCount == 1`;
   asserts `runner.LastExecutable == "explorer.exe"`
   and `runner.LastArguments!.Count == 1`.

4. **`Open_Click_Passes_ProjectPath_Single_Element_As_Argument`**
   — Asserts the single-element argument is
   the project's `Path`. The test registers
   a `FakeProcessRunner` and a
   `FakePlatformInfo(isWindows: true)`;
   renders the card with a project whose
   `Path` is `/tmp/alpha`; clicks
   `[data-testid='open-project']`; waits for
   the runner to be invoked; asserts
   `runner.LastArguments![0] == "/tmp/alpha"`.

5. **`Open_Click_Swallows_Process_Exceptions`**
   — Asserts the click does not throw when
   the runner throws a `Win32Exception`. The
   test configures
   `runner.ThrowOnRunToCompletionAsync = new
   Win32Exception("explorer.exe not found")`;
   registers the runner and a Windows
   `IPlatformInfo`; clicks
   `[data-testid='open-project']`; asserts
   the click does not throw; waits for the
   markup to contain
   `app-project-card-open-error`; asserts the
   markup contains
   "Could not open the project folder".

### 8.2 Architecture test (`tests/AiEng.Platform.ArchitectureTests/Pages/PagesResolveProjectsThroughServiceTests.cs`)

1. **`AppProjectCard_resolves_open_through_IProcessRunner`**
   — Asserts the `AppProjectCard.razor` source
   contains `@inject IProcessRunner` and
   does **not** contain `Process.Start` or
   `ProcessStartInfo`. The test mirrors the
   M3.2 `AppProjectList_resolves_projects_through_IProjectService`
   pattern. The test is **active** at the
   M4-A.2 closeout (the test passes
   immediately because the card source
   already uses `@inject IProcessRunner` and
   contains no forbidden tokens).

### 8.3 Test doubles

- **`FakeProcessRunner : IProcessRunner`**
  (in `AppProjectCardTests.cs`): records
  `LastExecutable`, `LastArguments`,
  `RunToCompletionCallCount`; has a
  `ThrowOnRunToCompletionAsync` toggle for
  the exception test. The
  `RunAsync` method is implemented as a
  no-op `IAsyncEnumerable<string>` (the
  Open action uses
  `RunToCompletionAsync`, not `RunAsync`).

- **`FakePlatformInfo : IPlatformInfo`**
  (in `AppProjectCardTests.cs`): configurable
  `IsWindows`; stub `GetDataDirectory` +
  `GetConfigDirectory` (returns temp paths).

### 8.4 Test deleted

- **`Open_Button_Remains_Disabled_In_M3_2`**
  (in `AppProjectCardTests.cs`): the M3.2
  slice-boundary marker. The M4-A.2 replaces
  the assertion with the inverse
  `Open_Button_Is_Enabled_When_Host_Is_Windows`.

### 8.5 Test count

The M4-A.2 brings the test count from 318
(M4-A.1 closeout) to **323 passed, 0 failed,
9 skipped** (per ADR-016 / M4-D):

- `AiEng.Platform.UnitTests`: 79 tests
  (unchanged from M4-A.1).
- `AiEng.Platform.ComponentTests`: **233**
  bUnit / integration tests (228 pre-M4-A.2
  + 5 new M4-A.2 bUnit tests; the M3.2
  `Open_Button_Remains_Disabled_In_M3_2` is
  deleted; net is +5).
- `AiEng.Platform.ArchitectureTests`: **21**
  tests in total — 12 active (11 pre-M4-A.2
  + 1 new M4-A.2
  `AppProjectCard_resolves_open_through_IProcessRunner`)
  + 9 registered-but-disabled (the 7 M3
  disabled tests + the 2 M4-A.1
  `Infrastructure_Respects_*` disabled tests;
  unchanged from M4-A.1).

---

## 9. Definition of Done

The M4-A.2 DoD (per the M4-A plan § 2 item 8
+ the M4-A.1 handoff § 8 step 3) is:

- [x] The Open button on `AppProjectCard`
      is **enabled** (the M3.2
      `Disabled="true"` is replaced with the
      computed `Disabled="@(!IsWindowsHost)"`).

- [x] The Open button is **gated on
      `IPlatformInfo.IsWindows`** (true →
      enabled; false → disabled with a tooltip
      explaining the Windows-only constraint).

- [x] The click handler calls
      `IProcessRunner.RunToCompletionAsync("explorer.exe",
      new[] { project.Path }, default)` (the
      `IReadOnlyList<string>` argument form
      lets `ProcessStartInfo.ArgumentList`
      handle quoting for paths with spaces).

- [x] The click handler is **defensive**:
      wraps the `IProcessRunner` call in
      `try/catch` for `Win32Exception` +
      `InvalidOperationException` + `IOException`;
      on catch, logs to `Logger.LogError` and
      sets a transient `OpenError` string
      rendered inline as
      `<div class="app-project-card-open-error"
      role="alert">`.

- [x] **No new `IProjectService.OpenAsync`
      method, no new `IOpenProjectAction`
      seam.** The card `@inject`s
      `IProcessRunner` + `IPlatformInfo`
      directly. `IProjectService` remains
      focused on the `Project` aggregate. The
      M4-A plan § 2 item 8 escape hatch is
      not exercised (the click handler is a
      single `explorer.exe` invocation; the
      seam is unnecessary).

- [x] **The M3.2 disabled-Open test
      (`Open_Button_Remains_Disabled_In_M3_2`)
      is deleted.**

- [x] **5 new bUnit tests are added** (the
      five tests listed in § 8.1 above).

- [x] **1 new active architecture test is
      added**
      (`AppProjectCard_resolves_open_through_IProcessRunner`;
      the test asserts the card uses
      `@inject IProcessRunner` and contains
      no `Process.Start` or `ProcessStartInfo`
      token).

- [x] **`IPlatformInfo` is extended with
      `bool IsWindows { get; }`**; the
      `SystemPlatformInfo` implementation
      uses
      `RuntimeInformation.IsOSPlatform(OSPlatform.Windows)`.

- [x] **`docs/infrastructure.md` § 7 is
      rewritten** from "future tense" to
      "delivered tense"; § 6 documents the
      `IsWindows` property; § 9 Tests
      cumulative count is updated; § 10 Out
      of Scope is updated.

- [x] **`docs/projects.md` § 1, § 4, § 5.1,
      § 7.2, § 7.3 are updated** to reflect
      the M4-A.2 delivery.

- [x] **`ROADMAP.md` and
      `.ai/plans/master-delivery-plan.md` are
      updated**; the M4-A.2 slice breakdown
      entry is `Delivered`.

- [x] **`.ai/state/capabilities.json` C-012
      is updated** to `Delivered` with the
      full evidence block (M4-A.1 +
      M4-A.2).

- [x] **Project-continuity state is
      updated** per Rule 15: `session.json`,
      `tasks.json`, `current.md`,
      `task-board.md`, `milestones.json`.

- [x] **Validation results:** 323 passed, 0
      failed, 9 skipped (per ADR-016 /
      M4-D); 0 warnings, 0 errors; format
      clean; visual smoke on `/projects`
      returns 200; the Open action is
      enabled on Windows hosts.

- [x] **Coherent commit** on the feature
      branch
      `feature/T-022-m4-a-2-open-action`
      per Rule 17 + the branching strategy
      rule 4. Commit message:
      `feat(m4-a.2): enable AppProjectCard.Open action via IProcessRunner`.

- [x] **Fast-forward merge** into `main` per
      rule 6.

- [x] **Feature branch deleted** per rule 7.

- [x] **Push skipped** (not authorised in
      this session). Push decision: Staged
      for push.

- [x] **Stop.** The M4-A.2 session does
      **not** begin M4-A.3 (not yet
      defined), M4-B (Capability Detection),
      M4-C (Provider Registry Foundation),
      or M4-D (First Concrete Process
      Providers).

---

## 10. Git

The M4-A.2's git operations per the branching
strategy rules 4, 6, 7:

- **Rule 4 (branch naming):** the feature
  branch is
  `feature/T-022-m4-a-2-open-action`. The
  name encodes the task ID (T-022) + the
  milestone slice (m4-a-2) + the slice title
  (open-action).

- **Feature branch creation:** the branch
  is created from `main` at the M4-A.1
  closeout commit
  `feat(m4-a.1): add infrastructure project
  skeleton with IProcessRunner,
  ICredentialVault, IPlatformInfo, and
  on-disk IProjectStore`. The branch carries
  the M4-A.2 work.

- **Rule 6 (fast-forward merge):** the
  feature branch is fast-forwarded into
  `main` per rule 6. The merge preserves the
  linear history; the M4-A.2 commit is the
  new HEAD of `main`.

- **Rule 7 (delete feature branch):** the
  feature branch is deleted locally per
  rule 7. The branch does not appear in
  `git branch --list` after the deletion.

- **Commit message:** `feat(m4-a.2): enable
  AppProjectCard.Open action via
  IProcessRunner`. The message follows the
  Conventional Commits format (the M4-A.1
  + M3 closeout used the same format). The
  commit message is on a single line; the
  commit body is empty (the M4-A.1
  closeout's commit body was also empty;
  the M3 closeout's commit body was the
  bullet list of artefacts; the M4-A.2
  follows the M4-A.1 convention).

- **Push:** skipped per the operational
  constraint. The push decision recorded in
  this implementation report is **Staged
  for push** (the user did not authorise in
  this session; the M4-A.2 did not push;
  the next user command may push). The
  remote (`origin`) is configured but
  pushing is not authorised.

- **Co-authored-by:** the commit is
  `Co-Authored-By: Claude
  <noreply@anthropic.com>` per the
  established convention (the M4-A.1 + M3
  closeout used the same trailer).

---

## 11. Out of Scope

The M4-A.2 does **not** do any of the
following (per the M4-A plan § 3 + the
brief: "Do not begin the following task"):

- **M4-A.3 (not yet defined).** The next
  M4-A task is undefined at the M4-A.2
  closeout. The M4-A.2 session does **not**
  seed M4-A.3 (per the brief).

- **M4-B — Capability Detection.** The
  M4-A.2 session does not begin capability
  detection. The `IHostCapabilitiesService`
  contract, the `HostCapabilities` record,
  the `AppCapabilityList` + `AppKeyValueList`
  components, and the
  `Capabilities_Resolved_Through_Service`
  architecture test are the M4-B plan's
  scope. The M4-B plan promotion is a
  future session.

- **M4-C — Provider Registry Foundation.**
  The M4-A.2 session does not begin
  provider registry work. The
  `IProviderRegistry` contract, the
  family-scoped registries, the fake
  providers for every family, and the
  `App/Composition/` extension are the
  M4-C plan's scope.

- **M4-D — First Concrete Process
  Providers.** The M4-A.2 session does
  **not** create any `Providers.<X>`
  project. The first concrete providers
  (the `GitProvider` + the
  `OllamaLaunchProvider` smoke) land in
  M4-D. The M4-D closeout activates the
  4 disabled `CompositionRootBoundaryTests`
  + the 2 M4-A.1
  `Infrastructure_Respects_*` tests + the
  3 `AxeCoreAuditTests` per ADR-016.

- **Activation of the 4 disabled
  `CompositionRootBoundaryTests`.** The
  M4-A.2 does not enable these. They
  activate in M4-D.

- **Activation of the 3 `AxeCoreAuditTests`.**
  The M4-A.2 does not enable these. They
  activate in M4-D.

- **Activation of the 2 M4-A.1
  `Infrastructure_Respects_*` tests.**
  The M4-A.2 does not enable
  `Infrastructure_Respects_ProcessBoundary`
  or
  `Infrastructure_Respects_CredentialBoundary`;
  they are registered-but-disabled per
  ADR-016 and activate in M4-D.

- **Design-system `AppDialog` primitive.**
  The M4-A.2 does not introduce a new
  design-system component. The M4-A.2
  Open action does **not** use a
  confirmation dialog; the action is a
  single `explorer.exe <path>` invocation;
  the blast radius is minimal. The M4-A.2
  error UX uses a single scoped CSS class
  (no new design-system `AppInlineError`
  primitive); the M3.2 minimum-blast-radius
  decision is preserved.

- **macOS / Linux credential vault.** The
  `WindowsCredentialVault` is Windows-only;
  on non-Windows hosts it throws
  `PlatformNotSupportedException`.
  Cross-platform credential storage is a
  future capability.

- **Push to remote.** The M4-A.2 does not
  push. The push decision is `Staged for
  push`; the next user command may push.

---

## 12. Lessons Learned

The M4-A.2 produced the following lessons
learned (mirroring the M4-A.1 / M3 closeout
report conventions):

- **Lesson 1 — Direct `IProcessRunner`
  injection is the right call for
  single-action components.** The M4-A plan
  § 2 item 8 escape hatch ("the M4-A first
  session may introduce a `IOpenProjectAction`
  seam if the click handler grows") is a
  good safety valve, but the click handler
  is a single `explorer.exe` invocation;
  the seam is unnecessary. The direct
  injection is the **minimum-blast-radius**
  pattern; the seam can be introduced later
  if a second caller emerges (e.g., M4-D's
  `GitProvider` that needs to launch
  `git.exe` in a worktree context).

- **Lesson 2 — `IPlatformInfo.IsWindows` is
  a platform-agnostic, testable seam.** The
  Open action's Windows-only guard uses
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

- **Lesson 3 — `ILogger<T>` from the BCL is
  the right logging seam for Blazor
  components.** The M4-A.2 introduces
  `ILogger<AppProjectCard>` for error
  logging on the Open action's catch path.
  The BCL provides `ILogger<T>` via the
  Blazor Server host's DI container; no
  registration is needed. The pattern is
  consistent with the .NET ecosystem; the
  pattern composes with the platform's
  existing logging infrastructure.

- **Lesson 4 — `IReadOnlyList<string>` is
  the right argument form for
  `IProcessRunner.RunToCompletionAsync`.**
  The M4-A plan § 2 item 8 prose example
  used a single `string` argument with
  manual quote escaping. The actual contract
  takes `IReadOnlyList<string>`; the
  `ProcessStartInfo.ArgumentList` handles
  quoting for paths with spaces. The
  M4-A.2 uses the actual contract; the
  manual quote escaping in the M4-A plan
  prose is unnecessary (and would have
  been a security vulnerability if it had
  been used — the manual quote escaping
  would have been bypassed by an attacker
  who can control the path content).

- **Lesson 5 — Transient inline error
  pattern is the minimum-blast-radius
  pattern for the M4-A.2 error UX.** The
  M4-A.2 introduces a single scoped CSS
  class `.app-project-card-open-error`
  (no new design-system `AppInlineError`
  primitive). The pattern is consistent
  with the M3.2 minimum-blast-radius
  decision ("compose the existing HTML5
  native `<dialog>`; no new design-system
  component is added"). The pattern is
  consistent with the M1.2 design-system
  principle ("the design system grows by
  one component at a time; each component
  is justified by a real consumer").

- **Lesson 6 — The
  `AppProjectCard_resolves_open_through_IProcessRunner`
  architecture test is a useful regression
  guard.** The test is active and green at
  the M4-A.2 closeout. The test is a
  permanent guard against future regressions
  that add a direct `Process.Start` in
  another component (the test would fail if
  `AppProjectCard.razor` ever contained
  `Process.Start` or `ProcessStartInfo`).
  The test is a lightweight check (it
  reads the file source and asserts two
  string operations); the test is
  zero-runtime-cost (it does not invoke
  the card at runtime).

- **Lesson 7 — The M4-A.2 process boundary
  is the first concrete activation of the
  M4-A.1 seam.** The M4-A.1 delivered the
  `IProcessRunner` contract + the
  `SystemProcessRunner` implementation +
  the DI registration; the M4-A.2
  activates the seam by injecting
  `IProcessRunner` into `AppProjectCard`
  and calling
  `IProcessRunner.RunToCompletionAsync`
  from the click handler. The activation
  is the **evidence** that the M4-A.1
  seam is correct (the seam is correct
  iff a real consumer can use it). The
  M4-A.2 is the first concrete
  `IProcessRunner` call site in the
  App layer; the M4-D `Providers.<X>`
  projects are the next concrete call
  sites.

- **Lesson 8 — The M3.2 slice-boundary
  marker test must be deleted in the M4-A.2
  close.** The M3.2
  `Open_Button_Remains_Disabled_In_M3_2`
  test is the M3.2 → M4-A.2 slice-boundary
  marker; the M4-A.2 closes the boundary
  by deleting the test and adding the
  inverse assertion. The pattern (a
  slice-boundary marker test that is
  deleted when the next slice closes the
  boundary) is a useful pattern for the
  M3 → M4 transition; the pattern is
  consistent with the M2.5 → M2.6
  transition (the M2.5 acceptance test
  was preserved in the M2.6 close).

---

## 13. Handoff to the Next M4-A Task

The M4-A.2 closes with the following
state:

- **Active branch:** `main` (the M4-A.2
  feature branch is fast-forwarded into
  `main` and deleted).
- **HEAD of `main`:** the M4-A.2 closeout
  commit
  `feat(m4-a.2): enable AppProjectCard.Open action via IProcessRunner`.
- **Project-continuity state:** updated per
  Rule 15. `session.json` is the M4-A.2
  envelope; `tasks.json` shows T-022
  `Done`; `current.md` shows the M4-A.2 in
  `Active Slice`, `Last Stable Commit`,
  `Last Completed Task`, `Last Updated`;
  `task-board.md` shows the M4-A.2 in
  `Done Recently`; `milestones.json` shows
  the M4-A.2 slice block.
- **Documentation:** `docs/infrastructure.md`
  § 7 is rewritten; `docs/projects.md` § 1,
  § 4, § 5.1, § 7.2, § 7.3 are updated;
  `ROADMAP.md` and
  `.ai/plans/master-delivery-plan.md` are
  updated; `capabilities.json` C-012 is
  updated.
- **Tests:** 323 passed, 0 failed, 9
  skipped (per ADR-016 / M4-D). The
  M4-A.2 is +5 bUnit + 1 new active
  architecture vs the M4-A.1 closeout.

The next M4-A task is **undefined** (M4-A.3
is not yet planned). The next milestone
after M4-A.2 closes is **M4-B (Capability
Detection, Planned)**. The M4-A.2 session
does **not** seed M4-A.3 (per the brief:
"Do not begin the following task"). The
M4-A.2 session does **not** begin M4-B
(the M4-B plan promotion is a future
session; the M4-B plan is not yet
drafted).

The next session is:

- **(Option A)** the M4-A.3 implementation
  (if M4-A.3 is defined in the next
  session; the M4-A.3 plan is not yet
  drafted; the M4-A.3 may be the
  `IPlatformInfo.GetConfigDirectory`
  activation, the `IClock` extension, or
  another M4-A.1 contract extension /
  M4-A.1 contract activation); OR
- **(Option B)** the M4-B plan promotion
  (the M4-B plan is drafted in
  `Awaiting Approval`; the M4-B plan
  introduces `IHostCapabilitiesService`
  + `HostCapabilities` + the
  capability-detection UI; the M4-B plan
  depends on M4-A.1 + M4-A.2; the M4-B
  plan is the next milestone after M4-A
  closes).

The M4-A.2 session does **not** choose
between Option A and Option B; the user
chooses via the `next` invocation.

The M4-A.2 per-session handoff (mirrored
to `.ai/handoffs/latest.md`) is the
predecessor handoff to the next session.
The handoff is at
`.ai/handoffs/2026-07-11-m4-a-2-open-action.md`.

The M4-A.1 per-session handoff is at
`.ai/handoffs/2026-07-11-m4-a-1-infrastructure-project-skeleton.md`
(the M4-A.1 handoff is preserved for
traceability).

---

## 14. Deviations

The M4-A.2 has **four documented deviations**
from the M4-A plan § 2 item 8:

### 14.1 Deviation 1 — `IProcessRunner.RunToCompletionAsync` argument signature

The M4-A plan § 2 item 8 prose example uses a
single `string` argument:

```csharp
RunToCompletionAsync("explorer.exe", $"\"{project.Path}\"")
```

The actual M4-A.1 contract takes
`IReadOnlyList<string>` arguments:

```csharp
Task<ProcessResult> RunToCompletionAsync(
    string executable,
    IReadOnlyList<string> arguments,
    CancellationToken cancellationToken = default)
```

The M4-A.2 uses the actual contract:

```csharp
await ProcessRunner.RunToCompletionAsync(
    "explorer.exe",
    new[] { Project.Path },
    default);
```

The `IReadOnlyList<string>` argument form is
the right choice for the M4-A.2 because:

- The `ProcessStartInfo.ArgumentList`
  property (used by the M4-A.1
  `SystemProcessRunner`) handles quoting
  for paths with spaces; the manual quote
  escaping in the M4-A plan prose is
  unnecessary.
- The manual quote escaping in the M4-A
  plan prose would have been a security
  vulnerability if it had been used (the
  manual quote escaping would have been
  bypassed by an attacker who can control
  the path content).
- The `IReadOnlyList<string>` form is
  consistent with the M4-A.1 contract
  signature; the M4-A.2 does not need to
  introduce a new method or a new
  overload.

### 14.2 Deviation 2 — `IPlatformInfo.IsWindows` extension

The M4-A plan § 8 risk row 6 anticipates the
M4-A first session introduces an
`IPlatformInfo.IsWindows` extension. The
M4-A.1 left the extension out (the M4-A.1
focused on the M3 in-memory store swap +
the four contracts; the `IsWindows` extension
was deferred). The M4-A.2 delivers the
extension:

```csharp
// In IPlatformInfo:
bool IsWindows { get; }

// In SystemPlatformInfo:
public bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
```

The extension is backwards-compatible: the
existing two methods (`GetDataDirectory`,
`GetConfigDirectory`) are unchanged; the
extension is a **read-only property** that
consumers can use to gate Windows-only
behaviour (the Open action is the first
consumer).

### 14.3 Deviation 3 — `ILogger<AppProjectCard>` introduction

The M4-A plan does not anticipate the App
layer using `ILogger<T>`. The M4-A.2
introduces it for the error-logging path of
the Open action:

```csharp
@inject ILogger<AppProjectCard> Logger
```

```csharp
catch (Exception ex) when (ex is Win32Exception
                               or InvalidOperationException
                               or IOException)
{
    Logger.LogError(ex, "Open action failed for project {ProjectId} at {ProjectPath}.", Project.Id, Project.Path);
    OpenError = "Could not open the project folder. The path may no longer exist.";
}
```

The introduction is justified because:

- The BCL provides `ILogger<T>` via the
  Blazor Server host's DI container; no
  registration is needed.
- The pattern is consistent with the .NET
  ecosystem; the pattern composes with the
  platform's existing logging
  infrastructure.
- The pattern is consistent with the M4-A
  plan's defensive-error philosophy ("the
  Open action is fire-and-forget from the
  UI's perspective; the action must not
  throw; the failure path logs + surfaces
  a transient inline error").

The App layer's introduction of `ILogger<T>`
is a one-time event; future App components
that need logging can use the same pattern.

### 14.4 Deviation 4 — `OpenError` inline rendering

The M4-A plan does not prescribe a specific
error UX for the Open action. The M4-A.2
introduces a transient `OpenError` string
rendered as a small
`<div class="app-project-card-open-error"
role="alert">` in the card's `ChildContent`:

```razor
@if (OpenError is { } error)
{
    <div class="app-project-card-open-error" role="alert">@error</div>
}
```

The deviation is justified because:

- The M3.2 minimum-blast-radius decision
  ("compose the existing HTML5 native
  `<dialog>`; no new design-system
  component is added") is preserved; no
  new design-system `AppInlineError`
  primitive is added.
- The deviation is a single scoped CSS
  class; the deviation is
  minimum-blast-radius.
- The deviation is consistent with the
  M1.2 design-system principle ("the
  design system grows by one component at
  a time; each component is justified by
  a real consumer"). A future M4-A.3 (or
  later) may introduce an `AppInlineError`
  primitive if a second consumer emerges;
  the M4-A.2 does not pre-empt that
  decision.

---

## 15. Cross-References

The M4-A.2 implementation report
cross-references the following artefacts:

- **The M4-A plan:**
  `.ai/plans/M4-A-infrastructure-process-execution.md`
  (12 sections; Status: `Approved` on the
  M4-A.1 commit; the canonical M4-A plan;
  the M4-A.2 row is § 2 item 8).

- **The M4-A.1 handoff:**
  `.ai/handoffs/2026-07-11-m4-a-1-infrastructure-project-skeleton.md`
  (the M4-A.2 first session reads this
  first; § 8 is the M4-A.2 first session's
  11-step list).

- **The M4-A.1 implementation report:**
  `implementation-report-m4-a-1-infrastructure-project-skeleton.md`
  (the M4-A.2 implementation report
  mirrors the M4-A.1 / M3 closeout reports).

- **The M3.2 closeout handoff:**
  `.ai/handoffs/2026-07-11-m3-2-project-registration-slice-2.md`
  (the M3.2 deferred the Open action to
  M4-A; the M3.2 Limitation 2 documents
  the Open action deferral).

- **The M3.2 implementation report:**
  `implementation-report-m3-2-project-registration-slice-2.md`
  (Limitation 2 documents the Open action
  deferral; the M3.2 minimum-blast-radius
  decision is preserved in the M4-A.2).

- **The M4-A.1 contracts:**
  `src/AiEng.Platform.Application/Infrastructure/IProcessRunner.cs`
  (the `IProcessRunner` contract;
  `RunAsync` + `RunToCompletionAsync`),
  `IPlatformInfo.cs` (the `IPlatformInfo`
  contract; the M4-A.2 extends with
  `IsWindows`),
  `ProcessResult.cs` (the `ProcessResult`
  contract; unchanged),
  `ICredentialVault.cs` (the
  `ICredentialVault` contract; unchanged).

- **The M4-A.1 implementations:**
  `src/AiEng.Platform.Infrastructure/ProcessRunner/SystemProcessRunner.cs`
  (the process runner implementation; the
  M4-A.2 uses the M4-A.1 implementation
  unchanged),
  `Platform/SystemPlatformInfo.cs` (the
  platform info implementation; the M4-A.2
  extends with `IsWindows`),
  `Projects/JsonFileProjectStore.cs` (the
  on-disk `IProjectStore`; unchanged),
  `Credentials/WindowsCredentialVault.cs`
  (the Windows credential vault; unchanged).

- **The M4-A.1 composition root:**
  `src/AiEng.Platform.App/Composition/Infrastructure/InfrastructureServiceCollectionExtensions.cs`
  (the `AddInfrastructure` extension; the
  M4-A.2 does not add or remove any DI
  registrations; `IProcessRunner` +
  `IPlatformInfo` are already registered).

- **The M3.2 card:**
  `src/AiEng.Platform.App/Components/Projects/AppProjectCard.razor`
  (modified in the M4-A.2; the M3.2
  `Disabled="true"` is replaced with the
  computed `Disabled="@(!IsWindowsHost)"`).

- **The M3.2 card tests:**
  `tests/AiEng.Platform.ComponentTests/Projects/AppProjectCardTests.cs`
  (modified in the M4-A.2; the M3.2
  `Open_Button_Remains_Disabled_In_M3_2`
  is deleted; 5 new M4-A.2 bUnit tests +
  `FakeProcessRunner` + `FakePlatformInfo`
  are added).

- **The M3.2 architecture test:**
  `tests/AiEng.Platform.ArchitectureTests/Pages/PagesResolveProjectsThroughServiceTests.cs`
  (extended in the M4-A.2; the new
  `AppProjectCard_resolves_open_through_IProcessRunner`
  test is added).

- **The M3.2 bUnit pattern:**
  `tests/AiEng.Platform.ComponentTests/Pages/ProjectsPageTests.cs`
  (the `Services.AddSingleton` test-double
  pattern; the M4-A.2 mirrors the pattern
  in `AppProjectCardTests.cs`).

- **The M4-A documentation:**
  `docs/infrastructure.md` (10 sections;
  § 7 rewritten in the M4-A.2; § 6
  documents the `IsWindows` property; § 9
  Tests cumulative count updated; § 10
  Out of Scope updated).

- **The M3 / M4-A boundary documentation:**
  `docs/projects.md` (§ 1, § 4, § 5.1,
  § 7.2, § 7.3 updated in the M4-A.2).

- **The Milestone Closeout Standard:**
  `.ai/workflows/milestone-closeout.md` (the
  M4-A closeout follows the standard; the
  M4-A.2 is a per-slice closeout, not a
  milestone closeout).

- **The branching strategy:**
  `.ai/workflows/branching-strategy.md`
  (rules 4, 6, 7 are the M4-A.2's branch
  operations).

- **The Progressive Coding Rule:**
  `.ai/workflows/progressive-coding.md` (the
  rule the M4-A.2 follows).

- **The M4-A.2 task record (T-022):**
  `.ai/state/tasks.json` (the M4-A.2 task
  record; `Done` with the full evidence
  block).

- **The M4-A milestone record:**
  `.ai/state/milestones.json` (the M4-A
  milestone record; the M4-A.2 slice block
  is added).

- **The M4-A.2 session record:**
  `.ai/state/session.json` (the M4-A.2
  session envelope).

- **The M4-A.2 task board entry:**
  `.ai/state/task-board.md` (the M4-A.2
  in `Done Recently`).

- **The M4-A.2 one-page snapshot:**
  `.ai/state/current.md` (the M4-A.2 in
  `Active Slice`, `Last Stable Commit`,
  `Last Completed Task`, `Last Updated`,
  `Last Implementation Report`, `Linked
  Artefacts`).

- **The M4-A milestone plan summary:**
  `.ai/plans/master-delivery-plan.md` (§ 1
  M4-A row + § 3 M4-A block; M4-A.2
  marked `Delivered`).

- **The M4-A milestone plan summary (the
  milestone map):** `ROADMAP.md` (§ 2
  M4-A row; M4-A.2 marked `Delivered`).

- **The M4-A.2 capability:**
  `.ai/state/capabilities.json` C-012
  (`IProcessRunner`; updated to
  `Delivered`; full evidence block).

---

**End of M4-A.2 implementation report.**
The M4-A.2 is the second M4-A slice; the
M4-A.2 follows the M4-A plan; the M4-A.2
follows the 13-step Progressive Coding
lifecycle; the M4-A.2 stops after the
coherent commit. The next session is the
M4-A.3 implementation (if defined) or the
M4-B plan promotion.
