using System.Runtime.InteropServices;
using AiEng.Platform.Application.Infrastructure;

namespace AiEng.Platform.Infrastructure.Platform;

public sealed class SystemPlatformInfo : IPlatformInfo
{
    private const string ProductSubdirectory = "AiEng/Platform";

    public string GetDataDirectory()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(localAppData, ProductSubdirectory, "data");
    }

    public string GetConfigDirectory()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(localAppData, ProductSubdirectory, "config");
    }

    public bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
}
