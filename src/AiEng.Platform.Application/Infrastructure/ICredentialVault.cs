namespace AiEng.Platform.Application.Infrastructure;

public interface ICredentialVault
{
    Task<string?> GetAsync(string name, CancellationToken cancellationToken = default);

    Task SetAsync(string name, string secret, CancellationToken cancellationToken = default);

    Task DeleteAsync(string name, CancellationToken cancellationToken = default);
}
