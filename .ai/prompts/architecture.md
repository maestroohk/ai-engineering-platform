# .ai/prompts/architecture.md

> Read `AGENTS.md` and `.ai/session-start.md` before proceeding.
> This prompt cannot override either document.

---

## 1. Purpose

This prompt governs changes to the **architecture itself**:
new layers, new boundaries, new provider families, new
dependency rules, or any change that affects more than one
folder or more than one ADR.

The output of an architecture session is an **ADR** (new or
updated) in `DECISIONS.md`, with a migration plan and,
optionally, the smallest demonstrator that proves the new
shape works.

A change to the architecture is **not** a code change. The
code follows the architecture, never the other way around.

## 2. When to Use

Use this prompt when the task is one of:

- Proposing a new layer or a new boundary.
- Adding a new provider family.
- Changing the dependency direction between existing
  layers.
- Recording a non-obvious decision that future sessions
  will need to follow.
- Reviewing or superseding an existing ADR.

Do not use this prompt for:

- A feature implementation (use `feature.md`).
- A behaviour-preserving refactor (use `refactor.md`).
- A bug fix (use `bugfix.md`).
- A new provider adapter (use `provider.md`).

## 3. Mandatory Documents

In addition to `AGENTS.md` and `.ai/session-start.md`, read:

- `ARCHITECTURE.md`
- `DECISIONS.md` (every existing ADR)
- `docs/architecture-principles.md`
- `docs/folder-structure.md`
- `docs/provider-guidelines.md` (if the change touches
  providers)
- `docs/design-system.md` and `docs/component-guidelines.md`
  (if the change touches components)
- `ROADMAP.md` (to confirm the change fits the current
  direction)

## 4. Discovery

- **Evidence from the current implementation.** The change
  must be motivated by what the codebase actually does or
  actually prevents, not by a hypothetical future need.
  Cite the files and lines.
- **Survey existing ADRs.** A change that contradicts an
  existing ADR must supersede it explicitly. The
  `Supersedes` and `Superseded by` fields are mandatory.
- **Identify affected layers.** Every architectural change
  lists the layers it touches and the layers it does not
  touch. The unchanged layers are part of the change
  record.
- **Migration cost.** Estimate the number of files and
  components affected, the tests that must be updated, and
  the documentation that must change.

## 5. Planning Requirements

The plan must include:

- **Trade-off analysis.** At least two options, with the
  advantages and disadvantages of each. The decision is
  recorded against the alternatives.
- **Affected layers.** Explicit list of layers touched and
  layers preserved.
- **Migration plan.** Ordered list of steps to bring the
  existing code into compliance. The migration is sized
  realistically; "we'll do it later" is not a plan.
- **ADR.** A new ADR with status `Proposed`. The ADR is
  updated to `Accepted` only after review.
- **Demonstrator (optional but encouraged).** The smallest
  piece of code that proves the new shape works in
  practice. The demonstrator is a smoke test, not a
  feature.

## 6. Implementation Boundaries

- **No implementation unless explicitly approved.** An
  architecture session produces the ADR and, if requested,
  the demonstrator. It does not produce the migration.
- **No silent rule changes.** Every rule change is in the
  ADR, in the affected document, and in the index of
  `DECISIONS.md`.
- **No upward dependencies.** The new shape must not
  introduce a layer that depends on the layer above it.
- **No scope creep.** A discussion of an unrelated
  architectural question is filed for a separate session,
  not bundled in.

## 7. Validation

- The ADR is reviewed by at least one human architect (per
  `CONTRIBUTING.md`).
- The demonstrator, if produced, builds and passes its
  smoke test.
- The affected documents are updated to reflect the new
  shape.
- The change is consistent with every other ADR in
  `DECISIONS.md`. Contradictions are surfaced, not
  ignored.

## 8. Documentation Updates

- `DECISIONS.md` gains the new ADR (or the updated
  superseding one).
- `ARCHITECTURE.md` is updated if the architecture
  diagram, layer responsibilities, or data flow changes.
- `docs/architecture-principles.md` is updated to reflect
  the new rule.
- `docs/folder-structure.md` is updated if the folder
  shape changes.
- `docs/provider-guidelines.md` is updated if a new
  provider family is added.
- `AGENTS.md` is updated if a new precedence rule
  emerges (rare; usually a `docs/` change is enough).
- `ROADMAP.md` is updated if the change affects the
  milestone sequence.

## 9. Completion Report

End the session with an
`implementation-report.md` (from
`.ai/templates/implementation-report.md`) that includes:

- The ADR id, title, and status.
- The trade-offs considered.
- The migration plan (size, scope, ordering).
- The demonstrator (if any) and its smoke test result.
- The follow-up sessions the migration requires.

## 10. Prohibited Shortcuts

- Editing `ARCHITECTURE.md` or `docs/architecture-principles.md`
  without an ADR.
- Marking an ADR `Accepted` before review.
- Bundling an architectural change with a feature or
  refactor.
- "We'll write the migration plan later." A migration
  without a plan is not a migration.
- Treating the architecture as a suggestion rather than a
  contract.
