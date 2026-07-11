using AiEng.Platform.App.Components.Shell;
using Bunit;
using Xunit;

namespace AiEng.Platform.ComponentTests.Shell;

public class AppThemeToggleTests : BunitContext
{
    public AppThemeToggleTests()
    {
        JSInterop.Setup<string?>("appTheme.get").SetResult(null);
        JSInterop.SetupVoid("appTheme.set", _ => true);
    }
    [Fact]
    public void Renders_A_Button_With_App_Theme_Toggle_Class()
    {
        var cut = Render<AppThemeToggle>();

        Assert.NotNull(cut.Find("button.app-theme-toggle"));
    }

    [Fact]
    public void Default_State_Is_Light()
    {
        var cut = Render<AppThemeToggle>();

        var button = cut.Find("button.app-theme-toggle");
        Assert.Equal("false", button.GetAttribute("aria-pressed"));
        Assert.Contains("app-theme-toggle-light", button.ClassList.ToString());
    }

    [Fact]
    public void Aria_Label_Defaults_To_Toggle_Colour_Theme()
    {
        var cut = Render<AppThemeToggle>();

        Assert.Equal("Toggle colour theme", cut.Find("button.app-theme-toggle").GetAttribute("aria-label"));
    }

    [Fact]
    public void Aria_Label_Uses_Explicit_Value_When_Provided()
    {
        var cut = Render<AppThemeToggle>(parameters => parameters
            .Add(p => p.AriaLabel, "Switch theme"));

        Assert.Equal("Switch theme", cut.Find("button.app-theme-toggle").GetAttribute("aria-label"));
    }

    [Fact]
    public void Click_Toggles_Aria_Pressed_And_Class()
    {
        var cut = Render<AppThemeToggle>();
        var button = cut.Find("button.app-theme-toggle");

        cut.Find("button.app-theme-toggle").Click();
        button = cut.Find("button.app-theme-toggle");
        Assert.Equal("true", button.GetAttribute("aria-pressed"));
        Assert.Contains("app-theme-toggle-dark", button.ClassList.ToString());
    }

    [Fact]
    public void Title_Attribute_Defaults_To_Toggle_Light_Dark_Theme()
    {
        var cut = Render<AppThemeToggle>();

        Assert.Equal("Toggle light / dark theme", cut.Find("button.app-theme-toggle").GetAttribute("title"));
    }
}
