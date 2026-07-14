using Xunit;

namespace AiEng.Platform.ArchitectureTests.Providers;

public sealed class Providers_Resolve_Through_Registry
{
    [Fact]
    public void Providers_page_resolves_providers_through_IProviderRegistry()
    {
        var providersPath = Path.Combine(LocateAppRoot(), "Components", "Pages", "Providers.razor");
        Assert.True(File.Exists(providersPath), $"Providers.razor not found at {providersPath}");

        var source = File.ReadAllText(providersPath);

        Assert.True(
            source.Contains("IProviderRegistry", StringComparison.Ordinal),
            "Providers.razor must reference IProviderRegistry to resolve providers. " +
            "Pages must not look up providers through a family registry or process boundary directly; " +
            "the IProviderRegistry abstraction is the only allowed seam.");

        Assert.True(
            source.Contains("@inject IProviderRegistry", StringComparison.Ordinal),
            "Providers.razor must @inject IProviderRegistry. " +
            "Direct service-locator or static state access is not allowed.");

        var forbidden = new[]
        {
            "RunToCompletionAsync",
            "ICredentialVault",
            "new SystemProviderRegistry"
        };
        var hits = forbidden
            .Where(token => source.Contains(token, StringComparison.Ordinal))
            .ToArray();
        Assert.True(
            hits.Length == 0,
            "Providers.razor must not reach into IProcessRunner, ICredentialVault, or the concrete " +
            "SystemProviderRegistry. The IProviderRegistry abstraction is the only allowed seam for " +
            "provider lookup; the host capability lookup (for the metadata context) is allowed via the " +
            "IHostCapabilitiesService abstraction, mirroring the M4-B.3 Diagnostics.razor pattern. " +
            "Forbidden tokens found: " + string.Join(", ", hits));
    }

    [Fact]
    public void Providers_folder_does_not_reference_process_or_credential_boundary_directly()
    {
        var providersRoot = LocateProvidersRoot();
        Assert.True(Directory.Exists(providersRoot), $"Providers root not found: {providersRoot}");

        var forbidden = new[]
        {
            "RunToCompletionAsync",
            "ICredentialVault",
            "new SystemProviderRegistry"
        };

        var failures = new List<string>();
        foreach (var path in Directory.EnumerateFiles(providersRoot, "*.razor*", SearchOption.AllDirectories))
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
            "No Blazor file under Components/Providers/ may reach into IProcessRunner, ICredentialVault, " +
            "or the concrete SystemProviderRegistry. The IProviderRegistry abstraction is the only " +
            "allowed seam for provider lookup. " +
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

    private static string LocateProvidersRoot() => Path.Combine(LocateAppRoot(), "Components", "Providers");
}
