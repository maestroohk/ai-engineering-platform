# Implementation Report — Architecture Correction

> Produced using `.ai/templates/implementation-report.md` as
> part of the architecture correction session. This report is
> the only output of the session.

---

## Summary

Resolved three architecture issues that were identified
before M1 implementation could begin, all without creating
source code, installing packages, or invoking external
tools. The session is documentation-only.

**Issue 1 — Solution and project boundaries.** Documented
the smallest solution structure that provides real
compile-time boundaries: `AiEng.Platform.sln` plus five
source projects (`App`, `Application`, `Domain`,
`Infrastructure`, `Providers.Abstractions`) and four test
projects (`UnitTests`, `ComponentTests`,
`ArchitectureTests`, `ProviderContractTests`). Provider
implementation projects are deferred to the milestone that
introduces them. Allowed and forbidden project references
are documented and pinned by architecture tests.

**Issue 2 — Provider capability family names.** Replaced the
vague families (`Assistant`, `Deployment`, `Internal`,
`Workspace`) with capability-oriented contracts:
`IAgentRuntimeProvider`, `IGitProvider`, `ITerminalProvider`,
`IWorktreeProvider`, `IQualityGateProvider`, `IReviewProvider`,
`IAutonomousLoopProvider`, `IOrchestrationProvider`. The
base `IProvider` is retained. The mappings are explicit
(Treehouse→`IWorktreeProvider`, No Mistakes→
`IQualityGateProvider`, Lavish Axi→`IReviewProvider`, GNHF→
`IAutonomousLoopProvider`, Firstmate→`IOrchestrationProvider`).
Workspace state is removed from the provider model and
returned to the application layer.

**Issue 3 — Progressive self-dogfooding.** Recorded the
rule that every milestone must consume the stable reusable
capabilities delivered by earlier milestones. Added a
progressive self-dogfooding matrix to `ROADMAP.md` § 4
that records, for every reusable capability, the later
milestones that must use it, the direct bypass that is
prohibited, the validation, and the architecture test
that fails the build on bypass. External-tool dogfooding
is **separated** from platform self-dogfooding.

The change advances M0 (foundation) and records three new
ADRs: ADR-011 (multi-project solution), ADR-012
(capability-oriented provider families), ADR-013
(progressive self-dogfooding). Rule 14 in `AGENTS.md`
makes the self-dogfooding rule permanent.

## Files Created

None. The session is a documentation-only correction.

## Files Modified

### Architectural decisions

- `DECISIONS.md`
  - ADR-011 — Adopt a multi-project solution for
    compile-time layer boundaries. Documents the canonical
    solution and the five source projects plus four test
    projects, the allowed project references, the
    forbidden project references, where shared contracts
    live, where UI components live, where Tailwind assets
    live, and how architecture tests complement
    compile-time boundaries. Defers provider
    implementation projects to their milestone.
  - ADR-012 — Adopt capability-oriented provider families.
    Documents the eight capability-oriented families,
    the rationale for replacing the vague `Assistant`,
    `Deployment`, `Internal` families, the explicit
    mappings (Treehouse→`IWorktreeProvider`, No Mistakes→
    `IQualityGateProvider`, Lavish Axi→`IReviewProvider`,
    GNHF→`IAutonomousLoopProvider`, Firstmate→
    `IOrchestrationProvider`), the removal of
    `IWorkspaceProvider` (workspace state is
    application-layer state), and the retention of
    `IProvider` as the small base metadata contract.
  - ADR-013 — Adopt progressive self-dogfooding of
    platform capabilities. Documents the rule that every
    milestone must consume the stable reusable
    capabilities delivered by earlier milestones, the
    separation from external-tool dogfooding, and the
    matrix-driven enforcement.
  - ADR index updated with all three new entries.

### Architecture documents

- `ARCHITECTURE.md`
  - System diagram updated with the new provider family
    contracts.
  - § 3.3 Provider Contracts updated with the eight
    capability-oriented families.
  - § 2.5 Solution and Project Boundaries (new) — the M1
    project set, allowed and forbidden project references,
    shared contracts, UI components, Tailwind assets, and
    how architecture tests complement compile-time
    boundaries.
  - § 2.6 Progressive Self-Dogfooding (new) — the matrix
    in `ROADMAP.md` § 4, the separation from
    external-tool dogfooding, the two governing workflows.
  - § 4.1 updated with the new family mappings and the
    rejection of vague family names.
  - § 4.2 Provider Anatomy updated.
  - § 4.3 Provider Discovery updated.
