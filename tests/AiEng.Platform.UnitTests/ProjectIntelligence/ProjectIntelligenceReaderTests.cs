using System.Text.Json;
using AiEng.Platform.Application.ProjectIntelligence;
using Xunit;

namespace AiEng.Platform.UnitTests.ProjectIntelligence;

public class ProjectIntelligenceReaderTests
{
    [Fact]
    public async Task GetSnapshotAsync_Reads_Tasks_Milestones_Capabilities_From_State_Directory()
    {
        var dir = CreateTempRepo(out var stateDir);
        try
        {
            WriteFile(Path.Combine(stateDir, "tasks.json"), "{\"tasks\":[{\"id\":\"T-001\",\"title\":\"M2.1 task\",\"status\":\"Done\",\"milestone\":\"M2\",\"slice\":\"M2.1\"},{\"id\":\"T-002\",\"title\":\"M2.4 task\",\"status\":\"In Progress\",\"milestone\":\"M2\",\"slice\":\"M2.4\",\"owner\":\"session\",\"plan\":\".ai/plans/M2.4.md\"}]}");
            WriteFile(Path.Combine(stateDir, "milestones.json"), "{\"milestones\":[{\"id\":\"M2\",\"name\":\"Application Shell\",\"status\":\"Active\"},{\"id\":\"M3\",\"name\":\"Project Registration\",\"status\":\"Planned\"}]}");
            WriteFile(Path.Combine(stateDir, "capabilities.json"), "{\"capabilities\":[{\"id\":\"C-001\",\"title\":\"IProvider base\",\"status\":\"Done\",\"completion_status\":\"Verified\",\"delivered_by_milestone\":\"M4-C\"},{\"id\":\"C-002\",\"title\":\"IAgentRuntimeProvider\",\"status\":\"Accepted\",\"completion_status\":\"Planned\",\"delivered_by_milestone\":\"M4-D\"}]}");
            WriteFile(Path.Combine(stateDir, "session.json"), "{\"scope\":{\"branch\":\"feature/m2-4-test\"},\"current_understanding\":{\"last_stable_commit\":\"abc1234 feat(m2.4): test commit on feature/m2-4-test\",\"build_status\":\"passing\",\"test_status\":\"146 passed\"}}");
            WriteFile(Path.Combine(dir, "PRODUCT.md"), "# AI Engineering Platform\n\n> The product\n");
            WritePlanFile(dir, "M2.4-project-intelligence-dashboard.md", "M2.4 — Project Intelligence Dashboard", "Awaiting Approval", "M2.4");
            WritePlanFile(dir, "M3.1-foo.md", "M3.1 — Foo", "Awaiting Approval", "M3.1");

            var reader = new ProjectIntelligenceReader(
                new FileSystemStateFileReader(),
                new ProjectIntelligenceOptions { RepoRoot = dir });

            var snap = await reader.GetSnapshotAsync();

            Assert.Equal("AI Engineering Platform", snap.ProductName);
            Assert.Equal("feature/m2-4-test", snap.Branch);
            Assert.Equal("abc1234", snap.LastCommitHash);
            Assert.Equal("passing", snap.BuildStatus);
            Assert.Equal("146 passed", snap.TestStatus);
            Assert.NotNull(snap.CurrentMilestone);
            Assert.Equal("M2", snap.CurrentMilestone!.Id);
            Assert.Equal("Application Shell", snap.CurrentMilestone.Title);
            Assert.Equal("Active", snap.CurrentMilestone.Status);
            Assert.NotNull(snap.CurrentTask);
            Assert.Equal("T-002", snap.CurrentTask!.Id);
            Assert.Equal("M2.4 task", snap.CurrentTask.Title);
            Assert.Equal("In Progress", snap.CurrentTask.Status);
            Assert.Single(snap.ImplementedCapabilities);
            Assert.Contains(snap.ImplementedCapabilities, c => c.Id == "C-001");
            Assert.Equal(1, snap.ProductProgress.Verified);
            Assert.Equal(1, snap.ProductProgress.Planned);
            Assert.Equal(2, snap.ProductProgress.Total);
            Assert.Equal(2, snap.PlansAwaitingApproval.Count);
            Assert.Null(snap.NextRecommendedAction);
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }

    [Fact]
    public async Task GetSnapshotAsync_Returns_Empty_Snapshot_When_State_Directory_Is_Missing()
    {
        var dir = CreateTempRepo(out var stateDir);
        try
        {
            Directory.Delete(stateDir, recursive: true);
            var reader = new ProjectIntelligenceReader(
                new FileSystemStateFileReader(),
                new ProjectIntelligenceOptions { RepoRoot = dir });

            var snap = await reader.GetSnapshotAsync();

            Assert.Equal(dir, snap.RepoRoot);
            Assert.Null(snap.CurrentMilestone);
            Assert.Null(snap.CurrentTask);
            Assert.Null(snap.CurrentPlan);
            Assert.Empty(snap.ImplementedCapabilities);
            Assert.Empty(snap.PlansAwaitingApproval);
            Assert.Equal(CapabilityProgressInfo.Empty, snap.ProductProgress);
            Assert.Null(snap.NextRecommendedAction);
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }

    [Fact]
    public async Task GetSnapshotAsync_Handles_Malformed_Json_Gracefully()
    {
        var dir = CreateTempRepo(out var stateDir);
        try
        {
            WriteFile(Path.Combine(stateDir, "tasks.json"), "{this is not valid json");
            WriteFile(Path.Combine(stateDir, "milestones.json"), "{\"milestones\":[{\"id\":\"M2\",\"name\":\"Application Shell\",\"status\":\"Active\"}]}");
            var reader = new ProjectIntelligenceReader(
                new FileSystemStateFileReader(),
                new ProjectIntelligenceOptions { RepoRoot = dir });

            var snap = await reader.GetSnapshotAsync();

            Assert.Null(snap.CurrentTask);
            Assert.NotNull(snap.CurrentMilestone);
            Assert.Equal("M2", snap.CurrentMilestone!.Id);
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }

    [Fact]
    public async Task GetSnapshotAsync_Counts_Capabilities_By_Completion_Status()
    {
        var dir = CreateTempRepo(out var stateDir);
        try
        {
            WriteFile(Path.Combine(stateDir, "capabilities.json"), "{\"capabilities\":[" +
                "{\"id\":\"C-001\",\"status\":\"Done\",\"completion_status\":\"Verified\",\"delivered_by_milestone\":\"M1\"}," +
                "{\"id\":\"C-002\",\"status\":\"Done\",\"completion_status\":\"Verified\",\"delivered_by_milestone\":\"M1\"}," +
                "{\"id\":\"C-003\",\"status\":\"Accepted\",\"completion_status\":\"Ready\",\"delivered_by_milestone\":\"M2\"}," +
                "{\"id\":\"C-004\",\"status\":\"Accepted\",\"completion_status\":\"InProgress\",\"delivered_by_milestone\":\"M2\"}," +
                "{\"id\":\"C-005\",\"status\":\"Accepted\",\"completion_status\":\"Planned\",\"delivered_by_milestone\":\"M3\"}" +
                "]}");
            var reader = new ProjectIntelligenceReader(
                new FileSystemStateFileReader(),
                new ProjectIntelligenceOptions { RepoRoot = dir });

            var snap = await reader.GetSnapshotAsync();

            Assert.Equal(2, snap.ProductProgress.Verified);
            Assert.Equal(1, snap.ProductProgress.Ready);
            Assert.Equal(1, snap.ProductProgress.InProgress);
            Assert.Equal(1, snap.ProductProgress.Planned);
            Assert.Equal(5, snap.ProductProgress.Total);
            Assert.Equal(2, snap.ImplementedCapabilities.Count);
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }

    [Fact]
    public async Task GetSnapshotAsync_Lists_Plans_With_Awaiting_Approval_Status()
    {
        var dir = CreateTempRepo(out var stateDir);
        try
        {
            WritePlanFile(dir, "M2.5-empty-routes.md", "M2.5 — Empty Routes", "Awaiting Approval", "M2.5");
            WritePlanFile(dir, "M3.1-project-registration.md", "M3.1 — Project Registration", "Draft", "M3.1");
            WritePlanFile(dir, "M4-D-process-providers.md", "M4-D — First Concrete Process Providers", "Awaiting Approval", "M4-D");
            var reader = new ProjectIntelligenceReader(
                new FileSystemStateFileReader(),
                new ProjectIntelligenceOptions { RepoRoot = dir });

            var snap = await reader.GetSnapshotAsync();

            Assert.Equal(2, snap.PlansAwaitingApproval.Count);
            Assert.All(snap.PlansAwaitingApproval, p => Assert.Equal("Awaiting Approval", p.Status));
            Assert.Contains(snap.PlansAwaitingApproval, p => p.Slice == "M2.5");
            Assert.Contains(snap.PlansAwaitingApproval, p => p.Slice == "M4-D");
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }

    [Fact]
    public async Task GetSnapshotAsync_Resolves_Next_Action_From_First_Ready_Task()
    {
        var dir = CreateTempRepo(out var stateDir);
        try
        {
            WriteFile(Path.Combine(stateDir, "tasks.json"), "{\"tasks\":[" +
                "{\"id\":\"T-001\",\"title\":\"M2.1 task\",\"status\":\"Done\",\"milestone\":\"M2\"}," +
                "{\"id\":\"T-014\",\"title\":\"M2.4 task\",\"status\":\"In Progress\",\"milestone\":\"M2\",\"plan\":\".ai/plans/M2.4.md\"}," +
                "{\"id\":\"T-015\",\"title\":\"M2.5 task\",\"status\":\"Ready\",\"milestone\":\"M2\",\"plan\":\".ai/plans/M2.5.md\"}" +
                "]}");
            WriteFile(Path.Combine(stateDir, "capabilities.json"), "{\"capabilities\":[{\"id\":\"C-022\",\"title\":\"Dashboard\",\"next_task\":\"T-015\",\"delivered_by_milestone\":\"M2\"}]}");
            WritePlanFile(dir, "M2.5-empty-routes.md", "M2.5 — Empty Routes", "Awaiting Approval", "M2.5");
            var reader = new ProjectIntelligenceReader(
                new FileSystemStateFileReader(),
                new ProjectIntelligenceOptions { RepoRoot = dir });

            var snap = await reader.GetSnapshotAsync();

            Assert.NotNull(snap.NextRecommendedAction);
            Assert.Equal("M2.5 task", snap.NextRecommendedAction!.Title);
            Assert.Equal("T-015", snap.NextRecommendedAction.Task);
            Assert.Equal("C-022", snap.NextRecommendedAction.Capability);
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }

    private static string CreateTempRepo(out string stateDir)
    {
        var root = Path.Combine(Path.GetTempPath(), "aieng-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        stateDir = Path.Combine(root, ".ai", "state");
        Directory.CreateDirectory(stateDir);
        Directory.CreateDirectory(Path.Combine(root, ".ai", "plans"));
        return root;
    }

    private static void WriteFile(string path, string content)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content);
    }

    private static void WritePlanFile(string root, string fileName, string title, string status, string slice)
    {
        var content = $"# {title}\n\n> **Status: {status}**\n> **Milestone: {slice} — test.**\n\n---\n\nTest plan.\n";
        WriteFile(Path.Combine(root, ".ai", "plans", fileName), content);
    }
}
