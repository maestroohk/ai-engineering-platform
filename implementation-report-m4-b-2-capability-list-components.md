# Implementation Report — M4-B.2 AppCapabilityList + AppKeyValueList Data-Owning Design-System Components — `m4-b-2-capability-list-components` (2026-07-13)

> **The M4-B.2 implementation report.** M4-B.2
> (T-025) is the second M4-B implementation
> slice. M4-B.2 follows the M4-B.1 first
> session per the Progressive Coding Rule.
> M4-B.2 ships the `AppCapabilityList` +
> `AppKeyValueList` data-owning four-state
> design-system components composing the
> M1.2 primitives, the `AppKeyValueListFormat`
> enum, the `Diagnostics/_Imports.razor`,
> 13 bUnit tests for `AppCapabilityList`,
> 15 bUnit tests for `AppKeyValueList`, and
> the `C-023 AppCapabilityList` + `C-024
> AppKeyValueList` capability records. The
> closeout commit is on `main`; the feature
> branch is fast-forwarded into `main` and
> deleted; push is staged for push (not
> authorised in this session).

## 1. Plan Reference

The M4-B.2 first session follows the M4-B
plan at
`.ai/plans/M4-B-capability-detection.md`
(Status: Awaiting Approval; the M4-B plan
promotion is T-023, Done 2026-07-13) and
the M4-B.2 implementation plan at
`.claude/plans/generic-seeking-oasis.md`
(the M4-B.2 implementation plan was
generated during the M4-B.2 plan session
and approved via ExitPlanMode on 2026-07-13).
The M4-B.2 implementation plan has 13
sections (Context, Goal, Approach, Files
to Add, Files to Modify, Files NOT
Touched, Critical Files to Read, Existing
Functions and Utilities to Reuse, Risks
and Mitigations, Validation, Implementation
Order, M4-B.2 vs M4-B Plan Alignment, Linked
Artefacts). The M4-B.2 implementation plan
is approved by the user; this implementation
report records the M4-B.2 closeout per the
approved plan.

## 2. Summary

The M4-B.2 first session (T-025) is **Done**
(2026-07-13).

M4-B.2 ships:

- **The `AppCapabilityList` data-owning
  four-state design-system component** at
  `src/AiEng.Platform.App/Components/Diagnostics/AppCapabilityList.razor`
  (+ `.razor.cs` + `.razor.css`); takes
  an `IReadOnlyList<HostCapability>` as
  the `Capabilities` parameter; renders
  an `AppStack` of `AppCard` entries by
  default; each card shows the `Key` +
  an `AppStatusDot` (Success for
  `Available=true`, Error for
  `Available=false`) + the `Version` in
  a monospaced muted font + an `AppBadge`
  "Credential set" (Success variant)
  when `CredentialAvailable=true`;
  exposes the four child-content slots
  (Loading, Empty, Error, Populated)
  per `docs/design-system.md` § 5.4;
  state machine is `IsLoading` →
  `ErrorMessage` → `Capabilities null /
  empty` → `Populated`; the populated
  list has `aria-live="polite"` and
  `role="list"`.
- **The `AppKeyValueList` data-owning
  four-state design-system component** at
  `src/AiEng.Platform.App/Components/Diagnostics/AppKeyValueList.razor`
  (+ `.razor.cs` + `.razor.css`); takes
  an `IReadOnlyList<KeyValuePair<string,
  string>>` as the `Items` parameter +
  an `AppKeyValueListFormat` enum
  (Plain, Boolean, Code) as the `Format`
  parameter; renders an `AppCard`
  containing a definition list
  (`<dl>`/`<dt>`/`<dd>`) of key-value
  rows; the `Format` parameter controls
  value rendering (Plain → literal
  text; Boolean → ✓ / ✗ icon for
  "true" / "false" (case-insensitive;
  non-boolean values render as literal
  text) with `aria-label` for screen
  readers; Code → monospaced `<code>`
  element); exposes the four
  child-content slots (Loading, Empty,
  Error, Populated); the populated
  container has `aria-live="polite"`.
- **The `AppKeyValueListFormat` enum**
  appended to
  `src/AiEng.Platform.App/Components/Common/Enums.cs`
  (three values: `Plain`, `Boolean`,
  `Code`).
- **The `Diagnostics/_Imports.razor`** at
  `src/AiEng.Platform.App/Components/Diagnostics/_Imports.razor`
  (mirrors the `Projects/_Imports.razor`
  pattern).
- **13 bUnit tests for `AppCapabilityList`**
  at
  `tests/AiEng.Platform.ComponentTests/Components/Diagnostics/AppCapabilityListTests.cs`.
- **15 bUnit tests for `AppKeyValueList`**
  at
  `tests/AiEng.Platform.ComponentTests/Components/Diagnostics/AppKeyValueListTests.cs`.
- **The `C-023 AppCapabilityList` +
  `C-024 AppKeyValueList` capability
  records** in
  `.ai/state/capabilities.json`.
