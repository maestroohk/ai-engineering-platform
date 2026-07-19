using System.Diagnostics;
using System.Runtime.InteropServices;
using AiEng.Platform.Application.Infrastructure;

namespace AiEng.Platform.Providers.Gnhf;

public interface IGnhfExecutableResolver
{
    GnhfExecutableResolution Resolve(string? configuredPath = null);
}

public sealed class GnhfExecutableResolver : IGnhfExecutableResolver
{
    private readonly IProcessRunner _processRunner;
    private readonly IPlatformInfo _platformInfo;
    private readonly string _homeDirectory;

    public GnhfExecutableResolver(
        IProcessRunner processRunner,
        IPlatformInfo platformInfo,
        string? homeDirectory = null)
    {
        _processRunner = processRunner;
        _platformInfo = platformInfo;
        _homeDirectory = homeDirectory ?? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    }

    public GnhfExecutableResolution Resolve(string? configuredPath = null)
    {
        if (!string.IsNullOrWhiteSpace(configuredPath))
        {
            if (File.Exists(configuredPath))
            {
                return new GnhfExecutableResolution(
                    ResolvedPath: configuredPath,
                    Mode: ResolveMode(),
                    ResolutionSource: "configured",
                    FailureReason: null);
            }

            return new GnhfExecutableResolution(
                ResolvedPath: null,
                Mode: GnhfExecutionMode.NotInstalled,
                ResolutionSource: "configured",
                FailureReason: $"configured path not found: {configuredPath}");
        }

        var candidates = CandidateNames(_platformInfo);
        foreach (var name in candidates)
        {
            var resolved = ResolveOnPath(name);
            if (resolved is not null)
            {
                return new GnhfExecutableResolution(
                    ResolvedPath: resolved,
                    Mode: ResolveMode(),
                    ResolutionSource: "path",
                    FailureReason: null);
            }
        }

        var npmGlobal = ResolveNpmGlobalBinary();
        if (npmGlobal is not null)
        {
            return new GnhfExecutableResolution(
                ResolvedPath: npmGlobal,
                Mode: ResolveMode(),
                ResolutionSource: "npm-global",
                FailureReason: null);
        }

        var pnpmGlobal = ResolvePnpmGlobalBinary();
        if (pnpmGlobal is not null)
        {
            return new GnhfExecutableResolution(
                ResolvedPath: pnpmGlobal,
                Mode: ResolveMode(),
                ResolutionSource: "pnpm-global",
                FailureReason: null);
        }

        return new GnhfExecutableResolution(
            ResolvedPath: null,
            Mode: GnhfExecutionMode.NotInstalled,
            ResolutionSource: null,
            FailureReason: $"not found on PATH, npm-global, or pnpm-global; tried: {string.Join(", ", candidates)}");
    }

    private GnhfExecutionMode ResolveMode()
    {
        if (_platformInfo.IsWindows)
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                ? GnhfExecutionMode.Wsl
                : GnhfExecutionMode.NativeWindows;
        }

        return RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
            ? GnhfExecutionMode.NativeMacOs
            : GnhfExecutionMode.NativeLinux;
    }

    private static IReadOnlyList<string> CandidateNames(IPlatformInfo platformInfo)
        => CandidateNamesForTests(platformInfo);

    public static IReadOnlyList<string> CandidateNamesForTests(IPlatformInfo platformInfo)
        => platformInfo.IsWindows
            ? new[] { "gnhf.cmd", "gnhf.exe", "gnhf" }
            : new[] { "gnhf" };

    private string? ResolveOnPath(string name)
    {
        try
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _platformInfo.IsWindows && !name.EndsWith(".cmd", StringComparison.OrdinalIgnoreCase)
                        ? "where.exe"
                        : (_platformInfo.IsWindows ? "where.exe" : "which"),
                    Arguments = name,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            var output = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit(2000);
            if (process.ExitCode == 0 && !string.IsNullOrWhiteSpace(output))
            {
                var first = output.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .Select(line => line.Trim())
                    .FirstOrDefault();
                return string.IsNullOrWhiteSpace(first) ? null : first;
            }
        }
        catch
        {
        }
        return null;
    }

    private string? ResolveNpmGlobalBinary()
    {
        try
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "npm",
                    Arguments = "root -g",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            var root = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit(5000);
            if (process.ExitCode != 0 || string.IsNullOrWhiteSpace(root))
            {
                return null;
            }

            var binDir = Path.Combine(root, "..", "bin");
            var gnhfCmd = Path.Combine(binDir, _platformInfo.IsWindows ? "gnhf.cmd" : "gnhf");
            return File.Exists(gnhfCmd) ? gnhfCmd : null;
        }
        catch
        {
            return null;
        }
    }

    private string? ResolvePnpmGlobalBinary()
    {
        try
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "pnpm",
                    Arguments = "root -g",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            var root = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit(5000);
            if (process.ExitCode != 0 || string.IsNullOrWhiteSpace(root))
            {
                return null;
            }

            var binDir = Path.Combine(root, "..", "..", "bin");
            var gnhfCmd = Path.Combine(binDir, _platformInfo.IsWindows ? "gnhf.cmd" : "gnhf");
            return File.Exists(gnhfCmd) ? gnhfCmd : null;
        }
        catch
        {
            return null;
        }
    }
}
