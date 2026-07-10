# Current State

> **One-page snapshot. Read this first in any new
> session.** The most recent update wins; older
> snapshots live in `.ai/handoffs/`.

## Project

- **Name:** AI Engineering Platform.
- **Repository:** `C:\Users\hkasozi\source\repos\ai-engineering-platform`.
- **Solution file:** `AiEng.Platform.slnx` (.NET 10 SLN-X format).
- **Primary deployment target:** Windows 11 (desktop first).
- **Stack:** .NET 10 / C# 14 / Blazor Web App / Tailwind v3 / bUnit 2.7.2 / xUnit 2.9.3.

## Milestone

- **Active milestone:** M1.3 — Application Shell.
- **Last closed milestone:** M1 — Design System Core (closed 2026-07-10).
- **Next planned milestone:** M2.1 — Application Shell Skeleton (plan in `.ai/plans/M2.1-application-shell-skeleton.md`, status: Awaiting Approval).

## Branch and Git State

- **Branch:** `master`.
- **Last commit hash:** (no commits yet — the repository was initialised during the M1 closeout session but the first commit is staged for PART 7).
- **Working tree:** clean of uncommitted changes from the M1 closeout session; the new architecture tests and `.ai/state/` + `.ai/handoffs/` directories are untracked.
- **Remote:** **none configured.** PART 7 was completed commit-only per user direction. Adding a remote is a separate decision.

## Last Validation Result

From the M1 closeout session (2026-07-10):

- `npm run css:build` → exit 0. Output: 12,890 bytes minified.
- `dotnet restore` → exit 0. All projects up to date.
- `dotnet build AiEng.Platform.slnx` → exit 0. **0 warnings, 0 errors.**
- `dotnet test AiEng.Platform.slnx --no-build` → **80 passed, 4 skipped, 0 failed.** (77 bUnit + 3 active architecture tests; 4 composition-root tests registered-but-disabled per ADR-016 / M4-D.)
- `dotnet format AiEng.Platform.slnx --verify-no-changes` → exit 0.
- App runs on `http://localhost:5286`. All five routes return 200 (`/`, `/counter`, `/weather`, `/design-system`, `/not-found`). 18/19 component CSS classes are present in the `/design-system` HTML. The missing one is `app-toolbar` (the `AppToolbar` component ships but the `/design-system` page does not yet exercise it; minor doc gap, not a DoD failure).

## Definition-of-Done Status

ROADMAP M1 DoD (verified during PART 1 of the M1 closeout):

- [x] Blazor Server runs and shows `/design-system` built exclusively from the components listed in the M1 reusable-components list.
- [x] Tailwind is wired with `@apply`-driven semantic classes.
- [x] A bUnit test verifies that `AppButton` renders all variants.
- [x] The solution compiles with the four source projects and the three test projects.
- [x] The architecture test `App_DoesNotReference_Providers_Implementations` is in place and green.
- [x] The architecture test `Pages_Use_DesignSystem_Components_Not_DOM` is in place and green.
- [x] The four composition-root architecture tests are **registered but disabled** with explicit skip messages citing ADR-016 and M4-D as the activation point.

**M1 is closed. The five M1.2 architecture tests added in the M1 closeout session satisfy the remaining DoD items.**

## Exact Next Step

The next session picks up the topmost `Ready` task in
`.ai/state/task-board.md`. As of the closeout session,
that is:

> **M2.1 — Application Shell Skeleton.** Read
> `.ai/plans/M2.1-application-shell-skeleton.md`. The
> plan is `Awaiting Approval`; the first action is
> either approving the plan (and starting M2.1
> implementation) or amending the plan and re-submitting
> it.

If the user prefers to close a smaller scope first
(e.g. add an `AppToolbar` example to `/design-system`,
wire the four composition-root tests as the M1.2
follow-up), the task board contains those as separate
`Ready` items.

## Documents the Next Session Must Read

1. `AGENTS.md` — the constitution (14 rules, precedence hierarchy).
2. `.ai/session-start.md` — the 13-step session-start procedure.
3. `.ai/state/current.md` — this file.
4. `.ai/state/task-board.md` — the live work queue.
5. `.ai/handoffs/latest.md` — the most recent session handoff.
6. The plan for the chosen task (e.g. `.ai/plans/M2.1-application-shell-skeleton.md`).
7. The implementation report(s) for the milestones the plan touches (e.g. `implementation-report-m1-2-design-system-core.md` for M2.1).

## Linked Artefacts

- `.ai/state/task-board.md` — live work queue.
- `.ai/handoffs/latest.md` — most recent handoff.
- `.ai/handoffs/2026-07-10-m1-closeout.md` — the M1 closeout session handoff.
- `.ai/reviews/M1-design-system-lavish-axi-review.md` — the deferred PART 2 review record.
- `.ai/plans/M2.1-application-shell-skeleton.md` — the M2.1 plan (Awaiting Approval).
- `ROADMAP.md` — milestone plan.
- `DECISIONS.md` — ADR index (ADR-011 through ADR-016 are the relevant ones for M1 / M2).
