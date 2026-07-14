# Plan — Autonomous Cloud Model Routing Optimisation

> **One-time AI session-efficiency task.**
> This is not a product feature. No `src/` code, no test
> code, no milestone reordering, and no accepted architecture
> change ships in this task. The deliverable is a Windows-first
> PowerShell supervisor that drives non-interactive Claude Code
> children, one bounded phase at a time, each on a different
> configured cloud model.

---

## Context

The current command-driven workflow (per `.ai/commands.md`)
is reliable but the `Next` command consumes too many tokens
because one powerful model performs every phase: repository
understanding, planning, implementation, tests, reporting,
state reconciliation, and Git closeout. The developer must
not have to close a terminal and manually relaunch Claude
Code with another model during a normal task.

The objective is to preserve quality while allowing different
Ollama cloud models to perform different phases automatically.
The platform is currently in M4-C closeout (T-030 is the next
`Ready` task; the active milestone is M4-C; the next
milestone is M4-D; the last completed task is T-029 / M4-C.2
delivered 2026-07-13). This task does **not** begin T-030,
M4-C closeout, M4-D, or any product feature work.

The deliverable is a router that:

1. Inspects the current task packet.
2. Classifies the work.
3. Selects a configured cloud model.
4. Launches Claude Code non-interactively for one bounded
   phase.
5. Persists the phase result into repository state.
6. Exits that model session.
7. Launches the next phase with another configured model.
8. Continues until the single selected task is closed.
9. Stops before beginning another task.

`ollama launch claude --model <model> --yes -- -p "<prompt>"`
is the non-interactive form. Model switching is handled by
an external supervisor, not by hot-swapping a running
Claude Code process. No interactive terminal model selectors,
no hardcoded model names in scripts.

The platform must support a `Routed Next` execution form
in addition to the existing `Interactive Next`. The existing
nine commands (`Continue`, `Approve`, `Status`, `Plan`,
`Resume`, `Review`, `Validate`, `Finish`, `Next`) and the
existing 13-step lifecycle are unchanged.

---

## Selected Approach

Ship a **Windows-first PowerShell 5.1+ supervisor** at
`tools/ai-session-router.ps1` that reads a per-task
**active task packet** (`.ai/context/active-task.json`),
classifies the work to a **model profile** (`high` /
`standard` / `economy` / `review` / `fallback`), resolves
a configured **cloud model name** from
`.ai/model-routing.json`, and launches one Claude Code
child per phase through the non-interactive
`ollama launch claude --model <model> --yes -- -p "<phase prompt>"`
form. The supervisor captures the exit code, parses the
phase receipt (`phase-receipt.schema.json`), and decides
whether to advance, retry, escalate, or stop.

The router is the **only** place that decides which model
performs which phase. The router never hot-swaps a running
Claude Code process. The router never invokes a model
twice for the same phase. The router never begins a second
task.

The architecture follows these principles:

- **Configuration is data, not code.** Model names, profile
  purposes, retry counts, timeouts, and budgets live in
  `.ai/model-routing.json` validated by
  `.ai/model-routing.schema.json`. The script reads the
  configuration; the user changes the configuration.
- **One model per phase.** The router launches exactly one
  Claude Code child per phase. The child is given a
  **phase-specific, bounded prompt** from
  `.ai/prompts/phases/<phase>.md`. The child is forbidden
  from performing another phase. The child writes a
  **phase receipt** at
  `.ai/receipts/phases/<task-id>-<phase>.json` validated by
  `.ai/templates/phase-receipt.schema.json`.
- **Safe process supervision.** No `Invoke-Expression`. No
  string concatenation into a command. Arguments are passed
  as an argument list. Model names and task IDs are
  validated against a strict regex. Paths are resolved to
  absolute paths under the repository root; paths outside
  the root are rejected. Ctrl+C cancels the supervisor and
  terminates the child process tree.
- **Bounded retries.** The default policy: one retry with
  the same profile, one escalation to `fallback` or `high`,
  then stop. The router records the failure mode, preserves
  the exact phase and handoff, and allows `Resume` after
  limits reset. A 429 / usage-exhaustion response
  short-circuits the retry and selects `fallback` (if
  configured) or stops cleanly.
