# Backlog — Ideas

> **Speculative thoughts, rejected alternatives,
> "what if" notes.**
>
> An idea is a thought the team has considered and
> not (yet) committed to. Ideas live here so they
> are not forgotten and so the team can revisit
> them. A `Rejected` idea is **not** a failed
> thought; it is a recorded decision that the
> idea is not the right approach **today**.
>
> See [README.md](./README.md) for the rules.

---

## Format

- **ID** — `I-###`.
- **Title** — a one-line statement of the idea.
- **Status** — `Proposed`, `Accepted`,
  `Deferred`, `Rejected`, or `Done`. (Most
  ideas are `Rejected` or `Deferred`; the
  backlog preserves the thinking.)
- **Source / Traceability** — the document,
  decision, or conversation that introduced
  the idea.
- **Notes** — the contributor's thinking, in
  their own words. The notes are the
  audit trail.
- **Reason for status** — when the idea is
  `Rejected` or `Deferred`, the reason is
  recorded here. The reason is the value of
  the entry.

---

<!-- New ideas are appended below. -->

## I-001 — Vague Provider Family Names

- **Title:** Use vague family names like
  `Assistant`, `Deployment`, and `Internal` for
  provider groupings.
- **Status:** `Rejected`.
- **Source / Traceability:**
  Pre-ADR-012 provider documentation.
- **Notes:** A vague family name does not
  represent a capability. The names were
  "things we built" rather than capabilities
  the platform exposes. A consumer of the
  provider model cannot tell from the family
  name what the provider does.
- **Reason for status:** Rejected in
  [`DECISIONS.md`](./../../DECISIONS.md) ADR-012
  in favour of capability-oriented family
  names (`IAgentRuntimeProvider`,
  `IGitProvider`, `ITerminalProvider`,
  `IWorktreeProvider`, `IQualityGateProvider`,
  `IReviewProvider`, `IAutonomousLoopProvider`,
  `IOrchestrationProvider`).

## I-002 — Single "Kitchen Sink" Provider Project

- **Title:** Put all concrete provider
  implementations in a single project so the
  host references only one provider
  implementation at a time.
- **Status:** `Rejected`.
- **Source / Traceability:** Pre-ADR-011
  multi-project discussion; pre-ADR-016
  composition-root discussion.
- **Notes:** A single project that ships
  every implementation mixes concerns and
  forces every consumer to take a dependency
  on every tool. A typical host run registers
  several providers simultaneously; the
  project-level "at most one" rule is wrong
  on two counts (too restrictive, and
  conflates the host with the composition
  root).
- **Reason for status:** Rejected in
  [`DECISIONS.md`](./../../DECISIONS.md) ADR-016
  in favour of the composition-root rule:
  the host may register any number of
  provider implementations, and only the
  composition root may reference concrete
  provider projects.

## I-003 — Build-Time Selection of Provider Project

- **Title:** Use a build-time selection step
  that swaps `Providers.<X>` projects per
  host run.
- **Status:** `Rejected`.
- **Source / Traceability:** Pre-ADR-016
  composition-root discussion.
- **Notes:** A build-time selection couples
  the build to the runtime and prevents the
  UI from listing the available providers at
  runtime. The selection is the user's
  choice at runtime, not the developer's
  choice at build time.
- **Reason for status:** Rejected in
  [`DECISIONS.md`](./../../DECISIONS.md)
  ADR-016; selection is per-operation at
  runtime through the registry.

## I-004 — `IWorkspaceProvider` Family

- **Title:** Add an `IWorkspaceProvider`
  family for workspace state.
- **Status:** `Rejected`.
- **Source / Traceability:** Pre-ADR-012
  provider documentation.
- **Notes:** Workspace state is
  application-layer state, not a provider
  capability. The workspace store and the
  project records live in the application
  and domain layers. A workspace provider
  is a layering violation: it would
  outsource application-layer state to a
  provider family.
- **Reason for status:** Rejected in
  [`DECISIONS.md`](./../../DECISIONS.md)
  ADR-012; workspace state is
  application-layer state.

## I-005 — Append-Only Design-System Catalogue

