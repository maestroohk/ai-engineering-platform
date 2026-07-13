namespace AiEng.Platform.Application.Providers.Families;

public interface IProviderFamily
{
    Task<IReadOnlyList<ProviderDescriptor>> ListProvidersAsync(CancellationToken cancellationToken = default);
}
