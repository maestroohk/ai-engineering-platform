namespace AiEng.Platform.Application.Providers;

public interface IProviderRegistry
{
    Task<IReadOnlyList<ProviderDescriptor>> ListProvidersAsync(
        ProviderFamily family,
        CancellationToken cancellationToken = default);
}
