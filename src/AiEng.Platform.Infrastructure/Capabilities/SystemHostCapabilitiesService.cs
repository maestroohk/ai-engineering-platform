using System.Text.RegularExpressions;
using AiEng.Platform.Application.Capabilities;
using AiEng.Platform.Application.Infrastructure;
using Microsoft.Extensions.Logging;

namespace AiEng.Platform.Infrastructure.Capabilities;

public sealed class SystemHostCapabilitiesService : IHostCapabilitiesService
{
    private const string ProviderCredentialKeyPrefix = "provider:";
    private const string TokenCredentialSlot = "token";
    private static readonly TimeSpan ProbeTimeout = TimeSpan.FromSeconds(5);

    private readonly IProcessRunner _processRunner;
    private readonly ICredentialVault _credentialVault;
    private readonly IPlatformInfo _platformInfo;
    private readonly ILogger<SystemHostCapabilitiesService> _logger;
    private readonly TimeProvider _clock;

    public SystemHostCapabilitiesService(
        IProcessRunner processRunner,
        ICredentialVault credentialVault,
        IPlatformInfo platformInfo,
        ILogger<SystemHostCapabilitiesService> logger)
        : this(processRunner, credentialVault, platformInfo, logger, TimeProvider.System)
    {
    }

    public SystemHostCapabilitiesService(
        IProcessRunner processRunner,
        ICredentialVault credentialVault,
        IPlatformInfo platformInfo,
        ILogger<SystemHostCapabilitiesService> logger,
        TimeProvider clock)
    {
        _processRunner = processRunner ?? throw new ArgumentNullException(nameof(processRunner));
        _credentialVault = credentialVault ?? throw new ArgumentNullException(nameof(credentialVault));
        _platformInfo = platformInfo ?? throw new ArgumentNullException(nameof(platformInfo));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    private static readonly HostToolProbe[] HostToolProbes = new[]
    {
        new HostToolProbe(
            Key: "git",
            Executable: "git",
            Arguments: "--version",
            WindowsOnly: false,
            VersionPattern: new Regex(@"git version (.+)", RegexOptions.Compiled)),
        new HostToolProbe(
            Key: "ollama",
            Executable: "ollama",
            Arguments: "--version",
            WindowsOnly: false,
            VersionPattern: new Regex(@"ollama version (.+)", RegexOptions.Compiled)),
        new HostToolProbe(
            Key: "powershell",
            Executable: "powershell.exe",
            Arguments: "-NoProfile -Command \"(Get-Host).Version.ToString()\"",
            WindowsOnly: true,
            VersionPattern: new Regex(@"^(.+)$", RegexOptions.Compiled)),
        new HostToolProbe(
            Key: "wsl",
            Executable: "wsl.exe",
            Arguments: "--version",
            WindowsOnly: true,
            VersionPattern: new Regex(@"WSL version: (.+)", RegexOptions.Compiled)),
        new HostToolProbe(
            Key: "wt",
            Executable: "wt.exe",
            Arguments: "-v",
            WindowsOnly: true,
            VersionPattern: new Regex(@"Windows Terminal (?<v>.+)", RegexOptions.Compiled)),
        new HostToolProbe(
            Key: "bash",
            Executable: "bash.exe",
            Arguments: "--version",
            WindowsOnly: false,
            VersionPattern: new Regex(@"GNU bash, version (.+)", RegexOptions.Compiled)),
    };

    private static readonly ProviderCredentialProbe[] ProviderCredentialProbes = new[]
    {
        new ProviderCredentialProbe(Key: "provider:git", CredentialName: BuildCredentialName("git")),
        new ProviderCredentialProbe(Key: "provider:ollama", CredentialName: BuildCredentialName("ollama")),
        new ProviderCredentialProbe(Key: "provider:powershell", CredentialName: BuildCredentialName("powershell")),
        new ProviderCredentialProbe(Key: "provider:wsl", CredentialName: BuildCredentialName("wsl")),
        new ProviderCredentialProbe(Key: "provider:wt", CredentialName: BuildCredentialName("wt")),
        new ProviderCredentialProbe(Key: "provider:bash", CredentialName: BuildCredentialName("bash")),
    };

    public async Task<HostCapabilities> DetectAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var detectedAt = _clock.GetUtcNow();
        var capabilities = new List<HostCapability>(HostToolProbes.Length + ProviderCredentialProbes.Length);

        foreach (var probe in HostToolProbes)
        {
            capabilities.Add(await ProbeHostToolAsync(probe, cancellationToken).ConfigureAwait(false));
        }

        foreach (var probe in ProviderCredentialProbes)
        {
            capabilities.Add(await ProbeProviderCredentialAsync(probe, cancellationToken).ConfigureAwait(false));
        }

        return new HostCapabilities(capabilities, detectedAt);
    }

    private async Task<HostCapability> ProbeHostToolAsync(HostToolProbe probe, CancellationToken cancellationToken)
    {
        if (probe.WindowsOnly && !_platformInfo.IsWindows)
        {
            return new HostCapability(probe.Key, Available: false, Version: null, CredentialAvailable: false, CredentialName: null);
        }

        using var timeoutCts = new CancellationTokenSource(ProbeTimeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        try
        {
            var result = await _processRunner
                .RunToCompletionAsync(probe.Executable, new[] { probe.Arguments }, linkedCts.Token)
                .ConfigureAwait(false);

            if (!result.Succeeded)
            {
                return new HostCapability(probe.Key, Available: false, Version: null, CredentialAvailable: false, CredentialName: null);
            }

            var version = ExtractVersion(result.StandardOutput, probe.VersionPattern);
            return new HostCapability(probe.Key, Available: true, Version: version, CredentialAvailable: false, CredentialName: null);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
        {
            _logger.LogWarning("Host tool probe {Tool} timed out after {Timeout} seconds.", probe.Executable, ProbeTimeout.TotalSeconds);
            return new HostCapability(probe.Key, Available: false, Version: null, CredentialAvailable: false, CredentialName: null);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Host tool probe {Tool} threw an exception.", probe.Executable);
            return new HostCapability(probe.Key, Available: false, Version: null, CredentialAvailable: false, CredentialName: null);
        }
    }

    private async Task<HostCapability> ProbeProviderCredentialAsync(ProviderCredentialProbe probe, CancellationToken cancellationToken)
    {
        var credential = await _credentialVault.GetAsync(probe.CredentialName, cancellationToken).ConfigureAwait(false);
        return new HostCapability(
            Key: probe.Key,
            Available: false,
            Version: null,
            CredentialAvailable: credential is not null,
            CredentialName: probe.CredentialName);
    }

    private static string? ExtractVersion(string standardOutput, Regex pattern)
    {
        if (string.IsNullOrWhiteSpace(standardOutput))
        {
            return null;
        }

        var match = pattern.Match(standardOutput);
        if (!match.Success)
        {
            return null;
        }

        if (match.Groups.TryGetValue("v", out var named) && named.Success)
        {
            return named.Value.Trim();
        }

        return match.Groups[1].Value.Trim();
    }

    private static string BuildCredentialName(string provider)
        => string.Concat(ProviderCredentialKeyPrefix, provider, ":", TokenCredentialSlot);
}
