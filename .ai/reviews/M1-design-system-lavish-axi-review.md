# M1 Design System — Lavish Axi Review (Deferred)

> **Status: DEFERRED — tool not available on the host.**
>
> This file is the placeholder for the M1 design-system
> review that PART 2 of the M1 closeout brief requested.
> The review itself could not be run during the M1
> closeout session because the prerequisite tool,
> `lavish-axi`, is not installed on the host that ran the
> M1 work. The prereq check, the missing-binary finding,
> and the no-fallback conclusion are recorded here so the
> next session has the full context.

---

## 1. Brief Recap

PART 2 of the M1 closeout brief authorised an M1
design-system review using `lavish-axi`, with three
guard rails:

- The exact command must be shown before execution.
- No upstream repository (the `lavish-axi` repository)
  may be modified.
- No `IReviewProvider` is created. No `lavish-axi`
  source is copied into this repository. The review is
  dogfooding the external tool, not integrating it.

The expected output of PART 2 was this file
(`.ai/reviews/M1-design-system-lavish-axi-review.md`)
populated with the review's findings, plus resolution of
all Blocker and High findings. PART 2 of the brief also
called for resolving all Blocker / High findings before
proceeding to PART 3.

## 2. Prerequisite Check (`.ai/workflows/tool-dogfooding.md`)

Per the tool-dogfooding workflow, before running any
external tool the session must (a) identify the tool,
(b) verify the executable, (c) read the tool's usage
documentation, (d) get user approval, and (e) show the
exact command before execution. The M1 closeout brief
collapsed (a) and (d) into the brief itself, leaving
(b), (c), and (e) to the session.

### 2.1 Tool identification

`lavish-axi` is documented in
`C:\Users\hkasozi\agent-workbench\tools\lavish-axi.md`
as a long-running, opinionated AI orchestration daemon
that listens to file-system events, runs project
commands, and exposes a small HTTP API (default port
7474) for other tools to query and trigger workflows.
The document lists `lavish-axi start`, `lavish-axi stop`,
and `lavish-axi status` as the documented commands.

The documented purpose of `lavish-axi` is **project
orchestration and a structured project-status view**:
"what changed, what depends on what, what passed, what
failed". It is **not** a code-review tool. The
documented HTTP API exposes project status, not
review findings. No `lavish-axi review` or equivalent
review command is described in the tool spec.

This is a mismatch with the PART 2 brief, which
expected `lavish-axi` to perform an M1 design-system
review.

### 2.2 Executable verification

The session searched the host for any `lavish-axi`
binary, service, or process:

| Probe | Command | Result |
| ----- | ------- | ------ |
| PATH lookup | `where lavish-axi`, `where lavish`, `where axi`, `where axiom` | No matches. |
| Global npm | `npm ls -g` | `continuedev/cli`, `openai/codex`, `opencode-ai`, `npm` only. No `lavish-axi`. |
| User Programs | `C:\Users\hkasozi\AppData\Local\Programs\*` | `Antigravity`, `Antigravity IDE`, `Common`, `LM Studio`, `Microsoft VS Code`, `Ollama`, `Python`, `TRAE SOLO`, `WezTerm`, `WinSCP`. No `lavish-axi`. |
| Windows services | `Get-Service \| Where-Object { Name like *axi* or *lavish* }` | Only `AxInstSV` (the unrelated Windows ActiveX Installer service). |
| Running processes | `Get-Process \| Where-Object { Name like *axi* or *lavish* }` | None. |
| TCP port 7474 | `Get-NetTCPConnection -LocalPort 7474` | Nothing listening. |
| Filesystem search | `find /c -maxdepth 5 -iname "*lavish*"` | Only `C:\Users\hkasozi\agent-workbench\tools\lavish-axi.md` (the spec). |
| Web search | `"lavish-axi" code review tool installation` | No public product match. |

**Conclusion.** `lavish-axi` is not installed on the
host that ran the M1 work. The only artefact on the
filesystem is the `agent-workbench/tools/lavish-axi.md`
spec document, which is a description of the tool, not
the tool itself.

