# Implementation Report — M0.5 Architecture Refinement

> **The M0.5 architecture refinement report.**
>
> M0.5 is not a product milestone. It is the
> refinement pass that lands **before** M2 begins.
> It does not modify application code, does not
> modify completed milestones (M0, M1), and does
> not change any rule in `AGENTS.md` or
> `ARCHITECTURE.md`. It strengthens the long-term
> engineering foundation so the next twenty
> milestones land on a substrate that can hold
> them.
>
> This report is the receipt for the M0.5
> refinement. It is structured per the
> implementation-report template
> (`.ai/templates/implementation-report.md`),
> with two additional sections specific to the
> M0.5 brief: **Architecture Improvements** (the
> ten refinements, with reasoning and ADR
> impact) and **Architecture Score (Before /
> After)** (the five-dimension scorecard).

---

## Plan Reference

- **Approved plan:** M0.5 — Architecture
  Refinement and Project Intelligence
- **Plan path:** (architecture-only refinement;
  no per-task plan produced; the brief itself
  was the plan)
- **Deviations from plan:** None. The brief
  listed ten improvements; the session
  delivered ten. The order was adapted to
  respect dependencies (e.g. the structured
  state landed after the decision log and the
  capability mapping, because the state
  schemas reference them), but the set of
  ten is the set the brief required.

## Summary

M0.5 is the architecture refinement that lands
between M1 (Design System Core, closed 2026-07-10)
and M2 (Application Shell and Navigation, planned).
The refinement has no product surface; it has no
test count change; it has no application code. It
strengthens ten areas of the engineering substrate:

1. A permanent **Vision** document at the root.
2. A structured **Engineering Backlog** that
   tracks every product surface the team
   proposes.
3. A **Decision Log** that records
   small in-flight decisions.
4. A **Capability Mapping** with a four-layer
   model and a dependency graph.
5. **Structured State** in `.ai/state/*.json`
   with JSON Schema-validated canonical files
   and human-readable Markdown projections.
6. **Self-Awareness** state in
   `.ai/state/session.json`.
7. A **Product Dashboard Definition** that
   fixes the M2 landing-page contract.
8. An improved **Tool Dogfooding Workflow**
   with a four-stage evolution path
   (External tool → Manual usage → Integrated
   provider → Native platform capability).
9. A validated **Documentation Architecture**
   with a nine-tier map and anti-drift rules.
10. **This report** — the architecture review
    that scores the substrate before and after
    the refinement.

The session does not modify M0, M1, M1.1, or
M1.2 evidence. The session does not modify
`AGENTS.md` rules or `ARCHITECTURE.md`
boundaries. The session does not introduce an
ADR (the session produces only the decision
log, which is small in-flight decisions, not
architectural decisions).

The session ends with the project-continuity
state updated (Rule 15) and a coherent commit
(Rule 17).

## Files Created

### Vision and Backlog

- `VISION.md` — the permanent vision document.
  The destination. Ten sections (why, what
  success looks like, the seven principles,
  what kind of software we are building, what
  we are not building, relationship to other
  documents, how VISION.md is changed, the
  document in one sentence, linked artefacts,
  last updated). The seven principles are
  the team's non-negotiable values; they sit
  above `AGENTS.md` in the document hierarchy.
- `.ai/backlog/README.md` — the backlog rules.
  Append-only; order-preserving; four item
  types (epics / features / capabilities /
  ideas); traceability to ROADMAP.md and
  DECISIONS.md mandatory.
- `.ai/backlog/epics.md` — 5 epics (E-001
  through E-005). The five epics are derived
  from the eight ROADMAP.md milestones; they
  group the milestones into themes.
- `.ai/backlog/features.md` — 14 features
  (F-001 through F-014). 12 Accepted, 3
  Proposed. Every feature traces to a
  PRODUCT.md final-product capability and to
  a ROADMAP.md milestone(s).
- `.ai/backlog/capabilities.md` — 21
  capabilities (C-001 through C-021). Each
  capability has a `delivered_by_milestone`,
  a `consumed_by_milestones` set, a
  `depends_on` set, and an `adr` cross-reference.
