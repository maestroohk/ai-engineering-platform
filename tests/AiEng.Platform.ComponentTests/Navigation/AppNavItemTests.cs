using AiEng.Platform.App.Components.Navigation;
using AiEng.Platform.Application.Navigation;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AiEng.Platform.ComponentTests.Navigation;

public class AppNavItemTests : BunitContext
{
    [Fact]
    public void Renders_a_NavLink_with_the_app_nav_item_class()
    {
        var route = new RouteMetadata("/counter", "Counter", 1, null, null, null, null);

        var cut = Render<AppNavItem>(parameters => parameters
            .Add(p => p.Route, route));

        var link = cut.Find("a.app-nav-item");
        Assert.NotNull(link);
        Assert.Equal("/counter", link.GetAttribute("href"));
    }

    [Fact]
    public void Match_is_All_by_default()
    {
        var route = new RouteMetadata("/counter", "Counter", 1, null, null, null, null);

        var cut = Render<AppNavItem>(parameters => parameters
            .Add(p => p.Route, route));

        var navLink = cut.FindComponent<NavLink>();
        Assert.Equal(NavLinkMatch.All, navLink.Instance.Match);
    }

    [Fact]
    public void Match_is_Prefix_when_Route_MatchPrefix_is_true()
    {
        var route = new RouteMetadata("/admin", "Admin", 0, null, null, null, null, MatchPrefix: true);

        var cut = Render<AppNavItem>(parameters => parameters
            .Add(p => p.Route, route));

        var navLink = cut.FindComponent<NavLink>();
        Assert.Equal(NavLinkMatch.Prefix, navLink.Instance.Match);
    }

    [Fact]
    public void Aria_current_page_is_set_when_NavigationManager_uri_matches_the_route()
    {
        var route = new RouteMetadata("/counter", "Counter", 1, null, null, null, null);

        var nav = Services.GetRequiredService<NavigationManager>();
        nav.NavigateTo("/counter");

        var cut = Render<AppNavItem>(parameters => parameters
            .Add(p => p.Route, route));

        Assert.Equal("page", cut.Find("a.app-nav-item").GetAttribute("aria-current"));
    }

    [Fact]
    public void Aria_current_is_not_set_when_NavigationManager_uri_does_not_match()
    {
        var route = new RouteMetadata("/counter", "Counter", 1, null, null, null, null);

        var nav = Services.GetRequiredService<NavigationManager>();
        nav.NavigateTo("/weather");

        var cut = Render<AppNavItem>(parameters => parameters
            .Add(p => p.Route, route));

        Assert.Null(cut.Find("a.app-nav-item").GetAttribute("aria-current"));
    }

    [Fact]
    public void Aria_label_defaults_to_Route_Title_when_AriaLabel_is_not_provided()
    {
        var route = new RouteMetadata("/counter", "Counter", 1, null, null, null, null);

        var cut = Render<AppNavItem>(parameters => parameters
            .Add(p => p.Route, route));

        Assert.Equal("Counter", cut.Find("a.app-nav-item").GetAttribute("aria-label"));
    }

    [Fact]
    public void Aria_label_uses_explicit_AriaLabel_when_provided()
    {
        var route = new RouteMetadata("/counter", "Counter", 1, null, null, null, null);

        var cut = Render<AppNavItem>(parameters => parameters
            .Add(p => p.Route, route)
            .Add(p => p.AriaLabel, "Counter page"));

        Assert.Equal("Counter page", cut.Find("a.app-nav-item").GetAttribute("aria-label"));
    }

    [Fact]
    public void Route_Title_is_the_visible_label()
    {
        var route = new RouteMetadata("/counter", "Counter", 1, null, null, null, null);

        var cut = Render<AppNavItem>(parameters => parameters
            .Add(p => p.Route, route));

        Assert.Equal("Counter", cut.Find(".app-nav-item-label").TextContent);
    }

    [Fact]
    public void Route_BadgeText_renders_an_AppBadge_when_present()
    {
        var route = new RouteMetadata("/counter", "Counter", 1, null, null, null, "new");

        var cut = Render<AppNavItem>(parameters => parameters
            .Add(p => p.Route, route));

        Assert.NotNull(cut.Find(".app-nav-item-badge .app-badge"));
        Assert.Contains("new", cut.Find(".app-nav-item-badge").TextContent);
    }
}
