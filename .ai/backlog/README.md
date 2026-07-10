# `.ai/backlog/` — Engineering Backlog

> **The unsorted, unprioritised, unfiltered pool of
> things the team has thought about.**
>
> The backlog is **not** the roadmap. The roadmap
> (`ROADMAP.md` and the master delivery plan) is the
> **ordered, committed** sequence. The backlog is the
> pool the roadmap draws from. An item in the backlog
> is not promised; an item in the roadmap is.
>
> The backlog exists so that thoughts are captured
> without ceremony and are not forgotten. A new item is
> added when a contributor says "we should also…". A
> later session promotes the item to the roadmap when
> the work is approved and sequenced; until then the
> item lives here, unprioritised, unfiltered, and
> revisable.

---

## 1. Why a Backlog

Without a backlog, three things happen:

- Contributors say "we should also do X" in
  conversation and forget it by the next session.
- A new contributor is unable to discover the
  team's latent thinking; the conversation that
  produced it is gone.
- A plan that disagrees with the backlog creates
  surprises; the plan is approved without the team
  noticing the disagreement.

The backlog is the answer to all three. It is the
**read-only, append-only** record of latent thinking.

---

## 2. Item Types

The backlog has four item types, each with its own
file. An item belongs to exactly one type. A
contributor is free to disagree with the type
("this should be a feature, not an idea") and the
item is moved in the same change.

| Type           | File                  | Meaning                                                                              |
| -------------- | --------------------- | ------------------------------------------------------------------------------------ |
| **Epic**       | `epics.md`            | A large delivery theme that spans many milestones. Multi-quarter. Strategic.        |
| **Feature**    | `features.md`         | A user-facing capability the platform must provide. May span milestones.            |
| **Capability** | `capabilities.md`     | A platform abstraction (contract, registry, service) the platform must expose.      |
| **Idea**       | `ideas.md`            | A speculative thought, a rejected alternative, a "what if" that may never ship.    |

The distinctions are operational:

- An **epic** is a strategic theme. The roadmap
  records the milestones that deliver it.
- A **feature** is what the user sees. The
  `PRODUCT.md` final product capabilities list
  is the source of truth for what the user
  expects to see.
- A **capability** is what the platform exposes.
  `ARCHITECTURE.md` and `DECISIONS.md` are the
  source of truth for what the platform is.
- An **idea** is a thought, not a commitment. It
  lives here so it is not forgotten; it does not
  reach the user or the codebase until promoted.

---

## 3. Item Status

Every item has a status. The status is the
contributor's stated confidence in the item.

| Status     | Meaning                                                                                |
| ---------- | -------------------------------------------------------------------------------------- |
| `Proposed` | The item is recorded. No commitment has been made. The default for new items.          |
| `Accepted` | The item has been accepted into the roadmap or the architecture. It will ship.        |
| `Deferred` | The item is on hold. It may be promoted later; it is not rejected.                      |
| `Rejected` | The item is rejected. The reason is recorded.                                          |
| `Done`     | The item has been implemented and is shipped. The plan and report are linked.          |

`Done` items are not removed. The backlog is a
**history** of thinking, not a working surface.
Working items live in the task board
(`.ai/state/task-board.md`).

---

## 4. Item Format

Every item is a self-contained section with:

- **ID** — a stable identifier (`E-001`, `F-001`,
  `C-001`, `I-001`).
- **Title** — a one-line summary.
- **Status** — one of the statuses above.
- **Source / Traceability** — links to the document
  that introduced or implied the item. The traceability
  is what makes the backlog auditable.
- **Notes** — the contributor's thinking, in their own
  words. The notes are not negotiable; they are the
  record.

Items are added in **append order**. Items are
**not** reordered. The order in the file is the order
the item was added; the file is the audit trail.

---

## 5. Promotion to the Roadmap

An item is promoted when the work that delivers it
is sequenced in the roadmap. Promotion follows the
hierarchy:

```
Backlog (proposed)
   → Master delivery plan (planned)
      → Plan (approved)
         → Implementation report (shipped)
            → Backlog (done)
```

A promotion is a **single change** that:

1. Adds the item to the master delivery plan (or
   the roadmap).
2. Updates the backlog item's status to `Accepted`.
3. Cites the new location in the item's Notes.

The reverse flow is also possible: an item in the
roadmap is **demoted** to the backlog when the
work is deferred or rejected. The change is a
single entry with the new status and a citation.

---

## 6. What This Directory Is Not

- **Not a priority list.** The backlog has no
  ordering. Priority is the roadmap's job.
- **Not a task board.** Working tasks live in
  `.ai/state/task-board.md`. The backlog holds
  thinking; the task board holds work.
- **Not a planning surface.** Plans live in
  `.ai/plans/`. The backlog is upstream of plans.
- **Not negotiable.** A disagreement about an
  item is resolved by changing the item's status
  (`Proposed` → `Rejected`, for example), not by
  deleting the entry. The history is the value.

---

## 7. Files in This Directory

| File            | Purpose                                                       |
| --------------- | ------------------------------------------------------------- |
| `README.md`     | This file.                                                    |
| `epics.md`      | Strategic delivery themes.                                    |
| `features.md`   | User-facing capabilities.                                     |
| `capabilities.md` | Platform abstractions (contracts, registries, services).    |
| `ideas.md`      | Speculative thoughts, rejected alternatives, "what if" notes. |

## 8. The Cardinal Rule

> **The backlog is append-only and order-preserving.**
> A new item is added at the bottom. A status
> change is recorded in the item's Notes. An item
> is never deleted; an item is `Rejected` when the
> team disagrees.

The rule exists so that the backlog is a **history
of thinking**, not a curated story. The team's
evolution is the value of the backlog; a curated
backlog loses the evolution.

---

## 9. Linked Artefacts

- [`ROADMAP.md`](./../../ROADMAP.md) — the
  ordered, committed delivery sequence.
- [`.ai/plans/master-delivery-plan.md`](./../../.ai/plans/master-delivery-plan.md) —
  the delivery view of the roadmap.
- [`.ai/state/task-board.md`](./../state/task-board.md) —
  the live work queue (the working surface).
- [`.ai/state/decision-log.md`](./../state/decision-log.md) —
  small in-flight decisions; the backlog is upstream
  of both the decision log and the task board.
- [`PRODUCT.md`](./../../PRODUCT.md) — the source
  of truth for what the user expects to see (the
  features file traces back to `PRODUCT.md`).
- [`ARCHITECTURE.md`](./../../ARCHITECTURE.md) —
  the source of truth for what the platform is
  (the capabilities file traces back to
  `ARCHITECTURE.md`).

## 10. Last Updated

- **2026-07-10** — created in the M0.5 architecture
  refinement. The backlog is empty at the moment of
  creation; the first items are added in the same
  change or in subsequent sessions.