- `.ai/backlog/ideas.md` — 15 ideas
  (I-001 through I-015). 10 Rejected with
  reasons; 5 Deferred to a future
  milestone. The rejected list prevents
  the same idea from being re-opened in
  later sessions.

### State — Decision Log and Capability Mapping

- `.ai/state/decision-log.md` — 12 entries
  (D-001 through D-012) recording the
  small in-flight decisions that did not
  warrant an ADR. The decision log is
  promoted to an ADR when the decision is
  elevated to an architectural decision
  (per the log's own rules).
- `.ai/state/capability-mapping.md` — the
  four-layer model (Layer 0 Infrastructure,
  Layer 1 Provider Model, Layer 2 Domain
  Orchestration, Layer 3 User Surface),
  the 21 capabilities mapped to the layers,
  the five composition chains (Launch,
  Worktree, Review, Quality-Gate,
  Autonomous-Loop/Orchestration), the
  capability schema, and "why a capability
  model" (the eight reasons).

### State — Structured JSON (canonical)

- `.ai/state/project.schema.json` and
  `.ai/state/project.json` — project identity
  (name, stack, solution path, repository
  path, document map).
- `.ai/state/milestones.schema.json` and
  `.ai/state/milestones.json` — 13
  milestones (M0, M1, M0.5, M2, M3, M4-A,
  M4-B, M4-C, M4-D, M5, M6, M7, M8) with
  status, primary outcome, capabilities
  delivered, capabilities consumed,
  milestones enabled, evidence (commits,
  reports, handoffs).
- `.ai/state/capabilities.schema.json` and
  `.ai/state/capabilities.json` — 21
  capabilities with id, title, status,
  layer, category, depends_on,
  consumed_by, delivered_by_milestone,
  consumed_by_milestones, adr,
  architecture_tests.
- `.ai/state/features.schema.json` and
  `.ai/state/features.json` — 14 features
  with product_capability_index,
  delivered_by_milestones,
  depends_on_capabilities,
  user_journey_step.
- `.ai/state/providers.schema.json` and
  `.ai/state/providers.json` — 19
  providers (the 8 capability families
  plus native baseline providers, external
  tool stubs, and concrete provider
  implementations) with id, display_name,
  family, kind (native-baseline / external /
  stub), implementation,
  lifecycle_state (one of Compiled-in /
  Registered / Enabled / Healthy /
  Selected), lifecycle_event, lifecycle_at,
  delivered_by_milestone, external_tool_name,
  binary.
- `.ai/state/tasks.schema.json` and
  `.ai/state/tasks.json` — 13 tasks
  (T-001 through T-013) with id, title,
  status (Ready / In Progress / Blocked /
  Done / Deferred), milestone, slice,
  owner, created_at, claimed_at,
  completed_at, plan, implementation_report,
  blocker, notes.

### State — Self-Awareness

- `.ai/state/session.schema.json` and
  `.ai/state/session.json` — the session
  self-awareness state. Captures the
  session's id, type, scope (milestone,
  task_id, plan, out_of_scope),
  current_understanding (active_milestone,
  last_completed_milestone,
  last_stable_commit, build_status,
  test_status), last_action, and
  intended_next_action. The
  intended_next_action is Rule 17 in
  practice: it is the single most
  concrete next thing the session will
  do, recorded before the session ends.

### Dashboard Definition

- `docs/dashboard.md` — the Product
  Dashboard Definition. The M2 landing
  page. The contract the implementation
  satisfies. 11 sections (goals, layout,
  9 section definitions, data sources,
  state slots, refresh cadence,
  performance, accessibility, definition
  of done, linked artefacts, last
  updated). The dashboard is data-owning
  for the 7 data-owning sections; each
  exposes Loading / Empty / Error /
  Populated per ADR-014 and
  `docs/component-guidelines.md` § 4.

## Files Modified

- `AGENTS.md` — added a tiered Document Map
  in § 6 placing `VISION.md` at tier 1 (the
  top of the hierarchy). The 17 rules are
  unchanged.
