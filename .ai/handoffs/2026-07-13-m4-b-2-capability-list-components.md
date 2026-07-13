# Handoff — M4-B.2 AppCapabilityList + AppKeyValueList Data-Owning Design-System Components — `m4-b-2-capability-list-components` (2026-07-13)

> **The M4-B.2 per-session handoff.** M4-B.2
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
> AppKeyValueList` capability records.
>
> M4-B.2 is the **second boundary slice**,
> not the activation. The `/diagnostics`
> page (Diagnostics.razor) + the startup
> capability-report log (ILogger<Program>)
> + the `docs/capabilities.md`
> documentation + the
> `Capabilities_Resolved_Through_Service`
> architecture test are M4-B.3's
> responsibility (the architecture test
> was deferred from M4-B.1 to M4-B.3 per
> the M4-B.1 plan section 14.1 Deviations
> — the test asserts `Diagnostics.razor`
> contains `@inject
> IHostCapabilitiesService`, and
> `Diagnostics.razor` does not exist in
> M4-B.2). M4-B.2 does **not** create
> providers (per the M4-B brief: "Do not
> create providers").

---

## 1. What was delivered

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
  `Code`; mirrors the M1.2 enum pattern;
  no `Flags` attribute; no `Description`
  attribute).
- **The `Diagnostics/_Imports.razor`** at
  `src/AiEng.Platform.App/Components/Diagnostics/_Imports.razor`
  (mirrors the `Projects/_Imports.razor`
  pattern: `@using
  AiEng.Platform.App.Components.Common` +
  `Primitive` + `Layout` + `Display` +
  `Feedback` + `@using
  AiEng.Platform.Application.Capabilities`
  for the `HostCapability` record
  + `@namespace
  AiEng.Platform.App.Components.Diagnostics`).
- **13 bUnit tests for `AppCapabilityList`**
  at
  `tests/AiEng.Platform.ComponentTests/Components/Diagnostics/AppCapabilityListTests.cs`
  (Populated slot with one card per
  capability; Empty slot when no
  capabilities; Loading slot when
  `IsLoading=true`; Error slot when
  `ErrorMessage` is set; `AppStatusDot`
  Success for `Available=true`;
  `AppStatusDot` Error for
  `Available=false`; `AppBadge` "Credential
  set" for `CredentialAvailable=true`;
  omits the badge when
  `CredentialAvailable=false`; custom
  Populated / Loading / Empty / Error
  slots override the default rendering;
  populated list has `aria-live="polite"`).
- **15 bUnit tests for `AppKeyValueList`**
  at
  `tests/AiEng.Platform.ComponentTests/Components/Diagnostics/AppKeyValueListTests.cs`
  (Populated slot with one row per item;
  Empty slot when no items; Loading slot
  when `IsLoading=true`; Error slot when
  `ErrorMessage` is set; Boolean format
  check for "true"; Boolean format
  cross for "false"; Boolean format
  literal text for non-boolean values;
  Code format in `<code>` element; Plain
  format literal text; custom Populated /
  Loading / Empty / Error slots override
  the default rendering; populated
  container has `aria-live="polite"`).
- **The `C-023 AppCapabilityList` +
  `C-024 AppKeyValueList` capability
  records** in
  `.ai/state/capabilities.json` (status:
  `Accepted`; `completion_status`:
  `Delivered`; `delivered_by_milestone`:
  `M4-B`; `consumed_by_milestones`:
  `M4-B.3` + `M5` + `M6` + `M7` + `M8`;
  full evidence block populated with the
  new files + commit + tests; the
  `next_task` is `null` because the
  components are delivered; the
  acceptance criteria are populated and
  the completed criteria are populated).
- **The `C-015 IHostCapabilitiesService`
  capability record** in
  `.ai/state/capabilities.json` is updated
  (the `next_task` is set to `T-026` =
  M4-B.3 first session; the
  `last_updated` is set to `2026-07-13`).

## 2. Test and build status

- **Format:** `dotnet format
  --verify-no-changes` passed.
- **Build:** 0 warnings, 0 errors.
- **Test:** **370 passed**, 0 failed,
  9 skipped (per ADR-016 / M4-D). The
  M4-B.2 ships 28 new bUnit tests (13
  for `AppCapabilityList` + 15 for
  `AppKeyValueList`); the pre-M4-B.2
  baseline was 343 passed (99 unit +
  232 bUnit + 12 architecture). The
  post-M4-B.2 total is 370 passed (99
  unit + 259 bUnit + 12 architecture).
  The 9 skipped are: 3
  `AxeCoreAuditTests` (activate in M4-D)
  + 4 `CompositionRootBoundaryTests`
  (activate in M4-D) + 2
  `Infrastructure_Respects_*` (activate
  in M4-D).
- **Architecture tests:** 0 new
  architecture tests. The M4-B plan
  § 2 In Scope § 9
  `Capabilities_Resolved_Through_Service`
  architecture test is still deferred to
  M4-B.3 (the test asserts
  `Diagnostics.razor` contains `@inject
  IHostCapabilitiesService`;
  `Diagnostics.razor` does not exist in
  M4-B.2). This is consistent with the
  M4-B.1 plan § 14.1 Deviations and the
  M4-B plan.
- **CRLF:** all new + modified files are
  CRLF (`unix2dos` applied to
  `AppCapabilityList.razor` +
  `.razor.cs` + `.razor.css`,
  `AppKeyValueList.razor` + `.razor.cs`
  + `.razor.css`, `Diagnostics/_Imports.razor`,
  `AppCapabilityListTests.cs`,
  `AppKeyValueListTests.cs`,
  `Enums.cs`, and `capabilities.json`).
- **JSON validation:**
  `capabilities.json` is valid (24
  records; C-023 + C-024 added;
  `updated_at` set to
  `2026-07-13T12:00:00Z`;
  `updated_by_session` set to
  `m4-b-2-capability-list-components`).

## 3. Deviations

1. **The `AppCapabilityList` default
   Populated layout uses an `AppStack` of
   `AppCard` entries with the `AppStatusDot`
   in the card header, and the
   `AppKeyValueList` default Populated uses
   a definition list (`<dl>`/`<dt>`/`<dd>`)
   with monospaced `<code>` elements for
   the Code format.** The plan's intent
   (one card per capability with status
   dot + version + credential badge; one
   row per item with key left + value
   right) is preserved. The exact
   composition (which header element holds
   the status dot, where the credential
   badge is positioned, how the definition
   list is laid out) is an implementation
   detail; the components follow the M1.2
   primitives' conventions and the
   `docs/design-system.md` § 5.4
   data-owning contract.
2. **The `CapabilityProbe` internal record
   types from M4-B.1 are not composed in
   the M4-B.2 components.** The M4-B.2
   components consume the public
   `IReadOnlyList<HostCapability>` (the
   data envelope) not the internal
   `CapabilityProbe` records (the
   probe definition envelope). The
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

## 4. Files added

### 4.1 New design-system components

- `src/AiEng.Platform.App/Components/Diagnostics/AppCapabilityList.razor`
  (75 lines): the
  `AppCapabilityList` markup. `@inherits
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
  parameters. `[Parameter, EditorRequired]
  IReadOnlyList<HostCapability>
  Capabilities { get; set; }` + the four
  child-content slots (Loading, Empty,
  Error, Populated) + the loading / error
  state parameters (IsLoading, ErrorMessage,
  ErrorCode, CorrelationId) + the
  `AdditionalAttributes` capture-all
  parameter. Uses `using
  AiEng.Platform.Application.Capabilities;`
  for the `HostCapability` record.
