using System.Threading.Tasks;
using AiEng.Platform.App.Components.Pages;
using AiEng.Platform.Application.Capabilities;
using AiEng.Platform.Application.Infrastructure;
using AiEng.Platform.Application.Navigation;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AiEng.Platform.ComponentTests.Pages;

public class DiagnosticsPageTests : BunitContext
{
    public DiagnosticsPageTests()
    {
        Services.AddSingleton<INavigationRegistry>(new EmptyNavigationRegistry());
        JSInterop.Setup<string>("appTheme.current").SetResult("light");
        JSInterop.SetupVoid("appTheme.set", _ => true);
    }

    [Fact]
    public void DiagnosticsPage_calls_DetectAsync_on_init()
    {
        var fake = new FakeHostCapabilitiesService(SampleCapabilities());
        Services.AddSingleton<IHostCapabilitiesService>(fake);
        Services.AddSingleton<IPlatformInfo>(new StaticPlatformInfo());

        var cut = Render<Diagnostics>();

        cut.WaitForState(() => cut.Markup.Contains("Diagnostics"));

        Assert.Equal(1, fake.DetectAsyncCallCount);
    }

    [Fact]
    public void DiagnosticsPage_renders_AppCapabilityList_with_capabilities()
    {
        var fake = new FakeHostCapabilitiesService(SampleCapabilities());
        Services.AddSingleton<IHostCapabilitiesService>(fake);
        Services.AddSingleton<IPlatformInfo>(new StaticPlatformInfo());

        var cut = Render<Diagnostics>();

        cut.WaitForState(() => cut.FindAll(".app-capability-list-item").Count == 12);

        Assert.Equal(12, cut.FindAll(".app-capability-list-item").Count);
        Assert.Contains("git", cut.Markup);
        Assert.Contains("ollama", cut.Markup);
        Assert.Contains("provider:git", cut.Markup);
    }

    [Fact]
    public async Task DiagnosticsPage_Refresh_button_reruns_DetectAsync()
    {
        var fake = new FakeHostCapabilitiesService(SampleCapabilities());
        Services.AddSingleton<IHostCapabilitiesService>(fake);
        Services.AddSingleton<IPlatformInfo>(new StaticPlatformInfo());

        var cut = Render<Diagnostics>();

        cut.WaitForState(() => cut.FindAll(".app-capability-list-item").Count == 12);

        var button = cut.Find("[data-testid=\"refresh-diagnostics\"]");
        button.Click();

        cut.WaitForState(() => fake.DetectAsyncCallCount == 2);

        Assert.Equal(2, fake.DetectAsyncCallCount);
    }

    [Fact]
    public void DiagnosticsPage_renders_AppKeyValueList_with_host_metadata()
    {
        var fake = new FakeHostCapabilitiesService(SampleCapabilities());
        Services.AddSingleton<IHostCapabilitiesService>(fake);
        Services.AddSingleton<IPlatformInfo>(new StaticPlatformInfo());

        var cut = Render<Diagnostics>();

        cut.WaitForState(() => cut.Markup.Contains("Detected at"));

        Assert.Contains("Detected at", cut.Markup);
        Assert.Contains("Data directory", cut.Markup);
        Assert.Contains("Config directory", cut.Markup);
        Assert.Contains("Is Windows host", cut.Markup);
    }

    private static IReadOnlyList<HostCapability> SampleCapabilities() => new HostCapability[]
    {
        new("git", true, "2.45.0", false, null),
        new("ollama", true, "0.3.12", false, null),
        new("powershell", false, null, false, null),
        new("wsl", false, null, false, null),
        new("wt", false, null, false, null),
        new("bash", false, null, false, null),
        new("provider:git", true, null, true, "git-token"),
        new("provider:ollama", true, null, false, null),
        new("provider:powershell", false, null, false, null),
        new("provider:wsl", false, null, false, null),
        new("provider:wt", false, null, false, null),
        new("provider:bash", false, null, false, null),
    };

    private sealed class FakeHostCapabilitiesService : IHostCapabilitiesService
    {
        private readonly HostCapabilities _capabilities;

        public FakeHostCapabilitiesService(IReadOnlyList<HostCapability> capabilities)
        {
            _capabilities = new HostCapabilities(capabilities, System.DateTimeOffset.UtcNow);
        }

        public int DetectAsyncCallCount { get; private set; }

        public Task<HostCapabilities> DetectAsync(CancellationToken cancellationToken = default)
        {
            DetectAsyncCallCount++;
            return Task.FromResult(_capabilities);
        }
    }

    private sealed class StaticPlatformInfo : IPlatformInfo
    {
        public string GetDataDirectory() => "/tmp/data";

        public string GetConfigDirectory() => "/tmp/config";

        public bool IsWindows => true;
    }

    private sealed class EmptyNavigationRegistry : INavigationRegistry
    {
        public IReadOnlyList<RouteMetadata> Routes { get; } = Array.Empty<RouteMetadata>();

        public RouteMetadata? FindByHref(string href) => null;

        public IReadOnlyList<RouteMetadata> ChildrenOf(string? parentHref) => Array.Empty<RouteMetadata>();
    }
}
