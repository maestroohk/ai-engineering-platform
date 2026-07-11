using System.Reflection;
using AiEng.Platform.Application.Navigation;
using Microsoft.Extensions.DependencyInjection;

namespace AiEng.Platform.App.Composition;

public static class NavigationServiceCollectionExtensions
{
    public static IServiceCollection AddNavigation(
        this IServiceCollection services,
        params Assembly[] scanAssemblies)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        var assemblies = scanAssemblies is { Length: > 0 }
            ? scanAssemblies
            : new[] { Assembly.GetExecutingAssembly() };

        services.AddSingleton<INavigationRegistry>(_ => new RouteRegistry(assemblies));
        return services;
    }
}
