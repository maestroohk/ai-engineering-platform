# .ai/workflows/documentation-update.md

> A checklist for updating the repository's documentation
> when something changes. The workflow exists so the
> documentation never silently drifts from the code, and
> so a reviewer can confirm the documentation matches the
> diff.

---

## 1. Purpose

Documentation in this repository is a first-class
deliverable. It is not a byproduct of the code. When the
code changes, the documentation changes in the same
change. This workflow lists the documents that must be
updated for the most common kinds of change.

A change that lands without its documentation is treated
as incomplete in review.

## 2. By Kind of Change

### 2.1 A New Component

A new component, whether primitive, container, or
domain, requires:

- **`docs/design-system.md`** — add the component to the
  catalogue, with its variants, sizes, state slots, and
  accessibility notes.
- **`docs/component-guidelines.md`** — add any new
  pattern the component introduces. If the component
  does not introduce a new pattern, the existing
  guidelines apply.
- **`docs/naming-conventions.md`** — confirm the
  component name follows the conventions. If the
  component is a domain component, confirm the
  convention for domain components is followed.
- **`STYLEGUIDE.md`** — confirm the markup rules are
  followed. The styleguide does not need an update for
  every component.
- **`AGENTS.md`** — only if the component introduces a
  new rule (rare). The component rules are
  already covered by the existing design-system
  section.

### 2.2 A New Provider

A new provider, whether to a new family or a new
implementation of an existing family, requires:

- **`docs/provider-guidelines.md`** — add the provider
  to the catalogue, with contract, transport, auth,
  status, fallback, and notes.
- **`docs/architecture-principles.md`** — if the
  provider introduces a new pattern (rare).
- **`docs/folder-structure.md`** — if the provider
  layout changes (rare).
- **`DECISIONS.md`** — if the provider requires a new
  family or a non-obvious mapping, add an ADR.
- **`ROADMAP.md`** — update the milestone status of
  the provider.
- **`AGENTS.md`** — only if the provider introduces a
  new rule (rare).

### 2.3 A New Folder

A new folder requires:

- **`docs/folder-structure.md`** — add the folder with
  its documented responsibility.
- **`DECISIONS.md`** — add an ADR explaining the
  responsibility and the rule that justifies the
  folder.
- **`AGENTS.md`** — only if the new folder introduces a
  new rule (rare).
- **`ARCHITECTURE.md`** — if the folder represents a
  new architectural concept.

### 2.4 An Architecture Change

An architecture change requires, in addition to the
ADRs in `DECISIONS.md`:

- **`ARCHITECTURE.md`** — update the diagram, the layer
  responsibilities, and the data flow.
- **`docs/architecture-principles.md`** — update the
  operational rules.
- **`docs/folder-structure.md`** — if the folder shape
  changes.
- **`docs/provider-guidelines.md`** — if the change
  touches providers.
- **`docs/design-system.md`** and
  **`docs/component-guidelines.md`** — if the change
  touches components.
- **`ROADMAP.md`** — if the milestone sequence changes.
- **`AGENTS.md`** — if the precedence hierarchy
  changes (rare).
- **`CONTRIBUTING.md`** — if the contribution process
  changes (rare).

### 2.5 A Naming Change

A naming change requires:

- **`docs/naming-conventions.md`** — update the
  conventions if the change introduces a new rule.
  If the change is a one-off correction, no update.
- **`DECISIONS.md`** — only if the naming change
  encodes a non-obvious decision.
- **All call sites** — the change is meaningless if
  call sites are not updated. The implementation
  report must list them.

### 2.6 A New Keyboard Shortcut

A new keyboard shortcut requires:

- **`docs/ui-principles.md`** — add the shortcut to
  the global shortcuts table.
- **`STYLEGUIDE.md`** — only if the shortcut changes
  the markup conventions.
- **`AGENTS.md`** — no update needed.

### 2.7 A New Design Token

A new design token requires:

- **`docs/design-system.md`** — add the token to the
  design tokens table.
- **`tailwind.config.js`** — define the token. (This
  is a code change, not just documentation; the
  change is reviewed like any other code change.)
- **`STYLEGUIDE.md`** — only if the token changes a
  rule.

### 2.8 A Release

A release requires, in addition to the items above
that the release changes:

- **`ROADMAP.md`** — mark the milestone as **Done** and
  add a one-line retrospective.
- **`DECISIONS.md`** — add the retrospective ADR.
- **The changelog** — add the new version, the date,
  the highlights, the known issues, and the upgrade
  notes.
- **`README.md`** (when one exists) — update the
  version, if the README records a version.

## 3. The "Did I Update the Docs?" Checklist

For every change, the implementation report answers:

- [ ] Did I add or modify a component? → `design-system.md`
- [ ] Did I add or modify a provider? → `provider-guidelines.md`
- [ ] Did I add a folder? → `folder-structure.md` + ADR
- [ ] Did I change the architecture? → `ARCHITECTURE.md` + ADR
- [ ] Did I change a name? → `naming-conventions.md` (if
      the rule changes)
- [ ] Did I add a shortcut? → `ui-principles.md`
- [ ] Did I add a token? → `design-system.md` + `tailwind.config.js`
- [ ] Did I cut a release? → `ROADMAP.md` + ADR + changelog

If any box is unchecked, the change is not done.

## 4. Anti-Patterns

- "The code is self-explanatory" used as a reason to
  skip documentation.
- A new component not added to the catalogue.
- A new provider not added to the catalogue.
- A new folder added without an ADR.
- A new design token used in code but not in the
  design system.
- A release that does not update the roadmap.
- A documentation change bundled with a code change
  in a way that hides the documentation change.
