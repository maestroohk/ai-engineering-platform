using AiEng.Platform.App.Components.Common;
using AiEng.Platform.App.Components.Diagnostics;
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace AiEng.Platform.ComponentTests.Components.Diagnostics;

public class AppKeyValueListTests : BunitContext
{
    [Fact]
    public void Renders_Populated_Slot_With_One_Row_Per_Item()
    {
        var items = new KeyValuePair<string, string>[]
        {
            new("Hostname", "dev-host-01"),
            new("OS", "Windows 11"),
            new("Architecture", "x64")
        };

        var cut = Render<AppKeyValueList>(parameters => parameters
            .Add(p => p.Items, items));

        var rows = cut.FindAll(".app-key-value-list-row");
        Assert.Equal(3, rows.Count);
        Assert.Contains("Hostname", cut.Markup);
        Assert.Contains("dev-host-01", cut.Markup);
        Assert.Contains("OS", cut.Markup);
        Assert.Contains("Windows 11", cut.Markup);
    }

    [Fact]
    public void Renders_Empty_Slot_When_No_Items()
    {
        var cut = Render<AppKeyValueList>(parameters => parameters
            .Add(p => p.Items, Array.Empty<KeyValuePair<string, string>>()));

        Assert.NotNull(cut.Find(".app-empty-state"));
        Assert.Contains("No metadata to display", cut.Markup);
    }

    [Fact]
    public void Renders_Loading_Slot_When_IsLoading_Is_True()
    {
        var cut = Render<AppKeyValueList>(parameters => parameters
            .Add(p => p.IsLoading, true)
            .Add(p => p.Items, Array.Empty<KeyValuePair<string, string>>()));

        Assert.NotNull(cut.Find(".app-loading"));
        Assert.Contains("Loading metadata", cut.Markup);
    }

    [Fact]
    public void Renders_Error_Slot_When_ErrorMessage_Is_Set()
    {
        var cut = Render<AppKeyValueList>(parameters => parameters
            .Add(p => p.ErrorMessage, "Failed to load metadata")
            .Add(p => p.ErrorCode, "META_LOAD_FAILED")
            .Add(p => p.CorrelationId, "corr-1")
            .Add(p => p.Items, Array.Empty<KeyValuePair<string, string>>()));

        Assert.NotNull(cut.Find(".app-error-state"));
        Assert.Contains("Cannot load metadata", cut.Markup);
        Assert.Contains("Failed to load metadata", cut.Markup);
        Assert.Contains("META_LOAD_FAILED", cut.Markup);
    }

