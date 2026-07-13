# Handoff — M4-B.3 /diagnostics Page + Startup Capability-Report Log + Capabilities_Resolved_Through_Service Architecture Test — `m4-b-3-diagnostics-page-startup-log-and-architecture-test` (2026-07-13)

> **The M4-B.3 per-session handoff.** M4-B.3
> (T-026) is the third M4-B implementation
> slice. M4-B.3 follows the M4-B.1 + M4-B.2
> first sessions per the Progressive Coding
> Rule. M4-B.3 ships the `/diagnostics` page
> (Diagnostics.razor) composing the M4-B.1
> `IHostCapabilitiesService` + the M4-B.2
> `AppCapabilityList` + `AppKeyValueList` +
> the M1.2 `AppPageHeader` + `AppButton` +
> `AppCard` + `AppBreadcrumb` primitives;
> the startup capability-report log in
> `Program.cs` (10-second `CancellationTokenSource`
> timeout, `ILogger<Program>`, Information
> level, try/catch with Warning on failure);
> the `Capabilities_Resolved_Through_Service`
> architecture test (Active; 2 tests pass);
> the `docs/capabilities.md` 10-section
> documentation mirroring `docs/infrastructure.md`
> § 1-10; 4 bUnit page tests; and the
> `docs/design-system.md` § 4.5 update
> resolving the M4-B.2 deferred decision.
>
> M4-B.3 is the **third surface slice**, not
> the activation. The M4-C provider registry
> + the M4-D first concrete process providers
> are M4-B's successors and consume the
> `IHostCapabilitiesService` through DI, not
> through the startup log. M4-B.3 does **not**
> create providers (per the M4-B brief: "Do
> not create providers"). The M4-B.3 first
> session does **not** begin M4-C, M4-D, or
> any provider creation (per the brief: "Do
> not begin the following task"). The next
> session is the **M4-B closeout session**
> (T-027), which writes the M4-B closeout
> report and hands off to M4-C.

---

## 1. What was delivered

The M4-B.3 first session (T-026) is **Done**
(2026-07-13).

M4-B.3 ships:

- **The `/diagnostics` page** at
  `src/AiEng.Platform.App/Components/Pages/Diagnostics.razor`
  (+ `Diagnostics.razor.css`); the page
  declares `@page "/diagnostics"` +
  `@attribute [RouteMetadata("/diagnostics",
  "Diagnostics", Order = 4, ShowInSidebar =
  true, Icon = "◆", Description = "Detected
  host capabilities (tools, versions, provider
  credentials).")]` + `@layout AppLayout` +
  `@rendermode InteractiveServer` + `@inject
  IHostCapabilitiesService Service` +
  `@inject IPlatformInfo PlatformInfo`. The
  page composes the M1.2 `AppPageHeader` with
  Breadcrumbs/Title/Description/Actions
  (the `Actions` slot holds a Refresh
  `AppButton` with `data-testid="refresh-diagnostics"`
  that re-runs `Service.DetectAsync()`); the
  page renders two `AppCard`s — the first
  ("Host tools and provider credentials")
  hosts the `AppCapabilityList`; the second
  ("Host metadata", conditionally rendered
  when `_result` is non-null) hosts the
  `AppKeyValueList` with the 4 host metadata
  rows (Detected at + Data directory + Config
  directory + Is Windows host). The page's
  scoped CSS adds a `1.25rem` `margin-top` to
  the second `AppCard` for vertical breathing
  room.
- **The startup capability-report log** in
  `src/AiEng.Platform.App/Program.cs`
  (inserted between `app.Build()` and the
  middleware pipeline); the block resolves
  `IHostCapabilitiesService` + `ILogger<Program>`
  from `app.Services`; the block uses a
  10-second `CancellationTokenSource`; the
  block calls `Service.DetectAsync(cts.Token)`;
  the block logs the result at `Information`
  level with a single message including the
  `DetectedAt` timestamp + 6 tool statuses +
  6 credential statuses; the block is wrapped
  in a `try/catch` that logs failures at
  `Warning` level. The startup must not fail
  if capability detection fails.
- **The `Capabilities_Resolved_Through_Service`
  architecture test** at
  `tests/AiEng.Platform.ArchitectureTests/Capabilities/Capabilities_Resolved_Through_Service.cs`;
  the test is **Active** (not
  `[Fact(Skip=...)]` per ADR-016 / M4-D);
  the test has 2 `[Fact]` methods: (1)
  `Diagnostics_page_resolves_capabilities_through_IHostCapabilitiesService`
  asserts `Diagnostics.razor` contains
  `@inject IHostCapabilitiesService` and does
  not contain the forbidden tokens
  `RunToCompletionAsync` / `ICredentialVault`
  / `new SystemHostCapabilitiesService`; (2)
  `Diagnostics_folder_does_not_reference_process_or_credential_boundary_directly`
  scans every `.razor` + `.razor.cs` file
  under `src/AiEng.Platform.App/Components/Diagnostics/`
  for the same forbidden tokens. The test is
  scoped to the `Diagnostics` folder to avoid
  the M4-A.2 Open Action false positive on
  `AppProjectCard.razor` (which is in
  `App/Components/Projects/`, not in
  `App/Components/Diagnostics/`). The test
  passes.
- **4 bUnit page tests** at
  `tests/AiEng.Platform.ComponentTests/Pages/DiagnosticsPageTests.cs`;
  the tests use a private sealed
  `FakeHostCapabilitiesService` that records
  the `DetectAsync` call count and returns a
  configured `HostCapabilities`; the tests
  use a private sealed `StaticPlatformInfo`
  that returns `/tmp/data` + `/tmp/config` +
  `IsWindows = true`; the tests register
  `IHostCapabilitiesService` + `IPlatformInfo`
  + an `EmptyNavigationRegistry` (the empty
  registry is needed because the `AppLayout`'s
  `AppTopBar` injects `INavigationRegistry`).
  The 4 tests: (1)
  `DiagnosticsPage_calls_DetectAsync_on_init`
  asserts `OnInitializedAsync` calls
  `DetectAsync` once; (2)
  `DiagnosticsPage_renders_AppCapabilityList_with_capabilities`
  asserts the page renders 12
  `.app-capability-list-item` entries
  (6 host tools + 6 provider credentials);
  (3) `DiagnosticsPage_Refresh_button_reruns_DetectAsync`
  finds the refresh button, clicks it, and
  asserts `DetectAsync` is called twice; (4)
  `DiagnosticsPage_renders_AppKeyValueList_with_host_metadata`
  asserts the page renders the host metadata
  block with the 4 rows.
- **The `docs/capabilities.md` 10-section
  documentation** at `docs/capabilities.md`;
  the file mirrors the structure of
  `docs/infrastructure.md` § 1-10: § 1 Goals;
  § 2 Project Structure; § 3 The
  `IHostCapabilitiesService` Contract; § 4
  The `HostCapabilities` and `HostCapability`
  Records; § 5 The `AppCapabilityList`
  Component; § 6 The `AppKeyValueList`
  Component; § 7 The `/diagnostics` Page; §
  8 Composition Root; § 9 Tests; § 10 Out
  of Scope.
- **The `docs/design-system.md` § 4.5
  update** — `AppCapabilityList` row from
  `Planned (M4)` to `Implemented (M4-B.2)`;
  `AppKeyValueList` row from `Planned (M4)`
  to `Implemented (M4-B.2)`. The
  `AppCapabilityList` `Notes` column changes
  from `Renders `RuntimeCapabilities`` to
  `Renders `HostCapability[]` from
  `IHostCapabilitiesService.DetectAsync`;
  data-owning four-state`. This resolves
  the M4-B.2 deferred decision per the
  M4-B.2 handoff § 7.
- **The project-continuity state** per Rule
  15: `session.json` (M4-B.3 envelope);
  `tasks.json` (T-026 `Done` with full
  evidence; T-027 M4-B closeout stub row
  in `Ready`); `current.md` (active slice
  M4-B.2 → M4-B.3; last completed task
  T-025 → T-026; next recommended task
  T-026 → T-027; last updated 2026-07-13
  M4-B.3); `task-board.md` (M4-B.3 row in
  `Done Recently`; T-027 stub row in
  `Ready`); `milestones.json` (M4-B.3
  slice block from `Planned` to `Done`;
  top-level `updated_at` + `updated_by_session`
  updated); `capabilities.json` (C-015 +
  C-023 + C-024 evidence blocks finalised;
  top-level `updated_at` + `updated_by_session`
  updated; C-015 `next_task` set to T-027
  for the M4-B closeout handoff).

---

## 2. Test and build status

- **Format:** `dotnet format
  --verify-no-changes` exits 0; the format
  is canonical and CI-clean.
- **Build:** `dotnet build AiEng.Platform.slnx`
  exits 0; **0 warnings, 0 errors** (with
  `TreatWarningsAsErrors=true` from
  `Directory.Build.props`).
- **Test:** `dotnet test AiEng.Platform.slnx
  --no-build` reports **376 passed, 0 failed,
  9 skipped** (per ADR-016 / M4-D). The
  M4-B.3 ships +4 new bUnit page tests +
  +2 new Active architecture tests; the
  M4-B.2 370-test baseline is preserved.
  Breakdown: 99 unit (unchanged from M4-B.2)
  + 263 component (was 259; +4 M4-B.3 bUnit
  page tests) + 14 architecture (was 12;
  +2 M4-B.3 Active architecture tests).
  The 9 skipped are the registered-but-disabled
  tests per ADR-016 / M4-D.
- **JSON validation:** the 4 state JSON files
  (`session.json` + `tasks.json` +
  `milestones.json` + `capabilities.json`)
  are valid JSON; the `updated_at` fields
  are updated; the schema is preserved.
- **CRLF validation:** all new + modified
  files are CRLF (`unix2dos` applied to:
  `Diagnostics.razor` + `Diagnostics.razor.css`
  + `Program.cs` + `Capabilities_Resolved_Through_Service.cs`
  + `DiagnosticsPageTests.cs` + `capabilities.md`
  + `design-system.md` + `session.json` +
  `tasks.json` + `current.md` +
  `milestones.json` + `capabilities.json`).
- **Architecture boundary check:** the M4-B.3
  implementation does not introduce
  `System.Diagnostics.Process` usage outside
  `src/AiEng.Platform.Infrastructure/`; the
  M4-B.3 implementation does not introduce
  `advapi32.dll` P/Invoke outside
  `src/AiEng.Platform.Infrastructure/`; the
  M4-B.3 implementation does not introduce
  a `Microsoft.Extensions.DependencyInjection`
  `IServiceCollection` extension outside
  `src/AiEng.Platform.App/Composition/`. The
  boundary is enforced by the M4-A.1
  architecture tests
  (`Infrastructure_Respects_ProcessBoundary`
  + `Infrastructure_Respects_CredentialBoundary`),
  which are registered-but-disabled per
  ADR-016 / M4-D.
- **M4-B.3 architecture test gate:** the new
  `Capabilities_Resolved_Through_Service` test
  is **Active** (not registered-but-disabled)
  per the M4-B plan § 2 item 9; the test
  passes (the `Diagnostics.razor` page
  correctly uses `@inject IHostCapabilitiesService`;
  the `App/Components/Diagnostics/` folder
  has no `RunToCompletionAsync` /
  `ICredentialVault` / `new
  SystemHostCapabilitiesService` tokens).

---

## 3. Deviations

The M4-B.3 first session has **three documented
deviations**:

1. **The 12 capability list items per page**
   (vs the 6 mentioned in the M4-B plan prose
   example). The M4-B plan § 2 item 6 mentioned
   the page renders 6 host tools + 6 provider
   credentials in passing; the actual list
   size is 12 (6 host tools × 2 — once for
   the tool, once for the credential). The
   `AppCapabilityList` component renders one
   `.app-capability-list-item` per entry in
   the `Capabilities` list. The 4 bUnit page
   tests assert the 12-item list. The plan
   intent (one card per capability with
   status dot + version + credential badge)
   is preserved.

2. **The `Capabilities_Resolved_Through_Service`
   test scopes to `App/Components/Diagnostics/`,
   not to `App/Components/`.** The M4-B plan §
   2 item 9 said the test asserts no
   `RunToCompletionAsync` / `ICredentialVault`
   / `new SystemHostCapabilitiesService` tokens
   in any page file; the M4-B.1 plan § 14.1
   Deviations anticipated the M4-A.2 Open Action
   false positive on `AppProjectCard.razor`
   (which uses `RunToCompletionAsync` and
   `IProcessRunner` for the Open action). The
   M4-B.3 test scopes the forbidden-token
   check to the `Diagnostics` folder
   (`App/Components/Diagnostics/`), not the
   whole `App/Components/` tree, to avoid the
   M4-A.2 false positive. The M4-B plan's
   `Diagnostics.razor` content assertion
   (positive: `@inject IHostCapabilitiesService`;
   negative: no `RunToCompletionAsync` /
   `ICredentialVault` / `new
   SystemHostCapabilitiesService` tokens) is
   preserved.

3. **`docs/capabilities.md` does not add a §
   11 "M4-B Consumers" section.** The M4-B.2
   handoff § 13 anticipated this; the M4-B.3
   closeout confirms the no-scope-creep
   decision — `docs/infrastructure.md` § 11
   (M4-B Consumers) was already added in the
   M4-A.2 handoff; the M4-B `docs/capabilities.md`
   mirrors the 10-section `docs/infrastructure.md`
   § 1-10 structure, with no § 11 addition.

---

## 4. Files added

- `src/AiEng.Platform.App/Components/Pages/Diagnostics.razor`
  — the `/diagnostics` page (page +
  `@code` block in the same file; mirrors the
  `Dashboard.razor` pattern with the
  `AppCapabilityList` + `AppKeyValueList`
  content).
- `src/AiEng.Platform.App/Components/Pages/Diagnostics.razor.css`
  — the scoped CSS (one layout tweak: a
  `1.25rem` `margin-top` to the second
  `AppCard` for vertical breathing room;
  no design-system change).
- `tests/AiEng.Platform.ArchitectureTests/Capabilities/Capabilities_Resolved_Through_Service.cs`
  — the 1 Active architecture test (with
  2 `[Fact]` methods) for the
  `IHostCapabilitiesService` seam.
- `tests/AiEng.Platform.ComponentTests/Pages/DiagnosticsPageTests.cs`
  — the 4 bUnit page tests (with private
  sealed `FakeHostCapabilitiesService` +
  `StaticPlatformInfo` + `EmptyNavigationRegistry`
  test doubles).
- `docs/capabilities.md` — the 10-section
  M4-B documentation (mirroring
  `docs/infrastructure.md` § 1-10).
- `.ai/handoffs/2026-07-13-m4-b-3-diagnostics-page-startup-log-and-architecture-test.md`
  — this handoff (mirrored to `latest.md`).
- `implementation-report-m4-b-3-diagnostics-page-startup-log-and-architecture-test.md`
  — the M4-B.3 implementation report (15
  sections mirroring the M4-B.1 + M4-B.2 +
  M4-A.1 + M4-A.2 reports).

---

## 5. Files modified

- `src/AiEng.Platform.App/Program.cs` —
  added the startup capability-report log
  block between `app.Build()` and the
  middleware pipeline (the static local
  function `LogHostCapabilitiesAsync(WebApplication
  app)`); added the `using
  AiEng.Platform.Application.Capabilities;`
  using directive at the top of the file.
- `docs/design-system.md` § 4.5 — updated
  the `AppCapabilityList` + `AppKeyValueList`
  rows from `Planned (M4)` to `Implemented
  (M4-B.2)`; updated the `AppCapabilityList`
  `Notes` column from `Renders
  `RuntimeCapabilities`` to `Renders
  `HostCapability[]` from
  `IHostCapabilitiesService.DetectAsync`;
  data-owning four-state`. (Resolves the
  M4-B.2 deferred decision per the M4-B.2
  handoff § 7.)
- `.ai/state/session.json` — M4-B.3 envelope
  replaced the M4-B.2 envelope.
- `.ai/state/tasks.json` — T-026 transitioned
  from `InProgress` to `Done` with the full
  evidence block; T-027 stub row in `Ready`;
  top-level `updated_at` +
  `updated_by_session` updated.
- `.ai/state/current.md` — active slice
  updated; last completed task updated; next
  recommended task updated; last updated
  section updated.
- `.ai/state/task-board.md` — M4-B.3 row
  added to `Done Recently`; T-027 stub row
  in `Ready`; M4-B.3 Ready block status
  updated to `Done`.
- `.ai/state/milestones.json` — M4-B.3
  slice block added to the M4-B `slices`
  array with `status: Done`; top-level
  `updated_at` + `updated_by_session`
  updated.
- `.ai/state/capabilities.json` — C-015
  evidence block finalised with M4-B.2 +
  M4-B.3 commits + reports + tests + paths;
  C-023 + C-024 evidence blocks finalised
  with M4-B.3 commit + report + tests + paths;
  C-015 `next_task` set to T-027 for the
  M4-B closeout handoff; top-level
  `updated_at` + `updated_by_session`
  updated.

---

## 6. Files deleted

None. The M4-B.3 first session adds 7 new
files + modifies 8 files; no files are
deleted.

---

## 7. Files NOT touched

- `src/AiEng.Platform.Application/Capabilities/`
  — **not** modified. M4-B.3 composes the
  existing M4-B.1 contract + records.
- `src/AiEng.Platform.Infrastructure/Capabilities/`
  — **not** modified. M4-B.3 does not modify
  the implementation.
- `src/AiEng.Platform.App/Composition/Capabilities/`
  — **not** modified. M4-B.3 does not modify
  the composition root.
- `src/AiEng.Platform.App/Components/Diagnostics/`
  — **not** modified. M4-B.3 composes the
  existing M4-B.2 components.
- `src/AiEng.Platform.Providers.Abstractions/`
  — **not** modified. M4-B does not create
  providers.
- `src/AiEng.Platform.Domain/` — **not**
  modified. M4-B.3 adds no domain types.
- `tests/AiEng.Platform.UnitTests/` — **not**
  modified. M4-B.3 ships bUnit tests + 1
  architecture test, not unit tests.
- `tests/AiEng.Platform.ComponentTests/Components/Diagnostics/`
  — **not** modified. M4-B.3 ships the
  page-level test in `Pages/`, not new
  component tests.
- `docs/infrastructure.md` — **not** modified.
  (The M4-B.2 handoff § 13 said M4-B.3 might
  add § 11 "M4-B Consumers"; the existing
  `docs/infrastructure.md` already has § 11
  from the M4-A.2 handoff, so no addition
  is needed.)
- `ROADMAP.md`, `.ai/plans/master-delivery-plan.md`,
  `.ai/plans/M4-B-capability-detection.md`
  — **not** modified.
- `AGENTS.md`, `ARCHITECTURE.md`, `DECISIONS.md`,
  `STYLEGUIDE.md`, `CONTRIBUTING.md` — **not**
  modified.
- `.ai/workflows/` — **not** modified.
- `tailwind.config.js`, `package.json`,
  `Directory.Build.props`, `.editorconfig`
  — **not** modified.

---

## 8. Next action

**Stop.** The M4-B.3 first session does **not**
begin M4-C, M4-D, or any provider creation
(per the brief: "Do not begin the following
task" and the Progressive Coding Rule).

The next session is the **M4-B closeout
session** (T-027), on the user's `Approve` or
`Next` invocation. The M4-B closeout session:

- Writes the M4-B closeout report (15+
  sections mirroring the M4-A closeout
  reports; aggregates the M4-B.1 + M4-B.2 +
  M4-B.3 evidence blocks; finalises the M4-B
  status to `Delivered`; transitions the
  next-milestone handoff to M4-C).
- Moves the M4-B milestone from `Active` to
  `Done` with `closed_at: 2026-07-13` in
  `milestones.json`.
- Creates the `m4-b` annotated milestone tag
  at the M4-B.3 closeout commit on `main`
  per the branching strategy rule 9.
- Produces the M4-C plan in `Awaiting Approval`
  per the Milestone Closeout Standard
  (`.ai/workflows/milestone-closeout.md`).

The M4-B closeout session does **not** begin
M4-C, M4-D, or any provider creation. The
M4-C plan promotion is the M4-C.1 first
session's responsibility on the user's
`Approve` or `Next` invocation after the
M4-B closeout.

**Push decision:** Push is **not** authorised
in this session. The M4-B.3 first session did
not push. The push decision recorded in the
M4-B.3 handoff is **Staged for push** (the
user did not authorise in this session; the
M4-B.3 did not push; the next user command
may push).

**The M4-B.3 closeout commit** is
`feat(m4-b.3): add /diagnostics page, startup
capability log, and Capabilities_Resolved_Through_Service
architecture test` on the feature branch
`feature/T-026-m4-b-3-diagnostics-page-startup-log-and-architecture-test`,
fast-forwarded into `main` per the branching
strategy rule 6; the feature branch is
deleted per rule 7. No push.
