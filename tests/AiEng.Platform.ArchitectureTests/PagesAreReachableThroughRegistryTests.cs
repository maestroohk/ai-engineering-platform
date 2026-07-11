using System.Reflection;
using AiEng.Platform.Application.Navigation;
using Xunit;

namespace AiEng.Platform.ArchitectureTests;

public sealed class PagesAreReachableThroughRegistryTests
{
    [Fact]
    public void Every_page_in_Components_Pages_has_a_RouteMetadata_attribute()
    {
        var failures = new List<string>();
        var appRoot = LocateAppRoot();
        var pagesRoot = Path.Combine(appRoot, "Components", "Pages");
        if (!Directory.Exists(pagesRoot))
        {
            Assert.Fail($"Pages root not found: {pagesRoot}");
        }

        var appAssembly = LoadAppAssembly();
        var registry = new RouteRegistry(appAssembly);
        var registryHrefs = new HashSet<string>(registry.Routes.Select(static r => r.Href), StringComparer.OrdinalIgnoreCase);

        foreach (var path in Directory.EnumerateFiles(pagesRoot, "*.razor", SearchOption.TopDirectoryOnly))
        {
            var typeName = Path.GetFileNameWithoutExtension(path);
            var pageType = appAssembly.GetType($"AiEng.Platform.App.Components.Pages.{typeName}", throwOnError: false);
            if (pageType is null)
            {
                failures.Add($"{typeName}: type not found in App assembly");
                continue;
            }

            var attribute = pageType.GetCustomAttribute<RouteMetadataAttribute>();
            if (attribute is null)
            {
                failures.Add($"{typeName}: missing [RouteMetadata] attribute");
                continue;
            }

            if (string.IsNullOrWhiteSpace(attribute.Href))
            {
                failures.Add($"{typeName}: [RouteMetadata] has empty Href");
            }
            else if (!registryHrefs.Contains(attribute.Href))
            {
                failures.Add($"{typeName}: Href '{attribute.Href}' is not in the registered routes");
            }
        }

        Assert.True(
            failures.Count == 0,
            "Every page in Components/Pages/ must have a [RouteMetadata] attribute whose Href is registered in the RouteRegistry. " +
            "Offending files: " + string.Join("; ", failures));
    }

    private static Assembly LoadAppAssembly()
    {
        var dir = new DirectoryInfo(LocateAppRoot());
        foreach (var dll in dir.EnumerateFiles("AiEng.Platform.App.dll", SearchOption.AllDirectories))
        {
            try
            {
                return Assembly.LoadFrom(dll.FullName);
            }
            catch
            {
                // try the next candidate
            }
        }
        throw new InvalidOperationException("Could not locate AiEng.Platform.App.dll from the App project root.");
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