- **The `C-015 IHostCapabilitiesService`
  capability record** in
  `.ai/state/capabilities.json` is updated
  (the `next_task` is set to `T-026` =
  M4-B.3 first session; the
  `last_updated` is set to `2026-07-13`).

## 3. Files Created

- `src/AiEng.Platform.App/Components/Diagnostics/AppCapabilityList.razor`
  (75 lines): the `AppCapabilityList`
  markup. `@inherits
  AppCapabilityListBase`; state machine
  renders one of four slots: Loading
  (default `<AppLoading><Label>Detecting
  host capabilities…</Label></AppLoading>`),
  Error (default `<AppErrorState>` with
  the `ErrorCode` + `CorrelationId` +
  `ErrorMessage`), Empty (default
  `<AppEmptyState><Title>No capabilities
  detected</Title><Description>The
  host capability detection has not
  detected any host tools. Verify the
  host tools are installed and try
  again.</Description></AppEmptyState>`),
  Populated (default
  `<AppStack Direction="Vertical"
  Gap="Medium">` of `<AppCard>` entries).
  Each card has a header (`Key` +
  `AppStatusDot`), a body (version meta
  + `AppBadge` "Credential set" when
  `CredentialAvailable=true`).
- `src/AiEng.Platform.App/Components/Diagnostics/AppCapabilityList.razor.cs`
  (40 lines): the `AppCapabilityListBase :
  ComponentBase` class with the
  parameters.
- `src/AiEng.Platform.App/Components/Diagnostics/AppCapabilityList.razor.css`
  (45 lines): the scoped CSS.
- `src/AiEng.Platform.App/Components/Diagnostics/AppKeyValueList.razor`
  (115 lines): the `AppKeyValueList`
  markup. `@inherits
  AppKeyValueListBase`; same state
  machine. The `FormatValue` private
  `RenderFragment` uses the builder API
  to switch on the `Format` parameter.
- `src/AiEng.Platform.App/Components/Diagnostics/AppKeyValueList.razor.cs`
  (40 lines): the `AppKeyValueListBase :
  ComponentBase` class with the
  parameters.
- `src/AiEng.Platform.App/Components/Diagnostics/AppKeyValueList.razor.css`
  (60 lines): the scoped CSS.
- `src/AiEng.Platform.App/Components/Diagnostics/_Imports.razor`
  (7 lines): the imports. Mirrors
  `Projects/_Imports.razor`.
- `tests/AiEng.Platform.ComponentTests/Components/Diagnostics/AppCapabilityListTests.cs`
  (250+ lines): the 13 bUnit tests +
  the file-scoped namespace +
  the `AppCapabilityListTests : BunitContext`
  class.
- `tests/AiEng.Platform.ComponentTests/Components/Diagnostics/AppKeyValueListTests.cs`
  (270+ lines): the 15 bUnit tests +
  the file-scoped namespace +
  the `AppKeyValueListTests : BunitContext`
  class.
- `.ai/handoffs/2026-07-13-m4-b-2-capability-list-components.md`
  (the M4-B.2 per-session handoff;
  mirrored to `.ai/handoffs/latest.md`).
- `implementation-report-m4-b-2-capability-list-components.md`
  (this report).

## 4. Files Modified

- `src/AiEng.Platform.App/Components/Common/Enums.cs`:
  appended the `AppKeyValueListFormat`
  enum after the existing
  `AppCardVariant` enum. The enum has
  three values (`Plain`, `Boolean`,
  `Code`); mirrors the M1.2 enum pattern.

## 5. Files Deleted

None.

## 6. Architecture

### 6.1 Component pattern (data-owning four-state)

M4-B.2 implements the **data-owning
four-state** component pattern per
`docs/design-system.md` § 5.4. The
pattern is:

1. The component takes a
   `T : IReadOnlyList<…>` parameter as
   the data envelope.
2. The component takes a `bool IsLoading`
   + a `string? ErrorMessage` + a
   `string? ErrorCode` + a `string?
   CorrelationId` parameter for the
   loading / error state.
3. The component takes four
   `RenderFragment?` parameters for
   the four child-content slots
   (Loading, Empty, Error, Populated).
4. The state machine renders one of the
   four slots:
   - `IsLoading` is `true` → render
     the `Loading` slot (or the default
     `AppLoading`).
   - `ErrorMessage` is set → render the
     `Error` slot (or the default
     `AppErrorState` with the
     `ErrorCode` + `CorrelationId`).
   - The data is null or empty → render
     the `Empty` slot (or the default
     `AppEmptyState`).
   - Else → render the `Populated` slot
     (or the default rendering).
5. The state machine is
   `IsLoading → ErrorMessage → Empty →
   Populated` (loading wins over error;
   error wins over empty; empty wins
   over populated).
6. The component is accessible
   (`aria-live="polite"` on the populated
   list / container; `role="list"` on the
   `AppCapabilityList` populated list).