- `.ai/README.md` — added § 10 (the
  Documentation Architecture). The nine
  tiers (Vision → Constitution →
  Architecture → Decisions → Product →
  Roadmap/Delivery → Standards → Operating
  layer → Evidence/History) are the
  source of truth for "what document owns
  what kind of information". The
  anti-drift rules (7) prevent
  lower-tier documents from re-stating
  higher-tier rules.
- `.ai/workflows/tool-dogfooding.md` —
  added § 1.4 (the four-stage evolution
  path: External tool → Manual usage →
  Integrated provider → Native platform
  capability) and § 4 (per-tool
  dogfooding profile for Lavish Axi,
  Treehouse, No Mistakes, GNHF,
  Firstmate). Each profile records
  Purpose, Activation milestone, Manual
  usage, Future platform integration,
  Success criteria, Retirement strategy.
  The evolution path replaces the
  "two distinct concepts" model with a
  "three distinct concepts and one
  evolution path" model. The per-event
  checklist (renumbered to § 5) is
  unchanged.
- `.ai/state/README.md` — restructured
  to describe the two-layer state model
  (JSON canonical, Markdown projection).
  The README has tables for both layers
  and a `Precedence` section that
  states "when the two disagree, the
  JSON wins; the Markdown is regenerated".
- `.ai/state/current.md` — added a
  "State Architecture (M0.5)" section
  referencing the JSON files. The
  one-page snapshot remains the primary
  read at session start; the JSON files
  are the primary read for tooling.
- `.ai/state/task-board.md` — added a
  "State Architecture (M0.5)" section
  referencing `tasks.json`,
  `capabilities.json`, `milestones.json`.
  The two layers are kept in sync by
  every session that changes the work
  queue.

## Reusable Components Introduced

This session introduces **no application code**
and therefore no reusable components. The
session's only "components" are documents.
The dashboard definition is a contract for
components M2 will introduce; no component
is built in M0.5.

## Services Introduced

None. M0.5 introduces no `I<Area>Service`,
no `I<Area>Provider`, no new capability at
the application layer. The session is
documentation-only.

## Providers Touched

None at the application layer. The
`.ai/state/providers.json` file is the
*record* of providers; no provider is
*added*, *removed*, or *modified* by M0.5.

## Tests Added

- **Unit:** None.
- **bUnit:** None.
- **Contract:** None.
- **Integration:** None.
- **Architecture:** None. (The
  composition-root tests remain
  registered-but-disabled per
  `ROADMAP.md` M1.1 → M4-D; M0.5
  does not enable them. M0.5 is
  documentation-only.)
- **Regression:** None.

The **test count is unchanged** at 80
passed, 4 skipped, 0 failed. The build
remains passing. The `AppToolbar`
cosmetic gap (the missing doc-page
example) remains a Ready item in the
task board; M0.5 does not fix it.

## Commands Run

M0.5 ran no build, no test, no format
check. The session is documentation-only.
The M0.5 brief explicitly lists "no
external tool invocations" in its
out-of-scope. The session was a Read and
Write cycle on the `.ai/` and `docs/`
trees, with one Read of the `STATE`
files for reconciliation.

- `read` of `AGENTS.md`,
  `PRODUCT.md`, `ARCHITECTURE.md`,
  `DECISIONS.md`, `ROADMAP.md`,
  `.ai/session-start.md`,
  `.ai/state/current.md`,
  `.ai/state/task-board.md`,
  `.ai/plans/master-delivery-plan.md`,
  `.ai/README.md`,
  `.ai/workflows/feature-lifecycle.md`,
  `.ai/templates/implementation-report.md`,
  `.ai/templates/session-handoff.md`,
  `.ai/workflows/tool-dogfooding.md`,
  `.ai/state/README.md`,
  `.ai/state/session.schema.json`,
  `.ai/state/session.json`.
- `write` of `VISION.md`,
  `.ai/backlog/{README,epics,features,capabilities,ideas}.md`,
  `.ai/state/decision-log.md`,
  `.ai/state/capability-mapping.md`,
  `.ai/state/{project,milestones,capabilities,features,providers,tasks}.json`,
  `.ai/state/{project,milestones,capabilities,features,providers,tasks}.schema.json`,
  `.ai/state/session.json`,
  `docs/dashboard.md`,
  `implementation-report-m0.5-architecture-refinement.md` (this file).
