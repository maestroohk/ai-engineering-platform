namespace AiEng.Platform.Providers.Gnhf;

public interface IGnhfProbeRunner
{
    Task<GnhfProbe> ProbeAsync(CancellationToken cancellationToken = default);
}