- **Cloud-only mode is the default.** The router rejects
  local-only models when `execution.cloud_only` is `true`.
  The router never assumes GPU, VRAM, or local server
  availability. The UI design for future product
  integration still permits local models later (per
  `PRODUCT.md` `IAgentRuntimeProvider` registry), but the
  current operational router is cloud-first.
- **Configuration persistence.** A `-Configure` invocation
  asks for the model names once and persists them to
  `.ai/model-routing.json`. The router does not ask the
  user for model names during every task.
- **No secrets.** No Ollama credentials, no API keys, no
  tokens. The router uses the existing authenticated Ollama
  session. The router never writes secrets to disk.

The router does **not** implement a long-running daemon.
The router runs as a foreground process; the parent shell
sees the exit code; CI systems can drive the router
non-interactively. The router is invoked as
`.\tools\ai-session-router.ps1 -Command Next`.

The router is **not** part of the Blazor platform's
runtime. The router is a developer-tooling tool, like
PowerShell, Git, or Windows Terminal. The router does not
ship inside the platform; the router lives at the
repository root in `tools/`. The router is a Windows-first
PowerShell script. Cross-platform support is a future task.

The future `AiEng.Platform` AI Session Router feature
(per `AGENTS.md` § 4 / `ROADMAP.md` / `DECISIONS.md`) will
consume the same logical model through a service
abstraction (`IAiSessionRouter`, `IModelRoutingPolicy`,
`IAgentSessionLauncher`, `ModelRoutingConfiguration`,
`TaskExecutionPipeline`) but is **not** implemented in
this task. The PowerShell supervisor is the bridge that
exists today.

---

## Alternatives Rejected

- **A single multi-phase daemon model.** The current
  approach. Token cost is the reason this task exists.
- **A custom PowerShell module that re-implements Claude
  Code internals.** Rejected; the task is to launch the
  existing Claude Code binary, not to replace it.
- **Hot-swapping the model of a running Claude Code
  process.** Rejected; the brief is explicit. The
  supervisor starts a fresh process.
- **A JSON-RPC bridge to a cloud API instead of
  `ollama launch claude`.** Rejected; the brief is
  explicit. The non-interactive Ollama launch form is the
  contract.
- **Implementing the future Blazor `IAiSessionRouter` in
  this task.** Rejected; the current roadmap does not
  schedule it. The PowerShell supervisor is the bridge
  that exists today.
- **A local-only model fallback.** Rejected; cloud-only
  is the operating mode. Local models are out of scope.
- **Implementing the task T-030 (M4-C closeout) first.**
  Rejected; the brief is explicit. The router is a
  one-time session-efficiency task that does not begin
  the next product task.

---

## Files to Add

### Router and configuration

- `tools/ai-session-router.ps1` — the production
  PowerShell supervisor (PowerShell 5.1+; Windows-first;
  uses `Start-Process` for the child; no
  `Invoke-Expression`; argument list, not string).
- `tools/ai-session-router.example.ps1` — a runnable
  example script the user can copy to
  `ai-session-router.ps1` and edit.
- `tools/ai-session-router.Tests.ps1` — Pester tests for
  the router. Mocked child processes; no real Claude Code
  invocation; no real Ollama call. Pester is the canonical
  PowerShell test framework.
- `tools/README.md` — a one-page readme for `tools/`. The
  router is the only entry; the future Blazor router lives
  in the platform.
- `.ai/model-routing.json` — model profile configuration.
  Profiles: `high` / `standard` / `economy` / `review` /
  `fallback`. Each profile names a model, an `enabled`
  flag, a `purpose` list, an optional `phase_permissions`
  allowlist, an optional `max_retries` field, a
  `timeout_seconds` field, and a `non_interactive_flags`
  field. The file also carries `budgets` (per-phase
  preferred profile, max_context_files, max_initial_source_files)
  and `execution` (`cloud_only`, `allow_local_models`,
  `fail_open_on_validation_failure`,
  `push_authorization_required`). The standard and
  economy model names are `CONFIGURE_ME` until the user
  runs `-Configure`. High / review / fallback default to
  `minimax-m3:cloud` per the brief.
- `.ai/model-routing.example.json` — a documented example
  the user can copy to `.ai/model-routing.json` and edit.
  Each field is annotated.
