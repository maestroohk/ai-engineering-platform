using AiEng.Platform.App.Components.Common;
using AiEng.Platform.App.Components.Feedback;
using Bunit;
using Xunit;

namespace AiEng.Platform.ComponentTests.Components.Feedback;

public class AppAlertTests : BunitContext
{
    [Fact]
    public void Each_Variant_Renders_Matching_Class()
    {
        var variants = new (AppAlertVariant variant, string expectedClass)[]
        {
            (AppAlertVariant.Information, "app-alert-information"),
            (AppAlertVariant.Success, "app-alert-success"),
            (AppAlertVariant.Warning, "app-alert-warning"),
            (AppAlertVariant.Error, "app-alert-error")
        };

        foreach (var (variant, expectedClass) in variants)
        {
            var cut = Render<AppAlert>(parameters => parameters
                .Add(p => p.Variant, variant)
                .Add(p => p.Title, "t"));

            Assert.Contains(expectedClass, cut.Find(".app-alert").ClassList.ToString());
        }
    }

    [Fact]
    public void Title_And_Description_Render()
    {
        var cut = Render<AppAlert>(parameters => parameters
            .Add(p => p.Title, "Saved")
            .Add(p => p.Description, "Your changes are persisted."));

        Assert.Contains("Saved", cut.Find(".app-alert-title").TextContent);
        Assert.Contains("Your changes are persisted.", cut.Find(".app-alert-description").TextContent);
    }

    [Fact]
    public void Dismissible_Renders_Dismiss_Button()
    {
        var cut = Render<AppAlert>(parameters => parameters
            .Add(p => p.Dismissible, true)
            .Add(p => p.Title, "t"));

        Assert.NotNull(cut.Find(".app-alert-dismiss"));
    }

    [Fact]
    public void Without_Dismissible_Omits_Dismiss_Button()
    {
        var cut = Render<AppAlert>(parameters => parameters
            .Add(p => p.Title, "t"));

        Assert.Empty(cut.FindAll(".app-alert-dismiss"));
    }

    [Fact]
    public void Error_Variant_Has_Alert_Role()
    {
        var cut = Render<AppAlert>(parameters => parameters
            .Add(p => p.Variant, AppAlertVariant.Error)
            .Add(p => p.Title, "t"));

        Assert.Equal("alert", cut.Find(".app-alert").GetAttribute("role"));
    }

    [Fact]
    public void Information_Variant_Has_Status_Role()
    {
        var cut = Render<AppAlert>(parameters => parameters
            .Add(p => p.Variant, AppAlertVariant.Information)
            .Add(p => p.Title, "t"));

        Assert.Equal("status", cut.Find(".app-alert").GetAttribute("role"));
    }

    [Fact]
    public void Click_Dismiss_Fires_OnDismiss()
    {
        var dismissed = false;
        var cut = Render<AppAlert>(parameters => parameters
            .Add(p => p.Dismissible, true)
            .Add(p => p.Title, "t")
            .Add(p => p.OnDismiss, () => dismissed = true));

        cut.Find(".app-alert-dismiss").Click();
        Assert.True(dismissed);
    }
}
