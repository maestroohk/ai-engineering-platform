using AiEng.Platform.App.Components.Navigation;
using AiEng.Platform.Application.Navigation;
using AiEng.Platform.App.Layouts;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AiEng.Platform.ComponentTests.Accessibility;

public class AriaCurrentInvariantTests : BunitContext
{
    public AriaCurrentInvariantTests()
    {
        Services.AddSingleton<INavigationRegistry>(new TestNavigationRegistry());
        JSInterop.Setup<string>("appTheme.current").SetResult("light");
        JSInterop.SetupVoid("appTheme.set", _ => true);
    }

    [Fact]
    public void Breadcrumb_Last_Segment_Has_Aria_Current_Page()
    {
        Services.GetRequiredService<NavigationManager>().NavigateTo("/counter");

        var cut = Render<AppBreadcrumb>();

        var current = cut.Find("[aria-current=\"page\"]");
        Assert.NotNull(current);
    }

    [Fact]
    public void NavLink_Renders_Aria_Current_Page_For_The_Active_Route()
    {
        Services.GetRequiredService<NavigationManager>().NavigateTo("/counter");

        var cut = Render<AppNavItem>(parameters => parameters
            .Add(p => p.Route, new RouteMetadata(
                "/counter", "Counter", 1, null, null, null, null)));

        var anchor = cut.Find("a.app-nav-item");
        Assert.Equal("page", anchor.GetAttribute("aria-current"));
    }

    [Fact]
    public void NavLink_Does_Not_Render_Aria_Current_Page_For_An_Inactive_Route()
    {
        Services.GetRequiredService<NavigationManager>().NavigateTo("/");

        var cut = Render<AppNavItem>(parameters => parameters
            .Add(p => p.Route, new RouteMetadata(
                "/counter", "Counter", 1, null, null, null, null)));

        var anchor = cut.Find("a.app-nav-item");
        Assert.Null(anchor.GetAttribute("aria-current"));
    }

    [Fact]
    public void Sidebar_Renders_The_Active_Route_With_Aria_Current_Page()
    {
        Services.GetRequiredService<NavigationManager>().NavigateTo("/dashboard");

        var cut = Render<AppSidebar>();

        var anchors = cut.FindAll("a.app-nav-item");
        var active = anchors.SingleOrDefault(a => a.GetAttribute("aria-current") == "page");
        Assert.NotNull(active);
        Assert.Equal("/dashboard", active.GetAttribute("href"));
    }

    [Fact]
    public void Layout_Renders_Semantic_Regions()
    {
        var cut = Render<AppLayout>(parameters => parameters
            .Add(p => p.Body, "<span>body</span>"));

        Assert.NotNull(cut.Find("header.app-shell-topbar"));
        Assert.NotNull(cut.Find("main.app-shell-content"));
        Assert.NotNull(cut.Find("aside.app-shell-sidebar"));
    }

    private sealed class TestNavigationRegistry : INavigationRegistry
    {
        private static readonly RouteMetadata Home = new("/", "Home", 0, null, null, null, null);
        private static readonly RouteMetadata Counter = new("/counter", "Counter", 1, null, null, null, null);
        private static readonly RouteMetadata Dashboard = new("/dashboard", "Dashboard", 0, null, null, null, null, ShowInSidebar: true);
        private static readonly RouteMetadata DesignSystem = new("/design-system", "Design system", 100, null, null, null, null);

        public IReadOnlyList<RouteMetadata> Routes { get; } = new[] { Home, Counter, Dashboard, DesignSystem };

        public RouteMetadata? FindByHref(string href) => Routes.FirstOrDefault(r =>
            string.Equals(r.Href, href, StringComparison.OrdinalIgnoreCase));

        public IReadOnlyList<RouteMetadata> ChildrenOf(string? parentHref) => Array.Empty<RouteMetadata>();
    }
}