### 2.3 No silent fallback

The `.ai/workflows/tool-dogfooding.md` workflow is
explicit: "If the tool is not available, the session
does **not** silently fall back to a different tool or
version. The session stops, reports the failure to the
human, and asks for a decision." The `AGENTS.md` "do
not invent" rule reinforces this.

The session therefore does **not**:

- Install `lavish-axi` from a non-canonical source.
- Substitute a different review tool (for example,
  `opencode-ai`, `codex`, or `feature-dev`'s
  `code-reviewer` agent) without user approval.
- Generate fabricated review findings.
- Modify the `agent-workbench/tools/lavish-axi.md`
  spec, the `agent-workbench` repository, or any
  upstream `lavish-axi` repository.

## 3. Decision Required

The M1 closeout session cannot complete PART 2 without
one of the following user decisions:

### 3.1 Option A — install `lavish-axi` and re-run

The user installs `lavish-axi` on the host (per the
`lavish-axi.md` "Installation" section:
`curl -sSf https://lavish-axi.dev/install.sh | sh` on
macOS / Linux; the Windows installer at
`https://lavish-axi.dev/releases` or WSL on Windows).
The next AI session re-runs PART 2 with the tool
available. **Risk:** the tool spec does not document a
review command; the next session may still need a
follow-up decision on what "dogfood Lavish Axi review"
means in practice.

### 3.2 Option B — substitute a documented review tool

The user picks a documented review tool that is
installed and reachable on the host. The candidate
list, in order of availability:

- `opencode-ai` (installed, v1.17.3) — an AI coding
  agent in the terminal; capable of running review
  prompts.
- The Anthropic `feature-dev` plugin's `code-reviewer`
  agent (installed under
  `C:\Users\hkasozi\.claude\plugins\marketplaces\claude-plugins-official\plugins\feature-dev\agents\code-reviewer.md`).
- A manual review (the AI session reads
  `/design-system` and the design-system catalogue,
  applies the `.ai/templates/review-report.md`
  dimensions, and writes the findings to this file).

The substitute tool runs the review against the M1
design system; findings are written to this file;
Blocker / High findings are resolved before PART 3
begins.

### 3.3 Option C — defer PART 2 to a later session

PART 2 is closed with a "deferred" status. M1 closes
without a `lavish-axi` review. A new task is added to
the project task board (`.ai/state/task-board.md`) to
run the review in a future session, after the
prerequisite tool is available. M1 closeout proceeds
with PART 3 through PART 7 unchanged.

The M1 closeout session's recommendation is **Option C
(defer to a later session)**, because Option A is
blocked by the missing review capability in the
documented tool spec, and Option B substitutes the
tool that the brief named. PART 2's explicit
constraint — "the user authorizes this M1 Lavish Axi
review session, provided the exact command is shown
before execution and no upstream repository is
modified" — is most faithfully honoured by deferring
the review rather than substituting a different tool.

## 4. What This File Is (and Is Not)

This file **is**:

- The prereq check for the M1 `lavish-axi` review
  (`AGENTS.md` "do not invent" rule; `.ai/workflows/tool-dogfooding.md`
  "no silent fallback" rule).
- A record of the missing-binary finding.
- A record of the documented-purpose mismatch (the
  spec describes an event-bus daemon, not a review
  tool).
- A decision request for the user.

This file **is not**:

- A code review. No findings are recorded.
- A workaround. The session did not substitute a
  different tool or fabricate findings.

## 5. Linked Artefacts

- `C:\Users\hkasozi\agent-workbench\tools\lavish-axi.md` —
  the tool spec the session was asked to dogfood.
- `.ai/workflows/tool-dogfooding.md` — the workflow the
  session followed.
- `.ai/templates/review-report.md` — the template the
  next session uses when it runs the actual review.
- `AGENTS.md` — Rule 1 (do not invent), Rule 13 (no
  comments), Rule 14 (progressive self-dogfooding).
- The M1 closeout brief — PART 2 specifies the
  `lavish-axi` review with the three guard rails
  enumerated in § 1 of this file.
