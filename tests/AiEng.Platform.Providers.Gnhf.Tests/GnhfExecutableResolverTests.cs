using AiEng.Platform.Application.Infrastructure;
using AiEng.Platform.Providers.Gnhf;
using Xunit;

namespace AiEng.Platform.Providers.Gnhf.Tests;

public sealed class GnhfExecutableResolverTests
{
    [Fact]
    public void Resolve_returns_configured_path_when_file_exists()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".cmd");
        File.WriteAllText(tempFile, "@echo off");
        try
        {
            var resolver = new GnhfExecutableResolver(
                new AlwaysFailingProcessRunner(),
                new TestPlatformInfo(isWindows: true));

            var resolution = resolver.Resolve(tempFile);

            Assert.Equal(tempFile, resolution.ResolvedPath);
            Assert.Equal("configured", resolution.ResolutionSource);
            Assert.Equal(GnhfExecutionMode.NativeWindows, resolution.Mode);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void Resolve_returns_not_installed_when_configured_path_missing()
    {
        var resolver = new GnhfExecutableResolver(
            new AlwaysFailingProcessRunner(),
            new TestPlatformInfo(isWindows: false));

        var resolution = resolver.Resolve("C:/this/does/not/exist/gnhf");

        Assert.Null(resolution.ResolvedPath);
        Assert.Equal(GnhfExecutionMode.NotInstalled, resolution.Mode);
        Assert.Equal("configured", resolution.ResolutionSource);
        Assert.Contains("not found", resolution.FailureReason, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Resolve_windows_candidate_names_includes_cmd_and_exe()
    {
        Assert.Equal(new[] { "gnhf.cmd", "gnhf.exe", "gnhf" }, GnhfExecutableResolver.CandidateNamesForTests(new TestPlatformInfo(isWindows: true)));
    }

    [Fact]
    public void Resolve_non_windows_candidate_names_is_only_gnhf()
    {
        Assert.Equal(new[] { "gnhf" }, GnhfExecutableResolver.CandidateNamesForTests(new TestPlatformInfo(isWindows: false)));
    }
}
