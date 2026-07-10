using AiEng.Platform.App.Components.Common;
using AiEng.Platform.App.Components.Primitive;
using Bunit;
using Xunit;

namespace AiEng.Platform.ComponentTests.Components.Primitive;

public class AppBadgeTests : BunitContext
{
    [Fact]
    public void Default_Renders_Neutral_Class()
    {
        var cut = Render<AppBadge>(parameters => parameters
            .Add(p => p.ChildContent, "Tag"));

        var badge = cut.Find("span.app-badge");
        Assert.Contains("app-badge-neutral", badge.ClassList.ToString());
        Assert.Contains("Tag", badge.TextContent);
    }

    [Fact]
    public void Each_Variant_Renders_Matching_Class()
    {
        var variants = new (AppBadgeVariant variant, string expectedClass)[]
        {
            (AppBadgeVariant.Neutral, "app-badge-neutral"),
            (AppBadgeVariant.Accent, "app-badge-accent"),
            (AppBadgeVariant.Success, "app-badge-success"),
            (AppBadgeVariant.Warning, "app-badge-warning"),
            (AppBadgeVariant.Error, "app-badge-error"),
            (AppBadgeVariant.Info, "app-badge-info")
        };

        foreach (var (variant, expectedClass) in variants)
        {
            var cut = Render<AppBadge>(parameters => parameters
                .Add(p => p.Variant, variant)
                .Add(p => p.ChildContent, "x"));

            Assert.Contains(expectedClass, cut.Find("span.app-badge").ClassList.ToString());
        }
    }

    [Fact]
    public void Dot_Renders_Dot_Element()
    {
        var cut = Render<AppBadge>(parameters => parameters
            .Add(p => p.Dot, true)
            .Add(p => p.ChildContent, "Live"));

        Assert.NotNull(cut.Find(".app-badge-dot"));
    }

    [Fact]
    public void Without_Dot_Omits_Dot_Element()
    {
        var cut = Render<AppBadge>(parameters => parameters
            .Add(p => p.ChildContent, "x"));

        Assert.Empty(cut.FindAll(".app-badge-dot"));
    }
}
