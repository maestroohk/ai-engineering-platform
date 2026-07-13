namespace AiEng.Platform.Application.Providers;

public sealed record class ProviderDescriptor(
    string Id,
    string DisplayName,
    ProviderFamily Family,
    ProviderStatus Status,
    string? Version,
    IReadOnlyDictionary<string, string> Metadata);