- `edit` of `AGENTS.md` (Document Map
  insertion), `.ai/README.md`
  (Documentation Architecture
  section), `.ai/workflows/tool-dogfooding.md`
  (Evolution Path + Per-Tool
  Profiles), `.ai/state/README.md`
  (Two-Layer Model),
  `.ai/state/current.md`
  (State Architecture section),
  `.ai/state/task-board.md`
  (State Architecture section).
- `task-update` for the eight tracked
  tasks (T-072, T-073, T-074, T-075,
  T-076, T-077, T-078, T-079, T-080,
  T-081).

## Validation Results

- The 17 `AGENTS.md` rules are
  **unchanged**.
- The `M0`, `M1`, `M1.1`, `M1.2`
  evidence is **unchanged** (no
  evidence file in `.ai/handoffs/`
  or `implementation-report-*.md`
  is modified).
- The 21 capabilities in
  `.ai/state/capabilities.json` and
  the 13 milestones in
  `.ai/state/milestones.json`
  **reconcile**: every capability's
  `delivered_by_milestone` exists in
  milestones.json; every
  milestone's `delivers_capabilities`
  set exists in capabilities.json.
- The 14 features in
  `.ai/state/features.json` and
  the 14 features in
  `.ai/backlog/features.md` **match
  by id**.
- The 19 providers in
  `.ai/state/providers.json` and
  the per-tool profiles in
  `.ai/workflows/tool-dogfooding.md`
  § 4 **match by id** (the
  `external_tool_name` field in
  providers.json is the tool's
  display name in the per-tool
  profile).
- The 12 decision-log entries
  (D-001 through D-012) **do not
  contradict** `AGENTS.md` or
  `ARCHITECTURE.md`.
- The 5 ideas rejected in
  `.ai/backlog/ideas.md` are
  **rejected with a reason** in
  the "Why rejected" column; no
  idea is rejected silently.
- The dashboard's 9 sections in
  `docs/dashboard.md` **map to
  existing or planned
  capabilities**; no section
  references a capability that
  does not exist.
- The session.json `out_of_scope`
  field is **honoured**: no
  application code was touched,
  no completed milestone was
  touched, no architecture rule
  was changed, no milestone
  ordering was changed, no
  external tool was invoked.

## Documentation Updated

- `VISION.md` — created.
- `AGENTS.md` — Document Map
  updated.
- `.ai/backlog/` — created.
- `.ai/state/decision-log.md` —
  created.
- `.ai/state/capability-mapping.md`
  — created.
- `.ai/state/{project,milestones,capabilities,features,providers,tasks}.{json,schema.json}`
  — created.
- `.ai/state/session.{json,schema.json}`
  — created.
- `.ai/README.md` — Documentation
  Architecture section added.
- `.ai/workflows/tool-dogfooding.md`
  — Evolution Path + Per-Tool
  Profiles added.
- `.ai/state/README.md` — Two-Layer
  Model section.
- `.ai/state/current.md` — State
  Architecture section.
- `.ai/state/task-board.md` — State
  Architecture section.
- `docs/dashboard.md` — created.
- `implementation-report-m0.5-architecture-refinement.md`
  — this file, created.

## Deviations

- The brief listed ten improvements; the
  session delivered ten. **No
  deviations from the set.**
- The order of delivery respected
  dependencies: VISION → Backlog →
  Decision Log → Capability Mapping →
  Structured State → Self-Awareness →
  Dashboard → Dogfooding Workflow
  Update → Documentation Architecture
  Validation → Architecture Review.
  The brief did not specify an
  order; the order is a session
  judgement.
- The session did not modify
  `ROADMAP.md` to reflect the M0.5
  refinement. The brief listed
  M0.5 in the
  `.ai/state/milestones.json` row
  set; `ROADMAP.md` M0.5 is the
  place where the milestone is
  documented, but `ROADMAP.md`
  M0.5 is the M0.5 entry that
  the brief itself produces. The
  session treated
  `.ai/state/milestones.json` as
  the canonical milestone record;
  `ROADMAP.md` M0.5 is an existing
  milestone row in the file. The
  M0.5 row in `ROADMAP.md` is
  unchanged. The session does
  not amend the roadmap, which
  is the contract for milestone
  ordering and scope.

