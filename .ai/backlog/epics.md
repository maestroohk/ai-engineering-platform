# Backlog — Epics

> **Strategic delivery themes that span many milestones.**
>
> An epic is a multi-quarter, multi-milestone theme
> the platform pursues. An epic is **not** a task and
> **not** a milestone. The roadmap records the
> milestones that deliver the epic; the epic records
> the strategic intent the milestones serve.
>
> See [README.md](./README.md) for the rules.

---

## Format

- **ID** — `E-###`.
- **Title** — a one-line statement of the strategic
  intent.
- **Status** — `Proposed`, `Accepted`, `Deferred`,
  `Rejected`, or `Done`.
- **Source / Traceability** — the document, decision,
  or conversation that introduced the epic.
- **Notes** — the strategic context. The "why" the
  epic exists.
- **Milestones** — the milestones in `ROADMAP.md`
  that deliver the epic. Empty for proposed epics.

---

<!-- New epics are appended below. -->

## E-001 — The Provider Spine

- **Title:** Every external integration in the platform
  is reached through a capability-based provider
  contract and resolved through a registry; no
  external integration is hardcoded in the UI.
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`DECISIONS.md`](./../../DECISIONS.md) ADR-002
  (provider-based integration architecture),
  ADR-012 (capability-oriented provider families),
  ADR-013 (progressive self-dogfooding),
  ADR-016 (composition root). [`ARCHITECTURE.md`](./../../ARCHITECTURE.md)
  § 3.3, § 4, § 4.5. [`PRODUCT.md`](./../../PRODUCT.md)
  § "Capability Map".
- **Notes:** The provider spine is the
  substrate of M4–M8. Every later milestone
  resolves its external integrations through a
  contract and a registry; the architecture
  tests in `AiEng.Platform.ArchitectureTests/`
  fail the build on any direct bypass.
  The spine is the difference between a
  platform that survives and a platform that
  fragments.
- **Milestones:** M4-A, M4-B, M4-C, M4-D, M5,
  M6, M7, M8.

## E-002 — First Useful Coding-Agent Vertical Slice

- **Title:** A developer can register a project,
  pick a worktree, choose a runtime, launch the
  agent, observe the run, cancel mid-stream, and
  see the history — from a single surface.
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`PRODUCT.md`](./../../PRODUCT.md) § "Core User
  Journey", § "Final Product Capabilities" (1–8).
  [`ROADMAP.md`](./../../ROADMAP.md) § M3, § M4-D,
  § M5, § M6.
- **Notes:** The "first useful" moment is the
  first time the platform stops being a
  configuration tool and starts being an
  engineering tool. M6 is the
  end-to-end milestone; M3, M4, M5 are the
  foundation M6 composes.
- **Milestones:** M3, M4-A, M4-B, M4-C, M4-D, M5,
  M6.

## E-003 — Review and Quality Gate Surface

- **Title:** A launched run can be reviewed and
  gated through provider families; the native
  baselines are the default, external tools
  (Lavish Axi, No Mistakes) are opt-in.
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`PRODUCT.md`](./../../PRODUCT.md) § "Final
  Product Capabilities" (9, 10). [`ROADMAP.md`](./../../ROADMAP.md)
  § M7. [`DECISIONS.md`](./../../DECISIONS.md)
  ADR-012 (review and quality-gate families).
- **Notes:** Review and quality gates are the
  two families that benefit most from the
  credential-vault boundary: their secrets are
  user-supplied and must not be in
  configuration. M7 ships the native baselines;
  the external tools are added when the user
  opts in.
- **Milestones:** M7.

## E-004 — Autonomous Loops and Orchestration

- **Title:** The platform can drive bounded
  autonomous loops and coordinate multiple
  agents through provider families; the native
  baselines are the default, external tools
  (GNHF, Firstmate) are opt-in.
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`PRODUCT.md`](./../../PRODUCT.md) § "Final
  Product Capabilities" (13, 14). [`ROADMAP.md`](./../../ROADMAP.md)
  § M8. [`DECISIONS.md`](./../../DECISIONS.md)
  ADR-012 (autonomous-loop and orchestration
  families).
- **Notes:** M8 is the last milestone. The
  remaining families depend on the others;
  the production-hardening work is meaningful
  only after a real codebase exists to
  instrument, profile, and stabilise.
- **Milestones:** M8.

## E-005 — Documentation Architecture

- **Title:** A new contributor can determine what
  the project is, why it exists, where it
  currently is, what is being worked on, and what
  comes next without asking the user.
- **Status:** `Accepted`.
- **Source / Traceability:**
  [`AGENTS.md`](./../../AGENTS.md) § 2, § 6, § 7.
  [`VISION.md`](./../../VISION.md). [`PRODUCT.md`](./../../PRODUCT.md).
  [`.ai/session-start.md`](./../session-start.md).
  M0.5 architecture refinement.
- **Notes:** The documentation architecture is
  the difference between a project that
  scales and a project that fragments. The
  nine-tier hierarchy (Vision → Constitution →
  Architecture → Decisions → Product →
  Roadmap/Delivery → Standards → Operating
  layer → Evidence) is the spine; the live
  state is the most mutable. Every new
  document is placed in the right tier and
  the right file. The M0.5 refinement
  introduced the backlogs, the structured
  state, the decision log, the capability
  mapping, the dashboard definition, the
  self-awareness state, the dogfooding
  evolution, and the architecture review.
- **Milestones:** M0.5, ongoing.

---

## Last Updated

- **2026-07-10** — created in the M0.5 architecture
  refinement with the five accepted epics derived
  from the existing roadmap and ADRs.