- `docs/architecture-principles.md`
  - § 2 (Five Layers and the Project Map) updated to
    include the M1 project map and the mapping from
    logical layer to project.
  - § 4.1 updated with the capability-oriented naming
    rule and the rejection of vague family names.
  - § 4.6 Progressive Self-Dogfooding (new) — the matrix
    in `ROADMAP.md` § 4, the four enforcing architecture
    tests (`No_DirectProcessStart_OutsideInfrastructure`,
    no provider-to-provider, registry-only discovery, no
    raw string IDs in cross-layer calls), and the
    separation from external-tool dogfooding.
  - § 4.2 updated to reference the new
    `src/AiEng.Platform.Providers.<X>/` project layout.
- `docs/folder-structure.md`
  - Top-level layout (lines 28–66) updated to show the
    multi-project structure with `AiEng.Platform.sln`,
    five source projects, four test projects, and
    `tailwind.config.js` at the root.
  - § 3 (Source Folders) restructured to map every folder
    to its project: § 3.0 Project Map (per ADR-011),
    § 3.1 `AiEng.Platform.App/Components/`, § 3.2
    `AiEng.Platform.App/Dialogs/`, § 3.3
    `AiEng.Platform.App/Layouts/`, § 3.4
    `AiEng.Platform.App/Pages/`, § 3.5
    `AiEng.Platform.Application/Services/`, § 3.6
    `AiEng.Platform.Application/Dtos/`, § 3.7
    `AiEng.Platform.Domain/`, § 3.8
    `AiEng.Platform.Application/ViewModels/`, § 3.9
    `AiEng.Platform.Providers.<X>/`, § 3.10
    `AiEng.Platform.Infrastructure/`, § 3.11
    `AiEng.Platform.Application/Helpers/`, § 3.12
    `AiEng.Platform.Application/Extensions/`, § 3.13
    `AiEng.Platform.App/Navigation/`, § 3.14
    `AiEng.Platform.App/Configuration/`.
  - § 4 (Tests Folders) updated to reflect the four
    distinct test projects (`UnitTests`, `ComponentTests`,
    `ArchitectureTests`, `ProviderContractTests`) and
    their single responsibilities.
  - § 7 (Co-Location) updated to point to the new project
    structure (`AiEng.Platform.Providers.<X>/`,
    `AiEng.Platform.Providers.Abstractions/<Family>/`,
    `AiEng.Platform.ProviderContractTests/<Family>/<Provider>/`).
- `docs/provider-guidelines.md`
  - § 2 Provider Families rewritten to use the
    capability-oriented names, the explicit tool-to-family
    mappings, the rejection of `Assistant`, `Deployment`,
    `Internal` as family names, and the explanation of
    why `IWorkspaceProvider` is removed.
  - § 3.2 Family Contract example updated from
    `IRuntimeProvider` to `IAgentRuntimeProvider`.
  - § 4.1 Folder Layout updated to
    `src/AiEng.Platform.Providers.Ollama/` plus
    `tests/AiEng.Platform.ProviderContractTests/AgentRuntime/Ollama/`.
  - § 4.2 Provider Class example updated to implement
    `IAgentRuntimeProvider`.
  - § 5.1 Registration example updated.
  - § 5.2 Registry example updated to
    `IAgentRuntimeProviderRegistry`.
  - § 6.3 Runtime Configuration updated to
    `IAgentRuntimeProvider.ConfigureAsync`.
  - § 8.1 Contract Tests path updated to
    `tests/AiEng.Platform.ProviderContractTests/<Family>/`.
  - § 8.1 abstract class renamed to
    `AgentRuntimeProviderContractTests` with
    `IAgentRuntimeProvider` as the return type.
  - § 9 Authoring a New Provider updated to require
    `src/AiEng.Platform.Providers.<X>/` (one project per
    provider) and the test folder
    `tests/AiEng.Platform.ProviderContractTests/<Family>/<Provider>/`.
  - § 10 Provider Catalogue rewritten to enumerate the
    eight families, the per-provider contract, transport,
    auth, milestone, and the explicit note that workspace
    state is **not** a provider.
