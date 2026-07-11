using AiEng.Platform.App.Composition;
using AiEng.Platform.Application.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AiEng.Platform.ComponentTests.Navigation;

public class NavigationServiceCollectionExtensionsTests
{
    [Fact]
    public void AddNavigation_registers_INavigationRegistry_as_singleton()
    {
        var services = new ServiceCollection();
        services.AddSingleton<RouteRegistryTestMarker>(); // ensures the test assembly is reachable
        services.AddNavigation(typeof(NavigationServiceCollectionExtensionsTests).Assembly);

        var registryDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(INavigationRegistry));
        Assert.NotNull(registryDescriptor);
        Assert.Equal(ServiceLifetime.Singleton, registryDescriptor!.Lifetime);

        using var provider = services.BuildServiceProvider();
        var first = provider.GetRequiredService<INavigationRegistry>();
        var second = provider.GetRequiredService<INavigationRegistry>();
        Assert.Same(first, second);
    }

    [Fact]
    public void AddNavigation_with_passed_in_assembly_reflects_its_RouteMetadata_attributes()
    {
        var services = new ServiceCollection();
        services.AddNavigation(typeof(NavigationServiceCollectionExtensionsTests).Assembly);

        using var provider = services.BuildServiceProvider();
        var registry = provider.GetRequiredService<INavigationRegistry>();

        Assert.Contains(registry.Routes, static r => r.Href == "/single");
        Assert.Contains(registry.Routes, static r => r.Href == "/first");
        Assert.Contains(registry.Routes, static r => r.Href == "/admin/users" && r.Parent == "/admin");
    }

    [Fact]
    public void AddPlatformServices_default_composition_root_registers_registry_with_App_assembly()
    {
        var services = new ServiceCollection();
        services.AddPlatformServices(typeof(ProgramMarker).Assembly);

        using var provider = services.BuildServiceProvider();
        var registry = provider.GetRequiredService<INavigationRegistry>();
        Assert.NotNull(registry);
    }

    [RouteMetadata("/integration-marker", "Integration Marker", Order = 0)]
    private sealed class RouteRegistryTestMarker;

    private sealed class ProgramMarker;
}
