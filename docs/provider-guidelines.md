# docs/provider-guidelines.md

> How providers are authored, registered, configured, and tested
> in the AI Engineering Platform. Read this after `AGENTS.md`,
> `ARCHITECTURE.md`, and `docs/architecture-principles.md`.

---

## 1. Why This Document Exists

The platform integrates with many external tools. The provider
model is the architecture's response to that reality. This
document is the **operational** counterpart to the architectural
principles in `docs/architecture-principles.md`.

If you are integrating a new external tool, this is the document
to read first. It walks through the contract, the implementation,
the registration, the configuration, and the tests.

---

## 2. Provider Families

A provider family groups providers that share a contract. Per
ADR-012, families are named after the **capability** they offer,
not after a vague category. A vague family name ("Assistant",
"Deployment", "Internal") is a smell: it tells the reader nothing
about what the contract actually does.

The platform recognises the following families:

| Family              | Contract                       | Examples                                            |
| ------------------- | ------------------------------ | --------------------------------------------------- |
| Agent Runtime       | `IAgentRuntimeProvider`        | Ollama Launch (M6), Ollama API (later), Claude, OpenAI, Codex |
| Git                 | `IGitProvider`                 | Git (CLI)                                           |
| Terminal            | `ITerminalProvider`            | PowerShell, WSL, Windows Terminal, Git Bash         |
| Worktree            | `IWorktreeProvider`            | Native (worktree), Treehouse                        |
| Quality Gate        | `IQualityGateProvider`         | No Mistakes                                         |
| Review              | `IReviewProvider`              | Native (review), Lavish Axi                         |
| Autonomous Loop     | `IAutonomousLoopProvider`      | GNHF                                                |
| Orchestration       | `IOrchestrationProvider`       | Native (orchestration), Firstmate                   |

A new family is added only when the capability is genuinely
shared by at least two providers. A one-off provider is filed
under the closest existing family, or its own contract is filed
as an ADR for review. The eight families above are the canonical
set; the abstract base `IProvider` is retained so the registry,
health poller, configuration UI, and diagnostics page can
operate on every provider uniformly.

Tool-to-family mapping (per ADR-012):

- **Treehouse** is a **Worktree** provider. It manages isolated
  per-task copies of a repository. The platform also offers a
  Native Worktree implementation built on `git worktree`.
- **No Mistakes** is a **Quality Gate** provider. It evaluates
  proposed changes against a configured quality rubric and
  reports pass/fail.
- **Lavish Axi** is a **Review** provider. It produces structured
  code review output that the application can render alongside
  human review.
- **GNHF** is an **Autonomous Loop** provider. It runs the
  iterative plan-build-review loop on a task.
- **Firstmate** is an **Orchestration** provider. It coordinates
  multiple sub-tasks across the team.

The mapping is operational, not editorial. The contracts name
the capability; the providers implement the capability; the
registry is unaware of which company built the implementation.

### 2.1 Ollama API vs Ollama Launch

Ollama exposes two distinct surfaces that the platform
integrates with. They are **not two flavours of one provider**.
They are two separate integrations with different transports,
different failure surfaces, and different lifecycles.

- **Ollama Launch** is a process boundary. The provider
  invokes `ollama launch <runtime> --model <model>` through
  `IProcessRunner` (from
  `AiEng.Platform.Infrastructure/Process/`, the only project
  allowed to call `Process.Start`). The provider owns the
  process lifecycle: start, stream output, stop, restart.
  The first coding-agent feature (M6 in `ROADMAP.md`) targets
  this integration. The example invocation is
  `ollama launch claude --model minimax-m3:cloud`; the
  application treats the model name as configuration.
- **Ollama API** is an HTTP boundary. The provider speaks
  the Ollama HTTP API (typically `http://localhost:11434`)
  through `IHttpClientFactory`. The provider owns the
  request/response lifecycle: connect, complete, stream
  events, close. This integration is deferred to a later
  milestone.

A provider that confuses the two — a provider that
`Process.Start`s `ollama` while claiming to integrate the
HTTP API, or a provider that calls
`HttpClient.PostAsync("http://localhost:11434/...")` while
claiming to integrate Launch — is rejected in review. The
two are different families of failure: process lifecycle
errors vs HTTP errors. The platform's
`ProviderResult<T>` envelope and the architecture tests can
detect a wrong mapping only when the contract is honest
about which surface is being integrated.

