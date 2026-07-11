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
            "Projects.razor must not reach into the project store, the file system, or deserialize JSON directly. " +
            "The IProjectService abstraction is the only allowed seam. " +
            "Forbidden tokens found: " + string.Join(", ", hits));
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
            "AppProjectList.razor must not reach into the project store, the file system, or deserialize JSON directly. " +
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
