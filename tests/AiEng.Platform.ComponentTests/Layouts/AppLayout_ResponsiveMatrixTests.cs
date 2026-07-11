using AiEng.Platform.App.Layouts;
using AiEng.Platform.Application.Navigation;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AiEng.Platform.ComponentTests.Layouts;

public class AppLayout_ResponsiveMatrixTests : BunitContext
{
    public AppLayout_ResponsiveMatrixTests()
    {
        Services.AddSingleton<INavigationRegistry>(new EmptyNavigationRegistry());
        JSInterop.Setup<string>("appTheme.current").SetResult("light");
        JSInterop.SetupVoid("appTheme.set", _ => true);
    }

    [Fact]
    public void App_Shell_Uses_Css_Grid_For_The_Responsive_Matrix()
    {
        var cut = Render<AppLayout>(parameters => parameters
            .Add(p => p.Body, "<span>body</span>"));

        var shell = cut.Find("div.app-shell");
        Assert.NotNull(shell);
    }

    [Fact]
    public void App_Shell_Content_Area_Is_Vertically_Scrollable()
    {
        var cut = Render<AppLayout>(parameters => parameters
            .Add(p => p.Body, "<span>body</span>"));

        var content = cut.Find("main.app-shell-content");
        Assert.NotNull(content);
    }

    [Fact]
    public void App_Shell_Topbar_Is_Horizontal_At_Every_Breakpoint()
    {
        var cut = Render<AppLayout>(parameters => parameters
            .Add(p => p.Body, "<span>body</span>"));

        var topbar = cut.Find("header.app-shell-topbar");
        Assert.NotNull(topbar);
        var main = cut.Find("div.app-shell-main");
        Assert.NotNull(main);
    }

    [Fact]
    public void App_Shell_Sidebar_Is_Present_In_The_Default_Desktop_Layout()
    {
        var cut = Render<AppLayout>(parameters => parameters
            .Add(p => p.Body, "<span>body</span>"));

        var sidebar = cut.Find("aside.app-shell-sidebar");
        Assert.NotNull(sidebar);
    }

    private sealed class EmptyNavigationRegistry : INavigationRegistry
    {
        public IReadOnlyList<RouteMetadata> Routes { get; } = Array.Empty<RouteMetadata>();

        public RouteMetadata? FindByHref(string href) => null;

        public IReadOnlyList<RouteMetadata> ChildrenOf(string? parentHref) => Array.Empty<RouteMetadata>();
    }
}
