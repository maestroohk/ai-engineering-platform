# Tools

> **Developer tooling.** The `tools/` directory hosts
> scripts that drive the AI operating layer. The router
> is the only entry today; future entries (CI helpers,
> benchmark runners, archive sync) follow the same
> conventions.

## Router

`ai-session-router.ps1` is the production Windows-first
PowerShell supervisor that drives one bounded Claude
Code child session per phase, each on a different
configured cloud model. Compatible with PowerShell 5.1+
(`powershell.exe`, not `pwsh`).

### Commands

| Command      | Purpose                                                          |
| ------------ | ---------------------------------------------------------------- |
| `Next`       | Dispatch the next phase of the active task.                      |
| `Plan`       | Dispatch the plan phase (forces `high` profile when available).  |
| `Resume`     | Resume from the last receipt's `next_phase`.                     |
| `Finish`     | Print a message that closeout is performed by the child.         |
| `Status`     | Print router version, model routing, and active task.             |
| `Configure`  | Ask for the standard and economy model names; persist to JSON.   |
| `DryRun`     | Print the launch command without executing it.                   |

### Flags

| Flag               | Purpose                                                          |
| ------------------ | ---------------------------------------------------------------- |
| `-TaskId T-NNN`    | Override the active task ID.                                     |
| `-ProfileOverride <p>` | Force a profile for the next dispatch.                       |
| `-NoPush`          | Allow the closeout phase to push when a remote is configured.    |
| `-DryRun`          | Print the launch command without executing it.                   |

### Safety

- Argument list, not string concatenation. No
  `Invoke-Expression`.
- Model name and task ID validated against a strict
  regex (`^[A-Za-z0-9._:-]+$` and `^T-[0-9]+$`).
- Paths validated against the repository root.
- Ctrl+C cancels the supervisor and terminates the
  child process tree.
- 429 / usage exhaustion selects the configured
  `fallback` profile when allowed; otherwise the router
  stops cleanly.
- The router never hot-swaps a running Claude Code
  process. Each phase launches a fresh child.

## Pester Tests

`ai-session-router.Tests.ps1` exercises the router with
mocked child processes. The tests do **not** invoke
`ollama launch claude` and do **not** consume Ollama
cloud quota. Run with:

```powershell
powershell.exe -NoProfile -Command "Invoke-Pester -Path tools\ai-session-router.Tests.ps1 -Output Detailed"
```

## Example

`ai-session-router.example.ps1` is a runnable example
the user can copy to `ai-session-router.ps1` and edit.
The production router is the canonical implementation.

## Conventions

- PowerShell 5.1+ syntax. No `pwsh`-only features
  (no `?:`, no `??`, no `&&`/`||` chain operators).
- Verb-Noun function names.
- `Set-StrictMode -Version Latest`.
- `$ErrorActionPreference = 'Stop'`.
- All paths under the repository root.
- No secrets written to disk.
