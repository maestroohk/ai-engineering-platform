using AiEng.Platform.Application.Infrastructure;
using AiEng.Platform.Infrastructure.ProcessRunner;
using AiEng.Platform.Providers.Gnhf;
using Microsoft.Extensions.Logging.Abstractions;

namespace AiEng.Platform.Providers.Gnhf.SmokeTest;

internal static class Program
{
    public static async Task<int> Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("usage: GnhfSmokeTest <path-to-gnhf> [--simulate-unavailable]");
            return 2;
        }

        var executable = args[0];
        var simulateUnavailable = args.Contains("--simulate-unavailable");

        IProcessRunner runner = simulateUnavailable
            ? new AlwaysFailingProcessRunner()
            : new SystemProcessRunner();

        var platformInfo = new HostPlatformInfo();
        var probe = new GnhfProcessProbeRunner(
            runner,
            platformInfo,
            NullLogger<GnhfProcessProbeRunner>.Instance,
            executable: executable);

        var result = await probe.ProbeAsync();

        Console.WriteLine($"Available : {result.Available}");
        Console.WriteLine($"Version   : {result.Version ?? "(none)"}");
        Console.WriteLine($"Help      : {result.HelpSummary ?? "(none)"}");
        Console.WriteLine($"Failure   : {result.FailureReason ?? "(none)"}");
        Console.WriteLine($"Executable: {probe.Executable}");

        return simulateUnavailable
            ? (result.Available ? 1 : 0)
            : (result.Available ? 0 : 1);
    }
}

internal sealed class AlwaysFailingProcessRunner : IProcessRunner
{
    public IAsyncEnumerable<string> RunAsync(string executable, IReadOnlyList<string> arguments, CancellationToken cancellationToken = default) => Empty();
    public Task<ProcessResult> RunToCompletionAsync(string executable, IReadOnlyList<string> arguments, CancellationToken cancellationToken = default) =>
        Task.FromResult(ProcessResult.From(127, "", $"{executable}: command not found (smoke-test)"));
    private static async IAsyncEnumerable<string> Empty()
    {
        await Task.CompletedTask;
        yield break;
    }
}
