using AiEng.Platform.App.Components.Navigation;
using AiEng.Platform.Application.Navigation;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AiEng.Platform.ComponentTests.Navigation;

public class AppSidebarTests : BunitContext
{
    [Fact]
    public void Default_renders_one_AppNavItem_per_non_hidden_route_from_the_registry()
    {
        var registry = new StaticNavigationRegistry(
            new RouteMetadata("/counter", "Counter", 1, null, null, null, null),
            new RouteMetadata("/weather", "Weather", 2, null, null, null, null));
        Services.AddSingleton<INavigationRegistry>(registry);

        var cut = Render<AppSidebar>();

        var items = cut.FindAll(".app-nav-item");
        Assert.Equal(2, items.Count);
    }

    [Fact]
    public void Aria_current_page_is_set_on_the_active_route()
    {
        var registry = new StaticNavigationRegistry(
            new RouteMetadata("/counter", "Counter", 1, null, null, null, null),
            new RouteMetadata("/weather", "Weather", 2, null, null, null, null));
        Services.AddSingleton<INavigationRegistry>(registry);

        var nav = Services.GetRequiredService<NavigationManager>();
        nav.NavigateTo("/counter");

        var cut = Render<AppSidebar>();

        var counter = cut.Find("a[href='/counter']");
        Assert.Equal("page", counter.GetAttribute("aria-current"));
    }

    [Fact]
    public void Renders_with_sidebar_region_attributes()
    {
        Services.AddSingleton<INavigationRegistry>(new StaticNavigationRegistry());

        var cut = Render<AppSidebar>();

        Assert.NotNull(cut.Find("section.app-region"));
        Assert.NotNull(cut.Find("[data-app-region=\"sidebar\"]"));
    }

    [Fact]
    public void Empty_registry_renders_an_empty_sidebar()
    {
        Services.AddSingleton<INavigationRegistry>(new StaticNavigationRegistry());

        var cut = Render<AppSidebar>();

        Assert.Empty(cut.FindAll(".app-nav-item"));
    }

    [Fact]
    public void Sidebar_reads_from_INavigationRegistry_not_hardcoded()
    {
        var registry = new StaticNavigationRegistry(
            new RouteMetadata("/only-this", "Only", 0, null, null, null, null));
        Services.AddSingleton<INavigationRegistry>(registry);

        var cut = Render<AppSidebar>();

        var items = cut.FindAll(".app-nav-item");
        Assert.Single(items);
        Assert.Equal("/only-this", items[0].GetAttribute("href"));
    }

    [Fact]
    public void AppShellRegion_Name_sidebar_is_preserved()
    {
        Services.AddSingleton<INavigationRegistry>(new StaticNavigationRegistry());

        var cut = Render<AppSidebar>();

        Assert.Equal("sidebar", cut.Find("[data-app-region]").GetAttribute("data-app-region"));
    }

    private sealed class StaticNavigationRegistry : INavigationRegistry
    {
        private readonly IReadOnlyList<RouteMetadata> _routes;

        public StaticNavigationRegistry(params RouteMetadata[] routes)
        {
            _routes = routes;
        }

        public IReadOnlyList<RouteMetadata> Routes => _routes;

        public RouteMetadata? FindByHref(string href) =>
            _routes.FirstOrDefault(r => string.Equals(r.Href, href, StringComparison.OrdinalIgnoreCase));

        public IReadOnlyList<RouteMetadata> ChildrenOf(string? parentHref) =>
            _routes.Where(r => string.Equals(r.Parent ?? string.Empty, parentHref ?? string.Empty, StringComparison.OrdinalIgnoreCase)).ToList();
    }
}