### 6.2 Component composition (M1.2 primitives)

The `AppCapabilityList` composes the
M1.2 primitives:

- **`AppStack`** with `Direction="Vertical"`
  + `Gap="Medium"` for the vertical list
  of cards.
- **`AppCard`** for each capability entry.
- **`AppStatusDot`** with `Variant="Success"`
  for `Available=true` and `Variant="Error"`
  for `Available=false`; the `Label` is
  the human-readable state (e.g.,
  "Available", "Not available").
- **`AppBadge`** with `Variant="Success"`
  for the "Credential set" badge.
- **`AppLoading`** + **`AppEmptyState`** +
  **`AppErrorState`** for the four-state
  fallback components.

The `AppKeyValueList` composes:

- **`AppCard`** for the list container.
- **`AppLoading`** + **`AppEmptyState`** +
  **`AppErrorState`** for the four-state
  fallback components.

### 6.3 Enum pattern (M1.2)

The `AppKeyValueListFormat` enum follows
the M1.2 enum pattern:

- One `enum` per category (the
  `AppKeyValueListFormat` is the
  `KeyValueList` category).
- No `[Flags]` attribute.
- No `[Description]` attribute.
- Three values: `Plain`, `Boolean`, `Code`.

### 6.4 Imports pattern (M1.2)

The `Diagnostics/_Imports.razor` follows
the `Projects/_Imports.razor` pattern:

- `@using AiEng.Platform.App.Components.Common`
  (for the `AppKeyValueListFormat` enum
  + the `AppBadgeVariant` enum).
- `@using AiEng.Platform.App.Components.Primitive`
  (for `AppStatusDot` + `AppBadge`).
- `@using AiEng.Platform.App.Components.Layout`
  (for `AppCard` + `AppStack`).
- `@using AiEng.Platform.App.Components.Display`
  (for any display primitives — not
  composed in M4-B.2 but imported for
  future use).
- `@using AiEng.Platform.App.Components.Feedback`
  (for `AppLoading` + `AppEmptyState` +
  `AppErrorState`).
- `@using AiEng.Platform.Application.Capabilities`
  (for the `HostCapability` record).
- `@namespace
  AiEng.Platform.App.Components.Diagnostics`.

### 6.5 Test pattern (bUnit 2.7.2)

The bUnit tests follow the M1.2 test
pattern:

- File-scoped namespace
  `AiEng.Platform.ComponentTests.Components.Diagnostics`.
- `public class AppCapabilityListTests :
  BunitContext` + `public class
  AppKeyValueListTests : BunitContext`
  (bUnit 2.x style; no `[TestContext]`).
- `[Fact]` test methods
  (xUnit 2.9.3).
- `Render<AppCapabilityList>(parameters
  => parameters.Add(...))` +
  `Render<AppKeyValueList>(parameters
  => parameters.Add(...))` (bUnit
  `Render<TComponent>(Action<ComponentParameterCollectionBuilder<TComponent>>)`).
- `cut.FindAll(".app-capability-list-item")`
  + `cut.Find(".app-capability-list-boolean-true")`
  + `cut.Markup` for the rendered
  markup.
- `RenderFragment` lambda built with
  the builder API for the custom-slot
  tests.
- `cut.Find("[role=\"list\"]")` +
  `cut.Find("[data-state=\"populated\"]")`
  + `Assert.Equal("polite", list.GetAttribute("aria-live"))`
  for the aria-live tests.

## 7. Validation Results

### 7.1 Format

`dotnet format --verify-no-changes
AiEng.Platform.slnx` exited with code 0.
The format is canonical; the new files
use 4-space indent + CRLF (per
`.editorconfig`); the modified
`Enums.cs` retains the existing format.

### 7.2 Build

`dotnet build AiEng.Platform.slnx`
exited with code 0. The build produced
0 warnings, 0 errors (with
`TreatWarningsAsErrors=true` from
`Directory.Build.props`). The 9
projects all built successfully:

- `AiEng.Platform.Domain`
- `AiEng.Platform.Providers.Abstractions`
- `AiEng.Platform.Application`
- `AiEng.Platform.Infrastructure`
- `AiEng.Platform.UnitTests`
- `AiEng.Platform.App`
- `AiEng.Platform.ArchitectureTests`
- `AiEng.Platform.ComponentTests`

### 7.3 Test

`dotnet test AiEng.Platform.slnx
--no-build` reported **370 passed**,
0 failed, 9 skipped (per ADR-016 / M4-D).
Breakdown: 99 unit (unchanged) + 259
component (was 231 pre-M4-B.2; +28 new
M4-B.2 bUnit tests: 13 for
`AppCapabilityList` + 15 for
`AppKeyValueList`) + 12 architecture
(unchanged; 0 new architecture tests
in M4-B.2; the
`Capabilities_Resolved_Through_Service`
architecture test is still deferred to
M4-B.3 per the M4-B.1 plan § 14.1
Deviations).

