using AiEng.Platform.App.Components.Navigation;
using AiEng.Platform.Application.Navigation;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AiEng.Platform.ComponentTests.Navigation;

public class AppBreadcrumbTests : BunitContext
{
    [Fact]
    public void Renders_A_Nav_With_Breadcrumb_Class()
    {
        var registry = new RouteRegistry(typeof(HomeMarker).Assembly);
        Services.AddSingleton<INavigationRegistry>(registry);
        Services.GetRequiredService<NavigationManager>().NavigateTo("/");

        var cut = Render<AppBreadcrumb>();

        Assert.NotNull(cut.Find("nav.app-breadcrumb"));
    }

    [Fact]
    public void Aria_Label_Is_Breadcrumb()
    {
        var registry = new RouteRegistry(typeof(HomeMarker).Assembly);
        Services.AddSingleton<INavigationRegistry>(registry);
        Services.GetRequiredService<NavigationManager>().NavigateTo("/");

        var cut = Render<AppBreadcrumb>();

        Assert.Equal("Breadcrumb", cut.Find("nav.app-breadcrumb").GetAttribute("aria-label"));
    }

    [Fact]
    public void Renders_Single_Item_As_Current_When_No_Parent_Chain()
    {
        var registry = new RouteRegistry(typeof(BreadcrumbCounterMarker).Assembly);
        Services.AddSingleton<INavigationRegistry>(registry);
        Services.GetRequiredService<NavigationManager>().NavigateTo("/breadcrumb-counter");

        var cut = Render<AppBreadcrumb>();

        Assert.Equal("BreadcrumbCounter", cut.Find(".app-breadcrumb-current").TextContent);
    }

    [Fact]
    public void Renders_Parent_Link_And_Current_For_Two_Level_Chain()
    {
        var registry = new RouteRegistry(typeof(BreadcrumbSectionMarker).Assembly);
        Services.AddSingleton<INavigationRegistry>(registry);
        Services.GetRequiredService<NavigationManager>().NavigateTo("/breadcrumb-section/item");

        var cut = Render<AppBreadcrumb>();

        var link = cut.Find("a.app-breadcrumb-link");
        Assert.Equal("/breadcrumb-section", link.GetAttribute("href"));
        Assert.Equal("BreadcrumbSection", link.TextContent);

        Assert.Equal("Item", cut.Find(".app-breadcrumb-current").TextContent);
    }

    [Fact]
    public void Renders_Three_Level_Chain_With_Two_Links_And_One_Current()
    {
        var registry = new RouteRegistry(typeof(HomeMarker).Assembly);
        Services.AddSingleton<INavigationRegistry>(registry);
        Services.GetRequiredService<NavigationManager>().NavigateTo("/breadcrumb-dashboard/projects");

        var cut = Render<AppBreadcrumb>();

        var links = cut.FindAll("a.app-breadcrumb-link");
        Assert.Equal(2, links.Count);
        Assert.Equal("Home", links[0].TextContent);
        Assert.Equal("/", links[0].GetAttribute("href"));
        Assert.Equal("BreadcrumbDashboard", links[1].TextContent);
        Assert.Equal("/breadcrumb-dashboard", links[1].GetAttribute("href"));
        Assert.Equal("Projects", cut.Find(".app-breadcrumb-current").TextContent);
    }

    [Fact]
    public void Current_Item_Has_Aria_Current_Page()
    {
        var registry = new RouteRegistry(typeof(BreadcrumbCounterMarker).Assembly);
        Services.AddSingleton<INavigationRegistry>(registry);
        Services.GetRequiredService<NavigationManager>().NavigateTo("/breadcrumb-counter");

        var cut = Render<AppBreadcrumb>();

        Assert.Equal("page", cut.Find(".app-breadcrumb-current").GetAttribute("aria-current"));
    }

    [Fact]
    public void Separator_Is_Aria_Hidden()
    {
        var registry = new RouteRegistry(typeof(BreadcrumbDashboardMarker).Assembly);
        Services.AddSingleton<INavigationRegistry>(registry);
        Services.GetRequiredService<NavigationManager>().NavigateTo("/breadcrumb-dashboard/projects");

        var cut = Render<AppBreadcrumb>();

        Assert.Equal("true", cut.Find(".app-breadcrumb-separator").GetAttribute("aria-hidden"));
    }

    [Fact]
    public void Renders_Only_The_Current_Item_When_Route_Is_Not_In_Registry()
    {
        var registry = new RouteRegistry(typeof(HomeMarker).Assembly);
        Services.AddSingleton<INavigationRegistry>(registry);
        Services.GetRequiredService<NavigationManager>().NavigateTo("/some/unknown");

        var cut = Render<AppBreadcrumb>();

        var links = cut.FindAll("a.app-breadcrumb-link");
        Assert.Empty(links);
        Assert.Equal("unknown", cut.Find(".app-breadcrumb-current").TextContent);
    }

    [RouteMetadata("/", "Home", Order = 0)]
    private sealed class HomeMarker;

    [RouteMetadata("/breadcrumb-counter", "BreadcrumbCounter", Order = 1)]
    private sealed class BreadcrumbCounterMarker;

    [RouteMetadata("/breadcrumb-dashboard", "BreadcrumbDashboard", Order = 1, Parent = "/", MatchPrefix = true)]
    private sealed class BreadcrumbDashboardMarker;

    [RouteMetadata("/breadcrumb-dashboard/projects", "Projects", Order = 2, Parent = "/breadcrumb-dashboard", MatchPrefix = true)]
    private sealed class BreadcrumbDashboardProjectsMarker;

    [RouteMetadata("/breadcrumb-section", "BreadcrumbSection", Order = 1, MatchPrefix = true)]
    private sealed class BreadcrumbSectionMarker;

    [RouteMetadata("/breadcrumb-section/item", "Item", Order = 2, Parent = "/breadcrumb-section", MatchPrefix = true)]
    private sealed class BreadcrumbSectionItemMarker;

    private sealed class AssemblyMarkerAccess;
}
