# .ai/workflows/release-checklist.md

> The full release procedure. **This workflow does not
> implement release automation.** It documents the checks
> a release must pass and the order in which they are
> performed. Automation is a future task; until it
> exists, the checklist is executed manually.

---

## 1. Purpose

A release is a discrete event with its own definition of
done. The release process is the moment the codebase is
judged as a whole. The procedure is the same for major,
minor, and patch releases; the depth of each check is
proportional to the change.

## 2. Pre-Release

Before the release branch is cut:

- [ ] The milestone in `ROADMAP.md` is complete.
- [ ] The changelog is up to date with every change
      since the last release.
- [ ] All open blockers are acknowledged in the release
      notes (and addressed in a follow-up, not
      bundled).
- [ ] The version is recorded in
      `.ai/templates/release-version.md` (when one
      exists) or in the implementation report for the
      release.

## 3. The Release Branch

- [ ] The release branch is cut from `main`.
- [ ] The working tree is clean.
- [ ] No new code is added on the release branch
      (with the possible exception of release notes
      and the changelog).

## 4. The Checks

The checks are run in order. A failure at any step halts
the release.

### 4.1 Clean Git Status

- [ ] `git status --short` is empty on the release
      branch.

### 4.2 Build

- [ ] `dotnet build` produces no warnings.
- [ ] `dotnet build -c Release` produces no warnings.

### 4.3 Tests

- [ ] `dotnet test` is green.
- [ ] `dotnet test -c Release` is green.
- [ ] Integration tests tagged appropriately are
      green.
- [ ] Architecture tests are green.

### 4.4 Formatting

- [ ] `dotnet format --verify-no-changes` is clean.
- [ ] No trailing whitespace.
- [ ] No files end without a newline.

### 4.5 Architecture Tests

- [ ] The architecture test suite passes.
- [ ] No new upward dependencies were introduced
      since the last release.

### 4.6 Documentation Validation

- [ ] The `documentation-update.md` workflow's
      checklist is satisfied.
- [ ] The changelog is consistent with the diff.
- [ ] The roadmap is consistent with the milestone
      status.
- [ ] Every ADR added since the last release is
      `Accepted` (or explicitly noted as
      `Superseded`).
- [ ] The `AGENTS.md` precedence hierarchy has not
      silently changed.

### 4.7 Versioning

- [ ] The semantic version is justified by the
      changes in the milestone.
- [ ] The version is recorded in the changelog and in
      the release notes.
- [ ] A Git tag is created at the release commit.

### 4.8 Package Validation

- [ ] The MSIX package builds.
- [ ] The package installs on a clean Windows
      machine.
- [ ] The installed application launches and reaches
      the main shell.
- [ ] The installed application's version matches the
      tag.

### 4.9 Rollback Notes

- [ ] The rollback steps are recorded in the release
      notes.
- [ ] The previous release's artefacts are still
      available.
- [ ] The database migration (when one exists) has a
      documented down-migration.

## 5. The Release Commit

- [ ] The release commit message follows the commit
      hygiene rules in `CONTRIBUTING.md`.
- [ ] The commit message references the milestone
      and the changelog entry.
- [ ] The tag is annotated, not lightweight.
- [ ] The release is published to the artefact
      distribution channel (when one exists).

## 6. Post-Release

- [ ] `ROADMAP.md` marks the milestone as **Done**.
- [ ] `DECISIONS.md` gains the retrospective ADR.
- [ ] The retrospective is shared with the team.
- [ ] The follow-up issues are filed.
- [ ] The release is announced.

## 7. Definition of Done

The release is done when:

- Every check in this workflow is satisfied.
- The release commit and tag exist.
- The artefacts are published.
- The retrospective is recorded.
- The follow-ups are filed.

## 8. Anti-Patterns

- Cutting a release with failing tests.
- Cutting a release with a dirty working tree.
- Skipping the package validation on a clean machine.
- Recording a known issue as "fixed" in the
  changelog.
- Bundling an unrelated change with the release.
- Bumping the version without justification.
- Releasing without a rollback plan.
- Forgetting the retrospective.
