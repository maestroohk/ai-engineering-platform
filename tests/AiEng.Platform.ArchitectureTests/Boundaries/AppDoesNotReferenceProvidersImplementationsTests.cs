using System.Text.RegularExpressions;
using Xunit;

namespace AiEng.Platform.ArchitectureTests.Boundaries;

public class AppDoesNotReferenceProvidersImplementationsTests
{
    private static readonly string AppRoot = LocateAppRoot();

    private static readonly Regex ConcreteProviderUsingPattern = new(
        @"^\s*using\s+AiEng\.Platform\.Providers\.[A-Za-z0-9_]+\s*;",
        RegexOptions.Compiled | RegexOptions.Multiline);

    private static readonly Regex ConcreteProviderTypePattern = new(
        @"\bnew\s+[A-Za-z0-9_]*Provider\s*\(",
        RegexOptions.Compiled);

    [Fact]
    public void App_Must_Not_Reference_Concrete_Provider_Projects_Outside_Composition_Root()
    {
        var compositionRoot = Path.Combine(AppRoot, "Composition");
        var failures = new List<string>();

        if (!Directory.Exists(AppRoot))
        {
            return;
        }

        foreach (var path in Directory.EnumerateFiles(AppRoot, "*.cs", SearchOption.AllDirectories))
        {
            if (path.StartsWith(compositionRoot, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var relative = Path.GetRelativePath(LocateRepoRoot(), path);
            var content = File.ReadAllText(path);
            var usingMatches = ConcreteProviderUsingPattern.Matches(content);
            if (usingMatches.Count > 0)
            {
                failures.Add($"{relative} (concrete provider using: {usingMatches.Count} match{(usingMatches.Count == 1 ? string.Empty : "es")})");
            }

            var newMatches = ConcreteProviderTypePattern.Matches(content);
            if (newMatches.Count > 0)
            {
                failures.Add($"{relative} (concrete provider instantiation: {newMatches.Count} match{(newMatches.Count == 1 ? string.Empty : "es")})");
            }
        }

        Assert.True(
            failures.Count == 0,
            "App must not reference concrete provider projects outside the composition root (App/Composition/). " +
            "Offending files: " + string.Join(", ", failures));
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
        var repoRoot = LocateRepoRoot();
        return Path.Combine(repoRoot, "src", "AiEng.Platform.App");
    }
}