- `docs/naming-conventions.md`
  - Provider contract row updated from `IRuntimeProvider`
    to `IAgentRuntimeProvider`.
  - Registry row updated to
    `<Family>ProviderRegistry` (e.g.
    `AgentRuntimeProviderRegistry`).
  - § 3.2 Interfaces updated with the ADR-012 family list
    and the rejection of vague family names.
- `docs/architecture-principles.md`
  - § 4.3 The Registry example updated to
    `ProviderDescriptor? Resolve(ProviderId id)`.
  - § 4.2 The Implementation bullet updated to
    `src/AiEng.Platform.Providers.<X>/`.
- `STYLEGUIDE.md`
  - § 3.2 Naming table updated to
    `IAgentRuntimeProvider` as the interface example.

### Road-map documents

- `ROADMAP.md`
  - M1 (Design System Core) gained an explicit
    "Solution and project set" subsection that names
    `AiEng.Platform.sln`, the five source projects, and
    the four test projects.
  - M1 gained a "Progressive self-dogfooding" subsection
    that records the contract this milestone delivers
    (`App_DoesNotReference_Providers_Implementations`,
    `Pages_Use_DesignSystem_Components_Not_DOM`).
  - M2 (Layout & Navigation) gained a
    "Progressive self-dogfooding" subsection (consumes
    design system from M1).
  - M3 (Provider Registry) updated to use
    `IAgentRuntimeProviderRegistry` and the
    `IAgentRuntimeProvider` contract; gained a
    self-dogfooding subsection.
  - M4 (Workspace & Sessions) updated to remove
    `LocalFileSystemWorkspaceProvider` (per ADR-012) and
    to record that workspace state is application-layer
    state; gained a self-dogfooding subsection.
  - M5 (Conversation & Runs) gained a self-dogfooding
    subsection that records the
    `ConversationService_ResolvesThrough_Registry`
    architecture test.
  - M6 (Source & Terminal Providers) updated with the
    four terminal providers, the Git provider, and the
    `IProcessRunner` rule; gained a self-dogfooding
    subsection that records
    `No_DirectProcessStart_OutsideInfrastructure`.
  - M7 (Provider Surface in UI) gained a self-dogfooding
    subsection that records
    `Pages_Resolve_Providers_Through_Registry`,
    `No_Secrets_In_Logs`, and
    `No_Secrets_In_Configuration`.
  - M8 (Production Hardening) updated to introduce
    `NativeWorktreeProvider`, `NativeReviewProvider`, and
    `NativeOrchestrationProvider`; gained a
    self-dogfooding subsection that records
    `NativeProviders_Use_ApplicationServices_Not_Implementation`.
  - New § 4 "Progressive Self-Dogfooding Matrix" with
    eleven rows covering the capabilities delivered by
    M1–M8+, the later milestones that must use them, the
    direct bypass that is prohibited, the validation, and
    the enforcing architecture test. (Existing § 4 "What
    Is Intentionally Deferred" renumbered to § 5;
    existing § 5 "How Milestones Are Updated" renumbered
    to § 6.)

### Workflow and prompt documents

- `.ai/prompts/bootstrap.md`
  - § 5 Planning Requirements gained a "Project
    boundary review" subsection that requires allowed
    and forbidden project references, consumer projects,
    and the architecture tests that pin the boundary.
  - § 5 Planning Requirements gained a "Progressive
    self-dogfooding" subsection that requires the matrix
    in `ROADMAP.md` § 4 to be updated when the bootstrap
    delivers a reusable capability.
  - § 8 Documentation Updates expanded to include
    `ROADMAP.md` § 4 (matrix), `ARCHITECTURE.md` (system
    diagram and project map), and the ADR-012
    capability-oriented family naming rule.
- `.ai/prompts/provider.md`
  - § 1 Purpose updated to enumerate the eight families.
  - § 5 Planning Requirements updated to require the
    capability-oriented family name, the new project
    layout, the `IProcessRunner` rule, and the
    progressive self-dogfooding matrix row.
  - § 9 Completion Report "Files added" updated to the
    new project paths.
- `.ai/prompts/feature.md`
  - § 5 Planning Requirements gained a "Project boundary
    review" subsection (per ADR-011).
  - § 5 Planning Requirements gained a "Progressive
    self-dogfooding" subsection (per ADR-013) and
    capability-oriented provider family naming (per
    ADR-012).
