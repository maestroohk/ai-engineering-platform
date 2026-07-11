using System.Reflection;
using AiEng.Platform.Application.ProjectIntelligence;

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
        return services;
    }
}
