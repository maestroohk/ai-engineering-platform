using System.Diagnostics;
using System.Text.RegularExpressions;
using AiEng.Platform.Application.Infrastructure;
using Microsoft.Extensions.Logging;

namespace AiEng.Platform.Providers.Gnhf;

public interface IGnhfProbeRunner
{
    Task<GnhfProbe> ProbeAsync(string? configuredPath = null, CancellationToken cancellationToken = default);
}

public sealed class GnhfProcessProbeRunner : IGnhfProbeRunner
{
    private static readonly Regex VersionRegex = new(
        @"\b(\d+\.\d+\.\d+(?:[-+][0-9A-Za-z.\-]+)?)\b",
        RegexOptions.Compiled);

    private readonly IProcessRunner _processRunner;
    private readonly IGnhfExecutableResolver _resolver;
    private readonly TimeProvider _time;
    private readonly ILogger<GnhfProcessProbeRunner> _logger;
    private readonly TimeSpan _timeout;

    public GnhfProcessProbeRunner(
        IProcessRunner processRunner,
        IGnhfExecutableResolver resolver,
        TimeProvider time,
        ILogger<GnhfProcessProbeRunner> logger,
        TimeSpan? timeout = null)
    {
        _processRunner = processRunner;
        _resolver = resolver;
        _time = time;
        _logger = logger;
        _timeout = timeout ?? TimeSpan.FromSeconds(5);
    }

    public TimeSpan Timeout => _timeout;

    public async Task<GnhfProbe> ProbeAsync(
        string? configuredPath = null,
        CancellationToken cancellationToken = default)
    {
        var resolution = _resolver.Resolve(configuredPath);
        if (resolution.ResolvedPath is null)
        {
            var snapshot = new GnhfHealthSnapshot(
                DetectedAt: _time.GetUtcNow(),
                DurationMs: 0,
                State: GnhfHealthState.NotInstalled,
                Version: null,
                HelpSummary: null,
                StandardOutput: null,
                StandardError: null,
                ExitCode: null,
                FailureReason: resolution.FailureReason);
            return new GnhfProbe(resolution, snapshot);
        }

        var start = _time.GetTimestamp();
        using var timeoutCts = new CancellationTokenSource(_timeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken, timeoutCts.Token);

        try
        {
            var versionResult = await _processRunner.RunToCompletionAsync(
                resolution.ResolvedPath,
                new[] { "--version" },
                linkedCts.Token).ConfigureAwait(false);

            if (!versionResult.Succeeded)
            {
                var unhealthyDuration = ElapsedMs(start);
                var unhealthySnapshot = new GnhfHealthSnapshot(
                    DetectedAt: _time.GetUtcNow(),
                    DurationMs: unhealthyDuration,
                    State: GnhfHealthState.InstalledButUnhealthy,
                    Version: null,
                    HelpSummary: null,
                    StandardOutput: versionResult.StandardOutput,
                    StandardError: versionResult.StandardError,
                    ExitCode: versionResult.ExitCode,
                    FailureReason: $"gnhf --version exited {versionResult.ExitCode}");
                return new GnhfProbe(resolution, unhealthySnapshot);
            }

            var version = ExtractFirstVersion(versionResult.StandardOutput)
                ?? ExtractFirstVersion(versionResult.StandardError);

            var helpResult = await _processRunner.RunToCompletionAsync(
                resolution.ResolvedPath,
                new[] { "--help" },
                linkedCts.Token).ConfigureAwait(false);

            var help = helpResult.Succeeded
                ? SummariseHelp(helpResult.StandardOutput)
                : null;

            var duration = ElapsedMs(start);
            var state = version is null
                ? GnhfHealthState.VersionUnknown
                : GnhfHealthState.InstalledAndHealthy;

            var snapshot = new GnhfHealthSnapshot(
                DetectedAt: _time.GetUtcNow(),
                DurationMs: duration,
                State: state,
                Version: version,
                HelpSummary: help,
                StandardOutput: versionResult.StandardOutput,
                StandardError: versionResult.StandardError,
                ExitCode: versionResult.ExitCode,
                FailureReason: null);
            return new GnhfProbe(resolution, snapshot);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            var duration = ElapsedMs(start);
            var snapshot = new GnhfHealthSnapshot(
                DetectedAt: _time.GetUtcNow(),
                DurationMs: duration,
                State: GnhfHealthState.Cancelled,
                Version: null,
                HelpSummary: null,
                StandardOutput: null,
                StandardError: null,
                ExitCode: null,
                FailureReason: "caller cancelled");
            return new GnhfProbe(resolution, snapshot);
        }
        catch (OperationCanceledException)
        {
            var duration = ElapsedMs(start);
            var snapshot = new GnhfHealthSnapshot(
                DetectedAt: _time.GetUtcNow(),
                DurationMs: duration,
                State: GnhfHealthState.TimedOut,
                Version: null,
                HelpSummary: null,
                StandardOutput: null,
                StandardError: null,
                ExitCode: null,
                FailureReason: $"timeout after {_timeout.TotalSeconds:0.#}s");
            return new GnhfProbe(resolution, snapshot);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "gnhf probe failed");
            var duration = ElapsedMs(start);
            var snapshot = new GnhfHealthSnapshot(
                DetectedAt: _time.GetUtcNow(),
                DurationMs: duration,
                State: GnhfHealthState.InstalledButUnhealthy,
                Version: null,
                HelpSummary: null,
                StandardOutput: null,
                StandardError: null,
                ExitCode: null,
                FailureReason: ex.GetType().Name + ": " + ex.Message);
            return new GnhfProbe(resolution, snapshot);
        }
    }

    private long ElapsedMs(long startTimestamp) =>
        (long)((_time.GetTimestamp() - startTimestamp) * 1000.0 / _time.TimestampFrequency);

    private static string? ExtractFirstVersion(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        var match = VersionRegex.Match(text);
        return match.Success ? match.Groups[1].Value : null;
    }

    private static string? SummariseHelp(string? helpText)
    {
        if (string.IsNullOrWhiteSpace(helpText))
        {
            return null;
        }

        var lines = helpText
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.TrimEnd('\r'))
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Take(2)
            .ToArray();

        return lines.Length == 0 ? null : string.Join(" | ", lines);
    }
}
