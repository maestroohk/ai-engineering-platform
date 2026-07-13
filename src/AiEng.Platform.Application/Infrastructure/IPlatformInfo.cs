namespace AiEng.Platform.Application.Infrastructure;

public interface IPlatformInfo
{
    string GetDataDirectory();

    string GetConfigDirectory();

    bool IsWindows { get; }
}