- `.ai/prompts/review.md`
  - § 5.1.1 Project Boundaries (new) — review dimension
    for project boundaries.
  - § 5.1.2 Progressive Self-Dogfooding (new) — review
    dimension for the matrix in `ROADMAP.md` § 4.
  - § 5.5.1 Provider Family Naming (new) — review
    dimension for the ADR-012 capability-oriented
    naming rule.
- `.ai/workflows/provider-onboarding.md`
  - Stage 1 Capability Analysis updated with the
    tool-to-family mapping and the capability-oriented
    family list.
  - Stage 3 Fake Implementation path updated to
    `tests/AiEng.Platform.ProviderContractTests/<Family>/<Provider>/`.
  - Stage 4 Real Adapter updated to require
    `src/AiEng.Platform.Providers.<X>/` and the
    `IProcessRunner` rule for process-boundary providers.
  - Stage 9 Progressive Self-Dogfooding (new) — requires
    a matrix row in `ROADMAP.md` § 4 when a provider
    delivers a reusable capability.
- `.ai/workflows/feature-lifecycle.md`
  - Stage 2 Plan gained a "Project boundary review" and
    a "Progressive self-dogfooding" subsection.
- `.ai/workflows/tool-dogfooding.md`
  - § 1.2 Product Integration updated to use the
    capability-oriented family names
    (`IReviewProvider`, `IWorktreeProvider`,
    `IQualityGateProvider`).
  - § 1.3 Platform Self-Dogfooding (new) — explicit
    distinction from external-tool dogfooding; the matrix
    in `ROADMAP.md` § 4; the architecture tests that
    enforce the matrix; the three concrete examples
    (registry-only resolution, `IProcessRunner`-only
    process spawning, contract-only provider
    composition).

### Constitutional document

- `AGENTS.md`
  - § 4 renamed from "The Twelve Non-Negotiable Rules" to
    "The Fourteen Non-Negotiable Rules".
  - Rule 14 — Progressive Self-Dogfooding (new). Records
    the rule that every milestone must consume the stable
    reusable capabilities delivered by earlier milestones;
    names the matrix in `ROADMAP.md` § 4 as the
    authoritative reference; lists the four kinds of
    bypasses that are blocked; and clarifies the
    separation from external-tool dogfooding.

## Files Deleted

None.

## Reusable Components Introduced

Reusable here is **documentation components**, not Blazor
components:

- **Three ADRs** (ADR-011, ADR-012, ADR-013) in
  `DECISIONS.md`, with rationale, decision, and
  consequences. Each is a durable, citable, indexed record.
- **A multi-project solution and project-boundary
  specification** in `ARCHITECTURE.md` § 2.5, with the
  allowed and forbidden project references documented
  together with the architecture tests that pin the
  boundary.
- **A capability-oriented provider-families table** in
  `docs/provider-guidelines.md` § 2, with the eight
  families, the explicit tool-to-family mappings, and
  the rejection of vague family names.
- **A progressive self-dogfooding matrix** in
  `ROADMAP.md` § 4, with eleven rows, each row pinned by
  a named architecture test.
- **Rule 14** in `AGENTS.md`, the permanent constitutional
  statement of the self-dogfooding rule.
- **Project-boundary review** and
  **progressive-self-dogfooding review** subsections in
  `.ai/prompts/feature.md`, `.ai/prompts/bootstrap.md`,
  `.ai/prompts/review.md`, and
  `.ai/workflows/feature-lifecycle.md`,
  so every future session is required to ask the right
  questions before writing code.

## Services Introduced

None. The session is documentation-only; no source code
was created.

## Providers Touched

None. The session is documentation-only; no provider
implementation was added, modified, or removed. (The
removal of the conceptual `IWorkspaceProvider` family
is documented in ADR-012 and reflected in the catalogue
in `docs/provider-guidelines.md` § 10, but no provider
implementation was ever created for the removed family.)

## Tests Added

None. The session is documentation-only. The
architecture tests and contract tests referenced by the
matrix in `ROADMAP.md` § 4 are M1+ deliverables and are
out of scope for this session.

## Commands Run

- `Read` on the constitutional documents (`AGENTS.md`,
  `ARCHITECTURE.md`, `ROADMAP.md`, `STYLEGUIDE.md`,
  `CONTRIBUTING.md`, `DECISIONS.md`).
