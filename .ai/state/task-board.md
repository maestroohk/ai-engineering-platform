# Task Board

> **Live work queue for the AI Engineering Platform.**
> Updated at the end of every AI session that changes
> project state. The most recent update wins.

## Status Codes

- **Ready** — task is defined, no blocker, no owner.
  Pick it up by setting `Status: In Progress` and adding
  your name to the row.
- **In Progress** — a session is actively working the
  task. One session, one task at a time.
- **Blocked** — task cannot proceed; the row names the
  blocker.
- **Review** — work is done; awaiting review /
  approval / merge.
- **Done** — completed, merged, and committed. Listed
  here for one cycle then archived to
  `.ai/handoffs/`.

## Ready

### M2.1 — Application Shell Skeleton

- **Plan:** `.ai/plans/M2.1-application-shell-skeleton.md`.
- **Status:** Awaiting Approval.
- **Owner:** (none).
- **Why Ready:** The M1 closeout session wrote the
  M2.1 plan to `.ai/plans/` with the
  `Awaiting Approval` status (per
  `.ai/session-start.md` step 9 and
  `.ai/templates/implementation-plan.md`).
- **Definition of done:** the M2.1 plan's "Definition
  of done" section (see the plan file).
- **First action:** review the plan; either approve it
  (and start implementation) or amend it and re-submit.

### M1 follow-up — Add `AppToolbar` example to `/design-system`

- **Source:** PART 1 verification found that
  `AppToolbar` ships and is unit-tested but is not
  exercised on the `/design-system` page (18/19
  component classes are present in the rendered HTML).
- **Status:** Ready (cosmetic, not a DoD blocker).
- **Owner:** (none).
- **Why Ready:** the design-system catalogue and the
  bUnit test for `AppToolbar` are in place; the doc
  page just does not show it.
- **Definition of done:** `app-toolbar` appears in the
  `/design-system` HTML; the example is at the right
  level of detail (matches the existing section
  examples).
- **First action:** add an `AppToolbar` example to
  `src/AiEng.Platform.App/Components/Pages/DesignSystem.razor`
  in a new "Toolbar" section; rebuild CSS; verify the
  class appears in the rendered output.

### M1 follow-up — Re-anchor the four composition-root architecture tests in `ROADMAP.md` matrix

- **Source:** PART 1 verification confirmed the four
  composition-root tests are now in place
  (registered-but-disabled) per ADR-016.
- **Status:** **Done (M1 closeout)** — the
  `ROADMAP.md` § 4 "Progressive self-dogfooding
  matrix" row for the composition-root tests was
  updated during the M1 closeout session to list
  the four tests as `Delivered in M1 closeout —
  Active in M4-D`. See the M1 closeout report's
  "Files Modified" entry for `ROADMAP.md`.

## In Progress

(none)

## Blocked

### Run M1 design-system `lavish-axi` review (deferred from M1 closeout)

- **Source:** PART 2 of the M1 closeout brief.
  Documented in
  `.ai/reviews/M1-design-system-lavish-axi-review.md`.
- **Blocker:** `lavish-axi` is not installed on the
  host. The only artefact on the filesystem is
  `agent-workbench/tools/lavish-axi.md`, a spec
  document for an event-bus daemon with no documented
  review command.
- **Unblock path:** either (a) install `lavish-axi`
  on the host, (b) the user picks a substitute review
  tool, or (c) the user decides the `lavish-axi`
  dogfooding is not the right step and removes PART 2
  from the brief.

## Review

(none)

## Done

### M1 — Design System Core (closed 2026-07-10)

- **Outcome:** 19 reusable Blazor components
  (Primitives 7, Layout 4, Display 2, Feedback 5,
  Inputs 1), 77 bUnit component tests, 3 active
  architecture tests + 4 registered-but-disabled
  composition-root tests, the `/design-system`
  documentation page, the Tailwind v3 + PostCSS
  pipeline, the design-token catalogue, light + dark
  themes. All seven ROADMAP M1 DoD items satisfied.
- **Reports:**
  `implementation-report-m1-bootstrap.md`,
  `implementation-report-m1-1-frontend-foundation.md`,
  `implementation-report-m1-2-design-system-core.md`.
- **Handoff:** `.ai/handoffs/2026-07-10-m1-closeout.md`.
- **Git:** first commit `1722bd235830cfd8b180191953116c058c92edef`
  applied on `master` at the end of the closeout
  session. 166 files committed. Working tree clean.
  No remote is configured; the commit is local-only
  per user direction. The commit subject is
  `chore(m1-closeout): close M1 milestone and prepare
  M2.1 plan`.
