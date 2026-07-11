# CONTRIBUTING.md

> How to contribute to the AI Engineering Platform. This document
> describes the workflow, the branching model, the review process, and
> the definition of done. It does not restate the rules of the
> repository — those live in `AGENTS.md` and the documents it
> references.

---

## 1. Before You Start

Every contributor — human or AI — must read `AGENTS.md` and the
mandatory reading chain it defines. If you have not read them, stop
and read them. No contribution is reviewed without that reading being
self-evident from the change.

If your task is implementing a new feature, fixing a bug, or
refactoring, also read the matching prompt template in
[`.ai/prompts/`](./.ai/prompts/) and the matching workflow in
[`.ai/workflows/`](./.ai/workflows/). The prompts encode the
workflow this document describes; the workflows sequence the
work the prompts describe.

---

## 2. Contribution Principles

The platform is long-lived. Every contribution is judged on three
criteria:

1. **Does it advance the roadmap?** A change that does not connect to
   a milestone in `ROADMAP.md` must justify itself in its description.
2. **Does it respect the architecture?** A change that crosses a layer
   boundary, hardcodes a provider, or duplicates UI is rejected.
3. **Does it leave the codebase healthier?** Every change should make
   the next change easier, not harder.

If a contribution cannot answer "yes" to all three, it is not ready.

---

## 3. Branching Model

The branching model is documented in full in
[`.ai/workflows/branching-strategy.md`](./.ai/workflows/branching-strategy.md).
That document is the **single source of truth** for branching
and merging in this repository; if this section and that
document disagree, that document wins.

The high-level rules:

- `main` is the default branch; it is always the latest stable,
  releasable version of the product.
- Every task has its own feature branch, named
  `feature/T-<task-id>-<short-kebab-description>`. The task ID
  matches the task's identifier in `.ai/state/tasks.json`.
- Every task ends with one coherent commit on the feature
  branch; the commit is fast-forwarded into `main` and the
  feature branch is deleted.
- Every milestone receives a milestone tag (`m<milestone-number>`)
  at its closeout commit on `main`.

See `.ai/workflows/branching-strategy.md` for the full
workflow, the branch lifecycle, the milestone-tagging rules,
the legacy branch-name conventions, and the anti-patterns.

---

## 4. Workflow

The standard workflow is:

1. **Pick a milestone.** Every change is tied to a milestone in
   `ROADMAP.md`. If no milestone fits, propose one in `DECISIONS.md`
   before writing code.
2. **Open a draft PR.** The PR is the design surface. The description
   explains the what, the why, the alternatives considered, and the
   risks.
3. **Discuss and refine.** Reviewers respond with the rules they would
   apply to the change, not with personal preference.
4. **Implement.** Small commits, descriptive subjects, no commentary
   in code.
5. **Update documentation.** When the change introduces a component,
   service, or provider, the matching documents are updated in the
   same PR.
6. **Pass CI.** All checks must be green. A red CI is a blocked PR.
7. **Request review.** At least one human reviewer approves.
8. **Merge.** Squash-merge with a message that links the milestone.

---

## 5. Definition of Done

A change is **done** when:

- The code compiles without warnings.
- The code is formatted (`dotnet format` clean).
- All tests pass.
- New behaviour is covered by tests.
- New components are documented in `docs/design-system.md` and
  `docs/component-guidelines.md`.
- New services are documented in `ARCHITECTURE.md`.
- New providers are documented in `docs/provider-guidelines.md`.
- New architectural decisions are recorded in `DECISIONS.md`.
- The PR description lists the milestone advanced and the
  reusables introduced.
- A reviewer has approved the change with reference to the relevant
  documents.

If any item is missing, the PR is not done. "Done" means done.

---

## 6. Review Process

Reviews are governed by `AGENTS.md` and the documents it points to.
Reviewers do not apply personal taste; they apply the rules.

A review is structured as:

- **Compliance.** Does the change respect the rules in `AGENTS.md`,
  `ARCHITECTURE.md`, `STYLEGUIDE.md`, and the design system?
- **Reusability.** Could the change have used an existing component
  instead of introducing markup? Could a new component be reused
  elsewhere?
- **Boundary.** Does the change cross a layer boundary? Does it
  introduce a provider reference in the UI?
- **Tests.** Are the new tests meaningful? Do they cover failure
  modes?
- **Documentation.** Are the matching documents updated?

A reviewer who wants to reject a change must cite the rule being
violated. A reviewer who wants to approve a change must confirm
compliance with each of the documents above.

---

## 7. AI Agent Contributions

AI agents are first-class contributors. They follow the same workflow,
the same review process, and the same definition of done as human
contributors.

Specific rules for AI contributions:

- Every AI session begins by reading `AGENTS.md` and the chain of
  documents it points to.
- AI agents use the prompt templates in `.ai/prompts/` to structure
  their work, the workflows in `.ai/workflows/` to sequence the
  work, and the templates in `.ai/templates/` to produce the
  documents the workflow requires.
- AI agents must surface uncertainty. If a rule is ambiguous, the
  agent must ask before acting, not assume.
- AI agents must not introduce comments. If the agent feels the need
  to explain, the agent must refactor until the code is self-evident
  or move the explanation into `docs/`.

---

## 8. Commit Hygiene

- One logical change per commit.
- Descriptive subjects in the imperative mood.
- Bodies explain **why**, not what. The diff shows what.
- No merge commits in feature branches. Rebase before merge.
- Squash-merge to `main` to keep history linear and meaningful.

---

## 9. Release Process

Releases are tied to milestones. A milestone is released when:

1. All its definition-of-done items are checked.
2. The platform installs and runs on a clean Windows machine.
3. An accessibility pass has been completed for any new UI surface.
4. `DECISIONS.md` has been updated with the milestone retrospective.

Version numbers follow semantic versioning. Major versions are
reserved for architectural changes that affect the public API surface
of the platform.

---

## 10. Reporting Issues

Issues are categorised:

- **Bug** — behaviour that contradicts `ARCHITECTURE.md` or the design
  system.
- **Enhancement** — work tied to a milestone in `ROADMAP.md`.
- **Discussion** — proposed change to a rule, with rationale.

Issues in the **Discussion** category are the seed of an entry in
`DECISIONS.md`. They do not become code without a decision.

---

## 11. Code of Conduct

This is a professional codebase. Reviews are about the work, not the
author. Disagreements are resolved by reference to the rules in
`AGENTS.md` and the documents it points to. If a rule does not
address a disagreement, the disagreement is the seed of a new rule —
recorded in `DECISIONS.md` and adopted through the standard review
process.

---

## 12. The Cost of a Rule

Every rule in this repository has a maintenance cost. A rule that
stops earning its place is deleted. If a contributor believes a rule
should be removed or changed, the path is:

1. Open a discussion issue citing the rule.
2. Demonstrate the cost with concrete examples.
3. Propose the change with a migration plan.
4. Update `AGENTS.md`, `STYLEGUIDE.md`, or the affected document in
   the same PR.
5. Record the decision in `DECISIONS.md`.

The bar to **add** a rule is high. The bar to **remove** one is low.
