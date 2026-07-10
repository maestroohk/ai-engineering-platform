using AiEng.Platform.App.Components.Layout;
using Bunit;
using Xunit;

namespace AiEng.Platform.ComponentTests.Components.Layout;

public class AppSectionTests : BunitContext
{
    [Fact]
    public void Renders_Section_With_App_Section_Class()
    {
        var cut = Render<AppSection>(parameters => parameters
            .Add(p => p.Title, "Heading")
            .Add(p => p.Content, "Body"));

        Assert.NotNull(cut.Find("section.app-section"));
    }

    [Fact]
    public void Title_Renders_In_Header()
    {
        var cut = Render<AppSection>(parameters => parameters
            .Add(p => p.Title, "Section title")
            .Add(p => p.Content, "Body"));

        Assert.Contains("Section title", cut.Find(".app-section-title").TextContent);
    }

    [Fact]
    public void Subtitle_Renders_When_Supplied()
    {
        var cut = Render<AppSection>(parameters => parameters
            .Add(p => p.Title, "t")
            .Add(p => p.Subtitle, "sub")
            .Add(p => p.Content, "Body"));

        Assert.NotNull(cut.Find(".app-section-subtitle"));
        Assert.Contains("sub", cut.Find(".app-section-subtitle").TextContent);
    }

    [Fact]
    public void Actions_Render_When_Supplied()
    {
        var cut = Render<AppSection>(parameters => parameters
            .Add(p => p.Title, "t")
            .Add(p => p.Actions, "<button class=\"probe\">x</button>")
            .Add(p => p.Content, "Body"));

        Assert.NotNull(cut.Find(".app-section-actions .probe"));
    }

    [Fact]
    public void Without_Header_Omits_Header_Element()
    {
        var cut = Render<AppSection>(parameters => parameters
            .Add(p => p.Content, "Body"));

        Assert.Empty(cut.FindAll(".app-section-header"));
    }

    [Fact]
    public void Content_Renders_In_Body()
    {
        var cut = Render<AppSection>(parameters => parameters
            .Add(p => p.Content, "<span class=\"probe\">x</span>"));

        Assert.NotNull(cut.Find(".app-section-content .probe"));
    }
}
