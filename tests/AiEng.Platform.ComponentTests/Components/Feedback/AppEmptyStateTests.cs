using AiEng.Platform.App.Components.Feedback;
using Bunit;
using Xunit;

namespace AiEng.Platform.ComponentTests.Components.Feedback;

public class AppEmptyStateTests : BunitContext
{
    [Fact]
    public void Title_Renders()
    {
        var cut = Render<AppEmptyState>(parameters => parameters
            .Add(p => p.Title, "No data"));

        Assert.Contains("No data", cut.Find(".app-empty-state-title").TextContent);
    }

    [Fact]
    public void Description_Renders_When_Supplied()
    {
        var cut = Render<AppEmptyState>(parameters => parameters
            .Add(p => p.Title, "t")
            .Add(p => p.Description, "There is nothing here."));

        Assert.Contains("There is nothing here.", cut.Find(".app-empty-state-description").TextContent);
    }

    [Fact]
    public void Actions_Render_When_Supplied()
    {
        var cut = Render<AppEmptyState>(parameters => parameters
            .Add(p => p.Title, "t")
            .Add(p => p.Actions, "<button class=\"probe\">x</button>"));

        Assert.NotNull(cut.Find(".app-empty-state-actions .probe"));
    }

    [Fact]
    public void Icon_Renders_When_Supplied()
    {
        var cut = Render<AppEmptyState>(parameters => parameters
            .Add(p => p.Title, "t")
            .Add(p => p.Icon, "<span class=\"probe\">★</span>"));

        Assert.NotNull(cut.Find(".app-empty-state-icon .probe"));
    }
}
