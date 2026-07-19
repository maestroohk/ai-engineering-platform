using System.Runtime.InteropServices;
using AiEng.Platform.Providers.Gnhf;
using Xunit;

namespace AiEng.Platform.Providers.Gnhf.Tests;

/// <summary>
/// Real-tool smoke tests. They invoke the actual installed gnhf executable
/// (when present) and assert on the truthful health snapshot. When gnhf is
/// not installed, these tests SKIP rather than fail — installation is
/// deliberately not performed from inside the test suite.
/// </summary>
public sealed class GnhfRealToolSmokeTests
{
    private static string? FindInstalledGnhf()
    {
        var names = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? new[] { "gnhf.cmd", "gnhf.exe", "gnhf" }
            : new[] { "gnhf" };

        foreach (var name in names)
        {
            try
            {
                using var probe = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "where.exe" : "which",
                        Arguments = name,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                probe.Start();
                var output = probe.StandardOutput.ReadToEnd().Trim();
                probe.WaitForExit(2000);
                if (probe.ExitCode == 0 && !string.IsNullOrWhiteSpace(output))
                {
                    var first = output.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                        .Select(line => line.Trim())
                        .FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(first))
                    {
                        return first;
                    }
                }
            }
            catch
            {
            }
        }
        return null;
    }

    [SkippableFact]
    public async Task RealGnhf_probe_succeeds_when_executable_is_installed()
    {
        var resolved = FindInstalledGnhf();
        Skip.If(resolved is null, "gnhf executable not installed. Documented install command: `npm install -g gnhf` (per gnhf README).");

        var runner = new AiEng.Platform.Providers.Gnhf.Tests.RealProcessRunner();
        var resolver = new AiEng.Platform.Providers.Gnhf.GnhfExecutableResolver(runner, new TestPlatformInfo(isWindows: RuntimeInformation.IsOSPlatform(OSPlatform.Windows)));

        var probeRunner = new GnhfProcessProbeRunner(
            runner,
            resolver,
            TimeProvider.System,
            Microsoft.Extensions.Logging.Abstractions.NullLogger<GnhfProcessProbeRunner>.Instance,
            timeout: TimeSpan.FromSeconds(10));

        var probe = await probeRunner.ProbeAsync(cancellationToken: CancellationToken.None);

        Assert.Equal(GnhfHealthState.InstalledAndHealthy, probe.Health.State);
        Assert.NotNull(probe.Health.Version);
        Assert.Equal(resolved, probe.Resolution.ResolvedPath);
    }
}

internal sealed class RealProcessRunner : AiEng.Platform.Application.Infrastructure.IProcessRunner
{
    public async IAsyncEnumerable<string> RunAsync(
        string executable,
        IReadOnlyList<string> arguments,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var psi = new System.Diagnostics.ProcessStartInfo
        {
            FileName = executable,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        foreach (var arg in arguments)
        {
            psi.ArgumentList.Add(arg);
        }
        using var process = System.Diagnostics.Process.Start(psi)!;
        await process.WaitForExitAsync(cancellationToken);
        yield break;
    }

    public async Task<AiEng.Platform.Application.Infrastructure.ProcessResult> RunToCompletionAsync(
        string executable,
        IReadOnlyList<string> arguments,
        CancellationToken cancellationToken = default)
    {
        var psi = new System.Diagnostics.ProcessStartInfo
        {
            FileName = executable,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        foreach (var arg in arguments)
        {
            psi.ArgumentList.Add(arg);
        }
        using var process = System.Diagnostics.Process.Start(psi)!;
        var stdoutTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var stderrTask = process.StandardError.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);
        var stdout = await stdoutTask;
        var stderr = await stderrTask;
        return AiEng.Platform.Application.Infrastructure.ProcessResult.From(process.ExitCode, stdout, stderr);
    }
}