- `.ai/model-routing.schema.json` — JSON Schema (draft
  2020-12) validating `.ai/model-routing.json`. Validates:
  profile name, model name, enabled status, phase
  permissions, fallback profile, retry limits, timeouts,
  non-interactive launch flags, budgets, execution.

### Task classification

- `.ai/model-classification.md` — the human-readable
  classification rules. High tier = architecture, new
  project boundaries, security-sensitive work, etc.
  Standard tier = implementation from approved plan,
  tests, normal refactoring. Economy tier = reports,
  handoffs, state projections, JSON receipts.
  Review tier = high-risk review only.
- `.ai/model-classification.json` — the machine-readable
  rules. Each rule carries a `tier` (`high` / `standard`
  / `economy` / `review`), a `match` predicate (task type,
  capability set, file paths, branch name, etc.), and a
  `phase` list the tier applies to. Deterministic; the
  router reads this file and picks the highest-priority
  matching tier.

### Phase prompts

- `.ai/prompts/phases/reconcile.md` — Reconcile phase.
  Profile: economy or standard. Allowed: validate Git
  state, validate active task, validate plan path,
  refresh active-task packet. Forbidden: broad
  exploration, planning, implementation.
- `.ai/prompts/phases/plan.md` — Plan phase. Profile:
  high. Allowed: produce or repair an execution-ready
  plan. Forbidden: implementation, validation, commit.
- `.ai/prompts/phases/implement.md` — Implement phase.
  Profile: standard. Allowed: read active-task packet,
  read approved plan, inspect relevant files, implement,
  run targeted tests. Forbidden: write the final
  implementation report; begin another phase.
- `.ai/prompts/phases/validate.md` — Validate phase.
  Profile: standard. Allowed: run full closeout
  validation, fix in-scope failures, write validation
  evidence. Forbidden: implementation; commit; begin
  another phase.
- `.ai/prompts/phases/document.md` — Document phase.
  Profile: economy. Allowed: write compact implementation
  receipt, write concise report, update handoff, update
  structured state, refresh Markdown projections only
  when needed. Forbidden: implementation; commit.
- `.ai/prompts/phases/review.md` — Review phase. Profile:
  review / high. Allowed: read the diff, the plan, and
  the receipts; produce a review report. Forbidden:
  implementation; commit.
- `.ai/prompts/phases/closeout.md` — Closeout phase.
  Profile: economy or standard. Allowed: review Git diff
  summary, create commit, merge according to branching
  strategy, update final evidence, prepare next task
  plan, stop. Forbidden: implement the next task; begin
  the next task.

### Phase receipts

- `.ai/receipts/phases/README.md` — one-page README for
  the receipts directory. The router writes a receipt per
  phase; the next phase reads the previous phase's
  receipt; the receipt is the handoff between models.
- `.ai/templates/phase-receipt.schema.json` — JSON Schema
  for the phase receipt. Required fields: `task_id`,
  `phase`, `model`, `profile`, `started_at`,
  `completed_at`, `exit_code`, `status`, `files_read`,
  `files_changed`, `commands_run`, `targeted_tests`,
  `validation`, `decisions`, `blockers`, `next_phase`,
  `retry_recommended`, `fallback_recommended`, `usage`
  (with `prompt_tokens`, `completion_tokens`,
  `elapsed_seconds`, `cloud_usage_percentage_before`,
  `cloud_usage_percentage_after`, or `unknown` when not
  available), `receipt_version`.
- `.ai/templates/implementation-receipt.schema.json` —
  JSON Schema for the per-task implementation receipt at
  `.ai/receipts/<task-id>.json`. The economy model writes
  this; the standard model never writes it. Lighter than
  the implementation report; the report remains the
  human-readable record.

### Context

- `.ai/context/README.md` — README for the context
  directory.
- `.ai/context/repository-map.json` — a small map of
  canonical paths: `repo_root`, `ai_dir`, `tools_dir`,
  `handoffs_dir`, `plans_dir`, `receipts_dir`,
  `state_dir`, `prompts_dir`. The router and the child
  sessions read this map instead of re-deriving paths.
