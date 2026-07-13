using Xunit;

namespace AiEng.Platform.ArchitectureTests.Capabilities;

public sealed class Capabilities_Resolved_Through_Service
{
    [Fact]
    public void Diagnostics_page_resolves_capabilities_through_IHostCapabilitiesService()
    {
        var diagnosticsPath = Path.Combine(LocateAppRoot(), "Components", "Pages", "Diagnostics.razor");
        Assert.True(File.Exists(diagnosticsPath), $"Diagnostics.razor not found at {diagnosticsPath}");

        var source = File.ReadAllText(diagnosticsPath);

        Assert.True(
            source.Contains("IHostCapabilitiesService", StringComparison.Ordinal),
            "Diagnostics.razor must reference IHostCapabilitiesService to resolve host capabilities. " +
            "Pages must not probe the host directly; the IHostCapabilitiesService abstraction is the only allowed seam.");

        Assert.True(
            source.Contains("@inject IHostCapabilitiesService", StringComparison.Ordinal),
            "Diagnostics.razor must @inject IHostCapabilitiesService. " +
            "Direct service-locator or static state access is not allowed.");

        var forbidden = new[]
        {
            "RunToCompletionAsync",
            "ICredentialVault",
            "new SystemHostCapabilitiesService"
        };
        var hits = forbidden
            .Where(token => source.Contains(token, StringComparison.Ordinal))
            .ToArray();
        Assert.True(
            hits.Length == 0,
            "Diagnostics.razor must not reach into IProcessRunner, ICredentialVault, or the concrete " +
            "SystemHostCapabilitiesService. The IHostCapabilitiesService abstraction is the only allowed seam. " +
            "Forbidden tokens found: " + string.Join(", ", hits));
    }

    [Fact]
    public void Diagnostics_folder_does_not_reference_process_or_credential_boundary_directly()
    {
        var diagnosticsRoot = LocateDiagnosticsRoot();
        Assert.True(Directory.Exists(diagnosticsRoot), $"Diagnostics root not found: {diagnosticsRoot}");

        var forbidden = new[]
        {
            "RunToCompletionAsync",
            "ICredentialVault",
            "new SystemHostCapabilitiesService"
        };

        var failures = new List<string>();
        foreach (var path in Directory.EnumerateFiles(diagnosticsRoot, "*.razor*", SearchOption.AllDirectories))
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
            "No Blazor file under Components/Diagnostics/ may reach into IProcessRunner, ICredentialVault, " +
            "or the concrete SystemHostCapabilitiesService. The IHostCapabilitiesService abstraction is the " +
            "only allowed seam. Offending files: " + string.Join("; ", failures));
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

    private static string LocateDiagnosticsRoot() => Path.Combine(LocateAppRoot(), "Components", "Diagnostics");
}
