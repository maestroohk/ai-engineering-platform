# docs/ui-principles.md

> The principles that govern how the AI Engineering Platform looks
> and feels. This document covers layout, density, motion,
> accessibility, and the three states every UI surface must handle.
> Read it after `AGENTS.md`, `ARCHITECTURE.md`, and
> `docs/design-system.md`.

---

## 1. The Product Is a Developer Tool

The platform is built for engineers who run many tools in parallel
and value **density, speed, and predictability** over decoration.
The visual language must reflect that:

- Information per square inch is high.
- Borders and dividers are subtle, not loud.
- Color is used to communicate state, not to decorate.
- Motion is rare and short.
- Keyboard is a first-class input device.

The platform should feel closer to an IDE than to a marketing site.

---

## 2. Layout Principles

### 2.1 The Shell

The application shell has three regions:

```
+----------------------------------------------------------+
| TopBar                                                    |
+--------+-------------------------------------------------+
|        |                                                  |
|        |  PageHeader                                      |
| Side   |  ----------------------------------------       |
| bar    |                                                  |
|        |  Content                                         |
|        |                                                  |
|        |                                                  |
+--------+-------------------------------------------------+
```

- The **TopBar** holds the workspace switcher, the global
  command palette trigger, and the user menu.
- The **Sidebar** is the primary navigation surface. It is
  always visible on desktop. It is collapsible to icon-only.
- The **Content** region holds one page at a time.

### 2.2 The Page

A page has three vertical regions:

1. **PageHeader** — title, description, primary actions.
2. **Toolbar** (optional) — filters, secondary actions.
3. **Body** — the actual content.

A page never uses a hero section. Marketing-style hero areas are
forbidden.

### 2.3 The Card

A card is a bounded region for a single concept (a metric, a
provider, a session). Cards are arranged in a grid. The grid
adjusts to the available width:

- 4 cards per row at 1600px+
- 3 cards per row at 1280–1599px
- 2 cards per row at 1024–1279px
- 1 card per row below 1024px

Cards are separated by gutter, not by heavy borders.

### 2.4 The Panel

A panel is a fixed or floating region. The sidebar, a settings
drawer, a terminal panel are all panels. Panels are not free-
floating; they are part of the layout.

---

## 3. Density

### 3.1 The Default

The platform uses a **comfortable** density by default. Lists,
rows, and cards have enough breathing room to be read at a glance
on a high-DPI screen.

### 3.2 Compact Mode

A compact density option is available in user settings. Compact
mode reduces row height, padding, and font size by one step. The
option is remembered per user.

### 3.3 Anti-Patterns

- Cards that are 80% white space.
- Headers that span the full width and contain a single icon.
- Empty states that fill the screen.
- Tooltips that explain obvious things.

---

## 4. Color and Contrast

### 4.1 Two Themes

The platform supports a light and a dark theme. Both must pass
WCAG AA contrast checks for text and interactive elements.

### 4.2 Color Roles

| Role       | Use                                    | Token family         |
| ---------- | -------------------------------------- | -------------------- |
| Surface    | Backgrounds                            | `app-bg`, `app-surface`, `app-surface-2` |
| Border     | Hairlines, dividers                    | `app-border`         |
| Text       | Primary, secondary, tertiary           | `app-fg`, `app-fg-muted`, `app-fg-subtle` |
| Accent     | Primary action, focus                  | `app-accent`         |
| Status     | Success, warning, error, info          | `app-success`, `app-warning`, `app-error`, `app-info` |
| Health     | Provider health                        | `health-*`           |

A color used outside its role is a review failure.

### 4.3 Status Is Never Color Alone

Status is communicated by **icon + text + color**. A red dot
without a label is rejected. A green button without text is
rejected.

---

## 5. Typography

### 5.1 Family

A single sans-serif family. Monospace is reserved for code,
terminal output, and values that look like code (provider IDs,
commit SHAs).

### 5.2 Scale

The scale in `docs/design-system.md` is the only source of
truth. Components do not invent new sizes.

### 5.3 Weight

- `400` (regular) for body.
- `500` (medium) for emphasised body and labels.
- `600` (semibold) for headers.
- `700` (bold) is reserved for metrics and large numbers.

### 5.4 Truncation

Text that exceeds its container is truncated with an ellipsis.
Long values get a tooltip on hover. Nothing wraps awkwardly.

---

## 6. Iconography

- A single icon set, used everywhere.
- Icons are sized via `AppIcon` with a `Size` parameter.
- Icons used for status accompany text.
- Icons in buttons are decorative; the button label is the
  accessible name.

---

## 7. Motion

### 7.1 When Motion Is Used

- **State change** — a panel opens, a dialog appears, a toast
  appears.
- **Spatial transition** — moving between routes.

Motion is **never** used to draw attention, decorate, or fill
silence. A still UI is the default.

### 7.2 Duration and Easing

- Most transitions: 120–180ms.
- Easing: `ease-out` for entrances, `ease-in` for exits.
- No bounce, no spring, no overshoot.

### 7.3 Reduced Motion

A user preference for reduced motion is respected globally. When
enabled, transitions are instant.

### 7.4 Loading Motion

- Spinners are the only motion that loops indefinitely.
- Skeletons do not animate. They are static placeholders.
- Progress bars are determinate when possible.

