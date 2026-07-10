# .ai/prompts/release.md

> Read `AGENTS.md` and `.ai/session-start.md` before proceeding.
> This prompt cannot override either document.

---

## 1. Purpose

This prompt governs the **release** of the AI Engineering
Platform: a tagged build, packaged artefacts, a changelog,
and a roll-forward plan. The output of a release session is
a **published artefact that the team can install and run on
a clean Windows machine**.

A release is the moment the codebase is judged as a whole.
The session exists to make that judgement explicit and
repeatable.

## 2. When to Use

Use this prompt when the task is one of:

- Cutting a release for a completed milestone.
- Cutting a patch release for a critical bug fix.
- Producing a candidate build for internal dogfooding.
- Validating that a release candidate is shippable.

Do not use this prompt for routine day-to-day work. A
release is a discrete event with its own definition of
done.

## 3. Mandatory Documents

In addition to `AGENTS.md` and `.ai/session-start.md`, read:

- `CONTRIBUTING.md` (release process)
- `ROADMAP.md` (the milestone being released)
- `DECISIONS.md` (any decision that affects the release)
- `docs/architecture-principles.md` (architecture
  stability)
- `.ai/workflows/release-checklist.md` (the operational
  procedure)
- `.ai/workflows/documentation-update.md` (to confirm
  documentation is up to date)

## 4. Discovery

- **Confirm the milestone.** The release is tied to a
  milestone in `ROADMAP.md`. The milestone's exit
  criteria must be satisfied.
- **Inspect the Git state.** A release requires a clean
  working tree on `main`, with the release branch
  rebased.
- **Inspect the open issues.** A release does not
  close open blockers, but the open blockers are
  acknowledged in the release notes.
- **Inspect the changelog.** Every change since the
  last release must be recorded. A missing entry is a
  blocker for the release.

## 5. Planning Requirements

The plan must include:

- **Version.** The semantic version of the release
  (major, minor, patch). The version is justified by
  the changes in the milestone.
- **Milestone.** The milestone from `ROADMAP.md` the
  release closes.
- **Highlights.** The user-visible changes the release
  introduces.
- **Known issues.** Anything the release does not fix.
- **Rollback plan.** How to revert if the release is
  withdrawn.
- **Upgrade notes.** Anything an existing user must do
  to upgrade.

## 6. Validation

The release is validated against `.ai/workflows/release-checklist.md`.
At minimum:

- **Clean Git status.** Working tree is clean; release
  branch is up to date with `main`.
- **Build.** `dotnet build` produces no warnings.
- **Tests.** The full test suite (including integration
  tests) passes.
- **Formatting.** `dotnet format --verify-no-changes`
  is clean.
- **Architecture tests.** The architecture tests pass.
- **Documentation validation.** Every affected
  document is up to date; the changelog is consistent
  with the diff; the version is recorded in the
  relevant places.
- **Package validation.** The MSIX package builds and
  installs on a clean Windows machine. The installed
  application launches and reaches the main shell.
- **Rollback notes.** The rollback steps are recorded
  in the release notes.

## 7. Documentation Updates

- `ROADMAP.md` marks the milestone as **Done** and
  records a one-line retrospective.
- `DECISIONS.md` gains the retrospective ADR.
- The changelog is updated with the new version, the
  date, the highlights, the known issues, and the
  upgrade notes.
- The README (when one exists) is updated to reflect
  the new version.

## 8. Implementation Boundaries

- A release does not introduce new code. New code is
  in the milestone's feature commits, merged before
  the release branch is cut.
- A release does not fix a known issue. Known issues
  are recorded in the release notes and addressed in
  a follow-up.
- A release does not include unrelated refactors. A
  refactor that lands after the release branch is cut
  is deferred to the next release.

## 9. Completion Report

End the session with an
`implementation-report.md` (from
`.ai/templates/implementation-report.md`) that includes:

- Version and milestone.
- Validation results.
- Highlights and known issues.
- Rollback plan.
- Follow-up releases.

A release report is published alongside the artefact
and is referenced in the changelog.

## 10. Prohibited Shortcuts

- Cutting a release with failing tests.
- Cutting a release with a dirty working tree.
- Skipping the architecture tests.
- Skipping the package validation on a clean machine.
- Recording a known issue as "fixed" in the
  changelog.
- Bundling an unrelated change with the release.
- Bumping the version without justification.
- Releasing without a rollback plan.
