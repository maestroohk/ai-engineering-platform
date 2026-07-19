# Task Board

> **Live work queue for the AI Engineering Platform.**
> Updated at the end of every AI session that changes
> project state. The most recent update wins. The
> state files reflect the actual state of the
> repository; the repository wins when the two
> disagree (see `.ai/session-start.md` step 6 —
> state reconciliation).
>
> **State architecture (M0.5).** This file is the
> human-readable projection. The canonical machine-readable
> work queue is in
> [`.ai/state/tasks.json`](./tasks.json). The
> capability graph is in
> [`.ai/state/capabilities.json`](./capabilities.json);
> the milestone list is in
> [`.ai/state/milestones.json`](./milestones.json). The
> two layers are kept in sync by every session that
> changes the work queue.
>
> **Status codes:**
>
> - **Ready** — task is defined, no blocker, no owner.
> - **In Progress** — a session is actively working
>   the task.
> - **Blocked** — task cannot proceed; the row names
>   the blocker.
> - **Done Recently** — completed, merged, and
>   committed. Recent items live here; older items
>   archive to `.ai/handoffs/`.
> - **Deferred** — task is intentionally out of
>   scope for the current plan; it lives here so it
>   is not forgotten.

---

## Ready

### M4-A.3 — Undefined (next M4-A slice)

- **Task ID:** (none — M4-A.3 is not
  yet planned)
- **Milestone:** M4-A — Infrastructure /
  Process Execution (Active 2026-07-11;
  M4-A.1 + M4-A.2 Delivered 2026-07-11)
- **Title:** M4-A.3 — the third M4-A
  implementation slice (not yet
  defined).
- **Why it matters:** M4-A.1 shipped
  the infrastructure seam
  (`IProcessRunner` +
  `ICredentialVault` + `IPlatformInfo` +
  on-disk `IProjectStore`).
  M4-A.2 shipped the first
  `IProcessRunner` activation (the
  Open action on `AppProjectCard`).
  The M4-A plan § 2 enumerates two
  slices (M4-A.1 + M4-A.2); a third
  slice is not yet planned. The next
  milestone after M4-A.2 closes is
  M4-B (Capability Detection,
  Active; the M4-B plan is in
  Awaiting Approval at
  `.ai/plans/M4-B-capability-detection.md`).
  The M4-B implementation is the next
  concrete step. M4-A.3 is undefined;
  M4-A.3 may be defined in a future
  session if the M4-B / M4-C / M4-D
  work requires a third M4-A slice.
- **Objective:** (not yet defined).
- **Acceptance criteria:** (not yet
  defined).
- **Dependencies:** M4-A.1 + M4-A.2
  (Done 2026-07-11).
- **Expected affected areas:**
  (not yet defined).
