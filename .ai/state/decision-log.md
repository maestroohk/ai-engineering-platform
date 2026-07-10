# Decision Log

> **Small, in-flight decisions the team has made.**
>
> The decision log is for decisions that are not
> architectural. An entry in the log is **not** an
> ADR; it is a record of a smaller choice (a
> folder name, a deferred item, a minor trade-off,
> a temporary compromise) that the team agreed to
> and that future sessions need to know about.
>
> The relationship to `DECISIONS.md` is:
>
> - A small in-flight decision lives here.
> - When the decision becomes architectural
>   (touches more than one folder, sets a rule
>   referenced by `AGENTS.md`, or changes a
>   previously recorded decision), the entry
>   graduates to an ADR. The log entry is marked
>   `Promoted` with the new ADR number.
> - When the decision is reversed, the entry is
>   marked `Superseded` and a new entry records
>   the reversal.
>
> The log is **append-only and order-preserving**.
> Entries are not deleted; they are marked
> `Superseded` or `Promoted`. The order in the file
> is the order the decision was made.

---

## Format

- **ID** — `D-###`.
- **Date** — the date the decision was made.
- **Title** — a one-line statement of the
  decision.
- **Status** — `Active`, `Superseded`, or
  `Promoted`. (`Promoted` entries reference
  the ADR they graduated to; `Superseded`
  entries reference the entry that reversed
  them.)
- **Context** — the situation that required
  the decision.
- **Decision** — what was decided.
- **Consequence** — the trade-off accepted.
- **Promoted to / Superseded by** — the
  ADR or entry that supersedes or absorbs
  this entry.

---

<!-- New entries are appended below. -->

## D-001 — Use the `.ai/` Directory for AI-Operating Layer

- **Date:** 2026-07-10.
- **Title:** The AI operating layer (prompts,
  workflows, templates, state, handoffs, plans,
  backlog) lives under `.ai/` at the repository
  root.
- **Status:** `Active`.
- **Context:** The platform needs an
  AI-collaboration surface that is distinct from
  the constitution (`AGENTS.md`), the
  architecture (`ARCHITECTURE.md`), and the
  standards (`docs/`). A new directory is needed.
- **Decision:** Use `.ai/`. The directory is
  committed to Git, reviewed, and versioned. It
  is **not** the constitution; the documents in
  `.ai/` operationalise the constitution.
- **Consequence:** A new contributor must learn
  to read the precedence hierarchy
  ([`AGENTS.md`](./../../AGENTS.md) § 2.2) and
  the directory map
  ([`.ai/README.md`](./../README.md) § 3). The
  boundary is intentional and is reviewed.
- **Promoted to / Superseded by:** —

## D-002 — Project Name `AiEng.Platform`

- **Date:** 2026-07-10.
- **Title:** The project name is
  `AiEng.Platform` (and the solution file is
  `AiEng.Platform.slnx`).
- **Status:** `Active`.
- **Context:** A long-lived project needs a
  stable, descriptive name. The name is used
  in the solution file, the namespace
  prefixes, the package names, and the
  documentation.
- **Decision:** `AiEng.Platform`. Short forms
  in conversation (`the platform`,
  `AiEng`) are acceptable; the formal name
  is `AiEng.Platform`.
- **Consequence:** A rename is a
  project-founding change and requires an
  ADR with explicit human approval. The
  current name is treated as permanent.
- **Promoted to / Superseded by:** —

## D-003 — Use SLN-X Format for the Solution File

- **Date:** 2026-07-10.
- **Title:** The solution file uses the
  XML-based SLN-X format (`.slnx`).
- **Status:** `Active`.
- **Context:** .NET 10 supports a new XML
  solution file format that is more
  diff-friendly than the legacy
  `.sln` format. The M0 bootstrap must
  pick one.
- **Decision:** Use `.slnx`. The file
  format is human-readable, well-diffable
  in code review, and is supported by
  `dotnet build`, `dotnet test`, and the
  .NET 10 CLI.
- **Consequence:** A consumer that requires
  the legacy `.sln` format (older IDEs,
  older CI tooling) would need to convert
  the file. The team commits to SLN-X for
  the project's lifetime.
- **Promoted to / Superseded by:** —

## D-004 — The Five Provider Lifecycle States

- **Date:** 2026-07-10.
- **Title:** A provider progresses through
  five distinct states during the lifetime
  of a host run: `Compiled-in`,
  `Registered`, `Enabled`, `Healthy`,
  `Selected`.
- **Status:** `Active`.
- **Context:** The platform's health
  poller, configuration UI, and
  diagnostics page all need a shared
  vocabulary for "is this provider in
  the binary", "did the host register
  it", "is the user opted in", "is it
  operational right now", and "did the
  user pick it for the current
  operation". Without a documented
  state model, every consumer invents
  its own.
- **Decision:** Five distinct states.
  The states are not cumulative. A
  provider is exactly one of these
  states at any moment; the next state
  is reached through a specific event
  (compile, register, enable,
  health-check, select).
