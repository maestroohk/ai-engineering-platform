using AiEng.Platform.App.Components.Feedback;
using Bunit;
using Xunit;

namespace AiEng.Platform.ComponentTests.Components.Feedback;

public class AppSkeletonTests : BunitContext
{
    [Fact]
    public void Default_Renders_Three_Lines()
    {
        var cut = Render<AppSkeleton>();

        var lines = cut.FindAll(".app-skeleton-line");
        Assert.Equal(3, lines.Count);
    }

    [Fact]
    public void Custom_Lines_Renders_That_Many()
    {
        var cut = Render<AppSkeleton>(parameters => parameters
            .Add(p => p.Lines, 5));

        Assert.Equal(5, cut.FindAll(".app-skeleton-line").Count);
    }

    [Fact]
    public void Rounded_Adds_Rounded_Class()
    {
        var cut = Render<AppSkeleton>(parameters => parameters
            .Add(p => p.Rounded, true));

        Assert.NotNull(cut.Find(".app-skeleton-line-rounded"));
    }

    [Fact]
    public void Without_Rounded_Omits_Rounded_Class()
    {
        var cut = Render<AppSkeleton>();

        Assert.Empty(cut.FindAll(".app-skeleton-line-rounded"));
    }
}
