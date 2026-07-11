# `.ai/commands.md` — Command Protocol

> **Operational shortcut for short user instructions.**
> **This document is subordinate to `AGENTS.md` and the
> precedence hierarchy in `AGENTS.md` § 2.2.** A recognised
> command is a discipline; it does not override the constitution,
> accepted ADRs, the approved roadmap, an approved task plan,
> the safety rules in `AGENTS.md`, or the Git rules in
> `AGENTS.md` and `CONTRIBUTING.md`.

---

## 1. Why This Document Exists

`.ai/session-start.md` and `.ai/workflows/progressive-coding.md`
define the full operational sequence: a session opens, classifies
the request, reads the prompt, inspects the repository, and
either begins work or asks for approval. The full sequence is
deliberate, but a user often wants a **shorter** conversation:

- "Continue." — pick up where the previous session left off.
- "Approved." — go.
- "Status." — where are we?
- "Plan." — show me the next thing.
- "Resume." — keep going on what's already running.
- "Review." — check the current task.
- "Validate." — run the gates.
- "Finish." — close out the current task.

This document defines those eight commands. The commands are
recognised when the user's message matches a command exactly
(case-insensitive). A message that contains a command as part
of a longer sentence is **not** a command; the session falls
back to the full `.ai/session-start.md` sequence.

A command never:

- Bypasses the constitution, an ADR, the roadmap, or a plan.
- Bypasses the safety rules in `AGENTS.md` § 4 (the 17 rules).
- Bypasses the Git rules in `AGENTS.md` and `CONTRIBUTING.md`.
- Implements a task whose plan is `Draft` or `Awaiting Approval`.
- Selects a new task while another is `In Progress` (except
  `Resume`, which is the one exception).
- Treats a previous session's "Stop" as a permanent prohibition
  against the next session's work.

A session closeout is not a permanent stop. A closeout ends the
session, updates the state, prepares the next task, and **awaits
the next user command**. A later `Continue` (or `Plan`, or
`Approve`, or any other command) begins a new operational cycle.

---

## 2. Command Precedence

When a command appears to conflict with a higher-precedence
document, the higher-precedence document wins. Specifically:

1. `AGENTS.md` — the constitution (highest).
2. `DECISIONS.md` — accepted ADRs.
3. The approved roadmap (`ROADMAP.md` and
   `.ai/plans/master-delivery-plan.md`).
4. An approved task plan in `.ai/plans/`.
5. The safety and Git rules in `AGENTS.md` and `CONTRIBUTING.md`.
6. This document (`.ai/commands.md`).

A command that asks the AI to violate any of (1)–(5) is **not
honoured**. The AI states the conflict, cites the rule, and
proposes a compliant alternative.

A command does not grant blanket authority. `Approve` approves
the **current** `Awaiting Approval` plan, not a future or
hypothetical plan. `Continue` resumes the **next** `Ready`
task in the queue, not a task the user names in the same
message (use a brief for that).

---

## 3. Recognised Commands

### 3.1 `Continue`

The user types only `Continue` (or `continue`, or `Continue.`).

When `Continue` is recognised:

1. Read:
   - `.ai/state/current.md`
   - `.ai/state/task-board.md`
   - `.ai/state/session.json`
   - `.ai/handoffs/latest.md`
2. Reconcile those files with the actual repository and Git
   state (`git status`, `git log`, file inspection). The
   repository wins when the files disagree (per
   `.ai/session-start.md` step 6); the session records the
   reconciliation in the next handoff.
3. Find the first dependency-satisfied `Ready` task in
   `.ai/state/task-board.md` (or `.ai/state/tasks.json`).
4. Read its canonical plan under `.ai/plans/`.
5. Apply the decision table in § 4.

`Continue` is the standard "pick up the work" command. It does
not commit the session to any specific task; the task is
selected by the state, not by the user.

### 3.2 `Approve` / `Approved`

The user types `Approve` or `Approved` (or `approve`, with or
without trailing punctuation).

When `Approve` is recognised:

