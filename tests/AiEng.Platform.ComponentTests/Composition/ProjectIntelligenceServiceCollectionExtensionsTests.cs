using AiEng.Platform.App.Composition;
using AiEng.Platform.Application.ProjectIntelligence;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AiEng.Platform.ComponentTests.Composition;

public class ProjectIntelligenceServiceCollectionExtensionsTests
{
    [Fact]
    public void AddProjectIntelligence_Registers_IProjectIntelligenceReader()
    {
        var services = new ServiceCollection();
        services.AddProjectIntelligence();

        using var provider = services.BuildServiceProvider();
        var reader = provider.GetService<IProjectIntelligenceReader>();

        Assert.NotNull(reader);
        Assert.IsType<ProjectIntelligenceReader>(reader);
    }

    [Fact]
    public void AddProjectIntelligence_Registers_Reader_As_Singleton()
    {
        var services = new ServiceCollection();
        services.AddProjectIntelligence();

        using var provider = services.BuildServiceProvider();
        var first = provider.GetRequiredService<IProjectIntelligenceReader>();
        var second = provider.GetRequiredService<IProjectIntelligenceReader>();

        Assert.Same(first, second);
    }

    [Fact]
    public void AddProjectIntelligence_Resolves_Repo_Root_Default_When_Not_Configured()
    {
        var services = new ServiceCollection();
        services.AddProjectIntelligence();

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<ProjectIntelligenceOptions>();

        Assert.False(string.IsNullOrEmpty(options.RepoRoot));
        Assert.True(System.IO.Directory.Exists(options.RepoRoot));
    }
}