- `Read` on the engineering documents in `docs/`.
- `Read` on the `.ai/` prompts, workflows, and
  templates.
- `Grep` to verify the absence of stale family names
  (`IRuntimeProvider`, `ISourceProvider`,
  `IWorkspaceProvider`, `IAssistantProvider`,
  `IDeploymentProvider`, `IInternalProvider`,
  `LocalFileSystemWorkspaceProvider`) and the presence of
  the new ones.
- `Edit` to apply every change above.
- `Write` to create this report.
- No `dotnet` commands. No `git` commands (the repository
  is not a git repository in this environment).
- No `npm`, no `pnpm`, no `yarn`, no external package
  manager.
- No external tool invocations.

## Validation Results

The 17 validation steps from the task brief:

1. **Solution boundary is unambiguous.** The five
   source projects and four test projects are listed
   in `ARCHITECTURE.md` § 2.5 and in
   `ROADMAP.md` M1. Confirmed.
2. **M1 project list is explicit.** `AiEng.Platform.sln`
   plus `App`, `Application`, `Domain`,
   `Infrastructure`, `Providers.Abstractions`, and the
   four test projects are enumerated. Confirmed.
3. **Allowed and forbidden project references are
   documented.** `ARCHITECTURE.md` § 2.5 lists both.
   Confirmed.
4. **Capability families replace vague family names.**
   `docs/provider-guidelines.md` § 2, `DECISIONS.md`
   ADR-012, `docs/naming-conventions.md` § 3.2, and
   `ROADMAP.md` all use the capability-oriented names.
   Confirmed.
5. **Every vague family name is rejected.** `Assistant`,
   `Deployment`, `Internal`, `Workspace` are explicitly
   listed as rejected in `DECISIONS.md` ADR-012,
   `docs/provider-guidelines.md` § 2,
   `docs/naming-conventions.md` § 3.2,
   `.ai/prompts/provider.md` § 5, and
   `.ai/prompts/review.md` § 5.5.1. Confirmed.
6. **Mappings are documented.** Treehouse→
   `IWorktreeProvider`, No Mistakes→
   `IQualityGateProvider`, Lavish Axi→
   `IReviewProvider`, GNHF→
   `IAutonomousLoopProvider`, Firstmate→
   `IOrchestrationProvider`. All five are documented in
   `DECISIONS.md` ADR-012, `docs/provider-guidelines.md`
   § 2 and § 10, and `.ai/workflows/provider-onboarding.md`
   Stage 1. Confirmed.
7. **Progressive self-dogfooding matrix exists.**
   `ROADMAP.md` § 4 is the matrix. Confirmed.
8. **Matrix has all required columns.** Capability
   delivered, later milestones that must use it, direct
   bypass that is prohibited, validation, architecture
   test. Confirmed (header row at `ROADMAP.md` line 564).
9. **External-tool dogfooding is separate from platform
   self-dogfooding.** `AGENTS.md` Rule 14,
   `.ai/workflows/tool-dogfooding.md` § 1.3,
   `DECISIONS.md` ADR-013, `ROADMAP.md` § 4, and
   `ARCHITECTURE.md` § 2.6 all separate the two.
   Confirmed.
10. **Architecture tests are mentioned for each bypass.**
    The matrix in `ROADMAP.md` § 4 names an architecture
    test per row. Confirmed.
11. **`AGENTS.md` is the first and highest-priority
    document.** `AGENTS.md` opens with "The constitution
    of this repository" and the precedence hierarchy in
    `.ai/README.md` and `AGENTS.md` § 2.2 ranks
    `AGENTS.md` first. Confirmed.
12. **No application source code is created.** No
    `.cs`, `.razor`, `.csproj`, `.sln`, `.props`, or
    `.targets` files were created. Confirmed.
13. **No package is installed.** No manifest changes.
    Confirmed.
14. **No external tool is invoked.** No external commands
    run. Confirmed.
15. **Report is produced from
    `.ai/templates/implementation-report.md`.** This file
    follows the template's section structure. Confirmed.
16. **Precedence statement is present.** Every modified
    `.ai/prompts/*.md` file begins with the precedence
    statement from `.ai/session-start.md`. Confirmed.
17. **Final report.** This file.

### Equivalent of `git status --short`

