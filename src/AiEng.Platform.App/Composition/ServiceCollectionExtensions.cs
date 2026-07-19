using System.Reflection;
using AiEng.Platform.App.Composition.Capabilities;
using AiEng.Platform.App.Composition.Infrastructure;
using AiEng.Platform.App.Composition.Projects;
using AiEng.Platform.App.Composition.Providers;
using AiEng.Platform.Application.ProjectIntelligence;
using AiEng.Platform.Providers.Gnhf;

namespace AiEng.Platform.App.Composition;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPlatformServices(
        this IServiceCollection services,
        params Assembly[] platformAssemblies)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        var assemblies = platformAssemblies is { Length: > 0 }
            ? platformAssemblies
            : new[] { Assembly.GetExecutingAssembly() };

        services.AddNavigation(assemblies);
        services.AddProjectIntelligence();
        services.AddProjects();
        services.AddInfrastructure();
        services.AddHostCapabilities();
        services.AddGnhfProvider();
        services.AddProviderRegistry();
        return services;
    }
}