- **Consequence:** The vocabulary is
  shared by the registry, the health
  poller, the configuration UI, and the
  diagnostics page. The five states are
  the basis of the user-facing
  affordances.
- **Promoted to / Superseded by:**
  [`DECISIONS.md`](./../../DECISIONS.md)
  ADR-016 § 5 (the lifecycle-states
  clause of the composition-root
  rule).

## D-005 — Composition Root Lives in `App/Composition/`

- **Date:** 2026-07-10.
- **Title:** The composition root (the
  set of files in `AiEng.Platform.App`
  that wire concrete provider
  implementations into DI) lives in
  `AiEng.Platform.App/Composition/`.
- **Status:** `Active`.
- **Context:** The composition root
  needs a home. Folders elsewhere in
  `App` (pages, components, services,
  view models, DTOs) are forbidden
  from referencing `Providers.<X>`
  projects. The composition root is
  the only place that may reference
  them.
- **Decision:** `AiEng.Platform.App/Composition/`.
  The folder is created in M4-C
  alongside the registry; the first
  registration extensions live there
  (one per provider family or per
  concrete provider implementation).
- **Consequence:** The four
  composition-root architecture tests
  pin the rule to the folder. The
  tests are registered but disabled
  until M4-D; they activate when the
  first concrete `Providers.<X>`
  project lands.
- **Promoted to / Superseded by:**
  [`DECISIONS.md`](./../../DECISIONS.md)
  ADR-016.

## D-006 — `AppErrorState` and `AppEmptyState` Are Primitives

- **Date:** 2026-07-10.
- **Title:** `AppErrorState` and
  `AppEmptyState` are **primitives**,
  not data-owning components; they do
  not expose the four data-fetching
  state slots.
- **Status:** `Active`.
- **Context:** The M1 design system
  introduces the four state slots
  conditionally on data ownership.
  The slot-rendering components
  themselves (`AppErrorState`,
  `AppEmptyState`) are rendered by
  data-owning components; the
  slot-rendering components do not
  themselves own data.
- **Decision:** `AppErrorState` and
  `AppEmptyState` are primitives.
  Data-owning components compose
  them inside their `Error` and
  `Empty` slots. The primitives
  render whatever the parent gives
  them.
- **Consequence:** The catalogue in
  [`docs/design-system.md`](./../../docs/design-system.md)
  marks both as `Primitive` (not
  `Data-owning`). A reviewer can
  verify the classification.
- **Promoted to / Superseded by:**
  [`DECISIONS.md`](./../../DECISIONS.md)
  ADR-014.

## D-007 — Use `npm` Scripts, Not `dotnet` Build Hooks, for the CSS Pipeline

- **Date:** 2026-07-10.
- **Title:** The Tailwind/PostCSS CSS
  pipeline is run through `npm`
  scripts (`npm run css:build`,
  `npm run css:watch`); the .NET build
  does not invoke the CSS build.
- **Status:** `Active`.
- **Context:** M1.1 introduced the
  Tailwind/PostCSS pipeline. The
  pipeline can be invoked by `npm` or
  by an MSBuild `Exec` task. The
  choice affects the developer
  experience.
- **Decision:** Use `npm` scripts.
  The two scripts are the only
  invocation. `dotnet build` does not
  invoke the CSS build. A future
  release engineering task may add a
  `<Target Name="BuildCss">` to the
  .csproj if the team wants a
  one-command build; that is not
  M1.1.
- **Consequence:** A developer
  working on the CSS must run
  `npm install` once and
  `npm run css:watch` (or
  `css:build` for a one-shot). The
  .NET build assumes the CSS is
  already built. The two-pipeline
  separation is documented in
  `package.json` and the README.
- **Promoted to / Superseded by:** —

## D-008 — `AppToolbar` Example Deferred from M1

- **Date:** 2026-07-10.
- **Title:** The `AppToolbar` component
  ships and is unit-tested in M1, but
  the `/design-system` page does not
  include a Toolbar section. The
  example is deferred to a follow-up
  task.
- **Status:** `Active`.
- **Context:** M1.2 introduces
  `AppToolbar` as a layout
  component. The component is
  complete and tested. The
  `/design-system` page renders 18
  of the 19 M1.2 components; the
  Toolbar example was not added
  before the M1 closeout.
- **Decision:** Defer the example
  to a small follow-up task
  (already in
  [`.ai/state/task-board.md`](./task-board.md)
  as `Ready`). The work is cosmetic
  and can be folded into M2.1.
- **Consequence:** A reviewer may
  notice the missing example; the
  task board makes the follow-up
  discoverable. M1 is closed with
  the gap documented; the gap is
  not a DoD failure.
- **Promoted to / Superseded by:** —

## D-009 — Lavish Axi M1 Review Deferred

- **Date:** 2026-07-10.
- **Title:** The M1 Lavish Axi design-system
  review is deferred; the tool is not
  installed on the host.
