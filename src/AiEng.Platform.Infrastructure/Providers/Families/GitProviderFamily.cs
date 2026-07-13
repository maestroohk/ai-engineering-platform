using AiEng.Platform.Application.Providers;
using AiEng.Platform.Application.Providers.Families;

namespace AiEng.Platform.Infrastructure.Providers.Families;

public sealed class GitProviderFamily : IGitProviderFamily
{
    public Task<IReadOnlyList<ProviderDescriptor>> ListProvidersAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IReadOnlyList<ProviderDescriptor>>(Array.Empty<ProviderDescriptor>());
    }
}
