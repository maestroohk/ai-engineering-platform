namespace AiEng.Platform.Application.Capabilities;

public interface IHostCapabilitiesService
{
    Task<HostCapabilities> DetectAsync(CancellationToken cancellationToken = default);
}
