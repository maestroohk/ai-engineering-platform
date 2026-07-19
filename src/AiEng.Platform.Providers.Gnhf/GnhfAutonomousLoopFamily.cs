using AiEng.Platform.Application.Providers;
using AiEng.Platform.Application.Providers.Families;

namespace AiEng.Platform.Providers.Gnhf;

public sealed class GnhfAutonomousLoopFamily : IAutonomousLoopProviderFamily
{
    private readonly IGnhfProbeRunner _probeRunner;
    private const string ProviderId = "gnhf";
    private const string DisplayName = "GNHF";

    public GnhfAutonomousLoopFamily(IGnhfProbeRunner probeRunner)
    {
        _probeRunner = probeRunner;
    }

    public async Task<IReadOnlyList<ProviderDescriptor>> ListProvidersAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var probe = await _probeRunner.ProbeAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        var metadata = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["entry_command"] = "gnhf <objective>",
            ["locked_commit"] = "fe202c4c92de3bc82b6319ed13bb35023d88410a",
            ["actual_executable_verified"] = probe.Health.State switch
            {
                GnhfHealthState.InstalledAndHealthy => "true",
                GnhfHealthState.VersionUnknown => "true",
                _ => "false"
            },
            ["bounded_workflow_verified"] = "false",
            ["execution_mode"] = probe.Resolution.Mode.ToString(),
            ["executable_resolution"] = probe.Resolution.ResolutionSource ?? "not-found",
            ["health_check_state"] = probe.Health.State.ToString(),
            ["health_check_timestamp"] = probe.Health.DetectedAt.ToString("O"),
            ["health_check_duration_ms"] = probe.Health.DurationMs.ToString()
        };

        if (probe.Resolution.ResolvedPath is not null)
        {
            metadata["executable_path"] = probe.Resolution.ResolvedPath;
        }
        else
        {
            metadata["executable_path"] = "(none)";
        }

        if (!string.IsNullOrWhiteSpace(probe.Health.HelpSummary))
        {
            metadata["help"] = probe.Health.HelpSummary!;
        }

        if (probe.Health.FailureReason is not null)
        {
            metadata["failure_reason"] = probe.Health.FailureReason;
        }

        if (probe.Health.ExitCode is int code)
        {
            metadata["exit_code"] = code.ToString();
        }

        var status = probe.Health.State switch
        {
            GnhfHealthState.InstalledAndHealthy => ProviderStatus.Available,
            GnhfHealthState.VersionUnknown => ProviderStatus.Available,
            _ => ProviderStatus.Unavailable
        };

        return new[]
        {
            new ProviderDescriptor(
                Id: ProviderId,
                DisplayName: DisplayName,
                Family: ProviderFamily.AutonomousLoop,
                Status: status,
                Version: probe.Health.Version,
                Metadata: metadata)
        };
    }
}