The 9 skipped tests are: 3
`AxeCoreAuditTests` (activate in M4-D)
+ 4 `CompositionRootBoundaryTests`
(activate in M4-D) + 2
`Infrastructure_Respects_*` (activate
in M4-D).

### 7.4 JSON validation

`capabilities.json` is valid JSON
(verified with `node -e "const data =
JSON.parse(fs.readFileSync('.ai/state/capabilities.json','utf8')); …"`).
The file has 24 records (the pre-M4-B.2
total was 22; +2 new M4-B.2 records:
C-023 `AppCapabilityList` + C-024
`AppKeyValueList`). The top-level
`updated_at` is set to
`2026-07-13T12:00:00Z`; the top-level
`updated_by_session` is set to
`m4-b-2-capability-list-components`.

### 7.5 CRLF

All new + modified files are CRLF
(`unix2dos` applied). The new files are:
`AppCapabilityList.razor` +
`AppCapabilityList.razor.cs` +
`AppCapabilityList.razor.css` +
`AppKeyValueList.razor` +
`AppKeyValueList.razor.cs` +
`AppKeyValueList.razor.css` +
`Diagnostics/_Imports.razor` +
`AppCapabilityListTests.cs` +
`AppKeyValueListTests.cs`. The modified
file is `Enums.cs`. The state file
`capabilities.json` is also CRLF.

### 7.6 Architecture boundary

The M4-B.2 implementation does not
introduce `System.Diagnostics.Process`
usage outside
`src/AiEng.Platform.Infrastructure/`;
the M4-B.2 implementation does not
introduce `advapi32.dll` P/Invoke
outside `src/AiEng.Platform.Infrastructure/`;
the M4-B.2 implementation does not
introduce a `Microsoft.Extensions.DependencyInjection`
`IServiceCollection` extension outside
`src/AiEng.Platform.App/Composition/`.
The boundary is enforced by the M4-A.1
architecture tests
(`Infrastructure_Respects_ProcessBoundary`
+ `Infrastructure_Respects_CredentialBoundary`),
which are registered-but-disabled per
ADR-016 / M4-D.

### 7.7 No scope creep

The diff does not modify any file under
`src/AiEng.Platform.Application/Capabilities/`,
`src/AiEng.Platform.Infrastructure/Capabilities/`,
`src/AiEng.Platform.App/Composition/`,
`src/AiEng.Platform.App/Components/Pages/`,
`src/AiEng.Platform.App/Program.cs`,
`src/AiEng.Platform.Providers.Abstractions/`,
`src/AiEng.Platform.Domain/`, `docs/`,
`ROADMAP.md`, `.ai/plans/`, `AGENTS.md`,
`ARCHITECTURE.md`, `DECISIONS.md`,
`STYLEGUIDE.md`, `CONTRIBUTING.md`,
`.ai/workflows/`, `tailwind.config.js`,
`package.json`, or
`Directory.Build.props`. The diff
touches only:

- New files in
  `src/AiEng.Platform.App/Components/Diagnostics/`
  (7 files: 6 source files +
  `_Imports.razor`).
- New files in
  `tests/AiEng.Platform.ComponentTests/Components/Diagnostics/`
  (2 files: `AppCapabilityListTests.cs`
  + `AppKeyValueListTests.cs`).
- Modified file
  `src/AiEng.Platform.App/Components/Common/Enums.cs`
  (1 enum appended).
- Modified state files
  (`.ai/state/session.json` +
  `.ai/state/tasks.json` +
  `.ai/state/current.md` +
  `.ai/state/task-board.md` +
  `.ai/state/milestones.json` +
  `.ai/state/capabilities.json`).
- New handoff
  (`.ai/handoffs/2026-07-13-m4-b-2-capability-list-components.md`).
- New implementation report
  (`implementation-report-m4-b-2-capability-list-components.md`).

## 8. Tests Added

M4-B.2 ships **28 new bUnit tests**:

- **13 bUnit tests for `AppCapabilityList`**
  in
  `tests/AiEng.Platform.ComponentTests/Components/Diagnostics/AppCapabilityListTests.cs`:
  - `Renders_Populated_Slot_With_One_Card_Per_Capability`
  - `Renders_Empty_Slot_When_No_Capabilities`
  - `Renders_Loading_Slot_When_IsLoading_Is_True`
  - `Renders_Error_Slot_When_ErrorMessage_Is_Set`
  - `Renders_Status_Dot_Success_For_Available_True`
  - `Renders_Status_Dot_Error_For_Available_False`
  - `Renders_Credential_Set_Badge_When_CredentialAvailable_Is_True`
  - `Omits_Credential_Set_Badge_When_CredentialAvailable_Is_False`
  - `Custom_Populated_Slot_Overrides_Default_Rendering`
  - `Custom_Loading_Slot_Overrides_Default_Loading`
  - `Custom_Empty_Slot_Overrides_Default_Empty`
  - `Custom_Error_Slot_Overrides_Default_Error`
  - `Populated_List_Has_AriaLive_Polite`
