using AiEng.Platform.Application.ProjectIntelligence;
using Microsoft.Extensions.DependencyInjection;

namespace AiEng.Platform.App.Composition;

public static class ProjectIntelligenceServiceCollectionExtensions
{
    public const string DefaultRepoRootMarker = "__AIENG_DEFAULT_REPO_ROOT__";

    public static IServiceCollection AddProjectIntelligence(
        this IServiceCollection services,
        Action<ProjectIntelligenceOptions>? configure = null)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        var options = new ProjectIntelligenceOptions();
        configure?.Invoke(options);
        if (string.IsNullOrWhiteSpace(options.RepoRoot) || options.RepoRoot == DefaultRepoRootMarker)
        {
            options.RepoRoot = ResolveDefaultRepoRoot();
        }

        services.AddSingleton(options);
        services.AddSingleton<IStateFileReader, FileSystemStateFileReader>();
        services.AddSingleton<IProjectIntelligenceReader, ProjectIntelligenceReader>();
        return services;
    }

    private static string ResolveDefaultRepoRoot()
    {
        var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "AiEng.Platform.slnx")))
            {
                return dir.FullName;
            }
            dir = dir.Parent;
        }
        return Directory.GetCurrentDirectory();
    }
}
