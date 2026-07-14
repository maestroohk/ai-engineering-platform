using System.Threading.Tasks;
using AiEng.Platform.App.Components.Pages;
using AiEng.Platform.Application.Capabilities;
using AiEng.Platform.Application.Infrastructure;
using AiEng.Platform.Application.Navigation;
using AiEng.Platform.Application.Providers;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AiEng.Platform.ComponentTests.Pages;

public class ProvidersPageTests : BunitContext
{
    public ProvidersPageTests()
    {
        Services.AddSingleton<INavigationRegistry>(new EmptyNavigationRegistry());
        JSInterop.Setup<string>("appTheme.current").SetResult("light");
        JSInterop.SetupVoid("appTheme.set", _ => true);
    }

    [Fact]
    public void ProvidersPage_calls_ListProvidersAsync_on_init()
    {
        var registry = new FakeProviderRegistry(BuildSampleProviders());
        Services.AddSingleton<IProviderRegistry>(registry);
        Services.AddSingleton<IHostCapabilitiesService>(new FakeHostCapabilitiesService(BuildSampleCapabilities()));
        Services.AddSingleton<IPlatformInfo>(new StaticPlatformInfo());

        var cut = Render<Providers>();

        cut.WaitForState(() => cut.Markup.Contains("Providers"));

        Assert.Equal(6, registry.CallCount);
    }

    [Fact]
    public void ProvidersPage_renders_AppProviderList_per_family()
    {
        var registry = new FakeProviderRegistry(BuildSampleProviders());
        Services.AddSingleton<IProviderRegistry>(registry);
        Services.AddSingleton<IHostCapabilitiesService>(new FakeHostCapabilitiesService(BuildSampleCapabilities()));
        Services.AddSingleton<IPlatformInfo>(new StaticPlatformInfo());

        var cut = Render<Providers>();

        cut.WaitForState(() => cut.Markup.Contains("Git CLI"));

        var familyHeaders = cut.FindAll(".app-providers-card");
        Assert.Equal(7, familyHeaders.Count);
        Assert.Contains("Git providers", cut.Markup);
        Assert.Contains("Agent runtime providers", cut.Markup);
        Assert.Contains("Review providers", cut.Markup);
        Assert.Contains("Quality gate providers", cut.Markup);
        Assert.Contains("Autonomous loop providers", cut.Markup);
        Assert.Contains("Orchestration providers", cut.Markup);
    }

    [Fact]
    public async Task ProvidersPage_Refresh_button_reruns_ListProvidersAsync()
    {
        var registry = new FakeProviderRegistry(BuildSampleProviders());
        Services.AddSingleton<IProviderRegistry>(registry);
        Services.AddSingleton<IHostCapabilitiesService>(new FakeHostCapabilitiesService(BuildSampleCapabilities()));
        Services.AddSingleton<IPlatformInfo>(new StaticPlatformInfo());

        var cut = Render<Providers>();

        cut.WaitForState(() => cut.Markup.Contains("Git CLI"));

        var button = cut.Find("[data-testid=\"refresh-providers\"]");
        button.Click();

        cut.WaitForState(() => registry.CallCount == 12);

        Assert.Equal(12, registry.CallCount);
    }

    [Fact]
    public void ProvidersPage_renders_host_metadata()
    {
        var registry = new FakeProviderRegistry(BuildSampleProviders());
        Services.AddSingleton<IProviderRegistry>(registry);
        Services.AddSingleton<IHostCapabilitiesService>(new FakeHostCapabilitiesService(BuildSampleCapabilities()));
        Services.AddSingleton<IPlatformInfo>(new StaticPlatformInfo());

        var cut = Render<Providers>();

        cut.WaitForState(() => cut.Markup.Contains("Detected at"));

        Assert.Contains("Detected at", cut.Markup);
        Assert.Contains("Data directory", cut.Markup);
        Assert.Contains("Config directory", cut.Markup);
        Assert.Contains("Is Windows host", cut.Markup);
    }

    [Fact]
    public void ProvidersPage_renders_AppProviderList_items_per_family()
    {
        var registry = new FakeProviderRegistry(BuildSampleProviders());
        Services.AddSingleton<IProviderRegistry>(registry);
        Services.AddSingleton<IHostCapabilitiesService>(new FakeHostCapabilitiesService(BuildSampleCapabilities()));
        Services.AddSingleton<IPlatformInfo>(new StaticPlatformInfo());

        var cut = Render<Providers>();

        cut.WaitForState(() => cut.FindAll(".app-provider-list-item").Count >= 2);

        var items = cut.FindAll(".app-provider-list-item");
        Assert.Contains("Git CLI", cut.Markup);
        Assert.Contains("Ollama", cut.Markup);
    }

    private static IReadOnlyDictionary<ProviderFamily, IReadOnlyList<ProviderDescriptor>> BuildSampleProviders()
    {
        return new Dictionary<ProviderFamily, IReadOnlyList<ProviderDescriptor>>
        {
            [ProviderFamily.Git] = new ProviderDescriptor[]
            {
                new("git-cli", "Git CLI", ProviderFamily.Git, ProviderStatus.Available, "2.45.0", new Dictionary<string, string>())
            },
            [ProviderFamily.AgentRuntime] = new ProviderDescriptor[]
            {
                new("ollama", "Ollama", ProviderFamily.AgentRuntime, ProviderStatus.Available, "0.3.12", new Dictionary<string, string>())
            },
            [ProviderFamily.Review] = Array.Empty<ProviderDescriptor>(),
            [ProviderFamily.QualityGate] = Array.Empty<ProviderDescriptor>(),
            [ProviderFamily.AutonomousLoop] = Array.Empty<ProviderDescriptor>(),
            [ProviderFamily.Orchestration] = Array.Empty<ProviderDescriptor>(),
        };
    }

    private static IReadOnlyList<HostCapability> BuildSampleCapabilities() => new HostCapability[]
    {
        new("git", true, "2.45.0", false, null),
        new("ollama", true, "0.3.12", false, null),
        new("powershell", false, null, false, null),
        new("wsl", false, null, false, null),
        new("wt", false, null, false, null),
        new("bash", false, null, false, null),
    };

    private sealed class FakeProviderRegistry : IProviderRegistry
    {
        private readonly IReadOnlyDictionary<ProviderFamily, IReadOnlyList<ProviderDescriptor>> _providers;

        public FakeProviderRegistry(IReadOnlyDictionary<ProviderFamily, IReadOnlyList<ProviderDescriptor>> providers)
        {
            _providers = providers;
        }

        public int CallCount { get; private set; }

        public Task<IReadOnlyList<ProviderDescriptor>> ListProvidersAsync(ProviderFamily family, CancellationToken cancellationToken = default)
        {
            CallCount++;
            if (_providers.TryGetValue(family, out var list))
            {
                return Task.FromResult(list);
            }
            return Task.FromResult<IReadOnlyList<ProviderDescriptor>>(Array.Empty<ProviderDescriptor>());
        }
    }

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
