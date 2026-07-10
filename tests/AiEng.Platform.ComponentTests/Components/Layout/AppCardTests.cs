using AiEng.Platform.App.Components.Common;
using AiEng.Platform.App.Components.Layout;
using Bunit;
using Xunit;

namespace AiEng.Platform.ComponentTests.Components.Layout;

public class AppCardTests : BunitContext
{
    [Fact]
    public void Default_Renders_With_App_Card_Default_Class()
    {
        var cut = Render<AppCard>(parameters => parameters
            .Add(p => p.ChildContent, "body"));

        Assert.Contains("app-card-default", cut.Find("section.app-card").ClassList.ToString());
    }

    [Fact]
    public void Header_Renders_When_Supplied()
    {
        var cut = Render<AppCard>(parameters => parameters
            .Add(p => p.Header, "<span class=\"probe\">title</span>")
            .Add(p => p.ChildContent, "body"));

        Assert.NotNull(cut.Find(".app-card-header .probe"));
    }

    [Fact]
    public void Without_Header_Omits_Header_Element()
    {
        var cut = Render<AppCard>(parameters => parameters
            .Add(p => p.ChildContent, "body"));

        Assert.Empty(cut.FindAll(".app-card-header"));
    }

    [Fact]
    public void Body_Renders_ChildContent()
    {
        var cut = Render<AppCard>(parameters => parameters
            .Add(p => p.ChildContent, "<span class=\"probe\">x</span>"));

        Assert.NotNull(cut.Find(".app-card-body .probe"));
    }

    [Fact]
    public void Footer_Renders_When_Supplied()
    {
        var cut = Render<AppCard>(parameters => parameters
            .Add(p => p.Footer, "<span class=\"probe\">foot</span>")
            .Add(p => p.ChildContent, "body"));

        Assert.NotNull(cut.Find(".app-card-footer .probe"));
    }

    [Fact]
    public void Flat_Variant_Renders_Flat_Class()
    {
        var cut = Render<AppCard>(parameters => parameters
            .Add(p => p.Variant, AppCardVariant.Flat)
            .Add(p => p.ChildContent, "body"));

        Assert.Contains("app-card-flat", cut.Find("section.app-card").ClassList.ToString());
    }
}
