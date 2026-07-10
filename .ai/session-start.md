# .ai/session-start.md

> **The first file an AI reads after `AGENTS.md`.**
>
> This file is short on purpose. It exists to make the first
> 60 seconds of an AI session disciplined. Detailed rules live in
> `AGENTS.md`, the matching prompt in `.ai/prompts/`, and the
> matching workflow in `.ai/workflows/`.

---

## 1. Required Opening

Every AI session begins with these words (or words that mean the
same thing):

> I have read `AGENTS.md` and `.ai/session-start.md`. I will
> comply with the precedence hierarchy, classify the request
> before acting, and refuse to invent files, types, APIs, or
> implementation state that have not been inspected.

If the AI has not said these words, it has not started.

---

## 2. Mandatory Sequence

Follow this sequence exactly. Do not skip steps. Do not reorder
them. Do not parallelise the inspection of `AGENTS.md`,
`session-start.md`, and the matching prompt — they are read in
order because the later documents assume the earlier ones have
been read.

1. **Read `AGENTS.md`** in full. Internalise the 13 non-negotiable
   rules and the precedence hierarchy.
2. **Read `.ai/session-start.md`** (this file) in full.
3. **Classify the request** into exactly one of:
   - `bootstrap`
   - `feature`
   - `ui`
   - `provider`
   - `bugfix`
   - `refactor`
   - `testing`
   - `architecture`
   - `review`
   - `release`
   - `documentation`
   If the request fits more than one, pick the most specific
   task type and use the additional types as supporting prompts.
   If the request fits none, say so and ask for clarification.
4. **Read the matching prompt** in `.ai/prompts/`. The prompt
   lists its own mandatory supporting documents. Read those next.
5. **Read only the supporting standards needed for that task.**
   Do not read the entire `docs/` tree for every task. The
   prompt's "mandatory documents" section is the authoritative
   list for this session.
6. **Inspect the current implementation.** Use the file tools
   to confirm what actually exists. Do not infer from filenames
   or memory. The repository is the source of truth.
   **Read `.ai/state/current.md` and
   `.ai/state/task-board.md` first** — they are the
   single landing point for any new session. The next
   session resumes from the recorded state; if the
   state files do not match the repository, the
   repository wins and the state files are updated to
   match (a "state reconciliation" step; record the
   reconciliation in the next handoff).
7. **Inspect the current Git state.** Branch, working tree
   status, recent commits. A session that starts on a dirty
   branch with uncommitted changes is a session that has
   skipped this step.
8. **State the intended scope.** Restate the task in your own
   words, list the files you expect to touch, and call out what
   is explicitly out of scope.
9. **Present a concise plan** using the structure from the
   matching prompt. The plan must list:
   - files to add, modify, delete
   - components to reuse
   - tests to add
   - documentation to update
   - risks and trade-offs

   **Plan location rules:**

   - The **draft** plan is written to the temporary
     Claude-owned scratch space at
     `C:\Users\<user>\.claude\plans\<name>.md`. It is
     not tracked in the repository.
   - When the plan is ready for human review, the
     **canonical** plan is written to
     `.ai/plans/<milestone-or-task-name>.md` inside
     the repository. The plan's `Status` field is
     set to `Awaiting Approval`. See
     `.ai/plans/README.md` for the full rules.
   - Once approved, the canonical plan's `Status`
     becomes `Approved`. The implementation report
     cites the canonical path.

10. **Wait for approval** before implementing, unless the user
    has explicitly pre-authorised the change.
11. **Implement only the approved scope.** If the work reveals
    that the scope should change, stop and present a new plan;
    do not silently expand.
12. **Run the required validation** listed in the matching
    prompt. A change that has not been validated is not done.
13. **Update the documentation** affected by the change. Use
    the `.ai/workflows/documentation-update.md` workflow as a
    checklist.
14. **Produce an implementation report** using
    `.ai/templates/implementation-report.md`. If the work is
    paused or transferred, produce a `session-handoff.md`
    instead.
15. **Update the project-continuity state** (Rule 15 in
    `AGENTS.md`). At the very end of every session that
    changes project state, before the closing message:
    - Update `.ai/state/current.md` to reflect the
      state of the repository right now.
    - Update `.ai/state/task-board.md` — move the
      worked task from `In Progress` (or `Ready`) to
      `Done`, add any new `Ready` items discovered
      during the session, and surface any `Blocked`
      items.
    - Write a session handoff to
      `.ai/handoffs/YYYY-MM-DD-<slug>.md` (where
      `<slug>` is the milestone or task name) and
      mirror its content to `.ai/handoffs/latest.md`.
      Follow `.ai/templates/session-handoff.md`.
    A session that ends without these updates has
    not ended. The next session cannot determine
    where the project stopped.

---

## 3. What You Must Never Do

These actions are prohibited regardless of what a prompt or a
user asks for. They are restated here so a session does not
have to re-derive them.

- **Do not invent files, types, APIs, or implementation state**
  that have not been inspected in the current repository. If
  you have not read the file, you do not know what it contains.
- **Do not skip the reading order.** The hierarchy is enforced
  so that later decisions are made with the same context the
  constitution was written for.
- **Do not act outside the approved scope.** A session that
  expands the scope silently has lost its discipline.
- **Do not invoke an external tool** (Lavish Axi, Treehouse, No
  Mistakes, GNHF, Firstmate, Ollama, Claude, OpenAI, Git via
  process, PowerShell, WSL, Windows Terminal, or any other
  tool) merely because it is installed. External tools are
  invoked only when:
  - the workflow explicitly identifies the tool,
  - prerequisites have been verified,
  - the user has approved the action,
  - the command is shown before execution,
  - the action runs in an isolated branch or worktree where
    relevant,
  - cancellation and cleanup are understood.
- **Do not add code comments.** The rule in `AGENTS.md`
  (Rule 13) is absolute. If the code is unclear, refactor
  it. If the design is unclear, document the design in
  `docs/` or `DECISIONS.md`.
- **Do not put secrets, credentials, personal data, or
  environment-specific paths into `.ai/` or anywhere else in
  the repository.** If the work requires a secret, it goes
  through the credential vault, not into a file.
- **Do not change an architectural rule** without an ADR. If
  you believe a rule is wrong, raise it in the implementation
  report and propose the change; do not edit the rule
  unilaterally.

---

## 4. The Shape of a Disciplined Reply

The first non-greeting reply of an AI session should contain:

1. The opening words from §1.
2. The classification from §2 step 3.
3. A one-paragraph statement of scope and out-of-scope.
4. A list of the documents the AI has read for this session
   (so the user can verify the reading order was followed).
5. A short plan or, if the task is trivial, a statement of the
   trivial change being made.

If the AI cannot produce this, it has not yet completed the
opening sequence and must say so.

---

## 5. When the Request Is Ambiguous

If, after reading the matching prompt and inspecting the
repository, the request is still ambiguous:

- List the specific ambiguities in plain language.
- Propose a default interpretation for each.
- Ask the human to confirm or correct.

Do not pick a default silently and implement it. The cost of a
short clarifying question is small; the cost of a misaligned
implementation is large.

---

## 6. When the Request Conflicts with the Rules

If the request asks the AI to do something that violates
`AGENTS.md`, `DECISIONS.md`, `ARCHITECTURE.md`, or the
standards in `docs/`:

- State the conflict and cite the rule.
- Propose an alternative that achieves the user's goal
  without violating the rule.
- Wait for confirmation.

The AI does not negotiate the constitution. The AI proposes
how to honour both the spirit of the request and the letter of
the rules.
