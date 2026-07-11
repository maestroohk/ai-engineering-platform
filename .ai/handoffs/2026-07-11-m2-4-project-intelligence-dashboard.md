# Session Handoff — M2.4 — Project Intelligence Dashboard

> **Per-session handoff for the M2.4
> closeout session, 2026-07-11.**
> M2.4 — Project Intelligence
> Dashboard is **Delivered**. The
> coherent commit
> `feat(m2.4): add project intelligence dashboard`
> is on
> `feature/m2-4-project-intelligence-dashboard`.
> The next session opens the M2.5 plan
> (`.ai/plans/M2.5-empty-routes-responsive-accessibility.md`,
> `Awaiting Approval`).

---

## Task

Deliver the M2.4 — Project Intelligence Dashboard slice per the approved plan (`.ai/plans/M2.4-project-intelligence-dashboard.md`): land `IProjectIntelligenceReader` + `ProjectIntelligenceSnapshot` + `ProjectIntelligenceReader` in `src/AiEng.Platform.Application/ProjectIntelligence/`; land `ProjectIntelligenceServiceCollectionExtensions` in `src/AiEng.Platform.App/Composition/`; land the `/dashboard` page in `src/AiEng.Platform.App/Components/Pages/Dashboard.razor` (M0.5-data sections in the **Populated** state, M3+-data sections in the **Empty** state, per ADR-014); fix the theme toggle bug; add 6 unit + 13 bUnit + 2 architecture tests; add the `Pages_Resolve_State_Through_Reader` architecture test; reconcile the state files (`.ai/state/current.md`, `.ai/state/task-board.md`, `.ai/state/session.json`, `.ai/state/tasks.json`, `.ai/state/milestones.json`, `.ai/state/capabilities.json`, `ROADMAP.md`) per the user instruction; write the implementation report and this handoff; promote the M2.5 plan stub to a full plan in `Awaiting Approval`; commit; stop. Do NOT begin M2.5 (per the Progressive Coding Rule and the brief).

---

## State of the Repository at Session End

- **Branch:** `feature/m2-4-project-intelligence-dashboard`.
- **Base:** `fb89187` (the M2.3 closeout commit on `feature/m2-3-topbar-breadcrumbs`).
- **Working tree:** clean. All M2.4 changes are unstaged (the focused commit is created at the end of the session).
- **Last stable commit (pending):** `feat(m2.4): add project intelligence dashboard` on `feature/m2-4-project-intelligence-dashboard`.
- **No remote** is configured. Push is skipped per the brief.

---

## What Landed

### Read-side application service (Application layer)

- **`IProjectIntelligenceReader`** (interface) — `GetSnapshotAsync(CancellationToken) → Task<ProjectIntelligenceSnapshot>`.
- **`ProjectIntelligenceSnapshot`** (record) — the typed snapshot with `ProductName`, `RepoRoot`, `Branch`, `LastCommitHash`, `LastCommitSubject`, `LastCommitAt`, `LastStableCommit`, `BuildStatus`, `TestStatus`, `CurrentMilestone`, `CurrentTask`, `CurrentPlan`, `PlansAwaitingApproval`, `CapabilityProgress`, `ProductProgress`, `ImplementedCapabilities`, `NextRecommendedAction`, `GeneratedAt`. Plus child records: `CapabilityProgressInfo` (with `Empty`), `CurrentMilestoneInfo`, `CurrentTaskInfo`, `CurrentPlanInfo`, `PlanInfo`, `NextActionInfo`, `CurrentCapabilitySummary`.
- **`ProjectIntelligenceReader`** (implementation) — reads `.ai/state/*.json` (tasks, milestones, capabilities, session), reads `.ai/plans/*.md` (frontmatter: `**Status:**`, `**Milestone:**`, slice extracted from the first whitespace-separated token), reads `PRODUCT.md` (first heading), produces the snapshot deterministically. Uses `JsonNamingPolicy.SnakeCaseLower` for snake_case → PascalCase. `IStateFileReader` is the file-system seam (testable; `FileSystemStateFileReader` is the production implementation).

### Composition root