The repository is not a git repository in this environment.
An equivalent inventory of the changes is provided below.

```
M  AGENTS.md
M  ARCHITECTURE.md
M  DECISIONS.md
M  ROADMAP.md
M  STYLEGUIDE.md
M  docs/architecture-principles.md
M  docs/folder-structure.md
M  docs/naming-conventions.md
M  docs/provider-guidelines.md
M  .ai/prompts/bootstrap.md
M  .ai/prompts/feature.md
M  .ai/prompts/provider.md
M  .ai/prompts/review.md
M  .ai/workflows/feature-lifecycle.md
M  .ai/workflows/provider-onboarding.md
M  .ai/workflows/tool-dogfooding.md
A  implementation-report-architecture-correction.md
```

## Documentation Updated

All updates are listed in "Files Modified" above. The
summary by document:

- `AGENTS.md` — Rule 14 added; rule count renamed
  "Twelve" → "Fourteen".
- `ARCHITECTURE.md` — system diagram; § 2.5 (new);
  § 2.6 (new); § 3.3; § 4.1; § 4.2; § 4.3.
- `DECISIONS.md` — ADR-011, ADR-012, ADR-013 added; ADR
  index updated.
- `ROADMAP.md` — M1–M8 self-dogfooding subsections; M1
  project set; new § 4 matrix; § 4 → § 5; § 5 → § 6.
- `STYLEGUIDE.md` — interface example updated.
- `docs/architecture-principles.md` — § 2 project map;
  § 4.1; § 4.2; § 4.3; § 4.6 (new).
- `docs/folder-structure.md` — top-level layout; § 3
  restructured per project; § 4 tests restructured; § 7
  co-location updated.
- `docs/naming-conventions.md` — provider contract row;
  registry row; § 3.2 capability-oriented list.
- `docs/provider-guidelines.md` — § 2, § 3.2, § 4.1,
  § 4.2, § 5.1, § 5.2, § 6.3, § 8.1, § 9, § 10.
- `.ai/prompts/bootstrap.md` — § 5; § 8.
- `.ai/prompts/feature.md` — § 5.
- `.ai/prompts/provider.md` — § 1; § 5; § 9.
- `.ai/prompts/review.md` — § 5.1.1 (new); § 5.1.2
  (new); § 5.5.1 (new).
- `.ai/workflows/feature-lifecycle.md` — Stage 2.
- `.ai/workflows/provider-onboarding.md` — Stage 1;
  Stage 3; Stage 4; Stage 9 (new).
- `.ai/workflows/tool-dogfooding.md` — § 1.2; § 1.3
  (new).

## Deviations

1. **`ROADMAP.md` "How to Read This Roadmap" and the
   milestone map were not renumbered.** The M0–M8
   milestone numbers are unchanged from the prior state;
   only § 4, § 5, § 6 in the lower-numbered sections
   were renumbered. Reason: the task brief resolved a
   contradiction in favour of keeping the existing
   milestone numbering for stability, and the new
   progressive self-dogfooding content was added as
   per-milestone subsections rather than as new
   milestones.
2. **The progressive self-dogfooding matrix in
   `ROADMAP.md` § 4 contains a row for
   `IAutonomousLoopProvider` that says "deferred to a
   later milestone".** The matrix is the authoritative
   reference; the row is included so the family is
   accounted for and a later milestone that needs an
   autonomous loop has the row already to update. The
   implementation row is empty because the implementation
   is intentionally deferred.
3. **A new project layout document was not added to
   `docs/`.** The project map lives in
   `docs/folder-structure.md` § 3.0 and in
   `ARCHITECTURE.md` § 2.5. A standalone
   `docs/project-layout.md` is not justified by the
   three-files rule (`docs/folder-structure.md` is the
   right place for layout, and `ARCHITECTURE.md` is the
   right place for the rationale).
4. **No `docs/coding-standards.md` change.** The
   existing rules in `STYLEGUIDE.md` and
   `docs/coding-standards.md` already cover the
   IProcessRunner / IProvider / family-naming rules by
   reference to the new ADRs; no separate standard was
   added.
5. **No `docs/component-guidelines.md` change.** The
   component guidelines are about the design system; the
   architecture correction is about provider families and
   project boundaries, not components.

## Known Limitations

