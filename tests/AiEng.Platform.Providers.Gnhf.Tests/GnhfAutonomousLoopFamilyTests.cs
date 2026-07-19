using AiEng.Platform.Application.Infrastructure;
using AiEng.Platform.Application.Providers;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace AiEng.Platform.Providers.Gnhf.Tests;

public sealed class GnhfAutonomousLoopFamilyTests
{
    [Fact]
    public async Task ListProvidersAsync_returns_available_descriptor_when_probe_reports_healthy()
    {
        var family = new GnhfAutonomousLoopFamily(
            FakeGnhfProbeRunner.InstalledAndHealthy("0.1.42", "Usage: gnhf [objective]"));

        var result = await family.ListProvidersAsync();

        var descriptor = Assert.Single(result);
        Assert.Equal("gnhf", descriptor.Id);
        Assert.Equal(ProviderFamily.AutonomousLoop, descriptor.Family);
        Assert.Equal(ProviderStatus.Available, descriptor.Status);
        Assert.Equal("0.1.42", descriptor.Version);
        Assert.Equal("gnhf <objective>", descriptor.Metadata["entry_command"]);
        Assert.Equal("fe202c4c92de3bc82b6319ed13bb35023d88410a", descriptor.Metadata["locked_commit"]);
        Assert.Equal("true", descriptor.Metadata["actual_executable_verified"]);
        Assert.Equal("false", descriptor.Metadata["bounded_workflow_verified"]);
        Assert.Equal("Usage: gnhf [objective]", descriptor.Metadata["help"]);
        Assert.True(descriptor.Metadata.ContainsKey("health_check_timestamp"));
        Assert.True(descriptor.Metadata.ContainsKey("health_check_duration_ms"));
    }

    [Fact]
    public async Task ListProvidersAsync_returns_unavailable_descriptor_when_not_installed()
    {
        var family = new GnhfAutonomousLoopFamily(
            FakeGnhfProbeRunner.NotInstalled("not found on PATH, npm-global, or pnpm-global"));

        var result = await family.ListProvidersAsync();

        var descriptor = Assert.Single(result);
        Assert.Equal(ProviderStatus.Unavailable, descriptor.Status);
        Assert.Null(descriptor.Version);
        Assert.Equal("false", descriptor.Metadata["actual_executable_verified"]);
        Assert.Equal("not-found", descriptor.Metadata["executable_resolution"]);
        Assert.Equal("(none)", descriptor.Metadata["executable_path"]);
        Assert.Equal("NotInstalled", descriptor.Metadata["health_check_state"]);
    }

    [Fact]
    public async Task ListProvidersAsync_throws_when_token_is_cancelled()
    {
        var family = new GnhfAutonomousLoopFamily(FakeGnhfProbeRunner.InstalledAndHealthy("0.1.42"));
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => family.ListProvidersAsync(cts.Token));
    }

    [Fact]
    public async Task ListProvidersAsync_invokes_probe_each_call()
    {
        var probe = FakeGnhfProbeRunner.InstalledAndHealthy("0.1.42");
        var family = new GnhfAutonomousLoopFamily(probe);

        await family.ListProvidersAsync();
        await family.ListProvidersAsync();

        Assert.Equal(2, probe.CallCount);
    }
}

public sealed class GnhfProcessProbeRunnerTests
{
    private static GnhfProcessProbeRunner BuildRunner(
        IProcessRunner processRunner,
        IGnhfExecutableResolver resolver,
        TimeProvider? time = null,
        TimeSpan? timeout = null) =>
        new(
            processRunner,
            resolver,
            time ?? TimeProvider.System,
            NullLogger<GnhfProcessProbeRunner>.Instance,
            timeout);

    [Fact]
    public async Task ProbeAsync_returns_not_installed_when_resolver_finds_nothing()
    {
        var runner = new ScriptedProcessRunner((_, _) => ProcessResult.From(0, "ignored", ""));
        var resolver = new NotFoundGnhfExecutableResolver();

        var probe = await BuildRunner(runner, resolver, timeout: TimeSpan.FromSeconds(1))
            .ProbeAsync();

        Assert.Equal(GnhfHealthState.NotInstalled, probe.Health.State);
        Assert.Empty(runner.Calls);
        Assert.Null(probe.Health.Version);
        Assert.Equal("gnhf: not on PATH", probe.Health.FailureReason);
    }

    [Fact]
    public async Task ProbeAsync_parses_version_and_help_when_executable_returns_zero()
    {
        var runner = new ScriptedProcessRunner(
            (_, args) => args[0] == "--version"
                ? ProcessResult.From(0, "gnhf 0.1.42 (commit abcdef)", "")
                : ProcessResult.From(0, "Usage: gnhf [objective]\nMore help", ""));

        var probe = await BuildRunner(runner, new FakeGnhfExecutableResolver())
            .ProbeAsync();

        Assert.Equal(GnhfHealthState.InstalledAndHealthy, probe.Health.State);
        Assert.Equal("0.1.42", probe.Health.Version);
        Assert.Contains("gnhf", probe.Health.HelpSummary);
    }