- `src/AiEng.Platform.App/Components/Diagnostics/AppCapabilityList.razor.css`
  (45 lines): the scoped CSS. The
  `.app-capability-list` (block;
  full-width), `.app-capability-list-item`
  (block; full-width),
  `.app-capability-list-header` (flex
  with space-between; gap 0.75rem),
  `.app-capability-list-key` (font-weight
  500; font-size 0.95rem),
  `.app-capability-list-meta` (flex with
  gap 0.5rem; flex-wrap wrap),
  `.app-capability-list-version` (monospaced
  font; surface-2 background; subtle
  border-radius), `.app-capability-list-version-muted`
  (subtle color; italic for null versions).
- `src/AiEng.Platform.App/Components/Diagnostics/AppKeyValueList.razor`
  (115 lines): the `AppKeyValueList`
  markup. `@inherits
  AppKeyValueListBase`; same state
  machine. The `FormatValue` private
  `RenderFragment` uses the builder API
  to switch on the `Format` parameter:
  `Boolean` → `<span class="app-key-value-list-boolean
  app-key-value-list-boolean-true / -false"
  aria-label="true / false">✓ / ✗</span>`
  for "true" / "false"
  (case-insensitive); any other value
  renders as literal text; `Code` →
  `<code class="app-key-value-list-code">`; default (Plain)
  → literal text. The populated
  container has `data-state="populated"`
  and `aria-live="polite"`.