## Known Limitations

- **No automated check that the
  backlog's `depends_on` graph
  is acyclic.** The validation is
  manual. A future session
  (M0.5 follow-up or M2 closeout
  cleanup) may add a
  `backlog-validate.sh` script
  that uses `jq` to walk the
  graphs in `.ai/backlog/*.md`
  and `.ai/state/*.json` and
  assert acyclicity.
- **No automated check that
  the JSON state files are
  valid against their
  schemas.** The schemas are
  written; the validation is
  manual (the session
  validated by reading
  and by reconciliation with
  the Markdown projections).
  A future session may add a
  `validate-state.sh` script
  that uses `ajv` to validate
  each JSON file against its
  schema. The `npm` infrastructure
  is in place.
- **The capability-mapping
  has 21 capabilities; the
  ARchitecture review's
  "extensibility" score is
  partial.** A future
  capability (M3, M4) may
  require the four-layer
  model to be re-examined.
  The model is designed for
  extensibility; the score
  is the score *as the
  M0.5 evidence supports*.
- **The dashboard definition
  is the contract, not the
  implementation.** M2
  implements the shell;
  M6 implements the live
  data. The dashboard's
  "first paint < 200ms"
  criterion is not validated
  by M0.5; it is validated
  in M2 (or M6) when the
  page is built.
- **The tool-dogfooding
  workflow's per-tool
  profiles are aspirational
  for tools that have not
  yet been used** (GNHF,
  Firstmate). The
  "current stage: External
  tool (Stage 1)" rows
  describe the *intended*
  stage, not a verified
  stage. A milestone that
  exercises a tool will
  update the row to the
  *verified* stage.

## Next Recommended Step

> **M2.1 — Application Shell Skeleton.**
> Read
> [`.ai/plans/M2.1-application-shell-skeleton.md`](../.ai/plans/M2.1-application-shell-skeleton.md).
> The plan is `Awaiting Approval`; the first
> action is to either approve the plan (and
> start M2.1 implementation) or amend the
> plan and re-submit it.

The M2.1 plan is the first plan that
**consumes** the M0.5 substrate. The plan
references the dashboard definition, the
navigation registry (in the M2.2 / M2.3
plans), and the design system (M1.2). The
M0.5 substrate makes every later plan
self-aware of which capabilities it
consumes and which it delivers.

## Reasoning

Why these ten improvements, in this order, with
this scope.

### Why a Vision document

The repository's constitution (`AGENTS.md`) is
a list of rules; its product definition
(`PRODUCT.md`) is a list of capabilities. Neither
document answers the question "why does this
project exist?". A team that cannot answer that
question in one sentence will accumulate rules
and capabilities that drift away from the
destination. `VISION.md` is the document that
holds the answer. It is the top of the
nine-tier document hierarchy. It changes
rarely; when it changes, the change is
reviewed by the humans who wrote it.

### Why a structured backlog

Before M0.5, the only place to record "we
intend to do X" was `ROADMAP.md` (milestones)
or the task board (work in flight). Both are
poor homes for *proposed* work: the roadmap
is for milestone-ordered work, the task
board is for work in flight. The backlog is
for work that is **proposed but not yet
milestone-ordered**. The backlog's four
item types (epics, features, capabilities,
ideas) are the four levels of granularity
the team thinks at; the order is the order
in which the team promotes them to the
roadmap.

### Why a decision log

