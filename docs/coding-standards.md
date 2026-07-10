# docs/coding-standards.md

> The complete coding standard for the AI Engineering Platform. This
> document expands on `STYLEGUIDE.md` with the operational details
> that govern how code is written day to day.

---

## 1. Scope

This document covers:

- C# language rules and idioms.
- Razor and Blazor patterns.
- Async, threading, and cancellation.
- Error handling.
- Logging and observability.
- Resource lifetime and disposal.
- Comments policy.
- Testing conventions.

It is the **third** document to read for any implementation task,
after `AGENTS.md` and `ARCHITECTURE.md`.

---

## 2. C# Language Rules

### 2.1 Modern Features

- File-scoped namespaces.
- Primary constructors for DI services and components.
- `record` and `record struct` for DTOs and immutable values.
- Collection expressions (`[...]`) for initialisation.
- Pattern matching (`switch` expressions) for branching.
- Nullable reference types are enabled. Warnings are errors.

### 2.2 Class Design

- A class has one responsibility, named in the class name.
- A class is `sealed` unless inheritance is part of the design.
- A class is `internal` unless it is part of the public surface.
- A class with more than 200 lines of code is a candidate for
  extraction. The exception is generated code and large
  configuration classes.
- A method longer than 30 lines is a candidate for extraction.

### 2.3 Constructors

- Primary constructors are preferred for DI.
- Multiple constructors are a smell; consolidate or split the class.
- Constructors do no work. Initialisation belongs in explicit methods
  or in `IHostedService.StartAsync`.

### 2.4 Properties

- Properties are simple, predictable, and side-effect free.
- Properties do not perform I/O.
- Properties that may be missing are nullable (`string?`).
- `init`-only setters are preferred for immutability.

### 2.5 Methods

- A method does one thing.
- A method has at most four parameters; more than that means a
  parameter object.
- A boolean parameter is almost always wrong. Replace with a named
  method or an enum.
- Async methods end in `Async`.
- Methods that may fail return `Result<T>` or `ProviderResult<T>`,
  not throw.

### 2.6 Extension Methods

- Extensions are scoped to a single namespace.
- Extensions live in `Extensions/` next to the type they extend.
- Extensions are minimal. A multi-line extension is a regular
  method.

### 2.7 Records and DTOs

- DTOs are `record` types.
- DTOs are immutable.
- DTOs do not contain behaviour.
- DTOs are the only type allowed in cross-layer data transfer.

### 2.8 Enums

- Enums describe a closed set of values.
- Enums are declared with explicit underlying type only when
  serialised.
- Enums that drive UI render an enum-to-class mapping in the
  component.

### 2.9 Null Handling

- `?` is used deliberately.
- A method that may return nothing returns `T?`.
- A method that may fail returns `Result<T>`, not `T` and not
  `T?`.
- `ArgumentNullException.ThrowIfNull(...)` is used at the start of
  every public method.

---

## 3. Razor and Blazor Rules

### 3.1 Component Anatomy

- Markup-only components use `@code { }` in the `.razor` file.
- Components with behaviour live in `Component.razor` +
  `Component.razor.cs`. The `partial` keyword is implicit.
- Parameters are declared at the top of the class.
- Event callbacks are declared after parameters.
- State fields follow callbacks.
- Lifecycle methods (`OnInitialized`, `OnParametersSet`,
  `OnAfterRender`) follow state.

### 3.2 Parameter Rules

- Parameters are public properties with `[Parameter]`.
- Required parameters use `[EditorRequired]`.
- Parameters are never `ref` or `out`.
- Parameters that are collections default to empty, not null.
- Parameters are immutable in spirit; do not mutate inputs.

### 3.3 State

- State lives in private fields with `_camelCase`.
- State that drives re-render is `[Parameter]` or instance field on
  the component.
- State that survives navigation lives in a scoped service.
- State that survives restart lives in infrastructure.

### 3.4 Lifecycle

- `OnInitialized` is used for one-time setup, not for service calls
  that need re-running on parameter change.
- `OnParametersSet` is used when derived state depends on
  parameters.
- `OnAfterRender` is used only for DOM interop. It is never used to
  load data.
- `IDisposable.Dispose` is implemented when a component subscribes
  to events or holds disposable resources.

### 3.5 Rendering

- A component must render in under 16ms for typical data.
- Lists larger than 50 items are virtualised.
- `ShouldRender` is overridden only when the cost of re-rendering
  is high and the inputs are reference-equal.
- Markup does not perform computation. Methods return
  `RenderFragment` when conditional markup is complex.

### 3.6 Component Composition

- Pages are thin. They compose components and forward events.
- Reusable components do not reference domain components directly.
- Domain components are leaf nodes; they depend on primitives and
  containers, not on other domain components.

---

## 4. Async, Threading, and Cancellation

### 4.1 Async Discipline

- All I/O is async.
- All async methods accept a `CancellationToken` as the last
  parameter. No exceptions.
- `ConfigureAwait(false)` is used in library code, not in Blazor
  code.
- `Task.Run` is used only for CPU-bound work that must be moved off
  the main thread.

### 4.2 Cancellation

- Every long-running call respects cancellation.
- Components that own work own the token. Navigating away cancels.
- A cancelled task is not an error. It is the expected outcome of
  cancellation.
- A token flows from the page to the service to the provider. No
  provider call runs without a token.

### 4.3 Streams

- Long-running work is exposed as `IAsyncEnumerable<T>`.
- A stream yields discrete events, not arbitrary progress pings.
- A stream is disposed by the consumer. Components implement
  `IAsyncDisposable` for cleanup.

### 4.4 Concurrency

- `lock` is used for short, well-understood critical sections.
- `Channel<T>` is used for producer/consumer flows (e.g. terminal
  output).