- `src/AiEng.Platform.App/Components/Diagnostics/AppKeyValueList.razor.cs`
  (40 lines): the `AppKeyValueListBase :
  ComponentBase` class with the
  parameters. `[Parameter, EditorRequired]
  IReadOnlyList<KeyValuePair<string,
  string>> Items { get; set; }` +
  `[Parameter] AppKeyValueListFormat
  Format { get; set; } = AppKeyValueListFormat.Plain;`
  + the four child-content slots + the
  loading / error state parameters +
  the `AdditionalAttributes` capture-all
  parameter. Uses `using
  AiEng.Platform.App.Components.Common;`
  for the `AppKeyValueListFormat` enum.
- `src/AiEng.Platform.App/Components/Diagnostics/AppKeyValueList.razor.css`
  (60 lines): the scoped CSS. The
  `.app-key-value-list` (block;
  full-width), `.app-key-value-list-body`
  (block; no margin; no padding),
  `.app-key-value-list-row` (flex with
  space-between; gap 1rem; padding
  0.5rem 0; border-bottom),
  `.app-key-value-list-key` (font-weight
  500; flex-shrink 0),
  `.app-key-value-list-value` (text-align
  right; word-break break-word),
  `.app-key-value-list-boolean` +
  `.app-key-value-list-boolean-true`
  (color `--app-success`),
  `.app-key-value-list-boolean-false`
  (color `--app-error`),
  `.app-key-value-list-code` (monospaced
  font; surface-2 background; subtle
  border-radius).
- `src/AiEng.Platform.App/Components/Diagnostics/_Imports.razor`
  (7 lines): the imports. Mirrors
  `Projects/_Imports.razor`; the
  `Diagnostics/` directory is a new
  directory under
  `src/AiEng.Platform.App/Components/`.

### 4.2 New bUnit tests

- `tests/AiEng.Platform.ComponentTests/Components/Diagnostics/AppCapabilityListTests.cs`
  (250+ lines): the 13 bUnit tests +
  the file-scoped namespace +
  the `AppCapabilityListTests : BunitContext`
  class. The tests use the bUnit
  `Render<AppCapabilityList>(parameters
  => parameters.Add(...))` API and
  assert on the rendered markup
  (`.FindAll(".app-capability-list-item")` +
  `Assert.Equal(3, items.Count)` etc.).
  The custom-slot tests use a
  `RenderFragment` lambda built with
  the builder API. The aria-live test
  uses `cut.Find("[role=\"list\"]")` +
  `Assert.Equal("polite", list.GetAttribute("aria-live"))`.
- `tests/AiEng.Platform.ComponentTests/Components/Diagnostics/AppKeyValueListTests.cs`
  (270+ lines): the 15 bUnit tests +
  the file-scoped namespace +
  the `AppKeyValueListTests : BunitContext`
  class. The tests follow the same
  pattern as the `AppCapabilityList`
  tests. The Boolean format tests assert
  the `aria-label` + the `TextContent`
  (✓ for "true", ✗ for "false"); the Code
  format test asserts the value is in
  a `<code class="app-key-value-list-code">`
  element; the Plain format test asserts
  the value is rendered as literal text
  (no `<code>` or boolean class); the
  aria-live test uses
  `cut.Find("[data-state=\"populated\"]")` +
  `Assert.Equal("polite", container.GetAttribute("aria-live"))`.

## 5. Files modified

### 5.1 Enums.cs

- `src/AiEng.Platform.App/Components/Common/Enums.cs`:
  appended the `AppKeyValueListFormat`
  enum after the existing
  `AppCardVariant` enum. The enum has
  three values (`Plain`, `Boolean`,
  `Code`); mirrors the M1.2 enum pattern
  (one `enum` per category; no `Flags`
  attribute; no `Description` attribute).

