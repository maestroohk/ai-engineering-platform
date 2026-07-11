using AiEng.Platform.App.Components.Shell;
using AiEng.Platform.Application.Navigation;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AiEng.Platform.ComponentTests.Shell;

public class AppTopBarTests : BunitContext
{
    public AppTopBarTests()
    {
        Services.AddSingleton<INavigationRegistry>(new RouteRegistry(typeof(TopBarRouteMarker).Assembly));
        JSInterop.Setup<string?>("appTheme.get").SetResult(null);
        JSInterop.SetupVoid("appTheme.set", _ => true);
    }
    [Fact]
    public void Renders_App_Region_Element()
    {
        var cut = Render<AppTopBar>();

        Assert.NotNull(cut.Find("section.app-region"));
    }

    [Fact]
    public void Has_Topbar_Data_Attribute()
    {
        var cut = Render<AppTopBar>();

        Assert.NotNull(cut.Find("[data-app-region=\"topbar\"]"));
    }

    [Fact]
    public void Has_Topbar_Aria_Label()
    {
        var cut = Render<AppTopBar>();

        var region = cut.Find("section.app-region");
        Assert.Equal("topbar", region.GetAttribute("aria-label"));
    }

    [Fact]
    public void Renders_The_Default_App_Theme_Toggle_In_Trailing_Slot()
    {
        var cut = Render<AppTopBar>();

        Assert.NotNull(cut.Find("button.app-theme-toggle"));
    }

    [Fact]
    public void Renders_The_Default_User_Avatar_Slot_In_Trailing_Slot()
    {
        var cut = Render<AppTopBar>();

        Assert.NotNull(cut.Find(".app-user-avatar-slot"));
    }

    [Fact]
    public void Default_Leading_Slot_Shows_The_Current_Route_Title()
    {
        Services.GetRequiredService<NavigationManager>().NavigateTo("/topbar-route");

        var cut = Render<AppTopBar>();

        Assert.Equal("TopBarRoute", cut.Find(".app-topbar-title").TextContent);
    }

    [Fact]
    public void Default_Leading_Slot_Falls_Back_To_Home_When_Route_Is_Unknown()
    {
        Services.GetRequiredService<NavigationManager>().NavigateTo("/");

        var cut = Render<AppTopBar>();

        Assert.Equal("Home", cut.Find(".app-topbar-title").TextContent);
    }

    [Fact]
    public void Leading_Slot_Override_Replaces_The_Default_Title()
    {
        var cut = Render<AppTopBar>(parameters => parameters
            .Add(p => p.Leading, builder => builder.AddContent(0, "Custom Leading")));

        Assert.Equal("Custom Leading", cut.Find(".app-topbar-leading").TextContent);
        Assert.Throws<ElementNotFoundException>(() => cut.Find(".app-topbar-title"));
    }

    [Fact]
    public void Trailing_Slot_Override_Replaces_The_Default_Theme_Toggle_And_Avatar()
    {
        var cut = Render<AppTopBar>(parameters => parameters
            .Add(p => p.Trailing, builder => builder.AddContent(0, "Custom Trailing")));

        Assert.Equal("Custom Trailing", cut.Find(".app-topbar-trailing").TextContent);
        Assert.Throws<ElementNotFoundException>(() => cut.Find("button.app-theme-toggle"));
        Assert.Throws<ElementNotFoundException>(() => cut.Find(".app-user-avatar-slot"));
    }

    [RouteMetadata("/topbar-route", "TopBarRoute", Order = 0)]
    private sealed class TopBarRouteMarker;

    private sealed class AssemblyMarkerAccess;
}
