# docs/component-guidelines.md

> How components are designed, authored, and composed in the AI
> Engineering Platform. Read this after `AGENTS.md`,
> `ARCHITECTURE.md`, `STYLEGUIDE.md`, and
> `docs/design-system.md`.

The rules in this document are operationalised from two
recent ADRs:

- **ADR-014** — the four-state rule (`Loading`, `Empty`,
  `Error`, `Populated`) is **conditional on data
  ownership**. Pure primitives and presentational
  containers do not require the four state slots; only
  data-owning components do.
- **ADR-015** — the design-system catalogue distinguishes
  **implemented** entries (versioned, public surface) from
  **planned** entries (revisable, not yet a public
  surface). A planned entry is a placeholder, not a
  commitment.

---

## 1. Component-First, by Force of Habit

The platform's UI is built by **composing** components, not by
writing markup. A page is a tree of named, documented, reusable
pieces. A component is a leaf or a container; it is never a
page-specific hack.

The discipline is enforced by `AGENTS.md` Rule 1 and Rule 4.
This document explains the operational details.

---

## 2. Component Categories

| Category      | Location                         | Examples                                                |
| ------------- | -------------------------------- | ------------------------------------------------------- |
| Primitives    | `Components/Primitive/`          | `AppButton`, `AppBadge`, `AppStatusDot`                 |
| Containers    | `Components/Container/`          | `AppCard`, `AppSection`, `AppDialog`                    |
| Navigation    | `Components/Navigation/`         | `AppSidebar`, `AppBreadcrumb`                           |
| Feedback      | `Components/Feedback/`           | `AppLoading`, `AppSkeleton`, `AppEmptyState`            |
| Forms         | `Components/Forms/`              | `AppTextField`, `AppSelect`                             |
| Domain        | `Components/Domain/`             | `AppProviderCard`, `AppProjectCard`, `AppMessageBubble` |
| Layouts       | `Layouts/`                       | `AppLayout`, `AppEmptyLayout`                           |
| Pages         | `Pages/<Area>/`                  | `Dashboard.razor`, `Workspace.razor`                    |

This is the canonical taxonomy. A new component is filed in the
matching category on creation.

---

## 3. The Three Files of a Component

A non-trivial component has three files:

```
Component.razor       // markup
Component.razor.cs    // behaviour
Component.razor.css   // scoped styles (semantic classes only)
```

A trivial presentation component (e.g. `AppBadge` with a single
parameter) may collapse to one file. Anything with state, services,
or non-trivial markup uses three.

### 3.1 `Component.razor`

- Pure markup.
- No `@code { ... }` block larger than 5 lines.
- No logic. Every value is computed in the code-behind.
- Markup uses semantic classes, not utility chains.

### 3.2 `Component.razor.cs`

- Parameters, state, methods, lifecycle.
- The class is `partial` (implicit) and `sealed` by default.
- Logic that is reused across multiple instances of the component
  lives in a service, not in the code-behind.

### 3.3 `Component.razor.css`

- Scoped CSS.
- Every rule is `@apply`-driven.
- No raw colors, sizes, or shadows. Tokens only.

---

## 4. The Component Contract

Every component in the design system answers four questions in
its public API:

1. **What does it render?** — markup + parameters.
2. **What does it do?** — methods and event callbacks.
3. **How does it fail?** — `Error` slot or `AppErrorState`,
   when the component owns a data fetch.
4. **How does it wait?** — `Loading` slot or `AppLoading`,
   when the component owns a data fetch.

The four questions are answered **by every component**, but the
**state slots** are conditional. A component that owns a data
fetch exposes the four state slots (`Loading`, `Empty`,
`Error`, `Populated`). A component that does not own a data
fetch — a pure primitive (`AppButton`, `AppBadge`,
`AppStatusDot`) or a presentational container (`AppCard`,
`AppSection`, `AppDialog`) — exposes the slots it has
(`Header`, `Footer`, `Actions`, etc.) but does not require
`Loading`, `Empty`, `Error`, or `Populated` slots of its own.
The data-fetching parent renders those states; the primitive
or container renders whatever the parent gives it.

The four-state rule is a discipline for **data-fetching
components**, not an absolute rule for every component. The
catalogue in `docs/design-system.md` distinguishes
**data-owning components** (which must implement the four
states) from **pure primitives** and **presentational
containers** (which must not invent data-fetching state
slots they will never use). The two lists are kept separate
so a reviewer can verify the rule was applied to the right
components. The design-system catalogue rule is recorded
alongside the versioned-entry rule in
`docs/design-system.md` § 4 and `DECISIONS.md` ADR-014.

A component without the four answers is incomplete. A
primitive or presentational container that nevertheless
exposes the four state slots is over-engineered. Both are
rejected in review.

