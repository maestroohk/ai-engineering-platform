using AiEng.Platform.Application.Infrastructure;
using AiEng.Platform.Infrastructure.Platform;
using AiEng.Platform.Infrastructure.ProcessRunner;
using Xunit;

namespace AiEng.Platform.UnitTests.Infrastructure;

public class SystemPlatformInfoTests
{
    [Fact]
    public void GetDataDirectory_returns_a_non_empty_path()
    {
        var info = new SystemPlatformInfo();

        var dir = info.GetDataDirectory();

        Assert.False(string.IsNullOrWhiteSpace(dir));
        Assert.True(Path.IsPathRooted(dir));
    }

    [Fact]
    public void GetConfigDirectory_returns_a_non_empty_path()
    {
        var info = new SystemPlatformInfo();

        var dir = info.GetConfigDirectory();

        Assert.False(string.IsNullOrWhiteSpace(dir));
        Assert.True(Path.IsPathRooted(dir));
    }

    [Fact]
    public void GetDataDirectory_and_GetConfigDirectory_return_distinct_paths()
    {
        var info = new SystemPlatformInfo();

        var data = info.GetDataDirectory();
        var config = info.GetConfigDirectory();

        Assert.NotEqual(data, config);
    }
}
