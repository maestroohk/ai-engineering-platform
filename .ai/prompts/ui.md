# .ai/prompts/ui.md

> Read `AGENTS.md` and `.ai/session-start.md` before proceeding.
> This prompt cannot override either document.

---

## 1. Purpose

This prompt governs the design and implementation of a
**user interface** in the AI Engineering Platform. A UI
session produces components and pages that compose the
design system, respect the Tailwind discipline, and handle
all four states (loading, empty, error, populated) for every
data surface.

The output of a UI session is a **rendering surface built
exclusively from reusable components, with no page-specific
duplication, no raw utility chains, and no accessibility
gap**.

## 2. When to Use

Use this prompt when the task is one of:

- Adding a new page.
- Adding a new domain component (for example,
  `AppProviderCard`, `AppMessageBubble`).
- Extending a primitive, container, or domain component.
- Changing the visual language (new tokens, new variants,
  new sizes).
- Fixing a UI defect that is not a runtime bug (use
  `bugfix.md` for runtime bugs).

Do not use this prompt for non-visual work. A change to a
service, a provider, or a contract that has no visible
surface uses `feature.md`, `provider.md`, or
`architecture.md` instead.

## 3. Mandatory Documents

In addition to `AGENTS.md` and `.ai/session-start.md`, read:

- `STYLEGUIDE.md`
- `docs/design-system.md`
- `docs/component-guidelines.md`
- `docs/ui-principles.md`
- `docs/naming-conventions.md`
- `docs/folder-structure.md`
- `docs/architecture-principles.md` (for the dependency
  direction)

## 4. Discovery

- **Inspect the design-system catalogue.** Confirm the
  primitives, containers, and domain components that
  already exist. The catalogue is the spec.
- **Search the codebase for similar surfaces.** A page
  that solves a similar problem is the closest prior art.
  Read it; do not copy it.
- **Identify tokens to use.** Color, spacing, radius,
  shadow, type — all from the design-system tokens. No
  raw values.
- **Identify state contracts.** A data surface needs
  `Loading`, `Empty`, `Error`, and `Populated` slots when
  the component **owns the data fetch**. Pure primitives
  (`AppButton`, `AppBadge`, `AppStatusDot`, `AppIcon`,
  `AppTooltip`) and presentational containers
  (`AppCard`, `AppSection`, `AppDialog`, `AppDrawer`,
  `AppTabs`, `AppPanel`, `AppToolbar`) do not own data
  and do not require the four state slots. Confirm the
  component is data-owning before adding the slots; a
  primitive or container that nevertheless exposes the
  four slots is over-engineered and is rejected. The
  four-state rule is conditional on data ownership
  (see `docs/design-system.md` § 5.4 and
  `docs/component-guidelines.md` § 4).

## 5. Planning Requirements

The plan must include:

- **Components reused** by name.
- **Components added** by name, with the new catalogue
  entry, the variants, the sizes, and the state slots.
- **Tokens used** (no new tokens unless explicitly
  justified and added to the design system).
- **Layout breakdown** (sidebar, topbar, page header,
  toolbar, body).
- **Keyboard map** for the surface.
- **Accessibility notes** (focus order, ARIA, contrast,
  motion).
- **Tests planned** (bUnit for the primary render, the
  state slots, the keyboard navigation, and the
  responsive behaviour).
- **Visual review plan.** When possible, the surface is
  reviewed against the design system before completion
  (see `.ai/workflows/ui-design-review.md`).

## 6. Implementation Boundaries

- **Component-first design.** Pages compose components.
  Pages do not contain presentation logic beyond
  orchestration. No page is a one-off shell around
  raw markup.
- **Tailwind semantic classes.** Use semantic classes
  from the design system. Long utility chains are
  rejected. New semantic classes are added to the
  design system before use, not after.
- **Reuse before creation.** If a component that
  satisfies the need exists, use it. If a close
  component exists, extend it. Only create a new
  component when neither path works.
- **All four states on data-owning components.**
  Every component that owns a data fetch renders
  `Loading`, `Empty`, `Error`, and `Populated` states.
  A data-owning component with only one state is
  incomplete. Pure primitives and presentational
  containers do not require the four state slots; they
  expose the slots they have (`Header`, `Footer`,
  `Actions`, `Leading`, `Trailing`) and render whatever
  the parent gives them. The four-state rule is
  conditional on data ownership (see
  `docs/design-system.md` § 5.4 and
  `docs/component-guidelines.md` § 4.3).
- **Keyboard navigation.** Every interactive element is
  reachable and operable with the keyboard. Focus is
  always visible. Focus order matches visual order.
- **Visible focus.** Focus rings are never suppressed.
- **Light and dark themes.** The surface renders
  correctly in both themes. The components do not
  branch on theme.
- **Desktop-first layout.** The primary layout is tuned
  for Windows desktop. Responsive adjustments are
  applied at the layout level, not the component level.
- **No page-specific duplicates.** A piece of markup
  that appears twice is a component. A piece of markup
  that appears once and is likely to appear again is
  also a component.
- **bUnit tests.** Every new or modified component has
  a bUnit test that covers the primary render and the
  state slots.
- **No provider implementation in components or pages
  (per ADR-016).** A page or a component does not
  reference a `Providers.<X>` namespace. A component
  that needs a provider resolves it through the
  registry (`IProviderRegistry` or the family-scoped
  `I<X>ProviderRegistry`); the registry is the only
  consumer-facing surface of a provider family.
- **Visual review.** Before declaring the work done,
  run the visual review checklist from
  `.ai/workflows/ui-design-review.md`.

## 7. Validation

- `dotnet build` produces no warnings.
- `dotnet test` is green, including the new bUnit
  tests.
- `dotnet format` is clean.
- A manual visual review confirms the design system is
  used and no page-specific duplication exists.
- A keyboard-only exercise of the surface confirms
  reachability, operability, and visible focus.
- A light/dark theme check confirms both render
  correctly.
- A responsive check confirms the surface behaves
  sensibly down to the smallest supported window.

## 8. Documentation Updates

- `docs/design-system.md` gains every new component,
  variant, or token.
- `docs/component-guidelines.md` gains every new
  pattern.
- `docs/ui-principles.md` is updated if a new pattern
  is introduced.
- `docs/naming-conventions.md` is updated if a naming
  rule emerges.
- `DECISIONS.md` gains an ADR if a non-obvious
  decision is made (for example, a new layout
  pattern).

## 9. Completion Report

End the session with an
`implementation-report.md` (from
`.ai/templates/implementation-report.md`) that includes:

- Components added and reused.
- Tokens used and added.
- State slots exposed.
- Accessibility checks performed.
- Visual review results.
- Documentation updated.

If a visual review artifact is produced, it is attached
to the report.

## 10. Prohibited Shortcuts

- Writing raw HTML because "this is just a quick
  surface".
- Inline styles or inline `@apply` blocks.
- Long utility chains in markup.
- Hardcoded colors, sizes, or shadows.
- Components without `Loading`, `Empty`, or `Error`
  states.
- Components without keyboard navigation.
- A focus ring that is invisible or removed.
- A new token used before it is added to
  `docs/design-system.md`.
- A new component used before it is added to the
  catalogue.
- Skipping the visual review because the diff is
  "small".
