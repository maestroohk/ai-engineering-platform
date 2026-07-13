using AiEng.Platform.Application.Capabilities;
using AiEng.Platform.Infrastructure.Capabilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AiEng.Platform.App.Composition.Capabilities;

public static class CapabilitiesServiceCollectionExtensions
{
    public static IServiceCollection AddHostCapabilities(this IServiceCollection services)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.TryAddSingleton<IHostCapabilitiesService, SystemHostCapabilitiesService>();
        return services;
    }
}
