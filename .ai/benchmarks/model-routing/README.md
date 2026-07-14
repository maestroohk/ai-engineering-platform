# Model Routing Benchmarks

> **Benchmark stubs.** The benchmark is documented but
> not run in this task. Future benchmark runs populate
> `results.json`.

## Tasks

The benchmark covers five task kinds. Each task is run
on the `high`, `standard`, `economy`, `review`, and
`fallback` profiles (when configured). The benchmark
records wall-clock time, prompt token cost, completion
token cost, validation pass rate, and reviewer verdict.

| Key                   | Task kind               | Expected phase | Expected profile hint |
| --------------------- | ----------------------- | -------------- | --------------------- |
| `plan-creation`       | Architecture planning   | plan           | high                  |
| `blazor-component`    | Feature implementation  | implement      | standard              |
| `service-and-tests`   | Service + tests         | implement      | standard              |
| `bug-fix`             | Targeted bug fix        | implement      | standard              |
| `documentation-closeout` | Documentation       | document       | economy               |

## Outputs

- `benchmark-tasks.json` — the task list (paths, expected
  outputs, profile hints).
- `results.json` — empty result list; populated by
  future benchmark runs.

## Lifecycle

The benchmark is **not** run in this task. The router
respects the user's `-NoBench` flag (the default).

## Stop Conditions

- A benchmark run invokes a paid cloud model without
  explicit user authorisation: stop and write
  `status: "blocked"`.
- A benchmark result fails to validate against
  `results.schema.json` (when added): stop and write
  `status: "blocked"`.