1. Identify the current `Awaiting Approval` plan (the most
   recent plan whose `Status` field is `Awaiting Approval`
   and whose task is the next dependency-satisfied `Ready`
   task in the queue).
2. Record approval in the canonical plan:
   - Change the plan's `Status` field from
     `Awaiting Approval` to `Approved`.
   - Add a line under the `Approval` block recording the
     approval date and the authorising command.
3. Mark the task `In Progress` in
   `.ai/state/task-board.md` and `.ai/state/tasks.json`.
4. Update `.ai/state/session.json` with the new session
   envelope.
5. Execute **only** the approved plan, following the 13-step
   task lifecycle in
   `.ai/workflows/progressive-coding.md` § 3.
6. Validate, produce the implementation report, update state,
   commit, push when authorised, prepare the next plan
   (expand the next slice's plan stub to `Awaiting Approval`),
   and stop.

`Approve` does **not** ask for approval again. The plan was
already presented to the user; `Approve` is the user's
confirmation. Re-asking is a defect (a session that re-asks
on a previously confirmed approval is treating the user as
untrustworthy).

The session does re-confirm approval only when:

- The plan changed materially since the user said `Approve`.
- A new destructive action is required that the plan did not
  authorise (a force-push, a schema migration, a deletion of
  committed history).
- An unresolved blocker changes the accepted scope.

### 3.3 `Status`

The user types `Status` (or `status`).

When `Status` is recognised, the AI shows **only** the
following fields, in the order listed, in the `Current /
Action / Result / Next` structure from § 5:

- Current milestone (from `.ai/state/current.md`).
- Current slice (from `.ai/state/current.md`).
- Active task and its status (from
  `.ai/state/task-board.md`).
- Active branch (from `git branch --show-current`).
- Last stable commit (from `git log -1 --format="%h %s"`).
- Build status (from the most recent `dotnet build` recorded
  in the latest handoff or implementation report).
- Test status (from the most recent `dotnet test` recorded
  in the latest handoff or implementation report; "X passed,
  Y failed, Z skipped").
- Blocked items (from `.ai/state/task-board.md`).
- Next recommended action (one line).

`Status` is read-only. The session does not modify any file,
does not run any validation command, and does not advance any
task. The session returns the snapshot and stops.

### 3.4 `Plan`

The user types `Plan` (or `plan`).

When `Plan` is recognised, the AI:

1. Loads the first `Ready` task in the queue.
2. Either:
   - Creates the missing plan from the
     `.ai/templates/implementation-plan.md` template, fills
     it in, and writes it to
     `.ai/plans/<milestone-or-task-name>.md` with
     `Status: Awaiting Approval`. **Stop.** Do not implement.
   - Summarises the existing plan (objective, included scope,
     excluded scope, main risks, acceptance criteria,
     expected commit) in the concise shape from § 5.3.

`Plan` does not implement. `Plan` does not validate. `Plan`
presents the plan and stops.

### 3.5 `Resume`

The user types `Resume` (or `resume`).

When `Resume` is recognised:

1. Find the task whose status is `In Progress` in
   `.ai/state/task-board.md` and `.ai/state/tasks.json`.
2. If no task is `In Progress`, treat `Resume` as `Continue`
   (the user likely meant "continue the work" without checking
   the state first).
3. If exactly one task is `In Progress`, read
   `.ai/handoffs/latest.md` and the matching implementation
   report (if any). Pick up at the step named in the handoff's
   "Exact Next Step" section.
4. Execute the remaining lifecycle steps; do **not** select a
   new task.

`Resume` does not start a new task. A session that finds a
task `In Progress` and starts a different task is a session
that ignored its own state.

### 3.6 `Review`

The user types `Review` (or `review`).

When `Review` is recognised:

1. Identify the current task (the first dependency-satisfied
   `Ready` task, or the `In Progress` task if one exists).
2. Read its approved plan, the implementation report (if any),
   and the current diff (`git diff` against the parent commit,
   or `git diff main...HEAD` if the branch is published).
3. Review the diff against:
   - The approved plan.
   - The 17 rules in `AGENTS.md`.
   - The architecture rules in `ARCHITECTURE.md`.
   - The design-system rules in `docs/design-system.md` and
     `STYLEGUIDE.md`.
   - The test coverage required by the plan.
   - The documentation requirements in
     `.ai/workflows/documentation-update.md`.
4. Produce findings using
   `.ai/templates/review-report.md` (severity-labelled,
   ranked most-severe first). **Produce findings only.**
   Do not apply fixes unless the user explicitly says so
   in the same message ("Review and fix", "Review and apply",
   "Fix the review findings").

### 3.7 `Validate`

The user types `Validate` (or `validate`).

When `Validate` is recognised:

1. Identify the current task (the `In Progress` task, or the
   first `Ready` task with an `Approved` plan).
2. Run the validation commands required by the plan
   (`npm run css:build`, `dotnet restore`, `dotnet build`,
   `dotnet test`, `dotnet format --verify-no-changes`, plus
   any visual smoke tests listed in the plan).
3. Update the validation fields in
   `.ai/state/current.md` (build status, test status, last
   validation date) and in the handoff.
4. Do **not** commit. `Validate` is read-only against the
   working tree except for the state file's validation
   fields. The user reviews the results and then says
   `Finish` (or `Approve` for the next task) to commit.

### 3.8 `Finish`

The user types `Finish` (or `finish`).

When `Finish` is recognised:

1. Identify the task whose status is `In Progress`.
2. If no task is `In Progress`, treat `Finish` as a no-op and
   surface that the user has nothing to finish.
3. If exactly one task is `In Progress`, complete the
   closeout for that task:
   - Run the validation (per `.ai/workflows/feature-lifecycle.md`
     stage 7).
   - Produce the implementation report from
     `.ai/templates/implementation-report.md`.
   - Update the project-continuity state per Rule 15 in
     `AGENTS.md` (`.ai/state/current.md`,
     `.ai/state/task-board.md`, `.ai/state/session.json`,
     `.ai/state/tasks.json`, `.ai/state/milestones.json`).
   - Write the handoff to
     `.ai/handoffs/YYYY-MM-DD-<slug>.md` and mirror to
     `.ai/handoffs/latest.md`.
   - Create the coherent commit per Rule 17 in `AGENTS.md`.
   - Push when authorised (separate decision; per
     `.ai/workflows/release-checklist.md`).
   - Promote the next dependency-satisfied `Ready` task to
     `In Progress` is **not** part of `Finish`. `Finish` ends
     at the closeout commit. The next session (or the next
     command) promotes the next task.
4. Stop. Do not implement the next task.

`Finish` is the closeout command. It does not start the next
task; the next session, or the next user command, does.

---

## 4. The `Continue` Decision Table

When `Continue` (or the fallback path of `Resume`) selects a
task and reads its plan, the session applies this table. The
table is the only place the plan-status enum is interpreted
for command-driven development.

| Plan status        | Behaviour                                                                                                                                                                                                                                                                                                                                                              |
| ------------------ | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `Draft`            | Summarise the missing planning work in the concise shape from § 5.3; complete the plan (fill the sections from `.ai/templates/implementation-plan.md`); set its `Status` to `Awaiting Approval`; commit the plan; stop. Do **not** implement. The user reviews the plan and says `Approve` (or amends and re-submits).                                                |
| `Awaiting Approval` | Present a concise plan summary using the shape from § 5.3 (objective, included scope, excluded scope, main risks, acceptance criteria, expected commit). Do not implement. The user reviews and says `Approve` (or amends).                                                                                                                                          |
| `Approved`         | Mark the task `In Progress` (`.ai/state/task-board.md` and `.ai/state/tasks.json`); update `.ai/state/session.json`; execute the plan through the 13-step task lifecycle in `.ai/workflows/progressive-coding.md` § 3 (validate, report, state, handoff, commit, push when authorised). Stop after the coherent commit. **The session does not re-ask for approval.** |
| `In Progress`      | Resume the task from the latest handoff. Do **not** select a new task. Apply the 13-step lifecycle to the remaining work.                                                                                                                                                                                                                                              |
| `Blocked`          | Explain the blocker in the shape from § 5.3 (objective: unblock; included scope: the unblock action; excluded scope: anything else). Surface the exact unblock action. Do not start another task automatically.                                                                                                                                                       |
| `Done`             | Promote the next dependency-satisfied `Ready` task and process it using this same table. (Promotion = update the state files; the actual `In Progress` transition is the next row's "Approved" / `Awaiting Approval` handling.)                                                                                                                                       |

A `Continue` that lands on a `Draft` plan does **not** count as
"starting implementation"; it counts as "finishing the plan
so the next command can approve it." A `Continue` that lands
on an `Awaiting Approval` plan does **not** count as
"starting implementation"; it counts as "presenting the
plan." A `Continue` that lands on an `Approved` plan starts
implementation. The distinction is the only thing that keeps
the approval gate meaningful.

---

## 5. Response Style

### 5.1 The Four-Section Shape

For every command response (and for command-driven sessions
generally), the AI uses this structure:

```
### Current
[the relevant state snapshot]

### Action
[what the AI did, is doing, or will do as a result of the command]

### Result
[the artefact produced, the test result, the state change, or "no file changes" for read-only commands]

### Next
[the single most concrete next step the user (or the next command) should take]
```

The shape is short. The longest permitted response to a
command is the `Awaiting Approval` summary in § 5.3. The AI
does not narrate the work between the four sections; the
state files and the implementation report carry the long
narrative.

### 5.2 Limits

- `Status` returns at most 12 lines (the bullet list in
  § 3.3). No prose outside the four sections.
- `Plan` returns at most the § 5.3 summary, plus the four
  sections, plus the canonical plan path.
- `Review` returns the review report (severity-ranked) and
  the four sections.
- `Validate` returns the command list, the exit code, and
  the four sections.
- `Finish` returns the closeout summary, the commit hash,
  and the four sections.
- `Approve` does not return a summary; it returns the
  one-line confirmation, the commit hash when the
  implementation is done, and the four sections.
- `Continue` returns the decision-table row it applied
  (one of: `Draft` → completed; `Awaiting Approval` → summary;
  `Approved` → implementing; `In Progress` → resumed;
  `Blocked` → surfaced; `Done` → next task promoted),
  plus the four sections.

### 5.3 The `Awaiting Approval` Summary Shape

When a `Continue` (or `Plan`) lands on an `Awaiting Approval`
plan, the summary uses this shape:

- **Objective** — one sentence.
- **Included scope** — bullets, one per file area.
- **Excluded scope** — bullets, one per deferred area.
- **Main risks** — bullets, one per risk; the mitigation
  in one short clause.
- **Acceptance criteria** — bullets, one per criterion.
- **Expected commit** — `type(scope): subject` in the
  Conventional Commits style the repository uses, plus the
  branch name.

The summary fits in one screen. The full plan is in
`.ai/plans/<name>.md`; the summary is a **link**, not a
restatement.

### 5.4 No Long Narrative

A command response is **not** an implementation report. The
report lives in `implementation-report-<task>.md` and in
`.ai/handoffs/`. A command response that re-states the
implementation report in the chat is a session that
duplicated evidence. Surface the path; do not paste the
content.

---

## 6. The Short-Form Approval Cycle

The eight commands compose into a compact workflow. The
canonical example:

```
User:  Continue
AI:    [reads state, finds M2.2, plan is Awaiting Approval]
       [returns the § 5.3 summary]
       [Next: "Reply 'Approve' to begin implementation."]
User:  Approve
AI:    [records approval, marks M2.2 In Progress]
       [executes the 13-step lifecycle]
       [validates, reports, commits, stops]
       [Next: "Run 'Continue' to start M2.3."]
User:  Continue
AI:    [finds M2.3, plan is Draft or Awaiting Approval]
       [...]
```

The cycle is short because each command is small. The
discipline is in the **gates**: a `Continue` that lands on
`Awaiting Approval` does not implement; an `Approve` does not
re-ask; a `Finish` does not start the next task.

A session that drifts (continues without approval, asks for
approval twice, implements two tasks in one commit, starts a
new task while one is `In Progress`) is a session that has
ignored this protocol. The protocol is the answer to "what
should the AI do next?" in command-driven mode.

---

## 7. What This Document Does Not Replace

This document operationalises the task-selection and
closeout steps of the existing operational sequence. It does
**not** replace:

- The 13-step task lifecycle in
  `.ai/workflows/progressive-coding.md` § 3. The lifecycle
  is still the per-task execution contract; the commands
  are the **front door** to the lifecycle.
- The mandatory reading order in `AGENTS.md` § 2. A command
  does not skip `AGENTS.md`; a session that begins with
  `Continue` still reads `AGENTS.md` and
  `.ai/session-start.md` first.
- The 17 rules in `AGENTS.md` § 4. A command that asks for
  a rule violation is not honoured.
- The prompt taxonomy in `.ai/prompts/`. A task that
  requires a specific prompt (a `bugfix`, a `provider`
  onboarding, a `release`) still uses the matching prompt.
  Commands are **task-agnostic**; prompts are task-specific.
- The implementation report and the session handoff. A
  command response is **not** the report; the report is a
  file. A session that ends with only a command response
  and no report has not ended per Rule 17 in `AGENTS.md`.

---

## 8. Adding or Changing a Command

A change to this document is a change to the operating layer
(tier 8 in `.ai/README.md` § 10.1). The change:

- Is reviewed against the constitution (`AGENTS.md`).
- Adds the new command's decision row to § 4 (if the command
  changes the `Continue` decision table).
- Adds the new command to § 3 with the same shape as the
  existing commands (recognition rule, behaviour, no-go list,
  the § 5 response shape).
- Updates `.ai/session-start.md` if the command changes the
  mandatory sequence.
- Updates `AGENTS.md` if the command changes a rule (with an
  ADR per Rule 8 in `AGENTS.md`).
- Updates `.ai/README.md` § 4 task routing table if the
  command introduces a new task type.

A new command that conflicts with an existing command, with
the 13-step lifecycle, or with a rule in `AGENTS.md` is
rejected. The constitution is not relaxed by this document.

---

## 9. Linked Artefacts

- [`AGENTS.md`](../AGENTS.md) — the constitution; the 17
  rules; the precedence hierarchy in § 2.2.
- [`.ai/session-start.md`](./session-start.md) — the
  operational sequence; the mandatory reading order.
- [`.ai/workflows/progressive-coding.md`](./workflows/progressive-coding.md)
  — the 13-step task lifecycle; the "one task per session"
  rule; the grouped-execution authorisation.
- [`.ai/plans/README.md`](./plans/README.md) — the
  plan-status enum (`Draft`, `Awaiting Approval`,
  `Approved`, `Superseded`).
- [`.ai/templates/implementation-plan.md`](./templates/implementation-plan.md)
  — the template a `Draft` plan is expanded from.
- [`.ai/templates/implementation-report.md`](./templates/implementation-report.md)
  — the receipt of every implementation.
- [`.ai/templates/session-handoff.md`](./templates/session-handoff.md)
  — the bridge between sessions.
- [`.ai/templates/review-report.md`](./templates/review-report.md)
  — the structured output of a `Review` command.
- [`.ai/state/README.md`](./state/README.md) — the
  two-layer state model; the JSON / Markdown projection.
- [`.ai/handoffs/README.md`](./handoffs/README.md) — the
  handoff format; the `latest.md` mirror.

---

## 10. Last Updated

- **2026-07-11** — created in the command-protocol
  instructions update. The document defines the eight
  recognised commands (`Continue`, `Approve`, `Status`,
  `Plan`, `Resume`, `Review`, `Validate`, `Finish`), the
  command precedence in § 2, the `Continue` decision table
  in § 4, the four-section response shape in § 5, the
  short-form approval cycle in § 6, and the relationship
  to the existing operational sequence in § 7. The
  document is the front door to the 13-step lifecycle
  for command-driven sessions.
