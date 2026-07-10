using AiEng.Platform.App.Components.Common;
using AiEng.Platform.App.Components.Primitive;
using Bunit;
using Xunit;

namespace AiEng.Platform.ComponentTests.Components.Primitive;

public class AppButtonTests : BunitContext
{
    [Fact]
    public void Primary_Renders_Button_With_App_Button_Primary_Class()
    {
        var cut = Render<AppButton>(parameters => parameters
            .Add(p => p.Variant, AppButtonVariant.Primary)
            .Add(p => p.ChildContent, "Save"));

        var button = cut.Find("button");
        Assert.Contains("app-button", button.ClassList.ToString());
        Assert.Contains("app-button-primary", button.ClassList.ToString());
        Assert.Equal("Save", button.TextContent.Trim());
    }

    [Fact]
    public void Each_Variant_Renders_Matching_Class()
    {
        var variants = new (AppButtonVariant variant, string expectedClass)[]
        {
            (AppButtonVariant.Primary, "app-button-primary"),
            (AppButtonVariant.Secondary, "app-button-secondary"),
            (AppButtonVariant.Outline, "app-button-outline"),
            (AppButtonVariant.Ghost, "app-button-ghost"),
            (AppButtonVariant.Danger, "app-button-danger"),
            (AppButtonVariant.Success, "app-button-success")
        };

        foreach (var (variant, expectedClass) in variants)
        {
            var cut = Render<AppButton>(parameters => parameters
                .Add(p => p.Variant, variant)
                .Add(p => p.ChildContent, "x"));

            Assert.Contains(expectedClass, cut.Find("button").ClassList.ToString());
        }
    }

    [Fact]
    public void Each_Size_Renders_Matching_Class()
    {
        var sizes = new (AppButtonSize size, string expectedClass)[]
        {
            (AppButtonSize.Small, "app-button-size-small"),
            (AppButtonSize.Medium, "app-button-size-medium"),
            (AppButtonSize.Large, "app-button-size-large")
        };

        foreach (var (size, expectedClass) in sizes)
        {
            var cut = Render<AppButton>(parameters => parameters
                .Add(p => p.Size, size)
                .Add(p => p.ChildContent, "x"));

            Assert.Contains(expectedClass, cut.Find("button").ClassList.ToString());
        }
    }

    [Fact]
    public void Disabled_Renders_Disabled_Attribute()
    {
        var cut = Render<AppButton>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.ChildContent, "x"));

        var button = cut.Find("button");
        Assert.True(button.HasAttribute("disabled"));
        Assert.Equal("true", button.GetAttribute("aria-disabled"));
    }

    [Fact]
    public void Loading_Renders_AriaBusy_And_Spinner()
    {
        var cut = Render<AppButton>(parameters => parameters
            .Add(p => p.Loading, true)
            .Add(p => p.ChildContent, "x"));

        var button = cut.Find("button");
        Assert.Equal("true", button.GetAttribute("aria-busy"));
        Assert.NotNull(cut.Find(".app-button-spinner"));
    }

    [Fact]
    public void Disabled_Suppresses_Click()
    {
        var clickCount = 0;
        var cut = Render<AppButton>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.ChildContent, "x")
            .Add(p => p.OnClick, _ => clickCount++));

        cut.Find("button").Click();
        Assert.Equal(0, clickCount);
    }

    [Fact]
    public void ChildContent_Renders()
    {
        var cut = Render<AppButton>(parameters => parameters
            .Add(p => p.ChildContent, "Hello world"));

        Assert.Equal("Hello world", cut.Find("button").TextContent.Trim());
    }

    [Fact]
    public void Click_Fires_OnClick_Handler()
    {
        var clickCount = 0;
        var cut = Render<AppButton>(parameters => parameters
            .Add(p => p.ChildContent, "x")
            .Add(p => p.OnClick, _ => clickCount++));

        cut.Find("button").Click();
        Assert.Equal(1, clickCount);
    }
}
