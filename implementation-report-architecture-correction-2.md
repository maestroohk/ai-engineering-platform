# Implementation Report — Architecture Correction 2 (Composition Root, M3 Persistence, M4 Slicing)

> Produced using `.ai/templates/implementation-report.md` as
> the only output of the architecture correction session.
> This session is documentation-only: no source code, no
> packages installed, no external tools invoked, M1 not
> started.

---

## Summary

Resolved five architecture issues that were identified
before M1 implementation could begin:

1. **CORRECTION 1 — Composition root may register
   multiple provider implementations.** The previous rule
   ("the `App` project may reference at most one provider
   implementation project") conflated two distinct
   concerns: how many providers the host can register
   (an arbitrary number), and where concrete provider
   references are allowed (only the composition root).
   The new rule, recorded in a new ADR-016, replaces
   "at most one" with "the composition root
   (`AiEng.Platform.App/Composition/`) is the only place
   in the solution that may reference a
   `Providers.<X>` project directly." The
   `App/Composition/` folder is the registration site
   for the lifetime of the platform; pages, components,
   application services, view models, DTOs, and domain
   types resolve providers through the registry, never
   through direct reference.
2. **CORRECTION 2 — M3 persistence contradiction fixed.**
   M3 ships an in-memory `IProjectStore` and is
   explicitly non-durable: projects do not survive an
   application restart. M4-A introduces the on-disk
   `IProjectStore` implementation that replaces the M3
   in-memory store behind the same contract. The M3 / M4
   boundary is the contract, not the storage medium. No
   persistence technology is selected in M0; the choice
   is deferred to M4-A.
3. **CORRECTION 3 — M4 split into M4-A / M4-B / M4-C /
   M4-D slices.** The previous M4 combined four
   independent deliverables (Infrastructure, capability
   detection, provider registry, first concrete
   providers) into a single undifferentiated milestone
   that was too large to review and too large to roll
   back. The four slices each have their own
   prerequisites, outcome, deliverables, excluded scope,
   definition of done, tests, dogfooding checkpoint, and
   handoff. M4-D is the milestone that activates the
   four composition-root architecture tests and the
   `No_DirectProcessStart_OutsideInfrastructure` test.
4. **CORRECTION 4 — Provider lifecycle states
   documented.** Five states — `Compiled-in`, `Registered`,
   `Enabled`, `Healthy`, `Selected` — are defined in
   ADR-016, `ARCHITECTURE.md` § 4.5,
   `docs/architecture-principles.md` § 4.6, and
   `docs/provider-guidelines.md` § 4.6. Registration
   (the DI wiring) is unconditional; enablement is
   configuration-section-driven; health is the live
   status; selection is the consumer's choice at the
   point of use.
5. **CORRECTION 5 — M1 scope preserved.** M1 is
   unchanged in body and DoD. M1 may create the solution,
   four source projects, three test projects, the Blazor
   app, Tailwind, the token system, semantic classes, the
   base design-system components, the design-system docs
   page, bUnit tests, and the architecture tests. M1 may
   not create concrete provider implementations, the
   provider registry behaviour, process execution, the
   `IProjectStore` (M3 introduces the in-memory store;
   M4-A migrates to durable storage), Git or Ollama
   integration, worktrees, runtime launching, review
   providers, or quality gates. The four composition-root
   architecture tests are **registered but disabled** in
   M1 (their bodies exist with explicit skip messages
   citing ADR-016 and M4-D); the
   `No_DirectProcessStart_OutsideInfrastructure` test
   follows the same pattern.

The session is documentation-only. No `.cs`, `.razor`,
`.csproj`, or `.sln` files were created. No packages
were installed. No external tools were invoked. M1 was
not begun.

---

## Files Modified

### `ARCHITECTURE.md`

- Replaced the "App → at most one provider
  implementation project" rule with the new
  composition-root rule (per ADR-016): the composition
  root may register any number of provider
  implementations, and only the composition root may
  reference a `Providers.<X>` project directly.
- Added a new "The Composition Root" section with
  the may / may-not list, the project-reference graph,
  and the architecture test.
- Added new § 4.5 "Provider Lifecycle States" with
  the five states: `Compiled-in`, `Registered`,
  `Enabled`, `Healthy`, `Selected`.
- Updated § 2.5 "How architecture tests complement
  compile-time boundaries" from three rules to four
  rules (added the composition-root rule as rule #3).

### `DECISIONS.md`

- Added ADR-016 ("Composition root may register
  multiple provider implementations; document the five
  provider lifecycle states") to the index.
- Updated ADR-011 `Superseded by: None` →
  `Superseded by: ADR-016 (the composition-root
  clause only; the rest of ADR-011 stands)`.
- Replaced the "at most one" bullet in ADR-011 with
  the composition-root clause.
- Added a new forbidden-reference entry: "A page,
  component, application service, view model, DTO,
  or domain type referencing a `Providers.<X>`
  project."

### `ROADMAP.md`

- Replaced M4 with four slices: M4-A (Infrastructure /
  Process Execution), M4-B (Capability Detection),
  M4-C (Provider Registry Foundation with fake
  providers), M4-D (First Concrete Process Providers).
  Each slice has prerequisites, outcome, outcome in
  detail, excluded scope, definition of done, tests
  added, dogfooding checkpoint, and handoff.
- Updated § 2 Milestone Map to expand the M4 row
  into four M4-X rows.
- Updated M3 to explicitly state the M3 store is
  in-memory and does **not** persist; the M3 / M4
  boundary is the contract, not the storage medium.
  Removed the prior "persists across application
  restart" claim.
- Updated M1 to (a) call out the four composition-root
  architecture tests as **registered but disabled** in
  M1, and (b) add an explicit "Out of scope for M1
  (verified preserved)" block listing every artefact
  that M1 must not create.
- Updated § 4 (Progressive Self-Dogfooding Matrix) to
  add a row for each of the four composition-root
  architecture tests, a row for the composition root
  itself, and to clarify that
  `No_DirectProcessStart_OutsideInfrastructure` is
  registered but disabled in M1 through M4-C and
  activates in M4-D.

### `docs/architecture-principles.md`

- Inserted § 4.5 "The Composition Root" (definition,
  may / may-not list, the four architecture tests).
- Inserted § 4.6 "Provider Lifecycle States" (the five
  states).
- Renumbered existing § 4.6 "Progressive Self-Dogfooding"
  to § 4.7.
- Updated § 12.2 "Architecture Tests" to list the four
  composition-root tests.

### `docs/folder-structure.md`

- Added `Composition/` to the top-level layout tree.
- Added new § 3.15 "`AiEng.Platform.App/Composition/`"
  describing the composition root folder structure
  (`Composition/Ollama/`, `Composition/Git/`,
  `Composition/Terminal/`, `Composition/Worktree/`,
  `Composition/Review/`, `Composition/QualityGate/`).

### `docs/provider-guidelines.md`

- Inserted § 4.6 "Provider Lifecycle States" with
  the five states.
- Replaced § 5 ("Provider Registration") with § 5.0
  ("The Composition Root"), § 5.1 ("The Registration
  Extension"), § 5.2 ("The Registry"), § 5.3
  ("Configuration-Driven Enablement"). The new § 5
  includes a multi-provider example showing
  `AddOllamaLaunchProvider`, `AddGitProvider`,
  `AddPowerShellProvider`, etc. wired through the
  composition root.
- Updated § 9 Authoring checklist step 7: the
  registration extension lives in
  `AiEng.Platform.App/Composition/<Capability>/`; step
  8: the registration call is unconditional (enablement
  is configuration-driven).

### `.ai/prompts/provider.md`

- Updated § 5 Planning Requirements: the plan must call
  out the registration extension under
  `App/Composition/<Capability>/` and document which of
  the five lifecycle states the new provider will reach.
- Updated § 6 Implementation Boundaries: added the
  composition-root rule (no source file outside
  `App/Composition/` may reference a `Providers.<X>`
  project).
- Updated § 7 Validation: the four composition-root
  architecture tests are green.
- Updated § 10 Prohibited Shortcuts: added two new
  shortcuts (importing `Providers.<X>` from anywhere
  outside `App/Composition/`, and bypassing the
  registry).

### `.ai/prompts/feature.md`

- Updated § 5 Planning Requirements: added the
  composition-root block — the feature may not import
  any `Providers.<X>` project directly; all provider
  resolution is through the registry.
- Updated § 6 Implementation Boundaries: explicitly
  extends "no component references a provider" to
  "no component, page, or application service
  references a provider implementation directly."
- Updated § 10 Prohibited Shortcuts: added the two
  composition-root shortcuts.

### `.ai/prompts/review.md`

- Added § 5.1.1a "Composition Root (per ADR-016)" —
  a new review dimension with five questions (where
  is the registration extension; are the four
  composition-root tests green; is the resolution
  through the registry; etc.).
- Updated § 5.1.2 to add a new bypass example:
  bypassing the composition root by importing a
  `Providers.<X>` project from a page, component, or
  service.

### `.ai/prompts/bootstrap.md`

- Updated § 5 Planning Requirements: if the bootstrap
  introduces a new `Providers.<X>` project, the plan
  must call out the registration extension under
  `App/Composition/<Capability>/` and the four
  composition-root architecture tests.
- Updated § 6 Implementation Boundaries: a
  `Providers.<X>` project introduced by a bootstrap
  may not be referenced from anywhere outside
  `App/Composition/`.

### `.ai/prompts/testing.md`

- Updated § 7.5 "Architecture Tests" with the
  composition-root rule and the four tests, and
  explained the "registered but disabled" pattern
  used in M1 / M2 / M3 / M4-A / M4-B / M4-C with
  activation in M4-D.

### `.ai/prompts/ui.md`

- Updated § 6 Implementation Boundaries with the
  "no provider implementation in components or pages"
  rule (per ADR-016): a component that needs a
  provider resolves it through the registry.

---

## Files Created

None. The session is documentation-only.

---

## Files Deleted

None.

---

## Reusable Components Introduced

None. The session is documentation-only. The following
component names are now documented in `ROADMAP.md` and
`docs/folder-structure.md` for future slices but are
not yet implemented:

- `AppProviderCard` (M4-C)
- `AppHealthDot` (M4-C)
- `AppCapabilityList` (M4-B)
- `AppKeyValueList` (M4-B)

---

## Services Introduced

None. The session is documentation-only. The following
services are now documented for future slices but are
not yet implemented:

- `IProviderRegistry` (M4-C)
- `IAgentRuntimeProviderRegistry` and the matching
  family-scoped registries (M4-C)
- `IProviderHealthService` (M4-C)
- `IHostCapabilitiesService` and `HostCapabilities`
  (M4-B)
- `IProcessRunner` (M4-A)
- `ICredentialVault` (M4-A)
- `IClock` (M4-A)
- `IProjectStore` on-disk implementation (M4-A,
  replacing the M3 in-memory store behind the same
  contract)

---

## Providers Touched

None. The session is documentation-only. The two
concrete process-boundary providers
(`GitProvider` and `OllamaLaunchProvider`) are
documented in M4-D for future implementation but are
not yet onboarded.

---

## Tests Added

None implemented. The session registers the following
test names for future milestones:

- `Only_CompositionRoot_MayReference_ConcreteProviders`
  (M4-D; registered but disabled in M1 / M2 / M3 /
  M4-A / M4-B / M4-C)
- `Pages_DoNotReference_ConcreteProviders` (M4-D;
  registered but disabled in M1 / M2 / M3 / M4-A /
  M4-B / M4-C)
- `Application_DoesNotReference_ConcreteProviders`
  (M4-D; registered but disabled in M1 / M2 / M3 /
  M4-A / M4-B / M4-C)
- `Components_DoNotInject_ConcreteProviders` (M4-D;
  registered but disabled in M1 / M2 / M3 / M4-A /
  M4-B / M4-C)
- `No_DirectProcessStart_OutsideInfrastructure`
  (M4-D; registered but disabled in M1 / M2 / M3 /
  M4-A / M4-B / M4-C)

---

## Commands Run

The session ran no commands. The session is
documentation-only by mandate.

---

## Validation Results

The 15-step validation produced the following results:

| #   | Step                                                                  | Result |
| --- | --------------------------------------------------------------------- | ------ |
| 1   | `ARCHITECTURE.md` has the new composition-root rule (no "at most one" in non-historical text) | PASS |
| 2   | `ARCHITECTURE.md` has the new "Composition Root" section | PASS |
| 3   | `ARCHITECTURE.md` has the new § 4.5 with the five lifecycle states | PASS |
| 4   | `DECISIONS.md` index lists ADR-016 | PASS |
| 5   | `DECISIONS.md` ADR-011 is marked `Superseded by: ADR-016 (the composition-root clause only; the rest of ADR-011 stands)` | PASS |
| 6   | `DECISIONS.md` has the ADR-016 text with status `Accepted` | PASS |
| 7   | `ROADMAP.md` M3 is in-memory and explicitly does not persist; M4-A is the durable migration | PASS |
| 8   | `ROADMAP.md` M4 is split into M4-A / M4-B / M4-C / M4-D with the documented fields per slice | PASS |
| 9   | `ROADMAP.md` M1 has the "Out of scope for M1 (verified preserved)" block | PASS |
| 10  | `ROADMAP.md` § 4 matrix includes rows for the four composition-root tests | PASS |
| 11  | `docs/architecture-principles.md` has new § 4.5 and § 4.6 with the right renumbering | PASS |
| 12  | `docs/folder-structure.md` has the `Composition/` entry in the top-level layout and § 3.15 | PASS |
| 13  | `docs/provider-guidelines.md` has the new § 4.6 (lifecycle states) and the new § 5 (composition root) | PASS |
| 14  | `.ai/prompts/{provider,feature,review,bootstrap,testing,ui}.md` each reference the composition root where appropriate | PASS |
| 15  | The session did not create the .NET solution, did not create any `.cs`, `.razor`, `.csproj`, or `.sln` files, did not install packages, did not invoke external tools, and did not begin M1 | PASS |

Cross-cutting checks:

- No file under the repository mentions "SQLite" or
  "sqlite" (no persistence technology was selected).
- No file under the repository contains the phrase
  "persists across application restart" applied to
  M3 (the contradiction is fixed).
- The only "at most one" references that remain are
  historical ("the previous 'at most one' restriction
  is removed") in `DECISIONS.md` ADR-011 and ADR-016.
- The four composition-root architecture tests are
  named consistently across `ARCHITECTURE.md`,
  `DECISIONS.md`, `ROADMAP.md`,
  `docs/architecture-principles.md`, and
  `.ai/prompts/{provider,feature,review,bootstrap,testing}.md`.
- The five lifecycle states
  (`Compiled-in`, `Registered`, `Enabled`, `Healthy`,
  `Selected`) are named consistently across
  `ARCHITECTURE.md`, `DECISIONS.md` (ADR-016),
  `ROADMAP.md` (M4-C),
  `docs/architecture-principles.md` § 4.6, and
  `docs/provider-guidelines.md` § 4.6.

---

## Documentation Updated

- `ARCHITECTURE.md` — composition root section, § 4.5
  lifecycle states, § 2.5 fourth rule.
- `DECISIONS.md` — ADR-016 added; ADR-011
  composition-root clause updated; index updated.
- `ROADMAP.md` — M3 persistence corrected; M4 split
  into M4-A / M4-B / M4-C / M4-D; M1 out-of-scope
  block added; § 2 milestone map updated; § 4 matrix
  updated with the four composition-root rows.
- `docs/architecture-principles.md` — new § 4.5 and
  § 4.6; renumbering; § 12.2 architecture tests
  updated.
- `docs/folder-structure.md` — `Composition/` added
  to the top-level layout; new § 3.15.
- `docs/provider-guidelines.md` — new § 4.6
  lifecycle states; § 5 replaced with the
  composition-root structure; § 9 authoring
  checklist updated.
- `.ai/prompts/provider.md` — composition root and
  lifecycle states referenced in § 5, § 6, § 7, § 10.
- `.ai/prompts/feature.md` — composition root and
  registry resolution referenced in § 5, § 6, § 10.
- `.ai/prompts/review.md` — new § 5.1.1a
  "Composition Root" review dimension; § 5.1.2
  bypass examples updated.
- `.ai/prompts/bootstrap.md` — composition root
  referenced in § 5 and § 6.
- `.ai/prompts/testing.md` — § 7.5 architecture
  tests updated with the composition-root rule and
  the four tests.
- `.ai/prompts/ui.md` — § 6 implementation boundaries
  extended with the registry resolution rule.

---

## Deviations

- **Deviation 1 — historical "at most one" retained
  verbatim.** The string "at most one" is preserved in
  `DECISIONS.md` inside ADR-011 and ADR-016, where it
  appears as the historical record of the removed rule.
  This is intentional; the historical record is part of
  the ADR. The string does not appear in any rule
  currently in force.
- **Deviation 2 — `App_DoesNotReference_Providers_Implementations`
  test name preserved.** The pre-existing
  `App_DoesNotReference_Providers_Implementations`
  test is still named in `ROADMAP.md` (M1) and
  historical implementation reports. The four new
  composition-root tests are added on top of it,
  giving the same coverage as the pre-existing test
  but split into four finer-grained rules. No
  backward-incompatible change is intended; a future
  M1 session can keep the pre-existing test, rename
  it, or split it, and the four new tests are the
  composition-root contract that activates in M4-D.
- **Deviation 3 — `Out of scope for M1 (verified
  preserved)` block added explicitly.** M1 was not
  modified in body (it is preserved), but an explicit
  "Out of scope for M1" block was added so that the
  milestone is unambiguously closed against scope
  creep.

---

## Known Limitations

- The composition-root architecture tests are
  **registered but disabled** in M1 / M2 / M3 / M4-A
  / M4-B / M4-C and **activate** in M4-D. The exact
  test bodies are not in scope for this session;
  they are documented in
  `docs/architecture-principles.md` § 4.5 and
  `.ai/prompts/testing.md` § 7.5. M4-D is the
  session that materialises the test bodies and flips
  them to active.
- The `IProviderRegistry` and family-scoped registries
  are documented but not yet implemented; M4-C is the
  session that materialises them. The fakes mentioned
  in M4-C are not yet implemented.
- The "Ollama Launch UI" remains in M6. M4-D ships
  the `OllamaLaunchProvider` as a smoke test of the
  process boundary, not a full launch-flow UI.
- No persistence technology is selected in M0; the
  choice is deferred to M4-A. The on-disk
  `IProjectStore` implementation may use a file-based
  store, SQLite, LiteDB, or any other technology the
  M4-A session selects; the choice is documented
  when the slice lands.

---

## Next Recommended Step

Begin **M1** in a new session, following the
unchanged M1 body in `ROADMAP.md` and the explicit
"Out of scope for M1 (verified preserved)" block
added in this session. M1 may now proceed with the
four source projects, three test projects, Blazor
app, Tailwind, tokens, semantic classes, the base
design-system components, the design-system docs
page, bUnit tests, and the architecture tests. M1
must not create concrete provider implementations,
the provider registry behaviour, process execution,
persistence, the `IProjectStore`, Git or Ollama
integration, worktrees, runtime launching, review
providers, or quality gates.

The single most important opening action for the
M1 session is to create the solution and the four
source projects (`AiEng.Platform.App`,
`AiEng.Platform.Application`, `AiEng.Platform.Domain`,
`AiEng.Platform.Providers.Abstractions`) and the
three test projects (`AiEng.Platform.UnitTests`,
`AiEng.Platform.ComponentTests`,
`AiEng.Platform.ArchitectureTests`), then add the
four composition-root architecture tests as
**registered but disabled** with skip messages
citing ADR-016 and M4-D as the activation point.

---

## Linked Artefacts

- `ARCHITECTURE.md` — composition root section, § 4.5
  lifecycle states.
- `DECISIONS.md` — ADR-016
  ("Composition root may register multiple provider
  implementations; document the five provider
  lifecycle states").
- `ROADMAP.md` — M3 in-memory store, M4-A / M4-B /
  M4-C / M4-D slices, M1 "Out of scope" block, § 4
  matrix composition-root rows.
- `docs/architecture-principles.md` — § 4.5 and
  § 4.6; renumbered § 4.7; § 12.2 architecture
  tests.
- `docs/folder-structure.md` — `Composition/` entry
  and § 3.15.
- `docs/provider-guidelines.md` — § 4.6 lifecycle
  states; § 5 composition root, registration
  extension, registry, configuration-driven
  enablement; § 9 authoring checklist.
- `.ai/prompts/provider.md` — § 5 / § 6 / § 7 / § 10
  updated.
- `.ai/prompts/feature.md` — § 5 / § 6 / § 10
  updated.
- `.ai/prompts/review.md` — § 5.1.1a added; § 5.1.2
  updated.
- `.ai/prompts/bootstrap.md` — § 5 / § 6 updated.
- `.ai/prompts/testing.md` — § 7.5 updated.
- `.ai/prompts/ui.md` — § 6 updated.
