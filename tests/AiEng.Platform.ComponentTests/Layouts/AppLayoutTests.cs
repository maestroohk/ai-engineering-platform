using AiEng.Platform.App.Layouts;
using AiEng.Platform.Application.Navigation;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AiEng.Platform.ComponentTests.Layouts;

public class AppLayoutTests : BunitContext
{
    public AppLayoutTests()
    {
        Services.AddSingleton<INavigationRegistry>(new EmptyNavigationRegistry());
        JSInterop.Setup<string>("appTheme.current").SetResult("light");
        JSInterop.SetupVoid("appTheme.set", _ => true);
    }

    [Fact]
    public void Default_Renders_App_Shell_Root()
    {
        var cut = Render<AppLayout>(parameters => parameters
            .Add(p => p.Body, "<span>body</span>"));

        Assert.NotNull(cut.Find("div.app-shell"));
    }

    [Fact]
    public void Sidebar_Region_Renders_App_Shell_Sidebar()
    {
        var cut = Render<AppLayout>(parameters => parameters
            .Add(p => p.Body, "<span>body</span>"));

        Assert.NotNull(cut.Find("aside.app-shell-sidebar"));
    }

    [Fact]
    public void Topbar_Region_Renders_App_Shell_Topbar()
    {
        var cut = Render<AppLayout>(parameters => parameters
            .Add(p => p.Body, "<span>body</span>"));

        Assert.NotNull(cut.Find("header.app-shell-topbar"));
    }

    [Fact]
    public void Content_Region_Renders_App_Shell_Content()
    {
        var cut = Render<AppLayout>(parameters => parameters
            .Add(p => p.Body, "<span>body</span>"));

        Assert.NotNull(cut.Find("main.app-shell-content"));
    }

    [Fact]
    public void Body_Content_Is_Rendered()
    {
        var cut = Render<AppLayout>(parameters => parameters
            .Add(p => p.Body, "<span class=\"probe\">page-body</span>"));

        Assert.NotNull(cut.Find(".app-shell-content .probe"));
    }

    [Fact]
    public void Sidebar_Region_Has_Sidebar_Data_Attribute()
    {
        var cut = Render<AppLayout>(parameters => parameters
            .Add(p => p.Body, "<span>body</span>"));

        Assert.NotNull(cut.Find("[data-app-region=\"sidebar\"]"));
    }

    [Fact]
    public void Topbar_Region_Has_Topbar_Data_Attribute()
    {
        var cut = Render<AppLayout>(parameters => parameters
            .Add(p => p.Body, "<span>body</span>"));

        Assert.NotNull(cut.Find("[data-app-region=\"topbar\"]"));
    }

    [Fact]
    public void Content_Region_Has_Content_Data_Attribute()
    {
        var cut = Render<AppLayout>(parameters => parameters
            .Add(p => p.Body, "<span>body</span>"));

        Assert.NotNull(cut.Find("[data-app-region=\"content\"]"));
    }

    private sealed class EmptyNavigationRegistry : INavigationRegistry
    {
        public IReadOnlyList<RouteMetadata> Routes { get; } = Array.Empty<RouteMetadata>();

        public RouteMetadata? FindByHref(string href) => null;

        public IReadOnlyList<RouteMetadata> ChildrenOf(string? parentHref) => Array.Empty<RouteMetadata>();
    }
}
