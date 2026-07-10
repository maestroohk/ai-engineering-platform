# Review Report

> Produced by the AI at the end of a review session. The
> report is the only output of a review; the report does not
> modify code.

---

## Reviewer

- Session:
- Date:
- Diff under review: (commit, branch, or PR)

## Final Recommendation

One of:

- **Approve** — the change is ready to merge.
- **Request changes** — the change is approvable after the
  listed findings are addressed.
- **Block** — the change violates a non-negotiable rule and
  must not merge as-is.

## Findings

Each finding is labelled by severity and dimension. Findings
are ordered most-severe first.

### Blocker

Findings that violate `AGENTS.md` or `ARCHITECTURE.md` or
break a contract. Must be resolved before merge.

- [ ] `B1` — `path/to/file.cs:LL` — short description —
  dimension — rule cited (e.g. "AGENTS.md Rule 8: UI must
  not reference provider implementations")
- [ ] `B2` — ...

### High

Findings that violate a rule in `docs/` that is not
architectural. Must be resolved before merge.

- [ ] `H1` — `path/to/file.cs:LL` — short description —
  dimension — rule cited
- [ ] `H2` — ...

### Medium

Findings that are inconsistent with the surrounding code or
could be simpler. Should be resolved before merge.

- [ ] `M1` — ...
- [ ] `M2` — ...

### Low

Nits, suggestions, or improvements. Can be resolved in a
follow-up.

- [ ] `L1` — ...
- [ ] `L2` — ...

### Praise

Particularly good choices worth calling out. Optional.

- `P1` — ...
- `P2` — ...

## Dimensions Covered

For traceability, the report notes which of the review
dimensions were inspected and which were not.

- [x] Architecture
- [x] DRY / Reuse
- [x] Component / Design system
- [x] Accessibility
- [x] Security
- [x] Tests
- [x] Documentation
- [x] Style and hygiene
- [ ] Other: ...

## Out-of-Scope Observations

Anything noticed during the review that is not a finding
against this diff but is worth a follow-up.

- Observation 1
- Observation 2

## Linked Artefacts

- The diff under review (commit / branch / PR)
- The matching `task-brief.md` (if any)
- The matching `implementation-plan.md` (if any)
- The matching `implementation-report.md` (if any)
