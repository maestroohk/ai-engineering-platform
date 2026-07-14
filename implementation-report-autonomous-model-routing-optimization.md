# Implementation Report — Autonomous Model Routing Optimisation

> **One-time AI session-efficiency task (T-031).**
> Not a product feature. The deliverable is a
> Windows-first PowerShell supervisor that drives
> non-interactive Claude Code children, one bounded
> phase at a time, each on a different configured
> cloud model.

## 1. Summary

Shipped the AI session router operating layer:

- **PowerShell 5.1+ supervisor** at
  `tools/ai-session-router.ps1` (Windows-first;
  `powershell.exe`, not `pwsh`).
- **Configurable cloud models** in
  `.ai/model-routing.json` (`high` / `standard` /
  `economy` / `review` / `fallback` profiles).
- **Phase-specific prompts** in
  `.ai/prompts/phases/{reconcile,plan,implement,validate,document,review,closeout}.md`.
- **Phase receipts** at
  `.ai/receipts/phases/<task-id>-<phase>.json`
  (schema at
  `.ai/templates/phase-receipt.schema.json`).
- **Compact active-task context** at
  `.ai/context/active-task.json` (with
  `repository-map.json` and `validation-cache.json`).
- **Mocked Pester tests** at
  `tools/ai-session-router.Tests.ps1`. No real Ollama
  invocation; no paid cloud quota consumed.
- **Dry-run end-to-end simulations** print the
  `ollama launch claude --model <model> -y -- -p
  "<prompt>"` form without executing it.

The supervisor stops at `closeout`. The user must
invoke `Next` again to begin the next task. The next
Ready task remains **T-030 (M4-C closeout)**.

## 2. Files Added (33)

| Path | Purpose |
| ---- | ------- |
| `tools/ai-session-router.ps1` | Production supervisor. |
| `tools/ai-session-router.example.ps1` | Runnable example. |
| `tools/ai-session-router.Tests.ps1` | Mocked Pester tests. |
| `tools/README.md` | One-page readme for `tools/`. |
| `.ai/model-routing.json` | Live model profile config. |
| `.ai/model-routing.example.json` | Annotated example. |
| `.ai/model-routing.schema.json` | JSON Schema. |
| `.ai/model-classification.md` | Human-readable rules. |
| `.ai/model-classification.json` | Machine-readable rules. |
| `.ai/prompts/phases/{reconcile,plan,implement,validate,document,review,closeout}.md` | 7 phase prompts. |
| `.ai/receipts/phases/README.md` | Receipts readme. |
| `.ai/templates/phase-receipt.schema.json` | Per-phase receipt schema. |
| `.ai/templates/implementation-receipt.schema.json` | Per-task implementation receipt schema. |
| `.ai/context/{README.md, repository-map.json, active-task.json, validation-cache.json}` | Context directory (4 files). |
| `.ai/index/{reports,plans,handoffs}.json` | Indexes (3 files). |
| `.ai/archive/README.md` | Archive readme. |
| `.ai/benchmarks/model-routing/{README.md, benchmark-tasks.json, results.json}` | Benchmark stubs (3 files). |
| `.ai/backlog/ai-session-router.md` | Future in-platform Blazor feature backlog. |
| `implementation-report-autonomous-model-routing-optimization.md` | This file. |

## 3. Files Modified (5)

- `DECISIONS.md` — ADR-017 appended.
- `ROADMAP.md` — additive note in § 4 (matrix
  unchanged) and § 5 (Future AI Session Router row).
- `.ai/commands.md` — `Routed Next` section added;
  existing nine commands and 13-step lifecycle
  unchanged.
- `.ai/session-start.md` — `Router-managed startup`
  section added; existing § 1–6 unchanged.
- `.ai/state/*` — incremental state update
  (session.json, tasks.json, current.md, task-board.md,
  milestones.json, capabilities.json).

## 4. Validation

| Gate | Result |
| ---- | ------ |
| JSON parse (every `.ai/**/*.json`) | OK |
| PowerShell syntax (3 scripts in `tools/`) | OK |
| Router `-Command Status` | OK |
| Router `-Command Next -DryRun -ProfileOverride high` | OK (prints the `ollama launch claude` form) |
| Router `-Command Plan -DryRun` | OK |
| Router `-Command Resume` | OK |
| Router `-Command Next -ProfileOverride standard` | Expected stop: `CONFIGURE_ME` for standard profile |
| Router `-Command Configure` | OK (writes UTF-8 without BOM) |

The Pester tests in
`tools/ai-session-router.Tests.ps1` are mocked and do
not invoke `ollama launch claude`.

## 5. Safety

- Argument list (`ProcessStartInfo.ArgumentList`),
  not string concatenation. No `Invoke-Expression`.
- Model name and task ID validated against a strict
  regex (`^[A-Za-z0-9._:-]+$` and `^T-[0-9]+$`).
- Paths validated against the repository root.
- Ctrl+C cancels the supervisor and terminates the
  child process tree (via
  `Register-EngineEvent` + `finally` block).
- 429 / usage exhaustion selects the configured
  `fallback` profile when allowed; otherwise the
  supervisor stops cleanly.
- The supervisor respects
  `execution.push_authorization_required` and never
  pushes to a remote without an explicit `-NoPush`
  override.

## 6. Git

A single focused commit will be created on
`feature/T-031-ai-session-router`, fast-forwarded into
`main`, the branch deleted locally, and **no push**.
The commit subject and body follow the
`feat(<scope>): ...` convention.

## 7. Backlog (Future)

The in-platform Blazor `AiEng.Platform` AI Session
Router is **Deferred**. It lands in a future milestone
when the platform is ready. See
`.ai/backlog/ai-session-router.md` and
`DECISIONS.md` ADR-017. The operating-layer PowerShell
supervisor is the bridge that exists today; the
service abstraction in the backlog
(`IAiSessionRouter`, `IModelRoutingPolicy`,
`IAgentSessionLauncher`,
`ModelRoutingConfiguration`,
`TaskExecutionPipeline`) will consume the same
logical model.

## 8. Linked Artefacts

- `.ai/plans/ai-session-efficiency-and-model-routing.md`
  (the approved plan, 767 lines, 24 sections).
- `DECISIONS.md` ADR-017.
- `ROADMAP.md` § 4 note + § 5 deferred row.
- `.ai/commands.md` § 11 (Routed Next).
- `.ai/session-start.md` § 7 (Router-managed startup).
- `.ai/state/tasks.json` T-031 (Done).
- `.ai/state/session.json` envelope
  (`autonomous-model-routing-optimization`).
- `.ai/state/milestones.json` M-Router (Done).
- `.ai/state/capabilities.json` C-027 (Deferred).

## 9. Next Recommended Command

```
.\tools\ai-session-router.ps1 -Command Status
```

The user must run `-Command Configure` once to set
the `standard` and `economy` model names
(currently `CONFIGURE_ME`). The next **product**
session remains T-030 (M4-C closeout) on the user's
`Approve` or `Next` invocation. The router does not
begin T-030 in this task.
