# Model Classification Rules

> **Deterministic rules the AI session router uses to pick
> a model profile for a phase.** The router reads
> `.ai/model-classification.json` (machine-readable) and
> `.ai/model-classification.md` (this file, human-readable).
> The rules are deterministic; the router does not guess.

---

## 1. Tiers

| Tier       | Profile key | Purpose                                                                                                |
| ---------- | ----------- | ------------------------------------------------------------------------------------------------------ |
| **High**       | `high`      | Architecture, complex planning, security-sensitive work, difficult debugging, cross-project reasoning. |
| **Standard**   | `standard`  | Implementation from approved plan, tests, normal refactoring, routine bug fixes.                       |
| **Economy**    | `economy`   | Reports, handoffs, state projections, JSON receipts, simple documentation.                             |
| **Review**     | `review`    | High-risk diff review, architecture review, security-sensitive review, repeated validation failure.     |
| **Fallback**   | `fallback`  | Selected when the active tier reports usage exhaustion, HTTP 429, or repeated non-zero exit.            |

The router selects the **highest-priority** matching tier in
this order: `high` → `review` → `standard` → `economy` →
`fallback`. `fallback` is selected by the router on
failure, not by classification.

---

## 2. High Tier Triggers

The router selects `high` when **any** of the following is
true:

- The task type is `architecture` or `bootstrap`.
- The plan requires a new project boundary, a new provider
  family contract, a new dependency rule, or a new ADR.
- The plan requires authentication, credential, or
  security-sensitive design.
- The task is the first task of a new milestone.
- A standard-model attempt has failed validation on a
  previous attempt (retry count > 0 + architecture-shaped
  failure).
- The task name or the task's affected paths include
  `Architecture`, `Security`, `Credential`, `Provider`,
  `Registry`, `Composition`, `Boundary`.
- The phase is `plan` and the plan crosses a milestone
  boundary.
- The task requires cross-project reasoning
  (`.ai/state/capabilities.json` cross-references more
  than one milestone).

---

## 3. Standard Tier Triggers

The router selects `standard` when **none** of the High
tier triggers apply and **any** of the following is true:

- The phase is `reconcile` and the active packet is
  malformed (router should repair the packet; this is
  read-then-write, not deep reasoning).
- The phase is `implement` and the approved plan is
  execution-ready.
- The phase is `validate` and the plan requires running
  the full closeout validation suite.
- The phase is `closeout` and the diff summary is
  non-trivial (more than 5 files changed) or includes
  source / test changes.
- The task type is `feature`, `bugfix`, `refactor`,
  `provider`, `testing`, or `release`.
- The task name or the task's affected paths include
  `Component`, `Service`, `Repository`, `Test`, `Refactor`,
  `Fix`, `Implement`, `Migrate`.

---

## 4. Economy Tier Triggers

The router selects `economy` when **none** of the High
or Standard tier triggers apply and **any** of the
following is true:

- The phase is `document` and the deliverable is a
  receipt, a report, a handoff, a state projection, a
  changelog entry, an index entry, or a markdown
  refresh.
- The phase is `closeout` and the diff summary is
  trivial (≤ 5 files changed, no source / test changes,
  only docs / state / receipts).
- The task name or the task's affected paths include
  `Doc`, `Receipt`, `Report`, `Handoff`, `State`, `Index`,
  `Archive`, `Schema`, `Plan`.
- The task is a documentation-only or state-only task.

The economy model **must not** be selected for
`implement`, `validate`, or any phase that modifies
source / test code.

---

## 5. Review Tier Triggers

The router selects `review` when **any** of the following
is true:

- The task is a milestone closeout (a 13-section
  retrospective + a `m<n>` annotated milestone tag is in
  scope).
- The plan requires a high-risk review (architecture
  boundary changed, security-sensitive code changed,
  provider contract changed).
- The Validate phase has reported failure on a previous
  attempt and the failure is not classified as a
  transient flake.
- The user explicitly requested deep review (the
  active packet's `objective` contains "deep review",
  "architecture review", or "high-risk review").
- The task type is `review`.

The review tier is **never** selected by default. A
normal implementation task must not automatically use
the high model for every phase.

---

## 6. Fallback Selection

The router selects `fallback` when:

- The active tier reports `usage_exhausted` or `429`
  on the most recent attempt.
- The active tier has exceeded its `max_retries`.
- The active tier is `disabled: true` and the
  `execution.fail_open_on_validation_failure` is
  `false`.

The router does not select `fallback` for a normal
phase dispatch. `fallback` is a recovery path, not a
preference.

---

## 7. Forbidden Escalations

The router must **not** escalate from a tier to a higher
tier when:

- Documentation wording is difficult.
- A report is long.
- State Markdown needs refreshing.
- A test was flaky once (the router retries; the
  router does not escalate).
- A user asked for "more detail" (more detail is
  delivered by a higher `max_context_files` budget, not
  by a higher tier).

The router **must** escalate from `standard` to `high`
when:

- Implementation repeatedly fails validation (≥ 2
  consecutive failed `validate` phases for the same
  task).
- Architecture ambiguity is discovered (the child
  writes `fallback_recommended: true` in the phase
  receipt).
- The approved plan is materially incomplete (the
  child reports `plan_incomplete: true`).
- Cross-project reasoning is required (the task
  touches more than one milestone's worth of files).

---

## 8. Configuration Override

The user can override the classification at the
command line:

```
.\tools\ai-session-router.ps1 -Command Next -ProfileOverride high
```

`-ProfileOverride` sets the active profile for the next
dispatch only; the override is recorded in the phase
receipt; the override does not persist.

The user can also force a profile for a specific task
in the active packet's `recommended_profile_by_phase`
field. The router prefers the per-phase recommendation
over the classification default.
