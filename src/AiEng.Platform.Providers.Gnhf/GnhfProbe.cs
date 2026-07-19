namespace AiEng.Platform.Providers.Gnhf;

public enum GnhfExecutionMode
{
    NotInstalled = 0,
    NativeWindows = 1,
    NativeLinux = 2,
    NativeMacOs = 3,
    Wsl = 4
}

public enum GnhfHealthState
{
    Unknown = 0,
    InstalledAndHealthy = 1,
    InstalledButUnhealthy = 2,
    NotInstalled = 3,
    TimedOut = 4,
    Cancelled = 5,
    VersionUnknown = 6
}

public sealed record GnhfExecutableResolution(
    string? ResolvedPath,
    GnhfExecutionMode Mode,
    string? ResolutionSource,
    string? FailureReason);

public sealed record GnhfHealthSnapshot(
    DateTimeOffset DetectedAt,
    long DurationMs,
    GnhfHealthState State,
    string? Version,
    string? HelpSummary,
    string? StandardOutput,
    string? StandardError,
    int? ExitCode,
    string? FailureReason);

public sealed record GnhfProbe(
    GnhfExecutableResolution Resolution,
    GnhfHealthSnapshot Health)
{
    public bool Available => Health.State == GnhfHealthState.InstalledAndHealthy;
}