- `SemaphoreSlim` is used for limiting concurrent access to a
  provider.
- `ConcurrentDictionary` is used for shared read-mostly maps
  (e.g. provider registry snapshot).

---

## 5. Error Handling

### 5.1 Layers of Error Handling

- **Infrastructure:** catches and logs every unhandled exception.
  No user-facing messages.
- **Provider:** returns `ProviderResult<T>`. The `Failure` case
  carries a typed error, a message, and a category.
- **Application:** translates `ProviderResult` into domain outcomes
  the UI understands. Logs at `Warning` for known failures and
  `Error` for unexpected ones.
- **Presentation:** renders an `AppErrorState` for failures. Never
  shows a stack trace.

### 5.2 ProviderResult Envelope

```csharp
public readonly record struct ProviderResult<T>(
    ProviderOutcome Outcome,
    T? Value,
    ProviderError? Error)
{
    public static ProviderResult<T> Success(T value) => ...;
    public static ProviderResult<T> Failure(ProviderError error) => ...;
    public static ProviderResult<T> Unavailable(string reason) => ...;
}
```

- `Success` carries a value.
- `Failure` carries a `ProviderError` with category, message, and
  optional inner exception.
- `Unavailable` indicates the provider is not configured or not
  reachable; the UI may offer to enable it.

### 5.3 Exceptions

- Exceptions are reserved for programmer errors and unrecoverable
  conditions.
- `ArgumentException` is used for invalid arguments.
- `InvalidOperationException` is used when the program is in a
  state that cannot proceed.
- Custom exception types are reserved for cases where the catch site
  needs to distinguish the failure by type. The default is a
  `Result` envelope.

---

## 6. Logging

### 6.1 Structured Logging

- Use `ILogger<T>` with structured message templates.
- Every log entry includes the correlation id of the operation
  (session id, run id, request id).
- Never log secrets, tokens, or full file contents.
- Use the appropriate level:
  - `Trace` / `Debug` — diagnostic detail (off in production).
  - `Information` — significant lifecycle events.
  - `Warning` — recoverable failures.
  - `Error` — unexpected failures that affected a single operation.
  - `Critical` — failures that may bring down the process.

### 6.2 Logging in Components

- Components do not log directly. They delegate to a service.
- The service logs with a stable category and includes the
  operation context.

---

## 7. Resource Lifetime

- All `IDisposable` resources are owned by the type that created
  them.
- `IAsyncDisposable` is used when disposal is naturally async.
- DI containers manage the lifetime of registered services.
- A component that subscribes to events unsubscribes in
  `Dispose`.
- A component that owns a long-running task cancels and disposes
  it in `Dispose`.

---

## 8. Configuration

- Configuration is bound to strongly-typed options classes.
- Options classes are validated at startup.
- Secrets are read from the OS credential vault through a
  dedicated service. Configuration files never contain secrets.
- Environment-specific configuration lives in
  `appsettings.{Environment}.json`.

---

## 9. Comments Policy

**Comments are forbidden in source files.** This rule is repeated
here so that no contributor can claim ignorance.

The only documentation that may appear in a source file is
**contractual** XML documentation on **public API surfaces only**:

- `<param>` — describes what the parameter represents when the name
  is not sufficient.
- `<returns>` — describes what the caller can expect.
- `<exception>` — lists the exception types the method may throw
  and the conditions.
- `<typeparam>` — describes a generic type parameter.

These are not commentary. They are the API contract the compiler
cannot infer.

Everything else — design rationale, business rules, "why we do
this", TODOs, NOTES, warnings — lives in `docs/` and
`DECISIONS.md`. If the code is unclear, the code is wrong; refactor
it. If the design is unclear, document the design.

A reviewer rejects any change that introduces a code comment.

---

## 10. Testing

### 10.1 Test Pyramid

- **Unit tests** for application services and pure logic.
- **Component tests** (bUnit) for components.
- **Contract tests** for every provider implementation.
- **Integration tests** for storage, configuration, and provider
  round-trips against a fake server.

### 10.2 Naming

Test classes and methods follow the pattern:

```
<ClassName>Tests
    <MethodName>_<Scenario>_<ExpectedResult>
```

Example: `RuntimeProviderRegistryTests.ListAsync_WhenProviderDisabled_ReturnsEmpty`.

### 10.3 Structure

- `Arrange / Act / Assert` blocks are separated by blank lines.
- One assertion per test is the default. Multiple assertions are
  acceptable when they describe a single behaviour.
- Tests are independent. No shared mutable state between tests.

### 10.4 Coverage

- New code is shipped with tests.
- A provider implementation without contract tests is rejected.
- A component without a bUnit test for its primary render is
  rejected.

### 10.5 What Not to Test

- Trivial accessors.
- Generated code.
- Third-party libraries (rely on their tests).

---

## 11. Forbidden Patterns

- `static` state that is not immutable.
- Thread.Sleep.
- `Task.Wait`, `.Result`, `.GetAwaiter().GetResult()` in
  application code.
- `new HttpClient()` (use `IHttpClientFactory`).
- `DateTime.Now` (use `IClock`).
- `Environment.GetEnvironmentVariable` outside the configuration
  layer.
- Magic strings naming providers, themes, or roles.

---

## 12. Tooling

- `dotnet format` runs in CI.
- Roslynator rules are enabled.
- A custom analyzer rejects code comments in `.cs` and `.razor`
  files. The analyzer is part of the build from M1 onwards.
- The build fails on any warning (treating warnings as errors is
  enabled in the project file).

---

## 13. When the Standard Is Wrong

Open a PR that:

1. Demonstrates the problem with a concrete example.
2. Proposes the new rule.
3. Updates the files the change affects.
4. Records the decision in `DECISIONS.md`.

The standard survives only if it earns its place.
