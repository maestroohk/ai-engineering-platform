using AiEng.Platform.Application.Infrastructure;
using AiEng.Platform.Application.Projects;
using AiEng.Platform.Infrastructure.Credentials;
using AiEng.Platform.Infrastructure.Platform;
using AiEng.Platform.Infrastructure.ProcessRunner;
using AiEng.Platform.Infrastructure.Projects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AiEng.Platform.App.Composition.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.TryAddSingleton<IPlatformInfo, SystemPlatformInfo>();
        services.TryAddSingleton<IProcessRunner, SystemProcessRunner>();
        services.TryAddSingleton<ICredentialVault, WindowsCredentialVault>();
        services.TryAddSingleton<IProjectStore, JsonFileProjectStore>();
        return services;
    }
}
