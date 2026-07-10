# Implementation Report — `.ai/` Migration

> Produced using `.ai/templates/implementation-report.md` as
> part of the `.ai/` directory introduction. This report is
> the only output of the migration session.

---

## Summary

Introduced the canonical `.ai/` directory as the **only
operational location for AI-assisted development workflows**.
The existing top-level `/prompts/` directory was migrated to
`.ai/prompts/` and expanded from 4 to 10 task-type templates.
Six multi-step workflows and five reusable templates were
added. `AGENTS.md` was updated to adopt the 11-step AI
session operational sequence and the new precedence
hierarchy. The change is documentation-only; no application
source code, packages, or external tools were involved.

The migration advances the M0 milestone (foundation) and
records three new ADRs' worth of decisions implicitly through
the new precedence hierarchy, the dogfooding workflow, and
the redesigned AI session sequence. No `DECISIONS.md` entry
was required: the new content operationalises existing rules
rather than changing them.

## Files Created

### `.ai/`

- `.ai/README.md` — collaboration hub; precedence hierarchy;
  task routing table; no-secrets policy.
- `.ai/session-start.md` — first file an AI reads after
  `AGENTS.md`; 11-step operational sequence; prohibited
  shortcuts.

### `.ai/prompts/` (10)

- `.ai/prompts/bootstrap.md` — milestone / project / provider
  family creation. Improves the previous bootstrap prompt
  with the new precedence statement and explicit ADR-driven
  flow.
- `.ai/prompts/feature.md` — end-to-end feature work. Improves
  the previous feature prompt with the precedence statement
  and a link to the matching workflow.
- `.ai/prompts/bugfix.md` — diagnose → fix → regression test.
  Improves the previous bugfix prompt with the precedence
  statement.
- `.ai/prompts/refactor.md` — behaviour-preserving refactor.
  Improves the previous refactor prompt with the precedence
  statement and the documentation-update workflow.
- `.ai/prompts/review.md` — **new**. Severity-labelled review
  across architecture, DRY, components, accessibility,
  security, tests, documentation, style.
- `.ai/prompts/architecture.md` — **new**. ADR-driven
  architectural change with trade-off analysis, affected
  layers, migration plan, optional demonstrator.
- `.ai/prompts/ui.md` — **new**. Component-first UI work
  with four-state coverage, keyboard, themes, responsive,
  visual review.
- `.ai/prompts/testing.md` — **new**. Test pyramid,
  contract tests, architecture tests, regression tests,
  coverage as a diagnostic.
- `.ai/prompts/provider.md` — **new**. Provider onboarding
  through the contract model: contract before implementation,
  fake, real adapter, health, conformance tests, controlled
  enablement.
- `.ai/prompts/release.md` — **new**. Release validation
  through the full checklist; no automation in this session.

### `.ai/workflows/` (6)

- `.ai/workflows/feature-lifecycle.md` — discovery → plan →
  approval → impl → tests → docs → review → report.
- `.ai/workflows/ui-design-review.md` — visual review with
  the four-state checklist; design-system update closure.
- `.ai/workflows/provider-onboarding.md` — capability →
  contract → fake → real → health → tests → docs →
  enablement.
- `.ai/workflows/tool-dogfooding.md` — distinguishes
  development-time dogfooding from product integration; staged
  dogfooding plan for Lavish Axi (M1/M2), Treehouse (M2),
  No Mistakes (M3), GNHF (M5), Firstmate (later).
- `.ai/workflows/documentation-update.md` — which documents
  change for which kind of change.
- `.ai/workflows/release-checklist.md` — full release
  procedure; no automation implemented.

### `.ai/templates/` (5)

- `.ai/templates/task-brief.md` — human's task
  specification.
- `.ai/templates/implementation-plan.md` — AI's plan
  produced before implementation.
- `.ai/templates/implementation-report.md` — AI's
  completion record.
- `.ai/templates/review-report.md` — structured code
  review output.
- `.ai/templates/session-handoff.md` — bridge between
  sessions.

### `prompts/`

- `prompts/README.md` — redirect to `.ai/prompts/`. The
  old `prompts/bootstrap.md`, `prompts/feature.md`,
  `prompts/bugfix.md`, and `prompts/refactor.md` were
  removed; their content is preserved, improved, and
  re-homed in `.ai/prompts/`.

