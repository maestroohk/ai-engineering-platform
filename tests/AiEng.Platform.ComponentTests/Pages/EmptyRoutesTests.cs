using AiEng.Platform.App.Layouts;
using AiEng.Platform.Application.Navigation;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AiEng.Platform.ComponentTests.Pages;

public class EmptyRoutesTests : BunitContext
{
    public EmptyRoutesTests()
    {
        Services.AddSingleton<INavigationRegistry>(new EmptyNavigationRegistry());
        JSInterop.Setup<string>("appTheme.current").SetResult("light");
        JSInterop.SetupVoid("appTheme.set", _ => true);
    }

    [Fact]
    public void Home_Renders_An_App_Empty_State_With_A_Dashboard_Link()
    {
        var cut = Render<AiEng.Platform.App.Components.Pages.Home>();

        Assert.NotNull(cut.Find(".app-empty-state"));
        var links = cut.FindAll(".app-empty-state a");
        Assert.Contains(links, a => a.GetAttribute("href") == "/dashboard");
    }

    [Fact]
    public void Home_Renders_An_App_Empty_State_With_A_Design_System_Link()
    {
        var cut = Render<AiEng.Platform.App.Components.Pages.Home>();

        var links = cut.FindAll(".app-empty-state a");
        Assert.Contains(links, a => a.GetAttribute("href") == "/design-system");
    }

    [Fact]
    public void Home_Declares_App_Layout_As_Its_Layout()
    {
        var attribute = typeof(AiEng.Platform.App.Components.Pages.Home)
            .GetCustomAttributes(typeof(Microsoft.AspNetCore.Components.LayoutAttribute), false)
            .Cast<Microsoft.AspNetCore.Components.LayoutAttribute>()
            .SingleOrDefault();

        Assert.NotNull(attribute);
        Assert.Equal(typeof(AiEng.Platform.App.Layouts.AppLayout), attribute.LayoutType);
    }

    [Fact]
    public void NotFound_Renders_An_App_Empty_State_With_A_Home_Link()
    {
        var cut = Render<AiEng.Platform.App.Components.Pages.NotFound>();

        Assert.NotNull(cut.Find(".app-empty-state"));
        var links = cut.FindAll(".app-empty-state a");
        Assert.Contains(links, a => a.GetAttribute("href") == "/");
    }

    [Fact]
    public void NotFound_Declares_App_Empty_Layout_As_Its_Layout()
    {
        var attribute = typeof(AiEng.Platform.App.Components.Pages.NotFound)
            .GetCustomAttributes(typeof(Microsoft.AspNetCore.Components.LayoutAttribute), false)
            .Cast<Microsoft.AspNetCore.Components.LayoutAttribute>()
            .SingleOrDefault();

        Assert.NotNull(attribute);
        Assert.Equal(typeof(AiEng.Platform.App.Layouts.AppEmptyLayout), attribute.LayoutType);
    }

    private sealed class EmptyNavigationRegistry : INavigationRegistry
    {
        public IReadOnlyList<RouteMetadata> Routes { get; } = Array.Empty<RouteMetadata>();

        public RouteMetadata? FindByHref(string href) => null;

        public IReadOnlyList<RouteMetadata> ChildrenOf(string? parentHref) => Array.Empty<RouteMetadata>();
    }
}
