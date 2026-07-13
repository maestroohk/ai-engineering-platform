using AiEng.Platform.Application.Providers;
using AiEng.Platform.Application.Providers.Families;

namespace AiEng.Platform.UnitTests.Providers;

public sealed class FakeOrchestrationProviderFamily : IOrchestrationProviderFamily
{
    public List<IReadOnlyList<ProviderDescriptor>> QueuedResults { get; } = new();
    public int CallCount { get; private set; }
    public List<CancellationToken> ObservedTokens { get; } = new();

    public void QueueResult(IReadOnlyList<ProviderDescriptor> descriptors)
        => QueuedResults.Add(descriptors);

    public Task<IReadOnlyList<ProviderDescriptor>> ListProvidersAsync(CancellationToken cancellationToken = default)
    {
        CallCount++;
        ObservedTokens.Add(cancellationToken);
        if (QueuedResults.Count == 0)
        {
            return Task.FromResult<IReadOnlyList<ProviderDescriptor>>(Array.Empty<ProviderDescriptor>());
        }
        var next = QueuedResults[0];
        QueuedResults.RemoveAt(0);
        return Task.FromResult(next);
    }
}