## Files Modified

- `AGENTS.md` — added § 2.1 (AI Session Operational Sequence,
  11 steps), § 2.2 (Precedence Hierarchy). Replaced all
  `/prompts/` references with `/.ai/prompts/` references.
  Expanded the document map with 23 new `.ai/` entries.
- `CONTRIBUTING.md` — updated prompt references to point
  to `.ai/prompts/` and added the `.ai/workflows/` and
  `.ai/templates/` layers to the AI-agent rules.
- `ROADMAP.md` — updated M0 deliverables to list the new
  `.ai/` artefacts (10 prompts, 6 workflows, 5 templates).
  Added four "Dogfooding checkpoint" subsections (M1
  Lavish Axi, M2 Treehouse, M3 No Mistakes, M5 GNHF).
- `docs/folder-structure.md` — added a documented
  responsibility for `.ai/` at the repository root. Added
  the precedence hierarchy in the `.ai/` section. Removed
  the stale `prompts/` reference from the top-level layout.

## Reusable Components Introduced

Reusable here is **documentation components**, not Blazor
components:

- **Prompts:** 10 task-type templates, each addressing one
  class of work. The four migrated prompts were improved
  with the new precedence statement.
- **Workflows:** 6 multi-step procedures that sequence the
  work the prompts describe.
- **Templates:** 5 reusable document templates that
  structure the work the workflows produce.
- **Entry points:** `.ai/README.md` (precedence + routing)
  and `.ai/session-start.md` (operational sequence).

## Services Introduced

None. This is a documentation-only change.

## Providers Touched

None.

## Tests Added

None. The architecture tests, provider contract tests, and
bUnit tests are M1 deliverables and are out of scope for
this session.

## Commands Run

- `New-Item -ItemType Directory -Force` to create
  `.ai/`, `.ai/prompts/`, `.ai/workflows/`, `.ai/templates/`.
- `Remove-Item -Force` to remove the four old prompt files
  under `prompts/`.
- `Get-Content` and `Select-String` (PowerShell) to
  validate the precedence statement, the `.ai/` reference
  coverage, and the absence of stale `/prompts/` links.
- No `dotnet` commands. No `git` commands (the repository
  is not a git repository in this environment).
- No external tool invocations.

## Validation Results

- **Step 1, `.ai/` structure:** 24 files in the expected
  layout. Confirmed.
- **Step 2, `/prompts/` duplicates:** four old prompts
  removed. Only the redirect `prompts/README.md` remains.
  Confirmed.
- **Step 3, stale `/prompts/` references:** none in any
  document outside `prompts/README.md` itself.
  Confirmed.
- **Step 4, `.ai/` references in entry points:** present
  in `AGENTS.md`, `CONTRIBUTING.md`, `ROADMAP.md`,
  `docs/folder-structure.md`, and `prompts/README.md`.
  Confirmed.
- **Step 5, precedence statement:** all 10 prompts in
  `.ai/prompts/` begin with the required statement.
  Confirmed via PowerShell.
- **Step 6, `.ai/README.md` explains precedence:**
  confirmed (lines 29–46).
- **Step 7, `AGENTS.md` is the first and highest-priority
  document:** confirmed. New § 2.2 ranks `AGENTS.md`
  first; the file opens with "The constitution of this
  repository."
- **Step 8, no application source code:** confirmed.
  No `.cs`, `.razor`, `.csproj`, `.sln`, `.props`, or
  `.targets` files were created.
- **Step 9, no package installed:** confirmed. No
  manifest changes.
- **Step 10, no external tool invoked:** confirmed. No
  external commands run.
- **Step 11, `git status --short`:** the repository is
  not a git repository in this environment. An
  equivalent inventory is provided below.
- **Step 12, final report using
  `.ai/templates/implementation-report.md`:** this file.

### Equivalent of `git status --short`

