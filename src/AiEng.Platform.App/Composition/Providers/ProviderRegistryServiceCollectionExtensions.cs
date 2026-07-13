using AiEng.Platform.Application.Providers;
using AiEng.Platform.Application.Providers.Families;
using AiEng.Platform.Infrastructure.Providers;
using AiEng.Platform.Infrastructure.Providers.Families;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AiEng.Platform.App.Composition.Providers;

public static class ProviderRegistryServiceCollectionExtensions
{
    public static IServiceCollection AddProviderRegistry(this IServiceCollection services)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.TryAddSingleton<IGitProviderFamily, GitProviderFamily>();
        services.TryAddSingleton<IAgentRuntimeProviderFamily, AgentRuntimeProviderFamily>();
        services.TryAddSingleton<IReviewProviderFamily, ReviewProviderFamily>();
        services.TryAddSingleton<IQualityGateProviderFamily, QualityGateProviderFamily>();
        services.TryAddSingleton<IAutonomousLoopProviderFamily, AutonomousLoopProviderFamily>();
        services.TryAddSingleton<IOrchestrationProviderFamily, OrchestrationProviderFamily>();
        services.TryAddSingleton<IProviderRegistry, SystemProviderRegistry>();
        return services;
    }
}
