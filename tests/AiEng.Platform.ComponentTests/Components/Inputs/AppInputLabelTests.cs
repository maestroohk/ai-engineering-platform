using AiEng.Platform.App.Components.Inputs;
using Bunit;
using Xunit;

namespace AiEng.Platform.ComponentTests.Components.Inputs;

public class AppInputLabelTests : BunitContext
{
    [Fact]
    public void Renders_Label_With_For_Attribute()
    {
        var cut = Render<AppInputLabel>(parameters => parameters
            .Add(p => p.For, "username"));

        var label = cut.Find("label.app-input-label");
        Assert.Equal("username", label.GetAttribute("for"));
    }

    [Fact]
    public void ChildContent_Renders_Inside_Label()
    {
        var cut = Render<AppInputLabel>(parameters => parameters
            .Add(p => p.For, "x")
            .Add(p => p.ChildContent, "Username"));

        Assert.Contains("Username", label_cut_text(cut));
    }

    [Fact]
    public void Without_Required_Omits_Required_Marker()
    {
        var cut = Render<AppInputLabel>(parameters => parameters
            .Add(p => p.For, "x")
            .Add(p => p.ChildContent, "Username"));

        Assert.Empty(cut.FindAll(".app-input-label-required"));
        Assert.Equal("false", cut.Find("label.app-input-label").GetAttribute("aria-required"));
    }

    [Fact]
    public void Required_Adds_Required_Class_And_Marker_And_Aria()
    {
        var cut = Render<AppInputLabel>(parameters => parameters
            .Add(p => p.For, "x")
            .Add(p => p.Required, true)
            .Add(p => p.ChildContent, "Email"));

        var label = cut.Find("label.app-input-label");
        Assert.Contains("app-input-label-required", label.ClassList.ToString());
        Assert.Equal("true", label.GetAttribute("aria-required"));
        Assert.NotNull(cut.Find(".app-input-label-required"));
    }

    private static string label_cut_text(IRenderedComponent<AppInputLabel> cut) =>
        cut.Find("label.app-input-label").TextContent;
}