    [Fact]
    public void Boolean_Format_Renders_Check_Icon_For_True_Value()
    {
        var items = new KeyValuePair<string, string>[]
        {
            new("IsAdmin", "true")
        };

        var cut = Render<AppKeyValueList>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.Format, AppKeyValueListFormat.Boolean));

        var booleanSpan = cut.Find(".app-key-value-list-boolean-true");
        Assert.NotNull(booleanSpan);
        Assert.Equal("✓", booleanSpan.TextContent.Trim());
        Assert.Equal("true", booleanSpan.GetAttribute("aria-label"));
    }

    [Fact]
    public void Boolean_Format_Renders_Cross_Icon_For_False_Value()
    {
        var items = new KeyValuePair<string, string>[]
        {
            new("IsAdmin", "false")
        };

        var cut = Render<AppKeyValueList>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.Format, AppKeyValueListFormat.Boolean));

        var booleanSpan = cut.Find(".app-key-value-list-boolean-false");
        Assert.NotNull(booleanSpan);
        Assert.Equal("✗", booleanSpan.TextContent.Trim());
        Assert.Equal("false", booleanSpan.GetAttribute("aria-label"));
    }

    [Fact]
    public void Boolean_Format_Renders_Literal_Text_For_Non_Boolean_Value()
    {
        var items = new KeyValuePair<string, string>[]
        {
            new("Status", "unknown")
        };

        var cut = Render<AppKeyValueList>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.Format, AppKeyValueListFormat.Boolean));

        Assert.Empty(cut.FindAll(".app-key-value-list-boolean"));
        Assert.Contains("unknown", cut.Markup);
    }

    [Fact]
    public void Code_Format_Renders_Value_In_Code_Element()
    {
        var items = new KeyValuePair<string, string>[]
        {
            new("CorrelationId", "abc-123")
        };

        var cut = Render<AppKeyValueList>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.Format, AppKeyValueListFormat.Code));

        var codeElement = cut.Find("code.app-key-value-list-code");
        Assert.NotNull(codeElement);
        Assert.Contains("abc-123", codeElement.TextContent);
    }

    [Fact]
    public void Plain_Format_Renders_Literal_Text()
    {
        var items = new KeyValuePair<string, string>[]
        {
            new("Region", "us-east-1")
        };

        var cut = Render<AppKeyValueList>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.Format, AppKeyValueListFormat.Plain));

        Assert.Empty(cut.FindAll("code.app-key-value-list-code"));
        Assert.Empty(cut.FindAll(".app-key-value-list-boolean"));
        Assert.Contains("us-east-1", cut.Markup);
    }

    [Fact]
    public void Custom_Populated_Slot_Overrides_Default_Rendering()
    {
        var items = new KeyValuePair<string, string>[]
        {
            new("k", "v")
        };

        RenderFragment customPopulated = builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "custom-populated-slot");
            builder.AddContent(2, "CUSTOM-POPULATED");
            builder.CloseElement();
        };

        var cut = Render<AppKeyValueList>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.Populated, customPopulated));

        Assert.NotNull(cut.Find(".custom-populated-slot"));
        Assert.DoesNotContain("app-key-value-list-row", cut.Markup);
    }

    [Fact]
    public void Custom_Loading_Slot_Overrides_Default_Loading()
    {
        RenderFragment customLoading = builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "custom-loading-slot");
            builder.AddContent(2, "CUSTOM-LOADING");
            builder.CloseElement();
        };

        var cut = Render<AppKeyValueList>(parameters => parameters
            .Add(p => p.IsLoading, true)
            .Add(p => p.Items, Array.Empty<KeyValuePair<string, string>>())
            .Add(p => p.Loading, customLoading));

        Assert.NotNull(cut.Find(".custom-loading-slot"));
        Assert.DoesNotContain("Loading metadata", cut.Markup);
    }

    [Fact]
    public void Custom_Empty_Slot_Overrides_Default_Empty()
    {
        RenderFragment customEmpty = builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "custom-empty-slot");
            builder.AddContent(2, "CUSTOM-EMPTY");
            builder.CloseElement();
        };

        var cut = Render<AppKeyValueList>(parameters => parameters
            .Add(p => p.Items, Array.Empty<KeyValuePair<string, string>>())
            .Add(p => p.Empty, customEmpty));

        Assert.NotNull(cut.Find(".custom-empty-slot"));
        Assert.DoesNotContain("No metadata to display", cut.Markup);
    }

    [Fact]
    public void Custom_Error_Slot_Overrides_Default_Error()
    {
        RenderFragment customError = builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "custom-error-slot");
            builder.AddContent(2, "CUSTOM-ERROR");
            builder.CloseElement();
        };

        var cut = Render<AppKeyValueList>(parameters => parameters
            .Add(p => p.ErrorMessage, "boom")
            .Add(p => p.Items, Array.Empty<KeyValuePair<string, string>>())
            .Add(p => p.Error, customError));

        Assert.NotNull(cut.Find(".custom-error-slot"));
        Assert.DoesNotContain("Cannot load metadata", cut.Markup);
    }

    [Fact]
    public void Populated_List_Has_AriaLive_Polite()
    {
        var items = new KeyValuePair<string, string>[]
        {
            new("k", "v")
        };

        var cut = Render<AppKeyValueList>(parameters => parameters
            .Add(p => p.Items, items));

        var container = cut.Find("[data-state=\"populated\"]");
        Assert.Equal("polite", container.GetAttribute("aria-live"));
    }
}