### 4.1 Parameters

Parameters are the component's public surface. They are:

- Strongly typed.
- Documented with `[Description]` and, on public surfaces,
  contractual XML `<param>`.
- Validated at the boundary (`ArgumentNullException.ThrowIfNull`).
- Immutable in spirit.

### 4.2 Event Callbacks

Event callbacks are how the component reports user intent. They
are:

- `EventCallback` for parameterless events.
- `EventCallback<T>` for events with one payload.
- Named with verb-led names (`OnClick`, `OnSelected`).
- Never used to expose internal state.

### 4.3 State Slots

**Data-owning components** (components that fetch or own a
data set, such as a list page, a project list, or a run
history) expose four child content slots:

```razor
<AppPanel>
    <ChildContent>...</ChildContent>
    <Loading>...</Loading>
    <Empty>...</Empty>
    <Error>...</Error>
    <Populated>...</Populated>
</AppPanel>
```

The four slots are:

- `Loading` — the data is in flight. The component renders
  the supplied skeleton or `AppLoading`.
- `Empty` — the data is loaded and the collection is empty.
  The component renders the supplied empty state or
  `AppEmptyState`.
- `Error` — the fetch failed. The component renders the
  supplied error state or `AppErrorState` (which reads the
  `ProviderError` and surfaces a retry).
- `Populated` — the data is loaded and the collection has
  one or more items. The component renders the supplied
  populated view.

If a slot is not provided, the component falls back to the
matching design-system surface (`AppLoading`,
`AppEmptyState`, `AppErrorState`, the default body
rendering). A data-owning component that has data but no
slot API is rejected.

**Pure primitives** (`AppButton`, `AppBadge`, `AppIcon`,
`AppStatusDot`, `AppTooltip`) and **presentational
containers** (`AppCard`, `AppSection`, `AppDialog`,
`AppDrawer`, `AppTabs`, `AppPanel`, `AppToolbar`) do not
own a data fetch. They do not expose `Loading`, `Empty`,
`Error`, or `Populated` slots. They expose the slots they
have (`Header`, `Footer`, `Actions`, `Leading`,
`Trailing`, etc.) and render whatever the parent gives
them. The data-fetching parent renders the four states and
composes the primitive or container inside the
appropriate slot.

### 4.4 Variants and Sizes

- `Variant` is an enum (e.g. `ButtonVariant`, `CardVariant`).
- `Size` is an enum (`Small`, `Medium`, `Large`).
- Defaults are explicit and intentional.
- A component without a `Variant` and `Size` is rejected if its
  visual presentation is non-trivial.

### 4.5 Presentational Containers

Presentational containers are the structuring layer of the
design system. They shape markup; they do not fetch data, do
not own state, and do not require the four-state slots
(`Loading`, `Empty`, `Error`, `Populated`).

The four M1.2 presentational containers are:

- `AppCard` — bordered surface with `Header`, `Body`, and
  `Footer` slots. The base for every "card" the platform
  shows; domain components compose `AppCard` rather than
  introducing a new card.
- `AppSection` — bordered content region with `Title`,
  `Subtitle`, `Actions`, and `Content` slots. Used to group
  a page into named regions.
- `AppPanel` — bordered region with internal padding; no
  named slots. Used for toolbars, side panels, and as a
  neutral background.
- `AppToolbar` — flex row with `Leading` and `Trailing`
  slots. The platform's toolbar surface; consumes
  `AppButton` / `AppIconButton` inside the slots.

The four containers all conform to the presentational
container rules:

- They expose the slots they have (`Header`, `Footer`,
  `Actions`, `Leading`, `Trailing`, `Content`, etc.) and
  render whatever the parent gives them.
- They do not expose `Loading`, `Empty`, `Error`, or
  `Populated` slots of their own. The data-fetching parent
  renders those states and composes the container inside
  the appropriate slot.
- They do not call services. They do not own a data fetch.

`AppErrorState` is a feedback surface, not a data-fetching
component in the four-state sense. It receives a failure
through its parameters (`Title`, `Description`, `ErrorCode`,
`CorrelationId`, `PrimaryAction`, `SecondaryAction`) and
renders the supplied values. It does not own a fetch and
does not call `Provider.HealthAsync` itself. A data-owning
component that catches a `ProviderResult.Failure` passes
the failure fields into `AppErrorState` — the data-owning
component, not `AppErrorState`, owns the data-fetching
state.

---

## 5. Composition Rules

### 5.1 Pages

A page is a route-bound component. It:

- Lives in `Pages/<Area>/`.
- Owns the page-level state and data fetching.
- Composes domain and container components.
- Renders the design system. Never raw HTML that re-implements a
  design system piece.

A page is the **only** place that talks to services directly. Domain
components receive data through parameters.

