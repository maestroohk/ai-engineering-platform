using AiEng.Platform.App.Components.Display;
using Bunit;
using Xunit;

namespace AiEng.Platform.ComponentTests.Components.Display;

public class AppPageHeaderTests : BunitContext
{
    [Fact]
    public void Renders_Header_With_App_Page_Header_Class()
    {
        var cut = Render<AppPageHeader>(parameters => parameters
            .Add(p => p.Title, "Page"));

        Assert.NotNull(cut.Find("header.app-page-header"));
    }

    [Fact]
    public void Title_Renders()
    {
        var cut = Render<AppPageHeader>(parameters => parameters
            .Add(p => p.Title, "My page"));

        Assert.Contains("My page", cut.Find(".app-page-header-title").TextContent);
    }

    [Fact]
    public void Description_Renders_When_Supplied()
    {
        var cut = Render<AppPageHeader>(parameters => parameters
            .Add(p => p.Title, "t")
            .Add(p => p.Description, "A short description"));

        Assert.Contains("A short description", cut.Find(".app-page-header-description").TextContent);
    }

    [Fact]
    public void Actions_Render_When_Supplied()
    {
        var cut = Render<AppPageHeader>(parameters => parameters
            .Add(p => p.Title, "t")
            .Add(p => p.Actions, "<button class=\"probe\">x</button>"));

        Assert.NotNull(cut.Find(".app-page-header-actions .probe"));
    }

    [Fact]
    public void Without_Description_Omits_Description_Element()
    {
        var cut = Render<AppPageHeader>(parameters => parameters
            .Add(p => p.Title, "t"));

        Assert.Empty(cut.FindAll(".app-page-header-description"));
    }
}