- **Title:** Keep the design-system catalogue
  append-only; planned entries are part of
  the public surface.
- **Status:** `Rejected`.
- **Source / Traceability:** Pre-ADR-015
  design-system rule.
- **Notes:** An append-only rule that
  applies to both implemented and planned
  entries treats a planned entry as if it
  were a public surface. A planned entry
  that turns out to be unnecessary cannot
  be removed without an ADR even though no
  code depends on it.
- **Reason for status:** Rejected in
  [`DECISIONS.md`](./../../DECISIONS.md)
  ADR-015 in favour of the
  Implemented/Planned distinction:
  implemented entries are public surface;
  planned entries are revisable.

## I-006 — Absolute Four-State Rule on Every Component

- **Title:** Every component exposes
  `Loading`, `Empty`, `Error`, and
  `Populated` state slots regardless of
  whether it owns a data fetch.
- **Status:** `Rejected`.
- **Source / Traceability:** Pre-ADR-014
  design-system rule.
- **Notes:** Pure primitives and
  presentational containers do not own a
  data fetch and have no business exposing
  data-fetch state slots. Forcing the rule
  on them produces over-engineered
  components that lie about their
  responsibility and pollute the API
  surface.
- **Reason for status:** Rejected in
  [`DECISIONS.md`](./../../DECISIONS.md)
  ADR-014 in favour of the conditional
  four-state rule: data-owning components
  expose the four slots; primitives and
  containers do not.

## I-007 — At Most One `Providers.<X>` Reference in the Host

- **Title:** The host may reference at most
  one `Providers.<X>` project at a time.
- **Status:** `Rejected`.
- **Source / Traceability:** Pre-ADR-016
  composition-root discussion; this was the
  rule stated in early versions of
  [`DECISIONS.md`](./../../DECISIONS.md)
  ADR-011.
- **Notes:** A typical host run registers
  several providers at the same time —
  the agent runtime, the source control,
  the terminals, the worktree, the review,
  and the quality gate. The rule was wrong
  on two counts (too restrictive, and
  conflates the host with the composition
  root).
- **Reason for status:** Rejected in
  [`DECISIONS.md`](./../../DECISIONS.md)
  ADR-016. The composition root may
  register any number of provider
  implementations; only the composition
  root may reference concrete provider
  projects.

## I-008 — Use the Ollama API as a Single `OllamaProvider`

- **Title:** Use a single `OllamaProvider`
  that handles both the Ollama Launch
  process boundary and the Ollama HTTP API
  boundary.
- **Status:** `Rejected`.
- **Source / Traceability:** Pre-ADR-012
  provider documentation.
- **Notes:** The two integrations have
  different transports (process vs HTTP)
  and different failure surfaces. Treating
  them as one provider hides the
  difference and makes the contract
  ambiguous.
- **Reason for status:** Rejected in
  [`DECISIONS.md`](./../../DECISIONS.md)
  ADR-012; the Ollama Launch and the
  Ollama API are separate
  `IAgentRuntimeProvider` implementations,
  not flavours of one provider.

## I-009 — Add Provider Discovery via Reflection

- **Title:** Discover providers at startup
  through reflection rather than through
  DI registration.
- **Status:** `Rejected`.
- **Source / Traceability:** Pre-ADR-016
  composition-root discussion.
- **Notes:** Reflection-based discovery
  hides the registration surface; the
  composition root is invisible; the
  configuration check is bypassed. The
  composition root is the **explicit**
  registration surface, and reflection
  defeats the explicitness.
- **Reason for status:** Rejected in
  [`DECISIONS.md`](./../../DECISIONS.md)
  ADR-016; the composition root is the
  only place that may register concrete
  providers, and registration is explicit.

## I-010 — Speculative `AiEng.Platform.Infrastructure` in M0

- **Title:** Create
  `AiEng.Platform.Infrastructure` in the
  M0 bootstrap so the project is "ready
  for M4-A".
- **Status:** `Rejected`.
- **Source / Traceability:** Pre-ADR-011
  multi-project discussion.
