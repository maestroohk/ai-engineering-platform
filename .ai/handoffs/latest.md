# Handoff — 2026-07-19 — tool-first-gnhf-real-verification

> **Session:** `tool-first-gnhf-real-verification`
> **Task:** T-032 (PartiallyVerified, 2026-07-19)
> **Milestone:** M4-D.1 (the first M4-D slice; the
> M4-D umbrella plan is not yet drafted; T-032 lands
> ahead of the plan so the platform can begin
> dogfooding immediately).
> **Supersedes:**
> `.ai/handoffs/2026-07-19-tool-first-recovery-and-gnhf-vertical-slice.md`
> (the overclaim that the stand-in test was a "real
> smoke test" is retracted; the stand-in is honestly
> named a deterministic local integration test in this
> handoff).
> **Implementation report:**
> `implementation-report-tool-first-recovery-and-gnhf-vertical-slice.md`
> (the § 7.5 wording is corrected; the report is the
> authoritative evidence).

## 1. Correction

The previous handoff claimed the gnhf provider was
verified end-to-end with a "real smoke test". That
claim is retracted. The earlier exercise used a
stand-in executable that printed expected output,
which proves only that the parser works against
mocked output — it does NOT prove that the platform
invokes the real upstream gnhf executable. Per the
user's directive:

> A provider is not "working" merely because its
> parser works against mocked output. It is working
> only after the platform invokes the real upstream
> executable and consumes its actual result.

T-032 is therefore **PartiallyVerified** (not
**Done**) until the actual upstream gnhf is invoked
end-to-end and a bounded real workflow succeeds.

## 2. Three verification levels

Recorded 2026-07-19 as the standing policy for every
tool-first task:

- **Implemented** — contracts, wrapper code, and
  unit / deterministic tests exist; the platform
  can compose the provider; nothing has been
  executed against the real upstream yet.
- **Executable verified** — the actual installed
  upstream executable (resolved by PATH, npm-
  global, pnpm-global, or configured absolute
  path) responds successfully to `--version`
  and/or `--help`; the truthful provider data
  (`actual_executable_verified`,
  `executable_path`, `health_check_state`,
  `health_check_timestamp`,
  `health_check_duration_ms`, `version`) is
  populated by the real process invocation.
- **Workflow verified** — a bounded real
  operation succeeds end-to-end in an isolated
  environment (temp git repo; one iteration;
  strict timeout; no remote; no push; no
  credentials; no destructive objective).

A task may move from `Implemented` to
`PartiallyVerified` once Executable verified is
reached. A task may move from `PartiallyVerified`
to `Done` once Workflow verified is reached.

## 3. Discovery of the actual gnhf

- `which gnhf` → not found.
- `where.exe gnhf` → not found.
- `which gnhf.cmd` → not found.
- `where.exe gnhf.cmd` → not found.
- `npm root -g` → directory contains
  `@continuedev`, `@openai`, `npm`, `opencode-ai`,
  `pnpm`. No `gnhf`.
- `pnpm root -g` → bin directory is not in PATH.
- Locked upstream clone at
  `code-kunchenguid/gnhf`: HEAD = `fe202c4c`
  (matches the lock); working tree clean;
  `dist/` does not exist; `node_modules/` does
  not exist; `package.json` version = `0.1.42`.

**Conclusion:** the real gnhf executable is NOT
installed on this host. The locked upstream clone
is unmodified (per brief § 4).

## 4. Real verification

**Executable verified: Pending.** The optional
real-tool smoke test
(`GnhfRealToolSmokeTests.RealGnhf_probe_succeeds_when_executable_is_installed`)
is a `[SkippableFact]` that locates the actual
gnhf executable at test time and **SKIPs** (not
fails) when gnhf is not installed. Run evidence
on this host:

```
[xUnit.net 00:00:00.39]
  AiEng.Platform.Providers.Gnhf.Tests.GnhfRealToolSmokeTests
  .RealGnhf_probe_succeeds_when_executable_is_installed
  [SKIP]
  Skipped AiEng.Platform.Providers.Gnhf.Tests.GnhfRealToolSmokeTests
  .RealGnhf_probe_succeeds_when_executable_is_installed [1 ms]
```

Test summary: 16 passed, 1 skipped, 0 failed.

Once the user authorises the documented install
command (see § 5), the test will execute end-to-end
and assert `State == InstalledAndHealthy` with a
real version, real path, real timestamp.

## 5. Blocked install — user authorisation required

Per the user's binding "Do not silently install
anything" and "If installation is required, stop
and report the exact documented command before
changing the machine", this session does NOT
install gnhf. The exact documented install
command (per the gnhf README) is:

```bash
npm install -g gnhf
```

Alternative from source (also documented in the
gnhf README, requires `corepack enable` first):

```bash
corepack enable
git clone <gnhf-repo> && cd gnhf
pnpm install
pnpm run build
pnpm link --global
```