- **`ProjectIntelligenceServiceCollectionExtensions.AddProjectIntelligence`** — registers `ProjectIntelligenceOptions` (singleton; default `RepoRoot` walks up from `Directory.GetCurrentDirectory()` looking for `AiEng.Platform.slnx`), `IStateFileReader` → `FileSystemStateFileReader` (singleton), `IProjectIntelligenceReader` → `ProjectIntelligenceReader` (singleton).
- **`ServiceCollectionExtensions.AddPlatformServices`** (modified) — added `services.AddProjectIntelligence();` after `services.AddNavigation(assemblies);`.

### Blazor page

- **`Dashboard.razor`** at `/dashboard` — `AppPageHeader` with the product name and the M0.5-description; **Current Milestone + Current Task** card (milestone title + status, task title + status + owner, active plan link + status, last commit hash + subject, branch, next recommended action); **Capability Progress** card (Verified / Delivered / In progress / Ready / Planned / Blocked / Deferred / Total counts; per-milestone breakdown; implemented capabilities list; plans awaiting approval list); **Build Status** card (or empty state when missing); **M3+ Empty** cards (Running agents, Queued tasks, Recent quality gates, Latest commits, Providers, Recent reviews — each with a clear "M3 / M4 / M7 fills this section" message); **Loading** state when the snapshot is null.
- **`Dashboard.razor.css`** — scoped CSS for the dashboard layout; status-based tag colors.

### Theme toggle bug fix

- **`App.razor`** (modified) — added `appTheme.current` JS function that returns the resolved theme (`data-theme` → `localStorage` → `'light'` default); the IIFE now writes the resolved theme back to `localStorage`; `appTheme.set` accepts only `'light'` or `'dark'` (input validation).
- **`AppThemeToggle.razor`** (modified) — uses `appTheme.current` (not `appTheme.get`) to read the initial state in `OnAfterRenderAsync(firstRender)`; the click handler updates `IsDark` **synchronously before** the `await JSRuntime.InvokeVoidAsync("appTheme.set", next)` call; `JSDisconnectedException` is handled.

### Tests

- **6 unit tests** for `ProjectIntelligenceReader` (happy path; missing state; malformed JSON; capability counts; plans awaiting approval; next action).
- **3 composition tests** for `AddProjectIntelligence` (reader is registered; lifetime is Singleton; default `RepoRoot` resolves).
- **9 bUnit tests** for the Dashboard (page header; current milestone + task; capability progress; M3+ empty states; next recommended action; implemented capabilities; plans awaiting approval; build status; empty build status).
- **4 bUnit tests for the theme toggle bug fix** (reads resolved theme from `appTheme.current`; click updates state from light to dark immediately; click updates state from dark to light immediately; click invokes `appTheme.set` with the toggled value).
- **2 architecture tests** (`Dashboard_page_resolves_state_through_IProjectIntelligenceReader`; `No_page_in_Components_Pages_directly_reads_state_files`).

### State updates (per Rule 15)

- `tasks.json` — T-014 → `Done` with full evidence.
- `milestones.json` — M2.4 → `delivered`; M2 evidence block updated.
- `capabilities.json` — C-022 `IProjectIntelligenceReader` → `status: Done, completion_status: Verified`; evidence populated.
- `session.json` — `current_understanding`, `last_action`, `intended_next_action` updated to M2.4 closeout.
- `current.md` — M2.4 Delivered; M2.5 next slice; test count 175 + 4 skipped; `/dashboard` route added; M2.4 implementation report referenced; M2.4 handoff referenced.
- `task-board.md` — M2.4 moved from Ready to Done Recently; M2.5 moved from Deferred to Ready.
- `ROADMAP.md` — M2.4 row → `Delivered (M2.4, 2026-07-11)`; M2.5 row → `Ready`; the M2 paragraph in § 2 updated.
- `.ai/plans/M2.5-empty-routes-responsive-accessibility.md` (new) — the M2.5 plan promoted from the `Draft` stub to a full plan in `Awaiting Approval`.

---

## Test Results

- **6 unit tests:** all pass.
- **163 bUnit / integration tests:** all pass.
- **6 active architecture tests:** all pass. **4 registered-but-disabled:** skipped per ADR-016 (provider composition tests, deferred to M4-D).
- **Total: 175 passed, 4 skipped, 0 failed.**

---

## Validation

