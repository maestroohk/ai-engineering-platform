using AiEng.Platform.App.Components.Shell;
using Bunit;
using Xunit;

namespace AiEng.Platform.ComponentTests.Shell;

public class AppTopBarSlotTests : BunitContext
{
    [Fact]
    public void Renders_App_Region_Element()
    {
        var cut = Render<AppTopBarSlot>();

        Assert.NotNull(cut.Find("section.app-region"));
    }

    [Fact]
    public void Has_Topbar_Data_Attribute()
    {
        var cut = Render<AppTopBarSlot>();

        Assert.NotNull(cut.Find("[data-app-region=\"topbar\"]"));
    }

    [Fact]
    public void Has_Topbar_Aria_Label()
    {
        var cut = Render<AppTopBarSlot>();

        var region = cut.Find("section.app-region");
        Assert.Equal("topbar", region.GetAttribute("aria-label"));
    }

    [Fact]
    public void Renders_App_Page_Header()
    {
        var cut = Render<AppTopBarSlot>();

        Assert.NotNull(cut.Find("header.app-page-header"));
    }

    [Fact]
    public void Renders_App_Alert_Placeholder()
    {
        var cut = Render<AppTopBarSlot>();

        Assert.NotNull(cut.Find(".app-alert"));
    }

    [Fact]
    public void Placeholder_Alert_Information_Variant()
    {
        var cut = Render<AppTopBarSlot>();

        Assert.Contains("app-alert-information", cut.Find(".app-alert").ClassList.ToString());
    }
}
