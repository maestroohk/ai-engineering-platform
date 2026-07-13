# docs/design-system.md

> The design system of the AI Engineering Platform. This document is
> the catalogue of design tokens, semantic classes, and reusable
> components. It is the source of truth for "what does a thing look
> like in this platform?". Read it after `AGENTS.md`,
> `ARCHITECTURE.md`, and `STYLEGUIDE.md`.

**Design system version: 0.2.0** (M1.2 — Design System
Core shipped; 19 components in 5 categories implemented:
7 Primitives, 4 Layout containers, 2 Display, 5 Feedback,
1 Input label). The version bumps to **1.0.0** when the
catalogue has at least one implemented entry in every
category and the public API is stable. Version history is
recorded in `DECISIONS.md` (ADR-015).

---

## 1. Purpose

The design system exists to ensure that the platform looks, behaves,
and feels like **one product** across every page, dialog, and panel.
It is also the contract between the visual language and the
components that implement it.

The system has three layers:

1. **Tokens** — primitive values (color, space, type, radius, shadow).
2. **Semantic classes** — composable utilities that apply tokens
   consistently.
3. **Components** — Blazor components that consume semantic classes
   and expose a small, documented API.

A contributor should never need to write raw CSS to ship a page. If
the design system does not cover a need, **extend the design system
first**, then use the new piece.

---

## 2. Design Tokens

Tokens are defined in `tailwind.config.js`. They are the only place
raw values live.

### 2.1 Color

The platform supports light and dark themes. Token names are
intentionally semantic; the raw value is the design system's concern.

| Token            | Purpose                                | Light      | Dark       |
| ---------------- | -------------------------------------- | ---------- | ---------- |
| `app-bg`         | Application background                 | `#f7f8fa`  | `#0e1014`  |
| `app-surface`    | Cards, panels, dialogs                 | `#ffffff`  | `#171a21`  |
| `app-surface-2`  | Elevated surfaces (popovers, menus)    | `#f1f3f6`  | `#1f232b`  |
| `app-border`     | Hairlines, dividers, card borders      | `#e3e6ec`  | `#262a33`  |
| `app-fg`         | Primary text                           | `#0d1117`  | `#e6e8ee`  |
| `app-fg-muted`   | Secondary text, captions               | `#57606a`  | `#aab0bb`  |
| `app-fg-subtle`  | Tertiary text, placeholders            | `#8a93a3`  | `#7c8493`  |
| `app-accent`     | Brand accent, primary action           | `#3b6cf6`  | `#5b85ff`  |
| `app-accent-fg`  | Foreground on accent                   | `#ffffff`  | `#0e1014`  |
| `app-success`    | Success state                          | `#1f9d55`  | `#3ec97c`  |
| `app-warning`    | Warning state                          | `#c97a16`  | `#f0a754`  |
| `app-error`      | Error state                            | `#cf222e`  | `#ff6b6b`  |
| `app-info`       | Informational state                    | `#1f6feb`  | `#5b9bff`  |

A status dot, badge, or message may use `bg-app-success`, etc. It
must never use a raw color.

### 2.2 Spacing

Tailwind's default 4px scale is used, but components consume it
through semantic classes. Components do not write `p-4` directly;
they apply a class that resolves to `p-4`.

### 2.3 Radius

| Token            | Value     | Use                              |
| ---------------- | --------- | -------------------------------- |
| `app-radius-sm`  | `4px`     | Tags, small buttons              |
| `app-radius`     | `6px`     | Inputs, buttons, cards           |
| `app-radius-lg`  | `10px`    | Dialogs, large panels            |
| `app-radius-xl`  | `16px`    | Modals                           |

### 2.4 Shadow

| Token            | Use                                |
| ---------------- | ---------------------------------- |
| `app-shadow-1`   | Cards, popovers                    |
| `app-shadow-2`   | Dialogs                            |
| `app-shadow-3`   | Modals, drag previews              |

### 2.5 Type

A single sans-serif family is used across the platform. The type
scale:

| Token            | Size / line height    | Use                          |
| ---------------- | --------------------- | ---------------------------- |
| `app-text-xs`    | `11 / 16`             | Captions, badges             |
| `app-text-sm`    | `13 / 18`             | Secondary text               |
| `app-text-base`  | `14 / 20`             | Body                         |
| `app-text-md`    | `15 / 22`             | Emphasised body              |
| `app-text-lg`    | `17 / 24`             | Subheaders                   |
| `app-text-xl`    | `20 / 28`             | Section headers              |
| `app-text-2xl`   | `24 / 32`             | Page headers                 |
| `app-text-3xl`   | `32 / 40`             | Empty states, metrics        |

Font weights: `400` (body), `500` (emphasised), `600` (headers).

---

## 3. Semantic Classes

Semantic classes are defined in `wwwroot/css/design-system/` and
consumed by markup. The most important ones:

### 3.1 Surfaces

- `.app-card` — bordered surface, padding, shadow.
- `.app-section` — content region inside a page.
- `.app-panel` — bordered region with internal padding.
- `.app-divider` — hairline.

### 3.2 Layout

- `.page-container` — top-level page wrapper (padding, max width).
- `.content-panel` — main content area inside the shell.
- `.sidebar-item` — sidebar entry.
- `.sidebar-item-active` — selected sidebar entry.

### 3.3 Status

- `.status-success`, `.status-warning`, `.status-error`,
  `.status-info`, `.status-neutral` — background + foreground.
- `.status-dot` — circular indicator (used by `AppStatusDot`).
- `.health-healthy`, `.health-degraded`, `.health-unhealthy` —
  provider health colours.

### 3.4 Component-Level Classes

Each reusable component has its own semantic class. The component
*is* the API; the class is its internal representation. For example,
`AppButton` renders as `<button class="app-button app-button--primary">`.

The full list lives alongside the component definitions and is
mirrored in `docs/component-guidelines.md`.

---

## 4. Component Catalogue

The catalogue is the **operational source of truth** for the
design system. It distinguishes two kinds of entries, and the
two are governed by different rules:

- **Implemented components** are present in the codebase
  (`AiEng.Platform.App/Components/...`). A rename, a removal,
  or a breaking API change to an implemented component is a
  public-surface change and is recorded as an ADR. The
  component's design-system version (per § 10) is bumped
  when the change lands.
- **Planned components** are entries the team has agreed to
  build at some point, but which are not yet in the codebase.
  A planned entry is a **placeholder**, not a commitment: it
  may be renamed, removed, or re-scoped without an ADR as
  the work that introduces it approaches. A planned entry is
  **not** a promise that the entry will exist; it is a
  signal that the work is on the roadmap.
- An entry that is **neither implemented nor planned** is
  rejected. The catalogue is the design system's surface; an
  entry that does not correspond to either state is noise.

A new component is added to the catalogue as **planned**
when the team agrees it should exist (typically at the
milestone that introduces it). It moves to **implemented**
in the same change that adds the source files under
`AiEng.Platform.App/Components/...`. A planned entry that
is not yet built must not be referenced from a page as if
it existed; an unimplemented component has no slot to bind
to.

### 4.1 Primitives

| Component         | Purpose                                | Status             | Source                                          | Notes                          |
| ----------------- | -------------------------------------- | ------------------ | ----------------------------------------------- | ------------------------------ |
| `AppButton`       | Primary, secondary, ghost, danger      | Implemented (M1.2) | `Components/Primitive/AppButton.razor`          | Sizes: sm, md, lg              |
| `AppIconButton`   | Icon-only button                       | Implemented (M1.2) | `Components/Primitive/AppIconButton.razor`      | Always has an accessible label |
| `AppBadge`        | Small status / count / label           | Implemented (M1.2) | `Components/Primitive/AppBadge.razor`           | Colour from status tokens      |
| `AppStatusDot`    | Coloured dot                           | Implemented (M1.2) | `Components/Primitive/AppStatusDot.razor`       | ARIA-hidden by default         |
| `AppDivider`      | Horizontal hairline                    | Implemented (M1.2) | `Components/Primitive/AppDivider.razor`         | Pure primitive                 |
| `AppStack`        | Flex container (direction, gap, align) | Implemented (M1.2) | `Components/Primitive/AppStack.razor`           | Strongly typed enums           |
| `AppContainer`    | Page-level max-width wrapper           | Implemented (M1.2) | `Components/Primitive/AppContainer.razor`       | Default / Fluid variants       |
| `AppLink`         | Internal link styled as text           | Planned (M1.3)     | (not yet implemented)                           | Used for in-app navigation     |
| `AppIcon`         | Icon wrapper                           | Planned (M1.3)     | (not yet implemented)                           | Wraps the icon set             |
| `AppTooltip`      | Hover/focus tooltip                    | Planned (M1.3)     | (not yet implemented)                           | Wraps another component        |

### 4.2 Containers

| Component         | Purpose                                | Status             | Source                                       | Notes                          |
| ----------------- | -------------------------------------- | ------------------ | -------------------------------------------- | ------------------------------ |
| `AppCard`         | Bordered surface                       | Implemented (M1.2) | `Components/Layout/AppCard.razor`            | Header, body, footer slots     |
| `AppSection`      | Bordered content region                | Implemented (M1.2) | `Components/Layout/AppSection.razor`         | Used within pages              |
| `AppPanel`        | Bordered region with internal padding  | Implemented (M1.2) | `Components/Layout/AppPanel.razor`           | Promoted from M2 to M1.2       |
| `AppToolbar`      | Toolbar                                | Implemented (M1.2) | `Components/Layout/AppToolbar.razor`         | Promoted from M6 to M1.2       |
| `AppDialog`       | Modal dialog                           | Planned (M2)       | (not yet implemented)                        | Focus trap, escape to close    |
| `AppDrawer`       | Side drawer                            | Planned (M2)       | (not yet implemented)                        | Slides from right              |
| `AppTabs`         | Tab strip                              | Planned (M2)       | (not yet implemented)                        | Tabs are routable              |

### 4.3 Feedback

| Component         | Purpose                                | Status             | Source                                            | Notes                          |
| ----------------- | -------------------------------------- | ------------------ | ------------------------------------------------- | ------------------------------ |
| `AppLoading`      | Inline loading indicator               | Implemented (M1.2) | `Components/Feedback/AppLoading.razor`             | Spinner + label                |
| `AppSkeleton`     | Skeleton placeholder                   | Implemented (M1.2) | `Components/Feedback/AppSkeleton.razor`            | For cards, lists, messages     |
| `AppEmptyState`   | Empty state with icon, title, action   | Implemented (M1.2) | `Components/Feedback/AppEmptyState.razor`         | Required on every list page    |
| `AppErrorState`   | Error state with retry                 | Implemented (M1.2) | `Components/Feedback/AppErrorState.razor`         | Renders `ProviderResult.Failure` |
| `AppAlert`        | Inline notification                    | Implemented (M1.2) | `Components/Feedback/AppAlert.razor`               | Information / Success / Warning / Error variants |
| `AppToast`        | Transient notification                 | Planned (M8)       | (not yet implemented)                             | Hosted in `AppToastHost`       |
| `AppProgressBar`  | Determinate progress                   | Planned (M6)       | (not yet implemented)                             | Used by long-running ops       |

### 4.4 Layout & Navigation

| Component         | Purpose                                | Status             | Source                                              | Notes                          |
| ----------------- | -------------------------------------- | ------------------ | --------------------------------------------------- | ------------------------------ |
| `AppPageHeader`   | Page title + actions                   | Implemented (M1.2) | `Components/Display/AppPageHeader.razor`            | Required at the top of a page  |
| `AppAvatar`       | Circular initial avatar                | Implemented (M1.2) | `Components/Display/AppAvatar.razor`                | Sizes: sm, md, lg              |
| `AppLayout`       | Main shell                             | Planned (M2)       | (not yet implemented)                               | Sidebar + topbar + content     |
| `AppEmptyLayout`  | Layout without chrome                  | Planned (M2)       | (not yet implemented)                               | For setup, login               |
| `AppSidebar`      | Primary navigation                     | Planned (M2)       | (not yet implemented)                               | Data-driven from registry      |
| `AppSidebarItem`  | Sidebar entry                          | Planned (M2)       | (not yet implemented)                               | One per route                  |
| `AppTopBar`       | Top bar                                | Planned (M2)       | (not yet implemented)                               | Workspace switcher, profile    |
| `AppBreadcrumb`   | Breadcrumb trail                       | Planned (M2)       | (not yet implemented)                               | Auto-derived from route        |
| `AppCommandBar`   | Search-driven command palette          | Planned (M4)       | (not yet implemented)                               | Opens with `Ctrl+K`            |

### 4.5 Domain

| Component         | Purpose                                | Status   | Notes                          |
| ----------------- | -------------------------------------- | -------- | ------------------------------ |
| `AppMetricCard`   | Single metric with label and trend     | Planned (M4) | For dashboards                 |
| `AppProviderCard` | Provider summary card                  | Planned (M4) | Name, status, capabilities     |
| `AppHealthDot`    | Provider health indicator              | Planned (M4) | Renders `ProviderHealth`       |
| `AppCapabilityList` | Provider capability list             | Implemented (M4-B.2) | Renders `HostCapability[]` from `IHostCapabilitiesService.DetectAsync`; data-owning four-state |
| `AppKeyValueList` | Generic key/value list                | Implemented (M4-B.2) | Used in diagnostics; data-owning four-state |
| `AppProjectCard`  | Project card                           | Planned (M3) | Name, path, last opened        |
| `AppProjectList`  | Virtualised project list               | Planned (M3) | Data-owning (four-state)       |
| `AppWorktreeCard` | Worktree card                          | Planned (M5) | Branch, status, path           |
| `AppWorktreeList` | Virtualised worktree list              | Planned (M5) | Data-owning (four-state)       |
| `AppDiffViewer`   | Diff view                              | Planned (M5) | Side-by-side or inline         |
| `AppTerminalPanel`| Terminal output                        | Planned (M6) | Wraps provider-backed stream   |
| `AppTerminalLine` | Single terminal line                   | Planned (M6) | Colour by stream               |
| `AppRunStatus`    | Run status badge                       | Planned (M6) | Pending, running, succeeded, failed |
| `AppRunHistory`   | Run history list                       | Planned (M6) | Data-owning (four-state)       |
| `AppModelPicker`  | Model selector                         | Planned (M6) | Reads from the runtime's capabilities |
| `AppRuntimePicker`| Runtime selector                       | Planned (M6) | Reads from `IAgentRuntimeProviderRegistry` |
| `AppReviewPanel`  | Review findings panel                  | Planned (M7) | Renders `IReviewProvider` output |
| `AppReviewFindingList` | List of review findings           | Planned (M7) | Data-owning (four-state)       |
| `AppQualityGateBadge` | Quality gate pass/fail badge       | Planned (M7) | Renders `IQualityGateProvider` output |
| `AppProviderSettingsForm` | Provider configuration form   | Planned (M7) | Reads/writes through the registry |
| `AppSecretField`  | Password / API key field               | Planned (M7) | Reveal toggle, paste warning   |
| `AppConnectionTestButton` | Provider connection tester    | Planned (M7) | Calls `Provider.HealthAsync`   |
| `AppToastHost`    | Toast container                        | Planned (M8) | Renders `AppToast`s            |
| `AppDiagnosticDrawer` | Diagnostic drawer                  | Planned (M8) | Surfaces logs and health       |
| `AppCrashBoundary`| Crash boundary                         | Planned (M8) | Preserves user state on crash  |
| `AppAutonomousLoopCard` | Autonomous loop card            | Planned (M8) | Status, scope, iteration count |
| `AppOrchestrationGraph` | Orchestration graph              | Planned (M8) | Renders the task graph         |

The `AppMessageBubble`, `AppMessageList`, `AppPromptInput`,
`AppSessionCard`, `AppTaskCard`, `AppTokenUsage`, `AppTimeline`,
`AppFileTree`, and `AppCommitList` entries that appeared in
the previous catalogue are **removed**; the platform's
session model was not centred on chat. The session model
that lands in M6 (Agent Runtime Launching) is a
`Run`-shaped stream-of-output model, not a chat-shaped
turn-by-turn model, and the M6 milestones will add the
matching components to the catalogue when the work
lands. Renaming a planned entry is permitted; removing a
planned entry that has not been built is permitted. An
entry that has not been built is not a public surface.

### 4.6 Forms

| Component         | Purpose                                | Status             | Source                                          | Notes                          |
| ----------------- | -------------------------------------- | ------------------ | ----------------------------------------------- | ------------------------------ |
| `AppInputLabel`   | Form label                             | Implemented (M1.2) | `Components/Inputs/AppInputLabel.razor`         | `For` (input id) required      |
| `AppTextField`    | Single-line input                      | Planned (M3)       | (not yet implemented)                           | Label, helper, error slots     |
| `AppTextArea`     | Multi-line input                       | Planned (M6)       | (not yet implemented)                           | Auto-resize option             |
| `AppSelect`       | Dropdown                               | Planned (M4)       | (not yet implemented)                           | Keyboard navigable             |
| `AppCheckbox`     | Checkbox                               | Planned (M4)       | (not yet implemented)                           | Indeterminate state supported  |
| `AppSwitch`       | Toggle                                 | Planned (M7)       | (not yet implemented)                           | Used in settings               |
| `AppSecretField`  | Password / API key field               | Planned (M7)       | (not yet implemented)                           | Reveal toggle, paste warning   |
| `AppFormSection`  | Grouped form section                   | Planned (M7)       | (not yet implemented)                           | Header + body                  |

---

## 5. Component Anatomy

