# Backlog — AI Session Router (Platform)

> **Future Blazor `AiEng.Platform` AI Session Router.**
> The PowerShell supervisor at `tools/ai-session-router.ps1`
> is the operating-layer bridge that exists today. The
> future in-platform feature is described here. Backlog
> only; no implementation in this milestone.

## Status

- **Status:** Deferred.
- **Milestone:** Future (after M8).
- **Capability ID:** C-027 (per `.ai/state/capabilities.json`).
- **Consumes:** `IAiSessionRouter`, `IModelRoutingPolicy`,
  `IAgentSessionLauncher`, `ModelRoutingConfiguration`,
  `TaskExecutionPipeline`.

## UI Capabilities

The future in-platform AI Session Router exposes the
following UI surfaces:

| Capability | Description |
| ---------- | ----------- |
| Configure cloud models by phase | Pick the cloud model for each profile (`high`, `standard`, `economy`, `review`, `fallback`). |
| Select planning / implementation / documentation / review / fallback models | The five profile selectors. |
| Configure budgets | Per-phase `max_context_files` and `max_initial_source_files`. |
| Display usage | The current prompt tokens, completion tokens, and cloud usage percentage. |
| Display current phase | The phase the router is currently dispatching. |
| Display selected model | The model the router has chosen for the current phase. |
| Pause routing | Pause the router between phases; the next phase does not start until the user resumes. |
| Resume routing | Resume the router from the last `next_phase`. |
| Approve escalation | Approve an escalation from `standard` to `high` (or to `fallback`) before the router applies it. |
| Inspect phase receipts | Browse the per-phase receipts at `.ai/receipts/phases/<task-id>-<phase>.json`. |
| Run `Next` through the platform | A button in the platform that calls `IAiSessionRouter.RunNext` instead of the PowerShell entry. |

## Service Abstractions

The platform consumes the same logical model through a
service abstraction. The PowerShell supervisor and the
in-platform router are two consumers of the same
configuration format (`.ai/model-routing.json`).

```csharp
public interface IAiSessionRouter
{
    Task<PhaseReceipt> RunNextAsync(CancellationToken ct);
    Task<PhaseReceipt> RunPhaseAsync(string phase, CancellationToken ct);
    Task<PhaseReceipt> ResumeAsync(CancellationToken ct);
    Task<ModelRoutingConfiguration> GetConfigurationAsync();
    Task SetConfigurationAsync(ModelRoutingConfiguration cfg);
    event EventHandler<PhaseReceipt> PhaseCompleted;
    event EventHandler<EscalationRequest> EscalationRequested;
}

public interface IModelRoutingPolicy
{
    string SelectProfile(ActiveTaskPacket task, string phase);
    string ResolveModel(string profile);
}

public interface IAgentSessionLauncher
{
    Task<AgentSessionResult> LaunchAsync(string model, string prompt, TimeSpan timeout, CancellationToken ct);
}

public sealed record ModelRoutingConfiguration(
    IReadOnlyDictionary<string, ModelProfile> Profiles,
    IReadOnlyDictionary<string, PhaseBudget> Budgets,
    ExecutionPolicy Execution);

public interface ITaskExecutionPipeline
{
    Task<TaskExecutionResult> RunTaskAsync(ActiveTaskPacket task, CancellationToken ct);
}
```

## Acceptance

The future AI Session Router feature is **accepted** when:

- The platform exposes a configuration UI for the five
  profiles.
- The platform exposes a `Next` button that drives a
  bounded child session per phase.
- The platform displays the current phase, the selected
  model, and the cumulative usage.
- The platform reads and writes
  `.ai/model-routing.json` (the same file the
  PowerShell supervisor reads).
- The platform's `IAiSessionRouter` and the PowerShell
  supervisor produce identical phase receipts.
- The Pester tests for the PowerShell supervisor pass
  against the platform's launch abstractions (the
  platform is a host, not a separate router).

## Out of Scope

- The platform does **not** implement the PowerShell
  supervisor's Ctrl+C handling; the platform uses
  `CancellationToken` instead.
- The platform does **not** implement the
  `execution.push_authorization_required` flag; the
  platform's Git integration respects the user's
  existing `push.autoSetupRemote` and `remote.pushDefault`
  configuration.
- The platform does **not** implement local-model
  fallback; cloud-only is the operating mode.

## Linked Artefacts

- `tools/ai-session-router.ps1` (the operating-layer
  bridge).
- `.ai/model-routing.json`,
  `.ai/model-routing.schema.json`.
- `.ai/model-classification.json`,
  `.ai/model-classification.md`.
- `.ai/prompts/phases/*.md`.
- `.ai/templates/phase-receipt.schema.json`,
  `.ai/templates/implementation-receipt.schema.json`.
- `DECISIONS.md` ADR-017 (the operating-layer ADR).