- `.ai/context/active-task.json` — the per-task packet.
  Fields: `task_id`, `milestone`, `slice`, `capability_ids`,
  `task_type`, `complexity_classification`, `current_phase`,
  `next_phase`, `recommended_profile_by_phase`,
  `approved_plan`, `objective`, `included_scope`,
  `excluded_scope`, `expected_affected_paths`,
  `relevant_adrs`, `required_context_files`,
  `optional_context_files`, `tests`, `validation`,
  `expected_branch`, `expected_commit`, `stop_conditions`,
  `last_successful_phase`, `failure_count`, `fallback_count`.
  This is the handoff between models. No child session
  needs to rediscover the entire repository.
- `.ai/context/validation-cache.json` — a cache of the
  last validation result keyed by working-tree fingerprint
  and commit hash. The router uses the cache to avoid
  re-running a full validation after a phase that did not
  change source or test files.

### Indexes and archives

- `.ai/index/reports.json` — index of every
  `implementation-report-*.md` (path, task_id,
  milestone, date, sha, key_phases).
- `.ai/index/plans.json` — index of every
  `.ai/plans/*.md` (path, milestone, status, date).
- `.ai/index/handoffs.json` — index of every
  `.ai/handoffs/YYYY-MM-DD-*.md` (path, task_id,
  session_slug, date, sha).
- `.ai/archive/README.md` — README for the archive.
  Archive stores the historical long-form records that
  the indexes point to. The fast path consults the
  indexes; the slow path reads the archive.

### Benchmarks (stubs, not run in this task)

- `.ai/benchmarks/model-routing/README.md` — README for
  the benchmark directory. Documents the five benchmark
  tasks: plan creation, Blazor component task, service
  and tests, bug fix, documentation closeout. The
  benchmark is not run in this task.
- `.ai/benchmarks/model-routing/benchmark-tasks.json` —
  the benchmark task list (paths, expected outputs).
- `.ai/benchmarks/model-routing/results.json` — empty
  result list; populated by future benchmark runs.

### Operating layer updates

- `.ai/session-start.md` — add a `Router-managed
  startup` section. The router-managed child session
  reads only: relevant command section,
  `active-task.json`, `repository-map.json`, the phase
  prompt, the approved plan, the files named in the
  task packet, `git status`, `git log -1`. The child
  does **not** run the full startup sequence unless
  architecture changes, the active packet is invalid,
  a contradiction exists, or the phase requires
  high-level reasoning.
- `.ai/commands.md` — define two execution forms:
  `Interactive Next` (existing command workflow) and
  `Routed Next` (`.\tools\ai-session-router.ps1
  -Command Next`). The routed form is the preferred
  cost-aware mode. Interactive remains available as a
  fallback. The existing nine commands and the 13-step
  lifecycle are unchanged.

### Backlog and architecture-ready planning

- `.ai/backlog/ai-session-router.md` — the future
  `AiEng.Platform` AI Session Router backlog entry. The
  UI capabilities: configure cloud models by phase,
  select planning / implementation / documentation /
  review / fallback models, configure budgets, display
  usage, display current phase, display selected model,
  pause routing, resume routing, approve escalation,
  inspect phase receipts, run `Next` through the
  platform. The future Blazor application calls a
  service abstraction (`IAiSessionRouter`,
  `IModelRoutingPolicy`, `IAgentSessionLauncher`,
  `ModelRoutingConfiguration`, `TaskExecutionPipeline`)
  rather than PowerShell directly. Backlog only; no
  implementation.

### Documentation

- `implementation-report-autonomous-model-routing-optimization.md`
  — the implementation report (under 200 lines;
  references the router, the configuration, the
  receipts, the tests, the Git commit, the new
  documentation, and the next recommended command).

---

## Files to Modify

- `ROADMAP.md` — add a `Future AI Session Router`
  paragraph to the § 5 "What Is Intentionally Deferred"
  table (backlog only) and a small note in the § 4
  self-dogfooding matrix that the operating-layer
  routing is shipped in this task. The note is
  informational; the matrix is unchanged.
- `DECISIONS.md` — add ADR-017 (`Adopt autonomous
  cloud-model routing for the AI operating layer`).
  The ADR is short; the router is an operating-layer
  change, not a platform architecture change. The
  ADR records: the rationale (token cost; one-model-
  does-everything is wasteful), the chosen approach
  (external supervisor, one model per phase, bounded
  child sessions, phase receipts), the rejected
  alternatives (multi-phase daemon, hot-swap, JSON-RPC
  bridge), the consequences (router scripts are
  developer tooling, not part of the platform; the
  future `IAiSessionRouter` consumes the same model),
  and the back-references (`AGENTS.md` Rule 15 +
  Rule 17, `.ai/commands.md` `Next`, the branching
  strategy).