    [Fact]
    public async Task ProbeAsync_uses_resolved_executable_path_from_resolver()
    {
        var runner = new ScriptedProcessRunner(
            (_, args) => args[0] == "--version"
                ? ProcessResult.From(0, "gnhf 0.1.42", "")
                : ProcessResult.From(0, "Usage", ""));

        var probe = await BuildRunner(runner, new FakeGnhfExecutableResolver())
            .ProbeAsync();

        Assert.Equal(GnhfExecutionMode.NativeLinux, probe.Resolution.Mode);
        Assert.Equal("path", probe.Resolution.ResolutionSource);
        Assert.Equal("/usr/local/bin/gnhf", probe.Resolution.ResolvedPath);
        Assert.All(runner.Calls, call => Assert.Equal("/usr/local/bin/gnhf", call.Executable));
    }

    [Fact]
    public async Task ProbeAsync_returns_unhealthy_when_version_exit_code_is_non_zero()
    {
        var runner = new ScriptedProcessRunner(
            (_, _) => ProcessResult.From(1, "", "gnhf: command not found"));

        var probe = await BuildRunner(runner, new FakeGnhfExecutableResolver())
            .ProbeAsync();

        Assert.Equal(GnhfHealthState.InstalledButUnhealthy, probe.Health.State);
        Assert.Null(probe.Health.Version);
        Assert.Equal(1, probe.Health.ExitCode);
        Assert.Contains("gnhf --version exited 1", probe.Health.FailureReason);
    }

    [Fact]
    public async Task ProbeAsync_returns_version_unknown_when_no_version_regex_match()
    {
        var runner = new ScriptedProcessRunner(
            (_, args) => args[0] == "--version"
                ? ProcessResult.From(0, "no version here", "")
                : ProcessResult.From(0, "Usage: ...", ""));

        var probe = await BuildRunner(runner, new FakeGnhfExecutableResolver())
            .ProbeAsync();

        Assert.Equal(GnhfHealthState.VersionUnknown, probe.Health.State);
        Assert.Null(probe.Health.Version);
    }

    [Fact]
    public async Task ProbeAsync_returns_timed_out_when_process_hangs()
    {
        var runner = new HangingProcessRunner();
        var resolver = new FakeGnhfExecutableResolver();

        var probe = await BuildRunner(runner, resolver, timeout: TimeSpan.FromMilliseconds(150))
            .ProbeAsync();

        Assert.Equal(GnhfHealthState.TimedOut, probe.Health.State);
        Assert.Contains("timeout", probe.Health.FailureReason, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ProbeAsync_returns_cancelled_when_caller_cancels()
    {
        var runner = new HangingProcessRunner();
        var resolver = new FakeGnhfExecutableResolver();

        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));

        var probe = await BuildRunner(
            runner,
            resolver,
            timeout: TimeSpan.FromSeconds(5))
            .ProbeAsync(cancellationToken: cts.Token);

        Assert.Equal(GnhfHealthState.Cancelled, probe.Health.State);
    }

    [Fact]
    public async Task ProbeAsync_records_resolution_and_health_timestamp()
    {
        var fixedTime = new DateTimeOffset(2026, 7, 19, 12, 0, 0, TimeSpan.Zero);
        var time = new FixedTimeProvider(fixedTime);

        var runner = new ScriptedProcessRunner(
            (_, args) => args[0] == "--version"
                ? ProcessResult.From(0, "gnhf 0.1.42", "")
                : ProcessResult.From(0, "Usage", ""));

        var probe = await BuildRunner(runner, new FakeGnhfExecutableResolver(), time: time)
            .ProbeAsync();

        Assert.Equal(fixedTime, probe.Health.DetectedAt);
        Assert.True(probe.Health.DurationMs >= 0);
    }
}

internal sealed class FakeGnhfExecutableResolver : IGnhfExecutableResolver
{
    public GnhfExecutableResolution Resolve(string? configuredPath = null)
    {
        if (!string.IsNullOrWhiteSpace(configuredPath))
        {
            if (File.Exists(configuredPath))
            {
                return new GnhfExecutableResolution(
                    ResolvedPath: configuredPath,
                    Mode: GnhfExecutionMode.NativeLinux,
                    ResolutionSource: "configured",
                    FailureReason: null);
            }
            return new GnhfExecutableResolution(
                ResolvedPath: null,
                Mode: GnhfExecutionMode.NotInstalled,
                ResolutionSource: "configured",
                FailureReason: $"configured path not found: {configuredPath}");
        }

        return new GnhfExecutableResolution(
            ResolvedPath: "/usr/local/bin/gnhf",
            Mode: GnhfExecutionMode.NativeLinux,
            ResolutionSource: "path",
            FailureReason: null);
    }
}

internal sealed class NotFoundGnhfExecutableResolver : IGnhfExecutableResolver
{
    public GnhfExecutableResolution Resolve(string? configuredPath = null) =>
        new(
            ResolvedPath: null,
            Mode: GnhfExecutionMode.NotInstalled,
            ResolutionSource: null,
            FailureReason: "gnhf: not on PATH");
}

internal sealed class FixedTimeProvider : TimeProvider
{
    private readonly DateTimeOffset _now;
    public FixedTimeProvider(DateTimeOffset now) => _now = now;
    public override DateTimeOffset GetUtcNow() => _now;
}