The M6 vertical slice in `ROADMAP.md` is explicit about
which surface it targets: the Ollama Launch process
invocation, not the Ollama API. The M6 contract reads
`IAgentRuntimeProvider`; the implementation owns the
process boundary.

---

## 3. The Provider Contract

### 3.1 The Base Contract

Every provider implements `IProvider`:

```csharp
public interface IProvider
{
    ProviderId Id { get; }
    string DisplayName { get; }
    Task<ProviderDescriptor> DescribeAsync(CancellationToken ct);
    Task<ProviderHealth> HealthAsync(CancellationToken ct);
    Task ConfigureAsync(ProviderConfiguration configuration, CancellationToken ct);
}
```

- `Id` is a stable, lower-snake identifier (`"ollama"`,
  `"claude"`, `"git"`). It is the key the registry uses.
- `DisplayName` is the user-facing name (`"Ollama"`, `"Claude"`,
  `"Git"`).
- `DescribeAsync` returns the provider's capabilities, version,
  and any user-facing metadata.
- `HealthAsync` returns the current health (`Healthy`,
  `Degraded`, `Unhealthy`).
- `ConfigureAsync` applies runtime configuration changes
  (e.g. an API key entered through the UI).

### 3.2 The Family Contract

A family contract extends `IProvider` with the family-specific
operations. Example:

```csharp
public interface IAgentRuntimeProvider : IProvider
{
    Task<ProviderResult<RuntimeCapabilities>> GetCapabilitiesAsync(
        CancellationToken ct);

    Task<ProviderResult<RuntimeResponse>> CompleteAsync(
        RuntimeRequest request,
        CancellationToken ct);

    IAsyncEnumerable<RuntimeEvent> StreamAsync(
        RuntimeRequest request,
        CancellationToken ct);
}
```

A family contract is pure. It has no implementation, no
external dependencies, no logic. A contract is a promise.

### 3.3 The Result Envelope

Family methods that can fail return `ProviderResult<T>`:

```csharp
public readonly record struct ProviderResult<T>(
    ProviderOutcome Outcome,
    T? Value,
    ProviderError? Error);
```

`ProviderError` carries:

- `Category` — typed error category (`Network`, `Auth`, `RateLimit`,
  `InvalidInput`, `Internal`).
- `Message` — user-facing message (already localised if
  applicable).
- `Inner` — optional underlying exception for diagnostics.

`ProviderResult<T>` is the only shape accepted from providers.
Exceptions are reserved for programmer errors and unrecoverable
conditions.

---

## 4. The Provider Implementation

### 4.1 Folder Layout

```
src/AiEng.Platform.Providers.Ollama/
├── OllamaProvider.cs
├── OllamaOptions.cs
├── OllamaClient.cs
└── README.md

tests/AiEng.Platform.ProviderContractTests/AgentRuntime/Ollama/
└── OllamaProviderContractTests.cs
```

- `OllamaProvider.cs` (in
  `src/AiEng.Platform.Providers.Ollama/`) — implements
  `IAgentRuntimeProvider`.
- `OllamaOptions.cs` (same project) — strongly-typed
  configuration.
- `OllamaClient.cs` (same project) — private HTTP / SDK
  wrapper.
- `OllamaProviderContractTests.cs` (in
  `tests/AiEng.Platform.ProviderContractTests/AgentRuntime/Ollama/`)
  — proves the implementation satisfies the contract.
- `README.md` (provider project) — operational notes specific
  to this provider.

The provider project is added to the solution only when the
provider is implemented. A speculative project is rejected.
The contract interface (`IAgentRuntimeProvider`) is shared and
lives in `AiEng.Platform.Providers.Abstractions/AgentRuntime/`.

### 4.2 The Provider Class

A provider class:

- Is `sealed`.
- Implements the family contract.
- Has a primary constructor receiving its dependencies.
- Exposes a `ProviderId` constant matching its `Id`.
- Returns `ProviderResult<T>` from every fallible method.
- Accepts a `CancellationToken` on every async method.
- Logs through `ILogger<OllamaProvider>`.
- Never references a UI type.