- `AGENTS.md` — no change to the 17 rules. The router
  is a tool; the rules still bind. The brief
  prohibits architectural change.
- `.ai/state/current.md` — append a one-line summary
  of the new state. Do not reorganise the file.
- `.ai/state/task-board.md` — add a `Deferred` row
  for T-031 (AI Session Router — backlog). The
  brief says no new `Ready` task; the router work
  this task produces is itself a single, completed
  task, not a `Ready` task.
- `.ai/state/tasks.json` — append a single
  `Done` task record for the current task (T-031
  is the candidate ID; the next-id assignment is
  decided in the implementation report). The
  `previous_session` is
  `m4-c-2-app-provider-list-and-providers-page`.
  The next `Ready` task after this commit remains
  T-030 (M4-C closeout); the brief prohibits
  advancing the work queue past the router task.
- `.ai/state/session.json` — replace the `m4-c-2`
  envelope with a `autonomous-model-routing-optimization`
  envelope. The `intended_next_action` is `Stop.
  The next session is the M4-C closeout (T-030) on
  the user's 'Approve' or 'Next' invocation.`
- `.ai/state/milestones.json` — append a small
  `M-Future` (or similar) record for the future
  AI Session Router backlog item. The current
  milestone list (M0 through M8) is unchanged.
- `.ai/state/capabilities.json` — add a single
  capability record C-027 (or the next free ID)
  for the AI Session Router (the future Blazor
  feature; status `Deferred`; consumed by
  `IAiSessionRouter` family contract per the
  backlog). The current capability graph is
  unchanged.
- `.ai/handoffs/latest.md` — mirror the
  implementation report and the handoff for this
  task to `latest.md`.
- `docs/infrastructure.md` (optional) — if it
  exists; if not, skip. The router is operating
  layer, not platform infrastructure. If the file
  has a `M-? Consumers` section, append a `M-Router
  Consumers` line: `tools/ai-session-router.ps1`,
  `.ai/prompts/phases/`, `.ai/context/`.
- `tools/.gitignore` (optional) — if the user
  copies the example script and the Pester test
  output, ignore the `TestResults.xml` if present.
  Default: do not create.

---

## Files to Delete

None.

---

## Components Reused

- `.ai/commands.md` `Next` (the
  `Completed / Git / Validation / Evidence / Next`
  response shape).
- `.ai/workflows/progressive-coding.md` (the
  13-step task lifecycle).
- `.ai/workflows/branching-strategy.md` (the
  fast-forward merge, branch deletion, milestone
  tag).
- `.ai/templates/implementation-plan.md`,
  `.ai/templates/implementation-report.md`,
  `.ai/templates/session-handoff.md` (the router
  consumes the canonical plan path, the
  implementation report path, and the handoff
  path; the canonical templates are not modified).
- `.ai/state/*.json` + `.ai/state/session.json`
  (the router reads the session envelope; the
  child writes the per-phase handoff).
- The branch name convention
  `feature/T-<task-id>-<short-description>` (the
  router names the feature branch after the task
  ID; the router does not invent a new
  convention).

---

## Components Added