- The progressive self-dogfooding matrix in `ROADMAP.md`
  § 4 is enforced **by future architecture tests**, not
  by tests in this session. The architecture tests
  (`No_DirectProcessStart_OutsideInfrastructure`,
  `App_DoesNotReference_Providers_Implementations`,
  `Pages_Resolve_Providers_Through_Registry`,
  `ConversationService_ResolvesThrough_Registry`,
  `NativeProviders_Use_Contracts_Not_Implementations`,
  `No_Secrets_In_Logs`, `No_Secrets_In_Configuration`,
  `No_WorkspaceProvider_Family_Reintroduced`,
  `QualityGate_Resolves_Through_Registry`,
  `Review_Resolves_Through_Registry`,
  `AutonomousLoop_Resolves_Through_Registry`,
  `Orchestration_Resolves_Through_Registry`,
  `Pages_Use_DesignSystem_Components_Not_DOM`,
  `WorkspaceService_IsScoped`) are placeholders until
  M1 lands. The matrix records the names; the tests
  are written when the layer that triggers them ships.
- The `docs/provider-guidelines.md` § 10 catalogue
  anticipates providers that the platform has not yet
  implemented. A provider that does not yet exist is
  listed with its planned milestone; the row is removed
  (not struck through) when the provider is added.
- The `git status --short` step is reported as
  "repository is not a git repository" with an equivalent
  inventory, rather than literal output. The change is
  observable; the inventory is exact.

## Next Recommended Step

Begin **M1 — Design System Core** under
`.ai/prompts/bootstrap.md`. The bootstrap session must:

1. Produce an `implementation-plan.md` using
   `.ai/templates/implementation-plan.md` that includes
   the project boundary review (per ADR-011) and the
   progressive self-dogfooding row for the design
   system (per ADR-013).
2. Create the canonical solution `AiEng.Platform.sln`
   and the five source projects plus the four test
   projects, exactly as documented in
   `ARCHITECTURE.md` § 2.5 and `ROADMAP.md` M1.
3. Land the design-system documentation page
   (`.ai/prompts/ui.md`) with the primitive, container,
   and feedback components listed in `ROADMAP.md` M1.
4. Write the architecture tests named in the matrix in
   `ROADMAP.md` § 4 for the M1 row, so the build fails
   the moment any later milestone imports a provider
   implementation directly from `App`.
5. Stop at the M1 dogfooding checkpoint (Lavish Axi
   for UI review, per `.ai/workflows/tool-dogfooding.md`).

The M1 dogfooding checkpoint (Lavish Axi for UI review)
becomes available once the documentation page renders.
Do not invoke Lavish Axi until that point.

## Linked Artefacts

- `AGENTS.md` — Rule 14 (Progressive Self-Dogfooding).
- `ARCHITECTURE.md` — § 2.5 (Solution and Project
  Boundaries), § 2.6 (Progressive Self-Dogfooding),
  § 3.3, § 4.1, § 4.2, § 4.3.
- `DECISIONS.md` — ADR-011, ADR-012, ADR-013, ADR index.
- `ROADMAP.md` — M1 project set; M1–M8 self-dogfooding
  subsections; § 4 matrix.
- `STYLEGUIDE.md` — interface example.
- `docs/architecture-principles.md` — § 2, § 4.1, § 4.2,
  § 4.3, § 4.6.
- `docs/folder-structure.md` — top-level layout; § 3
  per-project; § 4 tests; § 7 co-location.
- `docs/naming-conventions.md` — provider contract,
  registry, § 3.2.
- `docs/provider-guidelines.md` — § 2, § 3.2, § 4.1,
  § 4.2, § 5.1, § 5.2, § 6.3, § 8.1, § 9, § 10.
- `.ai/prompts/bootstrap.md` — § 5, § 8.
- `.ai/prompts/feature.md` — § 5.
- `.ai/prompts/provider.md` — § 1, § 5, § 9.
- `.ai/prompts/review.md` — § 5.1.1, § 5.1.2, § 5.5.1.
- `.ai/workflows/feature-lifecycle.md` — Stage 2.
- `.ai/workflows/provider-onboarding.md` — Stage 1,
  Stage 3, Stage 4, Stage 9.
- `.ai/workflows/tool-dogfooding.md` — § 1.2, § 1.3.
- `.ai/templates/implementation-report.md` — the
  template used to produce this report.
