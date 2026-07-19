using AiEng.Platform.Application.Infrastructure;
using AiEng.Platform.Application.Providers;

namespace AiEng.Platform.Providers.Gnhf.Tests;

internal sealed class FakeGnhfProbeRunner : IGnhfProbeRunner
{
    private readonly GnhfProbe _probe;
    public int CallCount { get; private set; }

    public FakeGnhfProbeRunner(GnhfProbe probe)
    {
        _probe = probe;
    }

    public Task<GnhfProbe> ProbeAsync(string? configuredPath = null, CancellationToken cancellationToken = default)
    {
        CallCount++;
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_probe);
    }

    public static FakeGnhfProbeRunner InstalledAndHealthy(
        string version,
        string? help = null,
        string? executablePath = "/usr/local/bin/gnhf")
    {
        var resolution = new GnhfExecutableResolution(
            ResolvedPath: executablePath,
            Mode: GnhfExecutionMode.NativeLinux,
            ResolutionSource: "path",
            FailureReason: null);
        var health = new GnhfHealthSnapshot(
            DetectedAt: DateTimeOffset.UtcNow,
            DurationMs: 42,
            State: GnhfHealthState.InstalledAndHealthy,
            Version: version,
            HelpSummary: help,
            StandardOutput: null,
            StandardError: null,
            ExitCode: 0,
            FailureReason: null);
        return new FakeGnhfProbeRunner(new GnhfProbe(resolution, health));
    }

    public static FakeGnhfProbeRunner NotInstalled(string reason)
    {
        var resolution = new GnhfExecutableResolution(
            ResolvedPath: null,
            Mode: GnhfExecutionMode.NotInstalled,
            ResolutionSource: null,
            FailureReason: reason);
        var health = new GnhfHealthSnapshot(
            DetectedAt: DateTimeOffset.UtcNow,
            DurationMs: 0,
            State: GnhfHealthState.NotInstalled,
            Version: null,
            HelpSummary: null,
            StandardOutput: null,
            StandardError: null,
            ExitCode: null,
            FailureReason: reason);
        return new FakeGnhfProbeRunner(new GnhfProbe(resolution, health));
    }
}

internal sealed class ScriptedProcessRunner : IProcessRunner
{
    private readonly Func<string, IReadOnlyList<string>, ProcessResult> _script;

    public List<(string Executable, IReadOnlyList<string> Arguments)> Calls { get; } = new();

    public ScriptedProcessRunner(Func<string, IReadOnlyList<string>, ProcessResult> script)
    {
        _script = script;
    }

    public IAsyncEnumerable<string> RunAsync(
        string executable,
        IReadOnlyList<string> arguments,
        CancellationToken cancellationToken = default)
    {
        Calls.Add((executable, arguments));
        return Empty();
        static async IAsyncEnumerable<string> Empty()
        {
            await Task.CompletedTask;
            yield break;
        }
    }

    public Task<ProcessResult> RunToCompletionAsync(
        string executable,
        IReadOnlyList<string> arguments,
        CancellationToken cancellationToken = default)
    {
        Calls.Add((executable, arguments));
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_script(executable, arguments));
    }
}

internal sealed class HangingProcessRunner : IProcessRunner
{
    public IAsyncEnumerable<string> RunAsync(
        string executable,
        IReadOnlyList<string> arguments,
        CancellationToken cancellationToken = default)
    {
        return Hanging(cancellationToken);
        static async IAsyncEnumerable<string> Hanging(
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct)
        {
            await Task.Delay(Timeout.Infinite, ct);
            yield break;
        }
    }

    public async Task<ProcessResult> RunToCompletionAsync(
        string executable,
        IReadOnlyList<string> arguments,
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(Timeout.Infinite, cancellationToken);
        return ProcessResult.From(0, "", "");
    }
}

internal sealed class AlwaysFailingProcessRunner : IProcessRunner
{
    public IAsyncEnumerable<string> RunAsync(string executable, IReadOnlyList<string> arguments, CancellationToken cancellationToken = default)
    {
        return Empty();
        static async IAsyncEnumerable<string> Empty()
        {
            await Task.CompletedTask;
            yield break;
        }
    }

    public Task<ProcessResult> RunToCompletionAsync(string executable, IReadOnlyList<string> arguments, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(ProcessResult.From(127, string.Empty, $"{executable}: command not found"));
    }
}

internal sealed class TestPlatformInfo : IPlatformInfo
{
    public TestPlatformInfo(bool isWindows) => IsWindows = isWindows;
    public bool IsWindows { get; }
    public string GetDataDirectory() => Path.Combine(Path.GetTempPath(), "aieng-test", "data");
    public string GetConfigDirectory() => Path.Combine(Path.GetTempPath(), "aieng-test", "config");
}