- `tools/ai-session-router.ps1` — the supervisor
  itself. PowerShell 5.1+ (no `pwsh` dependency;
  runs on the user's existing `powershell.exe`).
- `tools/ai-session-router.Tests.ps1` — Pester
  tests. Mocked `Start-Process`; mocked Ollama
  launch; no real child process.
- `tools/README.md` — one-page readme.
- `.ai/prompts/phases/*.md` — seven phase prompts.
  Each prompt is under 120 lines; each prompt
  states its allowed actions, its forbidden
  actions, and its expected output receipt.

---

## Services Added

None in the platform. The router is operating-layer
tooling, not a Blazor service. The future
`IAiSessionRouter` service is in the backlog only.

---

## Risks

- **Risk 1 — Ollama launch form may change.** Likelihood:
  low. Impact: high. Mitigation: the
  `non_interactive_flags` field in
  `.ai/model-routing.json` is configurable; the
  default is `--yes -- -p`; the user can change
  the form without editing the script.
- **Risk 2 — `powershell.exe` (5.1) lacks features
  the script assumes.** Likelihood: low. Impact:
  low. Mitigation: the script targets 5.1+; it
  avoids `pwsh`-only features (`$null -eq`,
  pipeline chain operators, ternary `? :`).
- **Risk 3 — `ollama launch claude` prompts for
  model confirmation on the first run.** Likelihood:
  medium. Impact: low. Mitigation: the router
  passes `--yes`; the user can pre-accept the
  model with a one-time `ollama launch claude
  --model <model> --yes` invocation during
  `-Configure`.
- **Risk 4 — The phase prompt injects a large
  context the user did not want.** Likelihood:
  medium. Impact: medium. Mitigation: each phase
  prompt states the required context files; the
  parent router only injects `task_id`, `phase`,
  `model profile`, `active-task path`, `receipt
  path`, and `stop conditions`. The router does
  not inject the full product history.
- **Risk 5 — A malformed model name produces a
  child that hangs.** Likelihood: low. Impact:
  medium. Mitigation: the router validates model
  names against a strict regex
  (`^[a-zA-Z0-9._:-]+$`); the router enforces a
  per-phase timeout; the router kills the
  process tree on timeout.
- **Risk 6 — The router advances a phase that
  should be escalated but is not.** Likelihood:
  low. Impact: high. Mitigation: the default
  policy is conservative: one retry with the
  same profile, one escalation, then stop. The
  user can override `max_retries` and
  `fallback_profile` in the configuration.
- **Risk 7 — The 429 / usage-exhaustion response
  triggers an infinite loop.** Likelihood: low.
  Impact: high. Mitigation: the router records
  the failure, does not retry the same model,
  selects the configured `fallback` profile
  when allowed, and stops cleanly if no fallback
  is available.
- **Risk 8 — The user accidentally runs `Next`
  interactively while a routed child is
  running.** Likelihood: low. Impact: high.
  Mitigation: the router writes a
  `.ai/router.lock` file at start; the user
  prompt says `Routed Next is in progress;
  open a second terminal?`. The lock is
  advisory, not enforced (the user can always
  interrupt).
- **Risk 9 — A child writes to a path outside
  the repository.** Likelihood: low. Impact:
  high. Mitigation: the router validates every
  file path against the repo root; the
  supervisor rejects paths outside the root.
- **Risk 10 — A child process survives the
  supervisor.** Likelihood: low. Impact: high.
  Mitigation: the router uses `Start-Process
  -PassThru` and tracks the PID; Ctrl+C
  terminates the supervisor and the process
  tree via `Stop-Process -Id $pid -Force
  -ErrorAction SilentlyContinue` on every
  tracked PID.

---

## Test Plan

- **Pester tests for the router** (`tools/ai-session-router.Tests.ps1`).
  - Configuration parsing:
    valid `.ai/model-routing.json` is accepted;
    invalid (`enabled: "not-a-bool"`, missing
    `model`, malformed `phase_permissions`,
    negative `max_retries`, missing `fallback`
    profile, `cloud_only: true` with a
    local-only model) is rejected.
  - Profile selection: a `high` task selects
    `high`; a `standard` task selects `standard`;
    a documentation task selects `economy`;
    a `review` task selects `review`; a failed
    standard task escalates to `high`; a 429
    response escalates to `fallback`.
  - Task classification: the classification
    rules resolve a `feature` task to `standard`,
    an `architecture` task to `high`, a
    `documentation` task to `economy`, an
    `architecture-sensitive` task to `high`.
  - Phase progression: the router advances
    `Reconcile` → `Plan` → `Implement` →
    `Validate` → `Document` → `Closeout`;
    `Review` runs only when classification
    requires it; the router stops at
    `Closeout` and does not begin another
    task.
  - Retry policy: one retry with the same
    profile, one escalation, then stop.
  - Fallback selection: 429 selects the
    configured `fallback` profile when
    allowed; no fallback means stop cleanly.
  - 429 handling: a simulated 429 does not
    trigger an infinite loop; the failure is
    recorded in the receipt; the router
    stops after the fallback attempt fails.
  - No-second-task: after a successful
    `Closeout`, the router refuses to advance
    to a new task; the user must invoke
    `Next` again.
  - Cloud-only enforcement:
    `execution.cloud_only = true` rejects a
    local-only model name; the router does
    not start the child.
  - Dry-run command generation: `-DryRun`
    prints the `ollama launch claude --model
    <model> --yes -- -p "<prompt>"` command
    without executing it.
  - Safe argument construction: model name
    and task ID are validated against the
    strict regex; a name with `;` or `&` is
    rejected.
  - Resume from a phase receipt: the router
    reads the last `phase-receipt.json` and
    advances from `next_phase`.
  - Stopping after closeout: a simulated
    closeout that produced a `next_task` does
    not start the next task.
- **Mocked child processes.** Pester mocks
  `Start-Process` to a stub that records the
  argument list, returns a fake `Process`
  object, and writes a fake phase receipt. The
  tests do not invoke `ollama launch claude`.
  The tests do not consume Ollama cloud
  quota.
- **JSON schema validation.** Pester
  `Test-Json -SchemaFile` validates every
  schema against every fixture: valid
  configurations pass; invalid configurations
  fail with a clear error.
- **PowerShell syntax validation.**
  `powershell.exe -NoProfile -Command
  "Get-Command -Syntax (Get-Content
  tools/ai-session-router.ps1 -Raw |
  Out-String)"` (or
  `[ScriptBlock]::Create((Get-Content -Raw
  tools/ai-session-router.ps1))` parse) for
  every script in `tools/`.
- **Dry-run simulation.** A small wrapper
  Pester test runs the router in `-DryRun`
  for the following scenarios and asserts
  the command output:
  - Planning task: profile `high`,
    phase `Plan`, model from `.ai/model-routing.json`
    high profile.
  - Implementation task: profile `standard`,
    phase `Implement`.
  - Documentation task: profile `economy`,
    phase `Document`.
  - Failed standard task escalating to
    `high`: phase `Implement`, first
    attempt fails, second attempt uses
    `high` profile.
  - 429 response recovery: child reports
    `usage_exhausted`, router selects
    `fallback` profile.

---

## Documentation Plan

- `tools/README.md` — one page. The router is the
  only entry.
- `.ai/receipts/phases/README.md` — one page.
  Receipts directory structure.
- `.ai/context/README.md` — one page. Context
  directory structure.
- `.ai/archive/README.md` — one page. Archive
  directory structure.
- `.ai/benchmarks/model-routing/README.md` — one
  page. The benchmark is documented but not run.
- `implementation-report-autonomous-model-routing-optimization.md`
  — under 200 lines. The receipt of the
  one-time task.
- `.ai/backlog/ai-session-router.md` — backlog
  entry for the future `IAiSessionRouter`
  Blazor feature.
- `DECISIONS.md` — append ADR-017.
- `ROADMAP.md` — append the `Future AI Session
  Router` row to the § 5 deferred table and the
  § 4 matrix note.
- `.ai/commands.md` — add the `Routed Next`
  section to § 3.1 (next to the existing
  `Next` command) and the `## 3.10 Routed
  Next` subsection.
- `.ai/session-start.md` — add the
  `Router-managed startup` section.

---

## Approval

- Submitted by: the autonomous-model-routing-optimization
  session.
- Reviewed by: (pending; the brief is a
  pre-authorised one-time task).
- Date: 2026-07-14.
- Status: `Draft` (the brief is the
  authorisation; the plan is the receipt).
- Canonical path:
  `C:\Users\hkasozi\.claude\plans\lucky-giggling-otter.md`.
- See: `.ai/plans/README.md` (the brief does not
  require a canonical plan in `.ai/plans/`; the
  one-time nature of the task is recorded in the
  implementation report).

---

## Linked Artefacts

- `.ai/commands.md` `Next` (the response shape
  the router mirrors).
- `.ai/workflows/progressive-coding.md` (the
  13-step lifecycle the router executes one
  phase at a time).
- `.ai/workflows/branching-strategy.md` (the
  merge, the branch deletion, the milestone
  tag).
- `.ai/state/tasks.json` T-030 (M4-C closeout;
  the next `Ready` task after this commit; the
  router does **not** begin T-030 in this task).
- `.ai/state/session.json` (the router reads
  the session envelope; the child writes the
  per-phase handoff).
- `ROADMAP.md` § 4 + § 5 (the matrix and the
  deferred table receive informational
  additions only).
- `DECISIONS.md` ADR-017 (the ADR for the
  one-time task).
