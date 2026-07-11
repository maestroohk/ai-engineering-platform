using AiEng.Platform.App.Layouts;
using AiEng.Platform.Application.Navigation;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AiEng.Platform.ComponentTests.Layouts;

public class AppLayout_ThemeToggleWiringTests : BunitContext
{
    public AppLayout_ThemeToggleWiringTests()
    {
        Services.AddSingleton<INavigationRegistry>(new EmptyNavigationRegistry());
        JSInterop.Setup<string>("appTheme.current").SetResult("light");
        JSInterop.SetupVoid("appTheme.set", _ => true);
    }

    [Fact]
    public void Layout_Renders_App_Theme_Toggle_Inside_The_Topbar_Region()
    {
        var cut = Render<AppLayout>(parameters => parameters
            .Add(p => p.Body, "<span>body</span>"));

        var topbar = cut.Find("header.app-shell-topbar");
        Assert.NotNull(topbar.QuerySelector("button.app-theme-toggle"));
    }

    [Fact]
    public void Clicking_The_Theme_Toggle_In_The_Layout_Invokes_AppTheme_Set()
    {
        var cut = Render<AppLayout>(parameters => parameters
            .Add(p => p.Body, "<span>body</span>"));

        var toggle = cut.Find("header.app-shell-topbar button.app-theme-toggle");
        toggle.Click();

        JSInterop.VerifyInvoke("appTheme.set", calledTimes: 1);
    }

    [Fact]
    public void Clicking_The_Theme_Toggle_In_The_Layout_Passes_Dark_From_Light_Initial_State()
    {
        var setCalls = new List<Bunit.JSRuntimeInvocation>();
        var ctx = new BunitContext();
        ctx.Services.AddSingleton<INavigationRegistry>(new EmptyNavigationRegistry());
        ctx.JSInterop.Setup<string>("appTheme.current").SetResult("light");
        ctx.JSInterop.SetupVoid("appTheme.set", invocation =>
        {
            setCalls.Add(invocation);
            return true;
        });

        var cut = ctx.Render<AppLayout>(parameters => parameters
            .Add(p => p.Body, "<span>body</span>"));

        cut.Find("header.app-shell-topbar button.app-theme-toggle").Click();

        Assert.Single(setCalls);
        Assert.Equal("dark", setCalls[0].Arguments[0]);
    }

    [Fact]
    public void Clicking_The_Theme_Toggle_In_The_Layout_Updates_Aria_Pressed_On_The_Toggle()
    {
        var cut = Render<AppLayout>(parameters => parameters
            .Add(p => p.Body, "<span>body</span>"));

        var toggle = cut.Find("header.app-shell-topbar button.app-theme-toggle");
        toggle.Click();

        Assert.Equal("true", toggle.GetAttribute("aria-pressed"));
        Assert.Contains("app-theme-toggle-dark", toggle.ClassList.ToString());
    }

    private sealed class EmptyNavigationRegistry : INavigationRegistry
    {
        public IReadOnlyList<RouteMetadata> Routes { get; } = Array.Empty<RouteMetadata>();

        public RouteMetadata? FindByHref(string href) => null;

        public IReadOnlyList<RouteMetadata> ChildrenOf(string? parentHref) => Array.Empty<RouteMetadata>();
    }
}