- **Notes:** A project without a consumer
  is a speculative project. The
  infrastructure abstractions
  (`IProcessRunner`, `ICredentialVault`,
  `IClock`) are not consumed by any
  earlier milestone; the M0 bootstrap
  must not create projects that no
  consumer requires.
- **Reason for status:** Rejected in
  [`DECISIONS.md`](./../../DECISIONS.md)
  ADR-011; `AiEng.Platform.Infrastructure`
  is created in M4-A, when the first
  consumer lands.

## I-011 — Use Storybook for the Design System

- **Title:** Migrate the design system to
  Storybook for visual testing and
  documentation.
- **Status:** `Deferred`.
- **Source / Traceability:** [`ROADMAP.md`](./../../ROADMAP.md)
  § 5 ("What Is Intentionally Deferred").
- **Notes:** Storybook would add a
  separate visual testing surface.
  Blazor's bUnit + the `/design-system`
  page already provide component
  documentation and visual review. The
  team may revisit Storybook when the
  design system is large enough to
  benefit from a separate tool, but it is
  not a v1 priority.
- **Reason for status:** Deferred; the
  team's design system tooling is bUnit
  + the `/design-system` page.

## I-012 — Cloud-Hosted Multi-Tenant Version

- **Title:** Offer a cloud-hosted,
  multi-tenant version of the platform
  for teams that want a managed
  deployment.
- **Status:** `Deferred`.
- **Source / Traceability:** [`ROADMAP.md`](./../../ROADMAP.md)
  § 5 ("What Is Intentionally Deferred");
  [`PRODUCT.md`](./../../PRODUCT.md) §
  "What the Product Is Not".
- **Notes:** The platform is a Windows
  application on the developer's machine.
  Cloud hosting is out of scope for v1.
  The architecture does not preclude a
  future cloud-hosted variant when the
  team agrees to expand the scope.
- **Reason for status:** Deferred; the
  scope expansion requires an ADR and
  the user's explicit approval per
  [`AGENTS.md`](./../../AGENTS.md)
  Rule 16.

## I-013 — Mobile / Touch-First UI

- **Title:** Add a mobile or touch-first
  surface for the platform.
- **Status:** `Deferred`.
- **Source / Traceability:**
  [`PRODUCT.md`](./../../PRODUCT.md) §
  "What the Product Is Not";
  [`DECISIONS.md`](./../../DECISIONS.md)
  ADR-005 ("Adopt desktop-first
  responsive design").
- **Notes:** The platform is desktop-first
  by design. Touch and mobile are
  explicitly out of scope. A future
  tablet companion would require a
  separate evaluation.
- **Reason for status:** Deferred; touch
  is out of scope for v1 per ADR-005.

## I-014 — Plugin Marketplace for Third-Party Providers

- **Title:** Add a plugin marketplace
  where third parties can publish
  provider implementations.
- **Status:** `Deferred`.
- **Source / Traceability:**
  [`ROADMAP.md`](./../../ROADMAP.md) § 5.
- **Notes:** A marketplace requires
  signing, distribution, and a review
  process. The architecture supports
  third-party providers through
  loadable assemblies; the marketplace
  is a separate product surface.
- **Reason for status:** Deferred; the
  plugin marketplace is a future
  product surface, not a v1 platform
  feature.

## I-015 — Telemetry and Analytics Beyond Local Diagnostics

- **Title:** Add cross-cutting telemetry
  and analytics beyond local
  diagnostics.
- **Status:** `Deferred`.
- **Source / Traceability:**
  [`ROADMAP.md`](./../../ROADMAP.md) § 5.
- **Notes:** Telemetry is meaningful only
  after a real codebase exists to
  instrument. The local diagnostics
  surface in M8 is the v1 deliverable;
  cross-cutting telemetry is a future
  surface.
- **Reason for status:** Deferred; the
  local diagnostics surface in M8 is
  the v1 deliverable.

---

## Last Updated

- **2026-07-10** — created in the M0.5
  architecture refinement with 15 entries
  derived from the existing ADRs and the
  deferred items in `ROADMAP.md`. Ten are
  `Rejected` (recorded decisions that
  alternatives were considered); five
  are `Deferred` (out-of-scope for v1).