### 5.2 Domain Components

A domain component (e.g. `AppProviderCard`) is reusable across
pages. It:

- Receives data through parameters.
- Emits events through callbacks.
- Does not call services.
- Does not fetch data.

If a domain component needs data, the **page** fetches it and passes
it in. The component remains pure.

### 5.3 Containers

A container component (e.g. `AppCard`, `AppDialog`) is the
structuring layer. It:

- Defines slots: `Header`, `Body`, `Footer`, `Actions`, etc.
- Owns no domain logic.
- Owns accessibility behaviour (focus trap, escape handling).
- Is used by pages and domain components.

### 5.4 Primitives

A primitive (e.g. `AppButton`, `AppBadge`) is the visual atom. It:

- Is a single visual concept.
- Has variants, sizes, and an accessible name.
- Does not own state.
- Is used everywhere.

---

## 6. Component Authoring Checklist

Before a component is merged, it must satisfy:

- [ ] It is filed in the correct category folder.
- [ ] It has the correct file trio (markup / code / css).
- [ ] It has a `Variant` and `Size` (if applicable).
- [ ] It has `Loading`, `Empty`, `Error`, and `Populated`
      slots (if it owns data). Pure primitives and
      presentational containers do not.
- [ ] It has an `Icon` slot or a default icon (if it has status
      colour).
- [ ] It is keyboard-navigable.
- [ ] It has a visible focus state.
- [ ] It has an `aria-label` or `aria-labelledby` (if interactive
      without text).
- [ ] It is documented in `docs/design-system.md` (catalogue
      entry, marked as data-owning or primitive/container).
- [ ] It has at least one bUnit test for its primary render.
- [ ] It uses semantic classes, not raw utility chains.
- [ ] It contains no code comments.

---

## 7. Implemented Catalogue Cross-Reference

The live list of implemented components — with their
`Source` column pointing to the component's `.razor` file
— lives in `docs/design-system.md` § 4. That section is
the operational source of truth. This document is the
contract; `docs/design-system.md` is the catalogue.

When a component is implemented:

1. Move its row in `docs/design-system.md` § 4 from
   `Planned (...)` to `Implemented (...)` and fill in the
   `Source` column with the path to its `.razor` file.
2. Confirm the row's `Notes` column still describes the
   component accurately.
3. Bump the design-system version in
   `docs/design-system.md` § 1 if the implementation is
   a public-surface change per the versioning rules in
   `docs/design-system.md` § 10.

When a planned entry is removed or renamed before it is
implemented, edit the row in `docs/design-system.md` § 4
in place; a planned entry is not a public surface and the
removal does not require an ADR (per ADR-015).

---

## 8. Slots vs. Parameters

Use **slots** for:

- Composable content regions (`Header`, `Footer`, `Actions`).
- Customisation that requires markup.
- `Loading`, `Empty`, `Error` states.

Use **parameters** for:

- Simple values (text, numbers, enums).
- Event callbacks.
- Strongly-typed data handed to the component.

A component that takes 15 parameters is a code smell. Extract
subcomponents.

---

## 9. Performance

- Components are designed for re-render efficiency.
- Parameters that are arrays or collections are compared by
  reference equality, not by value.
- Long lists are virtualised.
- `ShouldRender` is overridden only when the optimisation is
  measured.
- Expensive setup runs in `OnInitialized` once, not in
  `OnParametersSet`.

---

## 10. Accessibility

- Every interactive component has a keyboard equivalent.
- Every interactive component has a visible focus ring.
- Every interactive component has an accessible name.
- Status is communicated by **icon + text + colour**, not by
  colour alone.
- Dialogs trap focus and restore it on close.
- All controls are reachable in a logical order.

---

## 11. Internationalisation

- All user-facing text is routed through a localisation service
  (even if the platform ships English-only in v1).
- Date and time formatting uses `IClock` and the user's locale.
- Numbers and currencies use the user's locale.
- Layouts handle longer text (German, French) without breaking.

---

## 12. Versioning a Component

When a component's API changes:

1. Add a new optional parameter if possible.
2. If a parameter must be removed or renamed, deprecate it first
   (one milestone), then remove.
3. Update `docs/design-system.md` with the change.
4. Record the change in `DECISIONS.md`.

Components are public surface. Breaking changes are a big deal.

---

## 13. Anti-Patterns

- **Page-shaped components** that should have been components
  living in `Pages/`.
- **God components** with 30 parameters.
- **Service-calling domain components** that should be pure.
- **Inline `<style>` blocks** for anything that should be a
  semantic class.
- **Long utility chains** in markup.
- **Commented code**. Refactor or delete.
- **`Shared/` as a dumping ground.** `Shared/` contains only what
  is genuinely shared across the entire application.