## 6. Files deleted

None.

## 7. Files NOT touched

- `src/AiEng.Platform.Application/Capabilities/`,
  `src/AiEng.Platform.Infrastructure/Capabilities/`,
  `src/AiEng.Platform.App/Composition/Capabilities/`:
  **not** modified. M4-B.2 composes the
  existing M4-B.1 contract + records +
  implementation + composition root.
- `src/AiEng.Platform.App/Components/Projects/`,
  `Layout/`, `Display/`, `Feedback/`,
  `Primitive/`, `Common/`: **not**
  modified (except the `Enums.cs` enum
  append). M4-B.2 composes the M1.2
  primitives; M4-B.2 does not introduce
  a new design-system primitive.
- `src/AiEng.Platform.App/Components/Pages/`,
  `src/AiEng.Platform.App/Program.cs`:
  **not** modified. M4-B.2 does not ship
  the `/diagnostics` page (M4-B.3's
  responsibility); `Diagnostics.razor`
  does **not** exist in M4-B.2.
- `src/AiEng.Platform.Providers.Abstractions/`,
  `src/AiEng.Platform.Domain/`: **not**
  modified. M4-B does not create
  providers (per the brief).
- `tests/AiEng.Platform.UnitTests/`,
  `tests/AiEng.Platform.ArchitectureTests/`:
  **not** modified. M4-B.2 ships bUnit
  tests, not unit tests or architecture
  tests. The
  `Capabilities_Resolved_Through_Service`
  architecture test is deferred to M4-B.3.
- `docs/capabilities.md`,
  `docs/infrastructure.md`,
  `docs/design-system.md`, `docs/projects.md`,
  `ROADMAP.md`, `.ai/plans/`:
  **not** modified. M4-B.2 is a code
  change, not a doc change. The M4-B
  plan promotion + M4-B.1 + M4-B.2
  closeout do not update § 4.5 of
  `docs/design-system.md` (the
  `AppCapabilityList` + `AppKeyValueList`
  rows are not yet updated from "Planned
  (M4)" to "Implemented (M4-B.2)"; this
  is a deferred decision per the
  M4-B.2 plan § 11 Documentation Plan § 3).
- `AGENTS.md`, `ARCHITECTURE.md`,
  `DECISIONS.md`, `STYLEGUIDE.md`,
  `CONTRIBUTING.md`, `.ai/workflows/`:
  **not** modified. The 17
  non-negotiable rules and the
  workflows are preserved.
- `tailwind.config.js`, `package.json`,
  `Directory.Build.props`,
  `.editorconfig`: **not** modified.
  The CSS pipeline and the .NET build
  configuration are unchanged.

## 8. Next action

**Stop.** The M4-B.2 first session
delivers the second boundary slice. The
M4-B.2 session does **not** begin M4-B.3
(`/diagnostics` page + startup log +
documentation + architecture test) /
M4-C / M4-D / any provider creation
(per the brief: "Do not begin the
following task" and the Progressive
Coding Rule).

The next session is the **M4-B.3 first
session** (T-026) on the user's
`Approve` or `Next` invocation. M4-B.3
ships the `/diagnostics` page
(Diagnostics.razor) composing the new
`AppCapabilityList` + `AppKeyValueList`
components, the startup capability-report
log through `ILogger<Program>`, the
`docs/capabilities.md` documentation,
and the `Capabilities_Resolved_Through_Service`
architecture test (deferred from M4-B.1
per the M4-B.1 plan § 14.1 Deviations).

Push is **staged for push** (not
authorised in this session). The next
user command may push the M4-B.2 closeout
commit per the command protocol.

---

**End of M4-B.2 per-session handoff.**
The M4-B.2 first session is the second
boundary slice of M4-B; the closeout
commit `feat(m4-b.2): add
AppCapabilityList + AppKeyValueList
data-owning design-system components` is
on `main`; the feature branch is
fast-forwarded into `main` and deleted.
The handoff is mirrored to
`.ai/handoffs/latest.md`. The M4-B.2
implementation report is at
`implementation-report-m4-b-2-capability-list-components.md`.
