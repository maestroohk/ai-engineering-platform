using AiEng.Platform.Application.Capabilities;
using AiEng.Platform.Application.Providers;
using AiEng.Platform.Application.Providers.Families;
using Microsoft.Extensions.Logging;

namespace AiEng.Platform.Infrastructure.Providers;

public sealed class SystemProviderRegistry : IProviderRegistry
{
    private readonly IProviderFamily _git;
    private readonly IProviderFamily _agentRuntime;
    private readonly IProviderFamily _review;
    private readonly IProviderFamily _qualityGate;
    private readonly IProviderFamily _autonomousLoop;
    private readonly IProviderFamily _orchestration;
    private readonly IHostCapabilitiesService _capabilities;
    private readonly ILogger<SystemProviderRegistry> _logger;

    public SystemProviderRegistry(
        IGitProviderFamily git,
        IAgentRuntimeProviderFamily agentRuntime,
        IReviewProviderFamily review,
        IQualityGateProviderFamily qualityGate,
        IAutonomousLoopProviderFamily autonomousLoop,
        IOrchestrationProviderFamily orchestration,
        IHostCapabilitiesService capabilities,
        ILogger<SystemProviderRegistry> logger)
    {
        _git = git ?? throw new ArgumentNullException(nameof(git));
        _agentRuntime = agentRuntime ?? throw new ArgumentNullException(nameof(agentRuntime));
        _review = review ?? throw new ArgumentNullException(nameof(review));
        _qualityGate = qualityGate ?? throw new ArgumentNullException(nameof(qualityGate));
        _autonomousLoop = autonomousLoop ?? throw new ArgumentNullException(nameof(autonomousLoop));
        _orchestration = orchestration ?? throw new ArgumentNullException(nameof(orchestration));
        _capabilities = capabilities ?? throw new ArgumentNullException(nameof(capabilities));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IReadOnlyList<ProviderDescriptor>> ListProvidersAsync(
        ProviderFamily family,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var hostCapabilities = await _capabilities.DetectAsync(cancellationToken).ConfigureAwait(false);
        var capabilityKeys = hostCapabilities.Capabilities
            .Where(c => c.Available)
            .Select(c => c.Key)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var familyRegistry = family switch
        {
            ProviderFamily.Git => _git,
            ProviderFamily.AgentRuntime => _agentRuntime,
            ProviderFamily.Review => _review,
            ProviderFamily.QualityGate => _qualityGate,
            ProviderFamily.AutonomousLoop => _autonomousLoop,
            ProviderFamily.Orchestration => _orchestration,
            _ => throw new ArgumentOutOfRangeException(nameof(family), family, "Unknown provider family."),
        };

        var descriptors = await familyRegistry.ListProvidersAsync(cancellationToken).ConfigureAwait(false);
        var familyCapabilityKey = GetFamilyCapabilityKey(family);
        var familyCapabilityAvailable = string.IsNullOrEmpty(familyCapabilityKey)
            || capabilityKeys.Contains(familyCapabilityKey);

        var filtered = new List<ProviderDescriptor>(descriptors.Count);
        foreach (var descriptor in descriptors)
        {
            if (!familyCapabilityAvailable && descriptor.Status == ProviderStatus.Available)
            {
                filtered.Add(new ProviderDescriptor(
                    descriptor.Id,
                    descriptor.DisplayName,
                    descriptor.Family,
                    ProviderStatus.Unavailable,
                    descriptor.Version,
                    descriptor.Metadata));
            }
            else
            {
                filtered.Add(descriptor);
            }
        }

        _logger.LogInformation(
            "Provider registry lookup for family {Family}: {Total} total, {Filtered} after host capability filter.",
            family, descriptors.Count, filtered.Count);

        return filtered;
    }

    private static string GetFamilyCapabilityKey(ProviderFamily family) => family switch
    {
        ProviderFamily.Git => "git",
        ProviderFamily.AgentRuntime => "ollama",
        ProviderFamily.Review => "ollama",
        ProviderFamily.QualityGate => "powershell",
        ProviderFamily.AutonomousLoop => "ollama",
        ProviderFamily.Orchestration => "ollama",
        _ => string.Empty,
    };
}
