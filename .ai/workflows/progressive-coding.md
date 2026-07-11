# Progressive Coding Rule

> **The operating rule that governs which task the AI
> picks next, and how a single task moves from
> `Ready` to `Done`.**
>
> This workflow is the answer to "what should the AI
> do next?" in a session where multiple tasks are
> `Ready`. The rule prevents the AI from
> automatically beginning the next task in the same
> session (unless the user explicitly authorises
> grouped execution) and prevents the AI from
> picking a task whose dependencies are not
> complete.
>
> The rule sits above the per-feature lifecycle in
> [`.ai/workflows/feature-lifecycle.md`](./feature-lifecycle.md);
> it governs **task selection**, not
> **per-task execution**.

---

## 1. Purpose

The Progressive Coding Rule is the AI's
contract with the user about what work
gets done in a session. The rule
guarantees that:

- The AI selects only the **first
  `Ready` task** whose dependencies
  are complete and whose plan is
  `Approved`.
- The AI does **not** automatically
  begin the next task in the same
  session unless the user
  explicitly authorises grouped
  execution.
- The AI follows the **13-step task
  lifecycle** for every task it
  selects; no step is optional; the
  task is **not** `Done` until
  every step is complete.
- The user can **stop the AI at any
  step** by giving the AI feedback
  (a comment, a correction, a
  redirection); the AI stops the
  current step and waits.
- The state files (`.ai/state/*.json`
  + `.ai/state/*.md`) are the
  source of truth for "what is
  `Ready`, what is `In Progress`,
  what is `Blocked`, what is
  `Done`".

The rule is the operational
expression of `AGENTS.md` Rule 16
(Scope Discipline): the AI does the
task the user authorised, and only
the task the user authorised. The
rule prevents the AI from
"while we're here" drift.

## 2. Task Selection

The AI selects a task in this
order:

1. **Read `.ai/state/tasks.json`**
   (or `.ai/state/task-board.md`
   for the human-readable
   projection). Identify the
   first `Ready` task whose
   `depends_on` set is satisfied
   and whose `plan` is `Approved`.
2. **If the matching plan is
   `Draft` or `Awaiting Approval`:
   the AI does not implement
   the task.** The AI either
   promotes the plan (by
   reviewing and approving it,
   per the brief) or surfaces
   the gap to the user.
