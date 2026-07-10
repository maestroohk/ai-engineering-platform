using System.Text.RegularExpressions;
using Xunit;

namespace AiEng.Platform.ArchitectureTests.Boundaries;

public class PagesUseDesignSystemComponentsTests
{
    private static readonly string PagesRoot = LocatePagesRoot();

    private static readonly Regex LiteralButtonPattern = new(
        @"<\s*button\b",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex InlineStylePattern = new(
        @"\bstyle\s*=\s*""[^""]*""",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    [Fact]
    public void Pages_Must_Not_Use_Literal_Button_Elements()
    {
        var failures = ScanPages(LiteralButtonPattern, "literal <button>");

        Assert.True(
            failures.Count == 0,
            "Pages must not use literal <button> elements. Use <AppButton> or <AppIconButton>. " +
            "Offending files: " + string.Join(", ", failures));
    }

    [Fact]
    public void Pages_Must_Not_Use_Inline_Style_Attributes()
    {
        var failures = ScanPages(InlineStylePattern, "inline style=\"...\"");

        Assert.True(
            failures.Count == 0,
            "Pages must not use inline style attributes. Move the styling into the design system tokens or component CSS. " +
            "Offending files: " + string.Join(", ", failures));
    }

    private static List<string> ScanPages(Regex pattern, string label)
    {
        var results = new List<string>();
        if (!Directory.Exists(PagesRoot))
        {
            return results;
        }

        foreach (var path in Directory.EnumerateFiles(PagesRoot, "*.razor", SearchOption.AllDirectories))
        {
            var content = File.ReadAllText(path);
            var matches = pattern.Matches(content);
            if (matches.Count > 0)
            {
                var relative = Path.GetRelativePath(LocateRepoRoot(), path);
                results.Add($"{relative} ({label}: {matches.Count} match{(matches.Count == 1 ? string.Empty : "es")})");
            }
        }

        return results;
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

    private static string LocatePagesRoot()
    {
        var repoRoot = LocateRepoRoot();
        return Path.Combine(repoRoot, "src", "AiEng.Platform.App", "Components", "Pages");
    }
}
