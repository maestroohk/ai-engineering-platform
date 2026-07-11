using AiEng.Platform.Application.Projects;
using Microsoft.Extensions.DependencyInjection;

namespace AiEng.Platform.App.Composition.Projects;

public static class ProjectsServiceCollectionExtensions
{
    public static IServiceCollection AddProjects(this IServiceCollection services)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddSingleton<IProjectStore, InMemoryProjectStore>();
        services.AddSingleton<IProjectService, ProjectService>();
        return services;
    }
}
