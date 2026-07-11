using System.Collections.Generic;
using AiEng.Platform.App.Components.Shell;
using Bunit;
using Xunit;

namespace AiEng.Platform.ComponentTests.Shell;

public class AppThemeToggleTests : BunitContext
{
    public AppThemeToggleTests()
    {
        JSInterop.Setup<string>("appTheme.current").SetResult("light");
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

    [Fact]
    public void Reads_Resolved_Theme_From_AppTheme_Current_On_First_Render()
    {
        var ctx = new BunitContext();
        ctx.JSInterop.Setup<string>("appTheme.current").SetResult("dark");
        ctx.JSInterop.SetupVoid("appTheme.set", _ => true);

        var cut = ctx.Render<AppThemeToggle>();
        cut.WaitForState(() => cut.Find("button.app-theme-toggle").GetAttribute("aria-pressed") == "true");

        var button = cut.Find("button.app-theme-toggle");
        Assert.Equal("true", button.GetAttribute("aria-pressed"));
        Assert.Contains("app-theme-toggle-dark", button.ClassList.ToString());
    }

    [Fact]
    public void Click_Updates_State_From_Light_To_Dark_Immediately()
    {
        var ctx = new BunitContext();
        ctx.JSInterop.Setup<string>("appTheme.current").SetResult("light");
        ctx.JSInterop.SetupVoid("appTheme.set", _ => true);

        var cut = ctx.Render<AppThemeToggle>();
        cut.Find("button.app-theme-toggle").Click();
        var button = cut.Find("button.app-theme-toggle");

        Assert.Equal("true", button.GetAttribute("aria-pressed"));
        Assert.Contains("app-theme-toggle-dark", button.ClassList.ToString());
    }

    [Fact]
    public void Click_Updates_State_From_Dark_To_Light_Immediately()
    {
        var ctx = new BunitContext();
        ctx.JSInterop.Setup<string>("appTheme.current").SetResult("dark");
        ctx.JSInterop.SetupVoid("appTheme.set", _ => true);

        var cut = ctx.Render<AppThemeToggle>();
        cut.WaitForState(() => cut.Find("button.app-theme-toggle").GetAttribute("aria-pressed") == "true");
        cut.Find("button.app-theme-toggle").Click();
        var button = cut.Find("button.app-theme-toggle");

        Assert.Equal("false", button.GetAttribute("aria-pressed"));
        Assert.Contains("app-theme-toggle-light", button.ClassList.ToString());
    }

    [Fact]
    public void Click_Calls_AppTheme_Set_With_Toggled_Value()
    {
        var ctx = new BunitContext();
        ctx.JSInterop.Setup<string>("appTheme.current").SetResult("light");
        ctx.JSInterop.SetupVoid("appTheme.set", _ => true);

        var cut = ctx.Render<AppThemeToggle>();
        cut.Find("button.app-theme-toggle").Click();

        ctx.JSInterop.VerifyInvoke("appTheme.set", calledTimes: 1);
    }
    [Fact]
    public void Click_Invokes_AppTheme_Set_With_Dark_After_Light_Initial_State()
    {
        var ctx = new BunitContext();
        ctx.JSInterop.Setup<string>("appTheme.current").SetResult("light");
        var setCalls = new List<Bunit.JSRuntimeInvocation>();
        ctx.JSInterop.SetupVoid("appTheme.set", invocation =>
        {
            setCalls.Add(invocation);
            return true;
        });

        var cut = ctx.Render<AppThemeToggle>();
        cut.Find("button.app-theme-toggle").Click();

        Assert.Single(setCalls);
        Assert.Equal("dark", setCalls[0].Arguments[0]);
    }
}
