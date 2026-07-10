# Task Brief

> A filled-in copy of this template is the **single source of
> truth for a task**. The AI uses it to plan, the human uses it
> to review the plan, and the implementation report refers back
> to it.

---

## Objective

A single, declarative sentence. What is the user-visible
outcome of this task?

## Background

Why is this task being done? What problem does it solve or
what opportunity does it unlock? Reference the milestone from
`ROADMAP.md` and any prior ADRs that justify it.

## Current Behaviour

What does the platform currently do, in the area the task
touches? Be specific. Cite files and lines.

## Desired Behaviour

What should the platform do after the task is done? Be
specific. If the behaviour depends on a provider, a state
store, or a configuration, name it.

## Acceptance Criteria

A bulleted list of observable conditions that must be true
for the task to be considered done. Each criterion must be
testable.

- [ ] Criterion 1
- [ ] Criterion 2
- [ ] Criterion 3

## Constraints

Hard limits the task must respect. Examples:

- "Must not change the provider contract."
- "Must not introduce a new dependency."
- "Must not modify `AppButton`."
- "Must complete within the current milestone."

## Affected Areas

The folders, components, services, providers, and documents
the task is expected to touch. Anything not listed here is
out of scope.

- Folders:
- Components:
- Services:
- Providers:
- Documents:

## Out of Scope

What this task explicitly does not do. Naming the things the
task is not is as important as naming the things it is.

- Not in scope 1
- Not in scope 2

## Validation

How will the task be validated? Be specific about commands,
tests, and manual checks.

- Build: `dotnet build`
- Tests: `dotnet test`
- Format: `dotnet format --verify-no-changes`
- Manual check: ...

## Documentation

Which documents must be updated as part of this task?

- [ ] `docs/design-system.md`
- [ ] `docs/component-guidelines.md`
- [ ] `docs/provider-guidelines.md`
- [ ] `docs/architecture-principles.md`
- [ ] `docs/folder-structure.md`
- [ ] `docs/naming-conventions.md`
- [ ] `docs/coding-standards.md`
- [ ] `docs/ui-principles.md`
- [ ] `ROADMAP.md`
- [ ] `DECISIONS.md`
- [ ] `CONTRIBUTING.md`
- [ ] `ARCHITECTURE.md`
- [ ] `STYLEGUIDE.md`
- [ ] `AGENTS.md` (rare; requires an ADR)

## Approval

- Requested by:
- Approved by:
- Date:
