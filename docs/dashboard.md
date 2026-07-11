# Product Dashboard

> **The operational home page.**
>
> The dashboard is the surface the user sees when
> they open the platform. It is the single page that
> answers the five questions a developer asks at the
> start of every session:
>
> 1. **What is the platform doing right now?**
> 2. **What am I supposed to be working on?**
> 3. **What is the state of the build, the tests,
>    the providers?**
> 4. **What just happened?**
> 5. **What should I look at next?**
>
> The dashboard is **not** the application shell
> (which is the M2 deliverable; the dashboard is a
> page in the shell, the landing page of the
> application). The dashboard is **not** a marketing
> page. The dashboard is the operator's
> instrument panel.
>
> This document is the **definition** of the
> dashboard. The page is implemented in M2 (the
> shell) and M6 (the live data). The sections
> below are the contract the implementation must
> satisfy.

---

## 1. Goals

The dashboard exists to:

- **Orient the user.** A user opening the
  platform sees what is happening and what to
  do next without navigating.
- **Surface the actionable.** Running agents,
  queued tasks, failing builds, unhealthy
  providers, recent reviews, recent gates — the
  user does not have to dig.
- **Persist across restart.** The dashboard's
  state is read from the same stores the rest of
  the application uses. A restart does not lose
  the dashboard's content.
- **Stay honest.** Every number on the dashboard
  is real, current, and traceable to a store. No
  "we'll fill this in later" cells. An empty
  state is an empty state, with a link to
  the action that fills it.

---

## 2. Layout

The dashboard is a **single scrollable page**
broken into the following sections, in this
order. Each section is a named component the
implementation composes; no section is
hard-coded markup.

```
+----------------------------------------------------------+
| Header: title, current time, theme toggle                 |
+----------------------------------------------------------+
| Current milestone + current task (one row)               |
+----------------------------------------------------------+
| Left column (2/3)              | Right column (1/3)       |
|                                |                         |
| - Running agents               | - Providers (status)    |
| - Queued tasks                 | - Recent reviews        |
| - Recent quality gates         | - Latest commits        |
| - Latest commits               | - Build status          |
|                                | - Test status           |
+----------------------------------------------------------+
| Footer: links to design system, settings, help           |
+----------------------------------------------------------+
```

The left column is the work column. The right
column is the platform column. The user reads
left-to-right: "what is happening with my work"
to "what is the platform doing for me".

The exact breakpoint is documented in the
implementation; the dashboard is a single page
on a 1280x720 minimum window (per
[`DECISIONS.md`](./../DECISIONS.md) ADR-005).

---

## 3. Sections

### 3.1 Header

The header is always visible.

| Field             | Source                                                  | Notes                                                                                                            |
| ----------------- | ------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------- |
| Title             | `PRODUCT.md` "Product Name"                            | Static.                                                                                                          |
| Current time      | `IClock.UtcNow` (per `IClock` capability)              | Refreshes every second; uses `IClock` for testability.                                                          |
| Theme toggle      | Theme switcher in `App.razor`                           | Light / dark; the toggle writes `localStorage["app-theme"]` and sets the `data-theme` attribute.                 |

The header is not data-owning; it does not
expose the four state slots. It is a presentational
container.

### 3.2 Current Milestone + Current Task

A single row that names the milestone and the
task the user is working on right now.

| Field                | Source                                                                              | Notes                                                                                                                       |
| -------------------- | ----------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------- |
| Active milestone     | `.ai/state/milestones.json` (the one with `status: "In Progress"`)                  | Name + status. The milestone is rendered through the design system's milestone chip.                                      |
| Active task          | `.ai/state/tasks.json` (the one with `status: "In Progress"`)                       | Title + owner. The task is rendered through the design system's task chip.                                                 |
| Active plan          | `.ai/plans/<name>.md`                                                               | When the active task has a plan, a link to the plan is rendered. The link is a non-modal anchor.                          |
| Last commit          | `git rev-parse HEAD` (through the platform's Git provider)                          | Short hash + subject.                                                                                                       |
| Branch               | `git branch --show-current` (through the platform's Git provider)                  | The branch the user is on.                                                                                                  |

This section is **data-owning**. It exposes
`Loading`, `Empty`, `Error`, `Populated` slots
(per
[`DECISIONS.md`](./../DECISIONS.md) ADR-014 and
[`docs/component-guidelines.md`](./component-guidelines.md)
§ 4). The `Empty` state is the registration
prompt: "register a project to begin" (M3)
or "open the design system to begin" (M2).

### 3.3 Running Agents (left column)

A list of the agents currently running.

| Field             | Source                                                                              | Notes                                                                                                                       |
| ----------------- | ----------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------- |
| Run id            | `IHistoryStore`                                                                     | Stable id; links to the run detail page.                                                                                    |
| Project           | `IProjectService`                                                                   | The project the run is in.                                                                                                  |
| Worktree          | `IWorktreeProviderRegistry`                                                         | The worktree the run is in.                                                                                                  |
| Runtime           | `IAgentRuntimeProviderRegistry` (the resolved runtime)                              | Provider id + display name.                                                                                                  |
| Model             | The model selected at launch (stored on the run record)                             | The model name; e.g. `minimax-m3:cloud`.                                                                                    |
| Status            | `IRunService` (the run's current state)                                            | `queued`, `running`, `succeeded`, `failed`, `cancelled`, `recovered`.                                                       |
| Started at        | `IHistoryStore` (the run's `started_at`)                                            | Relative to now ("3 minutes ago").                                                                                            |
| Output preview    | `IStreamingChannel` (the last 10 lines of stdout)                                  | Truncated; clicking the row opens the run detail page.                                                                       |

A running agent is rendered as an
`AppRunCard`. The list is virtualised (per
[`ARCHITECTURE.md`](./../ARCHITECTURE.md) § 12).

This section is **data-owning**. The `Empty`
state is the prompt to launch an agent.

### 3.4 Queued Tasks (left column)

A list of the tasks waiting to start.

| Field             | Source                                                                              | Notes                                                                                                                       |
| ----------------- | ----------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------- |
| Task id           | `IRunService` (the queued task's id)                                                | Stable id; links to the task detail page.                                                                                    |
| Project           | `IProjectService`                                                                   | The project the task is for.                                                                                                  |
| Wait reason       | `IRunService` (the queue's reason)                                                  | "waiting for worktree", "waiting for runtime health check", "waiting for parallel gate", etc.                              |
| Queued at         | `IHistoryStore` (the task's `queued_at`)                                            | Relative to now.                                                                                                              |

This section is **data-owning**. The `Empty`
state is the absence of queued work — a
positive signal, not a failure.

### 3.5 Recent Quality Gates (left column)

A list of the most recent quality-gate results.

| Field             | Source                                                                              | Notes                                                                                                                       |
| ----------------- | ----------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------- |
| Gate id           | `IHistoryStore`                                                                     | Stable id; links to the gate detail page.                                                                                    |
| Project           | `IProjectService`                                                                   | The project the gate ran against.                                                                                            |
| Provider          | `IQualityGateProviderRegistry` (the resolved provider)                              | Provider id + display name.                                                                                                  |
| Result            | `IQualityGateService` (the gate's pass/fail)                                        | `pass` or `fail`. The result is rendered through `AppQualityGateBadge`.                                                       |
| Run at            | `IHistoryStore` (the gate's `run_at`)                                               | Relative to now.                                                                                                              |

A list of 10 most recent gates is shown. The
list is virtualised.

### 3.6 Latest Commits (left column)

A list of the most recent commits on the
active branch.

| Field             | Source                                                                              | Notes                                                                                                                       |
| ----------------- | ----------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------- |
| Commit hash       | `IGitProvider` (the commit's short hash)                                            | Short hash.                                                                                                                  |
| Subject           | `IGitProvider` (the commit's subject)                                               | First line of the commit message.                                                                                            |
| Author            | `IGitProvider` (the commit's author)                                                | Display name.                                                                                                                  |
| Date              | `IGitProvider` (the commit's date)                                                  | Relative to now.                                                                                                              |
| AI-authored       | The commit's trailer (e.g. `Co-Authored-By: Claude`)                                | A small badge if the commit was authored or co-authored by an AI.                                                            |

A list of 10 most recent commits is shown. The
list is virtualised.

### 3.7 Providers (right column)

A summary of the registered providers by
family.

| Field             | Source                                                                              | Notes                                                                                                                       |
| ----------------- | ----------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------- |
| Family            | `IProviderRegistry` (the family)                                                    | Capability name (e.g. `IAgentRuntimeProvider`).                                                                              |
| Provider id       | The provider's `Id`                                                                  | Stable id.                                                                                                                    |
| Display name      | The provider's `DisplayName`                                                         | Human-readable.                                                                                                              |
| Lifecycle state   | The provider's `Compiled-in` / `Registered` / `Enabled` / `Healthy` / `Selected`    | One state, not cumulative. The state is rendered through `AppHealthDot`.                                                    |
| Last health check | `IProviderHealthService` (the last health-check timestamp)                          | Relative to now.                                                                                                              |

A list of every family is shown, one row per
family, with the providers for that family
listed under it. The list is virtualised.

This section is **data-owning**. The `Empty`
state is "no providers registered" — the
composition root has not yet registered any
providers (M2 state).

### 3.8 Recent Reviews (right column)

A list of the most recent review findings.

| Field             | Source                                                                              | Notes                                                                                                                       |
| ----------------- | ----------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------- |
| Review id         | `IHistoryStore`                                                                     | Stable id; links to the review detail page.                                                                                  |
| Project           | `IProjectService`                                                                   | The project reviewed.                                                                                                        |
| Provider          | `IReviewProviderRegistry` (the resolved provider)                                   | Provider id + display name.                                                                                                  |
| Findings          | `IReviewService` (the review's findings count)                                      | Number of findings; the count is rendered through `AppBadge`.                                                                 |
| Severity          | `IReviewService` (the highest severity)                                             | Highest severity in the review.                                                                                              |
| Run at            | `IHistoryStore` (the review's `run_at`)                                             | Relative to now.                                                                                                              |

A list of 5 most recent reviews is shown. The
list is virtualised.

### 3.9 Build Status (right column)

A single tile showing the build and test
status of the platform itself (the in-app
status; this is **not** the host's tooling
status).

| Field             | Source                                                                              | Notes                                                                                                                       |
| ----------------- | ----------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------- |
| Build status      | The platform's own build status (read from a build artefact or CI feed)             | `passing`, `failing`, `unknown`.                                                                                            |
| Test status       | The platform's own test status                                                       | `passing X / failing Y / skipped Z`.                                                                                          |
| Last build at     | The build's timestamp                                                                | Relative to now.                                                                                                              |
| Branch            | The branch the build ran against                                                     | E.g. `main`, `feature/M2.1-application-shell-skeleton`.                                                                    |

This section is **data-owning**. The `Empty`
state is "no build information available"
when the platform is not yet connected to a
build feed (M2 state).

### 3.10 Footer

A simple row of links.

| Link                          | Target                                                                  |
| ----------------------------- | ----------------------------------------------------------------------- |
| Design system                 | `/design-system` page (M1)                                              |
| Settings                      | `/settings` page (M2 or M7)                                              |
| Help                          | `/help` page (M2 or later)                                               |
| About                         | `/about` page (M2 or later)                                              |

The footer is not data-owning; it is a
presentational container.

---

## 4. Data Sources

The dashboard reads from the following sources.
Each source is an existing capability or service;
the dashboard does not introduce new services.

| Source                | Capability or service                                  | Milestone that delivers it |
| --------------------- | ------------------------------------------------------ | -------------------------- |
| `.ai/state/*.json`    | Project state (canonical)                              | M0.5                       |
| `IProjectService`     | Project entity                                         | M3                         |
| `IWorktreeProviderRegistry` | Worktree listing                                  | M5                         |
| `IRunService`         | Runs, queue, status                                    | M6                         |
| `IHistoryStore`       | Run history, gate history, review history              | M6                         |
| `IAgentRuntimeProviderRegistry` | Resolved runtimes                             | M4-C / M4-D                |
| `IQualityGateProviderRegistry` | Resolved quality gates                         | M4-C / M7                  |
| `IReviewProviderRegistry` | Resolved reviews                                    | M4-C / M7                  |
| `IProviderHealthService` | Provider health                                      | M4-C                       |
| `IProviderRegistry`   | Cross-family provider listing                          | M4-C                       |
| `IGitProvider`        | Branch, commit log                                     | M4-D                       |
| `IClock`              | Current time                                          | M4-A                       |
| `IStreamingChannel`   | Live run output preview                               | M6                         |

The dashboard's dependency on these sources
is **read-only**. The dashboard does not
mutate any state.

---

## 5. State Slots

The dashboard sections are data-owning
components (per
[`docs/component-guidelines.md`](./component-guidelines.md)
§ 4 and
[`DECISIONS.md`](./../DECISIONS.md) ADR-014). Each
section exposes:

- `Loading` — the section is fetching data.
- `Empty` — the section has no data; the empty
  state is the call to action.
- `Error` — the section failed; the error
  state renders the failure with a retry
  affordance.
- `Populated` — the section has data; the
  populated state renders the data.

The four-state rule is conditional on data
ownership per ADR-014. The Header and the
Footer are presentational containers and do
not expose the four slots.

---

## 6. Refresh Cadence

Each section refreshes on a different cadence.
The refresh is driven by a `Timer` or by an
event from the data source; the section does
not poll when the source pushes.

| Section              | Cadence                                                                                |
| -------------------- | -------------------------------------------------------------------------------------- |
| Header (clock)       | Every second.                                                                          |
| Current milestone    | On state change.                                                                       |
| Running agents       | On `IRunService` event; otherwise every 2s.                                            |
| Queued tasks         | On `IRunService` event; otherwise every 5s.                                            |
| Recent quality gates | On `IHistoryStore` event; otherwise every 10s.                                         |
| Latest commits       | On `IGitProvider` event; otherwise every 30s.                                          |
| Providers            | On `IProviderHealthService` event; otherwise every 10s.                                 |
| Recent reviews       | On `IHistoryStore` event; otherwise every 30s.                                         |
| Build status         | On build feed event; otherwise every 60s.                                              |

A section that does not refresh for 5 minutes
shows a "stale" badge in the corner.

---

## 7. Performance

- The dashboard's first paint must occur in
  under 200ms (per
  [`ARCHITECTURE.md`](./../ARCHITECTURE.md) § 12).
- Each section's data fetch is independent;
  sections render in parallel.
- A section that fails does not block the
  others; the failed section renders its
  `Error` state.
- A section's data is cached for the duration
  of the page lifetime; a navigation away and
  back does not re-fetch the data unless the
  data has changed.

---

## 8. Accessibility

The dashboard is keyboard-navigable. Every
interactive element is reachable through `Tab`
and activatable through `Enter` or `Space`.
Every section has a heading that is a focus
target for screen readers. The `Loading` and
`Error` states announce their state to screen
readers through `aria-live="polite"`. The
color choices satisfy WCAG AA contrast in
both light and dark themes.

Per
[`DECISIONS.md`](./../DECISIONS.md) ADR-005 and
[`AGENTS.md`](./../AGENTS.md) Rule 7.

---

## 9. Definition of Done

The dashboard is **Done** when:

- Every section in § 3 is implemented.
- Every section's data source is wired to the
  matching service or registry.
- Every data-owning section exposes the four
  state slots per
  [`docs/component-guidelines.md`](./component-guidelines.md)
  § 4 and ADR-014.
- The dashboard is keyboard-navigable and
  passes the accessibility audit (M8
  deliverable).
- A bUnit test renders the dashboard and
  verifies the four states for every
  data-owning section.
- An architecture test verifies that the
  dashboard does not import a concrete
  provider implementation (per ADR-016).
- The dashboard's first paint occurs in under
  200ms on a developer laptop.

---

## 10. Linked Artefacts

- [`PRODUCT.md`](../PRODUCT.md) — the product
  definition; the dashboard is the surface
  that renders the "Current Delivery Stage"
  section of `PRODUCT.md`.
- [`ROADMAP.md`](../ROADMAP.md) — the milestone
  plan; the dashboard consumes the milestones
  in § 2.
- [`.ai/state/capabilities.json`](./../.ai/state/capabilities.json) —
  the canonical capability graph the dashboard
  reads.
- [`.ai/state/milestones.json`](./../.ai/state/milestones.json) —
  the canonical milestone list the dashboard
  reads.
- [`.ai/state/tasks.json`](./../.ai/state/tasks.json) —
  the canonical task list the dashboard reads.
- [`docs/component-guidelines.md`](./component-guidelines.md) —
  the component authoring rules the dashboard
  follows.
- [`docs/design-system.md`](./design-system.md) —
  the design tokens the dashboard consumes.
- [`docs/ui-principles.md`](./ui-principles.md) —
  the layout, density, and accessibility
  principles the dashboard follows.
- [`ARCHITECTURE.md`](../ARCHITECTURE.md) § 12 —
  the performance principles the dashboard
  follows.
- [`DECISIONS.md`](../DECISIONS.md) ADR-005,
  ADR-014, ADR-016 — the rules the dashboard
  follows.

## 11. Last Updated

- **2026-07-10** — created in the M0.5
  architecture refinement. The dashboard
  definition is the contract M2 implements
  (the shell) and M6 completes (the live
  data). No code ships in this session.
