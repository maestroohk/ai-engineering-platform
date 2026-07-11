using Xunit;

namespace AiEng.Platform.ArchitectureTests;

public sealed class PagesResolveStateThroughReaderTests
{
    [Fact]
    public void Dashboard_page_resolves_state_through_IProjectIntelligenceReader()
    {
        var dashboardPath = Path.Combine(LocateAppRoot(), "Components", "Pages", "Dashboard.razor");
        Assert.True(File.Exists(dashboardPath), $"Dashboard.razor not found at {dashboardPath}");

        var source = File.ReadAllText(dashboardPath);

        Assert.True(
            source.Contains("IProjectIntelligenceReader", StringComparison.Ordinal),
            "Dashboard.razor must reference IProjectIntelligenceReader to resolve state. " +
            "Pages must not read structured state directly; the reader is the only allowed seam.");

        Assert.True(
            source.Contains("@inject IProjectIntelligenceReader", StringComparison.Ordinal),
            "Dashboard.razor must @inject IProjectIntelligenceReader. " +
            "Direct service-locator or static state access is not allowed.");

        var forbidden = new[]
        {
            "Directory.GetCurrentDirectory",
            "File.ReadAllText",
            "JsonSerializer.Deserialize",
        };
        var hits = forbidden
            .Where(token => source.Contains(token, StringComparison.Ordinal))
            .ToArray();
        Assert.True(
            hits.Length == 0,
            "Dashboard.razor must not reach into the file system or deserialize JSON directly. " +
            "The IProjectIntelligenceReader abstraction is the only allowed seam. " +
            "Forbidden tokens found: " + string.Join(", ", hits));
    }

    [Fact]
    public void No_page_in_Components_Pages_directly_reads_state_files()
    {
        var pagesRoot = Path.Combine(LocateAppRoot(), "Components", "Pages");
        Assert.True(Directory.Exists(pagesRoot), $"Pages root not found: {pagesRoot}");

        var forbidden = new[]
        {
            "Directory.GetCurrentDirectory",
            "File.ReadAllText",
            "JsonSerializer.Deserialize",
        };

        var failures = new List<string>();
        foreach (var path in Directory.EnumerateFiles(pagesRoot, "*.razor", SearchOption.TopDirectoryOnly))
        {
            var source = File.ReadAllText(path);
            var hits = forbidden
                .Where(token => source.Contains(token, StringComparison.Ordinal))
                .ToArray();
            if (hits.Length > 0)
            {
                failures.Add($"{Path.GetFileName(path)}: forbidden tokens ({string.Join(", ", hits)})");
            }
        }

        Assert.True(
            failures.Count == 0,
            "No Blazor page may read structured state from the file system or deserialize JSON directly. " +
            "Pages must consume typed snapshots through the IProjectIntelligenceReader abstraction. " +
            "Offending files: " + string.Join("; ", failures));
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
