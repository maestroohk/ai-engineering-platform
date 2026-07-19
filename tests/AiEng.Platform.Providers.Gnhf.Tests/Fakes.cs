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

    public Task<GnhfProbe> ProbeAsync(CancellationToken cancellationToken = default)
    {
        CallCount++;
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_probe);
    }

    public static FakeGnhfProbeRunner Available(string version, string? help = null) =>
        new(new GnhfProbe(true, version, help, null));

    public static FakeGnhfProbeRunner Unavailable(string reason) =>
        new(new GnhfProbe(false, null, null, reason));
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
        return Task.FromResult(_script(executable, arguments));
    }
}

internal sealed class TestPlatformInfo : IPlatformInfo
{
    public TestPlatformInfo(bool isWindows) => IsWindows = isWindows;
    public bool IsWindows { get; }
    public string GetDataDirectory() => Path.Combine(Path.GetTempPath(), "aieng-test", "data");
    public string GetConfigDirectory() => Path.Combine(Path.GetTempPath(), "aieng-test", "config");
}
