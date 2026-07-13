using System.Text.RegularExpressions;

namespace AiEng.Platform.Infrastructure.Capabilities;

internal sealed record HostToolProbe(
    string Key,
    string Executable,
    string Arguments,
    bool WindowsOnly,
    Regex VersionPattern);

internal sealed record ProviderCredentialProbe(
    string Key,
    string CredentialName);
