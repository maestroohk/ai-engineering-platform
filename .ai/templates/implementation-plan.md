# Implementation Plan

> Produced by the AI before any code is written. Reviewed by
> the human. Updated as the plan evolves during implementation.
> The plan and the implementation report are paired; the
> report refers back to this file.

## Plan Location

The **canonical** plan lives in the repository at
`.ai/plans/<milestone-or-task-name>.md`. The
`Status` field in the `Approval` block (§ below)
governs whether the plan may be edited:

- `Draft` — under construction; the AI may continue
  to edit.
- `Awaiting Approval` — finished; the AI MUST NOT
  begin implementation. Edits are limited to
  clarification.
- `Approved` — historical artefact. Substantive
  edits are forbidden; deviations are recorded in
  the implementation report, not in the plan.
- `Superseded` — a newer plan replaces this one.

A temporary working copy may exist under
`C:\Users\<user>\.claude\plans\`. That copy is not
tracked; the canonical copy is the source of
truth. See `.ai/plans/README.md` for the full
rules.

When the plan moves to `Awaiting Approval`, the
file is renamed / written under `.ai/plans/` with
the same name. The naming convention is
`<Milestone-or-Task-Name>.md` (kebab-case;
milestones use the `M<n>.<n>` prefix, e.g.
`M1.2-design-system-core.md`).

---

## Current-State Findings

What the repository actually contains in the area the task
touches. Cite files and line numbers. Do not infer; verify.

- Finding 1 (file:line, evidence, implication)
- Finding 2
- Finding 3

## Selected Approach

The approach the AI will take, in one or two paragraphs.
Reference the prompt in `.ai/prompts/` that governs the work
and the workflow in `.ai/workflows/` that sequences it.

## Alternatives Rejected

At least one alternative the AI considered and chose not to
take, with the reason. A plan that does not consider
alternatives is a plan that did not think.

- Alternative A: rejected because ...
- Alternative B: rejected because ...

## Files to Add

- `path/to/new/file.cs` — purpose
- `path/to/new/file.razor` — purpose

## Files to Modify

- `path/to/existing/file.cs` — what changes
- `path/to/existing/file.razor` — what changes

## Files to Delete

- `path/to/old/file.cs` — why

## Components Reused

The reusable components the work composes, by name. A plan
that does not list reused components is a plan that has not
searched the design system.

- `AppButton` (primary action)
- `AppCard` (container)
- `AppEmptyState` (empty list)

## Components Added

New components introduced, with their catalogue entry
(variant, size, state slots).

- `App<NewThing>` — purpose, folder, variants, state slots

## Services Added

New services introduced, with their contract and lifetime.

- `I<Area>Service` — methods, lifetime, dependencies

## Risks

What could go wrong. Be specific.

- Risk 1 — likelihood, impact, mitigation
- Risk 2

## Test Plan

The tests the implementation will add or update.

- Unit tests: ...
- bUnit tests: ...
- Contract tests: ...
- Integration tests: ...
- Architecture tests: ...
- Regression tests: ...

## Documentation Plan

The documents the implementation will update, with the
specific change.

- `docs/design-system.md` — add `App<NewThing>` to the
  catalogue
- `DECISIONS.md` — add ADR-### for ...

## Approval

- Submitted by: (AI session)
- Reviewed by:
- Date:
- Status: `Draft` / `Awaiting Approval` / `Approved` / `Superseded`
- Canonical path: `.ai/plans/<milestone-or-task-name>.md`
- See: `.ai/plans/README.md`
