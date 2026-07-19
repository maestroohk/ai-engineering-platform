using AiEng.Platform.Application.Infrastructure;

namespace AiEng.Platform.Providers.Gnhf;

public sealed class HostPlatformInfo : IPlatformInfo
{
    public bool IsWindows => OperatingSystem.IsWindows();
    public string GetDataDirectory() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AiEng", "Platform", "data");
    public string GetConfigDirectory() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AiEng", "Platform", "config");
}