```
A  .ai/README.md
A  .ai/session-start.md
A  .ai/prompts/architecture.md
A  .ai/prompts/bootstrap.md
A  .ai/prompts/bugfix.md
A  .ai/prompts/feature.md
A  .ai/prompts/provider.md
A  .ai/prompts/refactor.md
A  .ai/prompts/release.md
A  .ai/prompts/review.md
A  .ai/prompts/testing.md
A  .ai/prompts/ui.md
A  .ai/templates/implementation-plan.md
A  .ai/templates/implementation-report.md
A  .ai/templates/review-report.md
A  .ai/templates/session-handoff.md
A  .ai/templates/task-brief.md
A  .ai/workflows/documentation-update.md
A  .ai/workflows/feature-lifecycle.md
A  .ai/workflows/provider-onboarding.md
A  .ai/workflows/release-checklist.md
A  .ai/workflows/tool-dogfooding.md
A  .ai/workflows/ui-design-review.md
A  prompts/README.md
D  prompts/bootstrap.md
D  prompts/bugfix.md
D  prompts/feature.md
D  prompts/refactor.md
M  AGENTS.md
M  CONTRIBUTING.md
M  ROADMAP.md
M  docs/folder-structure.md
```

## Documentation Updated

- `AGENTS.md` — new § 2.1 (operational sequence) and § 2.2
  (precedence hierarchy); updated document map; replaced
  `/prompts/` references.
- `CONTRIBUTING.md` — updated to `.ai/prompts/`,
  `.ai/workflows/`, `.ai/templates/`.
- `ROADMAP.md` — M0 deliverables list; four
  "Dogfooding checkpoint" subsections.
- `docs/folder-structure.md` — new § 2.1 documenting
  `.ai/`'s responsibility and precedence.

## Deviations

1. **The four migrated prompts now carry the new precedence
   statement at the top, not the old "Read AGENTS.md and
   every referenced document" opener.** The old opener is
   preserved as a *second* paragraph so the rule is
   unchanged; the file simply leads with the precedence
   statement that aligns with `.ai/session-start.md`.
2. **`prompts/` is preserved as a redirect directory** rather
   than deleted, per the task brief's instruction: "if
   compatibility is genuinely necessary, leave only a README
   explaining that prompts now live in `.ai/prompts/`." The
   redirect README points readers to the new home.
3. **No new ADR was added to `DECISIONS.md`.** The change is
   operational, not architectural. The new precedence
   hierarchy in `AGENTS.md` is a clarification of how
   existing documents relate, not a new rule. The dogfooding
   workflow and the redesigned AI session sequence encode
   practice that was already implicit; they do not change
   the architecture.

## Known Limitations

- The four new prompts (`review.md`, `architecture.md`,
  `ui.md`, `testing.md`, `provider.md`, `release.md`) have
  not yet been exercised on a real change. They are
  reviewed for internal consistency and against the
  constitutional documents, but their first use will
  reveal refinements.
- The dogfooding workflow lists checkpoint tools but does
  not invoke them in this session, per the task brief's
  instruction. The next session that reaches a dogfooding
  checkpoint (M1+) is the first to actually use a tool.
- The `git status --short` step is reported as
  "repository is not a git repository" with an equivalent
  inventory, rather than literal output. The change is
  observable; the inventory is exact.

## Next Recommended Step

Begin **M1 — Design System Core** under
`.ai/prompts/bootstrap.md` to create the Blazor Server
project skeleton, then `.ai/prompts/ui.md` for the
documentation page that exercises the primitive,
container, and feedback components. Use
`.ai/workflows/feature-lifecycle.md` to sequence the work
and `.ai/templates/implementation-plan.md` to record the
plan before any code is written.

The M1 dogfooding checkpoint (Lavish Axi for UI review)
becomes available once the documentation page renders.
Do not invoke Lavish Axi until that point.

## Linked Artefacts

- `.ai/README.md`
- `.ai/session-start.md`
- `.ai/prompts/bootstrap.md` and the other nine
  prompts
- `.ai/workflows/feature-lifecycle.md` and the other
  five workflows
- `.ai/templates/implementation-report.md` (the
  template used to produce this report)
- The four files removed from `prompts/`:
  `prompts/bootstrap.md`, `prompts/feature.md`,
  `prompts/bugfix.md`, `prompts/refactor.md`
- The modified entry-point documents: `AGENTS.md`,
  `CONTRIBUTING.md`, `ROADMAP.md`,
  `docs/folder-structure.md`
