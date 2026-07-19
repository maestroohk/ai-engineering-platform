namespace AiEng.Platform.Providers.Gnhf;

public sealed record class GnhfProbe(
    bool Available,
    string? Version,
    string? HelpSummary,
    string? FailureReason);