```csharp
public sealed class OllamaProvider : IAgentRuntimeProvider
{
    public const ProviderId Id = new("ollama");

    private readonly OllamaOptions _options;
    private readonly OllamaClient _client;
    private readonly ILogger<OllamaProvider> _logger;

    public OllamaProvider(
        OllamaOptions options,
        OllamaClient client,
        ILogger<OllamaProvider> logger)
    {
        _options = options;
        _client = client;
        _logger = logger;
    }

    public ProviderId Id => Id;
    public string DisplayName => "Ollama";

    public async Task<ProviderResult<RuntimeResponse>> CompleteAsync(
        RuntimeRequest request,
        CancellationToken ct)
    {
        try
        {
            var response = await _client.CompleteAsync(request, ct);
            return ProviderResult<RuntimeResponse>.Success(response);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Ollama request failed");
            return ProviderResult<RuntimeResponse>.Failure(
                new ProviderError(
                    ProviderErrorCategory.Network,
                    "The Ollama server could not be reached.",
                    ex));
        }
    }
}
```

### 4.3 The Client

A provider's client is the private wrapper around the external
SDK or HTTP API. It:

- Is `internal` or `private`.
- Owns the HTTP client (`IHttpClientFactory`).
- Translates between the external DTOs and the platform DTOs.
- Throws on external failures; the provider translates them
  into `ProviderResult`.

### 4.4 The Options

Every provider has a strongly-typed options class:

```csharp
public sealed record OllamaOptions
{
    public Uri Endpoint { get; init; } = new("http://localhost:11434");
    public TimeSpan Timeout { get; init; } = TimeSpan.FromMinutes(2);
    public string? DefaultModel { get; init; }
}
```

Options are:

- Validated with `DataAnnotations` or `IValidateOptions<T>`.
- Bound from configuration in `Program.cs`.
- Never contain secrets. Secrets go through `ICredentialVault`.
- Documented with `[Description]` attributes for tooling.

### 4.5 What a Provider Never Does

A provider never:

- References a UI type.
- References another provider.
- Persists state to a global location.
- Performs work without a `CancellationToken`.
- Throws for external failures (use `ProviderResult`).
- Logs secrets, full payloads, or stack traces at
  `Information` level.

### 4.6 Provider Lifecycle States

A provider progresses through five distinct states during
the lifetime of a host run. The states are the vocabulary
the registry, the health poller, the configuration UI, and
the diagnostics page share. The states are distinct, not
cumulative: a provider is exactly one of these states at
any moment; the next state is reached through a specific
event (compile, register, enable, health-check, select).
The state model is recorded in `ARCHITECTURE.md` § 4.5
and ADR-016.

1. **Compiled-in.** The provider implementation is a
   referenced project in the solution's compile graph.
   Reachable from the composition root (`ARCHITECTURE.md`
   § 2.5) and from the family contract test project. A
   provider that is not compiled in cannot be registered,
   enabled, healthy, or selected. Every concrete provider
   implementation added to the solution enters this state
   the moment the `AiEng.Platform.Providers.<X>/` project
   is created. The catalogue in § 10 records a planned
   provider as `Compiled-in` in the future; an
   implemented provider is `Compiled-in` on day one.

2. **Registered.** The composition root has added the
   provider to DI. Reachable through DI but may not be
   visible to the user. Registration is the act of
   binding the concrete `*Provider` class to the family
   contract
   (`services.AddSingleton<IGitProvider, GitProvider>()`).
   Registration happens once at host startup; the
   configuration section's presence is checked at
   enablement time. A registered provider is
   **not yet visible to the UI**; the UI sees only
   enabled providers through the family-scoped
   registry.

3. **Enabled.** The provider's configuration section is
   present and valid; the user (or the deployment) has
   opted in. The only providers the UI sees through the
   family-scoped registry. A registered provider that is
   not enabled is invisible to the UI but is still
   present in DI. The configuration UI's "disabled"
   toggle removes the configuration section and
   transitions the provider from `Enabled` back to
   `Registered`.