---

## 8. The Four States on Data-Owning Surfaces

The four states (`Loading`, `Empty`, `Error`, `Populated`)
apply to **data-owning surfaces** — surfaces that fetch or
own a data set. Pure primitives (`AppButton`, `AppBadge`,
`AppIcon`, `AppStatusDot`, `AppTooltip`) and presentational
containers (`AppCard`, `AppSection`, `AppDialog`,
`AppDrawer`, `AppTabs`, `AppPanel`, `AppToolbar`) do not
own a data fetch and do not require the four states.

### 8.1 Loading

- A list shows `AppSkeleton` rows until the first result arrives.
- A card shows a skeleton or `AppLoading`.
- Spinners are used only when the operation is short and the
  user is waiting on a button.

The loading state is rendered through a `Loading` slot or a
default `AppLoading` component.

### 8.2 Empty

- An empty list shows `AppEmptyState` with a title, a description,
  and a primary action.
- An empty card shows a contextual message.
- An empty page is rare; most pages have content by design.

The empty state answers "what does this page become when the data
is missing?" and "what can the user do about it?".

### 8.3 Error

- A failed fetch shows `AppErrorState` with the error category
  and a retry action.
- A failed operation shows a toast.
- A fatal error is logged and surfaces a `AppCrashBoundary` that
  preserves user state.

The error state is rendered through an `Error` slot or a default
`AppErrorState` component.

### 8.4 Populated

- The populated state is the default body of a data-owning
  surface. It is rendered through a `Populated` slot or
  the default body rendering.
- A data-owning surface that renders the populated view
  without an explicit `Populated` slot is still valid;
  the slot exists so the page can pass a tailored
  populated view when the default is not enough.

### 8.5 Forbidden States (on data-owning surfaces)

- A blank page with no message.
- An error message in a toast for a primary fetch failure.
- A spinner that never resolves.

---

## 9. Keyboard

### 9.1 Global Shortcuts

| Shortcut          | Action                              |
| ----------------- | ----------------------------------- |
| `Ctrl+K`          | Open the command palette            |
| `Ctrl+B`          | Toggle the sidebar                  |
| `Ctrl+,`          | Open settings                       |
| `Ctrl+Shift+P`    | Open the provider panel             |
| `F1`              | Open the keyboard shortcut help     |
| `Esc`             | Close the active dialog or popover  |

### 9.2 Focus

- The first focusable element on a page receives focus on
  navigation.
- Focus is visible at all times.
- Focus order matches visual order.
- Focus traps are implemented on dialogs and drawers.

### 9.3 ARIA

- All landmarks (`<header>`, `<nav>`, `<main>`, `<aside>`) are
  present.
- All interactive elements have a role and an accessible name.
- All status updates use `aria-live` regions (politely for toasts,
  assertively for errors).

---

## 10. Responsiveness

### 10.1 Breakpoints

| Name        | Min width | Use                                     |
| ----------- | --------- | --------------------------------------- |
| `xl`        | 1600px    | Maximum density (4 columns)             |
| `lg`        | 1440px    | Standard desktop (full sidebar)         |
| `md`        | 1280px    | M2 primary viewport (full sidebar)      |
| `sm`        | 1024px    | Compact desktop (narrow sidebar)        |
| `xs`        | 768px     | Small window, sidebar collapses to icons (icons missing — falls back to label-clipped sidebar) |
| —           | 0         | Below the supported range — show a "this application is desktop-only" message |

The M2.5 slice ships the responsive matrix in `Layouts/AppLayout.razor.css`:

- **≥ 1440px (lg / xl)** — grid columns `14rem 1fr`, sidebar padding 16px, content padding 24px.
- **1280–1439px (md)** — grid columns `10rem 1fr`, sidebar padding 12px, topbar padding 10px 20px, content padding 20px.
- **1024–1279px (sm)** — grid columns `8rem 1fr`, sidebar padding 10px, topbar padding 8px 16px, content padding 16px. Sidebar labels remain visible (icon-rail collapse is a future enhancement once every sidebar route carries an `Icon`).
- **< 1024px** — out of M2 scope per ADR-005; the M8 closeout adds the full mobile / icon-rail matrix.

The top bar remains horizontal at every breakpoint. The content area scrolls vertically (`overflow-y: auto`) at every breakpoint. The sidebar never collapses entirely in M2.5; the layout is usable down to 1024px and degrades gracefully below.

### 10.2 What Does Not Respond

- Dialogs. Dialogs are always centred on the viewport.
- Code. Monospace text does not reflow.
- Tables. Wide tables scroll horizontally within their container.

---

## 11. Empty States as a First-Class Surface

Empty states are the **first impression** of a new workspace. A
new user sees an empty workspace before they see anything else.
The empty state must:

- Explain what the page is for.
- Show one primary action.
- Use an illustration that is on-brand.
- Be fast to render.

The catalogue of empty state illustrations lives in
`wwwroot/images/empty/`. New empty states are added there.

---

## 12. When the UI Principles Conflict

The principles are ordered:

1. **Accessibility** always wins.
2. **Density and clarity** come next.
3. **Visual polish** comes last.

A "beautiful" change that breaks accessibility is rejected. A
"clean" change that hurts density is rejected.
