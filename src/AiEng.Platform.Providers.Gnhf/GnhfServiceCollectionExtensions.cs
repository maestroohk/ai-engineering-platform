using AiEng.Platform.Application.Infrastructure;
using AiEng.Platform.Application.Providers.Families;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AiEng.Platform.Providers.Gnhf;

public static class GnhfServiceCollectionExtensions
{
    public static IServiceCollection AddGnhfProvider(
        this IServiceCollection services,
        string? configuredExecutable = null,
        TimeSpan? timeout = null)
    {
        services.TryAddSingleton(TimeProvider.System);
        services.TryAddSingleton<IGnhfExecutableResolver, GnhfExecutableResolver>();
        services.TryAddSingleton<IGnhfProbeRunner>(sp => new GnhfProcessProbeRunner(
            sp.GetRequiredService<IProcessRunner>(),
            sp.GetRequiredService<IGnhfExecutableResolver>(),
            sp.GetRequiredService<TimeProvider>(),
            sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<GnhfProcessProbeRunner>>(),
            timeout));
        services.TryAddSingleton<IAutonomousLoopProviderFamily, GnhfAutonomousLoopFamily>();
        return services;
    }
}
