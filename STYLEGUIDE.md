# STYLEGUIDE.md

> The single source of truth for code style. This document governs C#,
> Razor, CSS, and the small set of conventions that keep the codebase
> consistent. Read it after `AGENTS.md` and `ARCHITECTURE.md`.

---

## 1. Principles

The style guide is not about taste. It is about **readability at scale**.
A platform with hundreds of components and dozens of providers can only
remain maintainable if every file looks like it was written by the same
author.

The rules in this document are deliberately opinionated. Where a rule
seems arbitrary, the rationale is given.

---

## 2. General Rules

- Code is read more often than it is written. Optimise for the reader.
- The best code is the code that is not there. Delete before you
  refactor.
- If a rule in `AGENTS.md` conflicts with a rule in this document, the
  rule in `AGENTS.md` wins.

---

## 3. C# Style

### 3.1 Language Version

- Target **.NET 10** and **C# 14** features.
- Use `file`-scoped namespaces.
- Use primary constructors for DI services.
- Use `record` types for DTOs and immutable value objects.
- Use `sealed` by default on classes that are not designed for
  inheritance.

### 3.2 Naming

| Element              | Convention                       | Example                                |
| -------------------- | -------------------------------- | -------------------------------------- |
| Namespace            | `PascalCase`, no underscores     | `AiEng.Platform.Providers.Ollama`      |
| Class                | `PascalCase`, descriptive        | `RuntimeProviderRegistry`              |
| Interface             | `IPascalCase`                    | `IAgentRuntimeProvider`                |
| Method               | `PascalCase`, verb-led           | `ListAsync`, `ResolveAsync`            |
| Property             | `PascalCase`                     | `IsHealthy`, `DisplayName`             |
| Field (private)      | `_camelCase`                     | `_logger`, `_registry`                 |
| Field (const)        | `PascalCase`                     | `DefaultTimeout`                       |
| Variable (local)     | `camelCase`                      | `sessionId`, `providerKey`             |
| Parameter            | `camelCase`                      | `cancellationToken`, `workspaceId`     |
| Enum value           | `PascalCase`                     | `ProviderState.Healthy`                |
| Async method suffix  | `Async`                          | `StreamAsync`                          |
| Generic type param   | `TPascalCase`                    | `TItem`, `TResult`                     |

Never use Hungarian notation. Never abbreviate (`btn`, `mgr`, `util`).
Names must read like English at the call site.

### 3.3 File Layout

```
// file-scoped namespace
namespace AiEng.Platform.Services;

public sealed class WorkspaceService : IWorkspaceService
{
    private readonly ILogger<WorkspaceService> _logger;
    private readonly IWorkspaceStore _store;

    public WorkspaceService(
        ILogger<WorkspaceService> logger,
        IWorkspaceStore store)
    {
        _logger = logger;
        _store = store;
    }

    public Task<Workspace> OpenAsync(string path, CancellationToken ct)
        => _store.LoadAsync(path, ct);
}
```

- One type per file, except tightly-coupled nested types.
- File name matches the primary type name.
- Members in order: fields, constructor, public methods, protected
  methods, private methods, nested types.

### 3.4 Async

- Every public method that performs I/O returns `Task` or `Task<T>`.
- Every async method accepts a `CancellationToken` as the last parameter.
- Never use `.Wait()`, `.Result`, or `.GetAwaiter().GetResult()` in
  application code.
- Use `ConfigureAwait(false)` only in library code, not in Blazor
  components or application services.
- Stream long-running work with `IAsyncEnumerable<T>`.

### 3.5 Errors

- Application services throw exceptions only for unrecoverable errors
  (e.g. invalid configuration, programmer mistakes).
- Provider methods return `ProviderResult<T>`. They do not throw for
  external failures (network, auth, rate limits).
- Validation errors are returned, not thrown, when a UI can correct
  them.

### 3.6 Nullability

- Nullable reference types are **enabled** for the entire codebase.
- Use `?` deliberately. A `string?` means the value may be missing; a
  `string` means it is not.
- The compiler's nullable warnings are treated as errors.

### 3.7 No Code Comments

**Comments are forbidden in this repository.** This rule is absolute
and inherits from `AGENTS.md` Rule 13.

- Do not add `//` line comments.
- Do not add `/* */` block comments.
- Do not add `<remarks>` or narrative `<summary>` XML doc comments.
- The only XML documentation permitted is **contractual**: `<param>`,
  `<returns>`, `<exception>`, `<typeparam>` on **public API surfaces
  only** (provider contracts, public service interfaces, public
  component parameters). These exist to describe what the compiler
  cannot infer — never to narrate the code.

Code must be self-explanatory through:

- Descriptive names (types, methods, parameters, variables).
- Small methods with a single responsibility.
- Type signatures that communicate intent.
- Tests that demonstrate behaviour.

If a piece of logic needs explanation, the answer is to refactor the
code so the logic is obvious — **not** to comment it.

Forbidden smells that signal a missing refactor:

- A `// this handles the edge case where…` comment.
- A `// TODO` left in source.
- A multi-line block comment explaining an algorithm.
- A header comment naming an author or a date.

If something genuinely must be communicated and refactoring cannot
remove the need, document it in `docs/` or `DECISIONS.md`, not in
source.

### 3.8 Format and Whitespace

- Four-space indentation. No tabs.
- One statement per line.
- One declaration per line.
- Braces on their own lines (Allman style) for types and methods.
- Single-line bodies for trivial accessors are allowed.
- A file ends with a single newline.