- **15 bUnit tests for `AppKeyValueList`**
  in
  `tests/AiEng.Platform.ComponentTests/Components/Diagnostics/AppKeyValueListTests.cs`:
  - `Renders_Populated_Slot_With_One_Row_Per_Item`
  - `Renders_Empty_Slot_When_No_Items`
  - `Renders_Loading_Slot_When_IsLoading_Is_True`
  - `Renders_Error_Slot_When_ErrorMessage_Is_Set`
  - `Boolean_Format_Renders_Check_Icon_For_True_Value`
  - `Boolean_Format_Renders_Cross_Icon_For_False_Value`
  - `Boolean_Format_Renders_Literal_Text_For_Non_Boolean_Value`
  - `Code_Format_Renders_Value_In_Code_Element`
  - `Plain_Format_Renders_Literal_Text`
  - `Custom_Populated_Slot_Overrides_Default_Rendering`
  - `Custom_Loading_Slot_Overrides_Default_Loading`
  - `Custom_Empty_Slot_Overrides_Default_Empty`
  - `Custom_Error_Slot_Overrides_Default_Error`
  - `Populated_List_Has_AriaLive_Polite`

## 9. Definition of Done

Every item in the M4-B.2 scope is
checked:

- [x] **The `AppCapabilityList` component
  is at
  `src/AiEng.Platform.App/Components/Diagnostics/AppCapabilityList.razor`
  (+ `.razor.cs` + `.razor.css`).**
- [x] **The `AppKeyValueList` component
  is at
  `src/AiEng.Platform.App/Components/Diagnostics/AppKeyValueList.razor`
  (+ `.razor.cs` + `.razor.css`).**
- [x] **The `AppKeyValueListFormat` enum
  is appended to
  `src/AiEng.Platform.App/Components/Common/Enums.cs`.**
- [x] **The `Diagnostics/_Imports.razor`
  is at
  `src/AiEng.Platform.App/Components/Diagnostics/_Imports.razor`.**
- [x] **13 bUnit tests for
  `AppCapabilityList` are at
  `tests/AiEng.Platform.ComponentTests/Components/Diagnostics/AppCapabilityListTests.cs`.**
- [x] **15 bUnit tests for
  `AppKeyValueList` are at
  `tests/AiEng.Platform.ComponentTests/Components/Diagnostics/AppKeyValueListTests.cs`.**
- [x] **The `C-023 AppCapabilityList` +
  `C-024 AppKeyValueList` capability
  records are in
  `.ai/state/capabilities.json`.**
- [x] **The `C-015 IHostCapabilitiesService`
  capability record is updated
  (`next_task` set to `T-026` =
  M4-B.3 first session; `last_updated`
  set to `2026-07-13`).**
- [x] **The `AppCapabilityList` supports
  the four child-content slots (Loading,
  Empty, Error, Populated) per
  `docs/design-system.md` § 5.4.**
- [x] **The `AppKeyValueList` supports
  the four child-content slots (Loading,
  Empty, Error, Populated) per
  `docs/design-system.md` § 5.4.**
- [x] **The `AppCapabilityList` renders
  the `AppStatusDot` Success for
  `Available=true` and Error for
  `Available=false`.**
- [x] **The `AppCapabilityList` renders
  the `AppBadge` "Credential set" for
  `CredentialAvailable=true`.**
- [x] **The `AppKeyValueList` `Format`
  parameter supports Plain, Boolean,
  and Code values.**
- [x] **The `AppKeyValueList` Boolean
  format renders ✓ for "true" and ✗ for
  "false" (case-insensitive; non-boolean
  values render as literal text).**
- [x] **The `AppKeyValueList` Code
  format renders the value in a
  monospaced `<code>` element.**
- [x] **The `AppCapabilityList` populated
  list has `aria-live="polite"` and
  `role="list"`.**
- [x] **The `AppKeyValueList` populated
  container has `aria-live="polite"`.**
- [x] **Validation gate: format clean
  (`dotnet format --verify-no-changes`
  exit 0); build clean (0 warnings, 0
  errors); test green (370 passed, 0
  failed, 9 skipped).**
- [x] **All new + modified files are
  CRLF (`unix2dos` applied).**
- [x] **`capabilities.json` is valid
  JSON (24 records; C-023 + C-024
  added).**
- [x] **No scope creep: the diff does
  not modify any file outside the
  M4-B.2 scope.**
- [x] **The project-continuity state is
  updated per Rule 15 (session.json,
  tasks.json, current.md, task-board.md,
  milestones.json, capabilities.json).**
- [x] **The M4-B.2 per-session handoff
  is at
  `.ai/handoffs/2026-07-13-m4-b-2-capability-list-components.md`
  (mirrored to `.ai/handoffs/latest.md`).**
- [x] **The M4-B.2 implementation report
  is at
  `implementation-report-m4-b-2-capability-list-components.md`.**
