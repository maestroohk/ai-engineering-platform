using AiEng.Platform.App.Components.Common;
using AiEng.Platform.App.Components.Primitive;
using Bunit;
using Xunit;

namespace AiEng.Platform.ComponentTests.Components.Primitive;

public class AppContainerTests : BunitContext
{
    [Fact]
    public void Default_Renders_With_Max_Width_Class()
    {
        var cut = Render<AppContainer>(parameters => parameters
            .Add(p => p.ChildContent, "content"));

        var el = cut.Find(".app-container");
        Assert.Contains("app-container-default", el.ClassList.ToString());
    }

    [Fact]
    public void Fluid_Renders_Fluid_Class()
    {
        var cut = Render<AppContainer>(parameters => parameters
            .Add(p => p.Variant, AppContainerVariant.Fluid)
            .Add(p => p.ChildContent, "content"));

        Assert.Contains("app-container-fluid", cut.Find(".app-container").ClassList.ToString());
    }

    [Fact]
    public void ChildContent_Renders_Inside()
    {
        var cut = Render<AppContainer>(parameters => parameters
            .Add(p => p.ChildContent, "<span class=\"probe\">x</span>"));

        Assert.NotNull(cut.Find(".probe"));
    }
}