4. **Healthy.** A periodic health check has confirmed
   the provider is reachable and operational. The
   `IProviderRegistry` and the family-scoped registries
   expose the provider's current `ProviderHealth`
   (`Healthy`, `Degraded`, `Unhealthy`). A provider may
   be enabled and unhealthy at the same time; the UI
   renders the health state through `AppHealthDot` and
   `AppProviderCard` and may offer the user a way to
   disable an unhealthy provider. The health poller is
   the source of truth for this state; the registry
   caches the result. The M6 runtime UI surfaces
   `Enabled` and `Healthy` together in the runtime
   picker (sort: `Healthy` first, then `Degraded`, then
   `Unhealthy`).

5. **Selected.** The user has chosen this provider for a
   specific operation. A runtime is selected when the
   user picks it from `AppRuntimePicker`; a worktree
   provider is selected when the user picks it from the
   worktree page; a quality-gate provider is selected
   when the user runs a gate. Selection is
   per-operation: the same provider may be selected for
   one operation and not for another. A provider that is
   not enabled cannot be selected; a provider that is
   selected but unhealthy produces a
   `ProviderResult<T>.Failure` with a category that the
   application layer translates into an `AppErrorState`.
   M7 surfaces `Selected` through the configuration
   form and the run review page.

The states drive the configuration UI's affordances:

- A "disabled" toggle removes the configuration
  section; the transition is `Enabled` → `Registered`.
- A "test connection" button triggers a health check
  and surfaces the `Healthy` / `Degraded` / `Unhealthy`
  state.
- A runtime picker lists only providers that are
  `Enabled`, sorted by `Healthy` first.

A provider that is `Registered` but not `Enabled` is
invisible to the UI; a provider that is `Enabled` but
not `Selected` is the only one the UI lists; a
provider that is `Selected` but `Unhealthy` produces a
`ProviderResult<T>.Failure` that the application
layer translates into an `AppErrorState`.

---

## 5. Provider Registration

### 5.0 The Composition Root

Provider registration is the act of binding a concrete
`*Provider` class to its family contract in the DI
container. Registration happens **only in the composition
root** (`AiEng.Platform.App/Composition/`); the
composition root is the only place in the solution that
may reference a `Providers.<X>` project. UI pages,
components, application services, view models, DTOs,
and domain types resolve providers through the
`IProviderRegistry` and the family-scoped registries, not
through direct project imports.

A typical host run registers **several** provider
implementations simultaneously. The composition root
calls a registration extension for each
implementation that is compiled into the solution
(see `ARCHITECTURE.md` § 2.5 and ADR-016):

```csharp
builder.Services.AddOllamaLaunchProvider(builder.Configuration);
builder.Services.AddGitProvider(builder.Configuration);
builder.Services.AddPowerShellProvider(builder.Configuration);
builder.Services.AddWindowsTerminalProvider(builder.Configuration);
builder.Services.AddNativeWorktreeProvider(builder.Configuration);
builder.Services.AddNativeReviewProvider(builder.Configuration);
builder.Services.AddNoMistakesProvider(builder.Configuration);
```

Each registration extension decides whether the
provider's configuration section is present and binds
the concrete `*Provider` to the family contract
accordingly. The composition root is the **only** place
that may call these extensions; pages, components,
services, and view models never call them.

### 5.1 The Registration Extension

Each provider exposes a registration extension under
`AiEng.Platform.App/Composition/<Capability>/` (or
`<Implementation>/`, depending on the
implementation). The extension is named after the
capability or the implementation:

```csharp
public static class OllamaServiceCollectionExtensions
{
    public static IServiceCollection AddOllamaLaunchProvider(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<OllamaLaunchOptions>()
            .Bind(configuration.GetSection("Providers:OllamaLaunch"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<OllamaLaunchClient>();
        services.AddSingleton<IAgentRuntimeProvider, OllamaLaunchProvider>();

        return services;
    }
}
```

The extension is called from `Program.cs` and from
host-specific configuration modules under
`AiEng.Platform.App/Configuration/`:

```csharp
builder.Services.AddOllamaLaunchProvider(builder.Configuration);
builder.Services.AddGitProvider(builder.Configuration);
builder.Services.AddPowerShellProvider(builder.Configuration);
```

### 5.2 The Registry

The provider registry is the only way the UI learns about
providers. It is built at startup from the registered
implementations and is consumed by pages, components,
services, and view models through DI:

```csharp
public interface IAgentRuntimeProviderRegistry
{
    IReadOnlyList<AgentRuntimeProviderDescriptor> List();
    IAgentRuntimeProvider? Resolve(ProviderId id);
}
```

A registry is a `Singleton` service. It is **not** a static
field. The registry is built once from DI at startup; it then
exposes a read-only view of the available providers.

The registry's `List()` method returns only the providers
that are `Enabled` (per the lifecycle states in § 4.6). A
provider that is `Registered` but not `Enabled` is
invisible to the UI; the registry filters it out.

### 5.3 Configuration-Driven Enablement

A provider is **registered** in DI by the composition
root unconditionally. A provider is **enabled** only if
its configuration section is present and valid. The
two are separate events: registration wires the
implementation; enablement surfaces it to the UI.

The registration extension checks the configuration
section and updates the `IProviderRegistry`'s internal
enablement map. A missing or invalid section
transitions the provider from `Registered` to
`Registered (disabled)`; the registry's `List()` does
not return it. A present and valid section transitions
the provider from `Registered` to `Enabled`; the
registry's `List()` returns it.

This is how a user enables or disables a provider: add
or remove the configuration section from
`appsettings.json` (or the user-level setting). The UI
sees the change on the next registry refresh. The
implementation (`*Provider` class) is still present
in DI; the registry simply does not surface it.

---

## 6. Provider Configuration

### 6.1 Strongly-Typed Options

Every provider has a strongly-typed options class. The class:

- Is a `record` with `init`-only properties.
- Has `[Description]` attributes for tooling.
- Is validated with `DataAnnotations` and `IValidateOptions`.
- Is bound to a configuration section
  (`Providers:<ProviderName>`).

### 6.2 Secrets

Secrets (API keys, OAuth tokens) are **never** in
`appsettings.json`. They are read from the OS credential vault
through `ICredentialVault`:

```csharp
public interface ICredentialVault
{
    Task<string?> GetAsync(string key, CancellationToken ct);
    Task SetAsync(string key, string value, CancellationToken ct);
    Task RemoveAsync(string key, CancellationToken ct);
}
```

A provider that needs a secret receives the vault through DI
and reads the secret at the start of a call.

### 6.3 Runtime Configuration

Some providers accept configuration through the UI
(`ConfigureAsync` on the base contract). A change goes through:

1. The user enters the value in `AppProviderSettingsForm`.
2. The form calls `ICredentialVault.SetAsync` for secrets.
3. The form calls `IAgentRuntimeProvider.ConfigureAsync` for
   non-secrets.
4. The provider updates its in-memory state.

---

## 7. Provider Health

`HealthAsync` returns a `ProviderHealth`:

```csharp
public readonly record struct ProviderHealth(
    ProviderHealthState State,
    string? Description)
{
    public static ProviderHealth Healthy() => new(ProviderHealthState.Healthy, null);
    public static ProviderHealth Degraded(string description) => new(ProviderHealthState.Degraded, description);
    public static ProviderHealth Unhealthy(string description) => new(ProviderHealthState.Unhealthy, description);
}
```

States:

- `Healthy` — the provider is fully operational.
- `Degraded` — the provider works but with limitations (slow,
  rate-limited, partial capability).
- `Unhealthy` — the provider is not operational.

The UI renders health through `AppHealthDot` and `AppProviderCard`.

Health is checked:

- On application startup (in parallel, with a short timeout).
- On a periodic schedule (configurable, default 30s).
- On user demand (a "test connection" button).

---

## 8. Provider Testing

### 8.1 Contract Tests

Every provider implementation passes the contract test suite
for its family. The test class is shared across implementations
and lives in
`tests/AiEng.Platform.ProviderContractTests/<Family>/`:

```csharp
public abstract class AgentRuntimeProviderContractTests
{
    protected abstract IAgentRuntimeProvider CreateProvider();

    [Fact]
    public async Task CompleteAsync_WhenValid_ReturnsSuccess() { ... }

    [Fact]
    public async Task CompleteAsync_WhenNetworkFails_ReturnsFailure() { ... }

    [Fact]
    public async Task HealthAsync_WhenConfigured_ReturnsHealthy() { ... }
}
```

Each provider implements the abstract class and supplies a
test double of the external service:

```csharp
public sealed class OllamaProviderContractTests : AgentRuntimeProviderContractTests
{
    protected override IAgentRuntimeProvider CreateProvider()
    {
        var fakeServer = new FakeOllamaServer();
        var options = new OllamaOptions { Endpoint = fakeServer.Uri };
        var client = new OllamaClient(options, ...);
        return new OllamaProvider(options, client, NullLogger<OllamaProvider>.Instance);
    }
}
```

### 8.2 Fake Servers

A provider that talks to an external service ships with a
**fake server** used in contract tests. The fake server:

- Is deterministic.
- Supports the happy path and the documented failure paths.
- Is fast (under 100ms per call).
- Runs in-process when possible.

For HTTP-based providers, the fake server is an
`HttpMessageHandler` mock. For process-based providers, it is
a stub process started by the test.

### 8.3 Integration Tests

Beyond contract tests, an integration test exercises the
provider against a real external service (or a close facsimile).
Integration tests are tagged `[Trait("Category", "Integration")]`
and are not run in the default CI pipeline.

---

## 9. Authoring a New Provider

The checklist for adding a new provider:

1. Identify the family. The families are listed in § 2. If no
   family fits, propose a new one in `DECISIONS.md` (per
   ADR-012). A vague family name is a smell: name the
   **capability**, not the implementation origin.
2. Add a new project `AiEng.Platform.Providers.<X>/` to the
   solution. Do not put provider implementations in a
   shared `Providers/` folder.
3. Add a strongly-typed options class with validation.
4. Implement the family contract.
5. Write a fake server (or stub) for testing.
6. Add a contract test class that inherits the family
   contract test base, in
   `tests/AiEng.Platform.ProviderContractTests/<Family>/<Provider>/`.
7. Add a registration extension method under
   `AiEng.Platform.App/Composition/<Capability>/`. The
   extension is the only place that may bind the
   concrete `*Provider` class to the family contract.
8. Add the registration call to `Program.cs` (and to
   the host-specific configuration module under
   `AiEng.Platform.App/Configuration/`) **unconditionally**.
   Enablement is a separate event driven by the
   configuration section's presence; the registration
   extension is called regardless.
9. Add the provider's documentation to this file under the
   family section.
10. Add a provider card in the UI through `AppProviderCard`.

A provider that ships without contract tests is rejected.
A provider that is registered outside the composition
root (per ADR-016) is rejected; the composition root is
the only place in the solution that may reference a
`Providers.<X>` project directly.

---

## 10. Provider Catalogue

The catalogue is the operational catalogue of every provider the
platform has on its roadmap. Each entry shows the family, the
contract, the transport, and the milestone that introduces it.
A provider that has not yet been implemented is listed with its
planned milestone; the row is removed (not struck through) when
the provider is added or its milestone is reassigned.

### 10.1 Agent Runtimes

#### Ollama Launch

- **Contract:** `IAgentRuntimeProvider`.
- **Transport:** Process invocation of `ollama launch <runtime>
  --model <model>` through `IProcessRunner` (the only project
  allowed to call `Process.Start` is
  `AiEng.Platform.Infrastructure`). The model is configuration,
  not code: the example invocation is
  `ollama launch claude --model minimax-m3:cloud`.
- **Status:** Planned for M6 (the platform's first coding-agent
  vertical slice).

#### Ollama API

- **Contract:** `IAgentRuntimeProvider`.
- **Transport:** HTTPS to the local Ollama HTTP server
  (typically `http://localhost:11434`) through
  `IHttpClientFactory`.
- **Status:** Planned for a later milestone. Distinct from
  Ollama Launch; see § 2.1.

#### Claude

- **Contract:** `IAgentRuntimeProvider`.
- **Transport:** HTTPS to the Anthropic API.
- **Auth:** API key from `ICredentialVault`.
- **Status:** Planned for M7.

#### OpenAI

- **Contract:** `IAgentRuntimeProvider`.
- **Transport:** HTTPS to the OpenAI API.
- **Auth:** API key from `ICredentialVault`.
- **Status:** Planned for M7.

#### Codex

- **Contract:** `IAgentRuntimeProvider`.
- **Transport:** Local Codex CLI (when stable) or HTTPS to
  the OpenAI API otherwise.
