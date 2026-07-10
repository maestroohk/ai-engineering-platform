using AiEng.Platform.App.Components.Common;
using AiEng.Platform.App.Components.Primitive;
using Bunit;
using Xunit;

namespace AiEng.Platform.ComponentTests.Components.Primitive;

public class AppStatusDotTests : BunitContext
{
    [Fact]
    public void Default_Renders_Neutral_Dot()
    {
        var cut = Render<AppStatusDot>();

        var dot = cut.Find(".app-status-dot-circle");
        Assert.Contains("app-status-dot-neutral", dot.ClassList.ToString());
    }

    [Fact]
    public void Each_Variant_Renders_Matching_Color_Class()
    {
        var variants = new (AppStatusDotVariant variant, string expectedClass)[]
        {
            (AppStatusDotVariant.Neutral, "app-status-dot-neutral"),
            (AppStatusDotVariant.Success, "app-status-dot-success"),
            (AppStatusDotVariant.Warning, "app-status-dot-warning"),
            (AppStatusDotVariant.Error, "app-status-dot-error"),
            (AppStatusDotVariant.Info, "app-status-dot-info")
        };

        foreach (var (variant, expectedClass) in variants)
        {
            var cut = Render<AppStatusDot>(parameters => parameters
                .Add(p => p.Variant, variant));

            Assert.Contains(expectedClass, cut.Find(".app-status-dot-circle").ClassList.ToString());
        }
    }

    [Fact]
    public void Label_Renders_When_Supplied()
    {
        var cut = Render<AppStatusDot>(parameters => parameters
            .Add(p => p.Label, "Healthy"));

        Assert.Contains("Healthy", cut.Markup);
        Assert.Contains("app-status-dot-with-label", cut.Find("span.app-status-dot").ClassList.ToString());
    }

    [Fact]
    public void Without_Label_Omits_Label_Span()
    {
        var cut = Render<AppStatusDot>();

        Assert.Empty(cut.FindAll(".app-status-dot-label"));
    }
}