- [x] **The M4-B.2 closeout commit
  `feat(m4-b.2): add AppCapabilityList +
  AppKeyValueList data-owning design-system
  components` is on `main`.**
- [x] **The M4-B.2 feature branch
  `feature/T-025-m4-b-2-capability-list-components`
  is fast-forwarded into `main` and
  deleted.**
- [x] **Push is staged for push (not
  authorised in this session).**
- [x] **Stop. The M4-B.2 first session
  does NOT begin M4-B.3 (page + startup
  log + documentation + architecture
  test) / M4-C / M4-D / any provider
  creation (per the brief: "Do not begin
  the following task" and the Progressive
  Coding Rule).**

## 10. Git

- **Branch:**
  `feature/T-025-m4-b-2-capability-list-components`
  (created from `main` at the M4-B.1
  closeout commit `c151e90`; the M4-B.2
  closeout commit
  `feat(m4-b.2): add AppCapabilityList +
  AppKeyValueList data-owning design-system
  components` is on this branch; the
  branch is fast-forwarded into `main`
  per the branching strategy rule 6; the
  branch is deleted per rule 7).
- **Base commit:** `c151e90` (the M4-B.1
  closeout commit
  `feat(m4-b.1): add IHostCapabilitiesService
  contract and SystemHostCapabilitiesService
  implementation` on `main`).
- **Closeout commit:** `feat(m4-b.2): add
  AppCapabilityList + AppKeyValueList
  data-owning design-system components`
  (body empty; trailer
  `Co-Authored-By: Claude <noreply@anthropic.com>`).
- **Push:** **staged for push** (not
  authorised in this session; the next
  user command may push the M4-B.2
  closeout commit per the command
  protocol).
- **Default branch HEAD at closeout:**
  the M4-B.2 closeout commit on `main`;
  the feature branch is fast-forwarded
  into `main` and deleted.

## 11. Out of Scope

M4-B.2 does not ship:

- The `/diagnostics` page
  (`Diagnostics.razor` + the
  `AppDiagnosticsPage` composition in
  `App/Components/Pages/`). M4-B.3's
  responsibility.
- The startup capability-report log
  through `ILogger<Program>`. M4-B.3's
  responsibility.
- The `docs/capabilities.md`
  documentation. M4-B.3's responsibility.
- The
  `Capabilities_Resolved_Through_Service`
  architecture test. M4-B.3's
  responsibility (deferred from M4-B.1
  per the M4-B.1 plan § 14.1 Deviations;
  the test asserts `Diagnostics.razor`
  contains `@inject
  IHostCapabilitiesService`;
  `Diagnostics.razor` does not exist in
  M4-B.2).
- The `docs/design-system.md` § 4.5
  `AppCapabilityList` + `AppKeyValueList`
  row update from "Planned (M4)" to
  "Implemented (M4-B.2)". Deferred
  decision per the M4-B.2 plan § 11
  Documentation Plan § 3.