3. **If the user has explicitly
   authorised grouped execution
   (e.g. "do M2.2 and M2.3 in
   this session"), the AI may
   pick the next `Ready` task
   after the current one is
   `Done`. The grouped-
   execution authorisation is
   recorded in the
   session's
   `.ai/state/session.json`
   `scope.out_of_scope` field
   (the inverse: it names what
   is in scope, not what is
   out of scope) and in the
   handoff.
4. **If the user has not
   authorised grouped
   execution, the AI works
   exactly one task per
   session.** When the task
   is `Done`, the AI writes
   the handoff and the state
   updates, produces the
   coherent commit, and
   stops. The next session
   picks the next `Ready`
   task.

## 3. The 13-Step Task Lifecycle

Every task the AI selects moves
through these 13 steps. The
lifecycle is fully gated; a
step that is skipped is a
defect.

1. **Read the brief and the
   approved plan.** The AI
   reads the task's approved
   plan (or plan stub) and
   the brief the user gave
   for this session. The
   plan is the contract; the
   brief is the
   authorising input.
2. **Read the project
   continuity state.** The
   AI reads
   `.ai/state/current.md`,
   `.ai/state/task-board.md`,
   `.ai/state/session.json`,
   and `.ai/handoffs/latest.md`
   to know where the
   project is right now.
3. **Inspect the actual
   repository and Git
   state.** The AI runs
   `git status`, `git log`,
   and the file-listing
   tools to verify the
   state matches the
   project-continuity
   state. The repository
   wins when the two
   disagree (per
   `.ai/session-start.md`
   step 6).
4. **Classify the
   request.** The AI
   classifies the request
   as `documentation` /
   `architecture` /
   `feature` / `bugfix` /
   `refactor` / `review`
   (per
   `.ai/session-start.md`
   § 1).
5. **Restate the task and
   the scope.** The AI
   states the task in
   its own words, lists
   the in-scope and
   out-of-scope items,
   and lists the
   documents it will
   read. The statement
   is in the first
   reply of the
   session.
6. **Implement the diff.**
   The AI writes the
   code, the
   configuration, the
   documentation, and
   the tests per the
   approved plan. The
   implementation
   follows the per-
   feature lifecycle in
   [`.ai/workflows/feature-lifecycle.md`](./feature-lifecycle.md).
7. **Run the validation
   commands.** The AI
   runs the commands the
   plan specifies
   (`dotnet build`,
   `dotnet test`,
   `dotnet format`,
   `npm run css:build`,
   `dotnet run`, etc.).
   Every command's
   exit code is
   recorded. A
   non-zero exit code
   is a defect; the AI
   fixes the defect
   before moving on.
8. **Update the
   design-system
   catalogue (if
   applicable).** If
   the task added
   components to the
   catalogue, the AI
   updates
   `docs/design-system.md`
   per ADR-015. If the
   task did not add
   components, the AI
   does not modify the
   catalogue.
9. **Produce the
   implementation
   report.** The AI
   writes the report
   at
   `implementation-report-<task-id>.md`
   from
   `.ai/templates/implementation-report.md`.
   The report names
   the files, the
   tests, the
   validation results,
   the deviations, the
   known limitations,
   and the next
   recommended step.
10. **Update the
    project-continuity
    state (Rule 15
    in `AGENTS.md`).**
    The AI updates
    `.ai/state/current.md`,
    `.ai/state/task-board.md`,
    `.ai/state/milestones.json`,
    `.ai/state/tasks.json`,
    `.ai/state/session.json`,
    and
    `.ai/handoffs/latest.md`
    to reflect the
    task's
    completion. The
    JSON files are
    canonical; the
    Markdown is the
    projection; the
    two are kept in
    sync.
11. **Write the
    session handoff.**
    The AI writes the
    handoff at
    `.ai/handoffs/YYYY-MM-DD-<task-id>.md`
    from
    `.ai/templates/session-handoff.md`
    and mirrors it to
    `.ai/handoffs/latest.md`.
    The handoff names
    the task, the
    outcome, the
    next recommended
    step, and the
    documents the
    next session
    must read.
12. **Produce the
    coherent commit
    (Rule 17 in
    `AGENTS.md`).**
    The AI creates
    one commit that
    includes the
    implementation,
    the
    documentation,
    the
    implementation
    report, the
    state updates,
    and the
    handoff. The
    commit is local;
    pushing requires
    explicit
    authorisation
    and is a
    separate step.
13. **Stop.** The
    AI stops. If
    the user has
    not
    authorised
    grouped
    execution,
    the AI does
    **not** begin
    the next
    `Ready` task
    in the same
    session. The
    AI confirms
    the commit
    and the
    handoff, and
    waits for the
    next user
    input.

## 4. Task Selection Anti-Patterns

- **Picking a `Deferred` task
  without the user's
  authorisation.** A
  `Deferred` task is in the
  backlog but is not
  committed. The AI surfaces
  the deferral and waits.
- **Picking a `Blocked` task
  without the user's
  authorisation.** A
  `Blocked` task has a named
  blocker; the AI does not
  start the task until the
  blocker is resolved. The AI
  surfaces the blocker and
  waits.
- **Picking a `Ready` task
  whose `depends_on` set is
  not satisfied.** The AI
  verifies the dependencies
  before picking; an
  unsatisfied dependency is
  a defect in the state
  files (the AI either
  fixes the defect or
  surfaces it to the user).
- **Automatically beginning
  the next `Ready` task
  after the current one is
  `Done` without the user's
  authorisation.** The AI
  stops at step 13 and
  waits. The next session
  picks the next task.
- **Bundling two `Ready`
  tasks into a single
  commit.** A commit is
  one task's deliverable.
  Two tasks = two commits.
- **Implementing before
  the plan is `Approved`.**
  The AI surfaces the
  `Awaiting Approval` plan
  and waits.
- **Skipping the validation
  commands in step 7.** A
  task that is not validated
  is not `Done`. The AI
  runs the commands and
  records the exit codes.
- **Skipping the
  project-continuity state
  updates in step 10.** A
  task that is not reflected
  in the state files is
  not `Done`; the next
  session cannot pick up
  where the AI left off.

## 5. Grouped Execution

The user may authorise **grouped
execution** of multiple tasks
in a single session. The
authorisation:

- Is **explicit** (the user
  names the tasks or the
  range of tasks, e.g. "do
  M2.2 through M2.4 in
  this session").
- Is **recorded** in the
  session's
  `.ai/state/session.json`
  `scope` field and in
  the handoff.
- **Does not waive** the
  13-step lifecycle. Each
  task in the group moves
  through the 13 steps; the
  only difference is the AI
  does not stop at step 13
  for the first task in
  the group.
- **Is per-session.** The
  next session is a fresh
  decision; the
  authorisation does not
  carry forward.
- **Is revocable.** The
  user may stop the group
  at any time; the AI
  finishes the current
  step, writes the
  handoff, and stops.

## 6. Task Selection and the
   Architecture Substrate

Task selection is informed by
the M0.5 architecture
substrate:

- **Capabilities** (in
  `.ai/state/capabilities.json`
  + `.ai/backlog/capabilities.md`)
  are the unit of dogfooding.
  A task that consumes a
  capability names the
  capability, the contract,
  and the registry or
  service the task resolves
  the capability through.
- **Milestones** (in
  `.ai/state/milestones.json`)
  are the unit of delivery.
  A task is a slice of a
  milestone; the slice's
  `delivered_by_milestone`
  is the milestone the task
  advances.
- **Backlog** (in
  `.ai/backlog/`) is the
  source of work the team
  proposes. A task that is
  not in the backlog is a
  speculative task; the AI
  does not start a
  speculative task without
  the user's authorisation.
- **Decision log** (in
  `.ai/state/decision-log.md`)
  is the pre-ADR record. A
  task that makes a
  non-architectural decision
  records the decision in
  the log; a task that makes
  an architectural decision
  writes an ADR.

## 7. Relationship to AGENTS.md
   and `.ai/session-start.md`

The Progressive Coding Rule is
**subordinate to** the rules in
`AGENTS.md` and the operational
sequence in `.ai/session-start.md`.
Specifically:

- **Rule 16 (Scope Discipline).**
  The Progressive Coding Rule's
  task-selection step (step 2)
  is the operational expression
  of Rule 16. The AI does the
  task the user authorised, and
  only the task the user
  authorised.
- **Rule 17 (Evidence of
  Completion).** The 13-step
  lifecycle's steps 9–12 are
  the operational expression
  of Rule 17. The AI does not
  declare a task `Done` until
  the implementation report,
  the state updates, the
  handoff, and the coherent
  commit are in place.
- **Rule 15 (Project Continuity
  State).** Step 10 is the
  operational expression of
  Rule 15. The AI updates the
  state files in the same
  commit that closes the
  task.
- **`.ai/session-start.md` step
  1 (Required Opening).** The
  AI's first reply includes
  the required opening
  statement from
  `.ai/session-start.md` § 1.
  The opening is the
  operational expression of
  the session-start
  contract.

The Progressive Coding Rule
does not relax any of these
rules; it operationalises them
on a per-task basis.

## 7.1 Relationship to the Command Protocol

The command protocol in
[`.ai/commands.md`](../commands.md)
defines the nine recognised
short-form commands
(`Continue`, `Approve`,
`Status`, `Plan`, `Resume`,
`Review`, `Validate`,
`Finish`, `Next`). The commands are
the **front door** to the
Progressive Coding Rule, not
a replacement for it.

Specifically:

- **`Continue`** selects
  the first dependency-
  satisfied `Ready` task
  (per § 2 above) and then
  applies the per-task
  plan-status decision
  table in
  `.ai/commands.md` § 4.
  The decision table is
  the bridge between the
  command and the 13-step
  lifecycle.
- **`Approve`** records
  approval and starts
  the 13-step lifecycle
  at step 6 (Implement).
  It does not re-ask for
  approval; the
  confirmation was the
  command.
- **`Resume`** finds the
  `In Progress` task
  (per § 2 above) and
  resumes the 13-step
  lifecycle at the
  step named in
  `.ai/handoffs/latest.md`.
- **`Validate`** runs the
  validation commands
  the plan specifies
  (per step 7 above) and
  updates the validation
  fields in
  `.ai/state/current.md`
  and the handoff. It
  does not commit.
- **`Finish`** completes
  steps 8 through 12 of
  the lifecycle
  (catalogue, report,
  state, handoff, commit)
  and stops. It does not
  begin the next task;
  the next command
  does.
- **`Next`** is the
  **end-to-end command**.
  It performs the work of
  `Continue` (select the
  task), `Approve`
  (promote the plan under
  the user's standing
  approval), and the full
  13-step lifecycle
  (validate, report,
  state, handoff, commit,
  merge, delete the
  feature branch) in one
  invocation. `Next` is
  the answer to "I have
  already authorised the
  work; do it." `Next`
  reconciles state first,
  resumes an `InProgress`
  task if one exists, and
  otherwise selects the
  first dependency-
  satisfied `Ready` task.
  `Next` may not
  implement two tasks in
  one invocation; it is
  the **collapsed** form,
  not a grouped-
  execution authorisation.
  Grouped execution is a
  separate user
  authorisation; `Next`
  does not subsume it.
- **`Status`**, **`Plan`**,
  and **`Review`** do
  not change the
  lifecycle; they are
  read-only commands
  against the state, the
  plan, or the diff.

A session that begins with
a command is still bound
by all 13 steps. The
command selects the task
and the response shape;
the lifecycle governs the
per-task execution.

## 8. When the Rule Does Not
   Apply

The Progressive Coding Rule
applies to **task selection**
in sessions that change
project state. The rule does
**not** apply to:

- **Read-only sessions.** A
  session that only reads
  (e.g. a code review, a
  state inspection) does not
  select a task; the session
  returns a review or
  inspection report and
  stops.
- **One-off Q&A.** A
  session that answers a
  question (e.g. "what is
  the build status?")
  does not select a task.
- **Emergency hotfixes.** A
  session that fixes a
  blocking defect may
  pick a `Blocked` task
  whose blocker is the
  defect being fixed; the
  AI documents the
  exception in the
  handoff.
- **User-driven
  deviations.** The user
  may explicitly authorise
  a deviation (e.g. "fix
  this and start the next
  Ready task without
  waiting"). The AI
  records the deviation
  in the handoff.

## 9. Linked Artefacts

- [`AGENTS.md`](./../../AGENTS.md)
  — the constitution;
  Rules 15, 16, 17.
- [`.ai/session-start.md`](./../session-start.md)
  — the operational
  sequence.
- [`.ai/workflows/feature-lifecycle.md`](./feature-lifecycle.md)
  — the per-feature
  lifecycle (Steps 6–8 of
  the 13-step task
  lifecycle).
- [`.ai/workflows/tool-dogfooding.md`](./tool-dogfooding.md)
  — the dogfooding
  workflow.
- [`.ai/state/tasks.json`](./../state/tasks.json)
  — the canonical task
  list.
- [`.ai/state/task-board.md`](./../state/task-board.md)
  — the human-readable
  task board.
- [`.ai/state/session.json`](./../state/session.json)
  — the session
  self-awareness state.
- [`.ai/state/capabilities.json`](./../state/capabilities.json)
  — the canonical
  capability graph.
- [`.ai/backlog/`](./../backlog/)
  — the engineering
  backlog.

## 10. Last Updated

- **2026-07-10** — created
  in the M2
  delivery-reconciliation
  session. The rule is
  the operational
  expression of
  `AGENTS.md` Rules 15,
  16, 17 and the
  per-feature lifecycle
  in
  `.ai/workflows/feature-lifecycle.md`.
  The rule is the
  meta-contract for task
  selection.
- **2026-07-11** — added
  § 7.1 linking the
  Progressive Coding Rule
  to the command protocol
  in
  [`.ai/commands.md`](../commands.md).
  The 13-step lifecycle
  is the per-task
  execution contract;
  the commands are the
  front door. The
  command-driven
  decision table in
  `.ai/commands.md` § 4
  is the bridge between
  the short-form command
  and the lifecycle. The
  Progressive Coding Rule
  is unchanged; the
  link is the only
  addition.
- **2026-07-11** — added
  the `Next` end-to-end
  command to the § 7.1
  command list. `Next`
  is the collapsed form
  of `Continue` +
  `Approve` + the 13-step
  lifecycle; it
  reconciles state,
  selects (or resumes)
  the task, promotes the
  plan under the
  user's standing
  approval, creates the
  feature branch,
  implements, validates,
  reports, commits,
  merges, deletes the
  feature branch, and
  stops. `Next` does not
  subsume grouped
  execution; it executes
  exactly one task per
  invocation. The
  Progressive Coding
  Rule is unchanged; the
  `Next` row is the only
  addition.
