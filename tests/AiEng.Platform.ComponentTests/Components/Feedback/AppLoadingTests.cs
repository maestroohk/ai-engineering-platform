using AiEng.Platform.App.Components.Feedback;
using Bunit;
using Xunit;

namespace AiEng.Platform.ComponentTests.Components.Feedback;

public class AppLoadingTests : BunitContext
{
    [Fact]
    public void Renders_Spinner_Element()
    {
        var cut = Render<AppLoading>();

        Assert.NotNull(cut.Find(".app-loading-spinner"));
        Assert.Equal("status", cut.Find(".app-loading").GetAttribute("role"));
    }

    [Fact]
    public void Without_Label_Omits_Label_Span()
    {
        var cut = Render<AppLoading>();

        Assert.Empty(cut.FindAll(".app-loading-label"));
    }

    [Fact]
    public void Label_Renders_When_Supplied()
    {
        var cut = Render<AppLoading>(parameters => parameters
            .Add(p => p.Label, "Loading data…"));

        Assert.Contains("Loading data…", cut.Find(".app-loading-label").TextContent);
    }
}