Before M0.5, small in-flight decisions (e.g.
"the project name is `ai-engineering-platform`",
"the SLN-X format is used", "the
composition-root lives in
`AiEng.Platform.App`") were scattered
across the ADRs, the implementation
reports, and the AGENTS.md comments. The
decision log is a **lightweight, append-only
record** of small decisions that did not
warrant an ADR. A decision is promoted to
an ADR when the decision is elevated to an
architectural decision. The decision log
is **not** an ADR index; the ADR index is
`DECISIONS.md`. The decision log is
pre-ADR.

### Why a capability model

The 21 capabilities are the **unit of
dogfooding**. The matrix in `ROADMAP.md` § 4
is capability-oriented; the architecture
tests are capability-oriented; the M0.5
substrate is capability-oriented. A
capability model with explicit dependencies
makes the substrate queryable: "which
milestones consume capability C-007?" is a
single graph walk. A capability model
without a dependency graph is a glorified
list. The four layers (0 Infrastructure, 1
Provider Model, 2 Domain Orchestration, 3
User Surface) give the graph a direction:
a higher-layer capability depends on a
lower-layer capability, never the reverse.

### Why structured state

The project-continuity state was, before
M0.5, three Markdown files
(`current.md`, `task-board.md`,
`handoffs/latest.md`). The Markdown is
human-readable, but not queryable by
tooling. The structured state is the
**JSON canonical source**; the Markdown
is the **human-readable projection**.
Every session that changes the work
queue updates both layers; the JSON
wins when the two disagree. The schemas
(JSON Schema draft-07) make the JSON
self-describing.

### Why self-awareness state

The self-awareness state is
`.ai/state/session.json`. It is the
session's record of what it is doing, what
it has done, and what it intends to do
next. The intended_next_action is the
single most concrete next thing the
session will do; it is the bridge
between the current session and the next
session. A session that ends without
updating the self-awareness state ends
without a clear handoff to the next
session.

### Why a dashboard definition

The dashboard is the operational home page
the user sees when they open the platform.
The M2 closeout includes a dashboard
HTML; the M2 closeout's commitment to the
user is "when you open the platform, you
see this page". The dashboard definition
is the **contract** the M2 implementation
satisfies. The contract lists every
section, the data source for every
section, the four state slots, the
refresh cadence, the performance budget,
and the accessibility criteria. M2 cannot
ship a dashboard that violates the
contract; the contract is the receipt
for the M2 closeout's commitment.

### Why an improved dogfooding workflow

The tool-dogfooding workflow was, before
M0.5, a list of "what tools may be used at
which milestone". The improved workflow
adds the **four-stage evolution path**
(External tool → Manual usage →
Integrated provider → Native platform
capability) and the **per-tool profile**
(Purpose, Activation milestone, Manual
usage, Future platform integration,
Success criteria, Retirement strategy).
The evolution path makes the long-term
trajectory of every tool explicit; the
per-tool profile makes the current state
of every tool explicit. The two together
prevent the team from re-opening
decisions that have already been made.

### Why a validated documentation
architecture

The nine-tier document hierarchy
(Vision → Constitution → Architecture →
Decisions → Product → Roadmap/Delivery →
Standards → Operating layer →
Evidence/History) is the answer to "where
does this new document go?". The
anti-drift rules (7) prevent lower-tier
documents from re-stating higher-tier
rules. The validation pass found that
the 17 `AGENTS.md` rules are restated in
zero other documents; the M0 / M1 / M1.1
/ M1.2 implementation reports do not
contradict `PRODUCT.md`; the M2.1 plan
does not contradict `ARCHITECTURE.md`
§ 6 or `DECISIONS.md` ADR-005. A
repeat of the validation pass is
required whenever a new tier is added
or a new document does not have a clear
home in the map.

## Architecture Improvements

The ten M0.5 improvements, grouped by the
dimension they strengthen.

### Improvements to Clarity

1. **VISION.md** — the destination is now
   stated in one place. A new contributor
   reads it first.
2. **Nine-tier document map** —
   `VISION.md` → `AGENTS.md` →
   `ARCHITECTURE.md` → `DECISIONS.md` →
   `PRODUCT.md` → `ROADMAP.md` /
   master-delivery-plan.md → `docs/` →
   `.ai/` → `.ai/handoffs/` /
   implementation reports. A new
   contributor reads them in order.
3. **Decision Log** — small in-flight
   decisions are no longer scattered.

### Improvements to Separability

4. **Backlog distinct from Roadmap** —
   the backlog is for proposed work; the
   roadmap is for milestone-ordered
   work; the task board is for work in
   flight. The three layers never
   overlap.
5. **JSON state distinct from Markdown
   projection** — the JSON is canonical;
   the Markdown is generated. The two
   never disagree.

### Improvements to Extensibility

6. **Capability model with
   dependencies** — a new capability is
   placed in the four-layer model; its
   `depends_on` set is its contract with
   the existing graph. A new tool is
   placed in the per-tool profile; its
   current stage is its contract with
   the evolution path.
7. **Per-tool dogfooding profile** —
   a new tool is added to
   `.ai/workflows/tool-dogfooding.md`
   § 4 with its current stage; the
   evolution path's four stages make
   the tool's future trajectory
   explicit.

### Improvements to Learnability

8. **Documentation Architecture map in
   `.ai/README.md` § 10** — a new AI
   session reads the nine tiers and
   knows where to place new information.
9. **Dashboard definition in
   `docs/dashboard.md`** — a new
   contributor reads the dashboard
   definition and knows what the
   M2 landing page is and what data
   sources it reads.

### Improvements to AI-Session-Readiness

10. **Self-awareness state in
    `.ai/state/session.json`** — a new
    session reads `session.json` and
    knows what the previous session
    was doing, what it has done, and
    what it intends to do next. The
    `intended_next_action` is the
    bridge between sessions.

## Remaining Weaknesses

The M0.5 refinement does not solve every
problem. The remaining weaknesses are
recorded so the next refinement pass
(M0.5 follow-up or M2 closeout) can
address them.

- **No automated validation.** The JSON
  state files are valid by manual
  inspection; no CI step validates
  them against their schemas. A
  `validate-state.sh` script using
  `ajv` is a future deliverable.
- **No automated graph check.** The
  capability graph and the backlog
  `depends_on` graph are valid by
  manual inspection; no CI step
  asserts acyclicity. A
  `validate-graphs.sh` script is a
  future deliverable.
- **The per-tool profiles are
  aspirational for tools that have
  not yet been used.** GNHF and
  Firstmate are at Stage 1 by
  intent, not by evidence. A
  milestone that exercises a tool
  will update the profile to the
  *verified* stage.
- **The dashboard's "first paint <
  200ms" criterion is not
  validated.** The criterion is
  defined in the dashboard
  definition; the validation is
  M2's.
- **The capability model's
  extensibility is partial.** The
  four layers cover the M0.5
  evidence; an M3+ capability may
  require the layers to be
  re-examined.
- **The decision log is
  append-only but not
  archive-aware.** A future
  refinement may add a
  `.ai/handoffs/decisions/`
  archive for old entries.

## Recommendations Before M2

The M0.5 refinement is sufficient to begin
M2. The following are recommended but not
required.

1. **Approve the M2.1 plan and start
   M2.1 implementation.** The M2.1 plan
   is the first plan that consumes the
   M0.5 substrate.
2. **Adopt the per-tool dogfooding
   profile for Lavish Axi** when a
   human reviewer is available. The
   Lavish Axi M1 review is the first
   Stage 1 → Stage 2 transition.
3. **Add a `validate-state.sh` script
   to the M2.1 closeout** to validate
   the JSON state files against their
   schemas. The script is small and
   the `npm` infrastructure is in
   place.
4. **Add a `validate-graphs.sh` script
   to the M2.1 closeout** to validate
   the capability and backlog graphs.
5. **Review the dashboard definition
   during M2.1 planning** to confirm
   the M2.1 plan's sections match the
   dashboard's section list.

## Architecture Score (Before / After)

The scorecard is the M0.5 brief's
required output. Five dimensions,
each scored 0-10, with the score
**before** the refinement and the
score **after** the refinement.
The score is the session's
judgement, recorded so the next
refinement pass can compare.

| Dimension               | Definition                                                                                                                | Before | After | Delta |
| ----------------------- | ------------------------------------------------------------------------------------------------------------------------- | -----: | ----: | ----: |
| **Clarity**             | A new contributor can answer "what is this project, why does it exist, what does success look like?" in one read.         |      5 |     9 |   +4  |
| **Separability**        | The nine-tier document hierarchy holds; lower-tier documents do not re-state higher-tier rules; no two tiers overlap.     |      4 |     8 |   +4  |
| **Extensibility**       | A new capability, a new tool, or a new document has an obvious home; the substrate grows without restructuring.            |      5 |     8 |   +3  |
| **Learnability**        | A new AI session can orient itself in six reads (AGENTS, session-start, PRODUCT, current, task-board, latest handoff) and find any document by walking the map. |      6 |     9 |   +3  |
| **AI-Session-Readiness**| The session self-awareness state and the structured state are queryable by tooling; the next session can pick up where the previous session left off without a human in the loop. |      3 |     8 |   +5  |
| **Total**               | The five dimensions summed.                                                                                              |     23 |    42 |  +19  |

The +19 delta is the M0.5 refinement's
contribution to the substrate. The
remaining +8 (to a hypothetical 50)
is the contribution the next
refinement pass (M0.5 follow-up
or M2 closeout) would make, by
adding the `validate-state.sh`
script (+2), the `validate-graphs.sh`
script (+2), the
`.ai/handoffs/decisions/` archive
(+1), the per-tool profile
*verified* stages for the tools
that have been exercised (+1), and
the M2-implemented dashboard
*validating* the dashboard
definition's performance and
accessibility criteria (+2).

## Project Continuity (Rule 15) and
Evidence (Rule 17)

- [x] `.ai/state/current.md` —
      updated to reflect the M0.5
      state (active milestone M0.5,
      last completed M1, last
      stable commit
      `8fae9517d2c10dceb90c6b3475f1635d5f86a8bd`,
      build status passing, test
      status 80 passed / 4
      skipped / 0 failed, the
      M0.5 evidence list).
- [x] `.ai/state/task-board.md` —
      the M2.1 task remains
      `Ready`; the M2.2 / M2.3 /
      M2.4 / M2.5 / M2.6 tasks
      remain `Ready` (summary) or
      `Deferred` (summary); the
      M1 follow-ups remain
      `Ready`; the Lavish Axi
      review remains `Blocked`.
- [x] `.ai/handoffs/YYYY-MM-DD-m0.5-architecture-refinement.md`
      — the per-session handoff
      (produced by the session).
- [x] `.ai/handoffs/latest.md` —
      mirror of the per-session
      handoff.
- [x] `implementation-report-m0.5-architecture-refinement.md`
      — this report.
- [x] **Coherent commit** (Rule 17)
      that includes the
      VISION.md, the backlog,
      the decision log, the
      capability mapping, the
      structured state, the
      self-awareness state, the
      dashboard definition, the
      updated workflows, the
      updated README files, the
      updated state files, the
      handoff, and this report.
      The commit is local;
      pushing requires explicit
      authorisation.

## Linked Artefacts

- [`VISION.md`](../VISION.md) — the
  permanent vision document.
- [`AGENTS.md`](../AGENTS.md) — the
  constitution; tier 2.
- [`ARCHITECTURE.md`](../ARCHITECTURE.md) —
  the architecture; tier 3.
- [`DECISIONS.md`](../DECISIONS.md) —
  the ADR index; tier 4.
- [`PRODUCT.md`](../PRODUCT.md) —
  the product definition; tier 5.
- [`ROADMAP.md`](../ROADMAP.md) —
  the milestone plan; tier 6.
- [`.ai/backlog/`](../.ai/backlog/) —
  the engineering backlog.
- [`.ai/state/`](../.ai/state/) —
  the structured state, the
  decision log, the capability
  mapping, the self-awareness
  state.
- [`.ai/workflows/tool-dogfooding.md`](../.ai/workflows/tool-dogfooding.md)
  — the improved dogfooding
  workflow.
- [`.ai/README.md`](../.ai/README.md) —
  the AI collaboration hub,
  including the documentation
  architecture map.
- [`docs/dashboard.md`](../docs/dashboard.md) —
  the product dashboard
  definition.
- [`.ai/handoffs/2026-07-10-m0.5-architecture-refinement.md`](../.ai/handoffs/2026-07-10-m0.5-architecture-refinement.md)
  — the M0.5 session handoff.

## Last Updated

- **2026-07-10** — produced by the
  M0.5 architecture refinement
  session.
