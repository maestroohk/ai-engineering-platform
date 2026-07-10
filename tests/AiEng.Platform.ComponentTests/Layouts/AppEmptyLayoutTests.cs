using AiEng.Platform.App.Layouts;
using Bunit;
using Xunit;

namespace AiEng.Platform.ComponentTests.Layouts;

public class AppEmptyLayoutTests : BunitContext
{
    [Fact]
    public void Default_Renders_App_Shell_Empty_Root()
    {
        var cut = Render<AppEmptyLayout>(parameters => parameters
            .Add(p => p.Body, "<span>body</span>"));

        Assert.NotNull(cut.Find("div.app-shell-empty"));
    }

    [Fact]
    public void Renders_Content_Region()
    {
        var cut = Render<AppEmptyLayout>(parameters => parameters
            .Add(p => p.Body, "<span>body</span>"));

        Assert.NotNull(cut.Find("main.app-shell-empty-content"));
    }

    [Fact]
    public void Body_Content_Is_Rendered()
    {
        var cut = Render<AppEmptyLayout>(parameters => parameters
            .Add(p => p.Body, "<span class=\"probe\">page-body</span>"));

        Assert.NotNull(cut.Find(".app-shell-empty-content .probe"));
    }

    [Fact]
    public void Content_Region_Has_Content_Data_Attribute()
    {
        var cut = Render<AppEmptyLayout>(parameters => parameters
            .Add(p => p.Body, "<span>body</span>"));

        Assert.NotNull(cut.Find("[data-app-region=\"content\"]"));
    }

    [Fact]
    public void Does_Not_Render_Sidebar_Region()
    {
        var cut = Render<AppEmptyLayout>(parameters => parameters
            .Add(p => p.Body, "<span>body</span>"));

        Assert.Empty(cut.FindAll("aside.app-shell-sidebar"));
    }

    [Fact]
    public void Does_Not_Render_Topbar_Region()
    {
        var cut = Render<AppEmptyLayout>(parameters => parameters
            .Add(p => p.Body, "<span>body</span>"));

        Assert.Empty(cut.FindAll("header.app-shell-topbar"));
    }
}
