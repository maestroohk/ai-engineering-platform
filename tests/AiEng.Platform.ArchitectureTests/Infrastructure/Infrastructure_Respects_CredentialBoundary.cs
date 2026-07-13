using System.Text.RegularExpressions;
using Xunit;

namespace AiEng.Platform.ArchitectureTests.Infrastructure;

public class Infrastructure_Respects_CredentialBoundary
{
    private const string ActivationMilestone = "M4-D";
    private const string AdrReference = "ADR-016";

    [Fact(Skip = "Registered but disabled per ADR-016. Activates in M4-D when the first concrete provider implementation project (Providers.<X>) lands. The M4-A.1 slice ships the credential boundary; this scan asserts the Infrastructure project is the only one that calls Windows Credential Manager.")]
    public void Only_Infrastructure_MayReference_Windows_Credential_Manager_APIs()
    {
        var repoRoot = LocateRepoRoot();
        var infrastructureRoot = Path.Combine(repoRoot, "src", "AiEng.Platform.Infrastructure");
        var credentialReferencePattern = new Regex(
            @"\bCredRead\b|\bCredWrite\b|\bCredDelete\b|\badvapi32\.dll\b",
            RegexOptions.Compiled);

        var failures = new List<string>();
        foreach (var csproj in Directory.EnumerateFiles(repoRoot, "*.csproj", SearchOption.AllDirectories))
        {
            if (csproj.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ||
                csproj.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }
            var projectRoot = Path.GetDirectoryName(csproj)!;
            if (Path.GetFullPath(projectRoot) == Path.GetFullPath(infrastructureRoot))
            {
                continue;
            }
            foreach (var path in Directory.EnumerateFiles(projectRoot, "*.cs", SearchOption.AllDirectories))
            {
                if (path.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ||
                    path.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                var content = File.ReadAllText(path);
                if (credentialReferencePattern.IsMatch(content))
                {
                    var relative = Path.GetRelativePath(repoRoot, path);
                    failures.Add(relative);
                }
            }
        }

        Assert.True(
            failures.Count == 0,
            "Only the Infrastructure project (src/AiEng.Platform.Infrastructure) may reference Windows Credential Manager APIs. " +
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
}
