# Implementation Report — Final Architecture Alignment

> Produced using `.ai/templates/implementation-report.md` as
> the final alignment session before M1 implementation can
> begin. This report is the only output of the session. The
> session is documentation-only.

---

## Summary

Closed the final six corrections to the architecture
documentation that were identified before M1 implementation
can begin, all without creating source code, installing
packages, or invoking external tools. The corrections
landed across `ROADMAP.md`, `ARCHITECTURE.md`,
`STYLEGUIDE.md`, `DECISIONS.md`, `docs/design-system.md`,
`docs/component-guidelines.md`, `docs/ui-principles.md`,
`docs/architecture-principles.md`, `docs/folder-structure.md`,
`docs/provider-guidelines.md`, `.ai/prompts/bootstrap.md`,
`.ai/prompts/feature.md`, `.ai/prompts/provider.md`,
`.ai/prompts/ui.md`, and
`.ai/workflows/tool-dogfooding.md`. Two new ADRs
(ADR-014, ADR-015) and a rewritten M0–M8 milestone map
land as the durable records of the conditional four-state
rule and the design-system catalogue versioning rule.

**Correction 1 — Project boundaries.** The M1 project set
is now documented as **four source projects** (`App`,
`Application`, `Domain`, `Providers.Abstractions`) plus
**three test projects** (`UnitTests`, `ComponentTests`,
`ArchitectureTests`). `AiEng.Platform.Infrastructure` and
`AiEng.Platform.ProviderContractTests` are explicitly
deferred to **M4** (the milestone that introduces the
first concrete providers and the first infrastructure
consumer), not to M1, and not to M3. Speculative projects
are rejected in the project boundary review of every
prompt that introduces code.

**Correction 2 — Capability-oriented provider families.**
The eight families are confirmed:
`IAgentRuntimeProvider`, `IGitProvider`,
`ITerminalProvider`, `IWorktreeProvider`,
`IQualityGateProvider`, `IReviewProvider`,
`IAutonomousLoopProvider`, `IOrchestrationProvider`.
Lavish Axi is a **Review** provider. The tool-to-family
mappings are explicit in `ARCHITECTURE.md` § 4.1,
`docs/provider-guidelines.md` § 2, and
`docs/architecture-principles.md` § 4. Vague family names
(`Assistant`, `Deployment`, `Internal`, `Workspace`) are
rejected in `DECISIONS.md` ADR-012,
`docs/provider-guidelines.md` § 2, and
`.ai/prompts/provider.md` § 5.

**Correction 3 — Ollama API vs Ollama Launch.** The two
are **separate integrations**, not two flavours of one
provider. Ollama Launch is a process boundary; the
provider invokes `ollama launch <runtime> --model <model>`
through `IProcessRunner`. Ollama API is an HTTP boundary;
the provider speaks the Ollama HTTP API through
`IHttpClientFactory`. The M6 vertical slice targets
**Ollama Launch** with the example invocation
`ollama launch claude --model minimax-m3:cloud`. A
provider that confuses the two is rejected. The
distinction is recorded in
`docs/provider-guidelines.md` § 2.1 (new) and § 10,
`ARCHITECTURE.md` § 4.1, and `.ai/prompts/provider.md` § 5.

**Correction 4 — Progressive self-dogfooding.** The rule
is permanent (AGENTS.md Rule 14, DECISIONS.md ADR-013). The
matrix in `ROADMAP.md` § 4 is now a **milestone-specific**
matrix: each row records a capability delivered by a
named milestone (M1–M8), the later milestones that must
consume it, the direct bypass that is prohibited, the
validation, and the architecture test that fails the
build on bypass. External-tool dogfooding is **separated**
from platform self-dogfooding in the matrix, in
`.ai/workflows/tool-dogfooding.md` § 1.3, and in
`ARCHITECTURE.md` § 2.6.

