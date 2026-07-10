using AiEng.Platform.App.Components.Common;
using AiEng.Platform.App.Components.Primitive;
using Bunit;
using Xunit;

namespace AiEng.Platform.ComponentTests.Components.Primitive;

public class AppStackTests : BunitContext
{
    [Fact]
    public void Default_Renders_Vertical_Flex_Container()
    {
        var cut = Render<AppStack>(parameters => parameters
            .Add(p => p.ChildContent, "child"));

        var el = cut.Find(".app-stack");
        Assert.Contains("flex-col", el.ClassList.ToString());
    }

    [Fact]
    public void Horizontal_Renders_Flex_Row()
    {
        var cut = Render<AppStack>(parameters => parameters
            .Add(p => p.Direction, AppStackDirection.Horizontal)
            .Add(p => p.ChildContent, "child"));

        Assert.Contains("flex-row", cut.Find(".app-stack").ClassList.ToString());
    }

    [Fact]
    public void Each_Gap_Adds_Tailwind_Class()
    {
        var gaps = new (AppStackGap gap, string expectedClass)[]
        {
            (AppStackGap.None, "gap-0"),
            (AppStackGap.ExtraSmall, "gap-1"),
            (AppStackGap.Small, "gap-2"),
            (AppStackGap.Medium, "gap-4"),
            (AppStackGap.Large, "gap-6"),
            (AppStackGap.ExtraLarge, "gap-8")
        };

        foreach (var (gap, expectedClass) in gaps)
        {
            var cut = Render<AppStack>(parameters => parameters
                .Add(p => p.Gap, gap)
                .Add(p => p.ChildContent, "x"));

            Assert.Contains(expectedClass, cut.Find(".app-stack").ClassList.ToString());
        }
    }

    [Fact]
    public void Align_Center_Renders_Items_Center()
    {
        var cut = Render<AppStack>(parameters => parameters
            .Add(p => p.Align, AppStackAlign.Center)
            .Add(p => p.ChildContent, "x"));

        Assert.Contains("items-center", cut.Find(".app-stack").ClassList.ToString());
    }

    [Fact]
    public void Justify_Between_Renders_Justify_Between()
    {
        var cut = Render<AppStack>(parameters => parameters
            .Add(p => p.Justify, AppStackJustify.Between)
            .Add(p => p.ChildContent, "x"));

        Assert.Contains("justify-between", cut.Find(".app-stack").ClassList.ToString());
    }

    [Fact]
    public void Wrap_Renders_Flex_Wrap()
    {
        var cut = Render<AppStack>(parameters => parameters
            .Add(p => p.Wrap, AppStackWrap.Wrap)
            .Add(p => p.ChildContent, "x"));

        Assert.Contains("flex-wrap", cut.Find(".app-stack").ClassList.ToString());
    }

    [Fact]
    public void ChildContent_Renders_Inside()
    {
        var cut = Render<AppStack>(parameters => parameters
            .Add(p => p.ChildContent, "<span class=\"probe\">hello</span>"));

        Assert.NotNull(cut.Find(".probe"));
    }
}
