using AiEng.Platform.App.Components.Shell;
using Bunit;
using Xunit;

namespace AiEng.Platform.ComponentTests.Shell;

public class AppSidebarSlotTests : BunitContext
{
    [Fact]
    public void Renders_App_Region_Element()
    {
        var cut = Render<AppSidebarSlot>();

        Assert.NotNull(cut.Find("section.app-region"));
    }

    [Fact]
    public void Has_Sidebar_Data_Attribute()
    {
        var cut = Render<AppSidebarSlot>();

        Assert.NotNull(cut.Find("[data-app-region=\"sidebar\"]"));
    }

    [Fact]
    public void Has_Sidebar_Aria_Label()
    {
        var cut = Render<AppSidebarSlot>();

        var region = cut.Find("section.app-region");
        Assert.Equal("sidebar", region.GetAttribute("aria-label"));
    }

    [Fact]
    public void Renders_App_Empty_State_Placeholder()
    {
        var cut = Render<AppSidebarSlot>();

        Assert.NotNull(cut.Find(".app-empty-state"));
    }

    [Fact]
    public void Placeholder_Title_Is_Visible()
    {
        var cut = Render<AppSidebarSlot>();

        Assert.Contains("Sidebar lands in M2.2", cut.Find(".app-empty-state-title").TextContent);
    }
}