- **Auth:** API key from `ICredentialVault` (HTTPS) or
  the user's `OPENAI_API_KEY` (CLI).
- **Status:** Planned for post-M7.

### 10.2 Git

#### Git

- **Contract:** `IGitProvider`.
- **Transport:** Process invocation of the `git` CLI through
  `IProcessRunner` (the only project allowed to call
  `Process.Start` is `AiEng.Platform.Infrastructure`).
- **Status:** Planned for M6.

### 10.3 Terminals

#### PowerShell

- **Contract:** `ITerminalProvider`.
- **Transport:** Process invocation through `IProcessRunner`.
- **Security:** Explicit per-call confirmation.
- **Status:** Planned for M6.

#### WSL

- **Contract:** `ITerminalProvider`.
- **Transport:** Process invocation of `wsl.exe` through
  `IProcessRunner`.
- **Security:** Explicit per-call confirmation.
- **Status:** Planned for M6.

#### Windows Terminal

- **Contract:** `ITerminalProvider`.
- **Transport:** Windows Terminal command-line arguments.
- **Status:** Planned for M6.

#### Git Bash

- **Contract:** `ITerminalProvider`.
- **Transport:** Process invocation of `git-bash.exe` through
  `IProcessRunner`.
- **Security:** Explicit per-call confirmation.
- **Status:** Planned for M6.

### 10.4 Worktrees

#### Native (worktree)

- **Contract:** `IWorktreeProvider`.
- **Transport:** Built on `IGitProvider` and the local file
  system. No external dependency.
- **Status:** Planned for M6.

#### Treehouse

- **Contract:** `IWorktreeProvider`.
- **Transport:** Treehouse HTTP API.
- **Status:** Planned for M2 dogfooding checkpoint (per
  `ROADMAP.md`); the Treehouse provider is added when the
  Application layer has a workspace to dogfood against.

### 10.5 Quality Gates

#### No Mistakes

- **Contract:** `IQualityGateProvider`.
- **Transport:** No Mistakes HTTP API.
- **Status:** Planned for M8; M3 dogfooding checkpoint uses
  the local rule set first.

### 10.6 Reviews

#### Native (review)

- **Contract:** `IReviewProvider`.
- **Transport:** Built on the file system and the Application
  layer's review orchestrator. No external dependency.
- **Status:** Planned for M8.

#### Lavish Axi

- **Contract:** `IReviewProvider`.
- **Transport:** Lavish Axi HTTP API.
- **Status:** Planned for M1 dogfooding checkpoint (per
  `ROADMAP.md`); the Lavish Axi provider is added when the
  design-system documentation page renders.

### 10.7 Autonomous Loops

#### GNHF

- **Contract:** `IAutonomousLoopProvider`.
- **Transport:** GNHF HTTP API.
- **Status:** Planned for M5 dogfooding checkpoint (per
  `ROADMAP.md`); the GNHF provider is added when the
  Application layer has an agent runtime to dogfood against.

### 10.8 Orchestrations

#### Native (orchestration)

- **Contract:** `IOrchestrationProvider`.
- **Transport:** Built on the Application layer's task
  orchestrator. No external dependency.
- **Status:** Planned for M8.

#### Firstmate

- **Contract:** `IOrchestrationProvider`.
- **Transport:** Firstmate HTTP API.
- **Status:** Planned for a later milestone; Firstmate is
  the last family to be onboarded.

### 10.9 Workspace State (Not a Provider)

Workspace state is **not** a provider. It is application-layer
state that lives in `AiEng.Platform.Application/Services/Workspace/`
backed by a session-tier service. The previous
`IWorkspaceProvider` family was removed in ADR-012 because the
file system is part of the platform's own runtime, not an
external tool.

---

## 11. When the Provider Model Is Wrong

A provider is a poor fit when:

- The external tool is interactive (CLI menus, prompts). Wrap
  it in a process and treat it as a terminal provider.
- The external tool has no network or process surface. It is
  not a provider; it is a service in `Services/`.
- The external tool's contract is unstable. Either pin a
  version or treat it as a terminal provider.

A provider that needs to know about UI is wrong. A provider
that needs to know about another provider is wrong. A provider
that needs to know about persistence is wrong.
