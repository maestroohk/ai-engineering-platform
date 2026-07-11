using AiEng.Platform.App.Components.Navigation;
using Bunit;
using Xunit;

namespace AiEng.Platform.ComponentTests.Navigation;

public class AppSidebarItemTests : BunitContext
{
    [Fact]
    public void Renders_a_section_with_a_title_heading_when_Title_is_set()
    {
        var cut = Render<AppSidebarItem>(parameters => parameters
            .Add(p => p.Title, "Projects"));

        Assert.Contains("Projects", cut.Find(".app-sidebar-item-title").TextContent);
    }

    [Fact]
    public void Renders_ChildContent_inside_an_AppStack()
    {
        var cut = Render<AppSidebarItem>(parameters => parameters
            .Add(p => p.Title, "Projects")
            .AddChildContent("<span class='probe'>x</span>"));

        Assert.NotNull(cut.Find(".app-stack .probe"));
    }

    [Fact]
    public void Aria_label_matches_the_Title()
    {
        var cut = Render<AppSidebarItem>(parameters => parameters
            .Add(p => p.Title, "Projects"));

        Assert.Equal("Projects", cut.Find("section.app-sidebar-item").GetAttribute("aria-label"));
    }

    [Fact]
    public void AppStack_wraps_the_child_content()
    {
        var cut = Render<AppSidebarItem>(parameters => parameters
            .Add(p => p.Title, "Projects")
            .AddChildContent("<span class='probe'>x</span>"));

        var stack = cut.Find("section.app-sidebar-item .app-stack");
        Assert.NotNull(stack);
        Assert.NotNull(stack.QuerySelector(".probe"));
    }
}
