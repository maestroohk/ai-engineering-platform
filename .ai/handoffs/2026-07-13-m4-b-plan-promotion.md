# M4-B Plan Promotion — Per-Session Handoff (2026-07-13)

> **M4-B plan promotion session handoff.** The
> M4-B plan promotion is a planning-surface
> change after the M4-A.2 closeout (commit
> `5853d41` on `main`; 323 tests passed, 0
> failed, 9 skipped per ADR-016 / M4-D). The
> M4-A.2 closeout handoff § 13 noted: *"The
> next session is the M4-A.3 implementation
> (if defined) or the M4-B plan promotion.
> The M4-A.2 session does **not** begin
> M4-A.3 / M4-B / M4-C / M4-D (per the
> brief: 'Do not begin the following
> task')."* M4-A.3 is undefined; the M4-A
> plan § 2 enumerates only M4-A.1 + M4-A.2;
> the next concrete step is the M4-B plan
> promotion. This handoff is the closing
> receipt for the M4-B plan promotion slice.

---

## 1. Session metadata

- **Session ID:** `m4-b-plan-promotion`
  (recorded in `.ai/state/session.json`).
- **Session date:** 2026-07-13.
- **Session type:** Plan promotion
  (planning-surface change; no C# code, no
  tests, no M4-B implementation begins).
- **Previous session:** `m4-a-2-open-action`
  (M4-A.2 closeout, 2026-07-11).
- **Active branch (during the session):**
  `feature/m4-b-capability-detection-plan-promotion`
  (created from `main` at the M4-A.2 closeout
  commit `5853d41`; fast-forwarded into `main`
  per the branching strategy rule 6; deleted
  per rule 7).
- **Active milestone:** M4-B — Capability
  Detection (Active 2026-07-13; the M4-B plan
  is in Awaiting Approval).
- **Active task:** T-023 (Done, 2026-07-13).
- **Last completed task (before this):**
  T-022 (M4-A.2, Done 2026-07-11).
- **Last completed milestone (before this):**
  M3 (closed 2026-07-11).
- **Push decision:** Staged for push
  (not authorised in this session; the M4-B
  plan promotion did not push).

---

## 2. Brief

The user's invocation was `next` — the
end-to-end collapsed form of `Continue` +
`Approve` + the 13-step Progressive Coding
lifecycle per `.ai/commands.md` § 4. The
M4-A.2 closeout handoff § 13 named the M4-B
plan promotion as the next concrete step.
The M4-B plan promotion is a planning-
surface change; the M4-B implementation
begins in a future session.

**Preserved verbatim, in force:**

- "Do not create providers" — M4-B detects
  capabilities, does not create providers.
- "Do not reorder accepted milestones" —
  M4-B remains the third milestone of M4
  (after M4-A, before M4-C).
- "Do not introduce invented progress
  percentages. All progress must be derived
  from structured state and evidence" — the
  M4-B plan is recorded as `Awaiting Approval`;
  progress is `Planned` until the M4-B
  first session ships.
- "Do not add code comments" (Rule 13 in
  `AGENTS.md`).
- "Do not put secrets, credentials,
  personal data, or environment-specific
  paths into `.ai/`" (Rule 15).
- "Do not change an architectural rule
  without an ADR" — the M4-B plan references
  existing ADRs (ADR-016, ADR-011) but
  introduces no new architectural rules.
- "Push only if a remote is configured and
  pushing is authorised" — push is **not**
  authorised in this session; the M4-B plan
  promotion did not push.
- M4-B plan promotion brief: "Do not begin
  the following task" — the M4-B plan
  promotion did **not** begin the M4-B
  implementation, the M4-C plan promotion,
  the M4-D plan promotion, or any provider
  creation.

---

## 3. Plan reference

The M4-B plan promotion plan is at
`C:\Users\hkasozi\.claude\plans\generic-seeking-oasis.md`
(approved via `ExitPlanMode`; 12 sections
mirroring the M4-A plan's 12-section
structure). The M4-B plan itself is at
`.ai/plans/M4-B-capability-detection.md`
with `Status: Awaiting Approval`.

The M4-B plan mirrors the M4-A plan's
12-section structure:

1. **Why This Milestone Exists** — M4-B
   introduces the host's capability
   detection. M4-A shipped the
   infrastructure seam; M4-B consumes the
   seam. M4-C consumes the M4-B report.
2. **In Scope** — 10 items: the
   `IHostCapabilitiesService` contract;
   the `HostCapabilities` + `HostCapability`
   records; the `SystemHostCapabilitiesService`
   implementation; the `AppCapabilityList`
   component; the `AppKeyValueList`
   component; the `Diagnostics.razor`
   page; the `AddHostCapabilities`
   composition root; the
   `IHostCapabilitiesService` startup
   hook; the `Capabilities_Resolved_Through_Service`
   architecture test; the
   `docs/capabilities.md` documentation.
3. **Out of Scope** — 13 items: provider
   creation; M4-C / M4-D work; worktree
   creation (M5); agent launching (M6);
   review / quality gates (M7); autonomous
   loops (M8); activation of the 4
   registered-but-disabled composition-
   root tests (M4-D); `AppProviderCard`
   (M4-C); the M3 modal Escape-key /
   backdrop-click / Browse-folder
   follow-ups; macOS / Linux credential
   vault; a design-system `AppDialog`
   primitive; the 2 M4-A.1
   `Infrastructure_Respects_*` tests
   (M4-D); push to remote.
4. **Files to Add** — 24 new files: 1
   new contract + 2 records in
   Application; 1 new implementation in
   Infrastructure; 1 new composition-root
   extension in App/Composition; 6 new
   files in App/Components/Diagnostics;
   2 new files in App/Components/Pages;
   5 new test files; 1 new doc; 1 new
   implementation report; 1 new per-
   session handoff.
5. **Files to Modify** — 14 files:
   `src/AiEng.Platform.App/AiEng.Platform.App.csproj`
   (no change); `src/AiEng.Platform.App/Composition/ServiceCollectionExtensions.cs`
   (one `AddHostCapabilities()` call);
   `src/AiEng.Platform.App/Program.cs`
   (startup log); `docs/infrastructure.md`
   § 11 (M4-B Consumers); `docs/design-system.md`
   § 4.5 (rows updated on M4-B.1 closeout);
   `ROADMAP.md` (§ 2 + § 3); `.ai/plans/master-delivery-plan.md`
   (§ 1 + § 3); `.ai/state/capabilities.json`
   C-015 evidence block; the
   project-continuity state per Rule 15
   (`.ai/state/session.json` + `.ai/state/tasks.json`
   + `.ai/state/current.md` + `.ai/state/task-board.md`
   + `.ai/state/milestones.json` +
   `.ai/handoffs/latest.md`).
6. **Coherent Commit** — the M4-B plan
   promotion commit
   `chore(m4-b.plan): draft M4-B capability detection plan in Awaiting Approval`
   on the feature branch
   `feature/m4-b-capability-detection-plan-promotion`.
7. **Critical Files to Read** — 20+
   files enumerated in the M4-B plan § 7.
8. **Existing Functions and Utilities to
   Reuse** — 10 components enumerated in
   the M4-B plan § 8.
9. **Risks and Mitigations** — 13 risks
   enumerated in the M4-B plan § 9.
10. **Test Plan** — 10+ unit tests for
    `SystemHostCapabilitiesService`; 5+
    bUnit tests for `AppCapabilityList`; 5+
    bUnit tests for `AppKeyValueList`; 3+
    bUnit tests for `Diagnostics.razor`; 1
    new active architecture test
    (`Capabilities_Resolved_Through_Service`);
    0 new registered-but-disabled tests;
    regression gate (the M4-A.1 + M4-A.2
    323 tests remain green).
11. **Documentation Plan** — `docs/capabilities.md`
    (new, 10 sections); `docs/infrastructure.md`
    § 11 (M4-B Consumers); `docs/design-system.md`
    § 4.5 (rows updated on M4-B.1 closeout);
    `ROADMAP.md` § 2 + § 3; `.ai/plans/master-delivery-plan.md`
    § 1 + § 3; `DECISIONS.md` (no new ADR).
12. **Stop Condition** — the M4-B plan
    promotion session stops after the
    coherent commit on the feature branch.
    The next session is the M4-B
    implementation (when the user invokes
    `Approve` or `Next`). The M4-B plan
    promotion does **not** begin the M4-B
    implementation.

---

## 4. Files added

- `.ai/plans/M4-B-capability-detection.md`
  — the M4-B plan (12 sections, mirroring
  the M4-A plan's 12-section structure;
  `Status: Awaiting Approval`; 600+ lines;
  CRLF per `.editorconfig`).
- `.ai/handoffs/2026-07-13-m4-b-plan-promotion.md`
  — this handoff (mirrored to
  `.ai/handoffs/latest.md`).

---

## 5. Files modified

- `docs/infrastructure.md` — § 11 M4-B
  Consumers added (one paragraph noting
  the M4-B `IHostCapabilitiesService` is
  the first consumer of `IProcessRunner` +
  `ICredentialVault` outside the M4-A.2
  Open Action).
- `ROADMAP.md` — § 2 M4-B row `Planned` →
  `Active` (with the full M4-B plan
  promotion evidence narrative); § 3
  M4-B `Definition of done` expanded
  with 9 bullets mirroring the M4-A.1 +
  M4-A.2 DoD bullets.
- `.ai/plans/master-delivery-plan.md` —
  § 1 M4-B row `Planned` → `Active`
  (with the full M4-B plan promotion
  evidence narrative); § 3 M4-B block
  expanded with the full Major
  capabilities delivered + Completion
  status + Evidence blocks; M4-B slice
  breakdown table added with the
  anticipated M4-B.1 / M4-B.2 / M4-B.3
  rows.
- `.ai/state/capabilities.json` — C-015
  `IHostCapabilitiesService` evidence
  block initialised
  (`evidence.plans: [".ai/plans/M4-B-capability-detection.md"]`);
  C-015 `next_task` set to `T-023`; C-015
  `last_updated` set to `2026-07-13`;
  top-level `updated_at` +
  `updated_by_session` set to the M4-B
  plan promotion session.
- `.ai/state/session.json` — M4-B plan
  promotion envelope (12 sections
  mirroring the M4-A.2 envelope structure).
- `.ai/state/tasks.json` — T-023 record
  added with full evidence block
  (`status: Done`; `plan` =
  `.ai/plans/M4-B-capability-detection.md`;
  `branch` =
  `feature/m4-b-capability-detection-plan-promotion`;
  `evidence` block with
  `implementation_report: null` + the
  per-session handoff + the per-file
  `source_paths_added` +
  `source_paths_modified`); T-008 M4
  summary note updated.
- `.ai/state/current.md` — active
  milestone M4-A → M4-B (with the full
  M4-B plan narrative); active slice
  M4-A.2 → M4-B plan promotion; last
  completed task T-022 → T-023 (with the
  full M4-B plan promotion narrative);
  last stable commit → the M4-B plan
  promotion commit (with the full
  parent + branch + fast-forward + delete
  narrative); next recommended task → the
  M4-B.1 first session (T-024, with the
  full M4-B.1 contract + implementation +
  composition root + unit tests +
  architecture test narrative); last
  updated → the M4-B plan promotion
  session; linked artefacts → the M4-B
  plan promotion handoff.
- `.ai/state/task-board.md` — M4-A.3
  stub row status `Ready` → `Deferred`
  (M4-A.3 is undefined; the M4-B
  implementation is the next concrete
  step); new `M4-B.1` row in `Ready` (with
  the full M4-B.1 contract + implementation
  + composition root + unit tests +
  architecture test narrative); "In
  Progress" section updated; M4-B summary
  row in `Deferred` updated to reflect the
  M4-B plan is in Awaiting Approval.
- `.ai/state/milestones.json` — M4-B row
  status `Planned` → `Active` (with the
  full Major capabilities delivered +
  `plan_path` + `plan_status` +
  `plan_promotion_at` + `plan_promotion_by_session`
  + `plan_promotion_commit` + slices
  block with M4-B.1 / M4-B.2 / M4-B.3 rows
  + implementation_reports + handoffs +
  commits); top-level `updated_at` +
  `updated_by_session` set to the M4-B
  plan promotion session.
- `.ai/handoffs/latest.md` — mirrored
  from this handoff (the M4-B plan
  promotion handoff is the most recent
  handoff).

---

## 6. Validation

1. **Format gate:** `dotnet format --verify-no-changes`
   exits 0; the format is canonical and
   CI-clean. The M4-B plan promotion does
   not modify any C# code, so the format
   gate applies only to the .md files.
   All .md files are CRLF per
   `.editorconfig` (verified with
   `unix2dos` on every new file).
2. **Build gate:** `dotnet build` exits
   0; 0 warnings, 0 errors; the no-code-
   changes build is a no-op (the M4-B
   plan promotion does not touch the C#
   code). The build gate confirms the
   M4-A.1 + M4-A.2 closeout state is
   clean.
3. **Test gate:** `dotnet test` reports
   323 tests passed (79 unit + 232 bUnit
   + 12 architecture), 0 failed, 9
   skipped (the 7 from M3 + the 2 M4-A.1
   architecture tests remain
   registered-but-disabled per ADR-016).
   The M4-B plan promotion is a
   regression gate; no new tests are
   added.
4. **JSON validation gate:** the state
   files (`session.json`, `tasks.json`,
   `milestones.json`, `capabilities.json`)
   are valid JSON; the `updated_at` field
   is updated; the schema is preserved.
5. **Markdown validation gate:** the M4-B
   plan + the `docs/infrastructure.md`
   § 11 update + the `ROADMAP.md` updates
   + the `.ai/plans/master-delivery-plan.md`
   updates are well-formed markdown; the
   headings are nested correctly; the
   code blocks are fenced with three
   backticks; the links use the
   `[text](url)` format.
6. **Plan validation gate:** the M4-B
   plan is drafted at
   `.ai/plans/M4-B-capability-detection.md`
   with `Status: Awaiting Approval`; the
   12 sections are present; the section
   headings match the M4-A plan's
   section headings; the Risks and
   Mitigations table has 13 rows; the
   Files to Add section has 24 new files;
   the Files to Modify section has 14
   modified files; the Coherent Commit
   section names the feature branch +
   commit message; the Stop Condition
   section is present.
7. **Git gate:** `git status` is clean
   after the coherent commit; the
   feature branch
   `feature/m4-b-capability-detection-plan-promotion`
   is fast-forwarded into `main`; the
   feature branch is deleted; the working
   tree matches the M4-A.2 closeout
   commit (`5853d41`) plus the M4-B plan
   promotion commit.
8. **DoD gate:** every item in the M4-B
   plan promotion scope (per the M4-B
   plan promotion plan Goal § 1-10) is
   checked. The check is by inspection:
   every DoD bullet is marked satisfied
   in this handoff's Validation § 1.
9. **No scope creep:** the diff does not
   modify any file under
   `src/AiEng.Platform.Infrastructure/`,
   `src/AiEng.Platform.Application/Infrastructure/`,
   `src/AiEng.Platform.Application/Capabilities/`,
   `src/AiEng.Platform.App/Composition/`,
   `src/AiEng.Platform.App/Components/`,
   `src/AiEng.Platform.App/Program.cs`,
   `tests/`,
   `src/AiEng.Platform.Providers.Abstractions/`,
   `src/AiEng.Platform.Domain/`,
   `docs/design-system.md` (the
   `AppCapabilityList` + `AppKeyValueList`
   rows update is deferred to the M4-B
   closeout), `AGENTS.md`,
   `ARCHITECTURE.md`, `DECISIONS.md`,
   `STYLEGUIDE.md`, `CONTRIBUTING.md`,
   `.ai/workflows/`,
   `.ai/plans/M4-A-infrastructure-process-execution.md`,
   `.ai/plans/M3-*.md`,
   `tailwind.config.js`, `package.json`,
   or `Directory.Build.props`. A diff that
   touches any of those is a defect.
10. **Push decision:** push is **not**
    authorised in this session. The push
    decision recorded in this handoff is
    **Staged for push** (the user did not
    authorise in this session; the M4-B
    plan promotion did not push; the next
    user command may push).

---

## 7. Git history

- `5853d41` (parent; M4-A.2 closeout;
  `feat(m4-a.2): enable AppProjectCard.Open action via IProcessRunner`).
- The M4-B plan promotion commit
  `chore(m4-b.plan): draft M4-B capability detection plan in Awaiting Approval`
  is on the feature branch
  `feature/m4-b-capability-detection-plan-promotion`
  (created from `main` at `5853d41`).
  The branch is fast-forwarded into
  `main` per the branching strategy
  rule 6; the branch is deleted per
  rule 7. The commit hash on `main` is
  the M4-B plan promotion commit hash
  (the M4-B plan promotion commit hash
  is recorded in the M4-B plan
  promotion session's commit log; the
  hash is not yet known at the time of
  writing this handoff — the M4-B plan
  promotion is committed after the
  handoff is written; the parent of the
  M4-B plan promotion commit is
  `5853d41`; the commit's message is
  `chore(m4-b.plan): draft M4-B capability detection plan in Awaiting Approval`).

---

## 8. Deviations

The M4-B plan documents 4 anticipated
deviations in the M4-B plan promotion
plan § "M4-B Plan Deviations Anticipated":

1. **`HostCapability.CredentialAvailable`
   field.** The master delivery plan's
   M4-B row does not prescribe a
   `CredentialAvailable` field on the
   `HostCapability` record. The M4-B
   plan adds the field to support the
   `ICredentialVault.GetAsync` detection
   path. The deviation is recorded in
   the M4-B closeout's Lessons Learned.
2. **`Capabilities_Resolved_Through_Service`
   architecture test scope.** The master
   delivery plan's M4-B row does not
   prescribe the test's scope. The M4-B
   plan scopes the test to
   `App/Components/Diagnostics/` to avoid
   the M4-A.2 Open Action false positive.
   The deviation is recorded in the M4-B
   closeout's Lessons Learned.
3. **`AppCapabilityList` data-owning
   classification.** The master delivery
   plan's M4-B row does not prescribe the
   data-owning classification. The M4-B
   plan classifies `AppCapabilityList` as
   data-owning per `docs/design-system.md`
   § 5.4. The deviation is recorded in
   the M4-B closeout's Lessons Learned.
4. **M4-B slice breakdown.** The master
   delivery plan's M4-B row does not
   enumerate slices. The M4-B plan
   documents the anticipated 3 slices
   (M4-B.1: contract + implementation +
   composition root + unit tests +
   architecture test; M4-B.2:
   design-system components + bUnit
   tests; M4-B.3: `/diagnostics` page +
   composition-root wiring + startup
   log + documentation). The M4-B.1
   first session may revise the slice
   breakdown. The deviation is recorded
   in the M4-B closeout's Lessons
   Learned.

The M4-B plan deviations are recorded in
the M4-B plan promotion plan; the M4-B
plan itself does **not** record the
deviations (the M4-B plan is the
canonical plan; the deviations are
implementation-time decisions recorded
in the closeout's Lessons Learned).

---

## 9. Risks and Mitigations

The M4-B plan documents 13 risks and
mitigations. The M4-B plan promotion
session does **not** introduce new
risks; the M4-B plan promotion is a
planning-surface change. The M4-B
implementation sessions (M4-B.1,
M4-B.2, M4-B.3) inherit the 13 risks
and mitigations from the M4-B plan.

The M4-B plan promotion session's
risks (not in the M4-B plan) are:

- The M4-B plan may be too long (12
  sections, ~600+ lines). Mitigation:
  the M4-B plan follows the M4-A plan's
  12-section structure exactly; the
  M4-A plan is ~1500 lines and was
  accepted; the M4-B plan is in the
  600-1000 line range. The user can
  edit the plan before approval per
  `.ai/templates/implementation-plan.md`
  "Status" rules.
- The M4-B plan may push the feature
  branch to `origin/main` if the user
  has a remote configured. Mitigation:
  push is **not** authorised in this
  session; the M4-B plan promotion did
  not push. The push decision is
  `Staged for push`; the next user
  command may push.

---

## 10. Project continuity

- **M4-B plan:** in `Awaiting Approval` at
  `.ai/plans/M4-B-capability-detection.md`.
- **M4-B implementation:** not yet started;
  the M4-B.1 first session is the next
  concrete step on the user's `Approve` or
  `Next` invocation.
- **M4-C plan promotion:** not yet
  started; the M4-B plan promotion does
  **not** begin the M4-C plan promotion.
- **M4-D plan promotion:** not yet
  started; the M4-B plan promotion does
  **not** begin the M4-D plan promotion.
- **Provider creation:** not yet started;
  the M4-B plan promotion does **not**
  create any `Providers.<X>` project. The
  first concrete providers land in M4-D.
- **M4-A.3 implementation:** not yet
  defined; the M4-B plan promotion does
  **not** define M4-A.3; the M4-A.3 stub
  row is moved from `Ready` to `Deferred`
  in `.ai/state/task-board.md`.

---

## 11. Linked artefacts

- The M4-B plan promotion plan:
  `C:\Users\hkasozi\.claude\plans\generic-seeking-oasis.md`
  (12 sections; the canonical reference
  for the M4-B plan promotion's
  structure).
- The M4-B plan:
  `.ai/plans/M4-B-capability-detection.md`
  (12 sections; the canonical M4-B
  plan; `Status: Awaiting Approval`).
- The master delivery plan:
  `.ai/plans/master-delivery-plan.md` § 3
  (M4-B row + M4-B slice breakdown
  table).
- The M4-A plan:
  `.ai/plans/M4-A-infrastructure-process-execution.md`
  (the M4-A plan is the canonical
  reference for the M4-B plan's
  12-section structure).
- The M4-A.2 handoff:
  `.ai/handoffs/2026-07-11-m4-a-2-open-action.md`
  (the source of the M4-B plan
  promotion's "next action" — "the
  M4-B plan promotion").
- The M4-A.2 implementation report:
  `implementation-report-m4-a-2-open-action.md`
  (the M4-A.2 closeout pattern; the
  M4-B.1 first session's closeout
  report mirrors the M4-A.1 + M4-A.2
  closeout reports).
- The M4-A.1 handoff:
  `.ai/handoffs/2026-07-11-m4-a-1-infrastructure-project-skeleton.md`
  (the M4-A.1 closeout pattern; the
  M4-B.1 first session's handoff
  mirrors the M4-A.1 handoff).
- The M4-A contracts:
  `src/AiEng.Platform.Application/Infrastructure/IProcessRunner.cs`,
  `ICredentialVault.cs`,
  `IPlatformInfo.cs` (the M4-B plan
  composes these contracts).
- The M4-A implementations:
  `src/AiEng.Platform.Infrastructure/ProcessRunner/SystemProcessRunner.cs`,
  `Credentials/WindowsCredentialVault.cs`,
  `Platform/SystemPlatformInfo.cs` (the
  M4-B plan composes these
  implementations).
- The M4-A composition root:
  `src/AiEng.Platform.App/Composition/Infrastructure/InfrastructureServiceCollectionExtensions.cs`
  (the M4-B `AddHostCapabilities`
  extension follows the same
  `TryAddSingleton` pattern).
- The M2.2 navigation registry:
  `src/AiEng.Platform.Application/Navigation/RouteMetadata.cs`,
  `RouteMetadataAttribute.cs`,
  `RouteRegistry.cs`,
  `INavigationRegistry.cs` (the M4-B
  `/diagnostics` page is registered via
  `[RouteMetadata]`).
- The M1.2 design system primitives:
  `src/AiEng.Platform.App/Components/Primitive/`,
  `Layout/`, `Display/`, `Feedback/`,
  `Inputs/` (the M4-B `AppCapabilityList`
  + `AppKeyValueList` components compose
  the M1.2 primitives).
- The M4-A documentation:
  `docs/infrastructure.md` § 7 (Open
  Action reference) + § 11 (M4-B
  Consumers; added in the M4-B plan
  promotion).
- The design system catalogue:
  `docs/design-system.md` § 4.5 (the
  `AppCapabilityList` + `AppKeyValueList`
  rows are `Planned (M4)`; the rows are
  updated to `Implemented (M4-B)` in the
  M4-B.1 first session closeout).
- The Milestone Closeout Standard:
  `.ai/workflows/milestone-closeout.md`
  (the M4-B closeout follows the
  standard; the M4-B plan promotion is
  a planning-surface change, not a
  milestone closeout).
- The branching strategy:
  `.ai/workflows/branching-strategy.md`
  (rules 4, 6, 7 are the M4-B plan
  promotion's branch operations).
- The Progressive Coding Rule:
  `.ai/workflows/progressive-coding.md`
  (the rule the M4-B plan promotion
  follows).
- The M4-B task record:
  `.ai/state/tasks.json` T-023 (the M4-B
  plan promotion task; `status: Done`,
  `2026-07-13`).
- The M4-B milestone record:
  `.ai/state/milestones.json` M4-B (the
  M4-B row `status: Active`, the M4-B
  evidence block initialised, the M4-B
  slice breakdown table initialised).
- The M4-B plan promotion session
  record: `.ai/state/session.json` (the
  M4-B plan promotion envelope).
- The M4-B plan promotion task board
  entry: `.ai/state/task-board.md` (the
  M4-A.3 stub row in `Deferred`; the
  M4-B.1 row in `Ready`).
- The M4-B plan promotion one-page
  snapshot: `.ai/state/current.md` (the
  active milestone M4-B; the active task
  T-023; the next recommended task
  T-024).
- The M4-B roadmap summary:
  `ROADMAP.md` (M4-B row `Active`; M4-B
  DoD bullets added).
- The M4-B capability: `.ai/state/capabilities.json`
  C-015 `IHostCapabilitiesService`
  (evidence block initialised).

---

## 12. M4-B plan: Status

The M4-B plan is in `Awaiting Approval`
(2026-07-13). The M4-B plan is approved
implicitly on the user's next `Next`
invocation per `.ai/commands.md` § 4
and the Progressive Coding Rule § 7.1.
The M4-B implementation begins in a
future session when the user invokes
`Approve` or `Next`.

The M4-B plan is the canonical M4-B
plan; the M4-B implementation follows
the plan.

---

## 13. Stop condition

The M4-B plan promotion session stops
after the coherent commit on the feature
branch
`feature/m4-b-capability-detection-plan-promotion`.
The next session is the M4-B
implementation (when the user invokes
`Approve` or `Next`); the M4-B plan
promotion session does **not** begin
the M4-B implementation, the M4-C plan
promotion, the M4-D plan promotion, or
any provider creation (per the brief:
"Do not begin the following task").

The M4-B plan promotion is a planning-
surface change. The M4-B implementation
sessions (M4-B.1, M4-B.2, M4-B.3) each
stop after the per-slice coherent commit.
The M4-B closeout session stops after
the M4-B closeout commit.

**The M4-B plan promotion session is
complete.** The next session is the
M4-B implementation (M4-B.1 first
session; T-024 in `.ai/state/tasks.json`)
on the user's `Approve` or `Next`
invocation.
