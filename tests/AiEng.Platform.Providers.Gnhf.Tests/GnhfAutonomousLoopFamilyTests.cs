using AiEng.Platform.Application.Infrastructure;
using AiEng.Platform.Application.Providers;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace AiEng.Platform.Providers.Gnhf.Tests;

public sealed class GnhfAutonomousLoopFamilyTests
{
    [Fact]
    public async Task ListProvidersAsync_returns_available_descriptor_when_probe_succeeds()
    {
        var family = new GnhfAutonomousLoopFamily(
            FakeGnhfProbeRunner.Available("0.1.42", "Usage: gnhf [objective]"));

        var result = await family.ListProvidersAsync();

        var descriptor = Assert.Single(result);
        Assert.Equal("gnhf", descriptor.Id);
        Assert.Equal(ProviderFamily.AutonomousLoop, descriptor.Family);
        Assert.Equal(ProviderStatus.Available, descriptor.Status);
        Assert.Equal("0.1.42", descriptor.Version);
        Assert.Equal("gnhf <objective>", descriptor.Metadata["entry_command"]);
        Assert.Equal("fe202c4c92de3bc82b6319ed13bb35023d88410a", descriptor.Metadata["locked_commit"]);
        Assert.Equal("Usage: gnhf [objective]", descriptor.Metadata["help"]);
    }

    [Fact]
    public async Task ListProvidersAsync_returns_unavailable_descriptor_when_probe_fails()
    {
        var family = new GnhfAutonomousLoopFamily(
            FakeGnhfProbeRunner.Unavailable("file not found"));

        var result = await family.ListProvidersAsync();

        var descriptor = Assert.Single(result);
        Assert.Equal(ProviderStatus.Unavailable, descriptor.Status);
        Assert.Null(descriptor.Version);
        Assert.Equal("file not found", descriptor.Metadata["failure_reason"]);
    }

    [Fact]
    public async Task ListProvidersAsync_throws_when_token_is_cancelled()
    {
        var family = new GnhfAutonomousLoopFamily(FakeGnhfProbeRunner.Available("0.1.42"));
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => family.ListProvidersAsync(cts.Token));
    }

    [Fact]
    public async Task ListProvidersAsync_invokes_probe_each_call()
    {
        var probe = FakeGnhfProbeRunner.Available("0.1.42");
        var family = new GnhfAutonomousLoopFamily(probe);

        await family.ListProvidersAsync();
        await family.ListProvidersAsync();

        Assert.Equal(2, probe.CallCount);
    }
}

public sealed class GnhfProcessProbeRunnerTests
{
    [Fact]
    public async Task ProbeAsync_returns_unavailable_when_process_fails()
    {
        var runner = new ScriptedProcessRunner(
            (_, _) => ProcessResult.From(1, "", "gnhf: command not found"));

        var probe = new GnhfProcessProbeRunner(
            runner,
            new TestPlatformInfo(isWindows: false),
            NullLogger<GnhfProcessProbeRunner>.Instance);

        var result = await probe.ProbeAsync();

        Assert.False(result.Available);
        Assert.Null(result.Version);
        Assert.Contains("gnhf: command not found", result.FailureReason);
    }

    [Fact]
    public async Task ProbeAsync_parses_version_and_help()
    {
        var runner = new ScriptedProcessRunner(
            (_, args) => args[0] == "--version"
                ? ProcessResult.From(0, "gnhf 0.1.42 (commit abcdef)", "")
                : ProcessResult.From(0, "Usage: gnhf [objective]\nMore help", ""));

        var probe = new GnhfProcessProbeRunner(
            runner,
            new TestPlatformInfo(isWindows: false),
            NullLogger<GnhfProcessProbeRunner>.Instance);

        var result = await probe.ProbeAsync();

        Assert.True(result.Available);
        Assert.Equal("0.1.42", result.Version);
        Assert.NotNull(result.HelpSummary);
        Assert.Contains("gnhf", result.HelpSummary);
    }

    [Fact]
    public async Task ProbeAsync_uses_gnhf_cmd_on_windows()
    {
        var runner = new ScriptedProcessRunner(
            (_, _) => ProcessResult.From(0, "gnhf 0.1.42", ""));

        var probe = new GnhfProcessProbeRunner(
            runner,
            new TestPlatformInfo(isWindows: true),
            NullLogger<GnhfProcessProbeRunner>.Instance);

        await probe.ProbeAsync();

        Assert.All(runner.Calls, call => Assert.Equal("gnhf.cmd", call.Executable));
    }

    [Fact]
    public async Task ProbeAsync_uses_gnhf_on_non_windows()
    {
        var runner = new ScriptedProcessRunner(
            (_, _) => ProcessResult.From(0, "gnhf 0.1.42", ""));

        var probe = new GnhfProcessProbeRunner(
            runner,
            new TestPlatformInfo(isWindows: false),
            NullLogger<GnhfProcessProbeRunner>.Instance);

        await probe.ProbeAsync();

        Assert.All(runner.Calls, call => Assert.Equal("gnhf", call.Executable));
    }

    [Fact]
    public async Task ProbeAsync_returns_unavailable_when_no_version_match()
    {
        var runner = new ScriptedProcessRunner(
            (_, args) => args[0] == "--version"
                ? ProcessResult.From(0, "no version here", "")
                : ProcessResult.From(0, "Usage: ...", ""));

        var probe = new GnhfProcessProbeRunner(
            runner,
            new TestPlatformInfo(isWindows: false),
            NullLogger<GnhfProcessProbeRunner>.Instance);

        var result = await probe.ProbeAsync();

        Assert.True(result.Available);
        Assert.Null(result.Version);
    }
}