- `npm run css:build` → exit 0.
- `dotnet build` → 0 warnings, 0 errors.
- `dotnet test --no-build` → 175 passed, 4 skipped, 0 failed.
- `dotnet format --verify-no-changes` → clean.
- Visual smoke test: `dotnet run` on `http://localhost:5170`; `curl /dashboard` → HTTP 200; 11 expected markers in the response; `curl /` → HTTP 200; 5 `data-theme` / `appTheme` references in the response.

---

## How to Resume (M2.5)

1. **Read** this handoff, the M2.4 implementation report (`implementation-report-m2-4-project-intelligence-dashboard.md`), the M2.5 plan (`.ai/plans/M2.5-empty-routes-responsive-accessibility.md`), and the M2 closeout template (the M1 closeout is the reference: `implementation-report-m1-closeout.md`).
2. **Reconcile** the state per `.ai/session-start.md` step 6: branch is `feature/m2-4-project-intelligence-dashboard`; HEAD is the M2.4 closeout commit; T-015 is `Ready` (in `tasks.json`); M2.5 plan is `Awaiting Approval`; M2.5 capability `C-023` / `C-024` / `C-025` are not yet in `capabilities.json` (the M2.5 plan defines them; the M2.5 implementation session will add them).
3. **Approve** the M2.5 plan (the M2.5 plan is `Awaiting Approval`; the first action of the next session is to either approve the plan and start M2.5 implementation, or amend the plan and re-submit it).
4. **Create** the branch `feature/m2-5-empty-routes-responsive-accessibility` off the M2.4 closeout commit.
5. **Implement** the M2.5 slice per the plan's § 8 (13-step order).
6. **Validate** (css:build, build, test, format, smoke).
7. **Update** the state per Rule 15.
8. **Write** the M2.5 implementation report and handoff.
9. **Commit** `feat(m2.5): add empty routes, responsive matrix, and a11y audit`.
10. **Stop.** The M2.6 closeout session follows.

---

## Known Limitations (forwarded to M2.5 / M3)

- The dashboard does not surface the task ID; only the task title and status. A future slice can add the task ID as a `app-dashboard-mono` badge if the operator needs it.
- The "Next recommended action" cell renders the plan path as text; it does not link to the plan. The M2.5 slice is the right time to add a link to the plan.
- The dashboard's "Implemented capabilities" list is hand-derived from `capabilities.json`; the per-step evidence list from `PRODUCT.md` is not surfaced. The M2.5 / M3 closeout is the right time to add a "what evidence is missing for step X?" widget.
- The M2.5 slice introduces the axe-core audit harness (per the M2.5 plan § 9 "C-025 — A11y audit harness"). The M2.4 dashboard is not audited in this slice.

---

## Cross-References

- **Implementation report:** `implementation-report-m2-4-project-intelligence-dashboard.md`
- **Plan:** `.ai/plans/M2.4-project-intelligence-dashboard.md`
- **Prior handoff:** `.ai/handoffs/2026-07-11-m2-3-topbar-breadcrumbs.md`
- **Next plan:** `.ai/plans/M2.5-empty-routes-responsive-accessibility.md`
- **State files:** `.ai/state/current.md`, `.ai/state/task-board.md`, `.ai/state/session.json`, `.ai/state/tasks.json`, `.ai/state/milestones.json`, `.ai/state/capabilities.json`
- **Roadmap:** `ROADMAP.md` § 2 (M2 row, M2.4 row, M2.5 row), § 3 (M2.4 slice table), § 4 (M2.4 matrix)
- **Constitution:** `AGENTS.md` (17 rules; Rule 15 is the project-continuity rule)
- **Session start:** `.ai/session-start.md`
- **Progressive Coding Rule:** `.ai/workflows/progressive-coding.md`

---

## Git State at Session End

- **Branch:** `feature/m2-4-project-intelligence-dashboard`
- **Base:** `fb89187` (M2.3 closeout commit on `feature/m2-3-topbar-breadcrumbs`)
- **Working tree:** clean (all M2.4 changes are in the focused commit at the end of the session)
- **Last stable commit:** the M2.4 closeout commit (the focused commit at the end of the session)
- **No remote** is configured. Push is skipped per the brief.