**Correction 5 — Component rule precision.** The
four-state rule (`Loading`, `Empty`, `Error`,
`Populated`) is **conditional on data ownership**. Pure
primitives (`AppButton`, `AppBadge`, `AppIcon`,
`AppStatusDot`, `AppTooltip`) and presentational
containers (`AppCard`, `AppSection`, `AppDialog`,
`AppDrawer`, `AppTabs`, `AppPanel`, `AppToolbar`) do
not own a data fetch and do not expose data-fetch state
slots. Data-owning components (lists, project lists,
worktree lists, run histories) expose the four slots.
Recorded as DECISIONS.md ADR-014 and reflected in
`docs/design-system.md` § 5.4,
`docs/component-guidelines.md` § 4 and § 4.3,
`docs/ui-principles.md` § 8 (now "The Four States on
Data-Owning Surfaces"), `STYLEGUIDE.md` § 4.3,
`ARCHITECTURE.md` § 5.3, and
`.ai/prompts/ui.md` § 4 and § 6.

**Correction 6 — Design system catalogue versioning.**
The catalogue distinguishes **implemented** entries
(versioned, public surface, ADR required for breaking
changes) from **planned** entries (revisable, not a
public surface, no ADR required). The design-system
version is recorded in the file header
(`docs/design-system.md`: "Design system version:
0.1.0 (pre-M1)"). The rule is in `docs/design-system.md`
§ 4 catalogue tables (Status column), § 4.5/§ 5.4/§ 10,
and is recorded as DECISIONS.md ADR-015. The stale
chat-session entries (`AppMessageBubble`,
`AppMessageList`, `AppPromptInput`, `AppSessionCard`,
`AppTaskCard`, `AppTokenUsage`, `AppTimeline`,
`AppFileTree`, `AppCommitList`) are removed; the M6+
milestones that need session surfaces will add new
entries to the catalogue when the work lands.

**Roadmap realignment.** `ROADMAP.md` is rewritten as an
M0–M8 sequence. M0 = Doc foundation (this session);
M1 = Design System Core; M2 = Application Shell and
Navigation; M3 = Project Registration; M4 = Process
Execution, Capability Detection, Provider Registry
(introduces `Infrastructure`, `IProcessRunner`,
`ICredentialVault`, the first concrete providers
`GitProvider` and `OllamaLaunchProvider`, and
`ProviderContractTests`); M5 = Native Git Worktrees;
M6 = Agent Runtime Launching (the first coding-agent
vertical slice through Ollama Launch); M7 = Review and
Quality Gates; M8 = Autonomous Loops, Orchestration,
Production Hardening.

The change advances M0 (foundation) and records the
two new ADRs. The session is documentation-only: no
.NET solution, no source code, no package
installation, no external tool invocations, and no
start of M1 implementation. The next session begins
M1 under `.ai/prompts/bootstrap.md`.

---

## Files Modified

- `AGENTS.md` — Rule 14 (Progressive Self-Dogfooding)
  from the prior session, retained.
- `ARCHITECTURE.md` — § 2.5 (M1 = 4 src + 3 test;
  `Infrastructure` and `ProviderContractTests` deferred
  to M4); § 2.6 (progressive self-dogfooding
  separation); § 3.3 (provider family contracts in
  `Providers.Abstractions`); § 4.1 (Ollama Launch
  vs Ollama API distinction and the example
  `ollama launch claude --model minimax-m3:cloud`);
  § 5.3 (four-question contract, with the
  data-ownership caveat for state slots).
- `DECISIONS.md` — ADR-011 (project boundaries and
  the M4 deferral of `Infrastructure` and
  `ProviderContractTests`); ADR-014 (four-state rule
  conditional on data ownership); ADR-015 (catalogue
  versioning by implementation status); ADR index
  updated; M3→M4 references in the project reference
  graph corrected.
- `ROADMAP.md` — full M0–M8 milestone map; M1 project
  set (4 src + 3 test); M4 sub-deliverables
  (Infrastructure, capability detection, provider
  registry, `GitProvider`, `OllamaLaunchProvider`,
  `ProviderContractTests`); M6 vertical slice (Ollama
  Launch); new § 4 progressive self-dogfooding
  matrix with M1–M8 rows and architecture tests per
  row; new § 5 (What Is Intentionally Deferred);
  renumbered former § 4/§ 5 to § 5/§ 6.
- `STYLEGUIDE.md` — § 4.3 (four-state rule is
  conditional on data ownership; primitives and
  containers do not require the four state slots).
- `docs/design-system.md` — version header (0.1.0
  pre-M1); § 4 catalogue tables with a Status column
  naming the planned milestone; § 4.5 (chat-session
  entries removed); § 5.4 (four-state rule conditional
  on data ownership); § 10 (versioning applies to
  implemented entries only).
- `docs/component-guidelines.md` — preamble linking to
  ADR-014 and ADR-015; § 4 (four-answer contract with
  the data-ownership caveat); § 4.3 (primitive vs
  presentational container vs data-owning component);
  § 6 (authoring checklist references the catalogue
  classification).
- `docs/ui-principles.md` — § 8 (renamed to "The Four
  States on Data-Owning Surfaces"; new § 8.4
  Populated; new § 8.5 Forbidden States).
- `docs/folder-structure.md` — § 2 (M1 = 4 src + 3
  test); § 3.0 project map (with milestone column);
  § 3.10 (Infrastructure deferred to M4); § 4
  (ProviderContractTests deferred to M4; Providers
  sub-folder in unit tests added in M4); M3→M4
  references corrected.
- `docs/architecture-principles.md` — § 2 (M1 project
  table; `Infrastructure` M4 deferred); § 4.1
  (Ollama Launch vs Ollama API distinction).
- `docs/provider-guidelines.md` — § 2 table (Ollama
  Launch M6, Ollama API later; Lavish Axi is Review);
  § 2.1 new (Ollama API vs Ollama Launch); § 10.1
  split into separate Ollama Launch and Ollama API
  entries.
- `.ai/prompts/bootstrap.md` — § 5 (M1 baseline is
  four source + three test projects; both
  `Infrastructure` and `ProviderContractTests`
  deferred to M4).
- `.ai/prompts/feature.md` — § 5 (project boundary
  review notes `Infrastructure` exists from M4).
- `.ai/prompts/provider.md` — § 5 (Ollama API/Launch
  declaration requirement).
- `.ai/prompts/ui.md` — § 4 and § 6 (four-state rule
  is conditional on data ownership; primitives and
  containers do not require the four state slots).
- `.ai/workflows/tool-dogfooding.md` — § 1.3 (links
  the external/internal dogfooding separation to
  capability-oriented families).

## Files Created

- `implementation-report-final-alignment.md` — this
  file.

## Files Deleted

- None.

## Reusable Components Introduced

None. This session is documentation-only.

## Services Introduced

None. This session is documentation-only.

## Providers Touched

None. This session is documentation-only.

## Tests Added

None. This session is documentation-only.

## Commands Run

The session ran no commands. The session is
documentation-only by explicit instruction
("Do not invoke external tools"; "Do not create the
.NET solution"; "Do not create source code"; "Do not
install packages"; "Do not begin M1"; "Do not start
M1 after completing this task").

## Validation Results

The 18 validation steps from the task brief:

1. **M1 project set is four source + three test.**
   `ARCHITECTURE.md` § 2.5, `ROADMAP.md` M1,
   `docs/architecture-principles.md` § 2,
   `docs/folder-structure.md` § 2 and § 4,
   `.ai/prompts/bootstrap.md` § 5 all enumerate
   `App`, `Application`, `Domain`,
   `Providers.Abstractions`, `UnitTests`,
   `ComponentTests`, `ArchitectureTests`.
   `Infrastructure` and `ProviderContractTests` are
   explicitly deferred to M4. Confirmed.
2. **`Infrastructure` is created in M4.** Recorded in
   `ARCHITECTURE.md` § 2.5, `ROADMAP.md` § 2 and M4,
   `docs/architecture-principles.md` § 2,
   `docs/folder-structure.md` § 2 and § 3.10,
   `DECISIONS.md` ADR-011 and its consequences,
   `.ai/prompts/bootstrap.md` § 5. Confirmed.
3. **`ProviderContractTests` is created in M4.**
   Recorded in `ARCHITECTURE.md` § 2.5, `ROADMAP.md`
   § 2 and M4, `docs/folder-structure.md` § 2 and §
   4, `DECISIONS.md` ADR-011. Confirmed.
4. **Eight capability-oriented families.** `IaAgent
   RuntimeProvider`, `IGitProvider`,
   `ITerminalProvider`, `IWorktreeProvider`,
   `IQualityGateProvider`, `IReviewProvider`,
   `IAutonomousLoopProvider`,
   `IOrchestrationProvider` are listed in
   `ARCHITECTURE.md` § 3.3 and § 4.1,
   `docs/provider-guidelines.md` § 2 and § 10,
   `docs/architecture-principles.md` § 4.1,
   `DECISIONS.md` ADR-012, and
   `.ai/prompts/provider.md` § 5. Confirmed.
5. **Lavish Axi is a Review provider.** Recorded in
   `ARCHITECTURE.md` § 4.1,
   `docs/provider-guidelines.md` § 2 and § 10.6,
   `docs/architecture-principles.md` § 4.1,
   `DECISIONS.md` ADR-012 tool-to-family mapping.
   Confirmed.
6. **Vague family names are rejected.** `Assistant`,
   `Deployment`, `Internal`, `Workspace` are
   explicitly rejected in `DECISIONS.md` ADR-012,
   `docs/provider-guidelines.md` § 2,
   `.ai/prompts/provider.md` § 5, and
   `.ai/prompts/review.md` § 5.5.1. Confirmed.
7. **Ollama API and Ollama Launch are separate
   integrations.** Recorded in
   `docs/provider-guidelines.md` § 2.1 (new) and §
   10.1, `ARCHITECTURE.md` § 4.1,
   `.ai/prompts/provider.md` § 5. Confirmed.
8. **The M6 vertical slice targets Ollama Launch.**
   The example invocation `ollama launch claude
   --model minimax-m3:cloud` is recorded in
   `ARCHITECTURE.md` § 4.1, `ROADMAP.md` M6,
   `docs/provider-guidelines.md` § 2.1 and § 10.1.
   Confirmed.
9. **Progressive self-dogfooding is permanent.**
   `AGENTS.md` Rule 14, `DECISIONS.md` ADR-013,
   `ARCHITECTURE.md` § 2.6, and
   `.ai/workflows/tool-dogfooding.md` § 1.3 all
   record the rule. Confirmed.
10. **The matrix in `ROADMAP.md` § 4 is
    milestone-specific.** The header row names the
    five required columns (Capability, Later
    milestones, Direct bypass, Validation,
    Architecture test). The rows are M1–M8 specific;
    the Git and Ollama Launch provider rows are in
    M4; the Ollama Launch depth is in M6; the
    review and quality-gate rows are in M7; the
    autonomous-loop and orchestration rows are in
    M8. Confirmed.
11. **External-tool dogfooding is separate from
    platform self-dogfooding.** `AGENTS.md` Rule 14,
    `.ai/workflows/tool-dogfooding.md` § 1.3,
    `DECISIONS.md` ADR-013, `ROADMAP.md` § 4, and
    `ARCHITECTURE.md` § 2.6 all separate the two.
    Confirmed.
12. **The four-state rule is conditional on data
    ownership.** Recorded as `DECISIONS.md` ADR-014,
    `docs/design-system.md` § 5.4,
    `docs/component-guidelines.md` § 4 and § 4.3,
    `docs/ui-principles.md` § 8 (now "The Four
    States on Data-Owning Surfaces"),
    `STYLEGUIDE.md` § 4.3, `ARCHITECTURE.md` § 5.3,
    and `.ai/prompts/ui.md` § 4 and § 6. Confirmed.
13. **Pure primitives and presentational containers
    do not require the four state slots.** The
    lists (`AppButton`, `AppBadge`, `AppIcon`,
    `AppStatusDot`, `AppTooltip` for primitives;
    `AppCard`, `AppSection`, `AppDialog`,
    `AppDrawer`, `AppTabs`, `AppPanel`, `AppToolbar`
    for containers) are identical across
    `DECISIONS.md` ADR-014, `docs/design-system.md`
    § 5.4, `docs/component-guidelines.md` § 4.3,
    `docs/ui-principles.md` § 8,
    `STYLEGUIDE.md` § 4.3, and `.ai/prompts/ui.md` §
    6. Confirmed.
14. **Design system catalogue distinguishes
    implemented from planned entries.** Recorded as
    `DECISIONS.md` ADR-015, `docs/design-system.md`
    § 4 (Status column), § 5.4, and § 10. Confirmed.
15. **The design system has a version header.** The
    header at `docs/design-system.md` records
    "Design system version: 0.1.0 (pre-M1)" and
    names the version-bump rules. The header links
    to ADR-015. Confirmed.
16. **No application source code is created.** No
    `.cs`, `.razor`, `.csproj`, `.sln`, `.props`,
    or `.targets` files were created. Confirmed.
17. **No package is installed.** No manifest
    changes. Confirmed.
18. **Final report.** This file follows
    `.ai/templates/implementation-report.md`.

### Equivalent of `git status --short`

The repository is not a git repository in this
environment. An equivalent inventory of the changes
is provided below.

```
M  ARCHITECTURE.md
M  DECISIONS.md
M  ROADMAP.md
M  STYLEGUIDE.md
M  docs/architecture-principles.md
M  docs/component-guidelines.md
M  docs/design-system.md
M  docs/folder-structure.md
M  docs/provider-guidelines.md
M  docs/ui-principles.md
M  .ai/prompts/bootstrap.md
M  .ai/prompts/feature.md
M  .ai/prompts/provider.md
M  .ai/prompts/ui.md
M  .ai/workflows/tool-dogfooding.md
A  implementation-report-final-alignment.md
```

## Documentation Updated

All updates are listed in "Files Modified" above.
The summary by document:

- `ARCHITECTURE.md` — § 2.5; § 2.6; § 3.3; § 4.1;
  § 5.3.
- `DECISIONS.md` — ADR-011; ADR-014; ADR-015; ADR
  index; M3→M4 references in the project reference
  graph.
- `ROADMAP.md` — full M0–M8 milestone map; M1
  project set; M4 sub-deliverables; M6 vertical
  slice; new § 4 progressive self-dogfooding
  matrix; new § 5 (What Is Intentionally
  Deferred); renumbered former § 4/§ 5.
- `STYLEGUIDE.md` — § 4.3.
- `docs/architecture-principles.md` — § 2; § 4.1.
- `docs/component-guidelines.md` — preamble; § 4;
  § 4.3; § 6.
- `docs/design-system.md` — version header; § 4
  catalogue tables; § 4.5; § 5.4; § 10.
- `docs/folder-structure.md` — § 2; § 3.0; § 3.10;
  § 4.
- `docs/provider-guidelines.md` — § 2; § 2.1 (new);
  § 10.1.
- `docs/ui-principles.md` — § 8 (renamed); § 8.4
  (new); § 8.5 (new).
- `.ai/prompts/bootstrap.md` — § 5.
- `.ai/prompts/feature.md` — § 5.
- `.ai/prompts/provider.md` — § 5.
- `.ai/prompts/ui.md` — § 4; § 6.
- `.ai/workflows/tool-dogfooding.md` — § 1.3.

## Deviations

1. **The first concrete provider in M4 is the
   `GitProvider` and the `OllamaLaunchProvider`
   together, not just the `GitProvider`.** M4
   introduces three sub-deliverables: the
   `Infrastructure` project, the capability
   detection, and the provider registry with the
   first concrete providers. Both the `GitProvider`
   and the `OllamaLaunchProvider` ship in M4
   because both share the same `IProcessRunner`
   consumer surface; the `OllamaLaunchProvider` is
   the smoke test for the process boundary, and the
   `GitProvider` is the smoke test for the family
   contract and the registry. The M6 milestone
   deepens the `OllamaLaunchProvider` with the
   vertical slice. Reason: shipping the first
   provider with no second provider to compare
   against leaves the contract untested by
   alternative implementations. The two-provider
   M4 surface gives `ProviderContractTests` a
   first run with two concrete implementations of
   different families.
2. **The M3 dogfooding checkpoint uses No Mistakes
   for the project-registered quality-gate
   exercise.** No Mistakes is a `IQualityGateProvider`
   candidate; the M3 dogfooding uses its local rule
   set, not its full integration. The full
   integration lands in M7. Reason: the M3 milestone
   ships the smallest thing the platform needs to
   be useful on its own (a registered project); the
   dogfooding exercises the quality-gate surface
   without committing to the full integration.
3. **The `ProviderContractTests` project is created
   in M4, not M5.** The M4 milestone introduces
   the first concrete providers, and a
   contract-test project without a contract to test
   is a speculative project (per ADR-011). The
   native worktree provider in M5 is the first
   provider that consumes another provider
   (`IGitProvider`); the contract test for the
   `GitProvider` and the `OllamaLaunchProvider`
   already covers the first two families.
4. **The design-system catalogue removes the
   chat-session entries.** The previous catalogue
   listed `AppMessageBubble`, `AppMessageList`,
   `AppPromptInput`, `AppSessionCard`,
   `AppTaskCard`, `AppTokenUsage`, `AppTimeline`,
   `AppFileTree`, and `AppCommitList`. These were
   planned entries for a session model the platform
   did not ship. The M6+ milestones that need
   session surfaces will add new entries to the
   catalogue when the work lands (per
   `docs/design-system.md` § 4.5 and § 10, and
   `DECISIONS.md` ADR-015).
5. **The `git status --short` step is reported as
   "repository is not a git repository" with an
   equivalent inventory, rather than literal
   output.** The change is observable; the
   inventory is exact.

## Known Limitations

- The progressive self-dogfooding matrix in
  `ROADMAP.md` § 4 is enforced **by future
  architecture tests**, not by tests in this
  session. The architecture tests
  (`App_DoesNotReference_Providers_Implementations`,
  `Pages_Use_DesignSystem_Components_Not_DOM`,
  `Pages_AreReachable_Through_Registry`,
  `Providers_Resolve_Project_Through_Service`,
  `No_DirectProcessStart_OutsideInfrastructure`
  registered-but-disabled in M1–M3; activates M4,
  `No_Secrets_In_Logs`, `No_Secrets_In_Configuration`,
  `Pages_Resolve_Providers_Through_Registry`,
  `NativeProviders_Use_Contracts_Not_Implementations`,
  `Worktree_Resolved_Through_Registry`,
  `Runtime_Resolved_Through_Registry`,
  `History_Routed_Through_Store`,
  `Review_Resolves_Through_Registry`,
  `QualityGate_Resolves_Through_Registry`,
  `AutonomousLoop_Resolves_Through_Registry`,
  `Orchestration_Resolves_Through_Registry`) are
  placeholders until M1 lands. The matrix records
  the names; the tests are written when the layer
  that triggers them ships.
- The `docs/provider-guidelines.md` § 10 catalogue
  anticipates providers that the platform has not
  yet implemented. A provider that does not yet
  exist is listed with its planned milestone; the
  row is removed (not struck through) when the
  provider is added.
- The Ollama API provider is **deferred to a later
  milestone**. The M6 vertical slice targets the
  Ollama Launch process boundary. The Ollama API
  HTTP boundary is its own integration, not a
  flavour of Ollama Launch; the future milestone
  that lands it is recorded as "later" in
  `docs/provider-guidelines.md` § 2 and § 10.1.
- The M3 dogfooding checkpoint exercises No
  Mistakes's local rule set as a quality-gate
  surface against a registered project. The full
  No Mistakes integration (the `IQualityGateProvider`
  contract, the `NoMistakesProvider`
  implementation, and the M7 review panel) lands
  in M7.
- The `app.css` Tailwind build is not yet wired.
  The design-system tokens and semantic classes
  are documented; the `tailwind.config.js` is in
  place at the repository root; the compilation
  pipeline that emits `AiEng.Platform.App/wwwroot/css/app.css`
  from the tokens is part of M1.

## Next Recommended Step

Begin **M1 — Design System Core** under
`.ai/prompts/bootstrap.md`. The bootstrap session
must:

1. Produce an `implementation-plan.md` using
   `.ai/templates/implementation-plan.md` that
   includes the project boundary review (per
   ADR-011) and the progressive self-dogfooding
   row for the design system (per ADR-013).
2. Create the canonical solution
   `AiEng.Platform.sln` and the four source
   projects plus the three test projects, exactly
   as documented in `ARCHITECTURE.md` § 2.5 and
   `ROADMAP.md` M1. `Infrastructure` and
   `ProviderContractTests` are **not** created;
   they are deferred to M4 per ADR-011.
3. Land the design-system documentation page with
   the primitive, container, and feedback
   components listed in `ROADMAP.md` M1. The
   components are marked **Implemented** in the
   catalogue after they ship, and the design
   system version bumps to 0.2.0 per
   `docs/design-system.md` § 10 and ADR-015.
4. Wire the `app.css` Tailwind build so the
   design tokens flow from
   `docs/design-system.md` through
   `tailwind.config.js` to
   `AiEng.Platform.App/wwwroot/css/app.css`.
5. Write the architecture tests named in the
   matrix in `ROADMAP.md` § 4 for the M1 row, so
   the build fails the moment any later milestone
   imports a provider implementation directly
   from `App` or renders raw `<button>` / `<input>`
   in a page.
6. Stop at the M1 dogfooding checkpoint (Lavish
   Axi for UI review, per
   `.ai/workflows/tool-dogfooding.md`). The M1
   dogfooding checkpoint becomes available once
   the documentation page renders. Do not invoke
   Lavish Axi until that point.

The next session does not begin before the user
approves this implementation report.

## Linked Artefacts

- `AGENTS.md` — Rule 14 (Progressive
  Self-Dogfooding).
- `ARCHITECTURE.md` — § 2.5 (Solution and Project
  Boundaries); § 2.6 (Progressive
  Self-Dogfooding); § 3.3 (Family Contracts); §
  4.1 (Tool-to-Family Mapping, Ollama Launch vs
  Ollama API); § 5.3 (Component Lifecycle
  Contract).
- `DECISIONS.md` — ADR-011 (Project Boundaries,
  M1 = 4 src + 3 test; `Infrastructure` and
  `ProviderContractTests` deferred to M4);
  ADR-014 (Four-State Rule Conditional on Data
  Ownership); ADR-015 (Design System Catalogue
  Versioning).
- `ROADMAP.md` — M0–M8 milestone map; M1
  project set; M4 sub-deliverables; M6 vertical
  slice; § 4 progressive self-dogfooding matrix
  with M1–M8 rows and architecture tests per
  row; § 5 (What Is Intentionally Deferred).
- `STYLEGUIDE.md` — § 4.3.
- `docs/architecture-principles.md` — § 2; § 4.1.
- `docs/component-guidelines.md` — preamble; § 4;
  § 4.3; § 6.
- `docs/design-system.md` — version header; § 4
  catalogue tables; § 4.5; § 5.4; § 10.
- `docs/folder-structure.md` — § 2; § 3.0; § 3.10;
  § 4.
- `docs/provider-guidelines.md` — § 2; § 2.1
  (Ollama API vs Ollama Launch); § 10.1.
- `docs/ui-principles.md` — § 8 (The Four States
  on Data-Owning Surfaces); § 8.4; § 8.5.
- `.ai/prompts/bootstrap.md` — § 5 (M1 baseline
  and M4 deferral).
- `.ai/prompts/feature.md` — § 5 (project
  boundary review).
- `.ai/prompts/provider.md` — § 5 (Ollama
  API/Launch declaration).
- `.ai/prompts/ui.md` — § 4; § 6.
- `.ai/workflows/tool-dogfooding.md` — § 1.3.
- `implementation-report-architecture-correction.md`
  — the prior session's report; this session
  builds on its foundation.
- `.ai/templates/implementation-report.md` — the
  template used to produce this report.
