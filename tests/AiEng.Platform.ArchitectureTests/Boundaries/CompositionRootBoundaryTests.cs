using System.Text.RegularExpressions;
using Xunit;

namespace AiEng.Platform.ArchitectureTests.Boundaries;

public class CompositionRootBoundaryTests
{
    private const string ActivationMilestone = "M4-D";
    private const string AdrReference = "ADR-016";

    [Fact(Skip = "Registered but disabled per ADR-016. Activates in M4-D when the first concrete provider implementation project (Providers.<X>) lands. In M1, no Providers.<X> project exists, so the rule is satisfied by construction.")]
    public void Only_CompositionRoot_MayReference_ConcreteProviders()
    {
        var compositionRoot = LocateAppCompositionRoot();
        var appRoot = LocateAppRoot();
        var failures = new List<string>();
        var concreteProviderUsingPattern = new Regex(
            @"^\s*using\s+AiEng\.Platform\.Providers\.[A-Za-z0-9_]+",
            RegexOptions.Compiled | RegexOptions.Multiline);

        if (!Directory.Exists(appRoot))
        {
            return;
        }

        foreach (var path in Directory.EnumerateFiles(appRoot, "*.cs", SearchOption.AllDirectories))
        {
            if (path.StartsWith(compositionRoot, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }
            var content = File.ReadAllText(path);
            if (concreteProviderUsingPattern.IsMatch(content))
            {
                var relative = Path.GetRelativePath(LocateRepoRoot(), path);
                failures.Add(relative);
            }
        }

        Assert.True(
            failures.Count == 0,
            "Only the composition root (App/Composition/) may reference concrete Providers.<X> projects. " +
            "Offending files: " + string.Join(", ", failures));
    }

    [Fact(Skip = "Registered but disabled per ADR-016. Activates in M4-D when the first concrete provider implementation project (Providers.<X>) lands.")]
    public void Pages_DoNotReference_ConcreteProviders()
    {
        var pagesRoot = Path.Combine(LocateAppRoot(), "Components", "Pages");
        var compositionRoot = LocateAppCompositionRoot();
        var failures = ScanForConcreteProviderReferences(pagesRoot, compositionRoot);

        Assert.True(
            failures.Count == 0,
            "Pages must not reference concrete provider implementations. " +
            "Offending files: " + string.Join(", ", failures));
    }

    [Fact(Skip = "Registered but disabled per ADR-016. Activates in M4-D when the first concrete provider implementation project (Providers.<X>) lands.")]
    public void Application_DoesNotReference_ConcreteProviders()
    {
        var applicationRoot = LocateProjectRoot("AiEng.Platform.Application");
        var compositionRoot = LocateAppCompositionRoot();
        var failures = ScanForConcreteProviderReferences(applicationRoot, compositionRoot);

        Assert.True(
            failures.Count == 0,
            "Application must not reference concrete provider implementations. " +
            "Offending files: " + string.Join(", ", failures));
    }

    [Fact(Skip = "Registered but disabled per ADR-016. Activates in M4-D when the first concrete provider implementation project (Providers.<X>) lands.")]
    public void Components_DoNotInject_ConcreteProviders()
    {
        var componentsRoot = Path.Combine(LocateAppRoot(), "Components");
        var compositionRoot = LocateAppCompositionRoot();
        var concreteProviderInjectionPattern = new Regex(
            @"\[FromServices\]\s*[A-Za-z0-9_]*Provider\b|\b[A-Za-z0-9_]*Provider\s+[A-Za-z0-9_]+\s*\{",
            RegexOptions.Compiled);
        var failures = new List<string>();

        if (!Directory.Exists(componentsRoot))
        {
            return;
        }

        foreach (var path in Directory.EnumerateFiles(componentsRoot, "*.razor.cs", SearchOption.AllDirectories))
        {
            if (path.StartsWith(compositionRoot, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }
            var content = File.ReadAllText(path);
            if (concreteProviderInjectionPattern.IsMatch(content))
            {
                var relative = Path.GetRelativePath(LocateRepoRoot(), path);
                failures.Add(relative);
            }
        }

        Assert.True(
            failures.Count == 0,
            "Blazor components must not inject concrete provider implementations. Inject the family contract instead. " +
            "Offending files: " + string.Join(", ", failures));
    }

    private static List<string> ScanForConcreteProviderReferences(string root, string compositionRoot)
    {
        var failures = new List<string>();
        if (!Directory.Exists(root))
        {
            return failures;
        }
        var pattern = new Regex(
            @"using\s+AiEng\.Platform\.Providers\.[A-Za-z0-9_]+|new\s+[A-Za-z0-9_]*Provider\s*\(",
            RegexOptions.Compiled);

        foreach (var path in Directory.EnumerateFiles(root, "*.cs", SearchOption.AllDirectories))
        {
            if (!string.IsNullOrEmpty(compositionRoot) && path.StartsWith(compositionRoot, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }
            var content = File.ReadAllText(path);
            if (pattern.IsMatch(content))
            {
                var relative = Path.GetRelativePath(LocateRepoRoot(), path);
                failures.Add(relative);
            }
        }
        return failures;
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

    private static string LocateAppRoot()
    {
        return Path.Combine(LocateRepoRoot(), "src", "AiEng.Platform.App");
    }

    private static string LocateAppCompositionRoot()
    {
        return Path.Combine(LocateAppRoot(), "Composition");
    }

    private static string LocateProjectRoot(string projectName)
    {
        return Path.Combine(LocateRepoRoot(), "src", projectName);
    }
}