These rules are enforced by `dotnet format` and verified in CI.

---

## 4. Razor Style

### 4.1 File Pairing

Every non-trivial component has two files:

- `Component.razor` — markup only.
- `Component.razor.cs` — behaviour.

A component is "non-trivial" if it owns state, calls services, or has
parameters that affect rendering beyond simple binding.

### 4.2 Code-Behind

```csharp
public partial class AppButton : ComponentBase
{
    [Parameter]
    public string Label { get; set; } = string.Empty;

    [Parameter]
    public ButtonVariant Variant { get; set; } = ButtonVariant.Primary;

    [Parameter]
    public EventCallback OnClick { get; set; }

    private async Task HandleClickAsync()
        => await OnClick.InvokeAsync();
}
```

- Parameters are declared with `[Parameter]`.
- Required parameters use `[EditorRequired]`.
- Event callbacks use `EventCallback` or `EventCallback<T>`, never
  `Action` or `Func`.
- Logic lives in the code-behind, not in `@code { ... }` blocks within
  the markup.

### 4.3 Markup

- Components are always self-closing when they have no children:
  `<AppButton Label="Save" />`.
- Components that contain children use the explicit close tag.
- No inline `style` attributes. Use semantic classes.
- No inline `@if` blocks longer than three branches. Extract a method
  returning `RenderFragment`.
- No `@foreach` over collections that are not pre-materialised. Avoid
  computing in markup.
- Use `Loading`, `Empty`, `Error`, and `Populated` templates
  (or `<Empty>`, `<Error>`, `<Populated>` child content) on
  every component that **owns a data fetch**. Pure
  primitives (`AppButton`, `AppBadge`, `AppIcon`,
  `AppStatusDot`, `AppTooltip`) and presentational
  containers (`AppCard`, `AppSection`, `AppDialog`,
  `AppDrawer`, `AppTabs`, `AppPanel`, `AppToolbar`) do
  not own data and do not require the four state slots.
  The four-state rule is conditional on data ownership
  (see `docs/design-system.md` § 5.4 and
  `docs/component-guidelines.md` § 4.3).

### 4.4 Tailwind

- Long utility chains in markup are a smell. Replace with a semantic
  class from the design system.
- Conditional classes use a small helper, not string concatenation
  inside `class="..."`.
- No `!important` overrides. If a Tailwind utility conflicts with a
  semantic class, the design system is the source of truth.

---

## 5. CSS and Tailwind

### 5.1 Tailwind Is the Engine

Tailwind is the only styling system. No Bootstrap, no Material, no
custom CSS frameworks.

### 5.2 Semantic Classes via `@apply`

Whenever the same combination of utilities appears more than twice, a
semantic class is created. The class is defined in the component's
co-located `.razor.css` file (scoped CSS) or in a design-system CSS
file under `wwwroot/css/design-system/`.

Example:

```css
.app-card {
    @apply rounded-md border border-app-border bg-app-surface p-4 shadow-sm;
}
```

### 5.3 Design Tokens

Colors, spacing, radius, and shadow are **tokens**, not raw values.
Tokens are defined in `tailwind.config.js` and consumed via
`bg-app-surface`, `text-app-fg`, `border-app-border`, etc.

Never write `bg-[#1a1a1a]`, `text-[12px]`, or `rounded-[6px]` in
markup. Use the token.

### 5.4 Forbidden Patterns

- `style="..."` in markup.
- Inline `<style>` blocks.
- `!important`.
- Deeply nested selectors in scoped CSS.
- Global CSS files beyond the design system.

---

## 6. Project Conventions

### 6.1 File Headers

There are no file header comments. The file name, namespace, and type
are the header.

### 6.2 `this.`

Do not prefix member access with `this.` unless required to disambiguate
from a local variable or parameter of the same name.

### 6.3 `var`

Use `var` when the type is obvious from the right-hand side. Use the
explicit type when it aids readability or when the right-hand side is
not obviously typed.

### 6.4 `using` Directives

`using` directives go **inside** the file-scoped namespace, sorted
alphabetically. `ImplicitUsings` is enabled.

### 6.5 Expression-Bodied Members

Use expression bodies for one-liners. Use a block body for anything
that spans more than one statement.

---

## 7. Commit Messages

A commit message:

- Has a subject line of 72 characters or fewer.
- Uses the imperative mood ("Add", not "Added").
- Has a body that explains **why** when the change is non-obvious.
- References the milestone or issue it advances.

Example:

```
Add provider registry contracts

The provider model is the foundation for M3. Defining the contracts
before any concrete implementation lets the diagnostics page render
against a real shape rather than an imagined one.
```

---

## 8. Tooling

- **Format:** `dotnet format` runs in CI. Style violations block merge.
- **Analyse:** Roslynator rules are enabled at the highest level.
- **Lint:** Tailwind classes are validated by a custom build step that
  fails on long utility chains and undefined tokens.
- **Pre-commit:** `dotnet format --verify-no-changes` and
  `dotnet build` run on staged files.

---

## 9. When the Style Guide Is Wrong

If a rule in this guide produces demonstrably worse code, the rule is
the problem. Open a PR that:

1. Demonstrates the problem with a concrete example.
2. Proposes the new rule.
3. Updates every file the change affects.

The change is reviewed like any other. The rule survives only if it
earns its place.