- **Status:** `Active`.
- **Context:** M1's dogfooding checkpoint
  (per [`ROADMAP.md`](./../../ROADMAP.md)
  § M1) authorises the development team
  to use Lavish Axi to review UI
  artefacts where practical. The
  session that closes M1 verifies the
  tool is not installed on the host;
  the only artefact on the filesystem
  is `agent-workbench/tools/lavish-axi.md`,
  a spec for an event-bus daemon, not
  a review tool.
- **Decision:** Defer the M1 review
  indefinitely. The team may resume
  dogfooding when Lavish Axi is
  installed or when an alternative
  review tool is available. The
  deferral is recorded in
  [`.ai/reviews/M1-design-system-lavish-axi-review.md`](./../reviews/M1-design-system-lavish-axi-review.md)
  and the `Blocked` section of
  [`.ai/state/task-board.md`](./task-board.md).
- **Consequence:** M1 is closed
  without an external-tool review.
  M2's dogfooding checkpoint
  (Treehouse) is similarly deferred
  until the team has the tool
  available. The deferrals do not
  block M2; the team's external-tool
  dogfooding is a separate
  discipline from platform
  self-dogfooding.
- **Promoted to / Superseded by:** —

## D-010 — The Layer Hierarchy of Documents

- **Date:** 2026-07-10.
- **Title:** The project's documentation
  has a hierarchy: Vision →
  Constitution → Architecture →
  Decisions → Product → Roadmap /
  Delivery → Standards → Operating
  layer → Evidence / History.
- **Status:** `Active`.
- **Context:** The M0.5 architecture
  refinement adds a `VISION.md` to
  the project. The new document is
  the destination; the existing
  documents translate the destination
  into action. The hierarchy must be
  explicit so a new contributor
  knows what to read first.
- **Decision:** The nine-tier
  hierarchy is recorded in
  [`AGENTS.md`](./../../AGENTS.md)
  § 6 (Document Map). The
  hierarchy is also captured in
  [`VISION.md`](./../../VISION.md) § 6
  (Relationship to Other Documents)
  and the M0.5 implementation
  report.
- **Consequence:** A new contributor
  can determine the order to read
  the documents in by following the
  tier numbers. A document that
  contradicts its neighbour is a
  bug; the higher-tier document
  wins.
- **Promoted to / Superseded by:** —

## D-011 — The Backlog Is Distinct from the Roadmap

- **Date:** 2026-07-10.
- **Title:** The backlog
  (`.ai/backlog/`) is **not** the
  roadmap. The backlog is the
  unsorted pool of thoughts; the
  roadmap is the ordered, committed
  sequence. An item in the backlog
  is not promised; an item in the
  roadmap is.
- **Status:** `Active`.
- **Context:** The M0.5 architecture
  refinement introduces a backlog
  for epics, features, capabilities,
  and ideas. The backlog's role
  must be distinct from the
  roadmap's role so the two do not
  become competing sources of
  truth.
- **Decision:** The backlog is
  append-only and order-preserving;
  the roadmap is sequenced. An
  item moves from the backlog to
  the roadmap when the work is
  approved and sequenced. The
  hierarchy is:
  `Backlog (proposed) → Master
  delivery plan (planned) →
  Plan (approved) → Implementation
  report (shipped) → Backlog
  (done)`.
- **Consequence:** The two
  surfaces have different
  mutability rules. A change to
  the roadmap follows the ADR
  process; a change to the
  backlog follows the
  append-only rule.
- **Promoted to / Superseded by:** —

## D-012 — The State Has Two Layers: Human and Machine

- **Date:** 2026-07-10.
- **Title:** The project's state has
  two layers: human-readable
  Markdown
  (`.ai/state/current.md`,
  `.ai/state/task-board.md`,
  `.ai/handoffs/latest.md`) and
  machine-readable JSON
  (`.ai/state/project.json`,
  `.ai/state/milestones.json`,
  `.ai/state/features.json`,
  `.ai/state/capabilities.json`,
  `.ai/state/providers.json`,
  `.ai/state/tasks.json`,
  `.ai/state/session.json`). The
  JSON is the canonical source; the
  Markdown is the human-readable
  projection.
- **Status:** `Active`.
- **Context:** Future tooling
  (CI, the platform itself,
  external automation) needs
  machine-readable access to the
  state. The Markdown files are
  diff-friendly for humans but
  brittle for machines. The M0.5
  refinement introduces a JSON
  layer.
- **Decision:** JSON files are the
  canonical source. Markdown
  files are projections rendered
  from the JSON. When the two
  disagree, the JSON wins; the
  Markdown is regenerated. The
  generation is a tool the team
  may build later; until then, a
  human updates both in the same
  change.
- **Consequence:** Every session
  that updates `current.md` or
  `task-board.md` also updates
  the matching JSON fields. The
  schema for each JSON file is
  recorded in the file's header
  and in
  [`.ai/state/README.md`](./README.md).
- **Promoted to / Superseded by:** —

---

## Last Updated

- **2026-07-10** — created in the M0.5
  architecture refinement with 12 entries
  derived from the M1 closeout, the M0.5
  refinements, and the deferred items
  recorded in the state files. The entries
  are `Active`; entries that have graduated
  to ADRs (D-004, D-005, D-006) cite the
  ADR.