When the user authorises one of the above, the
real-tool smoke test will execute end-to-end, the
descriptor's `actual_executable_verified` field
will flip from `false` to `true`, and T-032 will
be promoted to **Executable verified**.

## 6. Bounded workflow

**Workflow verified: NotAttempted.** Will run only
after Executable verified is reached, in an isolated
temp git repo (one iteration; strict timeout; no
remote; no push; no credentials; no destructive
objective), per the brief § 5 and the three-level
verification policy. T-032 is intentionally held
at PartiallyVerified until Workflow verified is
reached.

## 7. Tests (3 tiers)

| Tier | Class | Count | Status |
| ---- | ----- | ----- | ------ |
| Unit (mocks) | `GnhfAutonomousLoopFamilyTests` | 4 | passed |
| Deterministic process integration | `GnhfProcessProbeRunnerTests` | 8 | passed |
| Unit (resolver) | `GnhfExecutableResolverTests` | 4 | passed |
| Optional real-tool smoke | `GnhfRealToolSmokeTests` | 1 | **SKIP** (gnhf not installed) |
| **Total** | | **17** | **16 passed, 1 skip, 0 failed** |

The 9 tests previously labelled as "smoke tests"
in the implementation report § 7.5 are honestly
re-classified as deterministic local integration
tests in this handoff: they exercise the provider
plumbing with scripted `IProcessRunner` doubles
that print expected output. They prove the parser
+ the state machine, not the real upstream
executable.

## 8. Git

- Branch `feature/T-032-real-gnhf-verification`
  is created from `main` at the T-032 commit
  (`e06dc79`).
- Commit subject:
  `fix(gnhf): verify actual upstream executable
  and correct integration evidence`.
- Fast-forward merge to `main` per the branching
  strategy rule 6; branch deleted per rule 7.
- No push.

## 9. Truthful provider data on `/providers`

The M4-C.2 `/providers` page renders the
`ProviderDescriptor.Metadata` dictionary through
the existing `AppProviderList` → `AppKeyValueList`
path. No UI change. The new truthful fields
surface naturally on the gnhf card:

- `actual_executable_verified` — `true` /
  `false`
- `bounded_workflow_verified` — `false` (until
  workflow verification succeeds)
- `executable_path` — resolved PATH / npm-global
  / pnpm-global / configured path / `(none)`
- `executable_resolution` — `path` /
  `npm-global` / `pnpm-global` / `configured` /
  `not-found`
- `health_check_state` — `InstalledAndHealthy` /
  `InstalledButUnhealthy` / `NotInstalled` /
  `TimedOut` / `Cancelled` / `VersionUnknown`
- `health_check_timestamp` — ISO-8601
- `health_check_duration_ms` — integer
- `execution_mode` — `NativeWindows` /
  `NativeLinux` / `NativeMacOs` / `Wsl` /
  `NotInstalled`
- `exit_code` — integer (when available)
- `failure_reason` — string (when applicable)
- `help` — first two lines of `--help` (when
  available)
- `entry_command` — `gnhf <objective>` (per
  README)
- `locked_commit` — `fe202c4c92de3bc82b6319ed13bb35023d88410a`

## 10. State files updated

- `.ai/state/tasks.json` — T-032 status
  `Done` → `PartiallyVerified`; T-033 status
  `Ready` → `Blocked`; T-032 evidence block
  extended with `verification_levels` and
  `blockers`; `updated_at` and
  `updated_by_session` updated.
- `.ai/state/task-board.md` — T-032 entry
  relabelled `PartiallyVerified`; T-033 entry
  relabelled `Blocked`; 3-verification-level
  policy added to the tool-first rule; the
  "validation" table replaced with the truthful
  3-tier test summary; the install command
  recorded as the blocker.
- `.ai/handoffs/latest.md` — this file
  supersedes the previous overclaim handoff.
- `.ai/handoffs/2026-07-19-tool-first-recovery-and-gnhf-vertical-slice.md`
  — updated to point readers at the corrected
  handoff.
- `implementation-report-tool-first-recovery-and-gnhf-vertical-slice.md`
  — § 7.5 wording corrected to use the
  3-level verification language; the stand-in
  test is honestly named.

## 11. Next

**T-033 is Blocked** on T-032 reaching Executable
verified. The user has three natural next moves:

1. **Authorise the install** — run
   `npm install -g gnhf` (or the from-source
   variant) and re-run the real-tool smoke
   test. T-032 advances to Executable verified;
   T-033 unblocks.
2. **Pivot to T-030 (M4-C closeout)** — close
   M4-C, draft the M4-D umbrella plan, then
   re-attempt T-033 with a fully-scoped M4-D
   plan in hand.
3. **Re-prioritise** — pick a different
   upstream by updating
   `.ai/upstreams/upstream-lock.json` and the
   T-033 plan.

The session does NOT begin T-033 (per the
directive "Do not begin the no-mistakes provider
yet").