Every component in the catalogue follows the same anatomy. This is
the contract between the design system and the consumer.

### 5.1 Files

- `Component.razor` — markup.
- `Component.razor.cs` — behaviour (optional for trivial
  presentation components).
- `Component.razor.css` — scoped CSS, `@apply`-driven.

### 5.2 Variants

Variants are expressed as an enum parameter (`Variant`), never as
a raw CSS class. Example:

```razor
<AppButton Variant="ButtonVariant.Primary" Label="Save" />
```

### 5.3 Sizes

Sizes are expressed as a `Size` parameter. Default size is `Medium`.

### 5.4 State Slots

The four-state rule (`Loading`, `Empty`, `Error`, `Populated`)
is **conditional on data ownership**:

- **Data-owning components** — components that fetch or own
  a data set (a list page, a project list, a run history, a
  session list, a provider list). These components expose
  four child content slots:

  ```razor
  <AppPanel>
      <ChildContent>...</ChildContent>
      <Loading>...</Loading>
      <Empty>...</Empty>
      <Error>...</Error>
      <Populated>...</Populated>
  </AppPanel>
  ```

  If a slot is not provided, the component falls back to
  `AppLoading`, `AppEmptyState`, `AppErrorState`, or the
  default body rendering. A data-owning component without
  the four slots is rejected.

- **Pure primitives** — `AppButton`, `AppBadge`, `AppIcon`,
  `AppStatusDot`, `AppTooltip`. These components are visual
  atoms; they do not fetch data and do not own state. They
  do not expose `Loading`, `Empty`, `Error`, or `Populated`
  slots.

- **Presentational containers** — `AppCard`, `AppSection`,
  `AppDialog`, `AppDrawer`, `AppTabs`, `AppPanel`,
  `AppToolbar`. These components structure markup; they do
  not fetch data. They expose the slots they have
  (`Header`, `Footer`, `Actions`, `Leading`, `Trailing`)
  and render whatever the parent gives them.

The catalogue in § 4 marks each entry as **data-owning**,
**primitive**, or **container** so the rule can be applied
to the right component. A primitive or container that
nevertheless exposes the four state slots is over-engineered
and is rejected. A data-owning component without the four
slots is incomplete and is rejected. Both are caught in
review.

### 5.5 Accessibility

- All components have a role and an accessible name.
- All interactive components are keyboard navigable.
- Focus is visible.
- Colour is never the only signal (icons or text accompany every
  status colour).

---

## 6. Theming

The platform supports a light and a dark theme. The theme is a
CSS variable swap. Components consume tokens; the tokens resolve
to the active theme.

- No component branches on theme.
- No `if (isDark) ...` in markup.
- A user preference is persisted; the OS preference is respected on
  first launch.

---

## 7. Iconography

- A single icon set is used across the platform.
- Icons are sized via `AppIcon` with a `Size` parameter.
- Icons used for status always accompany text. Colour is paired
  with an icon to remain accessible.

---

## 8. Motion

- Motion is short (under 200ms) and purposeful.
- No bounce. No spring. No surprise.
- Reduced-motion preferences are respected globally.
- Loading spinners are the only motion that loops indefinitely.

---

## 9. When to Extend the Design System

If you find yourself wanting to:

- Write `class="bg-... text-... border-... rounded-... p-..."` in a
  component, **stop**. The design system should provide a class.
- Add a new colour, radius, or shadow value, **add it to the
  tokens** and to this document.
- Build a new visual primitive (e.g. a step indicator), **add it to
  the catalogue** before using it.

The design system grows by accumulation, not by accident.

---

## 10. Versioning

The design system has a major version that bumps on:

- Removal of a token or class.
- Breaking change to an **implemented** component's API.
- A visual rebrand.

Minor versions add tokens, classes, and components. Patch
versions fix visual bugs.

The current version is recorded at the top of this file in
the file header. When the version bumps, `DECISIONS.md`
records the change.

**The versioning rule applies to implemented components.**
A planned entry is not a public surface; renaming or
removing a planned entry does not bump the version and
does not require an ADR. An entry that has been built
is an implemented entry; renaming, removing, or changing
its API is a public-surface change and follows the
rules above.

The catalogue is therefore not strictly append-only. New
entries are added when planned or when implemented;
planned entries are revised or removed as the work that
introduces them approaches; implemented entries are
versioned and protected. A change to a planned entry is
part of the planning surface and is reviewed like any
other planning change. A change to an implemented entry
is a public-surface change and goes through the version
bump and (for breaking changes) the ADR process.
