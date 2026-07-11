using System.Threading.Tasks;
using AiEng.Platform.App.Components.Pages;
using AiEng.Platform.Application.Navigation;
using AiEng.Platform.Application.ProjectIntelligence;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AiEng.Platform.ComponentTests.Pages;

public class DashboardTests : BunitContext
{
    public DashboardTests()
    {
        Services.AddSingleton<INavigationRegistry>(new EmptyNavigationRegistry());
        JSInterop.Setup<string>("appTheme.current").SetResult("light");
        JSInterop.SetupVoid("appTheme.set", _ => true);
    }
    [Fact]
    public void Renders_App_Page_Header_With_Product_Name()
    {
        Services.AddSingleton<IProjectIntelligenceReader>(new StaticReader(SampleSnapshot));
        var cut = Render<Dashboard>();

        Assert.NotNull(cut.Find("header.app-page-header"));
        var heading = cut.Find("h1.app-page-header-title");
        Assert.Contains("AI Engineering Platform", heading.TextContent);
    }

    [Fact]
    public async Task Renders_Current_Milestone_And_Task_When_Snapshot_Is_Populated()
    {
        var reader = new StaticReader(SampleSnapshot);
        Services.AddSingleton<IProjectIntelligenceReader>(reader);
        var cut = Render<Dashboard>();

        cut.WaitForState(() => cut.FindAll(".app-dashboard-current").Count > 0);

        Assert.Contains("M2", cut.Markup);
        Assert.Contains("Application Shell", cut.Markup);
        Assert.Contains("M2.4 task", cut.Markup);
        Assert.Contains("feature/m2-4-test", cut.Markup);
        Assert.Contains("abc1234", cut.Markup);
    }

    [Fact]
    public async Task Renders_Capability_Progress_When_Snapshot_Has_Capabilities()
    {
        Services.AddSingleton<IProjectIntelligenceReader>(new StaticReader(SampleSnapshot));
        var cut = Render<Dashboard>();

        cut.WaitForState(() => cut.FindAll(".app-dashboard-progress-count").Count > 0);

        Assert.Contains("Verified", cut.Markup);
        Assert.Contains("In progress", cut.Markup);
        Assert.Contains("Planned", cut.Markup);
    }

    [Fact]
    public async Task Renders_Empty_States_For_M3_Sections_When_Snapshot_Has_No_Providers()
    {
        Services.AddSingleton<IProjectIntelligenceReader>(new StaticReader(SampleSnapshot));
        var cut = Render<Dashboard>();

        cut.WaitForState(() => cut.FindAll(".app-empty-state").Count >= 4);

        Assert.Contains("No running agents", cut.Markup);
        Assert.Contains("No queued tasks", cut.Markup);
        Assert.Contains("No recent quality gates", cut.Markup);
        Assert.Contains("No live commit log", cut.Markup);
        Assert.Contains("No providers registered", cut.Markup);
        Assert.Contains("No recent reviews", cut.Markup);
    }

    [Fact]
    public async Task Renders_Next_Recommended_Action_When_Snapshot_Has_One()
    {
        var snap = SampleSnapshot with
        {
            NextRecommendedAction = new NextActionInfo(
                Title: "Approve M2.5 plan",
                Plan: ".ai/plans/M2.5.md",
                Task: "T-015",
                Capability: "C-022",
                Milestone: "M2",
                Reason: "First Ready task in the structured state queue.")
        };
        Services.AddSingleton<IProjectIntelligenceReader>(new StaticReader(snap));
        var cut = Render<Dashboard>();

        cut.WaitForState(() => cut.Markup.Contains("Approve M2.5 plan"));

        Assert.Contains("Approve M2.5 plan", cut.Markup);
        Assert.Contains("T-015", cut.Markup);
    }

    [Fact]
    public async Task Renders_Implemented_Capabilities_List()
    {
        Services.AddSingleton<IProjectIntelligenceReader>(new StaticReader(SampleSnapshot));
        var cut = Render<Dashboard>();

        cut.WaitForState(() => cut.Markup.Contains("Implemented capabilities"));

        Assert.Contains("Implemented capabilities", cut.Markup);
        Assert.Contains("C-001", cut.Markup);
    }