- Provider creation. M4-B detects
  capabilities, does not create
  providers (per the M4-B brief: "Do not
  create providers"). M4-C's
  responsibility.
- The M4-C plan promotion. Not in
  M4-B.2's scope.
- The M4-D plan promotion. Not in
  M4-B.2's scope.
- M4-A.3 (the deferred M4-A slice). Not
  in M4-B.2's scope.

## 12. Lessons Learned

1. **The data-owning four-state pattern
   is a stable contract.** The
   `AppCapabilityList` + `AppKeyValueList`
   follow the same pattern as
   `AppProjectList` (the closest M1.2
   data-owning precedent). The four
   child-content slots (Loading, Empty,
   Error, Populated) + the state machine
   (`IsLoading → ErrorMessage → Empty →
   Populated`) + the `data-state="…"`
   attribute on the populated container
   are the data-owning contract. The
   `AppCapabilityList` adds `role="list"`
   on the populated list (a list
   semantic) and the `AppKeyValueList`
   uses semantic HTML `<dl>`/`<dt>`/`<dd>`
   (a definition list semantic). The
   state machine is identical; the
   rendered semantic is component-specific.
2. **The bUnit 2.x test pattern is
   stable.** The `BunitContext` base
   class + the `Render<TComponent>(parameters
   => parameters.Add(...))` API + the
   `cut.Find(...)` / `cut.FindAll(...)`
   / `cut.Markup` API + the
   `RenderFragment` lambda builder API
   compose a stable test pattern. The
   13 + 15 M4-B.2 tests follow the M1.2
   test pattern (e.g.,
   `AppPageHeaderTests` +
   `AppEmptyStateTests`); the tests
   read like the surrounding tests.
3. **The `AppKeyValueListFormat` enum is
   a small but expressive addition.**
   The three values (Plain, Boolean,
   Code) cover the three common
   key-value-rendering scenarios: literal
   text, boolean, code/identifier. The
   `Boolean` format is a UX win: a
   `key="IsAdmin"`, `value="true"` row
   renders a check icon instead of the
   literal text "true", which is more
   scannable for the user. The `Code`
   format is a UX win: a
   `key="CorrelationId"`,
   `value="abc-123"` row renders in a
   monospaced font, which signals
   "machine-readable identifier".
4. **The `unix2dos` step is mechanical
   but mandatory.** All new +
   modified files are CRLF; the
   `.editorconfig` rule + the
   `TreatWarningsAsErrors=true` build
   + the `dotnet format --verify-no-changes`
   gate enforce the format. The
   `unix2dos` step is applied to every
   new file before commit; the
   `Enums.cs` modification is also
   CRLF.
5. **The C-023 + C-024 capability
   records are the first two
   `DesignSystem` capabilities.** The
   pre-M4-B.2 capabilities are
   `DomainOrchestration` (the majority)
   + `Infrastructure` (a few). The M4-B.2
   capability records are the first two
   `DesignSystem` capabilities, which
   establishes the `DesignSystem`
   category for future component
   capabilities.

## 13. Handoff to M4-B.3

The M4-B.3 first session (T-026) is
**Ready** (created 2026-07-13; promoted
to Ready by the
`m4-b-2-capability-list-components`
session; depends on T-025; depends on
C-015 + C-023 + C-024). M4-B.3 ships:

- The `/diagnostics` page
  (`Diagnostics.razor` in
  `src/AiEng.Platform.App/Components/Pages/`)
  + the `AppDiagnosticsPage` composition
  (composes the new `AppCapabilityList`
  + `AppKeyValueList` components).
- The startup capability-report log
  through `ILogger<Program>`.
- The `docs/capabilities.md`
  documentation.
- The
  `Capabilities_Resolved_Through_Service`
  architecture test (deferred from
  M4-B.1 per the M4-B.1 plan § 14.1
  Deviations; the test asserts
  `Diagnostics.razor` contains `@inject
  IHostCapabilitiesService`; the test
  also asserts no `RunToCompletionAsync`
  token + no `ICredentialVault` direct
  call in `App/Components/Diagnostics/`).
- 3+ bUnit tests for the
  `/diagnostics` page.

The M4-B.3 first session does **not**
create providers (per the M4-B brief:
"Do not create providers"). The M4-B.3
first session is the third M4-B
implementation slice; the M4-B.3 first
session is the **activation** of the
M4-B (the page + the startup log +
the documentation + the architecture
test). The M4-B.3 first session depends
on the M4-B.1 + M4-B.2 components being
in place; the M4-B.3 first session is
the smallest M4-B.3 implementation
slice that produces a working
`/diagnostics` page.

The M4-B.3 first session does **not**
begin M4-C (provider registry) /
M4-D (first concrete process providers)
/ any provider creation (per the brief
and the Progressive Coding Rule). The
next session is the M4-B.3 first
session on the user's `Approve` or
`Next` invocation.

## 14. Deviations

1. **The `AppCapabilityList` default
   Populated layout uses an `AppStack` of
   `AppCard` entries with the
   `AppStatusDot` in the card header,
   and the `AppKeyValueList` default
   Populated uses a definition list
   (`<dl>`/`<dt>`/`<dd>`) with monospaced
   `<code>` elements for the Code
   format.** The plan's intent (one card
   per capability with status dot +
   version + credential badge; one row
   per item with key left + value right)
   is preserved. The exact composition
   (which header element holds the
   status dot, where the credential
   badge is positioned, how the
   definition list is laid out) is an
   implementation detail; the components
   follow the M1.2 primitives'
   conventions and the
   `docs/design-system.md` § 5.4
   data-owning contract. The deviation
   is recorded in the M4-B.2 handoff
   section 3 + the M4-B.2
   `milestones.json` `deviations` array
   + the M4-B.2 task record `notes`.
2. **The `CapabilityProbe` internal
   record types from M4-B.1 are not
   composed in the M4-B.2 components.**
   The M4-B.2 components consume the
   public `IReadOnlyList<HostCapability>`
   (the data envelope) not the internal
   `CapabilityProbe` records (the probe
   definition envelope). The
   `CapabilityProbe` records are
   `internal` to the
   `AiEng.Platform.Infrastructure`
   assembly and are an implementation
   detail of the M4-B.1
   `SystemHostCapabilitiesService`. The
   M4-B.2 components do not need to
   compose them; the M4-B.3
   `/diagnostics` page will consume the
   public `IHostCapabilitiesService` +
   the `HostCapabilities` +
   `HostCapability` records, which is
   the public contract every later
   consumer composes.
