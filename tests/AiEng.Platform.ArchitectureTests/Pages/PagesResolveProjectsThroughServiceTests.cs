using Xunit;

namespace AiEng.Platform.ArchitectureTests;

public sealed class PagesResolveProjectsThroughServiceTests
{
    [Fact]
    public void Projects_page_resolves_projects_through_IProjectService()
    {
        var projectsPath = Path.Combine(LocateAppRoot(), "Components", "Pages", "Projects.razor");
        Assert.True(File.Exists(projectsPath), $"Projects.razor not found at {projectsPath}");

        var source = File.ReadAllText(projectsPath);

        Assert.True(
            source.Contains("IProjectService", StringComparison.Ordinal),
            "Projects.razor must reference IProjectService to resolve projects. " +
            "Pages must not read the project store directly; the service is the only allowed seam.");

        AssertNoForbiddenTokens(source, "Projects.razor");
    }

    [Fact]
    public void AppProjectList_resolves_projects_through_IProjectService()
    {
        var listPath = Path.Combine(LocateAppRoot(), "Components", "Projects", "AppProjectList.razor");
        Assert.True(File.Exists(listPath), $"AppProjectList.razor not found at {listPath}");

        var source = File.ReadAllText(listPath);

        Assert.True(
            source.Contains("@inject IProjectService", StringComparison.Ordinal),
            "AppProjectList.razor must @inject IProjectService. " +
            "The list is a data-owning component; direct store access is not allowed.");

        AssertNoForbiddenTokens(source, "AppProjectList.razor");
    }

    [Fact]
    public void RegisterProjectForm_resolves_projects_through_IProjectService()
    {
        var path = Path.Combine(LocateAppRoot(), "Components", "Projects", "RegisterProjectForm.razor");
        AssertSource(path, "RegisterProjectForm.razor");
    }

    [Fact]
    public void RenameProjectForm_resolves_projects_through_IProjectService()
    {
        var path = Path.Combine(LocateAppRoot(), "Components", "Projects", "RenameProjectForm.razor");
        AssertSource(path, "RenameProjectForm.razor");
    }

    [Fact]
    public void ConfirmUnregisterProject_resolves_projects_through_IProjectService()
    {
        var path = Path.Combine(LocateAppRoot(), "Components", "Projects", "ConfirmUnregisterProject.razor");
        AssertSource(path, "ConfirmUnregisterProject.razor");
    }

    [Fact]
    public void AppProjectCard_resolves_open_through_IProcessRunner()
    {
        var cardPath = Path.Combine(LocateAppRoot(), "Components", "Projects", "AppProjectCard.razor");
        Assert.True(File.Exists(cardPath), $"AppProjectCard.razor not found at {cardPath}");

        var source = File.ReadAllText(cardPath);

        Assert.True(
            source.Contains("@inject IProcessRunner", StringComparison.Ordinal),
            "AppProjectCard.razor must @inject IProcessRunner to perform the Open action. " +
            "The process boundary is the only allowed seam; direct Process.Start is forbidden.");

        var forbidden = new[]
        {
            "Process.Start",
            "ProcessStartInfo",
        };
        var hits = forbidden
            .Where(token => source.Contains(token, StringComparison.Ordinal))
            .ToArray();
        Assert.True(
            hits.Length == 0,
            "AppProjectCard.razor must not call Process.Start or construct ProcessStartInfo directly. " +
            "The IProcessRunner abstraction is the only allowed seam. " +
            "Forbidden tokens found: " + string.Join(", ", hits));
    }

    private static void AssertSource(string path, string label)
    {
        Assert.True(File.Exists(path), $"{label} not found at {path}");

        var source = File.ReadAllText(path);

        Assert.True(
            source.Contains("@inject IProjectService", StringComparison.Ordinal),
            $"{label} must @inject IProjectService. " +
            "The form is a data-mutating component; direct store access is not allowed.");

        AssertNoForbiddenTokens(source, label);
    }

    private static void AssertNoForbiddenTokens(string source, string label)
    {
        var forbidden = new[]
        {
            "InMemoryProjectStore",
            "Directory.GetCurrentDirectory",
            "File.ReadAllText",
            "JsonSerializer.Deserialize",
        };
        var hits = forbidden
            .Where(token => source.Contains(token, StringComparison.Ordinal))
            .ToArray();
        Assert.True(
            hits.Length == 0,
            $"{label} must not reach into the project store, the file system, or deserialize JSON directly. " +
            "The IProjectService abstraction is the only allowed seam. " +
            "Forbidden tokens found: " + string.Join(", ", hits));
    }

    private static string LocateRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "AiEng.Platform.slnx")))
            {
                return dir.FullName;
            }
            dir = dir.Parent;
        }
        throw new InvalidOperationException("Could not locate the repository root (AiEng.Platform.slnx not found).");
    }

    private static string LocateAppRoot() => Path.Combine(LocateRepoRoot(), "src", "AiEng.Platform.App");
}
