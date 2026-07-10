# Implementation Report

> Produced by the AI at the end of every implementation
> session. Filed in the conversation, in the commit, or in
> the PR. A session that does not produce this report has
> not ended.

---

## Plan Reference

- **Approved plan:** `<display name, e.g. M1.2 — Design System Core>`
- **Plan path:** `.ai/plans/<file>.md`
- **Deviations from plan:** (filled in below in the
  `Deviations` section; if the work followed the plan
  exactly, write "None.")

The plan and the report are paired: the plan is the
contract, the report is the receipt. Every
implementation must cite the approved plan; the
`Deviations` section is mandatory even when empty.

---

## Summary

One or two paragraphs. What was done, why, and the milestone
or ADR it advances.

## Files Created

- `path/to/new/file.cs` — purpose

## Files Modified

- `path/to/existing/file.cs` — what changed

## Files Deleted

- `path/to/old/file.cs` — why

## Reusable Components Introduced

- `App<NewThing>` — purpose, folder, variants, state slots
- `App<OtherThing>` — purpose, folder, variants, state slots

## Services Introduced

- `I<Area>Service` — methods, lifetime, dependencies

## Providers Touched

- `OllamaProvider` — what changed
- `GitProvider` — what changed

## Tests Added

- Unit: ...
- bUnit: ...
- Contract: ...
- Integration: ...
- Architecture: ...
- Regression: ...

## Commands Run

The actual commands the session ran, in order.

- `dotnet build`
- `dotnet test`
- `dotnet format --verify-no-changes`
- `git status --short`

## Validation Results

The actual results. Be honest; if something failed and was
fixed, say so.

- `dotnet build`: clean
- `dotnet test`: 142 passed, 0 failed
- `dotnet format`: clean
- `git status --short`: empty

## Documentation Updated

- `docs/design-system.md` — added `App<NewThing>`
- `DECISIONS.md` — added ADR-###
- `ROADMAP.md` — milestone M3 advanced

## Deviations

Anything the implementation did that the plan did not
foresee. A deviation is not a failure; an unreported
deviation is.

- Deviation 1 — what and why
- Deviation 2

## Known Limitations

Anything the implementation does not solve, deferred to a
follow-up, or that the user should be aware of.

- Limitation 1
- Limitation 2

## Next Recommended Step

The single most important thing the next session should do.
If the work is complete, this is "release M3" or "close the
milestone". If the work is paused, this is the exact command
or file the next session should open.

## Project Continuity (Rule 15)

A session that ends without updating the
project-continuity state has not ended. Confirm that
the following were updated at session end:

- [ ] `.ai/state/current.md` — updated to reflect
      the state of the repository right now
      (milestone, branch, last commit, last
      validation result, exact next step).
- [ ] `.ai/state/task-board.md` — the task the
      session worked moved from `In Progress` (or
      `Ready`) to `Done` (or `Blocked`); any new
      `Ready` items are added.
- [ ] `.ai/handoffs/YYYY-MM-DD-<slug>.md` — the
      per-session handoff, written following this
      template.
- [ ] `.ai/handoffs/latest.md` — mirror of the
      per-session handoff.

## Linked Artefacts

- `.ai/plans/<milestone-or-task-name>.md` — the
  approved plan this report implements against
  (mandatory).
- `task-brief.md` (if one was produced)
- `session-handoff.md` (if the work was paused)
- `review-report.md` (if a review was performed)
- ADR-### (if a decision was recorded)
- `.ai/state/current.md` and `.ai/state/task-board.md`
  — the live state, updated at session end
  (Rule 15 in `AGENTS.md`).
- `.ai/handoffs/YYYY-MM-DD-<slug>.md` — the
  per-session handoff, written at session end
  (Rule 15 in `AGENTS.md`).
