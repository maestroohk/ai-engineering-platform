using System.Text.RegularExpressions;
using AiEng.Platform.Application.Infrastructure;
using AiEng.Platform.Application.Providers;
using Microsoft.Extensions.Logging;

namespace AiEng.Platform.Providers.Gnhf;

public sealed class GnhfProcessProbeRunner : IGnhfProbeRunner
{
    private static readonly Regex VersionRegex = new(
        @"\b(\d+\.\d+\.\d+(?:[-+][0-9A-Za-z.\-]+)?)\b",
        RegexOptions.Compiled);

    private readonly IProcessRunner _processRunner;
    private readonly IPlatformInfo _platformInfo;
    private readonly ILogger<GnhfProcessProbeRunner> _logger;
    private readonly string _executable;
    private readonly TimeSpan _timeout;

    public GnhfProcessProbeRunner(
        IProcessRunner processRunner,
        IPlatformInfo platformInfo,
        ILogger<GnhfProcessProbeRunner> logger,
        string? executable = null,
        TimeSpan? timeout = null)
    {
        _processRunner = processRunner;
        _platformInfo = platformInfo;
        _logger = logger;
        _executable = executable ?? DefaultExecutable(platformInfo);
        _timeout = timeout ?? TimeSpan.FromSeconds(5);
    }

    public string Executable => _executable;

    public async Task<GnhfProbe> ProbeAsync(CancellationToken cancellationToken = default)
    {
        using var timeoutCts = new CancellationTokenSource(_timeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken, timeoutCts.Token);

        try
        {
            var versionResult = await _processRunner.RunToCompletionAsync(
                _executable,
                new[] { "--version" },
                linkedCts.Token).ConfigureAwait(false);

            if (!versionResult.Succeeded)
            {
                return new GnhfProbe(
                    Available: false,
                    Version: null,
                    HelpSummary: null,
                    FailureReason: versionResult.StandardError.Trim());
            }

            var version = ExtractFirstVersion(versionResult.StandardOutput)
                ?? ExtractFirstVersion(versionResult.StandardError);

            var helpResult = await _processRunner.RunToCompletionAsync(
                _executable,
                new[] { "--help" },
                linkedCts.Token).ConfigureAwait(false);

            var help = helpResult.Succeeded
                ? SummariseHelp(helpResult.StandardOutput)
                : null;

            return new GnhfProbe(
                Available: true,
                Version: version,
                HelpSummary: help,
                FailureReason: null);
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
        {
            _logger.LogWarning("gnhf probe timed out after {Timeout}", _timeout);
            return new GnhfProbe(false, null, null, $"timeout after {_timeout.TotalSeconds:0.#}s");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "gnhf probe failed");
            return new GnhfProbe(false, null, null, ex.GetType().Name);
        }
    }

    private static string DefaultExecutable(IPlatformInfo platformInfo)
        => platformInfo.IsWindows ? "gnhf.cmd" : "gnhf";

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
