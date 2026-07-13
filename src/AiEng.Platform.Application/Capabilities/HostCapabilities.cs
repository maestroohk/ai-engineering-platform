namespace AiEng.Platform.Application.Capabilities;

public sealed record class HostCapabilities(
    IReadOnlyList<HostCapability> Capabilities,
    DateTimeOffset DetectedAt);

public sealed record class HostCapability(
    string Key,
    bool Available,
    string? Version,
    bool CredentialAvailable,
    string? CredentialName);
