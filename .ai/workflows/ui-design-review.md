# .ai/workflows/ui-design-review.md

> A structured review of a UI surface against the design
> system, the component guidelines, and the UI principles.
> Used both as a pre-merge check and as a self-review step
> before completing a UI feature.

---

## 1. Purpose

This workflow produces a review of a UI surface that is
**specific**, **evidence-based**, and **actionable**. It is
the operational counterpart to `.ai/prompts/review.md` for
UI changes, and the visual checklist `.ai/prompts/ui.md`
refers to as a "visual review before completion".

A UI review answers four questions:

1. Is the surface built exclusively from the design system?
2. Are all four states (loading, empty, error, populated)
   handled?
3. Is the surface accessible (keyboard, focus, ARIA, themes)?
4. Is the surface consistent with the rest of the
   platform's visual language?

## 2. Stages

### Stage 1 — Inspect the Relevant Design-System Components

- Read `docs/design-system.md` for the catalogue entry of
  every component the surface uses.
- Read the component's source file. Confirm the variants,
  sizes, and state slots are what the surface needs.
- If the surface uses a component not in the catalogue,
  the surface is rejected until the component is added.

### Stage 2 — Identify Reusable Components

- For every UI element the surface introduces, search the
  design system for an existing primitive, container, or
  domain component that satisfies it.
- A new component is added only when no existing component
  fits and no close component can be extended.
- The catalogue entry for the new component must be
  written before the component is used.

### Stage 3 — Build or Extend Primitives Before Page Markup

- Primitives and containers are created or extended first.
- Pages are composed last. Pages do not contain
  presentation logic beyond orchestration.
- The order is non-negotiable. Building the page first
  and extracting components later is the most expensive
  way to arrive at a design system.

### Stage 4 — Render Loading, Empty, Error, and Populated States

- For every data surface, render all four states.
- The populated state is the default; the others are
  demonstrated explicitly.
- Each state is captured (screenshot, video, or text
  description) for the review artifact.

### Stage 5 — Test Keyboard and Responsive Behaviour

- Tab through the surface. Every interactive element is
  reachable in a logical order.
- Use the keyboard for every action the surface
  supports. Confirm parity with mouse.
- Resize the window from the largest supported size to
  the smallest. Confirm the surface remains usable.
- Confirm focus is visible at every step.

### Stage 6 — Review Light and Dark Themes

- Render the surface in the light theme. Confirm
  contrast, color use, and shadow.
- Render the surface in the dark theme. Confirm the same.
- No component branches on theme. Both renders are
  produced by the same component, themed by tokens.

### Stage 7 — Create a Review Artefact When Possible

- A screenshot grid, a screen recording, or a written
  walk-through is attached to the review.
- The artefact is the ground truth for the review
  discussion. Disagreements about the visual are
  resolved by looking at the artefact.

### Stage 8 — Record Findings

- Findings are recorded in `review-report.md` (from
  `.ai/templates/review-report.md`) under the UI-specific
  dimensions (component reuse, state coverage,
  accessibility, theme, responsiveness).
- Severity labels follow the convention in
  `.ai/prompts/review.md`.

### Stage 9 — Resolve Findings

- The author of the surface resolves each finding.
- A finding is resolved by changing the surface, not by
  annotating the report.
- A finding that cannot be resolved becomes a follow-up
  in `ROADMAP.md` or in the issue tracker.

### Stage 10 — Update the Design-System Catalogue

- Every reusable component introduced or modified is
  reflected in `docs/design-system.md`.
- Every new pattern is reflected in
  `docs/component-guidelines.md`.
- The catalogue and the code are consistent before the
  review is closed.

## 3. Definition of Done

The UI review is done when:

- All four states render correctly.
- Keyboard navigation is verified.
- Light and dark themes are verified.
- The responsive behaviour is verified.
- The catalogue reflects the components and patterns
  introduced.
- The review artefact is attached to the review report.
- All findings are resolved or filed as follow-ups.

## 4. Anti-Patterns

- "The page works in one state" — the other three are
  forgotten.
- "Light theme is fine" — dark theme is not checked.
- "Mouse works" — keyboard is not checked.
- "We can add the catalogue entry later."
- "It's just a quick surface" — the review is skipped.
- "This visual is fine" — review by assertion instead
  of artefact.