    [Fact]
    public async Task Renders_Plans_Awaiting_Approval_List()
    {
        Services.AddSingleton<IProjectIntelligenceReader>(new StaticReader(SampleSnapshot));
        var cut = Render<Dashboard>();

        cut.WaitForState(() => cut.Markup.Contains("Plans awaiting approval"));

        Assert.Contains("Plans awaiting approval", cut.Markup);
        Assert.Contains("M2.4 plan", cut.Markup);
    }

    [Fact]
    public async Task Renders_Build_Status_When_Build_Status_Is_Present()
    {
        Services.AddSingleton<IProjectIntelligenceReader>(new StaticReader(SampleSnapshot));
        var cut = Render<Dashboard>();

        cut.WaitForState(() => cut.Markup.Contains("passing"));

        Assert.Contains("passing", cut.Markup);
        Assert.Contains("146 passed", cut.Markup);
    }

    [Fact]
    public async Task Renders_Empty_Build_Status_When_Build_Status_Is_Missing()
    {
        var snap = SampleSnapshot with { BuildStatus = null, TestStatus = null };
        Services.AddSingleton<IProjectIntelligenceReader>(new StaticReader(snap));
        var cut = Render<Dashboard>();

        cut.WaitForState(() => cut.Markup.Contains("No build information available"));

        Assert.Contains("No build information available", cut.Markup);
    }

    private static ProjectIntelligenceSnapshot SampleSnapshot { get; } = new(
        ProductName: "AI Engineering Platform",
        RepoRoot: "/tmp",
        Branch: "feature/m2-4-test",
        LastCommitHash: "abc1234",
        LastCommitSubject: "feat(m2.4): test commit on feature/m2-4-test",
        LastCommitAt: null,
        LastStableCommit: "abc1234 feat(m2.4): test commit on feature/m2-4-test",
        BuildStatus: "passing",
        TestStatus: "146 passed",
        CurrentMilestone: new CurrentMilestoneInfo(
            Id: "M2",
            Title: "Application Shell",
            Status: "Active",
            Evidence: new[] { "M2.1", "M2.2", "M2.3" }),
        CurrentTask: new CurrentTaskInfo(
            Id: "T-002",
            Title: "M2.4 task",
            Status: "In Progress",
            Owner: "session",
            Plan: ".ai/plans/M2.4.md",
            Notes: null),
        CurrentPlan: new CurrentPlanInfo(
            Path: ".ai/plans/M2.4.md",
            Title: "M2.4 plan",
            Status: "Awaiting Approval"),
        PlansAwaitingApproval: new[]
        {
            new PlanInfo(
                Path: ".ai/plans/M2.4.md",
                Title: "M2.4 plan",
                Status: "Awaiting Approval",
                Milestone: "M2.4",
                Slice: "M2.4")
        },
        CapabilityProgress: new[]
        {
            new CapabilityProgressInfo(
                Verified: 1, Delivered: 0, InProgress: 0, Ready: 0,
                Planned: 1, Blocked: 0, Deferred: 0, NotStarted: 0, Total: 2)
        },
        ProductProgress: new CapabilityProgressInfo(
            Verified: 1, Delivered: 0, InProgress: 0, Ready: 0,
            Planned: 1, Blocked: 0, Deferred: 0, NotStarted: 0, Total: 2),
        ImplementedCapabilities: new[]
        {
            new CurrentCapabilitySummary(
                Id: "C-001",
                Title: "IProvider base contract",
                Status: "Verified",
                Milestone: "M4-C")
        },
        NextRecommendedAction: null,
        GeneratedAt: System.DateTimeOffset.UtcNow);

    private sealed class StaticReader : IProjectIntelligenceReader
    {
        private readonly ProjectIntelligenceSnapshot _snapshot;

        public StaticReader(ProjectIntelligenceSnapshot snapshot) => _snapshot = snapshot;

        public Task<ProjectIntelligenceSnapshot> GetSnapshotAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(_snapshot);
    }

    private sealed class EmptyNavigationRegistry : INavigationRegistry
    {
        public IReadOnlyList<RouteMetadata> Routes { get; } = Array.Empty<RouteMetadata>();

        public RouteMetadata? FindByHref(string href) => null;

        public IReadOnlyList<RouteMetadata> ChildrenOf(string? parentHref) => Array.Empty<RouteMetadata>();
    }
}
