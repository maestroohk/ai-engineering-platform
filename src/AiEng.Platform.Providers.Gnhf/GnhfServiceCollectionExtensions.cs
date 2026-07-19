using AiEng.Platform.Application.Infrastructure;
using AiEng.Platform.Application.Providers.Families;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AiEng.Platform.Providers.Gnhf;

public static class GnhfServiceCollectionExtensions
{
    public static IServiceCollection AddGnhfProvider(
        this IServiceCollection services,
        string? executable = null,
        TimeSpan? timeout = null)
    {
        services.AddSingleton<IGnhfProbeRunner>(sp => new GnhfProcessProbeRunner(
            sp.GetRequiredService<IProcessRunner>(),
            sp.GetRequiredService<IPlatformInfo>(),
            sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<GnhfProcessProbeRunner>>(),
            executable,
            timeout));
        services.TryAddSingleton<IAutonomousLoopProviderFamily, GnhfAutonomousLoopFamily>();
        return services;
    }
}