- **Validation:** (not yet defined).
- **Approved plan path:**
  `.ai/plans/M4-A-infrastructure-process-execution.md`
  (Approved 2026-07-11 via the
  'Next' invocation). The M4-A.3
  plan is not yet drafted; the M4-A.2
  closeout does not seed M4-A.3 (per
  the brief: 'Do not begin the
  following task'). The M4-B plan
  promotion is also documented in
  the M4-B plan promotion handoff at
  `.ai/handoffs/2026-07-13-m4-b-plan-promotion.md`.
- **Status:** **Deferred** (the M4-A.2
  closeout does not seed a concrete
  M4-A.3 task; the M4-B plan
  promotion is `Done` 2026-07-13; the
  M4-B implementation is the next
  concrete step; M4-A.3 is undefined;
  M4-A.3 may be defined in a future
  session if the M4-B / M4-C / M4-D
  work requires a third M4-A slice).

### T-028 — M4-C.1 first session — IProviderRegistry contract + family registries + composition root + unit tests

- **Task ID:** T-028.
- **Milestone:** M4-C — Provider
  Registry Foundation (Awaiting
  Approval 2026-07-13; the M4-C plan
  is at
  `.ai/plans/M4-C-provider-registry-foundation.md`).
- **Title:** M4-C.1 — `IProviderRegistry`
  contract + 6 family registries +
  `SystemProviderRegistry` implementation
  + 6 family fakes + `AddProviderRegistry`
  composition root + 9+ unit tests —
  the boundary slice of M4-C (the first
  M4-C implementation slice).
- **Why it matters:** M4-B shipped the
  `IHostCapabilitiesService` + the
  `AppCapabilityList` + `AppKeyValueList`
  + the `/diagnostics` page + the
  startup capability-report log + the
  `Capabilities_Resolved_Through_Service`
  architecture test + `docs/capabilities.md`.
  M4-C ships the provider registry
  foundation: the `IProviderRegistry`
  contract (the single allowed seam
  between the application and the
  provider layer; the M4-C
  architecture requires that all
  provider access flows through
  `IProviderRegistry`, never through
  the concrete `SystemProviderRegistry`
  or any `IProvider` implementation);
  the 6 family registries
  (`ShellProviderRegistry` +
  `EditorProviderRegistry` +
  `AgentRuntimeProviderRegistry` +
  `TerminalProviderRegistry` +
  `WorktreeProviderRegistry` +
  `CredentialProviderRegistry`); the
  `SystemProviderRegistry`
  implementation that composes the 6
  family registries and consumes
  `IHostCapabilitiesService` through
  DI to filter eligible providers per
  host capabilities; the 6 family
  fakes; the `AddProviderRegistry`
  composition root extension; 9+ unit
  tests in
  `tests/AiEng.Platform.UnitTests/Providers/`.
  M4-C.1 does **not** ship the
  `AppProviderList` component, the
  `/providers` page, the startup
  provider-report log, the
  `Providers_Resolve_Through_Registry`
  architecture test, or
  `docs/providers.md` — those are in
  M4-C.2.
- **Objective:** Land the
  `IProviderRegistry` contract in
  `src/AiEng.Platform.Application/Providers/`;
  land the 6 family registry contracts
  in
  `src/AiEng.Platform.Application/Providers/Families/`;
  land the `ProviderDescriptor` +
  `ProviderFamily` + `ProviderStatus`
  records in
  `src/AiEng.Platform.Application/Providers/`;
  land the 6 family registry
  implementations in
  `src/AiEng.Platform.Infrastructure/Providers/Families/`;
  land the `SystemProviderRegistry`
  implementation in
  `src/AiEng.Platform.Infrastructure/Providers/`;
  land the `AddProviderRegistry`
  composition root extension in
  `src/AiEng.Platform.App/Composition/Providers/`;
  wire `AddProviderRegistry` into
  `AddPlatformServices`; land 9+ unit
  tests + 6 family fakes in
  `tests/AiEng.Platform.UnitTests/Providers/`.
- **Acceptance criteria:**
  - `IProviderRegistry` exposes
    `Task<IReadOnlyList<ProviderDescriptor>>
    ListAsync(CancellationToken)` +
    `Task<ProviderStatus>
    GetStatusAsync(string providerId,
    CancellationToken)` + `bool
    IsEligible(string providerId,
    HostCapabilities capabilities)`.
  - The 6 family registries are
    registered in
    `AddProviderRegistry` via
    `TryAddSingleton` (one per family).
  - `SystemProviderRegistry` consumes
    `IHostCapabilitiesService` through
    DI to gate eligibility per host
    capabilities; the eligibility
    check is the only side-effect the
    M4-C contract has on M4-B.
  - The 6 family registries return
    empty `IReadOnlyList` by default
    (no concrete providers are
    shipped; the providers come in
    M4-D per the M4-B brief: 'Do not
    create providers').
  - The 9+ unit tests cover
    `ListAsync` (empty) + per-family
    `ListAsync` (empty) + `GetStatusAsync`
    (not found) + `IsEligible` (per
    family + cross-family).
  - The M4-B closeout 376 tests
    remain green (regression gate);
    M4-C.1 ships 9+ new unit tests.
- **Dependencies:** M4-B
  (T-027, Done 2026-07-13);
  `IHostCapabilitiesService`
  (C-015, Verified).
- **Expected affected areas:**
  `src/AiEng.Platform.Application/Providers/`
  (new);
  `src/AiEng.Platform.Infrastructure/Providers/`
  (new);
  `src/AiEng.Platform.App/Composition/Providers/`
  (new);
  `src/AiEng.Platform.App/Composition/ServiceCollectionExtensions.cs`
  (wire `AddProviderRegistry`);
  `tests/AiEng.Platform.UnitTests/Providers/`
  (new).
- **Validation:** `dotnet restore`
  (exit 0); `dotnet build` (0
  warnings, 0 errors); `dotnet test`
  (376 + 9+ new = 385+ passed,
  0 failed, 9 skipped per ADR-016 /
  M4-D); `dotnet format
  --verify-no-changes` (exit 0);
  JSON validation; CRLF validation.
- **Approved plan path:**
  `.ai/plans/M4-C-provider-registry-foundation.md`
  (Awaiting Approval 2026-07-13; the
  M4-C plan is produced by the M4-B
  closeout session 2026-07-13).
- **Status:** **Done (delivered 2026-07-13; see the M4-C.1 entry in Done Recently).** M4-C.1 ships the provider registry foundation (boundary slice). The next session is the M4-C.2 first session (T-029, `Ready`; the `AppProviderList` data-owning design-system component + the `/providers` page + the startup provider-registry log + the `Providers_Resolve_Through_Registry` architecture test + `docs/providers.md` + the `docs/design-system.md` § 4.5 update) on the user's `Approve` or `Next` invocation.

### T-029 — M4-C.2 first session — AppProviderList + /providers page + startup log + architecture test + docs

- **Task ID:** T-029.
- **Milestone:** M4-C — Provider
  Registry Foundation (Active
  2026-07-13; the M4-C plan is at
  `.ai/plans/M4-C-provider-registry-foundation.md`).
- **Title:** M4-C.2 — `AppProviderList`
  data-owning design-system component +
  `/providers` page + startup
  provider-registry log +
  `Providers_Resolve_Through_Registry`
  architecture test + `docs/providers.md`
  + `docs/design-system.md` § 4.5 update
  — the surface slice of M4-C (the
  second M4-C implementation slice).
- **Why it matters:** M4-C.1 shipped
  the `IProviderRegistry` contract +
  the 6 family registry contracts +
  the `SystemProviderRegistry`
  implementation + the 6 no-op family
  stubs + the `AddProviderRegistry`
  composition root + the 6 family
  fakes + 19 unit tests (the
  boundary slice). M4-C.2 ships the
  user-visible surface: the
  `AppProviderList` data-owning
  four-state design-system component
  (renders an
  `IReadOnlyList<ProviderDescriptor>`
  as a list of `AppCard` entries with
  `AppStatusDot` Success/Error, the
  `Version` in a monospaced muted
  font, and an `AppBadge` "Disabled"
  for `Status = Disabled`; `aria-
  live="polite"` on the populated
  list); the `/providers` page
  (composes `AppPageHeader` + the
  `AppProviderList`; the 6 family
  cards in the `ProviderFamily` enum
  order); the startup provider-registry
  log in `Program.cs` (10-second
  `CancellationTokenSource` timeout;
  `ILogger<Program>`; Information
  level; try/catch with Warning on
  failure); the
  `Providers_Resolve_Through_Registry`
  architecture test (scoped to
  `App/Components/Providers/` to
  avoid the M4-A.2 Open Action
  false positive; the test asserts
  `Providers.razor` uses `@inject
  IProviderRegistry` and no file in
  `App/Components/Providers/`
  contains the forbidden tokens
  `new SystemProviderRegistry` /
  `IProviderRegistry` direct
  implementation); the
  `docs/providers.md` 10-section
  documentation mirroring
  `docs/infrastructure.md` § 1-10;
  the `docs/design-system.md` § 4.5
  update adding the `AppProviderList`
  row.
- **Objective:** Land the
  `AppProviderList` component in
  `src/AiEng.Platform.App/Components/DesignSystem/`;
  land the `/providers` page at
  `src/AiEng.Platform.App/Components/Pages/Providers.razor`;
  wire the startup provider-registry
  log in `src/AiEng.Platform.App/Program.cs`;
  land the
  `Providers_Resolve_Through_Registry`
  architecture test in
  `tests/AiEng.Platform.ArchitectureTests/Providers/`;
  land the `docs/providers.md` 10-
  section documentation; update
  `docs/design-system.md` § 4.5.
- **Acceptance criteria:** `/providers`
  page returns 200; the
  `AppProviderList` renders 0 or
  more `AppCard` entries (one per
  `ProviderDescriptor`); the startup
  log lists the providers per family
  at Information level; the
  `Providers_Resolve_Through_Registry`
  architecture test passes (Active per
  the M4-C plan § 2 item 12);
  `docs/providers.md` is published;
  `docs/design-system.md` § 4.5
  reflects the new component; the
  M4-C.1 395 tests remain green
  (regression gate); M4-C.2 ships 4+
  bUnit page tests + 1+ active
  architecture test.
- **Dependencies:** M4-C.1
  (T-028, Done 2026-07-13);
  `IProviderRegistry` (C-010,
  Delivered).
- **Expected affected areas:**
  `src/AiEng.Platform.App/Components/DesignSystem/`
  (new `AppProviderList`);
  `src/AiEng.Platform.App/Components/Pages/Providers.razor`
  (new);
  `src/AiEng.Platform.App/Program.cs`
  (startup log);
  `tests/AiEng.Platform.ComponentTests/Pages/ProvidersPageTests.cs`
  (new);
  `tests/AiEng.Platform.ArchitectureTests/Providers/Providers_Resolve_Through_Registry.cs`
  (new);
  `docs/providers.md` (new);
  `docs/design-system.md` (modified
  § 4.5).
- **Validation:** `dotnet restore`
  (exit 0); `dotnet build` (0
  warnings, 0 errors); `dotnet test`
  (395 + 4+ new bUnit + 1+ new
  architecture = 400+ passed, 0
  failed, 9 skipped per ADR-016 /
  M4-D); `dotnet format
  --verify-no-changes` (exit 0);
  JSON validation; CRLF validation;
  visual smoke on `/providers`.
- **Approved plan path:**
  `.ai/plans/M4-C-provider-registry-foundation.md`
  (Active 2026-07-13; the M4-C plan
  was approved at M4-B closeout).
- **Status:** **Done (delivered 2026-07-13; see the M4-C.2 entry in Done Recently).** M4-C.2 ships the user-visible surface slice. The next session is the M4-C closeout (T-030, `Ready`; the M4-C retrospective + M4-C status `Active` → `Done` + the `m4-c` annotated milestone tag + the M4-D plan + project-continuity state) on the user's `Approve` or `Next` invocation.

### T-030 — M4-C closeout — M4-C retrospective + M4-C -> Done + m4-c tag + M4-D plan + project-continuity state

- **Task ID:** T-030.
- **Milestone:** M4-C — Provider
  Registry Foundation (Active
  2026-07-13; the M4-C plan is at
  `.ai/plans/M4-C-provider-registry-foundation.md`;
  M4-C.1 + M4-C.2 Delivered
  2026-07-13).
- **Title:** M4-C closeout — the
  M4-C retrospective (13 sections per
  the Milestone Closeout Standard § 4)
  + M4-C status `Active` → `Done`
  with `closed_at: 2026-07-13` +
  `m4-c` annotated milestone tag at
  the M4-C closeout commit on `main`
  per the branching strategy rule 9
  + the M4-D plan in `Awaiting
  Approval` at
  `.ai/plans/M4-D-first-concrete-process-providers.md`
  (12 sections mirroring the M4-A +
  M4-B + M4-C plans) + the
  project-continuity state update per
  Rule 15.
- **Why it matters:** M4-C.1 + M4-C.2
  ship the M4-C boundary slice (the
  contract + the implementation + the
  composition root + 19 unit tests)
  + the M4-C surface slice (the
  `AppProviderList` data-owning
  component + the `/providers` page +
  the startup log + the
  `Providers_Resolve_Through_Registry`
  architecture test + `docs/providers.md`
  + 13 bUnit component tests + 5 bUnit
  page tests). The M4-C closeout is
  the closeout slice that aggregates
  the M4-C.1 + M4-C.2 evidence
  blocks; finalises the M4-C status
  to `Done`; transitions the
  next-milestone handoff to M4-D.
- **Objective:** Land the M4-C
  retrospective at
  `retrospective-m4-c-provider-registry-foundation.md`
  (13 sections per the Milestone
  Closeout Standard § 4); land the
  M4-D plan in `Awaiting Approval`
  at
  `.ai/plans/M4-D-first-concrete-process-providers.md`
  (12 sections); land the M4-C
  closeout handoff + implementation
  report; tag the M4-C closeout
  commit with the `m4-c` annotated
  milestone tag; update the
  project-continuity state (6 state
  files) per Rule 15.
- **Acceptance criteria:** M4-C
  retrospective exists and is 13
  sections per the standard; M4-D
  plan is in `Awaiting Approval`;
  the `m4-c` annotated tag is at
  the M4-C closeout commit on `main`
  per the branching strategy rule
  9; the 6 state files are updated;
  M4-C status is `Done` with
  `closed_at: 2026-07-13` in
  `.ai/state/milestones.json`; the
  M4-C closeout commit is on `main`
  and the feature branch is deleted
  per rule 7; the 416 tests remain
  green (regression gate); the M4-C
  closeout is a docs + workflow +
  state change with no new tests.
- **Dependencies:** M4-C.1 (T-028,
  Done 2026-07-13); M4-C.2 (T-029,
  Done 2026-07-13);
  `IProviderRegistry` (C-010,
  Delivered); `AppProviderList`
  (C-021, Delivered M4-C.2);
  `/providers` page (C-022,
  Delivered M4-C.2).
- **Expected affected areas:**
  `retrospective-m4-c-provider-registry-foundation.md`
  (new);
  `.ai/plans/M4-D-first-concrete-process-providers.md`
  (new);
  `.ai/handoffs/2026-07-13-m4-c-closeout.md`
  (new; mirrored to `latest.md`);
  `implementation-report-m4-c-closeout.md`
  (new);
  `.ai/state/milestones.json` (M4-C
  `Active` → `Done` with
  `closed_at: 2026-07-13`; M4-C
  closeout slice block; top-level
  `commits` + `handoffs` +
  `implementation_reports` arrays
  updated);
  `.ai/state/capabilities.json`
  (C-010 / C-021 / C-022 evidence
  finalised; C-010 `next_task`
  cleared on close);
  `.ai/state/tasks.json` (T-030
  Ready → InProgress → Done);
  `.ai/state/current.md` (active
  slice M4-C.2 → M4-C closeout);
  `.ai/state/task-board.md` (M4-C
  closeout row in `Done Recently`;
  T-031 M4-D stub in `Ready`);
  `ROADMAP.md` (§ 2 M4-C row
  `Active` → `Done`; M4-D row
  `Planned` → `Awaiting Approval`;
  § 3 M4-C DoD bullets checked);
  `.ai/plans/master-delivery-plan.md`
  (§ 1 M4-C row `Active` → `Done`
  with `closed_at: 2026-07-13`;
  M4-D row `Planned` → `Awaiting
  Approval`).
- **Validation:** `dotnet restore`
  (exit 0); `dotnet build` (0
  warnings, 0 errors); `dotnet test`
  (416 passed, 0 failed, 9 skipped
  — identical to M4-C.2 closeout);
  `dotnet format --verify-no-changes`
  (exit 0); JSON validation; CRLF
  validation.
- **Approved plan path:**
  `.ai/plans/M4-C-provider-registry-foundation.md`
  (Active 2026-07-13; the M4-C plan
  was approved at M4-B closeout).
- **Status:** **Ready** (M4-C.2
  first session is `Done` 2026-07-13;
  the M4-C closeout is the next
  concrete step on the user's
  `Approve` or `Next` invocation
  after T-029 is Done). The M4-C.2
  first session does **not** begin
  M4-C closeout (per the brief:
  'Do not begin the following
  task').

### M1 follow-up — Add `AppToolbar` example to `/design-system`

### M1 follow-up — Add `AppToolbar` example to `/design-system`

- **Task ID:** `M1-FU-1`
- **Milestone:** M1 — Design System Core
  (closed; this is a follow-up)
- **Title:** Add `AppToolbar` example to
  `/design-system`
- **Why it matters:** The `/design-system`
  page is the design-system catalogue's
  rendering; missing component examples hide
  shipped components from reviewers.
  `AppToolbar` ships and is unit-tested but
  is not exercised on the doc page (18/19
  component CSS classes appear in the
  rendered HTML).
- **Objective:** Add an `AppToolbar` example to
  `src/AiEng.Platform.App/Components/Pages/DesignSystem.razor`
  in a new "Toolbar" section; rebuild CSS;
  verify the `app-toolbar` class appears in the
  rendered output.
- **Acceptance criteria:** `app-toolbar`
  appears in the `/design-system` HTML; the
  example matches the level of detail of the
  existing section examples; all M1 validation
  remains green.
- **Dependencies:** M1 (closed). No new
  dependencies.
- **Expected affected areas:**
  `src/AiEng.Platform.App/Components/Pages/DesignSystem.razor`,
  possibly
  `src/AiEng.Platform.App/wwwroot/css/app.css`
  (rebuilt by `npm run css:build`).
- **Validation:** `npm run css:build`; visual
  smoke test on `/design-system`; the M1
  bUnit tests for `AppToolbar` remain green.
- **Approved plan path:** (cosmetic; no
  detailed plan required; can be folded into
  M2.6 if appropriate).
- **Status:** Ready (cosmetic; can be picked
  up at any time).

---

## In Progress

(none — the M4-C.2 first session
delivered in the
m4-c-2-app-provider-list-and-providers-page
session, 2026-07-13; T-029 (M4-C.2
first session) is `Done` in
`.ai/state/tasks.json`; M4-C.2 evidence
block is finalised in
`.ai/state/milestones.json` +
`.ai/state/capabilities.json`; the
`feat(m4-c.2)` closeout commit is on
`main` and the feature branch is
deleted per the branching strategy
rule 7; the next concrete step is
the M4-C closeout (T-030,
`Ready`; the M4-C retrospective +
the M4-C status `Active` → `Done` +
the `m4-c` annotated milestone tag +
the M4-D plan + project-continuity
state) on the user's `Approve` or
`Next` invocation. Per the
Progressive Coding Rule + the M4-C.2
brief: 'Do not begin the following
task', the M4-C.2 first session does
**not** begin the M4-C closeout,
the M4-D plan promotion, or any
provider creation.)

---

## Blocked

### Run M1 design-system `lavish-axi` review (deferred from M1 closeout)

- **Task ID:** `M1-REV-1`
- **Milestone:** M1 — Design System Core
- **Title:** Run `lavish-axi` design-system
  review of the M1 deliverable
- **Why it matters:** The M1 dogfooding
  checkpoint in `ROADMAP.md` § 3 authorises
  the development team to use `lavish-axi`
  externally to review the M1 deliverable.
  The review's findings inform the M2 design
  decisions.
- **Blocker:** `lavish-axi` is not installed on
  the host. The only artefact on the filesystem
  is `agent-workbench/tools/lavish-axi.md`, a
  spec document for an event-bus daemon, not a
  review tool. No review command is documented.
  Per `.ai/workflows/tool-dogfooding.md`, the
  no-silent-fallback rule applies.
- **Unblock path:** (a) install `lavish-axi`
  on the host; (b) the user picks a substitute
  review tool; (c) the user decides the
  `lavish-axi` dogfooding is not the right
  step and removes PART 2 from the brief.
- **Expected affected areas:**
  `.ai/reviews/` (the review record, when
  produced).
- **Validation:** the review report is
  produced; findings are filed with severity
  labels.
- **Approved plan path:** (none — a review
  record is the deliverable, not a plan).
- **Status:** Blocked.

---

## Done Recently

### M4-B.2 — AppCapabilityList + AppKeyValueList design-system components + 28 bUnit tests — 2026-07-13

- **Task ID:** `T-025`
- **Milestone:** M4-B — Capability Detection (Active 2026-07-13; the M4-B plan is at `.ai/plans/M4-B-capability-detection.md`).
- **Title:** `AppCapabilityList` + `AppKeyValueList` data-owning four-state design-system components + `AppKeyValueListFormat` enum + 28 bUnit tests — the boundary slice of M4-B.2 (the second M4-B implementation slice).
- **Why it matters:** M4-B.1 shipped the `IHostCapabilitiesService` contract + the `HostCapabilities` + `HostCapability` records + the `SystemHostCapabilitiesService` implementation + the `AddHostCapabilities` composition root. M4-B.2 ships the `AppCapabilityList` data-owning four-state design-system component (renders an `IReadOnlyList<HostCapability>` as a list of `AppCard` entries with `AppStatusDot` Success/Error, the `Version` in a monospaced muted font, and an `AppBadge` "Credential set" for `CredentialAvailable=true`; `aria-live="polite"` on the populated list) + the `AppKeyValueList` data-owning four-state design-system component (renders an `IReadOnlyList<KeyValuePair<string, string>>` as a definition list `<dl>`/`<dt>`/`<dd>`; the `AppKeyValueListFormat` enum (Plain, Boolean, Code) controls value rendering) + the `AppKeyValueListFormat` enum appended to `Enums.cs` + the `Diagnostics/_Imports.razor` mirroring `Projects/_Imports.razor` + 13 bUnit tests for `AppCapabilityList` + 15 bUnit tests for `AppKeyValueList`.
- **Branch:** `feature/T-025-m4-b-2-capability-list-components` (created from `main` at the M4-B.1 closeout commit `c151e90`; fast-forwarded into `main`; deleted per the branching strategy rule 7).
- **Commit:** `feat(m4-b.2): add AppCapabilityList + AppKeyValueList data-owning design-system components` (push is staged for push, not authorised in this session).
- **Test count:** 370 passed (was 343 pre-M4-B.2; +28 new), 0 failed, 9 skipped (per ADR-016 / M4-D). Breakdown: 99 unit + 259 component + 12 architecture.
- **Validation:** 0 warnings, 0 errors; format clean; all new + modified files are CRLF; `capabilities.json` valid (24 records; C-023 + C-024 added).
- **One documented deviation:** the `AppCapabilityList` default Populated uses an `AppStack` of `AppCard` entries with the `AppStatusDot` in the header (not the `AppBadge`-style credential slot), and the `AppKeyValueList` default Populated uses a definition list (`<dl>`/`<dt>`/`<dd>`) with monospaced `<code>` elements for the Code format; the plan's intent (one card per capability with status dot + version + credential badge; one row per item with key left + value right) is preserved.
- **Session does NOT begin:** M4-B.3 (`/diagnostics` page + startup log + documentation + architecture test) / M4-C / M4-D / provider creation. The next session is M4-B.3 on the user's `Approve` or `Next` invocation.

### M4-B.3 — /diagnostics page + startup log + Capabilities_Resolved_Through_Service architecture test + docs/capabilities.md — 2026-07-13

- **Task ID:** `T-026`
- **Milestone:** M4-B — Capability Detection (Active 2026-07-13; the M4-B plan is at `.ai/plans/M4-B-capability-detection.md`).
- **Title:** `/diagnostics` page + startup capability-report log + `Capabilities_Resolved_Through_Service` architecture test + `docs/capabilities.md` + 4 bUnit page tests — the surface slice of M4-B.3 (the third M4-B implementation slice).
- **Why it matters:** M4-B.1 shipped the `IHostCapabilitiesService` contract + the `HostCapabilities` + `HostCapability` records + the `SystemHostCapabilitiesService` implementation. M4-B.2 shipped the `AppCapabilityList` + `AppKeyValueList` data-owning design-system components. M4-B.3 ships the user-visible surface: the `/diagnostics` page at `src/AiEng.Platform.App/Components/Pages/Diagnostics.razor` (+ `.razor.css`) composing the M4-B.1 contract + the M4-B.2 components + the M1.2 `AppPageHeader` + `AppButton` + `AppCard` + `AppBreadcrumb`; the startup capability-report log in `Program.cs` (10-second `CancellationTokenSource` timeout; `ILogger<Program>`; Information level; try/catch with Warning on failure); the `Capabilities_Resolved_Through_Service` architecture test (Active per the M4-B plan § 2 item 9; deferred from M4-B.1 per the M4-B.1 plan § 14.1 Deviations; scoped to `App/Components/Diagnostics/` to avoid the M4-A.2 Open Action false positive; the test passes — `Diagnostics.razor` correctly uses `@inject IHostCapabilitiesService` and no file in `App/Components/Diagnostics/` contains the forbidden tokens `RunToCompletionAsync` / `ICredentialVault` / `new SystemHostCapabilitiesService`); the `docs/capabilities.md` 10-section documentation mirroring `docs/infrastructure.md` § 1-10; 4 bUnit page tests in `tests/AiEng.Platform.ComponentTests/Pages/DiagnosticsPageTests.cs`; the `docs/design-system.md` § 4.5 update resolving the M4-B.2 deferred decision (`AppCapabilityList` + `AppKeyValueList` rows from `Planned (M4)` to `Implemented (M4-B.2)`).
- **Branch:** `feature/T-026-m4-b-3-diagnostics-page-startup-log-and-architecture-test` (created from `main` at the M4-B.2 closeout commit `b1f0ec8`; fast-forwarded into `main`; deleted per the branching strategy rule 7).
- **Commit:** `feat(m4-b.3): add /diagnostics page, startup capability log, and Capabilities_Resolved_Through_Service architecture test` (push is staged for push, not authorised in this session).
- **Test count:** 376 passed (was 370 pre-M4-B.3; +4 new bUnit page tests + +2 new architecture tests), 0 failed, 9 skipped (per ADR-016 / M4-D). Breakdown: 99 unit + 263 component + 14 architecture.
- **Validation:** 0 warnings, 0 errors; format clean; all new + modified files are CRLF; the 4 state JSON files (`session.json` + `tasks.json` + `milestones.json` + `capabilities.json`) are valid JSON.
- **One documented deviation:** the `Diagnostics.razor` page uses a 1-card-per-capability layout (the M4-B.2 `AppCapabilityList` renders 12 capability list items — 6 host tools + 6 provider credentials). The plan anticipated 6; the actual list size is 12 (6 tools × 2). The 4 bUnit page tests assert the 12-item list; the architecture test scopes the forbidden-token check to the `App/Components/Diagnostics/` folder (not `App/Components/`) to avoid the M4-A.2 Open Action false positive on `AppProjectCard.razor` (which is in `App/Components/Projects/`, not in `App/Components/Diagnostics/`).
- **Session does NOT begin:** M4-C (provider registry) / M4-D (first concrete process providers) / provider creation. The next session is the M4-B closeout session (T-027) on the user's `Approve` or `Next` invocation.

### M4-B closeout — M4-B retrospective + M4-B -> Done + m4-b tag + M4-C plan + project-continuity state — 2026-07-13

- **Task ID:** `T-027`
- **Milestone:** M4-B — Capability Detection (Done 2026-07-13; the M4-B plan is at `.ai/plans/M4-B-capability-detection.md`; the M4-B closeout plan is at `.ai/plans/M4-B-closeout.md`; the M4-B retrospective is at `retrospective-m4-b-capability-detection.md`).
- **Title:** M4-B closeout — M4-B retrospective (13 sections per the Milestone Closeout Standard § 4) + M4-B status `Active` → `Done` with `closed_at: 2026-07-13` in `.ai/state/milestones.json` + `m4-b` annotated milestone tag at the M4-B closeout commit on `main` per the branching strategy rule 9 + the M4-C plan in `Awaiting Approval` at `.ai/plans/M4-C-provider-registry-foundation.md` (12 sections mirroring the M4-A + M4-B plans) + the project-continuity state update per Rule 15 (the 6 state files: `session.json` + `tasks.json` + `current.md` + `task-board.md` + `milestones.json` + `capabilities.json`; + `ROADMAP.md` + `.ai/plans/master-delivery-plan.md`) + the M4-B closeout handoff at `.ai/handoffs/2026-07-13-m4-b-closeout.md` (mirrored to `latest.md`) + the M4-B closeout implementation report at `implementation-report-m4-b-closeout.md`.
- **Why it matters:** M4-B.1 + M4-B.2 + M4-B.3 ship the M4-B boundary slice (the contract + the data-owning components + the user-visible surface + the startup log + the architecture test + the documentation). The M4-B closeout is the closeout slice that aggregates the M4-B.1 + M4-B.2 + M4-B.3 evidence blocks; finalises the M4-B status to `Done`; transitions the next-milestone handoff to M4-C. The M4-B closeout follows the Milestone Closeout Standard (`.ai/workflows/milestone-closeout.md`): the M4-B retrospective aggregates the M4-B.1 + M4-B.2 + M4-B.3 evidence blocks; the M4-C plan is the first next-milestone plan; the `m4-b` annotated milestone tag marks the M4-B boundary in git history (per the branching strategy rule 9); the project-continuity state update clears `C-015` `next_task` on close (the M4-B boundary is closed; the M4-C.1 first session is the next concrete step on the user's `Approve` or `Next` invocation).
- **Branch:** `feature/T-027-m4-b-closeout` (created from `main` at the M4-B.3 closeout commit `ec428cd`; the M4-B closeout commit `chore(m4-b.closeout): close M4-B with retrospective, M4-C plan, and m4-b milestone tag` is on this branch; the branch is fast-forwarded into `main` per the branching strategy rule 6; the branch is deleted per rule 7).
- **Commit:** `chore(m4-b.closeout): close M4-B with retrospective, M4-C plan, and m4-b milestone tag` (push is staged for push, not authorised in this session).
- **Tag:** `m4-b` (annotated; at the M4-B closeout commit on `main` per the branching strategy rule 9; the tag message references the M4-B retrospective path: `M4-B closeout: capability detection. See retrospective-m4-b-capability-detection.md`).
- **Test count:** 376 passed (identical to the M4-B.3 closeout; the M4-B closeout is a docs + workflow + state change with no new tests), 0 failed, 9 skipped (per ADR-016 / M4-D). Breakdown: 99 unit + 263 component + 14 architecture.
- **Validation:** 0 warnings, 0 errors; format clean; all new + modified files are CRLF (unix2dos applied); the 4 state JSON files (`session.json` + `tasks.json` + `milestones.json` + `capabilities.json`) are valid JSON; the `current.md` + `task-board.md` + `ROADMAP.md` + `.ai/plans/master-delivery-plan.md` are CRLF.
- **Zero deviations.** The M4-B closeout follows the Milestone Closeout Standard as-is (the standard is mature enough to be reused without modification; the M2.6 closeout's "introduce the standard" is amortised). The M4-B closeout mirrors the M3 closeout's structure with M4-B-specific evidence; the M4-B retrospective mirrors the M3 retrospective's structure with M4-B-specific evidence.
- **Session does NOT begin:** M4-C.1 (provider registry implementation; T-028 is the next concrete step in the next session on the user's `Approve` or `Next` invocation) / M4-D (first concrete process providers) / provider creation. The M4-B closeout does NOT begin any of the M4-C work (per the brief: 'Do not begin the following task' + the Progressive Coding Rule). The M4-C plan is in `Awaiting Approval` (the M4-C.1 first session begins in a future session on the user's `Approve` or `Next` invocation after the user has approved the M4-C plan).

### M4-C.2 — AppProviderList data-owning design-system component + /providers page + startup provider-registry log + Providers_Resolve_Through_Registry Active architecture test + docs/providers.md + docs/design-system.md § 4.5 AppProviderList row + 13 bUnit component tests + 5 bUnit page tests — 2026-07-13

- **Task ID:** `T-029`
- **Milestone:** M4-C — Provider Registry Foundation (Active 2026-07-13; the M4-C plan is at `.ai/plans/M4-C-provider-registry-foundation.md`).
- **Title:** M4-C.2 — `AppProviderList` data-owning four-state design-system component + `/providers` page + startup provider-registry log in `Program.cs` + `Providers_Resolve_Through_Registry` Active architecture test + `docs/providers.md` 10-section documentation + `docs/design-system.md` § 4.5 `AppProviderList` row update + 13 bUnit component tests + 5 bUnit page tests — the surface slice of M4-C (the second M4-C implementation slice).
- **Why it matters:** M4-C.1 shipped the `IProviderRegistry` contract + the `ProviderDescriptor` + `ProviderFamily` + `ProviderStatus` records + the 6 family registry contracts + the `IProviderFamily` base + the `SystemProviderRegistry` implementation + the 6 no-op family stubs + the `AddProviderRegistry` composition root + the 6 family fakes + 19 unit tests. M4-C.2 ships the user-visible surface: the `AppProviderList` data-owning four-state design-system component (4 data-owning slots Loading/Empty/Error/Populated; 4 state parameters Providers/IsLoading/ErrorMessage/ErrorCode; 4 RenderFragment slot overrides; `AdditionalAttributes` capture; maps `ProviderStatus` to `AppStatusDotVariant`; renders the "Disabled" `AppBadge` for `Status = Disabled`; renders the `Version` in a monospaced muted font; renders the `Metadata` as an `AppKeyValueList` with the Code format; the populated list has `role="list"` + `aria-live="polite"`); the `/providers` page at `src/AiEng.Platform.App/Components/Pages/Providers.razor` (`@page "/providers"` + `[RouteMetadata("/providers", "Providers", Order = 5, ShowInSidebar = true, Icon = "◇", Description = "...")]`; `@layout AppLayout` + `@rendermode InteractiveServer`; `@inject IProviderRegistry Service` + `@inject IHostCapabilitiesService Capabilities` + `@inject IPlatformInfo PlatformInfo`; 6 `AppProviderList` cards, one per `ProviderFamily`; 5-second `CancellationTokenSource` in `LoadAsync`; per-family try/catch with `PROVIDER_LOOKUP_FAILED` + top-level `PROVIDER_LOOKUP_TIMEOUT`; host-metadata `AppKeyValueList` block with Detected at/Data directory/Config directory/Is Windows host; Refresh `AppButton` with `data-testid="refresh-providers"`); the startup provider-registry log in `src/AiEng.Platform.App/Program.cs` (`LogProviderRegistryAsync` method after `LogHostCapabilitiesAsync`; 10-second `CancellationTokenSource`; iterates the 6 `ProviderFamily` values; logs the per-family provider count at Information level; try/catch with `LogWarning` on failure); the `Providers_Resolve_Through_Registry` Active architecture test in `tests/AiEng.Platform.ArchitectureTests/Providers/Providers_Resolve_Through_Registry.cs` (2 `[Fact]` methods: `Providers_page_resolves_providers_through_IProviderRegistry` + `Providers_folder_does_not_reference_process_or_credential_boundary_directly`; scoped to `App/Components/Providers/` to avoid the M4-A.2 Open Action + M4-B.3 `Diagnostics.razor` false positives; forbids `RunToCompletionAsync` + `ICredentialVault` + `new SystemProviderRegistry`); 13 bUnit component tests in `tests/AiEng.Platform.ComponentTests/Components/Providers/AppProviderListTests.cs` (Populated / Empty / Loading / Error / DisplayName / StatusDotSuccess / StatusDotError / Disabled / Version / MutedVersion / Metadata / NoMetadata / CustomPopulated / AriaLive); 5 bUnit page tests in `tests/AiEng.Platform.ComponentTests/Pages/ProvidersPageTests.cs` (calls_ListProvidersAsync_on_init / renders_AppProviderList_per_family / Refresh_reruns / host_metadata / items_per_family) with inline `FakeProviderRegistry` + `FakeHostCapabilitiesService` + `StaticPlatformInfo` + `EmptyNavigationRegistry` test doubles; `docs/providers.md` (10 sections mirroring `docs/capabilities.md` § 1-10: Goals, Project Structure, Contract, Records, Family Registries, Component, Page, Composition Root, Tests, Out of Scope); `docs/design-system.md` § 4.5 `AppProviderList` row in `Implemented (M4-C.2)` status with Notes: `Renders \`ProviderDescriptor[]\` from \`IProviderRegistry.ListProvidersAsync\`; data-owning four-state`.
- **Branch:** `feature/T-029-m4-c-2-provider-list-component-and-page` (created from `main` at the M4-C.1 closeout commit `9ddb5c5`; the M4-C.2 closeout commit `feat(m4-c.2): add AppProviderList data-owning design-system component and /providers page` is on this branch; the branch is fast-forwarded into `main` per the branching strategy rule 6; the branch is deleted per rule 7).
- **Commit:** `feat(m4-c.2): add AppProviderList data-owning design-system component and /providers page` (push is staged for push, not authorised in this session).
- **Test count:** 416 passed (was 395 pre-M4-C.2; +13 new bUnit component + 5 new bUnit page + 1 new active architecture), 0 failed, 9 skipped (per ADR-016 / M4-D). Breakdown: 118 unit + 282 component + 16 architecture.
- **Validation:** 0 warnings, 0 errors; format clean; all new + modified files are CRLF (`unix2dos` applied to every new + modified file); the 4 state JSON files (`session.json` + `tasks.json` + `milestones.json` + `capabilities.json`) are valid JSON; the M4-C.1 architecture tests remain registered-but-disabled per ADR-016 / M4-D; the M4-B.3 `Capabilities_Resolved_Through_Service` architecture test remains active and green; the M4-A.2 `AppProjectCard_resolves_open_through_IProcessRunner` architecture test remains active and green.
- **One documented deviation:** the `Providers_Resolve_Through_Registry` Active architecture test forbids `new SystemProviderRegistry` (the direct-instantiation escape hatch) + `RunToCompletionAsync` + `ICredentialVault`, but does not forbid `Capabilities.DetectAsync(` on the `Providers.razor` page. The page injects `IHostCapabilitiesService` for the host-metadata context (mirrors the M4-B.3 `Diagnostics.razor` pattern, which is allowed by the M4-B.3 `Capabilities_Resolved_Through_Service` architecture test). The test enforces the single-seam rule for `IProviderRegistry`; the capability service is composed through the page, not the registry.
- **Session does NOT begin:** M4-C closeout (the M4-C retrospective + M4-C status `Active` → `Done` + the `m4-c` annotated milestone tag + the M4-D plan; T-030 is the next concrete step in the next session on the user's `Approve` or `Next` invocation) / M4-D (first concrete process providers) / provider creation. The M4-C.2 first session does NOT begin any of the M4-C closeout work (per the brief: 'Do not begin the following task' + the Progressive Coding Rule).

### M4-C.1 — IProviderRegistry contract + 6 family registries + SystemProviderRegistry implementation + 6 family fakes + AddProviderRegistry composition root + 19 unit tests — 2026-07-13

- **Task ID:** `T-028`
- **Milestone:** M4-C — Provider Registry Foundation (Active 2026-07-13; the M4-C plan is at `.ai/plans/M4-C-provider-registry-foundation.md`).
- **Title:** M4-C.1 — `IProviderRegistry` contract + `ProviderDescriptor` + `ProviderFamily` + `ProviderStatus` + 6 family registry contracts + `IProviderFamily` base + `SystemProviderRegistry` implementation + 6 no-op family stubs + `AddProviderRegistry` composition root + 6 family fakes + 19 unit tests — the boundary slice of M4-C (the first M4-C implementation slice).
- **Why it matters:** M4-A + M4-B shipped the infrastructure seam (`IProcessRunner` + `ICredentialVault` + `IPlatformInfo`) + the on-disk `IProjectStore` + the `IHostCapabilitiesService` + the `AppCapabilityList` + `AppKeyValueList` + the `/diagnostics` page + the startup capability-report log + the `Capabilities_Resolved_Through_Service` architecture test + `docs/capabilities.md`. M4-C.1 ships the provider registry foundation (boundary slice): the `IProviderRegistry` contract (the single allowed seam between the application and the provider layer; per the M4-C architecture, all provider access flows through `IProviderRegistry`, never through the concrete `SystemProviderRegistry` or any `IProvider` implementation) + the `ProviderDescriptor` sealed record (data envelope: `Id` + `DisplayName` + `Family` + `Status` + `Version?` + `Metadata` dictionary) + the `ProviderFamily` enum (6 values: Git, AgentRuntime, Review, QualityGate, AutonomousLoop, Orchestration) + the `ProviderStatus` enum (3 values: Available, Unavailable, Disabled) + the `IProviderFamily` base interface + the 6 family-specific subinterfaces (`IGitProviderFamily`, `IAgentRuntimeProviderFamily`, `IReviewProviderFamily`, `IQualityGateProviderFamily`, `IAutonomousLoopProviderFamily`, `IOrchestrationProviderFamily`) in `src/AiEng.Platform.Application/Providers/Families/` + the `SystemProviderRegistry` implementation in `src/AiEng.Platform.Infrastructure/Providers/` (aggregates the 6 family registries through `IProviderFamily`; consumes `IHostCapabilitiesService` through DI; downgrades `Available` descriptors to `Unavailable` when the family capability is not available; preserves `Disabled` and `Unavailable` regardless; logs at `Information` level with the per-family lookup count) + the 6 no-op family stub implementations in `src/AiEng.Platform.Infrastructure/Providers/Families/` + the `AddProviderRegistry` composition root extension in `src/AiEng.Platform.App/Composition/Providers/` (registers the 6 no-op family stubs + `IProviderRegistry` → `SystemProviderRegistry` via `TryAddSingleton`) + the `AddProviderRegistry` wire-up in `AddPlatformServices` after `AddInfrastructure` + `AddHostCapabilities` + the 6 family fakes in `tests/AiEng.Platform.UnitTests/Providers/` + 19 unit tests in `SystemProviderRegistryTests.cs` (8 constructor-null tests + 7 happy-path/edge-case tests + 1 log test + 2 dispatch tests + 1 capability-call-count test + 1 `ArgumentOutOfRangeException` test + 1 fake-CallCount test + `FakeHostCapabilitiesService` + `ListLogger<T>`).
- **Branch:** `feature/T-028-m4-c-1-provider-registry-contract-and-family-registries` (created from `main` at the M4-B closeout commit `72d85b3`; the M4-C.1 closeout commit `feat(m4-c.1): add IProviderRegistry contract, family registries, SystemProviderRegistry implementation, family fakes, and AddProviderRegistry composition root` is on this branch; the branch is fast-forwarded into `main` per the branching strategy rule 6; the branch is deleted per rule 7).
- **Commit:** `feat(m4-c.1): add IProviderRegistry contract, family registries, SystemProviderRegistry implementation, family fakes, and AddProviderRegistry composition root` (push is staged for push, not authorised in this session).
- **Test count:** 395 passed (was 376 pre-M4-C.1; +19 new), 0 failed, 9 skipped (per ADR-016 / M4-D). Breakdown: 118 unit + 263 component + 14 architecture.
- **Validation:** 0 warnings, 0 errors; format clean; all new + modified files are CRLF; the 4 state JSON files (`session.json` + `tasks.json` + `milestones.json` + `capabilities.json`) are valid JSON; the M4-A.1 architecture tests (`Infrastructure_Respects_ProcessBoundary` + `Infrastructure_Respects_CredentialBoundary`) remain registered-but-disabled per ADR-016 / M4-D.
- **One documented deviation:** the M4-C.1 plan registered the 6 family fakes (test-only types) as the family registry implementations in the `AddProviderRegistry` composition root. This violated the DI layering rule (the `App` project cannot reference `AiEng.Platform.UnitTests`). The fix was: (a) 6 no-op family stub implementations were created in `src/AiEng.Platform.Infrastructure/Providers/Families/` (`GitProviderFamily` + 5 others) — the stubs return an empty `IReadOnlyList<ProviderDescriptor>` (no concrete providers ship in M4-C.1; the providers come in M4-D per the brief: 'Do not create providers'); (b) `AddProviderRegistry` registers the Infrastructure no-op stubs, not the test fakes; (c) the 6 family fakes stay in `tests/AiEng.Platform.UnitTests/Providers/` and are passed to `SystemProviderRegistry` via constructor injection in unit tests. The deviation preserves the M4-C.1 architecture (the family registries exist; the `SystemProviderRegistry` aggregates them; `IProviderRegistry` is the single seam) and the test surface (19 unit tests assert the `SystemProviderRegistry` behavior via constructor-injected fakes).
- **Session does NOT begin:** M4-C.2 (the `AppProviderList` data-owning design-system component + the `/providers` page + the startup provider-registry log + the `Providers_Resolve_Through_Registry` architecture test + `docs/providers.md` + the `docs/design-system.md` § 4.5 update; T-029 is the next concrete step in the next session on the user's `Approve` or `Next` invocation) / M4-C closeout (T-030; the M4-C retrospective + the M4-C status `Active` → `Done` + the `m4-c` annotated milestone tag + the M4-D plan) / M4-D (first concrete process providers) / provider creation. The M4-C.1 first session does NOT begin any of the M4-C.2 work (per the brief: 'Do not begin the following task' + the Progressive Coding Rule).

### M4-B.1 — IHostCapabilitiesService contract + implementation + composition root + unit tests — 2026-07-13

- **Task ID:** `T-024`
- **Milestone:** M4-B — Capability Detection (Active 2026-07-13; the M4-B plan is at `.ai/plans/M4-B-capability-detection.md`).
- **Title:** IHostCapabilitiesService contract + `SystemHostCapabilitiesService` implementation + `AddHostCapabilities` composition root + 20 unit tests — the boundary slice of M4-B (the first M4-B implementation slice).
- **Why it matters:** M4-A.1 + M4-A.2 shipped the infrastructure seam (`IProcessRunner` + `ICredentialVault` + `IPlatformInfo`) and the first activation (Open action on `AppProjectCard`). M4-B.1 ships the `IHostCapabilitiesService` contract + the `HostCapabilities` + `HostCapability` records + the `SystemHostCapabilitiesService` implementation (composes the three M4-A contracts; probes six host tools — `git`, `ollama`, `powershell.exe`, `wsl.exe`, `wt.exe`, `bash.exe` — with `--version` via `IProcessRunner.RunToCompletionAsync`; reads six provider credentials via `ICredentialVault.GetAsync("provider:<key>:token", ct)`; 5-second per-tool `CancellationTokenSource` timeout linked with the outer token; `IPlatformInfo.IsWindows` gating for Windows-only tools; outer-cancellation propagation via re-throw) + the `AddHostCapabilities` composition root extension (`TryAddSingleton<IHostCapabilitiesService, SystemHostCapabilitiesService>`) + the wire-up in `AddPlatformServices` + 20 unit tests + 3 in-line test doubles (`FakeProcessRunner`, `FakeCredentialVault`, `FakePlatformInfo`).
- **Branch:** `feature/T-024-m4-b-1-host-capabilities-contract-and-service` (created from `main` at the M4-B plan promotion commit `131b8bd`; fast-forwarded into `main`; deleted per the branching strategy rule 7).
- **Commit:** `feat(m4-b.1): add IHostCapabilitiesService contract and SystemHostCapabilitiesService implementation` (push is staged for push, not authorised in this session).
- **Test count:** 343 passed (was 323 pre-M4-B.1; +20 new), 0 failed, 9 skipped (per ADR-016 / M4-D).
- **Validation:** 0 warnings, 0 errors; format clean; all new + modified files are CRLF.
- **Two documented deviations:** (1) The `Capabilities_Resolved_Through_Service` architecture test is deferred to M4-B.3 (the test asserts `Diagnostics.razor` exists; the file does not exist in M4-B.1). (2) The `DetectedAt` test was split into two tests: deterministic TimeProvider assertion + non-deterministic call-window assertion.
- **Session does NOT begin:** M4-B.2 (design-system components) / M4-B.3 (page + startup log + docs + architecture test) / M4-C / M4-D / provider creation. The next session is M4-B.2 on the user's `Approve` or `Next` invocation.

### M4-A.2 — Open action on AppProjectCard (IProcessRunner activation) — 2026-07-11

- **Task ID:** `T-022`
- **Milestone:** M4-A — Infrastructure /
  Process Execution (Active 2026-07-11;
  M4-A.1 + M4-A.2 Delivered 2026-07-11)
- **Title:** Open action on
  `AppProjectCard` — the second M4-A
  implementation slice; the first
  process-boundary activation; the
  M4-A.1 `IProcessRunner` seam is
  activated by the Open button on
  `AppProjectCard` (Windows-only;
  gated on `IPlatformInfo.IsWindows`;
  exception-swallowed with inline
  `OpenError`).
- **Status:** **Done (delivered
  2026-07-11).** M4-A.2 ships the
  Open action on `AppProjectCard`:
  the card `@inject`s `IProcessRunner` +
  `IPlatformInfo` + `ILogger<AppProjectCard>`
  directly; the click handler calls
  `IProcessRunner.RunToCompletionAsync("explorer.exe",
  new[] { Project.Path }, default)`;
  the button is gated on
  `IPlatformInfo.IsWindows`; the
  exception path
  (`Win32Exception` +
  `InvalidOperationException` +
  `IOException`) is swallowed and
  surfaced as a transient inline
  `OpenError`. The M4-A.2 also
  extends `IPlatformInfo` with
  `bool IsWindows { get; }`
  (implemented in `SystemPlatformInfo`
  via
  `RuntimeInformation.IsOSPlatform(OSPlatform.Windows)`).
- **Outcome:** M4-A.2 ships 6 new
  files (the implementation report,
  the per-session handoff, and 4
  state-file updates are
  docs/state; the actual code
  changes are 5 source files
  modified + 1 source file created
  for the architecture test): (1)
  **`IPlatformInfo` extended with
  `IsWindows`**; (2)
  **`SystemPlatformInfo` implements
  `IsWindows`**; (3) **`AppProjectCard.razor`
  enables the Open button** (the
  M3.2 `Disabled="true"` is replaced
  with the computed
  `Disabled="@(!IsWindowsHost)"`; the
  click handler calls
  `IProcessRunner.RunToCompletionAsync("explorer.exe",
  new[] { Project.Path }, default)`;
  the `OpenError` is rendered as a
  transient inline
  `<div class="app-project-card-open-error"
  role="alert">`); (4) **The scoped
  CSS class
  `.app-project-card-open-error`** in
  `AppProjectCard.razor.css`; (5)
  **5 new bUnit tests** in
  `AppProjectCardTests.cs`
  (`Open_Button_Is_Enabled_When_Host_Is_Windows`,
  `Open_Button_Is_Disabled_When_Host_Is_Not_Windows`,
  `Clicking_Open_Invokes_IProcessRunner_With_Explorer_And_ProjectPath`,
  `Open_Click_Passes_ProjectPath_Single_Element_As_Argument`,
  `Open_Click_Swallows_Process_Exceptions`)
  + `FakeProcessRunner` and
  `FakePlatformInfo` test doubles;
  the M3.2
  `Open_Button_Remains_Disabled_In_M3_2`
  test is deleted; (6) **1 new
  active architecture test**
  `AppProjectCard_resolves_open_through_IProcessRunner`
  in
  `PagesResolveProjectsThroughServiceTests.cs`
  (asserts the card uses
  `@inject IProcessRunner` and
  contains no `Process.Start` or
  `ProcessStartInfo` token). (7)
  **Documentation**: `docs/infrastructure.md`
  § 7 rewritten from future tense
  to delivered tense; `docs/infrastructure.md`
  § 6 updated to document the
  `IsWindows` property; § 9 Tests
  cumulative count updated; § 10
  Out of Scope updated; `docs/projects.md`
  § 1/§ 4/§ 5.1/§ 7.2/§ 7.3 updated;
  `ROADMAP.md` and
  `.ai/plans/master-delivery-plan.md`
  updated; `.ai/state/capabilities.json`
  C-012 updated to `Delivered`.
- **Validation:** 323 passed, 0
  failed, 9 skipped (per ADR-016 /
  M4-D); 0 warnings, 0 errors; format
  clean; visual smoke on `/projects`
  returns 200; the Open action is
  enabled on Windows hosts and
  disabled with a tooltip on
  non-Windows hosts. M4-A.2 is +5
  bUnit + 1 new active architecture
  vs M4-A.1 closeout (the 2 M4-A.1
  architecture tests remain
  registered-but-disabled per
  ADR-016).
- **Branch / commit:** branch
  `feature/T-022-m4-a-2-open-action`
  (created from `main` at the M4-A.1
  closeout commit); commit
  `feat(m4-a.2): enable AppProjectCard.Open action via IProcessRunner`
  is on this branch; the branch is
  fast-forwarded into `main` per the
  branching strategy rule 6; the
  branch is deleted per rule 7.
- **Implementation report:**
  `implementation-report-m4-a-2-open-action.md`
  (15+ sections, mirrors the
  M4-A.1 / M3 closeout reports).
- **Per-session handoff:**
  `.ai/handoffs/2026-07-11-m4-a-2-open-action.md`
  (mirrored to
  `.ai/handoffs/latest.md`).
- **Push decision:** Staged for push
  (not authorised in this session;
  the user may push in a follow-up
  command per the command protocol).
- **Next step:** The M4-A.2
  session does **not** begin
  M4-A.3 (not yet defined) or
  M4-B / M4-C / M4-D (per the
  brief: 'Do not begin the
  following task'). The next
  session is the M4-A.3
  implementation (if defined) or
  the M4-B plan promotion.

### M4-A.1 — Infrastructure project skeleton — 2026-07-11

- **Task ID:** `T-021`
- **Milestone:** M4-A — Infrastructure /
  Process Execution (Active 2026-07-11;
  M4-A.1 Delivered 2026-07-11)
- **Title:** Infrastructure project
  skeleton (`IProcessRunner`,
  `ICredentialVault`, `IPlatformInfo`,
  on-disk `IProjectStore`) — the first
  M4-A implementation slice
- **Status:** **Done (delivered
  2026-07-11).** M4-A.1 ships the
  infrastructure seam every later
  milestone composes. M4-A.2 — the
  Open action on `AppProjectCard`
  (T-022) — is `Done` (delivered
  2026-07-11; see the M4-A.2 entry
  in Done Recently). The next M4-A
  task is undefined; the next
  milestone is M4-B (Capability
  Detection, Planned).
- **Outcome:** M4-A.1 is the
  **boundary**, not the activation.
  The slice ships 21 new files + 16
  modifications + 1 deletion across
  the contracts, the implementations,
  the composition root, the tests,
  the architecture tests, the
  documentation, and the project
  wiring: (1) **The new csproj**
  `AiEng.Platform.Infrastructure`
  (references `Application` + `Domain`;
  `Nullable` + `TreatWarningsAsErrors`
  per `Directory.Build.props`); (2)
  **The four contracts** in
  `Application/Infrastructure/`
  (`IProcessRunner` with `RunAsync`
  + `RunToCompletionAsync`;
  `ProcessResult` record struct;
  `ICredentialVault` with
  get/set/delete; `IPlatformInfo` with
  data/config directories); (3) **The
  four implementations** in
  `Infrastructure/` (`SystemProcessRunner`
  wrapping `System.Diagnostics.Process`;
  `WindowsCredentialVault` wrapping
  `advapi32.dll` via direct P/Invoke —
  no NuGet; `SystemPlatformInfo`
  resolving `%LOCALAPPDATA%\AiEng\Platform\<data|config>`;
  `JsonFileProjectStore` with
  `SemaphoreSlim` thread safety,
  `File.Replace` atomic writes, and
  corruption recovery); (4) **The
  `AddInfrastructure` composition root
  extension**; (5) **The one-line
  swap in `AddProjects`** (the M3
  in-memory `IProjectStore`
  registration is removed; the on-disk
  store is now registered through
  `AddInfrastructure`); (6) **The M3
  `InMemoryProjectStore` moved to
  `tests/AiEng.Platform.UnitTests/Infrastructure/`**
  as a test fixture; (7) **The new
  `<ProjectReference>` to
  `AiEng.Platform.Infrastructure`**
  in `App` + the new project added
  to `AiEng.Platform.slnx`; (8)
  **45 new unit tests** across 4 new
  test files (`IProcessRunnerTests`,
  `WindowsCredentialVaultTests`,
  `SystemPlatformInfoTests`,
  `JsonFileProjectStoreTests`); (9)
  **2 new architecture tests
  registered-but-disabled per
  ADR-016**
  (`Infrastructure_Respects_ProcessBoundary`,
  `Infrastructure_Respects_CredentialBoundary`); (10)
  **`docs/infrastructure.md`** (10
  sections); (11) **The M3
  `docs/projects.md` M3 / M4-A
  Boundary section** updated to
  reflect M4-A delivered.
  Cumulative test count after M4-A.1:
  318 passed, 0 failed, 9 skipped
  (the 7 from M3 + the 2 new
  architecture tests). Three
  documented deviations: (1)
  `WindowsCredentialVault` uses
  direct P/Invoke (no NuGet — minimal
  binary footprint; the M4-A plan's
  `CredentialManagement` suggestion
  was overridden after the M4-A.1
  research session); (2) `JsonFileProjectStore`
  uses `File.Replace` for atomic
  Windows file replacement (the M4-A
  plan's "atomic writes via temp
  file + rename" was implemented
  with `File.Replace` for cross-Windows-
  version robustness); (3) M4-A.1
  ships 45 new unit tests (within
  the plan's 50+ bound; the `IPlatformInfo`
  test count is intentionally small
  because the contract is two
  methods on a 1-line platform
  resolution).
- **Report:**
  `implementation-report-m4-a-1-infrastructure-project-skeleton.md`.
- **Handoff:**
  `.ai/handoffs/2026-07-11-m4-a-1-infrastructure-project-skeleton.md`
  (mirrored at
  `.ai/handoffs/latest.md`).
- **Git:** branch
  `feature/T-021-m4-a-1-infrastructure-project-skeleton`
  (created from `main` at the M3
  closeout commit `33c154d`; the
  M4-A.1 closeout commit
  `feat(m4-a.1): add infrastructure
  project skeleton with
  IProcessRunner, ICredentialVault,
  IPlatformInfo, and on-disk
  IProjectStore` is on this branch;
  the branch is fast-forwarded into
  `main` per the branching strategy
  rule 6; the branch is deleted per
  rule 7). No remote push (push is
  not authorised in this session;
  the user may push in a follow-up
  command per the command protocol).
- **Next action:** the M4-A.1
  closeout promotes M4-A.2 (T-022 —
  the Open action on `AppProjectCard`;
  the first `IProcessRunner`
  activation) to `Ready`. The next
  session starts the M4-A.2
  implementation per the Progressive
  Coding Rule. The M4-A.1 session
  does **not** begin M4-A.2 (per
  the brief: "Do not begin the
  following task").

### M3.2 closeout session — 2026-07-11

- **Task ID:** `T-019`
- **Milestone:** M3 — Project Registration
  (Active 2026-07-11)
- **Title:** Project registration slice 2
  (the registration form, the rename
  form, the unregister confirmation)
- **Status:** **Done (delivered 2026-07-11).**
  M3.2 is the second M3 implementation
  slice. M3 is **Active**; M3.x (the M3
  closeout) is the next `Ready` task.
- **Outcome:** M3.2 ships the three
  mutations the M3 surface exists for
  end-to-end: (1) **The registration
  modal** (`RegisterProjectForm.razor`;
  HTML5 native `<dialog open>`; name
  + folder path fields; validates
  non-empty name + non-empty path;
  resolves `IProjectService` through
  the composition root; calls
  `RegisterAsync`; emits
  `OnRegistered` on success); (2) **The
  rename modal** (`RenameProjectForm.razor`;
  pre-fills the project's current
  name; validates the new name is
  non-empty + differs from the current
  name; calls `RenameAsync`; emits
  `OnRenamed` on success); (3) **The
  unregister confirmation**
  (`ConfirmUnregisterProject.razor`;
  shows the project name; calls
  `UnregisterAsync`; emits
  `OnUnregistered` on success); (4) **The
  `AppProjectCard` Rename + Unregister
  buttons** are enabled; the Open
  button remains disabled (M4-A's
  responsibility); (5) **The page
  header's Register a project
  button** is enabled; clicking it
  opens the registration modal; (6)
  **The `AppProjectList` exposes
  `ShowRegisterDialog()`** (the page
  header delegates to it) and
  `RefreshAsync()` (the form
  components invoke it on success);
  (7) **The architecture test
  `Pages_Resolve_Projects_Through_Service`**
  is extended with three new tests
  covering the three new form
  components; the single-seam rule
  holds for every form; (8) **The
  surface documentation** at
  `docs/projects.md` documents the
  enabled M3.2 actions. 273 total
  tests pass (34 unit + 228 bUnit +
  11 architecture); 7 skipped per
  ADR-016 / M4-D; 0 warnings, 0
  errors; format clean; visual smoke
  (`curl http://localhost:5286/projects`
  returns 200; the Register a project
  button is enabled; clicking it
  opens the registration modal;
  registering / renaming /
  unregistering a project works
  end-to-end). Three documented
  deviations: (1) `AppDialog` is
  not introduced; HTML5 native
  `<dialog>` is used directly
  (minimum-blast-radius; the M1.2
  design system does not ship a
  dialog primitive; the design
  system is not extended); (2) M3.2
  unit tests are reused from M3.1
  (the M3.1 `IProjectServiceTests`
  already cover the `RegisterAsync`
  / `RenameAsync` / `UnregisterAsync`
  happy-path + failure paths); (3)
  Disabled tests are unchanged (the
  7 registered-but-disabled tests
  remain skipped per ADR-016 / M4-D).
- **Report:**
  `implementation-report-m3-2-project-registration-slice-2.md`.
- **Handoff:**
  `.ai/handoffs/2026-07-11-m3-2-project-registration-slice-2.md`
  (mirrored at
  `.ai/handoffs/latest.md`).
- **Git:** branch
  `feature/T-019-m3-2-project-registration-slice-2`
  (created from `main` at the M3.1
  closeout commit; the M3.2 closeout
  commit
  `feat(m3.2): enable project registration form, rename, and unregister`
  is on this branch; the branch is
  fast-forwarded into `main` per the
  branching strategy rule 6; the
  branch is deleted per rule 7). No
  remote push (push is not authorised
  in this session; the user may push
  in a follow-up command per the
  command protocol).
- **Next action:** the M3.2 closeout
  promotes the M3 closeout (T-020) to
  `Ready`. The next session reads the
  Milestone Closeout Standard, drafts
  `.ai/plans/M3-closeout.md`, and
  follows the standard to close M3
  with a retrospective. The M3.2
  session does **not** close M3 (per
  the brief: "Do not begin the
  following task").

### M3 closeout — M3 retrospective (per the Milestone Closeout Standard) — 2026-07-11

- **Task ID:** `T-020`
- **Milestone:** M3 — Project Registration
  (closed 2026-07-11)
- **Title:** M3 closeout — the M3
  retrospective per the Milestone
  Closeout Standard; the third M3
  slice (M3.1 + M3.2 + M3.x)
- **Status:** **Done (delivered
  2026-07-11).** M3 is **Done (closed
  2026-07-11)**. The M4-A plan is in
  `Awaiting Approval`; T-021 (M4-A.1
  infrastructure project skeleton) is
  the next `Ready` task.
- **Outcome:** The M3 closeout ships
  per the Milestone Closeout Standard
  § 4 + § 8: (1) **The M3 retrospective**
  at
  `retrospective-m3-project-registration.md`
  (13 sections: delivered
  capabilities, deferred capabilities,
  technical debt, known issues,
  lessons learned, architecture
  changes, documentation changes,
  testing summary, validation results,
  implementation reports, commit
  range, readiness for M4-A,
  recommendations for the next
  milestone); the M3 retrospective is
  the second milestone retrospective
  in the repository (the M2
  retrospective was the first). (2)
  **The M4-A plan** at
  `.ai/plans/M4-A-infrastructure-process-execution.md`
  (12 sections; Status: Awaiting
  Approval; the first M4-A task T-021
  is `Ready`). (3) **The M3 closeout
  plan** at `.ai/plans/M3-closeout.md`
  (mirrors the M2.6 closeout plan's
  structure). (4) **The M3 closeout
  implementation report** at
  `implementation-report-m3-closeout.md`
  (15+ sections; mirrors the M2.6
  closeout report). (5) **The M3
  closeout per-session handoff** at
  `.ai/handoffs/2026-07-11-m3-closeout.md`
  (mirrored to
  `.ai/handoffs/latest.md`). (6)
  **M3 moved from `Active` to `Done`**
  in `.ai/state/milestones.json` with
  `closed_at: 2026-07-11`. (7) **The
  `m3` annotated milestone tag** at
  the M3 closeout commit on `main`
  per the branching strategy rule 9.
  (8) **The project-continuity state
  updated** per Rule 15: session.json,
  tasks.json, current.md, task-board.md,
  milestones.json, ROADMAP.md,
  master delivery plan, the M3
  closeout handoff, the M3 closeout
  implementation report. (9)
  **Validation gate passed:** 273
  passed, 0 failed, 7 skipped (per
  ADR-016 / M4-D); 0 warnings, 0
  errors; format clean; visual smoke
  on `/projects` green.
- **Report:**
  `implementation-report-m3-closeout.md`.
- **Handoff:**
  `.ai/handoffs/2026-07-11-m3-closeout.md`
  (mirrored at
  `.ai/handoffs/latest.md`).
- **Git:** branch
  `feature/T-020-m3-closeout-and-retrospective`
  (created from `main` at the M3.2
  closeout commit `ff9010a`; the M3
  closeout commit
  `chore(m3.closeout): close M3 with retrospective, M4-A plan, and m3 milestone tag`
  is on this branch; the branch is
  fast-forwarded into `main` per the
  branching strategy rule 6; the
  branch is deleted per rule 7). The
  `m3` annotated milestone tag is at
  the M3 closeout commit on `main`
  per rule 9. No remote push (push
  is not authorised in this session;
  the user may push in a follow-up
  command per the command protocol).
- **Next action:** the M3 closeout
  promotes the M4-A.1 (T-021) to
  `Ready`. The next session approves
  the M4-A plan and begins the
  M4-A.1 implementation per the
  Progressive Coding Rule. The M3
  closeout session does **not**
  begin M4-A (per the brief: "Do not
  begin the following task").

### M3.1 closeout session — 2026-07-11

- **Task ID:** `T-018`
- **Milestone:** M3 — Project Registration
  (Active 2026-07-11)
- **Title:** Project registration slice 1
  (the contract, the in-memory store, the
  project-registration page, the projects
  list)
- **Status:** **Done (delivered 2026-07-11).**
  M3.1 is the first M3 implementation slice.
  M3 is **Active**; M3.2 is the next
  `Ready` task.
- **Outcome:** M3 surface lands end-to-end
  as a single slice: (1) **The contract**
  (`IProjectStore`, `IProjectService`) at
  `src/AiEng.Platform.Application/Projects/`;
  (2) **The in-memory store**
  (`InMemoryProjectStore`;
  `ConcurrentDictionary<Guid, Project>`;
  the M3 smoke test for the contract; M4-A
  swaps the `IProjectStore` registration
  in `AddProjects`); (3) **The domain
  entity** (`Project` aggregate root;
  immutable `Id`, human `Name`, absolute
  `Path`, immutable `CreatedAt`, mutable
  `LastUsedAt?`); (4) **The composition
  root**
  (`ProjectsServiceCollectionExtensions.AddProjects`); (5)
  **The UI surface** (`AppProjectCard`,
  `AppProjectList`, `/projects` page;
  page composes `AppPageHeader` +
  `AppBreadcrumb` (M2.3) +
  `AppProjectList`; sidebar entry
  registered through the M2.2
  `INavigationRegistry`); (6) **The
  architecture test**
  `Pages_Resolve_Projects_Through_Service`
  (enforces the single-seam rule on the
  page and the list); (7) **The unit + bUnit
  test coverage** (27 new unit tests: 16
  `IProjectServiceTests` + 11
  `InMemoryProjectStoreTests`; 13 new bUnit
  tests: 5 `AppProjectCardTests` + 4
  `AppProjectListTests` + 4
  `ProjectsPageTests`; 2 new architecture
  tests in
  `PagesResolveProjectsThroughServiceTests`);
  (8) **The surface documentation** at
  `docs/projects.md` (9 sections: Goals,
  Project Entity, Contract, M3/M4-A
  Boundary, UI Surface, Composition Root,
  Tests, Out of Scope, Acceptance Criteria).
  240 total tests pass (34 unit + 198
  bUnit + 8 architecture); 7 skipped per
  ADR-016 / M4-D; 0 warnings, 0 errors;
  format clean; visual smoke
  (`curl http://localhost:5286/projects`
  returns 200 with the expected markers).
  Three documented deviations: (1)
  `ValidationError` is a class, not a
  struct (`T?` semantics on the
  `Result<T>.Error` slot forced the
  change); (2) `IClock` is realised
  through .NET 8+ `TimeProvider`; (3)
  Disabled tests unchanged.
- **Report:**
  `implementation-report-m3-1-project-registration-slice-1.md`.
- **Handoff:**
  `.ai/handoffs/2026-07-11-m3-1-project-registration-slice-1.md`
  (mirrored at `.ai/handoffs/latest.md`).
- **Git:** branch
  `feature/m3-1-project-registration-slice-1`
  (created from `main` at the M2.6
  closeout commit; the M3.1 closeout
  commit
  `feat(m3.1): add project registration surface`
  is on this branch; the branch is
  fast-forwarded into `main` per the
  branching strategy rule 6; the branch
  is deleted per rule 7). No remote
  push (push is not authorised in this
  session; the user may push in a
  follow-up command per the command
  protocol).
- **Next action:** the M3.1 closeout
  promotes M3.2 (T-019) to `Ready`
  and creates the M3.2 plan at
  `.ai/plans/M3.2-project-registration-slice-2.md`
  (Status: Awaiting Approval); the
  next session approves the M3.2 plan
  and starts the M3.2 implementation
  per the Progressive Coding Rule. The
  M3.1 session does **not** implement
  M3.2 (per the brief: "Do not begin
  the following task").

### M2.6 closeout session — 2026-07-11

- **Task ID:** `M2.6`
- **Milestone:** M2 — Application Shell and
  Navigation (closed 2026-07-11)
- **Title:** M2 closeout and external
  Treehouse dogfooding checkpoint
- **Status:** **Done (closed 2026-07-11).**
  M2 is closed. The `m2` annotated
  milestone tag is at the M2 closeout
  commit on `main`.
- **Outcome:** Four sub-deliverables
  in one slice: (1) **The Milestone
  Closeout Standard** at
  `.ai/workflows/milestone-closeout.md`
  (10 sections; the canonical procedure
  every future milestone closeout must
  follow; the 13-section retrospective is
  the standard's required deliverable;
  the standard is the single source of
  truth for milestone closeout procedures;
  the standard is referenced from
  `.ai/README.md`'s workflows directory
  and task-routing table); (2) **The M2
  retrospective** at
  `retrospective-m2-application-shell-and-navigation.md`
  (13 sections, all populated; the first
  milestone retrospective in this
  repository; delivered capabilities C-019
  + C-022; deferred capabilities; technical
  debt; known issues; lessons learned —
  process + technical; architecture
  changes — ADR-005, ADR-013, ADR-014,
  ADR-016; documentation changes; testing
  summary 197 + 7 skipped; validation
  results 6 gates; implementation reports
  the 6 paths; commit range 11 commits
  from M0.5 closeout to M2 closeout;
  readiness for M3; recommendations for
  M3 — 8 concrete recommendations the M3
  plan accounts for); (3) **Project-
  continuity state + ROADMAP.md +
  master-delivery-plan.md updates**
  (M2.6 from in_progress to delivered;
  M2 from Active to Done with
  closed_at 2026-07-11; M2 evidence block
  updated; M3 plan path added; T-016
  Done; T-006 M2 summary Done; T-018
  M3.1 promoted to Ready; ROADMAP.md
  M2 row Done + M2.6 row Delivered +
  M2 DoD expanded; master-delivery-plan
  M2 row Done + M2.6 slice row Delivered
  + M2 evidence list updated); (4) **M3
  plan + first M3 task promotion** (the
  M3 plan at
  `.ai/plans/M3-project-registration.md`
  Status: Awaiting Approval; the first M3
  task T-018 M3.1 Ready; the M2 closeout
  commit
  `chore(m2.6): close M2 with retrospective,
  milestone closeout standard, and M3 plan`
  on the feature branch
  `feature/T-016-m2-closeout-and-treehouse-dogfooding`
  fast-forwarded into `main`; the
  feature branch deleted per the
  branching strategy rule 7; the `m2`
  annotated milestone tag at the M2
  closeout commit on `main` per rule 9).
- **Validation results:** the milestone-
  level validation gate per the Milestone
  Closeout Standard § 3 — `npm run
  css:build` (exit 0), `dotnet restore`
  (exit 0), `dotnet build` (0 warnings,
  0 errors), `dotnet test` (197 passed,
  0 failed, 7 skipped per ADR-016),
  `dotnet format --verify-no-changes`
  (exit 0), visual smoke (5 routes
  return 200 on `localhost:5211`; theme
  toggle markup present on every
  `AppLayout` page). Every gate is
  green.
- **Report:**
  `implementation-report-m2-6-m2-closeout.md`.
- **Retrospective:**
  `retrospective-m2-application-shell-and-navigation.md`.
- **Standard:**
  `.ai/workflows/milestone-closeout.md`.
- **Handoff:**
  `.ai/handoffs/2026-07-11-m2-6-m2-closeout.md`
  (mirrored at `.ai/handoffs/latest.md`).
- **Git:** branch
  `feature/T-016-m2-closeout-and-treehouse-dogfooding`
  (created from `main` at the M2.5
  closeout commit; the M2 closeout commit
  is on this branch; the branch is
  fast-forwarded into `main` per the
  branching strategy rule 6; the branch
  is deleted per rule 7; the `m2`
  annotated milestone tag is at the M2
  closeout commit on `main` per rule 9).
  No remote push (push is not authorised
  in this session; the user may push in
  a follow-up command per the command
  protocol).
- **Next action:** the M3 session
  follows per the Progressive Coding
  Rule. The M3 plan is in `Awaiting
  Approval`; the first M3 task
  (T-018 — M3.1) is `Ready`; the next
  session reads the M3 plan and the M2
  retrospective's § 13 recommendations,
  approves the M3 plan, and starts the
  M3.1 implementation per the
  Progressive Coding Rule.

### M2.5 closeout session — 2026-07-11

- **Task ID:** `M2.5`
- **Milestone:** M2 — Application Shell and
  Navigation
- **Title:** Empty routes, responsive,
  and accessibility (with T-017 theme
  toggle fix included at user opt-in)
- **Status:** **Done (closed 2026-07-11).**
- **Outcome:** Five sub-deliverables:
  (1) **Empty routes** —
  `Home.razor` and `NotFound.razor`
  rewritten to use `AppCard` +
  `AppEmptyState` with links to
  `/dashboard` and `/design-system`;
  (2) **Responsive matrix** —
  `AppLayout.razor.css` now has
  `@media` rules for the `lg` (≥1440),
  `md` (1280–1439), and `sm`
  (1024–1279) breakpoints; the content
  area gets `overflow-y: auto`; the
  topbar remains horizontal; the
  sidebar widths are 14rem / 12rem /
  10rem / 8rem across the breakpoints;
  `docs/ui-principles.md` § 10.1
  documents the matrix with the M2.5
  implementation;
  (3) **A11y audit** —
  `KeyboardSmokeTests` (4 tests),
  `AriaCurrentInvariantTests` (5 tests
  covering breadcrumb `aria-current`,
  `NavLink` active state, sidebar
  active link), `AxeCoreAuditTests`
  (3 tests registered but skipped
  per ADR-016 / M4-D);
  (4) **T-017 theme toggle fix** —
  `@rendermode InteractiveServer` added
  to `AppThemeToggle.razor` (not to
  `AppLayout.razor`; the layout's
  `@Body` is a `RenderFragment` delegate
  that Blazor refuses to serialize
  across the SSR → interactive boundary;
  declaring the directive on the layout
  throws `InvalidOperationException`
  at request time; the directive on
  the toggle itself is the
  minimum-blast-radius fix);
  (5) **Project-continuity state +
  implementation report + per-session
  handoff.** 18 new component tests
  (5 `EmptyRoutesTests` + 4
  `AppLayout_ThemeToggleWiringTests` +
  4 `AppLayout_ResponsiveMatrixTests` +
  5 `AriaCurrentInvariantTests`) + 3
  new architecture tests
  (`AxeCoreAuditTests`, all skipped).
  Total test count is now 197
  passing + 7 skipped, 0 failed
  (6 unit + 185 bUnit + 6 active
  architecture + 7
  registered-but-disabled). The
  visual smoke test confirms every
  route returns 200 and the theme
  toggle's markup is present on every
  page. The theme toggle's click
  handler is now wired: clicking the
  toggle in the running app changes
  the document theme immediately and
  persists across navigation and
  browser refresh (via the IIFE in
  `App.razor` that reads
  `localStorage["app-theme"]`).
- **Report:**
  `implementation-report-m2-5-empty-routes-responsive-accessibility.md`.
- **Handoff:**
  `.ai/handoffs/2026-07-11-m2-5-empty-routes-responsive-accessibility.md`
  (mirrored at
  `.ai/handoffs/latest.md`).
- **Git:** branch
  `feature/m2-5-empty-routes-responsive-accessibility`;
  closeout commit
  `feat(m2.5): add empty routes, responsive matrix, a11y audit, and theme toggle fix`.
  No remote configured; push skipped (per
  the brief).
- **Next action:** the M2.5 closeout
  promotes M2.6 to the next session
  (M2.6 plan promotion + closeout
  template + Treehouse dogfooding
  checkpoint). The M2.5 session does
  **not** implement M2.6 (per the
  Progressive Coding Rule).

### M2.4 closeout session — 2026-07-11

- **Task ID:** `M2.4`
- **Milestone:** M2 — Application Shell and
  Navigation
- **Title:** Project intelligence
  dashboard
- **Status:** **Done (closed 2026-07-11).**
- **Outcome:** 3 new types in
  `src/AiEng.Platform.Application/ProjectIntelligence/`
  (`IProjectIntelligenceReader`,
  `ProjectIntelligenceSnapshot`,
  `ProjectIntelligenceReader`); 1 new
  composition-root extension in
  `src/AiEng.Platform.App/Composition/`
  (`AddProjectIntelligence`); 1 new
  page at
  `src/AiEng.Platform.App/Components/Pages/Dashboard.razor`
  (the M0.5-data sections in
  **Populated** state; the M3+-data
  sections in **Empty** state); the
  theme toggle bug is fixed (added
  `appTheme.current` JS function;
  component reads the resolved theme
  in `OnAfterRenderAsync(firstRender)`;
  click handler updates `IsDark`
  synchronously before the JSInterop
  call; `JSDisconnectedException` is
  handled); 6 new unit tests for
  `ProjectIntelligenceReader`; 13 new
  bUnit tests (3 composition + 9
  dashboard + 4 theme toggle); 2 new
  architecture tests
  (`Dashboard_Resolves_State_Through_Reader`
  and
  `No_Page_Reaches_State_Directly`).
  Total test count is now 175
  passing + 4 skipped, 0 failed
  (6 unit + 163 bUnit + 6 active
  architecture). The dashboard
  consumes state only through
  `IProjectIntelligenceReader`
  (architecture test enforces this).
- **Report:**
  `implementation-report-m2-4-project-intelligence-dashboard.md`.
- **Handoff:**
  `.ai/handoffs/2026-07-11-m2-4-project-intelligence-dashboard.md`
  (mirrored at
  `.ai/handoffs/latest.md`).
- **Git:** branch
  `feature/m2-4-project-intelligence-dashboard`;
  closeout commit
  `feat(m2.4): add project intelligence dashboard`.
  No remote configured; push skipped (per
  the brief).
- **Next action:** the M2.4 closeout
  promotes T-015 (M2.5) to `Ready`
  and promotes the M2.5 plan stub
  to a full plan in `Awaiting
  Approval`; the next session
  approves the M2.5 plan and starts
  M2.5 implementation. The M2.4
  session does **not** implement
  M2.5 (per the Progressive Coding
  Rule).

### M2.3 closeout session — 2026-07-11

- **Task ID:** `M2.3`
- **Milestone:** M2 — Application Shell and
  Navigation
- **Title:** Top bar, breadcrumbs, and
  page header integration
- **Status:** **Done (closed 2026-07-11).**
- **Outcome:** 4 new components in
  `src/AiEng.Platform.App/Components/`:
  `AppTopBar` (replaces the M2.1
  `AppTopBarSlot` placeholder), the
  relocated `AppThemeToggle`
  (light/dark theme toggle; persists
  to `localStorage`; reads
  `data-theme` on `documentElement`),
  `AppUserAvatarSlot` (the user
  avatar placeholder; M3+ replaces
  it with the real user identity
  surface), and `AppBreadcrumb` (walks
  the M2.2 registry's `Parent` chain;
  `aria-current="page"` on the current
  item; separators are `aria-hidden`).
  `AppBreadcrumb` wired into
  `AppPageHeader.Breadcrumbs` on
  `DesignSystem.razor`. `AppTopBarSlot`
  and `AppTopBarSlotTests.cs` deleted;
  `AppLayout` updated to use
  `AppTopBar`. 27 new bUnit tests
  across 4 test files
  (`AppTopBarTests`,
  `AppThemeToggleTests`,
  `AppUserAvatarSlotTests`,
  `AppBreadcrumbTests`); 6 obsolete
  `AppTopBarSlotTests` removed. Total
  test count is now 150 passing + 4
  skipped, 0 failed (77 M1.2 + 25
  M2.1 + 23 M2.2 bUnit + 27 M2.3
  bUnit − 6 obsolete removed + 4
  active architecture). Two
  documented deviations: (1)
  `AppTopBar` uses `div.app-topbar`
  + `Leading` / `Trailing` slots
  rather than `AppStack` +
  `AppPageHeader`; the surface still
  composes `AppTopBar` +
  `AppPageHeader` + `AppBreadcrumb`,
  matching the plan's intent. (2)
  Optional architecture test
  `Breadcrumb_Follows_Registry_Parent_Chain`
  was skipped per plan § 8 step 11
  which marked it optional.
- **Report:**
  `implementation-report-m2-3-topbar-breadcrumbs.md`.
- **Handoff:**
  `.ai/handoffs/2026-07-11-m2-3-topbar-breadcrumbs.md`
  (mirrored at
  `.ai/handoffs/latest.md`).
- **Git:** branch
  `feature/m2-3-topbar-breadcrumbs`;
  closeout commit
  `feat(m2.3): add top bar, breadcrumb, and page header integration`.
  No remote configured; push skipped (per
  the brief).
- **Next action:** the M2.3 closeout
  promotes T-014 (M2.4) to `Ready`
  and promotes the M2.4 plan stub
  to a full plan in `Awaiting
  Approval`; the next session
  approves the M2.4 plan and starts
  M2.4 implementation. The M2.3
  session does **not** implement
  M2.4 (per the Progressive Coding
  Rule).

### M2.2 closeout session — 2026-07-11

- **Task ID:** `M2.2`
- **Milestone:** M2 — Application Shell and
  Navigation
- **Title:** Navigation registry and
  sidebar
- **Status:** **Done (closed 2026-07-11).**
- **Outcome:** 4 new types in
  `src/AiEng.Platform.Application/Navigation/`
  (`INavigationRegistry`,
  `RouteMetadata`,
  `RouteMetadataAttribute`,
  `RouteRegistry`); 2 new extension
  methods in
  `src/AiEng.Platform.App/Composition/`
  (`AddPlatformServices` +
  `AddNavigation`); 3 new components in
  `src/AiEng.Platform.App/Components/Navigation/`
  (`AppSidebar`, `AppSidebarItem`,
  `AppNavItem`); `[RouteMetadata]`
  applied to all 6 pages; the
  composition root wired in
  `Program.cs`; the
  `Pages_AreReachable_Through_Registry`
  architecture test is active and green;
  28 new bUnit / integration tests +
  1 new architecture test; the M2.1
  `AppSidebarSlot` placeholder is
  replaced by the registry-driven
  `AppSidebar`; the 19 M1.2 components,
  77 bUnit tests, and 3 active + 4
  registered-but-disabled architecture
  tests are preserved. Total test count
  is now 129 passing + 4 skipped, 0
  failed (77 M1.2 + 25 M2.1 + 23 M2.2
  bUnit + 4 active architecture; 4
  registered-but-disabled
  architecture).
- **Report:**
  `implementation-report-m2-2-navigation-registry-sidebar.md`.
- **Handoff:**
  `.ai/handoffs/2026-07-11-m2-2-navigation-registry-sidebar.md`
  (mirrored at
  `.ai/handoffs/latest.md`).
- **Git:** branch
  `feature/m2-2-navigation-registry-sidebar`;
  closeout commit
  `feat(m2.2): add navigation registry and sidebar`.
  No remote configured; push skipped (per
  the brief).
- **Next action:** the M2.2 closeout
  promotes T-003 (M2.3) to `Ready` and
  promotes the M2.3 plan stub to a
  full plan in `Awaiting Approval`;
  the next session approves the M2.3
  plan and starts M2.3
  implementation. The M2.2 session
  does **not** implement M2.3 (per
  the Progressive Coding Rule).

### M2.1 closeout session — 2026-07-11

- **Task ID:** `M2.1`
- **Milestone:** M2 — Application Shell and
  Navigation
- **Title:** Application shell foundation
- **Status:** **Done (closed 2026-07-11).**
- **Outcome:** 5 new Blazor components /
  layouts in the application shell
  foundation (2 layouts: `AppLayout`,
  `AppEmptyLayout`; 2 placeholder shell
  components: `AppSidebarSlot`,
  `AppTopBarSlot`; 1 presentational helper:
  `AppShellRegion`); 25 new bUnit
  component tests across 4 test files
  (8 + 6 + 5 + 6); the M1.1 chrome
  (`MainLayout.razor`, `MainLayout.razor.css`,
  `NavMenu.razor`, `NavMenu.razor.css`) is
  removed; the Tailwind content path
  includes the new `Layouts/` directory;
  the `Layouts/_Imports.razor` resolves
  cross-folder type references; the five
  M1 template pages and `/design-system`
  reach the new layout root in place; the
  19 M1.2 components, 77 bUnit tests, and
  3 active + 4 registered-but-disabled
  architecture tests are preserved.
  Total test count is now 105 passing +
  4 skipped, 0 failed (77 M1.2 + 25
  M2.1 + 3 active architecture; 4
  registered-but-disabled architecture).
- **Report:**
  `implementation-report-m2-1-application-shell-foundation.md`.
- **Handoff:**
  `.ai/handoffs/2026-07-11-m2-1-application-shell-foundation.md`
  (mirrored at `.ai/handoffs/latest.md`).
- **Git:** branch
  `feature/m2-1-application-shell`;
  closeout commit
  `feat(m2.1): add application shell foundation`.
  No remote configured; push skipped (per
  the brief).
- **Next action:** the M2.1 closeout
  promotes T-002 (M2.2) to `Ready` and
  expands the M2.2 plan stub to a full
  plan in `Awaiting Approval`; the next
  session approves the M2.2 plan and
  starts M2.2 implementation. The
  M2.1 session does **not** implement
  M2.2 (per the Progressive Coding
  Rule).

### M1 closeout session — 2026-07-10

- **Task ID:** `M1-CLOSEOUT`
- **Milestone:** M1 — Design System Core
- **Title:** Close M1, introduce project
  continuity, prepare M2.1 plan
- **Outcome:** 19 reusable Blazor components
  (Primitives 7, Layout 4, Display 2, Feedback
  5, Inputs 1), 77 bUnit component tests, 3
  active architecture tests + 4
  registered-but-disabled composition-root
  tests, the `/design-system` documentation
  page, the Tailwind v3 + PostCSS pipeline,
  the design-token catalogue, light + dark
  themes. All seven ROADMAP M1 DoD items
  satisfied. The project-continuity system
  (Rule 15 in `AGENTS.md`) is in place.
- **Reports:**
  `implementation-report-m1-bootstrap.md`,
  `implementation-report-m1-1-frontend-foundation.md`,
  `implementation-report-m1-2-design-system-core.md`,
  `implementation-report-m1-closeout.md`.
- **Handoff:**
  `.ai/handoffs/2026-07-10-m1-closeout.md`.
- **Git:** first two commits
  `1722bd235830cfd8b180191953116c058c92edef`
  and
  `2ba1fad3cc45bee513ba38c7269e024bf8667ef9`
  on `main`. 166 files committed. Working
  tree clean. No remote configured.

### M1 follow-up — Re-anchor composition-root tests in `ROADMAP.md` matrix

- **Task ID:** `M1-FU-2`
- **Milestone:** M1 — Design System Core
- **Title:** Re-anchor composition-root
  architecture tests in the
  `ROADMAP.md` § 4 matrix
- **Outcome:** `ROADMAP.md` § 4
  ("Progressive self-dogfooding matrix") was
  updated during the M1 closeout session to
  list the four composition-root tests as
  `Delivered in M1 closeout — Active in M4-D`.
  See the M1 closeout report's
  "Files Modified" entry for `ROADMAP.md`.

---

## Deferred

For later milestones, a single summary task is
kept here so the work is not forgotten but the
task board does not become a speculative
backlog. Each summary task is fleshed out into
detailed tasks when the milestone approaches.

### M3 — Project Registration (summary) — Done (archived 2026-07-11)

- **Milestone:** M3 — Project Registration
  (Done 2026-07-11; closed 2026-07-11).
- **Status:** Done (M3.1 Delivered
  2026-07-11; M3.2 Delivered 2026-07-11;
  M3 closeout M3.x Delivered 2026-07-11).
  M3 is moved to `Done` with
  `closed_at: 2026-07-11`; the `m3`
  annotated milestone tag is at the M3
  closeout commit on `main`. The M3
  retrospective is at
  `retrospective-m3-project-registration.md`.
  This summary entry is archived
  (M3 is closed; the summary is no
  longer in `Deferred`).

### M4-A — Infrastructure / Process Execution (summary) — Active

- **Milestone:** M4-A (Active
  2026-07-11; M4-A.1 Delivered
  2026-07-11; the M4-A plan is at
  `.ai/plans/M4-A-infrastructure-process-execution.md`
  Status: Approved 2026-07-11 via
  the 'Next' invocation per
  `.ai/commands.md` § 4).
- **Status:** Active (M4-A.1
  delivered; the first concrete
  `IProcessRunner` activation is
  M4-A.2 — the Open action on
  `AppProjectCard`; M4-A.2 is T-022
  `Ready`).
- **Slices delivered:** M4-A.1
  (T-021; Delivered 2026-07-11;
  the infrastructure seam: 4
  contracts + 4 implementations +
  1 composition root + 1 on-disk
  store + 45 unit tests + 2
  architecture tests
  registered-but-disabled per
  ADR-016).
- **Slices remaining:** M4-A.2
  (T-022; Ready; Open action +
  `IProcessRunner.RunAsync`
  activation); M4-A.3 and
  onwards (TBD; the M4-A.3+ slices
  are reserved for any further
  infrastructure-level additions
  the M4-B / M4-C / M4-D work
  requires; the M4-A plan does not
  enumerate M4-A.3+ explicitly).
- **Next action:** the next
  session approves the M4-A.2
  implementation per the
  Progressive Coding Rule.

### M4-B — Capability Detection (summary)

- **Milestone:** M4-B — Capability
  Detection (Done 2026-07-13; the
  M4-B plan is Approved at
  `.ai/plans/M4-B-capability-detection.md`;
  the M4-B closeout plan is at
  `.ai/plans/M4-B-closeout.md`; the
  M4-B retrospective is at
  `retrospective-m4-b-capability-detection.md`;
  the `m4-b` annotated milestone tag
  is at the M4-B closeout commit on
  `main` per the branching strategy
  rule 9).
- **First action (later):** the M4-B
  plan was drafted (T-023, Done
  2026-07-13); the M4-B
  implementation was delivered in
  M4-B.1 + M4-B.2 + M4-B.3 (T-024 +
  T-025 + T-026, all Done
  2026-07-13); the M4-B closeout was
  delivered in T-027 (Done
  2026-07-13). M4-B is closed.
  The next milestone is M4-C
  (Provider Registry Foundation) in
  `Awaiting Approval` at
  `.ai/plans/M4-C-provider-registry-foundation.md`;
  the next concrete step on the
  user's `Approve` or `Next`
  invocation is the M4-C.1 first
  session (T-028, `Ready`; the
  `IProviderRegistry` contract + the
  6 family registries + the
  `SystemProviderRegistry`
  implementation + the 6 family
  fakes + the `AddProviderRegistry`
  composition root + 9+ unit
  tests).

### M4-C — Provider Registry Foundation (summary)

- **Milestone:** M4-C — Provider
  Registry Foundation (Active
  2026-07-13; the M4-C plan is in
  Active at
  `.ai/plans/M4-C-provider-registry-foundation.md`;
  Status: Active; the M4-C plan was
  approved at M4-B closeout 2026-07-13;
  the M4-C.1 first session (T-028)
  was delivered 2026-07-13 and
  transitioned M4-C from `Awaiting
  Approval` to `Active`; the M4-C.2
  first session (T-029) was delivered
  2026-07-13 and shipped the surface
  slice).
- **First action:** the M4-C.1
  first session (T-028, Done
  2026-07-13) shipped the boundary
  slice: the `IProviderRegistry`
  contract + the 6 family registry
  contracts + the
  `SystemProviderRegistry`
  implementation + the 6 no-op
  family stubs + the
  `AddProviderRegistry` composition
  root + the 6 family fakes + 19
  unit tests.
- **Slices delivered:** M4-C.1
  (T-028; Delivered 2026-07-13; the
  boundary slice); M4-C.2 (T-029;
  Delivered 2026-07-13; the surface
  slice: `AppProviderList` +
  `/providers` page + startup log +
  architecture test + `docs/providers.md`
  + `docs/design-system.md` § 4.5
  update + 13 bUnit component + 5
  bUnit page tests).
- **Slices remaining:** M4-C.x
  (T-030; the M4-C closeout: M4-C
  retrospective + M4-C status
  `Active` → `Done` + `m4-c`
  annotated milestone tag + the M4-D
  plan + project-continuity state).
- **Next action:** the next session
  approves the M4-C closeout
  implementation per the Progressive
  Coding Rule.

### M4-D — First Concrete Process Providers (summary)

- **Milestone:** M4-D.
- **First action (later):** draft
  `.ai/plans/M4-D-first-concrete-process-providers.md`.

### M5 — Native Git Worktrees (summary)

- **Milestone:** M5.
- **First action (later):** draft
  `.ai/plans/M5-native-git-worktrees.md`.

### M6 — Agent Runtime Launching (summary)

- **Milestone:** M6.
- **First action (later):** draft
  `.ai/plans/M6-agent-runtime-launching.md`.

### M7 — Review and Quality Gates (summary)

- **Milestone:** M7.
- **First action (later):** draft
  `.ai/plans/M7-review-and-quality-gates.md`.

### M8 — Autonomous Loops, Orchestration, Production Hardening (summary)

- **Milestone:** M8.
- **First action (later):** draft
  `.ai/plans/M8-autonomous-loops-orchestration.md`.

### M-Router — AI Session Router (operating layer, one-time)

- **T-031** (Done, 2026-07-14) — Ship the
  operating-layer AI session router (PowerShell 5.1+
  supervisor at `tools/ai-session-router.ps1`; five
  profiles in `.ai/model-routing.json`; per-phase
  prompts in `.ai/prompts/phases/`; phase receipts in
  `.ai/receipts/phases/`; ADR-017 in `DECISIONS.md`).
  The future in-platform Blazor `IAiSessionRouter`
  (consumed through `IAiSessionRouter`,
  `IModelRoutingPolicy`, `IAgentSessionLauncher`,
  `ModelRoutingConfiguration`,
  `TaskExecutionPipeline`) is **Deferred**; it lands
  in a future milestone when the platform is ready.
  See `.ai/backlog/ai-session-router.md`. The
  operating-layer PowerShell supervisor is the
  bridge that exists today.

## Tool-First Execution Rule

The product priority (recorded 2026-07-19) is:

> Prove real upstream tools work end-to-end, one at a
> time, then grow and refine the platform UI around
> proven integrations.

Every external-tool onboarding task follows the
10-step rule:

1. inspect upstream tool,
2. pin the inspected upstream commit (the locked
   baseline in `.ai/upstreams/upstream-lock.json`),
3. prove its native intended usage,
4. determine Windows, WSL, or wrapper execution mode,
5. implement one provider or adapter,
6. expose health/version/status,
7. run one safe end-to-end operation,
8. add tests,
9. add only minimal UI required to observe it,
10. close the task and select the next tool.

### Three verification levels (recorded 2026-07-19)

A provider is not "working" merely because its parser
works against mocked output. It is working only after
the platform invokes the real upstream executable and
consumes its actual result. Every tool-first task
tracks three verification levels:

- **Implemented** — contracts, wrapper code, and
  unit / deterministic tests exist; the platform
  can compose the provider; nothing has been
  executed against the real upstream yet.
- **Executable verified** — the actual installed
  upstream executable (resolved by PATH, npm-
  global, pnpm-global, or configured absolute
  path) responds successfully to `--version`
  and/or `--help`; the truthful provider data
  (`actual_executable_verified`,
  `executable_path`, `health_check_state`,
  `health_check_timestamp`,
  `health_check_duration_ms`, `version`) is
  populated by the real process invocation.
- **Workflow verified** — a bounded real
  operation succeeds end-to-end in an isolated
  environment (temp git repo; one iteration;
  strict timeout; no remote; no push; no
  credentials; no destructive objective).

A task may move from `Implemented` to
`PartiallyVerified` once the **Executable
verified** level is reached. A task may move
from `PartiallyVerified` to `Done` once
**Workflow verified** is reached. A task that
is **Implemented** but blocked on user
authorisation for the install is held at
`PartiallyVerified` with the install command
and the blocker recorded in
`.ai/state/tasks.json` `blockers`.

The `GnhfRealToolSmokeTests` class is the
executable-verified-tier proof mechanism: a
`[SkippableFact]` that locates the actual gnhf
executable at test time and SKIPs when not
installed rather than failing.

## Done Recently

### T-032 — Tool-first recovery and gnhf provider vertical slice (M4-D.1) — PartiallyVerified

- **Task ID:** T-032.
- **Milestone:** M4-D (precedes the umbrella M4-D
  plan; the M4-D plan is not yet drafted).
- **Title:** tool-first recovery + gnhf provider
  vertical slice — the first real external-tool
  end-to-end proof.
- **Owner:** `tool-first-gnhf-real-verification`
  session.
- **Implementation report:**
  `implementation-report-tool-first-recovery-and-gnhf-vertical-slice.md`.
- **Branch:** `feature/T-032-real-gnhf-verification`
  (fast-forwarded into `main`; deleted per rule 7).
- **Commit:** `fix(gnhf): verify actual upstream
  executable and correct integration evidence`.
- **Verification levels (3-tier policy):**
  - **Implemented:** Done — the
    `GnhfExecutableResolver` +
    `GnhfProcessProbeRunner` +
    `GnhfAutonomousLoopFamily` +
    `GnhfServiceCollectionExtensions` ship
    clean; the family is registered with
    `TryAddSingleton` so the M4-C.1 stub
    remains the fallback when this package
    is absent; 6 health states
    (`InstalledAndHealthy`,
    `InstalledButUnhealthy`, `NotInstalled`,
    `TimedOut`, `Cancelled`, `VersionUnknown`)
    are distinguished; the descriptor's
    metadata exposes
    `actual_executable_verified`,
    `bounded_workflow_verified`,
    `health_check_state`,
    `health_check_timestamp`,
    `health_check_duration_ms`,
    `executable_path`, `executable_resolution`,
    `failure_reason`, `exit_code`, `help`,
    `entry_command`, `locked_commit`,
    `execution_mode`.
  - **Executable verified:** Pending. The
    optional real-tool smoke test
    (`GnhfRealToolSmokeTests.RealGnhf_probe_succeeds_when_executable_is_installed`)
    currently **SKIPs** because gnhf is not
    installed on this host. The host's PATH
    has no `gnhf` / `gnhf.cmd`; the npm
    global directory contains no `gnhf`;
    the locked upstream clone at
    `code-kunchenguid/gnhf` is unmodified
    (HEAD = `fe202c4c` per the lock). The
    documented install command — `npm
    install -g gnhf` (per the gnhf README)
    — is recorded as the blocker in
    `.ai/state/tasks.json` and is not
    performed in this session per the user's
    binding "Do not silently install
    anything."
  - **Workflow verified:** NotAttempted.
    Will run only after Executable verified
    is reached, in an isolated temp git repo
    (one iteration; strict timeout; no
    remote; no push; no credentials; no
    destructive objective), per the brief § 5
    and the three-verification-level policy
    above.
- **Truthful provider data surfaced on
  `/providers`:** no UI redesign — the
  existing `AppProviderList` renders the
  metadata dictionary through
  `AppKeyValueList`, so the truthful fields
  (including the `NotInstalled` state with
  `actual_executable_verified = false`,
  `executable_resolution = not-found`,
  `executable_path = (none)`,
  `health_check_state = NotInstalled`,
  `failure_reason = not found on PATH,
  npm-global, or pnpm-global; tried:
  gnhf.cmd, gnhf.exe, gnhf`) are
  observable to the user.
- **Tests:** 17 tests total in
  `tests/AiEng.Platform.Providers.Gnhf.Tests`
  (3 test classes):
  - 4 family tests
    (`ListProvidersAsync_returns_available_descriptor_when_probe_reports_healthy`,
    `…_returns_unavailable_descriptor_when_not_installed`,
    `…_throws_when_token_is_cancelled`,
    `…_invokes_probe_each_call`).
  - 8 process-probe-runner tests
    (`…_returns_not_installed_when_resolver_finds_nothing`,
    `…_parses_version_and_help_when_executable_returns_zero`,
    `…_uses_resolved_executable_path_from_resolver`,
    `…_returns_unhealthy_when_version_exit_code_is_non_zero`,
    `…_returns_version_unknown_when_no_version_regex_match`,
    `…_returns_timed_out_when_process_hangs`,
    `…_returns_cancelled_when_caller_cancels`,
    `…_records_resolution_and_health_timestamp`).
  - 4 resolver tests
    (`Resolve_returns_configured_path_when_file_exists`,
    `…_returns_not_installed_when_configured_path_missing`,
    `…_windows_candidate_names_includes_cmd_and_exe`,
    `…_non_windows_candidate_names_is_only_gnhf`).
  - 1 optional real-tool smoke
    (`RealGnhf_probe_succeeds_when_executable_is_installed`),
    `[SkippableFact]`, currently SKIPs.
  - All 16 deterministic tests pass; the 1
    real-tool test skips. Regression gate:
    118 existing unit tests still pass
    (134 unit total).
- **Files added:** 3
  (`tests/AiEng.Platform.Providers.Gnhf.Tests/GnhfExecutableResolverTests.cs`,
  `tests/AiEng.Platform.Providers.Gnhf.Tests/GnhfRealToolSmokeTests.cs`,
  `src/AiEng.Platform.Providers.Gnhf/GnhfExecutableResolver.cs`).
- **Files modified:** 4 in the Gnhf package
  (`GnhfProbe.cs`, `GnhfProcessProbeRunner.cs`,
  `GnhfAutonomousLoopFamily.cs`,
  `GnhfServiceCollectionExtensions.cs`) +
  4 test files (`Fakes.cs`,
  `GnhfAutonomousLoopFamilyTests.cs`,
  `AiEng.Platform.Providers.Gnhf.Tests.csproj`)
  + 4 state files (`.ai/state/tasks.json`,
  `.ai/state/task-board.md`,
  `.ai/handoffs/latest.md`,
  `.ai/handoffs/2026-07-19-tool-first-recovery-and-gnhf-vertical-slice.md`)
  + 1 report
  (`implementation-report-tool-first-recovery-and-gnhf-vertical-slice.md`).
  The legacy `IGnhfProbeRunner.cs` file is
  removed; the interface is now co-located
  with its implementation in
  `GnhfProcessProbeRunner.cs` to keep the
  public surface of the package in one place.
- **Status:** **PartiallyVerified (2026-07-19).
  Executable verified is blocked on the user's
  authorisation of the documented `npm install
  -g gnhf` install. The task is intentionally
  not `Done` until Workflow verified is also
  reached.**

## Ready

### T-033 — No-mistakes Quality Gate Provider vertical slice (M4-D.2 placeholder) — Blocked

- **Task ID:** T-033.
- **Milestone:** M4-D.
- **Title:** no-mistakes provider vertical slice —
  the second tool-first end-to-end proof; a local
  git proxy that runs an AI-driven validation
  pipeline before pushing to the configured remote
  and opening a PR.
- **Why it matters:** the gnhf slice proves the
  AutonomousLoop family. The no-mistakes slice
  proves the QualityGate family (C-006) — the
  same M4-D pattern, one family at a time. The
  QualityGate family is the upstream that turns
  M4-D from "find a tool" into "ship the
  pipeline the platform runs against itself".
- **Locked commit:** `3752c1a0fb7b76ff40f83143eea799fbd6e7d5b0`
  (per `.ai/upstreams/upstream-lock.json`).
- **Mapped family:** `QualityGateProviderFamily`
  (C-006).
- **Objective:** implement
  `INoMistakesProbeRunner`,
  `QualityGateProviderFamily`, deterministic
  tests, optional real-tool smoke, and minimal
  UI on `/providers`; the smoke test exercises
  the local git-proxy path against the
  installed no-mistakes binary (skips when not
  installed).
- **Acceptance criteria:** (not yet defined;
  T-033 plan lands when T-032 reaches
  Executable verified).
- **Dependencies:** T-032 (PartiallyVerified
  2026-07-19; Executable verified still
  pending on the documented `npm install -g
  gnhf` install).
- **Status:** **Blocked.** T-033 is blocked on
  T-032 reaching Executable verified. The
  user can re-prioritise on the next `Next`
  invocation (e.g. pivot to T-030 — M4-C
  closeout — to draft the M4-D umbrella plan
  in a single session, or directly authorise
  the gnhf install to unblock both T-032 and
  T-033).
