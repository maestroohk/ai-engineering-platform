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

        var probe = await _probeRunner.ProbeAsync(cancellationToken).ConfigureAwait(false);

        var metadata = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["executable"] = _probeRunner is GnhfProcessProbeRunner process
                ? process.Executable
                : "gnhf",
            ["entry_command"] = "gnhf <objective>",
            ["locked_commit"] = "fe202c4c92de3bc82b6319ed13bb35023d88410a"
        };

        if (!probe.Available)
        {
            metadata["failure_reason"] = probe.FailureReason ?? "unknown";
            return new[]
            {
                new ProviderDescriptor(
                    Id: ProviderId,
                    DisplayName: DisplayName,
                    Family: ProviderFamily.AutonomousLoop,
                    Status: ProviderStatus.Unavailable,
                    Version: null,
                    Metadata: metadata)
            };
        }

        if (!string.IsNullOrWhiteSpace(probe.HelpSummary))
        {
            metadata["help"] = probe.HelpSummary!;
        }

        return new[]
        {
            new ProviderDescriptor(
                Id: ProviderId,
                DisplayName: DisplayName,
                Family: ProviderFamily.AutonomousLoop,
                Status: ProviderStatus.Available,
                Version: probe.Version,
                Metadata: metadata)
        };
    }
}