3. **The `docs/design-system.md` § 4.5
   `AppCapabilityList` + `AppKeyValueList`
   row update from "Planned (M4)" to
   "Implemented (M4-B.2)" is deferred.**
   The M4-B.2 plan § 11 Documentation
   Plan § 3 mentions the § 4.5 update
   as a "decision deferred to the M4-B.2
   first session closeout". The M4-B.2
   first session closeout decides to
   **not** update § 4.5 in M4-B.2: the
   § 4.5 update is a documentation
   surface change, and the M4-B.2 first
   session is a code change. The
   documentation update will happen in
   the M4-B.3 first session (the
   activation slice) when the
   `docs/capabilities.md` documentation
   is also updated; the M4-B.3 first
   session will update § 4.5 + write
   `docs/capabilities.md` as a coherent
   documentation surface change. The
   deviation is recorded here for
   transparency.

## 15. Cross-References

- The M4-B plan: `.ai/plans/M4-B-capability-detection.md`
  (Status: Awaiting Approval; canonical
  M4-B scope).
- The M4-B.2 implementation plan: `.claude/plans/generic-seeking-oasis.md`
  (approved via ExitPlanMode on 2026-07-13).
- The M4-B.1 handoff: `.ai/handoffs/2026-07-13-m4-b-1-host-capabilities-contract-and-service.md`
  (the M4-B.2 handoff's template).
- The M4-B.1 implementation report: `implementation-report-m4-b-1-host-capabilities-contract-and-service.md`
  (the M4-B.2 implementation report's
  template).
- The M4-B.1 contract: `src/AiEng.Platform.Application/Capabilities/IHostCapabilitiesService.cs`
  (the M4-B.2 components consume this
  contract via the `IReadOnlyList<HostCapability>`
  parameter).
- The M4-B.1 records: `src/AiEng.Platform.Application/Capabilities/HostCapabilities.cs`
  (the `HostCapability` record is the
  data envelope the `AppCapabilityList`
  takes as the `Capabilities` parameter).
- The M4-B.1 implementation: `src/AiEng.Platform.Infrastructure/Capabilities/SystemHostCapabilitiesService.cs`
  (the M4-B.2 does not modify this; the
  M4-B.3 `/diagnostics` page consumes
  the result of `DetectAsync`).
- The M4-B.1 composition root: `src/AiEng.Platform.App/Composition/Capabilities/CapabilitiesServiceCollectionExtensions.cs`
  (the M4-B.2 does not modify this).
- The M1.2 design-system docs: `docs/design-system.md` § 5.4
  (the data-owning four-state rule).
- The M1.2 patterns: `src/AiEng.Platform.App/Components/Projects/AppProjectList.razor`
  + `AppProjectCard.razor` +
  `_Imports.razor`.
- The M1.2 primitives: `AppEmptyState`
  + `AppErrorState` + `AppLoading` +
  `AppCard` + `AppStack` + `AppPanel` +
  `AppStatusDot` + `AppBadge` +
  `AppButton`.
- The bUnit test patterns: `tests/AiEng.Platform.ComponentTests/Components/Display/AppPageHeaderTests.cs`
  + `tests/AiEng.Platform.ComponentTests/Components/Feedback/AppEmptyStateTests.cs`.
- The branching strategy: `.ai/workflows/branching-strategy.md`
  (rules 4, 6, 7 are the M4-B.2's branch
  operations).
- The Progressive Coding Rule: `.ai/workflows/progressive-coding.md`
  (the rule the M4-B.2 first session
  follows).
- The command protocol: `.ai/commands.md`
  (the `Next` command response shape —
  `Completed / Git / Validation /
  Evidence / Next`).
- The M4-B.2 task record: `.ai/state/tasks.json`
  T-025 (the M4-B.2 task transitions
  `Ready` → `InProgress` → `Done`).
- The M4-B.2 milestone record: `.ai/state/milestones.json`
  (the M4-B slice block M4-B.2 is
  added; the M4-B.2 `status` is `Done`).
- The M4-B.2 capability records: `.ai/state/capabilities.json`
  C-023 (AppCapabilityList) + C-024
  (AppKeyValueList).
- The M4-B.2 session record: `.ai/state/session.json`
  (the M4-B.2 envelope is set).
- The M4-B.2 task board entry: `.ai/state/task-board.md`
  (M4-B.2 row in `Done Recently`; the
  M4-B.2 row in `Ready` replaces the
  M4-B.1 row).
- The M4-B.2 one-page snapshot: `.ai/state/current.md`
  (active slice = M4-B.2; last completed
  task = T-025; next recommended task =
  T-026 = M4-B.3 first session).

---

**End of M4-B.2 implementation report.**
The M4-B.2 first session is the second
boundary slice of M4-B; the closeout
commit `feat(m4-b.2): add
AppCapabilityList + AppKeyValueList
data-owning design-system components` is
on `main`; the feature branch is
fast-forwarded into `main` and deleted.
The handoff is at
`.ai/handoffs/2026-07-13-m4-b-2-capability-list-components.md`
(mirrored to `.ai/handoffs/latest.md`).
The task record is at `.ai/state/tasks.json`
T-025. The M4-B.3 first session is the
next step on the user's `Approve` or
`Next` invocation.
