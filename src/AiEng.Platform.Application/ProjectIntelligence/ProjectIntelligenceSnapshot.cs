namespace AiEng.Platform.Application.ProjectIntelligence;

public sealed record CurrentMilestoneInfo(
    string Id,
    string Title,
    string? Status,
    IReadOnlyList<string> Evidence);

public sealed record CurrentTaskInfo(
    string Id,
    string Title,
    string? Status,
    string? Owner,
    string? Plan,
    string? Notes);

public sealed record CurrentPlanInfo(
    string Path,
    string Title,
    string? Status);

public sealed record BuildStatusInfo(
    string? Status,
    string? TestSummary,
    string? LastBuildAt,
    string? Branch);

public sealed record PlanInfo(
    string Path,
    string Title,
    string Status,
    string? Milestone,
    string? Slice);

public sealed record CapabilityProgressInfo(
    int Verified,
    int Delivered,
    int InProgress,
    int Ready,
    int Planned,
    int Blocked,
    int Deferred,
    int NotStarted,
    int Total)
{
    public static CapabilityProgressInfo Empty { get; } = new(
        Verified: 0,
        Delivered: 0,
        InProgress: 0,
        Ready: 0,
        Planned: 0,
        Blocked: 0,
        Deferred: 0,
        NotStarted: 0,
        Total: 0);
}

public sealed record NextActionInfo(
    string Title,
    string? Plan,
    string? Task,
    string? Capability,
    string? Milestone,
    string Reason);

public sealed record ProjectIntelligenceSnapshot(
    string ProductName,
    string RepoRoot,
    string? Branch,
    string? LastCommitHash,
    string? LastCommitSubject,
    DateTimeOffset? LastCommitAt,
    string? LastStableCommit,
    string? BuildStatus,
    string? TestStatus,
    CurrentMilestoneInfo? CurrentMilestone,
    CurrentTaskInfo? CurrentTask,
    CurrentPlanInfo? CurrentPlan,
    IReadOnlyList<PlanInfo> PlansAwaitingApproval,
    IReadOnlyList<CapabilityProgressInfo> CapabilityProgress,
    CapabilityProgressInfo ProductProgress,
    IReadOnlyList<CurrentCapabilitySummary> ImplementedCapabilities,
    NextActionInfo? NextRecommendedAction,
    DateTimeOffset GeneratedAt)
{
    public static ProjectIntelligenceSnapshot Empty(string repoRoot) => new(
        ProductName: string.Empty,
        RepoRoot: repoRoot,
        Branch: null,
        LastCommitHash: null,
        LastCommitSubject: null,
        LastCommitAt: null,
        LastStableCommit: null,
        BuildStatus: null,
        TestStatus: null,
        CurrentMilestone: null,
        CurrentTask: null,
        CurrentPlan: null,
        PlansAwaitingApproval: Array.Empty<PlanInfo>(),
        CapabilityProgress: Array.Empty<CapabilityProgressInfo>(),
        ProductProgress: CapabilityProgressInfo.Empty,
        ImplementedCapabilities: Array.Empty<CurrentCapabilitySummary>(),
        NextRecommendedAction: null,
        GeneratedAt: DateTimeOffset.UtcNow);
}

public sealed record CurrentCapabilitySummary(
    string Id,
    string Title,
    string Status,
    string? Milestone);
