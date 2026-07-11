using System.Text.Json;

namespace AiEng.Platform.Application.ProjectIntelligence;

public interface IStateFileReader
{
    bool Exists(string path);
    string ReadAllText(string path);
    IReadOnlyList<string> EnumerateFiles(string directory, string searchPattern);
}

public sealed class FileSystemStateFileReader : IStateFileReader
{
    public bool Exists(string path) => File.Exists(path) || Directory.Exists(path);

    public string ReadAllText(string path) => File.ReadAllText(path);

    public IReadOnlyList<string> EnumerateFiles(string directory, string searchPattern) =>
        Directory.Exists(directory)
            ? Directory.EnumerateFiles(directory, searchPattern).ToArray()
            : Array.Empty<string>();
}

public sealed class ProjectIntelligenceOptions
{
    public string RepoRoot { get; set; } = string.Empty;

    public string StateDirectory { get; set; } = ".ai/state";
}

public sealed class ProjectIntelligenceReader : IProjectIntelligenceReader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };

    private readonly IStateFileReader _files;
    private readonly ProjectIntelligenceOptions _options;

    public ProjectIntelligenceReader(
        IStateFileReader files,
        ProjectIntelligenceOptions options)
    {
        _files = files ?? throw new ArgumentNullException(nameof(files));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<ProjectIntelligenceSnapshot> GetSnapshotAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var repoRoot = string.IsNullOrWhiteSpace(_options.RepoRoot)
            ? ResolveRepoRootFromCurrentDirectory()
            : _options.RepoRoot;
        var stateDir = Path.Combine(repoRoot, _options.StateDirectory);
        var tasks = TryRead<TasksFile>(Path.Combine(stateDir, "tasks.json"));
        var milestones = TryRead<MilestonesFile>(Path.Combine(stateDir, "milestones.json"));
        var capabilities = TryRead<CapabilitiesFile>(Path.Combine(stateDir, "capabilities.json"));
        var session = TryRead<SessionFile>(Path.Combine(stateDir, "session.json"));
        var productFile = Path.Combine(repoRoot, "PRODUCT.md");
        var productName = TryReadProductName(productFile);
        var plans = ListPlans(repoRoot);

        var currentMilestone = ResolveCurrentMilestone(milestones, session);
        var currentTask = ResolveCurrentTask(tasks);
        var currentPlan = ResolveCurrentPlan(plans, currentTask, currentMilestone);
        var (branch, lastCommit, lastSubject, lastAt, lastStable) = ResolveGitContext(session);
        var (buildStatus, testStatus) = ResolveBuildAndTestStatus(session);
        var awaiting = plans
            .Where(p => string.Equals(p.Status, "Awaiting Approval", StringComparison.OrdinalIgnoreCase))
            .ToArray();
        var perMilestoneProgress = ResolvePerMilestoneProgress(milestones, capabilities);
        var productProgress = ResolveProductProgress(capabilities);
        var implemented = ResolveImplementedCapabilities(capabilities);
        var nextAction = ResolveNextAction(tasks, plans, capabilities);

        return new ProjectIntelligenceSnapshot(
            ProductName: productName,
            RepoRoot: repoRoot,
            Branch: branch,
            LastCommitHash: lastCommit,
            LastCommitSubject: lastSubject,
            LastCommitAt: lastAt,
            LastStableCommit: lastStable,
            BuildStatus: buildStatus,
            TestStatus: testStatus,
            CurrentMilestone: currentMilestone,
            CurrentTask: currentTask,
            CurrentPlan: currentPlan,
            PlansAwaitingApproval: awaiting,
            CapabilityProgress: perMilestoneProgress,
            ProductProgress: productProgress,
            ImplementedCapabilities: implemented,
            NextRecommendedAction: nextAction,
            GeneratedAt: DateTimeOffset.UtcNow);
    }

    private static string ResolveRepoRootFromCurrentDirectory()
    {
        var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "AiEng.Platform.slnx")))
            {
                return dir.FullName;
            }
            dir = dir.Parent;
        }
        return Directory.GetCurrentDirectory();
    }

    private T? TryRead<T>(string path) where T : class
    {
        if (!_files.Exists(path)) { return null; }
        try
        {
            var text = _files.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(text)) { return null; }
            return JsonSerializer.Deserialize<T>(text, JsonOptions);
        }
        catch (JsonException)
        {
            return null;
        }
        catch (IOException)
        {
            return null;
        }
        catch (UnauthorizedAccessException)
        {
            return null;
        }
    }

    private static string TryReadProductName(string productPath)
    {
        if (!File.Exists(productPath)) { return string.Empty; }
        try
        {
            foreach (var line in File.ReadAllLines(productPath))
            {
                var trimmed = line.TrimStart('#').Trim();
                if (string.IsNullOrEmpty(trimmed)) { continue; }
                if (trimmed.StartsWith("---", StringComparison.Ordinal)) { continue; }
                if (trimmed.StartsWith("> ", StringComparison.Ordinal)) { continue; }
                if (trimmed.StartsWith("[", StringComparison.Ordinal)) { continue; }
                return trimmed;
            }
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
        return string.Empty;
    }

    private IReadOnlyList<PlanInfo> ListPlans(string repoRoot)
    {
        var plansDir = Path.Combine(repoRoot, ".ai", "plans");
        var results = new List<PlanInfo>();
        if (!_files.Exists(plansDir)) { return results; }
        foreach (var path in _files.EnumerateFiles(plansDir, "*.md"))
        {
            var fileName = Path.GetFileName(path);
            if (string.Equals(fileName, "README.md", StringComparison.OrdinalIgnoreCase)) { continue; }
            if (string.Equals(fileName, "master-delivery-plan.md", StringComparison.OrdinalIgnoreCase)) { continue; }
            var (title, status, milestone, slice) = ReadPlanFrontmatter(path);
            if (string.IsNullOrEmpty(title)) { continue; }
            var planPath = Path.Combine(".ai", "plans", fileName).Replace('\\', '/');
            results.Add(new PlanInfo(planPath, title, status ?? "Unknown", milestone, slice));
        }
        return results.OrderBy(p => p.Milestone ?? "z", StringComparer.OrdinalIgnoreCase)
            .ThenBy(p => p.Title, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static (string? Title, string? Status, string? Milestone, string? Slice) ReadPlanFrontmatter(string path)
    {
        string? title = null;
        string? status = null;
        string? milestone = null;
        string? slice = null;
        try
        {
            using var reader = new StreamReader(path);
            var lineNo = 0;
            string? line;
            while ((line = reader.ReadLine()) is not null && lineNo < 80)
            {
                lineNo++;
                if (lineNo == 1)
                {
                    if (!line.StartsWith("# ", StringComparison.Ordinal))
                    {
                        return (null, null, null, null);
                    }
                    title = line.Substring(2).Trim();
                    continue;
                }
                var t = line.Trim();
                if (t.StartsWith(">", StringComparison.Ordinal))
                {
                    t = t.Substring(1).TrimStart();
                }
                if (string.IsNullOrEmpty(t)) { continue; }
                if (t.StartsWith("**Status:", StringComparison.OrdinalIgnoreCase))
                {
                    status = ExtractBoldValue(t, "Status");
                    continue;
                }
                if (t.StartsWith("**Milestone:", StringComparison.OrdinalIgnoreCase))
                {
                    milestone = ExtractBoldValue(t, "Milestone");
                    slice = ExtractSliceFromMilestone(milestone);
                }
                if (t.StartsWith("---", StringComparison.Ordinal)) { break; }
            }
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
        return (title, status, milestone, slice);
    }

    private static string? ExtractBoldValue(string line, string key)
    {
        var prefix = $"**{key}:";
        if (!line.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) { return null; }
        var rest = line.Substring(prefix.Length).Trim();
        if (rest.EndsWith("**", StringComparison.Ordinal)) { rest = rest.Substring(0, rest.Length - 2).Trim(); }
        return rest;
    }

    private static string? ExtractSliceFromMilestone(string? milestone)
    {
        if (string.IsNullOrEmpty(milestone)) { return null; }
        var firstToken = milestone.Split(new[] { ' ', '\t' }, 2, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        if (string.IsNullOrEmpty(firstToken)) { return null; }
        return firstToken.Trim();
    }

    private static CurrentMilestoneInfo? ResolveCurrentMilestone(MilestonesFile? milestones, SessionFile? session)
    {
        if (milestones?.Milestones is null) { return null; }
        var active = milestones.Milestones
            .Where(m => string.Equals(m.Status, "Active", StringComparison.OrdinalIgnoreCase))
            .FirstOrDefault();
        if (active is null) { return null; }
        var evidence = new List<string>();
        if (active.Evidence?.ImplementationReports is { } reports)
        {
            evidence.AddRange(reports);
        }
        if (active.Evidence?.Handoffs is { } handoffs)
        {
            evidence.AddRange(handoffs);
        }
        if (active.Evidence?.Slices is { } slices)
        {
            evidence.AddRange(slices.Select(s => $"slice:{s}"));
        }
        return new CurrentMilestoneInfo(
            Id: active.Id,
            Title: active.Name ?? string.Empty,
            Status: active.Status,
            Evidence: evidence);
    }

    private static CurrentTaskInfo? ResolveCurrentTask(TasksFile? tasks)
    {
        if (tasks?.Tasks is null) { return null; }
        var inProgress = tasks.Tasks
            .Where(t => string.Equals(t.Status, "In Progress", StringComparison.OrdinalIgnoreCase))
            .FirstOrDefault();
        if (inProgress is null) { return null; }
        return new CurrentTaskInfo(
            Id: inProgress.Id,
            Title: inProgress.Title,
            Status: inProgress.Status,
            Owner: inProgress.Owner,
            Plan: inProgress.Plan,
            Notes: inProgress.Notes);
    }

    private static CurrentPlanInfo? ResolveCurrentPlan(
        IReadOnlyList<PlanInfo> plans,
        CurrentTaskInfo? currentTask,
        CurrentMilestoneInfo? currentMilestone)
    {
        if (currentTask?.Plan is { Length: > 0 } planPath)
        {
            var plan = plans.FirstOrDefault(p => string.Equals(p.Path, planPath, StringComparison.OrdinalIgnoreCase));
            if (plan is not null)
            {
                return new CurrentPlanInfo(Path: plan.Path, Title: plan.Title, Status: plan.Status);
            }
        }
        if (currentMilestone is null) { return null; }
        var matchingSlice = plans.FirstOrDefault(p => string.Equals(p.Slice, currentMilestone.Id, StringComparison.OrdinalIgnoreCase));
        return matchingSlice is null
            ? null
            : new CurrentPlanInfo(Path: matchingSlice.Path, Title: matchingSlice.Title, Status: matchingSlice.Status);
    }

    private static (string? Branch, string? Hash, string? Subject, DateTimeOffset? At, string? LastStable) ResolveGitContext(SessionFile? session)
    {
        var scope = session?.Scope;
        var branch = scope?.Branch;
        var lastStable = session?.CurrentUnderstanding?.LastStableCommit;
        if (lastStable is null) { return (branch, null, null, null, null); }
        var (hash, subject) = SplitStableCommit(lastStable);
        return (branch, hash, subject, session?.UpdatedAt is { } at ? (DateTimeOffset?)at : null, lastStable);
    }

    private static (string? Hash, string? Subject) SplitStableCommit(string lastStable)
    {
        if (string.IsNullOrWhiteSpace(lastStable)) { return (null, null); }
        var parts = lastStable.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) { return (null, null); }
        return (parts[0], parts.Length > 1 ? parts[1] : null);
    }

    private static (string? Build, string? Test) ResolveBuildAndTestStatus(SessionFile? session)
    {
        var cu = session?.CurrentUnderstanding;
        return (cu?.BuildStatus, cu?.TestStatus);
    }

    private static IReadOnlyList<CapabilityProgressInfo> ResolvePerMilestoneProgress(
        MilestonesFile? milestones,
        CapabilitiesFile? capabilities)
    {
        if (milestones?.Milestones is null || capabilities?.Capabilities is null)
        {
            return Array.Empty<CapabilityProgressInfo>();
        }
        var rows = new List<CapabilityProgressInfo>();
        foreach (var m in milestones.Milestones)
        {
            var count = CountByStatus(capabilities.Capabilities.Where(c => string.Equals(c.DeliveredByMilestone, m.Id, StringComparison.OrdinalIgnoreCase)));
            if (count.Total == 0) { continue; }
            rows.Add(count);
        }
        return rows;
    }

    private static CapabilityProgressInfo ResolveProductProgress(CapabilitiesFile? capabilities)
    {
        if (capabilities?.Capabilities is null) { return CapabilityProgressInfo.Empty; }
        return CountByStatus(capabilities.Capabilities);
    }

    private static CapabilityProgressInfo CountByStatus(IEnumerable<CapabilityRecord> caps)
    {
        var list = caps.ToArray();
        return new CapabilityProgressInfo(
            Verified: list.Count(c => string.Equals(c.CompletionStatus, "Verified", StringComparison.OrdinalIgnoreCase)),
            Delivered: list.Count(c => string.Equals(c.CompletionStatus, "Delivered", StringComparison.OrdinalIgnoreCase)),
            InProgress: list.Count(c => string.Equals(c.CompletionStatus, "InProgress", StringComparison.OrdinalIgnoreCase)),
            Ready: list.Count(c => string.Equals(c.CompletionStatus, "Ready", StringComparison.OrdinalIgnoreCase)),
            Planned: list.Count(c => string.Equals(c.CompletionStatus, "Planned", StringComparison.OrdinalIgnoreCase)),
            Blocked: list.Count(c => string.Equals(c.CompletionStatus, "Blocked", StringComparison.OrdinalIgnoreCase)),
            Deferred: list.Count(c => string.Equals(c.CompletionStatus, "Deferred", StringComparison.OrdinalIgnoreCase)),
            NotStarted: list.Count(c => string.Equals(c.CompletionStatus, "NotStarted", StringComparison.OrdinalIgnoreCase)),
            Total: list.Length);
    }

    private static IReadOnlyList<CurrentCapabilitySummary> ResolveImplementedCapabilities(CapabilitiesFile? capabilities)
    {
        if (capabilities?.Capabilities is null) { return Array.Empty<CurrentCapabilitySummary>(); }
        return capabilities.Capabilities
            .Where(c => string.Equals(c.CompletionStatus, "Verified", StringComparison.OrdinalIgnoreCase)
                || string.Equals(c.CompletionStatus, "Delivered", StringComparison.OrdinalIgnoreCase))
            .OrderBy(c => c.Id, StringComparer.OrdinalIgnoreCase)
            .Select(c => new CurrentCapabilitySummary(c.Id, c.Title ?? string.Empty, c.CompletionStatus ?? string.Empty, c.DeliveredByMilestone))
            .ToArray();
    }

    private static NextActionInfo? ResolveNextAction(
        TasksFile? tasks,
        IReadOnlyList<PlanInfo> plans,
        CapabilitiesFile? capabilities)
    {
        if (tasks?.Tasks is null) { return null; }
        var nextTask = tasks.Tasks
            .Where(t => string.Equals(t.Status, "Ready", StringComparison.OrdinalIgnoreCase))
            .OrderBy(t => t.Id, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault();
        if (nextTask is null) { return null; }
        var planTitle = plans.FirstOrDefault(p => string.Equals(p.Path, nextTask.Plan, StringComparison.OrdinalIgnoreCase))?.Title;
        var capability = capabilities?.Capabilities?
            .FirstOrDefault(c => c.NextTask == nextTask.Id);
        return new NextActionInfo(
            Title: nextTask.Title,
            Plan: planTitle ?? nextTask.Plan,
            Task: nextTask.Id,
            Capability: capability?.Id,
            Milestone: nextTask.Milestone,
            Reason: "Highest-priority Ready task in the structured state queue.");
    }

    private sealed record TasksFile(
        string? SchemaVersion,
        IReadOnlyList<TaskRecord>? Tasks);

    private sealed record TaskRecord(
        string Id,
        string Title,
        string? Status,
        string? Milestone,
        string? Slice,
        string? Owner,
        string? Plan,
        string? Notes);

    private sealed record MilestonesFile(
        string? SchemaVersion,
        IReadOnlyList<MilestoneRecord>? Milestones);

    private sealed record MilestoneRecord(
        string Id,
        string? Name,
        string? Status,
        EvidenceRecord? Evidence);

    private sealed record EvidenceRecord(
        IReadOnlyList<string>? Slices,
        IReadOnlyList<string>? ImplementationReports,
        IReadOnlyList<string>? Handoffs);

    private sealed record CapabilitiesFile(
        string? SchemaVersion,
        IReadOnlyList<CapabilityRecord>? Capabilities);

    private sealed record CapabilityRecord(
        string Id,
        string? Title,
        string? Status,
        string? CompletionStatus,
        string? DeliveredByMilestone,
        string? NextTask);

    private sealed record SessionFile(
        string? SessionId,
        ScopeRecord? Scope,
        CurrentUnderstandingRecord? CurrentUnderstanding,
        DateTimeOffset? UpdatedAt);

    private sealed record ScopeRecord(string? Branch);

    private sealed record CurrentUnderstandingRecord(
        string? LastStableCommit,
        string? BuildStatus,
        string? TestStatus);
}
