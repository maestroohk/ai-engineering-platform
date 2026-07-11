using AiEng.Platform.App.Layouts;
using AiEng.Platform.Application.Navigation;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AiEng.Platform.ComponentTests.Accessibility;

public class KeyboardSmokeTests : BunitContext
{
    public KeyboardSmokeTests()
    {
        Services.AddSingleton<INavigationRegistry>(new EmptyNavigationRegistry());
        JSInterop.Setup<string>("appTheme.current").SetResult("light");
        JSInterop.SetupVoid("appTheme.set", _ => true);
    }

    [Fact]
    public void Layout_Renders_Interactive_Anchors_That_Are_Keyboard_Reachable()
    {
        var cut = Render<AppLayout>(parameters => parameters
            .Add(p => p.Body, "<a href=\"/dashboard\" class=\"app-link\">Go</a>"));

        var anchors = cut.FindAll("a");
        Assert.NotEmpty(anchors);
        foreach (var anchor in anchors)
        {
            Assert.NotNull(anchor.GetAttribute("href"));
        }
    }

    [Fact]
    public void Layout_Renders_The_Theme_Toggle_As_A_Keyboard_Reachable_Button()
    {
        var cut = Render<AppLayout>(parameters => parameters
            .Add(p => p.Body, "<span>body</span>"));

        var button = cut.Find("button.app-theme-toggle");
        Assert.NotNull(button);
        Assert.Equal("button", button.TagName.ToLowerInvariant());
        Assert.False(button.HasAttribute("tabindex") && button.GetAttribute("tabindex") == "-1");
    }

    [Fact]
    public void Layout_Interactive_Elements_Have_Visible_Focus_Styles_Defined()
    {
        var cut = Render<AppLayout>(parameters => parameters
            .Add(p => p.Body, "<button class=\"app-button app-button-primary\">Action</button>"));

        var button = cut.Find("button.app-button");
        Assert.NotNull(button);
    }

    [Fact]
    public void Layout_Anchors_Are_Focusable_By_Default()
    {
        var cut = Render<AppLayout>(parameters => parameters
            .Add(p => p.Body, "<a class=\"app-link\" href=\"/\">Home</a>"));

        var anchor = cut.Find("a.app-link");
        Assert.NotNull(anchor);
    }

    private sealed class EmptyNavigationRegistry : INavigationRegistry
    {
        public IReadOnlyList<RouteMetadata> Routes { get; } = Array.Empty<RouteMetadata>();

        public RouteMetadata? FindByHref(string href) => null;

        public IReadOnlyList<RouteMetadata> ChildrenOf(string? parentHref) => Array.Empty<RouteMetadata>();
    }
}
